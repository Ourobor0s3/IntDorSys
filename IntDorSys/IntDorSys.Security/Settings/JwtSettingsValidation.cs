using Microsoft.Extensions.Options;

namespace IntDorSys.Security.Settings
{
    internal sealed class JwtSettingsValidation : IValidateOptions<JwtSettings>
    {
        private const string placeholder = "00000000-0000-0000-0000-000000000000";
        private const int minSecretLength = 32;

        public ValidateOptionsResult Validate(string? name, JwtSettings options)
        {
            if (string.IsNullOrWhiteSpace(options.Issuer))
            {
                return ValidateOptionsResult.Fail("JWT Issuer is required");
            }

            if (string.IsNullOrWhiteSpace(options.Audience))
            {
                return ValidateOptionsResult.Fail("JWT Audience is required");
            }

            if (string.IsNullOrWhiteSpace(options.Secret))
            {
                return ValidateOptionsResult.Fail("JWT Secret is required");
            }

            if (options.Secret == placeholder)
            {
                return ValidateOptionsResult.Fail($"JWT Secret contains the default placeholder value. Set a real secret via config, env var, or User Secrets.");
            }

            if (options.Secret.Length < minSecretLength)
            {
                return ValidateOptionsResult.Fail($"JWT Secret must be at least {minSecretLength} characters long (HS256 requires 256-bit key). Current length: {options.Secret.Length}.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}