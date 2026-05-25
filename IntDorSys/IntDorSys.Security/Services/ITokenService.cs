using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;

namespace IntDorSys.Security.Services
{
    public interface ITokenService
    {
        Task<AuthToken> IssueTokenAsync(UserInfo user);
    }
}