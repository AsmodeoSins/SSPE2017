using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    partial class CreacionGruposViewModel
    {
        /* [descripcion de clase]
         * clase donde se definen las propiedades para el modulo de creacin de grupo
         * 
         * tener encuenta que hay propiedades donde su ejecucion es atemporal, practicamente las propiedades de fecha o boleanas bindiadas a un checkbox
         * 
         * generalmente si se presentara un problema seria a razon de esta propiedad "ListActividadParticipante" o "ListActividadParticipanteCompl" ya que es la que guarda los empalmes encontrados
         * 
         */

        #region [GENERALES]
        ObservableCollection<AREA> _ListArea;
        public ObservableCollection<AREA> ListArea
        {
            get { return _ListArea; }
            set
            {
                _ListArea = value;
                OnPropertyChanged("ListArea");
            }
        }
        ObservableCollection<NombreEmpleado> _ListResponsable;
        public ObservableCollection<NombreEmpleado> ListResponsable
        {
            get { return _ListResponsable; }
            set
            {
                _ListResponsable = value;
                OnPropertyChanged("ListResponsable");
            }
        }
        ObservableCollection<GRUPO_ESTATUS> _ListEstatus;
        public ObservableCollection<GRUPO_ESTATUS> ListEstatus
        {
            get { return _ListEstatus; }
            set
            {
                _ListEstatus = value;
                OnPropertyChanged("ListEstatus");
            }
        }

        DateTime _FechaServer;
        public DateTime FechaServer
        {
            get { return _FechaServer = Fechas.GetFechaDateServer; }
            set
            {
                _FechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        string _MotivoText;
        public string MotivoText
        {
            get { return _MotivoText; }
            set
            {
                _MotivoText = value;
                OnPropertyChanged("MotivoText");
            }
        }

        bool isMotivoShow;
        bool isGuardar;
        string varauxSentencia;
        #endregion

        #region [GRUPO]
        ObservableCollection<EJE> _ListEjes;
        public ObservableCollection<EJE> ListEjes
        {
            get { return _ListEjes; }
            set
            {
                _ListEjes = value;
                OnPropertyChanged("ListEjes");
            }
        }
        ObservableCollection<TIPO_PROGRAMA> _ListProgramas;
        public ObservableCollection<TIPO_PROGRAMA> ListProgramas
        {
            get { return _ListProgramas; }
            set
            {
                _ListProgramas = value;
                OnPropertyChanged("ListProgramas");
            }
        }
        ObservableCollection<ACTIVIDAD> _ListActividades;
        public ObservableCollection<ACTIVIDAD> ListActividades
        {
            get { return _ListActividades; }
            set
            {
                _ListActividades = value;
                OnPropertyChanged("ListActividades");
            }
        }
        ObservableCollection<ListaInternos> _ListGrupoParticipante;
        public ObservableCollection<ListaInternos> ListGrupoParticipante
        {
            get { return _ListGrupoParticipante; }
            set
            {
                if (value == null)
                    EmpalmeHorarioNuevo = 0;
                _ListGrupoParticipante = value;
                OnPropertyChanged("ListGrupoParticipante");
            }
        }
        ObservableCollection<GRUPO> _ListGrupo;
        public ObservableCollection<GRUPO> ListGrupo
        {
            get { return _ListGrupo; }
            set
            {
                _ListGrupo = value;
                OnPropertyChanged("ListGrupo");
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
        ObservableCollection<GRUPO_HORARIO> ListGrupoHorario;

        short? _SelectedEje;
        public short? SelectedEje
        {
            get { return _SelectedEje; }
            set
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                            {
                                OnPropertyChanged("SelectedEje");
                                return;
                            }
                        }

                        _SelectedEje = value;
                        OnPropertyChanged("SelectedEje");

                        SelectedCount = 0;
                        if (value.HasValue)
                            ProgramasLoad(value.Value);
                        else
                            ListProgramas = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar eje", ex);
                    }
                }));
            }
        }
        short? _SelectedPrograma;
        public short? SelectedPrograma
        {
            get { return _SelectedPrograma; }
            set
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
               {
                   try
                   {
                       if (StaticSourcesViewModel.SourceChanged)
                       {
                           if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                               StaticSourcesViewModel.SourceChanged = false;
                           else
                           {
                               OnPropertyChanged("SelectedPrograma");
                               return;
                           }
                       }
                       _SelectedPrograma = value;
                       OnPropertyChanged("SelectedPrograma");

                       SelectedCount = 0;
                       if (value.HasValue)
                           ActividadesLoad(value.Value);
                       else
                           ListActividades = null;
                   }
                   catch (Exception ex)
                   {
                       StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar programa", ex);
                   }
               }));
            }
        }
        short? _SelectedActividad;
        public short? SelectedActividad
        {
            get { return _SelectedActividad; }
            set
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                            {
                                OnPropertyChanged("SelectedActividad");
                                return;
                            }
                        }
                        SelectedCount = 0;
                        LimpiarCampos();
                        _SelectedActividad = value;
                        OnPropertyChanged("SelectedActividad");

                        if (value.HasValue)
                        {
                            SelectedCountText = string.Empty;
                            IsEnabledCrearGrupo = false;
                            EnabledPanelCrearGrupo = true;
                            if (SelectedGrupo == null)
                                if (PanelUpdate == Visibility.Collapsed)
                                    EnabledPanelCrearGrupo = false;
                            var configActividad = new cActividad().GetData().Where(w => w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == value).FirstOrDefault();

                            if (configActividad != null)
                            {
                                OCUPANTE_MAX = configActividad.OCUPANTE_MAX;
                                OCUPANTE_MIN = configActividad.OCUPANTE_MIN;
                                SelectedCountText = "Minimo de " + OCUPANTE_MIN + ", 0/" + OCUPANTE_MAX + " Seleccionados";
                            }
                            if (PanelUpdate != Visibility.Collapsed)
                                ListaInternosLoad(value.Value);
                            else
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    try
                                    {
                                        ListGrupo = new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_EJE == SelectedEje && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == value.Value && (w.ID_ESTATUS_GRUPO == 1 || w.ID_ESTATUS_GRUPO == 4)).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList());
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener grupos", ex);
                                    }
                                });
                        }
                        else
                        {
                            EnabledPanelCrearGrupo = false;
                            SelectedCountText = "Minimo de , 0/ Seleccionados";
                            ListGrupoParticipante = null;
                            ListGrupo = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar actividad", ex);
                    }
                }));
            }
        }
        short? _SelectedEstatus;
        public short? SelectedEstatus
        {
            get { return _SelectedEstatus; }
            set
            {
                _SelectedEstatus = value;
                if (PanelUpdate == Visibility.Collapsed)
                    OnPropertyValidateChanged("SelectedEstatus");
                else
                    OnPropertyChanged("SelectedEstatus");
            }
        }
        int? _SelectedResponsable;
        public int? SelectedResponsable
        {
            get { return _SelectedResponsable; }
            set
            {
                _SelectedResponsable = value;
                OnPropertyValidateChanged("SelectedResponsable");
                ValidateHorarioListaInterno();
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
            }
        }
        string _TextErrorResponsable;
        public string TextErrorResponsable
        {
            get { return _TextErrorResponsable; }
            set
            {
                _TextErrorResponsable = value;
                OnPropertyChanged("TextErrorResponsable");
                Validaciones();
            }
        }
        bool _SelectedTab = true;
        public bool SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                _SelectedTab = value;
                OnPropertyChanged("SelectedTab");
                //if (value)
                //    CreacionGruposLoad();
            }
        }

        GRUPO _SelectedGrupo;
        public GRUPO SelectedGrupo
        {
            get { return _SelectedGrupo; }
            set
            {
                _SelectedGrupo = value;

                SelectedCount = 0;
                ListGrupoParticipante = null;
                if (value == null)
                    if (PanelUpdate == Visibility.Collapsed)
                        EnabledPanelCrearGrupo = false;

                if (value != null)
                    if (value.ID_GRUPO == -1)
                    {
                        clickSwitch("menu_agregar");
                        SelectedGrupo = null;
                    }

                if (value != null)
                    if (value.ID_GRUPO != -1)
                    {
                        ListGrupoHorario = new ObservableCollection<GRUPO_HORARIO>(new cGrupoHorario().GetData().Where(w => w.ID_GRUPO == value.ID_GRUPO).ToList());
                        SelectedResponsable = value.ID_GRUPO_RESPONSABLE;
                        TextErrorResponsable = string.Empty;
                        GrupoDescripcion = value.DESCR;
                        FechaInicio = value.GRUPO_HORARIO.Count > 0 ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO : new Nullable<DateTime>();
                        FechaFin = value.GRUPO_HORARIO.Count > 0 ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).LastOrDefault().HORA_TERMINO : new Nullable<DateTime>();
                        PanelUpdate = Visibility.Collapsed;
                        EnabledPanelCrearGrupo = true;
                        SelectedEstatus = value.ID_ESTATUS_GRUPO;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                OnPropertyChanged("SelectedGrupo");

                if (_SelectedGrupo != null)
                    ListaInternosUpdate(value.ID_GRUPO);
                //else
                //{
                //    if (ListGrupo != null)
                //        if (ListGrupo.Count > 1)
                //            if (SelectedActividad.HasValue)
                //                ListaInternosLoad(SelectedActividad.Value);
                //}
            }
        }
        ListaInternos _SelectedInterno;
        public ListaInternos SelectedInterno
        {
            get { return _SelectedInterno; }
            set
            {
                if (_SelectedInterno == null && value != null)
                    value.HorarioInterno = null;
                if (_SelectedInterno != null && value == null)
                    _SelectedInterno.HorarioInterno = null;
                _SelectedInterno = value;
                OnPropertyChanged("SelectedInterno");

                if (value != null)
                {
                    _SelectedInterno.HorarioInterno = (_SelectedInterno.HorarioInterno != null ? _SelectedInterno.HorarioInterno.Count > 0 ? _SelectedInterno.HorarioInterno : null : null) ?? new ObservableCollection<ListaActividad>(GenerarListaActividades(value.Entity.INGRESO));
                    ListActividadParticipante = _SelectedInterno.HorarioInterno;
                }
                else
                    ListActividadParticipante = null;
            }
        }
        short? OCUPANTE_MAX;
        bool isOCUPANTE_MAX;
        short? OCUPANTE_MIN;
        bool _isOCUPANTE_MIN;
        public bool IsOCUPANTE_MIN
        {
            get { return _isOCUPANTE_MIN; }
            set
            {
                _isOCUPANTE_MIN = value;
                IsEnabledCrearGrupo = value;
            }
        }
        string _SelectedCountText;
        public string SelectedCountText
        {
            get { return _SelectedCountText; }
            set
            {
                _SelectedCountText = value;
                OnPropertyChanged("SelectedCountText");
            }
        }
        bool _IsEnabledCrearGrupo;
        public bool IsEnabledCrearGrupo
        {
            get { return _IsEnabledCrearGrupo; }
            set
            {
                _IsEnabledCrearGrupo = value;
                OnPropertyChanged("IsEnabledCrearGrupo");
            }
        }
        bool _EnabledEstatusGrupo = false;
        public bool EnabledEstatusGrupo
        {
            get { return _EnabledEstatusGrupo; }
            set
            {
                _EnabledEstatusGrupo = value;
                OnPropertyChanged("EnabledEstatusGrupo");
            }
        }
        bool _EnabledPanelCrearGrupo;
        public bool EnabledPanelCrearGrupo
        {
            get { return _EnabledPanelCrearGrupo; }
            set
            {
                ListEstatus = ListEstatus ?? new ObservableCollection<GRUPO_ESTATUS>(new cGrupoEstatus().GetData().ToList());
                SelectedEstatus = 1;

                if (!value)
                    SelectedEstatus = null;
                StaticSourcesViewModel.SourceChanged = false;

                _EnabledPanelCrearGrupo = value;
                OnPropertyChanged("EnabledPanelCrearGrupo");
            }
        }

        DateTime? _FechaInicio;
        public DateTime? FechaInicio
        {
            get { return _FechaInicio; }
            set
            {
                if (_FechaInicio == value)
                    return;
                AdvertenciaCambio("_FechaInicio", value, "FechaInicio").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("FechaInicio");
                        else
                        {
                            if (_FechaInicio < FechaServer.Date)
                                _FechaInicio = FechaServer.Date;
                            OnPropertyValidateChanged("FechaInicio");
                        }

                        ValidateHorarioListaInterno();

                        if (FechaFin == null)
                        {
                            _FechaFin = FechaServer.Date;
                            OnPropertyChanged("FechaFin");
                        }
                        if (FechaFin < FechaServer.Date)
                        {
                            _FechaFin = FechaServer.Date;
                            OnPropertyChanged("FechaFin");
                        }
                        else
                        {
                            InvalidatePropertyValidateChange = true;
                            FechaFin = FechaInicio.HasValue ? FechaInicio.Value.AddSeconds(.000001) : new Nullable<DateTime>();
                            InvalidatePropertyValidateChange = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de fecha de inicio", ex);
                    }
                });
            }
        }
        DateTime? _FechaFin;
        public DateTime? FechaFin
        {
            get { return _FechaFin; }
            set
            {
                ValidateHorarioListaInterno();
                if (_FechaFin == value)
                    return;
                AdvertenciaCambio("_FechaFin", value, "FechaFin").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("FechaFin");
                        else
                        {
                            if (_FechaFin < FechaServer.Date)
                                _FechaFin = FechaServer.Date;
                            OnPropertyChanged("FechaFin");
                        }
                        ValidateHorarioListaInterno();

                        FechaValidate = string.Empty;
                        if (FechaInicio > FechaFin)
                            FechaValidate = "LA FECHA FINAL NO PUEDE SER MENOR A LA DE INICIO";

                        if (CheckDomingo)
                        {
                            AreaValidateDomingo = ValidateAreaDisponible(SelectedAreaDomingo, DayOfWeek.Sunday);
                            if (_FinDiaDomingo.HasValue)
                                FinDiaDomingo = _FinDiaDomingo.Value.AddDays(1);
                        }
                        if (CheckLunes)
                        {
                            AreaValidateLunes = ValidateAreaDisponible(SelectedAreaLunes, DayOfWeek.Monday);
                            if (_FinDiaLunes.HasValue)
                                FinDiaLunes = _FinDiaLunes.Value.AddDays(1);
                        }
                        if (CheckMartes)
                        {
                            AreaValidateMartes = ValidateAreaDisponible(SelectedAreaMartes, DayOfWeek.Tuesday);
                            if (_FinDiaMartes.HasValue)
                                FinDiaMartes = _FinDiaMartes.Value.AddDays(1);
                        }
                        if (CheckMiercoles)
                        {
                            AreaValidateMiercoles = ValidateAreaDisponible(SelectedAreaMiercoles, DayOfWeek.Wednesday);
                            if (_FinDiaMiercoles.HasValue)
                                FinDiaMiercoles = _FinDiaMiercoles.Value.AddDays(1);
                        }
                        if (CheckJueves)
                        {
                            AreaValidateJueves = ValidateAreaDisponible(SelectedAreaJueves, DayOfWeek.Thursday);
                            if (_FinDiaJueves.HasValue)
                                FinDiaJueves = _FinDiaJueves.Value.AddDays(1);
                        }
                        if (CheckViernes)
                        {
                            AreaValidateViernes = ValidateAreaDisponible(SelectedAreaViernes, DayOfWeek.Friday);
                            if (_FinDiaViernes.HasValue)
                                FinDiaViernes = _FinDiaViernes.Value.AddDays(1);
                        }
                        if (CheckSabado)
                        {
                            AreaValidateSabado = ValidateAreaDisponible(SelectedAreaSabado, DayOfWeek.Saturday);
                            if (_FinDiaSabado.HasValue)
                                FinDiaSabado = _FinDiaSabado.Value.AddDays(1);
                        }

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de fecha de final", ex);
                    }
                });
            }
        }
        string _FechaValidate;
        public string FechaValidate
        {
            get { return _FechaValidate; }
            set
            {
                FechaValidateHasError = !string.IsNullOrEmpty(value);
                _FechaValidate = value;
                OnPropertyChanged("FechaValidate");
                Validaciones();
            }
        }
        bool FechaValidateHasError;
        private string _MensajeText = string.Empty;
        public string MensajeText
        {
            get { return _MensajeText; }
            set
            {
                _MensajeText = value;
                OnPropertyChanged("MensajeText");
            }
        }

        bool _CheckDomingo;
        public bool CheckDomingo
        {
            get { return _CheckDomingo; }
            set
            {
                if (_CheckDomingo == value)
                    return;
                AdvertenciaCambio("_CheckDomingo", value, "CheckDomingo").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckDomingo");
                        else
                            OnPropertyValidateChanged("CheckDomingo");
                        VisibilityDomingo = _CheckDomingo ? Visibility.Visible : Visibility.Collapsed;
                        RowDomingo = _CheckDomingo ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckDomingo)
                        {
                            InicioDiaDomingo = null;
                            FinDiaDomingo = null;
                            SelectedAreaDomingo = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia domingo", ex);
                    }
                });
            }
        }
        bool _CheckLunes;
        public bool CheckLunes
        {
            get { return _CheckLunes; }
            set
            {
                if (_CheckLunes == value)
                    return;
                AdvertenciaCambio("_CheckLunes", value, "CheckLunes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckLunes");
                        else
                            OnPropertyValidateChanged("CheckLunes");
                        VisibilityLunes = _CheckLunes ? Visibility.Visible : Visibility.Collapsed;
                        RowLunes = _CheckLunes ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckLunes)
                        {
                            InicioDiaLunes = null;
                            FinDiaLunes = null;
                            SelectedAreaLunes = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia lunes", ex);
                    }
                });
            }
        }
        bool _CheckMartes;
        public bool CheckMartes
        {
            get { return _CheckMartes; }
            set
            {
                if (_CheckMartes == value)
                    return;
                AdvertenciaCambio("_CheckMartes", value, "CheckMartes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckMartes");
                        else
                            OnPropertyChanged("CheckMartes");
                        VisibilityMartes = _CheckMartes ? Visibility.Visible : Visibility.Collapsed;
                        RowMartes = _CheckMartes ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckMartes)
                        {
                            InicioDiaMartes = null;
                            FinDiaMartes = null;
                            SelectedAreaMartes = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia martes", ex);
                    }
                });
            }
        }
        bool _CheckMiercoles;
        public bool CheckMiercoles
        {
            get { return _CheckMiercoles; }
            set
            {
                if (_CheckMiercoles == value)
                    return;
                AdvertenciaCambio("_CheckMiercoles", value, "CheckMiercoles").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckMiercoles");
                        else
                            OnPropertyValidateChanged("CheckMiercoles");
                        VisibilityMiercoles = _CheckMiercoles ? Visibility.Visible : Visibility.Collapsed;
                        RowMiercoles = _CheckMiercoles ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckMiercoles)
                        {
                            InicioDiaMiercoles = null;
                            FinDiaMiercoles = null;
                            SelectedAreaMiercoles = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia miercoles", ex);
                    }
                });
            }
        }
        bool _CheckJueves;
        public bool CheckJueves
        {
            get { return _CheckJueves; }
            set
            {
                if (_CheckJueves == value)
                    return;
                AdvertenciaCambio("_CheckJueves", value, "CheckJueves").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckJueves");
                        else
                            OnPropertyValidateChanged("CheckJueves");
                        VisibilityJueves = _CheckJueves ? Visibility.Visible : Visibility.Collapsed;
                        RowJueves = _CheckJueves ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckJueves)
                        {
                            InicioDiaJueves = null;
                            FinDiaJueves = null;
                            SelectedAreaJueves = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia jueves", ex);
                    }
                });
            }
        }
        bool _CheckViernes;
        public bool CheckViernes
        {
            get { return _CheckViernes; }
            set
            {
                if (_CheckViernes == value)
                    return;
                AdvertenciaCambio("_CheckViernes", value, "CheckViernes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckViernes");
                        else
                            OnPropertyValidateChanged("CheckViernes");
                        VisibilityViernes = _CheckViernes ? Visibility.Visible : Visibility.Collapsed;
                        RowViernes = _CheckViernes ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckViernes)
                        {
                            InicioDiaViernes = null;
                            FinDiaViernes = null;
                            SelectedAreaViernes = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia viernes", ex);
                    }
                });
            }
        }
        bool _CheckSabado;
        public bool CheckSabado
        {
            get { return _CheckSabado; }
            set
            {
                if (_CheckSabado == value)
                    return;
                AdvertenciaCambio("_CheckSabado", value, "CheckSabado").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckSabado");
                        else
                            OnPropertyValidateChanged("CheckSabado");
                        VisibilitySabado = _CheckSabado ? Visibility.Visible : Visibility.Collapsed;
                        RowSabado = _CheckSabado ? new Nullable<short>() : 0;
                        ValidateHorarioListaInterno();
                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                        if (!_CheckSabado)
                        {
                            InicioDiaSabado = null;
                            FinDiaSabado = null;
                            SelectedAreaSabado = null;
                            isInicioDia = true;
                        }

                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia sabado", ex);
                    }
                });
            }
        }

        short? _RowDomingo = 0;
        public short? RowDomingo
        {
            get { return _RowDomingo; }
            set
            {
                _RowDomingo = value;
                OnPropertyChanged("RowDomingo");
            }
        }
        short? _RowLunes = 0;
        public short? RowLunes
        {
            get { return _RowLunes; }
            set
            {
                _RowLunes = value;
                OnPropertyChanged("RowLunes");
            }
        }
        short? _RowMartes = 0;
        public short? RowMartes
        {
            get { return _RowMartes; }
            set
            {
                _RowMartes = value;
                OnPropertyChanged("RowMartes");
            }
        }
        short? _RowMiercoles = 0;
        public short? RowMiercoles
        {
            get { return _RowMiercoles; }
            set
            {
                _RowMiercoles = value;
                OnPropertyChanged("RowMiercoles");
            }
        }
        short? _RowJueves = 0;
        public short? RowJueves
        {
            get { return _RowJueves; }
            set
            {
                _RowJueves = value;
                OnPropertyChanged("RowJueves");
            }
        }
        short? _RowViernes = 0;
        public short? RowViernes
        {
            get { return _RowViernes; }
            set
            {
                _RowViernes = value;
                OnPropertyChanged("RowViernes");
            }
        }
        short? _RowSabado = 0;
        public short? RowSabado
        {
            get { return _RowSabado; }
            set
            {
                _RowSabado = value;
                OnPropertyChanged("RowSabado");
            }
        }

        Visibility _VisibilityDomingo = Visibility.Collapsed;
        public Visibility VisibilityDomingo
        {
            get { return _VisibilityDomingo; }
            set
            {
                _VisibilityDomingo = value;
                OnPropertyChanged("VisibilityDomingo");
            }
        }
        Visibility _VisibilityLunes = Visibility.Collapsed;
        public Visibility VisibilityLunes
        {
            get { return _VisibilityLunes; }
            set
            {
                _VisibilityLunes = value;
                OnPropertyChanged("VisibilityLunes");
            }
        }
        Visibility _VisibilityMartes = Visibility.Collapsed;
        public Visibility VisibilityMartes
        {
            get { return _VisibilityMartes; }
            set
            {
                _VisibilityMartes = value;
                OnPropertyChanged("VisibilityMartes");
            }
        }
        Visibility _VisibilityMiercoles = Visibility.Collapsed;
        public Visibility VisibilityMiercoles
        {
            get { return _VisibilityMiercoles; }
            set
            {
                _VisibilityMiercoles = value;
                OnPropertyChanged("VisibilityMiercoles");
            }
        }
        Visibility _VisibilityJueves = Visibility.Collapsed;
        public Visibility VisibilityJueves
        {
            get { return _VisibilityJueves; }
            set
            {
                _VisibilityJueves = value;
                OnPropertyChanged("VisibilityJueves");
            }
        }
        Visibility _VisibilityViernes = Visibility.Collapsed;
        public Visibility VisibilityViernes
        {
            get { return _VisibilityViernes; }
            set
            {
                _VisibilityViernes = value;
                OnPropertyChanged("VisibilityViernes");
            }
        }
        Visibility _VisibilitySabado = Visibility.Collapsed;
        public Visibility VisibilitySabado
        {
            get { return _VisibilitySabado; }
            set
            {
                _VisibilitySabado = value;
                OnPropertyChanged("VisibilitySabado");
            }
        }
        Visibility _PanelUpdate;
        public Visibility PanelUpdate
        {
            get { return _PanelUpdate; }
            set
            {
                EnabledEstatusGrupo = value == Visibility.Collapsed ? true : false;

                _PanelUpdate = value;
                OnPropertyChanged("PanelUpdate");
            }
        }

        DateTime? _InicioDiaDomingo;
        public DateTime? InicioDiaDomingo
        {
            get { return _InicioDiaDomingo; }
            set
            {
                if (_InicioDiaDomingo == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaDomingo", value, "InicioDiaDomingo").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaDomingo = _InicioDiaDomingo.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia domingo", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaDomingo;
        public DateTime? FinDiaDomingo
        {
            get { return _FinDiaDomingo; }
            set
            {
                if (_FinDiaDomingo == value)
                    return;
                AdvertenciaCambio("_FinDiaDomingo", value, "FinDiaDomingo").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateDomingo = string.Empty;
                        if (InicioDiaDomingo.HasValue && FinDiaDomingo.HasValue)
                            if (InicioDiaDomingo.Value.TimeOfDay >= FinDiaDomingo.Value.TimeOfDay)
                                FechaValidateDomingo = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateDomingo = ValidateAreaDisponible(SelectedAreaDomingo, DayOfWeek.Sunday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia domingo", ex);
                    }
                });
            }
        }
        string _FechaValidateDomingo;
        public string FechaValidateDomingo
        {
            get { return _FechaValidateDomingo; }
            set
            {
                FechaValidateDomingoHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorDomingo = FechaValidateDomingoHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateDomingo = value;
                OnPropertyChanged("FechaValidateDomingo");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateDomingoHasError;
        Visibility _ShowMarkErrorDomingo = Visibility.Collapsed;
        public Visibility ShowMarkErrorDomingo
        {
            get { return _ShowMarkErrorDomingo; }
            set
            {
                _ShowMarkErrorDomingo = value;
                OnPropertyChanged("ShowMarkErrorDomingo");
            }
        }

        DateTime? _InicioDiaLunes;
        public DateTime? InicioDiaLunes
        {
            get { return _InicioDiaLunes; }
            set
            {
                if (_InicioDiaLunes == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaLunes", value, "InicioDiaLunes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaLunes = _InicioDiaLunes.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia lunes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaLunes;
        public DateTime? FinDiaLunes
        {
            get { return _FinDiaLunes; }
            set
            {
                if (_FinDiaLunes == value)
                    return;
                AdvertenciaCambio("_FinDiaLunes", value, "FinDiaLunes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateLunes = string.Empty;
                        if (InicioDiaLunes.HasValue && FinDiaLunes.HasValue)
                            if (InicioDiaLunes.Value.TimeOfDay >= FinDiaLunes.Value.TimeOfDay)
                                FechaValidateLunes = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateLunes = ValidateAreaDisponible(SelectedAreaLunes, DayOfWeek.Monday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia lunes", ex);
                    }
                });
            }
        }
        string _FechaValidateLunes;
        public string FechaValidateLunes
        {
            get { return _FechaValidateLunes; }
            set
            {
                FechaValidateLunesHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorLunes = FechaValidateLunesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateLunes = value;
                OnPropertyChanged("FechaValidateLunes");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateLunesHasError;
        Visibility _ShowMarkErrorLunes = Visibility.Collapsed;
        public Visibility ShowMarkErrorLunes
        {
            get { return _ShowMarkErrorLunes; }
            set
            {
                _ShowMarkErrorLunes = value;
                OnPropertyChanged("ShowMarkErrorLunes");
            }
        }

        DateTime? _InicioDiaMartes;
        public DateTime? InicioDiaMartes
        {
            get { return _InicioDiaMartes; }
            set
            {
                if (_InicioDiaMartes == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaMartes", value, "InicioDiaMartes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaMartes = _InicioDiaMartes.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia martes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaMartes;
        public DateTime? FinDiaMartes
        {
            get { return _FinDiaMartes; }
            set
            {
                if (_FinDiaMartes == value)
                    return;
                AdvertenciaCambio("_FinDiaMartes", value, "FinDiaMartes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateMartes = string.Empty;
                        if (InicioDiaMartes.HasValue && FinDiaMartes.HasValue)
                            if (InicioDiaMartes.Value.TimeOfDay >= FinDiaMartes.Value.TimeOfDay)
                                FechaValidateMartes = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateMartes = ValidateAreaDisponible(SelectedAreaMartes, DayOfWeek.Tuesday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia martes", ex);
                    }
                });
            }
        }
        string _FechaValidateMartes;
        public string FechaValidateMartes
        {
            get { return _FechaValidateMartes; }
            set
            {
                FechaValidateMartesHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorMartes = FechaValidateMartesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateMartes = value;
                OnPropertyChanged("FechaValidateMartes");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateMartesHasError;
        Visibility _ShowMarkErrorMartes = Visibility.Collapsed;
        public Visibility ShowMarkErrorMartes
        {
            get { return _ShowMarkErrorMartes; }
            set
            {
                _ShowMarkErrorMartes = value;
                OnPropertyChanged("ShowMarkErrorMartes");
            }
        }

        DateTime? _InicioDiaMiercoles;
        public DateTime? InicioDiaMiercoles
        {
            get { return _InicioDiaMiercoles; }
            set
            {
                if (_InicioDiaMiercoles == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaMiercoles", value, "InicioDiaMiercoles").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaMiercoles = _InicioDiaMiercoles.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia miercoles", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaMiercoles;
        public DateTime? FinDiaMiercoles
        {
            get { return _FinDiaMiercoles; }
            set
            {
                if (_FinDiaMiercoles == value)
                    return;
                AdvertenciaCambio("_FinDiaMiercoles", value, "FinDiaMiercoles").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateMiercoles = string.Empty;
                        if (InicioDiaMiercoles.HasValue && FinDiaMiercoles.HasValue)
                            if (InicioDiaMiercoles.Value.TimeOfDay >= FinDiaMiercoles.Value.TimeOfDay)
                                FechaValidateMiercoles = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateMiercoles = ValidateAreaDisponible(SelectedAreaMiercoles, DayOfWeek.Wednesday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia miercoles", ex);
                    }
                });
            }
        }
        string _FechaValidateMiercoles;
        public string FechaValidateMiercoles
        {
            get { return _FechaValidateMiercoles; }
            set
            {
                FechaValidateMiercolesHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorMiercoles = FechaValidateMiercolesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateMiercoles = value;
                OnPropertyChanged("FechaValidateMiercoles");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateMiercolesHasError;
        Visibility _ShowMarkErrorMiercoles = Visibility.Collapsed;
        public Visibility ShowMarkErrorMiercoles
        {
            get { return _ShowMarkErrorMiercoles; }
            set
            {
                _ShowMarkErrorMiercoles = value;
                OnPropertyChanged("ShowMarkErrorMiercoles");
            }
        }

        DateTime? _InicioDiaJueves;
        public DateTime? InicioDiaJueves
        {
            get { return _InicioDiaJueves; }
            set
            {
                if (_InicioDiaJueves == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaJueves", value, "InicioDiaJueves").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaJueves = _InicioDiaJueves.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia jueves", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaJueves;
        public DateTime? FinDiaJueves
        {
            get { return _FinDiaJueves; }
            set
            {
                if (_FinDiaJueves == value)
                    return;
                AdvertenciaCambio("_FinDiaJueves", value, "FinDiaJueves").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateJueves = string.Empty;
                        if (InicioDiaJueves.HasValue && FinDiaJueves.HasValue)
                            if (InicioDiaJueves.Value.TimeOfDay >= FinDiaJueves.Value.TimeOfDay)
                                FechaValidateJueves = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateJueves = ValidateAreaDisponible(SelectedAreaJueves, DayOfWeek.Thursday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia jueves", ex);
                    }
                });
            }
        }
        string _FechaValidateJueves;
        public string FechaValidateJueves
        {
            get { return _FechaValidateJueves; }
            set
            {
                FechaValidateJuevesHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorJueves = FechaValidateJuevesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateJueves = value;
                OnPropertyChanged("FechaValidateJueves");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateJuevesHasError;
        Visibility _ShowMarkErrorJueves = Visibility.Collapsed;
        public Visibility ShowMarkErrorJueves
        {
            get { return _ShowMarkErrorJueves; }
            set
            {
                _ShowMarkErrorJueves = value;
                OnPropertyChanged("ShowMarkErrorJueves");
            }
        }

        DateTime? _InicioDiaViernes;
        public DateTime? InicioDiaViernes
        {
            get { return _InicioDiaViernes; }
            set
            {
                if (_InicioDiaViernes == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaViernes", value, "InicioDiaViernes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaViernes = _InicioDiaViernes.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia viernes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaViernes;
        public DateTime? FinDiaViernes
        {
            get { return _FinDiaViernes; }
            set
            {
                if (_FinDiaViernes == value)
                    return;
                AdvertenciaCambio("_FinDiaViernes", value, "FinDiaViernes").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateViernes = string.Empty;
                        if (InicioDiaViernes.HasValue && FinDiaViernes.HasValue)
                            if (InicioDiaViernes.Value.TimeOfDay >= FinDiaViernes.Value.TimeOfDay)
                                FechaValidateViernes = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateViernes = ValidateAreaDisponible(SelectedAreaViernes, DayOfWeek.Friday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia viernes", ex);
                    }
                });
            }
        }
        string _FechaValidateViernes;
        public string FechaValidateViernes
        {
            get { return _FechaValidateViernes; }
            set
            {
                FechaValidateViernesHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorViernes = FechaValidateViernesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateViernes = value;
                OnPropertyChanged("FechaValidateViernes");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateViernesHasError;
        Visibility _ShowMarkErrorViernes = Visibility.Collapsed;
        public Visibility ShowMarkErrorViernes
        {
            get { return _ShowMarkErrorViernes; }
            set
            {
                _ShowMarkErrorViernes = value;
                OnPropertyChanged("ShowMarkErrorViernes");
            }
        }

        DateTime? _InicioDiaSabado;
        public DateTime? InicioDiaSabado
        {
            get { return _InicioDiaSabado; }
            set
            {
                if (_InicioDiaSabado == value)
                    return;
                isNeeded = true;
                AdvertenciaCambio("_InicioDiaSabado", value, "InicioDiaSabado").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        if (value != null)
                        {
                            if (SelectedCount > 0)
                                if (!isNeeded)
                                    return;

                            FinDiaSabado = _InicioDiaSabado.Value.AddHours(1);
                            isInicioDia = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia sabado", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaSabado;
        public DateTime? FinDiaSabado
        {
            get { return _FinDiaSabado; }
            set
            {
                if (_FinDiaSabado == value)
                    return;
                AdvertenciaCambio("_FinDiaSabado", value, "FinDiaSabado").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInterno();

                        FechaValidateSabado = string.Empty;
                        if (InicioDiaSabado.HasValue && FinDiaSabado.HasValue)
                            if (InicioDiaSabado.Value.TimeOfDay >= FinDiaSabado.Value.TimeOfDay)
                                FechaValidateSabado = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateSabado = ValidateAreaDisponible(SelectedAreaSabado, DayOfWeek.Saturday);

                        TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);
                        RecargarActividadParticipante();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia sabado", ex);
                    }
                });
            }
        }
        string _FechaValidateSabado;
        public string FechaValidateSabado
        {
            get { return _FechaValidateSabado; }
            set
            {
                FechaValidateSabadoHasError = !string.IsNullOrEmpty(value);
                ShowMarkErrorSabado = FechaValidateSabadoHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateSabado = value;
                OnPropertyChanged("FechaValidateSabado");
                ValidateHorarioListaInterno();
                Validaciones();
            }
        }
        bool FechaValidateSabadoHasError;
        Visibility _ShowMarkErrorSabado = Visibility.Collapsed;
        public Visibility ShowMarkErrorSabado
        {
            get { return _ShowMarkErrorSabado; }
            set
            {
                _ShowMarkErrorSabado = value;
                OnPropertyChanged("ShowMarkErrorSabado");
            }
        }

        short? _SelectedAreaDomingo;
        public short? SelectedAreaDomingo
        {
            get { return _SelectedAreaDomingo; }
            set
            {
                _SelectedAreaDomingo = value;
                OnPropertyChanged("SelectedAreaDomingo");
                ValidateHorarioListaInterno();
                AreaValidateDomingo = ValidateAreaDisponible(value, DayOfWeek.Sunday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaDomingo.HasValue)
                    FinDiaDomingo.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateDomingo;

        short? _SelectedAreaLunes;
        public short? SelectedAreaLunes
        {
            get { return _SelectedAreaLunes; }
            set
            {
                _SelectedAreaLunes = value;
                OnPropertyChanged("SelectedAreaLunes");
                ValidateHorarioListaInterno();
                AreaValidateLunes = ValidateAreaDisponible(value, DayOfWeek.Monday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaLunes.HasValue)
                    FinDiaLunes.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateLunes;

        short? _SelectedAreaMartes;
        public short? SelectedAreaMartes
        {
            get { return _SelectedAreaMartes; }
            set
            {
                _SelectedAreaMartes = value;
                OnPropertyChanged("SelectedAreaMartes");
                ValidateHorarioListaInterno();
                AreaValidateMartes = ValidateAreaDisponible(value, DayOfWeek.Tuesday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaMartes.HasValue)
                    FinDiaMartes.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateMartes;

        short? _SelectedAreaMiercoles;
        public short? SelectedAreaMiercoles
        {
            get { return _SelectedAreaMiercoles; }
            set
            {
                _SelectedAreaMiercoles = value;
                OnPropertyChanged("SelectedAreaMiercoles");
                ValidateHorarioListaInterno();
                AreaValidateMiercoles = ValidateAreaDisponible(value, DayOfWeek.Wednesday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaMiercoles.HasValue)
                    FinDiaMiercoles.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateMiercoles;

        short? _SelectedAreaJueves;
        public short? SelectedAreaJueves
        {
            get { return _SelectedAreaJueves; }
            set
            {
                _SelectedAreaJueves = value;
                OnPropertyChanged("SelectedAreaJueves");
                ValidateHorarioListaInterno();
                AreaValidateJueves = ValidateAreaDisponible(value, DayOfWeek.Thursday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaJueves.HasValue)
                    FinDiaJueves.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateJueves;

        short? _SelectedAreaViernes;
        public short? SelectedAreaViernes
        {
            get { return _SelectedAreaViernes; }
            set
            {
                _SelectedAreaViernes = value;
                OnPropertyChanged("SelectedAreaViernes");
                ValidateHorarioListaInterno();
                AreaValidateViernes = ValidateAreaDisponible(value, DayOfWeek.Friday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaViernes.HasValue)
                    FinDiaViernes.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateViernes;

        short? _SelectedAreaSabado;
        public short? SelectedAreaSabado
        {
            get { return _SelectedAreaSabado; }
            set
            {
                _SelectedAreaSabado = value;
                OnPropertyChanged("SelectedAreaSabado");
                ValidateHorarioListaInterno();
                AreaValidateSabado = ValidateAreaDisponible(value, DayOfWeek.Saturday);
                TextErrorResponsable = ValidateResponsableDisponible(SelectedResponsable);

                if (FinDiaSabado.HasValue)
                    FinDiaSabado.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateSabado;

        bool isNeeded = true;
        bool isInicioDia = true;
        bool WaitValidated;

        string _GrupoDescripcion;
        public string GrupoDescripcion
        {
            get { return _GrupoDescripcion; }
            set
            {
                _GrupoDescripcion = value;
                OnPropertyValidateChanged("GrupoDescripcion");
                ValidateHorarioListaInterno();
            }
        }

        int SelectedCount;
        bool selectedCountTrue = false;

        bool _EnabledListaHorario;
        public bool EnabledListaHorario
        {
            get { return _EnabledListaHorario; }
            set
            {
                _EnabledListaHorario = value;
                OnPropertyChanged("EnabledListaHorario");
            }
        }

        List<bool> IsEmpalme;
        short? _EmpalmeHorarioNuevo = 0;
        public short? EmpalmeHorarioNuevo
        {
            get { return _EmpalmeHorarioNuevo; }
            set
            {
                _EmpalmeHorarioNuevo = value;
                OnPropertyChanged("EmpalmeHorarioNuevo");
            }
        }
        bool _EnabledEditGrupo;
        public bool EnabledEditGrupo
        {
            get { return _EnabledEditGrupo; }
            set
            {
                _EnabledEditGrupo = value;
                OnPropertyChanged("EnabledEditGrupo");
            }
        }
        #endregion

        #region [COMPLEMENTARIO]
        ObservableCollection<TIPO_PROGRAMA> _ListProgramasCompl;
        public ObservableCollection<TIPO_PROGRAMA> ListProgramasCompl
        {
            get { return _ListProgramasCompl; }
            set
            {
                _ListProgramasCompl = value;
                OnPropertyChanged("ListProgramasCompl");
            }
        }
        ObservableCollection<ACTIVIDAD> _ListActividadesCompl;
        public ObservableCollection<ACTIVIDAD> ListActividadesCompl
        {
            get { return _ListActividadesCompl; }
            set
            {
                _ListActividadesCompl = value;
                OnPropertyChanged("ListActividadesCompl");
            }
        }
        RangeEnabledObservableCollection<ListaInternosCompl> _ListGrupoParticipanteCompl;
        public RangeEnabledObservableCollection<ListaInternosCompl> ListGrupoParticipanteCompl
        {
            get { return _ListGrupoParticipanteCompl; }
            set
            {
                if (value == null)
                    EmpalmeHorarioNuevoCompl = 0;
                _ListGrupoParticipanteCompl = value;
                OnPropertyChanged("ListGrupoParticipanteCompl");
            }
        }
        ObservableCollection<GRUPO> _ListGrupoCompl;
        public ObservableCollection<GRUPO> ListGrupoCompl
        {
            get { return _ListGrupoCompl; }
            set
            {
                _ListGrupoCompl = value;
                OnPropertyChanged("ListGrupoCompl");
            }
        }
        ObservableCollection<GRUPO_HORARIO> ListGrupoHorarioCompl;
        ObservableCollection<ListaActividad> _ListActividadParticipanteCompl;
        public ObservableCollection<ListaActividad> ListActividadParticipanteCompl
        {
            get { return _ListActividadParticipanteCompl; }
            set
            {
                _ListActividadParticipanteCompl = value;
                OnPropertyChanged("ListActividadParticipanteCompl");
            }
        }
        ObservableCollection<DELITO> _ListDelitosCompl;
        public ObservableCollection<DELITO> ListDelitosCompl
        {
            get { return _ListDelitosCompl; }
            set
            {
                _ListDelitosCompl = value;
                OnPropertyChanged("ListDelitosCompl");
            }
        }
        ObservableCollection<SECTOR_CLASIFICACION> _ListPlanimetriaCompl;
        public ObservableCollection<SECTOR_CLASIFICACION> ListPlanimetriaCompl
        {
            get { return _ListPlanimetriaCompl; }
            set
            {
                _ListPlanimetriaCompl = value;
                OnPropertyChanged("ListPlanimetriaCompl");
            }
        }
        ObservableCollection<RangoEdad> _ListEdadesCompl = new ObservableCollection<RangoEdad>() { 
        new RangoEdad(){ ID_EDAD = null, RANGO_EDAD = string.Empty},
        new RangoEdad(){ ID_EDAD = "90-200", RANGO_EDAD = "MAYOR DE 90"},
        new RangoEdad(){ ID_EDAD = "85-90", RANGO_EDAD = "DE 85 A 90"},
        new RangoEdad(){ ID_EDAD = "80-85", RANGO_EDAD = "DE 80 A 85"},
        new RangoEdad(){ ID_EDAD = "75-80", RANGO_EDAD = "DE 75 A 80"},
        new RangoEdad(){ ID_EDAD = "70-75", RANGO_EDAD = "DE 70 A 75"},
        new RangoEdad(){ ID_EDAD = "65-70", RANGO_EDAD = "DE 65 A 70"},
        new RangoEdad(){ ID_EDAD = "60-65", RANGO_EDAD = "DE 60 A 65"},
        new RangoEdad(){ ID_EDAD = "55-60", RANGO_EDAD = "DE 55 A 60"},
        new RangoEdad(){ ID_EDAD = "50-55", RANGO_EDAD = "DE 50 A 55"},
        new RangoEdad(){ ID_EDAD = "45-50", RANGO_EDAD = "DE 45 A 50"},
        new RangoEdad(){ ID_EDAD = "40-45", RANGO_EDAD = "DE 40 A 45"},
        new RangoEdad(){ ID_EDAD = "35-40", RANGO_EDAD = "DE 35 A 40"},
        new RangoEdad(){ ID_EDAD = "30-35", RANGO_EDAD = "DE 30 A 35"},
        new RangoEdad(){ ID_EDAD = "25-30", RANGO_EDAD = "DE 25 A 30"},
        new RangoEdad(){ ID_EDAD = "18-25", RANGO_EDAD = "DE 18 A 25"},
        new RangoEdad(){ ID_EDAD = "0-18", RANGO_EDAD = "MENOR DE 18"},};
        public ObservableCollection<RangoEdad> ListEdadesCompl
        {
            get { return _ListEdadesCompl; }
            set
            {
                _ListEdadesCompl = value;
                OnPropertyChanged("ListEdadesCompl");
            }
        }
        ObservableCollection<RangoANIOS> _ListPAniosCompl;
        public ObservableCollection<RangoANIOS> ListPAniosCompl
        {
            get { return _ListPAniosCompl; }
            set
            {
                _ListPAniosCompl = value;
                OnPropertyChanged("ListPAniosCompl");
            }
        }
        ObservableCollection<ListaInternosCompl> _ListaRestaurarSeleccionadosCompl = new ObservableCollection<ListaInternosCompl>();
        public ObservableCollection<ListaInternosCompl> ListaRestaurarSeleccionadosCompl
        {
            get { return _ListaRestaurarSeleccionadosCompl; }
            set
            {
                _ListaRestaurarSeleccionadosCompl = value;
                OnPropertyChanged("ListaRestaurarSeleccionadosCompl");
            }
        }
        bool ChangeListSelected;

        MahApps.Metro.Controls.MetroAnimatedSingleRowTabControl Tab;
        bool cancellingTabSelectionChange;
        bool _SelectedTabComplementario;
        public bool SelectedTabComplementario
        {
            get { return _SelectedTabComplementario; }
            set
            {
                _SelectedTabComplementario = value;
                OnPropertyChanged("SelectedTabComplementario");
                if (value)
                    CreacionGruposComplementariosLoad();
            }
        }
        short? _SelectedProgramaCompl;
        public short? SelectedProgramaCompl
        {
            get { return _SelectedProgramaCompl; }
            set
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        if (ValidaCambioTab)
                        {
                            await TaskEx.Delay(1);
                            OnPropertyChanged("SelectedProgramaCompl");
                            return;
                        }
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                            {
                                OnPropertyChanged("SelectedProgramaCompl");
                                return;
                            }
                        }
                        _SelectedProgramaCompl = value;
                        OnPropertyChanged("SelectedProgramaCompl");

                        SelectedCount = 0;
                        if (value.HasValue)
                            ActividadesComplementariasLoad(value.Value);
                        else
                            ListActividadesCompl = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener actividades", ex);
                    }
                }));
            }
        }
        short? _SelectedActividadCompl;
        public short? SelectedActividadCompl
        {
            get { return _SelectedActividadCompl; }
            set
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        if (StaticSourcesViewModel.SourceChanged)
                        {
                            if (await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea continuar sin guardar?") != 0)
                                StaticSourcesViewModel.SourceChanged = false;
                            else
                            {
                                OnPropertyChanged("SelectedActividadCompl");
                                return;
                            }
                        }
                        _SelectedActividadCompl = value;
                        OnPropertyChanged("SelectedActividadCompl");
                        SelectedCountCompl = 0;
                        LimpiarCamposCompl();

                        if (value.HasValue)
                        {
                            SelectedCountTextCompl = string.Empty;
                            IsEnabledCrearGrupoCompl = false;
                            EnabledPanelCrearGrupoCompl = true;
                            if (SelectedGrupoCompl == null)
                                if (PanelUpdateCompl == Visibility.Collapsed)
                                    EnabledPanelCrearGrupoCompl = false;
                            var configActividad = new cActividad().GetData().Where(w => w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == value).FirstOrDefault();

                            if (configActividad != null)
                            {
                                OCUPANTE_MAXCompl = configActividad.OCUPANTE_MAX;
                                OCUPANTE_MINCompl = configActividad.OCUPANTE_MIN;
                                SelectedCountTextCompl = "Minimo de " + OCUPANTE_MINCompl + ", 0/" + OCUPANTE_MAXCompl + " Seleccionados";
                            }
                            if (PanelUpdateCompl != Visibility.Collapsed)
                                ListaInternosComplLoad(value.Value);
                            else
                            {
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    try
                                    {
                                        ListGrupoCompl = new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedProgramaCompl && w.ID_ACTIVIDAD == value.Value).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList());
                                        ListDelitosCompl = ListDelitosCompl ?? new ObservableCollection<DELITO>(new cDelito().GetData().OrderBy(o => o.DESCR).ToList());
                                        ListPlanimetriaCompl = ListPlanimetriaCompl ?? new ObservableCollection<SECTOR_CLASIFICACION>(new cSectorClasificacion().GetData().OrderBy(o => o.POBLACION).ToList());
                                    }
                                    catch (Exception ex)
                                    {
                                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información", ex);
                                    }
                                });
                                if (!ListDelitosCompl.Where(w => w.ID_DELITO == -1).Any())
                                    ListDelitosCompl.Insert(0, new DELITO() { ID_DELITO = -1, DESCR = string.Empty });

                                if (!ListPlanimetriaCompl.Where(w => w.ID_SECTOR_CLAS == -1).Any())
                                    ListPlanimetriaCompl.Insert(0, new SECTOR_CLASIFICACION() { ID_SECTOR_CLAS = -1, POBLACION = string.Empty });
                            }
                        }
                        else
                        {
                            EnabledPanelCrearGrupoCompl = false;
                            SelectedCountTextCompl = "Minimo de , 0/ Seleccionados";
                            ListGrupoParticipanteCompl = null;
                            ListGrupoCompl = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar actividad", ex);
                    }
                }));
            }
        }
        short? _SelectedEstatusCompl;
        public short? SelectedEstatusCompl
        {
            get { return _SelectedEstatusCompl; }
            set
            {
                _SelectedEstatusCompl = value;
                if (PanelUpdateCompl == Visibility.Collapsed)
                    OnPropertyValidateChanged("SelectedEstatusCompl");
                else
                    OnPropertyChanged("SelectedEstatusCompl");
            }
        }
        string _SelectedCountTextCompl;
        public string SelectedCountTextCompl
        {
            get { return _SelectedCountTextCompl; }
            set
            {
                _SelectedCountTextCompl = value;
                OnPropertyChanged("SelectedCountTextCompl");
            }
        }
        bool _IsEnabledCrearGrupoCompl;
        public bool IsEnabledCrearGrupoCompl
        {
            get { return _IsEnabledCrearGrupoCompl; }
            set
            {
                _IsEnabledCrearGrupoCompl = value;
                OnPropertyChanged("IsEnabledCrearGrupoCompl");
            }
        }
        ListaInternosCompl _SelectedInternoCompl;
        public ListaInternosCompl SelectedInternoCompl
        {
            get { return _SelectedInternoCompl; }
            set
            {
                if (_SelectedInternoCompl == null && value != null)
                    value.HorarioInterno = null;
                if (_SelectedInternoCompl != null && value == null)
                    _SelectedInternoCompl.HorarioInterno = null;
                _SelectedInternoCompl = value;
                OnPropertyChanged("SelectedInternoCompl");

                if (value != null)
                {
                    _SelectedInternoCompl.HorarioInterno = (_SelectedInternoCompl.HorarioInterno != null ? _SelectedInternoCompl.HorarioInterno.Count > 0 ? _SelectedInternoCompl.HorarioInterno : null : null) ?? new ObservableCollection<ListaActividad>(GenerarListaActividadesCompl(value.Entity));
                    ListActividadParticipanteCompl = _SelectedInternoCompl.HorarioInterno;
                }
                else
                    ListActividadParticipanteCompl = null;
            }
        }
        short? _SelectedDelitoCompl = null;
        public short? SelectedDelitoCompl
        {
            get { return _SelectedDelitoCompl; }
            set
            {
                _SelectedDelitoCompl = value;
                OnPropertyChanged("SelectedDelitoCompl");
                if (value == null)
                    return;
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                        ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información", ex);
                    }
                }));
            }
        }
        short? _SelectedPlanimetriaCompl;
        public short? SelectedPlanimetriaCompl
        {
            get { return _SelectedPlanimetriaCompl; }
            set
            {
                if (value == -1)
                    _SelectedPlanimetriaCompl = null;
                else
                    _SelectedPlanimetriaCompl = value;
                OnPropertyChanged("SelectedPlanimetriaCompl");
                if (value == null)
                    return;
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                        ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información", ex);
                    }
                }));
            }
        }
        string _SelectedEdades;
        public string SelectedEdades
        {
            get { return _SelectedEdades; }
            set
            {
                _SelectedEdades = value;
                OnPropertyChanged("SelectedEdades");
                if (ListGrupoParticipanteCompl == null)
                    return;

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                        ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información", ex);
                    }
                }));
            }
        }

        DateTime? _FechaInicioCompl;
        public DateTime? FechaInicioCompl
        {
            get { return _FechaInicioCompl; }
            set
            {
                if (_FechaInicioCompl == value)
                    return;
                AdvertenciaCambioCompl("_FechaInicioCompl", value, "FechaInicioCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                        {
                            _FechaFinCompl = FechaServer.Date;
                            OnPropertyChanged("FechaInicioCompl");
                        }
                        else
                        {
                            if (_FechaInicioCompl < FechaServer.Date)
                                _FechaInicioCompl = FechaServer.Date;
                            OnPropertyValidateChanged("FechaInicioCompl");
                        }
                        ValidateHorarioListaInternoCompl();

                        if (FechaFinCompl == null)
                        {
                            _FechaFinCompl = FechaServer.Date;
                            OnPropertyChanged("FechaFinCompl");
                        }
                        if (FechaFinCompl < FechaServer.Date)
                        {
                            _FechaFinCompl = FechaServer.Date;
                            OnPropertyChanged("FechaFinCompl");
                        }
                        else
                        {
                            InvalidatePropertyValidateChange = true;
                            FechaFinCompl = FechaInicioCompl.HasValue ? FechaInicioCompl.Value.AddSeconds(.000001) : new Nullable<DateTime>();
                            InvalidatePropertyValidateChange = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de fecha de inicio", ex);
                    }
                });
            }
        }
        DateTime? _FechaFinCompl;
        public DateTime? FechaFinCompl
        {
            get { return _FechaFinCompl; }
            set
            {
                if (_FechaFinCompl == value)
                    return;
                AdvertenciaCambioCompl("_FechaFinCompl", value, "FechaFinCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("FechaFinCompl");
                        else
                        {
                            if (_FechaFinCompl < FechaServer.Date)
                                _FechaFinCompl = FechaServer.Date;
                            OnPropertyChanged("FechaFinCompl");
                        }
                        ValidateHorarioListaInternoCompl();

                        FechaValidateCompl = string.Empty;
                        if (FechaInicioCompl > FechaFinCompl)
                            FechaValidateCompl = "LA FECHA FINAL NO PUEDE SER MENOR A LA DE INICIO";

                        if (CheckDomingoCompl)
                        {
                            AreaValidateDomingoCompl = ValidateAreaDisponibleCompl(SelectedAreaDomingoCompl, DayOfWeek.Sunday);
                            if (_FinDiaDomingoCompl.HasValue)
                                FinDiaDomingoCompl = _FinDiaDomingoCompl.Value.AddDays(1);
                        }
                        if (CheckLunesCompl)
                        {
                            AreaValidateLunesCompl = ValidateAreaDisponibleCompl(SelectedAreaLunesCompl, DayOfWeek.Monday);
                            if (_FinDiaLunesCompl.HasValue)
                                FinDiaLunesCompl = _FinDiaLunesCompl.Value.AddDays(1);
                        }
                        if (CheckMartesCompl)
                        {
                            AreaValidateMartesCompl = ValidateAreaDisponibleCompl(SelectedAreaMartesCompl, DayOfWeek.Tuesday);
                            if (_FinDiaMartesCompl.HasValue)
                                FinDiaMartesCompl = _FinDiaMartesCompl.Value.AddDays(1);
                        }
                        if (CheckMiercolesCompl)
                        {
                            AreaValidateMiercolesCompl = ValidateAreaDisponibleCompl(SelectedAreaMiercolesCompl, DayOfWeek.Wednesday);
                            if (_FinDiaMiercolesCompl.HasValue)
                                FinDiaMiercolesCompl = _FinDiaMiercolesCompl.Value.AddDays(1);
                        }
                        if (CheckJuevesCompl)
                        {
                            AreaValidateJuevesCompl = ValidateAreaDisponibleCompl(SelectedAreaJuevesCompl, DayOfWeek.Thursday);
                            if (_FinDiaJuevesCompl.HasValue)
                                FinDiaJuevesCompl = _FinDiaJuevesCompl.Value.AddDays(1);
                        }
                        if (CheckViernesCompl)
                        {
                            AreaValidateViernesCompl = ValidateAreaDisponibleCompl(SelectedAreaViernes, DayOfWeek.Friday);
                            if (_FinDiaViernesCompl.HasValue)
                                FinDiaViernesCompl = _FinDiaViernesCompl.Value.AddDays(1);
                        }
                        if (CheckSabadoCompl)
                        {
                            AreaValidateSabadoCompl = ValidateAreaDisponibleCompl(SelectedAreaSabado, DayOfWeek.Saturday);
                            if (_FinDiaSabadoCompl.HasValue)
                                FinDiaSabadoCompl = _FinDiaSabadoCompl.Value.AddDays(1);
                        }

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de fecha de final", ex);
                    }
                });
            }
        }
        string _FechaValidateCompl;
        public string FechaValidateCompl
        {
            get { return _FechaValidateCompl; }
            set
            {
                FechaValidateHasErrorCompl = !string.IsNullOrEmpty(value);
                _FechaValidateCompl = value;
                OnPropertyChanged("FechaValidateCompl");
                Validaciones();
            }
        }
        bool FechaValidateHasErrorCompl;
        private string _MensajeTextCompl = string.Empty;
        public string MensajeTextCompl
        {
            get { return _MensajeTextCompl; }
            set
            {
                _MensajeTextCompl = value;
                OnPropertyChanged("MensajeTextCompl");
            }
        }

        bool _CheckDomingoCompl;
        public bool CheckDomingoCompl
        {
            get { return _CheckDomingoCompl; }
            set
            {
                if (_CheckDomingoCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckDomingoCompl", value, "CheckDomingoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckDomingoCompl");
                        else
                            OnPropertyValidateChanged("CheckDomingoCompl");
                        VisibilityDomingoCompl = _CheckDomingoCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowDomingoCompl = _CheckDomingoCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckDomingoCompl)
                        {
                            InicioDiaDomingoCompl = null;
                            FinDiaDomingoCompl = null;
                            SelectedAreaDomingoCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia domingo", ex);
                    }
                });
            }
        }
        bool _CheckLunesCompl;
        public bool CheckLunesCompl
        {
            get { return _CheckLunesCompl; }
            set
            {
                if (_CheckLunesCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckLunesCompl", value, "CheckLunesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckLunesCompl");
                        else
                            OnPropertyValidateChanged("CheckLunesCompl");
                        VisibilityLunesCompl = _CheckLunesCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowLunesCompl = _CheckLunesCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckLunesCompl)
                        {
                            InicioDiaLunesCompl = null;
                            FinDiaLunesCompl = null;
                            SelectedAreaLunesCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia lunes", ex);
                    }
                });
            }
        }
        bool _CheckMartesCompl;
        public bool CheckMartesCompl
        {
            get { return _CheckMartesCompl; }
            set
            {
                if (_CheckMartesCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckMartesCompl", value, "CheckMartesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckMartesCompl");
                        else
                            OnPropertyChanged("CheckMartesCompl");
                        VisibilityMartesCompl = _CheckMartesCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowMartesCompl = _CheckMartesCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckMartesCompl)
                        {
                            InicioDiaMartesCompl = null;
                            FinDiaMartesCompl = null;
                            SelectedAreaMartesCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia martes", ex);
                    }
                });
            }
        }
        bool _CheckMiercolesCompl;
        public bool CheckMiercolesCompl
        {
            get { return _CheckMiercolesCompl; }
            set
            {
                if (_CheckMiercolesCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckMiercolesCompl", value, "CheckMiercolesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckMiercolesCompl");
                        else
                            OnPropertyValidateChanged("CheckMiercolesCompl");
                        VisibilityMiercolesCompl = _CheckMiercolesCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowMiercolesCompl = _CheckMiercolesCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckMiercolesCompl)
                        {
                            InicioDiaMiercolesCompl = null;
                            FinDiaMiercolesCompl = null;
                            SelectedAreaMiercolesCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia miercoles", ex);
                    }
                });
            }
        }
        bool _CheckJuevesCompl;
        public bool CheckJuevesCompl
        {
            get { return _CheckJuevesCompl; }
            set
            {
                if (_CheckJuevesCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckJuevesCompl", value, "CheckJuevesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicio == null)
                            OnPropertyChanged("CheckJuevesCompl");
                        else
                            OnPropertyValidateChanged("CheckJuevesCompl");
                        VisibilityJuevesCompl = _CheckJuevesCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowJuevesCompl = _CheckJuevesCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckJuevesCompl)
                        {
                            InicioDiaJuevesCompl = null;
                            FinDiaJuevesCompl = null;
                            SelectedAreaJuevesCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia jueves", ex);
                    }
                });
            }
        }
        bool _CheckViernesCompl;
        public bool CheckViernesCompl
        {
            get { return _CheckViernesCompl; }
            set
            {
                if (_CheckViernesCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckViernesCompl", value, "CheckViernesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckViernesCompl");
                        else
                            OnPropertyValidateChanged("CheckViernesCompl");
                        VisibilityViernesCompl = _CheckViernesCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowViernesCompl = _CheckViernesCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckViernesCompl)
                        {
                            InicioDiaViernesCompl = null;
                            FinDiaViernesCompl = null;
                            SelectedAreaViernesCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia viernes", ex);
                    }
                });
            }
        }
        bool _CheckSabadoCompl;
        public bool CheckSabadoCompl
        {
            get { return _CheckSabadoCompl; }
            set
            {
                if (_CheckSabadoCompl == value)
                    return;
                AdvertenciaCambioCompl("_CheckSabadoCompl", value, "CheckSabadoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_FechaInicioCompl == null)
                            OnPropertyChanged("CheckSabadoCompl");
                        else
                            OnPropertyValidateChanged("CheckSabadoCompl");
                        VisibilitySabadoCompl = _CheckSabadoCompl ? Visibility.Visible : Visibility.Collapsed;
                        RowSabadoCompl = _CheckSabadoCompl ? new Nullable<short>() : 0;
                        ValidateHorarioListaInternoCompl();
                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                        if (!_CheckSabadoCompl)
                        {
                            InicioDiaSabadoCompl = null;
                            FinDiaSabadoCompl = null;
                            SelectedAreaSabadoCompl = null;
                            isInicioDiaCompl = true;
                        }

                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en marcar/desmarcar del dia sabado", ex);
                    }
                });
            }
        }

        short? _RowDomingoCompl = 0;
        public short? RowDomingoCompl
        {
            get { return _RowDomingoCompl; }
            set
            {
                _RowDomingoCompl = value;
                OnPropertyChanged("RowDomingoCompl");
            }
        }
        short? _RowLunesCompl = 0;
        public short? RowLunesCompl
        {
            get { return _RowLunesCompl; }
            set
            {
                _RowLunesCompl = value;
                OnPropertyChanged("RowLunesCompl");
            }
        }
        short? _RowMartesCompl = 0;
        public short? RowMartesCompl
        {
            get { return _RowMartesCompl; }
            set
            {
                _RowMartesCompl = value;
                OnPropertyChanged("RowMartesCompl");
            }
        }
        short? _RowMiercolesCompl = 0;
        public short? RowMiercolesCompl
        {
            get { return _RowMiercolesCompl; }
            set
            {
                _RowMiercolesCompl = value;
                OnPropertyChanged("RowMiercolesCompl");
            }
        }
        short? _RowJuevesCompl = 0;
        public short? RowJuevesCompl
        {
            get { return _RowJuevesCompl; }
            set
            {
                _RowJuevesCompl = value;
                OnPropertyChanged("RowJuevesCompl");
            }
        }
        short? _RowViernesCompl = 0;
        public short? RowViernesCompl
        {
            get { return _RowViernesCompl; }
            set
            {
                _RowViernesCompl = value;
                OnPropertyChanged("RowViernesCompl");
            }
        }
        short? _RowSabadoCompl = 0;
        public short? RowSabadoCompl
        {
            get { return _RowSabadoCompl; }
            set
            {
                _RowSabadoCompl = value;
                OnPropertyChanged("RowSabadoCompl");
            }
        }

        Visibility _VisibilityDomingoCompl = Visibility.Collapsed;
        public Visibility VisibilityDomingoCompl
        {
            get { return _VisibilityDomingoCompl; }
            set
            {
                _VisibilityDomingoCompl = value;
                OnPropertyChanged("VisibilityDomingoCompl");
            }
        }
        Visibility _VisibilityLunesCompl = Visibility.Collapsed;
        public Visibility VisibilityLunesCompl
        {
            get { return _VisibilityLunesCompl; }
            set
            {
                _VisibilityLunesCompl = value;
                OnPropertyChanged("VisibilityLunesCompl");
            }
        }
        Visibility _VisibilityMartesCompl = Visibility.Collapsed;
        public Visibility VisibilityMartesCompl
        {
            get { return _VisibilityMartesCompl; }
            set
            {
                _VisibilityMartesCompl = value;
                OnPropertyChanged("VisibilityMartesCompl");
            }
        }
        Visibility _VisibilityMiercolesCompl = Visibility.Collapsed;
        public Visibility VisibilityMiercolesCompl
        {
            get { return _VisibilityMiercolesCompl; }
            set
            {
                _VisibilityMiercolesCompl = value;
                OnPropertyChanged("VisibilityMiercolesCompl");
            }
        }
        Visibility _VisibilityJuevesCompl = Visibility.Collapsed;
        public Visibility VisibilityJuevesCompl
        {
            get { return _VisibilityJuevesCompl; }
            set
            {
                _VisibilityJuevesCompl = value;
                OnPropertyChanged("VisibilityJuevesCompl");
            }
        }
        Visibility _VisibilityViernesCompl = Visibility.Collapsed;
        public Visibility VisibilityViernesCompl
        {
            get { return _VisibilityViernesCompl; }
            set
            {
                _VisibilityViernesCompl = value;
                OnPropertyChanged("VisibilityViernesCompl");
            }
        }
        Visibility _VisibilitySabadoCompl = Visibility.Collapsed;
        public Visibility VisibilitySabadoCompl
        {
            get { return _VisibilitySabadoCompl; }
            set
            {
                _VisibilitySabadoCompl = value;
                OnPropertyChanged("VisibilitySabadoCompl");
            }
        }
        Visibility _PanelUpdateCompl;
        public Visibility PanelUpdateCompl
        {
            get { return _PanelUpdateCompl; }
            set
            {
                EnabledEstatusGrupoCompl = value == Visibility.Collapsed ? true : false;

                _PanelUpdateCompl = value;
                OnPropertyChanged("PanelUpdateCompl");
            }
        }

        DateTime? _InicioDiaDomingoCompl;
        public DateTime? InicioDiaDomingoCompl
        {
            get { return _InicioDiaDomingoCompl; }
            set
            {
                if (_InicioDiaDomingoCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaDomingoCompl", value, "InicioDiaDomingoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaDomingoCompl = _InicioDiaDomingoCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia domingo", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaDomingoCompl;
        public DateTime? FinDiaDomingoCompl
        {
            get { return _FinDiaDomingoCompl; }
            set
            {
                if (_FinDiaDomingoCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaDomingoCompl", value, "FinDiaDomingoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateDomingoCompl = string.Empty;
                        if (InicioDiaDomingoCompl.HasValue && FinDiaDomingoCompl.HasValue)
                            if (InicioDiaDomingoCompl.Value.TimeOfDay >= FinDiaDomingoCompl.Value.TimeOfDay)
                                FechaValidateDomingoCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateDomingoCompl = ValidateAreaDisponibleCompl(SelectedAreaDomingoCompl, DayOfWeek.Sunday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia domingo", ex);
                    }
                });
            }
        }
        string _FechaValidateDomingoCompl;
        public string FechaValidateDomingoCompl
        {
            get { return _FechaValidateDomingoCompl; }
            set
            {
                FechaValidateDomingoHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorDomingoCompl = FechaValidateDomingoHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateDomingoCompl = value;
                OnPropertyChanged("FechaValidateDomingoCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateDomingoHasErrorCompl;
        Visibility _ShowMarkErrorDomingoCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorDomingoCompl
        {
            get { return _ShowMarkErrorDomingoCompl; }
            set
            {
                _ShowMarkErrorDomingoCompl = value;
                OnPropertyChanged("ShowMarkErrorDomingoCompl");
            }
        }

        DateTime? _InicioDiaLunesCompl;
        public DateTime? InicioDiaLunesCompl
        {
            get { return _InicioDiaLunesCompl; }
            set
            {
                if (_InicioDiaLunesCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaLunesCompl", value, "InicioDiaLunesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaLunesCompl = _InicioDiaLunesCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia lunes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaLunesCompl;
        public DateTime? FinDiaLunesCompl
        {
            get { return _FinDiaLunesCompl; }
            set
            {
                if (_FinDiaLunesCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaLunesCompl", value, "FinDiaLunesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateLunesCompl = string.Empty;
                        if (InicioDiaLunesCompl.HasValue && FinDiaLunesCompl.HasValue)
                            if (InicioDiaLunesCompl.Value.TimeOfDay >= FinDiaLunesCompl.Value.TimeOfDay)
                                FechaValidateLunesCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateLunesCompl = ValidateAreaDisponibleCompl(SelectedAreaLunesCompl, DayOfWeek.Monday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia lunes", ex);
                    }
                });
            }
        }
        string _FechaValidateLunesCompl;
        public string FechaValidateLunesCompl
        {
            get { return _FechaValidateLunesCompl; }
            set
            {
                FechaValidateLunesHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorLunesCompl = FechaValidateLunesHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateLunesCompl = value;
                OnPropertyChanged("FechaValidateLunesCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateLunesHasErrorCompl;
        Visibility _ShowMarkErrorLunesCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorLunesCompl
        {
            get { return _ShowMarkErrorLunesCompl; }
            set
            {
                _ShowMarkErrorLunesCompl = value;
                OnPropertyChanged("ShowMarkErrorLunesCompl");
            }
        }

        DateTime? _InicioDiaMartesCompl;
        public DateTime? InicioDiaMartesCompl
        {
            get { return _InicioDiaMartesCompl; }
            set
            {
                if (_InicioDiaMartesCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaMartesCompl", value, "InicioDiaMartesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaMartesCompl = _InicioDiaMartesCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia martes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaMartesCompl;
        public DateTime? FinDiaMartesCompl
        {
            get { return _FinDiaMartesCompl; }
            set
            {
                if (_FinDiaMartesCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaMartesCompl", value, "FinDiaMartesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateMartesCompl = string.Empty;
                        if (InicioDiaMartesCompl.HasValue && FinDiaMartesCompl.HasValue)
                            if (InicioDiaMartesCompl.Value.TimeOfDay >= FinDiaMartesCompl.Value.TimeOfDay)
                                FechaValidateMartesCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateMartesCompl = ValidateAreaDisponibleCompl(SelectedAreaMartesCompl, DayOfWeek.Tuesday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia martes", ex);
                    }
                });
            }
        }
        string _FechaValidateMartesCompl;
        public string FechaValidateMartesCompl
        {
            get { return _FechaValidateMartesCompl; }
            set
            {
                FechaValidateMartesHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorMartesCompl = FechaValidateMartesHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateMartesCompl = value;
                OnPropertyChanged("FechaValidateMartesCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateMartesHasErrorCompl;
        Visibility _ShowMarkErrorMartesCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorMartesCompl
        {
            get { return _ShowMarkErrorMartesCompl; }
            set
            {
                _ShowMarkErrorMartesCompl = value;
                OnPropertyChanged("ShowMarkErrorMartesCompl");
            }
        }

        DateTime? _InicioDiaMiercolesCompl;
        public DateTime? InicioDiaMiercolesCompl
        {
            get { return _InicioDiaMiercolesCompl; }
            set
            {
                if (_InicioDiaMiercolesCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaMiercolesCompl", value, "InicioDiaMiercolesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaMiercolesCompl = _InicioDiaMiercolesCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia miercoles", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaMiercolesCompl;
        public DateTime? FinDiaMiercolesCompl
        {
            get { return _FinDiaMiercolesCompl; }
            set
            {
                if (_FinDiaMiercolesCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaMiercolesCompl", value, "FinDiaMiercolesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateMiercolesCompl = string.Empty;
                        if (InicioDiaMiercolesCompl.HasValue && FinDiaMiercolesCompl.HasValue)
                            if (InicioDiaMiercolesCompl.Value.TimeOfDay >= FinDiaMiercolesCompl.Value.TimeOfDay)
                                FechaValidateMiercolesCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateMiercolesCompl = ValidateAreaDisponibleCompl(SelectedAreaMiercolesCompl, DayOfWeek.Wednesday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia miercoles", ex);
                    }
                });
            }
        }
        string _FechaValidateMiercolesCompl;
        public string FechaValidateMiercolesCompl
        {
            get { return _FechaValidateMiercolesCompl; }
            set
            {
                FechaValidateMiercolesHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorMiercolesCompl = FechaValidateMiercolesHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateMiercolesCompl = value;
                OnPropertyChanged("FechaValidateMiercolesCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateMiercolesHasErrorCompl;
        Visibility _ShowMarkErrorMiercolesCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorMiercolesCompl
        {
            get { return _ShowMarkErrorMiercolesCompl; }
            set
            {
                _ShowMarkErrorMiercolesCompl = value;
                OnPropertyChanged("ShowMarkErrorMiercolesCompl");
            }
        }

        DateTime? _InicioDiaJuevesCompl;
        public DateTime? InicioDiaJuevesCompl
        {
            get { return _InicioDiaJuevesCompl; }
            set
            {
                if (_InicioDiaJuevesCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaJuevesCompl", value, "InicioDiaJuevesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaJuevesCompl = _InicioDiaJuevesCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia jueves", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaJuevesCompl;
        public DateTime? FinDiaJuevesCompl
        {
            get { return _FinDiaJuevesCompl; }
            set
            {
                if (_FinDiaJuevesCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaJuevesCompl", value, "FinDiaJuevesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateJuevesCompl = string.Empty;
                        if (InicioDiaJuevesCompl.HasValue && FinDiaJuevesCompl.HasValue)
                            if (InicioDiaJuevesCompl.Value.TimeOfDay >= FinDiaJuevesCompl.Value.TimeOfDay)
                                FechaValidateJuevesCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateJuevesCompl = ValidateAreaDisponibleCompl(SelectedAreaJuevesCompl, DayOfWeek.Thursday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia jueves", ex);
                    }
                });
            }
        }
        string _FechaValidateJuevesCompl;
        public string FechaValidateJuevesCompl
        {
            get { return _FechaValidateJuevesCompl; }
            set
            {
                FechaValidateJuevesHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorJuevesCompl = FechaValidateJuevesHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateJuevesCompl = value;
                OnPropertyChanged("FechaValidateJuevesCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateJuevesHasErrorCompl;
        Visibility _ShowMarkErrorJuevesCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorJuevesCompl
        {
            get { return _ShowMarkErrorJuevesCompl; }
            set
            {
                _ShowMarkErrorJuevesCompl = value;
                OnPropertyChanged("ShowMarkErrorJuevesCompl");
            }
        }

        DateTime? _InicioDiaViernesCompl;
        public DateTime? InicioDiaViernesCompl
        {
            get { return _InicioDiaViernesCompl; }
            set
            {
                if (_InicioDiaViernesCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaViernesCompl", value, "InicioDiaViernesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaViernesCompl = _InicioDiaViernesCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia viernes", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaViernesCompl;
        public DateTime? FinDiaViernesCompl
        {
            get { return _FinDiaViernesCompl; }
            set
            {
                if (_FinDiaViernesCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaViernesCompl", value, "FinDiaViernesCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateViernesCompl = string.Empty;
                        if (InicioDiaViernesCompl.HasValue && FinDiaViernesCompl.HasValue)
                            if (InicioDiaViernesCompl.Value.TimeOfDay >= FinDiaViernesCompl.Value.TimeOfDay)
                                FechaValidateViernesCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateViernesCompl = ValidateAreaDisponibleCompl(SelectedAreaViernesCompl, DayOfWeek.Friday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia viernes", ex);
                    }
                });
            }
        }
        string _FechaValidateViernesCompl;
        public string FechaValidateViernesCompl
        {
            get { return _FechaValidateViernesCompl; }
            set
            {
                FechaValidateViernesHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorViernesCompl = FechaValidateViernesHasError ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateViernesCompl = value;
                OnPropertyChanged("FechaValidateViernesCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateViernesHasErrorCompl;
        Visibility _ShowMarkErrorViernesCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorViernesCompl
        {
            get { return _ShowMarkErrorViernesCompl; }
            set
            {
                _ShowMarkErrorViernesCompl = value;
                OnPropertyChanged("ShowMarkErrorViernesCompl");
            }
        }

        DateTime? _InicioDiaSabadoCompl;
        public DateTime? InicioDiaSabadoCompl
        {
            get { return _InicioDiaSabadoCompl; }
            set
            {
                if (_InicioDiaSabadoCompl == value)
                    return;
                isNeededCompl = true;
                AdvertenciaCambioCompl("_InicioDiaSabadoCompl", value, "InicioDiaSabadoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        if (value != null)
                        {
                            if (SelectedCountCompl > 0)
                                if (!isNeededCompl)
                                    return;

                            FinDiaSabadoCompl = _InicioDiaSabadoCompl.Value.AddHours(1);
                            isInicioDiaCompl = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia sabado", ex);
                    }
                });
            }
        }
        DateTime? _FinDiaSabadoCompl;
        public DateTime? FinDiaSabadoCompl
        {
            get { return _FinDiaSabadoCompl; }
            set
            {
                if (_FinDiaSabadoCompl == value)
                    return;
                AdvertenciaCambioCompl("_FinDiaSabadoCompl", value, "FinDiaSabadoCompl").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        ValidateHorarioListaInternoCompl();

                        FechaValidateSabadoCompl = string.Empty;
                        if (InicioDiaSabadoCompl.HasValue && FinDiaSabadoCompl.HasValue)
                            if (InicioDiaSabadoCompl.Value.TimeOfDay >= FinDiaSabadoCompl.Value.TimeOfDay)
                                FechaValidateSabadoCompl = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                            else
                                AreaValidateSabadoCompl = ValidateAreaDisponibleCompl(SelectedAreaSabadoCompl, DayOfWeek.Saturday);

                        TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
                        RecargarActividadParticipanteCompl();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de hora para el dia sabado", ex);
                    }
                });
            }
        }
        string _FechaValidateSabadoCompl;
        public string FechaValidateSabadoCompl
        {
            get { return _FechaValidateSabadoCompl; }
            set
            {
                FechaValidateSabadoHasErrorCompl = !string.IsNullOrEmpty(value);
                ShowMarkErrorSabadoCompl = FechaValidateSabadoHasErrorCompl ? Visibility.Visible : Visibility.Collapsed;
                _FechaValidateSabadoCompl = value;
                OnPropertyChanged("FechaValidateSabadoCompl");
                ValidateHorarioListaInternoCompl();
                Validaciones();
            }
        }
        bool FechaValidateSabadoHasErrorCompl;
        Visibility _ShowMarkErrorSabadoCompl = Visibility.Collapsed;
        public Visibility ShowMarkErrorSabadoCompl
        {
            get { return _ShowMarkErrorSabadoCompl; }
            set
            {
                _ShowMarkErrorSabadoCompl = value;
                OnPropertyChanged("ShowMarkErrorSabadoCompl");
            }
        }

        short? _SelectedAreaDomingoCompl;
        public short? SelectedAreaDomingoCompl
        {
            get { return _SelectedAreaDomingoCompl; }
            set
            {
                _SelectedAreaDomingoCompl = value;
                OnPropertyChanged("SelectedAreaDomingoCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateDomingoCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Sunday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaDomingoCompl.HasValue)
                    FinDiaDomingoCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateDomingoCompl;

        short? _SelectedAreaLunesCompl;
        public short? SelectedAreaLunesCompl
        {
            get { return _SelectedAreaLunesCompl; }
            set
            {
                _SelectedAreaLunesCompl = value;
                OnPropertyChanged("SelectedAreaLunesCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateLunesCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Monday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaLunesCompl.HasValue)
                    FinDiaLunesCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateLunesCompl;

        short? _SelectedAreaMartesCompl;
        public short? SelectedAreaMartesCompl
        {
            get { return _SelectedAreaMartesCompl; }
            set
            {
                _SelectedAreaMartesCompl = value;
                OnPropertyChanged("SelectedAreaMartesCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateMartesCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Tuesday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaMartesCompl.HasValue)
                    FinDiaMartesCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateMartesCompl;

        short? _SelectedAreaMiercolesCompl;
        public short? SelectedAreaMiercolesCompl
        {
            get { return _SelectedAreaMiercolesCompl; }
            set
            {
                _SelectedAreaMiercolesCompl = value;
                OnPropertyChanged("SelectedAreaMiercolesCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateMiercolesCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Wednesday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaMiercolesCompl.HasValue)
                    FinDiaMiercolesCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateMiercolesCompl;

        short? _SelectedAreaJuevesCompl;
        public short? SelectedAreaJuevesCompl
        {
            get { return _SelectedAreaJuevesCompl; }
            set
            {
                _SelectedAreaJuevesCompl = value;
                OnPropertyChanged("SelectedAreaJuevesCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateJuevesCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Thursday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaJuevesCompl.HasValue)
                    FinDiaJuevesCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateJuevesCompl;

        short? _SelectedAreaViernesCompl;
        public short? SelectedAreaViernesCompl
        {
            get { return _SelectedAreaViernesCompl; }
            set
            {
                _SelectedAreaViernesCompl = value;
                OnPropertyChanged("SelectedAreaViernesCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateViernesCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Friday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaViernesCompl.HasValue)
                    FinDiaViernesCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateViernesCompl;

        short? _SelectedAreaSabadoCompl;
        public short? SelectedAreaSabadoCompl
        {
            get { return _SelectedAreaSabadoCompl; }
            set
            {
                _SelectedAreaSabadoCompl = value;
                OnPropertyChanged("SelectedAreaSabadoCompl");
                ValidateHorarioListaInternoCompl();
                AreaValidateSabadoCompl = ValidateAreaDisponibleCompl(value, DayOfWeek.Saturday);
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);

                if (FinDiaSabadoCompl.HasValue)
                    FinDiaSabadoCompl.Value.AddMilliseconds(000001);

                Validaciones();
            }
        }
        string AreaValidateSabadoCompl;

        int? _SelectedResponsableCompl;
        public int? SelectedResponsableCompl
        {
            get { return _SelectedResponsableCompl; }
            set
            {
                _SelectedResponsableCompl = value;
                OnPropertyValidateChanged("SelectedResponsableCompl");
                ValidateHorarioListaInternoCompl();
                TextErrorResponsableCompl = ValidateResponsableDisponibleCompl(SelectedResponsableCompl);
            }
        }
        string _TextErrorResponsableCompl;
        public string TextErrorResponsableCompl
        {
            get { return _TextErrorResponsableCompl; }
            set
            {
                _TextErrorResponsableCompl = value;
                OnPropertyChanged("TextErrorResponsableCompl");
                Validaciones();
            }
        }

        bool WaitValidatedCompl;
        bool isNeededCompl = true;
        bool isInicioDiaCompl = true;
        int SelectedCountCompl;
        bool selectedCountComplTrue = false;
        bool _EnabledEstatusGrupoCompl = false;
        public bool EnabledEstatusGrupoCompl
        {
            get { return _EnabledEstatusGrupoCompl; }
            set
            {
                _EnabledEstatusGrupoCompl = value;
                OnPropertyChanged("EnabledEstatusGrupoCompl");
            }
        }
        string _GrupoDescripcionCompl;
        public string GrupoDescripcionCompl
        {
            get { return _GrupoDescripcionCompl; }
            set
            {
                _GrupoDescripcionCompl = value;
                OnPropertyValidateChanged("GrupoDescripcionCompl");
                ValidateHorarioListaInternoCompl();
            }
        }
        GRUPO _SelectedGrupoCompl;
        public GRUPO SelectedGrupoCompl
        {
            get { return _SelectedGrupoCompl; }
            set
            {
                _SelectedGrupoCompl = value;

                SelectedCountCompl = 0;
                ListGrupoParticipanteCompl = null;
                if (value == null)
                    if (PanelUpdateCompl == Visibility.Collapsed)
                        EnabledPanelCrearGrupoCompl = false;

                if (value != null)
                    if (value.ID_GRUPO == -1)
                    {
                        clickSwitch("menu_agregar");
                        SelectedGrupoCompl = null;
                    }

                if (value != null)
                    if (value.ID_GRUPO != -1)
                    {
                        totalseleccionado = null;
                        ListDelitosCompl = null;
                        ListEdadesCompl = null;
                        ListPlanimetriaCompl = null;
                        ListPAniosCompl = null;
                        ListGrupoHorarioCompl = new ObservableCollection<GRUPO_HORARIO>(new cGrupoHorario().GetData().Where(w => w.ID_GRUPO == value.ID_GRUPO).ToList());
                        SelectedResponsableCompl = value.ID_GRUPO_RESPONSABLE;
                        TextErrorResponsableCompl = string.Empty;
                        GrupoDescripcionCompl = value.DESCR;
                        FechaInicioCompl = value.GRUPO_HORARIO.Count > 0 ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO : new Nullable<DateTime>();
                        FechaFinCompl = value.GRUPO_HORARIO.Count > 0 ? value.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).LastOrDefault().HORA_TERMINO : new Nullable<DateTime>();
                        PanelUpdateCompl = Visibility.Collapsed;
                        EnabledPanelCrearGrupoCompl = true;
                        SelectedEstatusCompl = value.ID_ESTATUS_GRUPO;
                        StaticSourcesViewModel.SourceChanged = false;
                    }

                OnPropertyChanged("SelectedGrupoCompl");

                if (_SelectedGrupoCompl != null)
                    ListaInternosUpdateCompl(value.ID_GRUPO);
                else
                {
                    if (ListGrupoCompl != null)
                        if (ListGrupoCompl.Count > 1)
                            if (SelectedActividadCompl.HasValue)
                                if (PanelUpdateCompl != Visibility.Collapsed)
                                    ListaInternosComplLoad(SelectedActividadCompl.Value);
                }
            }
        }
        List<bool> IsEmpalmeCompl;
        bool _EnabledPanelCrearGrupoCompl;
        public bool EnabledPanelCrearGrupoCompl
        {
            get { return _EnabledPanelCrearGrupoCompl; }
            set
            {
                ListEstatus = ListEstatus ?? new ObservableCollection<GRUPO_ESTATUS>(new cGrupoEstatus().GetData().ToList());
                SelectedEstatusCompl = 1;

                if (!value)
                    SelectedEstatusCompl = null;
                StaticSourcesViewModel.SourceChanged = false;

                _EnabledPanelCrearGrupoCompl = value;
                OnPropertyChanged("EnabledPanelCrearGrupoCompl");
            }
        }
        short? _EmpalmeHorarioNuevoCompl = 0;
        public short? EmpalmeHorarioNuevoCompl
        {
            get { return _EmpalmeHorarioNuevoCompl; }
            set
            {
                _EmpalmeHorarioNuevoCompl = value;
                OnPropertyChanged("EmpalmeHorarioNuevoCompl");
            }
        }
        short? OCUPANTE_MAXCompl;
        bool isOCUPANTE_MAXCompl;
        short? OCUPANTE_MINCompl;
        bool _isOCUPANTE_MINCompl;
        public bool IsOCUPANTE_MINCompl
        {
            get { return _isOCUPANTE_MINCompl; }
            set
            {
                _isOCUPANTE_MINCompl = value;
                IsEnabledCrearGrupoCompl = value;
            }
        }
        bool _EnabledEditGrupoCompl;
        public bool EnabledEditGrupoCompl
        {
            get { return _EnabledEditGrupoCompl; }
            set
            {
                _EnabledEditGrupoCompl = value;
                OnPropertyChanged("EnabledEditGrupoCompl");
            }
        }
        bool _EnabledListaHorarioCompl;
        public bool EnabledListaHorarioCompl
        {
            get { return _EnabledListaHorarioCompl; }
            set
            {
                _EnabledListaHorarioCompl = value;
                OnPropertyChanged("EnabledListaHorarioCompl");
            }
        }
        int Pagina { get; set; }
        bool SeguirCargando { get; set; }
        short? _SelectedAniosCompl;
        public short? SelectedAniosCompl
        {
            get { return _SelectedAniosCompl; }
            set
            {
                _SelectedAniosCompl = value;
                OnPropertyChanged("SelectedAniosCompl");

                if (ListGrupoParticipanteCompl == null)
                    return;

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    ListGrupoParticipanteCompl = new RangeEnabledObservableCollection<ListaInternosCompl>();
                    ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes());
                }));
            }
        }
        bool ValidaCambioTab;
        bool InvalidatePropertyValidateChange;
        int? totalseleccionado;
        #endregion

        #region [CONFIGURACION PERMISOS]
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

        private bool _ejeEnabled;
        public bool EjeEnabled
        {
            get { return _ejeEnabled; }
            set { _ejeEnabled = value; OnPropertyChanged("EjeEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }

        private bool _programaEnabled;
        public bool ProgramaEnabled
        {
            get { return _programaEnabled; }
            set { _programaEnabled = value; OnPropertyChanged("ProgramaEnabled"); }
        }

        private bool _actividadEnabled;
        public bool ActividadEnabled
        {
            get { return _actividadEnabled; }
            set { _actividadEnabled = value; OnPropertyChanged("ActividadEnabled"); }
        }

        private bool _tabComplementarioEnabled;
        public bool TabComplementarioEnabled
        {
            get { return _tabComplementarioEnabled; }
            set { _tabComplementarioEnabled = value; OnPropertyChanged("TabComplementarioEnabled"); }
        }
        #endregion

        #region [USUARIO DEPARTAMENTO]
        private List<int> departamentosUsuarios;
        #endregion
    }

    public class ListaInternos
    {
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRECOMPLETO { get; set; }
        public byte[] ImageSource { get; set; }
        public string FOLIO { get; set; }
        public string UBICACION { get; set; }
        public string PLANIMETRIA { get; set; }
        public string PLANIMETRIACOLOR { get; set; }
        public string RESTANTE { get; set; }
        public string SENTENCIA { get; set; }
        public string RESTANTEsplit { get; set; }
        public bool elegido { get; set; }
        public GRUPO_PARTICIPANTE Entity { get; set; }
        public Thickness ShowEmpalme { get; set; }
        public ObservableCollection<ListaActividad> HorarioInterno { get; set; }
    }

    public class ListaInternosCompl
    {
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRECOMPLETO { get; set; }
        public byte[] ImageSource { get; set; }
        public string FOLIO { get; set; }
        public string UBICACION { get; set; }
        public string PLANIMETRIA { get; set; }
        public string PLANIMETRIACOLOR { get; set; }
        public string RESTANTE { get; set; }
        public string SENTENCIA { get; set; }
        public string RESTANTEsplit { get; set; }
        public bool elegido { get; set; }
        public INGRESO Entity { get; set; }
        public List<GRUPO_PARTICIPANTE> ListGRPA { get; set; }
        public Thickness ShowEmpalme { get; set; }
        public bool hasActividad { get; set; }
        public ObservableCollection<ListaActividad> HorarioInterno { get; set; }
    }

    public class NombreEmpleado
    {
        public int ID_PERSONA { get; set; }
        public string NOMBRE_COMPLETO { get; set; }
    }

    public class ListaActividad
    {
        public string NombreEje { get; set; }
        public string NombreActividad { get; set; }
        public string NombreGrupo { get; set; }
        public string RecurrenciaActividad { get; set; }
        public string InicioActividad { get; set; }
        public string FinActividad { get; set; }
        public ObservableCollection<ListHorario> ListHorario { get; set; }
        public short? orden { get; set; }
        public string State { get; set; }
        public bool Revision { get; set; }
    }

    public class ListHorario
    {
        public string NombreActividad { get; set; }
        public string NombreGrupo { get; set; }
        public string AREADESCR { get; set; }
        public string DESCRDIA { get; set; }
        public DateTime? HORA_INICIO { get; set; }
        public DateTime? HORA_TERMIINO { get; set; }
        public string strHORA_INICIO { get; set; }
        public string strHORA_TERMIINO { get; set; }
        public string GRUPO_HORARIO_ESTATUSDESCR { get; set; }
        public GRUPO_HORARIO GrupoHorarioEntity { get; set; }

        public string State { get; set; }
        public short? Id_Actividad { get; set; }
        public short? Id_Programa { get; set; }
    }

    public class RangoEdad
    {
        public string ID_EDAD { get; set; }
        public string RANGO_EDAD { get; set; }
    }

    public class RangoANIOS
    {
        public short? ID_ANIO { get; set; }
        public string ANIO { get; set; }
    }
}
