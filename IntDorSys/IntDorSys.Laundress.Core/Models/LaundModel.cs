using IntDorSys.Core.Entities.Users;

namespace IntDorSys.Laundress.Core.Models
{
    public class LaundModel
    {
        /// <summary>
        ///     User, who uses the laundress
        /// </summary>
        public UserInfo? SelectUser { get; set; }

        /// <summary>
        ///     Time washing
        /// </summary>
        public DateTime TimeWash { get; set; }
    }
}