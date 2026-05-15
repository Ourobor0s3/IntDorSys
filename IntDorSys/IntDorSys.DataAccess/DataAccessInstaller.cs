using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntDorSys.DataAccess
{
    public static class DataAccessInstaller
    {
        public static IServiceCollection AddDataAccessServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddDbContext<AppDataContext>(options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString(configuration.GetValue<bool>("BuildTest")
                            ? "Test"
                            : "Battle"),
                        builder =>
                        {
                            builder.SetPostgresVersion(new Version(16, 0));
                        });
                });

            return services;
        }
    }
}