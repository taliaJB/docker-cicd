using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iTextSharp.text.pdf;




namespace Eldan.ImageProcessing
{
    public static class clsImagesConverter
    {

        #region Convert Image 2 string 2 Image
        public static void Base64ToImage(string base64String, string FileName, ImageFormat format)
        {
            Image img = Base64ToImage(base64String);
            img.Save(FileName, format);
        }

        public static void Base64ToImage(string base64String, string FileName)
        {
            Image img = Base64ToImage(base64String);
            img.Save(FileName);
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static string ImageToBase64(string FileName, ImageFormat format)
        {
            Bitmap img = new Bitmap(FileName);
            return ImageToBase64(img, ImageFormat.Png);
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        } 
        #endregion

        #region Convert PDF 2 string 2 PDF
        public static string PDFtoBase64(string FileName)
        {
            byte[] pdfBytes = File.ReadAllBytes(FileName);
            return Convert.ToBase64String(pdfBytes);
        }

        public static void Base64toPDF(string base64String, string FileName)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            System.IO.FileStream stream =
            new FileStream(FileName, FileMode.Create);
            System.IO.BinaryWriter writer =
                new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        } 
        #endregion

        #region Embed Image In PDF
        public static void EmbedImageInPDF(string OrgPDFFileName, 
                                           string NewPDFFileName, 
                                           int PageNum,
                                           float ImageScaledWidth,
                                           float ImageScaledHeight,
                                           float Xpos,
                                           float Ypos,
                                           string imageFileName,
                                           enmSacleType SacleType,
                                           bool DockToTop)
        {
            iTextSharp.text.Image itextImage = iTextSharp.text.Image.GetInstance(imageFileName);

            EmbedImageInPDF(OrgPDFFileName, NewPDFFileName, PageNum, ImageScaledWidth, ImageScaledHeight, Xpos, Ypos, itextImage, SacleType, DockToTop);
        }

        public static void EmbedImageInPDF(string OrgPDFFileName, 
                                           string NewPDFFileName, 
                                           int PageNum,
                                           float ImageScaledWidth,
                                           float ImageScaledHeight,
                                           float Xpos,
                                           float Ypos, 
                                           string imageBase64, 
                                           ImageFormat imageFormat,
                                           enmSacleType SacleType,
                                           bool DockToTop)
        {
            Image sysImage = clsImagesConverter.Base64ToImage(imageBase64);

            EmbedImageInPDF(OrgPDFFileName, NewPDFFileName, PageNum, ImageScaledWidth, ImageScaledHeight, Xpos, Ypos, sysImage, imageFormat, SacleType, DockToTop);
            sysImage.Dispose();
        }

        public static void EmbedImageInPDF(string OrgPDFFileName, 
                                           string NewPDFFileName, 
                                           int PageNum,
                                           float ImageScaledWidth,
                                           float ImageScaledHeight,
                                           float Xpos,
                                           float Ypos, 
                                           System.Drawing.Image sysImage,
                                           ImageFormat imageFormat,
                                           enmSacleType SacleType,
                                           bool DockToTop)
        {
            iTextSharp.text.Image itextImage = iTextSharp.text.Image.GetInstance(sysImage, imageFormat);

            EmbedImageInPDF(OrgPDFFileName, NewPDFFileName, PageNum, ImageScaledWidth, ImageScaledHeight, Xpos, Ypos, itextImage, SacleType, DockToTop);
        }

        public enum enmSacleType
        {
            Absolute,
            FitToScale,
            FitToImage,
            NoAdjustment

        }

