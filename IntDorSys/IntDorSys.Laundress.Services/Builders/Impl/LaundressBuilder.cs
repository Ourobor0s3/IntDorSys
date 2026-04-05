using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services.Services;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Laundress.Services.Builders.Impl
{
    internal sealed class LaundressBuilder : ILaundressBuilder
    {
        private readonly ILaundressService _service;

        public LaundressBuilder(ILaundressService service)
        {
            _service = service;
        }

        public LaundModel Build(UseLaundress laund)
        {
            var laundInfo = new LaundModel
            {
                SelectUser = laund.SelectUser,
                TimeWash = laund.TimeWash,
            };

            return laundInfo;
        }

        public async Task<DataResult<List<PageLaundressModel>>> BuildAsync(
            LaundressFilterModel filter,
            CancellationToken ct)
        {
            var result = new DataResult<List<PageLaundressModel>>();

            var laundResult = await _service.GetTimeByFilterAsync(filter, ct);
            var res = Build(laundResult.Data.Select(Build).ToList());

            return !laundResult.IsSuccess ? result.WithError("Not found") : result.WithData(res);
        }

        private List<PageLaundressModel> Build(List<LaundModel> launds)
        {
            var res = new List<PageLaundressModel>();
            var dateList = launds.Select(x => x.TimeWash.ToShortDateString()).Distinct().ToList();

            foreach (var date in dateList)
            {
                res.Add(new PageLaundressModel
                {
                    Date = date,
                    LaundModels = launds.Where(x => x.TimeWash.ToShortDateString().Equals(date)).ToList(),
                });
            }

            return res;
        }
    }
}