using ControlPenales.BiometricoServiceReference;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class AgendaEnfermeroViewModel
    {
        #region Variables Privadas
        private DateTime? fechaOriginal = null;
        private ATENCION_CITA selectedAtencion_Cita;
        private List<short> roles = null;
        //private ModoAgregarAgenda tipoAgregarAgenda;
        private ModoBusqueda tipoBusquedaImputado;
        private IQueryable<PROCESO_USUARIO> permisos;
        private bool permisos_crear = false;
        private cUsuarioExtendida _empleado = null;
        private short?[] estatus_inactivos = null;
        DateTime _FechaServer = Fechas.GetFechaDateServer;
        #endregion

        #region Agenda
        private string headerAgenda = string.Empty;
        public string HeaderAgenda
        {
            get { return headerAgenda; }
            set { headerAgenda = value; RaisePropertyChanged("HeaderAgenda"); }
        }

        private ObservableCollection<Appointment> lstAgenda = new ObservableCollection<Appointment>();
        public ObservableCollection<Appointment> LstAgenda
        {
            get { return lstAgenda; }
            set { lstAgenda = value; RaisePropertyChanged("LstAgenda"); }
        }

        private DateTime fechaAgenda = DateTime.Now;
        public DateTime FechaAgenda
        {
            get { return fechaAgenda; }
            set { fechaAgenda = value; RaisePropertyChanged("FechaAgenda"); }
        }
        #endregion

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

        private DateTime? agregarAgendaFecha;
        public DateTime? AgregarAgendaFecha
        {
            get { return agregarAgendaFecha; }
            set { agregarAgendaFecha = value; RaisePropertyChanged("AgregarAgendaFecha"); }
        }

        private DateTime? agregarAgendaHoraI;
        public DateTime? AgregarAgendaHoraI
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

        private DateTime? agregarAgendaHoraF;
        public DateTime? AgregarAgendaHoraF
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


        private ObservableCollection<AREA> lstAreas;
        public ObservableCollection<AREA> LstAreas
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

        private List<cUsuarioExtendida> lstAgendaEmpleados = new List<cUsuarioExtendida>();
        public List<cUsuarioExtendida> LstAgendaEmpleados
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

        #region Variables para habilitas controles

        private bool isAgendaEnabled = false;
        public bool IsAgendaEnabled
        {
            get { return isAgendaEnabled; }
            set { isAgendaEnabled = value; RaisePropertyChanged("IsAgendaEnabled"); }
        }

        private bool isEmpleadoEnabled = false;
        public bool IsEmpleadoEnabled
        {
            get { return isEmpleadoEnabled; }
            set { isEmpleadoEnabled = value; RaisePropertyChanged("IsEmpleadoEnabled"); }
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

        private bool isEnabledBuscarImputadoAgregarAgenda = false;
        public bool IsEnabledBuscarImputadoAgregarAgenda
        {
            get { return isEnabledBuscarImputadoAgregarAgenda; }
            set { isEnabledBuscarImputadoAgregarAgenda = value; RaisePropertyChanged("IsEnabledBuscarImputadoAgregarAgenda"); }
        }
        #endregion

        #region Calendario
        private DateTime fechaInicial;
        public DateTime FechaInicial
        {
            get { return fechaInicial; }
            set { fechaInicial = value; RaisePropertyChanged("FechaInicial"); }
        }
        private DateTime? busquedaFecha;
        public DateTime? BusquedaFecha
        {
            get { return busquedaFecha; }
            set { busquedaFecha = value; RaisePropertyChanged("BusquedaFecha"); }
        }
        private List<DateTime> fechasAgendadas;
        public List<DateTime> FechasAgendadas
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

        #region Incidencia
        private ObservableCollection<ATENCION_CITA_IN_MOTIVO> lstIncidenteMotivo;
        public ObservableCollection<ATENCION_CITA_IN_MOTIVO> LstIncidenteMotivo
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
        #endregion

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

        private byte[] imagenIngreso; //compartido entre datos y busqueda
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set { imagenIngreso = value; RaisePropertyChanged("ImagenIngreso"); }
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
                    if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
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
                        if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                            ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
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

        private bool emptyIngresoVisible = true;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
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

        private INGRESO selectIngresoAuxiliar = null;
        private string textBotonSeleccionarIngreso = "seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        #region Variables privadas
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }
        #endregion
        #endregion

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

        private ObservableCollection<ATENCION_CITA> busquedaAgenda;
        public ObservableCollection<ATENCION_CITA> BusquedaAgenda
        {
            get { return busquedaAgenda; }
            set { busquedaAgenda = value; RaisePropertyChanged("BusquedaAgenda"); }
        }

        private ObservableCollection<ATENCION_TIPO> lstBusquedaAgendaAtencionTipos;
        public ObservableCollection<ATENCION_TIPO> LstBusquedaAgendaAtencionTipos
        {
            get { return lstBusquedaAgendaAtencionTipos; }
            set { lstBusquedaAgendaAtencionTipos = value; RaisePropertyChanged("LstBusquedaAgendaAtencionTipos"); }
        }

        private short? selectedBusquedaAgendaAtencionTipo = -1;
        public short? SelectedBusquedaAgendaAtencionTipo
        {
            get { return selectedBusquedaAgendaAtencionTipo; }
            set { selectedBusquedaAgendaAtencionTipo = value; RaisePropertyChanged("SelectedBusquedaAgendaAtencionTipo"); }
        }

        private ATENCION_CITA selectedBuscarAgenda;
        public ATENCION_CITA SelectedBuscarAgenda
        {
            get { return selectedBuscarAgenda; }
            set { selectedBuscarAgenda = value; RaisePropertyChanged("SelectedBuscarAgenda"); }
        }

        #endregion

        private enum ModoBusqueda
        {
            BUSQUEDA_AGENDA_IMPUTADO,
            BUSQUEDA_NORMAL_IMPUTADO
        }

    }
}
