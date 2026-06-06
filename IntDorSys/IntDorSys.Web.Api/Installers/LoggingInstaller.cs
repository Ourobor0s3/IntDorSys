using NLog.Web;

namespace IntDorSys.Web.Api.Installers
{
    internal static class LoggingInstaller
    {
        public static WebApplicationBuilder ConfigureAppLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog("./NLog.config");
            return builder;
        }
    }
}