using System;
using BChipDesktop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBitcoin;

namespace BChipTestSuite
{
    [TestClass]
    public class Bip39HelperTests
    {
        private static readonly string[] words = 
            new string[] {
                "below", "jealous", "whisper", "topic", "minute", "sort",
                "mountain", "evidence", "vacuum", "unaware", "involve", "talent" };

        private static readonly byte[] entropyData = new byte[]
        {
            0x14, 0xce, 0xf3, 0xea, 0x72, 0x88, 0xd5, 0x9f,
            0x24, 0x22, 0x6e, 0xf0, 0x9d, 0x8d, 0xd8, 0xee
        };

        [TestMethod]
        public void GenerateMnemonicFromEntropy()
        {
            string[] decodedWords= BIP39Helpers.DecodeMnemonicFromEntropy(entropyData, Wordlist.English);
            Assert.IsTrue(decodedWords.Length == words.Length, 
                $"Unexpected work count; {decodedWords.Length} != {words.Length}"); 
            for (int i = 0; i < words.Length; ++i)
            {
                Assert.AreEqual(words[i], decodedWords[i],
                    $"Words were not equal: {words[i]} != {decodedWords[i]}");
            }
        }

        [TestMethod]
        public void GenerateEntropyFromMnemonic()
        {
            byte[] decodedEntropy = BIP39Helpers.GenerateEntropyFromWords(words, Wordlist.English);
            Assert.IsTrue(decodedEntropy.Length == entropyData.Length,
                $"Unexpected byte count; {decodedEntropy.Length} != {entropyData.Length}");
            for (int i = 0; i < words.Length; ++i)
            {
                Assert.AreEqual(entropyData[i], decodedEntropy[i],
                    $"bytes were not equal: {entropyData[i]:X} != {decodedEntropy[i]}:X");
            }
        }

    }
}
