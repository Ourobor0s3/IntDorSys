using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntDorSys.Core
{
    public static class CoreInstaller
    {
        public static void AddCoreConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LinkSettings>(configuration.GetSection(ConfigSectionNames.LinkSection));
            services.Configure<AdminSettings>(configuration.GetSection(ConfigSectionNames.AdminIdsSection));
        }
    }
}
