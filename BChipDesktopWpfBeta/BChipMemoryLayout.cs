using bChipDesktop;
using NBitcoin;
using PCSC;
using PCSC.Iso7816;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace BChipDesktop
{
    /// 224 bytes free to write outside of the protected region
    /// Regions for v0a: 
    ///         Write Once Protected Area (unused): 0x00-0x20      (32 bytes)
    ///         ID Memory:        (0x20)+0x00-0x07                 (8 bytes)
    ///         Unused Memory:    (0x20)+0x08-0xFF                 (214 bytes)
    public abstract class BChipMemoryLayout 
    {
        // 8 bytes - Memory Layout Version Identifier
        // NB: Initial release is v0a-XXYYYYYYYYYYYYYY
        //  The "XX" in an mlvi identifier is the bchip card type identifier
        //  The "YY" is the cards unique identifier
        // TODO: This will eventually be part of the ROM of the card and generated when shipped
        public const int MLVI_ADDR = 0;
        public const int MLVI_MAX_DATA = 8;
        public byte[] mlvi { get; }

        public BChipMemoryLayout(CardType cardType, byte[] cardId)
        {
            if (cardId == null || cardId.Length != 7)
            {
                throw new Exception("Card ID should be 7 bytes.");
            }

            // Incase the mlvi was not pre-initialized, let's create one
            if (cardId.Count(a => a == 0xFF) >= 6)
            {
                cardId = new byte[7];
                RandomNumberGenerator.Create().GetBytes(cardId);
            }

            this.cardType = cardType;
            this.mlvi = new byte[8];
            this.mlvi[0] = (byte)cardType;
            for (int i = 1; i < mlvi.Length; ++i)
            {
                this.mlvi[i] = cardId[i - 1];
            }
        }

        public BChipMemoryLayout(byte[] mlvi)
        {
            if (mlvi == null || mlvi.Length != 8)
            {
                throw new Exception("MLVI should be 8 bytes.");
            }

            // Incase the mlvi was not pre-initialized, let's create one
            if (mlvi.Count(a => a == 0xFF) >= 7)
            {
                mlvi = new byte[8];
                RandomNumberGenerator.Create().GetBytes(mlvi);
            }

            this.cardType = (CardType)mlvi[0];
            this.mlvi = mlvi;
        }

        public abstract string IdLabel { get; }
        public abstract string CardTypeLabel { get; }
        public abstract string PublicAddress { get; }
        public abstract string PkSource { get; }

        //public abstract PKType PkType { get; }
        public CardType cardType { get; private set; }
    }


    /// Regions for initial bChip release (v0a): 
    public class BChipMemoryLayout_BCHIP : BChipMemoryLayout
    {
        public BChipMemoryLayout_BCHIP(
            byte[] mlvi,
            byte[] cardData,
            PKStatus pkStatus)
            : base(mlvi)
        {
            int len = cardData.Length;
            // Validate len

            this.Salt = cardData.Skip(SALT_ADDR).Take(SALT_MAX_DATA).ToArray();
            this.bchipVIDent = cardData.Skip(VID_DATA_ADDR).Take(VID_MAX_DATA).ToArray();
            this.privateKeyData = cardData.Skip(PK_DATA_ADDR).Take(PRIVATEKEY_MAX_DATA).ToArray();
            this.publicKeyData = cardData.Skip(PUBKEY_DATA_ADDR).Take(PUBKEY_MAX_DATA).ToArray();
            this.crcData = cardData.Skip(CRC_DATA_ADDR).Take(CRC_MAX_SIZE).ToArray();
            this.PkStatus = pkStatus;
        }

        public BChipMemoryLayout_BCHIP(byte[] mlvi) : base(mlvi.ToArray())
        {
            this.Salt = new byte[SALT_MAX_DATA];
            for (int i = 0; i < SALT_MAX_DATA; i++)
            {
                this.Salt[i] = 0x55;
            }
            this.bchipVIDent = new byte[VID_MAX_DATA];
            this.bchipVIDent[0] = (int)PKType.CUSTOM;
            for (int i = 1; i < VID_MAX_DATA; i++)
            {
                this.bchipVIDent[i] = 0xBB;
            }
            this.privateKeyData = new byte[PRIVATEKEY_MAX_DATA];
            for (int i = 0; i < PRIVATEKEY_MAX_DATA; i++)
            {
                this.privateKeyData[i] = 0x00;
            }
            this.publicKeyData = new byte[PUBKEY_MAX_DATA];
            for (int i = 0; i < PUBKEY_MAX_DATA; i++)
            {
                this.publicKeyData[i] = 0xAA;
            }

           this.PkStatus = PKStatus.NotAvailable;
       }

        public PKStatus PkStatus { get; private set; }

        public bool IsFormatted
         {
            get
            {
                bool saltConfig = (Salt.Where(a => a == 0xFF).Count() == Salt.Length);
                bool vidSet = (bchipVIDent.Where(a => a == 0xFF).Count() == bchipVIDent.Length);
                bool pkeydataSet = (privateKeyData.Where(a => a == 0xFF).Count() == privateKeyData.Length);
                return
                    // Spec for original Bchip has card new card data 
                    // set to 0xFF when unconfigured
                    saltConfig &&
                    vidSet &&
                    pkeydataSet
                    ;
            }
        }

        // 8 bytes RNG - 
        public const int SALT_ADDR = 0;
        public const int SALT_MAX_DATA = 8;

        public byte[] Salt { get; private set; }
        // 32 bytes
        // 00-07: Private Key Type Identifier (Private key source)
        // 01   : PK length (32, 64 or 96)
        // 02   : PubKey length (0-64 bytes)
        //        See PKType enum. Bytes 1-7 unused.
        public const int VID_PKTYPE_ADDR = 0;
        public const int VID_PKLEN_ADDR = 1;
        public const int VID_PUKLEN_ADDR = 2;
        // 08-15: Build version identifier
        public const int VID_BUILD_VERSION = 8;
        // 16-23: Friendly name
        public const int VID_FRIENDLYNAME_ADDR = 16;
        public const int VID_FRIENDLYNAME_MAX_DATA = 8;
        public const int VID_DATA_ADDR = SALT_ADDR + SALT_MAX_DATA; // 8+0==8
        public const int VID_MAX_DATA = 24;
        public byte[] bchipVIDent { get; }

        // Can hold a maximum of 64 bytes of public key data
        public const int PUBKEY_DATA_ADDR = VID_DATA_ADDR + VID_MAX_DATA; // 8+32==40
        public const int PUBKEY_MAX_DATA = 64;
        public byte[] publicKeyData { get; private set; }

        public const int PK_DATA_ADDR = PUBKEY_DATA_ADDR + PUBKEY_MAX_DATA; // 40+64==104
        // maximum bytes that can be encrypted on a BChip card
        // Most private keys are either 32 or 64 bytes, such as deterministic wallets,
        // and some keys supporting up to 512bits, for a total of 96 bytes. 
        public const int MAX_USER_PK = 96;
        // The total size of the encrypted key. The key size is "always" 96 bytes, + padding, is 112.
        public const int PRIVATEKEY_MAX_DATA = 112;
        public byte[] privateKeyData { get; private set; }

        // 104+112==216 -> 7 bytes remaining
        public const int CRC_DATA_ADDR = PK_DATA_ADDR + PRIVATEKEY_MAX_DATA;
        public const int CRC_MAX_SIZE = 7;
        public byte[] crcData { get; private set; }

        /// <summary>
        /// Takes in a passphrase, a salt and private key - stores the encrypted bits locally, publicKey is optional.
        /// TODO: This method will be deprecated sooner than later for a more secure alternative.
        /// </summary>
        public async void EncryptPrivateKeyData(
            PKType keyType,
            string passPhrase,
            byte[] privateKey,
            byte[] publicKey)
        {
            if (privateKey.Length > 96)
            {
                throw new Exception("Private Key length was larger than 96 bytes.");
            }

            byte[] dataToEncrypt = new byte[MAX_USER_PK];
            for (int i = 0; i < MAX_USER_PK; ++i)
            {
                if (i < privateKey.Length)
                {
                    dataToEncrypt[i] = privateKey[i];
                }
                else
                {
                    dataToEncrypt[i] = 0xFF;
                }
            }

            bchipVIDent[VID_PKLEN_ADDR] = (byte)privateKey.Length;

            // Quick and dirty
            int maxKeys = 256;

            // Generate keys:
            // This is the initial version and meant for speed. Eventually, we will force the
            // client to go through a large number of potential passfords as a form of POW
            string pass = passPhrase.ToString();
            var potPasswords = Encryptor.GeneratePassword(pass, maxKeys);
            byte[] initialPassword = Encryptor.CalculateSha512(CryptographicBuffer.ConvertStringToBinary(pass, BinaryStringEncoding.Utf8).ToArray()).ToArray();
            int generatedPin = Encryptor.GeneratePinCode(initialPassword, maxKeys);

            // Selected a key, Generate IV 
            byte[] chosenPassword = potPasswords[generatedPin % potPasswords.Count];
            // Nuke the salt *every* time
            byte[] rnd = new byte[SALT_MAX_DATA];
            RandomNumberGenerator.Create().GetBytes(rnd);
            this.Salt = rnd;
            byte[] chosenSalt = Encryptor.GenerateSalt(this.Salt);
            this.privateKeyData = Encryptor.Encrypt(dataToEncrypt.AsBuffer(), chosenPassword.AsBuffer(), chosenSalt.AsBuffer()).ToArray();
            this.PkType = keyType;

            byte[] publicKeyData = new byte[PUBKEY_MAX_DATA];
            int pubKeyLen = 0;
            if (publicKey != null)
            {
                pubKeyLen = publicKey.Length;
                bchipVIDent[VID_PUKLEN_ADDR] = (byte)pubKeyLen;
            }
            else
            {
                bchipVIDent[VID_PUKLEN_ADDR] = 0;
            }
            for (int i = 0; i < publicKeyData.Length; ++i)
            {
                if (i < pubKeyLen)
                {
                    publicKeyData[i] = publicKey[i];
                }
                else
                {
                    publicKeyData[i] = 0xFF;
                }
            }

            this.publicKeyData = publicKeyData;

            this.crcData = GetCardCheckSum();
        }

        public void SetFriendlyName(string friendlyName)
        {
            if (friendlyName.Length > VID_MAX_DATA)
            {
                throw new FormatException($"Friendly name was more than {VID_MAX_DATA} characters.");
            }

            if (String.IsNullOrWhiteSpace(friendlyName))
            {
                friendlyName = String.Empty;
            }

            if (friendlyName.Length < VID_FRIENDLYNAME_MAX_DATA)
            {
                for (int i = friendlyName.Length; i < VID_FRIENDLYNAME_MAX_DATA; ++i)
                {
                    // Q&D
                    friendlyName += " ";
                }
            }

            try
            {
                byte[] friendlyNameBytes = UTF8Encoding.UTF8.GetBytes(friendlyName);
                for (int i = 0; i < VID_FRIENDLYNAME_MAX_DATA; ++i)
                {
                    this.bchipVIDent[VID_FRIENDLYNAME_ADDR + i] = friendlyNameBytes[i];
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw new FormatException("Failed to parse friendly name.");
            }
        }

        public byte[] DecryptPrivateKeyData(string passPhrase)
        {
            int expectedLength = bchipVIDent[VID_PKLEN_ADDR];

            if (expectedLength > 96)
            {
                throw new Exception("Private Key length was larger than 96 bytes.");
            }

            // Quick and dirty
            int maxKeys = 256;

            // Generate keys
            var potPasswords = Encryptor.GeneratePassword(passPhrase, maxKeys);

            byte[] initialPassword = Encryptor.CalculateSha512(CryptographicBuffer.ConvertStringToBinary(passPhrase, BinaryStringEncoding.Utf8).ToArray());

            int generatedPin = Encryptor.GeneratePinCode(initialPassword, maxKeys);

            // Selected a key, Regenerate IV 
            IBuffer chosenPassword = potPasswords[generatedPin % potPasswords.Count].AsBuffer();
            IBuffer chosenSalt = Encryptor.GenerateSalt(this.Salt).AsBuffer();
            IBuffer decryptedData = Encryptor.Decrypt(this.privateKeyData.AsBuffer(), chosenPassword, chosenSalt);

            if (decryptedData == null || decryptedData.Length == 0)
            {
                return null;
            }

            byte[] parsedKeyData = new byte[expectedLength];
            byte[] decryptedBytes = decryptedData.ToArray();
            for (int i = 0; i < parsedKeyData.Length; ++i)
            {
                parsedKeyData[i] = decryptedBytes[i];
            }

            return parsedKeyData;
        }

        public byte[] GetCardCheckSum()
        {
            List<byte> cardBytes = new List<byte>();
            cardBytes.AddRange(this.Salt);
            cardBytes.AddRange(this.bchipVIDent);
            cardBytes.AddRange(this.publicKeyData);
            cardBytes.AddRange(this.privateKeyData);

            byte[] calc = SHA256.Create().ComputeHash(cardBytes.ToArray());
            calc = SHA256.Create().ComputeHash(calc);

            byte[] checksum = new byte[CRC_MAX_SIZE];
            for (int i = 0; i < checksum.Length; ++i)
            {
                checksum[i] = calc[i];
            }

            return checksum;
        }

        public bool IsChecksumValid()
        {
            byte[] expectedCrc = this.crcData;
            byte[] crc = this.GetCardCheckSum();
            if (expectedCrc.Length != crc.Length)
            {
                return false;
            }

            for (int i = 0; i < BChipMemoryLayout_BCHIP.CRC_MAX_SIZE; ++i)
            {
                if (expectedCrc[i] != crc[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        public override string IdLabel
        {
            get
            {
                string friendlyName = string.Empty;
                // If friendly name starts with 0xff, it is not set
                if (this.bchipVIDent[VID_FRIENDLYNAME_ADDR] != 0xFF)
                {
                    byte[] fname = this.bchipVIDent.Skip(VID_FRIENDLYNAME_ADDR).Take(VID_FRIENDLYNAME_MAX_DATA).ToArray();

                    try
                    {
                        friendlyName = UTF8Encoding.UTF8.GetString(this.bchipVIDent, VID_FRIENDLYNAME_ADDR, VID_FRIENDLYNAME_MAX_DATA);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                StringBuilder cardId = new StringBuilder();
                for (int i = 0; i < mlvi.Length; i += 2)
                {
                    cardId.Append($"{mlvi[i]:X}{mlvi[i + 1]:X} ");
                }

                return $"{friendlyName} (ID: {cardId.ToString().Trim()})";
            }
        }

        public PKType PkType
        {
            get
            {
                return (PKType)bchipVIDent[BChipMemoryLayout_BCHIP.VID_PKTYPE_ADDR];
            }
            set
            {
                bchipVIDent[BChipMemoryLayout_BCHIP.VID_PKTYPE_ADDR] = (byte)value;
            }
        }

        public override string CardTypeLabel
        {
            get
            {
                try
                {
                    PKType detectedPkType = (PKType)bchipVIDent[BChipMemoryLayout_BCHIP.VID_PKTYPE_ADDR];
                    return detectedPkType.ToString();
                }
                catch
                {
                    return "UNSUPPORTED";
                }
            }
        }

        public string DecryptedPrivateKeyString(string passphrase)
        {
            byte[] decryptedKey = this.DecryptPrivateKeyData(passphrase);

            if (decryptedKey == null)
            {
                //Nothing to decrypt?
                return "";
            }

            string privateKey;

            Key key = null;
            switch (this.PkType)
            {
                case PKType.BTC:
                    key = new Key(decryptedKey);
                    privateKey = key.GetWif(Network.Main).ToWif();
                    break;
                case PKType.BTC_TestNet:
                    key = new Key(decryptedKey);
                    privateKey = key.GetWif(Network.TestNet).ToWif();
                    break;
                // Generic failsafe, no parsing/validation
                case PKType.UNSET:
                case PKType.ETH:
                case PKType.CUSTOM:
                default:
                    privateKey = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedKey.AsBuffer());
                    break;
                case PKType.Mnemonic:
                    // TODO: Hardcoded to english, can add integrated OS detection or drop down for multilang
                    string[] words = BIP39Helpers.DecodeMnemonicFromEntropy(decryptedKey, Wordlist.English);
                    StringBuilder wordList = new StringBuilder();
                    foreach(string word in words)
                    {
                         wordList.Append($" {word}");
                    }
                    privateKey = wordList.ToString();
                    break;
            }

            return privateKey;
        }

        public override string PublicAddress
        {
            get
            {
                try
                {
                    // older card work around
                    if (bchipVIDent[VID_PUKLEN_ADDR] == 0xFF &&
                        (this.PkType == PKType.BTC || this.PkType == PKType.BTC_TestNet))
                    {
                        bchipVIDent[VID_PUKLEN_ADDR] = 0x21;
                    }

                    if (bchipVIDent[VID_PUKLEN_ADDR] != 0 && bchipVIDent[VID_PUKLEN_ADDR] != 0xFF)
                    {
                        byte[] pubKeyData = this.publicKeyData.Take(bchipVIDent[VID_PUKLEN_ADDR]).ToArray();
                        switch (this.PkType)
                        {
                            case PKType.BTC:
                                return new PubKey(pubKeyData).GetAddress(Network.Main).ToString();
                            case PKType.BTC_TestNet:
                                return new PubKey(pubKeyData).GetAddress(Network.TestNet).ToString();
                            case PKType.Mnemonic:
                            case PKType.UNSET:
                            case PKType.ETH:
                            case PKType.CUSTOM:
                            default:
                                return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, pubKeyData.AsBuffer());
                        }
                    }
                    return "";
                }
                catch
                {
                    return null;
                }
            }
        }

        public override string PkSource
        {
            get
            {
                return PkStatus.ToString();
            }
        }
    }
}