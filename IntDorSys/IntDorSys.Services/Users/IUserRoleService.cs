using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Users
{
    /// <summary>
    ///     Provides user role queries
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        ///     Gets user role by user identifier
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of role names for the user</returns>
        Task<DataResult<List<string>>> GetByIdAsync(long id, CancellationToken ct);

        /// <summary>
        ///     Gets roles for multiple users in one query
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Map of user ID to list of role keys</returns>
        Task<DataResult<Dictionary<long, List<string>>>> GetByUserIdsAsync(List<long> userIds, CancellationToken ct);
    }
}