using IntDorSys.Core.Enums;
using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Core.Entities.Users
{
    public sealed class UserInfo : SoftDeletableEntity<long>
    {
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
        ///     Email address
        /// </summary>
        required public string Email { get; set; }

        /// <summary>
        ///     User password hash
        /// </summary>
        required public string Password { get; set; }

        /// <summary>
        ///     Address your leave
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        ///     phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        ///     username by telegram
        /// </summary>
        required public string Username { get; set; }

        public UserStatus Status { get; set; }

        required public long TelegramId { get; set; }
        public string? LanguageCode { get; set; }
        public bool HasInitialDialog { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsBot { get; set; }
    }
}