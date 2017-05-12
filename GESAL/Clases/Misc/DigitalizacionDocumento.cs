using MVVMShared.Commands;
using MVVMShared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFPdfViewer;
using WIA;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Windows.Media.Imaging;
namespace GESAL.Clases.Misc
{
    public class DigitalizarDocumento
    {
        #region [Constructores]
        public DigitalizarDocumento()
        {
            PDFViewer = PopUpsViewModels.MainWindow.DigitalizacionDocumentosView.pdfViewer;
        }
        ~DigitalizarDocumento()
        {
            try
            {
                foreach (var item in scannedDoc)
                {
                    File.Delete(item);
                }
                if (!string.IsNullOrEmpty(fileNamepdf))
                    File.Delete(fileNamepdf);
            }
            catch { }
        }
        #endregion

        #region [Propiedades]
        public byte[] ScannedDocument { get; private set; }
        List<string> scannedDoc = new List<string>();
        string fileNamepdf;
        PdfViewer PDFViewer { get; set; }
        #endregion

        #region [Comandos]
        public ICommand startScanning
        {
            get { return new DelegateCommand<PdfViewer>(async (p) => { await Scann(); }); }
        }
        #endregion

        #region [Metodos]
        public Task<bool> Scann(PdfViewer obj = null)
        {
            scannedDoc = new List<string>();
            PDFViewer = obj;
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                obj.Visibility = Visibility.Collapsed;
            }));

            StaticSourcesViewModel.ShowMensajeProgreso("Espere Por Favor, Escaneando...");
            try
            {
                var morePages = true;
                var commonDialogClass = new CommonDialog();
                var scannerDevice = commonDialogClass.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, false);
                var pagecount = 0;

                if (scannerDevice != null)
                {
                    scannerDevice.Properties["Pages"].set_Value(1);
                    scannerDevice.Properties["Document Handling Select"].set_Value(1);

                    var document = new PdfDocument();
                    while (morePages)
                    {
                        try
                        {
                            var scannnerItem = scannerDevice.Items[1];
                            AdjustScannerSettings(scannnerItem, 150, 0, 0, 1250, 1700, 0, 0, WiaImageIntent.ColorIntent);

                            var img = (ImageFile)scannnerItem.Transfer("{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}");
                            if (img != null)
                            {

                                document.Pages.Add(new PdfPage());
                                var xgr = XGraphics.FromPdfPage(document.Pages[pagecount]);
                                var Ximg = XImage.FromBitmapSource(ConvertScannedImage(img));

                                xgr.DrawImage(Ximg, 0, 0, 595, 842);

                                pagecount++;
                            }

                            WIA.Property documentHandlingSelect = null;
                            WIA.Property documentHandlingStatus = null;

                            foreach (WIA.Property prop in scannerDevice.Properties)
                            {
                                if (prop.PropertyID == 3088)
                                    documentHandlingSelect = prop;

                                if (prop.PropertyID == 3087)
                                    documentHandlingStatus = prop;
                            }

                            morePages = false;

                            if (documentHandlingSelect != null)
                                if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & 0x00000001) != 0)
                                    morePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & 0x00000001) != 0);
                        }
                        catch (Exception ex)
                        {
                            if (pagecount <= 0)
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    if (ex.Message == "Exception from HRESULT: 0x80210003")
                                        StaticSourcesViewModel.Mensaje("Digitalización", "Revise que la bandeja tenga hojas", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                                    else
                                        if (ex.Message == "Value does not fall within the expected range.")
                                            StaticSourcesViewModel.Mensaje("Digitalización", "*- Revise que el escáner este encendido.\n*- Revise que el escáner este bien conectado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                                        else
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Hay problemas para conectarse con el escaner", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                                }));

                            morePages = false;
                        }
                    }

                    if (pagecount > 0)
                    {
                        fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";

                        document.Save(fileNamepdf);

                        ScannedDocument = File.ReadAllBytes(fileNamepdf);

                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            obj.LoadFile(fileNamepdf);
                            obj.Visibility = Visibility.Visible;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    if (ex.Message.Contains("HRESULT: 0x80210015"))
                        StaticSourcesViewModel.Mensaje("Digitalización", "*- No tienes ningun escaner instalado.\n*- Revise que el escáner este encendido.\n*- Revise que el escáner este bien conectado", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                    if (ex.Message.Contains("HRESULT: 0x80070021"))
                        StaticSourcesViewModel.Mensaje("Digitalización", "Hay problemas para conectarse con el escaner", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                }));
            }
            StaticSourcesViewModel.CloseMensajeProgreso();
            return TaskEx.FromResult(true);
        }

        private void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, WiaImageIntent colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";

            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }

        private void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            var prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        private BitmapSource ConvertScannedImage(ImageFile imageFile)
        {
            if (imageFile == null)
                return null;

            // save the image out to a temp file
            string fileName = Path.GetTempFileName();

            // this is pretty hokey, but since SaveFile won't overwrite, we 
            // need to do something to both guarantee a unique name and
            // also allow SaveFile to write the file
            File.Delete(fileName);

            // now save using the same filename
            imageFile.SaveFile(fileName);

            BitmapFrame img;

            // load the file back in to a WPF type, this is just 
            // to get around size issues with large scans
            using (FileStream stream = File.OpenRead(fileName))
            {
                img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                stream.Close();
            }

            // clean up
            scannedDoc.Add(fileName);

            return img;
        }

        public void Dispose()
        {
            try
            {
                foreach (var item in scannedDoc)
                {
                    File.Delete(item);
                }
                if (!string.IsNullOrEmpty(fileNamepdf))
                    File.Delete(fileNamepdf);

            }
            catch { }
            scannedDoc = null;
            fileNamepdf = null;

            ScannedDocument = null;
        }

        public void Hide()
        {
            if (PDFViewer == null)
                return;
            PDFViewer.Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            if (PDFViewer == null)
                return;
            PDFViewer.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
