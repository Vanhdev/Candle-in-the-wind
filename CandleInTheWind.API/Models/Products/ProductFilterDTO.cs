using System.Collections.Generic;

namespace CandleInTheWind.API.Models.Products
{
    public class ProductFilterDTO
    {
        public IEnumerable<ProductDTO> Products { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
