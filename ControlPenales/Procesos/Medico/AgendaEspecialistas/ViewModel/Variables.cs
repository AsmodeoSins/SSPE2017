using System.Linq;
namespace ControlPenales
{
    partial class AgendaEspecialistasViewModel
    {
        #region Variables Privadas
        private System.DateTime? fechaOriginal = null;
        private SSP.Servidor.ATENCION_CITA selectedAtencion_Cita;
        private System.Collections.Generic.List<short> roles = null;
        private ModoAgregarAgenda tipoAgregarAgenda;
        private ModoBusqueda tipoBusquedaImputado;
        private IQueryable<SSP.Servidor.PROCESO_USUARIO> permisos;
        private bool permisos_crear = false;
        private short?[] estatus_inactivos = null;
        #endregion
        #region Variables para habilitas controles
        private bool isAtencionTiposEnabled = true;
        public bool IsAtencionTiposEnabled
        {
            get { return isAtencionTiposEnabled; }
            set { isAtencionTiposEnabled = value; OnPropertyChanged("IsAtencionTiposEnabled"); }
        }

        private bool isEnabledBuscarImputadoAgregarAgenda = false;
        public bool IsEnabledBuscarImputadoAgregarAgenda
        {
            get { return isEnabledBuscarImputadoAgregarAgenda; }
            set { isEnabledBuscarImputadoAgregarAgenda = value; RaisePropertyChanged("IsEnabledBuscarImputadoAgregarAgenda"); }
        }

        private System.Windows.Visibility _VisibleCitasPendientes = System.Windows.Visibility.Visible;

        public System.Windows.Visibility VisibleCitasPendientes
        {
            get { return _VisibleCitasPendientes; }
            set { _VisibleCitasPendientes = value; OnPropertyChanged("VisibleCitasPendientes"); }
        }

        private bool _CamposCitasEspecialistas = true;

        public bool CamposCitasEspecialistas
        {
            get { return _CamposCitasEspecialistas; }
            set { _CamposCitasEspecialistas = value; OnPropertyChanged("CamposCitasEspecialistas"); }
        }
        private bool isAreasEnabled = true;
        public bool IsAreasEnabled
        {
            get { return isAreasEnabled; }
            set { isAreasEnabled = value; RaisePropertyChanged("IsAreasEnabled"); }
        }

        private bool isAgendaEnabled = true;
        public bool IsAgendaEnabled
        {
            get { return isAgendaEnabled; }
            set { isAgendaEnabled = value; RaisePropertyChanged("IsAgendaEnabled"); }
        }

        private bool menuBuscarEnabled;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set { menuBuscarEnabled = value; RaisePropertyChanged("MenuBuscarEnabled"); }
        }

        private bool menuLimpiarEnabled;
        public bool MenuLimpiarEnabled
        {
            get { return menuLimpiarEnabled; }
            set { menuLimpiarEnabled = value; RaisePropertyChanged("MenuLimpiarEnabled"); }
        }

        private bool isAgendaEmpleadoEnabled = false;
        public bool IsAgendaEmpleadoEnabled
        {
            get { return isAgendaEmpleadoEnabled; }
            set { isAgendaEmpleadoEnabled = value; RaisePropertyChanged("IsAgendaEmpleadoEnabled"); }
        }
        #endregion
        #region Variables para visualizacion de controles
        private System.Windows.Visibility isDatosPacienteVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsDatosPacienteVisible
        {
            get { return isDatosPacienteVisible; }
            set { isDatosPacienteVisible = value; RaisePropertyChanged("IsDatosPacienteVisible"); }
        }
        #endregion
        #region Busqueda Agenda
        #region Calendario
        private System.DateTime fechaInicial;
        public System.DateTime FechaInicial
        {
            get { return fechaInicial; }
            set { fechaInicial = value; RaisePropertyChanged("FechaInicial"); }
        }
        private System.DateTime? busquedaFecha;
        public System.DateTime? BusquedaFecha
        {
            get { return busquedaFecha; }
            set { busquedaFecha = value; RaisePropertyChanged("BusquedaFecha"); }
        }
        private System.Collections.Generic.List<System.DateTime> fechasAgendadas;
        public System.Collections.Generic.List<System.DateTime> FechasAgendadas
        {
            get { return fechasAgendadas; }
            set { fechasAgendadas = value; RaisePropertyChanged("FechasAgendadas"); }
        }

