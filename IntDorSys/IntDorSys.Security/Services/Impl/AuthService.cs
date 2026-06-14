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
        private readonly IUserCommandService _userCommand;
        private readonly IUserQueryService _userQuery;

        public AuthService(
            IPasswordHasher<UserInfo> passwordHasher,
            IUserQueryService userQuery,
            IUserCommandService userCommand,
            IPasswordService passwordService)
        {
            _passwordHasher = passwordHasher;
            _userQuery = userQuery;
            _userCommand = userCommand;
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
                Username = registrationData.Email.Split('@')[0],
                TelegramId = registrationData.TelegramId,
                LanguageCode = "En",
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, registrationData.Password);

            var user = await _userCommand.CreateAsync(newUser, ct);

            return !user.IsSuccess
                ? result.WithErrors(user.Errors)
                : result.WithData(user.Data);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> AuthenticateUser(AuthData authData, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            // Try to find user by email first
            var userResult = await _userQuery.GetByEmailAsync(authData.Login, ct);

            // If not found by email, try by Telegram ID
            if (!userResult.IsSuccess && long.TryParse(authData.Login, out var telegramId))
            {
                userResult = await _userQuery.GetByTgIdAsync(telegramId, ct);
            }

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