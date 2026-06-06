using IntDorSys.Core.Constants;
using IntDorSys.Services.AppSettings;
using IntDorSys.Services.Users;
using IntDorSys.TelegramBot.Service.AdminServices;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices.Impl
{
    internal sealed class BaseCommandsService : IBaseCommandsService
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserQueryService _userService;
        private readonly IAppSettingService _settings;

        public BaseCommandsService(
            IUserQueryService userService,
            ITelegramService telegramService,
            IAppSettingService settings)
        {
            _userService = userService;
            _telegramService = telegramService;
            _settings = settings;
        }

        public async Task StartHandleAsync(Message message, CancellationToken ct)
        {
            var userInfo = (await _userService.GetByTgIdAsync(message.From!.Id, ct)).Data;
            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.Start, ct);
        }

        public async Task RulesHandleAsync(Message message, CancellationToken ct)
        {
            var userInfo = (await _userService.GetByTgIdAsync(message.From!.Id, ct)).Data;
            var rules = await _settings.GetValueAsync("Rules", ct);
            await _telegramService.SendMessageAsync(userInfo.TelegramId, rules ?? MessageText.Rules, ct);
        }
    }
}