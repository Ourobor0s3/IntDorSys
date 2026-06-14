using IntDorSys.Core.Constants;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers.Laundress
{
    [Route("analitic")]
    [Authorize]
    public class AnaliticController : ApiController
    {
        [HttpGet("laund")]
        public async Task<DataResult<List<ChartPointDto>>> GetLaundAsync(
            [FromServices] ILaundAnaliticService service)
        {
            var filter = new LaundressFilterModel
            {
                StartDate = DateTime.Today.AddMonths(-3).ToString(DateFormatConstants.DateFormat),
                EndDate = DateTime.Today.ToString(DateFormatConstants.DateFormat),
            };

            return await service.GetTimeAnaliticAsync(filter, HttpContext.RequestAborted);
        }
    }
}