using Common;
using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using DataLayer.Mongo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models.Blog;
using System.Reflection;
using Validation.UserRegistration;

namespace API.ControllerLogic
{
    public class BlogControllerLogic : IBlogPostControllerLogic
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IEASExceptionRepository _exceptionRepository;
        private readonly INewsletterRepository _newsLetterRepository;
        private readonly BenchmarkMethodCache _benchmarkMethodCache;
        public BlogControllerLogic(
            IBlogPostRepository blogPostRepository,
            IEASExceptionRepository exceptionRepository,
            INewsletterRepository newsLetterRepository,
            BenchmarkMethodCache benchmarkMethodCache)
        {
            this._blogPostRepository = blogPostRepository;
            this._exceptionRepository = exceptionRepository;
            this._newsLetterRepository = newsLetterRepository;
            this._benchmarkMethodCache = benchmarkMethodCache;
        }

        #region AddEmailToNewsletter
        public async Task<IActionResult> AddEmailToNewsletter(HttpContext httpContext, AddEmailToNewsletter body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                RegisterUserValidation validator = new RegisterUserValidation();
                if (validator.IsEmailValid(body.Email))
                {
                    Newsletter subscription = await this._newsLetterRepository.GetSubscriptionByEmail(body.Email);
                    if (subscription == null)
                    {
                        await this._newsLetterRepository.AddEmailToNewsletter(new Newsletter
                        {
                            Email = body.Email,
                            CreateDate = DateTime.UtcNow
                        });
                        result = new OkObjectResult(new { message = String.Format("The email {0} was added to the newsletter.", body.Email) });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = String.Format("The email {0} is already part of the newsletter.", body.Email) });
                    }
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "You must enter a valid email" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region CreatePost
        public async Task<IActionResult> CreatePost(CreateBlogPost body, HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.BlogTitle) && !string.IsNullOrEmpty(body.BlogBody))
                {
                    BlogPost newBlogPost = new BlogPost()
                    {
                        BlogTitle = body.BlogTitle,
                        BlogBody = body.BlogBody,
                        CreateDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        WasNewsletterSent = false,
                        CreatedBy = httpContext.Items["UserID"].ToString()
                    };
                    await this._blogPostRepository.InsertBlogPost(newBlogPost);
                    result = new OkObjectResult(new { message = "You have create a new blog post" });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "You need to enter a blog title and post" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region DeletePost
        public async Task<IActionResult> DeletePost(HttpContext httpContext, DeleteBlogPost body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(body.id))
                {
                    await this._blogPostRepository.DeleteBlogPost(body.id);
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end." });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region GetBlogPosts
        public async Task<IActionResult> GetBlogPosts(HttpContext httpContext)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                List<BlogPost> blogPosts = await this._blogPostRepository.GetHomeBlogPosts();
                return new OkObjectResult(new { posts = blogPosts });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end getting blog posts" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region GetPost
        public async Task<IActionResult> GetPost(HttpContext httpContext, string blogPostTitle)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                if (!string.IsNullOrEmpty(blogPostTitle))
                {
                    blogPostTitle = blogPostTitle.Replace("-", " ");
                    BlogPost blogPost = await this._blogPostRepository.GetBlogPostByTitle(blogPostTitle);
                    if (blogPost != null)
                    {
                        result = new OkObjectResult(new { post = blogPost });
                    }
                    else
                    {
                        result = new BadRequestObjectResult(new { error = "There is no blog post with that title." });
                    }
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end getting the post" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region GetPostById
        public async Task<IActionResult> GetPostById(HttpContext httpContext, string id)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                BlogPost post = await this._blogPostRepository.GetBlogPostById(id);
                result = new OkObjectResult(new { post = post });
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our end getting the post" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion

        #region UpdatePost
        public async Task<IActionResult> UpdatePost(HttpContext httpContext, UpdateBlogPost body)
        {
            BenchmarkMethodLogger logger = new BenchmarkMethodLogger(httpContext);
            IActionResult result = null;
            try
            {
                BlogPost post = await this._blogPostRepository.GetBlogPostById(body.BlogId);
                if (post != null)
                {
                    await this._blogPostRepository.UpdateBlogPost(body);
                    result = new OkObjectResult(new { message = "" });
                }
                else
                {
                    result = new BadRequestObjectResult(new { error = "We were unable to find that post" });
                }
            }
            catch (Exception ex)
            {
                await this._exceptionRepository.InsertException(ex.ToString(), MethodBase.GetCurrentMethod().Name);
                result = new BadRequestObjectResult(new { error = "Something went wrong on our updating the post" });
            }
            logger.EndExecution();
            this._benchmarkMethodCache.AddLog(logger);
            return result;
        }
        #endregion
    }
}
