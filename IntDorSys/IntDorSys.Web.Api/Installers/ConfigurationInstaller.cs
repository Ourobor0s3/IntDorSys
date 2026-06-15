namespace IntDorSys.Web.Api.Installers
{
    internal static class ConfigurationInstaller
    {
        public static WebApplicationBuilder ConfigureAppConfiguration(this WebApplicationBuilder builder, string[] args)
        {
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", false, true);
            builder.Configuration.AddJsonFile("appsettings.local.json", true, true);
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddCommandLine(args);

            return builder;
        }
    }
}