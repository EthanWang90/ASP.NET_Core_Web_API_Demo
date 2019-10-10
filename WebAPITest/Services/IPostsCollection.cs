using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Domain;

namespace WebAPITest.Services
{
    public interface IPostsCollection
    {
        Task<List<Post>> getAllPosts();

        Task<Post> getPost(Guid id);

        Task<bool> createPostAsync(Post post);

        Task<bool> updatePost(Post post);

        Task<bool> deletePostAsync(Guid id);
    }
}
