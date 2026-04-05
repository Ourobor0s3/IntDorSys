using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Models;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Builders
{
    public interface IUserInfoBuilder
    {
        UserInfo Build(
            User user,
            string fullName = "",
            string numberGroup = "",
            string numberRoom = "",
            string email = "",
            string address = "",
            string phoneNumber = "");

        UserInfoModel Build(UserInfo userInfo);
        public Task<DataResult<List<UserInfoModel>>> BuildAsync(CancellationToken ct);
    }
}