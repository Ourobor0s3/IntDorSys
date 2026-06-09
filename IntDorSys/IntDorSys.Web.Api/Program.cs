using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using IntDorSys.Core;
using IntDorSys.Core.Constants;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Services;
using IntDorSys.Security;
using IntDorSys.Services;
using IntDorSys.TelegramBot.Service;
using IntDorSys.Web.Api.Bot;
using IntDorSys.Web.Api.Installers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Ouro.TelegramBot.Core;

namespace IntDorSys.Web.Api
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var isBattle = !builder.Configuration.GetValue<bool>("BuildTest");

            // Remove dependency on time zones for legacy timestamp behavior
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder
                .ConfigureAppConfiguration()
                .ConfigureAppLogging()
                .ConfigureAuthentication();

            builder.Services
                .AddHealthChecks()
                .AddCheck<BotHealthCheck>("telegram_bot");

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("AuthLimit", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "default",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));
            });

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:4201", "http://localhost:5050"];

            builder.Services
                .AddCors(options =>
                {
                    options.AddPolicy("Default",
                        policyBuilder => policyBuilder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.AllowInputFormatterExceptionMessages = false;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            builder.Services
                .AddSecurityServices(builder.Configuration)
                .AddDataAccessServices(builder.Configuration)
                .AddLaundressServices(builder.Configuration)
                .AddServices()
                .AddBuilders()
                .AddBotServices(builder.Configuration)
                .AddCoreConfiguration(builder.Configuration);

            builder.Services
                .AddSingleton<BotStatus>()
                .ConfigureTelegramBot(isBattle
                    ? builder.Configuration.GetSection(ConfigSectionNames.TelegramBattleSection)
                    : builder.Configuration.GetSection(ConfigSectionNames.TelegramTestSection))
                .AddTelegramServices()
                .AddSingleton<ResilientBotHostedService>()
                .AddHostedService(sp => sp.GetRequiredService<ResilientBotHostedService>())
                .AddSingleton<IBotControlService>(sp =>
                    sp.GetRequiredService<ResilientBotHostedService>());

            var app = builder.Build();

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/problem+json";

                    var problem = new
                    {
                        type = "https://httpstatuses.com/500",
                        title = "An unexpected error occurred",
                        status = StatusCodes.Status500InternalServerError,
                        detail = "Internal server error",
                    };

                    await JsonSerializer.SerializeAsync(context.Response.Body, problem);
                });
            });

            app.MigrateDb();
            app.SeedSettings();

            app.UseRouting();
            app.UseCors("Default");
            app.UseHttpsRedirection();

            app.UseHealthChecks("/health");

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}