using Store.Models.Enums;
using Store.Services.Dtos.OrderDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IOrderService
    {

        // the customer will be creating orders with specific items
        Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto);
        
        // getting an existnig order by its id
        Task<Result<OrderDto?>> GetOrderByIdAsync(int id);
        // get all orders of a specific order (my orders as a function for a user)
        Task<Result<IEnumerable<OrderDto>>> GetUserOrdersAsync(int userId);

        // cancelling or making an order have a new status
        Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
