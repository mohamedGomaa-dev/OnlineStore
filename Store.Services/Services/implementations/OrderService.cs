using AutoMapper;
using Store.DataAccess.Units.interfaces;
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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Result<OrderDto>> CreateOrderAsync(OrderCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<OrderDto?>> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<OrderDto>>> GetUserOrdersAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
