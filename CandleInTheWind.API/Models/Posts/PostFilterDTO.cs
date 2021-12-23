using System.Collections.Generic;


namespace CandleInTheWind.API.Models.Posts
{
    public class PostFilterDTO
    {
        public IEnumerable<PostDTO> Posts { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
