/*********************************************************
Copyright(c) 2018 bChip LLC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*********************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;

namespace BChipDesktop
{
    public class Configuration
    {
        public StreamWriter LogFileStream = null;
        public const string FEATURE_NAME = "bChip Controller";

        // Default PIN code to write to a bchip
        // NOTE: This is not a security boundary and essentially is  
        //       used like a reverse read-only memory schema.
        public byte[] BChipPinCode { get; private set; }

        public const int ADMIN_KEY_LENGTH_IN_BYTES = 24;

        public String SmartCardReaderDeviceId { get; set; }
        public List<BChipSmartCard> DetectedSmartCards { get; set; }
        
        public Configuration(byte[] smartCardPin = null)
        {
            if (smartCardPin == null)
            {
                BChipPinCode = new byte[] { 0xFF, 0xFF, 0xFF };
            }
            else
            {
                BChipPinCode = smartCardPin;
            }
        }

        public async void WriteToLogFile(string dataToLog, string source = "Confguration")
        {
            if (LogFileStream != null)
            {
                Monitor.Enter(LogFileStream);
                {
                    try
                    {
                        await LogFileStream.WriteLineAsync($"{DateTime.Now.ToShortTimeString()}({source}):{dataToLog}");
                        LogFileStream.Flush();
                    }
                    catch
                    { }
                    finally
                    {
                        Monitor.Exit(LogFileStream);
                    }
                }
            }
        }

        public async Task<int> ScanForbChipCards()
        {
            if (DetectedSmartCards == null)
            {
                DetectedSmartCards = new List<BChipSmartCard>();
            }
            
            if (Monitor.TryEnter(DetectedSmartCards))
            {
                try
                {
                    DetectedSmartCards.Clear();
                    string selector = SmartCardReader.GetDeviceSelector();
                    
                    DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

                    // DeviceInformation.FindAllAsync gives us a
                    // DeviceInformationCollection, which is essentially a list
                    // of DeviceInformation objects.  We must iterate through that
                    // list and instantiate SmartCardReader objects from the
                    // DeviceInformation objects.
                    foreach (DeviceInformation device in devices)
                    {
                        SmartCardReader reader = await SmartCardReader.FromIdAsync(device.Id);
                        
                        // For each reader, we want to find all the cards associated
                        // with it.  Then we will create a SmartCardListItem for
                        // each (reader, card) pair.
                        IReadOnlyList<SmartCard> cards = await reader.FindAllCardsAsync();

                        foreach (SmartCard card in cards)
                        {
                            try
                            {
                                var buf = await card.GetAnswerToResetAsync();
                                byte[] atrResp = buf.ToArray();

                                SmartCardProvisioning provisioning = await SmartCardProvisioning.FromSmartCardAsync(card);

                                var deviceKind = card.Reader.Kind;
                                BChipSmartCard bchipcard = new BChipSmartCard
                                {
                                    ReaderName = card.Reader.Name,
                                    ATR = atrResp,
                                    CardName = await provisioning.GetNameAsync(),
                                    ReaderDeviceId = card.Reader.DeviceId
                                };

                                DetectedSmartCards.Add(bchipcard);
                            }
                            catch (Exception ex)
                            {
                                WriteToLogFile($"Exception caught while scanning card from Reader:{reader.DeviceId} (ID: {reader.Kind}). Exception: {ex.Message} - {ex.StackTrace}", "ScanForbChipCards");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Enumerating smart cards failed with unhandled exception: " + ex.ToString());
                }
                finally
                { 
                    Monitor.Exit(DetectedSmartCards);
                }

                return DetectedSmartCards.Count;
            }

            return -1;
        }

        public async Task<SmartCard> GetSmartCard(BChipSmartCard bchipCard)
        {
            SmartCardReader reader = await SmartCardReader.FromIdAsync(bchipCard.ReaderDeviceId);
            IReadOnlyList<SmartCard> cards = await reader.FindAllCardsAsync();

            if (1 != cards.Count)
            {
                throw new InvalidOperationException("Reader has an unexpected number of cards (" + cards.Count + ") reporting back.");
            }

            return cards[0];
        }

        public BChipSmartCard GetConnectedBChip()
        {
            foreach (BChipSmartCard potentialBChip in DetectedSmartCards)
            {
                switch (GetCardType(potentialBChip))
                {
                    case CardType.BChip:
                    case CardType.BChipS:
                        return potentialBChip;
                    default:
                        continue;
                }
            }

            return null;
        }
        
        public CardType GetCardType(BChipSmartCard detectedCard)
        {
            // Basic v1 - FM4442
            // 3b 04 A2 13 10 91
            if (detectedCard.ATR.Length == 6)
            {
                if (detectedCard.ATR[0] == 0x3b &&
                    detectedCard.ATR[1] == 0x04 &&
                    detectedCard.ATR[2] == 0xA2 &&
                    detectedCard.ATR[3] == 0x13 &&
                    detectedCard.ATR[4] == 0x10 &&
                    detectedCard.ATR[5] == 0x91)
                {
                    return CardType.BChip;
                }
            }

            return CardType.Unknown;
        }

        public bool ValidateCardReaderConnected()
        {
            if (DetectedSmartCards == null)
            {
                DetectedSmartCards = new List<BChipSmartCard>();
                return false;
            }
            
            return true;
        }

      
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
