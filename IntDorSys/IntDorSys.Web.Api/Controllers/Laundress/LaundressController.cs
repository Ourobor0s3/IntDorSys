using IntDorSys.Core.Constants;
using IntDorSys.Core.Models;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services;
using IntDorSys.Services.Audit;
using IntDorSys.Services.Users;
using IntDorSys.Web.Api.Builders;
using IntDorSys.Web.Api.Controllers.Base;
using IntDorSys.Web.Api.ExportServices;
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
            [FromServices] ILaundressService service)
        {
            var useLaundress = new UseLaundress
            {
                TimeWash = request.TimeWash,
                CreatedUserId = request.CreatedUserId,
            };
            return await service.CreateTimeAsync(useLaundress, UserId, HttpContext.RequestAborted);
        }

        [HttpPost("range")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<int>> CreateRangeAsync(
            [FromBody] CreateLaundressRangeRequest request,
            [FromServices] ILaundressService service)
        {
            return await service.CreateTimeRangeAsync(
                request.Date, request.StartHour, request.EndHour,
                UserId, HttpContext.RequestAborted);
        }

        [HttpPost("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> BookAsync(
            [FromBody] BookLaundressRequest request,
            [FromServices] ILaundressService service)
        {
            return await service.UseTimeAsync(request.UserId, request.TimeWash, UserId, HttpContext.RequestAborted);
        }

        [HttpDelete("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> UnbookAsync(
            [FromQuery] DateTime timeWash,
            [FromQuery] long userId,
            [FromServices] ILaundressService service)
        {
            return await service.RemoveUseTimeAsync(userId, timeWash, true, UserId, HttpContext.RequestAborted);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> DeleteAsync(
            [FromQuery] DateTime timeWash,
            [FromServices] ILaundressService service)
        {
            return await service.RemoveTimeAsync(timeWash, UserId, HttpContext.RequestAborted);
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
                StartDate = startDate ?? DateTime.Today.AddMonths(-1).ToString(DateFormatConstants.DateFormat),
                EndDate = endDate ?? DateTime.Today.ToString(DateFormatConstants.DateFormat),
            };

            var bytes = await export.ExportToExcelAsync(filter, HttpContext.RequestAborted);
            if (bytes.Length == 0)
                return BadRequest("No data to export");

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"laundress_export_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}