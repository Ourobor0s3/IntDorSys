using IntDorSys.Core.Entities.Users;
using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Laundress.Core.Entities
{
    public class UseLaundress : SoftDeletableEntity<long>
    {
        /// <summary>
        ///     User id, who create time washing
        /// </summary>
        required public long CreatedUserId { get; set; }

        /// <summary>
        ///     User, who create time washing
        /// </summary>
        public UserInfo? CreatedUser { get; set; }

        /// <summary>
        ///     User id, who uses the laundress
        /// </summary>
        public long? SelectUserId { get; set; }

        /// <summary>
        ///     User, who uses the laundress
        /// </summary>
        public UserInfo? SelectUser { get; set; }

        /// <summary>
        ///     Time washing
        /// </summary>
        public DateTime TimeWash { get; set; }

        /// <summary>
        ///     Was the notification sent the day before the washing
        /// </summary>
        public bool IsSendDay { get; set; }

        /// <summary>
        ///     Was the notification sent hours before the washing
        /// </summary>
        public bool IsSendHours { get; set; }
    }
}