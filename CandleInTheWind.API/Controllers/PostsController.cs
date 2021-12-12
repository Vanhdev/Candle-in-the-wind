﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.API.Models.Posts;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetAllApprovedPosts()
        {

            var posts = await _context.Posts.Include(posts => posts.User)
                                            .Where(post => post.Status == PostStatus.Approved)
                                            .ToListAsync();
            // 1 is approved, 2 is not approved, 0 is pending approval.

            var postsResponse = posts.Select(Post => toDTO(Post));
            return Ok(postsResponse);
        }

        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDTO>>> getAllPost()
        {
            var posts = await _context.Posts.Include(post => post.User).ToListAsync();
            
            var postResponse = posts.Select(post => toDTO(post));

            return Ok(postResponse);
        } 
        */

        [HttpGet("{pageIndex}")]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetPagedApprovedPost(int pageIndex = 1, int pageSize = 5)
        {
            if(pageIndex < 1) pageIndex = 1;
            if(pageSize < 0) pageSize = 1;

            var approvedPosts = _context.Posts.Include(post => post.User)
                                              .Where(post => post.Status == PostStatus.Approved);
            int count = approvedPosts.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);

            if (pageIndex > totalPages) pageIndex = 1;

            var postsPerPage = await approvedPosts.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            if (postsPerPage.Count == 0)
                return NotFound();

            var responsePosts = postsPerPage.Select(post => toDTO(post)); 

            return Ok(
                new PostFilterDTO
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    PostDTOs = responsePosts,
                });

        }

        [HttpGet("mypost")]
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
            var mypostsReponse = myposts.Select(post => toDTO(post));
            return Ok(mypostsReponse);
        }
        /*
        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(toDTO(post));
        }
        */


        /*
        [HttpPost("Posts")]
        [Authorize]
        public async Task<ActionResult> CreatePost([FromBody]PostDTO dto)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            var userNameClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return BadRequest();

            var userId = int.Parse(userIdClaim.Value);
            var userName = userIdClaim.Value;

            

            try
            {
                if (dto == null)
                    return BadRequest();
                var createdPost = new Post.
                {
                    Commentable = true,
                    Title = dto.Title,
                    Content = dto.Content,
                    Status = 
                }
                
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database");
            }



        }
        
        */
        





        private PostDTO toDTO(Post post)
        {
            var postID = post.Id;
            var Comment_Count = _context.Comments.Count(Comment => Comment.Post.Id == postID);
            
            return new PostDTO
            {
                Id = postID,
                Title = post.Title,
                Content = post.Content,
                ApprovedAt = post.ApprovedAt,
                Commentable = post.Commentable,
                CommentCount = Comment_Count,
                UserId = post.User.Id,
                UserName = post.User.UserName,
                
            };
        }

    }   
}