using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.Core.Settings;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.TelegramBot.Core.Constants;
using Ouro.TelegramBot.Core.Models;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class UserBotService : IUserBotService
    {
        private readonly IUserQueryService _userQuery;
        private readonly IUserCommandService _userCommand;
        private readonly ILogger<UserBotService> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public UserBotService(
            IUserQueryService userQuery,
            IUserCommandService userCommand,
            ILogger<UserBotService> logger,
            ITelegramService telegramService,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _userQuery = userQuery;
            _userCommand = userCommand;
            _logger = logger;
            _telegramService = telegramService;
            _adminSettings = adminSettings;
        }

        /// <inheritdoc />
        public async Task SendUsersAsync(
            long userId,
            int messageId = 0,
            bool isBlockedUsers = false,
            CancellationToken ct = default)
        {
            try
            {
                var filteredUsers = (await _userQuery.GetListUsersAsync(ct)).Data
                    .Where(user => !_adminSettings.CurrentValue.ManagersLaundress.Contains(user.TelegramId))
                    .Where(user => user.Status == (isBlockedUsers ? UserStatus.Blocked : UserStatus.Registered))
                    .ToList();

                var inlineKeyboard = filteredUsers.Select(x => new[]
                    {
                        InlineKeyboardButton.WithCallbackData(x.FullName ?? x.Username,
                            $"{(isBlockedUsers ? "UnblockUser" : "BlockUser")}//{x.Id}"),
                    })
                    .ToList();
                string message;
                if (inlineKeyboard.Count != 0)
                {
                    message = isBlockedUsers
                        ? "Заблокированные пользователи (нажмите на пользователя, если хотите разблокировать):"
                        : "Пользователи с доступом (нажмите на пользователя, если хотите заблокировать):";
                }
                else
                {
                    message = isBlockedUsers
                        ? "Заблокированных пользователей нет"
                        : "Нет пользователей, имеющих доступ";
                }

                inlineKeyboard.Add([
                    KeyboardButtons.InlineButton(
                        MessageKeyConstants.Back,
                        MessageKeyConstants.Menu,
                        MessageText.Back),
                ]);

                var sendMessage = new BotResponceMessage
                {
                    Message = message,
                    InlineKeyboard = new InlineKeyboardMarkup(inlineKeyboard),
                };

                if (messageId != 0)
                {
                    await _telegramService.EditMessageTextAsync(userId, messageId, sendMessage, ct);
                }
                else
                {
                    await _telegramService.SendResponseMessageAsync(userId, sendMessage, ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in SendUsersAsync method");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task ChangeStatusUserAsync(
            long userId,
            long forUserId,
            UserStatus newStatus,
            CancellationToken ct)
        {
            try
            {
                var userResult = await _userQuery.GetAsync(forUserId, ct);
                if (!userResult.IsSuccess) return;
                var user = userResult.Data;
                var statusResult = await _userCommand.ChangeUserStatus(forUserId, newStatus, userId, ct);
                if (!statusResult.IsSuccess) return;
                var res = statusResult.Data;
                if (res)
                {
                    await _telegramService.SendMessageAsync(userId, $"User {user.FullName ?? user.Username} updated status to {newStatus}", ct);
                }
                else
                {
                    await _telegramService.SendMessageAsync(userId, $"User {user.FullName ?? user.Username} not updated status to {newStatus}", ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in SendUsersAsync method");
                throw;
            }
        }
    }
}