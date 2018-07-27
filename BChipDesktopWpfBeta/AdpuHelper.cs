using PCSC.Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BChipDesktop
{
    public abstract class AdpuInterface
    {
        public abstract bool RequiresInit { get; }

        public virtual CommandApdu InitCard()
        {
            if (RequiresInit)
            {
                throw new NotImplementedException("Card reader requires initialization before usage");
            }

            return null;
        }
        public abstract CommandApdu UnblockCard();
        public abstract CommandApdu FormatCard(CardType cardType, byte[] mlvi = null);
        public abstract CommandApdu WriteCard(BChipMemoryLayout_BCHIP bChip);
        public abstract CommandApdu ReadCard();
    }

    public class AcrAdpuInterface : AdpuInterface
    {
        public override bool RequiresInit
        {
            get
            {
                return true;
            }
        }

        public override CommandApdu InitCard()
        {
            return new CommandApdu(IsoCase.Case3Short, PCSC.SCardProtocol.Any)
                {
                    CLA = 0xFF,
                    INS = 0xA4,
                    P1 = 0,
                    P2 = 0,
                    Data = new byte[] { 0x06 },
                };
        }

        public override CommandApdu UnblockCard()
        {
            throw new NotImplementedException();

            byte[] cardPin = new byte[] { 0xFF, 0xFF, 0xFF };

            CommandApdu commandApdu = new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.Verify,
                P1 = 0x00, // Parameter 1
                P2 = 0x81, // Parameter 2
                Data = cardPin
            };

            return commandApdu;
        }

        public override CommandApdu FormatCard(CardType cardType, byte[] mlvi = null)
        {
            throw new NotImplementedException();

            if (mlvi == null)
            {
                mlvi = new byte[8];
                RandomNumberGenerator.Create().GetBytes(mlvi);
                mlvi[0] = (byte)cardType;
            }

            if (mlvi.Length != 8)
            {
                throw new FormatException("mlvi must should be exactly 8 bytes of data.");
            }

            List<byte> dataToWrite = new List<byte>();
            dataToWrite.AddRange(mlvi);

            int cardMemorySize = 0;
            switch (cardType)
            {
                case CardType.BChip:
                    cardMemorySize = 224;
                    break;
            }

            // pad the list
            int padding = cardMemorySize - mlvi.Length;
            for (int i = 0; i < padding; i++)
            {
                dataToWrite.Add(0xFF);
            }

            if (dataToWrite.Count != cardMemorySize)
            {
                throw new Exception("Data to write size was unexpected.");
            }

            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };
        }

        public override CommandApdu WriteCard(BChipMemoryLayout_BCHIP bChip)
        {
            throw new NotImplementedException();

            List<byte> dataToWrite = new List<byte>();

            dataToWrite.AddRange(bChip.mlvi);

            dataToWrite.AddRange(bChip.Salt);
            // Always replace the build version bits for backcompat safety 
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 0] = (byte)version.MajorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 1] = (byte)version.MinorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 2] = (byte)version.Build;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 3] = (byte)version.Revision;

            dataToWrite.AddRange(bChip.bchipVIDent);
            dataToWrite.AddRange(bChip.publicKeyData);
            dataToWrite.AddRange(bChip.privateKeyData);
            dataToWrite.AddRange(bChip.GetCardCheckSum());

            if (dataToWrite.Count != 223)
            {
                throw new Exception("BChip data to write exceeded card limit.");
            }

            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };
        }

        public override CommandApdu ReadCard()
        {
            return new CommandApdu(
                          IsoCase.Case2Short,
                          PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, 
                INS = 0xB0,
                P1 = 0x00, // Parameter 1
                P2 = 0x00, // Parameter 2
                Le = 0xFF // Read bytes 0 to 0xFF, protected bits are ignored
            };
        }
    }

    public class OmnikeyAdpuInterface : AdpuInterface
    {
        public override bool RequiresInit
        {
            get
            {
                return false;
            }
        }

        public override CommandApdu UnblockCard()
        {
            byte[] cardPin = new byte[] { 0xFF, 0xFF, 0xFF };

            CommandApdu commandApdu = new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.Verify,
                P1 = 0x00, // Parameter 1
                P2 = 0x81, // Parameter 2
                Data = cardPin
            };

            return commandApdu;
        }

        public override CommandApdu FormatCard(CardType cardType, byte[] mlvi = null)
        {
            if (mlvi == null)
            {
                mlvi = new byte[8];
                RandomNumberGenerator.Create().GetBytes(mlvi);
                mlvi[0] = (byte)cardType;
            }

            if (mlvi.Length != 8)
            {
                throw new FormatException("mlvi must should be exactly 8 bytes of data.");
            }

            List<byte> dataToWrite = new List<byte>();
            dataToWrite.AddRange(mlvi);

            int cardMemorySize = 0;
            switch (cardType)
            {
                case CardType.BChip:
                    cardMemorySize = 224;
                    break;
            }

            // pad the list
            int padding = cardMemorySize - mlvi.Length;
            for (int i = 0; i < padding; i++)
            {
                dataToWrite.Add(0xFF);
            }

            if (dataToWrite.Count != cardMemorySize)
            {
                throw new Exception("Data to write size was unexpected.");
            }

            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };
        }

        public override CommandApdu WriteCard(BChipMemoryLayout_BCHIP bChip)
        {
            List<byte> dataToWrite = new List<byte>();

            dataToWrite.AddRange(bChip.mlvi);

            dataToWrite.AddRange(bChip.Salt);
            // Always replace the build version bits for backcompat safety 
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 0] = (byte)version.MajorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 1] = (byte)version.MinorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 2] = (byte)version.Build;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 3] = (byte)version.Revision;

            dataToWrite.AddRange(bChip.bchipVIDent);
            dataToWrite.AddRange(bChip.publicKeyData);
            dataToWrite.AddRange(bChip.privateKeyData);
            dataToWrite.AddRange(bChip.GetCardCheckSum());

            if (dataToWrite.Count != 223)
            {
                throw new Exception("BChip data to write exceeded card limit.");
            }

            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };
        }

        public override CommandApdu ReadCard()
        {
            return new CommandApdu(
                          IsoCase.Case2Short,
                          PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                            // Instruction =  InstructionCode.ReadBinary,
                INS = 0xB0,
                P1 = 0x00, // Parameter 1
                P2 = 0x00, // Parameter 2
                Le = 0xFF // Read bytes 0 to 0xFF, protected bits are ignored
            };
        }
    }

    public static class OldAdpuHelper
    {
        public static CommandApdu SendPin()
        {
            byte[] cardPin = new byte[]{ 0xFF, 0xFF, 0xFF };
        
            CommandApdu commandApdu = new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.Verify,
                P1 = 0x00, // Parameter 1
                P2 = 0x81, // Parameter 2
                Data = cardPin
            };

            return commandApdu;
        }

        public static CommandApdu SendPinAcr()
        {
            byte[] cardPin = new byte[] { 0xFF, 0xFF, 0xFF };

            CommandApdu commandApdu = new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.Verify,
                P1 = 0x00, // Parameter 1
                P2 = 0x81, // Parameter 2
                Data = cardPin
            };

            return commandApdu;
        }

        public static CommandApdu FormatCard(CardType cardType, byte[] mlvi = null)
        {
            if (mlvi == null)
            {
                mlvi = new byte[8];
                RandomNumberGenerator.Create().GetBytes(mlvi);
                mlvi[0] = (byte)cardType;
            }

            if (mlvi.Length != 8)
            {
                throw new FormatException("mlvi must should be exactly 8 bytes of data.");
            }

            List<byte> dataToWrite = new List<byte>();
            dataToWrite.AddRange(mlvi);

            int cardMemorySize = 0;
            switch(cardType)
            {
                case CardType.BChip:
                    cardMemorySize = 224;
                    break;
            }

            // pad the list
            int padding = cardMemorySize - mlvi.Length;
            for (int i = 0; i < padding; i++)
            {
                dataToWrite.Add(0xFF);
            }
            
            if (dataToWrite.Count != cardMemorySize)
            {
                throw new Exception("Data to write size was unexpected.");
            }

            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };

        }

        public static CommandApdu WriteCardData(
            BChipMemoryLayout_BCHIP bChip)
        {
            List<byte> dataToWrite = new List<byte>();

            dataToWrite.AddRange(bChip.mlvi);

            dataToWrite.AddRange(bChip.Salt);
            // Always replace the build version bits for backcompat safety 
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 0] = (byte)version.MajorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 1] = (byte)version.MinorRevision;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 2] = (byte)version.Build;
            bChip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_BUILD_VERSION + 3] = (byte)version.Revision;
            
            dataToWrite.AddRange(bChip.bchipVIDent);
            dataToWrite.AddRange(bChip.publicKeyData);
            dataToWrite.AddRange(bChip.privateKeyData);
            dataToWrite.AddRange(bChip.GetCardCheckSum());

            if (dataToWrite.Count != 223)
            {
                throw new Exception("BChip data to write exceeded card limit.");
            }
            
            return new CommandApdu(
                           IsoCase.Case3Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
                Instruction = InstructionCode.UpdateBinary,
                P1 = 0x00, // Parameter 1
                P2 = 0x20, // Start address - 32 bytes
                Data = dataToWrite.ToArray()
            };

        }

        public static CommandApdu AcrInit()
        {
            return new CommandApdu(IsoCase.Case3Short, PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF,
                INS = 0xA4,
                P1 = 0,
                P2 = 0,
                Data = new byte[] { 0x06 },
            };
        }

        public static CommandApdu RetrieveFullCardData()
        {
            return new CommandApdu(
                           IsoCase.Case2Short,
                           PCSC.SCardProtocol.Any)
            {
                CLA = 0xFF, // Class
               // Instruction =  InstructionCode.ReadBinary,
               INS = 0xB0,
                P1 = 0x00, // Parameter 1
                P2 = 0x00, // Parameter 2
                Le = 0xFF // Read bytes 0 to 0xFF, protected bits are ignored
            };
        }
    }
}
