using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Cart
    {
        public decimal CustomerId { get; set; }
        public decimal ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
