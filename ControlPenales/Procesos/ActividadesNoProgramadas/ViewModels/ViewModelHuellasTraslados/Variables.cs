using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using DPUruNet;
using MahApps.Metro.Controls;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class BusquedaHuellaTraslado
    {
        #region Propiedades Flyout
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
            set { capturaIncidenciaVisible = value; OnPropertyChanged("CapturaIncidenciaVisible"); }
        }

        private bool incidenciaRevertirTrasladoVisible;
        public bool IncidenciaRevertirTrasladoVisible
        {
            get { return incidenciaRevertirTrasladoVisible; }
            set { incidenciaRevertirTrasladoVisible = value; OnPropertyChanged("IncidenciaRevertirTrasladoVisible"); }
        }

        private bool capturaFinalizarTrasladoVisible;
        public bool CapturaFinalizarTrasladoVisible
        {
            get { return capturaFinalizarTrasladoVisible; }
            set { capturaFinalizarTrasladoVisible = value; OnPropertyChanged("CapturaFinalizarTrasladoVisible"); }
        }

        private string textoIncidencia;
        public string TextoIncidencia
        {
            get { return textoIncidencia; }
            set { textoIncidencia = value; OnPropertyChanged("TextoIncidencia"); }
        }

        private string textoBotonCancelar;
        public string TextoBotonCancelar
        {
            get { return textoBotonCancelar; }
            set { textoBotonCancelar = value; OnPropertyChanged("TextoBotonCancelar"); }
        }

        private string textoIncidenciaCertificadoMedico;
        public string TextoIncidenciaCertificadoMedico
        {
            get { return textoIncidenciaCertificadoMedico; }
            set { textoIncidenciaCertificadoMedico = value; }
        }

        private string textoFlyout;
        public string TextoFlyout
        {
            get { return textoFlyout; }
            set
            {
                textoFlyout = value;
                OnPropertyChanged("TextoFlyout");
            }
        }

        private byte[] fotoTrasladoUltimoImputado;
        public byte[] FotoTrasladoUltimoImputado
        {
            get { return fotoTrasladoUltimoImputado; }
            set { fotoTrasladoUltimoImputado = value; OnPropertyChanged("FotoTrasladoUltimoImputado"); }
        }

        private string nombreUltimoInterno;

        public string NombreUltimoInterno
        {
            get { return nombreUltimoInterno; }
            set { nombreUltimoInterno = value; OnPropertyChanged("NombreUltimoInterno"); }
        }

        public enum enumIncidencias
        {
            CERTIFICADO_MEDICO_AUSENTE = 1
        }

        private enum enumEstatusUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            SALIDA_DEL_CENTRO = 2
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

        public string CampoCaptura { get; set; }
        #endregion

        #region Propiedades Visuales Lectura Huella
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

        private string estatusLector;
        public string EstatusLector
        {
            get { return estatusLector; }
            set
            {
                estatusLector = value;
                OnPropertyChanged("EstatusLector");
            }
        }

        private SolidColorBrush colorEstatusLector;
        public SolidColorBrush ColorEstatusLector
        {
            get { return colorEstatusLector; }
            set
            {
                colorEstatusLector = value;
                OnPropertyChanged("ColorEstatusLector");
            }
        }

        const string LECTOR_DETECTADO = "Lector Detectado";
        const string LECTOR_DESCONECTADO = "Lector No Conectado";

        private string textoSeleccionDedo;
        public string TextoSeleccionDedo
        {
            get { return textoSeleccionDedo; }
            set
            {
                textoSeleccionDedo = value;
                OnPropertyChanged("TextoSeleccionDedo");
            }
        }
        #endregion

        #region Propiedades Visuales Ventana
        private bool emptyVisible;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set
            {
                emptyVisible = value;
                OnPropertyChanged("EmptyVisible");
            }
        }


        private byte[] fotoIngreso;
        public byte[] FotoIngreso
        {
            get { return fotoIngreso; }
            set
            {
                fotoIngreso = value;
                OnPropertyChanged("FotoIngreso");
            }
        }

        private byte[] fotoCentro;
        public byte[] FotoCentro
        {
            get { return fotoCentro; }
            set
            {
                fotoCentro = value;
                OnPropertyChanged("FotoCentro");
            }
        }

        private MetroWindow ventana;
        public MetroWindow Ventana
        {
            get { return ventana; }
            set
            {
                ventana = value;
                OnPropertyChanged("Ventana");
            }
        }


        private bool permitirChecked;
        public bool PermitirChecked
        {
            get { return permitirChecked; }
            set
            {
                permitirChecked = value;
                SelectedImputado.PERMITIR = value;
                OnPropertyChanged("PermitirChecked");
            }
        }
        #endregion

        #region Propiedades Traslado
        private ObservableCollection<InternoTraslado> listaTrasladoInternos;
        public ObservableCollection<InternoTraslado> ListaTrasladoInternos
        {
            get { return listaTrasladoInternos; }
            set
            {
                listaTrasladoInternos = value;
                OnPropertyChanged("ListaTrasladoInternos");
            }
        }
        #endregion

        #region Propiedades Lectura Huella
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

        private InternoTraslado selectedImputado;
        public InternoTraslado SelectedImputado
        {
            get { return selectedImputado; }
            set
            {
                selectedImputado = value;
                OnPropertyChanged("SelectedImputado");
            }
        }

        public enum enumMensajeResultadoComparacion
        {
            HUELLA_VACIA = 0,
            NO_ENCONTRADO = 1,
            PROCESANDO = 2,
            COINCIDENCIAS = 3,
            ENCONTRADO = 4
        }

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
        #endregion

        #region Propiedades Lectura NIP
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

        private string incidenciaNIP;
        public string IncidenciaNIP
        {
            get { return incidenciaNIP; }
            set { incidenciaNIP = value; OnPropertyChanged("IncidenciaNIP"); }
        }
        #endregion

        #region Propiedades Generales
        const short SALIDA_DE_CENTRO = 111;

        private TRASLADO selectedTraslado;
        public TRASLADO SelectedTraslado
        {
            get { return selectedTraslado; }
            set { selectedTraslado = value; OnPropertyChanged("SelectedTraslado"); }
        }

        private enum enumUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            EN_SALIDA_DE_CENTRO = 2
        }

        private enum enumIncidente
        {
            NORMAL = 1
        }

        private string nombreResponsableTraslado;
        public string NombreResponsableTraslado
        {
            get { return nombreResponsableTraslado; }
            set {
                nombreResponsableTraslado = value; 
                OnPropertyChanged("NombreResponsableTraslado");
            }
        }

        private string apellidoPaternoResponsableTraslado;
        public string ApellidoPaternoResponsableTraslado
        {
            get { return apellidoPaternoResponsableTraslado; }
            set { apellidoPaternoResponsableTraslado = value; OnPropertyChanged("ApellidoPaternoResponsableTraslado"); }
        }

        private string apellidoMaternoResponsableTraslado;
        public string ApellidoMaternoResponsableTraslado
        {
            get { return apellidoMaternoResponsableTraslado; }
            set { apellidoMaternoResponsableTraslado = value; OnPropertyChanged("ApellidoMaternoResponsableTraslado"); }
        }
        #endregion

        #region Constantes
        const short TRASLADADO = 5;
        const string TRASLADO_ACTIVO = "AC";
        const string TRASLADO_FINALIZADO = "FI";
        const string TRASLADO_EN_PROCESO = "EP";
        const string TRASLADO_PROGRAMADO = "PR";

        const short ESTANCIA = 0;
        #endregion
    }
}
