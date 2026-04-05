using IntDorSys.Laundress.Core.Models;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;
using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.Laundress.Services.Services
{
    public interface ILaundReportService
    {
        /// <summary>
        ///     Получение всех файлов по фильтру
        /// </summary>
        /// <param name="filter">Базовая модель фильтра</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task<DataResult<List<FileInfo>>> GetFilterAsync(BaseFilterModel filter, CancellationToken ct);

        /// <summary>
        ///     Сохранение всех фотографий, отправленных юзером
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task SavePhotoAsync(Message message, CancellationToken ct);

        Task SaveReportAsync(Message message, CancellationToken ct);

        Task<DataResult<List<ReportModel>>> GetReportAsync(BaseFilterModel filter, CancellationToken ct);
    }
}