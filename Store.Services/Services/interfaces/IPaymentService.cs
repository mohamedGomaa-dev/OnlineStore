using Store.Services.Dtos.PaymentDtos;
using Store.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Services.interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentDto?>> GetPaymentByOrderIdAsync(int orderId);
        Task<Result<PaymentDto>> ProcessPaymentAsync(PaymentCreateDto dto);
    }
}
