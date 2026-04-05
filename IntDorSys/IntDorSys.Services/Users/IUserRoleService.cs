using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Users
{
    public interface IUserRoleService
    {
        /// <summary>
        ///     Gets user role by user identifier
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<List<string>>> GetByIdAsync(long id, CancellationToken ct);
    }
}