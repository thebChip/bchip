using bChipDesktop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
using BChipDesktop;

namespace BChipTestSuite
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void ValidateEncryption()
        {
            // Quick and dirty
            int maxKeys = 256;

            // Generate keys
            string passphrase = "Test";
            var potPasswords = Encryptor.GeneratePassword(passphrase, maxKeys);
            byte[] initialPassword = Encryptor.CalculateSha512(CryptographicBuffer.ConvertStringToBinary(passphrase, BinaryStringEncoding.Utf8).ToArray());
            // Eventually we'll implement the random function and force the client to go through all the keys
            int generatedPin = Encryptor.GeneratePinCode(initialPassword, maxKeys);

            // Selected a key, Generate IV 
            byte[] chosenPassword = potPasswords[generatedPin % potPasswords.Count];
            byte[] expectedChosenPassword = new byte[] { 0x68, 0x94, 0xe5, 0xe4, 0x67, 0x78, 0x55, 0x94, 0xd9, 0xdd, 0xbd, 0x4d, 0xbb, 0x01, 0x5b, 0xc6, 0x11, 0xc0, 0xa4, 0xed, 0x9c, 0x52, 0x9a, 0x82, 0x27, 0x3a, 0x7a, 0x8e, 0xeb, 0x0e, 0x68, 0x19, 0xde, 0xe8, 0x84, 0x9f, 0xe0, 0xea, 0x79, 0xc0, 0xcb, 0x95, 0xab, 0x4f, 0xd0, 0x20, 0xb2, 0x14, 0x09, 0x5b, 0x29, 0x9f, 0x1d, 0x56, 0xa1, 0xe2, 0xfe, 0x97, 0x59, 0xf7, 0xee, 0xc2, 0xba, 0x83 };
            Assert.IsTrue(chosenPassword.Length == expectedChosenPassword.Length, 
                $"Expected number of bytes was {expectedChosenPassword.Length} bytes, had {chosenPassword.Length}.");
            for (int i = 0; i < chosenPassword.Length; i++)
            {
                Assert.IsTrue(chosenPassword[i] == expectedChosenPassword[i], $"Byte[{i}] did not match: {chosenPassword[i]}!={expectedChosenPassword[i]}");
            }

            byte[] salt = new byte[] { 0x8f, 0x1a, 0x31, 0x7b, 0xcf, 0x31, 0x33, 0x91, 0x83 };
            byte[] chosenSalt = Encryptor.GenerateSalt(salt);
            byte[] expectedSalt = new byte[] { 0x4c, 0x03, 0x18, 0xad, 0xdd, 0xa7, 0x53, 0x9f, 0x6d, 0x01, 0x33, 0x68, 0x0b, 0xf9, 0x84, 0xeb, 0x71, 0x32, 0x33, 0x59, 0x37, 0xb9, 0x9b, 0x8d, 0x0b, 0x80, 0x3f, 0x21, 0x7e, 0xc6, 0xee, 0xd9, 0x7a, 0xd1, 0xaa, 0x5c, 0xdc, 0x56, 0xe2, 0x91, 0xb2, 0xd0, 0x69, 0xbf, 0x62, 0xb4, 0x0a, 0xf5, 0x56, 0x22, 0x18, 0xd4, 0x13, 0x10, 0xfe, 0x08, 0x5e, 0xc7, 0xde, 0x3f, 0xb0, 0x60, 0x58, 0xd8 };
            Assert.IsTrue(expectedSalt.Length == chosenSalt.Length, $"Expected number of bytes was {expectedSalt.Length} bytes, had {chosenSalt.Length}.");
            for (int i = 0; i < expectedSalt.Length; i++)
            {
                Assert.IsTrue(expectedSalt[i] == chosenSalt[i], $"Byte[{i}] did not match: {expectedSalt[i]}!={chosenSalt[i]}");
            }
            
            IBuffer toEnc = CryptographicBuffer.ConvertStringToBinary("This is a small test.", BinaryStringEncoding.Utf8);
            byte[] toEncrypt = toEnc.ToArray();
            byte[] expectedToEnc = new byte[]
                { 0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x73, 0x6d, 0x61, 0x6c, 0x6c, 0x20, 0x74, 0x65, 0x73, 0x74, 0x2e };
            Assert.IsTrue(toEncrypt.Length == expectedToEnc.Length, $"Expected number of bytes was {toEncrypt.Length} bytes, had {expectedToEnc.Length}.");
            for (int i = 0; i < toEncrypt.Length; i++)
            {
                Assert.IsTrue(toEncrypt[i] == expectedToEnc[i], $"Byte[{i}] did not match: {toEncrypt[i]}!={expectedToEnc[i]}");
            }

            byte[] encryptedData = Encryptor.Encrypt(toEnc, chosenPassword.AsBuffer(), chosenSalt.AsBuffer()).ToArray();
            byte[] expectedEncBytes = new byte[]
                { 0x8b, 0x2d,0x75,0xa6,0x36,0x29,0x2d,0x08,0x41,0x41,0x81,0x8a,0x02,0x35, 0xfa,0x19,
                  0x4e,0x16,0x74,0xe8,0x01,0xa9,0x14,0xba,0xbe,0xde, 0xbe, 0xc2, 0xd4, 0x9d, 0xda, 0x72 };
            Assert.IsTrue(encryptedData.Length == expectedEncBytes.Length, $"Expected number of bytes was {expectedEncBytes} bytes, had {encryptedData.Length}.");
            for (int i = 0; i < expectedEncBytes.Length; i++)
            {
                Assert.IsTrue(expectedEncBytes[i] == encryptedData[i], $"Byte[{i}] did not match: {expectedEncBytes[i]}!={encryptedData[i]}");
            }
        
            // Decrypt
            byte[] decryptedData = Encryptor.Decrypt(encryptedData.AsBuffer(), chosenPassword.AsBuffer(), chosenSalt.AsBuffer()).ToArray();
            byte[] expectedDecBytes = new byte[] { 0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x73, 0x6d, 0x61, 0x6c, 0x6c, 0x20, 0x74, 0x65, 0x73, 0x74, 0x2e };
            Assert.IsTrue(decryptedData.Length == expectedDecBytes.Length, $"Expected number of bytes was {expectedDecBytes.Length} bytes, had {decryptedData.Length}.");
            for (int i = 0; i < expectedDecBytes.Length; i++)
            {
                Assert.IsTrue(expectedDecBytes[i] == decryptedData[i], $"Byte[{i}] did not match: {expectedDecBytes[i]}!={decryptedData[i]}");
            }

            string decryptedString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedData.AsBuffer());
            Assert.AreEqual("This is a small test.", decryptedString, false, $"Encrypted message and decrypted message was not a match: {decryptedString} != This is a small test.");
        }

        [TestMethod]
        public void CrcValidation()
        {
            byte[] mlvi = CryptographicBuffer.DecodeFromHexString("a1c46b44c67a053a").ToArray();
            byte[] salt = CryptographicBuffer.DecodeFromHexString("5555555555555555").ToArray();
            byte[] bchipid = CryptographicBuffer.DecodeFromHexString("febbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb").ToArray();
            byte[] pubkey = new byte[BChipMemoryLayout_BCHIP.PUBKEY_MAX_DATA];
            for (int i = 0; i < BChipMemoryLayout_BCHIP.PUBKEY_MAX_DATA; i++)
            {
                pubkey[i] = 0xaa;
            }
            byte[] privkey = new byte[BChipMemoryLayout_BCHIP.PRIVATEKEY_MAX_DATA];
            for (int i = 0; i < BChipMemoryLayout_BCHIP.PRIVATEKEY_MAX_DATA; i++)
            {
                privkey[i] = 0x0;
            }
            
            byte[] checksum = CryptographicBuffer.DecodeFromHexString("6c66ba9ae44d9b").ToArray();
            List<byte> simulatedCard = new List<byte>();
            simulatedCard.AddRange(salt);
            simulatedCard.AddRange(bchipid);
            simulatedCard.AddRange(pubkey);
            simulatedCard.AddRange(privkey);
            simulatedCard.AddRange(checksum);
            
            BChipMemoryLayout_BCHIP bChipCardData =
                new BChipMemoryLayout_BCHIP(
                    mlvi,
                    simulatedCard.ToArray(),
                    false,
                    PKStatus.NotAvailable);

            Assert.IsTrue(bChipCardData.crcData.Length == BChipMemoryLayout_BCHIP.CRC_MAX_SIZE);

            for (int i = 0; i < checksum.Length; ++i)
            {
                Assert.AreEqual(checksum[i], bChipCardData.crcData[i]);
            }

            byte[] calculatedChecksum = bChipCardData.GetCardCheckSum();
            for (int i = 0; i < checksum.Length; ++i)
            {
                Assert.AreEqual(checksum[i], calculatedChecksum[i]);
            }

        }
    }
}
