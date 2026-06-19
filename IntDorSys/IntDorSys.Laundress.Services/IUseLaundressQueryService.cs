using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services
{
    /// <summary>Read-only queries for UseLaundress (laundry slot) data.</summary>
    public interface IUseLaundressQueryService
    {
        /// <summary>Returns laundry slots matching the given filter criteria.</summary>
        /// <param name="filter">Filter model with date range, user, and occupancy options</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of laundry slot entities</returns>
        Task<DataResult<List<UseLaundress>>> GetTimeByFilterAsync(
            LaundressFilterModel? filter = null,
            CancellationToken ct = default);
    }
}