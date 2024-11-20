using CASHelpers;
using DataLayer.Mongo.Entities;
using DataLayer.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Validation.Networking;

namespace Validation.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LogRequestQueuePublish _requestQueuePublish;

        public RequestLoggingMiddleware(RequestDelegate next, LogRequestQueuePublish requestQueuePublish)
        {
            _next = next;
            this._requestQueuePublish = requestQueuePublish;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string requestId = Guid.NewGuid().ToString();
                string clientIp = context.Request.Headers[Constants.HeaderNames.XForwardedFor].FirstOrDefault();
                if (string.IsNullOrEmpty(clientIp))
                {
                    clientIp = context.Connection.RemoteIpAddress.ToString();
                }
                string ip = IPAddressExtension.ConvertContextToLocalHostIp(clientIp);
                context.Items[Constants.HttpItems.IP] = ip;
                string token = context.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
                this._requestQueuePublish.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new LogRequest()
                {
                    IsStart = true,
                    RequestId = requestId,
                    IP = ip,
                    Token = token,
                    Method = context.Request.Method,
                    CreateTime = DateTime.UtcNow
                })));
                await _next(context);
                this._requestQueuePublish.BasicPublish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new LogRequest()
                {
                    IsStart = false,
                    RequestId = requestId,
                    IP = ip,
                    Token = token,
                    Method = context.Request.Method,
                    CreateTime = DateTime.UtcNow
                })));
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
