using CASHelpers;
using Encryption;
using Encryption.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Encryption.RustRSAWrapper;

namespace Tests.Helpers
{
    public class AuthenticationHelper
    {
        public AuthenticationHelper()
        {

        }

        public async Task<string> GetStandardTestFrameworkToken()
        {
            // This userId can be adjusted to any other test user in the database.
            string userId = "64d94d90474e4e404cdffc1b";
            JWT jwt = new JWT();
            ECDSAWrapper newEcdsa = new ECDSAWrapper("ES521");
            RustRSAWrapper rsa = new RustRSAWrapper(new ZSTDWrapper());
            RustRsaKeyPair clientKeyPairPtr = await rsa.GetKeyPairAsync(4096);
            // Clients RSA Key Pair
            string clientRsaPublicKey = Marshal.PtrToStringAnsi(clientKeyPairPtr.pub_key);
            string clientRsaPrivateKey = Marshal.PtrToStringAnsi(clientKeyPairPtr.priv_key);
            RustRSAWrapper.free_cstring(clientKeyPairPtr.pub_key);
            RustRSAWrapper.free_cstring(clientKeyPairPtr.priv_key);

            return jwt.GenerateECCToken(userId, false, newEcdsa, 1);
        }

        public async Task<string> GetExpiredTestFrameworkToken()
        {
            // This userId can be adjusted to any other test user in the database.
            string userId = "64d94d90474e4e404cdffc1b";
            JWT jwt = new JWT();
            ECDSAWrapper newEcdsa = new ECDSAWrapper("ES521");
            RustRSAWrapper rsa = new RustRSAWrapper(new ZSTDWrapper());
            RustRsaKeyPair clientKeyPairPtr = await rsa.GetKeyPairAsync(4096);
            // Clients RSA Key Pair
            string clientRsaPublicKey = Marshal.PtrToStringAnsi(clientKeyPairPtr.pub_key);
            string clientRsaPrivateKey = Marshal.PtrToStringAnsi(clientKeyPairPtr.priv_key);
            RustRSAWrapper.free_cstring(clientKeyPairPtr.pub_key);
            RustRSAWrapper.free_cstring(clientKeyPairPtr.priv_key);

            return jwt.GenerateECCToken(userId, false, newEcdsa, 0);
        }
    }
}
