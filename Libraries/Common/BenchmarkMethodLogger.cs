using CASHelpers;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Common
{
    public class BenchmarkMethodLogger
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserID { get; set; }
        public string Method { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BenchmarkMethodLogger(HttpContext context, [CallerMemberName] string callingMethod = null)
        {
            string token = context.Request.Headers[Constants.HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token) && token != "null")
            {
                this.UserID = new JWT().GetUserIdFromToken(token);
            }
            this.StartTime = DateTime.UtcNow;
            this.Method = callingMethod;
        }

        public void EndExecution()
        {
            this.EndTime = DateTime.UtcNow;
        }
    }
}
