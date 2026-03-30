using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ProductDtos
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }

        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MinLength(5)]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 1000)]
        public int QuantityStock { get; set; }

        public int CategoryId { get; set; }
    }
}
