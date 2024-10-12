using CasDotnetSdk.Hashers;
using CasDotnetSdk.Signatures;
using CasDotnetSdk.Signatures.Types;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Common.UniqueIdentifiers
{
    public class Generator
    {
        public Generator()
        {

        }

        public string CreateApiKey()
        {
            byte[] id = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashBytes = shaWrapper.Hash512(id);
            return Convert.ToBase64String(hashBytes);
        }
        public class EmailToken
        {
            public string Base64HashedToken { get; set; }
            public string Base64PublicKey { get; set; }
            public string UrlSignature { get; set; }
        }

        public EmailToken GenerateEmailToken()
        {
            string token = Guid.NewGuid().ToString();
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashedToken = shaWrapper.Hash512(Encoding.UTF8.GetBytes(token));
            ED25519Wrapper ed25519 = new ED25519Wrapper();
            byte[] keyPair = ed25519.GetKeyPairBytes();
            Ed25519ByteSignatureResult result = ed25519.SignBytes(keyPair, hashedToken);
            return new EmailToken()
            {
                Base64HashedToken = Convert.ToBase64String(hashedToken),
                Base64PublicKey = Convert.ToBase64String(result.PublicKey),
                UrlSignature = Base64UrlEncoder.Encode(result.Signature),
            };
        }
    }
}