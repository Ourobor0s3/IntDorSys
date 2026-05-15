namespace IntDorSys.Security.Models
{
    /// <summary>
    ///     Provides authorization token info
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        ///     Auth token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        ///     Refresh token
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        ///     User role
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// </summary>
        public long ExpiresIn { get; set; }
    }
}