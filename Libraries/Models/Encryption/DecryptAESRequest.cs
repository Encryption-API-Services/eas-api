namespace Models.Encryption
{
    public class DecryptAESRequest
    {
        public string DataToDecrypt { get; set; }
        public string Key { get; set; }
        public string NonceKey { get; set; }
        public int AesType { get; set; }
    }
}