using System.Globalization;
using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Settings;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Constants;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.CommonUtils.Results;
using Ouro.TelegramBot.Core.Constants;
using Ouro.TelegramBot.Core.Models;
using Ouro.TelegramBot.Core.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace IntDorSys.Laundress.Services.Services.Impl
{
    internal sealed class LaundressBotService : ILaundressBotService
    {
        private readonly AppDataContext _db;
        private readonly ILaundressService _laund;
        private readonly ILogger<LaundressBotService> _logger;
        private readonly ITelegramService _telegramService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public LaundressBotService(
            ILaundressService laund,
            ITelegramService telegramService,
            ILogger<LaundressBotService> logger,
            AppDataContext db,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _laund = laund;
            _telegramService = telegramService;
            _logger = logger;
            _db = db;
            _adminSettings = adminSettings;
        }

        /// <inheritdoc />
        public async Task SendMenu(UserInfo user, int messageId = 0, CancellationToken ct = default)
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

        /// <inheritdoc />
        public async Task CreateTimesAsync(
            UserInfo crUser,
            string date,
            int start,
            int end,
            CancellationToken ct)
        {
            try
            {
                var appointments = Enumerable.Range(start, end - start + 1)
                    .Where(i => i % 2 == 0)
                    .Select(i => new UseLaundress
                    {
                        CreatedUserId = crUser.Id,
                        CreatedUser = crUser,
                        TimeWash = DateTime.Parse($"{date} {i}:00"),
                    })
                    .ToList();

                var mess = "Time:";
                foreach (var appointment in appointments)
                {
                    var res = await _laund.CreateTimeAsync(appointment, ct);
                    mess += res.IsSuccess
                        ? $"\n* {appointment.TimeWash} created"
                        : $"\n* {appointment.TimeWash} not created";
                }

                _logger.LogInformation("User_id {user} create: {mess}.", crUser.Id, mess);
                await _telegramService.SendMessageAsync(crUser.TelegramId, mess, ct);
            }
            catch (Exception)
            {
                _logger.LogError("Error in CreateTimeLaundressAsync method");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task SendAllTimeAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                if (!_adminSettings.CurrentValue.ManagersLaundress.Contains(chatId))
                {
                    return;
                }

                var appointments = (await _laund.GetTimeByFilterAsync(
                    new LaundressFilterModel
                    {
                        StartDate = DateTime.Today.ToString(CultureInfo.CurrentCulture),
                    },
                    ct)).Data;
                var message = "";

                if (appointments.Count > 0)
                {
                    var currentDate = appointments[0].TimeWash.Date;
                    message += $"----- Все записи -----\n<< {currentDate.ToShortDateString()} >>";
                    foreach (var laundress in appointments)
                    {
                        if (laundress.TimeWash.Date != currentDate)
                        {
                            currentDate = laundress.TimeWash.Date;
                            message += $"\n<< {currentDate.ToShortDateString()} >>";
                        }

                        message += $"\n * {laundress.TimeWash.ToShortTimeString()} - {laundress.SelectUser?.FullName}";
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

        /// <inheritdoc />
        public async Task SendUseTimeByUserAsync(UserInfo user, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var appointments = (await _laund.GetTimeByFilterAsync(new LaundressFilterModel
                        {
                            StartDate = DateTime.Today.ToString(CultureInfo.CurrentCulture),
                            UserId = user.Id,
                        },
                        ct)).Data
                    .Select(x => x.TimeWash);
                var inlineKeyboard = appointments.Select(x => new[]
                    {
                        InlineKeyboardButton.WithCallbackData(x.ToString(CultureInfo.CurrentCulture),
                            $"DeleteUserTime//{x}"),
                    })
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

        /// <inheritdoc />
        public async Task SendDatesForDeleteAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var dates = (await _laund.GetTimeByFilterAsync(new LaundressFilterModel
                        {
                            IsOccupiedRecords = true,
                        },
                        ct))
                    .Data
                    .Select(x => x.TimeWash.Date)
                    .Distinct()
                    .OrderBy(x => x);

                var inlineKeyboard = dates.Select(x => new[]
                        { KeyboardButtons.InlineButton("DelDate", x.ToShortDateString(), x.ToLongDateString()) })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? "Выберите дату для удаления записи:"
                    : "Нет занятых записей";

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

        /// <inheritdoc />
        public async Task SendFreeDateAsync(long chatId, int messageId = 0, CancellationToken ct = default)
        {
            try
            {
                var dates = (await _laund.GetTimeByFilterAsync(new LaundressFilterModel
                        {
                            StartDate = DateTime.Today.ToString(CultureInfo.CurrentCulture),
                            IsUnoccupiedRecords = true,
                        },
                        ct))
                    .Data
                    .Select(x => x.TimeWash.Date)
                    .Distinct();

                var inlineKeyboard = dates.Select(x => new[]
                        { KeyboardButtons.InlineButton("UseDate", x.ToShortDateString(), x.ToLongDateString()) })
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

        /// <inheritdoc />
        public async Task SendFreeTimeAsync(long chatId, int messageId, DateTime date, CancellationToken ct)
        {
            try
            {
                var times = (await _laund.GetTimeByFilterAsync(new LaundressFilterModel
                        {
                            IsUnoccupiedRecords = true,
                            SearchDate = date.Date,
                        },
                        ct))
                    .Data
                    .Select(x => x.TimeWash);

                var inlineKeyboard = times.Select(x => new[]
                        { KeyboardButtons.InlineButton("UseTime", x.ToString(), x.ToShortTimeString()) })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? $"Свободное для записи время на {date.ToShortDateString()}:"
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

        /// <inheritdoc />
        public async Task DeleteLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct)
        {
            try
            {
                var res = await _laund.RemoveTimeAsync(dateTime, ct);
                if (res.IsSuccess)
                {
                    _logger.LogInformation("User_id: {user} delete time: {time}", user.Id, dateTime);
                    await _telegramService.SendMessageAsync(user.TelegramId, $"{dateTime} delete", ct);
                }
                else
                {
                    _logger.LogError("User_id: {user} attempt delete time: {time}, error: {error}",
                        user.Id,
                        dateTime,
                        res.GetErrorsString());
                    await _telegramService.SendMessageAsync(user.TelegramId, res.GetErrorsString(), ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UnUseLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct)
        {
            try
            {
                var res = await _laund.RemoveUseTimeAsync(
                    user.Id,
                    dateTime,
                    true,
                    ct);
                if (res.IsSuccess)
                {
                    _logger.LogInformation("User_id: {user} unuse time: {time}", user.Id, dateTime);
                    await _telegramService.SendMessageAsync(user.TelegramId, $"{dateTime} unuse", ct);
                }
                else
                {
                    _logger.LogError("User_id: {user} attempt unuse time: {time}, error: {error}",
                        user.Id,
                        dateTime,
                        res.GetErrorsString());
                    await _telegramService.SendMessageAsync(user.TelegramId, res.GetErrorsString(), ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UseTimeLaundByUserAsync(
            UserInfo user,
            DateTime time,
            int messageId = 0,
            CancellationToken ct = default)
        {
            try
            {
                var res = await _laund.UseTimeAsync(user.Id, time, ct);

                if (!res.IsSuccess)
                {
                    _logger.LogError("User_id: {user} use time: {time}, error: {error}",
                        user.Id,
                        time,
                        res.GetErrorsString());
                    await _telegramService.SendMessageAsync(user.TelegramId, res.GetErrorsString(), ct);
                    return;
                }

                if (messageId != 0)
                {
                    await SendFreeTimeAsync(user.TelegramId, messageId, time, ct);
                }

                await _telegramService.SendMessageAsync(
                    _adminSettings.CurrentValue.ManagersLaundress,
                    $"{user.FullName} use {time}",
                    ct);

                _logger.LogInformation("User_id: {user} use time: {time}", user.Id, time);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Вы записаны на {time}", ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RemoveTimeByUserAsync(UserInfo user, DateTime time, CancellationToken ct)
        {
            try
            {
                if (time < DateTime.Now)
                {
                    return;
                }

                var res = await _laund.RemoveUseTimeAsync(user.Id, time, ct: ct);
                if (!res.IsSuccess)
                {
                    _logger.LogError("User_id: {user} remove use time: {time}, error: {error}",
                        user.Id,
                        time,
                        res.GetErrorsString());
                    return;
                }

                await _telegramService.SendMessageAsync(_adminSettings.CurrentValue.ManagersLaundress,
                    $"{user.FullName} remove time on {time}",
                    ct);

                _logger.LogInformation("User_id: {user} remove use time: {time}", user.Id, time);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Запись на {time} отменена", ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task SendTimesForDeleteAsync(long chatId, int messageId, DateTime date, CancellationToken ct)
        {
            try
            {
                var times = (await _laund.GetTimeByFilterAsync(new LaundressFilterModel
                        {
                            SearchDate = date.Date,
                        },
                        ct))
                    .Data
                    .Where(x => x.SelectUserId != null)
                    .OrderBy(x => x.TimeWash)
                    .ToList();

                var inlineKeyboard = times.Select(x => new[]
                        { KeyboardButtons.InlineButton("DelUseTime", x.TimeWash.ToString(CultureInfo.CurrentCulture), $"{x.TimeWash.ToShortDateString()} {x.TimeWash.ToShortTimeString()} ({GetShortName(x.SelectUser?.FullName)})") })
                    .ToList();

                var message = inlineKeyboard.Count != 0
                    ? $"Занятые записи на {date.ToShortDateString()}:"
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

        /// <inheritdoc />
        public async Task DelUseTimeByAdminAsync(UserInfo user, DateTime time, CancellationToken ct)
        {
            try
            {
                var res = await _laund.RemoveUseTimeAsync(user.Id, time, true, ct);
                if (res.IsSuccess)
                {
                    _logger.LogInformation("Admin {user} delete use time: {time}", user.Id, time);
                    await _telegramService.SendMessageAsync(user.TelegramId, $"Запись на {time} удалена", ct);
                }
                else
                {
                    _logger.LogError("Admin {user} failed delete use time: {time}, error: {error}",
                        user.Id, time, res.GetErrorsString());
                    await _telegramService.SendMessageAsync(user.TelegramId, res.GetErrorsString(), ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in DelUseTimeByAdminAsync", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddUserToTimeAsync(UserInfo admin, string userName, DateTime time, CancellationToken ct)
        {
            try
            {
                var user = _db.Set<UserInfo>()
                    .FirstOrDefault(x =>
                        (x.FullName != null && x.FullName.Contains(userName)) ||
                        (x.Username != null && x.Username.Contains(userName)));

                if (user == null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Пользователь '{userName}' не найден", ct);
                    return;
                }

                var wash = _db.Set<UseLaundress>()
                    .Include(x => x.SelectUser)
                    .FirstOrDefault(x => x.TimeWash == time);

                if (wash == null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Слот на {time} не найден", ct);
                    return;
                }

                if (wash.SelectUserId != null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Слот на {time} уже занят ({wash.SelectUser?.FullName})", ct);
                    return;
                }

                wash.SelectUser = user;
                _db.AddOrUpdateEntity(wash);
                await _db.SaveChangesAsync(ct);

                await _telegramService.SendMessageAsync(admin.TelegramId, $"{user.FullName} записан на {time}", ct);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Администратор записал тебя на {time}", ct);
                _logger.LogInformation("Admin {admin} booked {user} on {time}", admin.Id, user.Id, time);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in AddUserToTimeAsync", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task CheckTimeAndSendNotifAsync(CancellationToken ct)
        {
            try
            {
                var usersWashDay = await _db.Set<UseLaundress>()
                    .Include(x => x.SelectUser)
                    .Where(x => x.SelectUserId != null)
                    .Where(x => x.TimeWash >= DateTime.Today.AddDays(1) && x.TimeWash < DateTime.Today.AddDays(2))
                    .Where(x => !x.IsSendDay)
                    .ToListAsync(ct);

                foreach (var time in usersWashDay)
                {
                    _logger.LogInformation("Send user_id: {user_id}, notification about wash on next day at {time}",
                        time.SelectUser!.Id,
                        time.TimeWash);
                    await _telegramService.SendMessageAsync(
                        time.SelectUser!.TelegramId,
                        $"Ты записался(-ась) на завтра на {time.TimeWash.ToShortTimeString()}",
                        ct);

                    time.IsSendDay = true;
                    _db.AddOrUpdateEntity(time);
                    await _db.SaveChangesAsync(ct);
                }

                var usersWashHours = await _db.Set<UseLaundress>()
                    .Include(x => x.SelectUser)
                    .Where(x => x.SelectUserId != null)
                    .Where(x => x.TimeWash.AddHours(-3) >= DateTime.Now && x.TimeWash.AddHours(-2) < DateTime.Now)
                    .Where(x => !x.IsSendHours)
                    .ToListAsync(ct);

                foreach (var time in usersWashHours)
                {
                    _logger.LogInformation("Send user_id: {user_id}, notification about wash on today at {time}",
                        time.SelectUser!.Id,
                        time.TimeWash);
                    await _telegramService.SendMessageAsync(
                        time.SelectUser!.TelegramId,
                        $"У тебя запись на {time.TimeWash.ToShortTimeString()}",
                        ct);

                    time.IsSendHours = true;
                    _db.AddOrUpdateEntity(time);
                    await _db.SaveChangesAsync(ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }
    }
}