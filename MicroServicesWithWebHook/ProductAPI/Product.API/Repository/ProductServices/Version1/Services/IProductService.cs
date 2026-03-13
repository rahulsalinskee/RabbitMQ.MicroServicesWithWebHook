using Shared.Data.DTOs.ProductDTOs.Version1;
using Shared.Data.DTOs.ResponseDTOs;

namespace Product.API.Repository.ProductServices.Version1.Services
{
    public interface IProductService
    {
        public Task<ResponseDto> GetAllProductsAsync(string? columnName = null, string? filterKeyWord = null);

        public Task<ResponseDto> GetProductByIdAsync(int id);

        public Task<ResponseDto> AddProductAsync(AddProductDto productDto);

        public Task<ResponseDto> UpdateProductByIdAsync(int id, UpdateProductDto updateProductDto);

        public Task<ResponseDto> DeleteProductByIdAsync(int id);

        public Task<ResponseDto> AddBulkProductsAsync(List<AddProductDto> productDtos);

        public Task<ResponseDto> DeleteBulkProductsAsync(int idToDeleteFrom, int idToDeleteTill);

        public Task<ResponseDto> DeleteProductByIdInOneDatabaseHitAsync(int id);
    }
}
