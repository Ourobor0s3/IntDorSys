using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Services;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class LaundressMessageHandler : ILaundressMessageHandler
    {
        private readonly ILaundressBotMenuService _laundMenu;
        private readonly ILaundressBotBookingService _laundBooking;
        private readonly IUserQueryService _userService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;
        private readonly ILogger<LaundressMessageHandler> _logger;

        public LaundressMessageHandler(
            ILaundressBotMenuService laundMenu,
            ILaundressBotBookingService laundBooking,
            IUserQueryService userService,
            IOptionsMonitor<AdminSettings> adminSettings,
            ILogger<LaundressMessageHandler> logger)
        {
            _laundMenu = laundMenu;
            _laundBooking = laundBooking;
            _userService = userService;
            _adminSettings = adminSettings;
            _logger = logger;
        }

        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            try
            {
                var userResult = await _userService.GetByTgIdAsync(message.From!.Id, ct);
                if (!userResult.IsSuccess) return;
                var userInfo = userResult.Data;

                if (message.Text != null)
                {
                    var userMessage = message.Text;

                    if (userMessage.Equals(MessageKeyConstants.Menu))
                    {
                        await _laundMenu.SendMenuAsync(userInfo, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.AllFreeRecords))
                    {
                        await _laundMenu.SendFreeDateAsync(userInfo.TelegramId, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.MyRecords))
                    {
                        await _laundMenu.SendUseTimeByUserAsync(userInfo, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.AllRecords)
                          && _adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId))
                    {
                        await _laundMenu.SendAllTimeAsync(userInfo.TelegramId, ct: ct);
                    }
                }

                if (_adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId) && message.Text != null)
                {
                    var specialMessage = message.Text.Split("$");

                    switch (specialMessage[0])
                    {
                        case "/crlaund":
                            switch (specialMessage.Length)
                            {
                                case 3:
                                    await _laundBooking.CreateTimesAsync(userInfo,
                                        specialMessage[1].Replace('.', '-'),
                                        int.Parse(specialMessage[2]),
                                        int.Parse(specialMessage[2]),
                                        ct);
                                    break;
                                case 4:
                                    await _laundBooking.CreateTimesAsync(userInfo,
                                        specialMessage[1].Replace('.', '-'),
                                        int.Parse(specialMessage[2]),
                                        int.Parse(specialMessage[3]),
                                        ct);
                                    break;
                                case 5:
                                    for (var i = 0; i < int.Parse(specialMessage[2]); i++)
                                    {
                                        var date = DateTime.Parse(specialMessage[1].Replace('.', '-')).AddDays(i).ToString(DateFormatConstants.DateFormat);

                                        await _laundBooking.CreateTimesAsync(userInfo,
                                            date,
                                            int.Parse(specialMessage[3]),
                                            int.Parse(specialMessage[4]),
                                            ct);
                                    }
                                    break;
                            }
                            break;
                        case "/rmlaund":
                            switch (specialMessage.Length)
                            {
                                case 2:
                                    await _laundBooking.DeleteLaundAsync(userInfo, DateTime.Parse(specialMessage[1].Replace('.', '-')), ct);
                                    break;
                                case 4:
                                    for (var i = int.Parse(specialMessage[2]); i <= int.Parse(specialMessage[3]); i += 2)
                                    {
                                        await _laundBooking.DeleteLaundAsync(userInfo,
                                            DateTime.Parse($"{specialMessage[1].Replace('.', '-')} {i}:00:00"),
                                            ct);
                                    }
                                    break;
                                case 5:
                                    var startDay = DateTime.Parse(specialMessage[1].Replace('.', '-'));
                                    var dayCountDel = int.Parse(specialMessage[2]);
                                    var startHour = int.Parse(specialMessage[3]);
                                    var endHour = int.Parse(specialMessage[4]);
                                    for (var i = 0; i < dayCountDel; i++)
                                    {
                                        var date = startDay.AddDays(i).ToString(DateFormatConstants.DateFormat);
                                        for (var h = startHour; h <= endHour; h += 2)
                                        {
                                            await _laundBooking.DeleteLaundAsync(userInfo,
                                                DateTime.Parse($"{date} {h}:00:00"),
                                                ct);
                                        }
                                    }
                                    break;
                            }
                            break;
                        case "/unuse" when specialMessage.Length == 2:
                            await _laundBooking.UnUseLaundAsync(userInfo, DateTime.Parse(specialMessage[1].Replace('.', '-')), ct);
                            break;
                        case "/rmuse" when specialMessage.Length == 2:
                            await _laundBooking.DelUseTimeByAdminAsync(userInfo, DateTime.Parse(specialMessage[1].Replace('.', '-')), ct);
                            break;
                        case "/setuser" when specialMessage.Length == 3:
                            await _laundBooking.AddUserToTimeAsync(userInfo, specialMessage[1], DateTime.Parse(specialMessage[2].Replace('.', '-')), ct);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling laundress message from user {UserId}", message.From?.Id);
            }
        }
    }
}