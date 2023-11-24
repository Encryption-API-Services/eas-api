namespace Models.Encryption.Signatures
{
    public class Blake2ED25519DalekSignRequest
    {
        public int HashSize { get; set; }
        public string DataToSign { get; set; }
    }
}
