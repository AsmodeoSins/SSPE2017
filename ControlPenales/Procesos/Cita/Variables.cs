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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class CitaViewModel : ValidationViewModelBase
    {
        #region Variables Privadas
        private List<short> areas_tecnicas_rol= new List<short>();
        private IQueryable<PROCESO_USUARIO> permisos;
        private List<short> roles = null;
        private short?[] estatus_inactivos = null;
        #endregion

        #region Citas
        private ObservableCollection<AREA_TECNICA> lstArea;
        public ObservableCollection<AREA_TECNICA> LstArea
        {
            get { return lstArea; }
            set { lstArea = value; OnPropertyChanged("LstArea"); }
        }

        private int Pagina { get; set; }
        private bool SeguirCargandoIngresos { get; set; }

        private RangeEnabledObservableCollection<cSolicitudCita> lstSolicitudes;
        public RangeEnabledObservableCollection<cSolicitudCita> LstSolicitudes
        {
            get { return lstSolicitudes; }
            set { lstSolicitudes = value;
                OnPropertyChanged("LstSolicitudes"); }
        }

        private cSolicitudCita selectedSolicitud;
        public cSolicitudCita SelectedSolicitud
        {
            get { return selectedSolicitud; }
            set { selectedSolicitud = value;
            if (value != null)
            {
                if (value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    SImagen = value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    SImagen = new Imagenes().getImagenPerson();

                if (value.ATENCION_INGRESO.ESTATUS == (short)enumSolicitudCita.SOLICITADA)
                    CMenuVisible = Visibility.Visible;
                else
                    CMenuVisible = Visibility.Collapsed;
            }
            else
            {
                SImagen = new Imagenes().getImagenPerson();
                CMenuVisible = Visibility.Collapsed;
            }
                OnPropertyChanged("SelectedSolicitud"); }
        }
        
        private Visibility emptySolicitudes = Visibility.Visible;
        public Visibility EmptySolicitudes
        {
            get { return emptySolicitudes; }
            set { emptySolicitudes = value; OnPropertyChanged("EmptySolicitudes"); }
        }

        private short? cAnio;
        public short? CAnio
        {
            get { return cAnio; }
            set { cAnio = value; OnPropertyChanged("CAnio"); }
        }

        private int? cFolio;
        public int? CFolio
        {
            get { return cFolio; }
            set { cFolio = value; OnPropertyChanged("CFolio"); }
        }

        private string cNombre;
        public string CNombre
        {
            get { return cNombre;  }
            set { cNombre = value; OnPropertyChanged("CNombre"); }
        }

        private string cPaterno;
        public string CPaterno
        {
            get { return cPaterno; }
            set { cPaterno = value; OnPropertyChanged("CPaterno"); }
        }

        private string cMaterno;
        public string CMaterno
        {
            get { return cMaterno; }
            set { cMaterno = value; OnPropertyChanged("CMaterno"); }
        }

        private DateTime? cFecha;
        public DateTime? CFecha
        {
            get { return cFecha; }
            set { cFecha = value; OnPropertyChanged("CFecha"); }
        }

        private short? cArea = -1;
        public short? CArea
        {
            get { return cArea; }
            set { cArea = value; OnPropertyChanged("CArea"); }
        }

        private short? cEstatus = (short)enumSolicitudCita.SOLICITADA;
        public short? CEstatus
        {
            get { return cEstatus; }
            set { cEstatus = value; OnPropertyChanged("CEstatus"); }
        }

        private Visibility cMenuVisible = Visibility.Collapsed;
        public Visibility CMenuVisible
        {
            get { return cMenuVisible; }
            set { cMenuVisible = value; OnPropertyChanged("CMenuVisible"); }
        }
        #endregion

        #region Configuracion Permisos
        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
                if (value)
                    MenuGuardarEnabled = value;
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
                if (value)
                    MenuBuscarEnabled = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
                if (value)
                    MenuReporteEnabled = value;
            }
        }
        #endregion

        #region Menu
        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; RaisePropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        
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
        #endregion
        
        #region Agenda
        private string tituloAgenda = string.Empty;
        public string TituloAgenda
        {
            get { return tituloAgenda; }
            set { tituloAgenda = value; RaisePropertyChanged("TituloAgenda"); }
        }

        private bool isAgendarCitaEnabled = false;
        public bool IsAgendarCitaEnabled
        {
            get { return isAgendarCitaEnabled; }
            set { isAgendarCitaEnabled = value; RaisePropertyChanged("IsAgendarCitaEnabled"); }
        }

        private List<cUsuarioExtendida> lstEmpleados = new List<cUsuarioExtendida>();
        public List<cUsuarioExtendida> LstEmpleados
        {
            get { return lstEmpleados; }
            set { lstEmpleados = value; RaisePropertyChanged("LstEmpleados"); }
        }

        private int selectedEmpleadoValue = -1;
        public int SelectedEmpleadoValue
        {
            get { return selectedEmpleadoValue; }
            set { selectedEmpleadoValue = value; RaisePropertyChanged("SelectedEmpleadoValue"); }
        }

        private bool isEmpleadoEnabled = false;
        public bool IsEmpleadoEnabled
        {
            get { return isEmpleadoEnabled; }
            set { isEmpleadoEnabled = value; RaisePropertyChanged("IsEmpleadoEnabled"); }
        }

        private DateTime? selectedDateCalendar;
        public DateTime? SelectedDateCalendar
        {
            get { return selectedDateCalendar; }
            set { selectedDateCalendar = value; RaisePropertyChanged("SelectedDateCalendar"); }
        }
        private AgendarCitaView _AgendaView;
        public AgendarCitaView AgendaView
        {
            get { return _AgendaView; }
            set { _AgendaView = value; }
        }
        private ObservableCollection<Appointment> lstAgenda;
        public ObservableCollection<Appointment> LstAgenda
        {
            get { return lstAgenda; }
            set { 
                lstAgenda = value; 
                OnPropertyChanged("LstAgenda"); 
            }
        }

        private bool agregarHora = false;
        public bool AgregarHora
        {
            get { return agregarHora; }
            set { agregarHora = value; OnPropertyChanged("AgregarHora"); }
        }

        private byte[] sImagen = new Imagenes().getImagenPerson();
        public byte[] SImagen
        {
            get { return sImagen; }
            set { sImagen = value; OnPropertyChanged("SImagen"); }
        }


        private DateTime? aFecha;
        public DateTime? AFecha
        {
            get { return aFecha; }
            set { aFecha = value;
            if (value.HasValue)
            {
                var hoy = Fechas.GetFechaDateServer;
                var now = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                if (value >= now)
                    AFechaValid = true;
                else
                {
                    AFechaValid = false;
                    AFechaMensaje = "La fecha a agendar debe ser igual o mayor al dia de hoy.";
                }
            }
            else
            { 
                AFechaMensaje = "La fecha es requerida..";
                AFechaValid = false;
            }
                OnPropertyChanged("AFecha"); }
        }

        private bool aFechaValid = false;
        public bool AFechaValid
        {
            get { return aFechaValid; }
            set { aFechaValid = value; OnPropertyChanged("AFechaValid"); }
        }

        private DateTime? aHoraI;
        public DateTime? AHoraI
        {
            get { return aHoraI; }
            set { aHoraI = value;
            if (!value.HasValue)
                AHorasValid = false;
            else
                if (value >= AHoraF)
                    AHorasValid = false;
                else
                    AHorasValid = true;
                OnPropertyChanged("AHoraI"); }
        }

        private DateTime? aHoraF;
        public DateTime? AHoraF
        {
            get { return aHoraF; }
            set { aHoraF = value;
            if(!value.HasValue)
                AHorasValid = false;
            else
            if (!AHoraI.HasValue)
                AHorasValid = false;
            else
                if (value <= AHoraI)
                    AHorasValid = false;
                else
                    AHorasValid = true;

                OnPropertyChanged("AHoraF"); }
        }

        private bool aHorasValid = false;
        public bool AHorasValid
        {
            get { return aHorasValid; }
            set { aHorasValid = value; OnPropertyChanged("AHorasValid");}

        }

        private string aFechaMensaje = "La fecha es requerida.";
        public string AFechaMensaje
        {
            get { return aFechaMensaje; }
            set { aFechaMensaje = value; OnPropertyChanged("AFechaMensaje"); }
        }

        private string mensajeError;
        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value;
            OnPropertyChanged("MensajeError");
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MensajeError = string.Empty; });
            }));
            }
        }

        #region variables privadas
        private cUsuarioExtendida _empleado = null;
        #endregion
        #endregion

        #region Fecha
        private DateTime? fHoy = Fechas.GetFechaDateServer;

        #endregion
    }
}
