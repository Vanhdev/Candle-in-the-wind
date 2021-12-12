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
        public DateTime PurchasedDate { get; set; } = DateTime.Now;

        [Required]
        [EnumDataType(typeof(OrderStatus))]
        [Column(TypeName = "tinyint")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        public int? VoucherId { get; set; }
        public virtual Voucher Voucher { get; set; } = null;

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
