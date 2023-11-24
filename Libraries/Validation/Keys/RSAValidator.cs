using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.IO;

namespace Validation.Keys
{
    public class RSAValidator
    {
        public bool IsPrivateKeyPEMValid(string privateKey)
        {
            bool result = false;
            using (var stringReader = new StringReader(privateKey))
            {
                var pemReader = new PemReader(stringReader);
                var pemObject = pemReader.ReadObject();
                if (pemObject is RsaKeyParameters prviateKeyObject)
                {
                    if (prviateKeyObject.IsPrivate)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool IsPublicKeyPEMValid(string publicKey)
        {
            bool result = false;
            using (var stringReader = new StringReader(publicKey))
            {
                var pemReader = new PemReader(stringReader);
                var pemObject = pemReader.ReadObject();
                if (pemObject is RsaKeyParameters privateKeyObject)
                {
                    if (!privateKeyObject.IsPrivate)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
