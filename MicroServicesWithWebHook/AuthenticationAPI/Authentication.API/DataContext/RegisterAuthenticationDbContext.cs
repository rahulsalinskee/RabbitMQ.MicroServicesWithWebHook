using BuildingBlocks.Logging.LoggingInterceptor;
using Microsoft.EntityFrameworkCore;

namespace Authentication.API.DataContext
{
    public static class RegisterAuthenticationDbContext
    {
        private const string CONNECTION_STRING = "AuthenticationDbContextConnectionString";

        public static void RegisterAuthenticationDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            /* Register the logging interceptor */
            services.AddSingleton<CrudLoggingInterceptor>();

            services.AddDbContext<AuthenticationDbContext>((serviceProvider, options) =>
            {
                /* Resolve The Logging Interceptor */
                var interceptor = serviceProvider.GetRequiredService<CrudLoggingInterceptor>();

                options.UseSqlServer(connectionString: configuration.GetConnectionString(CONNECTION_STRING)).AddInterceptors(interceptor);
            });
        }
    }
}
