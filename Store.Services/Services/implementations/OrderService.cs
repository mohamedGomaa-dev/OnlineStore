using AutoMapper;
using Microsoft.Extensions.Logging;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Models.Enums;
using Store.Services.Dtos.OrderDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto)
        {
            /*
             *  1. check if items list is not empty
             *  2. check if user id exists in db
             *  3. create a new order that has a status (pending) and the order date is now
             *  4. loop through order items in the order dto
             *  5. for each item, get its product from the db
             *  6. if the product doesn't exist, return not found
             *  7. check if the quantity is less than or equal to the product quantity in stock
             *  8. if not enough, return faliure, if enough decrease the quantity stock
             *  9. create a new item with price = product.price and total items price = item.quantity * item.price
             *  10. order total amount = item.totalitemsprice for each item
             *  11. commit changes
             *  12. return order dto (not the create dto but one that is readable)
             */
            if (dto.Items is null)
            {
                _logger.LogDebug("Null Items: {Items} are empty", nameof(dto.Items));
                return Utility.Failure<OrderDto>("items are empty");
            }
            if (dto.Items.Count <= 0)
            {
                return Utility.Failure<OrderDto>("items are empty");
            }

            if (!await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId))
            {
                return Utility.Failure<OrderDto>($"user with id: {dto.UserId} doesn't exist");
            }

            var order = new Order
            {
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                UserId = dto.UserId,
                TotalAmount = 0,
                OrderItems = new List<OrderItem>()

            };

            // group items of the same product to calculate the amount and stock correctly
            var groupedItems = dto.Items
    .GroupBy(i => i.ProductId)
    .Select(g => new OrderItemCreateDto
    {
        ProductId = g.Key,
        Quantity = g.Sum(i => i.Quantity)
    })
    .ToList();
            foreach (var item in groupedItems)
            {
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == item.ProductId);
                if (product is null)
                {
                    return Utility.Failure<OrderDto>($"product with id: {item.ProductId} not found");
                }
                // the dto includes annotation for the quantity of item to have more than 0
                if (item.Quantity > product.QuantityStock)
                {
                    return Utility.Failure<OrderDto>($"stock of product is not enough for the quantity stock you ordered");
                } else
                {
                    product.QuantityStock -= item.Quantity;
                }

                var newItem = new OrderItem
                {
                    Price = product.Price,
                    TotalItemsPrice = item.Quantity * product.Price,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                };
                order.OrderItems.Add(newItem);
                order.TotalAmount += newItem.TotalItemsPrice;
               
            }
            await _unitOfWork.Orders.AddAsync(order);

            await _unitOfWork.CommitChanges();
            _logger.LogInformation("Order Created: order created with id: {OrderId}", order.Id);

            return Utility.Success<OrderDto>("order created successfully", _mapper.Map<OrderDto>(order));
        }

        public async Task<Result<OrderDto?>> GetOrderByIdAsync(int id)
        {
            // check if id is valid > 0
            // check if the order exists
            // return failure if not
            // return the order dto if it exists
            if (id <= 0)
            {
                return Utility.Failure<OrderDto?>("please enter a valid id");
            }
            var order = await _unitOfWork.Orders.GetOrderByIdWithItemsAsync(id);
            if (order is null)
            {
                return Utility.Failure<OrderDto?>($"order with id: {id} not found");

            }
            return Utility.Success<OrderDto?>("found order!", _mapper.Map<OrderDto>(order));   
        }

        public async Task<Result<IEnumerable<OrderDto>>> GetUserOrdersAsync(int userId)
        {
            // check if user exists
            // if user doesn't exist send faliure not found
            // return a list of the orders that match the user id

            if (!await _unitOfWork.Users.ExistsAsync(u => u.Id == userId))
            {
                return Utility.Failure<IEnumerable<OrderDto>>($"user with id: {userId} not found");
            }
            var orders = await _unitOfWork.Orders.GetAllAsync(o => o.UserId == userId, "OrderItems.Product", "User");

            return Utility.Success<IEnumerable<OrderDto>>("success", _mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        public async Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            // search for order
            // if not exists return not found
            // if exists change status to new status
            // commit changes
            // return success
            var order = await _unitOfWork.Orders.GetAsync(o => o.Id == orderId);
            if (order is null)
            {
                return Utility.Failure($"order with id: {orderId} not found");
            }
            order.Status = newStatus;
            await _unitOfWork.CommitChanges();
            return Utility.Success("status updated successfully");
        }
    }
}
