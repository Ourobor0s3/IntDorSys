using IntDorSys.TelegramBot.Core.Models;
using IntDorSys.TelegramBot.Core.Services;
using IntDorSys.TelegramBot.Core.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace IntDorSys.TelegramBot.Core
{
    public static class BotInstaller
    {
        public static IServiceCollection AddBotHostServices(
            this IServiceCollection services,
            IConfiguration configuration,
            SettingsAndHandlers settingsAndHandlers)
        {
            services.AddSingleton(configuration);

            services.AddTransient<IBotHandler>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<BotHandler>>();
                return new BotHandler(logger, configuration, settingsAndHandlers);
            });

            services.AddHostedService<BotHostService>();


            return services;
        }

        public static IServiceCollection AddTelegramServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddSingleton<ITelegramBotClient>(_ =>
                    new TelegramBotClient((Convert.ToBoolean(configuration["BuildTest"])
                                              ? configuration["Telegram:Token:Test"]
                                              : configuration["Telegram:Token:Battle"])
                                       ?? throw new InvalidOperationException()));

            services.AddTransient<ITelegramService, TelegramService>();

            return services;
        }
    }
}