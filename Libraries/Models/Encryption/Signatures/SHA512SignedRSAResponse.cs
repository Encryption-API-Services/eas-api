namespace Models.Encryption.Signatures
{
    public class SHA512SignedRSAResponse
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Signature { get; set; }
    }
}
