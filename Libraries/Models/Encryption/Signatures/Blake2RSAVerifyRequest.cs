namespace Models.Encryption.Signatures
{
    public class Blake2RSAVerifyRequest
    {
        public int Blake2HashSize { get; set; }
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public string OriginalData { get; set; }
    }
}
