using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Order.API.LogConfiguration
{
    public class OrderLog
    {
        private const string LOG_CONFIGURATION_FOLDER = "LogConfiguration";
        private const string LOG_DIRECTORY_NAME = "Logs";
        private const string ORDER_LOG_DIRECTORY = "OrderLog";
        private const string ORDER_LOG_FILE_NAME = "OrderLog-.txt";
        private const string ORDER_PROJECT_NAME = "Order.API";

        public static Serilog.ILogger GenerateOrderLog()
        {
            var logFilePathForOrderApi = GetFilePath(logApiDirName: ORDER_LOG_DIRECTORY, logFileName: ORDER_LOG_FILE_NAME);
            return CreateLog(logPath: logFilePathForOrderApi, projectName: ORDER_PROJECT_NAME);
        }

        private static string GetFilePath(string logApiDirName, string logFileName)
        {
            /* Goes up 3 levels: net10.0 -> Debug -> bin -> reaches Order.API root */
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            /* Builds path: Order.API/LogConfiguration/Logs/OrderLog/ */
            var logDirectory = Path.Combine(projectRoot, LOG_CONFIGURATION_FOLDER, LOG_DIRECTORY_NAME, logApiDirName);
            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(logDirectory, logFileName);
            return logPath;
        }

        private static Logger CreateLog(string logPath, string projectName)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(name: "ApplicationName", value: projectName)
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
