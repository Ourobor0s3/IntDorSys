using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Builders
{
    public interface ILaundressBuilder
    {
        LaundModel Build(UseLaundress laund);

        Task<DataResult<List<PageLaundressModel>>> BuildAsync(LaundressFilterModel filter, CancellationToken ct);
    }
}