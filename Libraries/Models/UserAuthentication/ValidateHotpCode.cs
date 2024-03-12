namespace Models.UserAuthentication
{
    public class ValidateHotpCode
    {
        public string UserId { get; set; }
        public string HotpCode { get; set; }
        public string UserAgent { get; set; }
    }
}
