namespace Models.Encryption.ED25519
{
    public class Ed25519VerifyWithPublicKeyRequest
    {
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public string DataToVerify { get; set; }
    }
}
