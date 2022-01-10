using System.Collections.Generic;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderDTO : SimpleOrderDTO
    {
        public IEnumerable<string> ProductName { get; set; }
    }
}
