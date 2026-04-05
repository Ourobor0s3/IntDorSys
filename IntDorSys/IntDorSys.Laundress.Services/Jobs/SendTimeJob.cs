using IntDorSys.Laundress.Services.Services;
using Microsoft.Extensions.Logging;
using Ouro.QuartzCore.Jobs;
using Quartz;

namespace IntDorSys.Laundress.Services.Jobs
{
    public class SendTimeJob : BaseJob
    {
        private readonly ILaundressBotService _botService;

        public SendTimeJob(
            ILogger<SendTimeJob> logger,
            ILaundressBotService botService)
            : base(logger)
        {
            _botService = botService;
        }

        protected override async Task InnerExecuteAsync(IJobExecutionContext context)
        {
            Logger.LogInformation("Starting to send notification for time wash");
            await _botService.CheckTimeAndSendNotifAsync(context.CancellationToken);
            Logger.LogInformation("Completed sending notification for time wash");
        }
    }
}