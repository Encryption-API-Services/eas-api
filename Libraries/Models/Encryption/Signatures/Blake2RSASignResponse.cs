namespace Models.Encryption.Signatures
{
    public class Blake2RSASignResponse
    {
        public string Signature { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
    }
}
