using IntDorSys.Core.Models;

namespace IntDorSys.Laundress.Core.Models
{
    public class LaundModel
    {
        /// <summary>
        ///     User assigned to the slot
        /// </summary>
        public UserInfoModel? SelectUser { get; set; }

        /// <summary>
        ///     Time washing
        /// </summary>
        public DateTime TimeWash { get; set; }
    }
}