using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ouro.TelegramBot.Core.Extensions;
using Ouro.TelegramBot.Core.Models;
using Ouro.TelegramBot.Core.Services;

namespace IntDorSys.TelegramBot.Service.AdminServices.Impl
{
    internal sealed class AdminService : IAdminService
    {
        private readonly AppDataContext _dbContext;
        private readonly ILogger<AdminService> _logger;
        private readonly ITelegramService _telegramService;

        public AdminService(
            AppDataContext dbContext,
            ITelegramService telegramService,
            ILogger<AdminService> logger)
        {
            _dbContext = dbContext;
            _telegramService = telegramService;
            _logger = logger;
        }

        public async Task SendUsersNotificationAsync(UserInfo userInfo, CancellationToken ct)
        {
            var sendMessage = new BotResponceMessage
            {
                Message = $"New user {userInfo.TelegramId} : {userInfo.Username}\nFull name: {userInfo.FullName
                }\nGroup: {userInfo.NumGroup}\nRoom: {userInfo.NumRoom}".ToTelegramLiteral(),
                InlineKeyboard = ButtonConstants.BtnYesOrNo(userInfo.TelegramId, "ConfirmUser"),
            };

            await _telegramService.SendResponseMessageAsync(AdminConstants.AdminsChatId, sendMessage, ct);
            await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.CreateNotification, ct);
        }

        public async Task UpdateNotificateUserAsync(long userId, CancellationToken ct)
        {
            try
            {
                var userInfo = await _dbContext.Set<UserInfo>()
                    .FirstOrDefaultAsync(x => x.TelegramId == userId, ct);

                if (userInfo == null)
                {
                    return;
                }

                var userRoles = await _dbContext.Set<UserRoles>()
                    .FirstOrDefaultAsync(x => x.KeyRoles == UserRoleKeys.Student && x.UserId == userId, ct);

                if (userRoles == null)
                {
                    userRoles = new UserRoles
                    {
                        UserId = userId,
                        User = userInfo,
                        KeyRoles = UserRoleKeys.Student,
                    };
                    _dbContext.AddOrUpdateEntity(userRoles);
                }

                userInfo.IsConfirm = true;
                _dbContext.AddOrUpdateEntity(userInfo);
                await _dbContext.SaveChangesAsync(ct);

                _logger.LogInformation("User {name} has been confirmed (telegram_id: {id})",
                    userInfo.FullName,
                    userInfo.TelegramId);
                await _telegramService.SendMessageAsync(AdminConstants.AdminChatId,
                    $"User {userInfo.FullName}({userInfo.Username}) confirm!",
                    ct);
                await _telegramService.SendMessageAsync(userInfo.TelegramId, MessageText.ConfirmTg, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Exception)} UpdateNotificationUser: {JsonConvert.SerializeObject(userId)
                }\r\n" +
                                 $"{ex.Message}{ex.InnerException?.Message}{ex.StackTrace}");
            }
        }
    }
}