using DPUruNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ControlPenales.Clases
{
    public class FingerPrintScanner : ValidationViewModelBase, IDataErrorInfo
    {
        #region [Propiedades]
        private enum Action
        {
            UpdateReaderState,
            SendBitmap,
            SendMessage
        }

        private ImageSource _PropertyImage;
        public ImageSource PropertyImage
        {
            get { return _PropertyImage; }
            set
            {
                _PropertyImage = value;
                OnPropertyChanged("PropertyImage");
            }
        }

        private Fid _FingerPrintData;
        public Fid FingerPrintData
        {
            get { return _FingerPrintData; }
            set { _FingerPrintData = value; }
        }

        private bool reset;
        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }

        private Reader _CurrentReader;
        public Reader CurrentReader
        {
            get { return _CurrentReader; }
            set
            {
                _CurrentReader = value;
                SendMessage(Action.UpdateReaderState, value);
            }
        }

        private ReaderCollection _readers;
        public ReaderCollection Readers
        {
            get { return _readers != null ? _readers : ReaderCollection.GetReaders(); }
            set { _readers = value; }
        }

        private string _ScannerMessage;
        public string ScannerMessage
        {
            get { return _ScannerMessage; }
            set
            {
                _ScannerMessage = value.ToUpper();
                OnPropertyChanged("ScannerMessage");
            }
        }

        delegate void OnVerification(Window Window);

        private Thread _OnProgress;
        public Thread OnProgress
        {
            get { return _OnProgress; }
            set { _OnProgress = value; }
        }
        #endregion
        #region [Metodos]
        protected bool OpenReader()
        {
            try
            {
                reset = false;
                var result = Constants.ResultCode.DP_DEVICE_FAILURE;

                // Open reader
                result = _CurrentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

                if (result != Constants.ResultCode.DP_SUCCESS)
                {
                    //MessageBox.Show("Error:  " + result);
                    reset = true;
                    return false;
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
            return true;
        }

        protected bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            try
            {
                // Activate capture handler
                _CurrentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

                // Call capture
                if (!CaptureFingerAsync())
                {
                    return false;
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
            return true;
        }

        public virtual void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!CheckCaptureResult(captureResult)) return;

                // Create bitmap
                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    FingerPrintData = captureResult.Data;
                    SendMessage(Action.SendBitmap, CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
            }
            catch { }
        }

        private void SendMessage(Action action, object payload)
        {
            try
            {
                switch (action)
                {
                    case Action.SendMessage:
                        //MessageBox.Show((string)payload);
                        break;
                    case Action.SendBitmap:
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // You need to specify the image format to fill the stream. 
                            // I'm assuming it is PNG
                            ((Bitmap)payload).Save(memoryStream, ImageFormat.Bmp);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            // Make sure to create the bitmap in the UI thread
                            if (Dispatcher.CurrentDispatcher != Application.Current.Dispatcher)
                                PropertyImage = (BitmapSource)Application.Current.Dispatcher.Invoke(
                                    new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
                                    DispatcherPriority.Normal,
                                    memoryStream);

                            ScannerMessage = "Huella Capturada";
                            PropertyImage = CreateBitmapSourceFromBitmap(memoryStream);
                        }
                        break;
                    case Action.UpdateReaderState:
                        if ((Reader)payload != null)
                        {
                            //txtReaderSelected.Text = ((Reader)payload).Description.SerialNumber;
                            //btnCapture.Enabled = true;
                        }
                        else
                        {
                            ScannerMessage = "";
                            //btnCapture.Enabled = false;
                        }
                        break;
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
        }

        private bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();

                var captureResult = _CurrentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, _CurrentReader.Capabilities.Resolutions[0]);
                if (captureResult != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    throw new Exception("" + captureResult);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new ApplicationException(ex.Message);
            }
        }

        protected bool CheckCaptureResult(CaptureResult captureResult)
        {
            try
            {
                if (captureResult.Data == null)
                {
                    if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        reset = true;
                        throw new Exception(captureResult.ResultCode.ToString());
                    }

                    // Send message if quality shows fake finger
                    if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                    {
                        throw new Exception("Quality - " + captureResult.Quality);
                    }
                    return false;
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
            return true;
        }

        protected Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            Bitmap bmp;
            try
            {
                var rgbBytes = new byte[bytes.Length * 3];

                for (int i = 0; i <= bytes.Length - 1; i++)
                {
                    rgbBytes[(i * 3)] = bytes[i];
                    rgbBytes[(i * 3) + 1] = bytes[i];
                    rgbBytes[(i * 3) + 2] = bytes[i];
                }
                bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                for (int i = 0; i <= bmp.Height - 1; i++)
                {
                    var p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                    Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
                }

                bmp.UnlockBits(data);
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
            return bmp;
        }

        private void GetStatus()
        {
            try
            {
                Constants.ResultCode result = _CurrentReader.GetStatus();

                if ((result != Constants.ResultCode.DP_SUCCESS))
                {
                    if (CurrentReader != null)
                    {
                        CurrentReader.Dispose();
                        CurrentReader = null;
                    }
                    throw new Exception("" + result);
                }

                if ((_CurrentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
                {
                    Thread.Sleep(50);
                }
                else if ((_CurrentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
                {
                    _CurrentReader.Calibrate();
                }
                else if ((_CurrentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
                {
                    throw new Exception("Reader Status - " + _CurrentReader.Status.Status);
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
        }

        protected static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
        {
            WriteableBitmap writable;
            try
            {
                var bitmapDecoder = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);

                // This will disconnect the stream from the image completely...
                writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
                try
                {
                    writable.Freeze();
                }
                catch { }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
            return writable;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        protected static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        protected void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            try
            {
                if (_CurrentReader != null)
                {
                    // Dispose of reader handle and unhook reader events.
                    _CurrentReader.Dispose();

                    if (reset)
                    {
                        CurrentReader = null;
                    }
                }
            }
            catch (Exception ex) { throw new ApplicationException(ex.Message); }
        }

        protected void InvokeDelegate(Window Window)
        {
            Window.Dispatcher.BeginInvoke(new OnVerification(OnSucceed), Window);
        }

        public virtual void OnSucceed(Window Window) { }

        protected DataResult<Fmd> ExtractFmdfromBmp(Bitmap img)
        {
            byte[] imageByte = ExtractByteArray(img);
            //height, width and resolution must be same as those of image in ExtractByteArray
            return DPUruNet.FeatureExtraction.CreateFmdFromRaw(imageByte, 0, 1, img.Width, img.Height, 500, Constants.Formats.Fmd.ANSI);
        }

        protected byte[] ExtractBytefromBmp(Bitmap img)
        {
            byte[] imageByte = ExtractByteArray(img);


            var c = DPUruNet.Importer.ImportDpFid(imageByte, Constants.Formats.Fid.ANSI, 500, false);

            return ExtractByteArray(img);

        }

        private byte[] ExtractByteArray(Bitmap img)
        {
            byte[] bitData = null;
            //ToDo: CreateFmdFromRaw only works on 8bpp bytearrays. As such if we have an image with 24bpp then average every 3 values in Bitmapdata and assign it to bitdata
            if (img.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                //Lock the bitmap's bits
                BitmapData bitmapdata = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);
                //Declare an array to hold the bytes of bitmap
                byte[] imgData = new byte[bitmapdata.Stride * bitmapdata.Height]; //stride=360, height 392

                //Copy bitmapdata into array
                Marshal.Copy(bitmapdata.Scan0, imgData, 0, imgData.Length);//imgData.length =141120

                bitData = new byte[bitmapdata.Width * bitmapdata.Height];//ditmapdata.width =357, height = 392

                for (int y = 0; y < bitmapdata.Height; y++)
                    for (int x = 0; x < bitmapdata.Width; x++)
                        bitData[bitmapdata.Width * y + x] = imgData[y * bitmapdata.Stride + x];
            }

            else
            {
                bitData = new byte[img.Width * img.Height];//ditmapdata.width =357, height = 392, bitdata.length=139944
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        System.Drawing.Color pixel = img.GetPixel(x, y);
                        bitData[img.Width * y + x] = (byte)((Convert.ToInt32(pixel.R) + Convert.ToInt32(pixel.G) + Convert.ToInt32(pixel.B)) / 3);
                    }
                }
            }
            return bitData;
        }

        public IEnumerable<TI> FindVisualChildren<TI>(DependencyObject depObj) where TI : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is TI)
                    {
                        yield return (TI)child;
                    }

                    foreach (TI childOfChild in FindVisualChildren<TI>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        #endregion
    }
}
