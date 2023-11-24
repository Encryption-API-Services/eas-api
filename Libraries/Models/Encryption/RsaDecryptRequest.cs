namespace Models.Encryption
{
    public class RsaDecryptRequest
    {
        public string PrivateKey { get; set; }
        public string DataToDecrypt { get; set; }
    }
}