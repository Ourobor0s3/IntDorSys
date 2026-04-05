using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize
{
    public interface IAuthService
    {
        Task<bool> AuthUser(Update update, CancellationToken ct);
    }
}