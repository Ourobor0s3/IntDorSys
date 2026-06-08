using IntDorSys.Security.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Security.Services
{
    /// <summary>
    ///     Provides password change functionality
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        ///     Changes the user's password
        /// </summary>
        /// <param name="request">Old and new password data</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Success or failure result</returns>
        Task<Result> ChangePasswordAsync(PasswordChangeData request, CancellationToken ct);
    }
}