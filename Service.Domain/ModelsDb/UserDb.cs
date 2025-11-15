using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Domain.ModelsDb
{
    [Table("Users")]
    public class UserDb
    {
        public UserDb()
        {
            Takes = new HashSet<TakeDb>();
            Orders = new HashSet<OrderDb>();
        }

        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("login")]
        [MaxLength(100)]
        public required string Login { get; set; }

        [Required]
        [Column("password")]
        [MaxLength(255)]
        public required string Password { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Column("role")]
        public int Role { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<TakeDb> Takes { get; set; }
        public virtual ICollection<OrderDb> Orders { get; set; }
    }
}