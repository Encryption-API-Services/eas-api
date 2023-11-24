namespace Models.Encryption.AESRSAHybrid
{
    public class AESRSAHybridEncryptRequest
    {
        public string Nonce { get; set; }
        public int KeySize { get; set; }
        public string DataToEncrypt { get; set; }
        public int AesType { get; set; }
    }
}
