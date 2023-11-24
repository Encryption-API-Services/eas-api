using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace Encryption
{
    public class ECDSAWrapper
    {
        public ECDsa ECDKey { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public ECDSAWrapper(string curve)
        {
            switch (curve)
            {
                case "ES256":
                    this.ECDKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                    break;
                case "ES384":
                    this.ECDKey = ECDsa.Create(ECCurve.NamedCurves.nistP384);
                    break;
                case "ES521":
                    this.ECDKey = ECDsa.Create(ECCurve.NamedCurves.nistP521);
                    break;
                default:
                    this.ECDKey = ECDsa.Create(ECCurve.NamedCurves.nistP521);
                    break;
            }
            this.ExportKeys();
        }
        private void ExportKeys()
        {
            this.PublicKey = Convert.ToBase64String(this.ECDKey.ExportSubjectPublicKeyInfo());
            this.PrivateKey = Convert.ToBase64String(this.ECDKey.ExportECPrivateKey());
        }

        public void ImportFromPublicBase64String(string publicKey)
        {
            int bytesRead;
            this.ECDKey.ImportSubjectPublicKeyInfo(Base64UrlEncoder.DecodeBytes(publicKey), out bytesRead);
        }

        public void ImportFromPrivateBase64String(string privateKey)
        {
            int bytesRead;
            this.ECDKey.ImportECPrivateKey(Base64UrlEncoder.DecodeBytes(privateKey), out bytesRead);
        }
    }
}