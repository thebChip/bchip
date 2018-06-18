using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

namespace BChipDesktop
{
    public class QrHandler
    {
        private string QrCodeData { get; set; }
        private int PixelSize { get; set; }

        public QrHandler(string qrCodeData, int pixelSize)
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
    }
}