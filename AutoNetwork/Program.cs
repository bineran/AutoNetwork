using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using System.Runtime.InteropServices;

namespace AutoNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = CreateIConfigurationRoot();
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureLogging((hostContext, logging) =>
               {
                   logging.ClearProviders();
                   logging.AddNLog(new NLogProviderOptions
                   {
                       CaptureMessageTemplates = true,
                       CaptureMessageProperties = true
                   });
               });
            hostBuilder.UseNLog();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                hostBuilder.UseWindowsService();
            }


            hostBuilder.ConfigureServices((services) =>
            {
                services.Configure<VPNConfig>(config.GetSection("vpnConfig"));
                services.AddHostedService<Worker>();

            });

            hostBuilder.Build().Run();
        }
        private static IConfigurationRoot CreateIConfigurationRoot()
        {
            var configBuilder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json", true, true);
            return configBuilder.Build();
        }


    }
}