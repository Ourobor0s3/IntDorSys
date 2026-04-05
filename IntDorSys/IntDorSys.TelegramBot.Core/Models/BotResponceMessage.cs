using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.TelegramBot.Core.Models
{
    public class BotResponceMessage
    {
        public string? Message { get; set; }
        public InputFileUrl? PhotoUrl { get; set; }
        public List<BotResponceMessage>? SubMessages { get; set; } = [];
        public ReplyKeyboardMarkup? KeyboardMarkup { get; set; }
        public InlineKeyboardMarkup? InlineKeyboard { get; set; }

        /// <summary>
        /// Массив картинок для отправки группой картинок
        /// </summary>
        public List<InputMediaPhoto> ImagesList { get; set; } = [];

        /// <summary>
        /// gif для отправки с текстом
        /// </summary>
        public InputFileStream? Gif { get; set; }
    }
}