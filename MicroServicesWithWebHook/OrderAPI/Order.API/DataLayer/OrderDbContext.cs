using Microsoft.EntityFrameworkCore;

namespace Order.API.DataLayer
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Shared.Data.Models.OrderModel.Order> Orders { get; set; }

        public DbSet<Shared.Data.Models.ProductModel.Product> Products { get; set; }
    }
}
