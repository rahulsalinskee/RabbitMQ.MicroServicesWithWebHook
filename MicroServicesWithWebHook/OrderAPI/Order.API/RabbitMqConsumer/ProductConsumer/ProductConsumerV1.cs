using MassTransit;
using Order.API.DataLayer;
using ProductModel = Shared.Data.Models.ProductModel.Product;
using ProductDtoVersion1 = Shared.Data.DTOs.ProductDTOs.Version1.ProductDto;

namespace Order.API.RabbitMqConsumer.ProductConsumer
{
    public class ProductConsumerV1 : IConsumer<ProductDtoVersion1>
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly ILogger<ProductConsumerV1> _logger;

        public ProductConsumerV1(OrderDbContext orderDbContext, ILogger<ProductConsumerV1> logger)
        {
            this._orderDbContext = orderDbContext;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDtoVersion1> context)
        {
            var productMessage = context.Message;
            this._logger.LogInformation("Product Created: {productMessage}", productMessage);

            /* Check if we already have this product to avoid duplicate */
            var existingProduct = await this._orderDbContext.Products.FindAsync(productMessage.ID);

            if (existingProduct is null)
            {
                var newProduct = new ProductModel()
                {
                    ID = productMessage.ID,
                    Name = productMessage.Name,
                    Price = productMessage.Price
                };

                await this._orderDbContext.Products.AddAsync(newProduct);
                await this._orderDbContext.SaveChangesAsync();
            }
            else
            {
                /* The product already exists, update its details */
                existingProduct.Name = productMessage.Name;
                existingProduct.Price = productMessage.Price;
            }

            await this._orderDbContext.SaveChangesAsync();
            this._logger.LogInformation("Successfully synced product {ProductName} to local Order Database.", productMessage.Name);
        }
    }
}
