using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public class NewUserRequest
    {
        /// <summary>
        ///     Telegram ID (optional for web registration)
        /// </summary>
        public long? TelegramId { get; set; }

        /// <summary>
        ///     User email
        /// </summary>
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        ///     Not hashed password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression("[A-Za-z0-9!@#$%^&*()_+-=\\[\\]{};':\"\\\\|,.<>\\/?]+")]
        public string Password { get; set; } = string.Empty;
    }
}