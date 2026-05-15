using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Ouro.CommonUtils.Results;
using Ouro.WebApiUtils;

namespace IntDorSys.Web.Api.Controllers.Laundress
{
    [Route("analitic")]
    public class AnaliticController : ApiController
    {
        [HttpGet("laund")]
        public async Task<DataResult<List<ChartPointDto>>> GetLaundAsync(
            [FromServices] ILaundAnaliticService service)
        {
            var result = new DataResult<List<ChartPointDto>>();
            var filter = new LaundressFilterModel
            {
                StartDate = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd"),
                EndDate = DateTime.Today.ToString("yyyy-MM-dd"),
            };

            var laund = await service.GetTimeAnaliticAsync(filter, HttpContext.RequestAborted);
            if (!laund.IsSuccess)
            {
                return result.WithError("Not found");
            }

            var dto = laund.Data.Select(x => new ChartPointDto
            {
                Name = x.Name,
                Value1 = x.Value1,
                Value2 = x.Value2,
            }).ToList();

            return result.WithData(dto);
        }
    }
}
