using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IntDorSys.Web.Api.Bot
{
    internal sealed class BotHealthCheck : IHealthCheck
    {
        private readonly BotStatus _botStatus;

        public BotHealthCheck(BotStatus botStatus)
        {
            _botStatus = botStatus;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken)
        {
            if (_botStatus.IsRunning)
            {
                var data = new Dictionary<string, object>
                {
                    { "bot_username", _botStatus.BotUsername ?? "unknown" },
                    { "last_started", _botStatus.LastStartedAt?.ToString("O") ?? "never" },
                };
                return Task.FromResult(HealthCheckResult.Healthy("Telegram bot is running", data));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Telegram bot is not running"));
        }
    }
}