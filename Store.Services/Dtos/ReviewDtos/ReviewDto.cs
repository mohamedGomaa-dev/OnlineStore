using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public string? ReviewText { get; set; }

        public decimal Rating { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
