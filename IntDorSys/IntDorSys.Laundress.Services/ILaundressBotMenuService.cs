using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Services
{
    /// <summary>Bot menu display and slot listing operations.</summary>
    public interface ILaundressBotMenuService
    {
        /// <summary>Sends the main laundry menu inline keyboard.</summary>
        /// <param name="user">User to send the menu to</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendMenuAsync(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends a list of all time slots (occupied + free).</summary>
        /// <param name="chatId">Telegram chat ID</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendAllTimeAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends a list of dates that have free slots.</summary>
        /// <param name="chatId">Telegram chat ID</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendFreeDateAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends free time slots for a specific date.</summary>
        /// <param name="chatId">Telegram chat ID</param>
        /// <param name="messageId">Message ID to edit</param>
        /// <param name="date">Date to show free slots for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendFreeTimeAsync(long chatId, int messageId, DateTime date, CancellationToken ct);

        /// <summary>Sends the current user's booked slots.</summary>
        /// <param name="user">User whose bookings to show</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendUseTimeByUserAsync(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends dates with occupied records for admin to choose from.</summary>
        /// <param name="chatId">Telegram chat ID</param>
        /// <param name="messageId">Optional message ID to edit instead of sending new</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendDatesForDeleteAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends occupied times for a specific date for admin deletion.</summary>
        /// <param name="chatId">Telegram chat ID</param>
        /// <param name="messageId">Message ID to edit</param>
        /// <param name="date">Date to show occupied slots for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task SendTimesForDeleteAsync(long chatId, int messageId, DateTime date, CancellationToken ct);
    }
}