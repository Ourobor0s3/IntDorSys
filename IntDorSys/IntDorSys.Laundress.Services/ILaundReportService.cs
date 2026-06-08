using IntDorSys.Laundress.Core.Models;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;
using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.Laundress.Services
{
    /// <summary>
    ///     Manages laundry report files and photo uploads from Telegram bot
    /// </summary>
    public interface ILaundReportService
    {
        /// <summary>
        ///     Returns files matching the given filter
        /// </summary>
        /// <param name="filter">Base filter model</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of file info models</returns>
        Task<DataResult<List<FileInfo>>> GetFilterAsync(BaseFilterModel filter, CancellationToken ct);

        /// <summary>
        ///     Saves photos sent by a user from a Telegram message
        /// </summary>
        /// <param name="message">Telegram message containing photos</param>
        /// <param name="ct">Cancellation token</param>
        Task SavePhotoAsync(Message message, CancellationToken ct);

        /// <summary>
        ///     Saves a report from a Telegram message
        /// </summary>
        /// <param name="message">Telegram message with report data</param>
        /// <param name="ct">Cancellation token</param>
        Task SaveReportAsync(Message message, CancellationToken ct);

        /// <summary>
        ///     Returns reports matching the given filter
        /// </summary>
        /// <param name="filter">Base filter model</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of report models</returns>
        Task<DataResult<List<ReportModel>>> GetReportAsync(BaseFilterModel filter, CancellationToken ct);
    }
}