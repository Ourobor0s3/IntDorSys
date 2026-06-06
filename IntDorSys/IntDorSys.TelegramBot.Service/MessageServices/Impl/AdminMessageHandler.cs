using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using Microsoft.Extensions.Options;
using Ouro.Backup.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class AdminMessageHandler : IAdminMessageHandler
    {
        private readonly IDumpService _dumpService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;
        private const string DumpCommand = "/dump";

        public AdminMessageHandler(IDumpService dumpService, IOptionsMonitor<AdminSettings> adminSettings)
        {
            _dumpService = dumpService;
            _adminSettings = adminSettings;
        }

        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            if (message.Text == DumpCommand)
            {
                await _dumpService.CreateDump(_adminSettings.CurrentValue.AdminChatId, ct);
            }
        }
    }
}
