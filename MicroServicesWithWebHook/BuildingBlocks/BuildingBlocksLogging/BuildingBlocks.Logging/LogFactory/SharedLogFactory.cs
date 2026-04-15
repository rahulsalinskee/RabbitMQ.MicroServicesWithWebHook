using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BuildingBlocks.Logging.LogFactory
{
    public static class SharedLogFactory
    {
        private const string LOG_CONFIGURATION_FOLDER = "LogConfiguration";
        private const string LOG_DIRECTORY_NAME = "Logs";

        public static Logger GenerateSharedLogger(string applicationName)
        {
            var logFilePath = GetFilePath(applicationName);
            return CreateLog(logPath: logFilePath, applicationName: applicationName);
        }

        private static string GetFilePath(string applicationName)
        {
            /* Goes up 3 levels: net10.0 -> Debug -> bin -> reaches calling API root */
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            /* Builds path dynamically: AppName/LogConfiguration/Logs/AppNameLog/ */
            var logDirectory = Path.Combine(projectRoot, LOG_CONFIGURATION_FOLDER, LOG_DIRECTORY_NAME, $"{applicationName}Log");
            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(logDirectory, $"{applicationName}Log-.txt");
            return logPath;
        }

        private static Logger CreateLog(string logPath, string applicationName)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{ApplicationName}] {Message:lj}{NewLine}{Exception} \n",
                    retainedFileCountLimit: 7
                ).CreateLogger();
        }
    }
}
