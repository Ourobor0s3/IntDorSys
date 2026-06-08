namespace IntDorSys.Web.Api.Bot
{
    public sealed class BotStatus
    {
        public bool IsRunning { get; set; }
        public string? BotUsername { get; set; }
        public DateTime? LastStartedAt { get; set; }
    }
}