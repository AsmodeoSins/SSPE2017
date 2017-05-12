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
using ControlPenales.Clases;
using System.Windows.Controls;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;
using ControlPenales.Controls.Calendario;

namespace ControlPenales
{
    partial class ProgramasLibertadViewModel
    {
        #region Busqueda
        private bool nuevoProcesoEnabled = false;
        public bool NuevoProcesoEnabled
        {
            get { return nuevoProcesoEnabled; }
            set { nuevoProcesoEnabled = value; OnPropertyChanged("NuevoProcesoEnabled"); }
        }

        private bool seleccionarProcesoEnabled = false;
        public bool SeleccionarProcesoEnabled
        {
            get { return seleccionarProcesoEnabled; }
            set { seleccionarProcesoEnabled = value; OnPropertyChanged("SeleccionarProcesoEnabled"); }
        }

        private Visibility emptyProceso = Visibility.Visible;
        public Visibility EmptyProceso
        {
            get { return emptyProceso; }
            set { emptyProceso = value; OnPropertyChanged("EmptyProceso"); }
        }

        private PROCESO_LIBERTAD auxProcesoLibertad;

        private PROCESO_LIBERTAD selectedProcesoLibertad;
        public PROCESO_LIBERTAD SelectedProcesoLibertad
        {
            get { return selectedProcesoLibertad; }
            set
            {
                selectedProcesoLibertad = value;
                if (value != null)
                    SeleccionarProcesoEnabled = true;
                else
                    SeleccionarProcesoEnabled = false;
                OnPropertyChanged("SelectedProcesoLibertad");

            }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                // NuevaMJEnabled = value != null ? true : false;
                if (value != null)
                {
                    var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                    if (foto != null)
                        ImagenInterno = foto.BIOMETRICO;
                    else
                    {
                        if (value.INGRESO != null)
                        {
                            var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            if (ingreso != null)
                            {
                                var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                                if (fotoIngreso != null)
                                    ImagenInterno = fotoIngreso.BIOMETRICO;
                                else
                                    ImagenInterno = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenInterno = new Imagenes().getImagenPerson();
                        }
                    }
                    if (value.PROCESO_LIBERTAD != null)
                    {
                        if (value.PROCESO_LIBERTAD.Count == 0)
                        {
                            EmptyProceso = Visibility.Visible;
                        }
                        else
                            EmptyProceso = Visibility.Collapsed;
                    }
                    else
                        EmptyProceso = Visibility.Visible;

                    
                }
                else
                {
                    EmptyProceso = Visibility.Visible;
                    ImagenInterno = new Imagenes().getImagenPerson();
                   
                }
                OnPropertyChanged("SelectExpediente");
            }
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

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }

        private ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> lstLiberadoMJ;
        public ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> LstLiberadoMJ
        {
            get { return lstLiberadoMJ; }
            set { lstLiberadoMJ = value; OnPropertyChanged("LstLiberadoMJ"); }
        }

        private Visibility emptyMJVisible = System.Windows.Visibility.Collapsed;
        public Visibility EmptyMJVisible
        {
            get { return emptyMJVisible; }
            set { emptyMJVisible = value; OnPropertyChanged("EmptyMJVisible"); }
        }

        private bool nuevaMJEnabled = false;
        public bool NuevaMJEnabled
        {
            get { return nuevaMJEnabled; }
            set { nuevaMJEnabled = value; OnPropertyChanged("NuevaMJEnabled"); }
        }

        private string tituloModal;
        public string TituloModal
        {
            get { return tituloModal; }
            set { tituloModal = value; OnPropertyChanged("TituloModal"); }
        }

        private string _TextNombreEntrevistado;
        public string TextNombreEntrevistado
        {
            get { return _TextNombreEntrevistado; }
            set { _TextNombreEntrevistado = value; OnPropertyChanged("TextNombreEntrevistado"); }
        }

