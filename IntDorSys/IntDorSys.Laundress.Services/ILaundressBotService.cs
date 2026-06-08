namespace IntDorSys.Laundress.Services
{
    /// <summary>
    ///     Combines menu, booking and notification services for the laundry Telegram bot
    /// </summary>
    public interface ILaundressBotService :
        ILaundressBotMenuService,
        ILaundressBotBookingService,
        ILaundressBotNotificationService
    {
    }
}