using ClosedXML.Excel;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services;

namespace IntDorSys.Web.Api.ExportServices.Impl
{
    public sealed class LaundressExportService : ILaundressExportService
    {
        private readonly IUseLaundressQueryService _query;

        public LaundressExportService(IUseLaundressQueryService query)
        {
            _query = query;
        }

        public async Task<byte[]> ExportToExcelAsync(LaundressFilterModel filter, CancellationToken ct)
        {
            var records = await _query.GetTimeByFilterAsync(filter, ct);
            if (!records.IsSuccess || records.Data == null)
            {
                return [];
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Export");
            ws.Cell(1, 1).Value = "Date";
            ws.Cell(1, 2).Value = "Time";
            ws.Cell(1, 3).Value = "User";
            ws.Cell(1, 4).Value = "Username";
            ws.Cell(1, 5).Value = "Room";
            ws.Cell(1, 6).Value = "Status";
            var headerRange = ws.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;

            var row = 2;
            foreach (var r in records.Data)
            {
                var user = r.SelectUser;
                ws.Cell(row, 1).Value = r.TimeWash.ToString("yyyy-MM-dd");
                ws.Cell(row, 2).Value = r.TimeWash.ToString("HH:mm");
                ws.Cell(row, 3).Value = user?.FullName ?? "";
                ws.Cell(row, 4).Value = user?.Username ?? "";
                ws.Cell(row, 5).Value = user?.NumRoom ?? "";
                ws.Cell(row, 6).Value = user != null ? "Occupied" : "Free";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
