using System.Globalization;
using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Extensions;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Services.Impl
{
    internal sealed class LaundressService : ILaundressService
    {
        private readonly AppDataContext _db;

        public LaundressService(AppDataContext db)
        {
            _db = db;
        }

        public async Task<DataResult<List<UseLaundress>>> GetTimeByFilterAsync(
            LaundressFilterModel? filter = null,
            CancellationToken ct = default)
        {
            var result = new DataResult<List<UseLaundress>>();

            var query = _db.Set<UseLaundress>()
                .Include(x => x.SelectUser)
                .AsQueryable();

            // Учитываем UserId
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

            // Обработка StartDate
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

            // Обработка EndDate
            if (!string.IsNullOrWhiteSpace(filter?.EndDate) &&
                DateTime.TryParse(filter.EndDate,
                    null,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                    out var endDate))
            {
                var endOfDay = endDate.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.TimeWash <= endOfDay);
            }

            // Обработка SearchDate
            if (filter?.SearchDate is { } searchDate && searchDate != DateTime.MinValue)
            {
                var startOfDay = searchDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.TimeWash >= startOfDay && x.TimeWash <= endOfDay);
            }

            // Только свободные записи (и не в прошлом)
            if (filter?.IsUnoccupiedRecords == true)
            {
                query = query
                    .Where(x => x.SelectUser == null)
                    .Where(x => x.TimeWash >= DateTime.Now);
            }

            // Только занятые записи
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

            var wash = await _db.Set<UseLaundress>()
                .FirstOrDefaultAsync(x => x.TimeWash == newWash.TimeWash, ct);

            if (wash != null)
            {
                return res.WithError($"Time = {newWash.TimeWash.ToString(CultureInfo.CurrentCulture)} already exists");
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
                return res.WithError($"Not found time {timeWash.ToString(CultureInfo.CurrentCulture)}");
            }

            _db.DeleteEntity(wash);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<bool>> UseTimeAsync(long userId, DateTime timeWash, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var useWash = await _db.Set<UseLaundress>()
                .Where(x => x.TimeWash > DateTime.Now)
                .Where(x => x.SelectUserId == userId)
                .CountAsync(ct);
            if (useWash >= 2)
            {
                return res.WithError($"{userId} use two time wash already exists");
            }

            var wash = await _db.Set<UseLaundress>()
                .Where(x => x.SelectUserId == null)
                .FirstOrDefaultAsync(x => x.TimeWash == timeWash, ct);
            if (wash == null)
            {
                return res.WithError($"Not found time {timeWash.ToString()}");
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
                return res.WithError($"Not found time {timeWash.ToString(CultureInfo.CurrentCulture)}");
            }

            wash.SelectUser = null;
            _db.AddOrUpdateEntity(wash);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }
    }
}