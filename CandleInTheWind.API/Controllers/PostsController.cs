using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.API.Models.Posts;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using CandleInTheWind.API.Extensions;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts/AllPosts
        [HttpGet("AllPosts")]
        public async Task<ActionResult> GetAllApprovedPosts()
        {
            var posts = await _context.Posts.Include(posts => posts.User)
                                            .Where(post => post.Status == PostStatus.Approved)
                                            .ToListAsync();
            
            var postsResponse = posts.Select(post => post.ToDTO(_context));
            return Ok(postsResponse);
        }


        [HttpGet("Search")]
        public async Task<ActionResult> SearchPost(string searchText = "", int pageIndex = 1, int pageSize = 5)
        {
            if (pageSize <= 0) 
                pageSize = 5;
            if (pageIndex <= 0) 
                pageIndex = 1;

            if (searchText == null) 
                searchText = "";

            // get all approved post which title contains searchText 
            var posts = _context.Posts.Include(post => post.User)
                                      .Where(post => post.Title.Contains(searchText) && post.Status == PostStatus.Approved);
                                            
           
            
            int count = posts.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);

            if (pageIndex > totalPages) pageIndex = 1;

            var postsPerPage = await posts.OrderByDescending(post => post.ApprovedAt).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            var responsePosts = postsPerPage.Select(post => post.ToDTO(_context));

            return Ok(
                new PostFilterDTO
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    Posts = responsePosts,
                });

        }


        // GET: api/Posts?pageIndex=1&pageSize=5
        [HttpGet()]
        public async Task<ActionResult> GetPagedApprovedPost([FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 5)
        {
            if(pageIndex < 1) pageIndex = 1;
            if(pageSize < 0) pageSize = 1;

            var approvedPosts = _context.Posts.Include(post => post.User)
                                              .Where(post => post.Status == PostStatus.Approved);
            int count = approvedPosts.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);

            if (pageIndex > totalPages) pageIndex = 1;

            var postsPerPage = await approvedPosts.OrderByDescending(post => post.ApprovedAt).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            if (postsPerPage.Count == 0)
                return NotFound();

            var responsePosts = postsPerPage.Select(post => post.ToDTO(_context)); 

            return Ok(
                new PostFilterDTO
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    Posts = responsePosts,
                });
        }

        //GET: api/Posts/1
        [HttpGet("{postID}")]
        public async Task<ActionResult> GetPost(int postID)
        {
            var post = await _context.Posts.Include(post => post.User)
                                           .Where(post => post.Status == PostStatus.Approved)
                                           .FirstOrDefaultAsync(post => post.Id == postID);
            if (post == null) return NotFound(new {Error = "Không tìm thấy bài viết" });

            return Ok(post.ToDTO(_context));
        }
        
        // GET: api/Posts/MyPost
        [HttpGet("MyPost")]
        [Authorize]
        public async Task<ActionResult> GetPostByUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            var userId = int.Parse(userIdClaim.Value);

            var myposts = await _context.Posts
                                        .Include(post => post.User)
                                        .Where(post => post.User.Id == userId)
                                        .ToListAsync();
            var mypostsReponse = myposts.Select(post => post.ToDTO(_context));
            return Ok(mypostsReponse);
        }
        
        // POST: api/Posts
        [HttpPost("Posts")]
        [Authorize]
        public async Task<ActionResult> CreatePost([FromBody]PostCreateDTO dto)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null) return BadRequest();

            var userId = int.Parse(userIdClaim.Value);
            
            var user = await _context.Users.FindAsync(userId);

            var createdPost = new Post
            {
                User = user,
                Title = dto.Title,
                Content = dto.Content,
                ApprovedAt = null,
                Status = PostStatus.NotApprovedYet,
                Commentable = true,
                Comments = null
            };

            _context.Posts.Add(createdPost);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tạo bài viết thành công" });
        }
        
        // PUT: api/Posts/MyPost/4
        [HttpPut("MyPost/{PostId}"), Authorize]

        public async Task<ActionResult> ChangeCommentableState([FromRoute] int postId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var post = await _context.Posts.Include(post => post.User)
                                           .FirstOrDefaultAsync(post => post.User.Id == userId && post.Id == postId);

            if (post == null)
                return NotFound(new { Error = "Không tìm thấy bài viết" });

            if (post.Commentable == true)
                post.Commentable = false;
            else
                post.Commentable = true;
                //return Ok(new { Message = "Bài viết đã được khoá bình luận" });

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Chức năng bình luận của bài viết đã được {(post.Commentable ? "mở" : "khóa")}" });
        }
        
        // DELETE: api/Posts/MyPost/4
        [HttpDelete("MyPosts/{PostId}"), Authorize]
        public async Task<ActionResult> DeletePost ([FromRoute]int postId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var post = await _context.Posts.Include(post => post.User)
                                           .FirstOrDefaultAsync(post => post.Id == postId && post.User.Id == userId);

            if(post == null)
                return NotFound(new {Error = "Không tìm thấy bài viết hoặc bài viết đã bị xoá" });

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }   
}
