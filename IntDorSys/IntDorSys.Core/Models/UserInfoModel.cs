using IntDorSys.Core.Enums;

namespace IntDorSys.Core.Models
{
    public class UserInfoModel
    {
        /// <summary>
        ///     Identity
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Full name
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        ///     Number group by university
        /// </summary>
        public string? NumGroup { get; set; }

        /// <summary>
        ///     Number room by live
        /// </summary>
        public string? NumRoom { get; set; }

        /// <summary>
        ///     username by telegram
        /// </summary>
        required public string Username { get; set; }

        /// <summary>
        ///     User status for system
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        ///     Register date for system (click start)
        /// </summary>
        public string? RegisterDate { get; set; }

        /// <summary>
        ///     Is blocked user for system
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        ///     Admin confirmed user
        /// </summary>
        public bool IsConfirm { get; set; }
    }
}