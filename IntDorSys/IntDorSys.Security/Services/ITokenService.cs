using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;

namespace IntDorSys.Security.Services
{
    public interface ITokenService
    {
        AuthToken IssueToken(UserInfo user);
    }
}