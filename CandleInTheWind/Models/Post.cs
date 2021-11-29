using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandleInTheWind.Models
{
    public enum PostStatus
    {
        NotApprovedYet = 0,
        Approved,
        NotApproved,
    }

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        public PostStatus Status { get; set; } = PostStatus.NotApprovedYet;

        [Required]
        public bool Commentable { get; set; } = true;

        public virtual User User { get; set; }
    }
}
