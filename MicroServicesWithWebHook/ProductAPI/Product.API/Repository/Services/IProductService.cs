using Shared.Data.DTOs.ProductDTOs;
using Shared.Data.DTOs.ResponseDTOs;

namespace Product.API.Repository.Services
{
    public interface IProductService
    {
        public Task<ResponseDto> GetAllProductsAsync();

        public Task<ResponseDto> GetProductByIdAsync(int id);

        public Task<ResponseDto> AddProductAsync(ProductDto productDto);
    }
}
