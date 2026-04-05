using IntDorSys.Core.Constants;
using Microsoft.Extensions.Logging;
using Ouro.QuartzCore.Jobs;
using Ouro.TelegramBot.Core.Services;
using Quartz;

namespace IntDorSys.Laundress.Services.Jobs
{
    public class CheckWorkJob : BaseJob
    {
        private readonly ITelegramService _botService;

        public CheckWorkJob(
            ILogger<CheckWorkJob> logger,
            ITelegramService botService)
            : base(logger)
        {
            _botService = botService;
        }

        protected override async Task InnerExecuteAsync(IJobExecutionContext context)
        {
            await _botService.SendMessageAsync(AdminConstants.ManagersLaundress, "I'm working!", context.CancellationToken);
        }
    }
}