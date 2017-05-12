using AForge.Controls;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ControlPenales.Clases
{
    //class WebCam
    //{
    //    ~WebCam()
    //    {
    //        this.Stop();
    //    }

    //    private bool _isInitialized = false;
    //    private VideoCaptureDevice SelectedCamera;
    //    public IntPtr windowHandle { get; set; }

    //    public bool IsInitialized
    //    {
    //        get { return _isInitialized; }
    //        set { _isInitialized = value; }
    //    }

    //    public void InitializeWebCam(List<Image> ImageControl)
    //    {
    //        try
    //        {
    //            var VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
    //            if (VideoCaptureDevices.Count != 0)
    //            {
    //                if (VideoCaptureDevices.Cast<FilterInfo>().Where(w => w.Name.ToLower() == "Microsoft LifeCam Studio".ToLower()).Any())
    //                    SelectedCamera = new VideoCaptureDevice(VideoCaptureDevices.Cast<FilterInfo>().Where(w => w.Name.ToLower() == "Microsoft LifeCam Studio".ToLower()).FirstOrDefault().MonikerString);
    //                else
    //                    foreach (var item in VideoCaptureDevices)
    //                    {
    //                        if (((FilterInfo)item).Name.ToLower().Contains("Cogent Device".ToLower()))
    //                            continue;
    //                        SelectedCamera = new VideoCaptureDevice(((FilterInfo)item).MonikerString);
    //                        break;
    //                    }
    //            }
    //            else
    //                throw new ApplicationException("No se dectecto ningun dispositivo conectado");

    //            if (SelectedCamera != null)
    //            {
    //                SelectedCamera.ProvideSnapshots = true;
    //                SelectedCamera.NewFrame += (s, e) =>
    //                {
    //                    if (Application.Current != null)
    //                        Application.Current.Dispatcher.Invoke((Action)(delegate
    //                        {
    //                            foreach (var item in ImageControl)
    //                                item.Source = LoadBitmap((System.Drawing.Bitmap)e.Frame.Clone());
    //                        }));
    //                    else
    //                        this.Stop();
    //                };
    //            }
    //            else
    //                throw new ApplicationException("No se dectecto ningun dispositivo conectado");
    //            _isInitialized = true;
    //        }
    //        catch (Exception)
    //        {
    //            _isInitialized = false;
    //        }
    //    }

    //    public void Start()
    //    {
    //        try
    //        {
    //            if (SelectedCamera == null)
    //                return;
    //            SelectedCamera.Start();
    //            _isInitialized = true;
    //        }
    //        catch (Exception ex)
    //        {
    //            _isInitialized = false;
    //            throw new ApplicationException("Error al iniciar camara web \n\n" + ex.Message);
    //        }
    //    }

    //    public void Stop()
    //    {
    //        try
    //        {
    //            if (SelectedCamera != null)
    //                if (SelectedCamera.IsRunning)
    //                {
    //                    SelectedCamera.SignalToStop();
    //                    SelectedCamera.WaitForStop();
    //                    SelectedCamera.Stop();
    //                }
    //        }
    //        catch { _isInitialized = false; }
    //        _isInitialized = false;
    //    }

    //    public void Dispose()
    //    {
    //        SelectedCamera = null;
    //        _isInitialized = false;
    //    }

    //    public void ResolutionSetting()
    //    {

    //    }

    //    public void AdvanceSetting(List<Image> ImageControl)
    //    {
    //        this.Stop();
    //        this.Dispose();
    //        this.InitializeWebCam(ImageControl);
    //        SelectedCamera.DisplayPropertyPage(this.windowHandle);
    //        if (ImageControl.Any())
    //            this.Start();
    //    }

    //    #region [WebCam Helper]
    //    //Block Memory Leak
    //    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    //    public static extern bool DeleteObject(IntPtr handle);
    //    public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
    //    {

    //        var ip = source.GetHbitmap();

    //        var bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,

    //            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

    //        DeleteObject(ip);

    //        return bs;

    //    }

    //    public static void SaveImageCapture(BitmapSource bitmap)
    //    {
    //        var encoder = new JpegBitmapEncoder();
    //        encoder.Frames.Add(BitmapFrame.Create(bitmap));
    //        encoder.QualityLevel = 100;


    //        // Configure save file dialog box
    //        var dlg = new Microsoft.Win32.SaveFileDialog();
    //        dlg.FileName = "Image"; // Default file name
    //        dlg.DefaultExt = ".Jpg"; // Default file extension
    //        dlg.Filter = "Image (.jpg)|*.jpg"; // Filter files by extension

    //        // Show save file dialog box
    //        var result = dlg.ShowDialog();

    //        // Process save file dialog box results
    //        if (result == true)
    //        {
    //            // Save Image
    //            string filename = dlg.FileName;
    //            var fstream = new FileStream(filename, FileMode.Create);
    //            encoder.Save(fstream);
    //            fstream.Close();
    //        }
    //    }

    //    public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    //    {
    //        if (depObj != null)
    //        {
    //            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
    //            {
    //                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
    //                if (child != null && child is T)
    //                {
    //                    yield return (T)child;
    //                }

    //                foreach (T childOfChild in FindVisualChildren<T>(child))
    //                {
    //                    yield return childOfChild;
    //                }
    //            }
    //        }
    //    }
    //    #endregion
    //}

    public class WebCam
    {
        private VideoCaptureDevice SelectedCamera;
        public bool isVideoSourceInitialized;
        VideoSourcePlayer VideoSourcePlayer;
        public List<Image> ImageControls;
        IntPtr WindowHandle;

        public static IEnumerable<MediaInformation> GetVideoDevices
        {
            get
            {
                var filterVideoDeviceCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                return (from FilterInfo filterInfo in filterVideoDeviceCollection select new MediaInformation { DisplayName = filterInfo.Name, UsbId = filterInfo.MonikerString }).ToList();
            }
        }

        public WebCam(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;
        }

        ~WebCam()
        {
            ReleaseVideoDevice();
        }

        public async Task InitializeWebCam(List<Image> ListImageControl)
        {
            try
            {
                if (isVideoSourceInitialized)
                    return;

                var VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (VideoCaptureDevices.Count != 0)
                {
                    if (VideoCaptureDevices.Cast<FilterInfo>().Where(w => w.Name.ToLower() == "Microsoft LifeCam Studio".ToLower()).Any())
                        SelectedCamera = new VideoCaptureDevice(VideoCaptureDevices.Cast<FilterInfo>().Where(w => w.Name.ToLower() == "Microsoft LifeCam Studio".ToLower()).FirstOrDefault().MonikerString);
                    else
                        foreach (var item in VideoCaptureDevices)
                        {
                            if (((FilterInfo)item).Name.ToLower().Contains("Cogent Device".ToLower()))
                                continue;
                            SelectedCamera = new VideoCaptureDevice(((FilterInfo)item).MonikerString);
                            break;
                        }
                }
                else
                    throw new ApplicationException("No se dectecto ningun dispositivo conectado");

                if (SelectedCamera != null)
                    await InitializeVideoDevice(SelectedCamera.Source);
            }
            catch (Exception)
            {
                isVideoSourceInitialized = false;
            }
                    ImageControls = ListImageControl;
        }

        private async Task InitializeVideoDevice(string videoDeviceSourceId)
        {
            await this.ReleaseVideoDevice();
            if (string.IsNullOrEmpty(videoDeviceSourceId))
                return;

            try
            {
                if (!GetVideoDevices.Any(item => item.UsbId.Equals(videoDeviceSourceId)))
                    return;

                this.SelectedCamera = new VideoCaptureDevice(videoDeviceSourceId);
                SelectedCamera.ProvideSnapshots = true;
                SelectedCamera.NewFrame += SelectedCamera_NewFrame;
                this.VideoSourcePlayer = new VideoSourcePlayer() { VideoSource = this.SelectedCamera };
                this.VideoSourcePlayer.Start();
                this.isVideoSourceInitialized = true;
            }
            catch { }
        }

        void SelectedCamera_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                if (Application.Current != null)
                    if (ImageControls != null)
                        foreach (var item in ImageControls)
                        {
                            if (!isVideoSourceInitialized)
                            {
                                ImageControls = new List<Image>();
                                break;
                            }
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                item.Source = LoadBitmap((System.Drawing.Bitmap)eventArgs.Frame.Clone());
                            }));
                        }
            }
            catch { }
        }

        public async Task ReleaseVideoDevice()
        {
            try
            {
                this.isVideoSourceInitialized = false;
                if (null == this.SelectedCamera)
                    return;

                ImageControls = new List<Image>();
                this.SelectedCamera.NewFrame -= SelectedCamera_NewFrame;
                //this.SelectedCamera.SignalToStop();
                do
                {
                    this.SelectedCamera.SignalToStop();
                    await TaskEx.Delay(500);
                } while (SelectedCamera != null ? SelectedCamera.IsRunning : false);

                if (SelectedCamera != null)
                {
                    this.SelectedCamera.WaitForStop();
                    this.SelectedCamera.Stop();
                    this.SelectedCamera = null;
                }
                #region Comentado
                //} while (SelectedCamera.IsRunning);
                //if (SelectedCamera != null)
                //{
                //    this.SelectedCamera.WaitForStop();
                //    this.SelectedCamera.Stop();
                //    this.SelectedCamera = null;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        public BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {
            var ip = source.GetHbitmap();
            var bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bs;
        }

        public ImageSource TomarFoto(Image Control)
        {
            try
            {
                ImageControls.Remove(Control);
                return Control.Source;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Ocurrió un error mientras se trataba de capturar foto desde el dispositivo de video seleccionado", exception);
            }
        }

        public void QuitarFoto(Image Control)
        {
            try
            {
                ImageControls.Add(Control);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Ocurrió un error mientras se trataba de capturar foto desde el dispositivo de video seleccionado", exception);
            }
        }

        public void AdvanceSetting()
        {
            if (null == this.SelectedCamera)
                return;

            this.SelectedCamera.SignalToStop();
            this.SelectedCamera.WaitForStop();
            this.SelectedCamera.Stop();
            SelectedCamera.DisplayPropertyPage(this.WindowHandle);
            SelectedCamera.Start();
        }

        public ImageSourceToSave AgregarImagenControl(Image Control, ImageSource Imagen)
        {
            if (Imagen == null)
                return new ImageSourceToSave();
            if (ImageControls == null ? true : !ImageControls.Where(w => w.Name == Control.Name).Any())
                return new ImageSourceToSave();
            if (!isVideoSourceInitialized)
            {
                ImageControls.Remove(ImageControls.Where(w => w.Name == Control.Name).SingleOrDefault());
                Control.Source = Imagen;
                return new ImageSourceToSave() { FrameName = Control.Name, ImageCaptured = (BitmapSource)Imagen };
            }
            ImageControls.Remove(ImageControls.Where(w => w.Name == Control.Name).SingleOrDefault());
            Control.Source = Imagen;
            return new ImageSourceToSave() { FrameName = Control.Name, ImageCaptured = (BitmapSource)Imagen };
        }
    }
    public class ImageSourceToSave
    {
        public string FrameName { get; set; }
        public BitmapSource ImageCaptured { get; set; }
    }

    public sealed class MediaInformation
    {
        public string DisplayName { get; set; }
        public string UsbId { get; set; }
    }
}
