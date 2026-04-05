namespace IntDorSys.Security.Models
{
    /// <summary>
    ///     Contains new user registration data
    /// </summary>
    public class RegistrationData
    {
        /// <summary>
        ///     User email
        /// </summary>
        required public long TelegramId { get; set; }

        /// <summary>
        ///     User email
        /// </summary>
        required public string Email { get; set; }

        /// <summary>
        ///     Not hashed password
        /// </summary>
        required public string Password { get; set; }
    }
}