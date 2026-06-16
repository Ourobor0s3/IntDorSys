using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
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

        /// <inheritdoc />
        public async Task<Dictionary<long, int>> GetUsageCountsAsync(CancellationToken ct)
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            return await _db.Set<UseLaundress>()
                .Where(x => x.SelectUserId != null && x.TimeWash >= sixMonthsAgo)
                .GroupBy(x => x.SelectUserId!.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);
        }

        /// <inheritdoc />
        public async Task<int> GetUsageCountAsync(long userId, CancellationToken ct)
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            return await _db.Set<UseLaundress>()
                .CountAsync(x => x.SelectUserId == userId && x.TimeWash >= sixMonthsAgo, ct);
        }
    }
}