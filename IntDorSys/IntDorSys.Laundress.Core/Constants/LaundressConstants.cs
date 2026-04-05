using IntDorSys.Core.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.Laundress.Core.Constants
{
    public static class LaundressConstants
    {
        public static readonly InlineKeyboardMarkup BtnLaundress = new([
            [InlineKeyboardButton.WithCallbackData(MessageText.AllFreeRecords, $"laund//{MessageText.AllFreeRecords}")],
            [InlineKeyboardButton.WithCallbackData(MessageText.MyRecords, $"laund//{MessageText.MyRecords}")],
        ]);

        public static readonly InlineKeyboardMarkup BtnLaundressAdm = new([
            [InlineKeyboardButton.WithCallbackData(MessageText.AllFreeRecords, $"laund//{MessageText.AllFreeRecords}")],
            [InlineKeyboardButton.WithCallbackData(MessageText.MyRecords, $"laund//{MessageText.MyRecords}")],
            [InlineKeyboardButton.WithCallbackData(MessageText.AllRecords, $"laund//{MessageText.AllRecords}")],
            [InlineKeyboardButton.WithCallbackData(MessageText.GetUsers, $"{MessageText.GetUsers}")],
            [InlineKeyboardButton.WithCallbackData(MessageText.GetBlockedUsers, $"{MessageText.GetBlockedUsers}")],
        ]);
    }
}