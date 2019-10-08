using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPITest.Contracts.v1;
using WebAPITest.Domain;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPITest.Controllers.v1
{
    public class PostController : Controller
    {
        private List<Post> posts;
        
        public PostController()
        {
            posts = new List<Post>();
        }

        [HttpGet(ApiRoutes.Post.GetAllPosts)]
        public IActionResult getAllPosts()
        {
            for(int i=0; i<5; i++)
            {
                posts.Add(new Post { Id = Guid.NewGuid().ToString()});
            }
            return Ok(posts);
        }
    }
}
