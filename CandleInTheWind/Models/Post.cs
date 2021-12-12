using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public decimal Id { get; set; }
        public decimal CustomerId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Content { get; set; }
        public byte Status { get; set; }
        public bool? Commentable { get; set; }
        public string Title { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
