namespace IntDorSys.Security.Models
{
    /// <summary>
    ///     Provides authorization token info
    /// </summary>
    public class AuthToken
    {
        public string AccessToken { get; set; } = string.Empty;

        public string? Role { get; set; }

        public long ExpiresIn { get; set; }
    }
}