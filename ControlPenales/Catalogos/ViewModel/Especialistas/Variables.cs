using System.Linq;
namespace ControlPenales
{
    public partial class CatalogoEspecialistasViewModel
    {
        public string Name
        {
            get
            {
                return "catalogo_especialistas";
            }
        }

        #region [CONFIGURACION PERMISOS]

        private bool _guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return _guardarMenuEnabled; }
            set { _guardarMenuEnabled = value; RaisePropertyChanged("GuardarMenuEnabled"); }
        }

        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool editarEnabled;
        public bool EditarEnabled
        {
            get { return editarEnabled; }
            set { editarEnabled = value; RaisePropertyChanged("EditarEnabled"); }
        }
        #endregion

        #region MANEJO DE CONTROLES
        private bool agregarVisible = false;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; RaisePropertyChanged("AgregarVisible"); }
        }

        private bool bandera_agregar = true;
        public bool Bandera_Agregar
        {
            get { return bandera_agregar; }
            set { bandera_agregar = value; RaisePropertyChanged("Bandera_Agregar"); }
        }

        private bool emptyVisible = false;
        public bool EmptyVisible
        {
            get { return emptyVisible; }
            set { emptyVisible = value; RaisePropertyChanged("EmptyVisible"); }
        }

        #endregion


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> _listItems;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; OnPropertyChanged("ListItems"); }
        }

        private bool _IsEnabledEspecialidad = true;

        public bool IsEnabledEspecialidad
        {
            get { return _IsEnabledEspecialidad; }
            set { _IsEnabledEspecialidad = value; OnPropertyChanged("IsEnabledEspecialidad"); }
        }
        private SSP.Servidor.ESPECIALISTA _selectedItem;
        public SSP.Servidor.ESPECIALISTA SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem == null)
                {
                    EliminarMenuEnabled = false;
                    EditarMenuEnabled = false;
                }
                else
                {
                    if (editarEnabled)
                        EditarMenuEnabled = true;
                }

                OnPropertyChanged("SelectedItem");
            }
        }

        private bool _IsEnabledCamposBuasqueda = true;
        public bool IsEnabledCamposBuasqueda
        {
            get { return _IsEnabledCamposBuasqueda; }
            set { _IsEnabledCamposBuasqueda = value; OnPropertyChanged("IsEnabledCamposBuasqueda"); }
        }
        private bool _MenuBuscarEnabled = true;
        public bool MenuBuscarEnabled
        {
            get { return _MenuBuscarEnabled; }
            set { _MenuBuscarEnabled = value; OnPropertyChanged("MenuBuscarEnabled"); }
        }

        private ControlPenales.Clases.Estatus.Estatus _selectedEstatus = null;
        public ControlPenales.Clases.Estatus.Estatus SelectedEstatus
        {
            get { return _selectedEstatus; }
            set { _selectedEstatus = value; OnPropertyValidateChanged("SelectedEstatus"); }
        }

        private ControlPenales.Clases.Estatus.EstatusControl _lista_estatus = new ControlPenales.Clases.Estatus.EstatusControl();
        public ControlPenales.Clases.Estatus.EstatusControl Lista_Estatus
        {
            get { return _lista_estatus; }
            set { _lista_estatus = value; RaisePropertyChanged("Lista_Estatus"); }
        }

        private short? _SelectedEspecialidadBusqueda = -1;

        public short? SelectedEspecialidadBusqueda
        {
            get { return _SelectedEspecialidadBusqueda; }
            set { _SelectedEspecialidadBusqueda = value; OnPropertyChanged("SelectedEspecialidadBusqueda"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALISTA> lstEspecialistas;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALISTA> LstEspecialistas
        {
            get { return lstEspecialistas; }
            set { lstEspecialistas = value; OnPropertyChanged("LstEspecialistas"); }
        }

        private SSP.Servidor.ESPECIALISTA _selectedEspecialista;

        public SSP.Servidor.ESPECIALISTA SelectedEspecialista
        {
            get { return _selectedEspecialista; }
            set { _selectedEspecialista = value; OnPropertyChanged("SelectedEspecialista"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> _LstEspecialidadesCaptura;

        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> LstEspecialidadesCaptura
        {
            get { return _LstEspecialidadesCaptura; }
            set { _LstEspecialidadesCaptura = value; OnPropertyChanged("LstEspecialidadesCaptura"); }
        }

        private short? _SelectedEspecialidadEdicion = -1;

        public short? SelectedEspecialidadEdicion
        {
            get { return _SelectedEspecialidadEdicion; }
            set { _SelectedEspecialidadEdicion = value; OnPropertyChanged("SelectedEspecialidadEdicion"); }
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

        private bool _ReadCampoCodigo = false;
        public bool ReadCampoCodigo
        {
            get { return _ReadCampoCodigo; }
            set { _ReadCampoCodigo = value; OnPropertyChanged("ReadCampoCodigo"); }
        }

        private bool _EsOtroTipoCaptura = false;
        public bool EsOtroTipoCaptura
        {
            get { return _EsOtroTipoCaptura; }
            set { _EsOtroTipoCaptura = value; OnPropertyChanged("EsOtroTipoCaptura"); }
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

        private string selectSexo;
        public string SelectSexo
        {
            get { return selectSexo; }
            set { selectSexo = value; OnPropertyChanged("SelectSexo"); }
        }

        private System.DateTime? fechaNacimiento = null;
        public System.DateTime? FechaNacimiento
        {
            get { return fechaNacimiento; }
            set
            {
                fechaNacimiento = value;
                TextEdad = value != null ? new Fechas().CalculaEdad(value) : new short?();
                OnPropertyChanged("FechaNacimiento");
            }
        }

        private short? textEdad;
        public short? TextEdad
        {
            get { return textEdad; }
            set
            {
                textEdad = value;
                OnPropertyChanged("TextEdad");
            }
        }

        private string textCurp;
        public string TextCurp
        {
            get { return textCurp; }
            set
            {
                textCurp = value;
                OnPropertyChanged("TextCurp");
            }
        }
        private string textRfc;
        public string TextRfc
        {
            get { return textRfc; }
            set
            {
                textRfc = value;
                OnPropertyChanged("TextRfc");
            }
        }
        private int? textNip;
        public int? TextNip
        {
            get { return textNip; }
            set
            {
                textNip = value;
                OnPropertyChanged("TextNip");
            }
        }
        private string textTelefono;
        public string TextTelefono
        {
            get
            {
                if (textTelefono == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(textTelefono);
            }
            set
            {
                textTelefono = value;
                OnPropertyChanged("TextTelefono");
            }
        }
        private string textCorreo;
        public string TextCorreo
        {
            get { return textCorreo; }
            set
            {
                textCorreo = value;
                OnPropertyChanged("TextCorreo");
            }
        }

        private byte[] _FotoVisita = new Imagenes().getImagenPerson();
        public byte[] FotoVisita
        {
            get { return _FotoVisita; }
            set { _FotoVisita = value; OnPropertyChanged("FotoVisita"); }
        }
        private byte[] imagenPersona = new Imagenes().getImagenPerson();
        public byte[] ImagenPersona
        {
            get { return imagenPersona; }
            set { imagenPersona = value; OnPropertyChanged("ImagenPersona"); }
        }

        private SSP.Servidor.PERSONA SelectPersonaAuxiliar;
        private SSP.Servidor.PERSONA selectPersona;
        public SSP.Servidor.PERSONA SelectPersona
        {
            get { return selectPersona; }
            set
            {
                selectPersona = value;
                ImagenPersona = value == null ?
                    new Imagenes().getImagenPerson() :
                        value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any() ?
                            value.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectPersona");
            }
        }

        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        private bool SeguirCargandoPersonas { get; set; }

        private enum eEstatusAdministrativos
        {
            ADMINISTRADOR = 1
        };

        #region Enableds
        private bool situacionEnabled = false;
        public bool SituacionEnabled
        {
            get { return situacionEnabled; }
            set { situacionEnabled = value; OnPropertyChanged("SituacionEnabled"); }
        }
        private bool pertenenciasEnabled = true;
        public bool PertenenciasEnabled
        {
            get { return pertenenciasEnabled; }
            set { pertenenciasEnabled = value; OnPropertyChanged("PertenenciasEnabled"); }
        }
        private bool entradaEnabled = false;
        public bool EntradaEnabled
        {
            get { return entradaEnabled; }
            set { entradaEnabled = value; OnPropertyChanged("EntradaEnabled"); }
        }
        private bool salidaEnabled = false;
        public bool SalidaEnabled
        {
            get { return salidaEnabled; }
            set { salidaEnabled = value; OnPropertyChanged("SalidaEnabled"); }
        }
        private bool discapacitadoEnabled = false;
        public bool DiscapacitadoEnabled
        {
            get { return discapacitadoEnabled; }
            set { discapacitadoEnabled = value; OnPropertyChanged("DiscapacitadoEnabled"); }
        }
        private bool discapacidadEnabled = false;
        public bool DiscapacidadEnabled
        {
            get { return discapacidadEnabled; }
            set { discapacidadEnabled = value; OnPropertyChanged("DiscapacidadEnabled"); }
        }
        private bool entidadEnabled = false;
        public bool EntidadEnabled
        {
            get { return entidadEnabled; }
            set { entidadEnabled = value; OnPropertyChanged("EntidadEnabled"); }
        }
        private bool municipioEnabled = false;
        public bool MunicipioEnabled
        {
            get { return municipioEnabled; }
            set { municipioEnabled = value; OnPropertyChanged("MunicipioEnabled"); }
        }
        private bool coloniaEnabled = false;
        public bool ColoniaEnabled
        {
            get { return coloniaEnabled; }
            set { coloniaEnabled = value; OnPropertyChanged("ColoniaEnabled"); }
        }
        private bool validarEnabled = false;
        public bool ValidarEnabled
        {
            get { return validarEnabled; }
            set
            {
                validarEnabled = value;
                OnPropertyChanged("ValidarEnabled");
            }
        }
        private bool nombreReadOnly = true;
        public bool NombreReadOnly
        {
            get { return nombreReadOnly; }
            set { nombreReadOnly = value; OnPropertyChanged("NombreReadOnly"); }
        }
        private bool codigoEnabled = false;
        public bool CodigoEnabled
        {
            get { return codigoEnabled; }
            set { codigoEnabled = value; OnPropertyChanged("CodigoEnabled"); }
        }
        private bool generalEnabled = false;
        public bool GeneralEnabled
        {
            get { return generalEnabled; }
            set { generalEnabled = value; OnPropertyChanged("GeneralEnabled"); }
        }
        private bool BanderaHuella;
        private BusquedaHuella _WindowBusqueda;
        public BusquedaHuella WindowBusqueda
        {
            get { return _WindowBusqueda; }
            set { _WindowBusqueda = value; }
        }
        #endregion


        private string _HeaderAgregar = "Agregar Especialista";
        public string HeaderAgregar
        {
            get { return _HeaderAgregar; }
            set { _HeaderAgregar = value; OnPropertyChanged("HeaderAgregar"); }
        }

        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> _ListPersonas;
        public RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonas
        {
            get { return _ListPersonas; }
            set { _ListPersonas = value; OnPropertyChanged("ListPersonas"); }
        }

        private int? SelectTipoPersona = int.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA);

        private SeleccionarTipoVisitaAduanaView _SeleccionarTipoVisitaAduana;
        public SeleccionarTipoVisitaAduanaView SeleccionarTipoVisitaAduana
        {
            get { return _SeleccionarTipoVisitaAduana; }
            set { _SeleccionarTipoVisitaAduana = value; OnPropertyChanged("SeleccionarTipoVisitaAduana"); }
        }

        private BitacoraAduana _SelectBitacoraAcceso;
        public BitacoraAduana SelectBitacoraAcceso
        {
            get { return _SelectBitacoraAcceso; }
            set { _SelectBitacoraAcceso = value; OnPropertyChanged("SelectBitacoraAcceso"); }
        }

        private short selectSituacion;
        public short SelectSituacion
        {
            get { return selectSituacion; }
            set
            {
                selectSituacion = value;
                OnPropertyChanged("SelectSituacion");
            }
        }

        private RangeEnabledObservableCollection<SSP.Servidor.PERSONA> ListPersonasAuxiliar;
        private bool _EmptyBuscarRelacionInternoVisible;
        public bool EmptyBuscarRelacionInternoVisible
        {
            get { return _EmptyBuscarRelacionInternoVisible; }
            set { _EmptyBuscarRelacionInternoVisible = value; OnPropertyChanged("EmptyBuscarRelacionInternoVisible"); }
        }
    }
}