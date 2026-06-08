using IntDorSys.Core.Entities.Users;
using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Core.Entities
{
    public sealed class AuditLog : SoftDeletableEntity<long>
    {
        required public long UserId { get; set; }
        public UserInfo? User { get; set; }

        required public string Action { get; set; }

        required public string EntityName { get; set; }

        public string? EntityId { get; set; }

        public string? Details { get; set; }
    }
}