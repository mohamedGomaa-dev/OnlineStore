using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Extensions;
using Store.Models.Entities;
using Store.Models.Enums;
using Store.Services.Dtos.OrderDtos;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (dto.UserId != User.GetUserId() && !User.IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You can only order your own orders.");
            }
            var result = await _orderService.CreateOrderAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetOrderById), new {id = result.Data?.Id},result.Data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {

            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            if (result.Data?.UserId != User.GetUserId() && !User.IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You can only view your own orders.");
            }
            return Ok(result.Data);
        }

        [HttpGet("User/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserOrders([FromRoute] int userId)
        {
            if (userId != User.GetUserId() && !User.IsAdmin())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You can only view your own orders.");
            }
            var result = await _orderService.GetUserOrdersAsync(userId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }

        [Authorize(Roles ="Admin")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromQuery] OrderStatus newStatus)
        {

            var result = await _orderService.UpdateOrderStatusAsync(id, newStatus);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return NoContent();
        }
    }
}
