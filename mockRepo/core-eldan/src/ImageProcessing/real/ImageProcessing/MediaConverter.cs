using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
// using static System.Net.Mime.MediaTypeNames;

namespace Eldan.ImageProcessing472
{
    public class MediaConverter
    {
        public void ConvertMedia(string OriginalMediaFullName, string ConvertedMediaFullName)
        {
            FileInfo info = new FileInfo(OriginalMediaFullName);
            using (MagickImage image = new MagickImage(info.FullName))
            {
                image.Write(ConvertedMediaFullName);
            }
        }
    }
}
