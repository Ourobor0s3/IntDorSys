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
        /// </summary>
        public long ExpiresIn { get; set; }
    }
}