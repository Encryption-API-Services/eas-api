using DataLayer.Cache;
using DataLayer.Mongo.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Threading.Tasks;
using Validation.Networking;

namespace Validation.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogRequestCache _requestCache;

        public RequestLoggingMiddleware(RequestDelegate next, LogRequestCache logRequestCache)
        {
            _next = next;
            this._requestCache = logRequestCache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string requestId = Guid.NewGuid().ToString();
                string clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientIp))
                {
                    clientIp = context.Connection.RemoteIpAddress.ToString();
                }
                string ip = IPAddressExtension.ConvertContextToLocalHostIp(clientIp);
                context.Items["IP"] = ip;
                string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                LogRequest requestStart = new LogRequest()
                {
                    IsStart = true,
                    RequestId = requestId,
                    IP = ip,
                    Token = token,
                    Method = context.Request.Method,
                    CreateTime = DateTime.UtcNow
                };
                this._requestCache.AddRequest(requestStart);
                await _next(context);
                LogRequest requestEnd = new LogRequest()
                {
                    IsStart = false,
                    RequestId = requestId,
                    IP = ip,
                    Token = token,
                    Method = context.Request.Method,
                    CreateTime = DateTime.UtcNow
                };
                this._requestCache.AddRequest(requestEnd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
