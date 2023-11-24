namespace Models.Encryption.Signatures
{
    public class HMACSignRequest
    {
        public string Key { get; set; }
        public string Message { get; set; }
    }
}
