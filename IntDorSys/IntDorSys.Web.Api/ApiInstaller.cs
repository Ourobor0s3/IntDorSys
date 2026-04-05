using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

namespace IntDorSys.Web.Api
{
    internal static class ApiInstaller1
    {
        public static WebApplicationBuilder ConfigureAppConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", false, true);

            return builder;
        }

        public static WebApplicationBuilder ConfigureAppLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog("./NLog.config");
            return builder;
        }

        public static WebApplication MigrateDb(this WebApplication app)
        {
            var factory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = factory.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!config.GetValue<bool>("AutomaticMigrationsEnabled"))
            {
                return app;
            }

            using var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
            db.Database.Migrate();

            return app;
        }
    }
}