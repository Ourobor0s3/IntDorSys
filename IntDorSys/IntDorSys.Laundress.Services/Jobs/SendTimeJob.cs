using IntDorSys.Laundress.Services;
using Microsoft.Extensions.Logging;
using Ouro.QuartzCore.Jobs;
using Quartz;

namespace IntDorSys.Laundress.Services.Jobs
{
    public class SendTimeJob : BaseJob
    {
        private readonly ILaundressBotNotificationService _notification;

        public SendTimeJob(
            ILogger<SendTimeJob> logger,
            ILaundressBotNotificationService notification)
            : base(logger)
        {
            _notification = notification;
        }

        protected override async Task InnerExecuteAsync(IJobExecutionContext context)
        {
            Logger.LogInformation("Starting to send notification for time wash");
            await _notification.CheckTimeAndSendNotifAsync(context.CancellationToken);
            Logger.LogInformation("Completed sending notification for time wash");
        }
    }
}