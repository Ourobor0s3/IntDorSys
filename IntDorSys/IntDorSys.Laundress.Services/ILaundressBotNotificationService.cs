namespace IntDorSys.Laundress.Services
{
    /// <summary>Background notification checks for laundry slots.</summary>
    public interface ILaundressBotNotificationService
    {
        /// <summary>Checks upcoming slots and sends reminders to booked users.</summary>
        Task CheckTimeAndSendNotifAsync(CancellationToken ct);
    }
}