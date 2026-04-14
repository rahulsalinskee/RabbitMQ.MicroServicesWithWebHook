using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Logging.SeriLogConfiguration
{
    public static class SerilogConfiguration
    {
        public static IHostBuilder UseSharedSerilogExtension(this IHostBuilder hostBuilder, string applicationName)
        {
            hostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty(name: "ApplicationName", value: applicationName)
                    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            });

            return hostBuilder;
        }
    }
}
