using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Domain;

namespace WebAPITest.Services
{
    public interface IPostsCollection
    {
        List<Post> getAllPosts();

        Post getPost(string id);

        bool updatePost(Post post);
    }
}
