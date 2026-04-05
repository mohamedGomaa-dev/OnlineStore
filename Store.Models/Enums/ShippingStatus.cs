using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Enums
{
    public enum ShippingStatus
    {
        Processing,
        OutForDelivery,
        Delivered,
        ReturnToSender,
        OnHold,
        Delayed,
        Lost
    }
}
