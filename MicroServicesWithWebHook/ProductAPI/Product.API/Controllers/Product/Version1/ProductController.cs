using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Product.API.Repository.ProductServices.Version1.Services;
using Product.API.ServerSideValidation;
using Shared.Data.DTOs.ProductDTOs.Version1;

namespace Product.API.Controllers.Product.Version1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this._productService = productService;
            this._logger = logger;
        }

        #region Version 1
        [AllowAnonymous]
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllProductsVersion1([FromQuery] string? filterOnColumn, [FromQuery] string? filterKeyWord)
        {
            var response = await this._productService.GetAllProductsAsync(columnName: filterOnColumn, filterKeyWord: filterKeyWord);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to retrieve products: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetProductByIdVersion1(int id)
        {
            var response = await this._productService.GetProductByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to retrieve product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ModelValidation]
        //[ValidateAntiForgeryToken]
        [MapToApiVersion("1.0")]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> CreateProductVersion1([FromBody] AddProductDto addProductDto)
        {
            var response = await this._productService.AddProductAsync(addProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to create product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("{id:int}")]
        [ModelValidation]
        //[ValidateAntiForgeryToken]
        [MapToApiVersion("1.0")]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> UpdateProductVersion1(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var response = await this._productService.UpdateProductByIdAsync(id, updateProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to update product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:int}")]
        //[ValidateAntiForgeryToken]
        [MapToApiVersion("1.0")]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> DeleteProductVersion1(int id)
        {
            var response = await this._productService.DeleteProductByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to delete product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        #endregion        
    }
}
