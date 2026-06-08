using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services
{
    /// <summary>Read-only queries for UseLaundress (laundry slot) data.</summary>
    public interface IUseLaundressQueryService
    {
        /// <summary>Returns laundry slots matching the given filter criteria.</summary>
        Task<DataResult<List<UseLaundress>>> GetTimeByFilterAsync(
            LaundressFilterModel? filter = null,
            CancellationToken ct = default);
    }
}