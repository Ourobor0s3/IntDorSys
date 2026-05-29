namespace IntDorSys.Services.Statistics
{
    public interface IUsageStatsService
    {
        Task<Dictionary<long, int>> GetUsageCountsAsync(CancellationToken ct);
    }
}