using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Product.API.Repository.Version1.Services;
using Product.API.ServerSideValidation;
using Shared.Data.DTOs.ProductDTOs.Version1;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            this._productService = productService;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await this._productService.GetAllProductsAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to retrieve products: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await this._productService.GetProductByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to retrieve product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPost]
        [ModelValidation]
        //[ValidateAntiForgeryToken]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductDto addProductDto)
        {
            var response = await this._productService.AddProductAsync(addProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to create product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpPut("{id:int}")]
        [ModelValidation]
        //[ValidateAntiForgeryToken]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var response = await this._productService.UpdateProductByIdAsync(id, updateProductDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to update product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpDelete("{id:int}")]
        //[ValidateAntiForgeryToken]
        [EnableRateLimiting(policyName: "StrictPolicy")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await this._productService.DeleteProductByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            this._logger.LogError("Failed to delete product: {Message}", response.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