        public static void EmbedImageInPDF(string OrgPDFFileName,
                                                   string NewPDFFileName,
                                                   int PageNum,
                                                   float ImageScaledWidth,
                                                   float ImageScaledHeight,
                                                   float Xpos,
                                                   float Ypos,
                                                   iTextSharp.text.Image itextImage,
                                                   enmSacleType SacleType,
                                                   bool DockToTop)
        {
            // Open the PDF file to be signed
            PdfReader pdfReader = new PdfReader(OrgPDFFileName);
            PdfStamper pdfStamper = null;

            // Output stream to write the stamped PDF to
            using (FileStream outStream = new FileStream(NewPDFFileName, FileMode.Create))
            {
                try
                {
                    // Stamper to stamp the PDF with a signature
                    pdfStamper = new PdfStamper(pdfReader, outStream);

                    PdfContentByte over = pdfStamper.GetOverContent(PageNum);

                    float MaxPageHeight = over.PdfDocument.Top + over.PdfDocument.TopMargin;
                    float MaxPageWidth = over.PdfDocument.Right + over.PdfDocument.RightMargin;

                    // Scale image
                    switch (SacleType)
                    {
                        case enmSacleType.Absolute:
                            itextImage.ScaleAbsolute(ImageScaledWidth, ImageScaledHeight);
                            break;
                        case enmSacleType.FitToScale:
                            itextImage.ScaleToFit(ImageScaledWidth, ImageScaledHeight);
                            break;
                        case enmSacleType.FitToImage:
                            if (itextImage.PlainWidth + 2 * Xpos < MaxPageWidth)
                                ImageScaledWidth = itextImage.PlainWidth;
                            else
                                ImageScaledWidth = MaxPageWidth - 2 * Xpos;

                            if (itextImage.PlainHeight + 2 * Ypos < MaxPageHeight)
                                ImageScaledHeight = itextImage.PlainHeight;
                            else
                                ImageScaledHeight = MaxPageHeight - 2 * Ypos;

                            itextImage.ScaleToFit(ImageScaledWidth, ImageScaledHeight);
                            break;
                        default:
                            break;
                    }

                    if (DockToTop)
                        Ypos = MaxPageHeight - Ypos - itextImage.ScaledHeight;

                    // Set signature position on page
                    itextImage.SetAbsolutePosition(Xpos, Ypos);

                    // Add signatures to desired page
                    over.AddImage(itextImage);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "-" + ex.StackTrace);
                }
                finally
                {
                    // Clean up
                    if (pdfStamper != null)
                        pdfStamper.Close();

                    if (pdfReader != null)
                        pdfReader.Close();
                }
            }
        }

        public struct ImageDetails
        {
            public int ScaledWidth;
            public int ScaledHeight;
            public int Xpos;
            public int Ypos;
            public string Base64;
            public ImageFormat Format;
            public string userHeader;
            public string userComments;
            public enmSacleType SacleType;
            public bool DockToTop;
        }

        public static void AddImagesToPDF(string OrgPDFFileName, string NewPDFFileName, List<ImageDetails> Images, bool AddToEmptyPDF = false)
        {
            int ImageCount = 0;
            string SourceFileName = OrgPDFFileName;
            string DestFileName;
            
            foreach (ImageDetails Image in Images)
            {
                ImageCount++;
                DestFileName = (Images.Count == ImageCount) ? NewPDFFileName : Path.GetTempFileName();

                AddImageToPDF(SourceFileName, 
                              DestFileName, 
                              Image.ScaledWidth, 
                              Image.ScaledHeight, 
                              Image.Xpos, 
                              Image.Ypos, 
                              Image.Base64, 
                              Image.Format, 
                              Image.userHeader, 
                              Image.userComments,
                              Image.SacleType,
                              Image.DockToTop,
                              (ImageCount == 1 && AddToEmptyPDF)? 1 : (int?)null);

                SourceFileName = DestFileName;
            }
        }

