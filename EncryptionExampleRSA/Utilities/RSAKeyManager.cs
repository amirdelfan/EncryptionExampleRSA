using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

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

        internal static void CreateCertificates()
        {
            // Read the private key from the PEM file  
            string privateKeyPem = File.ReadAllText(privateKeyPath);
            RSA privateKey = RSA.Create();
            privateKey.ImportFromPem(privateKeyPem);

            // Create a self-signed certificate using the public and private keys  
            var certificateRequest = new CertificateRequest("CN=EcryptionCertificate", privateKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            X509Certificate2 certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
            certificate.FriendlyName = "Encryption Certificate";

            // Export the certificate with the private key  
            byte[] certBytes = certificate.Export(X509ContentType.Pfx, string.Empty);
            File.WriteAllBytes("encryption.pfx", certBytes);

            // Export the public certificate  
            byte[] publicCertBytes = certificate.Export(X509ContentType.Cert);
            File.WriteAllBytes("encryption.cer", publicCertBytes);
        }

        internal static void ImportCertificates()
        {
            // Check if the application is running with administrative privileges
            if (!IsAdministrator())
            {
                throw new UnauthorizedAccessException("Administrator privileges are required to import certificates.");
            }

            // Import the PFX certificate into the Personal certificates store
            byte[] pfxBytes = File.ReadAllBytes("encryption.pfx");
            X509Certificate2 pfxCert = new X509Certificate2(pfxBytes, string.Empty, X509KeyStorageFlags.PersistKeySet);
            using (X509Store personalStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                personalStore.Open(OpenFlags.ReadWrite);
                personalStore.Add(pfxCert);
                personalStore.Close();
            }

            // Import the CER certificate into the Trusted Root certificates store
            byte[] cerBytes = File.ReadAllBytes("encryption.cer");
            X509Certificate2 cerCert = new X509Certificate2(cerBytes);
            using (X509Store trustedRootStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                trustedRootStore.Open(OpenFlags.ReadWrite);
                trustedRootStore.Add(cerCert);
                trustedRootStore.Close();
            }
        }

        private static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
