using IntDorSys.Core.Entities.Users;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize
{
    public interface IBotRegistrationService
    {
        Task<bool> TryRegisterAsync(UserInfo userInfo, Update update, CancellationToken ct);
    }
}
