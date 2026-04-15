using BuildingBlocks.Logging.LoggingInterceptor;
using Microsoft.EntityFrameworkCore;

namespace Order.API.DataLayer
{
    public static class RegisterOrderDbContext
    {
        private const string CONNECTION_STRING = "OrderDbContextConnectionString";


        public static IServiceCollection RegisterOrderDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            /* Register the logging interceptor */
            services.AddSingleton<CrudLoggingInterceptor>();

            services.AddDbContext<OrderDbContext>((serviceProvider, options) =>
            {
                /* Resolve The Logging Interceptor */
                var interceptor = serviceProvider.GetRequiredService<CrudLoggingInterceptor>();

                options.UseSqlServer(connectionString: configuration.GetConnectionString(name: CONNECTION_STRING)).AddInterceptors(interceptor);
            });

            return services;
        }
    }
}