        public static void AddImagesToPDF(string OrgPDFFileName, string NewPDFFileName, List<string> ImagesPaths, bool AddToEmptyPDF = false)
        {
            int ImageCount = 0;
            string SourceFileName = OrgPDFFileName;
            string DestFileName;

            foreach (var file in ImagesPaths)
            {
                ImageCount++;
                DestFileName = (ImagesPaths.Count == ImageCount) ? NewPDFFileName : Path.GetTempFileName();

                var Image = new clsImagesConverter.ImageDetails()
                {
                    Base64 = clsImagesConverter.ImageToBase64(file, ImageFormat.Jpeg),
                    Format = ImageFormat.Jpeg,
                    ScaledHeight = 0,
                    ScaledWidth = 0,
                    userComments = "",
                    userHeader = "",
                    Xpos = 0,
                    Ypos = 0,
                    SacleType = clsImagesConverter.enmSacleType.FitToImage,
                    DockToTop = true
                };

                AddImageToPDF(SourceFileName,
                    DestFileName,
                    Image.ScaledWidth,
                    Image.ScaledHeight,
                    Image.Xpos,
                    Image.Ypos,
                    Image.Base64,
                    Image.Format,
                    Image.userHeader,
                    Image.userComments,
                    Image.SacleType,
                    Image.DockToTop,
                    (ImageCount == 1 && AddToEmptyPDF) ? 1 : (int?)null);

                SourceFileName = DestFileName;
            }
        }

        public static void AddImageToPDF(string OrgPDFFileName, 
                                         string NewPDFFileName,
                                         float ImageScaledWidth,
                                         float ImageScaledHeight,
                                         float Xpos,
                                         float Ypos, 
                                         string imageBase64,
                                         ImageFormat ImageFormat, 
                                         string userHeader,
                                         string userComments,
                                         enmSacleType SacleType,
                                         bool DockToTop,
                                         int? PageNum = null)
        {
            string TempFileName;
            int NewPageNum;
            //int NewPageNum = PageNum.HasValue? PageNum.Value : AddCommentsToFile(OrgPDFFileName, TempFileName, userHeader, userComments);

            if (PageNum.HasValue)
            {
                NewPageNum = PageNum.Value;
                TempFileName = OrgPDFFileName;
            }
            else
            {
                TempFileName = Path.GetTempFileName();
                NewPageNum = AddCommentsToFile(OrgPDFFileName, TempFileName, userHeader, userComments);
            }


            EmbedImageInPDF(TempFileName, NewPDFFileName, NewPageNum, ImageScaledWidth, ImageScaledHeight, Xpos, Ypos, imageBase64, ImageFormat, SacleType, DockToTop);

        }

        private static int AddCommentsToFile(string inputFileName, string outputFileName, string userHeader, string userComments)
        {
            //string outputFileName = Path.GetTempFileName();
            //Step 1: Create a Docuement-Object
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            int NumOfPageas;

            try
            {
                //Step 2: we create a writer that listens to the document
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputFileName, FileMode.Create));

                //Step 3: Open the document
                document.Open();

                PdfContentByte cb = writer.DirectContent;

                //The current file path
                string filename = inputFileName;

                // we create a reader for the document
                PdfReader reader = new PdfReader(filename);

                for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                {
                    document.SetPageSize(reader.GetPageSizeWithRotation(1));
                    document.NewPage();

                    //Insert to Destination on the first page
                    if (pageNumber == 1)
                    {
                        iTextSharp.text.Chunk fileRef = new iTextSharp.text.Chunk(" ");
                        fileRef.SetLocalDestination(filename);
                        document.Add(fileRef);
                    }

                    PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                    int rotation = reader.GetPageRotation(pageNumber);
                    if (rotation == 90 || rotation == 270)
                    {
                        cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                    }
                    else
                    {
                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }

                // Add a new page to the pdf file
                document.NewPage();
                NumOfPageas = writer.CurrentPageNumber;

                iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA
                                          , 15
                                          , iTextSharp.text.Font.BOLD
                                          , iTextSharp.text.Color.BLACK
                    );
                iTextSharp.text.Chunk titleChunk = new iTextSharp.text.Chunk(userHeader, titleFont);
                paragraph.Add(titleChunk);
                document.Add(paragraph);

