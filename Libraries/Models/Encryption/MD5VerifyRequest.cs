namespace Models.Encryption
{
    public class MD5VerifyRequest
    {
        public string HashToVerify { get; set; }
        public string ToHash { get; set; }
    }
}