using System.Collections.Generic;

namespace CandleInTheWind.API.Models.Orders
{
    public class OrderDetailDTO : SimpleOrderDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public IEnumerable<OrderProductDTO> Products { get; set; }

        public int? VoucherId { get; set; }

        public double? VoucherValue { get; set; }

        public string VoucherName { get; set; } = string.Empty;

        public int Points = 0;
    }
}
