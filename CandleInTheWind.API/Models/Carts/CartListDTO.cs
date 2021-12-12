using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Carts
{
    public class CartListDTO
    {
        //public int UserId { get; set; }
        public IEnumerable<CartDTO> Products { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
