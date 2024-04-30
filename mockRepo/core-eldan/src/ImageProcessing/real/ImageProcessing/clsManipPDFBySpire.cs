using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.General.Find;
using System.Drawing.Imaging;
using System.IO;
using Spire.Pdf.Graphics;


namespace Eldan.ImageProcessing
{
    public class clsManipPDFBySpire
    {
        public clsManipPDFBySpire()
        {
            Spire.License.LicenseProvider.SetLicenseKey("JnGW2L33xZ/iAceqigEAFQxu6DgxT+NsRc4YhGrlD8IOhW8ipOugCu9wZGFxb3LofPHKKDWbQKqbKJ9TDgPhWdG3ccqzDMt+wp0CHauNEmiUtx+SnKzqLRtS0ZQIy2xEmu8pm+1oh/FFCy2ttABgsdekJcRTohchxQ7S3Gs4DrXXhbfoM5YoI9b6EFIxl8EOa88z81Sf2kM6aqjUHWSSt1RKxx3pJNCd8v9k+XZfE4AUe7VGqGQe7JNwlrlrVhJnb2mmAoEnY3FxigLrGkD6av8HWVDC4g0EaUIsbGF0YBELHdsETySHqtbtGyGfIwOqt1QzOPclNxURPEBaqiLXxcQ95xdKdXqBjbQep3xzigikC3tv6Qc/qhEU5nf71esgxFv8gIhQb0l1+WQtkm8K3B6KUqE6qIIYpgGHN67Fr0LDsSIwvNIFgqU1zCc5GH4UgAcXIOe2/v8zk7d1hhhzJM51hyvSM/1OW+mhlCfNPh1D4SldT/yR2z3uQy1gaZYFxjcXzpTkeYbEaK3eSjclyZrjmU8s7frZvSBT9cPieR674feY+1JBvp3lvimtdFGprfBT7gcarZ7yUdJCQA3w1/kMqV63l03MksaKxslFwEqa0pwkslwk0lR8ctmnl/Zh0RSwcb7C4pnQNdIrdHCxpJGUB3GvFYKV5uDQ5ca81TOicFTBIJutKmFjViWJh22xjSVUfJ2C0HmzkGqV+/lwYsCRSeCKU1h+gYn2+ye+NzdEQX/Vm0XECx+CbcJtuTzA+Ec1XrgZgiQtRim4ShOWzKdPEb9xht2GVUmt2EyPuz11cf5l8oZ6qaZBf0PHQQZLcAqKRGQ+BV3BImYlO9zKuu5wy6bsn4Cg2+M19K3RRKFOwZqkfoYNo16cs8Q6SH3KhBpCiOx725rzDhvtJYzDiC/9Q96gZxBz4hRwe1VUnUpPKhIMTT8VDYUXCLDpfUygEkpAoaHZu0BqsF0ny4I2TG/q+t4/7UiSVvjul0zHFOlpI8SwLLoSzUzm/iDd+6/iecnU4ktGaEBQ4ZJzQE4HrdgRiZUbjjjzxz2k4hr/VWBByweOipZwABWUVBK1II5BVP6Iat6Up0LhsXV4VJLj9Vmt5oZWjuH8qtBfJO145PO+2EvaXOxlQQ8CczY39EDUbSoob8PzC6Lu/aEFpqOI6ZlTIA8CJG2BS4iYfJisbbMUOGu5NJ8oOdYBViaMta59UpszaWxa0FHbUPdkGhgCG60+H1AIDhTfBgYtgOqemoD6qzjg2sbla0VnuCcsrAAgizYgqNx4p9lhisGcGsMOF/InKpWdK81suRHgFWWf1iPmWYjPhYftx4N1JSiC0wqdcEzX4Nae1FaVLsN7YkCKCY9P5qjjiU1osJ/bKg/+dkKshLaJnVhMUSHGm2Opy/vGU/UNd5J2mNpHSN8N+Ed7IuplEvW1q+JYrf0CucTLB9fGpJ6O0Ck2pCULPH6abLkJkh/UspwrrQKJxi17PFcA/kQg1jFtSVwghM9yVN6TsVM=");
        }

        public List<TextPoint> GetTextCoordinatesInPDF(string PDFFileName, string text)
        {
            PdfDocument Doc = new PdfDocument();
            Doc.LoadFromFile(PDFFileName);
            List<TextPoint> TextPoints = new List<TextPoint>();

            int Index = 1;
            foreach (PdfPageBase page in Doc.Pages)
            {
                PdfTextFindCollection Results = page.FindText(text, TextFindParameter.None);

                foreach (PdfTextFind Result in Results.Finds)
                {
                    TextPoint TPoint = new TextPoint { Coordinates = Result.Position, PageNumber = Index };
                    TextPoints.Add(TPoint);
                }

                Index++;
            }

            return TextPoints;
        }

        public void ConvertTiff2PDF(string TIFFname, string PDFname)
        {
            ConvertTiff2PDF(TIFFname, PDFname, 0.22f, 0.22f);
        }

        public void ConvertTiff2PDF(string TIFFname, string PDFname, float scaleWidth, float scaleHeight )
        {
            using (PdfDocument pdfDocument = new PdfDocument())
            {
                Image tiffImage = Image.FromFile(TIFFname);
                Image[] images = SplitTIFFImage(tiffImage);
                for (int i = 0; i < images.Length; i++)
                {
                    PdfImage pdfImg = PdfImage.FromImage(images[i]);
                    PdfPageBase page = pdfDocument.Pages.Add();
                    float width = pdfImg.Width * scaleWidth;
                    float height = pdfImg.Height * scaleHeight;
                    float x = (page.Canvas.ClientSize.Width - width) / 2;
                    //set the image of the page 
                    page.Canvas.DrawImage(pdfImg, x, 0, width, height);
                }
                pdfDocument.SaveToFile(PDFname);
                //System.Diagnostics.Process.Start(PDFname);
            }
        }

