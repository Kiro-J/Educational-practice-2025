using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Domain.ModelsDb
{
    [Table("Delivery")]
    public class DeliveryDb
    {
        [Key]
        [Column("delivery_id")]
        public Guid DeliveryId { get; set; }

        [ForeignKey("Order")]
        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("delivery_address")]
        public string DeliveryAddress { get; set; }

        [Column("delivery_date")]
        public DateOnly DeliveryDate { get; set; }

        [Column("delivery_type")]
        [MaxLength(50)]
        public string DeliveryType { get; set; }

        [Column("created_at")]
        public TimeOnly CreatedAt { get; set; }

        // Navigation properties
        public virtual OrderDb Order { get; set; }
    }
}
