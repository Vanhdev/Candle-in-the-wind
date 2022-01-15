using System.ComponentModel.DataAnnotations;

namespace CandleInTheWind.API.Models.Comments
{
    public class CommentCreateDTO
    {
        [Required]
        public string Content { get; set; }
    }
}
