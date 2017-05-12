using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class BusquedaHuellaExcarcelaciones
    {
        #region Propiedades Flyout
        private bool incidenciaRevertirExcarcelacionVisible;
        public bool IncidenciaRevertirExcarcelacionVisible
        {
            get { return incidenciaRevertirExcarcelacionVisible; }
            set { incidenciaRevertirExcarcelacionVisible = value; OnPropertyChanged("IncidenciaRevertirExcarcelacionVisible"); }
        }


        private bool capturaIncidenciaVisible;
        public bool CapturaIncidenciaVisible
        {
            get { return capturaIncidenciaVisible; }
            set { capturaIncidenciaVisible = value; OnPropertyChanged("CapturaIncidenciaVisible"); }
        }

        private string textoIncidencia;
        public string TextoIncidencia
        {
            get { return textoIncidencia; }
            set { textoIncidencia = value; OnPropertyChanged("TextoIncidencia"); }
        }

        private string textoIncidenciaFalsoPositivo;
        public string TextoIncidenciaFalsoPositivo
        {
            get { return textoIncidenciaFalsoPositivo; }
            set { textoIncidenciaFalsoPositivo = value; OnPropertyChanged("TextoIncidenciaFalsoPositivo"); }
        }
        #endregion

        #region Propiedades Excarcelacion
        private ObservableCollection<InternoExcarcelacion> excarcelacion;
        public ObservableCollection<InternoExcarcelacion> Excarcelacion
        {
            get { return excarcelacion; }
            set { excarcelacion = value; OnPropertyChanged("Excarcelacion"); }
        }

        private InternoExcarcelacion selectedImputado;
        public InternoExcarcelacion SelectedImputado
        {
            get { return selectedImputado; }
            set
            {
                selectedImputado = value;

                OnPropertyChanged("SelectedImputado");
            }
        }

        public enum enumCertificadoMedicoRequerido
        {
            NO_REQUIERE_CERTIFICADO_MEDICO = 0,
            REQUIERE_CERTIFICADO_MEDICO = 1,
        }
        #endregion

        #region Propiedades Visuales Responsable
        private bool asignarResponsable;
        public bool AsignarResponsable
        {
            get { return asignarResponsable; }
            set
            {
                asignarResponsable = value;
                if (value)
                {
                    ColorEnabled = new SolidColorBrush(Colors.White);
                }
                else
                {
                    ColorEnabled = new SolidColorBrush(Colors.LightGray);
                }
                OnPropertyChanged("AsignarResponsable");
            }
        }

        private string nombreResponsableExcarcelacion;
        public string NombreResponsableExcarcelacion
        {
            get { return nombreResponsableExcarcelacion; }
            set { nombreResponsableExcarcelacion = value; OnPropertyChanged("NombreResponsableExcarcelacion"); }
        }

        private string apellidoPaternoResponsableExcarcelacion;
        public string ApellidoPaternoResponsableExcarcelacion
        {
            get { return apellidoPaternoResponsableExcarcelacion; }
            set { apellidoPaternoResponsableExcarcelacion = value; OnPropertyChanged("ApellidoPaternoResponsableExcarcelacion"); }
        }

        private string apellidoMaternoResponsableExcarcelacion;
        public string ApellidoMaternoResponsableExcarcelacion
        {
            get { return apellidoMaternoResponsableExcarcelacion; }
            set { apellidoMaternoResponsableExcarcelacion = value; OnPropertyChanged("ApellidoMaternoResponsableExcarcelacion"); }
        }

        private SolidColorBrush colorEnabled;
        public SolidColorBrush ColorEnabled
        {
            get { return colorEnabled; }
            set { colorEnabled = value; OnPropertyChanged("ColorEnabled"); }
        }

        #endregion

        #region Propiedades Visuales Ventana
        private BusquedaHuellaExcarcelacionView ventana;
        public BusquedaHuellaExcarcelacionView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private byte[] fotoIngreso;
        public byte[] FotoIngreso
        {
            get { return fotoIngreso; }
            set { fotoIngreso = value; OnPropertyChanged("FotoIngreso"); }
        }

        private byte[] fotoCentro;
        public byte[] FotoCentro
        {
            get { return fotoCentro; }
            set { fotoCentro = value; OnPropertyChanged("FotoCentro"); }
        }
        #endregion

        #region Propiedades Visuales Lectura Huella
        private Visibility progressRingVisible;
        public Visibility ProgressRingVisible
        {
            get { return progressRingVisible; }
            set
            {
                progressRingVisible = value;
                OnPropertyChanged("ProgressRingVisible");
            }
        }

        private byte[] imagenEvaluacion;
        public byte[] ImagenEvaluacion
        {
            get { return imagenEvaluacion; }
            set
            {
                imagenEvaluacion = value;
                OnPropertyChanged("ImagenEvaluacion");
            }
        }

        private bool imagenEvaluacionVisible;
        public bool ImagenEvaluacionVisible
        {
            get { return imagenEvaluacionVisible; }
            set
            {
                imagenEvaluacionVisible = value;
                OnPropertyChanged("ImagenEvaluacionVisible");
            }
        }

        private bool scannerMessageVisible;
        public bool ScannerMessageVisible
        {
            get { return scannerMessageVisible; }
            set
            {
                scannerMessageVisible = value;
                OnPropertyChanged("ScannerMessageVisible");
            }
        }

        private SolidColorBrush colorAprobacion;
        public SolidColorBrush ColorAprobacion
        {
            get { return colorAprobacion; }
            set
            {
                colorAprobacion = value;
                OnPropertyChanged("ColorAprobacion");
            }
        }

        public enum enumMensajeResultadoComparacion
        {
            HUELLA_VACIA = 0,
            NO_ENCONTRADO = 1,
            PROCESANDO = 2,
            COINCIDENCIAS = 3,
            ENCONTRADO = 4,
            OPERACION_LECTOR_INCORRECTA = 5
        }
        #endregion

        #region Propiedades Lectura NIP
        private bool capturaNIPVisible;
        public bool CapturaNIPVisible
        {
            get { return capturaNIPVisible; }
            set
            {
                capturaNIPVisible = value;
                OnPropertyChanged("CapturaNIPVisible");
            }
        }

        private string nipBuscar;
        public string NIPBuscar
        {
            get { return nipBuscar; }
            set { nipBuscar = value; OnPropertyChanged("NIPBuscar"); }
        }
        #endregion

        #region Propiedades Lectura Huella
        private List<Imputado_Huella> huellasImputados;
        public List<Imputado_Huella> HuellasImputados
        {
            get { return huellasImputados; }
            set
            {
                huellasImputados = value;
                OnPropertyChanged("HuellasImputados");
            }
        }


        private enumTipoBiometrico selectedFinger;
        public enumTipoBiometrico SelectedFinger
        {
            get { return selectedFinger; }
            set
            {
                selectedFinger = value;
                OnPropertyChanged("SelectedFinger");
            }

        }

        private bool isSucceed;
        public bool IsSucceed
        {
            get { return isSucceed; }
            set
            {
                isSucceed = value;
                OnPropertyChanged("IsSucceed");
            }
        }
        #endregion

        #region Propiedades Visuales Lectura NIP
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

        private SolidColorBrush colorAprobacionNIP;
        public SolidColorBrush ColorAprobacionNIP
        {
            get { return colorAprobacionNIP; }
            set
            {
                colorAprobacionNIP = value;
                RaisePropertyChanged("ColorAprobacionNIP");
            }
        }

        private SolidColorBrush fondoBackSpaceNIP;
        public SolidColorBrush FondoBackSpaceNIP
        {
            get { return fondoBackSpaceNIP; }
            set
            {
                fondoBackSpaceNIP = value;
                OnPropertyChanged("FondoBackSpaceNIP");
            }
        }

        private SolidColorBrush fondoLimpiarNIP;
        public SolidColorBrush FondoLimpiarNIP
        {
            get { return fondoLimpiarNIP; }
            set
            {
                fondoLimpiarNIP = value;
                OnPropertyChanged("FondoLimpiarNIP");
            }
        }

        public string CampoCaptura { get; set; }

        private string mensajeAprobacionNIP;
        public string MensajeAprobacionNIP
        {
            get { return mensajeAprobacionNIP; }
            set
            {
                mensajeAprobacionNIP = value;
                OnPropertyChanged("MensajeAprobacionNIP");
            }
        }

        private string incidenciaNIP;
        public string IncidenciaNIP
        {
            get { return incidenciaNIP; }
            set { incidenciaNIP = value; OnPropertyChanged("IncidenciaNIP"); }
        }
        #endregion

        #region Constantes
        const string EXCARCELACION_EN_PROCESO = "EP";
        const string EXCARCELACION_ACTIVA = "AC";
        const string EXCARCELACION_PROGRAMADA = "PR";
        const string EXCARCELACION_AUTORIZADA = "AU";
        #endregion

        #region Propiedades Generales
        public enum enumUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            ACTIVIDAD = 2
        }

        public enum enumAreas
        {
            AREA_MEDICA_PB = 46,
            SALIDA_DE_CENTRO = 111,
            ESTANCIA = 0
        }

        public enum enumRequiereCertificadoMedico
        {
            REQUIERE_CERTIFICADO_MEDICO = 1,
            NO_REQUIERE_CERTIFICADO_MEDICO = 0
        }

        private enum enumIncidente
        {
            NORMAL = 1
        }

        public enum enumIncidencias
        {
            CERTIFICADO_MEDICO_AUSENTE = 1
        }

        #endregion
    }
}
