using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Services.Services;
using Microsoft.Extensions.Options;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class UsersMessageHandler : IUsersMessageHandler
    {
        private readonly ILaundReportService _laundReportService;
        private readonly ITelegramService _telegramService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public UsersMessageHandler(
            ITelegramService telegramService,
            ILaundReportService iLaundReportService,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _telegramService = telegramService;
            _laundReportService = iLaundReportService;
            _adminSettings = adminSettings;
        }

        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            if (message is { Text: "/send", ReplyToMessage: not null })
            {
                await _laundReportService.SaveReportAsync(message, ct);
                await _telegramService.SendMessageAsync(
                    _adminSettings.CurrentValue.AdminChatId,
                    $"{message.From!.FirstName} send report",
                    ct);
                await _telegramService.ForwardMediaGroupAsync(
                    _adminSettings.CurrentValue.ManagerLaundressId,
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
