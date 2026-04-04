using AutoMapper;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Services.Dtos.PaymentDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<PaymentDto?>> GetPaymentByOrderIdAsync(int orderId)
        {
            // get the payment where order id equalt the parameter that we sent
            // if true return success with the payment mapped to the dto
            // if false return faliure
            var payment = await _unitOfWork.Payments.GetAsync(p => p.OrderId == orderId);
            if (payment == null)
            {
                return Utility.Failure<PaymentDto?>($"payment for order with id: {orderId} not found");
            }

            return Utility.Success<PaymentDto?>("found!", _mapper.Map<PaymentDto>(payment));
        }

        public async Task<Result<PaymentDto>> ProcessPaymentAsync(PaymentCreateDto dto)
        {
            // does order id exist?
            // does order has an existing payment before
            // does the amount in the payment equal the total amount in the order, if not then faliure
            // create payment object where transaction date = datetime.utcnow
            // once paid, order status will become processing
            // save changes 
            var order = await _unitOfWork.Orders.GetAsync(o => o.Id == dto.OrderId);
            if (order is null)
            {
                return Utility.Failure<PaymentDto>($"order with id: {dto.OrderId} doesn't exist");
            }
            if (await _unitOfWork.Payments.ExistsAsync(p => p.OrderId == dto.OrderId))
            {
                return Utility.Failure<PaymentDto>($"order with id: {dto.OrderId} is already payed!");
            }
            if (order.TotalAmount != dto.Amount)
            {
                return Utility.Failure<PaymentDto>("order amount doesn't equal payment amount");
            }
            var payment = _mapper.Map<Payment>(dto);
            payment.TransactionDate = DateTime.UtcNow;

            await _unitOfWork.Payments.AddAsync(payment);
            order.Status = Models.Enums.OrderStatus.Processing;
            await _unitOfWork.CommitChanges();
            return Utility.Success<PaymentDto>("order payed successfully", _mapper.Map<PaymentDto>(payment));

        }
    }
}
