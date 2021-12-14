using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderDTO : SimpleOrderDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<int> ProductIDs { get; set; }
        public IEnumerable<string> ProductImageUrls { get; set; }
        public IEnumerable<decimal> UnitPrices { get; set; }
        public IEnumerable<int> Quantity { get; set; }
        public int? VoucherId { get; set; }
        public double? VoucherValue { get; set; }
        public string VoucherName { get; set; } = " ";

    }
}
