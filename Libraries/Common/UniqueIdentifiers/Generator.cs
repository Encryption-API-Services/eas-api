using CasDotnetSdk.Hashers;
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
    }
}