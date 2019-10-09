using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPITest.Contracts.Request;
using WebAPITest.Contracts.Response;
using WebAPITest.Contracts.v1;
using WebAPITest.Domain;
using WebAPITest.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPITest.Controllers.v1
{
    public class PostController : Controller
    {
        private IPostsCollection _postsCollection;
        
        public PostController(IPostsCollection postsCollection)
        {
            _postsCollection = postsCollection;
        }

        [HttpGet(ApiRoutes.Post.GetAllPosts)]
        public IActionResult getAllPosts()
        {
            List<Post> posts = _postsCollection.getAllPosts();
            return Ok(posts);
        }

        [HttpGet(ApiRoutes.Post.Get)]
        public IActionResult get([FromRoute] string Id)
        {
            Post post = _postsCollection.getPost(Id);
            if (post == null)
            {
                return NotFound("sorry, post not found");
            }
            return Ok(post);
        }

        [HttpPut(ApiRoutes.Post.Update)]
        public IActionResult updatePost([FromRoute] string Id, [FromBody] UpdatePostRequest updatePostRequest)
        {
            Post post = new Post { Id = Id, Name = updatePostRequest.Name };
            bool result = _postsCollection.updatePost(post);
            if (result)
            {
                return Ok(post);
            }
            return NotFound();
        }

        [HttpPost(ApiRoutes.Post.Create)]
        public IActionResult createPost([FromBody] PostRequest postCreate)
        {

            var post = new Post { Id = postCreate.Id };

            if (string.IsNullOrEmpty(post.Id))
            {
                post.Id = Guid.NewGuid().ToString();
            }

            _postsCollection.getAllPosts().Add(post);

            string baseUrl = $"{HttpContext.Request.Scheme}//{HttpContext.Request.Host.ToUriComponent()}";
            string locationUrl = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id);

            var postResponse = new PostResponse { Id = post.Id };

            return Created(locationUrl, postResponse);
        }
    }
}
