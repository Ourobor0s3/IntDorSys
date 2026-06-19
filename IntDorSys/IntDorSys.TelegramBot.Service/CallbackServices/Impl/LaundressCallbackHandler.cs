using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Services;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CallbackServices.Impl
{
    internal sealed class LaundressCallbackHandler : ILaundressCallbackHandler
    {
        private readonly ILaundressBotMenuService _laundMenu;
        private readonly ILaundressBotBookingService _laundBooking;
        private readonly IUserQueryService _userService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;
        private readonly ILogger<LaundressCallbackHandler> _logger;

        public LaundressCallbackHandler(
            ILaundressBotMenuService laundMenu,
            ILaundressBotBookingService laundBooking,
            IUserQueryService userService,
            IOptionsMonitor<AdminSettings> adminSettings,
            ILogger<LaundressCallbackHandler> logger)
        {
            _laundMenu = laundMenu;
            _laundBooking = laundBooking;
            _userService = userService;
            _adminSettings = adminSettings;
            _logger = logger;
        }

        public async Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct = default)
        {
            try
            {
                var userInfo = (await _userService.GetByTgIdAsync(callbackQuery.From.Id, ct)).Data;
                var listCallback = callbackQuery.Data!.Split("//");

                switch (listCallback[0])
                {
                    case "laund":
                        if (listCallback[1].Equals(MessageText.AllFreeRecords))
                        {
                            await _laundMenu.SendFreeDateAsync(
                                userInfo.TelegramId,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }
                        else if (listCallback[1].Equals(MessageText.MyRecords))
                        {
                            await _laundMenu.SendUseTimeByUserAsync(
                                userInfo,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }
                        else if (listCallback[1].Equals(MessageText.AllRecords)
                                                      && _adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId))
                        {
                            await _laundMenu.SendAllTimeAsync(
                                userInfo.TelegramId,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }
                        else if (listCallback[1].Equals(MessageText.DeleteRecords)
                              && _adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId))
                        {
                            await _laundMenu.SendDatesForDeleteAsync(
                                userInfo.TelegramId,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }

                        break;
                    case "UseDate":
                        await _laundMenu.SendFreeTimeAsync(
                            userInfo.TelegramId,
                            callbackQuery.Message!.MessageId,
                            DateTime.Parse(listCallback[1]),
                            ct);
                        break;
                    case "UseTime":
                        await _laundBooking.UseTimeLaundByUserAsync(
                            userInfo,
                            DateTime.Parse(listCallback[1]),
                            callbackQuery.Message?.MessageId ?? 0,
                            ct);
                        await _laundMenu.SendFreeTimeAsync(
                            userInfo.TelegramId,
                            callbackQuery.Message!.MessageId,
                            DateTime.Parse(listCallback[1]),
                            ct);
                        break;
                    case "DeleteUserTime":
                        await _laundBooking.RemoveTimeByUserAsync(userInfo, DateTime.Parse(listCallback[1]), ct);
                        await _laundMenu.SendUseTimeByUserAsync(userInfo, callbackQuery.Message!.MessageId, ct);
                        break;
                    case "DelDate":
                        await _laundMenu.SendTimesForDeleteAsync(
                            userInfo.TelegramId,
                            callbackQuery.Message!.MessageId,
                            DateTime.Parse(listCallback[1]),
                            ct);
                        break;
                    case "DelUseTime":
                        await _laundBooking.DelUseTimeByAdminAsync(userInfo, DateTime.Parse(listCallback[1]), ct);
                        await _laundMenu.SendDatesForDeleteAsync(userInfo.TelegramId, callbackQuery.Message!.MessageId, ct);
                        break;

                    case MessageKeyConstants.Back:
                        if (listCallback[1].Equals(MessageKeyConstants.Menu))
                        {
                            await _laundMenu.SendMenuAsync(
                                userInfo,
                                callbackQuery.Message!.MessageId,
                                ct);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling laundress callback from user {UserId}", callbackQuery.From.Id);
            }
        }
    }
}