using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace BChipDesktop
{
    public partial class ExportKeyDialog : UserControl
    {
        public ExportKeyDialog()
        {
            InitializeComponent();
        }

        private string QrCodeData   { get; set; }
        private int PixelSize       { get; set; }

        public ExportKeyDialog(string qrCodeData, int pixelSize)
        {
            this.QrCodeData = qrCodeData;
            this.PixelSize = pixelSize;
        }

        public Bitmap GetQrCode()//string qrCodeData, int pixelSize)
        {
            QRCodeGenerator qrCodeGen = new QRCoder.QRCodeGenerator();
            QRCodeData data = qrCodeGen.CreateQrCode(QrCodeData, QRCoder.QRCodeGenerator.ECCLevel.H);
            QRCode qrc = new QRCode(data);
            return qrc.GetGraphic(PixelSize);
        }

        public void ExportKeyDialog_Load(object sender, EventArgs e)
        {
            qrCodeImage = new PictureBox();
            qrCodeImage.Image = GetQrCode();
            keyAddressLabel.Text = this.QrCodeData;
        }


        public void LoadData()
        {
            //qrCodeImage.Image = GetQrCode();
            //keyLabel.Text = this.QrCodeData;
        }

        public void Close_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void copyKey_Click(object sender, EventArgs e)
        {

        }
    }
}
