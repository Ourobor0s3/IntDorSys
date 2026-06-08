using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices
{
    /// <summary>
    ///     Handles basic Telegram bot commands (/start, /rules)
    /// </summary>
    public interface IBaseCommandsService
    {
        /// <summary>
        ///     Processes the /start command — sends welcome message
        /// </summary>
        /// <param name="message">Incoming Telegram message</param>
        /// <param name="ct">Cancellation token</param>
        Task StartHandleAsync(Message message, CancellationToken ct);

        /// <summary>
        ///     Processes the /rules command — sends usage rules
        /// </summary>
        /// <param name="message">Incoming Telegram message</param>
        /// <param name="ct">Cancellation token</param>
        Task RulesHandleAsync(Message message, CancellationToken ct);
    }
}