using CasDotnetSdk.Hybrid;
using CasDotnetSdk.Hybrid.Types;
using System;
using System.Text;

namespace Common.EmergencyKit
{
    public static class EmergencyKitUtils
    {
        public static EmergencyKitCreatedResult CreateEmergencyKit()
        {
            string newEmergencyKitId = Guid.NewGuid().ToString();
            HpkeWrapper hpke = new HpkeWrapper();
            HpkeKeyPairResult keys = hpke.GenerateKeyPair();
            HpkeEncryptResult encrypted = hpke.Encrypt(Encoding.UTF8.GetBytes(newEmergencyKitId), keys.PublicKey, keys.InfoStr);
            return new EmergencyKitCreatedResult()
            {
                EmergencyKitId = newEmergencyKitId,
                CipherText = encrypted.Ciphertext,
                Tag = encrypted.Tag,
                EncappedKey = encrypted.EncappedKey,
                InfoStr = keys.InfoStr,
                PrivateKey = keys.PrivateKey
            };
        }
    }
}
