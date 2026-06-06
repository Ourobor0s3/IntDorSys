using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;

namespace IntDorSys.Laundress.Services
{
    /// <summary>
    ///     Provides Telegram bot user listing and status management
    /// </summary>
    public interface IUserBotService
    {
        /// <summary>
        ///     Sends a list of users to the Telegram chat
        /// </summary>
        /// <param name="userId">Chat user ID to send to</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="isBlockedUsers">If true, show blocked users; otherwise show active users</param>
        /// <param name="ct">Cancellation token</param>
        Task SendUsersAsync(
            long userId,
            int messageId = 0,
            bool isBlockedUsers = false,
            CancellationToken ct = default);

        /// <summary>
        ///     Changes a user's status (block/unblock)
        /// </summary>
        /// <param name="userId">Admin user who performs the action</param>
        /// <param name="forUserId">Target user whose status changes</param>
        /// <param name="newStatus">New status value</param>
        /// <param name="ct">Cancellation token</param>
        Task ChangeStatusUserAsync(
            long userId,
            long forUserId,
            UserStatus newStatus,
            CancellationToken ct);
    }
}
