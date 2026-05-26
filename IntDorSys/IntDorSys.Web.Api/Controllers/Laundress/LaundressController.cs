using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Builders;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.Web.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers.Laundress
{
    [Route("laund")]
    [Authorize]
    public sealed class LaundressController : ApiController
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
            return await service.CreateTimeAsync(useLaundress, HttpContext.RequestAborted);
        }

        [HttpPost("range")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<int>> CreateRangeAsync(
            [FromBody] CreateLaundressRangeRequest request,
            [FromServices] ILaundressService service)
        {
            return await service.CreateTimeRangeAsync(
                request.Date, request.StartHour, request.EndHour,
                request.CreatedUserId, HttpContext.RequestAborted);
        }

        [HttpPost("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> BookAsync(
            [FromBody] BookLaundressRequest request,
            [FromServices] ILaundressService service)
        {
            return await service.UseTimeAsync(request.UserId, request.TimeWash, HttpContext.RequestAborted);
        }

        [HttpDelete("book")]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> UnbookAsync(
            [FromQuery] DateTime timeWash,
            [FromQuery] long userId,
            [FromServices] ILaundressService service)
        {
            return await service.RemoveUseTimeAsync(userId, timeWash, true, HttpContext.RequestAborted);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<DataResult<bool>> DeleteAsync(
            [FromQuery] DateTime timeWash,
            [FromServices] ILaundressService service)
        {
            return await service.RemoveTimeAsync(timeWash, HttpContext.RequestAborted);
        }
    }
}
