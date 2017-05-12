using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlInternos;
using ControlPenales.Clases.ControlProgramas;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ControlPenales
{
    partial class NewControlInternosViewModel
    {
        #region Filtros
        private ObservableCollection<EDIFICIO> lstEdificio;
        public ObservableCollection<EDIFICIO> LstEdificio
        {
            get { return lstEdificio; }
            set { lstEdificio = value; OnPropertyChanged("LstEdificio"); }
        }

        private EDIFICIO selectedEdificio;
        public EDIFICIO SelectedEdificio
        {
            get { return selectedEdificio; }
            set { selectedEdificio = value;

                LstSector = new ObservableCollection<SECTOR>(value.SECTOR);
                LstSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "TODOS" });
                FSector = -1;
                OnPropertyChanged("SelectedEdificio"); }
        }

        private short fEdificio = -1;
        public short FEdificio
        {
            get { return fEdificio; }
            set { fEdificio = value; OnPropertyChanged("FEdificio"); }
        }

        private ObservableCollection<SECTOR> lstSector;
        public ObservableCollection<SECTOR> LstSector
        {
            get { return lstSector; }
            set { lstSector = value; OnPropertyChanged("LstSector"); }
        }

        private short fSector = -1;
        public short FSector
        {
            get { return fSector; }
            set { fSector = value; OnPropertyChanged("FSector"); }
        }

        private DateTime? fFechaInicio;
        public DateTime? FFechaInicio
        {
            get { return fFechaInicio; }
            set { fFechaInicio = value; OnPropertyChanged("FFechaInicio"); }
        }

        private DateTime? fFechaFinal;
        public DateTime? FFechaFinal
        {
            get { return fFechaFinal; }
            set { fFechaFinal = value; OnPropertyChanged("FFechaFinal"); }
        }
        #endregion

        #region Control Internos
        private ObservableCollection<cControlInternoEdificio> lstInternosRequeridos;
        public ObservableCollection<cControlInternoEdificio> LstInternosRequeridos
        {
            get { return lstInternosRequeridos; }
            set { lstInternosRequeridos = value; OnPropertyChanged("LstInternosRequeridos"); }
        }

        private ObservableCollection<cControlInternoEdificio> lstInternosRequeridosSeleccionados;

        private ObservableCollection<cControlInternoEdificio> lstInternosAusentes;
        public ObservableCollection<cControlInternoEdificio> LstInternosAusentes
        {
            get { return lstInternosAusentes; }
            set { lstInternosAusentes = value; OnPropertyChanged("LstInternosAusentes"); }
        }

        private ObservableCollection<cControlInternoEdificio> lstInternosAusentesSeleccionados;
        #endregion

        #region Custodio
        private Visibility custodioHuellasVisibles = Visibility.Collapsed;
        public Visibility CustodioHuellasVisibles
        {
            get { return custodioHuellasVisibles; }
            set { custodioHuellasVisibles = value; OnPropertyChanged("CustodioHuellasVisibles"); }
        }

        private SSP.Servidor.PERSONA selectedCustodio;
        public SSP.Servidor.PERSONA SelectedCustodio
        {
            get { return selectedCustodio; }
            set { selectedCustodio = value;
            if (value != null)
            {
                AutorizarBtnEnabled = true;
                FolioBuscar = value.ID_PERSONA;
                NombreBuscar = value.NOMBRE;
                ApellidoPaternoBuscar = value.PATERNO;
                ApellidoMaternoBuscar = value.MATERNO;
                if (value.PERSONA_BIOMETRICO != null)
                {
                    var biometrico = value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                    if (biometrico != null)
                    {
                        if (biometrico.BIOMETRICO != null)
                            ImagenCustodio = biometrico.BIOMETRICO;
                        else
                            ImagenCustodio = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenCustodio = new Imagenes().getImagenPerson();
                }
                else
                    ImagenCustodio = new Imagenes().getImagenPerson();
            }
            else
            {
                FolioBuscar = null;
                NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                ImagenCustodio = new Imagenes().getImagenPerson();
            }
                OnPropertyChanged("SelectedCustodio"); }
        }

        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
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

        private string nombreBuscar;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set { nombreBuscar = value; OnPropertyChanged("NombreBuscar"); }
        }

        private byte[] imagenCustodio = new Imagenes().getImagenPerson();
        public byte[] ImagenCustodio
        {
            get { return imagenCustodio; }
            set { imagenCustodio = value; OnPropertyChanged("ImagenCustodio"); }
        }
        #endregion

        #region Pantalla
        private bool modoAlternativo = false;
        public bool ModoAlternativo
        {
            get { return modoAlternativo; }
            set { modoAlternativo = value;
            if (value)
                AutorizarBtnEnabled = false;
            else
                AutorizarBtnEnabled = true;
                CustodioHuellasVisibles = value ? Visibility.Visible : Visibility.Collapsed;
                SelectedCustodio = null;
                OnPropertyChanged("ModoAlternativo"); }
        }

        private bool modoAlternativoHabilitado;
        public bool ModoAlternativoHabilitado
        {
            get { return modoAlternativoHabilitado; }
            set { modoAlternativoHabilitado = value; OnPropertyChanged("ModoAlternativoHabilitado"); }
        }

        private short indiceTab = 0;
        public short IndiceTab
        {
            get { return indiceTab; }
            set { indiceTab = value;
            if (value == 0)
            {
                ModoAlternativoHabilitado = true;
                TipoTexto = "Entrada";
                if (ModoAlternativo)
                    CustodioHuellasVisibles = Visibility.Visible;
                else
                    CustodioHuellasVisibles = Visibility.Collapsed;
                TotalInternos = string.Format("Total de Internos: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count : 0);
                TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosRequeridos != null ? LstInternosRequeridos.Count(w => w.SELECCIONE) : 0);
            }
            else
            {
                ModoAlternativoHabilitado = false;
                TipoTexto = "Salida";
                CustodioHuellasVisibles = Visibility.Collapsed;
                TotalInternos = string.Format("Total de Internos: {0}",LstInternosAusentes != null ? LstInternosAusentes.Count : 0);
                TotalSeleccionados = string.Format("Total de Internos Seleccionados: {0}", LstInternosAusentes != null ? LstInternosAusentes.Count(w => w.SELECCIONE) : 0);
            }

                OnPropertyChanged("IndiceTab"); }
        }

        private bool selectRequerido;
        public bool SelectRequerido
        {
            get { return selectRequerido; }
            set { selectRequerido = value;
            if (value)
                IndiceTab = 0;
            OnPropertyChanged("SelectRequerido");
            }
        }
        
        private bool selectAusente;
        public bool SelectAusente
        {
            get { return selectAusente; }
            set { selectAusente = value;
            if (value)
                IndiceTab = 1;
                OnPropertyChanged("SelectAusente"); }
        }

        private string totalInternos;
        public string TotalInternos
        {
            get { return totalInternos; }
            set { totalInternos = value; OnPropertyChanged("TotalInternos"); }
        }

        private string totalSeleccionados;
        public string TotalSeleccionados
        {
            get { return totalSeleccionados; }
            set { totalSeleccionados = value; OnPropertyChanged("TotalSeleccionados"); }
        }

        private string tipoTexto = "Salida";
        public string TipoTexto
        {
            get { return tipoTexto; }
            set { tipoTexto = value; OnPropertyChanged("TipoTexto"); }
        }

        private bool autorizarBtnEnabled = true;
        public bool AutorizarBtnEnabled
        {
            get { return autorizarBtnEnabled; }
            set { autorizarBtnEnabled = value; OnPropertyChanged("AutorizarBtnEnabled"); }
        }

        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        #endregion

        #region Timer
        private DispatcherTimer timer;
        private DateTime FechaActualizacion;
        private string hora;
        private string minuto;
        private string segundo;
        private string _actualizacion;
        public string Actualizacion
        {
            get { return _actualizacion; }
            set
            {
                if (value != null)
                {
                    _actualizacion = value;
                    OnPropertyChanged("Actualizacion");
                }
            }
        }
        private short tMinuto = 1;
        private short tSegundo = 59;
        #endregion

        #region Permisos
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }

    public class cControlInternoEdificio
    {
        public bool SELECCIONE { get; set; }
        public short UBICACION { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string EXPEDIENTE { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public short? ID_UB_CENTRO { get; set; }
        public short? ID_UB_EDIFICIO { get; set; }
        public short? ID_UB_SECTOR { get; set; }
        public string ID_UB_CELDA { get; set; }
        public short? ID_UB_CAMA { get; set; }
        public string UBICACION_CENTRO { get; set; }
        public string UBICACION_ACTUAL { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public DateTime? FECHA_ACTIVIDAD { get; set; }
        public DateTime? HORA_ACTIVIDAD { get; set; }
        public short? ID_AREA { get; set; }
        public string AREA { get; set; }
        public string ACTIVIDAD { get; set; }
        public short? TIPO { get; set; }
    }
}
