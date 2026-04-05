using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Builders;
using IntDorSys.Laundress.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Laundress.Services.Controllers
{
    [Route("laund")]
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
    }
}