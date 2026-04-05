using IntDorSys.Security.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services
{
    public interface IPasswordService
    {
        /// <summary>
        ///     Sets new password to the user
        /// </summary>
        /// <param name="request">Password data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<Result> ChangePasswordAsync(PasswordChangeData request, CancellationToken ct);
    }
}