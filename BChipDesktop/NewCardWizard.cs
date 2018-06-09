using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace BChipDesktop
{
    public partial class NewCardWizard : UserControl
    {
        private BindingSource CardListBinding = null;

        public NewCardWizard(BindingSource cardListBinding)
        {
            InitializeComponent();
            this.CardListBinding = cardListBinding;
            IBuffer randomId = CryptographicBuffer.GenerateRandom(7);
            cardIdTextbox.Text = CryptographicBuffer.EncodeToHexString(randomId);
        }

        private void NewCardWizard_Load(object sender, EventArgs e)
        {
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to cancel? All data previously entered will be permanently erased.","Discard data", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Dispose();  
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            CardType cardType;
            if (!Enum.TryParse(cardTypeSelection.Text, out cardType))
            {
                cardType = CardType.Unknown;
            }
            IBuffer cardId = null;
            try
            {
                cardId = CryptographicBuffer.DecodeFromHexString(cardIdTextbox.Text);
            }
            catch
            {
                MessageBox.Show("A card id must be 7 bytes.","Invalid Id");
                return;
            }
            
            //BChipMemoryLayout_BCHIP bChipCardData =
            //                new BChipMemoryLayout_BCHIP(
            //                    dataFromCard.AsBuffer(
            //                        // Skip the first byte as the card type is
            //                        // a BCHIP that may or may not be initialized
            //                        BChipMemoryLayout.MLVI_ADDR + 1,
            //                        BChipMemoryLayout.MLVI_MAX_DATA - 1
            //                        ),
            //                    dataFromCard.AsBuffer(
            //                        BChipMemoryLayout_BCHIP.START_ADDR,
            //                        dataLength - BChipMemoryLayout_BCHIP.START_ADDR
            //                        ),
            //                        true,
            //                        PKStatus.NotAvailable
            //                );



            //PKType detectedPkType = (PKType)bChipCardData.bchipVIDent[BChipMemoryLayout_BCHIP.PKTYPE_ADDR];
            //currencyComboBox.Text = detectedPkType.ToString();

            //bChipMemoryLayoutBindingSource.Add(bChipCardData);




            ///CardListBinding.Add();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
