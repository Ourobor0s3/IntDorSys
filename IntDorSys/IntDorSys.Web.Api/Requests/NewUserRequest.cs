using System.ComponentModel;
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
        ///     User full name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

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
        [Required]
        [PasswordPropertyText]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression("[A-Za-z0-9!@#$%^&*()_+-=\\[\\]{};':\"\\\\|,.<>\\/?]+")]
        public string Password { get; set; } = string.Empty;
    }
}