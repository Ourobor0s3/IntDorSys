using System.Globalization;
using IntDorSys.Core.Entities;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Settings;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Services.FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Extensions;
using Ouro.CommonUtils.Results;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundReportService : ILaundReportService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppDataContext _db;
        private readonly IFileService _fileService;
        private readonly LinkSettings _link;
        private readonly ILogger<LaundReportService> _logger;

        public LaundReportService(
            ILogger<LaundReportService> logger,
            IOptionsMonitor<LinkSettings> link,
            ITelegramBotClient botClient,
            AppDataContext db,
            IFileService fileService)
        {
            _logger = logger;
            _botClient = botClient;
            _db = db;
            _fileService = fileService;
            _link = link.CurrentValue;
        }

        public async Task<DataResult<List<StoredFileInfo>>> GetFilterAsync(BaseFilterModel filter, CancellationToken ct)
        {
            var result = new DataResult<List<StoredFileInfo>>();
            var files = await _db.Set<StoredFileInfo>()
                .AsNoTracking()
                .WhereIf(filter?.StartDate != null,
                    x => x.CreatedAt >= DateTime.Parse(filter!.StartDate!, CultureInfo.InvariantCulture))
                .WhereIf(filter?.EndDate != null,
                    x => x.CreatedAt < DateTime.Parse(filter!.EndDate!, CultureInfo.InvariantCulture).AddDays(1))
                .ToListAsync(ct);

            if (files.Count == 0)
            {
                _logger.LogError(
                    "FileServices.GetAsync error => Files not found. ");

                return result.WithError("Not found");
            }

            foreach (var file in files)
            {
                file.FilePath = $"{_link.BackUrl}/file/{file.Guid}";
            }

            return result.WithData(files);
        }

        public async Task<DataResult<List<ReportModel>>> GetReportAsync(BaseFilterModel filter, CancellationToken ct)
        {
            var result = new DataResult<List<ReportModel>>();

            DateTime? startDate = null;
            DateTime? endDate = null;

            // Parse input dates in UTC
            if (!string.IsNullOrWhiteSpace(filter?.StartDate) &&
                DateTime.TryParse(filter.StartDate,
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var parsedStart))
            {
                startDate = parsedStart.Date.AddDays(1);
            }

            if (!string.IsNullOrWhiteSpace(filter?.EndDate) &&
                DateTime.TryParse(filter.EndDate,
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var parsedEnd))
            {
                endDate = parsedEnd.Date.AddDays(2).AddTicks(-1);
            }

            // Fetch files
            var filesQuery = _db.Set<StoredFileInfo>()
                .AsNoTracking()
                .Where(x => !string.IsNullOrEmpty(x.GroupId));

            if (startDate.HasValue)
            {
                filesQuery = filesQuery.Where(x => x.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                filesQuery = filesQuery.Where(x => x.CreatedAt <= endDate.Value);
            }

            var files = await filesQuery.ToListAsync(ct);

            foreach (var file in files)
            {
                file.FilePath = $"{_link.BackUrl}/file/{file.Guid}";
            }

            var filesByGroup = files.Count == 0
                ? new Dictionary<string, List<StoredFileInfo>>()
                : files
                    .GroupBy(f => f.GroupId)
                    .ToDictionary(g => g.Key!, g => g.ToList());

            // Fetch reports
            var reportsQuery = _db.Set<Report>()
                .Include(x => x.User)
                .AsNoTracking();

            if (startDate.HasValue)
            {
                reportsQuery = reportsQuery.Where(x => x.CreatedAt >= startDate);
            }

            if (endDate.HasValue)
            {
                reportsQuery = reportsQuery.Where(x => x.CreatedAt <= endDate);
            }

            var reports = await reportsQuery
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ReportModel
                {
                    UserId = x.UserId,
                    Username = x.User!.FullName!,
                    GroupId = x.GroupId,
                    Description = x.Description,
                    Files = x.GroupId == null ? null : filesByGroup.GetValueOrDefault(x.GroupId),
                    CreatedAt = x.CreatedAt.DateTime,
                })
                .ToListAsync(ct);

            return result.WithData(reports);
        }

        public async Task SaveReportAsync(Message message, CancellationToken ct)
        {
            if (message.ReplyToMessage == null)
            {
                return;
            }

            var user = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(x => x.TelegramId == message.Chat.Id, ct);

            if (user == null)
            {
                _logger.LogError("User not found");
                return;
            }

            var repGroupId = message.ReplyToMessage.MediaGroupId
                     ?? message.ReplyToMessage.Photo?[^1].FileId;
            var report = new Report
            {
                UserId = user.Id,
                GroupId = repGroupId,
                Description = message.ReplyToMessage.Caption ?? message.ReplyToMessage.Text,
            };

            _db.AddOrUpdateEntity(report);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Create new report, user: {userId}", user.Id);
        }

        public async Task SavePhotoAsync(Message message, CancellationToken ct)
        {
            if (message.Type != MessageType.Photo || message.Photo?.Length == 0)
            {
                return;
            }

            var photoSize = message.Photo![^1];
            var fileId = photoSize.FileId;
            var file = await _botClient.GetFile(fileId, ct);
            var filePath = file.FilePath;

            using var memoryStream = new MemoryStream();
            await _botClient.DownloadFile(file, memoryStream, ct);

            var groupId = message.MediaGroupId ?? photoSize.FileId;
            await _fileService.SaveAsync(filePath!, memoryStream, groupId, ct);
        }
    }
}