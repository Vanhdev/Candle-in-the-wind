using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CandleInTheWind.Models;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status{ get; set; }
        public decimal Total { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? VoucherId { get; set; }
        public string? VoucherName { get; set; }
        

    }
}
