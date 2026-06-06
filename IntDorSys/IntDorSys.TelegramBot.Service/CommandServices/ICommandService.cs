using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices
{
    /// <summary>
    ///     Registers and serves all Telegram bot command handlers
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        ///     Returns a dictionary mapping command names to their handler functions
        /// </summary>
        /// <returns>Command name to handler delegate map</returns>
        Dictionary<string, Func<Message, CancellationToken, Task>> GetDictCommands();
    }
}
