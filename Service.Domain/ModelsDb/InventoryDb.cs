using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Domain.ModelsDb
{
    [Table("Inventory")]
    public class InventoryDb
    {
        public InventoryDb()
        {
            ItemName = string.Empty;
            ItemCategory = string.Empty;
            Description = string.Empty;
            Location = string.Empty;
            Orders = new HashSet<OrderDb>();
        }

        [Key]
        [Column("item_id")]
        public Guid ItemId { get; set; }

        [Required]
        [Column("item_name")]
        [MaxLength(200)]
        public required string ItemName { get; set; }

        [Column("item_category")]
        [MaxLength(100)]
        public required string ItemCategory { get; set; }

        [Column("description")]
        public required string Description { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_price", TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Column("location")]
        [MaxLength(100)]
        public required string Location { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDb> Orders { get; set; }
    }
}