        private static Image[] SplitTIFFImage(Image tiffImage)
        {
            int frameCount = tiffImage.GetFrameCount(FrameDimension.Page);
            Image[] images = new Image[frameCount];
            Guid objGuid = tiffImage.FrameDimensionsList[0];
            FrameDimension objDimension = new FrameDimension(objGuid);
            for (int i = 0; i < frameCount; i++)
            {
                tiffImage.SelectActiveFrame(objDimension, i);
                using (MemoryStream ms = new MemoryStream())
                {
                    tiffImage.Save(ms, ImageFormat.Tiff);
                    images[i] = Image.FromStream(ms);
                }
            }
            return images;
        }

        public enum EnmPagePosition
        {
            FirstPage = 1,
            LastPage = 2,
            SpecificPage = 3
        }

        public void EmbedTextInPDF(string oldPDFname, string newPDFname, float X, float Y, List<TextPart> textParts, EnmPagePosition pagePosition, int? pageNum)
        {
            if (pagePosition == EnmPagePosition.SpecificPage && !pageNum.HasValue)
                throw new Exception("pageNum must have value when selecting specific page");

            //Create a PdfDocument instance
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(oldPDFname);

            //Get the first page in the PDF document
            PdfPageBase page = null;
            switch (pagePosition)
            {
                case EnmPagePosition.FirstPage:
                    page = pdf.Pages[0];
                    break;
                case EnmPagePosition.LastPage:
                    page = pdf.Pages[pdf.Pages.Count - 1];
                    break;
                case EnmPagePosition.SpecificPage:
                    page = pdf.Pages[pageNum.Value];
                    break;
                default:
                    break;
            }

            //TransformText(page);
            TransformText(page, X, Y, textParts);

            pdf.SaveToFile(newPDFname);
        }

        //private static void TransformText(PdfPageBase page)
        //{
        //    //save graphics state
        //    PdfGraphicsState state = page.Canvas.Save();

        //    //Draw the text - transform           
        //    // PdfFont font = new PdfFont(PdfFontFamily.Helvetica, 18f);
        //    //PdfFont font = new PdfFont(PdfFontFamily.TimesRoman, 18f);
        //    //PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("Alef-Regular.ttf", 16f, FontStyle.Regular), true);

        //    PdfTrueTypeFont font = new PdfTrueTypeFont(new Font("Ariel.ttf", 16f, FontStyle.Regular), true);
        //    //PdfTrueTypeFont font = new PdfTrueTypeFont(new Font(@"C:\Main\Eldan.ImageProcessing\Test2\TestFiles\ariel.ttf", 16f, FontStyle.Bold), true);

        //    //PdfFont font = new PdfFont(PdfFontFamily.TimesRoman, 18f);
        //    PdfSolidBrush brush1 = new PdfSolidBrush(Color.Blue);
        //    PdfSolidBrush brush2 = new PdfSolidBrush(Color.Red);

        //    page.Canvas.TranslateTransform(30, 100);
        //    //page.Canvas.ScaleTransform(1f, 0.6f);
        //    //page.Canvas.SkewTransform(-10, 0);
        //    page.Canvas.DrawString("Go! Turn Around! Go! Go! Go!", font, brush1, 0, 0);

        //    //page.Canvas.SkewTransform(10, 0);
        //    page.Canvas.DrawString(clsImagesConverter.Reverse("שלום עולם"), font, brush2, 300, 0);

        //    //page.Canvas.SkewTransform(10, 0);
        //    //page.Canvas.DrawString("Go! Turn Around! Go! Go! Go!", font, brush2, 0, 0);

        //    //page.Canvas.ScaleTransform(1f, -1f);
        //    //page.Canvas.DrawString("Go! Turn Around! Go! Go! Go!", font, brush2, 0, -2 * 18);
        //    //restor graphics
        //    page.Canvas.Restore(state);
        //}

        private static void TransformText(PdfPageBase page, float X, float Y, List<TextPart> textParts)
        {
            //save graphics state
            PdfGraphicsState state = page.Canvas.Save();

            page.Canvas.TranslateTransform(X, Y);

            foreach (TextPart textPart in textParts)
            {
                PdfTrueTypeFont font = new PdfTrueTypeFont(textPart.Font, true);
                PdfSolidBrush brush = new PdfSolidBrush(textPart.FontColor ?? Color.Black);

                page.Canvas.DrawString(textPart.RightToLeft? clsImagesConverter.Reverse(textPart.Text) : textPart.Text, font, brush, textPart.OffsetX, textPart.OffsetY);
            }
            page.Canvas.Restore(state);
        }

        public class TextPart
        {
            public TextPart()
            { }

            public TextPart(string text, float offsetX, float offsetY, bool rightToLeft, Color? fontColor, string fontFamilyName, float fontSize, FontStyle? fontStyle)
                : this(text, offsetX, offsetY, rightToLeft, fontColor,
                      new Font(fontFamilyName ?? "arial.ttf",
                                     fontSize,
                                     fontStyle ?? FontStyle.Regular))
            { }

            public TextPart(string text, float offsetX, float offsetY, bool rightToLeft, Color? fontColor, Font font)
            {
                Text = text;
                OffsetX = offsetX;
                OffsetY = offsetY;
                RightToLeft = rightToLeft;
                Font = font;
                FontColor = fontColor;
            }

            public string Text;
            public float OffsetX;
            public float OffsetY;
            public bool RightToLeft;
            public Font Font;
            public Color? FontColor;
        }

    }

    public struct TextPoint
    {
        public PointF Coordinates;
        public int PageNumber;
    }
}
