namespace IntDorSys.Security.Models
{
    /// <summary>
    ///     Contains new user registration data
    /// </summary>
    public class RegistrationData
    {
        /// <summary>
        ///     Telegram ID (optional for web registration)
        /// </summary>
        public long TelegramId { get; set; }

        /// <summary>
        ///     User email
        /// </summary>
        required public string Email { get; set; }

        /// <summary>
        ///     User full name
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        ///     User group number
        /// </summary>
        public string? NumGroup { get; set; }

        /// <summary>
        ///     User room number
        /// </summary>
        public string? NumRoom { get; set; }

        /// <summary>
        ///     Not hashed password
        /// </summary>
        required public string Password { get; set; }
    }
}