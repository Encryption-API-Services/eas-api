namespace DataLayer.Redis
{
    public interface IRedisClient
    {
        public void SetString(string key, string value);
        public string GetString(string key);
    }
}
