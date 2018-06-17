using PCSC;
using PCSC.Iso7816;
using PCSC.Monitoring;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BChipDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StreamWriter logWriter = null;
        // TODO: Eventually clear the passphrase from memory or 
        //       re-write this to be a tad more secure.
        private System.Threading.Timer passphraseClearTimer = null;

        private static readonly IContextFactory _contextFactory = ContextFactory.Instance;
        private ISCardMonitor _monitor = MonitorFactory.Instance.Create(SCardScope.System);

        private ConcurrentBag<BChipMemoryLayout> LoadedBChips = new ConcurrentBag<BChipMemoryLayout>();

        public MainWindow()
        {
            using (logWriter = File.AppendText(
               $"{DateTime.Now.ToString("yyyy-MM-dd")}-BChip.log"))
            {
                try
                {
                    WriteToLogFile($"App started", "Startup");

                    _monitor.CardInserted += _monitor_CardInserted;
                    //(sender, args) =>
                    //MessageBox.Show($"New state: {args.State}");
                    _monitor.CardRemoved += _monitor_CardRemoved;
                    StartMonitoring();

                    // Check for plugged in cards
                    LoadConnectedCards();

                    WriteToLogFile($"Load cards on disk", "Startup");

                    InitializeComponent();
                    //SetupDataGridView();
                    //HideAllPassphraseUI();
                    //sendBtn.Enabled = false;
                    //ReceiveBtn.Enabled = false;
                    //notConnectedIcon.Visible = true;

                    WriteToLogFile($"Timer started", "Startup");
                    passphraseClearTimer = new System.Threading.Timer(
                     _ =>
                     {
                         string statusMessage = String.Empty;
                         //if (!String.IsNullOrWhiteSpace(privateKey))
                         //{
                         //    if (DateTime.Now.Subtract(pkSetTime) > TimeSpan.FromSeconds(30))
                         //    {
                         //        privateKey = String.Empty;
                         //    }
                         //    else
                         //    {
                         //        statusMessage += $"Pkey cleared in {DateTime.Now.Subtract(pkSetTime).TotalSeconds} seconds. ";
                         //    }
                         //}

                         //if (!String.IsNullOrWhiteSpace(PassPhrase))
                         //{
                         //    if (DateTime.Now.Subtract(passSetTime) > TimeSpan.FromSeconds(30))
                         //    {
                         //        PassPhrase = String.Empty;
                         //    }
                         //    else
                         //    {
                         //        statusMessage += $"Passphrase cleared in {DateTime.Now.Subtract(passSetTime).TotalSeconds} seconds. ";
                         //    }
                         //}

                         if (!String.IsNullOrWhiteSpace(statusMessage))
                         {
                             //statusStrip1.Invoke((Action)delegate () { statusStrip1.Text = statusMessage; });
                         }
                     }
                    , null, 1000, 1000);

                }
                finally
                {
                    
                }
            }
        }

        private void _monitor_CardInserted(object sender, CardStatusEventArgs e)
        {
            if (e.State == SCRState.Present)
            {
                BChipSmartCard insertedCard =
                    new BChipSmartCard
                    {
                        ATR = e.Atr,
                        ReaderName = e.ReaderName,
                        LastConnected = DateTime.Now
                    };

                if (insertedCard.Type() == CardType.BChip)
                {
                    using (var ctx = _contextFactory.Establish(SCardScope.System))
                    {
                        using (var isoReader = new IsoReader(ctx, insertedCard.ReaderName, SCardShareMode.Shared, SCardProtocol.Any))
                        {
                            Response response = isoReader.Transmit(AdpuHelper.RetrieveFullCardData());
                            if (response.HasData)
                            {
                                byte[] data = response.GetData();
                                byte[] mlvi = data.Skip(0x20).Take(BChipMemoryLayout_BCHIP.MLVI_MAX_DATA).ToArray();
                                byte[] carddata = data.Skip(0x28).ToArray();

                                // At this stage, the card has not yet been validated/parsed. Another thread should handle cleanup/de-dupes
                                BChipMemoryLayout_BCHIP bchipCardMemory = new BChipMemoryLayout_BCHIP(mlvi, carddata, true, PKStatus.NotValidated);
                                
                                LoadedBChips.Add(bchipCardMemory);
                            }
                            else
                            {
                                WriteToLogFile("Card added had no data to download", "_monitor_CardInserted");
                            }
                        }
                    }
                }
            }
        }

        private void _monitor_CardRemoved(object sender, CardStatusEventArgs e)
        {
            MessageBox.Show("A card was just removed!");
        }

        private void StartMonitoring()
        {
            if (_monitor.Monitoring)
            {
                _monitor.Cancel();
            }
            
            string[] res = GetReaderNames();
            _monitor.Start(res);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
           

        }

        private void LoadConnectedCards()
        {
            using (var context = _contextFactory.Establish(SCardScope.System))
            {
                var readerNames = GetReaderNames();
                foreach (string readers in readerNames)
                {
                    byte[] atr = null;

                    using (var reader =
                        context.ConnectReader(readers, SCardShareMode.Shared, SCardProtocol.Any))
                    {
                        try
                        {
                            atr = reader.GetAttrib(SCardAttribute.AnswerToResetString);
                        }
                        catch { }
                    }

                    if (atr != null)
                    {
                        using (var isoReader = new IsoReader(context, readers, SCardShareMode.Shared, SCardProtocol.Any))
                        {
                            try
                            {
                                Response response = isoReader.Transmit(AdpuHelper.RetrieveFullCardData());
                                if (response.HasData)
                                {
                                    byte[] data = response.GetData();
                                    byte[] mlvi = data.Skip(0x20).Take(BChipMemoryLayout_BCHIP.MLVI_MAX_DATA).ToArray();
                                    byte[] carddata = data.Skip(0x28).ToArray();

                                    // At this stage, the card has not yet been validated/parsed. Another thread should handle cleanup/de-dupes
                                    BChipMemoryLayout_BCHIP bchipCardMemory = new BChipMemoryLayout_BCHIP(mlvi, carddata, true, PKStatus.NotValidated);

                                    LoadedBChips.Add(bchipCardMemory);
                                }
                                else
                                {
                                    WriteToLogFile("Card added had no data to download", "_monitor_CardInserted");
                                }
                            }
                            catch { };
                        }
                    }
                }
            }
        }

        private static string[] GetReaderNames()
        {
            using (var context = _contextFactory.Establish(SCardScope.System))
            {
                return context.GetReaders();
            }
        }

        public async void WriteToLogFile(string dataToLog, string source = "General")
        {
            Monitor.TryEnter(logWriter);
            {
                try
                {
                    await logWriter.WriteLineAsync($"{DateTime.Now.ToShortTimeString()}({source}):{dataToLog}");
                }
                catch
                { }
                finally
                {
                    Monitor.Exit(logWriter);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteToLogFile($"App closing", "Main");

            if (_monitor != null)
            {
                _monitor.Cancel();
                _monitor.Dispose();
            }
        }
    }
}
