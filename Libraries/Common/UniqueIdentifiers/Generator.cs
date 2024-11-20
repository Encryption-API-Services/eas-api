using CasDotnetSdk.Hashers;
using CasDotnetSdk.Signatures;
using CasDotnetSdk.Signatures.Types;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
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

        public string GenerateRandomPassword(int length)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

            if (length < 4) // Ensure we have enough room for special characters, lowercase, and uppercase
            {
                throw new ArgumentException("Password length must be at least 4 to accommodate special characters and basic structure.");
            }

            // Step 1: Ensure at least 2 special characters
            Random random = new Random();
            string password = "";

            // Add 2 random special characters
            password += specialChars[random.Next(specialChars.Length)];
            password += specialChars[random.Next(specialChars.Length)];

            // Step 2: Add other characters (remaining length) from other character sets
            string allChars = lowerChars + upperChars + digits;
            for (int i = password.Length; i < length; i++)
            {
                password += allChars[random.Next(allChars.Length)];
            }

            // Step 3: Shuffle the resulting password to mix the characters
            password = new string(password.ToCharArray().OrderBy(c => random.Next()).ToArray());

            return password;
        }
    }
}