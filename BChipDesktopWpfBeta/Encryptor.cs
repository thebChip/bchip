using System.Collections.Generic;
using Windows.Security.Cryptography;
using System.Linq;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;

namespace bChipDesktop
{
    public class Encryptor
    {
        public static int GeneratePinCode(byte[] byteToHash, int maxLength)
        {
            IBuffer buff = byteToHash.AsBuffer();
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            IBuffer buffHash = objAlgProv.HashData(buff);
            int finalCount = 0;
            foreach (byte entry in buffHash.ToArray())
            {
                finalCount += (int)entry;
            }
            return finalCount % maxLength;
        }

        public static byte[] CalculateSha512(byte[] byteToHash)
        {
            IBuffer buff = byteToHash.AsBuffer(); 
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            IBuffer buffHash = objAlgProv.HashData(buff);
            return buffHash.ToArray();
        }

        public static List<byte[]> GeneratePassword(string passphrase, int maxKeys)
        {
            return GeneratePassword(CryptographicBuffer.ConvertStringToBinary(passphrase, BinaryStringEncoding.Utf8).ToArray(), maxKeys);
        }

        public static List<byte[]> GeneratePassword(byte[] passphrase, int maxKeys)
        {
            List<byte[]> potentialKeys = new List<byte[]>();
            byte[] sha512Source = CalculateSha512(passphrase);
            for (int i = 0; i < maxKeys; i++)
            {
                potentialKeys.Add(sha512Source);
                sha512Source = CalculateSha512(sha512Source);
            }

            return potentialKeys;
        }

        public static byte[] GenerateSalt(byte[] passwordHash)
        {
            // Goal here is to make a rainbow table rather large as many users may choose simple passwords,
            // currently re-hashing the same bytes many times and randomly using one of those values.
            // In the future, we're likely to use the security
            // PIN forcing 3 attempts instead of wasting time.

            byte[] sha512Source = CalculateSha512(passwordHash);
            for (int i = 0; i < 16; i++)
            {
                sha512Source = CalculateSha512(sha512Source);
            }
            
            return sha512Source;
        }

        public static IBuffer Encrypt(IBuffer dataToEncrypt, IBuffer passwordBuffer, IBuffer saltBuffer)
        {
            // Generate a key and IV from the password and salt
            IBuffer aesKeyMaterial;
            IBuffer iv;
            uint iterationCount = 10000;
            GenerateKeyMaterial(passwordBuffer, saltBuffer, iterationCount, out aesKeyMaterial, out iv);

            SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

            IBuffer encrypted = CryptographicEngine.Encrypt(aesKey, dataToEncrypt, iv);
            return encrypted;
        }

        public static IBuffer Decrypt(IBuffer dataToDecrypt, IBuffer passwordBuffer, IBuffer saltBuffer)
        {
            // Generate a key and IV from the password and salt
            IBuffer aesKeyMaterial;
            IBuffer iv;
            uint iterationCount = 10000;
            GenerateKeyMaterial(passwordBuffer, saltBuffer, iterationCount, out aesKeyMaterial, out iv);

            try
            {
                SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

                // Decrypt the data and convert it back to a string
                IBuffer decrypted = CryptographicEngine.Decrypt(aesKey, dataToDecrypt, iv);
                return decrypted;
            }
            catch
            {
                return null;
            }
        }

        private static void GenerateKeyMaterial(IBuffer passwordBuffer, IBuffer saltBuffer, uint iterationCount, out IBuffer keyMaterial, out IBuffer iv)
        {
            KeyDerivationParameters kdfParameters = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, iterationCount);

            // Get a KDF provider for PBKDF2, and store the source password in a Cryptographic Key
            KeyDerivationAlgorithmProvider kdf = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha256);
            CryptographicKey passwordSourceKey = kdf.CreateKey(passwordBuffer);

            // Generate key material from the source password, salt, and iteration count.  Only call DeriveKeyMaterial once,
            // since calling it twice will generate the same data for the key and IV.
            int keySize = 256 / 8;
            int ivSize = 128 / 8;
            uint totalDataNeeded = (uint)(keySize + ivSize);
            IBuffer keyAndIv = CryptographicEngine.DeriveKeyMaterial(passwordSourceKey, kdfParameters, totalDataNeeded);

            // Split the derived bytes into a seperate key and IV
            byte[] keyMaterialBytes = keyAndIv.ToArray();
            keyMaterial = WindowsRuntimeBuffer.Create(keyMaterialBytes, 0, keySize, keySize);
            iv = WindowsRuntimeBuffer.Create(keyMaterialBytes, keySize, ivSize, ivSize);
        }
    }
}
