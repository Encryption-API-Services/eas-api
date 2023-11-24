using Encryption.Compression;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;
using static Encryption.AESWrapper;
using static Encryption.RustRSAWrapper;

namespace Encryption.Tests
{
    public class AESRSAHybridEncryptionTests
    {
        private readonly AESWrapper _aesWrapper;
        private readonly RustRSAWrapper _rsaWrapper;


        public AESRSAHybridEncryptionTests()
        {
            this._aesWrapper = new AESWrapper(new ZSTDWrapper());
            this._rsaWrapper = new RustRSAWrapper(new ZSTDWrapper());
        }

        [Fact]
        public void AESRSAHybridEncrypt()
        {
            string dataToEncrypt = "DataToEncrypt";
            string nonce = "TestingNonce";
            RustRsaKeyPair keyPair = this._rsaWrapper.GetKeyPair(2048);
            AesEncrypt encryptedData = this._aesWrapper.EncryptPerformant(nonce, dataToEncrypt);
            string ciphertext = Marshal.PtrToStringAnsi(encryptedData.ciphertext);
            string aesKey = Marshal.PtrToStringAnsi(encryptedData.key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            IntPtr encryptedAesKeyPtr = this._rsaWrapper.RsaEncrypt(publicKey, aesKey);
            string encryptedAesKey = Marshal.PtrToStringAnsi(encryptedAesKeyPtr);

            AESWrapper.free_cstring(encryptedData.ciphertext);
            AESWrapper.free_cstring(encryptedData.key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(encryptedAesKeyPtr);

            Assert.NotEqual(aesKey, encryptedAesKey);
            Assert.NotEqual(dataToEncrypt, ciphertext);
        }

        [Fact]
        public async Task AESRSAHybridEncryptAsync()
        {
            string dataToEncrypt = "DataToEncrypt";
            string nonce = "TestingNonce";
            RustRsaKeyPair keyPair = this._rsaWrapper.GetKeyPair(2048);
            AesEncrypt encryptedData = await this._aesWrapper.EncryptPerformantAsync(nonce, dataToEncrypt);
            string ciphertext = Marshal.PtrToStringAnsi(encryptedData.ciphertext);
            string aesKey = Marshal.PtrToStringAnsi(encryptedData.key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            IntPtr encryptedAesKeyPtr = await this._rsaWrapper.RsaEncryptAsync(publicKey, aesKey);
            string encryptedAesKey = Marshal.PtrToStringAnsi(encryptedAesKeyPtr);

            AESWrapper.free_cstring(encryptedData.ciphertext);
            AESWrapper.free_cstring(encryptedData.key);
            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            RustRSAWrapper.free_cstring(encryptedAesKeyPtr);

            Assert.NotEqual(aesKey, encryptedAesKey);
            Assert.NotEqual(dataToEncrypt, ciphertext);
        }

        [Fact]
        public void AESRSAHybridDecrypt()
        {
            string dataToEncrypt = "DataToEncrypt";
            string nonce = "TestingNonce";
            RustRsaKeyPair keyPair = this._rsaWrapper.GetKeyPair(2048);
            AesEncrypt encryptedData = this._aesWrapper.EncryptPerformant(nonce, dataToEncrypt);
            string ciphertext = Marshal.PtrToStringAnsi(encryptedData.ciphertext);
            string aesKey = Marshal.PtrToStringAnsi(encryptedData.key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            IntPtr encryptedAesKeyPtr = this._rsaWrapper.RsaEncrypt(publicKey, aesKey);
            string encryptedAesKey = Marshal.PtrToStringAnsi(encryptedAesKeyPtr);

            IntPtr decryptedAesKeyPtr = this._rsaWrapper.RsaDecrypt(privateKey, encryptedAesKey);
            string decryptedAesKey = Marshal.PtrToStringAnsi(decryptedAesKeyPtr);
            IntPtr decryptedDataPtr = this._aesWrapper.DecryptPerformant(nonce, decryptedAesKey, ciphertext);
            string decryptedData = Marshal.PtrToStringAnsi(decryptedDataPtr);

            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            AESWrapper.free_cstring(encryptedData.ciphertext);
            AESWrapper.free_cstring(encryptedData.key);
            AESWrapper.free_cstring(encryptedAesKeyPtr);
            AESWrapper.free_cstring(decryptedDataPtr);


            Assert.Equal(decryptedData, dataToEncrypt);
        }

        [Fact]
        public async Task AESRSAEncryptDecryptAsync()
        {
            string dataToEncrypt = "DataToEncrypt";
            string nonce = "TestingNonce";
            RustRsaKeyPair keyPair = await this._rsaWrapper.GetKeyPairAsync(2048);
            AesEncrypt encryptedData = await this._aesWrapper.EncryptPerformantAsync(nonce, dataToEncrypt);
            string ciphertext = Marshal.PtrToStringAnsi(encryptedData.ciphertext);
            string aesKey = Marshal.PtrToStringAnsi(encryptedData.key);
            string publicKey = Marshal.PtrToStringAnsi(keyPair.pub_key);
            string privateKey = Marshal.PtrToStringAnsi(keyPair.priv_key);
            IntPtr encryptedAesKeyPtr = await this._rsaWrapper.RsaEncryptAsync(publicKey, aesKey);
            string encryptedAesKey = Marshal.PtrToStringAnsi(encryptedAesKeyPtr);

            IntPtr decryptedAesKeyPtr = await this._rsaWrapper.RsaDecryptAsync(privateKey, encryptedAesKey);
            string decryptedAesKey = Marshal.PtrToStringAnsi(decryptedAesKeyPtr);
            IntPtr decryptedDataPtr = await this._aesWrapper.DecryptPerformantAsync(nonce, decryptedAesKey, ciphertext);
            string decryptedData = Marshal.PtrToStringAnsi(decryptedDataPtr);

            RustRSAWrapper.free_cstring(keyPair.pub_key);
            RustRSAWrapper.free_cstring(keyPair.priv_key);
            AESWrapper.free_cstring(encryptedData.ciphertext);
            AESWrapper.free_cstring(encryptedData.key);
            AESWrapper.free_cstring(encryptedAesKeyPtr);
            AESWrapper.free_cstring(decryptedDataPtr);


            Assert.Equal(decryptedData, dataToEncrypt);
        }
    }
}