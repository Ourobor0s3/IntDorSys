using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ouro.QuartzCore.Jobs;
using Ouro.TelegramBot.Core.Services;
using Quartz;

namespace IntDorSys.Laundress.Services.Jobs
{
    public class CheckWorkJob : BaseJob
    {
        private readonly ITelegramService _botService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        public CheckWorkJob(
            ILogger<CheckWorkJob> logger,
            ITelegramService botService,
            IOptionsMonitor<AdminSettings> adminSettings)
            : base(logger)
        {
            _botService = botService;
            _adminSettings = adminSettings;
        }

        protected override async Task InnerExecuteAsync(IJobExecutionContext context)
        {
            await _botService.SendMessageAsync(_adminSettings.CurrentValue.ManagersLaundress, "I'm working!", context.CancellationToken);
        }
    }
}
