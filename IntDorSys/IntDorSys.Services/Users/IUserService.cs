using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        ///     Gets the user by email
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Existing user data</returns>
        Task<DataResult<UserInfo>> GetByEmailAsync(string email, CancellationToken ct);

        /// <summary>
        ///     Gets the user by tg id
        /// </summary>
        /// <param name="id">Telegram identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> GetByTgIdAsync(long id, CancellationToken ct);

        /// <summary>
        ///     Gets the user by user id
        /// </summary>
        /// <param name="id">Telegram identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> GetByUserIdAsync(long id, CancellationToken ct);

        /// <summary>
        ///     Update user info
        /// </summary>
        /// <param name="newInfo">New user info</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> UpdateUserInfo(UserInfo newInfo, CancellationToken ct);

        /// <summary>
        ///     Create or update user info for system
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> CreateOrUpdateTgInfoAsync(User user, CancellationToken ct);

        /// <summary>
        ///     Creates new user record
        /// </summary>
        /// <param name="user">User data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Created user</returns>
        Task<DataResult<UserInfo>> CreateAsync(UserInfo user, CancellationToken ct);

        /// <summary>
        ///     Gets the user by id
        /// </summary>
        /// <param name="id">Identify of user</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Existing user data</returns>
        Task<DataResult<UserInfo>> GetAsync(long id, CancellationToken ct);

        /// <summary>
        ///     Get list users info
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<List<UserInfo>>> GetListUsersAsync(CancellationToken ct);

        /// <summary>
        ///     Change user status for system
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="newStatus">New user status</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<bool>> ChangeUserStatus(long userId, UserStatus newStatus, CancellationToken ct);
    }
}