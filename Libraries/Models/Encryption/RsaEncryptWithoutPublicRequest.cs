namespace Models.Encryption
{
    public class RsaEncryptWithoutPublicRequest
    {
        public string dataToEncrypt { get; set; }
        public int keySize { get; set; }
    }
}
