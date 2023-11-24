using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Encryption.Compression
{

    /// <summary>
    /// // ZSTD is not a encryption algorithm. It is a compression algorithm.
    // https://github.com/facebook/zstd
    // https://github.com/gyscos/zstd-rs
    // The level parameter into the Rust library is the level of compression.
    // The default is 3, that we use in EAS.
    /// </summary>
    public class ZSTDWrapper
    {
        public struct ZSTDCompressedBytes
        {
            public IntPtr raw_ptr;
            public int length;
        }

        public struct ZSTDDecompressedBytes
        {
            public IntPtr raw_ptr;
            public int length;
        }

        [DllImport("performant_encryption.dll")]
        private static extern IntPtr zstd_compress(string data, int level);
        [DllImport("performant_encryption.dll")]
        private static extern ZSTDCompressedBytes zstd_compress_bytes(byte[] data, int length, int level);
        [DllImport("performant_encryption.dll")]
        private static extern IntPtr zstd_decompress(string data);
        [DllImport("performant_encryption.dll")]
        private static extern ZSTDDecompressedBytes zstd_decompress_bytes(byte[] data, int length);
        [DllImport("performant_encryption.dll")]
        public static extern void free_bytes_vector(IntPtr data);
        [DllImport("performant_encryption.dll")]
        public static extern void free_cstring(IntPtr stringToFree);

        public IntPtr Compress(string data, int level = 3)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new Exception("You must send data to compress");
            }
            return zstd_compress(data, level);
        }

        public async Task<IntPtr> CompressAsync(string data, int level = 3)
        {
            return await Task.Run(() =>
            {
                return this.Compress(data, level);
            });
        }

        public IntPtr Decompress(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new Exception("You must send data to decompress");
            }
            return zstd_decompress(data);
        }

        public async Task<IntPtr> DecompressAsync(string data)
        {
            return await Task.Run(() =>
            {
                return this.Decompress(data);
            });
        }

        public ZSTDCompressedBytes CompressBytes(byte[] data, int level = 3)
        {
            if (data == null || data.Length <= 0)
            {
                throw new Exception("You must send data to compress");
            }
            return zstd_compress_bytes(data, data.Length, level);
        }

        public async Task<ZSTDCompressedBytes> CompressBytesAsync(byte[] data, int level = 3)
        {
            return await Task.Run(() =>
            {
                return this.CompressBytes(data, level);
            });
        }

        public ZSTDDecompressedBytes DecompressBytes(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new Exception("You must send data to decompress");
            }
            return zstd_decompress_bytes(data, data.Length);
        }

        public async Task<ZSTDDecompressedBytes> DecompressBytesAsync(byte[] data)
        {
            return await Task.Run(() =>
            {
                return this.DecompressBytes(data);
            });
        }
    }
}
