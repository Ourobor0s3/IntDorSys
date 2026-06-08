using IntDorSys.Laundress.Core.Entities;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services
{
    /// <summary>
    ///     Core service for managing laundry time slots, bookings and availability
    /// </summary>
    public interface ILaundressService
    {
        /// <summary>
        ///     Create wash time by model
        /// </summary>
        /// <param name="newWash">Use laundress model</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if slot was created</returns>
        Task<DataResult<bool>> CreateTimeAsync(UseLaundress newWash, CancellationToken ct);

        /// <summary>
        ///     Delete wash time by time
        /// </summary>
        /// <param name="timeWash">Wash time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if slot was removed</returns>
        Task<DataResult<bool>> RemoveTimeAsync(DateTime timeWash, CancellationToken ct);

        /// <summary>
        ///     Remove time user by user id and wash time
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="timeWash">Wash time</param>
        /// <param name="isAdmin">Is admin</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if the booking was removed</returns>
        Task<DataResult<bool>> RemoveUseTimeAsync(
            long userId,
            DateTime timeWash,
            bool isAdmin = false,
            CancellationToken ct = default);

        /// <summary>
        ///     Use time user by user id and wash time
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="timeWash">Wash time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if the slot was booked</returns>
        Task<DataResult<bool>> UseTimeAsync(long userId, DateTime timeWash, CancellationToken ct);

        /// <summary>
        ///     Create time slots for a range of even hours
        /// </summary>
        /// <param name="date">Date for slots</param>
        /// <param name="startHour">Starting hour (even)</param>
        /// <param name="endHour">Ending hour (even, inclusive)</param>
        /// <param name="createdUserId">User who creates the slots</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Number of slots created</returns>
        Task<DataResult<int>> CreateTimeRangeAsync(DateTime date, int startHour, int endHour, long createdUserId, CancellationToken ct);
    }
}