using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Encryption
{
    public class MD5Wrapper
    {
        [DllImport("performant_encryption.dll")]
        private static extern IntPtr md5_hash_string(string toHash);
        [DllImport("performant_encryption.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool md5_hash_verify(string hashToVerify, string toHash);
        [DllImport("performant_encryption.dll")]
        public static extern void free_cstring(IntPtr stringToFree);

        public IntPtr Hash(string toHash)
        {
            if (string.IsNullOrEmpty(toHash))
            {
                throw new Exception("You must provide data to hash the string");
            }
            return md5_hash_string(toHash);
        }

        public bool Verify(string hashToVerify, string toHash)
        {
            if (string.IsNullOrEmpty(hashToVerify))
            {
                throw new Exception("You must a hash to verify");
            }
            if (string.IsNullOrEmpty(toHash))
            {
                throw new Exception("You must provide a string to hash to verify");
            }
            return md5_hash_verify(hashToVerify, toHash);
        }

        public async Task<IntPtr> HashAsync(string toHash)
        {
            return await Task.Run(() =>
            {
                return Hash(toHash);
            });
        }

        public async Task<bool> VerifyAsync(string hashToVerify, string toHash)
        {
            return await Task.Run(() =>
            {
                return Verify(hashToVerify, toHash);
            });
        }
    }
}
