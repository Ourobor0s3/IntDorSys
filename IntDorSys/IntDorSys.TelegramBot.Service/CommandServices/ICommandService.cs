using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices
{
    public interface ICommandService
    {
        Dictionary<string, Func<Message, CancellationToken, Task>> GetDictCommands();
    }
}