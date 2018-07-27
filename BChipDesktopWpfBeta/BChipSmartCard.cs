using PCSC.Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChipDesktop
{
    public enum CardType : uint
    {
        Unknown = 0x0,
        // Physical bChips where data was copied to the system from
        // a physical bchip
        BChip = 0xA1,
        BChipS = 0xA5,

        // Hot cards were created on the system and not guarenteed to be 
        // backed up from a physical bchip
        HotCard = 0xFF
    }

    public enum CardMemory : uint
    {
        BChip = 224,
    }

    public enum PKStatus
    {
        NotAvailable,
        NotValidated,
        Encrypted,
        Imported,
        Generated,
    }

    public enum PKType : uint
    {
        // Officially supported coins
        BTC         = 0x20,
        BTC_TestNet = 0x21,
        ETH         = 0x30,
        // To be supported soon! 
        Mnemonic    = 0x50,

        // Custom entrees are manually created by users either for obscurity 
        // or unsupported key storage.
        CUSTOM = 0xFE,
        UNSET = 0xFF
    }

    public enum AdpuCommand
    {
        RetrieveFullCardData,
        RetrieveEncryptedData,
        RetrieveBody,
        ReadProtectedMemory,
        SendPinCode,
        // Headers only (first 8 bytes), 0x20
        WriteMlviData,
        // Write full card, 0x28->0xFF
        WriteCardData,
        // Write to all available space, 0x20->0xFF
        WriteDataToFullCard
    }

    public class BChipSmartCard
    {
        public string ReaderDeviceId { get; set; }
        public string ReaderName { get; private set; }
        public string CardName { get; set; }
        public byte[] ATR { get; set; }
        public DateTime LastConnected { get; set; }
        public bool IsConnected { get; set; }
        public BChipMemoryLayout SmartCardData { get; set; }
        public AdpuInterface AdpuInterface { get; private set; }

        public BChipSmartCard(string readerName)
        {
            this.ReaderName = readerName;
            
            switch (SmartCardInterface.AutoDetectReader(readerName))
            {
                case SmartCardInterface.ReaderType.Acr39:
                    this.AdpuInterface = new AcrAdpuInterface();
                    break;
                case SmartCardInterface.ReaderType.HidOmninkey:
                    this.AdpuInterface = new OmnikeyAdpuInterface();
                    break;
            }
        }


        public CardType Type()
        {
            // Basic v1 - FM4442
            // 3b 04 A2 13 10 91
            if (ATR.Length == 6)
            {
                if (ATR[0] == 0x3b &&
                    ATR[1] == 0x04 &&
                    ATR[2] == 0xA2 &&
                    ATR[3] == 0x13 &&
                    ATR[4] == 0x10 &&
                    ATR[5] == 0x91)
                {
                    return CardType.BChip;
                }
            }

            return CardType.Unknown;
        }
    }
    
    // TODO: We eventually want to include all potential codes
    // A good source is http://www.smartjac.biz/index.php/support/main-menu?view=kb&kbartid=3 
    public enum PCSCCodes : uint
    {
        Success = 0x9000,
        BadFormat = 0xFFFE,
        Unknown = 0xFFFF,
    }
}
