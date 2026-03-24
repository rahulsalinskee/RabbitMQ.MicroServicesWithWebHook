using MassTransit;
using Order.API.DataLayer;
using Shared.Data.DTOs.ProductDTOs.Version2;
using ProductModel = Shared.Data.Models.ProductModel.Product;
using ProductDtoVersion2 = Shared.Data.DTOs.ProductDTOs.Version2.ProductDto;

namespace Order.API.RabbitMqConsumer.ProductConsumer
{
    public class ProductConsumerV2 : IConsumer<ProductDtoVersion2>
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly ILogger<ProductConsumerV2> _logger;

        public ProductConsumerV2(OrderDbContext orderDbContext, ILogger<ProductConsumerV2> logger)
        {
            this._orderDbContext = orderDbContext;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductDto> context)
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
                    Price = productMessage.CostPrice,
                };

                await this._orderDbContext.Products.AddAsync(newProduct);
                await this._orderDbContext.SaveChangesAsync();
            }
            else
            {
                /* The product already exists, update its details */
                existingProduct.Name = productMessage.Name;
                existingProduct.Price = productMessage.CostPrice;
            }

            await this._orderDbContext.SaveChangesAsync();
            this._logger.LogInformation("Successfully synced product {ProductName} to local Order Database.", productMessage.Name);
        }
    }
}
