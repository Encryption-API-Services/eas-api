namespace Common.EmergencyKit
{
    public class EmergencyKitCreatedResult
    {
        public string EmergencyKitId { get; set; }
        public byte[] CipherText { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] Tag { get; set; }
        public byte[] InfoStr { get; set; }
        public byte[] EncappedKey { get; set; }
    }
}
