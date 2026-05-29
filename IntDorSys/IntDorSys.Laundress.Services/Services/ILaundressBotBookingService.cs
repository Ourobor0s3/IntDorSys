using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Services.Services
{
    /// <summary>Booking and slot management operations (create, delete, book, unbook).</summary>
    public interface ILaundressBotBookingService
    {
        /// <summary>Creates time slots for the specified date and hour range.</summary>
        Task CreateTimesAsync(
            UserInfo crUser,
            string date,
            int start,
            int end,
            CancellationToken ct);

        /// <summary>Deletes a specific time slot.</summary>
        Task DeleteLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct);

        /// <summary>Removes the current user's booking from a time slot.</summary>
        Task RemoveTimeByUserAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Admin — removes a user from a time slot.</summary>
        Task UnUseLaundAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Books the current user on a specific free time slot.</summary>
        Task UseTimeLaundByUserAsync(UserInfo user, DateTime time, int messageId = 0, CancellationToken ct = default);

        /// <summary>Admin — deletes an occupied record from a time slot.</summary>
        Task DelUseTimeByAdminAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>Admin — books a specific user on a specific time slot.</summary>
        Task AddUserToTimeAsync(UserInfo admin, string userName, DateTime time, CancellationToken ct);
    }
}
