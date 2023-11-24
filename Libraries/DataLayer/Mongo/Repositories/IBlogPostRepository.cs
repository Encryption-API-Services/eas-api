using DataLayer.Mongo.Entities;
using Models.Blog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer.Mongo.Repositories
{
    public interface IBlogPostRepository
    {
        public Task InsertBlogPost(BlogPost post);
        public Task UpdateBlogPost(UpdateBlogPost post);
        public Task<List<BlogPost>> GetHomeBlogPosts();
        public Task<BlogPost> GetBlogPostByTitle(string blogTitle);
        public Task<BlogPost> GetBlogPostById(string id);
        public Task DeleteBlogPost(string id);
        public Task<List<BlogPost>> GetBlogPostsNotSentInNewsletter();
        public Task UpdateBlogPostSentInNewsLetter(string id);
    }
}
