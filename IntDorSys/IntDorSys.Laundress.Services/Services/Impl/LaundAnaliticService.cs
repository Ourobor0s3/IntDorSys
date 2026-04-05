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

            var searchTime = await _db.Set<UseLaundress>()
                .WhereIf(
                    filter.StartDate != null,
                    x => x.TimeWash >= DateTime.Parse(filter.StartDate!))
                .WhereIf(
                    filter.EndDate != null,
                    x => x.TimeWash <= DateTime.Parse(filter.EndDate!).AddDays(1))
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