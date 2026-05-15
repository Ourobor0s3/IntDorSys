using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.Users;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Builders.Impl
{
    internal sealed class UserInfoBuilder : IUserInfoBuilder
    {
        private readonly IUserService _service;

        public UserInfoBuilder(IUserService service)
        {
            _service = service;
        }

        public UserInfoModel Build(UserInfo userInfo)
        {
            var model = new UserInfoModel
            {
                Id = userInfo.Id,
                FullName = userInfo.FullName,
                NumGroup = userInfo.NumGroup,
                NumRoom = userInfo.NumRoom,
                Username = userInfo.Username,
                Status = userInfo.Status,
                RegisterDate = userInfo.CreatedAt.ToString("dd-MM-yyyy HH:mm"),
                IsBlocked = userInfo.Status == UserStatus.Blocked,
                IsConfirm = userInfo.IsConfirm,
            };

            return model;
        }

        public async Task<DataResult<List<UserInfoModel>>> BuildAsync(CancellationToken ct)
        {
            var result = new DataResult<List<UserInfoModel>>();

            var users = await _service.GetListUsersAsync(ct);

            if (!users.IsSuccess || users.Data is null)
            {
                return result.WithError("Not found");
            }

            var res = users.Data.Select(Build).ToList();
            return result.WithData(res);
        }

    }
}