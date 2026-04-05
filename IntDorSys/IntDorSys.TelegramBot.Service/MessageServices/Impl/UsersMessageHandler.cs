using IntDorSys.Core.Constants;
using IntDorSys.Laundress.Services.Services;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class UsersMessageHandler : IUsersMessageHandler
    {
        private readonly ILaundReportService _laundReportService;
        private readonly ITelegramService _telegramService;

        public UsersMessageHandler(ITelegramService telegramService, ILaundReportService iLaundReportService)
        {
            _telegramService = telegramService;
            _laundReportService = iLaundReportService;
        }

        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            if (message is { Text: "/send", ReplyToMessage: not null })
            {
                await _laundReportService.SaveReportAsync(message, ct);
                await _telegramService.SendMessageAsync(
                    AdminConstants.AdminChatId,
                    $"{message.From!.FirstName} send report",
                    ct);
                await _telegramService.ForwardMediaGroupAsync(
                    AdminConstants.ManagerLaundressId,
                    message.ReplyToMessage!,
                    ct);
                await _telegramService.SendMessageAsync(
                    message.From!.Id,
                    $"Отчет отправлен!",
                    ct);
            }

            if (message is { Type: MessageType.Photo, Photo: not null })
            {
                await _laundReportService.SavePhotoAsync(message, ct);
            }
        }
    }
}