using IntDorSys.TelegramBot.Core.Extensions;
using IntDorSys.TelegramBot.Core.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace IntDorSys.TelegramBot.Core.Services.Impl
{
    internal sealed class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _botClient;

        /// <inheritdoc cref="ITelegramBotClient" />
        public TelegramService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <inheritdoc />
        public async Task SendMessageAsync(long chatId, string text, CancellationToken ct)
        {
            var sendMessage = new BotResponceMessage
            {
                Message = text,
            };
            await SendResponseMessageAsync(chatId, sendMessage, ct);
        }

        /// <inheritdoc />
        public async Task SendMessageAsync(List<long> chatId, string text, CancellationToken ct)
        {
            var sendMessage = new BotResponceMessage
            {
                Message = text,
            };
            foreach (var id in chatId)
            {
                await SendResponseMessageAsync(id, sendMessage, ct);
            }
        }

        /// <inheritdoc />
        public async Task EditMessageTextAsync(
            long chatId,
            int messageId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2)
        {
            var textMessage = message.Message!.ToTelegramLiteral();
            var inlineMarkup = message.InlineKeyboard;

            await _botClient.EditMessageText(
                chatId,
                messageId,
                textMessage,
                tgParseMode,
                replyMarkup: inlineMarkup,
                cancellationToken: ct
            );
        }

        /// <inheritdoc />
        public async Task DeleteMessageAsync(long chatId, int messageId, CancellationToken ct)
        {
            await _botClient.DeleteMessage(chatId, messageId, ct);
        }

        /// <inheritdoc />
        public async Task SendResponseMessageAsync(
            long chatId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2)
        {
            if (message.Gif != null)
            {
                await _botClient.SendAnimation(chatId,
                    message.Gif,
                    message.Message!.ToTelegramLiteral(),
                    tgParseMode,
                    cancellationToken: ct);
                message.Gif.Content.Close();
            }
            else if (message.ImagesList.Any())
            {
                await _botClient.SendMediaGroup(chatId, message.ImagesList, cancellationToken: ct);
            }
            else
            {
                try
                {
                    var replyMarkup = message.KeyboardMarkup;
                    if (replyMarkup != null)
                    {
                        replyMarkup.ResizeKeyboard = true;
                    }

                    var inlineMarkup = message.InlineKeyboard;

                    var textMessage = message.Message!.ToTelegramLiteral();
                    await _botClient.SendMessage(chatId,
                        textMessage,
                        tgParseMode,
                        replyMarkup: inlineMarkup != null ? inlineMarkup : replyMarkup,
                        cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public async Task SendResponseMessageAsync(
            List<long> chatId,
            BotResponceMessage message,
            CancellationToken ct,
            ParseMode tgParseMode = ParseMode.MarkdownV2)
        {
            foreach (var id in chatId)
            {
                await SendResponseMessageAsync(id, message, ct, tgParseMode);
            }
        }
    }
}