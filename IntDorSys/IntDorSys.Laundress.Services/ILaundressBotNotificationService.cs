namespace IntDorSys.Laundress.Services
{
    /// <summary>Background notification checks for laundry slots.</summary>
    public interface ILaundressBotNotificationService
    {
        /// <summary>Checks upcoming slots and sends reminders to booked users.</summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task CheckTimeAndSendNotifAsync(CancellationToken ct);
    }
}