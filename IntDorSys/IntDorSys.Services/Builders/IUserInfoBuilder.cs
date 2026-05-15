using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Builders
{
    public interface IUserInfoBuilder
    {
        UserInfoModel Build(UserInfo userInfo);
        Task<DataResult<List<UserInfoModel>>> BuildAsync(CancellationToken ct);
    }
}