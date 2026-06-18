using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Store.DataAccess.Helpers;
using Store.Services.Dtos.ProductDtos;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly IProductImageService _imageService;
        private readonly IOutputCacheStore _cache;
        public ProductsController(IProductService productService, IProductImageService imageService, IOutputCacheStore cache)
        {
            _productService = productService;
            _imageService = imageService;
            _cache = cache;
        }

        [AllowAnonymous] // you could navigate through products
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [OutputCache(PolicyName = "DefaultCachePolicy")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQuery query)
        {
            var result = await _productService.GetAllProductsAsync(query);

            return Ok(result);
        }
        
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [OutputCache(PolicyName = "SingleProduct")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Data is null)
                {
                    return NotFound(result.Message);
                }
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddProductAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            await _cache.EvictByTagAsync("products", default);
            return CreatedAtAction(nameof(GetProduct),new {id = result.Data?.Id},result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDto dto)
        {
            var result = await _productService.UpdateProductAsync(dto);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            await _cache.EvictByTagAsync("products", default);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {

            var result = await _productService.DeleteProductAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            await _cache.EvictByTagAsync("products", default);

            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("{id}/images")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductImages([FromRoute] int id)
        {
            var result = await _imageService.GetProductImagesAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Data is null)
                {
                    return NotFound(result.Message);
                }
                return BadRequest(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
