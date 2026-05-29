using IntDorSys.Core.Entities.Users;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Users
{
    /// <summary>Read-only queries for user data.</summary>
    public interface IUserQueryService
    {
        /// <summary>Gets a user by their email address (case-insensitive).</summary>
        Task<DataResult<UserInfo>> GetByEmailAsync(string email, CancellationToken ct);

        /// <summary>Gets a user by their Telegram ID.</summary>
        Task<DataResult<UserInfo>> GetByTgIdAsync(long id, CancellationToken ct);

        /// <summary>Gets a user by their Telegram user ID.</summary>
        Task<DataResult<UserInfo>> GetByUserIdAsync(long id, CancellationToken ct);

        /// <summary>Gets a user by their internal user ID.</summary>
        Task<DataResult<UserInfo>> GetAsync(long id, CancellationToken ct);

        /// <summary>Returns all users with a non-empty full name, ordered by name.</summary>
        Task<DataResult<List<UserInfo>>> GetListUsersAsync(CancellationToken ct);
    }
}
