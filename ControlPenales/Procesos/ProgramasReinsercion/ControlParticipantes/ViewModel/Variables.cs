using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class ControlParticipantesViewModel : ValidationViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se definen las variables a utilizar en el modulo control participante
         * 
         */

        #region [Control Participante]
        private int? _AnioBuscar;
        public int? AnioBuscar
        {
            get { return _AnioBuscar; }
            set
            {
                _AnioBuscar = value;
                OnPropertyChanged("AnioBuscar");
            }
        }

        private int? _FolioBuscar;
        public int? FolioBuscar
        {
            get { return _FolioBuscar; }
            set
            {
                _FolioBuscar = value;
                OnPropertyChanged("FolioBuscar");
            }
        }
        private string _PaternoD;
        public string PaternoD
        {
            get
            {
                return _PaternoD;
            }
            set
            {
                _PaternoD = value;
                OnPropertyChanged("PaternoD");
            }
        }
        private string _MaternoD;
        public string MaternoD
        {
            get
            {
                return _MaternoD;
            }
            set
            {
                _MaternoD = value;
                OnPropertyChanged("MaternoD");
            }
        }
        private string _NombreD;
        public string NombreD
        {
            get
            {
                return _NombreD;
            }
            set
            {
                _NombreD = value;
                OnPropertyChanged("NombreD");
            }
        }

        private string _TextAnio;
        public string TextAnio
        {
            get { return _TextAnio; }
            set { _TextAnio = value; OnPropertyChanged("TextAnio"); }
        }
        private string _TextFolio;
        public string TextFolio
        {
            get { return _TextFolio; }
            set { _TextFolio = value; OnPropertyChanged("TextFolio"); }
        }
        private string _TextPaternoImputado;
        public string TextPaternoImputado
        {
            get { return _TextPaternoImputado; }
            set { _TextPaternoImputado = value; OnPropertyChanged("TextPaternoImputado"); }
        }
        private string _TextMaternoImputado;
        public string TextMaternoImputado
        {
            get { return _TextMaternoImputado; }
            set { _TextMaternoImputado = value; OnPropertyChanged("TextMaternoImputado"); }
        }
        private string _TextNombreImputado;
        public string TextNombreImputado
        {
            get { return _TextNombreImputado; }
            set { _TextNombreImputado = value; OnPropertyChanged("TextNombreImputado"); }
        }

        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }

        private string textBotonSeleccionarIngreso = "Seleccionar Ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }

        private bool crearNuevoExpedienteEnabled = false;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }

        private bool selectIngresoEnabled = true;
        public bool SelectIngresoEnabled
        {
            get { return selectIngresoEnabled; }
            set { selectIngresoEnabled = value; OnPropertyChanged("SelectIngresoEnabled"); }
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

        private byte[] imagenIngreso = new Imagenes().getImagenPerson();
        public byte[] ImagenIngreso
        {
            get
            {
                return imagenIngreso;
            }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
                OnPropertyChanged("ImagenParticipante");
            }
        }
        private byte[] imagenImputado = new Imagenes().getImagenPerson();
        public byte[] ImagenImputado
        {
            get
            {
                return imagenImputado;

            }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
                OnPropertyChanged("ImagenParticipante");
            }
        }
        public byte[] ImagenParticipante
        {
            get
            {
                return imagenIngreso.Length != new Imagenes().getImagenPerson().Length ? imagenIngreso : imagenImputado.Length != new Imagenes().getImagenPerson().Length ? imagenImputado : new Imagenes().getImagenPerson();
            }
        }

        private byte[] _ImagenAcompanante = new Imagenes().getImagenPerson();
        public byte[] ImagenAcompanante
        {
            get
            {
                return _ImagenAcompanante;
            }
            set
            {
                _ImagenAcompanante = value;
                OnPropertyChanged("ImagenAcompanante");
            }
        }

        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }

        private ObservableCollection<IMPUTADO> listExpedientePadron;
        public ObservableCollection<IMPUTADO> ListExpedientePadron
        {
            get { return listExpedientePadron; }
            set { listExpedientePadron = value; }
        }
        private ObservableCollection<IMPUTADO> listExpedienteAsignacion;
        public ObservableCollection<IMPUTADO> ListExpedienteAsignacion
        {
            get { return listExpedienteAsignacion; }
            set { listExpedienteAsignacion = value; }
        }
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                if (value.Count > 0)
                {
                    SelectExpediente = value.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                }
                OnPropertyChanged("ListExpediente");
            }
        }

        private IMPUTADO selectExpediente;
        private IMPUTADO SelectExpedienteAuxiliar;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (value != null)
                {
                    //MUESTRA LOS INGRESOS
                    if (value.INGRESO!=null && value.INGRESO.Count > 0)
                    {
                        EmptyIngresoVisible = false;
                        SelectIngreso = value.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    }
                    else
                        EmptyIngresoVisible = true;

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

        public INGRESO selectIngreso { get; set; }
        private INGRESO SelectIngresoAuxiliar;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                selectIngreso = value;
                if (value == null)
                {
                    ImagenImputado = new Imagenes().getImagenPerson();
                    OnPropertyChanged("SelectIngreso");
                    return;
                }

                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();

                if (value.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                    ImagenIngreso = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();
                OnPropertyChanged("SelectIngreso");
            }
        }
        private bool aceptarBusquedaHuellaFocus;
        public bool AceptarBusquedaHuellaFocus
        {
            get { return aceptarBusquedaHuellaFocus; }
            set { aceptarBusquedaHuellaFocus = value; OnPropertyChanged("AceptarBusquedaHuellaFocus"); }
        }

        //VARIABLES SEGMENTACION 
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        string _TextUbicacion;
        public string TextUbicacion
        {
            get { return _TextUbicacion; }
            set
            {
                _TextUbicacion = value;
                OnPropertyChanged("TextUbicacion");
            }
        }
        string _TextPlanimetria;
        public string TextPlanimetria
        {
            get { return _TextPlanimetria; }
            set
            {
                _TextPlanimetria = value;
                OnPropertyChanged("TextPlanimetria");
            }
        }
        string _Planimetriacolor;
        public string Planimetriacolor
        {
            get { return _Planimetriacolor; }
            set
            {
                _Planimetriacolor = value;
                OnPropertyChanged("Planimetriacolor");
            }
        }
        string _TextSentenciaRes;
        public string TextSentenciaRes
        {
            get { return _TextSentenciaRes; }
            set
            {
                _TextSentenciaRes = value;
                OnPropertyChanged("TextSentenciaRes");
            }
        }
        string _TextEstatus;
        public string TextEstatus
        {
            get { return _TextEstatus; }
            set
            {
                _TextEstatus = value;
                OnPropertyChanged("TextEstatus");
            }
        }
        string _AvanceTratamiento = "0/0";
        public string AvanceTratamiento
        {
            get { return _AvanceTratamiento; }
            set
            {
                _AvanceTratamiento = value;
                OnPropertyChanged("AvanceTratamiento");
            }
        }
        int _MaxValueProBar = 1;
        public int MaxValueProBar
        {
            get { return _MaxValueProBar; }
            set
            {
                _MaxValueProBar = value;
                OnPropertyChanged("MaxValueProBar");
            }
        }
        int _CantidadActividadesAprovadas = 0;
        public int CantidadActividadesAprovadas
        {
            get { return _CantidadActividadesAprovadas; }
            set
            {
                _CantidadActividadesAprovadas = value;
                OnPropertyChanged("CantidadActividadesAprovadas");
            }
        }

        string _HorasTratamiento = "0/0";
        public string HorasTratamiento
        {
            get { return _HorasTratamiento; }
            set
            {
                _HorasTratamiento = value;
                OnPropertyChanged("HorasTratamiento");
            }
        }

        private ObservableCollection<Nota_Tecnica> _ListNotasTecnicas;
        public ObservableCollection<Nota_Tecnica> ListNotasTecnicas
        {
            get { return _ListNotasTecnicas; }
            set
            {
                _ListNotasTecnicas = value;
                OnPropertyChanged("ListNotasTecnicas");
            }
        }

        private ObservableCollection<HorarioParticipante> _ListHorario;
        public ObservableCollection<HorarioParticipante> ListHorario
        {
            get { return _ListHorario; }
            set
            {
                _ListHorario = value;
                OnPropertyChanged("ListHorario");
            }
        }

        private ObservableCollection<EJE> _ListaEjes;
        public ObservableCollection<EJE> ListaEjes
        {
            get { return _ListaEjes; }
            set
            {
                _ListaEjes = value;
                OnPropertyChanged("ListaEjes");
            }
        }

        private EJE _SelectedEje;
        public EJE SelectedEje
        {
            get { return _SelectedEje; }
            set
            {
                _SelectedEje = value;
                OnPropertyChanged("SelectedEje");

                if (value != null)
                    CargarNotasTecnicas(SelectIngreso.GRUPO_PARTICIPANTE, value);
                else
                    ListNotasTecnicas = null;
            }
        }

        private ObservableCollection<GRUPO> _ListaGrupo;
        public ObservableCollection<GRUPO> ListaGrupo
        {
            get { return _ListaGrupo; }
            set
            {
                _ListaGrupo = value;
                OnPropertyChanged("ListaGrupo");
            }
        }

        private GRUPO _SelectedGrupo;
        public GRUPO SelectedGrupo
        {
            get { return _SelectedGrupo; }
            set
            {
                try
                {
                    SelectedGrupoEvolucion = null;
                    _SelectedGrupo = value;
                    OnPropertyChanged("SelectedGrupo");

                    if (value != null)
                    {
                        InicioGrupo = value.GRUPO_HORARIO.Any() ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty;
                        FinGrupo = value.GRUPO_HORARIO.Any() ? value.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty;
                        CargarHorarioParticipante(SelectIngreso.GRUPO_PARTICIPANTE, value);
                        AsistenciasGrupo = ListHorario.Any() ? ListHorario.Where(w => w.ASISTENCIA.Value).Count().ToString() : string.Empty;
                        FaltasGrupo = ListHorario.Any() ? ListHorario.Where(w => !w.ASISTENCIA.Value && w.FechaHorario < FechaServer).Count().ToString() : string.Empty;
                        JustificadasGrupo = ListHorario.Any() ? ListHorario.Where(w => w.ShowLabel == Visibility.Visible).Count().ToString() : string.Empty;
                    }
                    else
                    {
                        InicioGrupo = string.Empty;
                        FinGrupo = string.Empty;
                        ListHorario = null;
                        AsistenciasGrupo = string.Empty;
                        FaltasGrupo = string.Empty;
                        JustificadasGrupo = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al elejir grupo", ex);
                }
            }
        }

        string _InicioGrupo;
        public string InicioGrupo
        {
            get { return _InicioGrupo; }
            set
            {
                _InicioGrupo = value;
                OnPropertyChanged("InicioGrupo");
            }
        }

        string _FinGrupo;
        public string FinGrupo
        {
            get { return _FinGrupo; }
            set
            {
                _FinGrupo = value;
                OnPropertyChanged("FinGrupo");
            }
        }

        Nota_Tecnica _SelectedGrupoEvolucion = new Nota_Tecnica();
        public Nota_Tecnica SelectedGrupoEvolucion
        {
            get { return _SelectedGrupoEvolucion; }
            set
            {
                try
                {
                    if (_SelectedGrupoEvolucion == value)
                        return;
                    if (_SelectedGrupoEvolucion == null)
                        return;
                    if (null == value)
                        return;

                    _SelectedGrupoEvolucion = value;
                    OnPropertyChanged("SelectedGrupoEvolucion");

                    if (value != null)
                        SelectedGrupo = value.EntityGRUPO;
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al elejir grupo", ex);
                }
            }
        }

        private ObservableCollection<EmpalmeParticipante> _ListEmpalme;
        public ObservableCollection<EmpalmeParticipante> ListEmpalme
        {
            get { return _ListEmpalme; }
            set
            {
                _ListEmpalme = value;
                OnPropertyChanged("ListEmpalme");
            }
        }

        string _AsistenciasGrupo;
        public string AsistenciasGrupo
        {
            get { return _AsistenciasGrupo; }
            set
            {
                _AsistenciasGrupo = value;
                OnPropertyChanged("AsistenciasGrupo");
            }
        }
        string _FaltasGrupo;
        public string FaltasGrupo
        {
            get { return _FaltasGrupo; }
            set
            {
                _FaltasGrupo = value;
                OnPropertyChanged("FaltasGrupo");
            }
        }
        string _JustificadasGrupo;
        public string JustificadasGrupo
        {
            get { return _JustificadasGrupo; }
            set
            {
                _JustificadasGrupo = value;
                OnPropertyChanged("JustificadasGrupo");
            }
        }

        string _TextSentencia;
        public string TextSentencia
        {
            get { return _TextSentencia; }
            set
            {
                _TextSentencia = value;
                OnPropertyChanged("TextSentencia");
            }
        }
        #endregion

        #region [Agregar Participante Grupo]
        ObservableCollection<EJE> _AgregarListEje;
        public ObservableCollection<EJE> AgregarListEje
        {
            get { return _AgregarListEje; }
            set
            {
                _AgregarListEje = value;
                OnPropertyChanged("AgregarListEje");
            }
        }
        ObservableCollection<ListaActividad> _ListActividadParticipante;
        public ObservableCollection<ListaActividad> ListActividadParticipante
        {
            get { return _ListActividadParticipante; }
            set
            {
                _ListActividadParticipante = value;
                OnPropertyChanged("ListActividadParticipante");
            }
        }

        EJE _AgregarGrupoSelectedEje;
        public EJE AgregarGrupoSelectedEje
        {
            get { return _AgregarGrupoSelectedEje; }
            set
            {
                if (value != null)
                    _AgregarGrupoSelectedEje = value.ID_EJE != 0 ? value : null;
                else
                    _AgregarGrupoSelectedEje = value;
                OnPropertyChanged("AgregarGrupoSelectedEje");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        AgregarListPrograma = _AgregarGrupoSelectedEje != null ? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupo().GetData().Where(w => w.ACTIVIDAD.ACTIVIDAD_EJE.Where(wh => wh.ID_EJE == _AgregarGrupoSelectedEje.ID_EJE).Any()).Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.ORDEN))) : new ObservableCollection<TIPO_PROGRAMA>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener programas", ex);
                    }
                }));
            }
        }

        EJE _AgregarFechaSelectedEje;
        public EJE AgregarFechaSelectedEje
        {
            get { return _AgregarFechaSelectedEje; }
            set
            {
                if (value != null)
                    _AgregarFechaSelectedEje = value.ID_EJE != 0 ? value : null;
                else
                    _AgregarFechaSelectedEje = value;
                OnPropertyChanged("AgregarFechaSelectedEje");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        AgregarListPrograma = _AgregarFechaSelectedEje != null ? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupoParticipante().GetData().Where(w => w.EJE1.ID_EJE == _AgregarFechaSelectedEje.ID_EJE && (w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)).Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.ORDEN))) : new ObservableCollection<TIPO_PROGRAMA>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener programas", ex);
                    }
                }));
            }
        }

        ObservableCollection<TIPO_PROGRAMA> _AgregarListPrograma;
        public ObservableCollection<TIPO_PROGRAMA> AgregarListPrograma
        {
            get { return _AgregarListPrograma; }
            set
            {
                _AgregarListPrograma = value;
                OnPropertyChanged("AgregarListPrograma");
            }
        }

        TIPO_PROGRAMA _AgregarGrupoSelectedPrograma;
        public TIPO_PROGRAMA AgregarGrupoSelectedPrograma
        {
            get { return _AgregarGrupoSelectedPrograma; }
            set
            {
                _AgregarGrupoSelectedPrograma = value;
                OnPropertyChanged("AgregarGrupoSelectedPrograma");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        AgregarListActividad = _AgregarGrupoSelectedPrograma != null ? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupo().GetData().Where(w => w.ID_TIPO_PROGRAMA == _AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.ORDEN))) : new ObservableCollection<ACTIVIDAD>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener actividades", ex);
                    }
                }));
            }
        }
        TIPO_PROGRAMA _AgregarFechaSelectedPrograma;
        public TIPO_PROGRAMA AgregarFechaSelectedPrograma
        {
            get { return _AgregarFechaSelectedPrograma; }
            set
            {
                _AgregarFechaSelectedPrograma = value;
                OnPropertyChanged("AgregarFechaSelectedPrograma");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        AgregarListActividad = _AgregarFechaSelectedPrograma != null ? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ACTIVIDAD>>(() => new ObservableCollection<ACTIVIDAD>(new cGrupoParticipante().GetData().Where(w => w.ID_TIPO_PROGRAMA == _AgregarFechaSelectedPrograma.ID_TIPO_PROGRAMA && w.EJE == AgregarFechaSelectedEje.ID_EJE && (w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)).Select(s => s.ACTIVIDAD).Distinct().OrderBy(o => o.ORDEN))) : new ObservableCollection<ACTIVIDAD>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener actividades", ex);
                    }
                }));
            }
        }

        ObservableCollection<ACTIVIDAD> _AgregarListActividad;
        public ObservableCollection<ACTIVIDAD> AgregarListActividad
        {
            get { return _AgregarListActividad; }
            set
            {
                _AgregarListActividad = value;
                OnPropertyChanged("AgregarListActividad");
            }
        }

        ACTIVIDAD _AgregarGrupoSelectedActividad;
        public ACTIVIDAD AgregarGrupoSelectedActividad
        {
            get { return _AgregarGrupoSelectedActividad; }
            set
            {
                AgregarEnableGrupo = true;
                AgregarInfo = string.Empty;
                _AgregarGrupoSelectedActividad = value;
                OnPropertyChanged("AgregarGrupoSelectedActividad");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                if (_AgregarGrupoSelectedEje != null && _AgregarGrupoSelectedPrograma != null && _AgregarGrupoSelectedActividad != null)
                                {
                                    if (new cGrupoParticipante().GetData().Where(w => (w.EJE == _AgregarGrupoSelectedEje.ID_EJE && w.ID_TIPO_PROGRAMA == _AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == _AgregarGrupoSelectedActividad.ID_ACTIVIDAD) && (w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO) && w.ID_GRUPO != null).Any())
                                    {
                                        AgregarEnableGrupo = false;
                                        AgregarInfo = "Ya Esta Integrado A Esta Actividad";
                                        AgregarGrupoSelectedGrupo = null;
                                        return;
                                    }
                                    if (_AgregarGrupoSelectedEje.COMPLEMENTARIO.Equals("N"))
                                        if (!new cGrupoParticipante().GetData().Where(w => (w.EJE == _AgregarGrupoSelectedEje.ID_EJE && w.ID_TIPO_PROGRAMA == _AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == _AgregarGrupoSelectedActividad.ID_ACTIVIDAD) && (w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)).Any())
                                        {
                                            AgregarEnableGrupo = false;
                                            AgregarInfo = "No Puede Ser Integrado A Esta Actividad";
                                            AgregarGrupoSelectedGrupo = null;
                                            return;
                                        }
                                }
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    try
                                    {
                                        AgregarListGrupo = _AgregarGrupoSelectedActividad != null ? new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_TIPO_PROGRAMA == _AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == _AgregarGrupoSelectedActividad.ID_ACTIVIDAD && w.ID_ESTATUS_GRUPO == 1)) : new ObservableCollection<GRUPO>();
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                    }
                }));
            }
        }
        ACTIVIDAD _AgregarFechaSelectedActividad;
        public ACTIVIDAD AgregarFechaSelectedActividad
        {
            get { return _AgregarFechaSelectedActividad; }
            set
            {
                _AgregarFechaSelectedActividad = value;
                OnPropertyChanged("AgregarFechaSelectedActividad");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    try
                                    {
                                        AgregarListGrupo = _AgregarFechaSelectedActividad != null ? new ObservableCollection<GRUPO>(new cGrupoParticipante().GetData().Where(w => w.EJE == AgregarFechaSelectedEje.ID_EJE && w.GRUPO.ID_TIPO_PROGRAMA == _AgregarFechaSelectedPrograma.ID_TIPO_PROGRAMA && w.GRUPO.ID_ACTIVIDAD == _AgregarFechaSelectedActividad.ID_ACTIVIDAD && (w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO) && w.GRUPO.ID_ESTATUS_GRUPO == 1).Select(s => s.GRUPO)) : new ObservableCollection<GRUPO>();
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                    }
                }));
            }
        }

        ObservableCollection<GRUPO> _AgregarListGrupo;
        public ObservableCollection<GRUPO> AgregarListGrupo
        {
            get { return _AgregarListGrupo; }
            set
            {
                _AgregarListGrupo = value;
                OnPropertyChanged("AgregarListGrupo");
            }
        }

        GRUPO _AgregarGrupoSelectedGrupo;
        public GRUPO AgregarGrupoSelectedGrupo
        {
            get { return _AgregarGrupoSelectedGrupo; }
            set
            {
                _AgregarGrupoSelectedGrupo = value;
                OnPropertyChanged("AgregarGrupoSelectedGrupo");

                RevisarHorarioCompatible();
            }
        }
        GRUPO _AgregarFechaSelectedGrupo;
        public GRUPO AgregarFechaSelectedGrupo
        {
            get { return _AgregarFechaSelectedGrupo; }
            set
            {
                _AgregarFechaSelectedGrupo = value;
                OnPropertyChanged("AgregarFechaSelectedGrupo");
            }
        }

        ObservableCollection<GRUPO_HORARIO_ESTATUS> _AgregarListEstatusGrupoHorario;
        public ObservableCollection<GRUPO_HORARIO_ESTATUS> AgregarListEstatusGrupoHorario
        {
            get { return _AgregarListEstatusGrupoHorario; }
            set
            {
                _AgregarListEstatusGrupoHorario = value;
                OnPropertyChanged("AgregarListEstatusGrupoHorario");
            }
        }

        GRUPO_HORARIO_ESTATUS _AgregarSelectedEstatusGrupoHorario;
        public GRUPO_HORARIO_ESTATUS AgregarSelectedEstatusGrupoHorario
        {
            get { return _AgregarSelectedEstatusGrupoHorario; }
            set
            {
                _AgregarSelectedEstatusGrupoHorario = value;
                OnPropertyChanged("AgregarSelectedEstatusGrupoHorario");
            }
        }

        bool _AgregarEnableGrupo;
        public bool AgregarEnableGrupo
        {
            get { return _AgregarEnableGrupo; }
            set
            {
                _AgregarEnableGrupo = value;
                OnPropertyChanged("AgregarEnableGrupo");
            }
        }

        string _AgregarInfo;
        public string AgregarInfo
        {
            get { return _AgregarInfo; }
            set
            {
                _AgregarInfo = value;
                OnPropertyChanged("AgregarInfo");
            }
        }

        DateTime? _AgregarFecha;
        public DateTime? AgregarFecha
        {
            get { return _AgregarFecha; }
            set
            {
                _AgregarFecha = value;
                OnPropertyChanged("AgregarFecha");
                RevisarFechasParticipante();
            }
        }
        DateTime _FechaServer;
        public DateTime FechaServer
        {
            get { return Fechas.GetFechaDateServer; }
            set
            {
                _FechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        DateTime? _InicioDia;
        public DateTime? InicioDia
        {
            get { return _InicioDia; }
            set
            {
                _InicioDia = value;
                OnPropertyChanged("InicioDia");
                if (value != null)
                    TerminoDia = value.Value.AddHours(1);
                RevisarFechasParticipante();
            }
        }

        DateTime? _TerminoDia;
        public DateTime? TerminoDia
        {
            get { return _TerminoDia; }
            set
            {
                _TerminoDia = value;
                OnPropertyChanged("TerminoDia");
                RevisarFechasParticipante();
            }
        }
        #endregion

        #region [Agregar Sancion Cancelacion/Suspender]
        ObservableCollection<GruposCancelarSuspender> _ListGruposInterno;
        public ObservableCollection<GruposCancelarSuspender> ListGruposInterno
        {
            get { return _ListGruposInterno; }
            set
            {
                _ListGruposInterno = value;
                OnPropertyChanged("ListGruposInterno");
            }
        }
        #endregion
       
        #region [Solicitar Cita]
        private ObservableCollection<AREA> lstAreaTraslado;
        public ObservableCollection<AREA> LstAreaTraslado
        {
            get { return lstAreaTraslado; }
            set { lstAreaTraslado = value; OnPropertyChanged("LstAreaTraslado"); }
        }
        private ObservableCollection<AREA_TECNICA> lstAreaTecnica;
        public ObservableCollection<AREA_TECNICA> LstAreaTecnica
        {
            get { return lstAreaTecnica; }
            set { lstAreaTecnica = value; OnPropertyChanged("LstAreaTraslado"); }
        }
        private short? cAreaTraslado = -1;
        public short? CAreaTraslado
        {
            get { return cAreaTraslado; }
            set { cAreaTraslado = value; OnPropertyChanged("CAreaTraslado"); }
        }
        private string cActividad;
        public string CActividad
        {
            get { return cActividad; }
            set { cActividad = value; OnPropertyChanged("CActividad"); }
        }
        private short? cAreaSolicita = -1;
        public short? CAreaSolicita
        {
            get { return cAreaSolicita; }
            set { cAreaSolicita = value; OnPropertyChanged("CAreaSolicita"); }
        }
        //private string cAutorizacion;
        //public string CAutorizacion
        //{
        //    get { return cAutorizacion; }
        //    set { cAutorizacion = value; OnPropertyChanged("CAutorizacion"); }
        //}
        //private string cOficialTraslado;
        //public string COficialTraslado
        //{
        //    get { return cOficialTraslado; }
        //    set { cOficialTraslado = value; OnPropertyChanged("COficialTraslado"); }
        //}

        private int VCitasMes = Parametro.SOLICITUD_ATENCION_POR_MES;
        #endregion

        #region [CONFIGURACION PERMISOS]
        //private bool _groupBoxGeneralesEnabled;
        //public bool GroupBoxGeneralesEnabled
        //{
        //    get { return _groupBoxGeneralesEnabled; }
        //    set { _groupBoxGeneralesEnabled = value; OnPropertyChanged("GroupBoxGeneralesEnabled"); }
        //}

        //private bool _editarMenuEnabled;
        //public bool EditarMenuEnabled
        //{
        //    get { return _editarMenuEnabled; }
        //    set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        //}

        //private bool _agregarMenuEnabled;
        //public bool AgregarMenuEnabled
        //{
        //    get { return _agregarMenuEnabled; }
        //    set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        //}

        //private bool _ejeEnabled;
        //public bool EjeEnabled
        //{
        //    get { return _ejeEnabled; }
        //    set { _ejeEnabled = value; OnPropertyChanged("EjeEnabled"); }
        //}

        //private bool _eliminarMenuEnabled;
        //public bool EliminarMenuEnabled
        //{
        //    get { return _eliminarMenuEnabled; }
        //    set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        //}

        //private bool _grupoEnabled;
        //public bool GrupoEnabled
        //{
        //    get { return _grupoEnabled; }
        //    set { _grupoEnabled = value; OnPropertyChanged("GrupoEnabled"); }
        //}
        #endregion
    }

    public class Nota_Tecnica
    {
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public GRUPO EntityGRUPO { get; set; }
        public string INICIO { get; set; }
        public string FIN { get; set; }
        public string ASISTENCIA { get; set; }
        public string NOTA { get; set; }
        public string ACREDITADO { get; set; }
    }

    public class HorarioParticipante
    {
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public string FECHA { get; set; }
        public string HORARIO { get; set; }
        public bool? ASISTENCIA { get; set; }
        public string ESTATUS { get; set; }
        public DateTime? FechaHorario { get; set; }

        public Visibility ShowCheck { get; set; }
        public Visibility ShowLabel { get; set; }
    }

    public class GruposCancelarSuspender
    {
        public GRUPO_PARTICIPANTE Entity { get; set; }
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS> ListEstatusGrupoParticipante { get; set; }
        public short? SelectEstatus { get; set; }
        public string MOTIVO { get; set; }
    }

    public class EmpalmeParticipante
    {
        public string HEADEREXPANDER { get; set; }
        public List<ListaEmpalmes> ListHorario { get; set; }
    }

    public class ListaEmpalmes
    {
        public string EJE { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string GRUPO { get; set; }
        public string HORARIO { get; set; }
        public bool ELEGIDA { get; set; }
    }
}
