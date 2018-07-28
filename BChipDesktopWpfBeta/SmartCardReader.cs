using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChipDesktop
{
    public class SmartCardInterface
    {
        public enum ReaderType
        {
            HidOmninkey, // Default assumed reader
            Acr39
        }

        public static ReaderType AutoDetectReader(string readerName)
        {
            ReaderType parsedReader;
            if (readerName.Contains("ACR39U") || readerName.Contains("ACS")) // Generic ACS override
            {
                return ReaderType.Acr39;
            }
            
            return ReaderType.HidOmninkey;
        }
    }
}
