// See https://aka.ms/new-console-template for more information
using EncryptionExampleRSA.Utilities;
using System.Security.Cryptography;


// Ensure keys exist and regenerate private key
RSAKeyManager.EnsureKeysExist();
RSAKeyManager.RegeneratePrivateKey();

// Original data
string plainText = "This is the data to be encrypted.";

// Load the public key for encryption
string publicKeyPem = File.ReadAllText("public_key.pem");
RSA rsaEncrypt = RSA.Create();
rsaEncrypt.ImportFromPem(publicKeyPem.ToCharArray());

// Encrypt data
var (encryptedData, encryptedKey, iv) = HybridEncryptionUtility.EncryptData(plainText, rsaEncrypt);

// Convert to base64 for storage/transfer
string encryptedDataBase64 = Convert.ToBase64String(encryptedData);
string encryptedKeyBase64 = Convert.ToBase64String(encryptedKey);
string ivBase64 = Convert.ToBase64String(iv);

// Print encrypted data (for demonstration purposes)
Console.WriteLine("Encrypted Data (Base64): " + encryptedDataBase64);

// Convert base64 back to byte arrays for decryption
byte[] encryptedDataBytes = Convert.FromBase64String(encryptedDataBase64);
byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKeyBase64);
byte[] ivBytes = Convert.FromBase64String(ivBase64);

// Load the private key for decryption
string privateKeyPem = File.ReadAllText("private_key.pem");
RSA rsaDecrypt = RSA.Create();
rsaDecrypt.ImportFromPem(privateKeyPem.ToCharArray());

// Decrypt data
string decryptedText = HybridEncryptionUtility.DecryptData(encryptedDataBytes, encryptedKeyBytes, ivBytes, rsaDecrypt);
Console.WriteLine("Decrypted Text: " + decryptedText);