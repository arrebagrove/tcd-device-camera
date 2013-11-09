
namespace TCD.Device.Camera.Barcodes
{    
    public class ScanResult
    {
        public Format Format { get; set; }
        public string Text { get; set; }

        public ScanResult(Format format, string text)
        {
            Format = format;
            Text = text;
        }
    }
    public enum Format { ALL_1D, CODE_128, CODE_39, DATAMATRIX, EAN_13, EAN_8, ITF, PDF417, QR_CODE, UPC_A, UPC_E, UPC_EAN }
}
