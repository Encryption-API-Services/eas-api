namespace Models.Encryption.ED25519
{
    public class ED25519SignWithKeyPairRequest
    {
        public string KeyPair { get; set; }
        public string DataToSign { get; set; }
    }
}