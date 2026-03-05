using Shared.Data.Models.ProductModel;

namespace Shared.Data.Mapper.ProductMapper
{
    public static class ProductMapping
    {
        #region Version 1        
        /// <summary>
        /// Converts the product to product dto extension version1.
        /// </summary>
        /// <param name="Product">The product.</param>
        /// <returns></returns>
        public static DTOs.ProductDTOs.Version1.ProductDto ConvertProductToProductDtoExtensionVersion1(this Product Product)
        {
            return new DTOs.ProductDTOs.Version1.ProductDto()
            {
                Name = Product.Name,
                Price = Product.Price,
            };
        }

        /// <summary>
        /// Converts the product dto to product extension version1.
        /// </summary>
        /// <param name="ProductDto">The product dto.</param>
        /// <returns></returns>
        public static Product ConvertProductDtoToProductExtensionVersion1(this DTOs.ProductDTOs.Version1.ProductDto ProductDto)
        {
            return new Product()
            {
                Name = ProductDto.Name,
                Price = ProductDto.Price,
            };
        }
        #endregion

        #region Version 2        
        /// <summary>
        /// Converts the product to product dto extension version2.
        /// </summary>
        /// <param name="Product">The product.</param>
        /// <returns></returns>
        public static DTOs.ProductDTOs.Version2.ProductDto ConvertProductToProductDtoExtensionVersion2(this Product Product)
        {
            return new DTOs.ProductDTOs.Version2.ProductDto()
            {
                Name = Product.Name,
                CostPrice = Product.Price,
            };
        }

        /// <summary>
        /// Converts the product dto to product extension version2.
        /// </summary>
        /// <param name="ProductDto">The product dto.</param>
        /// <returns></returns>
        public static Product ConvertProductDtoToProductExtensionVersion2(this DTOs.ProductDTOs.Version2.ProductDto ProductDto)
        {
            return new Product()
            {
                Name = ProductDto.Name,
                Price = ProductDto.CostPrice,
            };
        }
        #endregion
    }
}