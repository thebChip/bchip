using NBitcoin;
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
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.Security.Cryptography;

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
            NotInitialized,
            Error,
            CrcError,
            Unsupported,
            ProvisionCard,
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
            Visibility provisionNewKeysViewGridVisibility = Visibility.Collapsed;

            ClearPasswordBox(PassphraseEntryBox);
            ClearPasswordBox(passphrase);
            ClearPasswordBox(passphraseConfirmation);

            switch (pageToShow)
            {
                case PageToShow.ProvisionCard:
                    provisionNewKeysViewGridVisibility = Visibility.Visible;
                    break;
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

            ProvisionNewKeysViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProvisionNewKeysViewGrid.Visibility = provisionNewKeysViewGridVisibility;
            }));

            NonInitializedWizardViewGrid.Dispatcher.BeginInvoke(new Action(() =>
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
                    QrCodeImage.Visibility = Visibility.Hidden;
                    PubKeyCopyIcon.Visibility = Visibility.Hidden;
                    BChipMemoryLayout_BCHIP bchip = (BChipMemoryLayout_BCHIP)bChipSmartCard.SmartCardData;
                    BChipIdLabel.Content = bchip.IdLabel;
                    string publicAddress = bchip.PublicAddress;
                    if (publicAddress == "")
                    {
                        if (bchip.PkType == PKType.UNSET)
                        {
                            PublicKeyAddressLabel.Content = "Card not setup - remove and re-insert card to reload";
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
                        QrCodeImage.Visibility = Visibility.Visible;
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
                    WalletTypeComboBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        WalletTypeComboBox.SelectedItem = null;
                    }));
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
                WriteToLogFile($"Exception caught when analyzing card: {ex.Message} - {ex.StackTrace}", "AnalyzeCardData");
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
                        catch (Exception ex)
                        {
                            WriteToLogFile($"Exception while loading card in {insertedCard.ReaderName} - {ex.Message} - {ex.StackTrace}", "ScanAndLoadConnectedCards");
                        };
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
            DispatcherUpdater(ShowPassphraseViewGrid, Visibility.Collapsed);

            try
            {
                BChipMemoryLayout_BCHIP bchip = (BChipMemoryLayout_BCHIP)LoadedBChips.SmartCardData;
                string pk = bchip.DecryptedPrivateKeyString(PassphraseEntryBox.Password);

                PrivateKeyAddressLabel.Content = pk;
                PrivateQrCodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                   new QrHandler(pk, 5).GetQrCode().GetHbitmap(),
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
                
                ShowPrivateKeyViewGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Exception hit decrypting card data: {ex.Message} - {ex.StackTrace}");
                ShowPassphraseViewGrid.Visibility = Visibility.Visible;
                ErrorMessageLabel.Content = "Failed to decrypt private key. Was the passphrase correct?";
            }
        }

        private void ClearCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (PrivateQrCodeImage.Source != null)
            {
                PrivateQrCodeImage.Source = null;
            }
            ShowPrivateKeyViewGrid.Visibility = Visibility.Collapsed;
            ConfirmFormatViewGrid.Visibility = Visibility.Collapsed;
            ChangePageUi(AnalyzeCardData(LoadedBChips), LoadedBChips);
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

        #pragma warning disable 1998 // We want this to be async
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (FormatConfirmTextBox.Text.Trim().ToLowerInvariant().Contains("yes"))
            {
                ClearTextBox(FormatConfirmTextBox);
                DispatcherUpdater(ConfirmFormatViewGrid, Visibility.Collapsed);
                await Task.Run(() => FormatCardAsync());
            }
        }

        private async void FormatCardAsync()
        {
            try
            {
                DispatcherUpdater(FormatingViewGrid, Visibility.Visible);

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
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Unhandled exception formatting card: {ex.Message} - {ex.StackTrace}");
            }
            finally
            {
                DispatcherUpdater(FormatingViewGrid, Visibility.Collapsed);
                ScanAndLoadConnectedCards();
            }
        }

        private void FormatCard_Click(object sender, RoutedEventArgs e)
        {
            ChangePageUi(PageToShow.ConfirmFormat, LoadedBChips);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProvisionCardButton_Click(object sender, RoutedEventArgs e)
        {
            CreateKeyErrorLabel.Content = "";
            CreatingKeyLabel.Content = "";

            if (passphrase.Password.Length == 0 || passphraseConfirmation.Password.Length == 0)
            {
                CreateKeyErrorLabel.Content = "A passphrase was not entered.";
                return;
            }

            if (passphrase.Password != passphraseConfirmation.Password)
            {
                CreateKeyErrorLabel.Content = "Passphrases entered did not match.";
                return;
            }

            if (PrivateKeyTextBox.Text.Length == 0)
            {
                CreateKeyErrorLabel.Content = "No private key data to store.";
                return;
            }

            if (CardPkType == PKType.BTC || CardPkType == PKType.BTC_TestNet)
            {
                // Check if we are importing a user's wif or mnemonic, update public key field 
                // to allow the user to confirm
                if (CardGeneratedKey.GetBitcoinSecret(NBitcoin.Network.Main).ToWif() != PrivateKeyTextBox.Text)
                {
                    try
                    {
                        NBitcoin.Key importedKey = null;
                        int spacesDetected = PrivateKeyTextBox.Text.Count(a => a == ' ');
                        // Check for mnemonic
                        if (spacesDetected >= 11)
                        {
                            NBitcoin.Mnemonic mnemonic = new NBitcoin.Mnemonic(PrivateKeyTextBox.Text);
                            // Force bitcoin and first address
                            importedKey = mnemonic.DeriveExtKey().Derive(KeyPath.Parse("m/44'/0'/0'/0/0")).PrivateKey;
                        }
                        else
                        {
                            // Check for wif
                            importedKey = NBitcoin.Key.Parse(PrivateKeyTextBox.Text);
                        }

                        // Replace CardGeneratedKey with imported key. Only valid for bitcoin addresses.
                        CardGeneratedKey = importedKey;
                        PublicKeyTextBox.Text = importedKey.PubKey.GetAddress(Network).ToString();
                        PrivateKeyTextBox.Text = importedKey.GetBitcoinSecret(Network).ToWif();
                        CreatingKeyLabel.Content = "Key data imported, ready to setup your bChip.";
                        return;
                    }
                    catch (Exception ex)
                    {
                        WriteToLogFile($"Exception hit: {ex.Message} - {ex.StackTrace}", "ProvisionCardButton_Click");
                        CreateKeyErrorLabel.Content = "Failed to automatically parse private key data.";
                        return;
                    }
                }
            }

            byte[] privateKeyToEncrypt = null;
            byte[] publicKeyData = null;

            if (CardGeneratedKey != null)
            {
                // Users can optionally remove the public key portion, so we'll skip saving it.
                if (PublicKeyTextBox.Text.Length > 0)
                {
                    publicKeyData = CardGeneratedKey.PubKey.ToBytes();
                }
                byte[] privateKeyBytes = CardGeneratedKey.GetWif(Network.Main).ToBytes();
                // Always seem to get an extra bit at the end...
                if (privateKeyBytes.Length == 33 && privateKeyBytes[32] == 0x1)
                {
                    privateKeyToEncrypt = privateKeyBytes.Take(32).ToArray();
                }
                else
                {
                    privateKeyToEncrypt = privateKeyBytes;
                }
            }
            else
            {
                if (PublicKeyTextBox.Text.Length > 0)
                {
                    publicKeyData = CryptographicBuffer.ConvertStringToBinary(PublicKeyTextBox.Text, BinaryStringEncoding.Utf8).ToArray();
                }

                privateKeyToEncrypt = CryptographicBuffer.ConvertStringToBinary(PrivateKeyTextBox.Text, BinaryStringEncoding.Utf8).ToArray();
            }

            if (privateKeyToEncrypt.Length > BChipMemoryLayout_BCHIP.PRIVATEKEY_MAX_DATA)
            {
                CreateKeyErrorLabel.Content =
                    $"Private key was {PrivateKeyTextBox.Text.Length} bytes, {Math.Abs(PrivateKeyTextBox.Text.Length - BChipMemoryLayout_BCHIP.PRIVATEKEY_MAX_DATA)} bytes over the limit.";
                return;
            }

            if (publicKeyData != null)
            {
                if (publicKeyData.Length > BChipMemoryLayout_BCHIP.PUBKEY_MAX_DATA)
                {
                    CreateKeyErrorLabel.Content =
                        $"Public key was {PublicKeyTextBox.Text.Length} bytes, {Math.Abs(PublicKeyTextBox.Text.Length - BChipMemoryLayout_BCHIP.PUBKEY_MAX_DATA)} bytes over the limit.";
                    return;
                }
            }

            ProvisionNewKeysViewGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                ProvisionNewKeysViewGrid.IsEnabled = false;
            }));

            Task.Run(new Action(() =>
                {
                    try
                    {
                        UpdateTextLabel(CreatingKeyLabel, "Setting up bChip! Do not remove card.");

                        // Encrypt data
                        BChipMemoryLayout_BCHIP bchip = LoadedBChips.SmartCardData as BChipMemoryLayout_BCHIP;
                        bchip.EncryptPrivateKeyData(CardPkType, passphrase.Password, privateKeyToEncrypt, publicKeyData);

                        using (var context = _contextFactory.Establish(SCardScope.System))
                        {
                            using (var isoReader = new IsoReader(context, LoadedBChips.ReaderName, SCardShareMode.Shared, SCardProtocol.Any))
                            {
                                var unlockResponse = isoReader.Transmit(AdpuHelper.SendPin());
                                WriteToLogFile("CardUnblock", unlockResponse);
                                if (unlockResponse.StatusWord != 0x00009000)
                                {
                                    UpdateTextLabel(CreatingKeyLabel, "");
                                    UpdateTextLabel(CreateKeyErrorLabel, "Could not be unlock bchip for writing.");
                                    return;
                                }
                                var writeResponse = isoReader.Transmit(AdpuHelper.WriteCardData(bchip));
                                WriteToLogFile("WriteCardData", writeResponse);
                                if (unlockResponse.StatusWord != 0x00009000)
                                {
                                    UpdateTextLabel(CreatingKeyLabel, "");
                                    UpdateTextLabel(CreateKeyErrorLabel, "Failure writing data to the bchip.");
                                    return;
                                }
                            }
                        }

                        UpdateTextLabel(CreatingKeyLabel, string.Empty);

                        // Done? clear loaded card, reload card
                        LoadedBChips = null;
                        ScanAndLoadConnectedCards();
                    }
                    catch (Exception ex)
                    {
                        WriteToLogFile($"Exception caught while provisioning card: {ex.Message} - {ex.StackTrace}", "ProvisionCardButton_Click");
                    }
                    finally
                    {
                        ProvisionNewKeysViewGrid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ProvisionNewKeysViewGrid.IsEnabled = true;
                        }));
                    }
                }));
        }

        private void UpdateTextLabel(Label label, string text)
        {
            label.Dispatcher.BeginInvoke(new Action(() =>
            {
                label.Content = text;
            }));
        }

        private void ClearTextBox(TextBox textBox)
        {
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    textBox.Text = String.Empty;
                }));
            }
        }

        private void ClearPasswordBox(PasswordBox passwordBox)
        {
            if (!String.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    passwordBox.Password = String.Empty;
                }));
            }
        }

        private void DispatcherUpdater(StackPanel uiControl, Visibility visibility)
        {
            uiControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                uiControl.Visibility = visibility;
            }));
        }
        
        public PKType CardPkType { get; private set; }
        private NBitcoin.Key CardGeneratedKey { get; set; }
        private NBitcoin.Network Network { get; set; }
        private void SetupProvisioningWindow(PKType pKType)
        {
            CardPkType = pKType;
            CardGeneratedKey = new NBitcoin.Key();

            switch (pKType)
            {
                case PKType.BTC:
                    Network = NBitcoin.Network.Main;
                    break;
                case PKType.BTC_TestNet:
                    Network = NBitcoin.Network.TestNet;
                    break;
                case PKType.CUSTOM:
                    CardGeneratedKey = null;
                    Network = null;
                    break;
            }

            if (CardGeneratedKey != null)
            {
                PublicKeyTextBox.Text = CardGeneratedKey.PubKey.GetAddress(Network).ToString();
                PrivateKeyTextBox.Text = CardGeneratedKey.GetBitcoinSecret(Network).ToWif();

                HelperText.Text = "When setting up a Bitcoin address, a private key is automatically generated for you. You can replace the private key data with a seed phrase mnemonic or your own WIF address.";
            }
            else
            {
                PublicKeyTextBox.Text = String.Empty;
                PrivateKeyTextBox.Text = String.Empty;

                HelperText.Text = "Public data is not encrypted and optional. Any data may be used in the Private Key textbox, as long as it is under 96 bytes.";
            }
        }

        private void CreateBtcWallet_Click(object sender, RoutedEventArgs e)
        {
            SetupProvisioningWindow(PKType.BTC);
            ChangePageUi(PageToShow.ProvisionCard, LoadedBChips);
        }

        private void CreateBtcTestnetWallet_Click(object sender, RoutedEventArgs e)
        {
            SetupProvisioningWindow(PKType.BTC_TestNet);
            ChangePageUi(PageToShow.ProvisionCard, LoadedBChips);
        }

        private void CreateCustomButton_Click(object sender, RoutedEventArgs e)
        {
            SetupProvisioningWindow(PKType.CUSTOM);
            ChangePageUi(PageToShow.ProvisionCard, LoadedBChips);
        }

        private void PrivateKeyTextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            // TODO: We'll eventually shuffle raw memory around each keystroke. This incldues hid for fun.
        }
    }
}
