using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services
{
    /// <summary>
    ///     Provides analytics data for laundry usage charts
    /// </summary>
    public interface ILaundAnaliticService
    {
        /// <summary>
        ///     Returns chart data for laundry usage within a filter window
        /// </summary>
        /// <param name="filter">Filter with date range</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Chart data with label-value pairs</returns>
        Task<DataResult<List<ChartPointDto>>> GetTimeAnaliticAsync(
            LaundressFilterModel filter,
            CancellationToken ct);
    }
}
