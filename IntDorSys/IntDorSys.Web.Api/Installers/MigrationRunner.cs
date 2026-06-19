using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace IntDorSys.Web.Api.Installers
{
    internal static class MigrationRunner
    {
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public static WebApplication MigrateDb(this WebApplication app)
        {
            var factory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = factory.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!config.GetValue<bool>("AutomaticMigrationsEnabled"))
            {
                return app;
            }

            try
            {
                using var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
                db.Database.Migrate();
                _logger.Info("Database migration completed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Database migration failed");
                throw;
            }

            return app;
        }
    }
}