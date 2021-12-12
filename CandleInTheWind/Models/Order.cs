using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Order
    {
        public decimal Id { get; set; }
        public decimal CustomerId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal? VoucherId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ProductId { get; set; }
        public int Quantity { get; set; }
        public byte Status { get; set; }
        public decimal Total { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}
