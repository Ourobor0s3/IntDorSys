using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Services.AppSettings;
using IntDorSys.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Extensions;
using Ouro.CommonUtils.Results;


namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressService : ILaundressService
    {
        private readonly AppDataContext _db;
        private readonly IAppSettingService _settings;
        private readonly ILogger<LaundressService> _logger;
        private readonly IAuditService _audit;


        public LaundressService(AppDataContext db, IAppSettingService settings, ILogger<LaundressService> logger, IAuditService audit)
        {
            _db = db;
            _settings = settings;
            _logger = logger;
            _audit = audit;
        }

        private async Task<int> GetWashDurationHoursAsync(CancellationToken ct)
        {
            var val = await _settings.GetValueAsync("WashDurationHours", ct);
            return int.TryParse(val, out var hours) ? hours : DefaultSettings.WashDurationHours;
        }

        private async Task<int> GetMaxConcurrentBookingsAsync(CancellationToken ct)
        {
            var val = await _settings.GetValueAsync("MaxConcurrentBookings", ct);
            return int.TryParse(val, out var max) ? max : DefaultSettings.MaxConcurrentBookings;
        }

        private async Task<bool> HasOverlappingSlotsAsync(DateTime timeWash, int washDurationHours, CancellationToken ct)
        {
            var endTime = timeWash.AddHours(washDurationHours);
            return await _db.Set<UseLaundress>()
                .Where(x => x.TimeWash < endTime && x.TimeWash.AddHours(washDurationHours) > timeWash)
                .AnyAsync(ct);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> CreateTimeAsync(UseLaundress newWash, long actingUserId, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var washDuration = await GetWashDurationHoursAsync(ct);

            var existingWash = await _db.Set<UseLaundress>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.TimeWash == newWash.TimeWash, ct);

            if (existingWash != null)
            {
                if (existingWash.Deleted)
                {
                    existingWash.Deleted = false;
                    existingWash.SelectUser = null;
                    existingWash.SelectUserId = null;
                    await _db.SaveChangesAsync(ct);
                    if (actingUserId > 0)
                        await _audit.RecordAsync(actingUserId, "CreateSlot", "UseLaundress",
                            newWash.TimeWash.ToString("O"), $"Created by user {newWash.CreatedUserId}", ct);
                    return res.WithData(true);
                }

                return res.WithError($"Time {newWash.TimeWash:dd.MM.yyyy HH:mm} overlaps with an existing slot");
            }

            if (await HasOverlappingSlotsAsync(newWash.TimeWash, washDuration, ct))
            {
                return res.WithError($"Time {newWash.TimeWash:dd.MM.yyyy HH:mm} overlaps with an existing slot");
            }

            _db.AddOrUpdateEntity(newWash);
            await _db.SaveChangesAsync(ct);
            if (actingUserId > 0)
                await _audit.RecordAsync(actingUserId, "CreateSlot", "UseLaundress",
                    newWash.TimeWash.ToString("O"), $"Created by user {newWash.CreatedUserId}", ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> RemoveTimeAsync(DateTime timeWash, long actingUserId, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var wash = await _db.Set<UseLaundress>()
                .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);

            if (wash == null)
            {
                return res.WithError($"Not found time {timeWash:dd.MM.yyyy HH:mm}");
            }

            wash.Deleted = true;
            await _db.SaveChangesAsync(ct);
            await _audit.RecordAsync(actingUserId, "DeleteSlot", "UseLaundress",
                timeWash.ToString("O"), null, ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> UseTimeAsync(long userId, DateTime timeWash, long actingUserId, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var washDuration = await GetWashDurationHoursAsync(ct);
            var maxConcurrent = await GetMaxConcurrentBookingsAsync(ct);

            var useWash = await _db.Set<UseLaundress>()
                .Where(x => x.TimeWash > DateTime.Now)
                .Where(x => x.SelectUserId == userId)
                .CountAsync(ct);
            if (useWash >= maxConcurrent)
            {
                return res.WithError($"User already has {maxConcurrent} active booking(s)");
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

            if (user.Status == UserStatus.Blocked)
            {
                return res.WithError($"User id = {userId} is blocked");
            }

            if (!user.IsConfirm)
            {
                return res.WithError($"User id = {userId} is not confirmed");
            }

            wash.SelectUser = user;
            await _db.SaveChangesAsync(ct);
            await _audit.RecordAsync(actingUserId, "BookSlot", "UseLaundress",
                timeWash.ToString("O"), $"Booked user {userId}", ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> RemoveUseTimeAsync(
            long userId,
            DateTime timeWash,
            bool isAdmin,
            long actingUserId,
            CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var wash = await _db.Set<UseLaundress>()
                .WhereIf(!isAdmin, x => x.SelectUserId == userId)
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
            await _audit.RecordAsync(actingUserId, "UnbookSlot", "UseLaundress",
                timeWash.ToString("O"), $"Unbooked user {userId}", ct);
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

                var existingWash = await _db.Set<UseLaundress>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);

                if (existingWash != null)
                {
                    if (existingWash.Deleted)
                    {
                        existingWash.Deleted = false;
                        existingWash.SelectUser = null;
                        existingWash.SelectUserId = null;
                        created++;
                        continue;
                    }

                    continue;
                }

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
                    return res.WithError($"Concurrent slot creation conflict: {ex.Message}");
                }
            }

            await _audit.RecordAsync(createdUserId, "CreateSlotRange", "UseLaundress",
                $"{date:O}", $"Slots {startHour}:00-{endHour}:00, created {created}", ct);
            return res.WithData(created);
        }
    }
}