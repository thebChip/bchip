using NBitcoin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BChipDesktop
{
    public static class BIP39Helpers
    {
        public static string[] DecodeMnemonicFromEntropy(byte[] entropy, Wordlist wordList)
        {
            if (entropy == null || entropy.Length == 0)
            {
                throw new Exception("Seed was null or empty.");
            }

            List<string> mnemonic = new List<string>();

            if (entropy.Length % 4 > 0)
            {
                throw new Exception("Seed must be multiples of 32 bits of entropy");
            }
            else if (entropy.Length < 8)
            {
                throw new Exception("Seed must be at least 64 bits of entropy");
            }
            else if (entropy.Length > 124)
            {
                throw new Exception("Seed must not exceed 992 bits of entropy");
            }

            BitArray entropyBitArray = new BitArray(BytesToBoolArray(entropy));
            int checksumLengthBits = entropyBitArray.Length / 32;
            Array entropyBits = Array.CreateInstance(typeof(bool), entropyBitArray.Length);
            entropyBitArray.CopyTo(entropyBits, 0);

            byte[] hash = SHA256.Create().ComputeHash(entropy);
            BitArray hashBitArray = new BitArray(BytesToBoolArray(hash));
            Array hashBits = Array.CreateInstance(typeof(bool), checksumLengthBits);
            for (int i = 0; i < checksumLengthBits; i++)
            {
                bool bit = hashBitArray.Get(i); 
                hashBits.SetValue(bit, i);
            }
            
            Array concatBits = Array.CreateInstance(typeof(bool), entropyBits.Length + checksumLengthBits);
            entropyBits.CopyTo(concatBits, 0);
            hashBits.CopyTo(concatBits, entropyBits.Length);
            
            int nwords = concatBits.Length / 11;

            for (int i = 0; i < nwords; ++i)
            {
                // 11 bits per word (2048)
                int index = 0;
                for (int j = 0; j < 11; ++j)
                {
                    index <<= 1;
                    bool bit = (bool)concatBits.GetValue((i * 11) + j);
                    if (bit)
                    {
                        index |= 0b1;
                    }
                }

                mnemonic.Add(wordList.GetWordAtIndex(index));
            }
            
            return mnemonic.ToArray();
        }

        public static byte[] GenerateEntropyFromWords(string[] words, Wordlist wordList)
        {
            // Should always have at least 12 words
            if (words.Length % 3 > 0 || words.Length == 0)
            {
                throw new Exception("Word list size must be a multiple of three words");
            }

            int concatLenBits = words.Length * 11;
            Array concatBits = Array.CreateInstance(typeof(bool), concatLenBits);

            int[] wordIndices = wordList.ToIndices(words);

            int wordindex = 0;
            foreach(int wordIndice in wordIndices)
            {
                // 11 bits per word (2048)
                for (int i = 0; i < 11; ++i)
                {
                    bool bit = (wordIndice & (1 << (10 - i))) != 0;
                    concatBits.SetValue(bit, (wordindex * 11) + i);
                }
                ++wordindex;
            }

            int checksumLengthBits = concatLenBits / 33;
            int entropyLengthBits = concatLenBits - checksumLengthBits;
            
            byte[] entropy = new byte[entropyLengthBits / 8];
            for (int i = 0; i < entropy.Length; ++i)
            {
                // 8 bits per byte...
                for (int j = 0; j < 8; ++j)
                {
                    bool bit = (bool)concatBits.GetValue((i * 8) + j);
                    if (bit)
                    {
                        entropy[i] |= (byte)(1 << (7 - j));
                    }
                }
            }
            
            byte[] hash = SHA256.Create().ComputeHash(entropy);
            BitArray hashBitArray = new BitArray(BytesToBoolArray(hash));
            Array hashBits = Array.CreateInstance(typeof(bool), checksumLengthBits);
            for (int i = 0; i < checksumLengthBits; i++)
            {
                hashBits.SetValue(hashBitArray.Get(i), i);
            }
            
            for (int i = 0; i < checksumLengthBits; ++i)
            {
                bool concatbit = (bool)concatBits.GetValue(entropyLengthBits + i);
                bool hashbit = (bool)hashBits.GetValue(i);
                if (concatbit != hashbit)
                {
                    throw new Exception("CRC Checksum validation failed");
                }
            }

            return entropy;
        }

        // BitArray from byte[] is not reliable
        private static bool[] BytesToBoolArray(byte[] data)
        {
            bool[] bits = new bool[data.Length * 8];
            for (int i = 0; i < data.Length; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    bits[(i * 8) + j] = (data[i] & (1 << (7 - j))) != 0;
                }
            }
            return bits;
        }
    }
}
