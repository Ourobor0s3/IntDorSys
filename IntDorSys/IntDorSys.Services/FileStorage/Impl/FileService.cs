using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Results;
using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.Services.FileStorage.Impl
{
    internal sealed class FileService : IFileService
    {
        private readonly AppDataContext _db;
        private readonly ILogger<FileService> _logger;

        private readonly string _storagePath;

        public FileService(
            ILogger<FileService> logger,
            AppDataContext db)
        {
            _logger = logger;
            _db = db;

            const string relativePath = "FileFolderStorage";

            _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public async Task<DataResult<FileInfo>> GetAsync(Guid id, CancellationToken ct)
        {
            var result = new DataResult<FileInfo>();
            var file = await _db.Set<FileInfo>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Guid == id, ct);

            if (file == null)
            {
                _logger.LogError(
                    "FileServices.GetAsync error => File not found. " +
                    "[FileGuid : '{Guid}']",
                    id);

                return result.WithError("Not found");
            }

            var fileName = file.Guid + file.Extension;
            var filePath = $"{_storagePath}/{fileName}";

            _logger.LogInformation(
                "FileServices.GetAsync => [FileGuid : '{Guid}', filePath: '{path}']",
                id,
                filePath);

            file.Content = await File.ReadAllBytesAsync(filePath, ct);

            return result.WithData(file);
        }

        public async Task<DataResult<Guid>> SaveAsync(
            string name,
            MemoryStream stream,
            string? groupId = null,
            CancellationToken ct = default)
        {
            var result = new DataResult<Guid>();

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }

            if (stream.Length == 0)
            {
                _logger.LogError("FileService.SaveAsync error => Stream is empty");
                return result.WithError("Stream is empty");
            }

            stream.Position = 0;
            var content = stream.ToArray();
            var guid = Guid.NewGuid();
            var extension = Path.GetExtension(name);

            var fileInfo = new FileInfo
            {
                Name = guid + extension,
                Guid = guid,
                OriginalName = name,
                Size = content.Length,
                Extension = extension,
                GroupId = groupId,
            };

            _db.Add(fileInfo);
            await _db.SaveChangesAsync(ct);
            await File.WriteAllBytesAsync($"{_storagePath}/{fileInfo.Name}", content, ct);
            return result.WithData(fileInfo.Guid);
        }
    }
}