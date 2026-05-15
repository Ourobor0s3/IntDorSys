using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    /// <inheritdoc />
    internal sealed class LaundressMessageHandler : ILaundressMessageHandler
    {
        private readonly ILaundressBotService _laundBot;
        private readonly IUserService _userService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public LaundressMessageHandler(
            ILaundressBotService laundBot,
            IUserService userService,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _laundBot = laundBot;
            _userService = userService;
            _adminSettings = adminSettings;
        }

        /// <inheritdoc />
        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            try
            {
                var userInfo = (await _userService.GetByTgIdAsync(message.From!.Id, ct)).Data;
                if (message.Text != null)
                {
                    var userMessage = message.Text;

                    if (userMessage.Equals(MessageKeyConstants.Menu))
                    {
                        await _laundBot.SendMenu(userInfo, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.AllFreeRecords))
                    {
                        await _laundBot.SendFreeDateAsync(userInfo.TelegramId, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.MyRecords))
                    {
                        await _laundBot.SendUseTimeByUserAsync(userInfo, ct: ct);
                    }
                    else if (userMessage.Equals(MessageText.AllRecords)
                          && _adminSettings.CurrentValue.ManagersLaundress.Contains(userInfo.TelegramId))
                    {
                        await _laundBot.SendAllTimeAsync(userInfo.TelegramId, ct: ct);
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
                                    await _laundBot.CreateTimesAsync(userInfo,
                                        specialMessage[1],
                                        int.Parse(specialMessage[2]),
                                        int.Parse(specialMessage[2]),
                                        ct);
                                    break;
                                case 4:
                                    await _laundBot.CreateTimesAsync(userInfo,
                                        specialMessage[1],
                                        int.Parse(specialMessage[2]),
                                        int.Parse(specialMessage[3]),
                                        ct);
                                    break;
                                case 5:
                                    for (var i = 0; i < int.Parse(specialMessage[2]); i++)
                                    {
                                        var date = DateTime.Parse(specialMessage[1]).AddDays(i).ToShortDateString();

                                        await _laundBot.CreateTimesAsync(userInfo,
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
                                    await _laundBot.DeleteLaundAsync(userInfo, DateTime.Parse(specialMessage[1]), ct);
                                    break;
                                case 4:
                                    for (var i = int.Parse(specialMessage[2]); i <= int.Parse(specialMessage[3]); i += 2)
                                    {
                                        await _laundBot.DeleteLaundAsync(userInfo,
                                            DateTime.Parse($"{specialMessage[1]} {i}:00:00"),
                                            ct);
                                    }
                                    break;
                                case 5:
                                    var startDay = DateTime.Parse(specialMessage[1]);
                                    var dayCountDel = int.Parse(specialMessage[2]);
                                    var startHour = int.Parse(specialMessage[3]);
                                    var endHour = int.Parse(specialMessage[4]);
                                    for (var i = 0; i < dayCountDel; i++)
                                    {
                                        var date = startDay.AddDays(i).ToShortDateString();
                                        for (var h = startHour; h <= endHour; h += 2)
                                        {
                                            await _laundBot.DeleteLaundAsync(userInfo,
                                                DateTime.Parse($"{date} {h}:00:00"),
                                                ct);
                                        }
                                    }
                                    break;
                            }
                            break;
                        case "/unuse" when specialMessage.Length == 2:
                            await _laundBot.UnUseLaundAsync(userInfo, DateTime.Parse(specialMessage[1]), ct);
                            break;
                        case "/rmuse" when specialMessage.Length == 2:
                            await _laundBot.DelUseTimeByAdminAsync(userInfo, DateTime.Parse(specialMessage[1]), ct);
                            break;
                        case "/setuser" when specialMessage.Length == 3:
                            await _laundBot.AddUserToTimeAsync(userInfo, specialMessage[1], DateTime.Parse(specialMessage[2]), ct);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}