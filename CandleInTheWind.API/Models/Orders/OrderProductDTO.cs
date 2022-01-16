namespace CandleInTheWind.API.Models.Orders
{
    public class OrderProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; }
    }
}
