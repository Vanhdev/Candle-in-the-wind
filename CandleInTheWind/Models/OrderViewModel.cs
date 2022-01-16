using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandleInTheWind.Models
{
    public class OrderViewModel
    {
        public int UserID { get; set; }
        public List<int> ProductID { get; set; }
        public List<int> Quantity { get; set; }
        public int? VoucherID { get; set; }
    }
}
