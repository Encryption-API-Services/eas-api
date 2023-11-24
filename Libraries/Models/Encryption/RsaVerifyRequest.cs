namespace Models.Encryption
{
    public class RsaVerifyRequest
    {
        public string PublicKey { get; set; }
        public string OriginalData { get; set; }
        public string Signature { get; set; }
    }
}
