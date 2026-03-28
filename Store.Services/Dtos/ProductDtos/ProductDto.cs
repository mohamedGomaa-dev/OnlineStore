using Store.Models.Entities;
using Store.Services.Dtos.CateogryDtos;
using Store.Services.Dtos.ImageDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ProductDtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int QuantityStock { get; set; }

        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public ICollection<ProductImageDto>? ProductImages { get; set; }
    }
}
