using System.Windows.Media.Imaging;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode;

namespace TCD.Device.Camera.Barcodes
{
    public class Encoder
    {
        /// <summary>
        /// Generate a BitmapSource QRcode from a string.
        /// </summary>
        /// <param name="content">The string to encode</param>
        /// <param name="size">The width/height of the image (in pixels).</param>
        /// <returns>BitmapSource of the QR code</returns>
        public static BitmapSource GenerateQRCode(string content, int size)
        {
            //create buffered image to draw to
            WriteableBitmap writeableBitmap = new WriteableBitmap(size, size);
            if (string.IsNullOrWhiteSpace(content))
                return writeableBitmap;
            //generate qr code
            QRCodeWriter writer = new QRCodeWriter();
            ByteMatrix byteMatrix = writer.encode(content, BarcodeFormat.QR_CODE, size, size);
            sbyte[][] array = byteMatrix.Array;
            //iterate through the matrix and draw the pixels
            int grayValue;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    grayValue = array[y][x] & 0xff;
                    writeableBitmap.Pixels[y * size + x] = 255 << 24 | grayValue << 16 | grayValue << 8 | grayValue;
                }
            writeableBitmap.Invalidate();
            return writeableBitmap;
        }
    }
}
