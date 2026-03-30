using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Models.AuthenticationModel;

namespace Authentication.API.DataContext
{
    public class AuthenticationDbContext : IdentityDbContext<User>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> dbContextOptions) : base (options: dbContextOptions)
        {
            
        }

        public DbSet<User> ApplicationIdentityUsers { get; set; }
    }
}
