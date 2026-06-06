using IntDorSys.Core.Entities.Users;

namespace IntDorSys.TelegramBot.Service.AdminServices
{
    /// <summary>
    ///     Provides admin notification and user update functionality in the Telegram bot
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        ///     Sends a notification message to all users
        /// </summary>
        /// <param name="userInfo">Admin user who triggers the notification</param>
        /// <param name="ct">Cancellation token</param>
        Task SendUsersNotificationAsync(UserInfo userInfo, CancellationToken ct);

        /// <summary>
        ///     Updates the notification flag for a user
        /// </summary>
        /// <param name="userId">User ID to update</param>
        /// <param name="ct">Cancellation token</param>
        Task UpdateNotificateUserAsync(long userId, CancellationToken ct);
    }
}
