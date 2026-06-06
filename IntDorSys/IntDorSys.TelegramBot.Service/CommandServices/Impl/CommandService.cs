using IntDorSys.Core.Constants;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CommandServices.Impl
{
    internal sealed class CommandService : ICommandService
    {
        private readonly IBaseCommandsService _baseCommandsService;

        public CommandService(IBaseCommandsService baseCommandsService)
        {
            _baseCommandsService = baseCommandsService;
        }

        public Dictionary<string, Func<Message, CancellationToken, Task>> GetDictCommands()
        {
            return new Dictionary<string, Func<Message, CancellationToken, Task>>
            {
                [MessageKeyConstants.Start] = async (message, ct) =>
                    await _baseCommandsService.StartHandleAsync(message, ct),
                [MessageKeyConstants.Rules] = async (message, ct) =>
                    await _baseCommandsService.RulesHandleAsync(message, ct),
            };
        }
    }
}