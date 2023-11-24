namespace Models.Encryption.Signatures
{
    public class SHA512SignedRSAVerifyRequest
    {
        public string PublicKey { get; set; }
        public string OriginalData { get; set; }
        public string Signature { get; set; }
    }
}