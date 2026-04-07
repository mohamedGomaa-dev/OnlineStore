using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _imageService;
        private readonly IWebHostEnvironment _env; 
        public ProductImagesController(IProductImageService imageService, IWebHostEnvironment env)
        {
            _imageService = imageService;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage([FromRoute] int productId, IFormFile file, [FromForm] int imageOrder)
        {
            // check the file
            if (file == null || file.Length == 0) return BadRequest("Please upload a valid image");

            // prepare save folder
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            
            // generate uniqe path
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // save to hard disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // prepare url that front end will see
            var imageUrl = $"/images/products/{uniqueFileName}";

            // call service to add image
            var result = await _imageService.AddProductImageAsync(productId, imageUrl, imageOrder);

            if (!result.IsSuccess) return BadRequest(result.Message);

            return Ok(result.Data);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{imageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage([FromRoute]int imageId)
        {
            var result = await _imageService.DeleteProductImageAsync(imageId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            if (result.Data != null)
            {
                var filePath = Path.Combine(_env.WebRootPath ?? Directory.GetCurrentDirectory(), "wwwroot", result.Data.TrimStart('/'));


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            return NoContent();
        }
    }
}
