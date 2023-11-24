using Encryption;
using System;
using System.Runtime.InteropServices;
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
            string id = Guid.NewGuid().ToString();
            RustSHAWrapper shaWrapper = new RustSHAWrapper();
            IntPtr hashPtr = await shaWrapper.SHA512HashStringAsync(id);
            string hash = Marshal.PtrToStringAnsi(hashPtr);
            RustSHAWrapper.free_cstring(hashPtr);
            return hash;
        }
    }
}