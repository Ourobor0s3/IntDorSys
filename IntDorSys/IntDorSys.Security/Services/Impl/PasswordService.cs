using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using IntDorSys.Security.Models;
using IntDorSys.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services.Impl
{
    internal class PasswordService : IPasswordService
    {
        private readonly AppDataContext _db;
        private readonly ILogger<PasswordService> _log;
        private readonly IPasswordHasher<UserInfo> _passwordHasher;
        private readonly IUserService _userService;

        public PasswordService(
            ILogger<PasswordService> log,
            AppDataContext db,
            IPasswordHasher<UserInfo> passwordHasher,
            IUserService userService)
        {
            _log = log;
            _db = db;
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

            var useBeforeResult = await CheckPasswordUsageAsync(request, ct);

            if (!useBeforeResult.IsSuccess)
            {
                return result.WithErrors(useBeforeResult.Errors);
            }

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

            user.Password = _passwordHasher.HashPassword(user, request.NewPassword);

            request.NewPassword = user.Password;

            _db.AddOrUpdateEntity(user);
            await _db.SaveChangesAsync(ct);

            _log.LogInformation("Successful password change. UserId = {UserId};", request.UserId);

            return result;
        }

        /// <inheritdoc />
        private async Task<Result> CheckPasswordUsageAsync(PasswordChangeData request, CancellationToken ct)
        {
            var result = new Result();

            var user = await _userService.GetAsync(request.UserId, ct);

            return !user.IsSuccess ? result.WithErrors(user.Errors) : result;
        }
    }
}