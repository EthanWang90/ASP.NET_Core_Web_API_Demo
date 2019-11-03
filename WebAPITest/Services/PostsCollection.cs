using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Data;
using WebAPITest.Domain;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Post>> getAllPosts()
        {
            List<Post> posts = await _dbContext.Posts.ToListAsync<Post>();
            return posts;
        }

        public async Task<Post> getPost(Guid id)
        {
            Post post = await _dbContext.Posts.FirstOrDefaultAsync<Post>(x => x.Id == id);
            if(post == null)
            {
                return null;
            }
            return post;
        }

        public async Task<bool> updatePost(Post post)
        {
            _dbContext.Posts.Update(post);
            int updateCount = await _dbContext.SaveChangesAsync();
            return updateCount > 0;
        }

        public async Task<bool> checkPostOwnershipAsync(Guid Id, string userId)
        {
            Post post = await _dbContext.Posts.SingleAsync(x => x.Id == Id);
            if(post.UserId != userId)
            {
                return false;
            }

            return true;
        }
    }
}
