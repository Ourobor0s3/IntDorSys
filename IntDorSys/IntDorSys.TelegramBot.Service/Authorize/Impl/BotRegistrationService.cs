using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Services.Users;
using IntDorSys.TelegramBot.Service.AdminServices;
using Microsoft.Extensions.Logging;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize.Impl
{
    internal sealed class BotRegistrationService : IBotRegistrationService
    {
        private readonly ITelegramService _telegramService;
        private readonly IUserCommandService _userService;
        private readonly IAdminService _adminService;
        private readonly ILogger<BotRegistrationService> _logger;

        public BotRegistrationService(
            ITelegramService telegramService,
            IUserCommandService userService,
            IAdminService adminService,
            ILogger<BotRegistrationService> logger)
        {
            _telegramService = telegramService;
            _userService = userService;
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<bool> TryRegisterAsync(UserInfo userInfo, Update update, CancellationToken ct)
        {
            var message = update.Message ?? update.CallbackQuery?.Message;
            var messageReply = message?.ReplyToMessage;

            if (messageReply == null || messageReply.Text != MessageText.GetUserInfo)
            {
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.GetUserInfo, ct);
                return false;
            }

            var userUpdatingInfo = message?.Text?.Split(", ");
            if (userUpdatingInfo?.Length != 3)
            {
                _logger.LogError("User attempt to register is invalid, user_tg_id: {tgId}, info: {info}",
                    userInfo.TelegramId, userUpdatingInfo);
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.NotCorrect, ct);
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.GetUserInfo, ct);
                return false;
            }

            userInfo.FullName = userUpdatingInfo[0];
            userInfo.NumGroup = userUpdatingInfo[1];
            userInfo.NumRoom = userUpdatingInfo[2];

            _logger.LogInformation("New user registered the system: {name}, group: {group}, room: {room}",
                userInfo.FullName, userInfo.NumGroup, userInfo.NumRoom);
            await _userService.UpdateUserInfo(userInfo, ct);
            await _adminService.SendUsersNotificationAsync(userInfo, ct);
            return true;
        }
    }
}
