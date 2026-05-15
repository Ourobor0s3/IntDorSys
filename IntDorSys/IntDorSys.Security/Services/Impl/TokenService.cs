using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Models;
using IntDorSys.Security.Settings;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IntDorSys.Security.Services.Impl
{
    internal sealed class TokenService : ITokenService
    {
        private readonly SigningCredentials _creds;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly IOptionsSnapshot<JwtSettings> _settings;
        private readonly IUserRoleService _userRoleService;

        public TokenService(IOptionsSnapshot<JwtSettings> settings, IUserRoleService userRoleService)
        {
            _settings = settings;
            _userRoleService = userRoleService;
            _handler = new JwtSecurityTokenHandler();
            _creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Secret)),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public AuthToken IssueToken(UserInfo user)
        {
            var now = DateTime.UtcNow;
            
            var roles = _userRoleService.GetByIdAsync(user.Id, CancellationToken.None).GetAwaiter().GetResult();
            var userRoles = roles.IsSuccess ? roles.Data ?? [] : [];

            var role = user.Status == Core.Enums.UserStatus.Blocked ? "blocked"
                : userRoles.Contains(UserRoleKeys.Admin) ? "admin"
                : "user";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role),
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
                RefreshToken = Guid.NewGuid().ToString(),
                Role = role,
                ExpiresIn = (long)_settings.Value.Expiration.TotalSeconds,
            };
        }
    }
}