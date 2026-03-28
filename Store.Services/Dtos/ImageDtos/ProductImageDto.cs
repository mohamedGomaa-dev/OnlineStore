using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ImageDtos
{
    public class ProductImageDto
    {
        public int Id { get; set; }

        [MaxLength(400)]
        public string ImageURL { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public int ImageOrder { get; set; }
    }
}
