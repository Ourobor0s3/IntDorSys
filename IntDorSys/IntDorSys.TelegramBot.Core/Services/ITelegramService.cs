using IntDorSys.TelegramBot.Core.Models;
using Telegram.Bot.Types.Enums;

namespace IntDorSys.TelegramBot.Core.Services
{
    public interface ITelegramService
    {
        /// <summary>
        /// Delete message by chat id and message id
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="messageId">MessageHandler id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task DeleteMessageAsync(long chatId, int messageId, CancellationToken ct);

        /// <summary>
        /// Edit message by chat id and message id
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="messageId">MessageHandler id</param>
        /// <param name="message">Bot response message</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="tgParseMode">Parse mode</param>
        /// <returns></returns>
        Task EditMessageTextAsync(
            long chatId,
            int messageId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2);

        /// <summary>
        /// Send text by chat id
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="text">Text</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendMessageAsync(long chatId, string text, CancellationToken ct);

        /// <summary>
        /// Send text by list chat id
        /// </summary>
        /// <param name="chatId">List chat id</param>
        /// <param name="text">Text</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendMessageAsync(List<long> chatId, string text, CancellationToken ct);

        /// <summary>
        /// Send message by chat id
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="message">Bot response message</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="tgParseMode"></param>
        /// <returns></returns>
        Task SendResponseMessageAsync(
            long chatId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2);

        /// <summary>
        /// Send message by list chat id
        /// </summary>
        /// <param name="chatId">List chat id</param>
        /// <param name="message">Bot response message</param>
        /// <param name="ct">Cancellation token</param>
        /// <param name="tgParseMode">Parse mode</param>
        /// <returns></returns>
        Task SendResponseMessageAsync(
            List<long> chatId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2);
    }
}