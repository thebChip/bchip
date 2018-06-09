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
        public string ReaderName { get; set; }
        public string CardName { get; set; }
        public byte[] ATR { get; set; }
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
