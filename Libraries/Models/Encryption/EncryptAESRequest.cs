namespace Models.Encryption
{
    public class EncryptAESRequest
    {
        public string NonceKey { get; set; }
        public string DataToEncrypt { get; set; }
        public int AesType { get; set; }
    }
}
