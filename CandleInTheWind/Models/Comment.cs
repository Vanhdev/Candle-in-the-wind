using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandleInTheWind.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        [Required]
        public DateTime Time { get; set; } = DateTime.Now;

        [Required]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
