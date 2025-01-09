// See https://aka.ms/new-console-template for more information
using EncryptionExampleRSA.Utilities;
using System.Security.Cryptography;

byte[] encryptedDataBytes, encryptedKeyBytes, ivBytes;

// Load the public key for encryption
string publicKeyPem = File.ReadAllText("public_key.pem");
using (RSA rsaEncrypt = RSA.Create())
{

    rsaEncrypt.ImportFromPem(publicKeyPem);

    string plainText = "This is the data to be encrypted.";
    Console.WriteLine("Plain Text: " + plainText);  
    var (encryptedData, encryptedKey, iv) = HybridEncryptionUtility.EncryptData(plainText, rsaEncrypt);

    // Convert to base64 for storage/transfer
    var (encryptedDataBase64, encryptedKeyBase64, ivBase64) = HybridEncryptionUtility.ConvertToBase64(encryptedData, encryptedKey, iv);

    // Send the encrypted data to the receiving party
    Console.WriteLine("Encrypted Data (Base64): " + encryptedDataBase64);
    Console.WriteLine("Encrypted Key (Base64): " + encryptedKeyBase64);
    Console.WriteLine("IV (Base64): " + ivBase64);

    // Convert base64 back to byte arrays (received data)
    (encryptedDataBytes, encryptedKeyBytes, ivBytes) = HybridEncryptionUtility.ConvertFromBase64(encryptedDataBase64, encryptedKeyBase64, ivBase64);
}

// Load the private key for decryption
string privateKeyPem = File.ReadAllText("private_key.pem");
using (RSA rsaDecrypt = RSA.Create())
{
    rsaDecrypt.ImportFromPem(privateKeyPem);

    // Decrypt data
    string decryptedText = HybridEncryptionUtility.DecryptData(encryptedDataBytes, encryptedKeyBytes, ivBytes, rsaDecrypt);
    Console.WriteLine("Decrypted Text: " + decryptedText);
}