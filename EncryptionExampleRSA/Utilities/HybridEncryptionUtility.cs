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
        /// <summary>
        /// Encrypts the specified plain text data using AES and then encrypts the AES key using RSA
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <param name="rsa">The RSA instance used to encrypt the AES key.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description>EncryptedData: The encrypted data.</description></item>
        /// <item><description>EncryptedKey: The AES key encrypted with RSA.</description></item>
        /// <item><description>Iv: The initialization vector used for AES encryption.
        // The IV is a random or pseudorandom value used to ensure that the same plaintext encrypts to different ciphertexts
        // each time the encryption is performed. This provides an additional layer of security by preventing identical plaintext
        // blocks from producing identical ciphertext blocks, which could otherwise reveal patterns in the encrypted data.
        /// </description></item>
        /// </list>
        /// </returns>
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

        /// <summary>
        /// Decrypts the specified encrypted data using the provided RSA instance to decrypt the AES key.
        /// </summary>
        /// <param name="encryptedData">The data that was encrypted using AES.</param>
        /// <param name="encryptedKey">The AES key that was encrypted using RSA.</param>
        /// <param name="iv">The initialization vector used for AES encryption.</param>
        /// <param name="rsa">The RSA instance used to decrypt the AES key.</param>
        /// <returns>The decrypted plain text data.</returns>
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


        /// <summary>
        /// Converts the encrypted data, encrypted key, and IV to their Base64 string representations.
        /// </summary>
        /// <param name="encryptedData">The encrypted data as a byte array.</param>
        /// <param name="encryptedKey">The encrypted AES key as a byte array.</param>
        /// <param name="iv">The initialization vector used for AES encryption as a byte array.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description>encryptedDataBase64: The encrypted data as a Base64 string.</description></item>
        /// <item><description>encryptedKeyBase64: The encrypted AES key as a Base64 string.</description></item>
        /// <item><description>ivBase64: The initialization vector as a Base64 string.</description></item>
        /// </list>
        /// </returns>
        public static (string encryptedDataBase64, string encryptedKeyBase64, string ivBase64) ConvertToBase64(byte[] encryptedData, byte[] encryptedKey, byte[] iv)
        {
            string encryptedDataBase64 = Convert.ToBase64String(encryptedData);
            string encryptedKeyBase64 = Convert.ToBase64String(encryptedKey);
            string ivBase64 = Convert.ToBase64String(iv);
            return (encryptedDataBase64, encryptedKeyBase64, ivBase64);
        }

        /// <summary>
        /// Converts the Base64 string representations of the encrypted data, encrypted key, and IV to their byte array equivalents.
        /// </summary>
        /// <param name="encryptedDataBase64">The encrypted data as a Base64 string.</param>
        /// <param name="encryptedKeyBase64">The encrypted AES key as a Base64 string.</param>
        /// <param name="ivBase64">The initialization vector used for AES encryption as a Base64 string.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description>encryptedDataBytes: The encrypted data as a byte array.</description></item>
        /// <item><description>encryptedKeyBytes: The encrypted AES key as a byte array.</description></item>
        /// <item><description>ivBytes: The initialization vector as a byte array.</description></item>
        /// </list>
        /// </returns>
        public static (byte[] encryptedDataBytes, byte[] encryptedKeyBytes, byte[] ivBytes) ConvertFromBase64(string encryptedDataBase64, string encryptedKeyBase64, string ivBase64)
        {
            byte[] encryptedDataBytes = Convert.FromBase64String(encryptedDataBase64);
            byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKeyBase64);
            byte[] ivBytes = Convert.FromBase64String(ivBase64);
            return (encryptedDataBytes, encryptedKeyBytes, ivBytes);
        }
    }
}