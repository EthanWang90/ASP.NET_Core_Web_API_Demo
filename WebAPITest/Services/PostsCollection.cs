using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITest.Domain;

namespace WebAPITest.Services
{
    public class PostsCollection : IPostsCollection
    {
        private List<Post> _posts;

        public PostsCollection()
        {
            _posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                _posts.Add(new Post
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Post name {i}"
                });
            }
        }

        public List<Post> getAllPosts()
        {
            return _posts;
        }

        public Post getPost(string id)
        {
            Post post = _posts.FirstOrDefault<Post>(x => x.Id == id);
            if(post == null)
            {
                return null;
            }
            return post;
        }

        public bool updatePost(Post post)
        {
            Post targetPost = _posts.FirstOrDefault<Post>(x => x.Id == post.Id);
            if(targetPost == null)
            {
                return false;
            }
            int index = _posts.IndexOf(targetPost);
            _posts[index].Name = post.Name;
            return true;
        }
    }
}
