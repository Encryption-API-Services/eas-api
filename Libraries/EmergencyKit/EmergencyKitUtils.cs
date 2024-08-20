using CasDotnetSdk.Hybrid;
using CasDotnetSdk.Hybrid.Types;
using System.Text;

namespace EmergencyKit
{
    public class EmergencyKitUtils
    {
        public CreateEmergencyKitResponse CreateEmergencyKit()
        {
            AESRSAHybridInitializer initalizer = new AESRSAHybridInitializer(256, 4096);
            HybridEncryptionWrapper hybridEncryption = new HybridEncryptionWrapper();
            Guid newKitKey = Guid.NewGuid();
            AESRSAHybridEncryptResult encryptionResult = hybridEncryption.EncryptAESRSAHybrid(Encoding.UTF8.GetBytes(newKitKey.ToString()), initalizer);
            return new CreateEmergencyKitResponse()
            {
                Key = newKitKey,
                EncryptResult = encryptionResult,
                Initalizer = initalizer
            };
        }

        public void ValidateEmergencyKit(string rsaPrivateKey, AESRSAHybridEncryptResult encryptResult)
        {
            HybridEncryptionWrapper hybridWrapper = new HybridEncryptionWrapper();
            byte[] decrypted = hybridWrapper.DecryptAESRSAHybrid(rsaPrivateKey, encryptResult);
        }
    }
}
