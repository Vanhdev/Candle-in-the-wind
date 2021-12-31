using System.Collections.Generic;

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
