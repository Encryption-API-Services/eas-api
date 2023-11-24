using Encryption.Compression;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Encryption.AESWrapper;

namespace Encryption.Tests
{
    public class AESWrapperTests
    {
        private readonly AESWrapper _aESWrapper;

        public AESWrapperTests()
        {
            this._aESWrapper = new AESWrapper(new ZSTDWrapper());
        }

        [Fact]
        public void Aes128Encrypt()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "TestDataToIUSADKJALSD";
            AesEncrypt result = this._aESWrapper.Aes128Encrypt(nonceKey, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            Assert.NotEqual(encrypted, dataToEncrypt);
        }

        [Fact]
        public async Task Aes128EncryptAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "TestDataToIUSADKJALSD";
            AesEncrypt result = await this._aESWrapper.Aes128EncryptAsync(nonceKey, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            Assert.NotEqual(encrypted, dataToEncrypt);
        }

        [Fact]
        public void Aes128ZSTDEncrypt()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "TestDataToIUSADKJALSD";
            AesEncrypt result = this._aESWrapper.Aes128ZSTDEncrypt(nonceKey, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            Assert.NotEqual(encrypted, dataToEncrypt);
        }

        [Fact]
        public async Task Aes128ZSTDEncryptAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "TestDataToIUSADKJALSD";
            AesEncrypt result = await this._aESWrapper.Aes128ZSTDEncryptAsync(nonceKey, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            Assert.NotEqual(encrypted, dataToEncrypt);
        }

