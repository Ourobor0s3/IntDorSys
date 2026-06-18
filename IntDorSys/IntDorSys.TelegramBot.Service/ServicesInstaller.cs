using IntDorSys.TelegramBot.Service.AdminServices;
using IntDorSys.TelegramBot.Service.AdminServices.Impl;
using IntDorSys.TelegramBot.Service.Authorize;
using IntDorSys.TelegramBot.Service.Authorize.Impl;
using IntDorSys.TelegramBot.Service.CallbackServices;
using IntDorSys.TelegramBot.Service.CallbackServices.Impl;
using IntDorSys.TelegramBot.Service.CommandServices;
using IntDorSys.TelegramBot.Service.CommandServices.Impl;
using IntDorSys.TelegramBot.Service.MessageServices;
using IntDorSys.TelegramBot.Service.MessageServices.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ouro.Backup;
using Ouro.TelegramBot.Core.Models;

namespace IntDorSys.TelegramBot.Service
{
    public static class ServicesInstaller
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register bot services
            services
                .AddTransient<IBaseCommandsService, BaseCommandsService>()
                .AddTransient<ICommandService, CommandService>()
                .AddTransient<IBotAuthService, AuthService>()
                .AddTransient<IBotRegistrationService, BotRegistrationService>()
                .AddTransient<IAdminService, AdminService>();

            // Register message handler services
            services
                .AddTransient<IMessageHandlerService, MessageHandlerService>()
                .AddTransient<IUsersMessageHandler, UsersMessageHandler>()
                .AddTransient<IAdminMessageHandler, AdminMessageHandler>()
                .AddTransient<ILaundressMessageHandler, LaundressMessageHandler>();

            // Connect callback services
            services
                .AddTransient<ICallbackHandlerService, CallbackHandlerService>()
                .AddTransient<IAdminCallbackHandler, AdminCallbackHandler>()
                .AddTransient<ILaundressCallbackHandler, LaundressCallbackHandler>();

            services.AddBackupInstaller()
                .ConfigureDumpSettings(configuration.GetSection("DumpSettings"));


            return services;
        }

        public static Handlers GetSettingsAndHandlers(
            ICallbackHandlerService callbackHandler,
            IMessageHandlerService messageHandler,
            IBotAuthService authService,
            ICommandService commandService)
        {
            return new Handlers
            {
                CommandHandler = commandService.GetDictCommands(),
                MessageHandler = messageHandler.HandleAsync,
                CallbackHandler = callbackHandler.HandleAsync,
                AuthorizationUser = authService.AuthUser,
            };
        }
    }
}