using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;

namespace IntDorSys.Security.Services
{
    /// <summary>
    ///     Issues JWT tokens for authenticated users
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        ///     Generates a JWT token for the given user
        /// </summary>
        /// <param name="user">User entity to issue token for</param>
        /// <returns>Authentication token with access token and role</returns>
        Task<AuthToken> IssueTokenAsync(UserInfo user);
    }
}
