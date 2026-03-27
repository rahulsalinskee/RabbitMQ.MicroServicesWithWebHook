using MassTransit;
using Microsoft.EntityFrameworkCore;
using Product.API.DataLayer;
using Product.API.Repository.CacheServices.Services;
using Product.API.Repository.FilterServices.Services;
using Product.API.Repository.ProductServices.Version1.Services;
using Shared.Data.DTOs.ProductDTOs.Version1;
using Shared.Data.DTOs.ResponseDTOs;
using Shared.Data.Mapper.ProductMapper;
using Shared.Data.Models.ErrorModel;
using ProductModel = Shared.Data.Models.ProductModel.Product;

namespace Product.API.Repository.ProductServices.Version1.Implementations
{
    public class ProductServiceImplementation : IProductService
    {
        private readonly ProductDbContext _productDbContext;
        private readonly ICacheService _cacheService;
        private readonly IFilterService<ProductModel> _filterService;
        private readonly IPublishEndpoint _publishEndpoint;
        private const string ALL_PRODUCTS_CACHE_KEY = "ALL_PRODUCTS";

        public ProductServiceImplementation(ProductDbContext productDbContext, ICacheService cacheService, IFilterService<ProductModel> filterService, IPublishEndpoint publishEndpoint)
        {
            this._productDbContext = productDbContext;
            this._cacheService = cacheService;
            this._filterService = filterService;
            this._publishEndpoint = publishEndpoint;
        }

