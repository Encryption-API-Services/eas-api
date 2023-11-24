namespace Models.Encryption
{
    public class EncryptAESResponse
    {
        public string Nonce { get; set; }
        public string Key { get; set; }
        public string Encrypted { get; set; }
    }
}