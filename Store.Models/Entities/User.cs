using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Entities
{
    public class User: IBaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // navigation properties
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }

    }
}
