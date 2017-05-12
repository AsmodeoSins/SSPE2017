using ControlPenales.Controls.Calendario;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ManejoGruposViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se describe todas las propiedades del modulo manejo de grupo
         * 
         * propiedad importante es: SelectedGrupo
         * 
         */

        ///TODO:
        /// Revisar los querys, falta poner el centro del usuario y departamento
        /// 

        //#region [CONFIGURACION PERMISOS]
        //private bool _agregarMenuEnabled;
        //public bool AgregarMenuEnabled
        //{
        //    get { return _agregarMenuEnabled; }
        //    set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        //}

        //private bool _editarMenuEnabled;
        //public bool EditarMenuEnabled
        //{
        //    get { return _editarMenuEnabled; }
        //    set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
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

        //private bool _programaEnabled;
        //public bool ProgramaEnabled
        //{
        //    get { return _programaEnabled; }
        //    set { _programaEnabled = value; OnPropertyChanged("ProgramaEnabled"); }
        //}

        //private bool _actividadEnabled;
        //public bool ActividadEnabled
        //{
        //    get { return _actividadEnabled; }
        //    set { _actividadEnabled = value; OnPropertyChanged("ActividadEnabled"); }
        //}

        //private bool _grupoEnabled;
        //public bool GrupoEnabled
        //{
        //    get { return _grupoEnabled; }
        //    set { _grupoEnabled = value; OnPropertyChanged("GrupoEnabled"); }
        //}
        //#endregion

        CalendarioView Calendario;
        DataGrid DatagridHorario;
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
        ObservableCollection<GRUPO_HORARIO_ESTATUS> _ListEstatusGrupo;
        public ObservableCollection<GRUPO_HORARIO_ESTATUS> ListEstatusGrupo
        {
            get { return _ListEstatusGrupo; }
            set
            {
                _ListEstatusGrupo = value;
                OnPropertyChanged("ListEstatusGrupo");
            }
        }
        private ObservableCollection<ListaDiaInternos> _ListInternosDia;
        public ObservableCollection<ListaDiaInternos> ListInternosDia
        {
            get { return _ListInternosDia; }
            set
            {
                _ListInternosDia = value;
                OnPropertyChanged("ListInternosDia");
            }
        }
        private ObservableCollection<ListaManejoInternos> _ListInternosGrupo;
        public ObservableCollection<ListaManejoInternos> ListInternosGrupo
        {
            get { return _ListInternosGrupo; }
            set
            {
                _ListInternosGrupo = value;
                OnPropertyChanged("ListInternosGrupo");
            }
        }
        private ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS> _ListEstatus;
        public ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS> ListEstatus
        {
            get { return _ListEstatus; }
            set
            {
                _ListEstatus = value;
                OnPropertyChanged("ListEstatus");
            }
        }

        short? _SelectedPrograma;
        public short? SelectedPrograma
        {
            get { return _SelectedPrograma; }
            set
            {
                _SelectedPrograma = value;
                OnPropertyChanged("SelectedPrograma");

                if (value.HasValue)
                    ActividadesLoad(value.Value);
                else
                    ListActividades = null;
            }
        }
        short? _SelectedActividad;
        public short? SelectedActividad
        {
            get { return _SelectedActividad; }
            set
            {
                _SelectedActividad = value;
                OnPropertyChanged("SelectedActividad");

                if (value.HasValue)
                    GrupoLoad(value.Value);
                else
                    ListGrupo = null;
            }
        }
        short? _SelectedArea;
        public short? SelectedArea
        {
            get { return _SelectedArea; }
            set
            {
                _SelectedArea = value;
                OnPropertyChanged("SelectedArea");
                ValidateAreaDisponible();
            }
        }
        short? _SelectedEstatusGrupo;
        public short? SelectedEstatusGrupo
        {
            get { return _SelectedEstatusGrupo; }
            set
            {
                _SelectedEstatusGrupo = value;
                OnPropertyChanged("SelectedEstatusGrupo");
            }
        }
        GRUPO _SelectedGrupo;
        public GRUPO SelectedGrupo
        {
            get { return _SelectedGrupo; }
            set
            {
                _SelectedGrupo = value;
                ListInternosDia = null;
                ListInternosGrupo = null;
                NombreResponsable = value != null ? _SelectedGrupo.PERSONA.PATERNO.Trim() + " " + _SelectedGrupo.PERSONA.MATERNO.Trim() + " " + _SelectedGrupo.PERSONA.NOMBRE : string.Empty;
                OnPropertyChanged("SelectedGrupo");

                CargarCalendarioGrupo(value);
            }
        }

        string _NombreResponsable;
        public string NombreResponsable
        {
            get { return _NombreResponsable; }
            set
            {
                _NombreResponsable = value;
                OnPropertyChanged("NombreResponsable");
            }
        }

        private int selectedMes;
        public int SelectedMes
        {
            get { return selectedMes; }
            set
            {
                selectedMes = value;
                //if (value > 0)
                //    SeleccionarFechasAgenda(SelectedAlmacen.ID_ALMACEN, value, SelectedAnio);
                //RaisePropertyChanged("SelectedMes");
            }
        }
        private string bindCmdDayClick = string.Empty;
        public string BindCmdDayClick
        {
            get { return bindCmdDayClick; }
            set { bindCmdDayClick = value; RaisePropertyChanged("BindCmdDayClick"); }
        }
        DateTime? _EditFechaInicio;
        public DateTime? EditFechaInicio
        {
            get { return _EditFechaInicio; }
            set
            {
                _EditFechaInicio = value;
                OnPropertyChanged("EditFechaInicio");

                FechaValidateInicio = string.Empty;
                if (EditFechaFin.HasValue && EditFechaInicio.HasValue)
                {
                    ValidateAreaDisponible();
                    if (EditFechaInicio.Value.TimeOfDay > EditFechaFin.Value.TimeOfDay)
                        FechaValidateInicio = "LA HORA INICIAL NO PUEDE SER MAYOR A LA FINAL";
                }

                if (value.HasValue)
                    EditFechaFin = value.Value.AddHours(1);
            }
        }
        DateTime? _EditFechaFin;
        public DateTime? EditFechaFin
        {
            get { return _EditFechaFin; }
            set
            {
                _EditFechaFin = value;
                OnPropertyChanged("EditFechaFin");

                FechaValidateFin = string.Empty;
                if (EditFechaInicio.HasValue && EditFechaFin.HasValue)
                {
                    ValidateAreaDisponible();
                    if (EditFechaInicio.Value.TimeOfDay >= EditFechaFin.Value.TimeOfDay)
                        FechaValidateFin = "LA HORA FINAL NO PUEDE SER MENOR A LA DE INICIO";
                    else
                        FechaValidateInicio = string.Empty;
                }
            }
        }
        string _FechaValidateInicio;
        public string FechaValidateInicio
        {
            get { return _FechaValidateInicio; }
            set
            {
                FechaValidateEditHasError = !string.IsNullOrEmpty(value);
                _FechaValidateInicio = value;
                OnPropertyChanged("FechaValidateInicio");
            }
        }
        string _FechaValidateFin;
        public string FechaValidateFin
        {
            get { return _FechaValidateFin; }
            set
            {
                FechaValidateEditHasError = !string.IsNullOrEmpty(value);
                _FechaValidateFin = value;
                OnPropertyChanged("FechaValidateFin");
            }
        }
        bool FechaValidateEditHasError;
        GRUPO_HORARIO EntityUpdate;

        string _TextError;
        public string TextError
        {
            get { return _TextError; }
            set
            {
                _TextError = value;
                OnPropertyChanged("TextError");
                RowError = string.IsNullOrEmpty(value) ? 0 : new Nullable<short>();
                HasError = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        Visibility _HasError;
        public Visibility HasError
        {
            get { return _HasError; }
            set
            {
                _HasError = value;
                OnPropertyChanged("HasError");
            }
        }
        short? _RowError = 0;
        public short? RowError
        {
            get { return _RowError; }
            set
            {
                _RowError = value;
                OnPropertyChanged("RowError");
            }
        }

        DateTime? _SelectedFecha = Fechas.GetFechaDateServer;
        public DateTime? SelectedFecha
        {
            get { return _SelectedFecha; }
            set
            {                
                ValidarFecha("_SelectedFecha", value, "SelectedFecha").GetAwaiter().OnCompleted(() =>
                {
                    try
                    {
                        if (_SelectedFecha == null)
                            OnPropertyChanged("SelectedFecha");
                        else
                        {
                            if (_SelectedFecha < FechaServer.Date)
                                _SelectedFecha = FechaServer.Date;
                            OnPropertyChanged("SelectedFecha");
                        }

                        if (value.HasValue)
                            ListInternosDia = new ObservableCollection<ListaDiaInternos>(new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO == SelectedGrupo.ID_GRUPO).AsEnumerable().Select(s => new ListaDiaInternos()
                            {
                                Entity = s,
                                FOLIO = s.INGRESO.IMPUTADO.ID_ANIO + "\\" + s.INGRESO.IMPUTADO.ID_IMPUTADO,
                                PATERNO = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.PATERNO) ? string.Empty : s.INGRESO.IMPUTADO.PATERNO.Trim(),
                                MATERNO = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.MATERNO) ? string.Empty : s.INGRESO.IMPUTADO.MATERNO.Trim(),
                                NOMBRE = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.NOMBRE) ? string.Empty : s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                                Id_Actividad = s.GRUPO.ACTIVIDAD.ID_ACTIVIDAD,
                                Id_Programa = s.GRUPO.ACTIVIDAD.ID_TIPO_PROGRAMA,
                                ListHorario = ListaHorario(s, value.Value.Date, value.Value.AddDays(1).Date)
                            }));
                        else
                            ListInternosDia = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el cambio de fecha de inicio", ex);
                    }
                });
            }
        }
        Visibility _AgregarFecha = Visibility.Collapsed;
        public Visibility AgregarFecha
        {
            get { return _AgregarFecha; }
            set
            {
                _AgregarFecha = value;
                OnPropertyChanged("AgregarFecha");
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

        bool _VerGrupos;
        public bool VerGrupos
        {
            get { return _VerGrupos; }
            set
            {
                _VerGrupos = value;
                OnPropertyChanged("VerGrupos");

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    try
                    {
                        if (_VerGrupos)
                            ListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro &&  w.ACTIVIDAD.ACTIVIDAD_EJE.Where(wh => wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA).Select(se => se.EJE.COMPLEMENTARIO).FirstOrDefault() == "N").Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
                        else
                            ListProgramas = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TIPO_PROGRAMA>>(() => new ObservableCollection<TIPO_PROGRAMA>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ACTIVIDAD.ACTIVIDAD_EJE.Where(wh => wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA).Select(se => se.EJE.COMPLEMENTARIO).FirstOrDefault() == "S").Select(s => s.ACTIVIDAD.TIPO_PROGRAMA).Distinct().OrderBy(o => o.NOMBRE).ToList()));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar programas", ex);
                    }
                }));
            }
        }

        #region [USUARIO DEPARTAMENTO]
        private List<int> departamentosUsuarios;
        #endregion
    }

    public class ListaDiaInternos
    {
        public GRUPO_PARTICIPANTE Entity { get; set; }
        public string FOLIO { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE { get; set; }
        public ObservableCollection<ListHorario> ListHorario { get; set; }
        public string State { get; set; }
        public short? Id_Actividad { get; set; }
        public short? Id_Programa { get; set; }
        public bool Revision { get; set; }
    }

    public class ListaManejoInternos
    {
        public GRUPO_PARTICIPANTE Entity { get; set; }
        public string FOLIO { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE { get; set; }
        public ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS> ListEstatusGrupoParticipante { get; set; }
        public short? SelectEstatus { get; set; }
        public string MOTIVO { get; set; }
    }
}
