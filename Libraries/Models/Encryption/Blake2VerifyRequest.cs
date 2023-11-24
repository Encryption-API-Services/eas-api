namespace Models.Encryption
{
    public class Blake2VerifyRequest
    {
        public int HashSize { get; set; }
        public string DataToVerify { get; set; }
        public string Hash { get; set; }
    }
}
