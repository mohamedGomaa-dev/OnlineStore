using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Extensions;
using Store.DataAccess.Helpers;
using Store.Services.Dtos.ReviewDtos;
using Store.Services.Services.implementations;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDto dto)
        {
            // to add a review you must be the user that is sent with the dto
            var currentUserId = User.GetUserId();
            if (dto.UserId != currentUserId && !User.IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to review.");
            }
            var result = await _reviewService.AddReviewAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            
            return Ok(result.Data);
        }

        [AllowAnonymous] // without logging in you could see the reviews of a certain product
        [HttpGet("product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews([FromRoute] int productId)
        {
            var result = await _reviewService.GetProductReviewsAsync(productId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }

        [AllowAnonymous]
        [HttpGet("product/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductReviews([FromQuery] ReviewQuery query)
        {
            var result = await _reviewService.GetProductReviewsAsync(query);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
