using ClosedXML.Excel;
using IntDorSys.Core.Entities;
using IntDorSys.Core.Models;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Builders;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.Services.Audit;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Controllers.Base;
using IntDorSys.Web.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Controllers.Laundress
{
    [Route("laund")]
    [Authorize]
    public sealed class LaundressController : ProtectedApiController
    {

        [HttpGet]
        public Task<DataResult<List<PageLaundressModel>>> GetLaundAsync(
            [FromQuery] LaundressFilterModel filter,
            [FromServices] ILaundressBuilder builder)
        {
            return builder.BuildAsync(filter, HttpContext.RequestAborted);
        }

        [HttpGet("reports")]
        public async Task<DataResult<List<ReportModel>>> GetFilesAsync(
            [FromQuery] BaseFilterModel filter,
            [FromServices] ILaundReportService service)
        {
            return await service.GetReportAsync(filter, HttpContext.RequestAborted);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> CreateAsync(
            [FromBody] CreateLaundressRequest request,
            [FromServices] ILaundressService service,
            [FromServices] IAuditService audit)
        {
            var userId = UserId;
            var useLaundress = new UseLaundress
            {
                TimeWash = request.TimeWash,
                CreatedUserId = request.CreatedUserId,
            };
            var result = await service.CreateTimeAsync(useLaundress, HttpContext.RequestAborted);
            if (result.IsSuccess)
            {
                await audit.RecordAsync(userId, "CreateSlot", "UseLaundress",
                    request.TimeWash.ToString("O"), $"Created by user {request.CreatedUserId}");
            }
            return result;
        }

        [HttpPost("range")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<int>> CreateRangeAsync(
            [FromBody] CreateLaundressRangeRequest request,
            [FromServices] ILaundressService service,
            [FromServices] IAuditService audit)
        {
            var userId = UserId;
            var result = await service.CreateTimeRangeAsync(
                request.Date, request.StartHour, request.EndHour,
                request.CreatedUserId, HttpContext.RequestAborted);
            if (result.IsSuccess && result.Data > 0)
            {
                await audit.RecordAsync(userId, "CreateSlotRange", "UseLaundress",
                    $"{request.Date:O}", $"Slots {request.StartHour}:00-{request.EndHour}:00, created {result.Data}");
            }
            return result;
        }

        [HttpPost("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> BookAsync(
            [FromBody] BookLaundressRequest request,
            [FromServices] ILaundressService service,
            [FromServices] IAuditService audit)
        {
            var userId = UserId;
            var result = await service.UseTimeAsync(request.UserId, request.TimeWash, HttpContext.RequestAborted);
            if (result.IsSuccess)
            {
                await audit.RecordAsync(userId, "BookSlot", "UseLaundress",
                    request.TimeWash.ToString("O"), $"Booked user {request.UserId}");
            }
            return result;
        }

        [HttpDelete("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> UnbookAsync(
            [FromQuery] DateTime timeWash,
            [FromQuery] long userId,
            [FromServices] ILaundressService service,
            [FromServices] IAuditService audit)
        {
            var adminId = UserId;
            var result = await service.RemoveUseTimeAsync(userId, timeWash, true, HttpContext.RequestAborted);
            if (result.IsSuccess)
            {
                await audit.RecordAsync(adminId, "UnbookSlot", "UseLaundress",
                    timeWash.ToString("O"), $"Unbooked user {userId}");
            }
            return result;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> DeleteAsync(
            [FromQuery] DateTime timeWash,
            [FromServices] ILaundressService service,
            [FromServices] IAuditService audit)
        {
            var userId = UserId;
            var result = await service.RemoveTimeAsync(timeWash, HttpContext.RequestAborted);
            if (result.IsSuccess)
            {
                await audit.RecordAsync(userId, "DeleteSlot", "UseLaundress",
                    timeWash.ToString("O"), null);
            }
            return result;
        }

        [HttpGet("audit")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<List<AuditLogModel>>> GetAuditAsync(
            [FromServices] IAuditService audit,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            return await audit.GetLogsAsync(page, pageSize, HttpContext.RequestAborted);
        }

        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAsync(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromServices] ILaundressService service)
        {
            var filter = new LaundressFilterModel
            {
                StartDate = startDate ?? DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"),
                EndDate = endDate ?? DateTime.Today.ToString("yyyy-MM-dd"),
            };

            var records = await service.GetTimeByFilterAsync(filter, HttpContext.RequestAborted);
            if (!records.IsSuccess || records.Data == null)
                return BadRequest(records.GetErrorsString());

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
            stream.Position = 0;
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"laundress_export_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
