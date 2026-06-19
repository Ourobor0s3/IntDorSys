using IntDorSys.Core.Constants;
using IntDorSys.Core.Models;
using IntDorSys.Laundress.Core.Entities;
using IntDorSys.Laundress.Core.Models;
using IntDorSys.Laundress.Core.Models.Filters;
using IntDorSys.Laundress.Services;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Builders.Impl
{
    internal sealed class LaundressBuilder : ILaundressBuilder
    {
        private readonly IUseLaundressQueryService _query;

        public LaundressBuilder(IUseLaundressQueryService query)
        {
            _query = query;
        }

        public LaundModel Build(UseLaundress laund)
        {
            var laundInfo = new LaundModel
            {
                SelectUser = laund.SelectUser == null ? null : new UserInfoModel
                {
                    Id = laund.SelectUser.Id,
                    FullName = laund.SelectUser.FullName,
                    Username = laund.SelectUser.Username,
                    NumGroup = laund.SelectUser.NumGroup,
                    NumRoom = laund.SelectUser.NumRoom,
                },
                TimeWash = laund.TimeWash,
            };

            return laundInfo;
        }

        public async Task<DataResult<List<PageLaundressModel>>> BuildAsync(
            LaundressFilterModel filter,
            CancellationToken ct)
        {
            var result = new DataResult<List<PageLaundressModel>>();

            var laundResult = await _query.GetTimeByFilterAsync(filter, ct);
            var res = Build(laundResult.Data.Select(Build).ToList());

            return !laundResult.IsSuccess ? result.WithError("Not found") : result.WithData(res);
        }

        private List<PageLaundressModel> Build(List<LaundModel> launds)
        {
            return launds
                .GroupBy(x => x.TimeWash.ToString(DateFormatConstants.DisplayDateFormat))
                .Select(g => new PageLaundressModel
                {
                    Date = g.Key,
                    LaundModels = g.ToList(),
                })
                .ToList();
        }
    }
}