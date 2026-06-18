using IntDorSys.Core.Constants;
using IntDorSys.Core.Enums;
using IntDorSys.Services.Users;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize.Impl
{
    internal sealed class AuthService : IBotAuthService
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserCommandService _userService;
        private readonly IBotRegistrationService _registrationService;

        public AuthService(
            ITelegramService telegramService,
            IUserCommandService userService,
            IBotRegistrationService registrationService)
        {
            _telegramService = telegramService;
            _userService = userService;
            _registrationService = registrationService;
        }

        /// <inheritdoc />
        public async Task<bool> AuthUser(Update update, CancellationToken ct)
        {
            var user = update.Message?.From ?? update.CallbackQuery?.From;
            if (user == null)
            {
                return false;
            }

            var userInfo = (await _userService.CreateOrUpdateTgInfoAsync(user, ct)).Data;

            if (userInfo is { IsConfirm: false, FullName: "" or null })
            {
                await _registrationService.TryRegisterAsync(userInfo, update, ct);
            }
            else if (!userInfo.IsConfirm)
            {
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.NotConfirmTg, ct);
            }

            return userInfo is { IsConfirm: true, Status: not UserStatus.Blocked };
        }
    }
}