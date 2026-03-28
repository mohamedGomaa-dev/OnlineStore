using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.CateogryDtos
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [MinLength(5)]
        public string Name { get; set; } = string.Empty;
    }
}
