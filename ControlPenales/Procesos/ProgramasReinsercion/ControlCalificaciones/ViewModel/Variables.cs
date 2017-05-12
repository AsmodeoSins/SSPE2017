using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    partial class ControlCalificacionesViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase que se describe todas las propiedades para el modulo crontrol calificaciones
         * 
         * la propiedad importante es: SelectedParticipante
         * 
         */

        #region [propiedades]
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
        ObservableCollection<ListaInternosCalificacion> _ListParticipantes;
        public ObservableCollection<ListaInternosCalificacion> ListParticipantes
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
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar EJE", ex);
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
                            ListGrupo = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO>>(() => new ObservableCollection<GRUPO>(new cGrupo().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == SelectedPrograma && w.ID_ACTIVIDAD == value.Value && w.ID_ESTATUS_GRUPO == 1).Select(s => s).Distinct().OrderBy(o => o.DESCR).ToList()));
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
                            ListaInternosCalificacion(value);
                        else
                            ListParticipantes = null;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error ", ex);
                    }
                }));
            }
        }

        ListaInternosCalificacion _SelectedParticipante;
        public ListaInternosCalificacion SelectedParticipante
        {
            get { return _SelectedParticipante; }
            set
            {
                _SelectedParticipante = value;
                OnPropertyValidateChanged("SelectedParticipante");

                if (value != null)
                {
                    ObtenerCursosAprovadosTotalHoras(value.Entity.INGRESO.GRUPO_PARTICIPANTE, value.Entity.INGRESO.GRUPO_PARTICIPANTE.Count);
                    ListParticipantes.Where(w => w == value).FirstOrDefault().ACREDITO = value.ACREDITO;
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
        string varauxSentencia;
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

        private bool _enabledEditGrupo;
        public bool EnabledEditGrupo
        {
            get { return _enabledEditGrupo; }
            set { _enabledEditGrupo = value; OnPropertyChanged("EnabledEditGrupo"); }
        }
        #endregion

        #region [USUARIO DEPARTAMENTO]
        private List<int> departamentosUsuarios;
        #endregion
    }

    public class ListaInternosCalificacion
    {
        public GRUPO_PARTICIPANTE Entity { get; set; }
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
        public bool ACREDITO { get; set; }
        public string DELITO { get; set; }
        public string ASISTENCIA { get; set; }
        public string NOTA_TECNICA { get; set; }
    }
}
