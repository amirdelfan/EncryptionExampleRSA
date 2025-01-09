using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionExampleRSA.Utilities
{
    internal class HybridEncryptionUtility
    {
        public static (byte[] EncryptedData, byte[] EncryptedKey, byte[] Iv) EncryptData(string plainText, RSA rsa)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedData;
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encryptedData = msEncrypt.ToArray();
                }
                byte[] encryptedKey = rsa.Encrypt(aesAlg.Key, RSAEncryptionPadding.OaepSHA256);
                return (encryptedData, encryptedKey, aesAlg.IV);
            }
        }

        public static string DecryptData(byte[] encryptedData, byte[] encryptedKey, byte[] iv, RSA rsa)
        {
            byte[] key = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new System.IO.MemoryStream(encryptedData))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}