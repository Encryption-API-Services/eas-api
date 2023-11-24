namespace Models.Encryption.Signatures
{
    public class Blake2ED25519DalekVerifyRequest
    {
        public int HashSize { get; set; }
        public string PublicKey { get; set; }
        public string DataToVerify { get; set; }
        public string Signature { get; set; }
    }
}
