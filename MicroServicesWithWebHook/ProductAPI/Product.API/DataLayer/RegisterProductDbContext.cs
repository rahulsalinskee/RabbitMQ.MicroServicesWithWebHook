using Microsoft.EntityFrameworkCore;

namespace Product.API.DataLayer
{
    public static class RegisterProductDbContext
    {
        private const string CONNECTION_STRING = "ProductDbContextConnectionString";

        public static IServiceCollection RegisterProductDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseSqlServer(connectionString: configuration.GetConnectionString(name: CONNECTION_STRING));
            });

            return services;
        }
    }
}
