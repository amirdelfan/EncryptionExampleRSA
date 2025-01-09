using System.Security.Cryptography;

namespace EncryptionExampleRSA.Utilities
{
    internal static class RSAKeyManager
    {
        private static string publicKeyPath = "public_key.pem";
        private static string privateKeyPath = "private_key.pem";

        public static void GenerateAndStoreKeys()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                // Export and store the public key
                string publicKeyPem = rsa.ExportRSAPublicKeyPem();
                File.WriteAllText(publicKeyPath, publicKeyPem);

                // Export and store the private key
                string privateKeyPem = rsa.ExportRSAPrivateKeyPem();
                File.WriteAllText(privateKeyPath, privateKeyPem);
            }
        }
    }
}
