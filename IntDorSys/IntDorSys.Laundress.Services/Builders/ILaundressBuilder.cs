using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Builders
{
    public interface ILaundressBuilder
    {
        public LaundModel Build(UseLaundress laund);
        public Task<DataResult<List<PageLaundressModel>>> BuildAsync(LaundressFilterModel filter, CancellationToken ct);
    }
}