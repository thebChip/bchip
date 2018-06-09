using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.SmartCards;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace BChipDesktop
{
    public static class AdpuHandler
    {
        public static PCSCCodes ParseADPUState(IBuffer buffer)
        {
            PCSCCodes returnCode = PCSCCodes.Unknown;
            List<byte> returnBuffer = new List<byte>();
            
            // odd return code
            if (buffer.Length < 2)
            {
                return PCSCCodes.BadFormat;
            }
            
            byte[] buf = buffer.ToArray();
            byte[] code = new byte[] { buf[buf.Length - 2], buf[buf.Length - 1] };
            string parsedBuf = CryptographicBuffer.EncodeToHexString(code.AsBuffer());
            Enum.TryParse($"{parsedBuf}", out returnCode);

            return returnCode;
        }

        public static async Task<IBuffer> SendADPUCommand(SmartCard card, AdpuCommand adpu, byte[] dataToWrite)
        {
            List<byte> commandsToSend = new List<byte>();

            int length = 0;
            if (dataToWrite != null)
            {
                length = dataToWrite.Length;
            }

            if (length > 256 - 0x20) // reserve 32 bytes for currently unused protected area
            {
                throw new InvalidDataException("Too much data to write to the card.");
            }

            // This should be a "build an ADPU" instead of static entries or loaded by XML file
            switch (adpu)
            {
                case AdpuCommand.RetrieveFullCardData:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xB0, 0x00, 0x00, 0xFF });
                    break;
                // For test purposes, this is only 80 bytes.
                case AdpuCommand.RetrieveEncryptedData:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xB0, 0x00, 0x20, 0x50 });
                    break;
                case AdpuCommand.RetrieveBody:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xB0, 0x00, 0x20, 0xFF-0x20+0x01 });
                    break;
                case AdpuCommand.SendPinCode:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0x20, 0x00, 0x81 });
                    if (length == 0)
                    {
                        throw new InvalidDataException("No PIN Supplied.");
                    }
                    {
                        // Data length
                        commandsToSend.Add((byte)length);
                        // PIN code
                        commandsToSend.AddRange(dataToWrite);
                    }
                    break;
                case AdpuCommand.WriteMlviData:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xD6, 0x00 });
                    if (length != 8)
                    {
                        throw new InvalidDataException("MLVI Data should be exactly 8 bytes.");
                    }

                    // Writing start address:
                    commandsToSend.Add(0x20);
                    // Data length
                    commandsToSend.Add((byte)length);
                    // bytes to write
                    commandsToSend.AddRange(dataToWrite);
                    break;
                case AdpuCommand.WriteCardData:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xD6, 0x00 });
                    if (length == 0)
                    {
                        throw new InvalidDataException("Need actual data to write to the card.");
                    }

                    // Writing start address:
                    commandsToSend.Add(0x28);
                    // Data length
                    commandsToSend.Add((byte)length);
                    // bytes to write
                    commandsToSend.AddRange(dataToWrite);
                    break;
                case AdpuCommand.WriteDataToFullCard:
                    commandsToSend.AddRange(new byte[] { 0xFF, 0xD6, 0x00 });
                    if (length == 0)
                    {
                        throw new InvalidDataException("Need actual data to write to the card.");
                    }

                    // Writing start address:
                    commandsToSend.Add(0x20);
                    // Data length
                    commandsToSend.Add((byte)length);
                    // bytes to write
                    commandsToSend.AddRange(dataToWrite);
                    break;
            }

            return await SendADPUCommand(card, commandsToSend.ToArray());
        }

        private static async Task<IBuffer> SendADPUCommand(SmartCard card, byte[] adpu)
        {
            IBuffer retData = null;
            
            using (SmartCardConnection connection = await card.ConnectAsync())
            {
                byte[] readEfAtrBytes = adpu;

                IBuffer readEfAtr = CryptographicBuffer.CreateFromByteArray(readEfAtrBytes);
                retData = await connection.TransmitAsync(readEfAtr);
            }

            return retData;
        }
    }
}
