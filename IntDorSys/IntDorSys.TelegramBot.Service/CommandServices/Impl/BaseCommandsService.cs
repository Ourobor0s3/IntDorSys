using IntDorSys.Core.Constants;
using IntDorSys.Services.AppSettings;
using IntDorSys.Services.Users;
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

        /// <inheritdoc />
        public async Task StartHandleAsync(Message message, CancellationToken ct)
        {
            var userResult = await _userService.GetByTgIdAsync(message.From!.Id, ct);
            if (!userResult.IsSuccess) return;
            await _telegramService.SendMessageAsync(userResult.Data.TelegramId, MessageText.Start, ct);
        }

        /// <inheritdoc />
        public async Task RulesHandleAsync(Message message, CancellationToken ct)
        {
            var userResult = await _userService.GetByTgIdAsync(message.From!.Id, ct);
            if (!userResult.IsSuccess) return;
            var rules = await _settings.GetValueAsync("Rules", ct);
            await _telegramService.SendMessageAsync(userResult.Data.TelegramId, rules ?? MessageText.Rules, ct);
        }
    }
}