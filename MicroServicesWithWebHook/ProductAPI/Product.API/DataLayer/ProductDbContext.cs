using Microsoft.EntityFrameworkCore;

namespace Product.API.DataLayer
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Shared.Data.Models.ProductModel.Product> Products { get; set; }
    }
}
