using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Encryption.PasswordHash
{
    public class SCryptWrapper
    {
        [DllImport("performant_encryption.dll")]
        private static extern IntPtr scrypt_hash(string passToHash);

        [DllImport("performant_encryption.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool scrypt_verify(string password, string hash);
        [DllImport("performant_encryption.dll")]
        public static extern bool free_cstring(IntPtr stringToFree);
        public IntPtr HashPassword(string passToHash)
        {
            if (string.IsNullOrEmpty(passToHash))
            {
                throw new Exception("Please provide a password to hash");
            }
            return scrypt_hash(passToHash);
        }
        public async Task<IntPtr> HashPasswordAsync(string passToHash)
        {
            if (string.IsNullOrEmpty(passToHash))
            {
                throw new Exception("Please provide a password to hash");
            }

            return await Task.Run(() =>
            {
                return scrypt_hash(passToHash);
            });
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                throw new Exception("Please provide a password and a hash to verify");
            }
            return scrypt_verify(password, hash);
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                throw new Exception("Please provide a password and a hash to verify");
            }

            return await Task.Run(() =>
            {
                return scrypt_verify(password, hash);
            });
        }
    }
}