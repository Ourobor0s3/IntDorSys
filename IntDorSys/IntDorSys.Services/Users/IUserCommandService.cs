using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Users
{
    /// <summary>Write operations for user data.</summary>
    public interface IUserCommandService
    {
        /// <summary>Creates a new user record.</summary>
        /// <param name="user">User data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Created user info</returns>
        Task<DataResult<UserInfo>> CreateAsync(UserInfo user, CancellationToken ct);

        /// <summary>Updates user profile fields (FullName, NumGroup, NumRoom).</summary>
        /// <param name="newInfo">User data with updated fields</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Updated user info</returns>
        Task<DataResult<UserInfo>> UpdateUserInfo(UserInfo newInfo, CancellationToken ct);

        /// <summary>Creates or updates a user from Telegram user data.</summary>
        /// <param name="user">Telegram user data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>User info</returns>
        Task<DataResult<UserInfo>> CreateOrUpdateTgInfoAsync(User user, CancellationToken ct);

        /// <summary>Changes the status of a user (e.g. Blocked / Registered).</summary>
        /// <param name="userId">User ID</param>
        /// <param name="newStatus">New status</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if status was changed</returns>
        Task<DataResult<bool>> ChangeUserStatus(long userId, UserStatus newStatus, CancellationToken ct);

        /// <summary>Updates the password hash for a user.</summary>
        /// <param name="userId">User ID</param>
        /// <param name="passwordHash">New password hash</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Updated user info</returns>
        Task<DataResult<UserInfo>> UpdatePasswordAsync(long userId, string passwordHash, CancellationToken ct);

        /// <summary>Confirms a user and assigns a role.</summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleKey">Role key to assign</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Updated user info</returns>
        Task<DataResult<UserInfo>> ConfirmUserWithRoleAsync(long userId, string roleKey, CancellationToken ct);
    }
}