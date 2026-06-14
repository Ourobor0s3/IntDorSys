using IntDorSys.Services.AppSettings;
using IntDorSys.Services.AppSettings.Impl;
using IntDorSys.Services.Audit;
using IntDorSys.Services.Audit.Impl;
using IntDorSys.Services.FileStorage;
using IntDorSys.Services.FileStorage.Impl;
using IntDorSys.Services.Statistics;
using IntDorSys.Services.Statistics.Impl;
using IntDorSys.Services.Users;
using IntDorSys.Services.Users.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace IntDorSys.Services
{
    public static class ServicesInstaller
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddTransient<IAppSettingService, AppSettingService>()
                .AddTransient<IUserQueryService, UserQueryService>()
                .AddTransient<IUserCommandService, UserCommandService>()
                .AddTransient<IFileService, FileService>()
                .AddTransient<IUserRoleService, UserRoleService>()
                .AddTransient<IAuditService, AuditService>()
                .AddTransient<IUsageStatsService, UsageStatsService>();

            return services;
        }
    }
}