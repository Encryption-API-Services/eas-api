using System.Threading.Tasks;

namespace DataLayer.Redis
{
    public interface IRedisClient
    {
        public void SetString(string key, string value);
        public string GetString(string key);
        public Task<string> GetDelete(string key);
    }
}
