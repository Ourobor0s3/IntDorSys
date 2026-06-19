using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.Core.Models;
using IntDorSys.Services.AppSettings;
using IntDorSys.Services.Statistics;
using IntDorSys.Services.Users;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Web.Api.Builders.Impl
{
    internal sealed class UserInfoBuilder : IUserInfoBuilder
    {
        private readonly IUserQueryService _service;
        private readonly IUsageStatsService _usageStats;
        private readonly IUserRoleService _roleService;
        private readonly IAppSettingService _settings;

        public UserInfoBuilder(IUserQueryService service, IUsageStatsService usageStats, IUserRoleService roleService, IAppSettingService settings)
        {
            _service = service;
            _usageStats = usageStats;
            _roleService = roleService;
            _settings = settings;
        }

        public UserInfoModel Build(UserInfo userInfo)
        {
            return new UserInfoModel
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
        }

        public async Task<DataResult<List<UserInfoModel>>> BuildAsync(CancellationToken ct)
        {
            var result = new DataResult<List<UserInfoModel>>();

            var users = await _service.GetListUsersAsync(ct);

            if (!users.IsSuccess || users.Data is null)
            {
                return result.WithError("Not found");
            }

            var usageCounts = await _usageStats.GetUsageCountsAsync(ct);
            var userIds = users.Data.Select(u => u.Id).ToList();
            var rolesResult = await _roleService.GetByUserIdsAsync(userIds, ct);
            var rolesMap = rolesResult.Data ?? [];

            var res = users.Data.Select(u =>
            {
                var model = Build(u);
                model.UsageCount = usageCounts.GetValueOrDefault(u.Id, 0);
                model.Roles = rolesMap.GetValueOrDefault(u.Id, []);
                return model;
            }).ToList();

            return result.WithData(res);
        }

        public async Task<UserInfoModel> BuildAsync(UserInfo userInfo, CancellationToken ct)
        {
            var model = Build(userInfo);
            var rolesResult = await _roleService.GetByIdAsync(userInfo.Id, ct);
            model.Roles = rolesResult.Data ?? [];
            var usageCounts = await _usageStats.GetUsageCountsAsync(ct);
            model.UsageCount = usageCounts.GetValueOrDefault(userInfo.Id, 0);
            var val = await _settings.GetValueAsync("MaxConcurrentBookings", ct);
            var recentCount = (int.TryParse(val, out var m) ? m : DefaultSettings.MaxConcurrentBookings) * 3;
            model.RecentWashes = await _usageStats.GetRecentWashesAsync(userInfo.Id, recentCount, ct);
            return model;
        }
    }
}