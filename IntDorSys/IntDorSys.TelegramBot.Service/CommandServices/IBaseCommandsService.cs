using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices
{
    public interface IBaseCommandsService
    {
        Task StartHandleAsync(Message message, CancellationToken ct);
        Task RulesHandleAsync(Message message, CancellationToken ct);
    }
}