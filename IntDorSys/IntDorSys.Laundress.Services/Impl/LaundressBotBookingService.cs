using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Settings;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.CommonUtils.Results;
using Ouro.TelegramBot.Core.Services;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressBotBookingService : ILaundressBotBookingService
    {
        private readonly AppDataContext _db;
        private readonly ILaundressService _laund;
        private readonly ITelegramService _telegramService;
        private readonly ILogger<LaundressBotBookingService> _logger;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;
        private readonly IAuditService _audit;

        private const int SlotIntervalHours = 2;

        public LaundressBotBookingService(
            ILaundressService laund,
            ITelegramService telegramService,
            ILogger<LaundressBotBookingService> logger,
            AppDataContext db,
            IOptionsMonitor<AdminSettings> adminSettings,
            IAuditService audit)
        {
            _laund = laund;
            _telegramService = telegramService;
            _logger = logger;
            _db = db;
            _adminSettings = adminSettings;
            _audit = audit;
        }

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
                    .Where(i => i % SlotIntervalHours == 0)
                    .Select(i =>
                    {
                        var dt = DateTime.Parse($"{date} {i}:00");
                        return new UseLaundress
                        {
                            CreatedUserId = crUser.Id,
                            TimeWash = dt,
                        };
                    })
                    .ToList();

                var mess = "Time:";
                foreach (var appointment in appointments)
                {
                    var res = await _laund.CreateTimeAsync(appointment, ct);
                    mess += res.IsSuccess
                        ? $"\n* {appointment.TimeWash:dd.MM.yyyy HH:mm} created"
                        : $"\n* {appointment.TimeWash:dd.MM.yyyy HH:mm} not created";
                }

                await _audit.RecordAsync(crUser.Id, "CreateSlotRange", "UseLaundress",
                    $"{date} {start}:00-{end}:00", $"Slots created: {appointments.Count}");
                _logger.LogInformation("User_id {user} create: {mess}.", crUser.Id, mess);
                await _telegramService.SendMessageAsync(crUser.TelegramId, mess, ct);
            }
            catch (Exception)
            {
                _logger.LogError("Error in CreateTimeLaundressAsync method");
                throw;
            }
        }

        public async Task DeleteLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct)
        {
            try
            {
                var res = await _laund.RemoveTimeAsync(dateTime, ct);
                if (res.IsSuccess)
                {
                    await _audit.RecordAsync(user.Id, "DeleteSlot", "UseLaundress", dateTime.ToString("O"));
                    _logger.LogInformation("User_id: {user} delete time: {time}", user.Id, dateTime);
                    await _telegramService.SendMessageAsync(user.TelegramId, $"{dateTime:dd.MM.yyyy HH:mm} удалено", ct);
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
                    await _telegramService.SendMessageAsync(user.TelegramId, $"{dateTime:dd.MM.yyyy HH:mm} отменено", ct);
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

                await _telegramService.SendMessageAsync(
                    _adminSettings.CurrentValue.ManagersLaundress,
                    $"{user.FullName} записался на {time:dd.MM.yyyy HH:mm}",
                    ct);

                _logger.LogInformation("User_id: {user} use time: {time}", user.Id, time);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Вы записаны на {time:dd.MM.yyyy HH:mm}", ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

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
                    $"{user.FullName} отменил запись на {time:dd.MM.yyyy HH:mm}",
                    ct);

                _logger.LogInformation("User_id: {user} remove use time: {time}", user.Id, time);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Запись на {time:dd.MM.yyyy HH:mm} отменена", ct);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in attempt to Get edit domain", ex);
                throw;
            }
        }

        public async Task DelUseTimeByAdminAsync(UserInfo user, DateTime time, CancellationToken ct)
        {
            try
            {
                var res = await _laund.RemoveUseTimeAsync(user.Id, time, true, ct);
                if (res.IsSuccess)
                {
                    await _audit.RecordAsync(user.Id, "UnbookSlot", "UseLaundress", time.ToString("O"), "Admin-forced unbook");
                    _logger.LogInformation("Admin {user} delete use time: {time}", user.Id, time);
                    await _telegramService.SendMessageAsync(user.TelegramId, $"Запись на {time:dd.MM.yyyy HH:mm} удалена", ct);
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

        public async Task AddUserToTimeAsync(UserInfo admin, string userName, DateTime time, CancellationToken ct)
        {
            try
            {
                var user = await _db.Set<UserInfo>()
                    .FirstOrDefaultAsync(x =>
                        (x.FullName != null && x.FullName.Contains(userName)) ||
                        (x.Username != null && x.Username.Contains(userName)), ct);

                if (user == null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Пользователь '{userName}' не найден", ct);
                    return;
                }

                var wash = await _db.Set<UseLaundress>()
                    .Include(x => x.SelectUser)
                    .FirstOrDefaultAsync(x => x.TimeWash == time, ct);

                if (wash == null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Слот на {time:dd.MM.yyyy HH:mm} не найден", ct);
                    return;
                }

                if (wash.SelectUserId != null)
                {
                    await _telegramService.SendMessageAsync(admin.TelegramId, $"Слот на {time:dd.MM.yyyy HH:mm} уже занят ({wash.SelectUser?.FullName})", ct);
                    return;
                }

                wash.SelectUser = user;
                await _db.SaveChangesAsync(ct);

                await _audit.RecordAsync(admin.Id, "BookSlot", "UseLaundress", time.ToString("O"), $"Booked user {user.Id} ({user.FullName})");
                await _telegramService.SendMessageAsync(admin.TelegramId, $"{user.FullName} записан на {time:dd.MM.yyyy HH:mm}", ct);
                await _telegramService.SendMessageAsync(user.TelegramId, $"Администратор записал тебя на {time:dd.MM.yyyy HH:mm}", ct);
                _logger.LogInformation("Admin {admin} booked {user} on {time}", admin.Id, user.Id, time);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}, Error in AddUserToTimeAsync", ex);
                throw;
            }
        }
    }
}
