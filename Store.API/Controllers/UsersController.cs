using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Store.API.Extensions;
using Store.DataAccess.Helpers;
using Store.Services.Dtos.UserDtos;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        [EnableRateLimiting(policyName: "SlidingWindow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQuery query)
        {
            var result = await _userService.GetAllUsersAsync(query);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                if (result.Data is null)
                {
                    return NotFound(result.Message);
                }
                return BadRequest(result.Message);
            }
            var currentUserId = User.GetUserId();
            if (result.Data?.Id != currentUserId && !User.IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view this user.");
            }


            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.AddUserAsync(dto, dto.Password);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.Data.Id }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.UpdateUserAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting(policyName: "ApiUserPolicy")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
