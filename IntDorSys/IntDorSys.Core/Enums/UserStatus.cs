using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Core.Enums
{
    public enum UserStatus
    {
        /// <summary>
        ///     User blocked for the system
        /// </summary>
        [Display(Name = "Blocked")]
        Blocked = -1,

        /// <summary>
        ///     User registered for the system
        /// </summary>
        [Display(Name = "Registered")]
        Registered = 0,
    }
}