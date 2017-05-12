using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class PrincipalVisitaExternaViewModel : FingerPrintScanner
    {
        public PrincipalVisitaExternaViewModel() { }

        private async void Load_Window(PrincipalVisitaExternaView Window)
        {
            #region [Huellas Digitales]
            var myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 240;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTargetName(myDoubleAnimation, "Ln");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            if (PopUpsViewModels.MainWindow.HuellasView != null)
                myStoryboard.Begin(PopUpsViewModels.MainWindow.HuellasView.Ln);

            #endregion

            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { llenarCombos(); });
            await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerBitacora);
        }

        #region [Huellas Digitales]

        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;

                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);

                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }

                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion

                                ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });

                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();

                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }
        private void OnBuscarPorHuella(string obj = "")
        {
            var windowBusqueda = new BusquedaHuella();
            windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_EXTERNA);
            if (((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0)
                return;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;

                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                        return;

                    if (huella.SelectRegistro == null)
                        return;

                    if (huella.SelectRegistro.Persona == null)
                        return;

                    SelectPersona = huella.SelectRegistro.Persona;
                    TextNombre = SelectPersona.NOMBRE;
                    TextPaterno = SelectPersona.MATERNO;
                    TextMaterno = SelectPersona.PATERNO;
                    SelectDiscapacitado = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                    SelectDiscapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : new Nullable<short>();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
        }

        #endregion

        #region [Camara]

        #endregion

        private void llenarCombos()
        {
            if (ListArea == null ? true : ListArea.Count <= 0)
            {
                ListArea = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                ListArea.Insert(0, new AREA() { DESCR = "SELECCIONE", ID_AREA = -1, ID_TIPO_AREA = -1 });
            }
            if (ListDepartamentos == null ? true : ListDepartamentos.Count <= 0)
            {
                ListDepartamentos = new ObservableCollection<DEPARTAMENTO>(new cDepartamentos().ObtenerTodos());
                ListDepartamentos.Insert(0, new DEPARTAMENTO() { DESCR = "SELECCIONE", ID_DEPARTAMENTO = -1 });
            }
            if (ListTipoVisitante == null ? true : ListTipoVisitante.Count <= 0)
            {
                ListTipoVisitante = new ObservableCollection<TIPO_VISITANTE>(new cTipoVisitante().ObtenerTodos().OrderBy(o => o.DESCR));
                ListTipoVisitante.Insert(0, new TIPO_VISITANTE() { DESCR = "SELECCIONE", ID_TIPO_VISITANTE = -1 });
            }
            if (ListDiscapacidades == null ? true : ListDiscapacidades.Count <= 0)
            {
                ListDiscapacidades = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos().OrderBy(o => o.DESCR));
                ListDiscapacidades.Insert(0, new TIPO_DISCAPACIDAD() { DESCR = "SELECCIONE", ID_TIPO_DISCAPACIDAD = -1 });
            }
            if (ListPuestos == null ? true : ListPuestos.Count <= 0)
            {
                ListPuestos = new ObservableCollection<PUESTO>(new cPuesto().ObtenerTodos().OrderBy(o => o.DESCR));
                ListPuestos.Insert(0, new PUESTO() { DESCR = "SELECCIONE", ID_PUESTO = -1 });
            }
            if (ListPais == null ? true : ListPais.Count <= 0)
            {
                ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos().OrderBy(o => o.PAIS));
                ListPais.Insert(0, new PAIS_NACIONALIDAD() { PAIS = "SELECCIONE", ID_PAIS_NAC = -1 });
            }
        }

        private void ObtenerBitacora()
        {
            /// TODO: poner los bindings en el grid de registros
            ListRegistros = new ObservableCollection<VISITA_EXTERNA>(new cVisitaExterna().ObtenerXFechaYCentro(Fechas.GetFechaDateServer, 4));
        }

        private Task<bool> GetPersona(int pers)
        {
            SelectPersona = new cPersona().Obtener(pers).FirstOrDefault();
            return TaskEx.FromResult(true);
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region BUSCAR_PERSONA
                case "seleccionar_buscar_persona":
                    if (SelectPersona != null)
                    {
                        if (ExisteNuevo == true)
                        {
                            SetValidacionesAgregarNuevaVisitaExterna();
                            GetDatosNuevoPadronVisitanteSeleccionado();
                            Existente = true;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
                        }
                        else
                        {
                            //Modificacion de modelo, PENDIENTE
                            if (SelectPersona.PERSONA_EXTERNO != null)
                            {
                                SetValidacionesVisitaExterna();
                                GetDatosPersonaSeleccionada();
                                FotoDetrasCredencial = FotoDetrasCredencialAuxiliar = FotoFrenteCredencial = FotoFrenteCredencialAuxiliar = null;
                                FotoPersonaExterna = null;
                                Existente = true;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
                            }
                            else
                            {
                                var respuesta = await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "LA PERSONA SELECCIONADA NO ESTA REGISTRADA COMO EXTERNA,"
                                    + " DESEA REGISTRARLA AHORA?");
                                if (respuesta == 1)
                                {
                                    SetValidacionesAgregarNuevaVisitaExterna();
                                    GetDatosNuevoPadronVisitanteSeleccionado();
                                    Existente = ExisteNuevo = true;
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
                                }
                            }
                        }
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Debes seleccionar una persona.");
                    break;
                case "cancelar_buscar_persona":
                    if (ExisteNuevo == true)
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    var pers = new SSP.Servidor.PERSONA();
                    pers = SelectPersonaAuxiliar;
                    SelectPersona = pers;
                    var exist = true;
                    exist = ExistenteAuxiliar;
                    Existente = exist;
                    GetDatosPersonaSeleccionada();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
                    break;
                case "buscar_visitante":
                    BuscarPersonas();
                    break;
                #endregion

                #region Foto_Credencial
                case "capturar_credencial_visita_externa":
                    ComboFrontBackFotoVisible = Visibility.Visible;
                    if (FotoFrenteCredencial != null)
                    {
                        var foto = FotoFrenteCredencial;
                        FotoFrenteCredencialAuxiliar = foto;
                    }
                    if (FotoDetrasCredencial != null)
                    {
                        var foto = FotoDetrasCredencial;
                        FotoDetrasCredencialAuxiliar = foto;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    TaskEx.Delay(150);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    break;
                #endregion

                #region Foto_Persona
                case "abrir_camara_visita_externa"://PERSONA
                    ComboFrontBackFotoVisible = Visibility.Collapsed;
                    SelectFrenteDetrasFoto = "F";
                    if (FotoPersonaExterna != null)
                    {
                        var foto = FotoPersonaExterna;
                        FotoPersonaExternaAuxiliar = foto;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    break;
                #endregion

                #region Fotos_Generales
                case "aceptar_tomar_foto_senas":
                    if (ComboFrontBackFotoVisible == Visibility.Visible)
                    {
                        if (FotoFrenteCredencial != null)
                        {
                            if (FotoDetrasCredencial != null)
                            {
                                SelectFrenteDetrasFoto = string.Empty;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Error!", "Debes tomar la foto trasera.");
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Error!", "Debes tomar la foto frontal.");
                    }
                    else
                    {
                        if (FotoPersonaExterna != null)
                        {
                            SelectFrenteDetrasFoto = string.Empty;
                            ImagenVisitaExterna = FotoPersonaExterna;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Error!", "Debes tomar la foto del visitante.");
                    }
                    break;
                case "cancelar_tomar_foto_senas":
                    if (ComboFrontBackFotoVisible == Visibility.Visible)
                    {
                        if (FotoFrenteCredencialAuxiliar != null)
                        {
                            var foto = FotoFrenteCredencialAuxiliar;
                            FotoFrenteCredencial = foto;
                        }
                        if (FotoDetrasCredencialAuxiliar != null)
                        {
                            var foto = FotoDetrasCredencialAuxiliar;
                            FotoDetrasCredencial = foto;
                        }
                    }
                    else
                    {
                        if (FotoPersonaExternaAuxiliar != null)
                        {
                            var foto = FotoPersonaExternaAuxiliar;
                            FotoPersonaExterna = foto;
                        }
                    }

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    await TaskEx.Delay(150);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    break;
                #endregion

                #region MENU
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "ayuda_menu":
                    break;
                case "buscar_menu":
                    TextNombre = TextPaterno = TextMaterno = string.Empty;
                    var persona = new SSP.Servidor.PERSONA();
                    persona = SelectPersona;
                    SelectPersonaAuxiliar = persona;
                    ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
                    break;
                case "guardar_menu":
                    SetValidacionesVisitaExterna();
                    if (!HasErrors())
                    {
                        if (SelectPersona != null)
                        {
                            if (Existente)
                            {
                                var respuesta = new cVisitaExterna().ObtenerXFechaCentroYPersona(Fechas.GetFechaDateServer, GlobalVar.gCentro/*4*/, SelectPersona.ID_PERSONA).Any() ?
                                    await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "YA EXISTE UN REGISTRO DE ESTA PERSONA EL DIA DE HOY, "
                                    + "ESTA SEGURO QUE DESEA INGRESAR UNO NUEVO?") :
                                        1;
                                if (respuesta == 1)
                                {
                                    //Modificacion de modelo, PENDIENTE
                                    if (SelectPersona.PERSONA_EXTERNO == null)
                                    {
                                        #region Persona NIP
                                        //var personNIP = new PERSONA_NIP()
                                        //{
                                        //    NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                                        //    ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_EXTERNA,
                                        //    ID_PERSONA = SelectPersona.ID_PERSONA,
                                        //    ID_CENTRO = GlobalVar.gCentro
                                        //};
                                        //if (new cPersonaNIP().Insertar(personNIP))
                                        //{
                                            #region Persona Externa
                                            var personExterna = new PERSONA_EXTERNO()
                                            {
                                                CREDENCIAL_DETRAS = FotoDetrasCredencial,
                                                CREDENCIAL_FRENTE = FotoFrenteCredencial,
                                                ID_PERSONA = SelectPersona.ID_PERSONA,
                                                ID_CENTRO = GlobalVar.gCentro,//4, ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                                ESTATUS = "S",
                                                OBSERV = TextObservacionNuevo
                                            };
                                            if (!new cPersonaExterna().Insertar(personExterna))
                                                (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL CREAR LA PERSONA COMO VISITANTE EXTERNA");
                                            #endregion
                                        //}
                                        //else
                                        //    (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR EL NIP COMO VISITANTE EXTERNO");
                                        #endregion

                                    }

                                    #region Visita Externa
                                    var visitaExterna = new VISITA_EXTERNA()
                                    {
                                        AREA_VISITA = SelectArea,
                                        ASUNTO = TextAsunto,
                                        FEC_ENTRADA = DateTime.Parse(Fechas.GetFechaDateServer.ToString("dd/MM/yyyy") + " " + TextHoraEntrada.ToString()),
                                        FEC_SALIDA = null,
                                        ID_CENTRO = GlobalVar.gCentro,//4, ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                        ID_CONS = new cPersona().GetSequence<int>("VISITA_EXTERNA_SEQ"),
                                        ID_PERSONA = SelectPersona.ID_PERSONA,
                                        ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_EXTERNA,
                                        INSTITUCION = TextInstitucion,
                                        OBSERV = TextObservaciones,
                                        DEP_ID_CENTRO = SelectDepartamento.HasValue ? SelectDepartamento.Value != -1 ? SelectDepartamentoItem.ID_CENTRO : new Nullable<short>() : new Nullable<short>(),
                                        DEP_ID_DEPARTAMENTO = SelectDepartamento.HasValue ? SelectDepartamento.Value > 0 ? SelectDepartamentoItem.ID_DEPARTAMENTO : SelectDepartamento : SelectDepartamento,
                                        ID_PUESTO = SelectPuesto == -1 ? new Nullable<short>() : SelectPuesto,
                                        //PERTENENCIAS=TextPertenencias
                                    };
                                    if (new cVisitaExterna().Insertar(visitaExterna))
                                    {
                                        LimpiarRegistroAccesoAduana();
                                        await StaticSourcesViewModel.CargarDatosMetodoAsync(ObtenerBitacora);
                                        base.ClearRules();
                                        OnPropertyChanged();
                                        (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "LA INFORMACIÓN SE GUARDO CON ÉXITO");
                                    }
                                    else
                                        (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL AGENDAR LA VISITA AL CENTRO");
                                    #endregion
                                }
                            }
                            else
                            {
                                #region Visita Externa
                                var visitaExterna = new VISITA_EXTERNA()
                                {
                                    FEC_ENTRADA = DateTime.Parse(Fechas.GetFechaDateServer.ToString("dd/MM/yyyy") + " " + TextHoraEntrada.ToString()),
                                    ID_PUESTO = SelectPuesto.HasValue ? SelectPuesto.Value : new Nullable<short>(),
                                    ID_CONS = new cPersona().GetSequence<int>("VISITA_EXTERNA_SEQ"),
                                    ID_PERSONA = SelectPersona.ID_PERSONA,
                                    INSTITUCION = TextInstitucion,
                                    OBSERV = TextObservaciones,
                                    AREA_VISITA = SelectArea,
                                    ASUNTO = TextAsunto,
                                    ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_EXTERNA,
                                    FEC_SALIDA = null,
                                    ID_CENTRO = GlobalVar.gCentro,//4, ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                    //PERTENENCIAS=TextPertenencias
                                };
                                if (new cVisitaExterna().Insertar(visitaExterna))
                                {
                                    LimpiarRegistroAccesoAduana();
                                    (new Dialogos()).ConfirmacionDialogo("ÉXITO!", "SE GUARDO EL REGISTRO DE ENTRADA DE VISITA CON ÉXITO");
                                }
                                else
                                    (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL AGENDAR LA VISITA AL CENTRO");
                                #endregion
                            }
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar a una persona o crear una nueva.");
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", string.Format("Al validar el campo: {0}.", base.Error));
                    break;
                case "nuevo_menu":
                    LimpiarAgregarVisitaExterna();
                    SetValidacionesAgregarNuevaVisitaExterna();
                    OnPropertyChanged();
                    SelectFrenteDetrasFoto = "F";
                    Existente = false;
                    ImagenVisitaExterna = new Imagenes().getImagenPerson();
                    FotoFrenteCredencial = FotoDetrasCredencial = FotoPersonaExterna = null;
                    SelectPersona = new SSP.Servidor.PERSONA();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    break;
                case "imprimir_menu":
                    if (!GafeteFrente)
                    {
                        GafeteFrente = true;
                    }
                    CrearGafete();
                    break;
                case "limpiar_menu":
                    LimpiarRegistroAccesoAduana();
                    LimpiarAgregarVisitaExterna();
                    break;
                #endregion

                #region Nuevo_Visitante_Externo
                case "capturar_huellas_visita_externa":
                    SelectSexoNuevo = "M";
                    SelectTipoVisitanteNuevo = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                case "cancelar_visita_externa":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                    break;
                case "guardar_visita_externa":
                    SetValidacionesAgregarNuevaVisitaExterna();
                    if (!HasErrors())
                    {
                        if (ExisteNuevo == true)
                        {
                            if (SelectPersona != null)
                            {
                                #region ACTUALIZAR PERSONA
                                var person = new SSP.Servidor.PERSONA()
                                {
                                    NOMBRE = TextNombreNuevo,
                                    PATERNO = TextPaternoNuevo,
                                    MATERNO = TextMaternoNuevo,
                                    CORREO_ELECTRONICO = TextCorreoNuevo,
                                    DOMICILIO_CALLE = TextCalle,
                                    DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                                    DOMICILIO_NUM_EXT = TextNumExt,
                                    DOMICILIO_NUM_INT = TextNumInt,
                                    FEC_NACIMIENTO = FechaNacimientoNuevo,
                                    ID_PAIS = SelectPais,
                                    ID_ENTIDAD = SelectEntidad,
                                    ID_MUNICIPIO = SelectMunicipio,
                                    ID_COLONIA = SelectColonia,
                                    ID_TIPO_PERSONA = SelectPersona.ID_TIPO_PERSONA,
                                    ID_TIPO_DISCAPACIDAD = SelectDiscapacitadoNuevo == "S" ? SelectDiscapacidadNuevo : 0,
                                    SEXO = SelectSexoNuevo,
                                    TELEFONO = TextTelefonoFijoNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                    TELEFONO_MOVIL = TextTelefonoMovilNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                    CORIGINAL = SelectPersona.CORIGINAL,
                                    CURP = SelectPersona.CURP,
                                    ESTADO_CIVIL = SelectPersona.ESTADO_CIVIL,
                                    ID_ETNIA = SelectPersona.ID_ETNIA,
                                    ID_PERSONA = SelectPersona.ID_PERSONA,
                                    IFE = SelectPersona.IFE,
                                    LUGAR_NACIMIENTO = SelectPersona.LUGAR_NACIMIENTO,
                                    NACIONALIDAD = SelectPersona.NACIONALIDAD,
                                    NORIGINAL = SelectPersona.NORIGINAL,
                                    RFC = SelectPersona.RFC,
                                    SMATERNO = SelectPersona.SMATERNO,
                                    SNOMBRE = SelectPersona.SNOMBRE,
                                    SPATERNO = SelectPersona.SPATERNO
                                };
                                if (new cPersona().Actualizar(person))
                                {
                                    await StaticSourcesViewModel.CargarDatosMetodoAsync(async () => { await GetPersona(person.ID_PERSONA); });
                                    //Modificacion de modelo, PENDIENTE
                                    if (SelectPersona.PERSONA_EXTERNO == null)
                                    {
                                        #region Persona NIP
                                        //var personNIP = new PERSONA_NIP()
                                        //{
                                        //    NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                                        //    ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_EXTERNA,
                                        //    ID_PERSONA = person.ID_PERSONA,
                                        //    ID_CENTRO = GlobalVar.gCentro,//4 ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                        //};
                                        //if (new cPersonaNIP().Insertar(personNIP))
                                        //{
                                            #region Persona Externa
                                            var personExterna = new PERSONA_EXTERNO()
                                            {
                                                CREDENCIAL_DETRAS = FotoDetrasCredencial,
                                                CREDENCIAL_FRENTE = FotoFrenteCredencial,
                                                ID_PERSONA = person.ID_PERSONA,
                                                ID_CENTRO = GlobalVar.gCentro,//4, ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                                ESTATUS = "S",
                                                ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo,
                                                OBSERV = TextObservacionNuevo
                                            };
                                            if (new cPersonaExterna().Insertar(personExterna))
                                            {
                                                LimpiarRegistroAccesoAduana();
                                                await StaticSourcesViewModel.CargarDatosMetodoAsync(async () => { await GetPersona(person.ID_PERSONA); });
                                                SetValidacionesVisitaExterna();
                                                GetDatosPersonaSeleccionada();
                                                ImagenVisitanteExterno = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                                            }
                                            else
                                                (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR LAS FOTOS DE LA CREDENCIAL");
                                            #endregion
                                        //}
                                        //else
                                        //    (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR EL NIP COMO VISITANTE EXTERNO");
                                        #endregion
                                    }
                                    else
                                    {
                                        var prsnXtrn = SelectPersona.PERSONA_EXTERNO;
                                        #region Persona Externa
                                        var personExterna = new PERSONA_EXTERNO()
                                        {
                                            CREDENCIAL_DETRAS = prsnXtrn.CREDENCIAL_DETRAS == FotoDetrasCredencial ? prsnXtrn.CREDENCIAL_DETRAS : FotoDetrasCredencial,
                                            CREDENCIAL_FRENTE = prsnXtrn.CREDENCIAL_FRENTE == FotoFrenteCredencial ? prsnXtrn.CREDENCIAL_FRENTE : FotoFrenteCredencial,
                                            ID_PERSONA = prsnXtrn.ID_PERSONA,
                                            ID_CENTRO = prsnXtrn.ID_CENTRO,
                                            ESTATUS = prsnXtrn.ESTATUS,
                                            ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo,
                                            OBSERV = TextObservacionNuevo
                                        };
                                        if (new cPersonaExterna().Actualizar(personExterna))
                                        {
                                            LimpiarRegistroAccesoAduana();
                                            await StaticSourcesViewModel.CargarDatosMetodoAsync(async () => { await GetPersona(person.ID_PERSONA); });
                                            SetValidacionesVisitaExterna();
                                            GetDatosPersonaSeleccionada();
                                            ImagenVisitanteExterno = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                                        }
                                        else
                                            (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR LAS FOTOS DE LA CREDENCIAL");
                                        #endregion
                                    }
                                }
                                else
                                    (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR LA PERSONA");
                                #endregion
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar una persona existente.");
                        }
                        else
                        {
                            #region INSERTAR
                            var person = new SSP.Servidor.PERSONA()
                            {
                                NOMBRE = TextNombreNuevo,
                                PATERNO = TextPaternoNuevo,
                                MATERNO = TextMaternoNuevo,
                                CORREO_ELECTRONICO = TextCorreoNuevo,
                                DOMICILIO_CALLE = TextCalle,
                                DOMICILIO_CODIGO_POSTAL = TextCodigoPostal,
                                DOMICILIO_NUM_EXT = TextNumExt,
                                DOMICILIO_NUM_INT = TextNumInt,
                                FEC_NACIMIENTO = FechaNacimientoNuevo,
                                ID_PAIS = SelectPais,
                                ID_ENTIDAD = SelectEntidad,
                                ID_MUNICIPIO = SelectMunicipio,
                                ID_COLONIA = SelectColonia,
                                ID_TIPO_PERSONA = 4,
                                ID_TIPO_DISCAPACIDAD = SelectDiscapacitadoNuevo == "S" ? SelectDiscapacidadNuevo : 0,
                                SEXO = SelectSexoNuevo,
                                TELEFONO = TextTelefonoFijoNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""),
                                TELEFONO_MOVIL = TextTelefonoMovilNuevo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                            };
                            if (new cPersona().Insertar(person))
                            {
                                #region Persona Biometrico
                                var personBiometrico = new List<SSP.Servidor.PERSONA_BIOMETRICO>();
                                if (HuellasCapturadas != null ? HuellasCapturadas.Count > 0 : false)
                                {
                                    personBiometrico = HuellasCapturadas.Select(s => new SSP.Servidor.PERSONA_BIOMETRICO
                                    {
                                        ID_PERSONA = person.ID_PERSONA,
                                        ID_TIPO_BIOMETRICO = (short)s.ID_TIPO_BIOMETRICO,
                                        ID_FORMATO = (short)s.ID_TIPO_FORMATO,
                                        BIOMETRICO = s.BIOMETRICO
                                    }).ToList();
                                }
                                if (FotoPersonaExterna != null)
                                {
                                    personBiometrico.Add(new SSP.Servidor.PERSONA_BIOMETRICO()
                                    {
                                        ID_TIPO_BIOMETRICO = (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO,
                                        ID_FORMATO = (short)enumTipoFormato.FMTO_JPG,
                                        BIOMETRICO = FotoPersonaExterna,
                                        ID_PERSONA = person.ID_PERSONA
                                    });
                                }
                                if (personBiometrico.Count > 0)
                                {
                                    if (!new cPersonaBiometrico().Insertar(personBiometrico))
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR HUELLAS");
                                        return;
                                    }
                                }
                                #endregion

                                #region Persona NIP
                                var personNIP = new PERSONA_NIP()
                                {
                                    NIP = new cPersona().GetSequence<int>("NIP_SEQ"),
                                    ID_TIPO_VISITA = Parametro.ID_TIPO_VISITA_EXTERNA,
                                    ID_PERSONA = person.ID_PERSONA,
                                    ID_CENTRO = GlobalVar.gCentro,//4 ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                };
                                if (new cPersonaNIP().Insertar(personNIP))
                                {
                                    #region Persona Externa
                                    var personExterna = new PERSONA_EXTERNO()
                                    {
                                        CREDENCIAL_DETRAS = FotoDetrasCredencial,
                                        CREDENCIAL_FRENTE = FotoFrenteCredencial,
                                        ID_PERSONA = person.ID_PERSONA,
                                        ID_CENTRO = GlobalVar.gCentro,//4, ///TODO: SE TOMARA EL ID_CENTRO DEL USUARIO LOGUEADO
                                        ESTATUS = "S",
                                        ID_TIPO_VISITANTE = SelectTipoVisitanteNuevo,
                                        OBSERV = TextObservacionNuevo
                                    };
                                    if (new cPersonaExterna().Insertar(personExterna))
                                    {
                                        await StaticSourcesViewModel.CargarDatosMetodoAsync(async () => { await GetPersona(person.ID_PERSONA); });
                                        GetDatosPersonaSeleccionada();
                                        ImagenVisitanteExterno = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
                                    }
                                    else
                                        (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR LAS FOTOS DE LA CREDENCIAL");
                                    #endregion
                                }
                                else
                                    (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR EL NIP COMO VISITANTE EXTERNO");
                                #endregion
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("ERROR", "OCURRIÓ UN ERROR AL GUARDAR LA PERSONA");
                            #endregion
                        }
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", string.Format("Al validar el campo: {0}.", base.Error));
                    break;
                #endregion

                #region CORRESPONDENCIA
                case "Registro_Correspondencia":
                    PopUpsViewModels.ShowPopUp(new CorrespondenciasViewModel(), PopUpsViewModels.TipoPopUp.REGISTRO_CORRESPONDENCIA);
                    break;
                #endregion
            }
        }

        private void CrearGafete()
        {
            #region VALIDACIONES
            if (SelectPersona == null)
            {
                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar o crear un visitante.");
                return;
            }
            if (string.IsNullOrEmpty(SelectPersona.TELEFONO) && string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL))
            {
                (new Dialogos()).ConfirmacionDialogo("Error", "Se debe registrar un teléfono del visitante.");
                return;
            }
            var fotos = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG);
            if (!fotos.Any())
            {
                (new Dialogos()).ConfirmacionDialogo("Error", "Debes tomarle foto al visitante.");
                return;
            }
            ///TODO: Cambiar centro por usuario login
            //var nip = SelectPersona.PERSONA_NIP.Where(w => w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_EXTERNA && w.ID_CENTRO == 4);
            //if (!nip.Any())
            //{
            //    (new Dialogos()).ConfirmacionDialogo("Error", "El visitante no tiene NIP registrado.");
            //    return;
            //}
            ///TODO: Cambiar centro por usuario login

            //Modificacion de modelo, PENDIENTE
            if (SelectPersona.PERSONA_EXTERNO == null)
            {
                (new Dialogos()).ConfirmacionDialogo("Error", "El visitante no tiene registro como visitante externo.");
                return;
            }
            #endregion

            #region GAFETE
            List<GafeteVisitaExterna> gafetes = new List<GafeteVisitaExterna>();
            var gafeteView = new GafetesPVCView();
            var gaf = new GafeteVisitaExterna();
            ///TODO: Cambiar centro por usuario login
            var centro = new cCentro().Obtener(4).FirstOrDefault();
            gaf.CERESO = centro.DESCR.Trim();
            gaf.DirectorCentro = centro.DIRECTOR;
            gaf.Discapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD == null || SelectPersona.ID_TIPO_DISCAPACIDAD == 0 ? "NINGUNA" : SelectPersona.TIPO_DISCAPACIDAD.DESCR;

            gaf.Fecha = centro.MUNICIPIO.ENTIDAD.DESCR + " A " + Fechas.GetFechaDateServer.ToString("dd DE MMM de yyyy");
            gaf.ImagenVisitante = fotos.FirstOrDefault().BIOMETRICO;
            //gaf.NIP = nip.FirstOrDefault().NIP.ToString();
            gaf.Fecha = Fechas.GetFechaDateServer.ToString("B.C. A dd DE MMMM DE yyyy").ToUpper();
            gaf.NombreVisitante = SelectPersona.NOMBRE.Trim() + " " + SelectPersona.PATERNO.Trim() + " " + SelectPersona.MATERNO.Trim();
            gaf.NumeroCredencial = SelectPersona.ID_PERSONA.ToString();
            gaf.Telefono = string.IsNullOrEmpty(SelectPersona.TELEFONO) ? string.IsNullOrEmpty(SelectPersona.TELEFONO_MOVIL) ? "N/A" : SelectPersona.TELEFONO_MOVIL.Trim() : SelectPersona.TELEFONO.Trim();
            var tipoVisita = "V I S I T A  ";
            //foreach (var item in nip.FirstOrDefault().TIPO_VISITA.DESCR.Trim())
            //{
            //    tipoVisita = tipoVisita + item + " ";
            //}
            if (SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE != null)
                    tipoVisita = string.Format("{0} {1}",tipoVisita,SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE.DESCR);
            gaf.TipoVisita = tipoVisita;
            //Modificacion de modelo, PENDIENTE
            if (SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE != null)
                gaf.TipoVisitante = SelectPersona.PERSONA_EXTERNO.TIPO_VISITANTE.DESCR;
            else
                gaf.TipoVisitante = string.Empty;

            gaf.Titulo = "SISTEMA ESTATAL PENITENCIARIO";

            gafeteView.GafetesPVCReport.LocalReport.ReportPath = "../../Reportes/" + gafeteExterno + ".rdlc";

            gafeteView.GafetesPVCReport.ShowExportButton = false;

            gafetes.Add(gaf);
            var rds = new Microsoft.Reporting.WinForms.ReportDataSource();
            rds.Name = "DataSet1";
            rds.Value = gafetes;

            gafeteView.GafetesPVCReport.ShowExportButton = false;

            gafeteView.GafetesPVCReport.LocalReport.DataSources.Clear();

            gafeteView.GafetesPVCReport.LocalReport.DataSources.Add(rds);
            gafeteView.GafetesPVCReport.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            gafeteView.GafetesPVCReport.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            gafeteView.GafetesPVCReport.ZoomPercent = 100;
            gafeteView.GafetesPVCReport.VerticalScroll.Visible = false;
            gafeteView.GafetesPVCReport.HorizontalScroll.Visible = false;
            gafeteView.GafetesPVCReport.VerticalScroll.Enabled = false;
            gafeteView.GafetesPVCReport.HorizontalScroll.Enabled = false;
            gafeteView.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 0 };
            gafeteView.GafetesPVCReport.RefreshReport();

            gafeteView.DataContext = this;

            gafeteView.rbFrente.Checked += (s, e) =>
            {
                try
                {
                    if (GafeteFrente)
                    {
                        gafeteView.GafetesPVCReport.LocalReport.ReportPath = "../../Reportes/" + gafeteExterno + ".rdlc";
                        gafeteView.GafetesPVCReport.RefreshReport();
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar", ex);
                }
            };
            gafeteView.rbDetras.Checked += (s, e) =>
            {
                try
                {
                    if (GafeteDetras)
                    {
                        gafeteView.GafetesPVCReport.LocalReport.ReportPath = "../../Reportes/" + gafeteExterno + ".rdlc";
                        gafeteView.GafetesPVCReport.RefreshReport();
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar", ex);
                }
            };
            gafeteView.Owner = PopUpsViewModels.MainWindow;
            gafeteView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            gafeteView.ShowDialog();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            #endregion

        }

        private bool HasErrors()
        {
            return base.HasErrors;
        }

        private void ClickEnter(Object obj)
        {
            base.ClearRules();

            if (obj != null)
            {
                var textbox = (TextBox)obj;
                switch (textbox.Name)
                {
                    case "NombreBuscar":
                        TextNombre = textbox.Text;
                        break;
                    case "PaternoBuscar":
                        TextPaterno = textbox.Text;
                        break;
                    case "MaternoBuscar":
                        TextMaterno = textbox.Text;
                        break;
                    case "NIPBuscar":
                        TextNIP = textbox.Text;
                        break;
                }
            }

            BuscarPersonas();

        }

        private void ClickEnterNuevo(Object obj)
        {
            base.ClearRules();
            if (obj != null)
            {
                var textbox = (TextBox)obj;
                switch (textbox.Name)
                {
                    case "NombreBuscarNuevo":
                        TextNombreNuevo = textbox.Text;
                        break;
                    case "PaternoBuscarNuevo":
                        TextPaternoNuevo = textbox.Text;
                        break;
                    case "MaternoBuscarNuevo":
                        TextMaternoNuevo = textbox.Text;
                        break;
                }
            }

            BuscarPersonasNuevo();

        }

        private async void BuscarPersonas()
        {
            if (string.IsNullOrEmpty(TextNIP))
                TextNIP = string.Empty;
            if (string.IsNullOrEmpty(TextNombre))
                TextNombre = string.Empty;
            if (string.IsNullOrEmpty(TextPaterno))
                TextPaterno = string.Empty;
            if (string.IsNullOrEmpty(TextMaterno))
                TextMaterno = string.Empty;
            var lista = new List<PERSONAVISITAAUXILIAR>();

            var personas = await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() => new cPersona().ObtenerXNombreYNIP(TextNombre,
                                            TextPaterno, TextMaterno, string.IsNullOrEmpty(TextNIP) ? 0 : int.Parse(TextNIP)));

            ListPersonas =new ObservableCollection<SSP.Servidor.PERSONA>(personas);
            if (ListPersonas.Count > 0)//Empty row
                EmptyBuscarRelacionInternoVisible = false;
            else
                EmptyBuscarRelacionInternoVisible = true;
            var persona = new SSP.Servidor.PERSONA();
            persona = SelectPersona;
            SelectPersonaAuxiliar = persona;
            var existe = true;
            existe = Existente;
            ExistenteAuxiliar = existe;
            ExisteNuevo = false;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
        }

        private async void BuscarPersonasNuevo()
        {
            if (string.IsNullOrEmpty(TextNombreNuevo))
                TextNombreNuevo = string.Empty;
            if (string.IsNullOrEmpty(TextPaternoNuevo))
                TextPaternoNuevo = string.Empty;
            if (string.IsNullOrEmpty(TextMaternoNuevo))
                TextMaternoNuevo = string.Empty;
            var lista = new List<PERSONAVISITAAUXILIAR>();

            var personas = await StaticSourcesViewModel.CargarDatosAsync<IQueryable<SSP.Servidor.PERSONA>>(() => 
                new cPersona().ObtenerXNombreYNIP(TextNombreNuevo, TextPaternoNuevo, TextMaternoNuevo, 0));


            ListPersonas = new ObservableCollection<SSP.Servidor.PERSONA>(personas);
            if (ListPersonas.Count > 0)//Empty row
                EmptyBuscarRelacionInternoVisible = false;
            else
                EmptyBuscarRelacionInternoVisible = true;
            var persona = new SSP.Servidor.PERSONA();
            persona = SelectPersona;
            SelectPersonaAuxiliar = persona;
            var existe = true;
            existe = Existente;
            ExistenteAuxiliar = existe;
            ExisteNuevo = true;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXTERNAS);
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EXTERNA);
        }

        private void GetDatosPersonaSeleccionada()
        {
            if (SelectPersona != null)
            {
                TextNombre = SelectPersona.NOMBRE;
                TextPaterno = SelectPersona.PATERNO;
                TextMaterno = SelectPersona.MATERNO;
                var externo = SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == 4 && w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_EXTERNA);
                TextNIP = externo.Any() ? externo.FirstOrDefault().NIP.ToString() : "0";
                SelectDiscapacitado = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                SelectDiscapacidad = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : (short)0;
                TextHoraEntrada = Fechas.GetFechaDateServer.ToString("HH:mm");
                TextHoraSalida = string.Empty;
                TextInstitucion = string.Empty;
                TextAsunto = string.Empty;
                TextObservaciones = string.Empty;
                TextDepartamento = string.Empty;
                SelectPuesto = -1;
                SelectArea = -1;
                ImagenVisitaExterna = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                    SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        new Imagenes().getImagenPerson();
            }
        }

        private void GetDatosNuevoPadronVisitanteSeleccionado()
        {
            TextNombreNuevo = SelectPersona.NOMBRE;
            TextPaternoNuevo = SelectPersona.PATERNO;
            TextMaternoNuevo = SelectPersona.MATERNO;
            SelectSexoNuevo = SelectPersona.SEXO;
            FechaNacimientoNuevo = SelectPersona.FEC_NACIMIENTO;
            TextTelefonoFijoNuevo = SelectPersona.TELEFONO;
            TextTelefonoMovilNuevo = SelectPersona.TELEFONO_MOVIL;
            TextCorreoNuevo = SelectPersona.CORREO_ELECTRONICO;
            //Modificacion de modelo, PENDIENTE
            if (SelectPersona.PERSONA_EXTERNO != null)
            { 
                SelectTipoVisitanteNuevo = SelectPersona.PERSONA_EXTERNO.ID_TIPO_VISITANTE != null ? SelectPersona.PERSONA_EXTERNO.ID_TIPO_VISITANTE : -1;
                TextObservacionNuevo = SelectPersona.PERSONA_EXTERNO.OBSERV;
                FotoDetrasCredencial = SelectPersona.PERSONA_EXTERNO.CREDENCIAL_DETRAS;
                FotoFrenteCredencial = SelectPersona.PERSONA_EXTERNO.CREDENCIAL_FRENTE;
            }
            
            //SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == 4).FirstOrDefault().NIP.ToString() : string.Empty;
            SelectPais = SelectPersona.ID_PAIS;
            SelectEntidad = SelectPersona.ID_ENTIDAD;
            SelectMunicipio = SelectPersona.ID_MUNICIPIO;
            SelectColonia = SelectPersona.ID_COLONIA;
            TextCalle = SelectPersona.DOMICILIO_CALLE;
            TextNumExt = SelectPersona.DOMICILIO_NUM_EXT;
            TextNumInt = SelectPersona.DOMICILIO_NUM_INT;
            TextCodigoPostal = SelectPersona.DOMICILIO_CODIGO_POSTAL;
            SelectDiscapacitadoNuevo = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
            SelectDiscapacidadNuevo = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : new Nullable<short>();
            FotoPersonaExterna = ImagenVisitaExterna = SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                    new Imagenes().getImagenPerson();
            //Modificacion de modelo, PENDIENTE
            //FotoDetrasCredencial = SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == 4).Any() ?
            //    SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == 4).FirstOrDefault().CREDENCIAL_DETRAS : null;
            //FotoFrenteCredencial = SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == 4).Any() ?
            //    SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == 4).FirstOrDefault().CREDENCIAL_FRENTE : null;
        }

        private void LimpiarRegistroAccesoAduana()
        {
            TextNombre = string.Empty;
            TextPaterno = string.Empty;
            TextMaterno = string.Empty;
            TextNIP = string.Empty;
            SelectDiscapacitado = null;
            SelectDiscapacidad = new Nullable<short>();
            TextHoraEntrada = Fechas.GetFechaDateServer.ToString("HH:mm");
            TextHoraSalida = string.Empty;
            TextInstitucion = string.Empty;
            TextAsunto = string.Empty;
            TextObservaciones = string.Empty;
            TextDepartamento = string.Empty;
            SelectPuesto = null;
            ImagenVisitanteExterno = ImagenVisitaExterna = new Imagenes().getImagenPerson();
            SelectPersona = null;
            SelectDepartamento = -1;
            ExisteNuevo = false;
            base.ClearRules();
            OnPropertyChanged();
        }

        private void LimpiarAgregarVisitaExterna()
        {
            TextNombreNuevo = TextPaternoNuevo = TextMaternoNuevo = TextCorreoNuevo =
                TextObservacionNuevo = TextCalle = TextNumInt = TextNIPNuevo = string.Empty;
            ///TODO: cambiar centro por usuario
            TextTelefonoFijoNuevo = TextTelefonoMovilNuevo = new cCentro().Obtener(4).FirstOrDefault().MUNICIPIO.LADA.ToString();
            TextNumExt = TextCodigoPostal = new Nullable<int>();
            SelectSexoNuevo = "";
            SelectDiscapacitadoNuevo = null;
            SelectDiscapacidadNuevo = SelectTipoVisitante = new Nullable<short>();
            FechaNacimientoNuevo = new Nullable<DateTime>();
            FechaAltaNuevo = Fechas.GetFechaDateServer;
            SelectPersona = null;
            SelectPais = Parametro.PAIS; //82;
            SelectEntidad = Parametro.ESTADO; //2;
            SelectMunicipio = -1;
            ImagenPersona = new Imagenes().getImagenPerson();
            ExisteNuevo = false;
            base.ClearRules();
            OnPropertyChanged();
        }

    }
}