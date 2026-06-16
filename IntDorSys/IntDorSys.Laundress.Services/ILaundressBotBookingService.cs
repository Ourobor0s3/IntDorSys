using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Services
{
    /// <summary>Booking and slot management operations (create, delete, book, unbook).</summary>
    public interface ILaundressBotBookingService
    {
        /// <summary>Creates time slots for the specified date and hour range.</summary>
        /// <param name="crUser">User who creates the slots</param>
        /// <param name="date">Date string (yyyy-MM-dd)</param>
        /// <param name="start">Starting hour</param>
        /// <param name="end">Ending hour (inclusive)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task CreateTimesAsync(
            UserInfo crUser,
            string date,
            int start,
            int end,
            CancellationToken ct);

        /// <summary>Deletes a specific time slot.</summary>
        /// <param name="user">User performing the deletion</param>
        /// <param name="dateTime">Slot date and time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task DeleteLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct);

        /// <summary>Removes the current user's booking from a time slot.</summary>
        /// <param name="user">User removing their booking</param>
        /// <param name="time">Slot date and time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task RemoveTimeByUserAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Admin — removes a user from a time slot.</summary>
        /// <param name="user">Admin performing the action</param>
        /// <param name="time">Slot date and time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task UnUseLaundAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Books the current user on a specific free time slot.</summary>
        /// <param name="user">User booking the slot</param>
        /// <param name="time">Slot date and time</param>
        /// <param name="messageId">Optional message ID to edit</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task UseTimeLaundByUserAsync(UserInfo user, DateTime time, int messageId = 0, CancellationToken ct = default);

        /// <summary>Admin — deletes an occupied record from a time slot.</summary>
        /// <param name="user">Admin performing the action</param>
        /// <param name="time">Slot date and time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task DelUseTimeByAdminAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Admin — books a specific user on a specific time slot.</summary>
        /// <param name="admin">Admin performing the action</param>
        /// <param name="userName">User name to search and book</param>
        /// <param name="time">Slot date and time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task AddUserToTimeAsync(UserInfo admin, string userName, DateTime time, CancellationToken ct);
    }
}