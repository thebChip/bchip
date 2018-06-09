using bChipDesktop;
using NBitcoin;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace BChipDesktop
{
    public partial class MainUI : Form
    {
        public StreamWriter logWriter = null;
        private static Configuration config = new Configuration();
        private BChipMemoryLayout_BCHIP loadedCardInMemory = null;
        private bool UnsavedData = false;
        //private DataGridViewRow selectedCard = null;
        //private BChipMemoryLayout_BCHIP bChipCardData = null;

        // TODO: Eventually clear the passphrase from memory or 
        //       re-write this to be a tad more secure.
        private System.Threading.Timer passphraseClearTimer = null;
        private DateTime passSetTime = DateTime.MinValue;
        private string passPhrase = null;
        private string PassPhrase
        {
            get
            {
                return passPhrase;
            }

            set
            {
                passPhrase = value;
                passSetTime = DateTime.Now;
            }
        }
        private DateTime pkSetTime = DateTime.MinValue;
        private string privateKey = null;
        private string PrivateKey
        {
            get
            {
                return privateKey;
            }

            set
            {
                privateKey = value;
                pkSetTime = DateTime.Now;
            }
        }

        public MainUI()
        {
            using (logWriter = File.AppendText(
                $"{DateTime.Now.ToString("yyyy-MM-dd")}-BChip.log"))
            {
                try
                {
                    WriteToLogFile($"App started", "Startup");
                    InitializeComponent();
                    SetupDataGridView();
                    HideAllPassphraseUI();
                    sendBtn.Enabled = false;
                    ReceiveBtn.Enabled = false;
                    notConnectedIcon.Visible = true;

                    WriteToLogFile($"Timer started", "Startup");
                    passphraseClearTimer = new System.Threading.Timer(
                     _=>
                    {
                        string statusMessage = String.Empty;
                        if (!String.IsNullOrWhiteSpace(privateKey))
                        {
                            if (DateTime.Now.Subtract(pkSetTime) > TimeSpan.FromSeconds(30))
                            {
                                privateKey = String.Empty;
                            }
                            else
                            {
                                statusMessage += $"Pkey cleared in {DateTime.Now.Subtract(pkSetTime).TotalSeconds} seconds. ";
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(PassPhrase))
                        {
                            if (DateTime.Now.Subtract(passSetTime) > TimeSpan.FromSeconds(30))
                            {
                                PassPhrase = String.Empty;
                            }
                            else
                            {
                                statusMessage += $"Passphrase cleared in {DateTime.Now.Subtract(passSetTime).TotalSeconds} seconds. ";
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(statusMessage))
                        {
                            statusStrip1.Invoke((Action)delegate () { statusStrip1.Text = statusMessage; });   
                        }
                    }
                    , null, 1000, 1000);
                    
                }
                finally
                {
                    WriteToLogFile($"App closing", "Main");
                    logWriter.Flush();
                    logWriter.Close();
                }
            }
        }

        

        public async void WriteToLogFile(string dataToLog, string source = "General")
        {
            Monitor.Enter(logWriter);
            {
                try
                {
                    await logWriter.WriteLineAsync($"{DateTime.Now.ToShortTimeString()}({source}):{dataToLog}");
                    logWriter.Flush();
                }
                catch
                { }
                finally
                {
                    Monitor.Exit(logWriter);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bChipMemoryLayoutBindingSource.CurrentChanged += BChipMemoryLayoutBindingSource_CurrentChanged;
            currencyComboBox.Items.Clear();
            foreach(string pkType in Enum.GetNames(typeof(PKType)))
            {
                currencyComboBox.Items.Add(pkType);
            }

            ClearPanelUi();
            insertCardLabel.Visible = true;
            notConnectedIcon.Visible = true;
        }

        private void BChipMemoryLayoutBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (bChipMemoryLayoutBindingSource.Current == null)
            {
                return;
            }

            // NOTE: Hardcoded to BCHIP format
            if (loadedCardInMemory != (BChipMemoryLayout_BCHIP)bChipMemoryLayoutBindingSource.Current)
            {
                if (UnsavedData)
                {
                    switch(
                        MessageBox.Show(
                            "You have unsaved changes that will be lost if you selected another card. Are you sure?",
                            "Unsaved changes detected",
                            MessageBoxButtons.YesNo))
                    {
                        case DialogResult.Yes:
                            break;
                        case DialogResult.No:
                        default:
                            return;
                    }
                }

                ClearPanelUi();
                // NOTE: Hardcoded to BCHIP format
                loadedCardInMemory = (BChipMemoryLayout_BCHIP)bChipMemoryLayoutBindingSource.Current;
                LoadCardInUI(loadedCardInMemory);
            }
        }

        private void SetupDataGridView()
        {
            cardList.RowHeadersVisible = false;
            foreach(DataGridViewColumn col in cardList.Columns)
            {
            
            }

            return;

            // TODO: Remove all references, moving to data bound object
            /// old logic
            cardList.SelectionChanged += CardList_SelectionChanged;
            cardList.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            cardList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            cardList.ColumnHeadersDefaultCellStyle.Font =
                new Font(cardList.Font, FontStyle.Bold);

            //cardList.AutoSizeRowsMode =
            //    DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            //cardList.ColumnHeadersBorderStyle =
            //    DataGridViewHeaderBorderStyle.Single;
            //cardList.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            //cardList.GridColor = Color.Black;
            cardList.RowHeadersVisible = false;
            cardList.BorderStyle = BorderStyle.None;
            cardList.CellBorderStyle = DataGridViewCellBorderStyle.None;
            cardList.AllowUserToResizeRows = false;
            cardList.ScrollBars = ScrollBars.None;

            cardList.ReadOnly = true;

            // ID Type Addr BChip PK
            cardList.ColumnCount = 5;
            cardList.Columns[0].Name = "ID";
            cardList.Columns[0].ToolTipText = "Card Identifier";
            cardList.Columns[1].Name = "Type";
            cardList.Columns[1].ToolTipText = "Expected Private Key Usage";
            cardList.Columns[2].Name = "Addr";
            cardList.Columns[2].ToolTipText = "Copy public address to clipboard";
            cardList.Columns[3].Name = "BChip";
            cardList.Columns[3].ToolTipText = "Currently connected BChips";
            cardList.Columns[4].Name = "PK";
            cardList.Columns[4].ToolTipText = "Source of Private Key";

            for (int i = 0; i < cardList.Columns.Count; ++i)
            {
                cardList.Columns[i].Resizable = DataGridViewTriState.False;
                cardList.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cardList.Columns[i].Width = 88;
            }

            cardList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            cardList.MultiSelect = false;
            cardList.Dock = DockStyle.Fill;

        }

        private void CardList_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CardList_SelectionChanged(object sender, EventArgs e)
        {
            if (cardList.SelectedRows != null || cardList.SelectedRows.Count > 0)
            {
                
            }
        }

        private void LoadCardPage()
        {

        }

        private void confirmWifEncrypt_Click(object sender, EventArgs e)
        {

        }

        private void SaveCard()
        {

        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (UnsavedData)
                {
                    if (
                        MessageBox.Show(
                            "Unsaved changes were made, are you sure you want to add a new card and lose unsaved changes?",
                            "Unsaved card data", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                ClearPanelUi();
                NewCardWizard ncw = new NewCardWizard(bChipMemoryLayoutBindingSource);
                ncw.Show();
                ncw.Dock = DockStyle.Fill;
                panel3.Controls.Add(ncw);
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Exception adding card to list: {ex.Message}-{ex.StackTrace}", "addBtn_Click");
            }
        }

        public const int COPY_PUBKEY_ICON = 3;
        public const int PKEY_ICON = 6;
        public const int REMOVE_ICON = 7;
        private async void cardList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = 
                cardList.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DataGridViewRow dataGridViewRow = cardList.Rows[e.RowIndex];
            BChipMemoryLayout bchipCardFromList = bChipMemoryLayoutBindingSource[e.RowIndex] as BChipMemoryLayout;

            if (cell.Selected)
            {
                switch (e.ColumnIndex)
                {
                    case COPY_PUBKEY_ICON:
                        {
                            // TODO: Actually do what I said I would do
                            MessageBox.Show($"This data will eventually be copied to your clipboard: {bchipCardFromList.PublicAddress}");
                        }
                        break;
                    case PKEY_ICON:
                        {
                            // TODO: Add support for export via this part.
                            MessageBox.Show($"The status of the PK key on this system, if it is present, encrypted or unavailable.");
                        }
                        break;
                    case REMOVE_ICON:
                        {
                            // TODO: Actually do what I said I would do
                            if (
                                MessageBox.Show(
                                    $"Are you sure you want to delete all data associated with this entry (ID: {bchipCardFromList.IdLabel})?",
                                    "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {

                                //  bChipMemoryLayoutBindingSource.RemoveAt(e.RowIndex);
                                //  cardList.Rows.RemoveAt(e.RowIndex);
                                //  bChipMemoryLayoutBindingSource.ResetCurrentItem();
                                RemoveCardFromList(bchipCardFromList, dataGridViewRow);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private async void RemoveCardFromList(BChipMemoryLayout cardFromList, DataGridViewRow dataGridViewRow)
        {
            if (CompareCrc(cardFromList as BChipMemoryLayout_BCHIP, loadedCardInMemory))
            {
                ClearPanelUi();
                insertCardLabel.Visible = true;
                notConnectedIcon.Visible = true;
            }
            bChipMemoryLayoutBindingSource.Remove(cardFromList);
            if (cardList.Rows.Contains(dataGridViewRow))
            {
                cardList.Rows.Remove(dataGridViewRow);
            }
            cardList.Refresh();
        }

        private void AddCardToList(BChipMemoryLayout bChipMemoryLayout = null)
        {
            cardList.Rows.Add();
            
            cardList.Rows[cardList.Rows.Count - 2].Height = 50;
            DataGridViewImageCell img = new DataGridViewImageCell();
            img.Value = Image.FromStream(
                    Assembly.GetEntryAssembly().GetManifestResourceStream(
                        "BChipDesktop.Assets.Editing-Copy-icon.png"));
            
            cardList[COPY_PUBKEY_ICON, cardList.Rows.Count - 2] = img;
        }

        private AddCardResult AddPhysicalCardToList(
            byte[] dataFromCard, out BChipMemoryLayout_BCHIP bChipCardData)
        {
            // We expect exactly 0xDE bytes, with an additional 4 bytes for 9000
            int dataLength = 222 + 4;
            if (dataFromCard.Length == dataLength)
            {
                // Start processing the card
                bChipCardData =
                    new BChipMemoryLayout_BCHIP(
                        dataFromCard.AsBuffer(
                            // Skip the first byte as the card type is
                            // a BCHIP that may or may not be initialized
                            BChipMemoryLayout.MLVI_ADDR + 1,
                            BChipMemoryLayout.MLVI_MAX_DATA - 1
                            ),
                        dataFromCard.AsBuffer(
                            BChipMemoryLayout.MLVI_MAX_DATA,
                            dataLength - BChipMemoryLayout.MLVI_MAX_DATA
                            ),
                            true,
                            PKStatus.NotAvailable
                    );

                // check if card is already in the list
                int existingCardIndex = IndexOfCardInCardList(bChipCardData);
                if (existingCardIndex >= 0)
                {
                    BChipMemoryLayout_BCHIP existingCard = bChipMemoryLayoutBindingSource[existingCardIndex] as BChipMemoryLayout_BCHIP;
                    if (CompareCrc(bChipCardData, existingCard))
                    {
                        // same card
                        bChipMemoryLayoutBindingSource.Position = existingCardIndex;
                        return AddCardResult.NoChanges;
                    }

                    if (MessageBox.Show("Data on BChip does not match data in list, do you wish to delete and replace the local card data?", "Replace local card?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bChipMemoryLayoutBindingSource[existingCardIndex] = bChipCardData;
                        return AddCardResult.CardReplaced;
                    }
                }
                else
                {
                    // New card, add it.
                    bChipMemoryLayoutBindingSource.Insert(0, bChipCardData);
                    return AddCardResult.CardAdded;
                }
            }
            else
            {
                bChipCardData = null;
            }

            // Unepexted data output 
            errorLoadingLbl.Visible = true;
            notConnectedIcon.Visible = true;
            wipeBchipBtn.Visible = true;
            
            return AddCardResult.UnexpectedError;
        }

        public bool CompareCrc(BChipMemoryLayout_BCHIP a, BChipMemoryLayout_BCHIP b)
        {
            byte[] expectedCrc = a.crcData;
            byte[] crc = b.crcData;
            for (int i = 0; i < BChipMemoryLayout_BCHIP.CRC_MAX_SIZE; ++i)
            {
                if (expectedCrc[i] != crc[i])
                {
                    return false;
                }
            }

            return true;
        }


        public int IndexOfCardInCardList(BChipMemoryLayout cardData)
        {
            if (bChipMemoryLayoutBindingSource.Count > 0)
            {
                int index = 0;
                foreach (BChipMemoryLayout bchipdata in bChipMemoryLayoutBindingSource.List)
                {
                    for (int i = 0; i < BChipMemoryLayout.MLVI_MAX_DATA; i++)
                    {
                        if (bchipdata.mlvi[i] != cardData.mlvi[i])
                        {
                            break;
                        }

                        if (i == BChipMemoryLayout.MLVI_MAX_DATA - 1)
                        {
                            return index;
                        }
                    }
                }
            }

            return -1;
        }

        private async Task<int> LoadCardInUI(BChipMemoryLayout_BCHIP bChipCardData)
        {
            //BChipSmartCard loadedBchip = config.GetConnectedBChip();
            // Handle BCHIP cards (v0a)
            if (
                //loadedBchip != null &&
                //config.GetCardType(loadedBchip) == CardType.BChip
                bChipMemoryLayoutBindingSource.Current != null &&
                ((BChipMemoryLayout)bChipMemoryLayoutBindingSource.Current).cardType == CardType.BChip
                )
            {
                // Check if card is initialized/invalid format
                // NOTE: Hardcoded to BCHIP only.
                if (bChipCardData.mlvi[0] != (byte)CardType.BChip)
                {
                    // Determine if the card is likely a new bchip or 
                    // formatted with an unexpected format
                    bool initialized = false;
                    if (!initialized)
                    {
                        notFormattedLabel.Visible = true;
                    }
                    else
                    {
                        errorLoadingLbl.Visible = true;
                    }

                    wipeBchipBtn.Visible = true;
                    connectedIcon.Visible = true;
                    return 1;
                }

                // Recently formatted card?
                if (bChipCardData.IsFormatted)
                {
                    importKeyBtn.Visible = true;
                    createKeyBtn.Visible = true;
                    currencyComboBox.Enabled = true;
                }
                else
                {
                    wipeBchipBtn.Visible = true;

                    // Check if card CRC is good
                    byte[] computerCrc = bChipCardData.GetCardCheckSum();
                    byte[] cardCrc = bChipCardData.crcData;
                    bool isCrcGood = false;
                    for (int i = 0; i < computerCrc.Length && i < cardCrc.Length; i++)
                    {
                        isCrcGood = computerCrc[i] == cardCrc[i];
                        if (isCrcGood)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!isCrcGood)
                    {
                        bchipCrcFailLabel.Visible = true;
                        connectedIcon.Visible = true;
                        return -2; // bad crc 
                    }

                    // CRC good, show export options
                    exportKeyBtn.Visible = true;
                    // No changing key type
                    currencyComboBox.Enabled = false;
                }

                currencyComboBox.Visible = true;
                currencyComboBox.Enabled = true;

                currencyComboBox.Text = bChipCardData.PkType.ToString();

                connectedIcon.Visible = true;
                setPinBtn.Visible = true;
                setPinBtn.Enabled = true;
                saveToBchipBtn.Visible = true;
                keyTypeLabel.Visible = true;

                bChipMemoryLayoutBindingSource.Add(bChipCardData);

                // Done
                return 0;
            }

            // General error
            return -1;
        }

        private async void bchipBtn_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            try
            {
                button.Enabled = false;

                await config.ScanForbChipCards();

                // Load bchip data
                SmartCard card = await config.GetSmartCard(config.GetConnectedBChip());

                // Pull the body of the card; ignore the first 32 bytes
                IBuffer result = await AdpuHandler.SendADPUCommand(card, AdpuCommand.RetrieveBody, null);
                byte[] dataFromCard = result.ToArray();

                BChipMemoryLayout_BCHIP parsedCard;
                switch (AddPhysicalCardToList(dataFromCard, out parsedCard))
                {
                    case AddCardResult.CardAdded:
                    case AddCardResult.CardReplaced:
                        int index = IndexOfCardInCardList(parsedCard);
                        bChipMemoryLayoutBindingSource.Position = index;
                        return;
                    case AddCardResult.NoChanges:
                        return;
                    case AddCardResult.UnexpectedError:
                        WriteToLogFile($"LoadCardInUI returned {result}. Load failure.", "bchipBtn_Click");
                        return;
                    default:
                        cardList.ClearSelection();
                        ClearPanelUi();
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Exception processing data from smart card: {ex.Message}-{ex.StackTrace}", "bchipBtn_Click");
                MessageBox.Show("There was an error processing data from the card.","Failed to load card data");
            }
            finally
            {
                button.Enabled = true; 
            }

            // No card connected? Disable items
            insertCardLabel.Visible = true;
            notConnectedIcon.Visible = true;

            setPinBtn.Enabled = false;
            currencyComboBox.Enabled = false;
            return;
        }

        private void ClearPanelUi()
        {
            qrCodeImage.Visible = false;
            keyAddressLabel.Visible = false;
            copyKeyIcon.Visible = false;
            keyTypeLabel.Visible = false;
            notFormattedLabel.Visible = false;
            insertCardLabel.Visible = false;
            connectedIcon.Visible = false;
            setPinBtn.Visible = false;
            currencyComboBox.Visible = false;
            createKeyBtn.Visible = false;
            importKeyBtn.Visible = false;
            saveToBchipBtn.Visible = false;
            errorLoadingLbl.Visible = false;
            wipeBchipBtn.Visible = false;
            exportKeyBtn.Visible = false;
            notConnectedIcon.Visible = false;
            bchipCrcFailLabel.Visible = false;

            importConfirmKeyButton.Visible = false;
            importKeyTextBox.Visible = false;
            importKeyTextBox.Text = string.Empty;
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {

        }

        private void ReceiveBtn_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This functionality is not yet implemented.", "Not Implemented");
        }

        private void currencyComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if (currencyComboBox.SelectedText.Contains("CUSTOM"))
            //{
                //MessageBox.Show("Adding a custom string allows for general encryption and backup, however, there is no wallet support.", "Warning");
            //}
        }

        private void importKeyBtn_Click(object sender, EventArgs e)
        {
            importConfirmKeyButton.Visible = true;
            importKeyTextBox.Visible = true;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void wipeBchipBtn_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.Enabled = false;
            try
            {
                switch (MessageBox.Show("This card will be formatted. Please make sure you only have a BChip connected and the built in KEY has not been changed.", "WARNING", MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        IBuffer result = await BChipMemoryLayout.FormatCard(config, CardType.BChip);
                        string strRes = CryptographicBuffer.EncodeToHexString(result);
                        if (strRes.EndsWith("9000"))
                        {
                            ClearPanelUi();
                            MessageBox.Show("Formatting complete. Please reload the card.", "Complete");
                        }
                        else
                        {
                            WriteToLogFile($"Format failure. Return result: '{strRes}'", "Format");
                            MessageBox.Show("Card formatting failure.", "Failure");
                        }
                        break;
                    case DialogResult.No:
                    default:
                        break;
                }
            }
            finally
            {
                button.Enabled = true;
            }
        }

        private void setPinBtn_Click(object sender, EventArgs e)
        {
            try
            {
                setPinBtn.Visible = false;
                savePassphraseBtn.Visible = true;
                passphraseEntryBox.Visible = true;
            }
            finally
            {
             //   HideAllPassphraseUI();
             //   setPinBtn.Visible = true;
            }
        }

        private void HideAllPassphraseUI()
        {
            savePassphraseBtn.Visible = false;
            confirmPassphraseBtn.Visible = false;
            passphraseEntryBox.Visible = false;
            reconfirmPassphraseBox.Visible = false;   
        }

        private void MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (logWriter != null)
            {
                logWriter = null;
            }
        }

        private void confirmPassphraseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (passphraseEntryBox.Text != reconfirmPassphraseBox.Text)
                {
                    MessageBox.Show("The passphrases entered do not match. It is imparative your passphrase is never lost as there is no recovery.", "Passphrase Error");
                }
                else if (passphraseEntryBox.Text.TrimEnd().Length == 0)
                {
                    MessageBox.Show("The passphrase cannot be left blank.", "Passphrase Error");
                }
                else
                {
                    PassPhrase = passphraseEntryBox.Text;
                    passphraseEntryBox.Clear();
                    reconfirmPassphraseBox.Clear();
                }
            }
            finally
            {
                HideAllPassphraseUI();
                setPinBtn.Visible = true;
            }
        }

        private void savePassphraseBtn_Click(object sender, EventArgs e)
        {
            savePassphraseBtn.Visible = false;
            passphraseEntryBox.Visible = false;

            confirmPassphraseBtn.Visible = true;
            reconfirmPassphraseBox.Visible = true;
        }

        private async void saveToBchipBtn_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.Enabled = false;

            try
            {
                if (PassPhrase == null || PassPhrase.Length == 0)
                {
                    MessageBox.Show("The passphrase must be set before saving to a BChip", "Passphrase Error");
                    return;
                }

                await config.ScanForbChipCards();

                BChipSmartCard loadedBchip = config.GetConnectedBChip();
                // Handle BCHIP cards (v0a)
                if (
                    loadedBchip != null &&
                    config.GetCardType(loadedBchip) == CardType.BChip)
                {
                    if (loadedCardInMemory != null)
                    {
                        BChipMemoryLayout_BCHIP bChipCardData = loadedCardInMemory as BChipMemoryLayout_BCHIP;

                        IBuffer res = await bChipCardData.WriteDataToCard(config);
                        string atrResult = CryptographicBuffer.EncodeToHexString(res);
                        if (!atrResult.EndsWith("9000"))
                        {
                            WriteToLogFile($"Failed to data to card. Data result: {atrResult}", "saveToBchipBtn_Click");
                            MessageBox.Show("Error writing data to the card.");
                        }
                    }
                }
                else
                {
                    WriteToLogFile($"Failed to load card or card data", "saveToBchipBtn_Click");
                    MessageBox.Show("Failed to load card or card data.");
                }
            }
            catch (Exception ex)
            {
                WriteToLogFile($"Exception writing data to smart card: {ex.Message}-{ex.StackTrace}", "saveToBchipBtn_Click");
                MessageBox.Show("There was an error writing data to the card.", "Failed to write card data");
            }
            finally
            {
                button.Enabled = true;
            }
        }

        private void cardList_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // TODO: Save to database file on add and remove

            // Q&D hack to tweak rows as we build them
            if (e.RowCount > 0)
            {
                DataGridView gridView = sender as DataGridView;
                DataGridViewRow row = gridView.Rows[e.RowIndex];
                if (row.Height <= 50)
                {
                    row.Height = 75;
                }
            }
        }

        private void createKeyBtn_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.Enabled = false;

            try
            {
                if (PassPhrase == null || PassPhrase.Length == 0)
                {
                    MessageBox.Show("The passphrase must be set before creating a private key", "Passphrase Error");
                    return;
                }

                PKType pKType;
                if (!Enum.TryParse(currencyComboBox.Text, out pKType))
                {
                    MessageBox.Show("The key type selected cannot have a private key generated.", "KeyType not supported");
                    return;
                }

                Key PrivateKey = new NBitcoin.Key();
                switch (pKType)
                {
                    case PKType.BTC_TestNet:
                    case PKType.BTC:
                        break;
                    case PKType.ETH:
                    case PKType.CUSTOM:
                    case PKType.UNSET:
                    default:
                        MessageBox.Show($"{pKType.ToString()} cannot have a private key generated. Try import?", "KeyType not supported");
                        return;
                }

                Type cardType = loadedCardInMemory.GetType();
                // BChipMemoryLayout is the abstract class and likely an unformatted or unsupported card
                if (cardType == typeof(BChipMemoryLayout))
                {
                    WriteToLogFile($"Cardlist item was BChipMemoryLayout object", "createKeyBtn_Click");
                    MessageBox.Show("Unsupported (physical) card type", "Invalid card");
                    return;
                }
                else if (cardType == typeof(BChipMemoryLayout_BCHIP))
                {
                    BChipMemoryLayout_BCHIP bChipCard = loadedCardInMemory as BChipMemoryLayout_BCHIP;
                    bChipCard.EncryptPrivateKeyData(
                        pKType, PassPhrase, PrivateKey.ToBytes(), PrivateKey.PubKey.ToBytes(), config);

                    MessageBox.Show("Private key data generated, encrypted and saved to your bchip.");

                    importKeyBtn.Visible = false;
                    createKeyBtn.Visible = false;
                    exportKeyBtn.Visible = true;
                }
                // Unsupported
                else
                {
                    WriteToLogFile($"Cardlist item type was {cardType.ToString()}; unsupported.", "createKeyBtn_Click");
                    MessageBox.Show("Unsupported (physical) card type", "Invalid card");
                    return;
                }
            }
            finally
            {
                button.Enabled = true;
            }
        }

        private void cardList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void exportKeyBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to export your private key? It will be displayed onscreen.","Confirm export", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (PassPhrase == null || PassPhrase.Length == 0)
                {
                    MessageBox.Show("The passphrase must be set before you can decrypt and export your private key", "Passphrase Error");
                    return;
                }

                BChipMemoryLayout_BCHIP bChipCard = bChipMemoryLayoutBindingSource.Current as BChipMemoryLayout_BCHIP;
                byte[] decryptedKey = bChipCard.DecryptPrivateKeyData(PassPhrase);
                Key key = null;
                switch (bChipCard.PkType)
                {
                    case PKType.BTC:
                        key = new Key(decryptedKey);
                        PrivateKey = key.GetWif(Network.Main).ToWif();
                        break;
                    case PKType.BTC_TestNet:
                        key = new Key(decryptedKey);
                        PrivateKey = key.GetWif(Network.TestNet).ToWif();
                        break;
                    case PKType.UNSET:
                        MessageBox.Show($"{bChipCard.PkType.ToString()} cannot have a private key generated. Try import?", "KeyType not supported");
                        break;
                    case PKType.ETH:
                    case PKType.CUSTOM:
                    default:
                        PrivateKey = CryptographicBuffer.ConvertBinaryToString( BinaryStringEncoding.Utf8, decryptedKey.AsBuffer());
                        break;
                }

                // If successful
                ClearPanelUi();
                QRCodeGenerator qrCodeGen = new QRCoder.QRCodeGenerator();
                QRCodeData data = qrCodeGen.CreateQrCode(PrivateKey, QRCoder.QRCodeGenerator.ECCLevel.H);
                QRCode qrc = new QRCode(data);
                qrCodeImage.Image = qrc.GetGraphic(5);
                qrCodeImage.Visible = true;
                keyAddressLabel.Text = PrivateKey;
                keyAddressLabel.Visible = true;
                copyKeyIcon.Visible = true;
            }
        }

        private void copyKeyIcon_Click(object sender, EventArgs e)
        {
            keyAddressLabel_LinkClicked(sender, null);
        }

        private void keyAddressLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(PrivateKey.ToString());
        }

        private void importConfirmKeyButton_Click(object sender, EventArgs e)
        {
            if (PassPhrase == null || PassPhrase.Length == 0)
            {
                MessageBox.Show("The passphrase must be set before importing a key to a BChip", "Passphrase Error");
                return;
            }

            string contentToEncrypt = importKeyTextBox.Text.Trim();
            byte[] dataToEncrypt;
            byte[] optionalPublicKey = null;
            BChipMemoryLayout_BCHIP cardData = bChipMemoryLayoutBindingSource.Current as BChipMemoryLayout_BCHIP;
            PKType privateKeyType;
            if (!Enum.TryParse(currencyComboBox.Text, out privateKeyType))
            {
                MessageBox.Show("The selected type is unknown, for custom key usage, please use CUSTOM.", "KeyType not supported");
                return;
            }
            switch (privateKeyType)
            {
                case PKType.BTC:
                    try
                    {
                        // Do wif import
                        BitcoinExtKey address = new BitcoinExtKey(contentToEncrypt, Network.Main);
                        dataToEncrypt = address.PrivateKey.ToBytes();
                    }
                    catch (Exception ex)
                    {
                        WriteToLogFile($"Exception importing data from smart card: {ex.Message}-{ex.StackTrace}", "bchipBtn_Click");
                        MessageBox.Show("There was an error processing imported data. Is it a valid base58 BTC address?", "Failed to load card data");
                        return;
                    }
                    break;
                case PKType.BTC_TestNet:
                    try
                    {
                        // Do wif import
                        BitcoinExtKey address = new BitcoinExtKey(contentToEncrypt, Network.TestNet);
                        dataToEncrypt = address.PrivateKey.ToBytes();
                    }
                    catch (Exception ex)
                    {
                        WriteToLogFile($"Exception importing data from smart card: {ex.Message}-{ex.StackTrace}", "bchipBtn_Click");
                        MessageBox.Show("There was an error processing imported data. Is it a valid base58 BTC address?", "Failed to load card data");
                        return;
                    }
                    break;
                default:
                    // Reset PkType
                    privateKeyType = PKType.CUSTOM;
                    // Store as...raw data. Upto 96 bytes.
                    if (contentToEncrypt.Length > BChipMemoryLayout_BCHIP.MAX_USER_PK)
                    {
                        MessageBox.Show($"The key data must not exceed {BChipMemoryLayout_BCHIP.MAX_USER_PK} bytes","Keysize too large");
                        return;
                    }
                    dataToEncrypt = CryptographicBuffer.ConvertStringToBinary(contentToEncrypt, BinaryStringEncoding.Utf8).ToArray();
                    // TODO: we can optionally add public/readable data in the PK section.
                    break;
            }

            importKeyTextBox.Clear();
            cardData.EncryptPrivateKeyData(privateKeyType, PassPhrase, dataToEncrypt, optionalPublicKey, config);
            
            MessageBox.Show("Private key data generated, encrypted and saved to your bchip.");

            HideAllPassphraseUI();
        }

        private void cardList_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (loadedCardInMemory != null)
            {
                foreach (BChipMemoryLayout bchipItem in bChipMemoryLayoutBindingSource.List)
                {
                    int index = IndexOfCardInCardList(loadedCardInMemory);
                    bchipItem.IsConnected = (index == bChipMemoryLayoutBindingSource.Position) ;
                }
            }
        }
    }
}
