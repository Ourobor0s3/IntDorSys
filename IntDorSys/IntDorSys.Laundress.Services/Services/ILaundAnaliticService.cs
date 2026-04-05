using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Base.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Services
{
    public interface ILaundAnaliticService
    {
        Task<DataResult<List<ChartDataModel<string, int>>>> GetTimeAnaliticAsync(
            LaundressFilterModel filter,
            CancellationToken ct);
    }
}