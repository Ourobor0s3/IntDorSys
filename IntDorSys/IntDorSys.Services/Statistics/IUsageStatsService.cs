namespace IntDorSys.Services.Statistics
{
    /// <summary>
    ///     Provides usage statistics for the laundry service
    /// </summary>
    public interface IUsageStatsService
    {
        /// <summary>
        ///     Returns a map of user IDs to their usage counts
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Dictionary mapping user ID to number of laundry uses</returns>
        Task<Dictionary<long, int>> GetUsageCountsAsync(CancellationToken ct);
    }
}