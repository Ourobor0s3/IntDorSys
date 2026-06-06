using IntDorSys.Core.Enums;

namespace IntDorSys.Security.Models
{
    public class PasswordChangeData
    {
        required public long UserId { get; set; }

        required public PasswordChangeType Type { get; set; }

        required public string NewPassword { get; set; }

        public string? OldPassword { get; set; }
    }
}