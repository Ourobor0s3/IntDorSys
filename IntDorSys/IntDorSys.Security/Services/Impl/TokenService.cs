using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using IntDorSys.Security.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IntDorSys.Security.Services.Impl
{
    internal sealed class TokenService : ITokenService
    {
        private readonly SigningCredentials _creds;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly IOptionsSnapshot<JwtSettings> _settings;

        public TokenService(IOptionsSnapshot<JwtSettings> settings)
        {
            _settings = settings;
            _handler = new JwtSecurityTokenHandler();
            _creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret)),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public AuthToken IssueToken(UserInfo user)
        {
            var now = DateTime.UtcNow;

            var token = _handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ]),
                Expires = now.Add(_settings.Value.Expiration),
                IssuedAt = now,
                Issuer = _settings.Value.Issuer,
                Audience = _settings.Value.Audience,
                SigningCredentials = _creds,
            });

            var jwt = _handler.WriteToken(token);

            return new AuthToken
            {
                AccessToken = jwt,
                ExpiresIn = (long)_settings.Value.Expiration.TotalSeconds,
            };
        }
    }
}