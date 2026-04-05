using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Core.Models
{
    /// <summary>
    /// A class for configuring the bot and handlers
    /// </summary>
    public class SettingsAndHandlers
    {
        /// <summary>
        /// Dictionary of teams with their handlers
        /// </summary>
        public required Dictionary<string, Func<Message, CancellationToken, Task>> CommandHandler { get; set; }

        /// <summary>
        /// А handler for message
        /// </summary>
        public required Func<Message, CancellationToken, Task> MessageHandler { get; set; }

        /// <summary>
        /// А handler for callback
        /// </summary>
        public required Func<CallbackQuery, CancellationToken, Task> CallbackHandler { get; set; }

        /// <summary>
        /// Function check authorization user for system
        /// </summary>
        public required Func<Update, CancellationToken, Task<bool>> AuthorizationUser { get; set; }
    }
}