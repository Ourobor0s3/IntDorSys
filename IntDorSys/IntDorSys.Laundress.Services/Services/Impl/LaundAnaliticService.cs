using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Base.Models;
using Ouro.CommonUtils.Extensions;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Services.Impl
{
    internal sealed class LaundAnaliticService : ILaundAnaliticService
    {
        private readonly AppDataContext _db;

        public LaundAnaliticService(AppDataContext db)
        {
            _db = db;
        }

        public async Task<DataResult<List<ChartDataModel<string, int>>>> GetTimeAnaliticAsync(
            LaundressFilterModel filter,
            CancellationToken ct)
        {
            var res = new DataResult<List<ChartDataModel<string, int>>>();

            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrWhiteSpace(filter.StartDate) &&
                DateTime.TryParse(filter.StartDate, out var parsedStart))
            {
                startDate = parsedStart;
            }

            if (!string.IsNullOrWhiteSpace(filter.EndDate) &&
                DateTime.TryParse(filter.EndDate, out var parsedEnd))
            {
                endDate = parsedEnd;
            }

            var query = _db.Set<UseLaundress>().AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.TimeWash >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.TimeWash <= endDate.Value.AddDays(1));
            }

            var searchTime = await query
                .GroupBy(x => x.TimeWash.TimeOfDay)
                .OrderBy(x => x.Key)
                .Select(g => new ChartDataModel<string, int>
                {
                    Name = g.Key.ToString(@"hh\:mm"),
                    Value1 = g.Count(),
                    Value2 = g.Count(x => x.SelectUserId != null),
                })
                .ToListAsync(ct);

            return res.WithData(searchTime);
        }
    }
}