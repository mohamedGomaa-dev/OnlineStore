using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ProductDtos
{
    public class ProductCreateDto
    {
        
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int QuantityStock { get; set; }

        public int CategoryId { get; set; }
        
    }
}
