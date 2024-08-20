using CasDotnetSdk.Hybrid.Types;

namespace EmergencyKit
{
    public class CreateEmergencyKitResponse
    {
        public Guid Key { get; set; }
        public AESRSAHybridEncryptResult EncryptResult { get; set; } 
        public AESRSAHybridInitializer Initalizer { get; set; }
    }
}
