using Encryption.Compression;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Encryption.Compression.ZSTDWrapper;

namespace Encryption.Tests
{
    public class ZSTDWrapperTests
    {
        private readonly ZSTDWrapper _zSTDWrapper;
        private string TestString = "Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!Hello World!";
        public ZSTDWrapperTests()
        {
            this._zSTDWrapper = new ZSTDWrapper();
        }

        [Fact]
        public void CompressBytes()
        {
            byte[] dataToCompressBytes = Encoding.UTF8.GetBytes(this.TestString);
            ZSTDCompressedBytes compressedData = this._zSTDWrapper.CompressBytes(dataToCompressBytes);
            byte[] compressedBytes = new byte[compressedData.length];
            Marshal.Copy(compressedData.raw_ptr, compressedBytes, 0, compressedBytes.Length);
            Assert.True(compressedBytes.Length < dataToCompressBytes.Length);
            ZSTDWrapper.free_bytes_vector(compressedData.raw_ptr);
        }

        [Fact]
        public async Task CompressBytesAsync()
        {
            byte[] dataToCompressBytes = Encoding.UTF8.GetBytes(this.TestString);
            ZSTDCompressedBytes compressedData = await this._zSTDWrapper.CompressBytesAsync(dataToCompressBytes);
            byte[] compressedBytes = new byte[compressedData.length];
            Marshal.Copy(compressedData.raw_ptr, compressedBytes, 0, compressedBytes.Length);
            Assert.True(compressedBytes.Length < dataToCompressBytes.Length);
            ZSTDWrapper.free_bytes_vector(compressedData.raw_ptr);
        }

        [Fact]
        public void CompressString()
        {
            IntPtr compressedString = this._zSTDWrapper.Compress(this.TestString);
            string compressedData = Marshal.PtrToStringAnsi(compressedString);
            ZSTDWrapper.free_cstring(compressedString);
            Assert.True(compressedData.Length < this.TestString.Length);
        }

        [Fact]
        public async Task CompressStringAsync()
        {
            IntPtr compressedString = await this._zSTDWrapper.CompressAsync(this.TestString);
            string compressedData = Marshal.PtrToStringAnsi(compressedString);
            ZSTDWrapper.free_cstring(compressedString);
            Assert.True(compressedData.Length < this.TestString.Length);
        }

        [Fact]
        public void DecompressString()
        {
            IntPtr compressedString = this._zSTDWrapper.Compress(this.TestString);
            string compressedData = Marshal.PtrToStringAnsi(compressedString);
            IntPtr decompressedString = this._zSTDWrapper.Decompress(compressedData);
            string decompressedData = Marshal.PtrToStringAnsi(decompressedString);
            ZSTDWrapper.free_cstring(compressedString);
            ZSTDWrapper.free_cstring(decompressedString);
            Assert.Equal(decompressedData, this.TestString);
        }

        [Fact]
        public async Task DecompressStringAsync()
        {
            IntPtr compressedString = await this._zSTDWrapper.CompressAsync(this.TestString);
            string compressedData = Marshal.PtrToStringAnsi(compressedString);
            IntPtr decompressedString = await this._zSTDWrapper.DecompressAsync(compressedData);
            string decompressedData = Marshal.PtrToStringAnsi(decompressedString);
            ZSTDWrapper.free_cstring(compressedString);
            ZSTDWrapper.free_cstring(decompressedString);
            Assert.Equal(decompressedData, this.TestString);
        }

        [Fact]
        public void DecompressBytes()
        {
            byte[] dataToCompressBytes = Encoding.UTF8.GetBytes(this.TestString);
            ZSTDCompressedBytes compressedData = this._zSTDWrapper.CompressBytes(dataToCompressBytes);
            byte[] compressedBytes = new byte[compressedData.length];
            Marshal.Copy(compressedData.raw_ptr, compressedBytes, 0, compressedBytes.Length);
            ZSTDDecompressedBytes decompressedBytes = this._zSTDWrapper.DecompressBytes(compressedBytes);
            byte[] decompressedData = new byte[decompressedBytes.length];
            Marshal.Copy(decompressedBytes.raw_ptr, decompressedData, 0, decompressedData.Length);
            string decompressedString = Encoding.UTF8.GetString(decompressedData);
            Assert.Equal(dataToCompressBytes.Length, decompressedData.Length);
            Assert.Equal(this.TestString, decompressedString);
            ZSTDWrapper.free_bytes_vector(compressedData.raw_ptr);
            ZSTDWrapper.free_bytes_vector(decompressedBytes.raw_ptr);
        }

        [Fact]
        public async Task DecompressBytesAsync()
        {
            byte[] dataToCompressBytes = Encoding.UTF8.GetBytes(this.TestString);
            ZSTDCompressedBytes compressedData = await this._zSTDWrapper.CompressBytesAsync(dataToCompressBytes);
            byte[] compressedBytes = new byte[compressedData.length];
            Marshal.Copy(compressedData.raw_ptr, compressedBytes, 0, compressedBytes.Length);
            ZSTDDecompressedBytes decompressedBytes = await this._zSTDWrapper.DecompressBytesAsync(compressedBytes);
            byte[] decompressedData = new byte[decompressedBytes.length];
            Marshal.Copy(decompressedBytes.raw_ptr, decompressedData, 0, decompressedData.Length);
            string decompressedString = Encoding.UTF8.GetString(decompressedData);
            Assert.Equal(dataToCompressBytes.Length, decompressedData.Length);
            Assert.Equal(this.TestString, decompressedString);
            ZSTDWrapper.free_bytes_vector(compressedData.raw_ptr);
            ZSTDWrapper.free_bytes_vector(decompressedBytes.raw_ptr);
        }
    }
}