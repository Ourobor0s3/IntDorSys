using IntDorSys.Core.Entities;
using IntDorSys.Core.Models;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services;
using IntDorSys.Web.Api.ExportServices;
using IntDorSys.Services.Audit;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Builders;
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
        public Task<DataResult<List<AuditLogModel>>> GetAuditAsync(
            [FromServices] IAuditService audit,
            [FromQuery] BaseFilterModel filter)
        {
            return audit.GetLogsAsync(filter, HttpContext.RequestAborted);
        }

        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAsync(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromServices] ILaundressExportService export)
        {
            var filter = new LaundressFilterModel
            {
                StartDate = startDate ?? DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd"),
                EndDate = endDate ?? DateTime.Today.ToString("yyyy-MM-dd"),
            };

            var bytes = await export.ExportToExcelAsync(filter, HttpContext.RequestAborted);
            if (bytes.Length == 0)
                return BadRequest("No data to export");

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"laundress_export_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
