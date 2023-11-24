using Microsoft.AspNetCore.Mvc;
using Models.Blog;

namespace API.ControllerLogic
{
    public interface IBlogPostControllerLogic
    {
        Task<IActionResult> CreatePost(CreateBlogPost body, HttpContext httpContext);
        Task<IActionResult> GetBlogPosts(HttpContext httpContext);
        Task<IActionResult> GetPost(HttpContext httpContext, string blogPostTitle);
        Task<IActionResult> GetPostById(HttpContext httpContext, string id);
        Task<IActionResult> UpdatePost(HttpContext httpContext, UpdateBlogPost body);
        Task<IActionResult> DeletePost(HttpContext httpContext, DeleteBlogPost body);
        Task<IActionResult> AddEmailToNewsletter(HttpContext httpContext, AddEmailToNewsletter body);
    }
}