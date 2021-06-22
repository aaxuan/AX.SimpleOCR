using System.Drawing;
using System.Drawing.Imaging;

namespace AX.SimpleOCR.OCRProvider
{
    public abstract class BaseOCRProvider
    {
        public abstract string OCR(Image image);

        internal static void SaveToTempFile(Image image)
        {
            image.Save("ocr_temporaryfile.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        internal static byte[] ImageToByte(Image image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.GetBuffer();
            }
        }
    }
}