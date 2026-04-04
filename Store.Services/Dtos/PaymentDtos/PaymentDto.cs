using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.PaymentDtos
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; }
    }
}
