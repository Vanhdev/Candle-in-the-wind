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
using CandleInTheWind.API.Extensions;

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

        // GET: api/Comments/5
        [HttpGet("{postId}")]
        public async Task<ActionResult> GetComment(int postId)
        {
            var post = _context.Posts.FirstOrDefault(post => post.Id == postId && post.Status == PostStatus.Approved);
            if (post == null) return NotFound();

            var comments = await _context.Comments.Include(comment => comment.User)
                                                  .Where(comment => comment.Post.Id == postId)
                                                  .ToListAsync();

            var responseComments = comments.Select(comment => comment.ToDTO());

            return Ok(responseComments);
        }

        //Delete comment
        // DELETE: api/Comments/Post/1/Comment/4
        [HttpDelete("Post/{PostId}/Comment/{CommentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var post = await _context.Posts.FindAsync(postId);

            if (post == null) 
                return NotFound(new {Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });    

            var userId = int.Parse(userIdClaim.Value);
            
            var comment = await _context.Comments.Include(comment => comment.Post)
                                                 .Include(comment => comment.User)
                                                 .Where(comment => comment.Post.Id == postId && comment.User.Id == userId)
                                                 .FirstOrDefaultAsync(comment => comment.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Edit comment
        // PUT: api/Comments/Post/1/Comment/4
        [HttpPut("Post/{PostId}/Comment/{CommentId}")]
        [Authorize]
        public async Task<IActionResult> PutComment(int postId, int commentId, [FromBody] string content)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
                return NotFound(new { Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });

            var userId = int.Parse(userIdClaim.Value);

            var comment = await _context.Comments.Include(comment => comment.Post)
                                                 .Include(comment => comment.User)
                                                 .Where(comment => comment.Post.Id == postId && comment.User.Id == userId)
                                                 .FirstOrDefaultAsync(comment => comment.Id == commentId);

            if(comment == null)  
                return NotFound();

            comment.Content = content;
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(commentId))
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

        // POST: api/Comments/1
        [HttpPost("{postId}")]
        [Authorize]
        public async Task<ActionResult> AddComment([FromRoute]int postId, [FromBody] CommentCreateDTO dto)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            
            var userId = int.Parse(userIdClaim.Value);
            var post = await _context.Posts.FindAsync(postId);
            
            if (post == null)
                return NotFound(new { Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });

            if (post.Commentable == false)
                return BadRequest(new { Error = "Bài viết đã khoá bình luận" });

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = post.Id,
                UserId = userId,
            };
            
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(GetComment), new { postId = comment.Id });
            return Ok(new { Message = "Tạo comment thành công!" });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

    }
}
