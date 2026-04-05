using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;

namespace IntDorSys.Laundress.Services.Services
{
    public interface IUserBotService
    {
        /// <summary>
        /// Отправить список пользователей юзеру
        /// </summary>
        /// <param name="userId">Юзер</param>
        /// <param name="messageId">Идентификатор сообщения</param>
        /// <param name="isBlockedUsers">Получить заблокированных пользователей</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task SendUsersAsync(
            long userId,
            int messageId = 0,
            bool isBlockedUsers = false,
            CancellationToken ct = default);

        /// <summary>
        /// Обновление статуса пользователя
        /// </summary>
        /// <param name="userId">Пользователь, который запустил обновление</param>
        /// <param name="forUserId">Пользователь, требующий обновления</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task ChangeStatusUserAsync(
            long userId,
            long forUserId,
            UserStatus newStatus,
            CancellationToken ct);
    }
}