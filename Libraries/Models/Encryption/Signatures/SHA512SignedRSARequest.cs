namespace Models.Encryption.Signatures
{
    public class SHA512SignedRSARequest
    {
        public string DataToHash { get; set; }
        public int KeySize { get; set; }
    }
}
