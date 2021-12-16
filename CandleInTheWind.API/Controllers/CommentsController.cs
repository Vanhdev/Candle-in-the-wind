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
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments()
        {
            var comments = await _context.Comments.Include(comment => comment.User).Include(comment => comment.Post).ToListAsync();
            var responseComments = comments.Select(comment => toDTO(comment));
            return Ok(responseComments);
        }

        // GET: api/Comments/5
        [HttpGet("{PostId}")]
        public async Task<ActionResult<CommentDTO>> GetComment(int PostId)
        {
            var post = _context.Posts.FirstOrDefault(post => post.Id == PostId && post.Status == PostStatus.Approved);
            if (post == null) return NotFound();

            var comments = await _context.Comments.Include(comment => comment.User)
                                                  .Where(comment => comment.Post.Id == PostId)
                                                  .ToListAsync();

            var responseComments = comments.Select(comment => toDTO(comment));

            return Ok(responseComments);
        }

        //Delete comment
        [HttpDelete("Post/{PostId}/Comment/{CommentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int PostId, int CommentId)
        {

            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var post = await _context.Posts.FindAsync(PostId);

            if (post == null) 
                return NotFound(new {Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });    

            var userId = int.Parse(userIdClaim.Value);
            
            var comment = await _context.Comments.Include(comment => comment.Post)
                                                 .Include(comment => comment.User)
                                                 .Where(comment => comment.Post.Id == PostId && comment.User.Id == userId)
                                                 .FirstOrDefaultAsync(comment => comment.Id == CommentId);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Edit comment
        [HttpPut("Post/{PostId}/Comment/{CommentId}")]
        [Authorize]
        public async Task<IActionResult> PutComment(int PostId, int CommentId, [FromBody] string Content)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var post = await _context.Posts.FindAsync(PostId);

            if (post == null)
                return NotFound(new { Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });

            var userId = int.Parse(userIdClaim.Value);

            var comment = await _context.Comments.Include(comment => comment.Post)
                                                 .Include(comment => comment.User)
                                                 .Where(comment => comment.Post.Id == PostId && comment.User.Id == userId)
                                                 .FirstOrDefaultAsync(comment => comment.Id == CommentId);

            if(comment == null)  
                return NotFound();

            comment.Content = Content;
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(CommentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost("Post/{PostId}/Comments")]
        [Authorize]
        public async Task<ActionResult<Comment>> AddComment([FromRoute]int PostId,[FromBody] CommentCreateDTO dto)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            
            var userId = int.Parse(userIdClaim.Value);
            var post = await _context.Posts.FindAsync(PostId);
            
            if (post == null)
                return NotFound(new { Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });

            if (post.Commentable == false)
                return Ok("Bài viết đã khoá bình luận");


            var comment = new Comment
            {
                Content = dto.Content,
                PostId = post.Id,
                UserId = userId,
            };
            


            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
            return Ok("Tao cmt thanh cong!");
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

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

    }
}
