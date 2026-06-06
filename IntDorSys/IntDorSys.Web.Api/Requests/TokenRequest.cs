using System.ComponentModel.DataAnnotations;

namespace IntDorSys.Web.Api.Requests
{
    public sealed class TokenRequest
    {
        /// <summary>
        ///     User email or username
        /// </summary>
        [Required]
        [StringLength(50)]
        required public string Login { get; set; }

        /// <summary>
        ///     Not hashed password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression("[A-Za-z0-9!@#$%^&*()_+-=\\[\\]{};':\"\\\\|,.<>\\/?]+")]
        required public string Password { get; set; }
    }
}