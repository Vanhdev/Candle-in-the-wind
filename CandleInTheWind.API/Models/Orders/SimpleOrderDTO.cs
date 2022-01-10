using CandleInTheWind.Models;
using System;

namespace CandleInTheWind.API.Models.Orders
{
    public class SimpleOrderDTO
    {
        public int Id { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }

        public string StatusName { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
