using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Base.Models;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Laundress.Services.Controllers
{
    [Route("analitic")]
    public class AnaliticController : ApiController
    {
        [HttpGet("laund")]
        public async Task<DataResult<List<ChartDataModel<string, int>>>> GetLaundAsync(
            [FromServices] ILaundAnaliticService service)
        {
            var result = new DataResult<List<ChartDataModel<string, int>>>();
            var filter = new LaundressFilterModel
            {
                StartDate = DateTime.Today.AddMonths(-3).ToShortDateString(),
                EndDate = DateTime.Today.AddMonths(3).ToShortDateString(),
            };

            var laund = await service.GetTimeAnaliticAsync(filter, HttpContext.RequestAborted);
            return !laund.IsSuccess ? result.WithError("Not found") : result.WithData(laund.Data);
        }
    }
}