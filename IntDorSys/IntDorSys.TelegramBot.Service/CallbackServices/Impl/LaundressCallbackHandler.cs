using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CallbackServices.Impl
{
    /// <inheritdoc />
    internal sealed class LaundressCallbackHandler : ILaundressCallbackHandler
    {
        private readonly ILaundressBotService _laundBot;
        private readonly IUserService _userService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        /// <inheritdoc cref="ILaundressCallbackHandler" />
        public LaundressCallbackHandler(
            ILaundressBotService laundBot,
            IUserService userService,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _laundBot = laundBot;
            _userService = userService;
            _adminSettings = adminSettings;
        }

        /// <inheritdoc />
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
                            await _laundBot.SendFreeDateAsync(
                                userInfo.TelegramId,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }
                        else if (listCallback[1].Equals(MessageText.MyRecords))
                        {
                            await _laundBot.SendUseTimeByUserAsync(
                                userInfo,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }
                        else if (listCallback[1].Equals(MessageText.AllRecords)
                              && _adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId))
                        {
                            await _laundBot.SendAllTimeAsync(
                                userInfo.TelegramId,
                                callbackQuery.Message?.MessageId ?? 0,
                                ct);
                        }

                        break;
                    case "UseDate":
                        await _laundBot.SendFreeTimeAsync(
                            userInfo.TelegramId,
                            callbackQuery.Message!.MessageId,
                            DateTime.Parse(listCallback[1]),
                            ct);
                        break;
                    case "UseTime":
                        await _laundBot.UseTimeLaundByUserAsync(
                            userInfo,
                            DateTime.Parse(listCallback[1]),
                            callbackQuery.Message?.MessageId ?? 0,
                            ct);
                        break;
                    case "DeleteUserTime":
                        await _laundBot.RemoveTimeByUserAsync(userInfo, DateTime.Parse(listCallback[1]), ct);
                        await _laundBot.SendUseTimeByUserAsync(userInfo, callbackQuery.Message!.MessageId, ct);
                        break;

                    case MessageKeyConstants.Back:
                        if (listCallback[1].Equals(MessageKeyConstants.Menu))
                        {
                            await _laundBot.SendMenu(
                                userInfo,
                                callbackQuery.Message!.MessageId,
                                ct);
                        }

                        break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
