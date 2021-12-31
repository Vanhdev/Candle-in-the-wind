using System;

namespace CandleInTheWind.API.Models.Posts
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool Commentable { get; set; }

        public int  UserId { get; set; }
        public string UserName { get; set; }
        public int CommentCount { get; set; }
    }
}
