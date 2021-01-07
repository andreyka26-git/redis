using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Redis.Common.Infrastructure
{
    public class WebHost
    {
        public static IHostBuilder CreateWebHostBuilder<TStartup>()
            where TStartup : class
        {
            var assembly = typeof(GlobalConsts).Assembly;
            var infrastructurePath = Path.GetDirectoryName(assembly.Location);
            var fileProvider = new PhysicalFileProvider(infrastructurePath);

            var builder = Host.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    config
                        .AddJsonFile(fileProvider, "appsettings.shared.json", false, true)
                        .AddJsonFile("appsettings.json", false)
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel(options => options.AddServerHeader = false);
                    webHostBuilder.UseStartup<TStartup>();
                });

            return builder;
        }
    }
}