        [Fact]
        public void Aes128EncryptWithKey()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = this._aESWrapper.Aes128Key();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            string dataToEncrypt = "EncryptThisString";
            IntPtr encryptedPtr = this._aESWrapper.EncryptAES128WithKey(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public async Task Aes128EncryptWithKeyAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = await this._aESWrapper.Aes128KeyAsync();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            string dataToEncrypt = "EncryptThisString";
            IntPtr encryptedPtr = await this._aESWrapper.EncryptAES128WithKeyAsync(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public void Aes128ZSTDEncryptWithKey()
        {
            string dataToEncrypt = "Hello World!";
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = this._aESWrapper.Aes128Key();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            IntPtr encryptedPtr = this._aESWrapper.Aes128ZSTDEncryptWithKey(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public async Task Aes128ZSTDEncryptWithKeyAsync()
        {
            string dataToEncrypt = "Hello World!";
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = await this._aESWrapper.Aes128KeyAsync();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            IntPtr encryptedPtr = await this._aESWrapper.Aes128ZSTDEncryptWithKeyAsync(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            Assert.NotEqual(dataToEncrypt, encrypted);
        }

        [Fact]
        public async Task Aes128KeyAsync()
        {
            IntPtr keyPtr = await this._aESWrapper.Aes128KeyAsync();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            Assert.True(!string.IsNullOrEmpty(key));
        }

        [Fact]
        public void Aes128Key()
        {
            IntPtr keyPtr = this._aESWrapper.Aes128Key();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            Assert.True(!string.IsNullOrEmpty(key));
        }

        [Fact]
        public void Aes128Decrypt()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = this._aESWrapper.Aes128Key();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            string dataToEncrypt = "EncryptThisString";
            IntPtr encryptedPtr = this._aESWrapper.EncryptAES128WithKey(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            IntPtr decryptedPtr = this._aESWrapper.DecryptAES128WithKey(nonceKey, key, encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public void Aes128ZSTDDecrypt()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "EncryptThisData";
            AesEncrypt result = this._aESWrapper.Aes128ZSTDEncrypt(nonceKey, dataToEncrypt);
            string compressedEncrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            string key = Marshal.PtrToStringAnsi(result.key);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            IntPtr decryptedPtr = this._aESWrapper.DecryptAES128ZSTDWithKey(nonceKey, key, compressedEncrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }
        [Fact]
        public async Task Aes128ZSTDDecryptAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string dataToEncrypt = "EncryptThisData";
            AesEncrypt result = await this._aESWrapper.Aes128ZSTDEncryptAsync(nonceKey, dataToEncrypt);
            string compressedEncrypted = Marshal.PtrToStringAnsi(result.ciphertext);
            string key = Marshal.PtrToStringAnsi(result.key);
            AESWrapper.free_cstring(result.key);
            AESWrapper.free_cstring(result.ciphertext);
            IntPtr decryptedPtr = await this._aESWrapper.DecryptAES128ZSTDWithKeyAsync(nonceKey, key, compressedEncrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }


        [Fact]
        public async Task Aes128DecryptAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            IntPtr keyPtr = await this._aESWrapper.Aes128KeyAsync();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            string dataToEncrypt = "EncryptThisString";
            IntPtr encryptedPtr = await this._aESWrapper.EncryptAES128WithKeyAsync(nonceKey, key, dataToEncrypt);
            string encrypted = Marshal.PtrToStringAnsi(encryptedPtr);
            AESWrapper.free_cstring(encryptedPtr);
            IntPtr decryptedPtr = await this._aESWrapper.DecryptAES128WithKeyAsync(nonceKey, key, encrypted);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(dataToEncrypt, decrypted);
        }

        [Fact]
        public void Aes256Key()
        {
            IntPtr keyPtr = this._aESWrapper.Aes256Key();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            Assert.True(!string.IsNullOrEmpty(key));
        }

        [Fact]
        public async Task Aes256KeyAsync()
        {
            IntPtr keyPtr = await this._aESWrapper.Aes256KeyAsync();
            string key = Marshal.PtrToStringAnsi(keyPtr);
            AESWrapper.free_cstring(keyPtr);
            Assert.True(!string.IsNullOrEmpty(key));
        }

        [Fact]
        public void EncryptPerformant()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = this._aESWrapper.EncryptPerformant(nonceKey, toEncrypt);
            string cipherText = Convert.ToBase64String(Encoding.ASCII.GetBytes(Marshal.PtrToStringAnsi(encrypted.ciphertext)));
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            Assert.NotEqual(toEncrypt, cipherText);
        }

        [Fact]
        public async Task EncryptPerformantAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to Asyn up";
            AesEncrypt encrypted = await this._aESWrapper.EncryptPerformantAsync(nonceKey, toEncrypt);
            string cipherText = Convert.ToBase64String(Encoding.ASCII.GetBytes(Marshal.PtrToStringAnsi(encrypted.ciphertext)));
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            Assert.NotEqual(toEncrypt, cipherText);
        }

        [Fact]
        public void EncryptZSTDPerformant()
        {
            string nonce = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = this._aESWrapper.EncryptZSTDPerformant(nonce, toEncrypt);
            string encryptedText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            Assert.NotEqual(toEncrypt, encryptedText);
        }

        [Fact]
        public async Task EncryptZSTDPerformantAsync()
        {
            string nonce = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = await this._aESWrapper.EncryptZSTDPerformantAsync(nonce, toEncrypt);
            string encryptedText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            Assert.NotEqual(toEncrypt, encryptedText);
        }

        [Fact]
        public void DecryptPerformant()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = this._aESWrapper.EncryptPerformant(nonceKey, toEncrypt);
            string cipherText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            string key = Marshal.PtrToStringAnsi(encrypted.key);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            IntPtr decryptedPtr = this._aESWrapper.DecryptPerformant(nonceKey, key, cipherText);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(toEncrypt, decrypted);
        }

        [Fact]
        public async Task DecryptPerformantAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = await this._aESWrapper.EncryptPerformantAsync(nonceKey, toEncrypt);
            string cipherText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            string key = Marshal.PtrToStringAnsi(encrypted.key);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            IntPtr decryptedPtr = await this._aESWrapper.DecryptPerformantAsync(nonceKey, key, cipherText);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(toEncrypt, decrypted);
        }

        [Fact]
        public void DecryptZSTDPerformant()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = this._aESWrapper.EncryptZSTDPerformant(nonceKey, toEncrypt);
            string cipherText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            string key = Marshal.PtrToStringAnsi(encrypted.key);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            IntPtr decryptedPtr = this._aESWrapper.DecryptZSTDPerformant(nonceKey, key, cipherText);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(toEncrypt, decrypted);
        }

        [Fact]
        public async Task DecryptZSTDPerformantAsync()
        {
            string nonceKey = this._aESWrapper.GenerateAESNonce();
            string toEncrypt = "Text to encrypt";
            AesEncrypt encrypted = await this._aESWrapper.EncryptZSTDPerformantAsync(nonceKey, toEncrypt);
            string cipherText = Marshal.PtrToStringAnsi(encrypted.ciphertext);
            string key = Marshal.PtrToStringAnsi(encrypted.key);
            AESWrapper.free_cstring(encrypted.ciphertext);
            AESWrapper.free_cstring(encrypted.key);
            IntPtr decryptedPtr = await this._aESWrapper.DecryptZSTDPerformantAsync(nonceKey, key, cipherText);
            string decrypted = Marshal.PtrToStringAnsi(decryptedPtr);
            AESWrapper.free_cstring(decryptedPtr);
            Assert.Equal(toEncrypt, decrypted);
        }
    }
}
