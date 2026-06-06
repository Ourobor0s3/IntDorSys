using IntDorSys.Web.Api.Builders;
using IntDorSys.Web.Api.Builders.Impl;
using IntDorSys.Web.Api.ExportServices;
using IntDorSys.Web.Api.ExportServices.Impl;

namespace IntDorSys.Web.Api.Installers
{
    internal static class BuildersInstaller
    {
        public static IServiceCollection AddBuilders(this IServiceCollection services)
        {
            services
                .AddTransient<IUserInfoBuilder, UserInfoBuilder>()
                .AddTransient<ILaundressBuilder, LaundressBuilder>()
                .AddTransient<ILaundressExportService, LaundressExportService>();

            return services;
        }
    }
}