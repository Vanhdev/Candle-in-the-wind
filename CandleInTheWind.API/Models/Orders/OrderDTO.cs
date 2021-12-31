using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderDTO : SimpleOrderDTO
    {
        public IEnumerable<string> ProductName { get; set; }
    }
}
