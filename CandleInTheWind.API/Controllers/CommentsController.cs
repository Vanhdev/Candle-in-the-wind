using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.API.Models.Comments;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{PostId}")]
        public async Task<ActionResult<Comment>> GetComment(int PostId)
        {
            var post = _context.Posts.FirstOrDefault(post => post.Id == PostId && (int)post.Status == 1);
            if (post == null) return NotFound();

            var comments = await _context.Comments.Include(comment => comment.User)
                                                  .Where(comment => comment.Post.Id == PostId)
                                                  .ToListAsync();

            var responseComments = comments.Select(comment => toDTO(comment));

            return Ok(responseComments);
        }

        private CommentDTO toDTO(Comment comment)
        {
            return new CommentDTO
            {
                Id = comment.Id,
                UserId = comment.User.Id,
                UserName = comment.User.UserName,
                PostId = comment.Post.Id,
                Content = comment.Content,
                Time = comment.Time
            };
        }

    }
}
