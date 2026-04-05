using System.Text;
using IntDorSys.Core.Entities.Users;
using IntDorSys.Security.Services;
using IntDorSys.Security.Services.Impl;
using IntDorSys.Security.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IntDorSys.Security
{
    public static class SecurityInstaller
    {
        /// <summary>
        ///     Registers services by module <see cref="DDS.DMaple.Security" />
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors</param>
        /// <param name="configuration">
        ///     Represents a set of key/value application configuration properties
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddSecurityServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddTransient<ITokenService, TokenService>()
                .AddTransient<IPasswordHasher<UserInfo>, PasswordHasher<UserInfo>>()
                .AddTransient<IPasswordService, PasswordService>()
                .AddTransient<IAuthService, AuthService>();

            return services;
        }

        /// <summary>
        ///     Configure authentication
        /// </summary>
        /// <param name="builder">Builds the application instance</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        ///     Throws if <see cref="JwtSettings" /> is not defined in app configuration
        /// </exception>
        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection(JwtSettings.SectionName);

            builder.Services.Configure<JwtSettings>(section);
            var jwt = section.Get<JwtSettings>() ?? throw new Exception("Missing JwtSettings section.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = jwt.Issuer;
                    options.Audience = jwt.Audience;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),

                        ValidateLifetime = true,
                    };
                });

            return builder;
        }
    }
}