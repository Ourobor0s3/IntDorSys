using IntDorSys.Core.Constants;
using Ouro.Backup.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class AdminMessageHandler : IAdminMessageHandler
    {
        private readonly IDumpService _dumpService;

        public AdminMessageHandler(IDumpService dumpService)
        {
            _dumpService = dumpService;
        }

        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            // if (message is { Text: "/send", ReplyToMessage: not null })
            // {
            //     await _telegramService.SendMessageAsync(AdminConstants.AdminsChatId, message.Text, ct);
            // }

            if (message.Text == "/dump")
            {
                await _dumpService.CreateDump(AdminConstants.AdminChatId, ct);
            }
        }
    }
}