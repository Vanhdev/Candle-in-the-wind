using System;

namespace CandleInTheWind.API.Models.Comments
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content {get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }

    }
}
