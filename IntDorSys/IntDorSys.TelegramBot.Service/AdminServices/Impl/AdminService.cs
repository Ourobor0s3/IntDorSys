using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.TelegramBot.Core.Extensions;
using Ouro.TelegramBot.Core.Models;
using Ouro.TelegramBot.Core.Services;

namespace IntDorSys.TelegramBot.Service.AdminServices.Impl
{
    internal sealed class AdminService : IAdminService
    {
        private readonly IUserQueryService _userQueryService;
        private readonly IUserCommandService _userCommandService;
        private readonly ILogger<AdminService> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public AdminService(
            IUserQueryService userQueryService,
            IUserCommandService userCommandService,
            ITelegramService telegramService,
            ILogger<AdminService> logger,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _userQueryService = userQueryService;
            _userCommandService = userCommandService;
            _telegramService = telegramService;
            _logger = logger;
            _adminSettings = adminSettings;
        }

        public async Task SendUsersNotificationAsync(UserInfo userInfo, CancellationToken ct)
        {
            if (_adminSettings.CurrentValue.AdminsChatId.Contains(userInfo.TelegramId))
            {
                await AutoConfirmAsync(userInfo, ct);
                return;
            }

            var sendMessage = new BotResponceMessage
            {
                Message = $"New user {userInfo.TelegramId} : {userInfo.Username}\nFull name: {userInfo.FullName}\nGroup: {userInfo.NumGroup}\nRoom: {userInfo.NumRoom}".ToTelegramLiteral(),
                InlineKeyboard = ButtonConstants.BtnYesOrNo(userInfo.TelegramId, "ConfirmUser"),
            };

            await _telegramService.SendResponseMessageAsync(_adminSettings.CurrentValue.AdminsChatId, sendMessage, ct);
            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.CreateNotification, ct);
        }

        public async Task UpdateNotificateUserAsync(long userId, CancellationToken ct)
        {
            var userInfoResult = await _userQueryService.GetByTgIdAsync(userId, ct);

            if (!userInfoResult.IsSuccess)
            {
                return;
            }

            await _userCommandService.ConfirmUserWithRoleAsync(userInfoResult.Data.Id, UserRoleKeys.Student, ct);

            await _telegramService.SendMessageAsync(_adminSettings.CurrentValue.AdminChatId,
                $"User {userInfoResult.Data.FullName}({userInfoResult.Data.Username}) confirm!",
                ct);
        }

        private async Task AutoConfirmAsync(UserInfo userInfo, CancellationToken ct)
        {
            await _userCommandService.ConfirmUserWithRoleAsync(userInfo.Id, UserRoleKeys.Admin, ct);
            await _userCommandService.ConfirmUserWithRoleAsync(userInfo.Id, UserRoleKeys.Student, ct);

            _logger.LogInformation("User {name} (telegram_id: {id}) auto-confirmed as Admin, Student",
                userInfo.FullName, userInfo.TelegramId);

            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.ConfirmTg, ct);
        }
    }
}