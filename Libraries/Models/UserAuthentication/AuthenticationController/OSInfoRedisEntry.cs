namespace Models.UserAuthentication.AuthenticationController
{
    public class OSInfoRedisEntry
    {
        public string IP { get; set; }
        public string OperatingSystem { get; set; }
        public string ApiKey { get; set; }
        public bool IsApiKeyProd { get; set; }
    }
}
