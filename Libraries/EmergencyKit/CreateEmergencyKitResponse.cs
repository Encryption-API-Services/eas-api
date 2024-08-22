using CasDotnetSdk.Hybrid.Types;

namespace EmergencyKit
{
    public class CreateEmergencyKitResponse
    {
        public string Key { get; set; }
        public AESRSAHybridEncryptResult EncryptResult { get; set; }
        public AESRSAHybridInitializer Initalizer { get; set; }
    }
}
