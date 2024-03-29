﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPITest.Contracts.Request;
using WebAPITest.Contracts.Response;
using WebAPITest.Contracts.v1;
using WebAPITest.Domain;
using WebAPITest.Extensions;
using WebAPITest.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPITest.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private IPostsCollection _postsCollection;
        
        public PostController(IPostsCollection postsCollection)
        {
            _postsCollection = postsCollection;
        }

        [HttpGet(ApiRoutes.Post.GetAllPosts)]
        public async Task<IActionResult> getAllPosts()
        {
            List<Post> posts = await _postsCollection.getAllPosts();
            return Ok(posts);
        }

        [HttpGet(ApiRoutes.Post.Get)]
        public async Task<IActionResult> get([FromRoute] Guid Id)
        {
            Post post = await _postsCollection.getPost(Id);
            if (post == null)
            {
                return NotFound("sorry, post not found");
            }
            return Ok(post);
        }

        [HttpPut(ApiRoutes.Post.Update)]
        public async Task<IActionResult> updatePost([FromRoute] Guid Id, [FromBody] UpdatePostRequest updatePostRequest)
        {
            string userId = HttpContext.getUserId();
            bool isPostBelongsToUser = await _postsCollection.checkPostOwnershipAsync(Id, userId);

            if (!isPostBelongsToUser)
            {
                return BadRequest("The post doesn't belong to the user");
            }

            Post post = await _postsCollection.getPost(Id);
            post.Name = updatePostRequest.Name;
            bool result = await _postsCollection.updatePost(post);
            if (result)
            {
                return Ok(post);
            }
            return NotFound();
        }

        [HttpPost(ApiRoutes.Post.Create)]
        public async Task<IActionResult> createPost([FromBody] PostRequest postCreate)
        {

            string userId = HttpContext.getUserId();

            var post = new Post {
                Name = postCreate.Name,
                UserId = userId
            };

            await _postsCollection.createPostAsync(post);

            string baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.ToUriComponent()}";
            string locationUrl = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id.ToString());

            var postResponse = new PostResponse { Id = post.Id };

            return Created(locationUrl, postResponse);
        }

        [HttpDelete(ApiRoutes.Post.Delete)]
        public async Task<IActionResult> deletePost([FromRoute] Guid Id)
        {
            string userId = HttpContext.getUserId();
            bool isPostBelongsToUser = await _postsCollection.checkPostOwnershipAsync(Id, userId);

            if (!isPostBelongsToUser)
            {
                return BadRequest("The post doesn't belong to the user");
            }

            bool result = await _postsCollection.deletePostAsync(Id);
            if (result)
                return NoContent();

            return NotFound();
        }
    }
}
