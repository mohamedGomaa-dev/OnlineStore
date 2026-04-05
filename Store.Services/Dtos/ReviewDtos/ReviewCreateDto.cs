using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.ReviewDtos
{
    public class ReviewCreateDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [MaxLength(500)]
        public string? ReviewText { get; set; }

        [Range(1, 5)]
        public decimal Rating { get; set; }
    }
}
