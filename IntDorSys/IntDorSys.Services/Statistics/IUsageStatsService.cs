using IntDorSys.Core.Models;

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

        /// <summary>
        ///     Returns usage count for a single user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Number of laundry uses in the last 6 months</returns>
        Task<int> GetUsageCountAsync(long userId, CancellationToken ct);

        /// <summary>
        ///     Returns the last N wash records for a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="count">Number of records to return</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of recent wash records</returns>
        Task<List<WashRecordModel>> GetRecentWashesAsync(long userId, int count, CancellationToken ct);
    }
}