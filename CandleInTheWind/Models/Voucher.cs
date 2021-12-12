using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Orders = new HashSet<Order>();
        }

        public decimal Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expired { get; set; }
        public double Value { get; set; }
        public int Quantity { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
