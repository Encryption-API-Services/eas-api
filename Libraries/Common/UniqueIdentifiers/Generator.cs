using CasDotnetSdk.Hashers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.UniqueIdentifiers
{
    public class Generator
    {
        public Generator()
        {

        }

        public async Task<string> CreateApiKey()
        {
            byte[] id = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            SHAWrapper shaWrapper = new SHAWrapper();
            byte[] hashBytes = shaWrapper.Hash512(id);
            return Convert.ToBase64String(hashBytes);
        }
    }
}