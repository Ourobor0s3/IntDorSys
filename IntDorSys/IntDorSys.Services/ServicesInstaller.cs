using IntDorSys.Services.Builders;
using IntDorSys.Services.Builders.Impl;
using IntDorSys.Services.FileStorage;
using IntDorSys.Services.FileStorage.Impl;
using IntDorSys.Services.Users;
using IntDorSys.Services.Users.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace IntDorSys.Services
{
    public static class ServicesInstaller
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Подключение сервисов
            services
                .AddTransient<IUserInfoBuilder, UserInfoBuilder>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IFileService, FileService>()
                .AddTransient<IUserRoleService, UserRoleService>();

            return services;
        }
    }
}