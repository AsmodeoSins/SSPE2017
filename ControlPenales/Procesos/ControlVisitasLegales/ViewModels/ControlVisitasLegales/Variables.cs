using ControlPenales.BiometricoServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class BusquedaHuellaVisitaViewModel
    {
        #region Propiedades Flyout
        private bool incidenciaRevertirVisitaLegalVisible;

        public bool IncidenciaRevertirVisitaLegalVisible
        {
            get { return incidenciaRevertirVisitaLegalVisible; }
            set { incidenciaRevertirVisitaLegalVisible = value; OnPropertyChanged("IncidenciaRevertirVisitaLegalVisible"); }
        }

        private string textoIncidenciaFalsoPositivo;

        public string TextoIncidenciaFalsoPositivo
        {
            get { return textoIncidenciaFalsoPositivo; }
            set { textoIncidenciaFalsoPositivo = value; OnPropertyChanged("TextoIncidenciaFalsoPositivo"); }
        }

        private enum enumIncidente
        {
            NORMAL = 1
        }
        #endregion

        #region Propiedades Lectura Huellas

        private bool isSucceed;
        public bool IsSucceed
        {
            get { return isSucceed; }
            set { isSucceed = value; OnPropertyChanged("IsSucceed"); }
        }

        private List<Imputado_Huella> huellasImputadosVisitas;
        public List<Imputado_Huella> HuellasImputadosVisitas
        {
            get { return huellasImputadosVisitas; }
            set { huellasImputadosVisitas = value; OnPropertyChanged("HuellasImputadosVisitas"); }
        }

        private enumTipoBiometrico selectedFinger;
        public enumTipoBiometrico SelectedFinger
        {
            get { return selectedFinger; }
            set { selectedFinger = value; OnPropertyChanged("SelectedFinger"); }
        }

        private List<InternoVisitaLegal> imputadoEntrante;
        public List<InternoVisitaLegal> ImputadoEntrante
        {
            get { return imputadoEntrante; }
            set { imputadoEntrante = value; OnPropertyChanged("ImputadoEntrante"); }
        }

        private InternoVisitaLegal selectedImputado;
        public InternoVisitaLegal SelectedImputado
        {
            get { return selectedImputado; }
            set { selectedImputado = value; OnPropertyChanged("SelectedImputado"); }
        }

        #endregion

        #region Propiedades Generales
        public enum enumUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            ACTIVIDAD = 2
        }

        const string INTERNO_NOTIFICADO = "S";
        const string INTERNO_NO_NOTIFICADO = "N";
        #endregion

        #region Propiedades Visuales Ventana
        private byte[] imagenImputado;
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set { imagenImputado = value; OnPropertyChanged("ImagenImputado"); }
        }

        private bool ubicacionPermitidaChecked;
        public bool UbicacionPermitidaChecked
        {
            get { return ubicacionPermitidaChecked; }
            set { ubicacionPermitidaChecked = value; OnPropertyChanged("UbicacionPermitidadChecked"); }
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

        #region Propiedades Visuales Captura NIP
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

        private string nipBuscar;
        public string NIPBuscar
        {
            get { return nipBuscar; }
            set { nipBuscar = value; OnPropertyChanged("NIPBuscar"); }
        }

        private string incidenciaNIP;
        public string IncidenciaNIP
        {
            get { return incidenciaNIP; }
            set { incidenciaNIP = value; OnPropertyChanged("IncidenciaNIP"); }
        }

        private SolidColorBrush colorAprobacionNIP;
        public SolidColorBrush ColorAprobacionNIP
        {
            get { return colorAprobacionNIP; }
            set { colorAprobacionNIP = value; OnPropertyChanged("ColorAprobacionNIP"); }
        }

        private string checkMark;
        public string CheckMark
        {
            get { return checkMark; }
            set { checkMark = value; OnPropertyChanged("CheckMark"); }
        }
        #endregion

        #region Constantes
        const short ESTANCIA = 0;
        const short ABOGADO = 2;
        const string VISITA_LEGAL = "VISITA LEGAL";
        #endregion
    }
}
