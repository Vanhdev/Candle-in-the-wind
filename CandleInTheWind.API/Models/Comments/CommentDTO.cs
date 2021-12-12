using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
