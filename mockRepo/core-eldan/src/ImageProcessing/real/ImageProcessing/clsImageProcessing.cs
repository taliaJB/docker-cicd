using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Eldan.ImageProcessing
{
    public class clsImageProcessing
    {
        public static void WriteTextOnImage(string imagePathIn, string imagePathOut, string text, Font textFont, Brush textColor, Point startPos, bool RTL = false)
        {
            Bitmap Image = new Bitmap(imagePathIn);
            Bitmap NewImage = WriteTextOnImage(Image, text, textFont, textColor, startPos, RTL);

            NewImage.Save(imagePathOut);
        }

        public static Bitmap WriteTextOnImage(Bitmap image, string text, Font textFont, Brush textColor, Point startPos, bool RTL = false)
        {
            RectangleF rect = new RectangleF(startPos.X, startPos.Y, image.Width, image.Height);

            Graphics graphics = Graphics.FromImage(image);
            if (RTL)
            {
                StringFormat format = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                graphics.DrawString(text, textFont, textColor, rect, format);
            }
            else
                graphics.DrawString(text, textFont, textColor, rect);
            return image;
        }

        public static void ResizeImage(string orgImagePath, string newImagePath, int newImageWidth, int newImageHeight)
        {
            ResizeImage(orgImagePath, newImagePath, new Size(newImageWidth, newImageHeight));
        }

        public static void ResizeImage(string orgImagePath, string newImagePath, Size newImageSize)
        {
            const double EPSILON = 0.001;

            Bitmap orgImage = new Bitmap(orgImagePath);

            double orgAR = (double)orgImage.Width / (double)orgImage.Height;
            double newAR = (double)newImageSize.Width / (double)newImageSize.Height;

            Bitmap newImage;

            if (Math.Abs(orgAR - newAR) < EPSILON)
            {
                newImage = new Bitmap((Image)orgImage, newImageSize);
            }
            else
            {
                if (orgAR > newAR)
                {
                    newImage = GetHorizontalCrop(orgImage, newImageSize);
                }
                else
                {
                    newImage = GetVerticalCrop(orgImage, newImageSize);
                }
            }

            newImage.Save(newImagePath, ImageFormat.Jpeg);

        }

        private static Bitmap GetHorizontalCrop(Bitmap orgImage, Size newImageSize)
        {
            int scalWidth = newImageSize.Height * orgImage.Width / orgImage.Height;
            Bitmap scalImgage = new Bitmap((Image)orgImage, new Size(scalWidth, newImageSize.Height));
            int cropX = (scalWidth - newImageSize.Width) / 2;
            return (Bitmap)cropImage(scalImgage, new Rectangle(cropX, 0, newImageSize.Width, newImageSize.Height));
        }

        private static Bitmap GetVerticalCrop(Bitmap orgImage, Size newImageSize)
        {
            int scalHeight = newImageSize.Width * orgImage.Height / orgImage.Width;
            Bitmap scalImgage = new Bitmap((Image)orgImage, new Size(newImageSize.Width, scalHeight));
            int cropY = (scalHeight - newImageSize.Height) / 2;
            return (Bitmap)cropImage(scalImgage, new Rectangle(0, cropY, newImageSize.Width, newImageSize.Height));
        }

        private static Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }

    }
}
