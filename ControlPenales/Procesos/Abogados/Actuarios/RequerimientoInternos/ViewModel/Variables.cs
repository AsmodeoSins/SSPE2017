using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Controls;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using DPUruNet;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using WPFPdfViewer;
using System.Windows.Media;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RequerimientoInternosViewModel
    {
        #region Text
        private string _TextFolioImputado;
        public string TextFolioImputado
        {
            get { return _TextFolioImputado; }
            set
            {
                _TextFolioImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextFolioImputado");
            }
        }
        private string _TextAnioImputado;
        public string TextAnioImputado
        {
            get { return _TextAnioImputado; }
            set
            {
                _TextAnioImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextAnioImputado");
            }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set
            {
                _TextPaternoImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextPaternoImputado");
            }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set
            {
                _TextMaternoImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextMaternoImputado");
            }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set
            {
                _TextNombreImputado = value;
                FiltrarImputados();
                OnPropertyChanged("TextNombreImputado");
            }
        }

        private string textNombre;
        public string TextNombre
        {
            get { return textNombre; }
            set { textNombre = value; OnPropertyChanged("TextNombre"); }
        }
        private string textPaterno;
        public string TextPaterno
        {
            get { return textPaterno; }
            set { textPaterno = value; OnPropertyChanged("TextPaterno"); }
        }
        private string textMaterno;
        public string TextMaterno
        {
            get { return textMaterno; }
            set { textMaterno = value; OnPropertyChanged("TextMaterno"); }
        }
        private string textCodigo;
        public string TextCodigo
        {
            get { return textCodigo; }
            set { textCodigo = value; OnPropertyChanged("TextCodigo"); }
        }
        private string textNombreAbogado;
        public string TextNombreAbogado
        {
            get { return textNombreAbogado; }
            set { textNombreAbogado = value; OnPropertyChanged("TextNombreAbogado"); }
        }
        private string textPaternoAbogado;
        public string TextPaternoAbogado
        {
            get { return textPaternoAbogado; }
            set { textPaternoAbogado = value; OnPropertyChanged("TextPaternoAbogado"); }
        }
        private string textMaternoAbogado;
        public string TextMaternoAbogado
        {
            get { return textMaternoAbogado; }
            set { textMaternoAbogado = value; OnPropertyChanged("TextMaternoAbogado"); }
        }
        private string textCodigoAbogado;
        public string TextCodigoAbogado
        {
            get { return textCodigoAbogado; }
            set { textCodigoAbogado = value; OnPropertyChanged("TextCodigoAbogado"); }
        }
        private string textCurp;
        public string TextCurp
        {
            get { return textCurp; }
            set { textCurp = value; OnPropertyChanged("TextCurp"); }
        }
        private string textRfc;
        public string TextRfc
        {
            get { return textRfc; }
            set { textRfc = value; OnPropertyChanged("TextRfc"); }
        }
        private string textTelefonoFijo;
        public string TextTelefonoFijo
        {
            get
            {
                if (textTelefonoFijo == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoFijo);
            }
            set { textTelefonoFijo = value; OnPropertyChanged("TextTelefonoFijo"); }
        }
        private string textTelefonoMovil;
        public string TextTelefonoMovil
        {
            get
            {
                if (textTelefonoMovil == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefonoMovil);
            }
            set { textTelefonoMovil = value; OnPropertyChanged("TextTelefonoMovil"); }
        }
        private string textCorreo;
        public string TextCorreo
        {
            get { return textCorreo; }
            set { textCorreo = value; OnPropertyChanged("TextCorreo"); }
        }
        private string textIne;
        public string TextIne
        {
            get { return textIne; }
            set { textIne = value; OnPropertyChanged("TextIne"); }
        }
        private string cedulaCJF = "Credencial CJF";
        public string CedulaCJF
        {
            get { return cedulaCJF; }
            set { cedulaCJF = value; OnPropertyChanged("CedulaCJF"); }
        }
        private string textCedulaCJF;
        public string TextCedulaCJF
        {
            get { return textCedulaCJF; }
            set { textCedulaCJF = value; OnPropertyChanged("TextCedulaCJF"); }
        }
        private string textNip;
        public string TextNip
        {
            get { return textNip; }
            set { textNip = value; OnPropertyChanged("TextNip"); }
        }
        #endregion

        #region List
        private ObservableCollection<string> _ListFechasGuardadas;
        public ObservableCollection<string> ListFechasGuardadas
        {
            get { return _ListFechasGuardadas; }
            set { _ListFechasGuardadas = value; OnPropertyChanged("ListFechasGuardadas"); }
        }
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> _ListPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return _ListPersonas; }
            set { _ListPersonas = value; OnPropertyChanged("ListPersonas"); }
        }
        private ObservableCollection<ESTATUS_VISITA> _ListEstatus;
        public ObservableCollection<ESTATUS_VISITA> ListEstatus
        {
            get { return _ListEstatus; }
            set { _ListEstatus = value; OnPropertyChanged("ListEstatus"); }
        }
        private ObservableCollection<JUZGADO> _ListJuzgado;
        public ObservableCollection<JUZGADO> ListJuzgado
        {
            get { return _ListJuzgado; }
            set { _ListJuzgado = value; OnPropertyChanged("ListJuzgado"); }
        }
        private ObservableCollection<InternoPorNotificar> _ListInternosSeleccionadosPorNotificarAuxiliar;
        public ObservableCollection<InternoPorNotificar> ListInternosSeleccionadosPorNotificarAuxiliar
        {
            get { return _ListInternosSeleccionadosPorNotificarAuxiliar; }
            set { _ListInternosSeleccionadosPorNotificarAuxiliar = value; OnPropertyChanged("ListInternosSeleccionadosPorNotificarAuxiliar"); }
        }
        private ObservableCollection<InternoPorNotificar> _ListInternosSeleccionadosPorNotificar;
        public ObservableCollection<InternoPorNotificar> ListInternosSeleccionadosPorNotificar
        {
            get { return _ListInternosSeleccionadosPorNotificar; }
            set { _ListInternosSeleccionadosPorNotificar = value; OnPropertyChanged("ListInternosSeleccionadosPorNotificar"); }
        }
        private ObservableCollection<ACTUARIO_INGRESO> _ListInternosPorNotificar;
        public ObservableCollection<ACTUARIO_INGRESO> ListInternosPorNotificar
        {
            get { return _ListInternosPorNotificar; }
            set { _ListInternosPorNotificar = value; OnPropertyChanged("ListInternosPorNotificar"); }
        }
        #endregion

        #region Select
        private short? _SelectEstatusVisita;
        public short? SelectEstatusVisita
        {
            get { return _SelectEstatusVisita; }
            set { _SelectEstatusVisita = value; OnPropertyChanged("SelectEstatusVisita"); }
        }
        private short? selectJuzgado;
        public short? SelectJuzgado
        {
            get { return selectJuzgado; }
            set { selectJuzgado = value; OnPropertyChanged("SelectJuzgado"); }
        }
        private JUZGADO selectJuzgadoItem;
        public JUZGADO SelectJuzgadoItem
        {
            get { return selectJuzgadoItem; }
            set { selectJuzgadoItem = value; OnPropertyChanged("SelectJuzgadoItem"); }
        }
        private string selectSexo;
        public string SelectSexo
        {
            get { return selectSexo; }
            set { selectSexo = value; OnPropertyChanged("SelectSexo"); }
        }
        private string _SelectFechaAlta;
        public string SelectFechaAlta
        {
            get { return _SelectFechaAlta; }
            set { _SelectFechaAlta = value; OnPropertyChanged("SelectFechaAlta"); }
        }
        private string _SelectFechaGuardada;
        public string SelectFechaGuardada
        {
            get { return _SelectFechaGuardada; }
            set
            {
                _SelectFechaGuardada = value;
                var hoy = Fechas.GetFechaDateServer;
                ListInternosSeleccionadosPorNotificarAuxiliar = new ObservableCollection<InternoPorNotificar>();
                var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                if (value != null)

                if (value == "NUEVO")
                {
                    ListInternosPorNotificar = new ObservableCollection<ACTUARIO_INGRESO>();
                    SelectFechaAsignacion = new Nullable<DateTime>();
                }
                    else
                        if (DateTime.Parse(value) > hoy.AddDays(-1) && SelectPersona != null)
                {
                    //MenuGuardarEnabled = false;
                    SelectFechaAsignacion = DateTime.Parse(value);
                    var val = DateTime.Parse(value);
                    ListInternosPorNotificar = SelectPersona.ABOGADO.ACTUARIO_LISTA.Where(w => w.CAPTURA_FEC.HasValue ?
                       value == null ?
                           (w.CAPTURA_FEC.Value.Day == hoy.Day && w.CAPTURA_FEC.Value.Month == hoy.Month && w.CAPTURA_FEC.Value.Year == hoy.Year)
                               : (w.CAPTURA_FEC.Value.Day == val.Day && w.CAPTURA_FEC.Value.Month == val.Month && w.CAPTURA_FEC.Value.Year == val.Year)
                           : false).Any() ?
                                new ObservableCollection<ACTUARIO_INGRESO>(SelectPersona.ABOGADO.ACTUARIO_LISTA.Where(w => w.CAPTURA_FEC.HasValue ?
                                    value == null ?
                                        (w.CAPTURA_FEC.Value.Day == hoy.Day && w.CAPTURA_FEC.Value.Month == hoy.Month && w.CAPTURA_FEC.Value.Year == hoy.Year)
                                   : (w.CAPTURA_FEC.Value.Day == val.Day && w.CAPTURA_FEC.Value.Month == val.Month && w.CAPTURA_FEC.Value.Year == val.Year)
                               : false).FirstOrDefault().ACTUARIO_INGRESO.OrderBy(o => o.INGRESO.ID_ANIO).ThenBy(t => t.INGRESO.ID_IMPUTADO))
                           : new ObservableCollection<ACTUARIO_INGRESO>();

                    //Se modifica codigo de candado de traslado para manejarlo como IEnumerable debido a un problema con la funcion Entityfunctions.AddHours que marca un error 
                    // invalid interval debido al manejo de una variable en la busqueda.
                }
                var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                var consulta = new cIngreso().ObtenerIngresosPorJuzgado(SelectJuzgadoItem.ID_PAIS,
                    SelectJuzgadoItem.ID_ENTIDAD, SelectJuzgadoItem.ID_MUNICIPIO, SelectJuzgadoItem.ID_JUZGADO, SelectJuzgadoItem.ID_FUERO).Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro &&
                        !EstatusInactivos.Any(wh => wh.HasValue ? wh.Value == w.ID_ESTATUS_ADMINISTRATIVO : false)).AsEnumerable().Where(w => !w.TRASLADO_DETALLE
                        .Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false) && wh.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= hoy));
                if (consulta.Any())
                {
                    ListInternosSeleccionadosPorNotificarAuxiliar = new ObservableCollection<InternoPorNotificar>(consulta.AsEnumerable()
                               .Select(s => new InternoPorNotificar
                               {
                                   INGRESO = s,
                                   ELEGIDO = ListInternosPorNotificar.Where(w => w.ID_CENTRO == s.ID_CENTRO && w.ID_ANIO == s.ID_ANIO &&
                                       w.ID_IMPUTADO == s.ID_IMPUTADO && w.ID_INGRESO == s.ID_INGRESO).Any()
                               }));
                }
                ListInternosSeleccionadosPorNotificar = new ObservableCollection<InternoPorNotificar>(ListInternosSeleccionadosPorNotificarAuxiliar);
                OnPropertyChanged("SelectFechaGuardada");
            }
        }
        private DateTime? _SelectFechaAsignacion = null;
        public DateTime? SelectFechaAsignacion
        {
            get { return _SelectFechaAsignacion; }
            set { _SelectFechaAsignacion = value; OnPropertyChanged("SelectFechaAsignacion"); }
        }
        private DateTime? _SelectFechaNacimiento = null;
        public DateTime? SelectFechaNacimiento
        {
            get { return _SelectFechaNacimiento; }
            set { _SelectFechaNacimiento = value; OnPropertyChanged("SelectFechaNacimiento"); }
        }
        private SSP.Servidor.PERSONA _SelectPersona;
        private SSP.Servidor.PERSONA SelectPersonaAuxiliar;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return _SelectPersona; }
            set
            {
                _SelectPersona = value;
                ImagenPersona = value == null ?
                    new Imagenes().getImagenPerson() :
                        value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectPersona");
            }
        }
        #endregion

        #region Huellas
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
        IList<PlantillaBiometrico> HuellasCapturadas;
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
        #endregion

        #region Otros
        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonasAuxiliar;
        private bool _HeaderSortin;
        public bool HeaderSortin
        {
            get { return _HeaderSortin; }
            set { _HeaderSortin = value; }
        }
        private bool _MenuInsertarEnabled = false;
        public bool MenuInsertarEnabled
        {
            get { return _MenuInsertarEnabled; }
            set { _MenuInsertarEnabled = value; OnPropertyChanged("MenuInsertarEnabled"); }
        }
        private bool BanderaSelectAll = false;
        private bool _SeleccionarTodoInternos = false;
        public bool SeleccionarTodoInternos
        {
            get { return _SeleccionarTodoInternos; }
            set
            {
                _SeleccionarTodoInternos = value;

                OnPropertyChanged("SeleccionarTodoInternos");
            }
        }
        private bool _SeleccionarTodosEnable = false;
        public bool SeleccionarTodosEnable
        {
            get { return _SeleccionarTodosEnable; }
            set { _SeleccionarTodosEnable = value; OnPropertyChanged("SeleccionarTodosEnable"); }
        }
        private bool _Credencializado;
        public bool Credencializado
        {
            get { return _Credencializado; }
            set { _Credencializado = value; OnPropertyChanged("Credencializado"); }
        }
        private byte[] _ImagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return _ImagenPersona; }
            set { _ImagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }
        private byte[] _ImagenAbogado = new Imagenes().getImagenPerson();
        public byte[] ImagenAbogado
        {
            get { return _ImagenAbogado; }
            set { _ImagenAbogado = value; OnPropertyChanged("ImagenAbogado"); }
        }
        private bool emptyBuscarRelacionInternoVisible = false;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return emptyBuscarRelacionInternoVisible; }
            set { emptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
        private void FiltrarImputados()
        {
            if (ListInternosSeleccionadosPorNotificarAuxiliar != null)
                ListInternosSeleccionadosPorNotificar = new ObservableCollection<InternoPorNotificar>(ListInternosSeleccionadosPorNotificarAuxiliar.Where(w =>
                    (string.IsNullOrEmpty(TextAnioImputado) ? true : w.INGRESO.ID_ANIO.ToString().Contains(TextAnioImputado)) &&
                    (string.IsNullOrEmpty(TextFolioImputado) ? true : w.INGRESO.ID_IMPUTADO.ToString().Contains(TextFolioImputado)) &&
                    (string.IsNullOrEmpty(TextPaternoImputado) ? true : (w.INGRESO.IMPUTADO.PATERNO.Contains(TextPaternoImputado)) || (w.INGRESO.IMPUTADO.ALIAS.Any(a=>a.PATERNO.Contains(TextPaternoImputado)))) &&
                    (string.IsNullOrEmpty(TextMaternoImputado) ? true : (w.INGRESO.IMPUTADO.MATERNO.Contains(TextMaternoImputado)) || (w.INGRESO.IMPUTADO.ALIAS.Any(a => a.MATERNO.Contains(TextMaternoImputado)))) &&
                    (string.IsNullOrEmpty(TextNombreImputado) ? true : (w.INGRESO.IMPUTADO.NOMBRE.Contains(TextNombreImputado)) || (w.INGRESO.IMPUTADO.ALIAS.Any(a => a.NOMBRE.Contains(TextNombreImputado))))));
        }
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private bool SeguirCargandoPersonas { get; set; }
        private string _TituloRelacionInterno = "Buscar Persona";
        public string TituloRelacionInterno
        {
            get { return _TituloRelacionInterno; }
            set { _TituloRelacionInterno = value; OnPropertyChanged("TituloRelacionInterno"); }
        }
        #endregion

        #region Menu
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
        #endregion
    }
    public class InternoPorNotificar
    {
        public INGRESO INGRESO { get; set; }
        public bool ELEGIDO { get; set; }
    }
}