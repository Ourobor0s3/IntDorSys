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

        /// <inheritdoc />
        public Dictionary<string, Func<Message, CancellationToken, Task>> GetDictCommands()
        {
            return new Dictionary<string, Func<Message, CancellationToken, Task>>
            {
                [MessageKeyConstants.Start] = _baseCommandsService.StartHandleAsync,
                [MessageKeyConstants.Rules] = _baseCommandsService.RulesHandleAsync,
            };
        }
    }
}