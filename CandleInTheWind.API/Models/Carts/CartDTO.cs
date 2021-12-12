using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Carts
{
    public class CartDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
