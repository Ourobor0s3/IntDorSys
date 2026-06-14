using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Settings;
using IntDorSys.Laundress.Core.Constants;
using IntDorSys.Laundress.Core.Models.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.TelegramBot.Core.Constants;
using Ouro.TelegramBot.Core.Models;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressBotMenuService : ILaundressBotMenuService
    {
        private readonly IUseLaundressQueryService _query;
        private readonly ITelegramService _telegramService;
        private readonly ILogger<LaundressBotMenuService> _logger;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public LaundressBotMenuService(
            IUseLaundressQueryService query,
            ITelegramService telegramService,
            ILogger<LaundressBotMenuService> logger,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _query = query;
            _telegramService = telegramService;
            _logger = logger;
            _adminSettings = adminSettings;
        }

        public async Task SendMenuAsync(UserInfo user, int messageId = 0, CancellationToken ct = default)
        {
            var mes = new BotResponceMessage
            {
                Message = "Меню прачечной:",
                InlineKeyboard = _adminSettings.CurrentValue.ManagersLaundress.Contains(user.TelegramId)
                    ? LaundressConstants.BtnLaundressAdm
                    : LaundressConstants.BtnLaundress,
            };

            if (messageId == 0)
            {
                await _telegramService.SendResponseMessageAsync(user.TelegramId, mes, ct);
            }
            else
            {
                await _telegramService.EditMessageTextAsync(user.TelegramId,
                    messageId,
                    mes,
                    ct);
            }
        }

        public async Task SendAllTimeAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                if (!_adminSettings.CurrentValue.ManagersLaundress.Contains(chatId))
                {
                    return;
                }

                var appointments = (await _query.GetTimeByFilterAsync(
                    new LaundressFilterModel
                    {
                        StartDate = DateTime.Today.ToString(DateFormatConstants.DateFormat),
                    },
                    ct)).Data;
                var message = "";

                if (appointments.Count > 0)
                {
                    var currentDate = appointments[0].TimeWash.Date;
                    message += $"----- Все записи -----\n<< {currentDate:dd.MM.yyyy} >>";
                    foreach (var laundress in appointments)
                    {
                        if (laundress.TimeWash.Date != currentDate)
                        {
                            currentDate = laundress.TimeWash.Date;
                            message += $"\n<< {currentDate:dd.MM.yyyy} >>";
                        }

                        message += $"\n * {laundress.TimeWash:HH:mm} - {laundress.SelectUser?.FullName}";
                    }
                }
                else
                {
                    message = MessageText.NoEntries;
                }

                var sendMessage = new BotResponceMessage
                {
                    Message = message,
                    InlineKeyboard = KeyboardButtons.InlineButton(
                        MessageKeyConstants.Back,
                        MessageKeyConstants.Menu,
                        MessageText.Back),
                };

                if (messageId != 0)
                {
                    await _telegramService.EditMessageTextAsync(chatId, messageId, sendMessage, ct);
                }
                else
                {
                    await _telegramService.SendResponseMessageAsync(chatId, sendMessage, ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in SendAdminAllTime method");
                throw;
            }
        }

        public async Task SendUseTimeByUserAsync(UserInfo user, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var appointments = (await _query.GetTimeByFilterAsync(new LaundressFilterModel
                {
                    StartDate = DateTime.Today.ToString(DateFormatConstants.DateFormat),
                    UserId = user.Id,
                },
                        ct)).Data
                    .Select(x => x.TimeWash);
                var inlineKeyboard = appointments.Select(x => new[]
                        { InlineKeyboardButton.WithCallbackData(x.ToString("dd.MM.yyyy HH:mm"), $"DeleteUserTime//{x:yyyy-MM-dd HH:mm}") })
                    .ToList();
                var message = inlineKeyboard.Count != 0
                    ? "Ваши записи (нажмите на дату, если хотите отменить):"
                    : "У вас нету активных записей";

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
                    await _telegramService.EditMessageTextAsync(user.TelegramId, messageId, sendMessage, ct);
                }
                else
                {
                    await _telegramService.SendResponseMessageAsync(user.TelegramId, sendMessage, ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in GetMyTimeLaundressAsync method");
                throw;
            }
        }

        public async Task SendDatesForDeleteAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var dates = (await _query.GetTimeByFilterAsync(new LaundressFilterModel
                {
                    IsOccupiedRecords = true,
                },
                        ct))
                    .Data
                    .Select(x => x.TimeWash.Date)
                    .Distinct()
                    .OrderBy(x => x);

                var inlineKeyboard = dates.Select(x => new[]
                        { KeyboardButtons.InlineButton("DelDate", x.ToString(DateFormatConstants.DateFormat), x.ToString("dd.MM.yyyy")) })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? "Выберите дату для удаления слота:"
                    : "Нет записей для удаления";

                inlineKeyboard.Add([
                    KeyboardButtons.InlineButton(MessageKeyConstants.Back, $"{MessageText.Back}", MessageText.Back),
                ]);
                var sendMessage = new BotResponceMessage
                {
                    Message = message,
                    InlineKeyboard = new InlineKeyboardMarkup(inlineKeyboard),
                };

                if (messageId != 0)
                {
                    await _telegramService.EditMessageTextAsync(chatId, messageId, sendMessage, ct);
                }
                else
                {
                    await _telegramService.SendResponseMessageAsync(chatId, sendMessage, ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in SendDatesForDeleteAsync method");
                throw;
            }
        }

        public async Task SendFreeDateAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var dates = (await _query.GetTimeByFilterAsync(new LaundressFilterModel
                {
                    StartDate = DateTime.Today.ToString(DateFormatConstants.DateFormat),
                    IsUnoccupiedRecords = true,
                },
                        ct))
                    .Data
                    .Select(x => x.TimeWash.Date)
                    .Distinct();

                var inlineKeyboard = dates.Select(x => new[]
                        { KeyboardButtons.InlineButton("UseDate", x.ToString(DateFormatConstants.DateFormat), x.ToString("dd.MM.yyyy")) })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? "Свободные для записи даты:"
                    : "Свободных дат нету";

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
                    await _telegramService.EditMessageTextAsync(chatId, messageId, sendMessage, ct);
                }
                else
                {
                    await _telegramService.SendResponseMessageAsync(chatId, sendMessage, ct);
                }
            }
            catch (Exception)
            {
                _logger.LogError("Error in GetAllFreeDateLaundressAsync method");
                throw;
            }
        }

        public async Task SendFreeTimeAsync(long chatId, int messageId, DateTime date, CancellationToken ct)
        {
            try
            {
                var times = (await _query.GetTimeByFilterAsync(new LaundressFilterModel
                {
                    IsUnoccupiedRecords = true,
                    SearchDate = date.Date,
                },
                        ct))
                    .Data
                    .Select(x => x.TimeWash);

                var inlineKeyboard = times.Select(x => new[]
                        { KeyboardButtons.InlineButton("UseTime", x.ToString("yyyy-MM-dd HH:mm"), x.ToString("HH:mm")) })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? $"Свободное для записи время на {date:dd.MM.yyyy}:"
                    : "Свободного времени нету";

                inlineKeyboard.Add([
                    KeyboardButtons.InlineButton("laund", $"{MessageText.AllFreeRecords}", MessageText.Back),
                ]);

                await _telegramService.EditMessageTextAsync(chatId,
                    messageId,
                    new BotResponceMessage
                    {
                        Message = message,
                        InlineKeyboard = new InlineKeyboardMarkup(inlineKeyboard),
                    },
                    ct);
            }
            catch (Exception)
            {
                _logger.LogError("Error in GetAllFreeTimeLaundressAsync method");
                throw;
            }
        }

        public async Task SendTimesForDeleteAsync(long chatId, int messageId, DateTime date, CancellationToken ct)
        {
            try
            {
                var times = (await _query.GetTimeByFilterAsync(new LaundressFilterModel
                {
                    SearchDate = date.Date,
                },
                        ct))
                    .Data
                    .Where(x => x.SelectUserId != null)
                    .OrderBy(x => x.TimeWash)
                    .ToList();

                var inlineKeyboard = times.Select(x => new[]
                        { KeyboardButtons.InlineButton("DelUseTime", x.TimeWash.ToString("yyyy-MM-dd HH:mm"), $"{x.TimeWash:dd.MM.yyyy} {x.TimeWash:HH:mm} ({GetShortName(x.SelectUser?.FullName)})") })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? $"Занятые записи на {date:dd.MM.yyyy}:"
                    : "Занятых записей нет";

                inlineKeyboard.Add([
                    KeyboardButtons.InlineButton(MessageKeyConstants.Back, MessageText.Back, MessageText.Back),
                ]);

                await _telegramService.EditMessageTextAsync(chatId,
                    messageId,
                    new BotResponceMessage
                    {
                        Message = message,
                        InlineKeyboard = new InlineKeyboardMarkup(inlineKeyboard),
                    },
                    ct);
            }
            catch (Exception)
            {
                _logger.LogError("Error in SendTimesForDeleteAsync method");
                throw;
            }
        }

        private static string GetShortName(string? fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "неизвестно";
            var parts = fullName.Split(' ');
            return parts.Length >= 2 ? $"{parts[0]} {parts[1][0]}." : parts[0];
        }
    }
}
