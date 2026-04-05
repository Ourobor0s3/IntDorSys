namespace IntDorSys.TelegramBot.Core.Services
{
    /// <summary>
    /// Main handler
    /// </summary>
    public interface IBotHandler
    {
        /// <summary>
        /// Launching the bot
        /// </summary>
        /// <returns></returns>
        Task StartBot();
    }
}