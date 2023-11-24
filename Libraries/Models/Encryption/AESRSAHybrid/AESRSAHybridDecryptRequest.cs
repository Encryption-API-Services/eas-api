namespace Models.Encryption.AESRSAHybrid
{
    public class AESRSAHybridDecryptRequest
    {
        public string PrivateRsaKey { get; set; }
        public string EncryptedAesKey { get; set; }
        public string Nonce { get; set; }
        public string EncryptedData { get; set; }
        public int AesType { get; set; }
    }
}