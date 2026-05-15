using System.Text.Json;
using System.Text.Json.Serialization;
using IntDorSys.Core;
using IntDorSys.Core.Constants;
using IntDorSys.DataAccess;
using IntDorSys.Laundress.Services;
using IntDorSys.Security;
using IntDorSys.Services;
using IntDorSys.TelegramBot.Service;
using IntDorSys.TelegramBot.Service.Authorize;
using IntDorSys.TelegramBot.Service.CallbackServices;
using IntDorSys.TelegramBot.Service.CommandServices;
using IntDorSys.TelegramBot.Service.MessageServices;
using Ouro.TelegramBot.Core;
using ServicesInstaller = IntDorSys.TelegramBot.Service.ServicesInstaller;
using IntDorSys.Web.Api.Blazor.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace IntDorSys.Web.Api
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var isBattle = !builder.Configuration.GetValue<bool>("BuildTest");

            // Для убирания зависимости от часовых поясов
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder
                .ConfigureAppConfiguration()
                .ConfigureAppLogging()
                .ConfigureAuthentication();

            builder.Services
                .AddHealthChecks();

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

            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services
                .AddHttpContextAccessor()
                .AddScoped<AuthSession>()
                .AddScoped<ApiClient>()
                .AddScoped(sp =>
                {
                    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                    var request = httpContext?.Request;
                    var baseUri = request == null
                        ? "http://localhost:5050/"
                        : $"{request.Scheme}://{request.Host}/";

                    return new HttpClient
                    {
                        BaseAddress = new Uri(baseUri),
                    };
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
                .AddBotServices(builder.Configuration)
                .AddCoreConfiguration(builder.Configuration);


builder.Services
                .ConfigureTelegramBot(isBattle
                    ? builder.Configuration.GetSection(ConfigSectionNames.TelegramBattleSection)
                    : builder.Configuration.GetSection(ConfigSectionNames.TelegramTestSection))
                .AddTelegramServices();

#pragma warning disable ASP0000 // Disabling warning for service provider pattern required by bot setup
            var serviceProvider = builder.Services.BuildServiceProvider();
            builder.Services
                .AddBotHostServices(
                    ServicesInstaller.GetSettingsAndHandlers(
                        serviceProvider.GetRequiredService<ICallbackHandlerService>(),
                        serviceProvider.GetRequiredService<IMessageHandlerService>(),
                        serviceProvider.GetRequiredService<IAuthService>(),
                        serviceProvider.GetRequiredService<ICommandService>())
                );
#pragma warning restore ASP0000

            var app = builder.Build();

            app.MigrateDb();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("Default");
            app.UseHttpsRedirection();


            app.UseHealthChecks("/health");

app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseRateLimiter();

            app.MapControllers();
            app.MapRazorComponents<Blazor.App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
