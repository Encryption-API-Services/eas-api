namespace Models.Encryption
{
    public class RsaSignWithoutKeyRequest
    {
        public string dataToSign { get; set; }
        public int keySize { get; set; }
    }
}
