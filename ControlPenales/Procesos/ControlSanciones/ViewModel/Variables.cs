using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class ControlSancionesViewModel
    {
        private ObservableCollection<INCIDENTE_TIPO> _ListIncidente;
        public ObservableCollection<INCIDENTE_TIPO> ListIncidente
        {
            get { return _ListIncidente; }
            set
            {
                _ListIncidente = value;
                OnPropertyChanged("ListIncidente");
            }
        }

        private INCIDENTE_TIPO _SelectIncidente;
        public INCIDENTE_TIPO SelectIncidente
        {
            get { return _SelectIncidente; }
            set
            {
                _SelectIncidente = value;
                OnPropertyChanged("SelectIncidente");
            }
        }

        private DateTime? _FechaRegistro;
        public DateTime? FechaRegistro
        {
            get { return _FechaRegistro; }
            set
            {
                _FechaRegistro = value;
                OnPropertyChanged("FechaRegistro");
            }
        }

        private DateTime? _FechaSancionesCumplidas;
        public DateTime? FechaSancionesCumplidas
        {
            get { return _FechaSancionesCumplidas; }
            set
            {
                _FechaSancionesCumplidas = value;
                FiltrarCumplimentados(value);
                OnPropertyChanged("FechaSancionesCumplidas");
            }
        }

        private string _TextMotivo;
        public string TextMotivo
        {
            get { return _TextMotivo; }
            set
            {
                _TextMotivo = value;
                OnPropertyChanged("TextMotivo");
            }
        }

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value ?? new Imagenes().getImagenPerson();
                OnPropertyChanged("ImagenImputado");
            }
        }

        private ObservableCollection<SANCION_TIPO> _ListTipoSanciones;
        public ObservableCollection<SANCION_TIPO> ListTipoSanciones
        {
            get { return _ListTipoSanciones; }
            set
            {
                _ListTipoSanciones = value;
                OnPropertyChanged("ListTipoSanciones");
            }
        }

        private SANCION_TIPO _SelectSancion;
        public SANCION_TIPO SelectSancion
        {
            get { return _SelectSancion; }
            set
            {
                _SelectSancion = value;
                OnPropertyChanged("SelectSancion");
            }
        }

        private DateTime _MinimunDate = Fechas.GetFechaDateServer;
        public DateTime MinimunDate
        {
            get { return _MinimunDate; }
            set
            {
                _MinimunDate = value;
                OnPropertyChanged("MinimunDate");
            }
        }

        private DateTime _MaximumDate = Fechas.GetFechaDateServer.AddYears(2);
        public DateTime MaximumDate
        {
            get { return _MaximumDate; }
            set
            {
                _MaximumDate = value;
                OnPropertyChanged("MaximumDate");
            }
        }

        private DateTime _FechaLowerVal;
        public DateTime FechaLowerVal
        {
            get { return _FechaLowerVal; }
            set
            {
                _FechaLowerVal = value;
                OnPropertyChanged("FechaLowerVal");
            }
        }

        private DateTime _FechaUpperVal;
        public DateTime FechaUpperVal
        {
            get { return _FechaUpperVal; }
            set
            {
                _FechaUpperVal = value;
                OnPropertyChanged("FechaUpperVal");
            }
        }

        private DateTime _HoraLowerVal;
        public DateTime HoraLowerVal
        {
            get { return _HoraLowerVal; }
            set
            {
                _HoraLowerVal = value;
                OnPropertyChanged("HoraLowerVal");
            }
        }

        private DateTime _HoraUpperVal;
        public DateTime HoraUpperVal
        {
            get { return _HoraUpperVal; }
            set
            {
                _HoraUpperVal = value;
                OnPropertyChanged("HoraUpperVal");
            }
        }

        private ObservableCollection<ListadoIncidentes> _ListIncidentes;
        public ObservableCollection<ListadoIncidentes> ListIncidentes
        {
            get { return _ListIncidentes; }
            set
            {
                _ListIncidentes = value;
                OnPropertyChanged("ListIncidentes");
            }
        }

        private ObservableCollection<ListadoIncidentes> _ListIncidentesCumplimentar;
        public ObservableCollection<ListadoIncidentes> ListIncidentesCumplimentar
        {
            get { return _ListIncidentesCumplimentar; }
            set
            {
                _ListIncidentesCumplimentar = value;
                OnPropertyChanged("ListIncidentesCumplimentar");
            }
        }

        private ListadoIncidentes _SelectIncidentes;
        public ListadoIncidentes SelectIncidentes
        {
            get { return _SelectIncidentes; }
            set
            {
                _SelectIncidentes = value;
                OnPropertyChanged("SelectIncidentes");
                if (value == null)
                    return;

                ImagenImputado = value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ? value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ? value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();

                TextMotivo = value.Motivo;
                FechaRegistro = value.Registro_Fecha.Value;
                SelectedIncidente = value.Id_Incidente_Tipo;
                CargarSancionesImputado(value.Id_Centro, value.Id_Anio, value.Id_Imputado, value.Id_Ingreso,value.Id_Incidente);

            }
        }

        private ListadoIncidentes _SelectIncidentesCumplimentar;
        public ListadoIncidentes SelectIncidentesCumplimentar
        {
            get { return _SelectIncidentesCumplimentar; }
            set
            {
                _SelectIncidentesCumplimentar = value;
                OnPropertyChanged("SelectIncidentesCumplimentar");
                if (value == null)
                    return;

                ImagenImputado = value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ? value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ? value.ImagenImputado.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : new Imagenes().getImagenPerson();

                TextMotivo = value.Motivo;
                FechaRegistro = value.Registro_Fecha.Value;
                SelectedIncidente = value.Id_Incidente_Tipo;
                CargarSancionesImputado(value.Id_Centro, value.Id_Anio, value.Id_Imputado, value.Id_Ingreso, value.Id_Incidente);

            }
        }

        private short? _SelectedIncidente = -1;
        public short? SelectedIncidente
        {
            get { return _SelectedIncidente; }
            set
            {
                _SelectedIncidente = value;
                OnPropertyChanged("SelectedIncidente");
            }
        }

        private ObservableCollection<ListadoSanciones> _ListSanciones;
        public ObservableCollection<ListadoSanciones> ListSanciones
        {
            get { return _ListSanciones; }
            set
            {
                _ListSanciones = value;
                OnPropertyChanged("ListSanciones");
            }
        }

        private ListadoSanciones _SelectSanciones;
        public ListadoSanciones SelectSanciones
        {
            get { return _SelectSanciones; }
            set
            {
                _SelectSanciones = value;
                OnPropertyChanged("SelectSanciones");
            }
        }

        private short _SelectedSancion;
        public short SelectedSancion
        {
            get { return _SelectedSancion; }
            set
            {
                _SelectedSancion = value;
                OnPropertyChanged("SelectedSancion");
            }
        }

        private bool Agregar { get; set; }

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
                //if (value)
                //    MenuGuardarEnabled = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set { pEditar = value; }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                //if (value)
                //    MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
                //if (value)
                //    MenuReporteEnabled = value;
            }
        }
        #endregion

        #region Menu
        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuBuscarEnabled = false;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }
        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        private bool menuInsertarEnabled = false;
        public bool MenuInsertarEnabled
        {
            get { return menuInsertarEnabled; }
            set { menuInsertarEnabled = value; OnPropertyChanged("MenuInsertarEnabled"); }
        }

        #endregion

        #region Pantalla
        private int tabIndex = 0;
        public int TabIndex
        {
            get { return tabIndex; }
            set { 
                tabIndex = value;
                if (value == 0)
                { 
                    SelectIncidentes = null;
                    FechaRegistro = null;
                    SelectedIncidente = -1;
                    TextMotivo = string.Empty;
                    ListSanciones = null;
                    SelectSanciones = null;
                    ImagenImputado = new Imagenes().getImagenPerson();
                }
                else if (value == 1)
                {
                    SelectIncidentesCumplimentar = null;
                    FechaRegistro = null;
                    SelectedIncidente = -1;
                    TextMotivo = string.Empty;
                    ListSanciones = null;
                    SelectSanciones = null;
                    ImagenImputado = new Imagenes().getImagenPerson();
                }
                OnPropertyChanged("TabIndex"); }
        }

      
        #endregion

        #region Reporte
        private bool esActa = false;
        public bool EsActa
        {
            get { return esActa; }
            set { esActa = value; OnPropertyChanged("EsActa"); }
        }

        private bool esParteInformativo = false;
        public bool EsParteInformativo
        {
            get { return esParteInformativo; }
            set { esParteInformativo = value; OnPropertyChanged("EsParteInformativo"); }
        }

        private bool esCitatorioInterno = false;
        public bool EsCitatorioInterno
        {
            get { return esCitatorioInterno; }
            set { esCitatorioInterno = value; OnPropertyChanged("EsCitatorioInterno"); }
        }
        #endregion

        #region Sanciones
        private short? idSancionTipo = -1;
        public short? IdSancionTipo
        {
            get { return idSancionTipo; }
            set { idSancionTipo = value; OnPropertyChanged("IdSancionTipo"); }
        }

        private DateTime? fechaInicio;
        public DateTime? FechaInicio
        {
            get { return fechaInicio; }
            set { fechaInicio = value; OnPropertyChanged("FechaInicio"); }
        }

        private DateTime? fechaFin;
        public DateTime? FechaFin
        {
            get { return fechaFin; }
            set { fechaFin = value; OnPropertyChanged("FechaFin"); }
        }

        private string horaInicio;
        public string HoraInicio
        {
            get { return horaInicio; }
            set { horaInicio = value; OnPropertyChanged("HoraInicio"); }
        }

        private string minutoInicio;
        public string MinutoInicio
        {
            get { return minutoInicio; }
            set { minutoInicio = value; OnPropertyChanged("MinutoInicio"); }
        }

        private string horaFin;
        public string HoraFin
        {
            get { return horaFin; }
            set { horaFin = value; OnPropertyChanged("HoraFin"); }
        }

        private string minutoFin;
        public string MinutoFin
        {
            get { return minutoFin; }
            set { minutoFin = value; OnPropertyChanged("MinutoFin"); }
        }

        private Visibility vAgregarSancion = Visibility.Collapsed;
        public Visibility VAgregarSancion
        {
            get { return vAgregarSancion; }
            set { vAgregarSancion = value; OnPropertyChanged("VAgregarSancion"); }
        }

        private Visibility vEditarSancion = Visibility.Collapsed;
        public Visibility VEditarSancion
        {
            get { return vEditarSancion; }
            set { vEditarSancion = value; OnPropertyChanged("VEditarSancion"); }
        }

        private Visibility vEliminarSancion = Visibility.Collapsed;
        public Visibility VEliminarSancion
        {
            get { return vEliminarSancion; }
            set { vEliminarSancion = value; OnPropertyChanged("VEliminarSancion"); }
        }
        #endregion
    }

    public class ListadoIncidentes
    {
        public short Id_Anio { get; set; }
        public short Id_Centro { get; set; }
        public int Id_Imputado { get; set; }
        public short Id_Ingreso { get; set; }
        public short Id_Incidente { get; set; }
        public short? Id_Incidente_Tipo { get; set; }
        public DateTime? Registro_Fecha { get; set; }
        public string Motivo { get; set; }
        public string Estatus { get; set; }
        public DateTime? Autorizacion_Fecha { get; set; }
        public ICollection<INGRESO_BIOMETRICO> ImagenImputado { get; set; }

        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
    }

    public class ListadoSanciones
    {
        public string STR_Sancion { get; set; }
        public DateTime? IniciaFecha { get; set; }
        public DateTime? TerminaFecha { get; set; }
        public short Id_Sancion { get; set; }
        public short Id_Centro { get; set; }
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public short Id_Ingreso { get; set; }
        public short Id_Incidente { get; set; }
    }
}
