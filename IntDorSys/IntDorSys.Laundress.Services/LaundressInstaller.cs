using IntDorSys.Core.Constants;
using IntDorSys.Laundress.Services.Impl;
using IntDorSys.Laundress.Services.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ouro.QuartzCore;
using Ouro.QuartzCore.Extensions;

namespace IntDorSys.Laundress.Services
{
    public static class LaundressInstaller
    {
        public static IServiceCollection AddLaundressServices(
            this IServiceCollection services,
            ConfigurationManager configuration)
        {
            // Подключение сервисов
            services
                .AddTransient<ILaundressService, LaundressService>()
                .AddTransient<IUseLaundressQueryService, LaundressService>()
                .AddTransient<ILaundReportService, LaundReportService>()
                .AddTransient<IUserBotService, UserBotService>()
                .AddTransient<ILaundAnaliticService, LaundAnaliticService>();

            services
                .AddTransient<ILaundressBotService, LaundressBotService>()
                .AddTransient<ILaundressBotMenuService, LaundressBotService>()
                .AddTransient<ILaundressBotBookingService, LaundressBotService>()
                .AddTransient<ILaundressBotNotificationService, LaundressBotService>();

            services.AddQuartzCoreServices(
                jobConfigurator =>
                {
                    jobConfigurator.ScheduleNonConcurrentJob<SendTimeJob>(
                        TimeSpan.FromMinutes(15));

                    jobConfigurator.ScheduleNonConcurrentJob<CheckWorkJob>(
                        TimeSpan.FromHours(18));
                },
                configuration.GetSection(ConfigSectionNames.QuartzSection));

            return services;
        }
    }
}