                paragraph = new iTextSharp.text.Paragraph();
                iTextSharp.text.Font textFont = new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA
                                         , 12
                                         , iTextSharp.text.Font.NORMAL
                                         , iTextSharp.text.Color.BLACK
                    );
                iTextSharp.text.Chunk textChunk = new iTextSharp.text.Chunk(userComments, textFont);
                paragraph.Add(textChunk);

                document.Add(paragraph);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                document.Close();
            }
            return NumOfPageas;
        }

        public static void GeneratePDFFromTiff(string tiffFullName, 
                                        string destPDFFullName, 
                                        string basePDFFullName)
        {
            GeneratePDFFromTiff(tiffFullName, destPDFFullName, basePDFFullName, null);
        }

        public static void GeneratePDFFromTiff(string tiffFullName, 
                                               string destPDFFullName, 
                                               string basePDFFullName,
                                               int? maxPages)
        {
            if (maxPages.HasValue)
            {
                if (maxPages <= 0)
                {
                    File.Copy(basePDFFullName, destPDFFullName, true);
                    return;
                }
            }

            List<Image> imagesTiff = clsMulitiff.GetAllPages(tiffFullName);
            
            List<clsImagesConverter.ImageDetails> Images = new List<clsImagesConverter.ImageDetails>();

            int PageCount = 0;

            foreach (var imageTiff in imagesTiff)
            {
                if (maxPages.HasValue)
                {
                    if (PageCount >= maxPages)
                        break;
                    else
                        PageCount++;
                }

                string Base64 = ImageToBase64(imageTiff, ImageFormat.Bmp);
                if (!string.IsNullOrWhiteSpace(Base64))
                    Images.Add(new clsImagesConverter.ImageDetails()
                    {
                        Base64 = Base64,
                        Format = ImageFormat.Bmp,
                        ScaledHeight = 0,
                        ScaledWidth = 0,
                        userComments = "",
                        userHeader = "",
                        Xpos = 30,
                        Ypos = 30,
                        SacleType = clsImagesConverter.enmSacleType.FitToImage,
                        DockToTop = true
                    }); 
            }

            AddImagesToPDF(basePDFFullName, destPDFFullName, Images, true);

        }
        #endregion

        #region Embed text In PDF
        public struct TextPart
        {
            public string Text;
            public float X;
            public float Y;
            public float Rotation;
            public bool RightToLeft;
            public System.Drawing.Color? FontColor;
        }


        public static void EmbedTextInPDF(string oldPDFFile, 
                                         string newPDFFile, 
                                         TextPart textPart,
                                         System.Drawing.Color fontColor,
                                         float fontSize)
        {
            EmbedTextInPDF(oldPDFFile, newPDFFile, textPart, fontColor, fontSize, GetFontFile());
        }

        public static void EmbedTextInPDF(string oldPDFFile, 
                                          string newPDFFile, 
                                          TextPart textPart,
                                          System.Drawing.Color fontColor,
                                          float fontSize,
                                          string fontFile)
        {
            List<TextPart> TextParts = new List<TextPart>();
            TextParts.Add(textPart);

            EmbedTextInPDF(oldPDFFile, newPDFFile, TextParts, fontColor, fontSize, fontFile);
        }

        public static void EmbedTextInPDF(string oldPDFFile, 
                                          string newPDFFile, 
                                          List<TextPart> textParts,
                                          System.Drawing.Color fontColor,
                                          float fontSize)
        {
            EmbedTextInPDF(oldPDFFile, newPDFFile, textParts, fontColor, fontSize, GetFontFile());
        }

        public static void EmbedTextInPDF(string oldPDFFile,
                                            string newPDFFile,
                                            List<TextPart> textParts,
                                            System.Drawing.Color fontColor,
                                            float fontSize,
                                            string fontFile)
        {

            

            // open the reader
            PdfReader reader = new PdfReader(oldPDFFile);
            iTextSharp.text.Rectangle size = reader.GetPageSizeWithRotation(1);
            iTextSharp.text.Document document = new iTextSharp.text.Document(size);

            // open the writer
            FileStream fs = new FileStream(newPDFFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            // the pdf content
            PdfContentByte cb = writer.DirectContent;
            BaseFont bf;

            if (string.IsNullOrWhiteSpace(fontFile))
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            else
                bf = BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            //iTextSharp.text.Color iTextFontColor = GetItextFontColor(fontColor);
            //cb.SetColorFill(iTextFontColor);
            cb.SetFontAndSize(bf, fontSize);

            int i = 0;
            foreach (TextPart textPart in textParts)
            {
                i++;

                if (textPart.FontColor.HasValue)
                    cb.SetColorFill(GetItextFontColor(textPart.FontColor.Value));
                else
                    cb.SetColorFill(GetItextFontColor(fontColor));

                // write the text in the pdf content
                cb.BeginText();
                string text = textPart.RightToLeft ? Reverse(textPart.Text) : textPart.Text;
                // put the alignment and coordinates here
                cb.ShowTextAligned(i, text, textPart.X, textPart.Y, textPart.Rotation);
                cb.EndText();
            }

            // create the new page and add it to the pdf
            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            cb.AddTemplate(page, 0, 0);

            // close the streams and voilá the file should be changed :)
            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();
        }

        private static iTextSharp.text.Color GetItextFontColor(System.Drawing.Color fontColor)
        {
            if (fontColor == System.Drawing.Color.White)
                return iTextSharp.text.Color.WHITE;
            if (fontColor == System.Drawing.Color.LightGray)
                return iTextSharp.text.Color.LIGHT_GRAY;
            if (fontColor == System.Drawing.Color.Gray)
                return iTextSharp.text.Color.GRAY;
            if (fontColor == System.Drawing.Color.DarkGray)
                return iTextSharp.text.Color.DARK_GRAY;
            if (fontColor == System.Drawing.Color.Black)
                return iTextSharp.text.Color.BLACK;
            if (fontColor == System.Drawing.Color.Red)
                return iTextSharp.text.Color.RED;
            if (fontColor == System.Drawing.Color.Pink)
                return iTextSharp.text.Color.PINK;
            if (fontColor == System.Drawing.Color.Orange)
                return iTextSharp.text.Color.ORANGE;
            if (fontColor == System.Drawing.Color.Yellow)
                return iTextSharp.text.Color.YELLOW;
            if (fontColor == System.Drawing.Color.Green)
                return iTextSharp.text.Color.GREEN;
            if (fontColor == System.Drawing.Color.Magenta)
                return iTextSharp.text.Color.MAGENTA;
            if (fontColor == System.Drawing.Color.Cyan)
                return iTextSharp.text.Color.CYAN;
            if (fontColor == System.Drawing.Color.Blue)
                return iTextSharp.text.Color.BLUE;

            throw new Exception(string.Format("clsImagesConverter.GetItextFontColor: Color: '{0}' is not supported", fontColor.ToString()));
        }

        internal static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static string GetFontFile()
        {
            string FontFile = Path.Combine(Environment.CurrentDirectory, "ARIAL.TTF");
            if (!File.Exists(FontFile))
                throw new Exception(string.Format("clsImagesConverter:GetFontFile - font file: '{0}' does not exist", FontFile));

            return FontFile;

        }
        #endregion

        /// <summary>
        /// Converts tiff image(s) to jpeg image(s).
        /// </summary>
        /// <param name="fileName">Full name to tiff image.</param>
        /// <param name="destDir">Destination directory to jpeg image.[Optional]</param>
        /// <returns>String array having full name to jpeg images.</returns>
        public static Image ConvertTiffToJpeg(string fileName,string destDir = "", int qualityLevel = 0)
        {
            destDir = destDir != "" ? destDir : Path.GetDirectoryName(fileName);
            Image myImage = Image.FromFile(fileName);
            Graphics mygraphics = Graphics.FromImage(myImage);

            mygraphics.SmoothingMode = SmoothingMode.AntiAlias;
            mygraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mygraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            ImageCodecInfo[] codecsarray = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo myCodec = null;

            myCodec = GetEncoder(ImageFormat.Jpeg);

            EncoderParameters myEncoderParameters = new EncoderParameters();

            
            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)qualityLevel);

            using (myImage)
            {
                FrameDimension frameDimensions = new FrameDimension(
                    myImage.FrameDimensionsList[0]);

                // Gets the number of pages from the tiff image (if multipage)
                int frameNum = myImage.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];

                for (int frame = 0; frame < frameNum; frame++)
                {
                    // Selects one frame at a time and save as jpeg.
                    myImage.SelectActiveFrame(frameDimensions, frame);
                    using (Bitmap bmp = new Bitmap(myImage))
                    {
                        jpegPaths[frame] = String.Format("{0}\\{1}{2}.jpg",
                            //Path.GetDirectoryName(fileName),
                            destDir,
                            Path.GetFileNameWithoutExtension(fileName),
                            "_P" + (frame + 1));
                        bmp.Save(jpegPaths[frame], myCodec, myEncoderParameters);
                    }
                }
                return myImage;
            }
        }



        /// <summary>
        /// Converts jpeg image(s) to tiff image(s).
        /// </summary>
        /// <param name="fileNames">String array having full name to jpeg images.</param>
        /// <param name="isMultipage">true to create single multipage tiff file otherwise, false.</param>
        /// <param name="destDir">Destination directory to jpeg image.[Optional].</param>
        /// <param name="compressLevel">Tiff image compression level (CCITT3,CCITT4,LZW,Packbits,JPEG,JBIG2,JPEG2000).[Optional].</param>
        /// <param name="qualityLevel">Tiff image quality level (20 - 100) [Optional].</param>
        /// <returns>String array having full name to tiff images.</returns>
        public static Image ConvertJpegToTiff(string[] fileNames, bool isMultipage, string destDir = "", string compressLevel = "", int qualityLevel = 0)
        {
            bool bIsDest = destDir != "" ? true : false;
            string tifPaths = null;
            int paramsConut = 0;
            int paramNum = 0;
            Image tiffImg = null;
                

            // Check if destDir exist, if not set bIsDest to false
            if (destDir != "" && !Directory.Exists(destDir))
            {
                bIsDest = false;
            }

            paramsConut += compressLevel != "" ? 1 : 0;
            if (compressLevel != "JPEG")
                paramsConut += qualityLevel > 0 ? 1 : 0;
            paramsConut += isMultipage ? 1 : 0;

            EncoderParameters encoderParams = new EncoderParameters(paramsConut);


            ImageCodecInfo tiffCodecInfo = ImageCodecInfo.GetImageEncoders()
                .First(ie => ie.MimeType == "image/tiff");

                

            //// Save the bitmap as a TIFF file with Other compressions.
            if (compressLevel != "")
            {
                switch (compressLevel)
                {
                    case "CCITT3":
                        encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression,
                                                                        (long) EncoderValue.CompressionCCITT3);
                        break;
                    case "CCITT4":
                        encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression,
                                                                        (long) EncoderValue.CompressionCCITT4);
                        break;
                    case "LZW":
                        encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression,
                                                                        (long) EncoderValue.CompressionLZW);
                        break;
                    case "Packbits":
                        encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionRle);
                        break;
                    case "BMP":
                        encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionNone);
                        break;
                    //case "JPEG":
                    //    //tiffCodecInfo = GetEncoder(ImageFormat.Jpeg);
                    //    //encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Quality, 90L);
                    //    encoderParams.Param[paramNum] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.);
                    //    break;
                        //case "JBIG2":

                        //    break;
                        //case "JPEG2000":

                        //    break;
                    default:
                        break;
                }
                paramNum++;
            }

            //// Save the bitmap as a TIFF file with Other Quality.
            if (qualityLevel > 0)
            {
                Encoder myEncoder = Encoder.Quality;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)qualityLevel);
                encoderParams.Param[paramNum] = myEncoderParameter;
                paramNum++;
            }

            //TiffBitmapEncoder myEncoder = new TiffBitmapEncoder();
            //myEncoder.Compression = TiffCompressOption.Zip;

            string[] tiffPaths = null;
            if (isMultipage)
            {
                tiffPaths = new string[1];
                Image tiffTmpImg = null;
                
                try
                {
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        if (i == 0)
                        {
                            tiffPaths[i] = String.Format("{0}\\{1}.tif",
                                tifPaths = bIsDest ? destDir : Path.GetDirectoryName(fileNames[i]),
                                Path.GetFileNameWithoutExtension(fileNames[i]));

                            // Initialize the first frame of multipage tiff.
                            tiffTmpImg = Image.FromFile(fileNames[i]);
                            //Bitmap myBitmap = new Bitmap();
                            encoderParams.Param[paramNum] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.MultiFrame);

                                

                            tiffTmpImg.Save(tiffPaths[i], tiffCodecInfo, encoderParams);
                        }
                        else
                        {
                            // Add additional frames.
                            encoderParams.Param[paramNum] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);

                            using (Image frame = Image.FromFile(fileNames[i]))
                            {
                                tiffTmpImg.SaveAdd(frame, encoderParams);
                            }
                        }

                        if (i == fileNames.Length - 1)
                        {
                            // When it is the last frame, flush the resources and closing.
                            encoderParams.Param[paramNum] = new EncoderParameter(
                                Encoder.SaveFlag, (long)EncoderValue.Flush);

                            tiffTmpImg.SaveAdd(encoderParams);
                        }
                    }

                    tiffImg = tiffTmpImg;
                }
                finally
                {
                    if (tiffTmpImg != null)
                    {
                        tiffTmpImg.Dispose();
                        tiffTmpImg = null;
                    }
                }
            }
            else
            {
                tiffPaths = new string[fileNames.Length];

                for (int i = 0; i < fileNames.Length; i++)
                {
                    tiffPaths[i] = String.Format("{0}\\{1}.tif",
                        tifPaths = bIsDest ? destDir : Path.GetDirectoryName(fileNames[i]),
                        Path.GetFileNameWithoutExtension(fileNames[i]));

                    // Save as individual tiff files.
                    using (Image tiffTmpImg = Image.FromFile(fileNames[i]))
                    {
                        if (paramsConut == 0)
                        {
                            Bitmap bitMapTiff = new Bitmap(tiffTmpImg);
                            tiffTmpImg.Save(tiffPaths[i], ImageFormat.Tiff);
                            //myEncoder.Frames.Add(BitmapFrame.Create(bitMapTiff));
                        }
                        else
                        {
                            tiffTmpImg.Save(tiffPaths[i], tiffCodecInfo, encoderParams);
                        }

                        tiffImg = tiffTmpImg;
                    }
                }
            }

            return tiffImg;
        }


        /// <summary>
        /// Change JPEG compression level (Jpeg to Jpeg)
        /// </summary>
        /// <param name="fullFileName">String of full name to jpeg image.</param>
        /// <param name="destDir">String of full path to jpeg image.[Optional]</param>
        /// <returns>return processed image</returns>
        public static Image ChangeJPEGQualityLevel(string fullFileName, string destDir = "", int qualityLevel = 0)
        {
            string sourcePath = Path.GetDirectoryName(fullFileName);
            string fileName = Path.GetFileNameWithoutExtension(fullFileName);
            destDir = (destDir != "" ? destDir : sourcePath) + @"\";

            // Get a bitmap.
            Image myImage = Image.FromFile(fullFileName);
            Bitmap myBitmap = new Bitmap(myImage);
            Graphics myGraphics = Graphics.FromImage(myImage);
            
            myGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            myGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            myGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            ImageCodecInfo[] codecsarray = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo myCodec = null;

            myCodec = GetEncoder(ImageFormat.Jpeg);

            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters();

            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)qualityLevel);
            myBitmap.Save(destDir + fileName + "_" + qualityLevel + "L.jpg", myCodec, myEncoderParameters);

            myImage = myBitmap;

            return myImage;
        }

        /// <summary>
        /// Change JPEG to Grayscale (RGB to GrayScale)
        /// </summary>
        /// <param name="fullFileName">String of full name to jpeg image.</param>
        /// <param name="destDir">String of full path to jpeg image.[Optional]</param>
        /// <returns>return processed image</returns>
        public static Image ConverImage2Grayscale(string fullFileName, string destDir = "")
        {
            string sourcePath = Path.GetDirectoryName(fullFileName);
            string fileName = Path.GetFileNameWithoutExtension(fullFileName);
            destDir = (destDir != "" ? destDir : sourcePath) + @"\";

            // Get a bitmap.
            Image myImage = Image.FromFile(fullFileName);
            
            //BitmapPalette 
            //Image newImage = new Image();
            //for (i=0; i<myImage.Width-1; i++)
            //{
            //    for (j=0; j<myImage.Height; j++)
            //    {
            //        Color p = myImage[i,j];
            //        byte gray = (p.R+p.G+p.B)/3;
            //        new[i,j] = new Color(gray,gray,gray);
            //    }
            // }



            Bitmap myBitmap = new Bitmap(myImage);
            Graphics myGraphics = Graphics.FromImage(myImage);

            myGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            myGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            myGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            ImageCodecInfo[] codecsarray = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo myCodec = null;

            myCodec = GetEncoder(ImageFormat.Jpeg);

            //// Create an EncoderParameters object. 
            //// An EncoderParameters object has an array of EncoderParameter 
            //// objects. In this case, there is only one 
            //// EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters();

            //myBitmap.Save(destDir + fileName + "_Gr.jpg", myCodec, myEncoderParameters);

            myImage = myBitmap;

            return myImage;
        }


        /// <summary>
        /// change the rotation of source image (Angle) 
        /// </summary>
        /// <param name="source">the source file</param>
        /// <param name="destantion">destantion file</param>
        /// <param name="destantion">the angle of rotation</param>
        /// <returns>return processed image</returns>
        public static Image Rotation(string source, string destantion, int angle)
        {
            Image myImage = Image.FromFile(source);
            Bitmap myBitmap = new Bitmap(myImage.Width, myImage.Height);

            Graphics mygraphics = Graphics.FromImage(myBitmap);
            mygraphics.TranslateTransform((float)myBitmap.Width / 2, (float)myBitmap.Height / 2);
            mygraphics.RotateTransform(angle);
            mygraphics.TranslateTransform(-(float)myBitmap.Width / 2, -(float)myBitmap.Height / 2);
            mygraphics.DrawImage(myImage, new Point(0, 0));

            if (destantion != "")
                myBitmap.Save(destantion, ImageFormat.Jpeg);

            myImage = myBitmap;

            return myImage;
        }



        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static void MergePdf(string[] pSource_files, String pResult)
        {
            String[] source_files = pSource_files;
            String result = pResult;

            //create Document object
            var document = new iTextSharp.text.Document();

            //create PdfCopy object
            var copy = new PdfCopy(document, new FileStream(result, FileMode.Create));

            
            //open the document
            document.Open();

            //PdfReader variable
            PdfReader reader;
            
            for (int i = 0; i < source_files.Length; i++)
            {
                //create PdfReader object
                reader = new PdfReader(new RandomAccessFileOrArray(source_files[i], true), null);
                //merge combine pages
                for (int page = 1; page <= reader.NumberOfPages; page++)
                    copy.AddPage(copy.GetImportedPage(reader, page));

                reader.Close();
            }

            document.Close();

        }
    }
}
