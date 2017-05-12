using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class ManejoSancionesViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se definen las variables para el modulo manejo de cancelados/suspendidos
         * 
         */

        #region [Bandeja]
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
        ObservableCollection<ListaInternosSancionados> _ListParticipantes;
        public ObservableCollection<ListaInternosSancionados> ListParticipantes
        {
            get { return _ListParticipantes; }
            set
            {
                _ListParticipantes = value;
                OnPropertyChanged("ListParticipantes");
            }
        }

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

                        if (value.HasValue)
                            ProgramasLoad(value.Value);
                        else
                            ListProgramas = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error seleccionar eje", ex);
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
                        _SelectedActividad = value;
                        OnPropertyChanged("SelectedActividad");

                        if (value.HasValue)
                            ListGrupo = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() => new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.GRUPO_PARTICIPANTE.Where(wh => wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => whe.ESTATUS == null && whe.GRUPO_PARTICIPANTE.ESTATUS == 2).Any()).Any() && w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == value.Value && w.ID_ESTATUS_GRUPO == 1).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList()));
                        else
                            ListGrupo = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar actividad", ex);
                    }
                }));
            }
        }
        short _SelectedGrupo;
        public short SelectedGrupo
        {
            get { return _SelectedGrupo; }
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
                                OnPropertyValidateChanged("SelectedGrupo");
                                return;
                            }
                        }
                        _SelectedGrupo = value;
                        OnPropertyChanged("SelectedGrupo");

                        if (value > 0)
                            ListaInternosAEvaluar(value);
                        else
                            ListParticipantes = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar grupo", ex);
                    }
                }));
            }
        }

        ListaInternosSancionados _SelectedParticipante;
        public ListaInternosSancionados SelectedParticipante
        {
            get { return _SelectedParticipante; }
            set
            {
                _SelectedParticipante = value;
                OnPropertyValidateChanged("SelectedParticipante");

                if (value != null)
                {
                    ObtenerCursosAprovadosTotalHoras(value.Entity.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE, value.Entity.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE.Count);
                    ListParticipantes.Where(w => w == value).FirstOrDefault().APROBADO = value.APROBADO;
                    EnabledNota = true;
                    OnPropertyValidateChanged("ImagenParticipante");
                }
                else
                {
                    EnabledNota = false;
                    HorasTratamiento = "0/0";
                    AvanceTratamiento = "0/0";
                    MaxValueProBar = 1;
                    CantidadActividadesAprovadas = 0;
                    EnabledNota = false;
                    OnPropertyValidateChanged("ImagenParticipante");
                }
            }
        }

        public byte[] ImagenParticipante
        {
            get
            {
                return SelectedParticipante != null ? SelectedParticipante.ImagenParticipante.Length != new Imagenes().getImagenPerson().Length ? SelectedParticipante.ImagenParticipante : SelectedParticipante.ImagenParticipante.Length != new Imagenes().getImagenPerson().Length ? SelectedParticipante.ImagenParticipante : new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson();
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
        bool _EnabledNota;
        public bool EnabledNota
        {
            get { return _EnabledNota; }
            set
            {
                _EnabledNota = value;
                OnPropertyChanged("EnabledNota");
            }
        }
        #endregion

        #region[Suspendidos]
        ObservableCollection<EJE> _SuspenListEjes;
        public ObservableCollection<EJE> SuspenListEjes
        {
            get { return _SuspenListEjes; }
            set
            {
                _SuspenListEjes = value;
                OnPropertyChanged("SuspenListEjes");
            }
        }
        ObservableCollection<TIPO_PROGRAMA> _SuspenListProgramas;
        public ObservableCollection<TIPO_PROGRAMA> SuspenListProgramas
        {
            get { return _SuspenListProgramas; }
            set
            {
                _SuspenListProgramas = value;
                OnPropertyChanged("SuspenListProgramas");
            }
        }
        ObservableCollection<ACTIVIDAD> _SuspenListActividades;
        public ObservableCollection<ACTIVIDAD> SuspenListActividades
        {
            get { return _SuspenListActividades; }
            set
            {
                _SuspenListActividades = value;
                OnPropertyChanged("SuspenListActividades");
            }
        }
        ObservableCollection<GRUPO> _SuspenListGrupo;
        public ObservableCollection<GRUPO> SuspenListGrupo
        {
            get { return _SuspenListGrupo; }
            set
            {
                _SuspenListGrupo = value;
                OnPropertyChanged("SuspenListGrupo");
            }
        }
        ObservableCollection<ListaInternosSancionados> _SuspenListParticipantes;
        public ObservableCollection<ListaInternosSancionados> SuspenListParticipantes
        {
            get { return _SuspenListParticipantes; }
            set
            {
                _SuspenListParticipantes = value;
                OnPropertyChanged("SuspenListParticipantes");
            }
        }

        short? _SuspenSelectedEje;
        public short? SuspenSelectedEje
        {
            get { return _SuspenSelectedEje; }
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
                                OnPropertyChanged("SuspenSelectedEje");
                                return;
                            }
                        }

                        _SuspenSelectedEje = value;
                        OnPropertyChanged("SuspenSelectedEje");

                        if (value.HasValue)
                            ProgramasLoadSuspen(value.Value);
                        else
                            SuspenListProgramas = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar eje", ex);
                    }
                }));
            }
        }
        short? _SuspenSelectedPrograma;
        public short? SuspenSelectedPrograma
        {
            get { return _SuspenSelectedPrograma; }
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
                                OnPropertyChanged("SuspenSelectedPrograma");
                                return;
                            }
                        }
                        _SuspenSelectedPrograma = value;
                        OnPropertyChanged("SuspenSelectedPrograma");

                        if (value.HasValue)
                            ActividadesLoadSuspen(value.Value);
                        else
                            SuspenListActividades = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar programa", ex);
                    }
                }));
            }
        }
        short? _SuspenSelectedActividad;
        public short? SuspenSelectedActividad
        {
            get { return _SuspenSelectedActividad; }
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
                                OnPropertyChanged("SuspenSelectedActividad");
                                return;
                            }
                        }
                        _SuspenSelectedActividad = value;
                        OnPropertyChanged("SuspenSelectedActividad");

                        if (value.HasValue)
                            SuspenListGrupo = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() => new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.GRUPO_PARTICIPANTE.Where(wh => wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => whe.ID_ESTATUS == 4 && whe.ESTATUS == 1 && whe.RESPUESTA_FEC != null && whe.GRUPO_PARTICIPANTE.ESTATUS == 4).Any()).Any() && w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SuspenSelectedPrograma && w.ID_ACTIVIDAD == value.Value && w.ID_ESTATUS_GRUPO == 1).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList()));
                        else
                            SuspenListGrupo = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar actividad", ex);
                    }
                }));
            }
        }
        short _SuspenSelectedGrupo;
        public short SuspenSelectedGrupo
        {
            get { return _SelectedGrupo; }
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
                                OnPropertyValidateChanged("SuspenSelectedGrupo");
                                return;
                            }
                        }
                        _SuspenSelectedGrupo = value;
                        OnPropertyChanged("SuspenSelectedGrupo");

                        if (value > 0)
                            ListaInternosSuspendidos(value);
                        else
                            SuspenListParticipantes = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error ", ex);
                    }
                }));
            }
        }
        ListaInternosSancionados _SuspenSelectedParticipante;
        public ListaInternosSancionados SuspenSelectedParticipante
        {
            get { return _SuspenSelectedParticipante; }
            set
            {
                _SuspenSelectedParticipante = value;
                OnPropertyValidateChanged("SuspenSelectedParticipante");

                if (value != null)
                {
                    ObtenerCursosAprovadosTotalHorasSuspen(value.Entity.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE, value.Entity.GRUPO_PARTICIPANTE.INGRESO.GRUPO_PARTICIPANTE.Count);
                    SuspenListParticipantes.Where(w => w == value).FirstOrDefault().APROBADO = value.APROBADO;
                    EnabledNota = true;
                    OnPropertyValidateChanged("SuspenImagenParticipante");
                }
                else
                {
                    SuspenHorasTratamiento = "0/0";
                    SuspenAvanceTratamiento = "0/0";
                    SuspenMaxValueProBar = 1;
                    SuspenCantidadActividadesAprovadas = 0;
                    OnPropertyValidateChanged("SuspenImagenParticipante");
                }
            }
        }

        public byte[] SuspenImagenParticipante
        {
            get
            {
                return SuspenSelectedParticipante != null ? SuspenSelectedParticipante.ImagenParticipante.Length != new Imagenes().getImagenPerson().Length ? SuspenSelectedParticipante.ImagenParticipante : SuspenSelectedParticipante.ImagenParticipante.Length != new Imagenes().getImagenPerson().Length ? SuspenSelectedParticipante.ImagenParticipante : new Imagenes().getImagenPerson() : new Imagenes().getImagenPerson();
            }
        }
        int _SuspenMaxValueProBar = 1;
        public int SuspenMaxValueProBar
        {
            get { return _SuspenMaxValueProBar; }
            set
            {
                _SuspenMaxValueProBar = value;
                OnPropertyChanged("SuspenMaxValueProBar");
            }
        }
        int _SuspenCantidadActividadesAprovadas = 0;
        public int SuspenCantidadActividadesAprovadas
        {
            get { return _SuspenCantidadActividadesAprovadas; }
            set
            {
                _SuspenCantidadActividadesAprovadas = value;
                OnPropertyChanged("SuspenCantidadActividadesAprovadas");
            }
        }
        string _SuspenHorasTratamiento = "0/0";
        public string SuspenHorasTratamiento
        {
            get { return _SuspenHorasTratamiento; }
            set
            {
                _SuspenHorasTratamiento = value;
                OnPropertyChanged("SuspenHorasTratamiento");
            }
        }
        string _SuspenAvanceTratamiento = "0/0";
        public string SuspenAvanceTratamiento
        {
            get { return _SuspenAvanceTratamiento; }
            set
            {
                _SuspenAvanceTratamiento = value;
                OnPropertyChanged("SuspenAvanceTratamiento");
            }
        }
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

        private bool _grupoEnabled;
        public bool GrupoEnabled
        {
            get { return _grupoEnabled; }
            set { _grupoEnabled = value; OnPropertyChanged("GrupoEnabled"); }
        }

        private bool _tabSuspendidoEnabled;
        public bool TabSuspendidoEnabled 
        {
            get { return _tabSuspendidoEnabled; }
            set { _tabSuspendidoEnabled = value; OnPropertyChanged("TabSuspendidoEnabled"); }
        }
        #endregion

        bool cancellingTabSelectionChange;
        MahApps.Metro.Controls.MetroAnimatedSingleRowTabControl Tab;
        bool _SelectedTabSuspendidos;
        public bool SelectedTabSuspendidos
        {
            get { return _SelectedTabSuspendidos; }
            set
            {
                _SelectedTabSuspendidos = value;
                OnPropertyChanged("SelectedTabSuspendidos");
            }
        }
        string varauxSentencia;

        #region [USUARIO DEPARTAMENTO]
        private List<int> departamentosUsuarios;
        #endregion
    }

    public class ListaInternosSancionados
    {
        public GRUPO_PARTICIPANTE_CANCELADO Entity { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public byte[] ImagenParticipante { get; set; }
        public short? ID_ANIO { get; set; }
        public int? ID_IMPUTADO { get; set; }
        public string UBICACION { get; set; }
        public string PLANIMETRIA { get; set; }
        public string PLANIMETRIACOLOR { get; set; }
        public string RESTANTE { get; set; }
        public string SENTENCIA { get; set; }
        public bool APROBADO { get; set; }
        public string MOTIVO { get; set; }
        public string ASISTENCIA { get; set; }
        public string MOVIMIENTO { get; set; }

        public DateTime? RESPUESTA_FECHA { get; set; }
        public string RESPUESTA_TECNICA { get; set; }
        public bool REACTIVAR { get; set; }
    }
}
