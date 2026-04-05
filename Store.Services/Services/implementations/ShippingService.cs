using AutoMapper;
using Store.DataAccess.Repositories.interfaces;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using Store.Models.Enums;
using Store.Services.Dtos.ShippingDtos;
using Store.Services.Helpers;
using Store.Services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.implementations
{
    public class ShippingService : IShippingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ShippingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ShippingDto>> CreateShippingAsync(ShippingCreateDto dto)
        {
            // check if carrier name is white space or empty
            // check if tracking number is white space or empty
            // check if estimated delivery date is after now
            // check if order with order id exists
            // check if order has shipping before, if it has, we don't create another shipping
            // make the default shipping status processing
            // add the shipping to db
            // commit changes 
            // return success
            if (string.IsNullOrWhiteSpace(dto.TrackingNumber))
            {
                return Utility.Failure<ShippingDto>("enter a tracking number");
            }
            if (string.IsNullOrWhiteSpace(dto.CarrierName))
            {
                return Utility.Failure<ShippingDto>("enter a carrier name");
            }
            if (dto.EstimatedDeliveryDate <= DateTime.UtcNow)
            {
                // a quick solution would be to add 10 days to the estimation
                dto.EstimatedDeliveryDate = dto.EstimatedDeliveryDate.AddDays(10);
                //return Utility.Failure<ShippingDto>("enter a valid date");
            }
            if (!await _unitOfWork.Orders.ExistsAsync(o => o.Id == dto.OrderId))
            {
                return Utility.Failure<ShippingDto>($"order with id: {dto.OrderId} doesn't exist");
            }
            if (await _unitOfWork.Shippings.ExistsAsync(s => s.OrderId == dto.OrderId))
            {
                return Utility.Failure<ShippingDto>($"order with id: {dto.OrderId} already has a shipping");
            }

            var shipping = _mapper.Map<Shipping>(dto);
            shipping.ShippingStatus = ShippingStatus.Processing;
            await _unitOfWork.Shippings.AddAsync(shipping);
            await _unitOfWork.CommitChanges();
            return Utility.Success<ShippingDto>($"successfully created", _mapper.Map<ShippingDto>(shipping));
        }

        public async Task<Result<ShippingDto?>> GetShippingByOrderIdAsync(int orderId)
        {
            // get shipping from db
            // if null return faliure
            // return shipping if exists
            var shipping = await _unitOfWork.Shippings.GetAsync(s => s.OrderId == orderId);
            if (shipping is null)
            {
                return Utility.Failure<ShippingDto?>($"shipping for order id: {orderId}, not found");
            }

            return Utility.Success<ShippingDto?>("Success", _mapper.Map<ShippingDto>(shipping));
        }

        public async Task<Result> UpdateShippingStatusAsync(int shippingId, ShippingStatus newStatus)
        {
            // get the shipping
            // if null return faliure
            // if exists, update the status
            // if the status is delivered, update actual delivery date to utcNow
            // return success
            var shipping = await _unitOfWork.Shippings.GetAsync(s => s.Id == shippingId);
            if (shipping is null)
            {
                return Utility.Failure($"shipping with id: {shippingId}, not found");
            }
            shipping.ShippingStatus = newStatus;
            if (shipping.ShippingStatus == ShippingStatus.Delivered)
            {
                shipping.ActualDeliveryDate = DateTime.UtcNow;
            }

            _unitOfWork.Shippings.Update(shipping);
            await _unitOfWork.CommitChanges();
            return Utility.Success("Successfully updated status");

        }
    }
}
