using PCSC;
using PCSC.Iso7816;
using PCSC.Monitoring;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        private static readonly IContextFactory _contextFactory = ContextFactory.Instance;
        private ISCardMonitor _monitor = MonitorFactory.Instance.Create(SCardScope.System);

        // Only support a single bchip, regardless of reader(s)
        private BChipSmartCard LoadedBChips = null;

        public MainWindow()
        {
            using (logWriter = File.AppendText(
               $"{DateTime.Now.ToString("yyyy-MM-dd")}-BChip.log"))
            {
                try
                {
                    WriteToLogFile($"App started", "Startup");

                    StartMonitoring();

                    //WriteToLogFile($"Load cards on disk", "Startup");

                    InitializeComponent();
                    
                    // Check for plugged in cards
                    ScanAndLoadConnectedCards();

                    // Memory walker timer, which we'll ideally want to use to annoy any potential
                    // binaries that are sniffing the users memory. Ideally, this will also mess with hid
                    // messages in the future (via admin)
                    //WriteToLogFile($"Timer started", "Startup");
                    //passphraseClearTimer = new System.Threading.Timer(
                    // _ =>
                    // {
                         
                    // }
                    //, null, 1000, 1000);

                }
                catch (Exception ex)
                {
                    WriteToLogFile($"Exception caught on startup: {ex.Message} - {ex.StackTrace}", "Startup");
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
                try
                {
                    BChipSmartCard insertedCard =
                        new BChipSmartCard
                        {
                            ATR = e.Atr,
                            ReaderName = e.ReaderName,
                            LastConnected = DateTime.Now,
                            IsConnected = true
                        };

                    if (insertedCard.Type() == CardType.BChip)
                    {
                        ScanAndLoadConnectedCards(insertedCard);
                    }
                }
                catch (Exception ex)
                {
                    WriteToLogFile($"Exception caught: {ex.Message} - {ex.StackTrace}", "_monitor_CardInserted");
                }
                
            }
        }

        private void _monitor_CardRemoved(object sender, CardStatusEventArgs e)
        {
            if (LoadedBChips != null)
            {
                if (LoadedBChips.ReaderName == e.ReaderName)
                {
                    ChangePageUi(PageToShow.NoCard, null);
                    // single card only.
                    // bchip.IsConnected = false;
                    LoadedBChips = null;
                }
            }
        }

        public enum PageToShow
        {
            NoCard,
            ShowPassphraseDialog,
            Ready,
            ConfirmFormat,
            // TODO
            NotInitialized,
            Error,
            CrcError,
            Unsupported,
        }
        public void ChangePageUi(PageToShow pageToShow, BChipSmartCard bChipSmartCard)
        {
            Visibility noCardVisibility = Visibility.Collapsed;
            Visibility readyVisibility = Visibility.Collapsed;
            Visibility confirmFormatVisibility = Visibility.Collapsed;
            Visibility passphraseDialogVisibility = Visibility.Collapsed;

            Visibility notInitializedWizardVisibility = Visibility.Collapsed;
            Visibility crcErrorViewGridVisibility = Visibility.Collapsed;
            Visibility unsupportedDialogVisibility = Visibility.Collapsed;


            switch (pageToShow)
            {
                case PageToShow.ConfirmFormat:
                    confirmFormatVisibility = Visibility.Visible;
                    break;
                case PageToShow.Ready:
                    readyVisibility = Visibility.Visible;
                    break;
                case PageToShow.ShowPassphraseDialog:
                    passphraseDialogVisibility = Visibility.Visible;
                    break;
                case PageToShow.NoCard:
                    noCardVisibility = Visibility.Visible;
                    break;
                case PageToShow.Unsupported:
                case PageToShow.Error:
                    unsupportedDialogVisibility = Visibility.Visible;
                    break;
                case PageToShow.NotInitialized:
                    notInitializedWizardVisibility = Visibility.Visible;
                    break;
                case PageToShow.CrcError:
                    crcErrorViewGridVisibility = Visibility.Visible;
                    break;
            }

            InsertCardGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                NonInitializedWizardViewGrid.Visibility = notInitializedWizardVisibility;
            }));

            CrcErrorViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                CrcErrorViewGrid.Visibility = crcErrorViewGridVisibility;
            }));

            InitializedUnknownCardViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                InitializedUnknownCardViewGrid.Visibility = unsupportedDialogVisibility;
            }));

            ShowPassphraseViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowPassphraseViewGrid.Visibility = passphraseDialogVisibility;

                if (pageToShow == PageToShow.ShowPassphraseDialog)
                {
                    // Clear last error if set
                    ErrorMessageLabel.Content = "";
                }
            }));

            InsertCardGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                InsertCardGrid.Visibility = noCardVisibility;
            }));

            ConfirmFormatViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                ConfirmFormatViewGrid.Visibility = confirmFormatVisibility;
            }));

            ReadyCardViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                ReadyCardViewGrid.Visibility = readyVisibility;

                // Populate for card UI
                if (pageToShow == PageToShow.Ready)
                {
                    PubKeyCopyIcon.Visibility = Visibility.Hidden;
                    BChipMemoryLayout_BCHIP bchip = (BChipMemoryLayout_BCHIP)bChipSmartCard.SmartCardData;
                    BChipIdLabel.Content = bchip.IdLabel;
                    string publicAddress = bchip.PublicAddress;
                    if (publicAddress == "")
                    {
                        if (bchip.PkType == PKType.UNSET)
                        {
                            PublicKeyAddressLabel.Content = "Card not provisioned";
                            DisplayPrivateKeyButton.IsEnabled = false;
                        } 
                        else
                        {
                            PublicKeyAddressLabel.Content = "No public key data on card";
                        }
                    }
                    else if (publicAddress == null)
                    {
                        PublicKeyAddressLabel.Content = "Failed to parse public key data";
                    }
                    else
                    {
                        QrCodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                               new QrHandler(publicAddress, 5).GetQrCode().GetHbitmap(),
                               IntPtr.Zero,
                               Int32Rect.Empty,
                               BitmapSizeOptions.FromEmptyOptions());
                        PubKeyCopyIcon.Visibility = Visibility.Visible;
                        PublicKeyAddressLabel.Content = publicAddress;
                    }

                    if (bchip.PkType != PKType.UNSET)
                    {
                        DisplayPrivateKeyButton.IsEnabled = true;
                    }
                }
            }));
        }

        public PageToShow AnalyzeCardData(BChipSmartCard bChipSmartCard)
        {
            if (!bChipSmartCard.IsConnected)
            {
                return PageToShow.NoCard;
            }

            if  (bChipSmartCard.Type() != CardType.BChip || bChipSmartCard.SmartCardData == null)
            {
                return PageToShow.Unsupported;
            }

            // Analyze card
            try
            {
                BChipMemoryLayout_BCHIP cardMemory = (BChipMemoryLayout_BCHIP)bChipSmartCard.SmartCardData;

                if (cardMemory.IsFormatted)
                {
                    return PageToShow.NotInitialized;
                }

                if (!cardMemory.IsChecksumValid())
                {
                    return PageToShow.CrcError;
                }
                
                // All seems good
                return PageToShow.Ready;
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Exception caught when analyzing card: {0} - {1}", "AnalyzeCardData");
            }

            return PageToShow.Error;
        }

        private void StopMonitoring()
        {
            if (_monitor != null && _monitor.Monitoring)
            {
                _monitor.Cancel();
                MonitorFactory.Instance.Release(_monitor);
            }
        }

        private void StartMonitoring()
        {
            string[] res = GetReaderNames();
            if (res.Length > 0)
            {
                _monitor = MonitorFactory.Instance.Create(SCardScope.System);
                _monitor.CardInserted += _monitor_CardInserted;
                _monitor.CardRemoved += _monitor_CardRemoved;
                _monitor.Start(res);
            }
        }
        
        private void ScanAndLoadConnectedCards(BChipSmartCard insertedCard = null)
        {
            using (var context = _contextFactory.Establish(SCardScope.System))
            {
                if (insertedCard == null)
                {
                    List<string> readerNames = GetReaderNames().ToList();
                    foreach (string curReader in readerNames)
                    {
                        try
                        {
                            using (var reader =
                            context.ConnectReader(curReader, SCardShareMode.Shared, SCardProtocol.Any))
                            {
                                insertedCard = new BChipSmartCard
                                {
                                    ATR = reader.GetAttrib(SCardAttribute.AtrString),
                                    ReaderName = curReader,
                                    LastConnected = DateTime.Now,
                                    IsConnected = true
                                };
                            }
                            if (insertedCard.Type() != CardType.BChip)
                            {
                                WriteToLogFile($"Card type was not an expected BChip and skipped ({insertedCard.Type()})", "InitConnectedCards");
                                continue;
                            }
                        }
                        // We try to check for an inserted card, eat errors.
                        catch (Exception ex)
                        {
                            if ((uint)ex.HResult == 0x80131500)
                            {
                                // ignore card removed
                                continue;
                            }
                        }
                    }
                }

                if (insertedCard == null)
                {
                    ChangePageUi(PageToShow.NoCard, null);
                }

                if (insertedCard != null)
                {
                    using (var isoReader = new IsoReader(context, insertedCard.ReaderName, SCardShareMode.Shared, SCardProtocol.Any))
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
                                insertedCard.SmartCardData = new BChipMemoryLayout_BCHIP(mlvi, carddata, true, PKStatus.NotValidated);
                                LoadedBChips = insertedCard;

                                ChangePageUi(AnalyzeCardData(insertedCard), insertedCard);
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

        private static string[] GetReaderNames()
        {
            using (var context = _contextFactory.Establish(SCardScope.System))
            {
                return context.GetReaders();
            }
        }

        public async void WriteToLogFile(string request, Response adpuResponse, string source = "ADPU Response")
        {
            WriteToLogFile($"ADPU response from {request}: {adpuResponse.StatusWord}", source); 
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
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Hook into the message pump
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null)
            {
                source.AddHook(HwndHandler);
                DeviceEventMonitor.RegisterDeviceNotification(source.Handle);
            }
        }

        // Message pump hook
        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == DeviceEventMonitor.WmDevicechange)
            {
                switch ((int)wparam)
                {
                    case DeviceEventMonitor.DbtDeviceRemoveComplete:
                        // Not too many devices leaving on a normal basis, just do a sanity scan each removal
                        CheckIfReaderRemoved();
                        break;
                    case DeviceEventMonitor.DbtDeviceArrival:
                        DeviceEventMonitor.DEV_BROADCAST_HDR hdr;
                        hdr = (DeviceEventMonitor.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lparam, typeof(DeviceEventMonitor.DEV_BROADCAST_HDR));
                        if (hdr.dbch_devicetype == DeviceEventMonitor.DbyDevTypDeviceInterface) 
                        {
                            DeviceEventMonitor.DEV_BROADCAST_DEVICEINTERFACE deviceInterface = (DeviceEventMonitor.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lparam, typeof(DeviceEventMonitor.DEV_BROADCAST_DEVICEINTERFACE));
                            string usbDeviceIdentifier = new string(deviceInterface.dbcc_name);
                            // If we find a "slot", we'll start scanning for cards
                            CheckIfReaderConnected();
                        }
                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }

        private static Object readLock = new object();
        private void CheckIfReaderRemoved()
        {
            if (LoadedBChips != null)
            {
                if (Monitor.TryEnter(readLock))
                {
                    try
                    {
                        string readerName = LoadedBChips.ReaderName;

                        string[] readers = GetReaderNames();
                        foreach (string reader in readers)
                        {
                            if (readerName == reader)
                            {
                                return;
                            }
                        }

                        // Card pulled with reader
                        LoadedBChips = null;
                        ChangePageUi(PageToShow.NoCard, null);
                        StopMonitoring();
                    }
                    finally
                    {
                        Monitor.Exit(readLock);
                    }
                }
            }
        }

        private void CheckIfReaderConnected()
        {
            if (LoadedBChips == null)
            {
                if (Monitor.TryEnter(readLock))
                {
                    try
                    {
                        ScanAndLoadConnectedCards();
                        StartMonitoring();
                    }
                    finally
                    {
                        Monitor.Exit(readLock);
                    }
                }
            }
        }

        private void CopyAddressToClipBoard()
        {
            if (LoadedBChips != null)
            {
                BChipMemoryLayout_BCHIP bchip = (BChipMemoryLayout_BCHIP)LoadedBChips.SmartCardData;
                if (
                    bchip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_PUKLEN_ADDR] != 0 &&
                    bchip.bchipVIDent[BChipMemoryLayout_BCHIP.VID_PUKLEN_ADDR] != 0xFF
                    )
                {
                    Clipboard.SetText(PublicKeyAddressLabel.Content.ToString());
                    MessageBox.Show($"Your public key has been copied to your clipboard!");
                }
            }
        }

        private void PublicKeyAddressLabel_TouchDown(object sender, TouchEventArgs e)
        {
            CopyAddressToClipBoard();
        }

        private void PublicKeyAddressLabel_TouchDown(object sender, MouseButtonEventArgs e)
        {
            CopyAddressToClipBoard();
        }

        private void DisplayPrivateKey_Click(object sender, RoutedEventArgs e)
        {
            ReadyCardViewGrid.Visibility = Visibility.Collapsed;
            ShowPassphraseViewGrid.Visibility = Visibility.Visible;
            ChangePageUi(PageToShow.ShowPassphraseDialog, LoadedBChips);
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPassphraseViewGrid.Visibility = Visibility.Collapsed;

            try
            {
                BChipMemoryLayout_BCHIP bchip = (BChipMemoryLayout_BCHIP)LoadedBChips.SmartCardData;
                string pk = bchip.DecryptedPrivateKeyString(PassphraseEntryBox.Password);

                PrivateKeyAddressLabel.Content = pk;
                QrCodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                   new QrHandler(pk, 5).GetQrCode().GetHbitmap(),
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
                
                ShowPrivateKeyViewGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowPassphraseViewGrid.Visibility = Visibility.Visible;
                ErrorMessageLabel.Content = "Exception hit - passphrase?";
            }
        }

        private void ClearCancelButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPrivateKeyViewGrid.Visibility = Visibility.Collapsed;
            ConfirmFormatViewGrid.Visibility = Visibility.Collapsed;
            if (LoadedBChips != null)
            {
                ChangePageUi(PageToShow.Ready, LoadedBChips);
            }
        }

        private void CopyPrivateAddressToClipBoard()
        {
            Clipboard.SetText(PrivateKeyAddressLabel.Content.ToString());
            MessageBox.Show($"Your private key has been copied to your clipboard - make sure you clear it as soon as possible.");
        }

        private void PrivateKeyAddressLabel_TouchDown(object sender, MouseButtonEventArgs e)
        {
            CopyPrivateAddressToClipBoard();
        }

        private void PrivateKeyAddressLabel_TouchDown(object sender, TouchEventArgs e)
        {
            CopyPrivateAddressToClipBoard();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (FormatConfirmTextBox.Text.Trim().ToLowerInvariant().Contains("yes"))
            {
                try
                {
                    var res = FormatingViewGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        FormatingViewGrid.Visibility = Visibility.Visible;
                    }));

                    using (var context = _contextFactory.Establish(SCardScope.System))
                    {
                        using (var isoReader = new IsoReader(context, LoadedBChips.ReaderName, SCardShareMode.Shared, SCardProtocol.Any))
                        {
                            var unlockResponse = isoReader.Transmit(AdpuHelper.SendPin());
                            WriteToLogFile("CardUnblock", unlockResponse);
                            var writeResponse = isoReader.Transmit(AdpuHelper.FormatCard(CardType.BChip));
                            WriteToLogFile("FormatCard", writeResponse);
                        }
                    }
                    LoadedBChips = null;
                    ScanAndLoadConnectedCards();
                }
                catch (Exception ex)
                { }
                finally
                {
                    var res = FormatingViewGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        FormatingViewGrid.Visibility = Visibility.Collapsed;
                    }));
                }
            }

            ClearCancelButton_Click(sender, e);
        }

        private void FormatCard_Click(object sender, RoutedEventArgs e)
        {
            ChangePageUi(PageToShow.ConfirmFormat, LoadedBChips);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           var selection = e.AddedItems;
        }
    }
}
