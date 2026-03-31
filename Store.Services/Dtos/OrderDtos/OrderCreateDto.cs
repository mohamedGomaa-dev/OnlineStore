using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.OrderDtos
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }

        public List<OrderItemCreateDto>? Items { get; set; }
    }
}