        public Task<ResponseDto> AddBulkProductsAsync(List<AddProductDto> productDtos)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> AddProductAsync(AddProductDto addProductDto)
        {
            if (addProductDto is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Is Null!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            /* Check duplication in the database, not in-memory */
            var isDuplicatedProduct = await IsProductNameDuplicatedAsync(addProductDto.Name);

            if (isDuplicatedProduct)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Is Duplicated!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var productDto = new ProductDto()
            {
                Name = addProductDto.Name,
                Price = addProductDto.Price,
            };

            var product = productDto.ConvertProductDtoToProductExtensionVersion1();

            await this._productDbContext.Products.AddAsync(product);
            await this._productDbContext.SaveChangesAsync();

            /* Invalidate (Delete) the cache */
            await this._cacheService.RemoveDataAsync(key: ALL_PRODUCTS_CACHE_KEY);

            var addedProductDto = product.ConvertProductToProductDtoExtensionVersion1();

            /* Publish the message to the queue to notify the other micro services */
            await this._publishEndpoint.Publish(message: addedProductDto);

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = addedProductDto,
                Message = "New Product Added!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> DeleteBulkProductsAsync(int idToDeleteFrom, int idToDeleteTill)
        {
            if (idToDeleteFrom < 0 || idToDeleteTill < 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id can not be negative!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            if (idToDeleteFrom == 0 || idToDeleteTill == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id can not be zero!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            if (idToDeleteFrom == idToDeleteTill)
            {
                var response = await DeleteProductByIdInOneDatabaseHitAsync(id: idToDeleteFrom);

                if (response.IsSuccess)
                {
                    return response;
                }
            }

            var existingProductIds = await this._productDbContext.Products.Where(product => product.ID >= idToDeleteFrom && product.ID <= idToDeleteTill).Select(product => product.ID).ToListAsync();

            if (existingProductIds is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product IDs Are Null!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            if (!existingProductIds.Any())
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Not Found!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var expectedProductIds = Enumerable.Range(idToDeleteFrom, idToDeleteTill - idToDeleteFrom + 1).ToList();
            var missingBookIds = expectedProductIds.Except(existingProductIds).ToList();

            if (missingBookIds.Any())
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Not Found!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var productsToDeleteCount = await this._productDbContext.Products.Where(product => product.ID >= idToDeleteFrom && product.ID <= idToDeleteTill).ExecuteDeleteAsync();

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = new { DeleteCount = productsToDeleteCount, MissingProductIds = missingBookIds },
                Message = $"{productsToDeleteCount} Products Deleted!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> DeleteProductByIdAsync(int id)
        {
            if (id < 1 || id == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id Is Invalid!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.ID == id);

            if (product is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Is Null!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            this._productDbContext.Products.Remove(product);
            await this._productDbContext.SaveChangesAsync();

            /* Invalidate (Delete) the cache */
            await this._cacheService.RemoveDataAsync(key: $"PRODUCT_{id}");
            await this._cacheService.RemoveDataAsync(key: ALL_PRODUCTS_CACHE_KEY);

            /* Publish the message to the queue to notify the other micro services */
            await this._publishEndpoint.Publish(message: product);

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = product,
                Message = "Product Deleted!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> GetAllProductsAsync(string? columnName = null, string? filterKeyWord = null)
        {
            var products = await this._productDbContext.Products.AsNoTracking().ToListAsync();

            if (products is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Products Are Null",
                };

                return new ResponseDto()
                {
                    Result = null,
                    IsSuccess = false,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            if (products.Count is 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Not Found!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = 0,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            ProductDto productDto = new();

            IList<ProductDto> productDtos = new List<ProductDto>();

            var filteredProducts = this._filterService.ApplyFilterOn(queryOn: products.AsQueryable(), columnName: columnName, filterKeyWord: filterKeyWord);

            if (filteredProducts.Any())
            {
                foreach (var product in filteredProducts)
                {
                    productDto = product.ConvertProductToProductDtoExtensionVersion1();

                    productDtos.Add(productDto);
                }

                return new ResponseDto()
                {
                    IsSuccess = true,
                    Result = productDtos,
                    Message = "Products Found!",
                    When = DateTime.Now,
                };
            }
            else
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Not Found With This Filter Keyword!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }
        }

        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            var cacheKey = $"PRODUCT_{id}";

            /* Initially, Check the cache for the product */

            var getProductFromCache = await this._cacheService.GetDataAsync<ProductModel>(key: cacheKey);

            if (getProductFromCache is not null)
            {
                return new ResponseDto()
                {
                    IsSuccess = true,
                    Result = getProductFromCache,
                    Message = "Product Found In Cache!",
                    When = DateTime.Now,
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.ID == id);

            if (product is null)
            {
                ApplicationError applicationError = new()
                {
                    Message = "Product Is Null!",
                    When = DateTime.Now,
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            /* Add the product to the cache */
            await this._cacheService.SetDataAsync<ProductModel>(key: cacheKey, data: product, absoluteExpireTime: TimeSpan.FromMinutes(5));

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = product,
                Message = "Product Found!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> UpdateProductByIdAsync(int id, UpdateProductDto updateProductDto)
        {
            if (updateProductDto is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Is Null!",
                };

                return (new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                });
            }

            if (updateProductDto.Name == string.Empty || updateProductDto.Price == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Detail Is Empty!",
                };

                return (new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                });
            }

            if (id < 1 || id == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id Is Invalid!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var isDuplicatedProductName = await IsProductDuplicatedAsync(updateProductDto: updateProductDto, excludeProductId: id);

            if (isDuplicatedProductName)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Is Duplicated!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            var product = await this._productDbContext.Products.FirstOrDefaultAsync(product => product.ID == id);

            if (product is null)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product is unavailable with this product ID!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            product.Name = updateProductDto.Name;
            product.Price = updateProductDto.Price;

            this._productDbContext.Products.Update(product);
            await this._productDbContext.SaveChangesAsync();

            /* Invalidate (Delete) the cache */
            await this._cacheService.RemoveDataAsync(key: $"PRODUCT_{id}");
            await this._cacheService.RemoveDataAsync(key: ALL_PRODUCTS_CACHE_KEY);

            var updatedProductDto = product.ConvertProductToProductDtoExtensionVersion1();

            /* Publish the message to the queue to notify the other micro services */
            await this._publishEndpoint.Publish(message: updatedProductDto);

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = updatedProductDto,
                Message = "Product Updated!",
                When = DateTime.Now,
            };
        }

        public async Task<ResponseDto> DeleteProductByIdInOneDatabaseHitAsync(int id)
        {
            if (id < 1 || id == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id Is Invalid!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            ProductModel product = new()
            {
                ID = id
            };

            if (id < 1 || id == 0)
            {
                ApplicationError applicationError = new()
                {
                    When = DateTime.Now,
                    Message = "Product Id Is Invalid!",
                };

                return new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = applicationError.Message,
                    When = applicationError.When,
                };
            }

            this._productDbContext.Entry<ProductModel>(product).State = EntityState.Deleted;
            await this._productDbContext.SaveChangesAsync();

            var deletedProductDto = product.ConvertProductToProductDtoExtensionVersion1();

            return new ResponseDto()
            {
                IsSuccess = true,
                Result = deletedProductDto,
                Message = "Product Deleted!",
                When = DateTime.Now,
            };
        }

        private async Task<bool> IsProductNameDuplicatedAsync(string productNameFromDto, int? excludeProductId = null)
        {
            var query = this._productDbContext.Products.Where(product => product.Name.ToLower() == productNameFromDto.ToLower());

            /* If an ID is provided (like during an update), exclude it from the duplicate check */
            if (excludeProductId.HasValue)
            {
                query = query.Where(product => product.ID != excludeProductId.Value);
            }

            return await query.AnyAsync();
        }

        private async Task<bool> IsProductDuplicatedAsync(UpdateProductDto updateProductDto, int? excludeProductId = null)
        {
            var query = this._productDbContext.Products.Where(product => product.Name.ToLower() == updateProductDto.Name.ToLower() && product.Price == updateProductDto.Price);

            /* If an ID is provided (like during an update), exclude it from the duplicate check */
            if (excludeProductId.HasValue)
            {
                query = query.Where(product => product.ID != excludeProductId.Value);
            }

            return await query.AnyAsync();
        }
    }
}