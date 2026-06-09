namespace IntDorSys.Core.Models
{
    public sealed class BotStatusDto
    {
        public bool Running { get; set; }
        public string? Username { get; set; }
        public DateTime? LastStarted { get; set; }
    }
}