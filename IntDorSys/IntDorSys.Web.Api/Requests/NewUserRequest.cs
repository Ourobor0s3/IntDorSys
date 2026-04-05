using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public class NewUserRequest
    {
        /// <summary>
        ///     User email
        /// </summary>
        [Required]
        required public long TelegramId { get; set; }

        /// <summary>
        ///     User email
        /// </summary>
        [Required]
        [StringLength(50)]
        required public string Email { get; set; }

        /// <summary>
        ///     Not hashed password
        /// </summary>
        [Required]
        [PasswordPropertyText]
        [RegularExpression("[A-Za-z0-9!@#$%^&*()_+-=\\[\\]{};':\"\\\\|,.<>\\/?]+")]
        required public string Password { get; set; }
    }
}