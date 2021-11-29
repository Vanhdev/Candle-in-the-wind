using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandleInTheWind.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Approved,
        NotApproved,
        Canceled
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime PurchasedDate { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        public virtual Voucher Voucher { get; set; } = null;
        
        public virtual User User { get; set; }
        
        public virtual Product Product { get; set; }
    }
}
