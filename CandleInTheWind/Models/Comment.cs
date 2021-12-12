using System;
using System.Collections.Generic;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class Comment
    {
        public decimal Id { get; set; }
        public decimal CustomerId { get; set; }
        public string Content { get; set; }
        public decimal PostId { get; set; }
        public DateTime Time { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Post Post { get; set; }
    }
}
