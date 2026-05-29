using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IntDorSys.Web.Api.Installers
{
    internal static class MigrationRunner
    {
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