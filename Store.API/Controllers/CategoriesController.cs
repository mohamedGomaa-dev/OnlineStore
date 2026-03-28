using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Helpers;
using Store.Services.Dtos.CateogryDtos;
using Store.Services.Dtos.UserDtos;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories([FromQuery] CategoryQuery query)
        {
            var result = await _categoryService.GetAllCategoriesAsync(query);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _categoryService.AddCategoryAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _categoryService.UpdateCategoryAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
