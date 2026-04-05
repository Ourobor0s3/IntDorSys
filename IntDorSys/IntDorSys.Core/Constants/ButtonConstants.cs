using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.Core.Constants
{
    public static class ButtonConstants
    {
        public static InlineKeyboardMarkup BtnYesOrNo(long telegramId, string text)
        {
            List<string> list = ["Yes", "No"];

            return new InlineKeyboardMarkup(list
                .Select(x => new[] { InlineKeyboardButton.WithCallbackData(x, $"{text}//{x}//{telegramId}") })
                .ToList());
        }
    }
}