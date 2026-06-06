using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services
{
    /// <summary>
    ///     Provides user registration and authentication
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        ///     Registers a new user account
        /// </summary>
        /// <param name="registrationData">Registration details</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Created user info on success</returns>
        Task<DataResult<UserInfo>> RegisterUserAsync(RegistrationData registrationData, CancellationToken ct);

        /// <summary>
        ///     Authenticates user credentials and returns user info
        /// </summary>
        /// <param name="authData">Authentication credentials</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Authenticated user info on success</returns>
        Task<DataResult<UserInfo>> AuthenticateUser(AuthData authData, CancellationToken ct);
    }
}
