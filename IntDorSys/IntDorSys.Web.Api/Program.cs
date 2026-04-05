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

            builder.Services
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        policyBuilder => policyBuilder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.AllowInputFormatterExceptionMessages = false;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.IncludeFields = true;
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

            var serviceProvider = builder.Services.BuildServiceProvider();
            builder.Services
                .AddBotHostServices(
                    ServicesInstaller.GetSettingsAndHandlers(
                        serviceProvider.GetRequiredService<ICallbackHandlerService>(),
                        serviceProvider.GetRequiredService<IMessageHandlerService>(),
                        serviceProvider.GetRequiredService<IAuthService>(),
                        serviceProvider.GetRequiredService<ICommandService>()
                    )
                );

            var app = builder.Build();

            app.MigrateDb();

            app.UseRouting();
            app.UseCors(policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });


            app.UseHealthChecks("/health");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}