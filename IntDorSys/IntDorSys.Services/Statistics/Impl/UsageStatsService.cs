using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Services.Statistics;
using Microsoft.EntityFrameworkCore;

namespace IntDorSys.Services.Statistics.Impl
{
    internal sealed class UsageStatsService : IUsageStatsService
    {
        private readonly AppDataContext _db;

        public UsageStatsService(AppDataContext db)
        {
            _db = db;
        }

        public async Task<Dictionary<long, int>> GetUsageCountsAsync(CancellationToken ct)
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            return await _db.Set<UseLaundress>()
                .Where(x => x.SelectUserId != null && x.TimeWash >= sixMonthsAgo)
                .GroupBy(x => x.SelectUserId!.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);
        }
    }
}