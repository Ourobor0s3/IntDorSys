using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Services.Services
{
    public interface ILaundressBotService
    {
        /// <summary>
        ///     Send menu
        /// </summary>
        /// <param name="user">User info</param>
        /// <param name="messageId"></param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendMenu(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>
        ///     Create time laund
        /// </summary>
        /// <param name="crUser">User info, who create time</param>
        /// <param name="date">Date time</param>
        /// <param name="start">First time create</param>
        /// <param name="end">Last time create</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task CreateTimesAsync(
            UserInfo crUser,
            string date,
            int start,
            int end,
            CancellationToken ct);

        /// <summary>
        ///     Delete time laund
        /// </summary>
        /// <param name="user">User info, who delete time</param>
        /// <param name="dateTime">Date time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task DeleteLaundAsync(UserInfo user, DateTime dateTime, CancellationToken ct);

        /// <summary>
        ///     Remove use time laund by user
        /// </summary>
        /// <param name="user">User info, who remove use time</param>
        /// <param name="time">Time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task RemoveTimeByUserAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>
        ///     Delete user in time laund
        /// </summary>
        /// <param name="user">User info, who remove use time</param>
        /// <param name="time">Time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task UnUseLaundAsync(UserInfo user, DateTime time, CancellationToken ct);

        /// <summary>
        ///     Send all time
        /// </summary>
        /// <param name="chatId">Chat id, who sent request</param>
        /// <param name="messageId">Message id, to change the message</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendAllTimeAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>
        ///     Send free date
        /// </summary>
        /// <param name="chatId">Chat id, who sent request</param>
        /// <param name="messageId">Message id, to change the message</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendFreeDateAsync(long chatId, int messageId = 0, CancellationToken ct = default);

        /// <summary>
        ///     Send free time
        /// </summary>
        /// <param name="chatId">Chat id, who sended request</param>
        /// <param name="messageId">Message id, to change the message</param>
        /// <param name="date">Date of interest</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendFreeTimeAsync(long chatId, int messageId, DateTime date, CancellationToken ct);

        /// <summary>
        ///     Send use time by user
        /// </summary>
        /// <param name="user">User info, who sent request</param>
        /// <param name="messageId">Message id, to change the message</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task SendUseTimeByUserAsync(UserInfo user, int messageId = 0, CancellationToken ct = default);

        /// <summary>
        ///     Use time laund by user
        /// </summary>
        /// <param name="user">User info, who sent request</param>
        /// <param name="time">Time</param>
        /// <param name="messageId">Message id, to change last message about time</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task UseTimeLaundByUserAsync(UserInfo user, DateTime time, int messageId = 0, CancellationToken ct = default);

        /// <summary>
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task CheckTimeAndSendNotifAsync(CancellationToken ct);
    }
}