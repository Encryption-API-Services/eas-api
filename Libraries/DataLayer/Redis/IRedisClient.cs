using System;
using System.Threading.Tasks;

namespace DataLayer.Redis
{
    public interface IRedisClient
    {
        public void SetString(string key, string value, TimeSpan? expiry = null);
        public string GetString(string key);
        public Task<string> GetDelete(string key);
    }
}
