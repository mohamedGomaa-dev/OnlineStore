using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Entities
{
    public class ProductImage : IBaseEntity
    {
        public int Id { get; set; }

        [MaxLength(400)]
        public string ImageURL { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int ImageOrder { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
