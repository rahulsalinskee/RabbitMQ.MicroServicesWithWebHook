using Microsoft.EntityFrameworkCore;

namespace Authentication.API.DataContext
{
    public static class RegisterAuthenticationDbContext
    {
        public static void RegisterAuthenticationDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthenticationDbContext>(options =>
            {
                options.UseSqlServer(connectionString: configuration.GetConnectionString("AuthenticationDbContextConnectionString"));
            });
        }
    }
}
