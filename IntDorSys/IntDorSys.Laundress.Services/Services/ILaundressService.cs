using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Services
{
    public interface ILaundressService
    {
        /// <summary>
        ///     Create wash time by model
        /// </summary>
        /// <param name="newWash">Use laundress model</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<bool>> CreateTimeAsync(UseLaundress newWash, CancellationToken ct);

        /// <summary>
        ///     Delete wash time by time
        /// </summary>
        /// <param name="timeWash">Wash time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<bool>> RemoveTimeAsync(DateTime timeWash, CancellationToken ct);

        /// <summary>
        ///     Get wash time by filter
        /// </summary>
        /// <param name="filter">Laundress filter model</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<List<UseLaundress>>> GetTimeByFilterAsync(
            LaundressFilterModel? filter = null,
            CancellationToken ct = default);

        /// <summary>
        ///     Remove time user by user id and wash time
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="timeWash">Wash time</param>
        /// <param name="isAdmin">Is admin</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<bool>> RemoveUseTimeAsync(
            long userId,
            DateTime timeWash,
            bool isAdmin = false,
            CancellationToken ct = default);

        /// <summary>
        ///     Use time user by user id and wash time
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="timeWash">Wash time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<bool>> UseTimeAsync(long userId, DateTime timeWash, CancellationToken ct);
    }
}