        private bool fechaAgendaValid = true;
        public bool FechaAgendaValid
        {
            get { return fechaAgendaValid; }
            set { fechaAgendaValid = value; OnPropertyChanged("FechaAgendaValid"); }
        }
        #endregion
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> lstAtencionTipos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> LstAtencionTipos
        {
            get { return lstAtencionTipos; }
            set { lstAtencionTipos = value; RaisePropertyChanged("LstAtencionTipos"); }
        }

        private short? selectedAtencionTipo = 1;
        public short? SelectedAtencionTipo
        {
            get { return selectedAtencionTipo; }
            set { selectedAtencionTipo = value; RaisePropertyChanged("SelectedAtencionTipo"); }
        }

        private System.Collections.Generic.List<cUsuarioExtendida> lstEmpleados = new System.Collections.Generic.List<cUsuarioExtendida>();
        public System.Collections.Generic.List<cUsuarioExtendida> LstEmpleados
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

        private string tituloAgenda = string.Empty;
        public string TituloAgenda
        {
            get { return tituloAgenda; }
            set { tituloAgenda = value; RaisePropertyChanged("TituloAgenda"); }
        }

        #region variables privadas
        private cUsuarioExtendida _empleado = null;
        #endregion
        #endregion
        #region Agenda
        private string headerAgenda = string.Empty;
        public string HeaderAgenda
        {
            get { return headerAgenda; }
            set { headerAgenda = value; RaisePropertyChanged("HeaderAgenda"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<Appointment> lstAgenda = new System.Collections.ObjectModel.ObservableCollection<Appointment>();
        public System.Collections.ObjectModel.ObservableCollection<Appointment> LstAgenda
        {
            get { return lstAgenda; }
            set { lstAgenda = value; RaisePropertyChanged("LstAgenda"); }
        }

        private System.DateTime fechaAgenda = System.DateTime.Now;
        public System.DateTime FechaAgenda
        {
            get { return fechaAgenda; }
            set { fechaAgenda = value; RaisePropertyChanged("FechaAgenda"); }
        }

        #region Variables Privadas
        public short? selectedAtencionTipoAgenda;
        #endregion
        #endregion
        #region Agregar Cita

        private short? anioImputadoAgregarAgenda;
        public short? AnioImputadoAgregarAgenda
        {
            get { return anioImputadoAgregarAgenda; }
            set { anioImputadoAgregarAgenda = value; RaisePropertyChanged("AnioImputadoAgregarAgenda"); }
        }

        private int? folioImputadoAgregarAgenda;
        public int? FolioImputadoAgregarAgenda
        {
            get { return folioImputadoAgregarAgenda; }
            set { folioImputadoAgregarAgenda = value; RaisePropertyChanged("FolioImputadoAgregarAgenda"); }
        }

        private string apPaternoAgregarAgenda;
        public string ApPaternoAgregarAgenda
        {
            get { return apPaternoAgregarAgenda; }
            set { apPaternoAgregarAgenda = value; RaisePropertyChanged("ApPaternoAgregarAgenda"); }
        }

        private string apMaternoAgregarAgenda;
        public string ApMaternoAgregarAgenda
        {
            get { return apMaternoAgregarAgenda; }
            set { apMaternoAgregarAgenda = value; RaisePropertyChanged("ApMaternoAgregarAgenda"); }
        }
        private string nombreAgregarAgenda;
        public string NombreAgregarAgenda
        {
            get { return nombreAgregarAgenda; }
            set { nombreAgregarAgenda = value; RaisePropertyChanged("NombreAgregarAgenda"); }
        }
        private string sexoImputadoAgregarAgenda;
        public string SexoImputadoAgregarAgenda
        {
            get { return sexoImputadoAgregarAgenda; }
            set { sexoImputadoAgregarAgenda = value; RaisePropertyChanged("SexoImputadoAgregarAgenda"); }
        }
        private short? edadImputadoAgregarAgenda;
        public short? EdadImputadoAgregarAgenda
        {
            get { return edadImputadoAgregarAgenda; }
            set { edadImputadoAgregarAgenda = value; RaisePropertyChanged("EdadImputadoAgregarAgenda"); }
        }
        private byte[] imagenAgregarAgenda;
        public byte[] ImagenAgregarAgenda
        {
            get { return imagenAgregarAgenda; }
            set { imagenAgregarAgenda = value; RaisePropertyChanged("ImagenAgregarAgenda"); }
        }

        private bool agregarAgendaFechaValid = false;
        public bool AgregarAgendaFechaValid
        {
            get { return agregarAgendaFechaValid; }
            set { agregarAgendaFechaValid = value; RaisePropertyChanged("AgregarAgendaFechaValid"); }
        }

        private System.DateTime? agregarAgendaFecha;
        public System.DateTime? AgregarAgendaFecha
        {
            get { return agregarAgendaFecha; }
            set { agregarAgendaFecha = value; RaisePropertyChanged("AgregarAgendaFecha"); }
        }

        private System.DateTime? agregarAgendaHoraI;
        public System.DateTime? AgregarAgendaHoraI
        {
            get { return agregarAgendaHoraI; }
            set
            {
                agregarAgendaHoraI = value;
                if (value.HasValue)
                {
                    if (AgregarAgendaHoraF.HasValue)
                    {
                        if (value <= AgregarAgendaHoraF.Value)
                            AgregarAgendaHorasValid = true;
                        else
                            AgregarAgendaHorasValid = false;
                    }
                }
                OnPropertyChanged("AgregarAgendaHoraI");
            }
        }

        private System.DateTime? agregarAgendaHoraF;
        public System.DateTime? AgregarAgendaHoraF
        {
            get { return agregarAgendaHoraF; }
            set
            {
                agregarAgendaHoraF = value;
                if (!value.HasValue)
                    AgregarAgendaHorasValid = false;
                else
                    if (!agregarAgendaHoraI.HasValue)
                        AgregarAgendaHorasValid = false;
                    else
                        if (value <= agregarAgendaHoraI)
                            AgregarAgendaHorasValid = false;
                        else
                            AgregarAgendaHorasValid = true;

                OnPropertyChanged("AgregarAgendaHoraF");
            }
        }

        private bool agregarAgendaHorasValid;
        public bool AgregarAgendaHorasValid
        {
            get { return agregarAgendaHorasValid; }
            set { agregarAgendaHorasValid = value; RaisePropertyChanged("AgregarAgendaHorasValid"); }
        }

        private bool isReadOnlyAnioImputadoAgregarAgenda = true;
        public bool IsReadOnlyAnioImputadoAgregarAgenda
        {
            get { return isReadOnlyAnioImputadoAgregarAgenda; }
            set { isReadOnlyAnioImputadoAgregarAgenda = value; RaisePropertyChanged("IsReadOnlyAnioImputadoAgregarAgenda"); }
        }
        private bool isReadOnlyFolioImputadoAgregarAgenda = true;
        public bool IsReadOnlyFolioImputadoAgregarAgenda
        {
            get { return isReadOnlyFolioImputadoAgregarAgenda; }
            set { isReadOnlyFolioImputadoAgregarAgenda = value; RaisePropertyChanged("IsReadOnlyFolioImputadoAgregarAgenda"); }
        }

        private bool isReadOnlyApPaternoAgregarAgenda = true;
        public bool IsReadOnlyApPaternoAgregarAgenda
        {
            get { return isReadOnlyApPaternoAgregarAgenda; }
            set { isReadOnlyApPaternoAgregarAgenda = value; RaisePropertyChanged("IsReadOnlyApPaternoAgregarAgenda"); }
        }

        private bool isReadOnlyApMaternoAgregarAgenda = true;
        public bool IsReadOnlyApMaternoAgregarAgenda
        {
            get { return isReadOnlyApMaternoAgregarAgenda; }
            set { isReadOnlyApMaternoAgregarAgenda = value; RaisePropertyChanged("IsReadOnlyApMaternoAgregarAgenda"); }
        }


        private bool isReadOnlyNombreAgregarAgenda = true;
        public bool IsReadOnlyNombreAgregarAgenda
        {
            get { return isReadOnlyNombreAgregarAgenda; }
            set { isReadOnlyNombreAgregarAgenda = value; RaisePropertyChanged("IsReadOnlyNombreAgregarAgenda"); }
        }


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> lstAreas;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.AREA> LstAreas
        {
            get { return lstAreas; }
            set { lstAreas = value; RaisePropertyChanged("LstAreas"); }
        }

        private short? selectedArea = -1;
        public short? SelectedArea
        {
            get { return selectedArea; }
            set { selectedArea = value; RaisePropertyChanged("SelectedArea"); }
        }

        private string tipoServicioDescripcion = string.Empty;
        public string TipoServicioDescripcion
        {
            get { return tipoServicioDescripcion; }
            set { tipoServicioDescripcion = value; RaisePropertyChanged("TipoServicioDescripcion"); }
        }

        private System.Collections.Generic.List<cUsuarioExtendida> lstAgendaEmpleados = new System.Collections.Generic.List<cUsuarioExtendida>();
        public System.Collections.Generic.List<cUsuarioExtendida> LstAgendaEmpleados
        {
            get { return lstAgendaEmpleados; }
            set { lstAgendaEmpleados = value; RaisePropertyChanged("LstAgendaEmpleados"); }
        }

        private int selectedAgendaEmpleadoValue = -1;
        public int SelectedAgendaEmpleadoValue
        {
            get { return selectedAgendaEmpleadoValue; }
            set { selectedAgendaEmpleadoValue = value; RaisePropertyChanged("SelectedAgendaEmpleadoValue"); }
        }

        #endregion


        System.DateTime _FechaServer = Fechas.GetFechaDateServer;
        private enum ModoAgregarAgenda
        {
            INSERTAR,
            EDICION
        }

        private enum ModoBusqueda
        {
            BUSQUEDA_AGENDA_IMPUTADO,
            BUSQUEDA_NORMAL_IMPUTADO
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

        private bool selectIngresoEnabled;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private bool crearNuevoExpedienteEnabled = true;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private SSP.Servidor.INGRESO selectIngreso;
        public SSP.Servidor.INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (selectIngreso == null)
                {
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                    
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)ControlPenales.BiometricoServiceReference.enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)ControlPenales.BiometricoServiceReference.enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                if (selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_TRASLADO && selectIngreso.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                {
                    TextBotonSeleccionarIngreso = "aceptar";
                    SelectIngresoEnabled = true;
                }
                else
                {
                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                    SelectIngresoEnabled = false;
                }


                OnPropertyChanged("SelectIngreso");
            }
        }

        private byte[] imagenIngreso; //compartido entre datos y busqueda
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; RaisePropertyChanged("ImagenIngreso"); }
        }

        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private SSP.Servidor.INGRESO selectIngresoAuxiliar = null;


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> lstBusquedaAgendaAtencionTipos;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> LstBusquedaAgendaAtencionTipos
        {
            get { return lstBusquedaAgendaAtencionTipos; }
            set { lstBusquedaAgendaAtencionTipos = value; RaisePropertyChanged("LstBusquedaAgendaAtencionTipos"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> lstEspecialidades;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ESPECIALIDAD> LstEspecialidades
        {
            get { return lstEspecialidades; }
            set { lstEspecialidades = value; OnPropertyChanged("LstEspecialidades"); }
        }


        private short? _SelectedEspecialidad;
        public short? SelectedEspecialidad
        {
            get { return _SelectedEspecialidad; }
            set { _SelectedEspecialidad = value; OnPropertyChanged("SelectedEspecialidad"); }
        }
        private short? selectedBusquedaAgendaAtencionTipo = -1;
        public short? SelectedBusquedaAgendaAtencionTipo
        {
            get { return selectedBusquedaAgendaAtencionTipo; }
            set { selectedBusquedaAgendaAtencionTipo = value; RaisePropertyChanged("SelectedBusquedaAgendaAtencionTipo"); }
        }

        private SSP.Servidor.ATENCION_CITA selectedBuscarAgenda;
        public SSP.Servidor.ATENCION_CITA SelectedBuscarAgenda
        {
            get { return selectedBuscarAgenda; }
            set { selectedBuscarAgenda = value; RaisePropertyChanged("SelectedBuscarAgenda"); }
        }
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";

        #region Busqueda Agenda por Imputado
        private int? buscarAnioImputadoAgenda;
        public int? BuscarAnioImputadoAgenda
        {
            get { return buscarAnioImputadoAgenda; }
            set { buscarAnioImputadoAgenda = value; RaisePropertyChanged("BuscarAnioImputadoAgenda"); }
        }

        private int? buscarFolioImputadoAgenda;
        public int? BuscarFolioImputadoAgenda
        {
            get { return buscarFolioImputadoAgenda; }
            set { buscarFolioImputadoAgenda = value; RaisePropertyChanged("BuscarFolioImputadoAgenda"); }
        }

        private string buscarNombreImputadoAgenda;
        public string BuscarNombreImputadoAgenda
        {
            get { return buscarNombreImputadoAgenda; }
            set { buscarNombreImputadoAgenda = value; RaisePropertyChanged("BuscarNombreImputadoAgenda"); }
        }

        private string buscarApPaternoImputadoAgenda;
        public string BuscarApPaternoImputadoAgenda
        {
            get { return buscarApPaternoImputadoAgenda; }
            set { buscarApPaternoImputadoAgenda = value; RaisePropertyChanged("BuscarApPaternoImputadoAgenda"); }
        }

        private string buscarApMaternoImputadoAgenda;
        public string BuscarApMaternoImputadoAgenda
        {
            get { return buscarApMaternoImputadoAgenda; }
            set { buscarApMaternoImputadoAgenda = value; RaisePropertyChanged("BuscarApMaternoImputadoAgenda"); }
        }

        private byte[] buscarImagenImputadoAgenda;
        public byte[] BuscarImagenImputadoAgenda
        {
            get { return buscarImagenImputadoAgenda; }
            set { buscarImagenImputadoAgenda = value; RaisePropertyChanged("BuscarImagenImputadoAgenda"); }
        }

        private string headerBuscarAgenda;
        public string HeaderBuscarAgenda
        {
            get { return headerBuscarAgenda; }
            set { headerBuscarAgenda = value; RaisePropertyChanged("HeaderBuscarAgenda"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA> busquedaAgenda;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA> BusquedaAgenda
        {
            get { return busquedaAgenda; }
            set { busquedaAgenda = value; RaisePropertyChanged("BusquedaAgenda"); }
        }

        #endregion

        public class NombresEspecialistas
        {
            public string NombreEspecialista { get; set; }
            public int? IdPersona { get; set; }
            public short IdEspecialista { get; set; }
        };

        private short? _SelectedEspecialista;
        public  short? SelectedEspecialista
        {
            get { return _SelectedEspecialista; }
            set 
            {
                _SelectedEspecialista = value;
                if (value.HasValue)
                    if(value != -1)
                        CargarAgenda();

                OnPropertyChanged("SelectedEspecialista");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas> lstNombresEspecialistas;
        public System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas> LstNombresEspecialistas
        {
            get { return lstNombresEspecialistas; }
            set { lstNombresEspecialistas = value; OnPropertyChanged("LstNombresEspecialistas"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas> lstEspecialistasMostrar;
        public System.Collections.ObjectModel.ObservableCollection<NombresEspecialistas> LstEspecialistasMostrar
        {
            get { return lstEspecialistasMostrar; }
            set { lstEspecialistasMostrar = value; OnPropertyChanged("LstEspecialistasMostrar"); }
        }

        private int _IdEspecialistaMostrado;

        public int IdEspecialistaMostrado
        {
            get { return _IdEspecialistaMostrado; }
            set { _IdEspecialistaMostrado = value; OnPropertyChanged("IdEspecialistaMostrado"); }
        }


   

        #region Buscar Imputado
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

        private RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private SSP.Servidor.IMPUTADO selectExpediente;
        public SSP.Servidor.IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (selectExpediente != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (selectExpediente.INGRESO!=null && selectExpediente.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                    {
                        SelectIngreso = null;
                        EmptyIngresoVisible = true;
                    }


                    //OBTENEMOS FOTO DE FRENTE
                    if (SelectIngreso != null)
                    {
                        //if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        //    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        //else
                        //    ImagenImputado = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenImputado = new Imagenes().getImagenPerson();
                    TextBotonSeleccionarIngreso = "aceptar";
                    SelectIngresoEnabled = true;
                    if (estatus_inactivos != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                    {
                        TextBotonSeleccionarIngreso = "seleccionar ingreso";
                        SelectIngresoEnabled = false;
                    }
                }
                else
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    EmptyIngresoVisible = true;
                }
                OnPropertyChanged("SelectExpediente");
            }
        }

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }
        
        #region Variables privadas
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        #endregion
        #endregion


        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_NIVEL_PRIORIDAD> _LstPrioridadesBuscar;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_NIVEL_PRIORIDAD> LstPrioridadesBuscar
        {
            get { return _LstPrioridadesBuscar; }
            set { _LstPrioridadesBuscar = value; OnPropertyChanged("LstPrioridadesBuscar"); }
        }

        private short? _IdBuscarPrioridades = -1;
        public short? IdBuscarPrioridades
        {
            get { return _IdBuscarPrioridades; }
            set { _IdBuscarPrioridades = value; OnPropertyChanged("IdBuscarPrioridades"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO> lstIncidenteMotivo;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_CITA_IN_MOTIVO> LstIncidenteMotivo
        {
            get { return lstIncidenteMotivo; }
            set { lstIncidenteMotivo = value; RaisePropertyChanged("LstIncidenteMotivo"); }
        }

        private short selectedIncidenteMotivoValue = -1;
        public short SelectedIncidenteMotivoValue
        {
            get { return selectedIncidenteMotivoValue; }
            set { selectedIncidenteMotivoValue = value; RaisePropertyChanged("SelectedIncidenteMotivoValue"); }
        }

        private string observacion = string.Empty;
        public string Observacion
        {
            get { return observacion; }
            set { observacion = value; RaisePropertyChanged("Observacion"); }
        }


        #region Busqueda de Solicitudes de Interconsulta
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_TIPO> lstInterconsulta_TiposBuscar = null;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_TIPO> LstInterconsulta_TiposBuscar
        {
            get { return lstInterconsulta_TiposBuscar; }
            set { lstInterconsulta_TiposBuscar = value; RaisePropertyChanged("LstInterconsulta_TiposBuscar"); }
        }

        private short selectedInter_TipoBuscarValue = -1;
        public short SelectedInter_TipoBuscarValue
        {
            get { return selectedInter_TipoBuscarValue; }
            set { selectedInter_TipoBuscarValue = value; RaisePropertyChanged("SelectedInter_TipoBuscarValue"); }
        }

        private short? anioBuscarInter = null;
        public short? AnioBuscarInter
        {
            get { return anioBuscarInter; }
            set { anioBuscarInter = value; RaisePropertyChanged("AnioBuscarInter"); }
        }

        private short? folioBuscarInter = null;
        public short? FolioBuscarInter
        {
            get { return folioBuscarInter; }
            set { folioBuscarInter = value; RaisePropertyChanged("FolioBuscarInter"); }
        }

        private string nombreBuscarInter = string.Empty;
        public string NombreBuscarInter
        {
            get { return nombreBuscarInter; }
            set { nombreBuscarInter = value; RaisePropertyChanged("NombreBuscarInter"); }
        }

        private string apellidoPaternoBuscarInter = string.Empty;
        public string ApellidoPaternoBuscarInter
        {
            get { return apellidoPaternoBuscarInter; }
            set { apellidoPaternoBuscarInter = value; RaisePropertyChanged("ApellidoPaternoBuscarInter"); }
        }

        private string apellidoMaternoBuscarInter = string.Empty;
        public string ApellidoMaternoBuscarInter
        {
            get { return apellidoMaternoBuscarInter; }
            set { apellidoMaternoBuscarInter = value; RaisePropertyChanged("ApellidoMaternoBuscarInter"); }
        }

        private System.DateTime? fechaInicialBuscarInter = null;
        public System.DateTime? FechaInicialBuscarInter
        {
            get { return fechaInicialBuscarInter; }
            set { fechaInicialBuscarInter = value; RaisePropertyChanged("FechaInicialBuscarInter"); }
        }

        private System.DateTime? fechaFinalBuscarInter = null;
        public System.DateTime? FechaFinalBuscarInter
        {
            get { return fechaFinalBuscarInter; }
            set { fechaFinalBuscarInter = value; RaisePropertyChanged("FechaFinalBuscarInter"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA> listaInterconsultasBusqueda;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SOL_INTERCONSULTA_INTERNA> ListaInterconsultasBusqueda
        {
            get { return listaInterconsultasBusqueda; }
            set { listaInterconsultasBusqueda = value; RaisePropertyChanged("ListaInterconsultasBusqueda"); }
        }

        private SSP.Servidor.SOL_INTERCONSULTA_INTERNA selectedInterconsultaBusqueda = null;
        public SSP.Servidor.SOL_INTERCONSULTA_INTERNA SelectedInterconsultaBusqueda
        {
            get { return selectedInterconsultaBusqueda; }
            set { selectedInterconsultaBusqueda = value; RaisePropertyChanged("SelectedInterconsultaBusqueda"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> lstAtencion_TipoBuscar = null;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO> LstAtencion_TipoBuscar
        {
            get { return lstAtencion_TipoBuscar; }
            set { lstAtencion_TipoBuscar = value; RaisePropertyChanged("LstAtencion_TipoBuscar"); }
        }

        private short selectedAtencion_TipoBuscarValue = -1;
        public short SelectedAtencion_TipoBuscarValue
        {
            get { return selectedAtencion_TipoBuscarValue; }
            set { selectedAtencion_TipoBuscarValue = value; RaisePropertyChanged("SelectedAtencion_TipoBuscarValue"); }
        }

        #region Validaciones Busqueda Solicitudes
        private bool isFechaIniBusquedaSolValida = true;
        public bool IsFechaIniBusquedaSolValida
        {
            get { return isFechaIniBusquedaSolValida; }
            set { isFechaIniBusquedaSolValida = value; RaisePropertyChanged("IsFechaIniBusquedaSolValida"); }
        }
        #endregion
        #endregion

        private enumModo modoVistaModelo = enumModo.INSERCION;

        private enum enumCita_Tipo
        {
            PRIMERA = 1,
            SUBSECUENTE = 2
        }

        private enum enumModo
        {
            INSERCION = 1,
            REAGENDA = 2
        }

        private SSP.Servidor.SOL_INTERCONSULTA_INTERNA selectedInterconsulta_solicitud;

        private System.DateTime? fechaMinima = Fechas.GetFechaDateServer;
        public System.DateTime? FechaMinima
        {
            get { return fechaMinima; }
            set { fechaMinima = value; RaisePropertyChanged("FechaMinima"); }
        }

        private bool isInterconsultaEnabled = false;
        public bool IsInterconsultaEnabled
        {
            get { return isInterconsultaEnabled; }
            set { isInterconsultaEnabled = value; RaisePropertyChanged("IsInterconsultaEnabled"); }
        }

        private bool isModoInsercion = true;
        public bool IsModoInsercion
        {
            get { return isModoInsercion; }
            set { isModoInsercion = value; RaisePropertyChanged("IsModoInsercion"); }
        }

        private bool _permisos_editar = false;
        private bool _permisos_agregar = false;


        #region Variables para habilitar controles
        private bool menuGuardarEnabled = false;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; RaisePropertyChanged("MenuGuardarEnabled"); }
        }
        private bool menuAgregarEnabled = false;
        public bool MenuAgregarEnabled
        {
            get { return menuAgregarEnabled; }
            set { menuAgregarEnabled = value; RaisePropertyChanged("MenuAgregarEnabled"); }
        }

        private bool menuEditarEnabled = false;
        public bool MenuEditarEnabled
        {
            get { return menuEditarEnabled; }
            set { menuEditarEnabled = value; RaisePropertyChanged("MenuEditarEnabled"); }
        }

        private bool eliminarMenuEnabled = false;
        public bool EliminarMenuEnabled
        {
            get { return eliminarMenuEnabled; }
            set { eliminarMenuEnabled = value; RaisePropertyChanged("EliminarMenuEnabled"); }
        }
        #endregion


        private short selectedInterconsultaTipo = -1;
        public short SelectedInterconsultaTipo
        {
            get { return selectedInterconsultaTipo; }
            set { selectedInterconsultaTipo = value; OnPropertyValidateChanged("SelectedInterconsultaTipo"); }
        }

        private short selectedNvlPrioridad = -1;
        public short SelectedNvlPrioridad
        {
            get { return selectedNvlPrioridad; }
            set { selectedNvlPrioridad = value; OnPropertyValidateChanged("SelectedNvlPrioridad"); }
        }

        private short selectedInterconsultaAtencionTipo;
        public short SelectedInterconsultaAtencionTipo
        {
            get { return selectedInterconsultaAtencionTipo; }
            set { selectedInterconsultaAtencionTipo = value; OnPropertyValidateChanged("SelectedInterconsultaAtencionTipo"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> lstServAuxSeleccionados;
        public System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO> LstServAuxSeleccionados
        {
            get { return lstServAuxSeleccionados; }
            set { lstServAuxSeleccionados = value; OnPropertyValidateChanged("LstServAuxSeleccionados"); }
        }

        private bool isServAuxSeleccionadosValid = false;
        public bool IsServAuxSeleccionadosValid
        {
            get { return isServAuxSeleccionadosValid; }
            set { isServAuxSeleccionadosValid = value; RaisePropertyChanged("IsServAuxSeleccionadosValid"); }
        }

        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_ATENCION_TIPO> lstInterconsultaAtencionTipo;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.INTERCONSULTA_ATENCION_TIPO> LstInterconsultaAtencionTipo
        {
            get { return lstInterconsultaAtencionTipo; }
            set { lstInterconsultaAtencionTipo = value; RaisePropertyChanged("LstInterconsultaTipo"); }
        }


        #region Visibilidad de Controles
        private System.Windows.Visibility isEspecialidadVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsEspecialidadVisible
        {
            get { return isEspecialidadVisible; }
            set { isEspecialidadVisible = value; RaisePropertyChanged("IsEspecialidadVisible"); }
        }

        private System.Windows.Visibility isServicioAuxiliarVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsServicioAuxiliarVisible
        {
            get { return isServicioAuxiliarVisible; }
            set { isServicioAuxiliarVisible = value; RaisePropertyChanged("IsServicioAuxiliarVisible"); }
        }

        private System.Windows.Visibility isReferenciaVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsReferenciaVisible
        {
            get { return isReferenciaVisible; }
            set { isReferenciaVisible = value; RaisePropertyChanged("IsReferenciaVisible"); }
        }

        private System.Windows.Visibility isSolicitudInternaVisible = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility IsSolicitudInternaVisible
        {
            get { return isSolicitudInternaVisible; }
            set { isSolicitudInternaVisible = value; RaisePropertyChanged("IsSolicitudInternaVisible"); }
        }

        private System.Windows.Visibility isCertificadoMedico = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsCertificadoMedico
        {
            get { return isCertificadoMedico; }
            set { isCertificadoMedico = value; RaisePropertyChanged("IsCertificadoMedico"); }
        }

        private System.Windows.Visibility isOtroHospitalSelected = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsOtroHospitalSelected
        {
            get { return isOtroHospitalSelected; }
            set { isOtroHospitalSelected = value; RaisePropertyChanged("IsOtroHospitalSelected"); }
        }

        public System.Windows.Visibility exploracionFisicaVisible = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ExploracionFisicaVisible
        {
            get { return exploracionFisicaVisible; }
            set { exploracionFisicaVisible = value; RaisePropertyChanged("ExploracionFisicaVisible"); }
        }

        #endregion

        private short selectedTipoServAux = -1;
        public short SelectedTipoServAux
        {
            get { return selectedTipoServAux; }
            set { selectedTipoServAux = value; OnPropertyValidateChanged("SelectedTipoServAux"); }
        }

        #region Datos Imputado
        private string textAnioImputado = string.Empty;
        public string TextAnioImputado
        {
            get { return textAnioImputado; }
            set { textAnioImputado = value; RaisePropertyChanged("TextAnioImputado"); }
        }

        private string textFolioImputado = string.Empty;
        public string TextFolioImputado
        {
            get { return textFolioImputado; }
            set { textFolioImputado = value; RaisePropertyChanged("TextFolioImputado"); }
        }

        private string textPaternoImputado = string.Empty;
        public string TextPaternoImputado
        {
            get { return textPaternoImputado; }
            set { textPaternoImputado = value; RaisePropertyChanged("TextPaternoImputado"); }
        }

        private string textMaternoImputado = string.Empty;
        public string TextMaternoImputado
        {
            get { return textMaternoImputado; }
            set { textMaternoImputado = value; RaisePropertyChanged("TextMaternoImputado"); }
        }

        private string textNombreImputado = string.Empty;
        public string TextNombreImputado
        {
            get { return textNombreImputado; }
            set { textNombreImputado = value; RaisePropertyChanged("TextNombreImputado"); }
        }

        private string textSexoImputado = string.Empty;
        public string TextSexoImputado
        {
            get { return textSexoImputado; }
            set { textSexoImputado = value; RaisePropertyChanged("TextSexoImputado"); }
        }

        private string textEdadImputado = string.Empty;
        public string TextEdadImputado
        {
            get { return textEdadImputado; }
            set { textEdadImputado = value; RaisePropertyChanged("TextEdadImputado"); }
        }

        private string textFechaNacImputado;
        public string TextFechaNacImputado
        {
            get { return textFechaNacImputado; }
            set { textFechaNacImputado = value; RaisePropertyChanged("TextFechaNacImputado"); }
        }

        #endregion


        private enum eTiposInterconsulta
        {
            INTERNA = 1
        };

        private enum eTiposAtencionInterconsulta
        {
            ESTUDIOS_DE_GABINETE = 4
        };
    }
}