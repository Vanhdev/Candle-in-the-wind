using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CandleInTheWind.API.Models.Orders
{
    public class SimpleOrderDTO
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        
        public int OrderStatus { get; set; }
        public decimal Total { get; set; }
        public string ProductName { get; set; }

    }
}
