using IntDorSys.Core.Constants;
using IntDorSys.Core.Enums;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.TelegramBot.Service.AdminServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CallbackServices.Impl
{
    /// <inheritdoc />
    internal sealed class AdminCallbackHandler : IAdminCallbackHandler
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminCallbackHandler> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IUserBotService _userService;

        /// <inheritdoc cref="IAdminCallbackHandler" />
        public AdminCallbackHandler(
            IAdminService adminService,
            ITelegramService telegramService,
            ILogger<AdminCallbackHandler> logger,
            IUserBotService userService)
        {
            _adminService = adminService;
            _telegramService = telegramService;
            _logger = logger;
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct = default)
        {
            try
            {
                var listCallback = callbackQuery.Data!.Split("//");

                switch (listCallback[0])
                {
                    case "ConfirmUser":
                        switch (listCallback[1])
                        {
                            case "Yes":
                                await _telegramService.DeleteMessageAsync(callbackQuery.From.Id,
                                    callbackQuery.Message!.MessageId,
                                    ct);
                                await _adminService.UpdateNotificateUserAsync(long.Parse(listCallback[2]), ct);
                                break;
                            case "No":
                                await _telegramService.DeleteMessageAsync(AdminConstants.AdminChatId,
                                    callbackQuery.Message!.MessageId,
                                    ct);
                                await _telegramService.SendMessageAsync(long.Parse(listCallback[2]),
                                    MessageText.NotConfirmTg,
                                    ct);
                                await _telegramService.SendMessageAsync(AdminConstants.AdminChatId,
                                    $"User {listCallback[2]} not confirm",
                                    ct);
                                break;
                        }
                        break;
                    case "BlockUser":
                        await _userService.ChangeStatusUserAsync(
                            callbackQuery.From.Id,
                            long.Parse(listCallback[1]),
                            UserStatus.Blocked,
                            ct);
                        await _userService.SendUsersAsync(
                            callbackQuery.From.Id,
                            callbackQuery.Message!.MessageId,
                            ct: ct);
                        break;
                    case "UnblockUser":
                        await _userService.ChangeStatusUserAsync(
                            callbackQuery.From.Id,
                            long.Parse(listCallback[1]),
                            UserStatus.Registered,
                            ct);
                        await _userService.SendUsersAsync(
                            callbackQuery.From.Id,
                            callbackQuery.Message!.MessageId,
                            true,
                            ct: ct);
                        break;
                }

                if (listCallback[0].Equals(MessageText.GetUsers))
                {
                    await _userService.SendUsersAsync(
                        callbackQuery.From.Id,
                        callbackQuery.Message!.MessageId,
                        ct: ct);

                }
                else if (listCallback[0].Equals(MessageText.GetBlockedUsers))
                {
                    await _userService.SendUsersAsync(
                        callbackQuery.From.Id,
                        callbackQuery.Message!.MessageId,
                        true,
                        ct: ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Exception)} HandleMessage: {
                    JsonConvert.SerializeObject(callbackQuery.Message)}\r\n" +
                                 $"{ex.Message}{ex.InnerException?.Message}{ex.StackTrace}");
            }
        }
    }
}