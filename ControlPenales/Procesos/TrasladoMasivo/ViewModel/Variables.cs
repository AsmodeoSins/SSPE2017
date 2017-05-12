using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
     partial class TrasladoMasivoViewModel
     {
         #region Variables Manejo Controles
         private bool habilitaImputados = true;
         public bool HabilitaImputados
         {
             get { return habilitaImputados; }
             set { habilitaImputados = value; RaisePropertyChanged("HabilitaImputados"); }
         }

         private bool cancelarMenuEnabled = false;
         public bool CancelarMenuEnabled
         {
             get { return cancelarMenuEnabled; }
             set { cancelarMenuEnabled = value; RaisePropertyChanged("CancelarMenuEnabled"); }
         }

         private bool menuGuardarEnabled = false;
         public bool MenuGuardarEnabled
         {
             get { return menuGuardarEnabled; }
             set { menuGuardarEnabled = value; RaisePropertyChanged("MenuGuardarEnabled"); }
         }

         private bool menuBuscarEnabled=false;
         public bool MenuBuscarEnabled
         {
             get{return menuBuscarEnabled;}
             set {menuBuscarEnabled=value;RaisePropertyChanged("MenuBuscarEnabled");}
         }

         private bool menuLimpiarEnabled=false;
         public bool MenuLimpiarEnabled
         {
             get {return menuLimpiarEnabled;}
             set {menuLimpiarEnabled=value;RaisePropertyChanged("MenuLimpiarEnabled");}
         }

         private bool menuReporteEnabled = false;
         public bool MenuReporteEnabled
         {
             get { return menuReporteEnabled; }
             set { menuReporteEnabled = value; RaisePropertyChanged("MenuReporteEnabled"); }
         }

         private bool eliminarMenuEnabled = false;
         public bool EliminarMenuEnabled
         {
             get { return eliminarMenuEnabled; }
             set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
         }

         #endregion
         #region GENERALES
         public string Name
         {
            get
            {
                return "traslado_individual";
            }
         }

         private ObservableCollection<CT_EXCARCELACION_DESTINO> excarcelacion_destinos;
         public ObservableCollection<CT_EXCARCELACION_DESTINO> Excarcelacion_Destinos
         {
             get { return excarcelacion_destinos; }
             set { excarcelacion_destinos = value; RaisePropertyChanged("Excarcelacion_Destinos"); }
         }

         private string tituloExcarcelaciones = string.Empty;
         public string TituloExcarcelaciones
         {
             get { return tituloExcarcelaciones; }
             set { tituloExcarcelaciones = value; RaisePropertyChanged("TituloExcarcelaciones"); }
         }

        #endregion

        #region Imputados        
        private RangeEnabledObservableCollection<cTrasladoIngreso> lstIngresos;
        public RangeEnabledObservableCollection<cTrasladoIngreso> LstIngresos
        {
            get { return lstIngresos; }
            set { lstIngresos = value; OnPropertyChanged("LstIngresos"); }
        }

        private cTrasladoIngreso selectedIngreso;
        public cTrasladoIngreso SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value;
            if (selectedIngreso == null)
                return;
            if (selectedIngreso.Ingreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
            {
                ImagenIngreso = selectedIngreso.Ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                OnPropertyChanged("SelectedIngreso");
            }
            else
                ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedIngreso"); }
        }

        private List<INGRESO> seleccionadosIngresos;
        public List<INGRESO> SeleccionadosIngresos
        {
            get { return seleccionadosIngresos; }
            set { seleccionadosIngresos = value; RaisePropertyChanged("SeleccionadosIngresos"); }
        }

        private ObservableCollection<INGRESO> lstIngresosSeleccionados;
        public ObservableCollection<INGRESO> LstIngresosSeleccionados
        {
            get { return lstIngresosSeleccionados; }
            set { lstIngresosSeleccionados = value; OnPropertyChanged("LstIngresosSeleccionados"); }
        }

        private INGRESO selectedIngresoSeleccionado;
        public INGRESO SelectedIngresoSeleccionado
        {
            get { return selectedIngresoSeleccionado; }
            set
            {
                selectedIngresoSeleccionado = value;
                if (selectedIngresoSeleccionado != null)
                {
                    if (selectedIngresoSeleccionado.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                        ImagenIngreso = selectedIngresoSeleccionado.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectedIngresoSeleccionado");
            }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        #endregion

        #region Ampliar Descripcion
        private string tituloHeaderExpandirDescripcion;
        public string TituloHeaderExpandirDescripcion
        {
            get { return tituloHeaderExpandirDescripcion; }
            set { tituloHeaderExpandirDescripcion = value; OnPropertyChanged("TituloHeaderExpandirDescripcion"); }
        }

        private string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set { textAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }

        private short maxLengthAmpliarDescripcion = 1000;
        public short MaxLengthAmpliarDescripcion
        {
            get { return maxLengthAmpliarDescripcion; }
            set { maxLengthAmpliarDescripcion = value; OnPropertyChanged("MaxLengthAmpliarDescripcion"); }
        }

        private int? anioBuscarImputado;
        public int? AnioBuscarImputado
        {
            get { return anioBuscarImputado; }
            set { anioBuscarImputado = value; RaisePropertyChanged("AnioBuscarImputado"); }
        }

        private int? _anioBuscarImputado;

        private int? folioBuscarImputado;
        public int? FolioBuscarImputado
        {
            get { return folioBuscarImputado; }
            set { folioBuscarImputado = value; RaisePropertyChanged("FolioBuscarImputado"); }
        }

        private int? _folioBuscarImputado;

        private string nombreBuscarImputado;
        public string  NombreBuscarImputado
        {
            get { return nombreBuscarImputado; }
            set { nombreBuscarImputado = value; RaisePropertyChanged("NombreBuscarImputado"); }
        }

        private string _nombreBuscarImputado;

        private string apellidoPaternoBuscarImputado;
        public string ApellidoPaternoBuscarImputado
        {
            get { return apellidoPaternoBuscarImputado; }
            set { apellidoPaternoBuscarImputado = value; RaisePropertyChanged("ApellidoPaternoBuscarImputado"); }
        }

        private string _apellidoPaternoBuscarImputado;

        private string apellidoMaternoBuscarImputado;
        public string ApellidoMaternoBuscarImputado
        {
            get { return apellidoMaternoBuscarImputado; }
            set { apellidoMaternoBuscarImputado = value; RaisePropertyChanged("ApellidoMaternoBuscarImputado"); }
        }

        private string _apellidoMaternoBuscarImputado;

        #endregion

        #region Traslados

        private TRASLADO selectedTraslado;
        public TRASLADO SelectedTraslado
        {
            get { return selectedTraslado; }
            set { selectedTraslado = value; OnPropertyChanged("SelectedTraslado"); }
        }
        #endregion

        #region Datos Traslado
        private ObservableCollection<TRASLADO_MOTIVO> lstMotivo;
        public ObservableCollection<TRASLADO_MOTIVO> LstMotivo
        {
            get { return lstMotivo; }
            set { lstMotivo = value; OnPropertyChanged("LstMotivo"); }
        }

        private ObservableCollection<CENTRO> lstCentro;
        public ObservableCollection<CENTRO> LstCentro
        {
            get { return lstCentro; }
            set { lstCentro = value; OnPropertyChanged("LstCentro"); }
        }

      
        private int id_autoridad_traslado;
        private string autoridad_traslado;
        public string Autoridad_Traslado
        {
            get { return autoridad_traslado; }
            set { autoridad_traslado = value; RaisePropertyChanged("Autoridad_Traslado"); }
        }

      
        private bool justificacion = true;
        public bool Justificacion
        {
            get { return justificacion; }
            set { justificacion = value; }
        }

        private bool dTFechaValid = false;
        public bool DTFechaValid
        {
            get { return dTFechaValid; }
            set { dTFechaValid = value; OnPropertyChanged("DTFechaValid"); }
        }

        private DateTime? dTFecha;
        public DateTime? DTFecha
        {
            get { return dTFecha; }
            set
            {
                dTFecha = value;
                if (value != null)
                {
                    DTFechaValid = true;
                    OnPropertyValidateChanged("DTFecha");
                }
                else
                {
                    DTFechaValid = false;
                    OnPropertyValidateChanged("DTFecha");
                }
                    
                
            }
        }

        private short? dTMotivo;
        public short? DTMotivo
        {
            get { return dTMotivo; }
            set { 
                dTMotivo = value;
                if (dTMotivo.HasValue)
                    OnPropertyValidateChanged("DTMotivo");
                else
                    RaisePropertyChanged("DTMotivo");
            }
        }

        private string dTJustificacion;
        public string DTJustificacion
        {
            get { return dTJustificacion; }
            set { 
                dTJustificacion = value;
                if (!string.IsNullOrWhiteSpace(dTJustificacion))
                    OnPropertyValidateChanged("DTJustificacion");
                else
                    RaisePropertyChanged("DTJustificacion");
            }
        }

        private short? dTCentroDestino;
        public short? DTCentroDestino
        {
            get { return dTCentroDestino; }
            set { 
                dTCentroDestino = value;
                if (dTCentroDestino.HasValue)
                    OnPropertyValidateChanged("DTCentroDestino");
                else
                    RaisePropertyChanged("DTCentroDestino");
            }
        }

        private string dTNoOficio;
        public string DTNoOficio
        {
            get { return dTNoOficio; }
            set { 
                dTNoOficio = value;
                if (!string.IsNullOrWhiteSpace(dTNoOficio))
                    OnPropertyValidateChanged("DTNoOficio"); 
                else
                    RaisePropertyChanged("DTNoOficio"); 
            }
        }

        #endregion

        #region Datos Egreso

        private string dENoOficio;
        public string DENoOficio
        {
            get { return dENoOficio; }
            set { 
                dENoOficio = value;
                if (!string.IsNullOrWhiteSpace(dENoOficio))
                    OnPropertyValidateChanged("DENoOficio");
                else
                    OnPropertyValidateChanged("DENoOficio");
            }
        }

        private List<string> autoridadesSalida = new List<string>();
        public List<string> AutoridadesSalida
        {
            get { return autoridadesSalida; }
            set { autoridadesSalida = value; RaisePropertyChanged("AutoridadesSalida"); }
        }

        private string autorizaSalida;
        public string AutorizaSalida
        {
            get { return autorizaSalida; }
            set { autorizaSalida = value; OnPropertyValidateChanged("AutorizaSalida"); }
        }

        private short? id_motivo_traslado;

        private string motivoSalida;
        public string MotivoSalida
        {
            get { return motivoSalida; }
            set { 
                motivoSalida = value; RaisePropertyChanged("MotivoSalida"); }
        }

        DateTime _FechaServer;
        public DateTime FechaServer
        {
            get { return _FechaServer = Fechas.GetFechaDateServer; }
            set
            {
                _FechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }
        #endregion

        #region busqueda segmentada
        private int Pagina { get; set; }

        private bool SeguirCargandoIngresos { get; set; }

        #endregion

        #region busqueda traslados
        private List<Tipo_Traslado> tipos_traslado;
         public List<Tipo_Traslado> Tipos_Traslado
        {
            get { return tipos_traslado; }
            set { tipos_traslado = value; RaisePropertyChanged("Tipos_Traslado"); }
        }

         private Tipo_Traslado selectedTipo_Traslado;
         public Tipo_Traslado SelectedTipo_Traslado
         {
             get { return selectedTipo_Traslado; }
             set { selectedTipo_Traslado = value; RaisePropertyChanged("SelectedTipo_Traslado"); }
         }

         private bool tipo_TrasladoHabilitado = false;
         public bool Tipo_TrasladoHabilitado
         {
             get { return tipo_TrasladoHabilitado; }
             set { tipo_TrasladoHabilitado = value; RaisePropertyChanged("Tipo_TrasladoHabilitado"); }
         }

         private int? anioBuscarTraslado;
         public int? AnioBuscarTraslado
         {
             get { return anioBuscarTraslado; }
             set { anioBuscarTraslado = value; RaisePropertyChanged("AnioBuscarTraslado"); }
         }

         private int? folioBuscarTraslado;
         public int? FolioBuscarTraslado
         {
             get { return folioBuscarTraslado; }
             set { folioBuscarTraslado = value; RaisePropertyChanged("FolioBuscarTraslado"); }
         }

         private string nombreBuscarTraslado;
         public string NombreBuscarTraslado
         {
             get { return nombreBuscarTraslado; }
             set { nombreBuscarTraslado = value; RaisePropertyChanged("NombreBuscarTraslado"); }
         }

         private string apellidoPaternoBuscarTraslado;
         public string ApellidoPaternoBuscarTraslado
         {
             get { return apellidoPaternoBuscarTraslado; }
             set { apellidoPaternoBuscarTraslado = value; RaisePropertyChanged("ApellidoPaternoBuscarTraslado"); }
         }

         private string apellidoMaternoBuscarTraslado;
         public string ApellidoMaternoBuscarTraslado
         {
             get { return apellidoMaternoBuscarTraslado; }
             set { apellidoMaternoBuscarTraslado = value; RaisePropertyChanged("ApellidoMaternoBuscarTraslado"); }
         }

         private DateTime? fechaBuscarTraslado;
         public DateTime? FechaBuscarTraslado
         {
             get { return fechaBuscarTraslado; }
             set { fechaBuscarTraslado = value; RaisePropertyChanged("FechaBuscarTraslado"); }
         }

         private ObservableCollection<TRASLADO> busquedaTraslado;
         public ObservableCollection<TRASLADO> BusquedaTraslado
         {
             get { return busquedaTraslado; }
             set { busquedaTraslado = value; RaisePropertyChanged("BusquedaTraslado"); }
         }
        #endregion

         #region reportes
         private List<EXT_REPORTE_TRASLADO_DETALLE> ds_detalle;
         private List<EXT_REPORTE_TRASLADO_ENCABEZADO> ds_encabezado;
         #endregion

         private short?[] estatus_inactivos = null;
     }

}
