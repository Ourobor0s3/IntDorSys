using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Services
{
    /// <summary>Bot menu display and slot listing operations.</summary>
    public interface ILaundressBotMenuService
    {
        /// <summary>Sends the main laundry menu inline keyboard.</summary>
        Task SendMenu(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends a list of all time slots (occupied + free).</summary>
        Task SendAllTimeAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends a list of dates that have free slots.</summary>
        Task SendFreeDateAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends free time slots for a specific date.</summary>
        Task SendFreeTimeAsync(long chatId, int messageId, DateTime date, CancellationToken ct);

        /// <summary>Sends the current user's booked slots.</summary>
        Task SendUseTimeByUserAsync(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends dates with occupied records for admin to choose from.</summary>
        Task SendDatesForDeleteAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>Sends occupied times for a specific date for admin deletion.</summary>
        Task SendTimesForDeleteAsync(long chatId, int messageId, DateTime date, CancellationToken ct);
    }
}
