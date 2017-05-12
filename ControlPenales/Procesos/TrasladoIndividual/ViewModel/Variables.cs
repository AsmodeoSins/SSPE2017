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
    partial class TrasladoIndividualViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region GENERALES
        private bool buscarVisible;
        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }

        public string Name
        {
            get
            {
                return "traslado_individual";
            }
        }
        #endregion

        #region Buscar
        private int? anioBuscar;
        public int? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
        }

        private string apellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string apellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set { listExpediente = value; OnPropertyChanged("ListExpediente"); }
        }

        private bool emptyExpedienteVisible = false;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = false;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        else
                            ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }

        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                    return;
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; }
        }

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; OnPropertyChanged("ImagenIngreso"); }
        }
        
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set { imagenImputado = value; OnPropertyChanged("ImagenImputado"); }
        }

        
        #endregion

        #region Kardex
        private short? anioD;
        public short? AnioD
        {
            get { return anioD; }
            set { anioD = value; OnPropertyChanged("AnioD"); }
        }

        private int? folioD;
        public int? FolioD
        {
            get { return folioD; }
            set { folioD = value; OnPropertyChanged("FolioD"); }
        }

        private string paternoD;
        public string PaternoD
        {
            get { return paternoD; }
            set { paternoD = value; OnPropertyChanged("PaternoD"); }
        }

        private string maternoD;
        public string MaternoD
        {
            get { return maternoD; }
            set { maternoD = value; OnPropertyChanged("MaternoD"); }
        }

        private string nombreD;
        public string NombreD
        {
            get { return nombreD; }
            set { nombreD = value; OnPropertyChanged("NombreD"); }
        }

        private short? ingresosD;
        public short? IngresosD
        {
            get { return ingresosD; }
            set { ingresosD = value; OnPropertyChanged("IngresosD"); }
        }

        private string noControlD;
        public string NoControlD
        {
            get { return noControlD; }
            set { noControlD = value; OnPropertyChanged("NoControlD"); }
        }

        private string ubicacionD;
        public string UbicacionD
        {
            get { return ubicacionD; }
            set { ubicacionD = value; OnPropertyChanged("UbicacionD"); }
        }

        private string tipoSeguridadD;
        public string TipoSeguridadD
        {
            get { return tipoSeguridadD; }
            set { tipoSeguridadD = value; OnPropertyChanged("TipoSeguridadD"); }
        }
        
        private DateTime? fecIngresoD;
        public DateTime? FecIngresoD
        {
            get { return fecIngresoD; }
            set { fecIngresoD = value; OnPropertyChanged("FecIngresoD"); }
        }

        private string clasificacionJuridicaD;
        public string ClasificacionJuridicaD
        {
            get { return clasificacionJuridicaD; }
            set { clasificacionJuridicaD = value; OnPropertyChanged("ClasificacionJuridicaD"); }
        }
        
        private string estatusD;
        public string EstatusD
        {
            get { return estatusD; }
            set { estatusD = value; OnPropertyChanged("EstatusD"); }
        }

        private int pagina;
        public int Pagina
        {
            get { return pagina; }
            set { pagina = value; OnPropertyChanged("Pagina"); }
        }

        private bool seguirCargando;
        public bool SeguirCargando
        {
            get { return seguirCargando; }
            set { seguirCargando = value; OnPropertyChanged("SeguirCargando"); }
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
        #endregion

        #region Traslados
        private ObservableCollection<TRASLADO> lstTraslados;
        public ObservableCollection<TRASLADO> LstTraslados
        {
            get { return lstTraslados; }
            set { lstTraslados = value; OnPropertyChanged("LstTraslados"); }
        }

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

        private ObservableCollection<TIPO_AUTORIDAD_SALIDA> lstAutoridadSalida;
        public ObservableCollection<TIPO_AUTORIDAD_SALIDA> LstAutoridadSalida
        {
            get { return lstAutoridadSalida; }
            set { lstAutoridadSalida = value; OnPropertyChanged("LstAutoridadSalida"); }
        }

        private ObservableCollection<cAuxiliar> lstEmpleado;
        public ObservableCollection<cAuxiliar> LstEmpleado
        {
            get { return lstEmpleado; }
            set { lstEmpleado = value; OnPropertyChanged("LstEmpleado"); }
        }

        private ObservableCollection<TRASLADO_MOTIVO_SALIDA> lstMotivoSalida;
        public ObservableCollection<TRASLADO_MOTIVO_SALIDA> LstMotivoSalida
        {
            get { return lstMotivoSalida; }
            set { lstMotivoSalida = value; OnPropertyChanged("LstMotivoSalida"); }
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

        private bool dEFechaValid = false;
        public bool DEFechaValid
        {
            get { return dEFechaValid; }
            set { dEFechaValid = value; OnPropertyChanged("DEFechaValid"); }
        }

        private DateTime? dTFecha;
        public DateTime? DTFecha
        {
            get { return dTFecha; }
            set { dTFecha = value;
            if (value != null)
                DTFechaValid = true;
            else
                DTFechaValid = false;
                OnPropertyChanged("DTFecha"); }
        }

        private short? dTMotivo;
        public short? DTMotivo
        {
            get { return dTMotivo; }
            set { dTMotivo = value; OnPropertyChanged("DTMotivo"); }
        }

        private string dTJustificacion;
        public string DTJustificacion
        {
            get { return dTJustificacion; }
            set { dTJustificacion = value; OnPropertyChanged("DTJustificacion"); }
        }

        private short? dTCentroDestino;
        public short? DTCentroDestino
        {
            get { return dTCentroDestino; }
            set { dTCentroDestino = value; OnPropertyChanged("DTCentroDestino"); }
        }

        private string dTNoOficio;
        public string DTNoOficio
        {
            get { return dTNoOficio; }
            set { dTNoOficio = value; OnPropertyChanged("DTNoOficio"); }
        }

        private int? dTAutorizado;
        public int? DTAutorizado
        {
            get { return dTAutorizado; }
            set { dTAutorizado = value; OnPropertyChanged("DTAutorizado"); }
        }
        #endregion

        #region Datos Egreso
        private DateTime? dEFecha;
        public DateTime? DEFecha
        {
            get { return dEFecha; }
            set { dEFecha = value;
            if (value != null)
                DEFechaValid = true;
            else
                DEFechaValid = false;
                OnPropertyChanged("DEFecha"); }
        }

        private string dENoOficio;
        public string DENoOficio
        {
            get { return dENoOficio; }
            set { dENoOficio = value; OnPropertyChanged("DENoOficio"); }
        }

        private short? dEAutoridad;
        public short? DEAutoridad
        {
            get { return dEAutoridad; }
            set { dEAutoridad = value; OnPropertyChanged("DEAutoridad"); }
        }

        private short? dEMotivo;
        public short? DEMotivo
        {
            get { return dEMotivo; }
            set { dEMotivo = value; OnPropertyChanged("DEMotivo"); }
        }
        #endregion
    }
}
