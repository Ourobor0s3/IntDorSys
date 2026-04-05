using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Core.Interface
{
    /// <summary>
    /// Interface callback service
    /// </summary>
    public interface ICallbackService
    {
        /// <summary>
        /// The basic method that processes the callback request
        /// </summary>
        /// <param name="callbackQuery">Callback query</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct = default);
    }
}