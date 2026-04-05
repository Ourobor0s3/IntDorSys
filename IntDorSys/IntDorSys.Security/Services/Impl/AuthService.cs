using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using IntDorSys.Services.Users;
using Microsoft.AspNetCore.Identity;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services.Impl
{
    /// <summary>
    ///     Authorization and authentication service
    /// </summary>
    internal sealed class AuthService : IAuthService
    {
        private readonly IPasswordHasher<UserInfo> _passwordHasher;
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;

        /// <summary>
        ///     Initializes object of type <see cref="AuthService" />
        /// </summary>
        /// <param name="passwordHasher">Generates hash of password and validates this</param>
        /// <param name="userService">User service</param>
        /// <param name="passwordService">Password service</param>
        public AuthService(
            IPasswordHasher<UserInfo> passwordHasher,
            IUserService userService,
            IPasswordService passwordService)
        {
            _passwordHasher = passwordHasher;
            _userService = userService;
            _passwordService = passwordService;
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> RegisterUserAsync(
            RegistrationData registrationData,
            CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var newUser = new UserInfo
            {
                Email = registrationData.Email,
                Password = null!,
                Username = null!,
                TelegramId = registrationData.TelegramId,
                LanguageCode = "En",
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, registrationData.Password);

            var user = await _userService.CreateAsync(newUser, ct);

            return !user.IsSuccess
                ? result.WithErrors(user.Errors)
                : result.WithData(user.Data);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> AuthenticateUser(AuthData authData, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userResult = await _userService.GetByEmailAsync(authData.Login, ct);
            if (!userResult.IsSuccess)
            {
                return result.WithError("Incorrect email or password");
            }

            var user = userResult.Data;
            var hashValidationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, authData.Password);

            if (hashValidationResult == PasswordVerificationResult.Failed)
            {
                return result.WithError("Incorrect email or password");
            }

            return result.WithData(user);
        }
    }
}