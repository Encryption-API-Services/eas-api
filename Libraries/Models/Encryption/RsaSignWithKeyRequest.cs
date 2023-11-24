namespace Models.Encryption
{
    public class RsaSignWithKeyRequest
    {
        public string PrivateKey { get; set; }
        public string DataToSign { get; set; }
    }
}
