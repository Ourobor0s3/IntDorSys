using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using IntDorSys.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services.Impl
{
    internal sealed class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _log;
        private readonly IPasswordHasher<UserInfo> _passwordHasher;
        private readonly IUserService _userService;

        public PasswordService(
            ILogger<PasswordService> log,
            IPasswordHasher<UserInfo> passwordHasher,
            IUserService userService)
        {
            _log = log;
            _passwordHasher = passwordHasher;
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task<Result> ChangePasswordAsync(PasswordChangeData request, CancellationToken ct)
        {
            var result = new DataResult<string>();

            var userResult = await _userService.GetAsync(request.UserId, ct);

            if (!userResult.IsSuccess)
            {
                return result.WithErrors(userResult.Errors);
            }

            var user = userResult.Data;

            var hashValidationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.OldPassword ?? string.Empty);

            if (hashValidationResult == PasswordVerificationResult.Failed)
            {
                _log.LogError(
                    "PasswordService.ChangePasswordAsync error => Password validation failed. [User id: '{UserId}']",
                    request.UserId);
                return result.WithError("Old password incorrect");
            }

            var hash = _passwordHasher.HashPassword(user, request.NewPassword);

            var updateResult = await _userService.UpdatePasswordAsync(request.UserId, hash, ct);

            if (!updateResult.IsSuccess)
            {
                return result.WithErrors(updateResult.Errors);
            }

            _log.LogInformation("Successful password change. UserId = {UserId};", request.UserId);

            return result;
        }

    }
}