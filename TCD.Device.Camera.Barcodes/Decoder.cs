using System;
using System.Collections.Generic;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode;

namespace TCD.Device.Camera.Barcodes
{
    public class Decoder
    {
        /// <summary>
        /// It's essential, that this property is set before Decoder.Decode() is called:
        /// PhotoCamera.GetPreviewBufferY(PreviewBufferY);
        /// </summary>
        public byte[] PreviewBufferY
        {
            get { return luminance.PreviewBufferY; }
            set { Array.Copy(value, luminance.PreviewBufferY, value.Length); }
        }

        private PhotoCameraLuminanceSource luminance;
        private QRCodeReader reader;
        private HybridBinarizer binarizer;
        private BinaryBitmap binBitmap;
        private Dictionary<object, object> hints = new Dictionary<object, object>();

        /// <summary>
        /// Set up a Decoder, specify possible formats
        /// </summary>
        /// <param name="formats">Possible formats to decode</param>
        public Decoder(params Format[] formats)
        {
            reader = new QRCodeReader();
            foreach (Format format in formats)
                hints.Add(DecodeHintType.POSSIBLE_FORMATS, ConvertFormat(format));
        }

        /// <summary>
        /// Provide width and height (PhotoCamera.PreviewResolution)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Init(int width, int height)
        {
            luminance = new PhotoCameraLuminanceSource(width, height);
        }

        private BarcodeFormat ConvertFormat(Format f)
        {
            switch (f)
            {
                case Format.ALL_1D: return BarcodeFormat.ALL_1D;
                case Format.CODE_128: return BarcodeFormat.CODE_128;
                case Format.CODE_39: return BarcodeFormat.CODE_39;
                case Format.DATAMATRIX: return BarcodeFormat.DATAMATRIX;
                case Format.EAN_13: return BarcodeFormat.EAN_13;
                case Format.EAN_8: return BarcodeFormat.EAN_8;
                case Format.ITF: return BarcodeFormat.ITF;
                case Format.PDF417: return BarcodeFormat.PDF417;
                case Format.QR_CODE: return BarcodeFormat.QR_CODE;
                case Format.UPC_A: return BarcodeFormat.UPC_A;
                case Format.UPC_E: return BarcodeFormat.UPC_E;
                case Format.UPC_EAN: return BarcodeFormat.UPC_EAN;
            }
            return null;
        }
        private Format ConvertFormat(BarcodeFormat f)
        {
            if (f == BarcodeFormat.ALL_1D) return Format.ALL_1D;
            if (f == BarcodeFormat.CODE_128) return Format.CODE_128;
            if (f == BarcodeFormat.CODE_39) return Format.CODE_39;
            if (f == BarcodeFormat.DATAMATRIX) return Format.DATAMATRIX;
            if (f == BarcodeFormat.EAN_13) return Format.EAN_13;
            if (f == BarcodeFormat.EAN_8) return Format.EAN_8;
            if (f == BarcodeFormat.ITF) return Format.ITF;
            if (f == BarcodeFormat.PDF417) return Format.PDF417;
            if (f == BarcodeFormat.QR_CODE) return Format.QR_CODE;
            if (f == BarcodeFormat.UPC_A) return Format.UPC_A;
            if (f == BarcodeFormat.UPC_E) return Format.UPC_E;
            if (f == BarcodeFormat.UPC_EAN) return Format.UPC_EAN;
            return Format.QR_CODE;
        }

        /// <summary>
        /// Try to decode. Throws exception!!
        /// </summary>
        /// <returns>ScanResult if successful</returns>
        public ScanResult Decode()
        {
            binarizer = new HybridBinarizer(luminance);
            binBitmap = new BinaryBitmap(binarizer);
            Result r = reader.decode(binBitmap, hints);
            return new ScanResult(ConvertFormat(r.BarcodeFormat), r.Text);
        }

        /// <summary>
        /// Release all resources
        /// </summary>
        public void Dispose()
        {
            reader = null;
            binarizer = null;
            binBitmap = null;
        }
    }
}
