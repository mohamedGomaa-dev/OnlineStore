using Store.Models.Entities;
using Store.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ShippingDtos
{
    public class ShippingDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public string CarrierName { get; set; } = string.Empty;

        public string TrackingNumber { get; set; } = string.Empty;

        public ShippingStatus ShippingStatus { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
