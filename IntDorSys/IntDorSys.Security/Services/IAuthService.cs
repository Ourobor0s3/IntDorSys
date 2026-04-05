using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services
{
    public interface IAuthService
    {
        /// <summary>
        ///     Registers new user
        /// </summary>
        /// <param name="registrationData">User data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> RegisterUserAsync(RegistrationData registrationData, CancellationToken ct);

        /// <summary>
        ///     Authenticates user
        /// </summary>
        /// <param name="authData">User auth data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<UserInfo>> AuthenticateUser(AuthData authData, CancellationToken ct);
    }
}