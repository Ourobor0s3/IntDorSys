using IntDorSys.Core.Constants;
using IntDorSys.Core.Entities;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace IntDorSys.Web.Api.Installers
{
    internal static class SettingsSeeder
    {
        public static WebApplication SeedSettings(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();

            var existingKeys = db.Set<AppSetting>()
                .IgnoreQueryFilters()
                .Select(s => s.Key)
                .ToHashSet();

            foreach (var entry in DefaultSettings.All)
            {
                if (existingKeys.Contains(entry.Key))
                    continue;

                db.Set<AppSetting>().Add(new AppSetting
                {
                    Key = entry.Key,
                    Value = entry.Value,
                    IsEditable = entry.IsEditable
                });
            }

            db.SaveChanges();
            return app;
        }
    }
}
