using CASHelpers;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    public class AuthenticationHelper
    {
        public async Task<string> GetStandardTestFrameworkToken()
        {
            // This userId can be adjusted to any other test user in the database.
            string userId = "64d94d90474e4e404cdffc1b";
            JWT jwt = new JWT();
            ECDSAWrapper newEcdsa = new ECDSAWrapper("ES521");
            return jwt.GenerateECCToken(userId, false, newEcdsa, 1);
        }

        public async Task<string> GetExpiredTestFrameworkToken()
        {
            // This userId can be adjusted to any other test user in the database.
            string userId = "64d94d90474e4e404cdffc1b";
            JWT jwt = new JWT();
            ECDSAWrapper newEcdsa = new ECDSAWrapper("ES521");
            return jwt.GenerateECCToken(userId, false, newEcdsa, 0);
        }
    }
}
