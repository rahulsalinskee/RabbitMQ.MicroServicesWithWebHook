using Microsoft.EntityFrameworkCore;

namespace Order.API.DataLayer
{
    public static class RegisterOrderDbContext
    {
        private const string CONNECTION_STRING = "OrderDbContextConnectionString";

        public static IServiceCollection RegisterOrderDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer(connectionString: configuration.GetConnectionString(name: CONNECTION_STRING));
            });

            return services;
        }
    }
}
