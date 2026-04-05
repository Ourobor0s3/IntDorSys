using IntDorSys.TelegramBot.Core.Services;
using Microsoft.Extensions.Hosting;

namespace IntDorSys.TelegramBot.Core
{
    public class BotHostService : IHostedService
    {
        private readonly IBotHandler _botHandler;

        public BotHostService(IBotHandler botHandler)
        {
            _botHandler = botHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _botHandler.StartBot();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}