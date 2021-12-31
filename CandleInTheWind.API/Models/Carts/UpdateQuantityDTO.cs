namespace CandleInTheWind.API.Models.Carts
{
    public class UpdateQuantityDTO
    {
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
