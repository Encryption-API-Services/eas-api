namespace Models.Encryption.AESRSAHybrid
{
    public class AESRSAHybridEncryptResponse
    {
        public string EncryptedAesKey { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string EncryptedData { get; set; }
    }
}
