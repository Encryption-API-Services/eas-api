namespace Models.Encryption
{
    public class RsaDecryptWithStoredPrivateRequest
    {
        public string PublicKey { get; set; }
        public string DataToDecrypt { get; set; }
    }
}
