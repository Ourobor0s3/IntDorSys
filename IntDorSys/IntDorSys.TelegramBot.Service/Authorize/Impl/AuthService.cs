using IntDorSys.Core.Constants;
using IntDorSys.Core.Enums;
using IntDorSys.Services.Users;
using IntDorSys.TelegramBot.Service.AdminServices;
using Microsoft.Extensions.Logging;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize.Impl
{
    internal sealed class AuthService : IAuthService
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AuthService> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IUserService _userService;

        public AuthService(
            ITelegramService telegramService,
            IAdminService adminService,
            IUserService userService,
            ILogger<AuthService> logger)
        {
            _telegramService = telegramService;
            _adminService = adminService;
            _userService = userService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> AuthUser(Update update, CancellationToken ct)
        {
            var user = update.Message != null ? update.Message.From : update.CallbackQuery!.From;
            var userInfo = (await _userService.CreateOrUpdateTgInfoAsync(user!, ct)).Data;
            var message = update.Message ?? update.CallbackQuery?.Message;

            if (userInfo is { IsConfirm: false, FullName: "" or null })
            {
                var messageReply = message?.ReplyToMessage;
                if (messageReply == null || !messageReply.Text!.Equals(MessageText.GetUserInfo))
                {
                    await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.GetUserInfo, ct);
                }
                else
                {
                    var userUpdatingInfo = message?.Text?.Split(", ");
                    if (userUpdatingInfo?.Length == 3)
                    {
                        userInfo.FullName = userUpdatingInfo[0];
                        userInfo.NumGroup = userUpdatingInfo[1];
                        userInfo.NumRoom = userUpdatingInfo[2];

                        _logger.LogInformation("New user registered the system: {name}, group: {group}, room: {room}",
                            userInfo.FullName,
                            userInfo.NumGroup,
                            userInfo.NumRoom);
                        await _userService.UpdateUserInfo(userInfo, ct);
                        await _adminService.SendUsersNotificationAsync(userInfo, ct);
                    }
                    else
                    {
                        _logger.LogError("User attempt to register is invalid, user_tg_id: {tgId}, info: {info}",
                            userInfo.TelegramId,
                            userUpdatingInfo);
                        await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.NotCorrect, ct);
                        await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.GetUserInfo, ct);
                    }
                }
            }
            else if (!userInfo.IsConfirm)
            {
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.NotConfirmTg, ct);
            }

            return userInfo is { IsConfirm: true, Status: not UserStatus.Blocked };
        }
    }
}