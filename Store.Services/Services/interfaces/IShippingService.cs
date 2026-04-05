using Store.Models.Enums;
using Store.Services.Dtos.ShippingDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IShippingService
    {
        Task<Result<ShippingDto>> CreateShippingAsync(ShippingCreateDto dto);
        Task<Result<ShippingDto?>> GetShippingByOrderIdAsync(int orderId);
        Task<Result> UpdateShippingStatusAsync(int shippingId, ShippingStatus newStatus);
    }
}
