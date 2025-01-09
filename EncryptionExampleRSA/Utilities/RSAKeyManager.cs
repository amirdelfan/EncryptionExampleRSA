using System.Security.Cryptography;

namespace EncryptionExampleRSA.Utilities
{
    internal static class RSAKeyManager
    {
        internal static string publicKeyPath = "public_key.pem";
        internal static string privateKeyPath = "private_key.pem";

        internal static void GenerateAndStoreKeys()
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
