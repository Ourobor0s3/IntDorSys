using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Core.Interface
{
    /// <summary>
    /// Interface message service
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// The basic method that processes the messages request
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public Task HandleAsync(Message message, CancellationToken ct = default);
    }
}