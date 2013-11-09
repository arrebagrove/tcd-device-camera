using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using TCD.Device.Camera.Barcodes;

namespace TCD.Device.Camera.Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private async void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            CodeScannerPopup scp = new CodeScannerPopup(SystemTray.IsVisible, Format.ALL_1D);
            //important: hide ApplicationBar and handle BackKeyPress
            ApplicationBar.IsVisible = false;
            this.BackKeyPress += scp.HandleBackKeyPress;
            //now let's go
            ScanResult r = await scp.ShowAsync("BARCODE SCANNER", "scan");
            if (r != null)
                output.Text = string.Format("{0}\n{1}", r.Text, r.Format.ToString());
            //clean up
            this.BackKeyPress -= scp.HandleBackKeyPress;
            ApplicationBar.IsVisible = true;
        }
        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            qrCode.Source = Encoder.GenerateQRCode(input.Text, 400);
        }
    }
}