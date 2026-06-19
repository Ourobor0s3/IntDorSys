using System.Globalization;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressQueryService : IUseLaundressQueryService
    {
        private readonly AppDataContext _db;

        public LaundressQueryService(AppDataContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
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
                    CultureInfo.InvariantCulture,
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
    }
}
