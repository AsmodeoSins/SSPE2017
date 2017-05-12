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
using System.Windows.Media;

namespace ControlPenales
{
    partial class AtencionCitaListaViewModel
    {
        #region Variables Privadas
        private short?[] estatus_inactivos = null;
        #endregion

        #region Listado
        private ObservableCollection<ATENCION_CITA> lstAtencionCita;
        public ObservableCollection<ATENCION_CITA> LstAtencionCita
        {
            get { return lstAtencionCita; }
            set { lstAtencionCita = value; OnPropertyChanged("LstAtencionCita"); }
        }

        private ATENCION_CITA selectedAtencionCita;
        public ATENCION_CITA SelectedAtencionCita
        {
            get { return selectedAtencionCita; }
            set { selectedAtencionCita = value; OnPropertyChanged("SelectedAtencionCita"); }
        }
        #endregion

        #region Pantalla
        private Visibility listaVaciaVisible = Visibility.Collapsed;
        public Visibility ListaVaciaVisible
        {
            get { return listaVaciaVisible; }
            set { listaVaciaVisible = value; OnPropertyChanged("ListaVaciaVisible"); }
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

        private bool menuReporteEnabled = false;
        public bool MenuReporteEnabled
        {
            get { return menuReporteEnabled; }
            set { menuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }

        private bool menuFichaEnabled = false;
        public bool MenuFichaEnabled
        {
            get { return menuFichaEnabled; }
            set { menuFichaEnabled = value; OnPropertyChanged("MenuFichaEnabled"); }
        }
        #endregion

        #region Auxiliares
        private DateTime FechaHoy = Fechas.GetFechaDateServer;

        private cSolicitudCita selectedSolicitud;
        public cSolicitudCita SelectedSolicitud
        {
            get { return selectedSolicitud; }
            set
            {
                selectedSolicitud = value;
                if (value != null)
                {
                    if (value.ATENCION_INGRESO != null)
                    {
                        if (value.ATENCION_INGRESO.INGRESO != null)
                        {
                            if (value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO != null)
                            {
                                if (value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    SImagen = value.ATENCION_INGRESO.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    SImagen = new Imagenes().getImagenPerson();
                            }
                            else
                                SImagen = new Imagenes().getImagenPerson();                            
                        }
                        else
                            SImagen = new Imagenes().getImagenPerson();
                    }
                    else
                        SImagen = new Imagenes().getImagenPerson();
                }
                else
                {
                    SImagen = new Imagenes().getImagenPerson();
                }
                OnPropertyChanged("SelectedSolicitud");
            }
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
            set
            {
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
            set
            {
                aFecha = value;
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
                OnPropertyChanged("AFecha");
            }
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
            set
            {
                aHoraI = value;
                if (!value.HasValue)
                    AHorasValid = false;
                else
                    if (value >= AHoraF)
                        AHorasValid = false;
                    else
                        AHorasValid = true;
                OnPropertyChanged("AHoraI");
            }
        }

        private DateTime? aHoraF;
        public DateTime? AHoraF
        {
            get { return aHoraF; }
            set
            {
                aHoraF = value;
                if (!value.HasValue)
                    AHorasValid = false;
                else
                    if (!AHoraI.HasValue)
                        AHorasValid = false;
                    else
                        if (value <= AHoraI)
                            AHorasValid = false;
                        else
                            AHorasValid = true;

                OnPropertyChanged("AHoraF");
            }
        }

        private bool aHorasValid = false;
        public bool AHorasValid
        {
            get { return aHorasValid; }
            set { aHorasValid = value; OnPropertyChanged("AHorasValid"); }

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
            set
            {
                mensajeError = value;
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

        #region BUSQUEDA HUELLA
        private BuscarPorHuellaYNipView HuellaWindow;

        private Visibility _ShowPopUp = Visibility.Hidden;
        public Visibility ShowPopUp
        {
            get { return _ShowPopUp; }
            set
            {
                _ShowPopUp = value;
                OnPropertyChanged("ShowPopUp");
            }
        }

        private Visibility _ShowFingerPrint = Visibility.Hidden;
        public Visibility ShowFingerPrint
        {
            get { return _ShowFingerPrint; }
            set
            {
                _ShowFingerPrint = value;
                OnPropertyChanged("ShowFingerPrint");
            }
        }

        private Visibility _ShowLine = Visibility.Visible;
        public Visibility ShowLine
        {
            get { return _ShowLine; }
            set
            {
                _ShowLine = value;
                OnPropertyChanged("ShowLine");
            }
        }

        private Visibility _ShowOk = Visibility.Hidden;
        public Visibility ShowOk
        {
            get { return _ShowOk; }
            set
            {
                _ShowOk = value;
                OnPropertyChanged("ShowOk");
            }
        }

        private ImageSource _GuardaHuella;
        public ImageSource GuardaHuella
        {
            get { return _GuardaHuella; }
            set
            {
                _GuardaHuella = value;
                OnPropertyChanged("GuardaHuella");
            }
        }

        private Brush _PulgarDerecho;
        public Brush PulgarDerecho
        {
            get { return _PulgarDerecho; }
            set
            {
                _PulgarDerecho = value;
                RaisePropertyChanged("PulgarDerecho");
            }
        }

        private ImageSource _PulgarDerechoBMP;
        public ImageSource PulgarDerechoBMP
        {
            get { return _PulgarDerechoBMP; }
            set
            {
                _PulgarDerechoBMP = value;
                RaisePropertyChanged("PulgarDerechoBMP");
            }
        }

        private Brush _IndiceDerecho;
        public Brush IndiceDerecho
        {
            get { return _IndiceDerecho; }
            set
            {
                _IndiceDerecho = value;
                OnPropertyChanged("IndiceDerecho");
            }
        }

        private Brush _MedioDerecho;
        public Brush MedioDerecho
        {
            get { return _MedioDerecho; }
            set
            {
                _MedioDerecho = value;
                OnPropertyChanged("MedioDerecho");
            }
        }

        private Brush _AnularDerecho;
        public Brush AnularDerecho
        {
            get { return _AnularDerecho; }
            set
            {
                _AnularDerecho = value;
                OnPropertyChanged("AnularDerecho");
            }
        }

        private Brush _MeñiqueDerecho;
        public Brush MeñiqueDerecho
        {
            get { return _MeñiqueDerecho; }
            set
            {
                _MeñiqueDerecho = value;
                OnPropertyChanged("MeñiqueDerecho");
            }
        }

        private Brush _PulgarIzquierdo;
        public Brush PulgarIzquierdo
        {
            get { return _PulgarIzquierdo; }
            set
            {
                _PulgarIzquierdo = value;
                OnPropertyChanged("PulgarIzquierdo");
            }
        }

        private Brush _IndiceIzquierdo;
        public Brush IndiceIzquierdo
        {
            get { return _IndiceIzquierdo; }
            set
            {
                _IndiceIzquierdo = value;
                OnPropertyChanged("IndiceIzquierdo");
            }
        }

        private Brush _MedioIzquierdo;
        public Brush MedioIzquierdo
        {
            get { return _MedioIzquierdo; }
            set
            {
                _MedioIzquierdo = value;
                OnPropertyChanged("MedioIzquierdo");
            }
        }

        private Brush _AnularIzquierdo;
        public Brush AnularIzquierdo
        {
            get { return _AnularIzquierdo; }
            set
            {
                _AnularIzquierdo = value;
                OnPropertyChanged("AnularIzquierdo");
            }
        }

        private Brush _MeñiqueIzquierdo;
        public Brush MeñiqueIzquierdo
        {
            get { return _MeñiqueIzquierdo; }
            set
            {
                _MeñiqueIzquierdo = value;
                OnPropertyChanged("MeñiqueIzquierdo");
            }
        }
        private bool _IsSucceed = false;
        public bool IsSucceed
        {
            get { return _IsSucceed; }
        }
        private bool LeyendoHuellas { get; set; }
        private bool Conectado;
        public enumTipoPersona BuscarPor = enumTipoPersona.IMPUTADO;
        private Visibility _ShowCapturar = Visibility.Collapsed;
        public Visibility ShowCapturar
        {
            get { return _ShowCapturar; }
            set
            {
                _ShowCapturar = value;
                OnPropertyChanged("ShowCapturar");
            }
        }
        private Visibility _ShowContinuar = Visibility.Collapsed;
        public Visibility ShowContinuar
        {
            get { return _ShowContinuar; }
            set
            {
                _ShowContinuar = value;
                OnPropertyChanged("ShowContinuar");
            }
        }
        private Visibility _ShowLoading = Visibility.Collapsed;
        public Visibility ShowLoading
        {
            get { return _ShowLoading; }
            set
            {
                _ShowLoading = value;
                OnPropertyChanged("ShowLoading");
            }
        }
        private bool isKeepSearching { get; set; }
        private bool GuardandoHuellas { get; set; }
        private bool CancelKeepSearching { get; set; }
        private bool _GuardarHuellas { get; set; }
        public IList<PlantillaBiometrico> HuellasCapturadas { get; set; }
        private string _CabeceraBusqueda;
        public string CabeceraBusqueda
        {
            get { return _CabeceraBusqueda; }
            set
            {
                _CabeceraBusqueda = value;
                OnPropertyChanged("CabeceraBusqueda");
            }
        }
        private string _CabeceraFoto;
        public string CabeceraFoto
        {
            get { return _CabeceraFoto; }
            set
            {
                _CabeceraFoto = value;
                OnPropertyChanged("CabeceraFoto");
            }
        }
        private System.Windows.Media.Brush _ColorMessage;
        public System.Windows.Media.Brush ColorMessage
        {
            get { return _ColorMessage; }
            set
            {
                _ColorMessage = value;
                RaisePropertyChanged("ColorMessage");
            }
        }
        private enumTipoBiometrico? _DD_Dedo = enumTipoBiometrico.INDICE_DERECHO;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set
            {
                LimpiarCampos();
                _DD_Dedo = value;
                OnPropertyChanged("DD_Dedo");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set
            {
                aceptarBusquedaHuellaFocus = value;
                OnPropertyChanged("AceptarBusquedaHuellaFocus");
            }
        }
        private IList<ResultadoBusquedaBiometrico> _ListResultado;
        public IList<ResultadoBusquedaBiometrico> ListResultado
        {
            get { return _ListResultado; }
            set
            {
                _ListResultado = value;
                var bk = SelectRegistro;
                OnPropertyChanged("ListResultado");
                if (CancelKeepSearching)
                    SelectRegistro = bk;
            }
        }
        private ResultadoBusquedaBiometrico _SelectRegistro;
        public ResultadoBusquedaBiometrico SelectRegistro
        {
            get { return _SelectRegistro; }
            set
            {
                _SelectRegistro = value;
                FotoRegistro = value == null ? new Imagenes().getImagenPerson() : new Imagenes().ConvertBitmapToByte((BitmapSource)value.Foto);
                OnPropertyChanged("SelectRegistro");
            }
        }
        private byte[] _FotoRegistro = new Imagenes().getImagenPerson();
        public byte[] FotoRegistro
        {
            get { return _FotoRegistro; }
            set { _FotoRegistro = value; OnPropertyChanged("FotoRegistro"); }
        }
        private string _TextNipBusqueda;
        public string TextNipBusqueda
        {
            get { return _TextNipBusqueda; }
            set { _TextNipBusqueda = value; OnPropertyChanged("TextNipBusqueda"); }
        }
        #endregion

        #region Busqueda
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
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
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }

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

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }

        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private IMPUTADO InputadoInterno { get; set; }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    SelectIngreso = null;
                    EmptyIngresoVisible = !(selectExpediente.INGRESO.Count > 0);

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
                if (value == null)
                {
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    return;
                }
                SelectIngresoEnabled = !Parametro.ESTATUS_ADMINISTRATIVO_INACT.Contains(selectIngreso.ID_ESTATUS_ADMINISTRATIVO);
                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    OnPropertyChanged("SelectIngreso");
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
            }
        }

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }
        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool _ElementosDisponibles = false;
        public bool ElementosDisponibles
        {
            get { return _ElementosDisponibles; }
            set { _ElementosDisponibles = value; OnPropertyChanged("ElementosDisponibles"); }
        }


        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
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
    }
}
