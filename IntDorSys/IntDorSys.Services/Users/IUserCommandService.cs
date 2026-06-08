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
        Task<DataResult<UserInfo>> CreateAsync(UserInfo user, CancellationToken ct);

        /// <summary>Updates user profile fields (FullName, NumGroup, NumRoom).</summary>
        Task<DataResult<UserInfo>> UpdateUserInfo(UserInfo newInfo, CancellationToken ct);

        /// <summary>Creates or updates a user from Telegram user data.</summary>
        Task<DataResult<UserInfo>> CreateOrUpdateTgInfoAsync(User user, CancellationToken ct);

        /// <summary>Changes the status of a user (e.g. Blocked / Registered).</summary>
        Task<DataResult<bool>> ChangeUserStatus(long userId, UserStatus newStatus, CancellationToken ct);

        /// <summary>Updates the password hash for a user.</summary>
        Task<DataResult<UserInfo>> UpdatePasswordAsync(long userId, string passwordHash, CancellationToken ct);
    }
}