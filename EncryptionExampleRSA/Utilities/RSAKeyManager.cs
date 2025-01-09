using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionExampleRSA.Utilities
{
    internal class RSAKeyManager
    {
        private static string publicKeyPath = "public_key.pem";
        private static string privateKeyPath = "private_key.pem";

        public static void EnsureKeysExist()
        {
            if (!File.Exists(publicKeyPath))
            {
                // Generate RSA keys
                RSA rsa = RSA.Create();
                rsa.KeySize = 2048;

                // Export and store the public key
                string publicKeyPem = ExportPublicKeyToPem(rsa);
                File.WriteAllText(publicKeyPath, publicKeyPem);

                // Export and store the private key
                string privateKeyPem = ExportPrivateKeyToPem(rsa);
                File.WriteAllText(privateKeyPath, privateKeyPem);
            }
        }

        public static void RegeneratePrivateKey()
        {
            // Load the constant public key
            string publicKeyPem = File.ReadAllText(publicKeyPath);

            // Generate a new private key
            RSA rsa = RSA.Create();
            rsa.KeySize = 2048;

            // Ensure the new private key matches the constant public key
            rsa.ImportFromPem(publicKeyPem.ToCharArray());

            // Export and store the new private key
            string privateKeyPem = ExportPrivateKeyToPem(rsa);
            File.WriteAllText(privateKeyPath, privateKeyPem);
        }

        private static string ExportPublicKeyToPem(RSA rsa)
        {
            var key = rsa.ExportParameters(false);
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PUBLIC KEY-----");
            sb.AppendLine(Convert.ToBase64String(key.Modulus, Base64FormattingOptions.InsertLineBreaks));
            sb.AppendLine("-----END PUBLIC KEY-----");
            return sb.ToString();
        }

        private static string ExportPrivateKeyToPem(RSA rsa)
        {
            var key = rsa.ExportParameters(true);
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PRIVATE KEY-----");
            sb.AppendLine(Convert.ToBase64String(key.Modulus, Base64FormattingOptions.InsertLineBreaks));
            sb.AppendLine("-----END PRIVATE KEY-----");
            return sb.ToString();
        }
    }
}
