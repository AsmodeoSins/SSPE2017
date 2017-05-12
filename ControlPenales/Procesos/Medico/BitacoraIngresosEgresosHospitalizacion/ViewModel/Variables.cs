using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace ControlPenales
{
    public partial class BitacoraIngresosEgresosHospitalizacionViewModel
    {
        private int indexTab = 0;
        public int IndexTab
        {
            get { return indexTab; }
            set { indexTab = value; RaisePropertyChanged("IndexTab"); }
        }

        private bool hospitalizacionesConAntiguedad;
        public bool HospitalizacionesConAntiguedad
        {
            get { return hospitalizacionesConAntiguedad; }
            set { hospitalizacionesConAntiguedad = value; OnPropertyChanged("HospitalizacionesConAntiguedad"); }
        }

        private DateTime selectedFechaHospitalizacion;
        public DateTime SelectedFechaHospitalizacion
        {
            get { return selectedFechaHospitalizacion; }
            set { selectedFechaHospitalizacion = value; OnPropertyChanged("SelectedFechaHospitalizacion"); }
        }

        private bool ingresoHospitalizacionEnabled;
        public bool IngresoHospitalizacionEnabled
        {
            get { return ingresoHospitalizacionEnabled; }
            set { ingresoHospitalizacionEnabled = value; OnPropertyChanged("IngresoHospitalizacionEnabled"); }
        }

        private byte[] fotoIngresoNotaMedica;
        public byte[] FotoIngresoNotaMedica
        {
            get { return fotoIngresoNotaMedica; }
            set { fotoIngresoNotaMedica = value; OnPropertyChanged("FotoIngresoNotaMedica"); }
        }

        private bool notaMedicaSelected;
        public bool NotaMedicaSelected
        {
            get { return notaMedicaSelected; }
            set { notaMedicaSelected = value; OnPropertyChanged("NotaMedicaSelected"); }
        }

        private DateTime? hospitalizacionFecha;
        public DateTime? HospitalizacionFecha
        {
            get { return hospitalizacionFecha; }
            set { hospitalizacionFecha = value; OnPropertyValidateChanged("HospitalizacionFecha"); }
        }

        private bool pInsertar = false;
        public bool PInsertar
        {
            get { return pInsertar; }
            set
            {
                pInsertar = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set
            {
                pEditar = value;
            }
        }

        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
            }
        }

        private bool pImprimir = false;
        public bool PImprimir
        {
            get { return pImprimir; }
            set
            {
                pImprimir = value;
            }
        }

        private bool ingresarEnabled;
        public bool IngresarEnabled
        {
            get { return ingresarEnabled; }
            set { ingresarEnabled = value; OnPropertyChanged("IngresarEnabled"); }
        }

        private bool guardarMenuEnabled;
        public bool GuardarMenuEnabled
        {
            get { return guardarMenuEnabled; }
            set { guardarMenuEnabled = value; OnPropertyChanged("GuardarMenuEnabled"); }
        }

        private bool ingresarMenuEnabled;
        public bool IngresarMenuEnabled
        {
            get { return ingresarMenuEnabled; }
            set { ingresarMenuEnabled = value; OnPropertyChanged("IngresarMenuEnabled"); }
        }

        private bool egresarMenuEnabled;
        public bool EgresarMenuEnabled
        {
            get { return egresarMenuEnabled; }
            set { egresarMenuEnabled = value; OnPropertyChanged("EgresarMenuEnabled"); }
        }

        private bool buscarMenuEnabled;
        public bool BuscarMenuEnabled
        {
            get { return buscarMenuEnabled; }
            set { buscarMenuEnabled = value; OnPropertyChanged("BuscarMenuEnabled"); }
        }

        private bool cancelarMenuEnabled;
        public bool CancelarMenuEnabled
        {
            get { return cancelarMenuEnabled; }
            set { cancelarMenuEnabled = value; OnPropertyChanged("CancelarMenuEnabled"); }
        }

        private bool reporteMenuEnabled;
        public bool ReporteMenuEnabled
        {
            get { return reporteMenuEnabled; }
            set { reporteMenuEnabled = value; OnPropertyChanged("ReporteMenuEnabled"); }
        }

        private bool salirMenuEnabled;
        public bool SalirMenuEnabled
        {
            get { return salirMenuEnabled; }
            set { salirMenuEnabled = value; OnPropertyChanged("SalirMenuEnabled"); }
        }

        private int? anioHospitalizacion;
        public int? AnioHospitalizacion
        {
            get { return anioHospitalizacion; }
            set { anioHospitalizacion = value; OnPropertyChanged("AnioHospitalizacion"); }
        }

        private int? folioHospitalizacion;
        public int? FolioHospitalizacion
        {
            get { return folioHospitalizacion; }
            set { folioHospitalizacion = value; OnPropertyChanged("FolioHospitalizacion"); }
        }

        private string nombreHospitalizacion;
        public string NombreHospitalizacion
        {
            get { return nombreHospitalizacion; }
            set { nombreHospitalizacion = value; OnPropertyChanged("NombreHospitalizacion"); }
        }

        private string apellidoPaternoHospitalizacion;
        public string ApellidoPaternoHospitalizacion
        {
            get { return apellidoPaternoHospitalizacion; }
            set { apellidoPaternoHospitalizacion = value; OnPropertyChanged("ApellidoPaternoHospitalizacion"); }
        }

        private string apellidoMaternoHospitalizacion;
        public string ApellidoMaternoHospitalizacion
        {
            get { return apellidoMaternoHospitalizacion; }
            set { apellidoMaternoHospitalizacion = value; OnPropertyChanged("ApellidoMaternoHospitalizacion"); }
        }

        private string tituloGroupBoxHospitalizaciones;
        public string TituloGroupBoxHospitalizaciones
        {
            get { return tituloGroupBoxHospitalizaciones; }
            set
            {
                tituloGroupBoxHospitalizaciones = value;
                OnPropertyChanged("TituloGroupBoxHospitalizaciones");
            }
        }

        private string camasDesocupadas;
        public string CamasDesocupadas
        {
            get { return camasDesocupadas; }
            set { camasDesocupadas = value; OnPropertyChanged("CamasDesocupadas"); }
        }

        private string camasOcupadas;
        public string CamasOcupadas
        {
            get { return camasOcupadas; }
            set { camasOcupadas = value; OnPropertyChanged("CamasOcupadas"); }
        }

        private BitacoraIngresosEgresosHospitalizacionView ventana;
        public BitacoraIngresosEgresosHospitalizacionView Ventana
        {
            get { return ventana; }
            set { ventana = value; OnPropertyChanged("Ventana"); }
        }

        private DateTime fechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return fechaServer; }
            set
            {
                fechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        private List<NOTA_MEDICA> listaNotasMedicas;
        public List<NOTA_MEDICA> ListaNotasMedicas
        {
            get { return listaNotasMedicas; }
            set { listaNotasMedicas = value; OnPropertyChanged("ListaNotasMedicas"); }
        }

        private NOTA_MEDICA selectedNotaMedica;
        public NOTA_MEDICA SelectedNotaMedica
        {
            get { return selectedNotaMedica; }
            set { selectedNotaMedica = value; OnPropertyChanged("SelectedNotaMedica"); }
        }

        private List<HOSPITALIZACION> listaHospitalizaciones;
        public List<HOSPITALIZACION> ListaHospitalizaciones
        {
            get { return listaHospitalizaciones; }
            set { listaHospitalizaciones = value; OnPropertyChanged("ListaHospitalizaciones"); }
        }

        private HOSPITALIZACION selectedHospitalizacion;
        public HOSPITALIZACION SelectedHospitalizacion
        {
            get { return selectedHospitalizacion; }
            set { selectedHospitalizacion = value; OnPropertyChanged("SelectedHospitalizacion"); }
        }

        private List<HOSPITALIZACION_INGRESO_TIPO> listaTipoHospitalizaciones;
        public List<HOSPITALIZACION_INGRESO_TIPO> ListaTipoHospitalizaciones
        {
            get { return listaTipoHospitalizaciones; }
            set
            {
                listaTipoHospitalizaciones = value;

                OnPropertyChanged("ListaTipoHospitalizaciones");
            }
        }

        private decimal selectedTipoHospitalizacionValue;
        public decimal SelectedTipoHospitalizacionValue
        {
            get { return selectedTipoHospitalizacionValue; }
            set
            {
                selectedTipoHospitalizacionValue = value;
                if (SelectedNotaMedica != null)
                {
                    SelectedNotaMedica = null;
                    GuardarMenuEnabled = false;
                    AnioHospitalizacion = FolioHospitalizacion = null;
                    ApellidoPaternoHospitalizacion = ApellidoMaternoHospitalizacion = NombreHospitalizacion = string.Empty;
                }
                OnPropertyValidateChanged("SelectedTipoHospitalizacionValue");
            }
        }

        private List<CAMA_HOSPITAL> listaCamasHospitalizacion;
        public List<CAMA_HOSPITAL> ListaCamasHospitalizacion
        {
            get { return listaCamasHospitalizacion; }
            set { listaCamasHospitalizacion = value; OnPropertyChanged("ListaCamasHospitalizacion"); }
        }

        private short selectedCamaHospitalValue;
        public short SelectedCamaHospitalValue
        {
            get { return selectedCamaHospitalValue; }
            set { selectedCamaHospitalValue = value; OnPropertyValidateChanged("SelectedCamaHospitalValue"); }
        }

        private string usuarioAreaMedica;
        public string UsuarioAreaMedica
        {
            get { return usuarioAreaMedica; }
            set { usuarioAreaMedica = value; OnPropertyChanged("UsuarioAreaMedica"); }
        }

        private DateTime? fechaMinimaHospitalizacion;
        public DateTime? FechaMinimaHospitalizacion
        {
            get { return fechaMinimaHospitalizacion; }
            set { fechaMinimaHospitalizacion = value; OnPropertyChanged("FechaMinimaHospitalizacion"); }
        }

        private DateTime? fechaMaximaHospitalizacion;
        public DateTime? FechaMaximaHospitalizacion
        {
            get { return fechaMaximaHospitalizacion; }
            set { fechaMaximaHospitalizacion = value; OnPropertyChanged("FechaMaximaHospitalizacion"); }
        }

        private enum enumHospitalizacionIngresoTipo
        {
            EXTERNA = 1,
            URGENCIA = 2,
            ESPECIALIDAD = 3
        }

        private enum enumHospitalizacionEstatus
        {
            HOSPITALIZADO = 1,
            ALTA = 2
        }

        const int SELECCIONE = -1;
        const string ACTIVA = "S";
        const string INACTIVA = "N";
        const string OCUPADA = "O";


        #region Egreso
        #region Datos Imputado Egreso
        private string textAnioImputadoEgreso = string.Empty;
        public string TextAnioImputadoEgreso
        {
            get { return textAnioImputadoEgreso; }
            set { textAnioImputadoEgreso = value; RaisePropertyChanged("TextAnioImputadoEgreso"); }
        }

        private string textFolioImputadoEgreso = string.Empty;
        public string TextFolioImputadoEgreso
        {
            get { return textFolioImputadoEgreso; }
            set { textFolioImputadoEgreso = value; RaisePropertyChanged("TextFolioImputadoEgreso"); }
        }

        private string textPaternoImputadoEgreso = string.Empty;
        public string TextPaternoImputadoEgreso
        {
            get { return textPaternoImputadoEgreso; }
            set { textPaternoImputadoEgreso = value; RaisePropertyChanged("TextPaternoImputadoEgreso"); }
        }

        private string textMaternoImputadoEgreso = string.Empty;
        public string TextMaternoImputadoEgreso
        {
            get { return textMaternoImputadoEgreso; }
            set { textMaternoImputadoEgreso = value; RaisePropertyChanged("TextMaternoImputadoEgreso"); }
        }

        private string textNombreImputadoEgreso = string.Empty;
        public string TextNombreImputadoEgreso
        {
            get { return textNombreImputadoEgreso; }
            set { textNombreImputadoEgreso = value; RaisePropertyChanged("TextNombreImputadoEgreso"); }
        }

        private string textSexoImputadoEgreso = string.Empty;
        public string TextSexoImputadoEgreso
        {
            get { return textSexoImputadoEgreso; }
            set { textSexoImputadoEgreso = value; RaisePropertyChanged("TextSexoImputadoEgreso"); }
        }

        private string textEdadImputadoEgreso = string.Empty;
        public string TextEdadImputadoEgreso
        {
            get { return textEdadImputadoEgreso; }
            set { textEdadImputadoEgreso = value; RaisePropertyChanged("TextEdadImputadoEgreso"); }
        }

        private string textCamaHospitalizacionEgreso;
        public string TextCamaHospitalizacionEgreso
        {
            get { return textCamaHospitalizacionEgreso; }
            set { textCamaHospitalizacionEgreso = value; RaisePropertyChanged("TextCamaHospitalizacionEgreso"); }
        }


        private byte[] imagenIngresoEgreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoEgreso
        {
            get { return imagenIngresoEgreso; }
            set
            {
                imagenIngresoEgreso = value;
                OnPropertyChanged("ImagenIngresoEgreso");
            }
        }
        #endregion

        #region Catalogos
        private ObservableCollection<MOTIVO_EGRESO_MEDICO> lstMotivoEgresoMedico = null;
        public ObservableCollection<MOTIVO_EGRESO_MEDICO> LstMotivoEgresoMedico
        {
            get { return lstMotivoEgresoMedico; }
            set { lstMotivoEgresoMedico = value; RaisePropertyChanged("LstMotivoEgresoMedico"); }
        }

        private decimal selectedMotivoEgresoMedicoValue = -1;
        public decimal SelectedMotivoEgresoMedicoValue
        {
            get { return selectedMotivoEgresoMedicoValue; }
            set { selectedMotivoEgresoMedicoValue = value; OnPropertyValidateChanged("SelectedMotivoEgresoMedicoValue"); }
        }

        private List<cUsuarioExtendida> lstEmpleados = new List<cUsuarioExtendida>();
        public List<cUsuarioExtendida> LstEmpleados
        {
            get { return lstEmpleados; }
            set { lstEmpleados = value; RaisePropertyChanged("LstEmpleados"); }
        }


        #endregion

        #region Excarcelacion
        private string fechaExcarcelacionEgresoHospitalizacion = string.Empty;
        public string FechaExcarcelacionEgresoHospitalizacion
        {
            get { return fechaExcarcelacionEgresoHospitalizacion; }
            set { fechaExcarcelacionEgresoHospitalizacion = value; RaisePropertyChanged("FechaExcarcelacionEgresoHospitalizacion"); }
        }

        private string destinoExcarcelacionEgresoHospitalizacion = string.Empty;
        public string DestinoExcarcelacionEgresoHospitalizacion
        {
            get { return destinoExcarcelacionEgresoHospitalizacion; }
            set { destinoExcarcelacionEgresoHospitalizacion = value; RaisePropertyChanged("DestinoExcarcelacionEgresoHospitalizacion"); }
        }
        #endregion

        #region Liberacion
        private string folioLiberacion=string.Empty;
        public string FolioLiberacion
        {
            get { return folioLiberacion; }
            set { folioLiberacion = value; RaisePropertyChanged("FolioLiberacion"); }
        }
        #endregion

        #region Nota de Egreso
        private DateTime fechaIngresoHospitalizacion;
        public DateTime FechaIngresoHospitalizacion
        {
            get { return fechaIngresoHospitalizacion; }
            set { fechaIngresoHospitalizacion = value; RaisePropertyChanged("FechaIngresoHospitalizacion"); }
        }

        private string diagnosticoIngresoHospitalizacion = string.Empty;
        public string DiagnosticoIngresoHospitalizacion
        {
            get { return diagnosticoIngresoHospitalizacion; }
            set { diagnosticoIngresoHospitalizacion = value; RaisePropertyChanged("DiagnosticoIngresoHospitalizacion"); }
        }

        private DateTime? fechaEgresoHospitalizacion = null;
        public DateTime? FechaEgresoHospitalizacion
        {
            get { return fechaEgresoHospitalizacion; }
            set { fechaEgresoHospitalizacion = value; RaisePropertyChanged("FechaEgresoHospitalizacion"); }
        }

        private short? diasHospitalizado = null;
        public short? DiasHospitalizado
        {
            get { return diasHospitalizado; }
            set { diasHospitalizado = value; RaisePropertyChanged("DiasHospitalizado"); }
        }

        private DateTime? fechaMaximaEgresoHospitalizacion = Fechas.GetFechaDateServer;
        public DateTime? FechaMaximaEgresoHospitalizacion
        {
            get { return fechaMaximaEgresoHospitalizacion; }
            set { fechaMaximaEgresoHospitalizacion = value; RaisePropertyChanged("FechaMaximaEgresoHospitalizacion"); }
        }

        private bool isFechaEgresoValida = false;
        public bool IsFechaEgresoValida
        {
            get { return isFechaEgresoValida; }
            set { isFechaEgresoValida = value; RaisePropertyChanged("IsFechaEgresoValida"); }
        }

        private List<TratamientoHistorial> lstTratamientoMedicoHistorial=null;
        public List<TratamientoHistorial> LstTratamientoMedicoHistorial
        {
            get { return lstTratamientoMedicoHistorial; }
            set { lstTratamientoMedicoHistorial = value; RaisePropertyChanged("LstTratamientoMedicoHistorial"); }
        }

        private List<NOTA_MEDICA_DIETA> lstDietas;
        public List<NOTA_MEDICA_DIETA> LstDietas
        {
            get { return lstDietas; }
            set { lstDietas = value; RaisePropertyChanged("LstDietas"); }
        }

        private List<PROC_ATENCION_MEDICA_PROG> lstProcedimientos;
        public List<PROC_ATENCION_MEDICA_PROG> LstProcedimientos
        {
            get { return lstProcedimientos; }
            set { lstProcedimientos = value; RaisePropertyChanged("LstProcedimientos"); }
        }

        private string textEvolucionEstadoActualEgresoMedico = string.Empty;
        public string TextEvolucionEstadoActualEgresoMedico
        {
            get { return textEvolucionEstadoActualEgresoMedico; }
            set { textEvolucionEstadoActualEgresoMedico = value; RaisePropertyChanged("TextEvolucionEstadoActualEgresoMedico"); }
        }

        #region Autocomplete Enfermedad
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteTB;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteTB
        {
            get { return _AutoCompleteTB; }
            set { _AutoCompleteTB = value; }
        }

        private ListBox _AutoComplete;
        public ListBox AutoCompleteLB
        {
            get { return _AutoComplete; }
            set { _AutoComplete = value; }
        }
        private ControlPenales.Controls.AutoCompleteTextBox _AutoCompleteReceta;
        public ControlPenales.Controls.AutoCompleteTextBox AutoCompleteReceta
        {
            get { return _AutoCompleteReceta; }
            set { _AutoCompleteReceta = value; }
        }
        private ListBox _AutoCompleteRecetaLB;
        public ListBox AutoCompleteRecetaLB
        {
            get { return _AutoCompleteRecetaLB; }
            set { _AutoCompleteRecetaLB = value; }
        }
        #endregion

        private ObservableCollection<ENFERMEDAD> _ListEnfermedades;
        public ObservableCollection<ENFERMEDAD> ListEnfermedades
        {
            get { return _ListEnfermedades; }
            set { _ListEnfermedades = value; OnPropertyChanged("ListEnfermedades"); }
        }
        private ENFERMEDAD _SelectEnfermedad;
        public ENFERMEDAD SelectEnfermedad
        {
            get { return _SelectEnfermedad; }
            set { _SelectEnfermedad = value; OnPropertyChanged("SelectEnfermedad"); }
        }

        private RecetaMedica _SelectReceta;
        public RecetaMedica SelectReceta
        {
            get { return _SelectReceta; }
            set { _SelectReceta = value; OnPropertyChanged("SelectReceta"); }
        }
        private ObservableCollection<RecetaMedica> _ListRecetas;
        public ObservableCollection<RecetaMedica> ListRecetas
        {
            get { return _ListRecetas; }
            set { _ListRecetas = value; OnPropertyChanged("ListRecetas"); }
        }
        private ObservableCollection<DietaMedica> lstDietasTratamiento = null;
        public ObservableCollection<DietaMedica> LstDietasTratamiento
        {
            get { return lstDietasTratamiento; }
            set { lstDietasTratamiento = value; RaisePropertyChanged("LstDietasTratamiento"); }
        }

        private DietaMedica selectedDietaTratamiento;
        public DietaMedica SelectedDietaTratamiento
        {
            get { return selectedDietaTratamiento; }
            set { selectedDietaTratamiento = value; RaisePropertyChanged("SelectedDietaTratamiento"); }
        }

        private ObservableCollection<PROC_MED_SUBTIPO> lstProcMedSubTipo = null;
        public ObservableCollection<PROC_MED_SUBTIPO> LstProcMedSubTipo
        {
            get { return lstProcMedSubTipo; }
            set { lstProcMedSubTipo = value; RaisePropertyChanged("LstProcMedSubTipo"); }
        }

        private short selectedProcMedSubTipoValue = -1;
        public short SelectedProcMedSubTipoValue
        {
            get { return selectedProcMedSubTipoValue; }
            set { selectedProcMedSubTipoValue = value; OnPropertyValidateChanged("SelectedProcMedSubTipoValue"); }
        }

        private ObservableCollection<PROC_MED> lstProcedimientoMedico = null;
        public ObservableCollection<PROC_MED> LstProcedimientoMedico
        {
            get { return lstProcedimientoMedico; }
            set { lstProcedimientoMedico = value; RaisePropertyChanged("LstProcedimientoMedico"); }
        }

        private short selectedProcedimientoMedicoValue = -1;
        public short SelectedProcedimientoMedicoValue
        {
            get { return selectedProcedimientoMedicoValue; }
            set { selectedProcedimientoMedicoValue = value; RaisePropertyChanged("SelectedProcedimientoMedicoValue"); }
        }

        private ObservableCollection<CustomProcedimientosMedicosSeleccionados> _ListProcMedsSeleccionados;
        public ObservableCollection<CustomProcedimientosMedicosSeleccionados> ListProcMedsSeleccionados
        {
            get { return _ListProcMedsSeleccionados; }
            set { _ListProcMedsSeleccionados = value; OnPropertyChanged("ListProcMedsSeleccionados"); }
        }

        private CustomProcedimientosMedicosSeleccionados selectProcMedSeleccionado = null;
        public CustomProcedimientosMedicosSeleccionados SelectProcMedSeleccionado
        {
            get { return selectProcMedSeleccionado; }
            set { selectProcMedSeleccionado = value; RaisePropertyChanged("SelectProcMedSeleccionado"); }
        }

        private bool _CheckedSeguimiento = false;
        public bool CheckedSeguimiento
        {
            get { return _CheckedSeguimiento; }
            set
            {
                _CheckedSeguimiento = value;
                OnPropertyValidateChanged("CheckedSeguimiento");
                SeguimientoEnabled = value;
                StrFechaSeguimiento = string.Empty;
                AtencionCitaSeguimiento = null;
            }
        }

        private bool _SeguimientoEnabled;
        public bool SeguimientoEnabled
        {
            get { return _SeguimientoEnabled; }
            set { _SeguimientoEnabled = value; OnPropertyChanged("SeguimientoEnabled"); }
        }

        private bool solicitaInterconsultaCheck = false;
        public bool SolicitaInterconsultaCheck
        {
            get { return solicitaInterconsultaCheck; }
            set { solicitaInterconsultaCheck = value; RaisePropertyChanged("SolicitaInterconsultaCheck"); }
        }

        #endregion

        #region Agenda 
        private AgendarCitaConCalendarioView _AgendaView;
        public AgendarCitaConCalendarioView AgendaView
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

        private bool _EmpleadosEnAgendaEnabled = false;
        public bool EmpleadosEnAgendaEnabled
        {
            get { return _EmpleadosEnAgendaEnabled; }
            set { _EmpleadosEnAgendaEnabled = value; OnPropertyChanged("EmpleadosEnAgendaEnabled"); }
        }

        private string _ProcedimientoMedicoPorAgendar;
        public string ProcedimientoMedicoPorAgendar
        {
            get { return _ProcedimientoMedicoPorAgendar; }
            set { _ProcedimientoMedicoPorAgendar = value; OnPropertyChanged("ProcedimientoMedicoPorAgendar"); }
        }

        private Visibility _AgregarProcedimientoMedicoLayoutVisible = Visibility.Visible;
        public Visibility AgregarProcedimientoMedicoLayoutVisible
        {
            get { return _AgregarProcedimientoMedicoLayoutVisible; }
            set { _AgregarProcedimientoMedicoLayoutVisible = value; OnPropertyChanged("AgregarProcedimientoMedicoLayoutVisible"); }
        }

        private cUsuarioExtendida selectedEmpleadoValue;
        public cUsuarioExtendida SelectedEmpleadoValue
        {
            get { return selectedEmpleadoValue; }
            set
            {
                BuscarAgenda = (value != null ? selectedEmpleadoValue != null ? selectedEmpleadoValue.ID_EMPLEADO == value.ID_EMPLEADO : false : false);
                selectedEmpleadoValue = value;
                RaisePropertyChanged("SelectedEmpleadoValue");
            }
        }

        private DateTime? selectedDateCalendar;
        public DateTime? SelectedDateCalendar
        {
            get { return selectedDateCalendar; }
            set { selectedDateCalendar = value; RaisePropertyChanged("SelectedDateCalendar"); }
        }

        private DateTime? selectedDateBusqueda = DateTime.Now;
        public DateTime? SelectedDateBusqueda
        {
            get { return selectedDateBusqueda; }
            set { selectedDateBusqueda = value; RaisePropertyChanged("SelectedDateBusqueda"); }
        }

        private string tituloAgenda = string.Empty;
        public string TituloAgenda
        {
            get { return tituloAgenda; }
            set { tituloAgenda = value; RaisePropertyChanged("TituloAgenda"); }
        }

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value; OnPropertyChanged("SelectedIngreso"); }
        }

        private bool isAgendarCitaEnabled = true;
        public bool IsAgendarCitaEnabled
        {
            get { return isAgendarCitaEnabled; }
            set { isAgendarCitaEnabled = value; RaisePropertyChanged("IsAgendarCitaEnabled"); }
        }

        private bool _GuardarAgendaEnabled;
        public bool GuardarAgendaEnabled
        {
            get { return _GuardarAgendaEnabled; }
            set { _GuardarAgendaEnabled = value; OnPropertyChanged("GuardarAgendaEnabled"); }
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

        private bool aFechaValid = false;
        public bool AFechaValid
        {
            get { return aFechaValid; }
            set { aFechaValid = value; OnPropertyChanged("AFechaValid"); }
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
        private string aFechaMensaje = "La fecha es requerida.";
        public string AFechaMensaje
        {
            get { return aFechaMensaje; }
            set { aFechaMensaje = value; OnPropertyChanged("AFechaMensaje"); }
        }

        private bool agregarHora = false;
        public bool AgregarHora
        {
            get { return agregarHora; }
            set { agregarHora = value; OnPropertyChanged("AgregarHora"); }
        }

        private ObservableCollection<CustomCitasProcedimientosMedicos> _ProcedimientosMedicosEnCitaEnMemoria;
        public ObservableCollection<CustomCitasProcedimientosMedicos> ProcedimientosMedicosEnCitaEnMemoria
        {
            get { return _ProcedimientosMedicosEnCitaEnMemoria; }
            set { _ProcedimientosMedicosEnCitaEnMemoria = value; OnPropertyChanged("ProcedimientosMedicosEnCitaEnMemoria"); }
        }

        private string mensajeError;
        public string MensajeError
        {
            get { return mensajeError; }
            set
            {
                mensajeError = value;
                OnPropertyChanged("MensajeError");
            }
        }

        private string strFechaSeguimiento = string.Empty;
        public string StrFechaSeguimiento
        {
            get { return strFechaSeguimiento; }
            set { strFechaSeguimiento = value; RaisePropertyChanged("StrFechaSeguimiento"); }
        }

        private PROC_MED SelectProcMedEnCitaParaAgendarAux;
        private PROC_MED _SelectProcMedEnCitaParaAgendar;
        public PROC_MED SelectProcMedEnCitaParaAgendar
        {
            get { return _SelectProcMedEnCitaParaAgendar; }
            set
            {
                _SelectProcMedEnCitaParaAgendar = value;
                if (value != null ? value.ID_PROCMED != -1 : false)
                {
                    SelectProcMedEnCitaParaAgendarAux = new PROC_MED
                    {
                        DESCR = value.DESCR,
                        ESTATUS = value.ESTATUS,
                        ID_PROCMED = value.ID_PROCMED,
                        ID_PROCMED_SUBTIPO = value.ID_PROCMED_SUBTIPO,
                        PROC_MED_SUBTIPO = value.PROC_MED_SUBTIPO,
                        PROC_ATENCION_MEDICA = value.PROC_ATENCION_MEDICA,
                        PROC_MATERIAL = value.PROC_MATERIAL
                    }; ;
                }
                OnPropertyChanged("SelectProcMedEnCitaParaAgendar");
            }
        }

        private CustomCitasProcedimientosMedicos _SelectProcedimientoMedicoEnCitaEnMemoria;
        public CustomCitasProcedimientosMedicos SelectProcedimientoMedicoEnCitaEnMemoria
        {
            get { return _SelectProcedimientoMedicoEnCitaEnMemoria; }
            set { _SelectProcedimientoMedicoEnCitaEnMemoria = value; OnPropertyChanged("SelectProcedimientoMedicoEnCitaEnMemoria"); }
        }
        #endregion

        private bool isNotaEgresoEnabled = false;
        public bool IsNotaEgresoEnabled
        {
            get { return isNotaEgresoEnabled; }
            set { isNotaEgresoEnabled = value; RaisePropertyChanged("IsNotaEgresoEnabled"); }
        }
        #endregion


        #region Variables Privadas
        private enumProcesos? proceso_origen = null;
        private DateTime? fecha_proceso_origen = null;
        private enumResultadoOperacion? resultado_operacion=null;
        private CustomProcedimientosMedicosSeleccionados procedimientoMedicoParaAgenda;
        private bool BuscarAgenda = false;
        private DateTime? fHoy = Fechas.GetFechaDateServer;
        private short?[] ParametroEstatusInactivos;
        private ATENCION_CITA AtencionCitaSeguimiento;
        private IQueryable<PROCESO_USUARIO> permisos;
        private bool _permisos_agregar = false;
        private string ParametroEstatusCitaSinAtender;

        private TIPO_GUARDAR_HOSPITALIZACION? tipo_seleccionado = null;
        private List<EXT_REPORTE_BITACORA_HOSPITALIZACION_DETALLE> ds_detalle = null;
        private List<EXT_REPORTE_BITACORA_HOSPITALIZACION_ENCABEZADO> ds_encabezado = null;
        #endregion

        #region Variables Visualizacion de Controles
        private Visibility isNotaEgresoVisible = Visibility.Collapsed;
        public Visibility IsNotaEgresoVisible
        {
            get { return isNotaEgresoVisible; }
            set { isNotaEgresoVisible = value; RaisePropertyChanged("IsNotaEgresoVisible"); }
        }
        private Visibility isExcarcelacionVisible = Visibility.Collapsed;
        public Visibility IsExcarcelacionVisible
        {
            get { return isExcarcelacionVisible; }
            set { isExcarcelacionVisible = value; RaisePropertyChanged("IsExcarcelacionVisible"); }
        }

        private Visibility isLiberacionVisible = Visibility.Collapsed;
        public Visibility IsLiberacionVisible
        {
            get { return isLiberacionVisible; }
            set { isLiberacionVisible = value; RaisePropertyChanged("IsLiberacionVisible"); }
        }

        #endregion
        #region Variables para Habilitar/Deshabilitar Controles
        private bool menuAgregarEnabled = false;
        public bool MenuAgregarEnabled
        {
            get { return menuAgregarEnabled; }
            set { menuAgregarEnabled = value; RaisePropertyChanged("MenuAgregarEnabled"); }
        }
        private bool isBuscarporFechaEnabled = false;
        public bool IsBuscarporFechaEnabled
        {
            get { return isBuscarporFechaEnabled; }
            set { isBuscarporFechaEnabled = value; RaisePropertyChanged("IsBuscarporFechaEnabled"); }
        }
        #endregion
    }
}
