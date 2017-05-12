using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biometrico.DigitalPersona;
using System.Windows;
using System.Threading;
using GESAL.Views;
using GESAL.BiometricoServiceReference;
using DPUruNet;
using MVVMShared.ViewModels;
using SSP.Controlador.Catalogo.Justicia;
using MVVMShared.Manager;
namespace GESAL.ViewModels
{
    public partial class AutenticacionViewModel : FingerPrintScanner
    {

        private void OnLoad(object sender)
        {
            try
            {
                Error = "Cargando servicios de lectura";
                var _view = (AutenticacionView)(sender);
                
                CrearListaImagenes();

                if (CurrentReader != null)
                {
                    CurrentReader.Dispose();
                    CurrentReader = null;
                }

                CurrentReader = Readers[0];

                if (CurrentReader == null)
                {
                    CapturaFalladaDigitalPersona = true;
                    setAutenticaContrasena();
                    CmdOkHabilitado = true;
                    Error = "El lector de huella no se encuentra conectado";
                    return;
                }

                _view.Closed += (s, e) =>
                {
                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                };

                if (!OpenReader())
                {
                    setAutenticaContrasena();
                    CapturaFalladaDigitalPersona = true;
                    CmdOkHabilitado = true;
                    Error = "No se puede accesar al lector de huella";
                }
                if (!StartCaptureAsync(OnCaptured))
                {
                    setAutenticaContrasena();
                    CapturaFalladaDigitalPersona = true;
                    CmdOkHabilitado = true;
                    Error = "No se puede iniciar la aunteticación por huella";
                }
                Error = string.Empty;
                OnProgress = new Thread(() => InvokeDelegate(_view));
            }
            catch { }
        }
        #region Metodos y Eventos del Digital Persona
        public override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            Error = string.Empty;
            base.OnCaptured(captureResult);
            Identify();
        }

        private async void Identify(object Huella = null)
        {
            try
            {
                if (FingerPrintData == null)
                    return;

                Error = "Verificando huella...";
                IniciaAnimacion = true;
                Thread.Sleep(5000);
                var Service = new BiometricoServiceClient();

                var CompareResult = Service.CompararHuellaImputado(new ComparationRequest { BIOMETRICO = FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes, ID_TIPO_BIOMETRICO = DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP });

                if (CompareResult.Identify)
                {
                    var result = CompareResult.Result[0];
                    var NombreLogin = await StaticSourcesViewModel.CargarDatosAsync(() => new cImputado().GetData().Where(w => w.ID_ANIO == result.ID_ANIO && w.ID_CENTRO == result.ID_CENTRO && w.ID_IMPUTADO == result.ID_IMPUTADO).FirstOrDefault());
                    var nombreComparar= NombreLogin.NOMBRE.Trim() + " " + NombreLogin.PATERNO.Trim() + " " + NombreLogin.MATERNO.Trim();
                    //cambiar el proceso para que traiga el username y de ahi sacar el resto de los datos.
                    if (_usuario.NombreCompleto != nombreComparar)
                        Error = "El capturista no pertenece a esta sesion";
                    else
                    {
                        Error = "Su huella ha sido autenticada con exito";
                        
                        OnProgress.Start();
                        Autenticado = true;
                    }
                }
                else
                    Error = "Esta huella no esta registrada en el sistema";
                IniciaAnimacion = false;
                FingerPrintData = null;

            }
            catch(Exception ex)
            {
                Error = "Ocurrió un error al verficar la huella";
                IniciaAnimacion = false;
            }
        }

        public override void OnSucceed(Window Window)
        {
            try
            {
                CmdOkHabilitado = true;
                IniciaAnimacion = false;
                if(Autenticado) //autocerrado
                    Window.DialogResult = true;
            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        public void Autenticar (object sender)
        {
            var _window = (AutenticacionView)(sender);
            _window.DialogResult = true;
        }

        public void CrearListaImagenes()
        {
            var imagenes = new List<byte[]>();
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint1.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint2.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint3.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint4.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint5.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint6.png"));
            imagenes.Add(new FileManagerProvider().fileToByteArray("pack://application:,,,/GESAL;component/Resources/Images/fingerprint7.png"));
            ListaImagenes = imagenes;
        }

        
    }
}
