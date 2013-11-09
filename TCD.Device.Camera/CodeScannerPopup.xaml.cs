using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Devices;
using TCD.Device.Camera.Barcodes;

namespace TCD.Device.Camera
{
    /// <summary>
    /// Provides a control for reverse-autocompletition.
    /// </summary>
    public partial class CodeScannerPopup : UserControl
    {
        /// <summary>
        /// Is the control opened?
        /// </summary>
        public bool IsOpen
        {
            get { return popup.IsOpen; }
            private set { popup.IsOpen = value; }
        }

        private Task scanTask;
        private bool cancel;
        private ScanResult result;

        private PhotoCamera Camera;
        private Decoder decoder;
        
        
        /// <summary>
        /// Make a new QRScannerPopup
        /// </summary>
        /// <param name="withSystemTray">Do we need a margin at the top?</param>
        /// TODO: hints as parameter
        public CodeScannerPopup(bool withSystemTray)
        {
            Init(withSystemTray);
        }
        /// <summary>
        /// Make a new QRScannerPopup
        /// </summary>
        /// <param name="withSystemTray">Do we need a margin at the top?</param>
        /// <param name="formats">Scan for these Formats</param>
        public CodeScannerPopup(bool withSystemTray, params Format[] formats)
        {
            Init(withSystemTray, formats);
        }
        private void Init(bool withSystemTray, params Format[] formats)
        {
            InitializeComponent();
            if (withSystemTray)
            {
                LayoutRoot.Height = 768;
                LayoutRoot.Margin = new Thickness(0, 32, 0, 0);
            }
            decoder = new Decoder(formats);
        }
                
        /// <summary>
        /// Show the CodeScannerPopup and look for something scannable
        /// </summary>
        /// <param name="appTitle">Your app title</param>
        /// <param name="pageTitle">The page title</param>
        /// <returns>ScanResult or null (BackKeyPress/close())</returns>
        public async Task<ScanResult> ShowAsync(string appTitle, string pageTitle = "scan")
        {
            result = null;
            //set up ui
            ApplicationTitle.Text = appTitle;
            PageTitle.Text = pageTitle;
            Camera = new PhotoCamera(CameraType.Primary);//use the camera we were given
            Camera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(Camera_Initialized);
            previewVideo.SetSource(Camera);
            previewTransform.Rotation = 90;
            //initialize camera
            scanTask = new Task(new Action(delegate
            {
                while(!cancel)
                {
                    Scan();
                }
            }));//start scanning
            scanTask.Start();
            //show us
            this.IsOpen = true;
            //wait for the scan to complete
            await scanTask;
            Dispose();//release resources and hide
            this.IsOpen = false;//hide
            return result;
        }
        private void Camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(Camera.PreviewResolution.Width);
            int height = Convert.ToInt32(Camera.PreviewResolution.Height);
            decoder.Init(width, height);
        }
        private void Dispose()
        {
            decoder.Dispose();
            Camera.Dispose();//release resource
        }

        private void Scan()
        {
            try
            {                
                Camera.GetPreviewBufferY(decoder.PreviewBufferY);//get the image
                result = decoder.Decode();//try to decode
                cancel = true;//signal success
            }
            catch//decode was not successful
            { }
        }
        
        /// <summary>
        /// Handles the BackKeyPress for you. (this.BackKeyPress += reverseAutocompletePopup.HandleBackKeyPress)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleBackKeyPress(object sender, CancelEventArgs e)
        {
            if (IsOpen)
            {
                e.Cancel = true;
                cancel = true;//signal cancelling of the scan operation
            }
        }
       
        /// <summary>
        /// Close the popup. Results in the result being null
        /// </summary>
        public void Close()
        {
            cancel = true;
        }
    }
}
