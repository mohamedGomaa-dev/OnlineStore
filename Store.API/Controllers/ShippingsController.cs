using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.API.Extensions;
using Store.Models.Entities;
using Store.Models.Enums;
using Store.Services.Dtos.ShippingDtos;
using Store.Services.Services.interfaces;

namespace Store.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        private readonly IShippingService _shippingService;
        public ShippingsController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShipping(ShippingCreateDto dto)
        {
            var result = await _shippingService.CreateShippingAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return CreatedAtAction(nameof(GetShippingByOrderId),new {orderId = result.Data?.OrderId},result.Data);
        }

        [HttpGet("order/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShippingByOrderId([FromRoute] int orderId)
        {
            var result = await _shippingService.GetShippingByOrderIdAsync(orderId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            
            return Ok(result.Data);
        }

        [Authorize(Roles ="Admin")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateShippingStatus([FromRoute] int id, [FromQuery] ShippingStatus newStatus)
        {
            var result = await _shippingService.UpdateShippingStatusAsync(id, newStatus);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return NoContent();
        }


    }
}
