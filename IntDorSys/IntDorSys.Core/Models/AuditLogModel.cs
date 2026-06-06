namespace IntDorSys.Core.Models
{
    public sealed class AuditLogModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
