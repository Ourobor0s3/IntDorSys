using IntDorSys.Core.Entities.Users;

namespace IntDorSys.TelegramBot.Service.AdminServices
{
    public interface IAdminService
    {
        Task SendUsersNotificationAsync(UserInfo userInfo, CancellationToken ct);
        Task UpdateNotificateUserAsync(long userId, CancellationToken ct);
    }
}