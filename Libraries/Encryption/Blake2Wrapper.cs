using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Encryption
{
    public class Blake2Wrapper
    {
        [DllImport("performant_encryption.dll")]
        private static extern IntPtr blake2_512(string toHash);
        [DllImport("performant_encryption.dll")]
        public static extern IntPtr blake2_256(string toHash);
        [DllImport("performant_encryption.dll")]
        public static extern bool blake2_256_verify(string dataToVerify, string hash);
        [DllImport("performant_encryption.dll")]
        public static extern bool blake2_512_verify(string dataToVerify, string hash);
        [DllImport("performant_encryption.dll")]
        public static extern void free_cstring(IntPtr stringToFree);

        public IntPtr Blake2512(string toHash)
        {
            if (string.IsNullOrEmpty(toHash))
            {
                throw new Exception("Please provide a string to hash with Blake2 512");
            }
            return blake2_512(toHash);
        }

        public async Task<bool> Blake2512VerifyAsync(string dataToVerify, string hash)
        {
            return await Task.Run(() =>
            {
                return Blake2512Verify(dataToVerify, hash);
            });
        }
        public bool Blake2512Verify(string dataToVerify, string hash)
        {
            if (string.IsNullOrEmpty(dataToVerify))
            {
                throw new Exception("Please provide data to verify with Blake2 512");
            }
            else if (string.IsNullOrEmpty(hash))
            {
                throw new Exception("Please provide a hash to verify with Blake2 512");
            }
            return blake2_512_verify(dataToVerify, hash);
        }

        public IntPtr Blake2256(string toHash)
        {
            if (string.IsNullOrEmpty(toHash))
            {
                throw new Exception("Please provide a string to hash with Blake2 256");
            }
            return blake2_256(toHash);
        }

        public bool Blake2256Verify(string dataToVerify, string hash)
        {
            if (string.IsNullOrEmpty(dataToVerify))
            {
                throw new Exception("Please provide data to verify with Blake2 256");
            }
            else if (string.IsNullOrEmpty(hash))
            {
                throw new Exception("Please provide a hash to verify with Blake2 256");
            }
            return blake2_256_verify(dataToVerify, hash);
        }

        public async Task<bool> Blake2256VerifyAsync(string dataToVerify, string hash)
        {
            return await Task.Run(() =>
            {
                return Blake2256Verify(dataToVerify, hash);
            });
        }
        public async Task<IntPtr> Blake2512Async(string toHash)
        {
            return await Task.Run(() =>
            {
                return Blake2512(toHash);
            });
        }
        public async Task<IntPtr> Blake2256Async(string toHash)
        {
            return await Task.Run(() =>
            {
                return blake2_256(toHash);
            });
        }
    }
}