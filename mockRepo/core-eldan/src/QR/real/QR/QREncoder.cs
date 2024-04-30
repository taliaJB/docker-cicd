using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Rendering;

namespace Eldan.QR
{
    public class QREncoder
    {

        public void CreateQRImage(string content, int moduleSize, string fileName)
        {
            CreateQRImage(content, moduleSize, fileName, ImageFormat.Png);
        }
        
        public void CreateQRImage(string content, int moduleSize, string fileName, ImageFormat imageFormat)
        {
            CreateQRImage(content, moduleSize, fileName, imageFormat, Brushes.Black, Brushes.White);
        }

        public void CreateQRImage(string content, int moduleSize, string fileName, ImageFormat imageFormat, Brush darkBrush, Brush lightBrush)
        {
            string suffix = imageFormat.ToString().ToLower();

            if (fileName.ToLower().IndexOf("." + suffix, fileName.Length - 1 - suffix.Length) < 0)
            {
                fileName += "." + suffix;
            }

            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = qrEncoder.Encode(content);

            Renderer renderer = new Renderer(moduleSize, darkBrush, lightBrush);
            renderer.CreateImageFile(qrCode.Matrix, fileName, imageFormat);
        }
    }
}
