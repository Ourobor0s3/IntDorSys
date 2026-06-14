using IntDorSys.DataAccess;
using IntDorSys.Laundress.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Results;
using Ouro.TelegramBot.Core.Services;

namespace IntDorSys.Laundress.Services.Impl
{
    internal sealed class LaundressBotNotificationService : ILaundressBotNotificationService
    {
        private readonly AppDataContext _db;
        private readonly ITelegramService _telegramService;
        private readonly ILogger<LaundressBotNotificationService> _logger;

        private static readonly TimeSpan NotificationLeadHours = TimeSpan.FromHours(-3);
        private static readonly TimeSpan NotificationFollowUpHours = TimeSpan.FromHours(-2);

        public LaundressBotNotificationService(
            AppDataContext db,
            ITelegramService telegramService,
            ILogger<LaundressBotNotificationService> logger)
        {
            _db = db;
            _telegramService = telegramService;
            _logger = logger;
        }

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
                        $"Ты записался(-ась) на завтра на {time.TimeWash:HH:mm}",
                        ct);

                    time.IsSendDay = true;
                    await _db.SaveChangesAsync(ct);
                }

                var usersWashHours = await _db.Set<UseLaundress>()
                    .Include(x => x.SelectUser)
                    .Where(x => x.SelectUserId != null)
                    .Where(x => x.TimeWash.Add(NotificationLeadHours) >= DateTime.Now && x.TimeWash.Add(NotificationFollowUpHours) < DateTime.Now)
                    .Where(x => !x.IsSendHours)
                    .ToListAsync(ct);

                foreach (var time in usersWashHours)
                {
                    _logger.LogInformation("Send user_id: {user_id}, notification about wash on today at {time}",
                        time.SelectUser!.Id,
                        time.TimeWash);
                    await _telegramService.SendMessageAsync(
                        time.SelectUser!.TelegramId,
                        $"У тебя запись на {time.TimeWash:HH:mm}",
                        ct);

                    time.IsSendHours = true;
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
