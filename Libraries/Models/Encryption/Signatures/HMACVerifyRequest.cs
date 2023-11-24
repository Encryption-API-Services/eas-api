namespace Models.Encryption.Signatures
{
    public class HMACVerifyRequest
    {
        public string Key { get; set; }
        public string Message { get; set; }
        public string Signature { get; set; }
    }
}
