using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Domain.Models
{
    public class Take
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid InventoryId { get; set; }
        public int Quantity { get; set; }
        public DateTime TakenAt { get; set; }
    }
}
