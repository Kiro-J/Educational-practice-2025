using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Domain.ModelsDb
{
    [Table("Orders")]
    public class OrderDb
    {
        [Key]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("total_amount", TypeName = "money")]
        public decimal TotalAmount { get; set; }

        [Column("payment_method")]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Column("payment_status")]
        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        [ForeignKey("Inventory")]
        [Column("item_id")]
        public Guid ItemId { get; set; }

        // Navigation properties
        public virtual UserDb User { get; set; }
        public virtual InventoryDb Inventory { get; set; }
        public virtual DeliveryDb Delivery { get; set; }
    }
}