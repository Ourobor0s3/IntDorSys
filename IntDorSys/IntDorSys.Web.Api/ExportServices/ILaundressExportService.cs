using IntDorSys.Laundress.Core.Models.Filters;

namespace IntDorSys.Web.Api.ExportServices
{
    public interface ILaundressExportService
    {
        Task<byte[]> ExportToExcelAsync(LaundressFilterModel filter, CancellationToken ct);
    }
}