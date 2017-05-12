using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using ControlPenales.Clases.ControlProgramas;
using DPUruNet;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlPenales.Procesos.Actividades.ListaActividades
{
    public partial class LeerInternosViewModel : FingerPrintScanner
    {

        #region Constructor
        public LeerInternosViewModel(AREA areaActividad = null, bool? set442 = null, bool GuardarHuellas = false, MetroWindow ventanaLecturaInterno = null, List<Imputado_Huella> Huellas = null, List<EQUIPO_AREA> Areas = null)
        {
            WindowEnrolamientoInternos = ventanaLecturaInterno;
            Huellas_Imputado = Huellas;
            NIPBuscar = "";
            this.Areas = Areas;
        }
        #endregion

        #region Propiedades

        private List<EQUIPO_AREA> areas;
        public List<EQUIPO_AREA> Areas
        {
            get { return areas; }
            set
            {
                areas = value;
                OnPropertyChanged("Areas");
            }
        }


        private List<InternosActividad> selectedInterno;
        public List<InternosActividad> SelectedInterno
        {
            get { return selectedInterno; }
            set
            {
                selectedInterno = value;
                OnPropertyChanged("SelectedInterno");
            }
        }



        public Imagenes ImagenPlaceHolder { get; set; }

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }


        private List<Imputado_Huella> huellas_Imputado;
        public List<Imputado_Huella> Huellas_Imputado
        {
            get { return huellas_Imputado; }
            set
            {
                huellas_Imputado = value;
                OnPropertyChanged("Huellas_Imputado");
            }
        }

        private Visibility showLine;
        public Visibility ShowLine
        {
            get { return showLine; }
            set
            {
                showLine = value;
                OnPropertyChanged("ShowLine");
            }
        }


        private Visibility showImagenInterno;
        public Visibility ShowImagenInterno
        {
            get { return showImagenInterno; }
            set
            {
                showImagenInterno = value;
                OnPropertyChanged("ShowImagenInterno");
            }
        }

        private byte[] imagenInterno;
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set
            {
                imagenInterno = value; OnPropertyChanged("ImagenInterno");
            }
        }

        private bool progressRingVisible;
        public bool ProgressRingVisible
        {
            get { return progressRingVisible; }
            set
            {
                progressRingVisible = value; OnPropertyChanged("ProgressRingVisible");
            }
        }


        private bool asistenciaBiometricaSelected;
        public bool AsistenciaBiometricaSelected
        {
            get { return asistenciaBiometricaSelected; }
            set
            {
                asistenciaBiometricaSelected = value;
                OnPropertyChanged("AsistenciaBiometricaSelected");
            }
        }




        private bool asistenciaBiometricaEnabled;
        public bool AsistenciaBiometricaEnabled
        {
            get { return asistenciaBiometricaEnabled; }
            set
            {
                asistenciaBiometricaEnabled = value;
                OnPropertyChanged("AsistenciaBiometricaEnabled");
            }
        }


        private bool asistenciaNIPSelected;
        public bool AsistenciaNIPSelected
        {
            get { return asistenciaNIPSelected; }
            set
            {
                asistenciaNIPSelected = value;
                OnPropertyChanged("AsistenciaNIPSelected");
            }
        }

        private string nipBuscar;
        public string NIPBuscar
        {
            get { return nipBuscar; }
            set
            {
                nipBuscar = value;
                OnPropertyChanged("NIPBuscar");

            }
        }

        private string textoBorrarIcono;
        public string TextoBorrarIcono
        {
            get { return textoBorrarIcono; }
            set
            {
                textoBorrarIcono = value;
                OnPropertyChanged("TextoBorrarIcono");
            }
        }


        private string borrarTodoIcono;
        public string BorrarTodoIcono
        {
            get { return borrarTodoIcono; }
            set
            {
                borrarTodoIcono = value;
                OnPropertyChanged("BorrarTodoIcono");
            }
        }

        private string checkMark;
        public string CheckMark
        {
            get { return checkMark; }
            set
            {
                checkMark = value;
                OnPropertyChanged("CheckMark");
            }
        }

        private bool imagenHuellaVisible;
        public bool ImagenHuellaVisible
        {
            get { return imagenHuellaVisible; }
            set
            {
                imagenHuellaVisible = value;
                OnPropertyChanged("ImagenHuellaVisible");
            }
        }


        private Visibility capturaNIPVisible;
        public Visibility CapturaNIPVisible
        {
            get { return capturaNIPVisible; }
            set
            {
                capturaNIPVisible = value;
                OnPropertyChanged("CapturaNIPVisible");
            }
        }


        private Visibility capturaHuellaVisible;
        public Visibility CapturaHuellaVisible
        {
            get { return capturaHuellaVisible; }
            set
            {
                capturaHuellaVisible = value;
                OnPropertyChanged("CapturaHuellaVisible");
            }
        }

        private byte[] imagen;
        public byte[] Imagen
        {
            get { return imagen; }
            set
            {
                imagen = value;
                OnPropertyChanged("Imagen");
            }
        }

        private MetroWindow WindowEnrolamientoInternos { get; set; }

        private ResultadoBusquedaBiometrico _SelectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _SelectRegistro; }
            set
            {
                _SelectRegistro = value;
                OnPropertyChanged("SelectRegistro");
            }
        }

        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }

        private System.Windows.Media.Brush colorAprobacion;
        public System.Windows.Media.Brush ColorAprobacion
        {
            get { return colorAprobacion; }
            set
            {
                colorAprobacion = value;
                RaisePropertyChanged("ColorAprobacion");
            }
        }

        private Brush colorAprobacionNIP;
        public Brush ColorAprobacionNIP
        {
            get { return colorAprobacionNIP; }
            set
            {
                colorAprobacionNIP = value;
                RaisePropertyChanged("ColorAprobacionNIP");
            }
        }


        private List<InternosActividad> listaImputadosPorHora;
        public List<InternosActividad> ListaImputadosPorHora
        {
            get { return listaImputadosPorHora; }
            set
            {
                listaImputadosPorHora = value;
                RaisePropertyChanged("ListaImputadosPorHora");
            }
        }


        private Brush fondoLimpiarNIP;
        public Brush FondoLimpiarNIP
        {
            get { return fondoLimpiarNIP; }
            set
            {
                fondoLimpiarNIP = value;
                OnPropertyChanged("FondoLimpiarNIP");
            }
        }


        private Brush fondoBackSpaceNIP;
        public Brush FondoBackSpaceNIP
        {
            get { return fondoBackSpaceNIP; }
            set
            {
                fondoBackSpaceNIP = value;
                OnPropertyChanged("FondoBackSpaceNIP");
            }
        }

        #endregion

        #region Commands


        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }

        }


        private ICommand busquedaHuellas;
        public ICommand BusquedaHuellas
        {
            get { return busquedaHuellas ?? (busquedaHuellas = new RelayCommand(SeleccionHuella)); }
        }


        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LeerInternos>(OnLoad); }
        }


        private ICommand buttonMouseEnter;
        public ICommand ButtonMouseEnter
        {
            get { return buttonMouseEnter ?? (buttonMouseEnter = new RelayCommand(MouseEnterSwitch)); }
        }


        private ICommand buttonMouseLeave;
        public ICommand ButtonMouseLeave
        {
            get { return buttonMouseLeave ?? (buttonMouseLeave = new RelayCommand(MouseLeaveSwitch)); }
        }


        #endregion

        #region MetodosEventos


        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
            }
        }


        private void MouseLeaveSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
                    break;
            }
        }


        private void OnLoad(LeerInternos Window)
        {
            ImagenPlaceHolder = new Imagenes();
            WindowEnrolamientoInternos = Window;
            ImagenHuellaVisible = true;
            ProgressRingVisible = false;
            CapturaHuellaVisible = Visibility.Visible;
            CapturaNIPVisible = Visibility.Collapsed;
            Imagen = new Imagenes().getImagenHuella();
            ImagenInterno = ImagenPlaceHolder.getImagenPerson();
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            CheckMark = "🔍";
            DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
            TextoBorrarIcono = "\u232B";
            BorrarTodoIcono = "\u2718";
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            #region [Huellas Digitales]
            var myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 185;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTargetName(myDoubleAnimation, "Ln");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(Window.Ln);
            #endregion

            Window.Closed += (s, e) =>
            {
                try
                {
                    if (OnProgress == null)
                        return;

                    if (!_IsSucceed)
                        SelectRegistro = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lectura de las huellas", ex);
                }
            };

            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            CurrentReader = Readers[0];

            if (CurrentReader == null)
                return;

            if (!OpenReader())
                Window.Close();

            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));

            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorAprobacion = new SolidColorBrush(Colors.Green);
                ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            }));

        }


        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "terminarEnrolamiento":
                    LeerInternos.GetWindow(WindowEnrolamientoInternos).Close();
                    break;
                case "onBuscarPorNIP":
                    CompararNIPAsistenciaInterno();
                    break;
                case "0":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "0";
                    }
                    break;
                case "1":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "1";
                    }
                    break;
                case "2":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "2";
                    }
                    break;
                case "3":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "3";
                    }
                    break;
                case "4":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "4";
                    }
                    break;
                case "5":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "5";
                    }
                    break;
                case "6":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "6";
                    }
                    break;
                case "7":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "7";
                    }
                    break;
                case "8":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "8";
                    }
                    break;
                case "9":
                    if (NIPBuscar.Length < 10)
                    {
                        NIPBuscar += "9";
                    }
                    break;
                case "backspace":
                    if (NIPBuscar.Length > 0)
                    {
                        NIPBuscar = NIPBuscar.Substring(0, NIPBuscar.Length - 1);
                    }
                    break;
                case "limpiarNIP":
                    NIPBuscar = "";
                    break;
            }

        }



        private void SeleccionHuella(Object obj)
        {
            var fecha_server = Fechas.GetFechaDateServer;

            var biometrico = (enumTipoBiometrico)obj;

            //QUERY ERNESTO
            //Huellas_Imputado = new cGrupoAsistencia().GetData().Where(w => w.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year && w.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month && w.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day && w.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour
            //   && w.GRUPO_HORARIO.ID_AREA == GlobalVar.gArea).SelectMany(s => s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)biometrico && w.BIOMETRICO != null).AsEnumerable().Select(s => new Imputado_Huella
            //   {
            //       IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
            //       FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
            //       tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
            //   })
            //       .ToList();

            var grupo_asistencia = new cGrupoAsistencia();

            foreach (var item in Areas)
            {
                Huellas_Imputado.AddRange(grupo_asistencia.GetData().Where(w => w.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year && w.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month && w.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day && w.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour
               && w.GRUPO_HORARIO.ID_AREA == item.ID_AREA).SelectMany(s => s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w => w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)biometrico && w.BIOMETRICO != null).AsEnumerable().Select(s => new Imputado_Huella
               {
                   IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                   FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                   tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
               })
                   .ToList());
            }
        }


        #endregion

        #region Metodos


        public override void OnCaptured(CaptureResult captureResult)
        {

            try
            {
                base.OnCaptured(captureResult);
                ImagenHuellaVisible = false;
                ProgressRingVisible = true;
                CompararHuellaImputado(DD_Dedo);
                ShowImagenInterno = Visibility.Visible;
                ImagenHuellaVisible = true;
                ProgressRingVisible = false;
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
            }

        }


        private async void NotificarResultadoLecturaNIP(enumResultadoAsistencia Resultado)
        {
            var color = new SolidColorBrush();
            switch (Resultado)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    color.Color = Colors.Red;
                    ColorAprobacionNIP = color;
                    CheckMark = "X";
                    break;
                case enumResultadoAsistencia.INTERNO_NO_AUTORIZADO:
                    color.Color = Colors.Red;
                    ColorAprobacionNIP = color;
                    CheckMark = "X";
                    break;
                case enumResultadoAsistencia.ASISTENCIA_CAPTURADA:
                    color.Color = Colors.DarkOrange;
                    ColorAprobacionNIP = color;
                    CheckMark = "\u2713 \u2713";
                    break;
                case enumResultadoAsistencia.ASISTENCIA_PREVIA:
                    color.Color = Colors.Green;
                    ColorAprobacionNIP = color;
                    CheckMark = "!!";
                    break;
            }

            await TaskEx.Delay(1500);
            color.Color = Colors.DarkBlue;
            ColorAprobacionNIP = color;
            CheckMark = "🔍";
        }


        private void CompararNIPAsistenciaInterno()
        {


            var NIP = 0;
            int.TryParse(NIPBuscar, out NIP);
            var fecha_server = Fechas.GetFechaDateServer;
            GRUPO_ASISTENCIA participacion_elegida = null;
            try
            {
                List<GRUPO_ASISTENCIA> participaciones = new List<GRUPO_ASISTENCIA>();
                var grupo_asistencia = new cGrupoAsistencia();
                foreach (var item in Areas)
                {
                    participaciones.AddRange(grupo_asistencia.GetData(g =>
                g.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day &&
                g.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month &&
                g.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year &&
                g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour).Where(w =>
                w.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP == NIP &&
                w.GRUPO_HORARIO.ID_AREA == item.ID_AREA));
                }



                var participacion_unica = participaciones.Where(w =>
                (w.EMP_COORDINACION == 0 || (w.EMP_COORDINACION == 2 && w.EMP_APROBADO == 1))).FirstOrDefault();

                var participaciones_sin_resolucion = participaciones.Where(w =>
                (w.EMP_COORDINACION == 1)).ToList();


                participacion_elegida = participacion_unica != null ? participacion_unica : (participaciones_sin_resolucion != null ? participaciones_sin_resolucion.OrderBy(o => o.FEC_REGISTRO).ToList().FirstOrDefault() : null);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar la actividad a la que pertenece el interno.", ex);

            }

            enumResultadoAsistencia resultado = enumResultadoAsistencia.EN_PROCESO;

            if (participacion_elegida != null)
            {
                try
                {

                    var tiempo_tolerancia = (participacion_elegida.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - participacion_elegida.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1 ? Parametro.TOLERANCIA_ASISTENCIA_HORAS : Parametro.TOLERANCIA_ASISTENCIA_HORA;

                    if (fecha_server.Minute < tiempo_tolerancia)
                    {
                        var Autorizado = new cIngresoUbicacion().Obtener(
                            (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                            (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                            (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Where(w =>
                            w.ESTATUS == 1).FirstOrDefault();





                        if (Autorizado != null)
                        {

                            var Asistencia = participacion_elegida.ASISTENCIA;
                            if (Asistencia == null)
                            {
                                var ingreso_ubicacion = new cIngresoUbicacion().Actualizar(new INGRESO_UBICACION()
                                {
                                    ID_CENTRO = Autorizado.ID_CENTRO,
                                    ID_ANIO = Autorizado.ID_ANIO,
                                    ID_IMPUTADO = Autorizado.ID_IMPUTADO,
                                    ID_INGRESO = Autorizado.ID_INGRESO,
                                    ID_CONSEC = Autorizado.ID_CONSEC,
                                    ID_CUSTODIO = null,
                                    ESTATUS = 2,
                                    MOVIMIENTO_FEC = fecha_server
                                });

                                var grupo_asistencia = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                {
                                    ID_CENTRO = participacion_elegida.ID_CENTRO,
                                    ID_ACTIVIDAD = participacion_elegida.ID_ACTIVIDAD,
                                    ID_TIPO_PROGRAMA = participacion_elegida.ID_TIPO_PROGRAMA,
                                    ID_GRUPO = participacion_elegida.ID_GRUPO,
                                    ID_CONSEC = participacion_elegida.ID_CONSEC,
                                    ID_GRUPO_HORARIO = participacion_elegida.ID_GRUPO_HORARIO,
                                    ASISTENCIA = 1
                                });
                                resultado = enumResultadoAsistencia.ASISTENCIA_CAPTURADA;
                            }
                            else
                            {
                                resultado = enumResultadoAsistencia.ASISTENCIA_PREVIA;
                            }

                        }
                        else
                        {
                            resultado = enumResultadoAsistencia.INTERNO_NO_AUTORIZADO;
                        }

                    }
                    else
                    {
                        resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                    }

                }
                catch (Exception ex)
                {

                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar la asistencia del interno.", ex);
                }

            }
            else
            {
                resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
            }


            NotificarResultadoLecturaNIP(resultado);



        }

        //METODO ACTUAL
        private void CompararHuellaImputado(enumTipoBiometrico? Biometrico)
        {

            //SE OBTIENE LA INFORMACIÓN DE LA HUELLA Y SE ALMACENA
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;

            Color color = Colors.Transparent;


            //SE VERIFICA SI HAY INFORMACIÓN DE LA HUELLA EN LA LECTURA.
            if (bytesHuella == null)
            {
                //SI NO HAY INFORMACIÓN, SE NOTIFICA QUE LA HUELLA SEA CAPTURADA DE NUEVO
                Imagen = new Imagenes().getImagenDenegado();
                color = Colors.Red;
                ScannerMessage = "Capture de nuevo";
                ShowLine = Visibility.Collapsed;
                PropertyImage = null;
                return;
            }

            //SE NOTIFICA QUE LA HUELLA ESTA SIENDO PROCESADA...
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Procesando...";
                ColorAprobacion = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                ShowLine = Visibility.Visible;
                ShowImagenInterno = Visibility.Collapsed;
            }));

            #region PROCESAMIENTO_HUELLAS
            //SE VERIFICA SI EXISTEN HUELLAS CON LAS CUALES COMPARAR (ES DECIR, SI HAY GRUPO ALGUNO)...
            if (Huellas_Imputado.Count == 0)
            {
                //SI NO HAY HUELLAS CON LAS CUALES COMPARAR, SE NOTIFICA AL INTERNO QUE NO TIENE PERMITIDO EL ACCESO AL ÁREA Y QUE NO HAY ACTIVIDADES EN CURSO
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Imagen = new Imagenes().getImagenDenegado();
                    color = Colors.Red;
                    ScannerMessage = "Acceso Negado,\nno hay actividades.";
                    ShowLine = Visibility.Collapsed;
                    PropertyImage = null;
                    ColorAprobacion = new SolidColorBrush(color);
                }));
            }

            //SI HAY GRUPOS ACTIVOS EN DETERMINADO MOMENTO, ENTONCES...
            else
            {
                #region COMPARACION_HUELLA
                //SE REALIZA LA COMPARACIÓN DE HUELLAS DE LOS GRUPOS CON LA HUELLA OBTENIDA POR MEDIO DEL LECTOR
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, Huellas_Imputado.Where(w => w.FMD != null && w.tipo_biometrico == Biometrico).Select(s => s.FMD), (0x7fffffff / 100000), 10);



                //INICIALIZA VARIABLES DE VERIFICACION
                var identify = false;
                var result = new List<object>();


                //SI LA OPERACIÓN NO FUE EXITOSA, SE ENTRA EN UN ESTADO DE VERIFICACIÓN DE LA OPERACIÓN...
                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    //SE OBTIENE EL RESULTADO DE LA OPERACIÓN Y SE REALIZA UN ACCIÓN A CONTINUACIÓN... 
                    switch (doIdentify.ResultCode)
                    {
                        //=====> TODO: VALIDAR RESULTADOS DE LA OPERACIÓN
                    }
                }
                else

                //SI LA OPERACIÓN DE COMPARACIÓN FUE EXITOSA Y NO HUBO NINGÚN PROBLEMA, ENTONCES...
                {



                    //SE VERIFICA LA CANTIDAD DE COINCIDENCIAS QUE EXISTEN EN LA COMPARACION. 
                    if (doIdentify.Indexes.Count() > 0)
                    {
                        //SI HAY COINCIDENCIAS, SE ALMACENAN TODAS EN EL ARREGLO DE RESULTADOS...
                        foreach (var item in doIdentify.Indexes.ToList())
                            result.Add(Huellas_Imputado[item.FirstOrDefault()].IMPUTADO);


                        //SE MARCA LA IDENTIFICACION DE LA HUELLA
                        identify = true;
                    }
                    //SI NO EXISTEN COINCIDENCIAS, ENTONCES SE INDICA QUE NO HUBO IDENTIFICACION DE LA HUELLA
                    else
                        identify = false;


                    #region VERIFICACIÓN_INTERNO_EN_GRUPOS
                    //SI HUBO IDENTIFICACION DE LA HUELLA, ENTONCES....
                    if (identify)
                    {
                        #region VERIFICACION_COINCIDENCIA_HUELLAS
                        //SE VERIFICA LA CANTIDAD DE COINCIDENCIAS. SI HUBO MAS COINCIDENCIAS, ENTONCES....
                        if (result.Count() > 1)
                        {
                            //SE AGREGAN LOS INTERNOS RESULTANTES DE LAS COINCIDENCIAS
                            var internos = new List<cHuellasImputado>();
                            foreach (var item in result)
                            {
                                internos.Add((cHuellasImputado)item);
                            }

                            //SE DETERMINA CUANTOS INTERNOS EXISTEN "REALMENTE" EN LAS COINCIDENCIAS
                            var internos_en_coincidencias = internos.OrderBy(o =>
                                o.ID_CENTRO).ThenBy(t =>
                                    t.ID_ANIO).ThenBy(t2 =>
                                        t2.ID_IMPUTADO).GroupBy(g =>
                                            g.ID_IMPUTADO).Distinct().ToList().Count;

                            //SI EXISTE MAS DE UN INTERNO, SE MUESTRA MENSAJE DE COINCIDENCIAS
                            if (internos_en_coincidencias != 1)
                            {
                                Imagen = new Imagenes().getImagenAdvertencia();
                                color = Colors.DarkOrange;
                                ScannerMessage = "Más de una coincidencia.\nCapture de nuevo";
                                ShowLine = Visibility.Collapsed;
                                PropertyImage = null;
                            }
                            else
                            //DE OTRO MODO, SI SE TIENE UN SOLO INTERNO EN LAS COINCIDENCIAS, ENTONCES...
                            {
                                /*EL INTERNO TIENE UN EMPALME, YA QUE SE ENCONTRÓ SU HUELLA MÁS DE 1 VEZ, DEBIDO A QUE TIENE MÁS DE UNA ACTIVIDAD
                                EN LAS ÁREAS QUE CONTEMPLA EL EQUIPO, POR LO CUAL, SE SIGUE EL PROCESO NORMAL DEL EMPALME...*/

                                #region CANTIDAD DE INTERNOS EN COINCIDENCIAS  = 1
                                //SE OBTIENE INFORMACION DEL INTERNO
                                var interno = (cHuellasImputado)result[0];

                                //SE OBTIENE FECHA DEL SERVIDOR
                                var fecha_server = Fechas.GetFechaDateServer;

                                //SE OBTIENEN LAS PARTICIPACIONES DEL INTERNO
                                var participaciones = new cGrupoAsistencia().GetData(g =>
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour &&
                                    g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == interno.ID_CENTRO &&
                                    g.GRUPO_PARTICIPANTE.ING_ID_ANIO == interno.ID_ANIO &&
                                    g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == interno.ID_IMPUTADO).ToList();


                                //SE VERIFICA SI TIENE MAS DE UNA PARTICIPACIÓN
                                #region PARTICIPACIONES_COINCIDENCIAS
                                if (participaciones.Count > 1)
                                {
                                    #region RESOLUCION_EMPALME_PARTICIPACIONES_COINCIDENCIAS
                                    //SI TIENE MAS DE UNA PARTICIPACIÓN, ENTONCES... 
                                    var resolucion = participaciones.Where(w =>
                                        w.EMP_COORDINACION == 2 && w.EMP_APROBADO == 1).FirstOrDefault();



                                    //SE REVISA SI COORDINACIÓN YA RESOLVIÓ EL EMPALME DE HORAS DEL INTERNO
                                    if (resolucion != null)
                                    {
                                        //SE VERIFICA SI EL AREA DE LA RESOLUCION EXISTE EN LAS AREAS QUE CONTEMPLA EL EQUIPO

                                        var area = Areas.Where(w =>
                                            w.ID_AREA == resolucion.GRUPO_HORARIO.ID_AREA).FirstOrDefault();
                                        if (area != null)
                                        {


                                            //SI EL EMPALME FUE RESUELTO, SE VERIFICA LA ASISTENCIA
                                            var asistencia = resolucion.ASISTENCIA;


                                            #region ASISTENCIA_INTERNO_EMPALME

                                            //SI LA ASISTENCIA NO HA SIDO MARCADA, ENTONCES...
                                            if (asistencia == null)
                                            {
                                                #region AUTORIZACION_INTERNO_EMPALME
                                                //SE OBTIENE LA AUTORIZACIÓN DEL INTERNO
                                                var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w =>
                                                    w.ID_ANIO == interno.ID_ANIO &&
                                                    w.ID_CENTRO == interno.ID_CENTRO &&
                                                    w.ID_IMPUTADO == interno.ID_IMPUTADO &&
                                                    w.ESTATUS == 1).FirstOrDefault();

                                                //SE VERIFICA LA AUTORIZACIÓN DEL INTERNO
                                                if (Autorizado != null)
                                                {
                                                    //SI EL INTERNO ESTA AUTORIZADO DESDE EL CONTROL DE INTERNOS,
                                                    //SE VERIFICA SI NO HA PASADO EL TIEMPO DE TOLERANCIA. SI NO HA PASADO LA TOLERANCIA, ENTONCES....
                                                    var tiempo_tolerancia = 0;
                                                    if ((resolucion.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - resolucion.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1)
                                                    {
                                                        tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORAS;
                                                    }
                                                    else
                                                    {
                                                        tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORA;
                                                    }

                                                    if (fecha_server.Minute < tiempo_tolerancia)
                                                    {


                                                        //SE OBTIENE LA INFORMACION DEL INTERNO
                                                        var ingreso = new cImputado().Obtener(interno.ID_IMPUTADO, interno.ID_ANIO, interno.ID_CENTRO).FirstOrDefault().INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                                        var InternoAusente = new cIngresoUbicacion();

                                                        //SE CAMBIA LA UBICACION DEL INTERNO AL AREA DE LA ACTIVIDAD
                                                        InternoAusente.Eliminar(interno.ID_CENTRO, interno.ID_ANIO, interno.ID_IMPUTADO, ingreso);
                                                        InternoAusente.Insertar(new INGRESO_UBICACION()
                                                        {
                                                            ESTATUS = 2,
                                                            ID_AREA = resolucion.GRUPO_HORARIO.ID_AREA,
                                                            ID_ANIO = interno.ID_ANIO,
                                                            ID_CENTRO = interno.ID_CENTRO,
                                                            ID_IMPUTADO = interno.ID_IMPUTADO,
                                                            ID_CONSEC = resolucion.ID_CONSEC,
                                                            ID_INGRESO = ingreso,
                                                            MOVIMIENTO_FEC = fecha_server,
                                                            ACTIVIDAD = resolucion.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR
                                                        });


                                                        //SE MARCA LA ASISTENCIA DEL INTERNO
                                                        var asistencia_interno = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                        {
                                                            ID_CENTRO = resolucion.ID_CENTRO,
                                                            ID_ACTIVIDAD = resolucion.ID_ACTIVIDAD,
                                                            ID_TIPO_PROGRAMA = resolucion.ID_TIPO_PROGRAMA,
                                                            ID_GRUPO = resolucion.ID_GRUPO,
                                                            ID_CONSEC = resolucion.ID_CONSEC,
                                                            ID_GRUPO_HORARIO = resolucion.ID_GRUPO_HORARIO,
                                                            ASISTENCIA = 1
                                                        });

                                                        //SE NOTIFICA LA ASISTENCIA CAPTURADA
                                                        #region NOTIFICACION_ASISTENCIA_CAPTURADA_EMPALME
                                                        Imagen = new Imagenes().getImagenPermitido();
                                                        ScannerMessage = "Asistencia capturada";
                                                        ShowLine = Visibility.Collapsed;

                                                        //MUESTREO DE IMAGEN: SI NO EXISTE LA IMAGEN DE SEGUIMIENTO, ENTONCES SE TOMA LA IMAGEN DE REGISTRO; SI NINGUNA DE ELLAS EXISTE, SE MUESTRA EL PLACEHOLDER
                                                        var imagen_foto_seguimiento = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                            new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                                                        var imagen_foto_registro = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                            new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;


                                                        ImagenInterno = imagen_foto_seguimiento == null ? (imagen_foto_registro == null ? new Imagenes().getImagenPerson() : imagen_foto_registro) : imagen_foto_seguimiento;

                                                        ShowImagenInterno = Visibility.Visible;
                                                        PropertyImage = null;
                                                        color = Colors.Green;
                                                        #endregion
                                                    }
                                                    //SI LA TOLERANCIA YA PASO, ENTONCES...
                                                    else
                                                    {
                                                        //SE NOTIFICA QUE EL INTERNO NO TIENE ACCESO AL AREA
                                                        Imagen = new Imagenes().getImagenDenegado();
                                                        color = Colors.Red;
                                                        ScannerMessage = "Interno no permitido";
                                                        ShowLine = Visibility.Collapsed;
                                                        PropertyImage = null;
                                                    }
                                                }
                                                else
                                                {
                                                    Imagen = new Imagenes().getImagenDenegado();
                                                    color = Colors.Red;
                                                    ScannerMessage = "Interno no autorizado";
                                                    ShowLine = Visibility.Collapsed;
                                                    PropertyImage = null;
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                //SI LA ASISTENCIA FUE MARCADA, SE NOTIFICA QUE LA ASISTENCIA YA FUE CAPTURADA, INDEPENDIENTEMENTE DE SI ES UNA ASISTENCIA O AUSENCIA A LA ACTIVIDAD
                                                Imagen = new Imagenes().getImagenAdvertencia();
                                                ScannerMessage = "Asistencia previa";
                                                ShowLine = Visibility.Collapsed;
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                                PropertyImage = null;
                                                color = Colors.DarkOrange;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            //SE NOTIFICA QUE EL INTERNO NO TIENE PERMITIDO EL ACCESO AL ÁREA
                                            Imagen = new Imagenes().getImagenDenegado();
                                            color = Colors.Red;
                                            ScannerMessage = "Interno no permitido";
                                            ShowLine = Visibility.Collapsed;
                                            PropertyImage = null;
                                        }
                                    }
                                    else
                                    {
                                        //SI EL EMPALME NO HA SIDO RESUELTO, SE OBTIENE LA ACTIVIDAD QUE TENGA MAS ANTIGUEDAD DE REGISTRO

                                        var actividad_fecha_mayor_antiguedad = participaciones.OrderBy(o => o.GRUPO_HORARIO.HORA_INICIO).FirstOrDefault();

                                        //SE REVISA SI ESTA ACTIVIDAD SE ENCUENTRA DENTRO DE LAS AREAS QUE EL EQUIPO CONTEMPLA
                                        var area = Areas.Where(w =>
                                            w.ID_AREA == actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.ID_AREA).FirstOrDefault();
                                        if (area != null)
                                        {
                                            //SI ESA ACTIVIDAD PERTENECE AL GRUPO DE AREAS DEL EQUIPO, ENTONCES...
                                            #region ASISTENCIA_INTERNO_CON_EMPALME_SIN_RESOLUCIÓN

                                            var asistencia = actividad_fecha_mayor_antiguedad.ASISTENCIA;




                                            //SI LA ASISTENCIA NO HA SIDO MARCADA, ENTONCES...
                                            if (asistencia == null)
                                            {
                                                #region AUTORIZACION_INTERNO_EMPALME_COINCIDENCIAS
                                                //SE OBTIENE LA AUTORIZACIÓN DEL INTERNO
                                                var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w =>
                                                    w.ID_ANIO == interno.ID_ANIO &&
                                                    w.ID_CENTRO == interno.ID_CENTRO &&
                                                    w.ID_IMPUTADO == interno.ID_IMPUTADO &&
                                                    w.ESTATUS == 1).FirstOrDefault();

                                                //SE VERIFICA LA AUTORIZACIÓN DEL INTERNO
                                                if (Autorizado != null)
                                                {

                                                    var tiempo_tolerancia = 0;
                                                    if ((resolucion.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - resolucion.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1)
                                                    {
                                                        tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORAS;
                                                    }
                                                    else
                                                    {
                                                        tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORA;
                                                    }


                                                    if (fecha_server.Minute < tiempo_tolerancia)
                                                    {
                                                        //SI EL INTERNO ESTA AUTORIZADO DESDE EL CONTROL DE INTERNOS, SE OBTIENE SU INFORMACION
                                                        var ingreso = new cImputado().Obtener(interno.ID_IMPUTADO, interno.ID_ANIO, interno.ID_CENTRO).FirstOrDefault().INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                                        var InternoAusente = new cIngresoUbicacion();

                                                        //SE CAMBIA LA UBICACION DEL INTERNO AL AREA DE LA ACTIVIDAD
                                                        InternoAusente.Eliminar(interno.ID_CENTRO, interno.ID_ANIO, interno.ID_IMPUTADO, ingreso);
                                                        InternoAusente.Insertar(new INGRESO_UBICACION()
                                                        {
                                                            ESTATUS = 2,
                                                            ID_AREA = actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.ID_AREA,
                                                            ID_ANIO = interno.ID_ANIO,
                                                            ID_CENTRO = interno.ID_CENTRO,
                                                            ID_IMPUTADO = interno.ID_IMPUTADO,
                                                            ID_CONSEC = actividad_fecha_mayor_antiguedad.ID_CONSEC,
                                                            ID_INGRESO = ingreso,
                                                            MOVIMIENTO_FEC = fecha_server,
                                                            ACTIVIDAD = actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR
                                                        });


                                                        //SE MARCA LA ASISTENCIA DEL INTERNO
                                                        var asistencia_interno = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                        {
                                                            ID_CENTRO = actividad_fecha_mayor_antiguedad.ID_CENTRO,
                                                            ID_ACTIVIDAD = actividad_fecha_mayor_antiguedad.ID_ACTIVIDAD,
                                                            ID_TIPO_PROGRAMA = actividad_fecha_mayor_antiguedad.ID_TIPO_PROGRAMA,
                                                            ID_GRUPO = actividad_fecha_mayor_antiguedad.ID_GRUPO,
                                                            ID_CONSEC = actividad_fecha_mayor_antiguedad.ID_CONSEC,
                                                            ID_GRUPO_HORARIO = actividad_fecha_mayor_antiguedad.ID_GRUPO_HORARIO,
                                                            ASISTENCIA = 1
                                                        });

                                                        //SE NOTIFICA LA ASISTENCIA CAPTURADA
                                                        #region NOTIFICACION_ASISTENCIA_CAPTURADA_EMPALME_COINCIDENCIAS
                                                        Imagen = new Imagenes().getImagenPermitido();
                                                        ScannerMessage = "Asistencia capturada";
                                                        ShowLine = Visibility.Collapsed;

                                                        //MUESTREO DE IMAGEN: SI NO EXISTE LA IMAGEN DE SEGUIMIENTO, ENTONCES SE TOMA LA IMAGEN DE REGISTRO; SI NINGUNA DE ELLAS EXISTE, SE MUESTRA EL PLACEHOLDER
                                                        var imagen_foto_seguimiento = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                            new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                                                        var imagen_foto_registro = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                            new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;


                                                        ImagenInterno = imagen_foto_seguimiento == null ? (imagen_foto_registro == null ? new Imagenes().getImagenPerson() : imagen_foto_registro) : imagen_foto_seguimiento;

                                                        ShowImagenInterno = Visibility.Visible;
                                                        PropertyImage = null;
                                                        color = Colors.Green;
                                                        #endregion

                                                    }
                                                    else
                                                    {
                                                        //SE NOTIFICA QUE EL INTERNO NO TIENE ACCESO AL AREA
                                                        Imagen = new Imagenes().getImagenDenegado();
                                                        color = Colors.Red;
                                                        ScannerMessage = "Interno no permitido";
                                                        ShowLine = Visibility.Collapsed;
                                                        PropertyImage = null;
                                                    }

                                                }

                                                else
                                                {
                                                    //SI EL INTERNO NO ESTA AUTORIZADO, SE NOTIFICA QUE NO TIENE PERMITIDO EL ACCESO AL ÁREA
                                                    Imagen = new Imagenes().getImagenDenegado();
                                                    color = Colors.Red;
                                                    ScannerMessage = "Interno no autorizado";
                                                    ShowLine = Visibility.Collapsed;
                                                    PropertyImage = null;
                                                }
                                                #endregion

                                            }
                                            else
                                            {
                                                //SI LA ASISTENCIA FUE MARCADA, SE NOTIFICA QUE LA ASISTENCIA YA FUE CAPTURADA, INDEPENDIENTEMENTE DE SI ES UNA ASISTENCIA O AUSENCIA A LA ACTIVIDAD
                                                Imagen = new Imagenes().getImagenAdvertencia();
                                                ScannerMessage = "Asistencia previa";
                                                ShowLine = Visibility.Collapsed;
                                                ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                                PropertyImage = null;
                                                color = Colors.DarkOrange;
                                            }




                                            #endregion


                                        }
                                        else
                                        {
                                            /*SI ESA ACTIVIDAD NO PERTENECE AL GRUPO DE AREAS DEL EQUIPO, ENTONCES
                                              SE NOTIFICA AL INTERNO QUE NO TIENE PERMITIDO EL ACCESO A ESA AREA*/

                                            Imagen = new Imagenes().getImagenDenegado();
                                            color = Colors.Red;
                                            ScannerMessage = "Interno no permitido";
                                            ShowLine = Visibility.Collapsed;
                                            PropertyImage = null;

                                        }


                                    }
                                    #endregion
                                }
                                #endregion

                                #endregion

                            }

                        }
                        //SI SOLO HUBO UNA SOLA COINCIDENCIA, ENTONCES...
                        else
                        {
                            #region  COINCIDENCIA = 1

                            //SE OBTIENE INFORMACION DEL INTERNO
                            var interno = (cHuellasImputado)result[0];

                            //SE OBTIENE FECHA DEL SERVIDOR
                            var fecha_server = Fechas.GetFechaDateServer;

                            //SE OBTIENEN LAS PARTICIPACIONES DEL INTERNO
                            var participaciones = new cGrupoAsistencia().GetData(g =>
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour &&
                                g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == interno.ID_CENTRO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_ANIO == interno.ID_ANIO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == interno.ID_IMPUTADO).ToList();

                            //SE VERIFICA SI TIENE MAS DE UNA PARTICIPACIÓN

                            #region PARTICIPACIONES > 1
                            if (participaciones.Count > 1)
                            {
                                #region RESOLUCIÓN_EMPALME_PARTICIPACIONES
                                //SI TIENE MAS DE UNA PARTICIPACIÓN, ENTONCES... 
                                var resolucion = participaciones.Where(w =>
                                    w.EMP_COORDINACION == 2 && w.EMP_APROBADO == 1).FirstOrDefault();



                                //SE REVISA SI COORDINACIÓN YA RESOLVIÓ EL EMPALME DE HORAS DEL INTERNO
                                if (resolucion != null)
                                {
                                    //SE VERIFICA SI EL AREA DE LA RESOLUCION EXISTE EN LAS AREAS QUE CONTEMPLA EL EQUIPO

                                    var area = Areas.Where(w =>
                                        w.ID_AREA == resolucion.GRUPO_HORARIO.ID_AREA).FirstOrDefault();
                                    if (area != null) // CONDICION ANTERIOR: resolucion.GRUPO_HORARIO.ID_AREA == area.ID_AREA
                                    {


                                        //SI EL EMPALME FUE RESUELTO, SE VERIFICA LA ASISTENCIA
                                        var asistencia = resolucion.ASISTENCIA;


                                        #region ASISTENCIA_INTERNO_EMPALME

                                        //SI LA ASISTENCIA NO HA SIDO MARCADA, ENTONCES...
                                        if (asistencia == null)
                                        {
                                            #region AUTORIZACION_INTERNO_EMPALME
                                            //SE OBTIENE LA AUTORIZACIÓN DEL INTERNO
                                            var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w =>
                                                w.ID_ANIO == interno.ID_ANIO &&
                                                w.ID_CENTRO == interno.ID_CENTRO &&
                                                w.ID_IMPUTADO == interno.ID_IMPUTADO &&
                                                w.ESTATUS == 1).FirstOrDefault();

                                            //SE VERIFICA LA AUTORIZACIÓN DEL INTERNO
                                            if (Autorizado != null)
                                            {


                                                var tiempo_tolerancia = 0;
                                                if ((resolucion.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - resolucion.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1)
                                                {
                                                    tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORAS;
                                                }
                                                else
                                                {
                                                    tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORA;
                                                }

                                                if (fecha_server.Minute < tiempo_tolerancia)
                                                {




                                                    //SI EL INTERNO ESTA AUTORIZADO DESDE EL CONTROL DE INTERNOS, SE OBTIENE SU INFORMACION
                                                    var ingreso = new cImputado().Obtener(interno.ID_IMPUTADO, interno.ID_ANIO, interno.ID_CENTRO).FirstOrDefault().INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                                    var InternoAusente = new cIngresoUbicacion();

                                                    //SE CAMBIA LA UBICACION DEL INTERNO AL AREA DE LA ACTIVIDAD
                                                    InternoAusente.Eliminar(interno.ID_CENTRO, interno.ID_ANIO, interno.ID_IMPUTADO, ingreso);
                                                    InternoAusente.Insertar(new INGRESO_UBICACION()
                                                    {
                                                        ESTATUS = 2,
                                                        ID_AREA = resolucion.GRUPO_HORARIO.ID_AREA,
                                                        ID_ANIO = interno.ID_ANIO,
                                                        ID_CENTRO = interno.ID_CENTRO,
                                                        ID_IMPUTADO = interno.ID_IMPUTADO,
                                                        ID_CONSEC = resolucion.ID_CONSEC,
                                                        ID_INGRESO = ingreso,
                                                        MOVIMIENTO_FEC = fecha_server,
                                                        ACTIVIDAD = resolucion.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR
                                                    });


                                                    //SE MARCA LA ASISTENCIA DEL INTERNO
                                                    var asistencia_interno = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                    {
                                                        ID_CENTRO = resolucion.ID_CENTRO,
                                                        ID_ACTIVIDAD = resolucion.ID_ACTIVIDAD,
                                                        ID_TIPO_PROGRAMA = resolucion.ID_TIPO_PROGRAMA,
                                                        ID_GRUPO = resolucion.ID_GRUPO,
                                                        ID_CONSEC = resolucion.ID_CONSEC,
                                                        ID_GRUPO_HORARIO = resolucion.ID_GRUPO_HORARIO,
                                                        ASISTENCIA = 1
                                                    });

                                                    //SE NOTIFICA LA ASISTENCIA CAPTURADA
                                                    #region NOTIFICACION_ASISTENCIA_CAPTURADA_EMPALME
                                                    Imagen = new Imagenes().getImagenPermitido();
                                                    ScannerMessage = "Asistencia capturada";
                                                    ShowLine = Visibility.Collapsed;

                                                    //MUESTREO DE IMAGEN: SI NO EXISTE LA IMAGEN DE SEGUIMIENTO, ENTONCES SE TOMA LA IMAGEN DE REGISTRO; SI NINGUNA DE ELLAS EXISTE, SE MUESTRA EL PLACEHOLDER
                                                    var imagen_foto_seguimiento = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                        new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                                                    var imagen_foto_registro = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                        new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;


                                                    ImagenInterno = imagen_foto_seguimiento == null ? (imagen_foto_registro == null ? new Imagenes().getImagenPerson() : imagen_foto_registro) : imagen_foto_seguimiento;

                                                    ShowImagenInterno = Visibility.Visible;
                                                    PropertyImage = null;
                                                    color = Colors.Green;
                                                    #endregion
                                                }
                                                else
                                                {
                                                    //SE NOTIFICA QUE EL INTERNO NO TIENE ACCESO AL AREA
                                                    Imagen = new Imagenes().getImagenDenegado();
                                                    color = Colors.Red;
                                                    ScannerMessage = "Interno no permitido";
                                                    ShowLine = Visibility.Collapsed;
                                                    PropertyImage = null;
                                                }

                                            }
                                            else
                                            {
                                                Imagen = new Imagenes().getImagenDenegado();
                                                color = Colors.Red;
                                                ScannerMessage = "Interno no autorizado";
                                                ShowLine = Visibility.Collapsed;
                                                PropertyImage = null;
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                            //SI LA ASISTENCIA FUE MARCADA, SE NOTIFICA QUE LA ASISTENCIA YA FUE CAPTURADA, INDEPENDIENTEMENTE DE SI ES UNA ASISTENCIA O AUSENCIA A LA ACTIVIDAD
                                            Imagen = new Imagenes().getImagenAdvertencia();
                                            ScannerMessage = "Asistencia previa";
                                            ShowLine = Visibility.Collapsed;
                                            ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            PropertyImage = null;
                                            color = Colors.DarkOrange;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //SE NOTIFICA QUE EL INTERNO NO TIENE PERMITIDO EL ACCESO AL ÁREA
                                        Imagen = new Imagenes().getImagenDenegado();
                                        color = Colors.Red;
                                        ScannerMessage = "Interno no permitido";
                                        ShowLine = Visibility.Collapsed;
                                        PropertyImage = null;
                                    }
                                }
                                else
                                {
                                    //SI EL EMPALME NO HA SIDO RESUELTO, SE OBTIENE LA ACTIVIDAD QUE TENGA MAS ANTIGUEDAD DE REGISTRO

                                    var actividad_fecha_mayor_antiguedad = participaciones.OrderByDescending(o => o.GRUPO_HORARIO.HORA_INICIO).FirstOrDefault();

                                    //SE REVISA SI ESTA ACTIVIDAD SE ENCUENTRA DENTRO DE LAS AREAS QUE EL EQUIPO CONTEMPLA
                                    var area = Areas.Where(w =>
                                        w.ID_AREA == actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.ID_AREA).FirstOrDefault();
                                    if (area != null)
                                    {
                                        //SI ESA ACTIVIDAD PERTENECE AL GRUPO DE AREAS DEL EQUIPO, ENTONCES...
                                        #region ASISTENCIA_INTERNO_CON_EMPALME_SIN_RESOLUCIÓN

                                        var asistencia = actividad_fecha_mayor_antiguedad.ASISTENCIA;




                                        //SI LA ASISTENCIA NO HA SIDO MARCADA, ENTONCES...
                                        if (asistencia == null)
                                        {
                                            #region AUTORIZACION_INTERNO_SIN_EMPALME
                                            //SE OBTIENE LA AUTORIZACIÓN DEL INTERNO
                                            var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w =>
                                                w.ID_ANIO == interno.ID_ANIO &&
                                                w.ID_CENTRO == interno.ID_CENTRO &&
                                                w.ID_IMPUTADO == interno.ID_IMPUTADO &&
                                                w.ESTATUS == 1).FirstOrDefault();

                                            //SE VERIFICA LA AUTORIZACIÓN DEL INTERNO
                                            if (Autorizado != null)
                                            {

                                                var tiempo_tolerancia = 0;
                                                if ((resolucion.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - resolucion.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1)
                                                {
                                                    tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORAS;
                                                }
                                                else
                                                {
                                                    tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORA;
                                                }


                                                if (fecha_server.Minute < tiempo_tolerancia)
                                                {



                                                    //SI EL INTERNO ESTA AUTORIZADO DESDE EL CONTROL DE INTERNOS, SE OBTIENE SU INFORMACION
                                                    var ingreso = new cImputado().Obtener(interno.ID_IMPUTADO, interno.ID_ANIO, interno.ID_CENTRO).FirstOrDefault().INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                                    var InternoAusente = new cIngresoUbicacion();

                                                    //SE CAMBIA LA UBICACION DEL INTERNO AL AREA DE LA ACTIVIDAD
                                                    InternoAusente.Eliminar(interno.ID_CENTRO, interno.ID_ANIO, interno.ID_IMPUTADO, ingreso);
                                                    InternoAusente.Insertar(new INGRESO_UBICACION()
                                                    {
                                                        ESTATUS = 2,
                                                        ID_AREA = actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.ID_AREA,
                                                        ID_ANIO = interno.ID_ANIO,
                                                        ID_CENTRO = interno.ID_CENTRO,
                                                        ID_IMPUTADO = interno.ID_IMPUTADO,
                                                        ID_CONSEC = actividad_fecha_mayor_antiguedad.ID_CONSEC,
                                                        ID_INGRESO = ingreso,
                                                        MOVIMIENTO_FEC = fecha_server,
                                                        ACTIVIDAD = actividad_fecha_mayor_antiguedad.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR
                                                    });


                                                    //SE MARCA LA ASISTENCIA DEL INTERNO
                                                    var asistencia_interno = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                    {
                                                        ID_CENTRO = actividad_fecha_mayor_antiguedad.ID_CENTRO,
                                                        ID_ACTIVIDAD = actividad_fecha_mayor_antiguedad.ID_ACTIVIDAD,
                                                        ID_TIPO_PROGRAMA = actividad_fecha_mayor_antiguedad.ID_TIPO_PROGRAMA,
                                                        ID_GRUPO = actividad_fecha_mayor_antiguedad.ID_GRUPO,
                                                        ID_CONSEC = actividad_fecha_mayor_antiguedad.ID_CONSEC,
                                                        ID_GRUPO_HORARIO = actividad_fecha_mayor_antiguedad.ID_GRUPO_HORARIO,
                                                        ASISTENCIA = 1
                                                    });

                                                    //SE NOTIFICA LA ASISTENCIA CAPTURADA
                                                    #region NOTIFICACION_ASISTENCIA_CAPTURADA_SIN_EMPALME
                                                    Imagen = new Imagenes().getImagenPermitido();
                                                    ScannerMessage = "Asistencia capturada";
                                                    ShowLine = Visibility.Collapsed;

                                                    //MUESTREO DE IMAGEN: SI NO EXISTE LA IMAGEN DE SEGUIMIENTO, ENTONCES SE TOMA LA IMAGEN DE REGISTRO; SI NINGUNA DE ELLAS EXISTE, SE MUESTRA EL PLACEHOLDER
                                                    var imagen_foto_seguimiento = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                        new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                                                    var imagen_foto_registro = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                        new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;


                                                    ImagenInterno = imagen_foto_seguimiento == null ? (imagen_foto_registro == null ? new Imagenes().getImagenPerson() : imagen_foto_registro) : imagen_foto_seguimiento;

                                                    ShowImagenInterno = Visibility.Visible;
                                                    PropertyImage = null;
                                                    color = Colors.Green;
                                                    #endregion

                                                }
                                                else
                                                {
                                                    //SE NOTIFICA QUE EL INTERNO NO TIENE ACCESO AL AREA
                                                    Imagen = new Imagenes().getImagenDenegado();
                                                    color = Colors.Red;
                                                    ScannerMessage = "Interno no permitido";
                                                    ShowLine = Visibility.Collapsed;
                                                    PropertyImage = null;
                                                }

                                            }

                                            else
                                            {
                                                //SI EL INTERNO NO ESTA AUTORIZADO, SE NOTIFICA QUE NO TIENE PERMITIDO EL ACCESO AL ÁREA
                                                Imagen = new Imagenes().getImagenDenegado();
                                                color = Colors.Red;
                                                ScannerMessage = "Interno no autorizado";
                                                ShowLine = Visibility.Collapsed;
                                                PropertyImage = null;
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                            //SI LA ASISTENCIA FUE MARCADA, SE NOTIFICA QUE LA ASISTENCIA YA FUE CAPTURADA, INDEPENDIENTEMENTE DE SI ES UNA ASISTENCIA O AUSENCIA A LA ACTIVIDAD
                                            Imagen = new Imagenes().getImagenAdvertencia();
                                            ScannerMessage = "Asistencia previa";
                                            ShowLine = Visibility.Collapsed;
                                            ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                            PropertyImage = null;
                                            color = Colors.DarkOrange;
                                        }




                                        #endregion


                                    }
                                    else
                                    {
                                        /*SI ESA ACTIVIDAD NO PERTENECE AL GRUPO DE AREAS DEL EQUIPO, ENTONCES
                                          SE NOTIFICA AL INTERNO QUE NO TIENE PERMITIDO EL ACCESO A ESA AREA*/

                                        Imagen = new Imagenes().getImagenDenegado();
                                        color = Colors.Red;
                                        ScannerMessage = "Interno no permitido";
                                        ShowLine = Visibility.Collapsed;
                                        PropertyImage = null;

                                    }


                                }
                                #endregion

                            }
                            #endregion

                            #region PARTICIPACIONES = 1
                            else
                            {
                                var participacion = participaciones.FirstOrDefault();

                                var area = Areas.Where(w =>
                                        w.ID_AREA == participacion.GRUPO_HORARIO.ID_AREA).FirstOrDefault();

                                //SE VERIFICA QUE LA ACTIVIDAD SEA EN EL AREA(S) ACTUAL(ES)
                                if (area != null)
                                { //CONDICION ANTERIOR: participacion.GRUPO_HORARIO.ID_AREA == GlobalVar.gArea
                                    var asistencia = participacion.ASISTENCIA;


                                    #region ASISTENCIA_INTERNO_SIN_EMPALME

                                    //SI LA ASISTENCIA NO HA SIDO MARCADA, ENTONCES...
                                    if (asistencia == null)
                                    {
                                        #region AUTORIZACION_INTERNO_SIN_EMPALME
                                        //SE OBTIENE LA AUTORIZACIÓN DEL INTERNO
                                        var Autorizado = new cIngresoUbicacion().ObtenerTodos().Where(w =>
                                            w.ID_ANIO == interno.ID_ANIO &&
                                            w.ID_CENTRO == interno.ID_CENTRO &&
                                            w.ID_IMPUTADO == interno.ID_IMPUTADO &&
                                            w.ESTATUS == 1).FirstOrDefault();

                                        //SE VERIFICA LA AUTORIZACIÓN DEL INTERNO
                                        if (Autorizado != null)
                                        {


                                            var tiempo_tolerancia = 0;
                                            if ((participacion.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - participacion.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1)
                                            {
                                                tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORAS;
                                            }
                                            else
                                            {
                                                tiempo_tolerancia = Parametro.TOLERANCIA_ASISTENCIA_HORA;
                                            }

                                            if (fecha_server.Minute < tiempo_tolerancia)
                                            {



                                                //SI EL INTERNO ESTA AUTORIZADO DESDE EL CONTROL DE INTERNOS, SE OBTIENE SU INFORMACION
                                                var ingreso = new cImputado().Obtener(interno.ID_IMPUTADO, interno.ID_ANIO, interno.ID_CENTRO).FirstOrDefault().INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                                var InternoAusente = new cIngresoUbicacion();

                                                //SE CAMBIA LA UBICACION DEL INTERNO AL AREA DE LA ACTIVIDAD
                                                InternoAusente.Eliminar(interno.ID_CENTRO, interno.ID_ANIO, interno.ID_IMPUTADO, ingreso);
                                                InternoAusente.Insertar(new INGRESO_UBICACION()
                                                {
                                                    ESTATUS = 2,
                                                    ID_AREA = participacion.GRUPO_HORARIO.ID_AREA,
                                                    ID_ANIO = interno.ID_ANIO,
                                                    ID_CENTRO = interno.ID_CENTRO,
                                                    ID_IMPUTADO = interno.ID_IMPUTADO,
                                                    ID_CONSEC = participacion.ID_CONSEC,
                                                    ID_INGRESO = ingreso,
                                                    MOVIMIENTO_FEC = fecha_server,
                                                    ACTIVIDAD = participacion.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR
                                                });


                                                //SE MARCA LA ASISTENCIA DEL INTERNO
                                                var asistencia_interno = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                {
                                                    ID_CENTRO = participacion.ID_CENTRO,
                                                    ID_ACTIVIDAD = participacion.ID_ACTIVIDAD,
                                                    ID_TIPO_PROGRAMA = participacion.ID_TIPO_PROGRAMA,
                                                    ID_GRUPO = participacion.ID_GRUPO,
                                                    ID_CONSEC = participacion.ID_CONSEC,
                                                    ID_GRUPO_HORARIO = participacion.ID_GRUPO_HORARIO,
                                                    ASISTENCIA = 1
                                                });

                                                //SE NOTIFICA LA ASISTENCIA CAPTURADA
                                                #region NOTIFICACION_ASISTENCIA_CAPTURADA_SIN_EMPALME
                                                Imagen = new Imagenes().getImagenPermitido();
                                                ScannerMessage = "Asistencia capturada";
                                                ShowLine = Visibility.Collapsed;

                                                //MUESTREO DE IMAGEN: SI NO EXISTE LA IMAGEN DE SEGUIMIENTO, ENTONCES SE TOMA LA IMAGEN DE REGISTRO; SI NINGUNA DE ELLAS EXISTE, SE MUESTRA EL PLACEHOLDER
                                                var imagen_foto_seguimiento = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                    new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                                                var imagen_foto_registro = new cIngresoBiometrico().Obtener(interno.ID_ANIO, interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                    new cIngresoBiometrico().Obtener(interno.ID_ANIO, (short)interno.ID_CENTRO, interno.ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;


                                                ImagenInterno = imagen_foto_seguimiento == null ? (imagen_foto_registro == null ? new Imagenes().getImagenPerson() : imagen_foto_registro) : imagen_foto_seguimiento;

                                                ShowImagenInterno = Visibility.Visible;
                                                PropertyImage = null;
                                                color = Colors.Green;
                                                #endregion
                                            }
                                            else
                                            {
                                                //SI EL INTERNO NO ESTA AUTORIZADO, SE NOTIFICA QUE NO TIENE PERMITIDO EL ACCESO AL ÁREA
                                                Imagen = new Imagenes().getImagenDenegado();
                                                color = Colors.Red;
                                                ScannerMessage = "Interno no permitido";
                                                ShowLine = Visibility.Collapsed;
                                                PropertyImage = null;
                                            }
                                        #endregion

                                        }
                                        else
                                        {
                                            //SE NOTIFICA QUE EL INTERNO NO TIENE ACCESO AL AREA
                                            Imagen = new Imagenes().getImagenDenegado();
                                            color = Colors.Red;
                                            ScannerMessage = "Interno no autorizado";
                                            ShowLine = Visibility.Collapsed;
                                            PropertyImage = null;
                                        }

                                    }
                                    else
                                    {
                                        //SI LA ASISTENCIA FUE MARCADA, SE NOTIFICA QUE LA ASISTENCIA YA FUE CAPTURADA, INDEPENDIENTEMENTE DE SI ES UNA ASISTENCIA O AUSENCIA A LA ACTIVIDAD
                                        Imagen = new Imagenes().getImagenAdvertencia();
                                        ScannerMessage = "Asistencia previa";
                                        ShowLine = Visibility.Collapsed;
                                        ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                                        PropertyImage = null;
                                        color = Colors.DarkOrange;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //SI EL AREA NO CONCUERDA, SE LE NIEGA EL ACCESO AL INTERNO
                                    Imagen = new Imagenes().getImagenDenegado();
                                    color = Colors.Red;
                                    ScannerMessage = "Interno no permitido";
                                    ShowLine = Visibility.Collapsed;
                                    PropertyImage = null;
                                }
                            }
                            #endregion


                            #endregion

                        }
                        #endregion

                    }
                    //SI NO HUBO IDENTIFICACION DE LA HUELLA, ENTONCES...
                    else
                    {
                        //SE NOTIFICA QUE EL INTERNO NO TIENE PERMITIDO EL ACCESO AL ÁREA
                        Imagen = new Imagenes().getImagenDenegado();
                        color = Colors.Red;
                        ScannerMessage = "Interno no permitido";
                        ShowLine = Visibility.Collapsed;
                        PropertyImage = null;

                    }
                    #endregion

                }

                #endregion
            }
            #endregion

            //SE INICIALIZA EL MENSAJE DE MUESTRA PARA LA SIGUIENTE CAPTURA
            #region INICIALIZA NOTIFICACIÓN
            Application.Current.Dispatcher.Invoke((System.Action)(async delegate
            {
                ColorAprobacion = new SolidColorBrush(color);
                await TaskEx.Delay(1500);
                Imagen = new Imagenes().getImagenHuella();
                ScannerMessage = "Capture Huella";
                ColorAprobacion = new SolidColorBrush(Colors.Green);
                ImagenInterno = new Imagenes().getImagenPerson();
                PropertyImage = null;

            }));
            #endregion

        }
        #endregion


        //METODO A PROBAR
        private async void CompararHuellasImputado(enumTipoBiometrico? Biometrico)
        {
            enumResultadoAsistencia resultado = enumResultadoAsistencia.CAPTURE_HUELLA;
            var fecha_server = Fechas.GetFechaDateServer;
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            if (bytesHuella == null)
            {
                resultado = enumResultadoAsistencia.CAPTURE_DE_NUEVO;

            }
            else
            {
                resultado = enumResultadoAsistencia.EN_PROCESO;

                if (Huellas_Imputado.Count != 0)
                {

                    var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, Huellas_Imputado.Where(w => w.FMD != null && w.tipo_biometrico == Biometrico).Select(s => s.FMD), (0x7fffffff / 100000), 10);




                    var identify = false;
                    var result = new List<object>();



                    if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {

                        switch (doIdentify.ResultCode)
                        {
                            //=====> TODO: VALIDAR RESULTADOS DE LA OPERACIÓN
                        }
                    }
                    else
                    {



                        foreach (var item in doIdentify.Indexes.ToList())
                            result.Add(Huellas_Imputado[item.FirstOrDefault()].IMPUTADO);

                        identify = result.Count != 0 ? true : false;


                        if (identify)
                        {
                            var imputados_sin_repeticiones = result.Cast<cHuellasImputado>()
                                .OrderBy(o => o.ID_CENTRO)
                                .ThenBy(t => t.ID_ANIO)
                                .ThenBy(t => t.ID_IMPUTADO)
                                .Distinct()
                                .ToList();

                            var imputado = imputados_sin_repeticiones.Count == 1 ? imputados_sin_repeticiones.FirstOrDefault() : null;
                            


                            if(imputado != null){


                                GRUPO_ASISTENCIA participacion_elegida = null;

                                try
                                {
                                    List<GRUPO_ASISTENCIA> participaciones = new List<GRUPO_ASISTENCIA>();
                                    var grupo_asistencia = new cGrupoAsistencia();
                               
                                    foreach (var item in Areas)
                                    {
                                        participaciones.AddRange(grupo_asistencia.GetData(g =>
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Day == fecha_server.Day &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Month == fecha_server.Month &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Year == fecha_server.Year &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == fecha_server.Hour).Where(w =>
                                    w.GRUPO_HORARIO.ID_AREA == item.ID_AREA));
                                    }

                                    var participacion_unica = participaciones.Where(w =>
                                    (w.EMP_COORDINACION == 0 || (w.EMP_COORDINACION == 2 && w.EMP_APROBADO == 1))).FirstOrDefault();

                                    var participaciones_sin_resolucion = participaciones.Where(w =>
                                    (w.EMP_COORDINACION == 1)).ToList();

                                    participacion_elegida = participacion_unica != null ? participacion_unica : (participaciones_sin_resolucion != null ? participaciones_sin_resolucion.OrderBy(o => o.FEC_REGISTRO).ToList().FirstOrDefault() : null);


                                }
                                catch (Exception ex)
                                {

                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las actividades que tiene el imputado.", ex);
                                }


                                if (participacion_elegida != null)
                                {
                                    try
                                    {

                                        var tiempo_tolerancia = (participacion_elegida.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - participacion_elegida.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1 ? Parametro.TOLERANCIA_ASISTENCIA_HORAS : Parametro.TOLERANCIA_ASISTENCIA_HORA;

                                        if (fecha_server.Minute < tiempo_tolerancia)
                                        {
                                            var Autorizado = new cIngresoUbicacion().Obtener(
                                                (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                                                (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                                (short)participacion_elegida.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Where(w =>
                                                w.ESTATUS == 1).FirstOrDefault();





                                            if (Autorizado != null)
                                            {

                                                var Asistencia = participacion_elegida.ASISTENCIA;
                                                if (Asistencia == null)
                                                {
                                                    var ingreso_ubicacion = new cIngresoUbicacion().Actualizar(new INGRESO_UBICACION()
                                                    {
                                                        ID_CENTRO = Autorizado.ID_CENTRO,
                                                        ID_ANIO = Autorizado.ID_ANIO,
                                                        ID_IMPUTADO = Autorizado.ID_IMPUTADO,
                                                        ID_INGRESO = Autorizado.ID_INGRESO,
                                                        ID_CONSEC = Autorizado.ID_CONSEC,
                                                        ID_CUSTODIO = null,
                                                        ESTATUS = 2,
                                                        MOVIMIENTO_FEC = fecha_server
                                                    });

                                                    var grupo_asistencia = new cGrupoAsistencia().ActualizarAsistencia(new GRUPO_ASISTENCIA()
                                                    {
                                                        ID_CENTRO = participacion_elegida.ID_CENTRO,
                                                        ID_ACTIVIDAD = participacion_elegida.ID_ACTIVIDAD,
                                                        ID_TIPO_PROGRAMA = participacion_elegida.ID_TIPO_PROGRAMA,
                                                        ID_GRUPO = participacion_elegida.ID_GRUPO,
                                                        ID_CONSEC = participacion_elegida.ID_CONSEC,
                                                        ID_GRUPO_HORARIO = participacion_elegida.ID_GRUPO_HORARIO,
                                                        ASISTENCIA = 1
                                                    });
                                                    resultado = enumResultadoAsistencia.ASISTENCIA_CAPTURADA;
                                                }
                                                else
                                                {
                                                    resultado = enumResultadoAsistencia.ASISTENCIA_PREVIA;
                                                }

                                            }
                                            else
                                            {
                                                resultado = enumResultadoAsistencia.INTERNO_NO_AUTORIZADO;
                                            }

                                        }
                                        else
                                        {
                                            resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar la asistencia del interno.", ex);
                                    }

                                }
                                else
                                {
                                    resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                                }




                            }else{
                                resultado = enumResultadoAsistencia.FALSO_POSITIVO;
                            }
                            


                        }
                        else
                        {
                            resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                        }

                    }


                }
                else
                {
                    resultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                }




            }
            NotificarResultadoLectura(resultado);
            await TaskEx.Delay(1500);
            NotificarResultadoLectura(enumResultadoAsistencia.CAPTURE_HUELLA);
        }

        private void NotificarResultadoLectura(enumResultadoAsistencia Resultado)
        {
            Color color = Colors.Transparent;
            switch (Resultado)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    Imagen = new Imagenes().getImagenDenegado();
                    color = Colors.Red;
                    ScannerMessage = "Interno no permitido";
                    ShowLine = Visibility.Collapsed;
                    PropertyImage = null;
                    break;
                case enumResultadoAsistencia.INTERNO_NO_AUTORIZADO:
                    Imagen = new Imagenes().getImagenDenegado();
                    color = Colors.Red;
                    ScannerMessage = "Interno no autorizado";
                    ShowLine = Visibility.Collapsed;
                    PropertyImage = null;
                    break;
                case enumResultadoAsistencia.ASISTENCIA_CAPTURADA:
                    Imagen = new Imagenes().getImagenPermitido();
                    ScannerMessage = "Asistencia capturada";
                    ShowLine = Visibility.Collapsed;
                    ShowImagenInterno = Visibility.Visible;
                    PropertyImage = null;
                    color = Colors.Green;
                    break;
                case enumResultadoAsistencia.ASISTENCIA_PREVIA:
                    Imagen = new Imagenes().getImagenAdvertencia();
                    ScannerMessage = "Asistencia previa";
                    ShowLine = Visibility.Collapsed;
                    ImagenInterno = ImagenPlaceHolder.getImagenPerson();
                    PropertyImage = null;
                    color = Colors.DarkOrange;
                    break;
                case enumResultadoAsistencia.EN_PROCESO:
                    ScannerMessage = "Procesando...";
                    ColorAprobacion = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    ShowLine = Visibility.Visible;
                    ShowImagenInterno = Visibility.Collapsed;
                    break;
                case enumResultadoAsistencia.CAPTURE_HUELLA:
                    Imagen = new Imagenes().getImagenHuella();
                    ScannerMessage = "Capture Huella";
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    ImagenInterno = new Imagenes().getImagenPerson();
                    PropertyImage = null;
                    break;
                case enumResultadoAsistencia.CAPTURE_DE_NUEVO:
                    Imagen = new Imagenes().getImagenDenegado();
                    color = Colors.Red;
                    ScannerMessage = "Capture de nuevo";
                    ShowLine = Visibility.Collapsed;
                    PropertyImage = null;
                    break;
                case enumResultadoAsistencia.FALSO_POSITIVO:
                    Imagen = new Imagenes().getImagenAdvertencia();
                    color = Colors.DarkOrange;
                    ScannerMessage = "Más de una coincidencia.\nCapture de nuevo";
                    ShowLine = Visibility.Collapsed;
                    PropertyImage = null;
                    break;
            }
            
            

        }


        #region ResultadoBusquedaBiometrico
        public class ResultadoBusquedaBiometrico
        {
            public string Expediente { get; set; }
            public string NIP { get; set; }
            public string APaterno { get; set; }
            public string AMaterno { get; set; }
            public string Nombre { get; set; }
            public ImageSource Foto { get; set; }
            public IMPUTADO Imputado { get; set; }
            public SSP.Servidor.PERSONA Persona { get; set; }
        }
        #endregion

    }
}
