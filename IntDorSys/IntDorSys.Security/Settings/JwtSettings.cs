namespace IntDorSys.Security.Settings
{
    internal sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>
        ///     Token expiration
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        ///     Token issuer
        /// </summary>
        required public string Issuer { get; set; }

        /// <summary>
        ///     Token audience
        /// </summary>
        required public string Audience { get; set; }

        /// <summary>
        ///     Key for token signature
        /// </summary>
        required public string Secret { get; set; }
    }
}