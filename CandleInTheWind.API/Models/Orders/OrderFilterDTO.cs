using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderFilterDTO
    {
        public IEnumerable<OrderDTO> Orders { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
