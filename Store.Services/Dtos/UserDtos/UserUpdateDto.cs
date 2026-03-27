using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        [MinLength(5)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MinLength(5)]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MinLength(5)]
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MinLength(5)]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MinLength(5)]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

    }
}
