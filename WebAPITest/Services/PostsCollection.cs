using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Data;
using WebAPITest.Domain;

namespace WebAPITest.Services
{
    public class PostsCollection : IPostsCollection
    {
        ApplicationDbContext _dbContext;

        public PostsCollection(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> createPostAsync(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            int creationCount = await _dbContext.SaveChangesAsync();
            return creationCount > 0;
        }

        public async Task<bool> deletePostAsync(Guid id)
        {
            Post post = _dbContext.Posts.FirstOrDefault<Post>(x => x.Id == id);

            if (post == null)
                return false;

            _dbContext.Posts.Remove(post);
            int deleteCount = await _dbContext.SaveChangesAsync();
            return deleteCount > 0;
        }

        public List<Post> getAllPosts()
        {
            List<Post> posts = _dbContext.Posts.ToList<Post>();
            return posts;
        }

        public Post getPost(Guid id)
        {
            Post post = _dbContext.Posts.FirstOrDefault<Post>(x => x.Id == id);
            if(post == null)
            {
                return null;
            }
            return post;
        }

        public bool updatePost(Post post)
        {
            _dbContext.Posts.Update(post);
            int updateCount = _dbContext.SaveChanges();
            return updateCount > 0;
        }
    }
}
