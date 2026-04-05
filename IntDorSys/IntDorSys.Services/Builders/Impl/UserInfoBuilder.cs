using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.Users;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Builders.Impl
{
    internal sealed class UserInfoBuilder : IUserInfoBuilder
    {
        private readonly IUserService _service;

        public UserInfoBuilder(IUserService service)
        {
            _service = service;
        }

        public UserInfo Build(
            User user,
            string fullName = "",
            string numberGroup = "",
            string numberRoom = "",
            string email = "",
            string address = "",
            string phoneNumber = "")
        {
            var userInfo = new UserInfo
            {
                FullName = fullName,
                NumGroup = numberGroup,
                NumRoom = numberRoom,
                Email = email,
                Password = "",
                Address = address,
                PhoneNumber = phoneNumber,
                Username = GetUsername(user),
                TelegramId = user.Id,
                LanguageCode = user.LanguageCode,
                IsBot = user.IsBot,
            };

            return userInfo;
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
            var res = users.Data.Select(Build).ToList();

            return !users.IsSuccess ? result.WithError("Not found") : result.WithData(res);
        }

        public string GetUsername(User user)
        {
            return user.Username != null ? "@" + user.Username : $"{user.Id}";
        }
    }
}