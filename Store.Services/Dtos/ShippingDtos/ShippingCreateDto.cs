using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ShippingDtos
{
    public class ShippingCreateDto
    {
        [MaxLength(100)]
        public string CarrierName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TrackingNumber { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }

    }
}
