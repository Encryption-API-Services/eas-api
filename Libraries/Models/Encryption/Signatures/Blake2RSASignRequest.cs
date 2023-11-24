namespace Models.Encryption.Signatures
{
    public class Blake2RSASignRequest
    {
        public int Blake2HashSize { get; set; }
        public int RsaKeySize { get; set; }
        public string DataToSign { get; set; }
    }
}