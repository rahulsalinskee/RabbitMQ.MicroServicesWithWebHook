using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Product.API.LogConfiguration
{
    public static class ProductLog
    {
        private const string LOG_CONFIGURATION_FOLDER = "LogConfiguration";
        private const string LOG_DIRECTORY_NAME = "Logs";
        private const string PRODUCT_LOG_DIRECTORY = "ProductLog";
        private const string PRODUCT_LOG_FILE_NAME = "ProductLog-.txt";
        private const string PRODUCT_PROJECT_NAME = "Product.API";

        public static Serilog.ILogger GenerateProductLog()
        {
            var logFilePathForProductApi = GetFilePath(logApiDirName: PRODUCT_LOG_DIRECTORY, logFileName: PRODUCT_LOG_FILE_NAME);
            return CreateLog(logPath: logFilePathForProductApi, projectName: PRODUCT_PROJECT_NAME);
        }

        private static string GetFilePath(string logApiDirName, string logFileName)
        {
            // Goes up 3 levels: net10.0 -> Debug -> bin -> reaches Product.API root
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            // Builds path: Product.API/LogConfiguration/Logs/ProductLog/
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