        private string _ApellidoPaternoBuscar;
        public string ApellidoPaternoBuscar
        {
            get { return _ApellidoPaternoBuscar; }
            set { _ApellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }

        private string _ApellidoMaternoBuscar;
        public string ApellidoMaternoBuscar
        {
            get { return _ApellidoMaternoBuscar; }
            set { _ApellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
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

        private short? anioBuscar;
        public short? AnioBuscar
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
        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }

        private bool SeguirCargando { get; set; }
        #endregion

        #region Liberados
        private string _NUCBuscar;
        public string NUCBuscar
        {
            get { return _NUCBuscar; }
            set { _NUCBuscar = value; OnPropertyChanged("NUCBuscar"); }
        }

        private RangeEnabledObservableCollection<cLiberados> lstLiberados;
        public RangeEnabledObservableCollection<cLiberados> LstLiberados
        {
            get { return lstLiberados; }
            set { lstLiberados = value; OnPropertyChanged("LstLiberados"); }
        }

        private cLiberados selectedLiberado;
        public cLiberados SelectedLiberado
        {
            get { return selectedLiberado; }
            set
            {
                selectedLiberado = value;
                if (value != null)
                {
                    SelectExpediente = new cImputado().Obtener(value.ID_IMPUTADO, value.ID_ANIO, value.ID_CENTRO).FirstOrDefault();
                }
                else
                {
                    SelectExpediente = null;
                }
                OnPropertyChanged("SelectedLiberado");
            }
        }
        #endregion

        #region Huellas
        IList<PlantillaBiometrico> HuellasCapturadas;

        private enumTipoBiometrico? _DD_Dedo;
        public enumTipoBiometrico? DD_Dedo
        {
            get { return _DD_Dedo; }
            set { _DD_Dedo = value; }
        }

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
        #endregion

        #region Unidad Receptora
        private ObservableCollection<UNIDAD_RECEPTORA> lstUnidadreceptora;
        public ObservableCollection<UNIDAD_RECEPTORA> LstUnidadreceptora
        {
            get { return lstUnidadreceptora; }
            set { lstUnidadreceptora = value; OnPropertyChanged(); }
        }

        private UNIDAD_RECEPTORA selectedUnidadReceptora;
        public UNIDAD_RECEPTORA SelectedUnidadReceptora
        {
            get { return selectedUnidadReceptora; }
            set { selectedUnidadReceptora = value;
            SetUnidadReceptora();
            //if (value != null)
            //{
            //    LstResponsable = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>();
            //    //LstResponsable = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>(new cUnidadReceptoraReponsable().ObtenerTodos((int)value.ID_UNIDAD_RECEPTORA).Select(w => new UNIDAD_RECEPTORA_RESPONSABLE()
            //    //{
            //    //    ID_UNIDAD_RECEPTORA_RES = w.ID_UNIDAD_RECEPTORA_RES,
            //    //    NOMBRE = string.Format("{0} {1} {2}",
            //    //    !string.IsNullOrEmpty(w.NOMBRE) ? w.NOMBRE : string.Empty,
            //    //    !string.IsNullOrEmpty(w.PATERNO) ? w.PATERNO : string.Empty,
            //    //    !string.IsNullOrEmpty(w.NOMBRE) ? w.NOMBRE : string.Empty)
            //    //}));
            //}
            //else
            //{
            //    LstResponsable = new ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE>();
            //}
            //LstResponsable.Insert(0, new UNIDAD_RECEPTORA_RESPONSABLE() { ID_UNIDAD_RECEPTORA_RES = -1, NOMBRE = "SELECCIONE" });

                OnPropertyChanged("SelectedUnidadReceptora"); }
        }

        private ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE> lstResponsable;
        public ObservableCollection<UNIDAD_RECEPTORA_RESPONSABLE> LstResponsable
        {
            get { return lstResponsable; }
            set { lstResponsable = value; OnPropertyChanged("LstResponsable"); }
        }

        private string direccionUR;
        public string DireccionUR
        {
            get { return direccionUR; }
            set { direccionUR = value; OnPropertyChanged("DireccionUR"); }
        }
        private string telefonoUR;
        public string TelefonoUR
        {
            get { 
                if (telefonoUR == null)
                    return string.Empty;
                return new Converters().MascaraTelefono(telefonoUR);
            }
            set { telefonoUR = value; OnPropertyChanged("TelefonoUR"); }
        }
        #endregion

        #region Programa
        private ObservableCollection<PROGRAMA_LIBERTAD> lstPrograma;
        public ObservableCollection<PROGRAMA_LIBERTAD> LstPrograma
        {
            get { return lstPrograma; }
            set { lstPrograma = value; OnPropertyChanged("LstPrograma"); }
        }

        private ObservableCollection<ACTIVIDAD_PROGRAMA> lstActividad;
        public ObservableCollection<ACTIVIDAD_PROGRAMA> LstActividad
        {
            get { return lstActividad; }
            set { lstActividad = value; OnPropertyChanged("LstActividad"); }
        }

        private PROGRAMA_LIBERTAD selectedPrograma;
        public PROGRAMA_LIBERTAD SelectedPrograma
        {
            get { return selectedPrograma; }
            set { selectedPrograma = value;
            if (value != null)
                LstActividad = new ObservableCollection<ACTIVIDAD_PROGRAMA>(value.ACTIVIDAD_PROGRAMA);
            else
                LstActividad = new ObservableCollection<ACTIVIDAD_PROGRAMA>();
            LstActividad.Insert(0, new ACTIVIDAD_PROGRAMA() { ID_ACTIVIDAD_PROGRAMA = -1, DESCR = "SELECCIONE" });
            ActividadProgramadaAL = -1;

                OnPropertyChanged("SelectedPrograma"); }
        }

        #endregion

        #region Seguimiento
        private ObservableCollection<AGENDA_ACTIVIDAD_LIBERTAD> lstAgenda;
        public ObservableCollection<AGENDA_ACTIVIDAD_LIBERTAD> LstAgenda
        {
            get { return lstAgenda; }
            set { lstAgenda = value;
            if (value == null)
            {
                VisibleAgenda = Visibility.Collapsed;
            }
            else
            { 
                VisibleAgenda = value.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
                OnPropertyChanged("LstAgenda"); }
        }

        private AGENDA_ACTIVIDAD_LIBERTAD _AgendaLibertadDetalle;// = new AGENDA_ACTIVIDAD_LIBERTAD();
        public AGENDA_ACTIVIDAD_LIBERTAD AgendaLibertadDetalle
        {
            get { return _AgendaLibertadDetalle; }
            set {
                _AgendaLibertadDetalle = value;
                //if (value == null)
                //{
                //    EstatusAL = -1;
                //    UnidadReceptoraAL = -1;
                //    ProgramaLibertadAL = ActividadProgramadaAL  = - 1;
                //    FechaInicioAL = FechaFinalAL = null;
                //    RemuneradaAL = FirmaInicioAL = FirmaFinalAL = false;
                //    LstSeguimientoDetalle = new ObservableCollection<cSeguimientoDetalle>();
                //    LstSeguimientoDetalleTodo = new ObservableCollection<AGENDA_ACT_LIB_DETALLE>();

                //    #region Dias
                //    Domingo = Lunes = Martes = Miercoles = Jueves = Viernes = Sabado = Domingo = false;
                //    DomingoInicio = DomingoFin = LunesInicio = LunesFin = MartesInicio = MartesFin = MiercolesInicio = MiercolesFin = JuevesInicio = JuevesFin = ViernesInicio = ViernesFin = SabadoInicio = SabadoFin = null;
                //    #endregion
                //}
                //else
                //{
                //    var x = value.ID_ACTIVIDAD_PROGRAMA;
                //    EstatusAL = value.ID_ESTATUS;
                //    UnidadReceptoraAL = value.ID_UNIDAD_RECEPTORA;
                //    ProgramaLibertadAL = value.ID_PROCESO_LIBERTAD;
                //    ActividadProgramadaAL = x;
                //    FechaInicioAL = value.FECHA_INICIO;
                //    FechaFinalAL = value.FECHA_FINAL;
                //    RemuneradaAL = value.REMUNERADA != null ? value.REMUNERADA == "S" ? true : false : false;
                //    FirmaInicioAL = value.FIRMA_INICIO != null ? value.FIRMA_INICIO == "S" ? true : false : false;
                //    FirmaFinalAL = value.FIRMA_FINAL != null ? value.FIRMA_FINAL == "S" ? true : false : false;
                //    LstSeguimientoDetalleTodo = new ObservableCollection<AGENDA_ACT_LIB_DETALLE>(value.AGENDA_ACT_LIB_DETALLE);
                //    PopulateRecurrencia();
                //}
                OnPropertyChanged("AgendaLibertadDetalle"); }
        }

        private decimal? estatusAL = -1;
        public decimal? EstatusAL
        {
            get { return estatusAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{ 
                //    estatusAL = value;
                //    AgendaLibertadDetalle.ID_ESTATUS = value;
                //}
                //else
                    estatusAL = value;
                OnPropertyChanged("EstatusAL"); }
        }

        private decimal? unidadReceptoraAL = -1;
        public decimal? UnidadReceptoraAL
        {
            get { return unidadReceptoraAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{
                //    unidadReceptoraAL = value;
                //    AgendaLibertadDetalle.ID_UNIDAD_RECEPTORA = value;
                //}
                //else
                    unidadReceptoraAL = value;
                OnPropertyChanged("UnidadReceptoraAL"); }
        }

        private decimal? programaLibertadAL = -1;
        public decimal? ProgramaLibertadAL
        {
            get { return programaLibertadAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{
                //    programaLibertadAL = value;
                //    AgendaLibertadDetalle.ID_PROGRAMA_LIBERTAD = value;
                //}
                //else
                    programaLibertadAL = value;
                OnPropertyChanged("ProgramaLibertadAL");
            }
        }

        private decimal? actividadProgramadaAL = -1;
        public decimal? ActividadProgramadaAL
        {
            get { return actividadProgramadaAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{
                //    actividadProgramadaAL = value;
                //    AgendaLibertadDetalle.ID_ACTIVIDAD_PROGRAMA = value;
                //}
                //else
                actividadProgramadaAL = value.HasValue ? value : -1;
                OnPropertyChanged("ActividadProgramadaAL"); }
        }

        private DateTime? fechaInicioAL;
        public DateTime? FechaInicioAL
        {
            get { return fechaInicioAL; }
            set { 
            //if (AgendaLibertadDetalle != null)
            //    fechaInicioAL = AgendaLibertadDetalle.FECHA_INICIO = value;
            //else
                fechaInicioAL = value;
                OnPropertyChanged("FechaInicioAL"); }
        }

        private DateTime? fechaFinalAL;
        public DateTime? FechaFinalAL
        {
            get { return fechaFinalAL; }
            set { 
            //if (AgendaLibertadDetalle != null)
            //{
            //    fechaFinalAL = AgendaLibertadDetalle.FECHA_FINAL = value;
            //}
            //else
                fechaFinalAL = value;
                OnPropertyChanged("FechaFinalAL"); }
        }

        private bool remuneradaAL = false;
        public bool RemuneradaAL
        {
            get { return remuneradaAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{
                //    remuneradaAL = value;
                //    AgendaLibertadDetalle.REMUNERADA = value ? "S" : "N";
                //}
                //else
                    remuneradaAL = value;
                
                OnPropertyChanged("RemuneradaAL"); }
        }

        private bool firmaInicioAL = false;
        public bool FirmaInicioAL
        {
            get { return firmaInicioAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{ 
                //    firmaInicioAL = value;
                //    AgendaLibertadDetalle.FIRMA_INICIO = value ? "S" : "N";
                //}
                //else
                    firmaInicioAL = value; 
                OnPropertyChanged("FirmaInicioAL"); }
        }

        private bool firmaFinalAL = false;
        public bool FirmaFinalAL
        {
            get { return firmaFinalAL; }
            set {
                //if (AgendaLibertadDetalle != null)
                //{
                //    firmaFinalAL = value;
                //    AgendaLibertadDetalle.FIRMA_FINAL = value ? "S" : "N";
                //}
                //else
                    firmaFinalAL = value;
                OnPropertyChanged("FirmaFinalAL"); }
        }
        #endregion

        #region Seguimiento Detalle
        private ObservableCollection<cSeguimientoDetalle> lstSeguimientoDetalle;
        public ObservableCollection<cSeguimientoDetalle> LstSeguimientoDetalle
        {
            get { return lstSeguimientoDetalle; }
            set { lstSeguimientoDetalle = value; OnPropertyChanged("LstSeguimientoDetalle"); }
        }

        private ObservableCollection<AGENDA_ACT_LIB_DETALLE> lstSeguimientoDetalleTodo;
        public ObservableCollection<AGENDA_ACT_LIB_DETALLE> LstSeguimientoDetalleTodo
        {
            get { return lstSeguimientoDetalleTodo; }
            set { lstSeguimientoDetalleTodo = value; OnPropertyChanged("LstSeguimientoDetalleTodo"); }
        }
       
        #endregion

        #region Semana
        private bool domingo  = false;
        public bool Domingo
        {
            get { return domingo; }
            set { domingo = value;
            if (value)
            {
                base.AddRule(() => DomingoInicio, () => DomingoInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => DomingoFin, () => DomingoFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("DomingoInicio");
                OnPropertyChanged("DomingoFin");
            }
            else
            {
                base.RemoveRule("DomingoInicio");
                base.RemoveRule("DomingoFin");
                OnPropertyChanged("DomingoInicio");
                OnPropertyChanged("DomingoFin");
            }
                OnPropertyChanged("Domingo"); }
        }

        private DateTime? domingoInicio;
        public DateTime? DomingoInicio
        {
            get { return domingoInicio; }
            set { domingoInicio = value; OnPropertyChanged("DomingoInicio"); }
        }

        private DateTime? domingoFin;
        public DateTime? DomingoFin
        {
            get { return domingoFin; }
            set { domingoFin = value; OnPropertyChanged("DomingoFin"); }
        }

        private bool lunes = false;
        public bool Lunes
        {
            get { return lunes; }
            set { lunes = value;
            if (value)
            {
                base.AddRule(() => LunesInicio, () => LunesInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => LunesFin, () => LunesFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("LunesInicio");
                OnPropertyChanged("LunesFin");
            }
            else
            {
                base.RemoveRule("LunesInicio");
                base.RemoveRule("LunesFin");
                OnPropertyChanged("LunesInicio");
                OnPropertyChanged("LunesFin");
            }
                OnPropertyChanged("Lunes"); }
        }

        private DateTime? lunesInicio;
        public DateTime? LunesInicio
        {
            get { return lunesInicio; }
            set { lunesInicio = value; OnPropertyChanged("LunesInicio"); }
        }
        private DateTime? lunesFin;
        public DateTime? LunesFin
        {
            get { return lunesFin; }
            set { lunesFin = value; OnPropertyChanged("LunesFin"); }
        }

        private bool martes = false;
        public bool Martes
        {
            get { return martes; }
            set { martes = value;
            if (value)
            {
                base.AddRule(() => MartesInicio, () => MartesInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => MartesFin, () => MartesFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("MartesInicio");
                OnPropertyChanged("MartesFin");
            }
            else
            {
                base.RemoveRule("MartesInicio");
                base.RemoveRule("MartesFin");
                OnPropertyChanged("MartesInicio");
                OnPropertyChanged("MartesFin");
            }
                OnPropertyChanged("Martes"); }
        }

        private DateTime? martesInicio;
        public DateTime? MartesInicio
        {
            get { return martesInicio; }
            set { martesInicio = value; OnPropertyChanged("MartesInicio"); }
        }
        private DateTime? martesFin;
        public DateTime? MartesFin
        {
            get { return martesFin; }
            set { martesFin = value; OnPropertyChanged("MartesFin"); }
        }

        private bool miercoles = false;
        public bool Miercoles
        {
            get { return miercoles; }
            set { miercoles = value;
            if (value)
            {
                base.AddRule(() => MiercolesInicio, () => MiercolesInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => MiercolesFin, () => MiercolesFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("MiercolesInicio");
                OnPropertyChanged("MiercolesFin");
            }
            else
            {
                base.RemoveRule("MiercolesInicio");
                base.RemoveRule("MiercolesFin");
                OnPropertyChanged("MiercolesInicio");
                OnPropertyChanged("MiercolesFin");
            }
                OnPropertyChanged("Miercoles"); }
        }

        private DateTime? miercolesInicio;
        public DateTime? MiercolesInicio
        {
            get { return miercolesInicio; }
            set { miercolesInicio = value; OnPropertyChanged("MiercolesInicio"); }
        }
        private DateTime? miercolesFin;
        public DateTime? MiercolesFin
        {
            get { return miercolesFin; }
            set { miercolesFin = value; OnPropertyChanged("MiercolesFin"); }
        }

        private bool jueves = false;
        public bool Jueves
        {
            get { return jueves; }
            set { jueves = value;
            if (value)
            {
                base.AddRule(() => JuevesInicio, () => JuevesInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => JuevesFin, () => JuevesFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("JuevesInicio");
                OnPropertyChanged("JuevesFin");
            }
            else
            {
                base.RemoveRule("JuevesInicio");
                base.RemoveRule("JuevesFin");
                OnPropertyChanged("JuevesInicio");
                OnPropertyChanged("JuevesFin");
            }
                OnPropertyChanged("Jueves"); }
        }

        private DateTime? juevesInicio;
        public DateTime? JuevesInicio
        {
            get { return juevesInicio; }
            set { juevesInicio = value; OnPropertyChanged("JuevesInicio"); }
        }
        private DateTime? juevesFin;
        public DateTime? JuevesFin
        {
            get { return juevesFin; }
            set { juevesFin = value; OnPropertyChanged("JuevesFin"); }
        }

        private bool viernes = false;
        public bool Viernes
        {
            get { return viernes; }
            set { viernes = value;
            if (value)
            {
                base.AddRule(() => ViernesInicio, () => ViernesInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => ViernesFin, () => ViernesFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("ViernesInicio");
                OnPropertyChanged("ViernesFin");
            }
            else
            {
                base.RemoveRule("ViernesInicio");
                base.RemoveRule("ViernesFin");
                OnPropertyChanged("ViernesInicio");
                OnPropertyChanged("ViernesFin");
            }
                OnPropertyChanged("Viernes"); }
        }

        private DateTime? viernesInicio;
        public DateTime? ViernesInicio
        {
            get { return viernesInicio; }
            set { viernesInicio = value; OnPropertyChanged("ViernesInicio"); }
        }
        private DateTime? viernesFin;
        public DateTime? ViernesFin
        {
            get { return viernesFin; }
            set { viernesFin = value; OnPropertyChanged("ViernesFin"); }
        }

        private bool sabado = false;
        public bool Sabado
        {
            get { return sabado; }
            set { sabado = value;
            if (value)
            {
                base.AddRule(() => SabadoInicio, () => SabadoInicio.HasValue, "HORA INICIO ES REQUERIDA!");
                base.AddRule(() => SabadoFin, () => SabadoFin.HasValue, "HORA FIN ES REQUERIDA!");
                OnPropertyChanged("SabadoInicio");
                OnPropertyChanged("SabadoFin");
            }
            else
            {
                base.RemoveRule("SabadoInicio");
                base.RemoveRule("SabadoFin");
                OnPropertyChanged("SabadoInicio");
                OnPropertyChanged("SabadoFin");
            }
                OnPropertyChanged("Sabado"); }
        }

        private DateTime? sabadoInicio;
        public DateTime? SabadoInicio
        {
            get { return sabadoInicio; }
            set { sabadoInicio = value; OnPropertyChanged("SabadoInicio"); }
        }
        private DateTime? sabadoFin;
        public DateTime? SabadoFin
        {
            get { return sabadoFin; }
            set { sabadoFin = value; OnPropertyChanged("SabadoFin"); }
        }
        #endregion

        #region Configuracion Permisos
        //private bool pInsertar = true;
        //public bool PInsertar
        //{
        //    get { return pInsertar; }
        //    set { pInsertar = value;
        //    if (value)
        //        MenuGuardarEnabled = value;
        //    }
        //}

        //private bool pEditar = true;
        //public bool PEditar
        //{
        //    get { return pEditar; }
        //    set { pEditar = value;}
        //}

        //private bool pConsultar = true;
        //public bool PConsultar
        //{
        //    get { return pConsultar; }
        //    set { pConsultar = value;
        //    if (value)
        //        MenuBuscarEnabled = value;
        //    }
        //}

        //private bool pImprimir = true;
        //public bool PImprimir
        //{
        //    get { return pImprimir; }
        //    set { pImprimir = value;
        //    //if (value)
        //    //    MenuReporteEnabled = value;
        //    }
        //}

        private bool menuGuardarEnabled = true;
        public bool MenuGuardarEnabled
        {
            get { return menuGuardarEnabled; }
            set { menuGuardarEnabled = value; OnPropertyChanged("MenuGuardarEnabled"); }
        }

        private bool menuBuscarEnabled = true;
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

        #region Calendario
        CalendarioView Calendario;
        #endregion

        #region Pantalla
        private Visibility _VisibleDetalle = Visibility.Collapsed;
        public Visibility VisibleDetalle
        {
            get { return _VisibleDetalle; }
            set { _VisibleDetalle = value; OnPropertyChanged(); }
        }

        private Visibility _VisibleAgenda = Visibility.Collapsed;
        public Visibility VisibleAgenda
        {
            get { return _VisibleAgenda; }
            set { _VisibleAgenda = value; OnPropertyChanged("VisibleAgenda"); }
        }

        private bool _CrearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return _CrearNuevoExpedienteEnabled; }
            set { _CrearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

       
        #endregion

        #region Oficio de Asignacion (NSJP yTradicional)
        private byte[] _OAOficio;
        public byte[] OAOficio
        {
            get { return _OAOficio; }
            set { _OAOficio = value; OnPropertyChanged("OAOficio"); }
        }

        private string _OAFuero = "C";
        public string OAFuero
        {
            get { return _OAFuero; }
            set { _OAFuero = value; OnPropertyChanged("OAFuero"); }
        }

        private DateTime? _OAFEcha;
        public DateTime? OAFEcha
        {
            get { return _OAFEcha;  }
            set { _OAFEcha = value; OnPropertyChanged("OAFEcha"); }
        }
        
        private short? _OACPAnio;
        public short? OACPAnio
        {
            get { return _OACPAnio; }
            set { _OACPAnio = value; OnPropertyChanged("OACPAnio"); }
        }

        private int? _OACPFolio;
        public int? OACPFolio
        {
            get { return _OACPFolio; }
            set { _OACPFolio = value; OnPropertyChanged("OACPFolio"); }
        }

        private string _OANUC;
        public string OANUC
        {
            get { return _OANUC; }
            set { _OANUC = value; OnPropertyChanged("OANUC"); }
        }

        private string _OAJuzgado;
        public string OAJuzgado
        {
            get { return _OAJuzgado; }
            set { _OAJuzgado = value; OnPropertyChanged("OAJuzgado"); }
        }

        private string _OADelito;
        public string OADelito
        {
            get { return _OADelito; }
            set { _OADelito = value; OnPropertyChanged("OADelito"); }
        }

        private string _OASustitucionPena;
        public string OASustitucionPena
        {
            get { return _OASustitucionPena; }
            set { _OASustitucionPena = value; OnPropertyChanged("OASustitucionPena"); }
        }

        private string _OANoJornadasLetra;
        public string OANoJornadasLetra
        {
            get { return _OANoJornadasLetra; }
            set { _OANoJornadasLetra = value; OnPropertyChanged("OANoJornadasLetra"); }
        }

        private string _OAObseervacion;
        public string OAObseervacion
        {
            get { return _OAObseervacion; }
            set { _OAObseervacion = value; OnPropertyChanged("OAObseervacion"); }
        }
        #endregion

        #region Oficio Conclusion
        private byte[] _OCOficio;
        public byte[] OCOficio
        {
            get { return _OCOficio; }
            set { _OCOficio = value; OnPropertyChanged("OCOficio"); }
        }

        private DateTime? _OCFecha;
        public DateTime? OCFecha
        {
            get { return _OCFecha; }
            set { _OCFecha = value; OnPropertyChanged("OCFecha"); }
        }

        private short? _OCCPAnio;
        public short? OCCPAnio
        {
            get { return _OCCPAnio; }
            set { _OCCPAnio = value; OnPropertyChanged("OCCPAnio"); }
        }

        private int? _OCCPFolio;
        public int? OCCPFolio
        {
            get { return _OCCPFolio; }
            set { _OCCPFolio = value; OnPropertyChanged("OCCPFolio"); }
        }

        private string _OCJuzgado;
        public string OCJuzgado
        {
            get { return _OCJuzgado; }
            set { _OCJuzgado = value; OnPropertyChanged("OCJuzgado"); }
        }

        private string _OCDelito;
        public string OCDelito
        {
            get { return _OCDelito; }
            set { _OCDelito = value; OnPropertyChanged("OCDelito"); }
        }

        private string _OCJornadasCumplidas;
        public string OCJornadasCumplidas
        {
            get { return _OCJornadasCumplidas; }
            set { _OCJornadasCumplidas = value; OnPropertyChanged("OCJornadasCumplidas"); }
        }

        private string _OCOficioConclusion;
        public string OCOficioConclusion
        {
            get { return _OCOficioConclusion; }
            set { _OCOficioConclusion = value; OnPropertyChanged("OCOficioConclusion"); }
        }

        private DateTime? _OCFechaConclusion;
        public DateTime? OCFechaConclusion
        {
            get { return _OCFechaConclusion; }
            set { _OCFechaConclusion = value; OnPropertyChanged("OCFechaConclusion"); }
        }

        private string _OCObservacion;
        public string OCObservacion
        {
            get { return _OCObservacion; }
            set { _OCObservacion = value; OnPropertyChanged("OCObservacion"); }
        }
        #endregion

        #region Oficio Baja
        private DateTime? _OBFecha;
        public DateTime? OBFecha
        {
            get { return _OBFecha; }
            set { _OBFecha = value; OnPropertyChanged("OBFecha"); }
        }
        private string _OBNombre;
        public string OBNombre
        {
            get { return _OBNombre; }
            set { _OBNombre = value; OnPropertyChanged("OBNombre"); }
        }
        private int? _OBDias;
        public int? OBDias
        {
            get { return _OBDias; }
            set { _OBDias = value;
            if (value.HasValue && OBDiasRegistrados.HasValue)
            {
                var x = value - OBDiasRegistrados;
                if (x < 0)
                    x = 0;
                OBDiasPendientes = x;
            }
            else
                OBDiasPendientes = 0;
                OnPropertyChanged("OBDias"); }
        }
        private short? _OBCPAnio;
        public short? OBCPAnio
        {
            get { return _OBCPAnio; }
            set { _OBCPAnio = value; OnPropertyChanged("OBCPAnio"); }
        }
        private int? _OBCPFolio;
        public int? OBCPFolio
        {
            get { return _OBCPFolio; }
            set { _OBCPFolio = value; OnPropertyChanged("OBCPFolio"); }
        }
        private int? _OBDiasRegistrados;
        public int? OBDiasRegistrados
        {
            get { return _OBDiasRegistrados; }
            set { _OBDiasRegistrados = value;

            if (value.HasValue && OBDias.HasValue)
            {
                var x = OBDias - value;
                if (x < 0)
                    x = 0;
                OBDiasPendientes = x;
            }
            else
                OBDiasPendientes = 0;
                OnPropertyChanged("OBDiasRegistrados"); }
        }

        private string _OBPrograma;
        public string OBPrograma
        {
            get { return _OBPrograma; }
            set { _OBPrograma = value; OnPropertyChanged("OBPrograma"); }
        }
        private int? _OBMesBaja = -1;
        public int? OBMesBaja
        {
            get { return _OBMesBaja; }
            set { _OBMesBaja = value; OnPropertyChanged("OBMesBaja"); }
        }
        private int? _OBDiasPendientes;
        public int? OBDiasPendientes
        {
            get { return _OBDiasPendientes; }
            set { _OBDiasPendientes = value; OnPropertyChanged("OBDiasPendientes"); }
        }
        private short? _OBNumeroBaja = -1;
        public short? OBNumeroBaja
        {
            get { return _OBNumeroBaja; }
            set { _OBNumeroBaja = value; OnPropertyChanged("OBNumeroBaja"); }
        }
        private string _OBObservacion;
        public string OBObservacion
        {
            get { return _OBObservacion; }
            set { _OBObservacion = value; OnPropertyChanged("OBObservacion"); }
        }
        #endregion

        #region Oficios
        private ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO> lstOficios;
        public ObservableCollection<AGENDA_LIBERTAD_DOCUMENTO> LstOficios
        {
            get { return lstOficios; }
            set { lstOficios = value;
            if (value == null)
                VisibleOficio = Visibility.Visible;
            else
            { 
                if(value.Count > 0)
                    VisibleOficio = Visibility.Collapsed;
                else
                    VisibleOficio = Visibility.Visible;
            }
                OnPropertyChanged("LstOficios"); }
        }

        private AGENDA_LIBERTAD_DOCUMENTO selectedOficio;
        public AGENDA_LIBERTAD_DOCUMENTO SelectedOficio
        {
            get { return selectedOficio; }
            set { selectedOficio = value; OnPropertyChanged("SelectedOficio"); }
        }

        private Visibility visibleOficio = Visibility.Collapsed;
        public Visibility VisibleOficio
        {
            get { return visibleOficio; }
            set { visibleOficio = value; OnPropertyChanged("visibleOficio"); }
        }
   
        #endregion

        #region Reporte
        private bool oficio_Asignacion = false;
        public bool Oficio_Asignacion
        {
            get { return oficio_Asignacion; }
            set { oficio_Asignacion = value; OnPropertyChanged("Oficio_Asignacion"); }
        }

        private bool oficio_Cumplimiento = false;
        public bool Oficio_Cumplimiento
        {
            get { return oficio_Cumplimiento; }
            set { oficio_Cumplimiento = value; OnPropertyChanged("Oficio_Cumplimiento"); }
        }

        private bool oficio_Jornadas_Pendientes = false;
        public bool Oficio_Jornadas_Pendientes
        {
            get { return oficio_Jornadas_Pendientes; }
            set { oficio_Jornadas_Pendientes = value; OnPropertyChanged("Oficio_Jornadas_Pendientes"); }
        }

        private bool oficio_Conclusion = false;
        public bool Oficio_Conclusion
        {
            get { return oficio_Conclusion; }
            set { oficio_Conclusion = value; OnPropertyChanged("Oficio_Conclusion"); }
        }

        private bool oficio_Baja = false;
        public bool Oficio_Baja
        {
            get { return oficio_Baja; }
            set { oficio_Baja = value; OnPropertyChanged("Oficio_Baja"); }
        }
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

        #region Filtros
        private bool porNUC = true;
        public bool PorNUC
        {
            get { return porNUC; }
            set { porNUC = value;
            if (value)
            {
                PorCP = false;
                PorNUCVisible = Visibility.Visible;
                PorCPVisible = Visibility.Collapsed;
                AnioBuscar = null;
                FolioBuscar = null;
            }
            else
            {
                PorCP = true;
                PorNUCVisible = Visibility.Collapsed;
                PorCPVisible = Visibility.Visible;
                NUCBuscar = string.Empty;
            }
                OnPropertyChanged("PorNUC"); }
        }

        private bool porCP = false;
        public bool PorCP
        {
            get { return porCP; }
            set { porCP = value; OnPropertyChanged("PorCP"); }
        }
        
        private Visibility porNUCVisible = Visibility.Visible;
        public Visibility PorNUCVisible
        {
            get { return porNUCVisible; }
            set { porNUCVisible = value; OnPropertyChanged("PorNUCVisible"); }
        }

        private Visibility porCPVisible = Visibility.Collapsed;
        public Visibility PorCPVisible
        {
            get { return porCPVisible; }
            set { porCPVisible = value; OnPropertyChanged("PorCPVisible"); }
        }
        #endregion

    }

    public class cSeguimientoDetalle
    {
        public short DIA { set; get; }
        public DateTime? HORAINICIO { set; get; }
        public DateTime? HORAFIN { set; get; }
    }
}
