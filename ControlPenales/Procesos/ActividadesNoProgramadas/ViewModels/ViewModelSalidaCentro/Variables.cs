using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class IngresarSalidaCentroViewModel
    {
        #region Propiedades Flyouts

        /// <summary>
        /// Propiedades que involucran al control "Flyout" de la ventana
        /// 1.- Habilita/Deshabilita la captura del NIP
        /// 2.- Habilita/Deshabilita la captura de incidencias
        /// </summary>

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

        private bool capturaIncidenciaVisible;
        public bool CapturaIncidenciaVisible
        {
            get { return capturaIncidenciaVisible; }
            set
            {
                capturaIncidenciaVisible = value;
                OnPropertyChanged("CapturaIncidenciaVisible");
            }
        }


        private bool capturaIncidenciaFalsoPositivoVisible;
        public bool CapturaIncidenciaFalsoPositivoVisible
        {
            get { return capturaIncidenciaFalsoPositivoVisible; }
            set { capturaIncidenciaFalsoPositivoVisible = value; OnPropertyChanged("CapturaIncidenciaFalsoPositivoVisible"); }
        }


        private string incidenciaNIP;
        public string IncidenciaNIP
        {
            get { return incidenciaNIP; }
            set { incidenciaNIP = value; OnPropertyChanged("IncidenciaNIP"); }
        }


        private string textoIncidenciaFalsoPositivo;
        public string TextoIncidenciaFalsoPositivo
        {
            get { return textoIncidenciaFalsoPositivo; }
            set { textoIncidenciaFalsoPositivo = value; OnPropertyChanged("TextoIncidenciaFalsoPositivo"); }
        }
        #endregion

        #region Propiedades Visuales Lectura Huella
        /// <summary>
        /// Propiedades visuales para la muestra de resultados de comprobación de la huella
        /// 1.- Habilita/Deshabilita el indicador (Control: ProgressRing) de una comparación de huella
        /// 2.- Imagen que indica el resultado de evaluación de la comparación de huella
        /// 3.- Habilita/Deshabilita la imagen que indica el resultado de evaluación
        /// 4.- Habilita/Deshabilita la visibilidad del mensaje de la lectura de huellas
        /// 5.- Color resultante de una evaluación de comparación
        /// </summary>

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
        #endregion

        #region Propiedades Visuales Lectura NIP

        /// <summary>
        /// Propiedades visuales para la muestra de resultados de comparación del NIP
        /// 1.- Indica el resultado de la comparación
        /// 2.- Indica el color del resultado de la evaluación
        /// 3.- Color de fondo del botón de "Borrar 1 caractér" de la captura del NIP
        /// 4.- Color de fondo del botón de "Borrar todo el NIP" de la captura del NIP
        /// 5.- Indica el tipo de captura que se esta realizando
        /// 6.- *PENDIENTE*
        /// </summary>


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
        #endregion

        #region Propiedades Lectura NIP

        /// <summary>
        /// Propiedades requeridas para la lectura y comparación del NIP
        /// 1.- Campo donde se aloja el NIP a buscar
        /// </summary>

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
        #endregion

        #region Propiedades Lectura Huella
        /// <summary>
        /// Propiedades requeridas para la obtención de resultados de comparación de la huella
        /// 1.- Tipo de biométrico (dedo) utilizado para la obtención del universo sobre el cuál se van a comparar las huellas
        /// 2.- Enumerable que indica los distintos resultados posibles de una comparación de huellas
        /// 3.-*PENDIENTE*
        /// 4.- Universo de huellas de traslados
        /// 5.- Universo de huellas de excarcelaciones a realizarse
        /// 6.- Universo de huellas de excarcelaciones activas por concluir (internos que se encuentran en una excarcelación y regresan al centro)
        /// </summary>

        private enumTipoBiometrico? selectedFinger;
        public enumTipoBiometrico? SelectedFinger
        {
            get { return selectedFinger; }
            set
            {
                selectedFinger = value;
                OnPropertyChanged("SelectedFinger");
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

        private string textoSeleccionDedo;
        public string TextoSeleccionDedo
        {
            get { return textoSeleccionDedo; }
            set { textoSeleccionDedo = value; }
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

        private List<Imputado_Huella> huellasImputadosTraslados;
        public List<Imputado_Huella> HuellasImputadosTraslados
        {
            get { return huellasImputadosTraslados; }
            set
            {
                huellasImputadosTraslados = value;
                OnPropertyChanged("HuellasImputadosTraslados");
            }
        }

        private List<Imputado_Huella> huellasImputadosExcarcelaciones;
        public List<Imputado_Huella> HuellasImputadosExcarcelaciones
        {
            get { return huellasImputadosExcarcelaciones; }
            set
            {
                huellasImputadosExcarcelaciones = value;
                OnPropertyChanged("HuellasImputadosExcarcelaciones");
            }
        }

        private List<Imputado_Huella> huellasImputadosExcarcelacionesEntrada;
        public List<Imputado_Huella> HuellasImputadosExcarcelacionesEntrada
        {
            get { return huellasImputadosExcarcelacionesEntrada; }
            set
            {
                huellasImputadosExcarcelacionesEntrada = value;
                OnPropertyChanged("HuellasImputadosExcarcelacionesEntrada");
            }
        }
        #endregion

        #region Constantes

        /// <summary>
        /// Propiedades con valores fijos necesarias para comparaciones
        /// 1-3.- Estatus de excarcelaciones
        /// 4.- Estatus de traslado en proceso
        /// 5.- ID del área: Salida del Centro
        /// 6.- Enumerable que indica las áreas de retorno posibles de una excarcelación
        /// 7.- Enumerable que indica algunos de los distintos departamentos a los que pertenece un empleado 
        /// </summary>

        const string EXCARCELACION_EN_PROCESO = "EP";
        const string EXCARCELACION_ACTIVA = "AC";
        const string EXCARCELACION_CONCLUIDA = "CO";
        const string EXCARCELACION_AUTORIZADA = "AU";

        const string TRASLADO_EN_PROCESO = "EP";

        const string INCIDENTE_PENDIENTE = "P";

        const int SALIDA_DE_CENTRO = 111;

        public enum enumAreasRetorno : short
        {
            ESTANCIA = 0,
            AREA_MEDICA_PB = 46
        }

        private enum enumAreaTrabajo
        {
            MEDICO_DE_GUARDIA = 12,
            COMANDANCIA = 4
        }

        public enum enumIncidenteTipo : short
        {
            NORMAL = 1
        }
        #endregion

        #region Propiedades Generales

        /// <summary>
        /// Propiedades generales del control de ingreso a la Salida del Centro
        /// 1.- Indica si el interno tiene permitido el acceso a la salida del centro
        /// 2.- 
        /// </summary>

        private bool ubicacionPermitidaChecked;
        public bool UbicacionPermitidaChecked
        {
            get { return ubicacionPermitidaChecked; }
            set
            {
                ubicacionPermitidaChecked = value;
                OnPropertyChanged("UbicacionPermitidaChecked");
            }
        }

        private INGRESO_UBICACION selectedImputadoUbicacion;
        public INGRESO_UBICACION SelectedImputadoUbicacion
        {
            get { return selectedImputadoUbicacion; }
            set
            {
                selectedImputadoUbicacion = value;
                OnPropertyChanged("SelectedImputadoUbicacion");
            }
        }

        private IngresarSalidaCentro ventana;
        public IngresarSalidaCentro Ventana
        {
            get { return ventana; }
            set
            {
                ventana = value;
                OnPropertyChanged("Ventana");
            }
        }

        #region Propiedades Dialogo
        private bool dialogoIncidenciaVisible;
        public bool DialogoIncidenciaVisible
        {
            get { return dialogoIncidenciaVisible; }
            set
            {
                dialogoIncidenciaVisible = value;
                OnPropertyChanged("DialogoIncidencia");
            }
        }

        private string textoIncidencia;
        public string TextoIncidencia
        {
            get { return textoIncidencia; }
            set
            {
                textoIncidencia = value;
                OnPropertyChanged("TextoIncidencia");
            }
        }


        #endregion

        private enum enumEstatusUbicacion
        {
            EN_ESTANCIA = 0,
            EN_TRANSITO = 1,
            EN_ACTIVIDAD = 2
        }

        private List<IMPUTADO> imputadoEntrante;
        public List<IMPUTADO> ImputadoEntrante
        {
            get { return imputadoEntrante; }
            set
            {
                imputadoEntrante = value;
                OnPropertyChanged("ImputadoEntrante");
            }
        }

        private byte[] imagenImputado;
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        public enum enumCertificadoMedicoRequerido
        {
            NO_REQUIERE_CERTIFICADO_MEDICO = 0,
            REQUIERE_CERTIFICADO_MEDICO = 1,
        }

        public enum enumIncidencias
        {
            CERTIFICADO_MEDICO_AUSENTE = 1
        }
        #endregion
    }
}
