using BuildingBlocks.Logging.LoggingInterceptor;
using Microsoft.EntityFrameworkCore;

namespace Product.API.DataLayer
{
    public static class RegisterProductDbContext
    {
        private const string CONNECTION_STRING = "ProductDbContextConnectionString";

        public static IServiceCollection RegisterProductDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            /* Register Logging Interceptor */
            services.AddSingleton<CrudLoggingInterceptor>();

            services.AddDbContext<ProductDbContext>((serviceProvider, options) =>
            {
                /* Resolve The Logging Interceptor */
                var interceptor = serviceProvider.GetRequiredService<CrudLoggingInterceptor>();

                options.UseSqlServer(connectionString: configuration.GetConnectionString(name: CONNECTION_STRING)).AddInterceptors(interceptor);
            });

            return services;
        }
    }
}
