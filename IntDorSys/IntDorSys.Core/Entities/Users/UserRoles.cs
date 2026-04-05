using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Core.Entities.Users
{
    public class UserRoles : SoftDeletableEntity<long>
    {
        required public long UserId { get; init; }
        public UserInfo? User { get; init; }
        required public string KeyRoles { get; init; }
    }
}