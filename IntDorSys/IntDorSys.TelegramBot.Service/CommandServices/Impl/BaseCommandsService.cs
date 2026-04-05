using IntDorSys.Core.Constants;
using IntDorSys.Services.Users;
using IntDorSys.TelegramBot.Service.AdminServices;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices.Impl
{
    internal sealed class BaseCommandsService : IBaseCommandsService
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserService _userService;

        public BaseCommandsService(
            IUserService userService,
            IAdminService adminService,
            ITelegramService telegramService)
        {
            _userService = userService;
            _telegramService = telegramService;
        }

        public async Task StartHandleAsync(Message message, CancellationToken ct)
        {
            var userInfo = (await _userService.GetByTgIdAsync(message.From!.Id, ct)).Data;
            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.Start, ct);
        }

        public async Task RulesHandleAsync(Message message, CancellationToken ct)
        {
            var userInfo = (await _userService.GetByTgIdAsync(message.From!.Id, ct)).Data;
            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.Rules, ct);
        }
    }
}