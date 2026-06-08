using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.Authorize
{
    /// <summary>
    ///     Authenticates Telegram users against the system
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        ///     Validates whether the Telegram update comes from an authorized user
        /// </summary>
        /// <param name="update">Telegram update</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if the user is authorized</returns>
        Task<bool> AuthUser(Update update, CancellationToken ct);
    }
}