using System.Globalization;
using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Services.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Extensions;
using Ouro.CommonUtils.Results;


namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressService : ILaundressService, IUseLaundressQueryService
    {
        private readonly AppDataContext _db;
        private readonly IAppSettingService _settings;
        private readonly ILogger<LaundressService> _logger;
        private const int DefaultWashDurationHours = 2;
        private const int MaxConcurrentBookings = 2;

        public LaundressService(AppDataContext db, IAppSettingService settings, ILogger<LaundressService> logger)
        {
            _db = db;
            _settings = settings;
            _logger = logger;
        }

        private async Task<int> GetWashDurationHoursAsync(CancellationToken ct)
        {
            var val = await _settings.GetValueAsync("WashDurationHours", ct);
            return int.TryParse(val, out var hours) ? hours : DefaultWashDurationHours;
        }

        private async Task<bool> HasOverlappingSlotsAsync(DateTime timeWash, int washDurationHours, CancellationToken ct)
        {
            var endTime = timeWash.AddHours(washDurationHours);
            return await _db.Set<UseLaundress>()
                .Where(x => x.TimeWash < endTime && x.TimeWash.AddHours(washDurationHours) > timeWash)
                .AnyAsync(ct);
        }

        public async Task<DataResult<List<UseLaundress>>> GetTimeByFilterAsync(
            LaundressFilterModel? filter = null,
            CancellationToken ct = default)
        {
            var result = new DataResult<List<UseLaundress>>();

            var query = _db.Set<UseLaundress>()
                .Include(x => x.SelectUser)
                .AsQueryable();

            // Filter by UserId
            if (filter?.UserId > 0)
            {
                query = query.Where(x =>
                    x.TimeWash >= DateTime.Now &&
                    x.SelectUserId == filter.UserId);
            }
            else if (filter?.UserId == null)
            {
                query = query.Where(x => x.TimeWash >= DateTime.Now.Date);
            }

            // Process StartDate filter
            if (!string.IsNullOrWhiteSpace(filter?.StartDate) &&
                DateTime.TryParse(filter.StartDate,
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var startDate))
            {
                query = query.Where(x => x.TimeWash >= startDate.Date);
            }
            else
            {
                query = query.Where(x => x.TimeWash >= DateTime.Now.Date);
            }

            // Process EndDate filter
            if (!string.IsNullOrWhiteSpace(filter?.EndDate) &&
                DateTime.TryParse(filter.EndDate,
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var endDate))
            {
                var endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.TimeWash <= endOfDay);
            }

            // Process SearchDate filter
            if (filter?.SearchDate is { } searchDate && searchDate != DateTime.MinValue)
            {
                var startOfDay = searchDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.TimeWash >= startOfDay && x.TimeWash <= endOfDay);
            }

            // Only free records (not in the past)
            if (filter?.IsUnoccupiedRecords == true)
            {
                query = query
                    .Where(x => x.SelectUser == null)
                    .Where(x => x.TimeWash >= DateTime.Now);
            }

            // Only occupied records
            if (filter?.IsOccupiedRecords == true)
            {
                query = query.Where(x => x.SelectUser != null);
            }

            var records = await query
                .OrderBy(x => x.TimeWash)
                .ToListAsync(ct);

            return result.WithData(records);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> CreateTimeAsync(UseLaundress newWash, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var washDuration = await GetWashDurationHoursAsync(ct);

            if (await HasOverlappingSlotsAsync(newWash.TimeWash, washDuration, ct))
            {
                return res.WithError($"Time {newWash.TimeWash:dd.MM.yyyy HH:mm} overlaps with an existing slot");
            }

            _db.AddOrUpdateEntity(newWash);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> RemoveTimeAsync(DateTime timeWash, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var wash = await _db.Set<UseLaundress>()
                .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);

            if (wash == null)
            {
                return res.WithError($"Not found time {timeWash:dd.MM.yyyy HH:mm}");
            }

            _db.DeleteEntity(wash);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> UseTimeAsync(long userId, DateTime timeWash, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var washDuration = await GetWashDurationHoursAsync(ct);

            var useWash = await _db.Set<UseLaundress>()
                .Where(x => x.TimeWash > DateTime.Now)
                .Where(x => x.SelectUserId == userId)
                .CountAsync(ct);
            if (useWash >= MaxConcurrentBookings)
            {
                return res.WithError($"{userId} use two time wash already exists");
            }

            var wash = await _db.Set<UseLaundress>()
                .Where(x => x.SelectUserId == null)
                .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);
            if (wash == null)
            {
                return res.WithError($"Not found time {timeWash:dd.MM.yyyy HH:mm}");
            }

            var user = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (user == null)
            {
                return res.WithError($"User id = {userId} not found");
            }

            wash.SelectUser = user;
            _db.AddOrUpdateEntity(wash);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> RemoveUseTimeAsync(
            long userId,
            DateTime timeWash,
            bool isAdmin = false,
            CancellationToken ct = default)
        {
            var res = new DataResult<bool>();

            var wash = await _db.Set<UseLaundress>()
                .WhereIf(!isAdmin ,x => x.SelectUserId == userId)
                .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);

            if (wash == null)
            {
                return res.WithError($"Not found time {timeWash:dd.MM.yyyy HH:mm}");
            }

            if (wash.SelectUserId == null)
            {
                return res.WithError("Slot is already free");
            }

            wash.SelectUser = null;
            wash.SelectUserId = null;
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<int>> CreateTimeRangeAsync(DateTime date, int startHour, int endHour, long createdUserId, CancellationToken ct)
        {
            var res = new DataResult<int>();
            var dateBase = date.Date;
            var created = 0;
            var washDuration = await GetWashDurationHoursAsync(ct);

            for (var hour = startHour; hour <= endHour; hour += washDuration)
            {
                var timeWash = dateBase.AddHours(hour);

                if (await HasOverlappingSlotsAsync(timeWash, washDuration, ct))
                {
                    continue;
                }

                var wash = new UseLaundress
                {
                    TimeWash = timeWash,
                    CreatedUserId = createdUserId,
                };

                _db.Set<UseLaundress>().Add(wash);
                created++;
            }

            if (created > 0)
            {
                try
                {
                    await _db.SaveChangesAsync(ct);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogWarning(ex, "Concurrent slot creation conflict at CreateTimeRangeAsync (date={Date}, start={Start}, end={End})", date, startHour, endHour);
                }
            }

            return res.WithData(created);
        }
    }
}