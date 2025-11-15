using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Domain.ModelsDb
{
    [Table("Take")]
    public class TakeDb
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("reception_date")]
        public DateOnly ReceptionDate { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("payment_status_text")]
        [MaxLength(50)]
        public string PaymentStatusText { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual UserDb User { get; set; }
    }
}