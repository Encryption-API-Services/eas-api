namespace Models.Encryption.Signatures
{
    public class SHA512ED25519DalekVerifyRequest
    {
        public string Signature { get; set; }
        public string DataToVerify { get; set; }
        public string PublicKey { get; set; }
    }
}