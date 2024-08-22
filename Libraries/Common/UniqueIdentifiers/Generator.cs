using CasDotnetSdk.Hashers;
using CasDotnetSdk.Signatures.Types;
using CasDotnetSdk.Signatures;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;

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

        public string GeneratePassword(int totalLength, int specialCharCount)
        {
            char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] Digits = "0123456789".ToCharArray();
            char[] SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?/`~".ToCharArray();
            if (totalLength < specialCharCount)
            {
                throw new ArgumentException("Total length must be greater than or equal to the number of special characters.");
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                var passwordChars = new char[totalLength];
                var randomBytes = new byte[totalLength];
                rng.GetBytes(randomBytes);

                // Fill in special characters
                for (int i = 0; i < specialCharCount; i++)
                {
                    int specialCharIndex = i; // Replace with another index for each special character
                    passwordChars[specialCharIndex] = SpecialChars[randomBytes[i] % SpecialChars.Length];
                }

                // Fill in remaining characters
                int remainingLength = totalLength - specialCharCount;
                for (int i = specialCharCount; i < totalLength; i++)
                {
                    var category = randomBytes[i] % 3;
                    if (category == 0)
                    {
                        passwordChars[i] = Letters[randomBytes[i] % Letters.Length];
                    }
                    else if (category == 1)
                    {
                        passwordChars[i] = Digits[randomBytes[i] % Digits.Length];
                    }
                    else
                    {
                        passwordChars[i] = SpecialChars[randomBytes[i] % SpecialChars.Length];
                    }
                }

                // Shuffle the array to randomize the special character positions
                var shuffledChars = passwordChars.OrderBy(x => randomBytes[Array.IndexOf(passwordChars, x)]).ToArray();
                return new string(shuffledChars);
            }
        }
    }
}