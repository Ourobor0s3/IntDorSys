using IntDorSys.Core.Constants;
using IntDorSys.Laundress.Services.Builders;
using IntDorSys.Laundress.Services.Builders.Impl;
using IntDorSys.Laundress.Services.Jobs;
using IntDorSys.Laundress.Services.Services;
using IntDorSys.Laundress.Services.Services.Impl;
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
            // Подключение билдеров
            services.AddTransient<ILaundressBuilder, LaundressBuilder>();

            // Подключение сервисов
            services
                .AddTransient<ILaundressService, LaundressService>()
                .AddTransient<ILaundReportService, LaundReportService>()
                .AddTransient<IUserBotService, UserBotService>()
                .AddTransient<ILaundAnaliticService, LaundAnaliticService>();

            // Подключение сервиса бота
            services.AddTransient<ILaundressBotService, LaundressBotService>();

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