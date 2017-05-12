using ControlPenales.Controls.Calendario;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    class AgendasViewModel : ViewModelBase
    {
        public AgendasViewModel(AGENDA_ACTIVIDAD_LIBERTAD AAL) 
        {
            aal = AAL;
        }

        #region Valiables
        private AGENDA_ACTIVIDAD_LIBERTAD aal;

        CalendarioView Calendario;
        DataGrid DatagridHorario;
        
        private string bindCmdDayClick = string.Empty;
        public string BindCmdDayClick
        {
            get { return bindCmdDayClick; }
            set { bindCmdDayClick = value; RaisePropertyChanged("BindCmdDayClick"); }
        }

        private bool agregarHora;
        public bool AgregarHora
        {
            get { return agregarHora; }
            set { agregarHora = value; OnPropertyChanged("AgregarHora"); }
        }

        private AGENDA_ACT_LIB_DETALLE AgendaDetalle;

        private bool _ChkAsistencia;
        public bool ChkAsistencia
        {
            get { return _ChkAsistencia; }
            set { _ChkAsistencia = value; OnPropertyChanged("ChkAsistencia"); }
        }

        private string observacion;
        public string Observacion
        {
            get { return observacion; }
            set { observacion = value; OnPropertyChanged("Observacion"); }
        }

        private string fechaTitulo;
        public string FechaTitulo
        {
            get { return fechaTitulo; }
            set { fechaTitulo = value; OnPropertyChanged("FechaTitulo"); }
        }
        #endregion

        #region Eventos
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        public ICommand ManejoGruposLoading
        {
            get { return new DelegateCommand<AgendasView>(ManejoGruposLoad); }
        }

        public ICommand CmdAbrirAgenda
        {
            get { return new DelegateCommand<object>(AbrirAgenda); }
        }
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "asistencia":
                    GuardarAsistencia();
                    break;
            }
        }
        
        private void ManejoGruposLoad(AgendasView obj)
        {
            try
            {
                BindCmdDayClick = "CmdAbrirAgenda";
                Calendario = obj.Calendario;
                DatagridHorario = PopUpsViewModels.MainWindow.EditarFechaView.DG_HorarioDia;
                CargarCalendario();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar manejo de grupos", ex);
            }
        }
      
        private async void AbrirAgenda(object obj)
        {
            try
            {
                var gh = (GRUPO_HORARIO)obj;
                if (gh != null)
                {
                    AgregarHora = true;
                    if (gh.HORA_INICIO != null)
                        FechaTitulo = string.Format("{0:dd/MM/yyyy}", gh.HORA_INICIO.Value);
                    else
                        FechaTitulo = string.Empty;

                    AgendaDetalle = aal.AGENDA_ACT_LIB_DETALLE.FirstOrDefault(w => w.FECHA.Value.Date == gh.HORA_INICIO.Value.Date);
                }
                //ListArea = ListArea ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<AREA>>(() => new ObservableCollection<AREA>(new cArea().GetData().Where(w => w.ID_TIPO_AREA != 5)));
                //ListEstatusGrupo = ListEstatusGrupo ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<GRUPO_HORARIO_ESTATUS>>(() => new ObservableCollection<GRUPO_HORARIO_ESTATUS>(new cGrupoHorarioEstatus().GetData()));

                //SelectedArea = null;
                //SelectedEstatusGrupo = 1;
                //EditFechaInicio = null;
                //EditFechaFin = null;
                //SelectedFecha = null;
                //await TaskEx.Delay(1);
                //EntityUpdate = null;
                //AgregarFecha = Visibility.Collapsed;

                //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EDITAR_FECHA);

                //if (obj == null)
                //    return;

                //// se valida de que tipo es el objeto
                //if (obj is GRUPO_HORARIO)
                //{
                //    EntityUpdate = (GRUPO_HORARIO)obj;

                //    SelectedArea = EntityUpdate.ID_AREA;
                //    SelectedEstatusGrupo = EntityUpdate.ESTATUS;
                //    EditFechaInicio = EntityUpdate.HORA_INICIO;
                //    EditFechaFin = EntityUpdate.HORA_TERMINO;

                //    SelectedFecha = EntityUpdate.HORA_INICIO.Value.Date;
                //}
                //else
                //    SelectedFecha = (DateTime)obj;

                //await TaskEx.Delay(1);
                //if (ListInternosDia == null)
                //    await TaskEx.Delay(1);

                //DatagridHorario.ItemsSource = ListInternosDia.Where(w => w.ListHorario.Any());
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir agenda", ex);
            }
        }
        #endregion

        #region Calendario
        void CargarCalendario(short? anio = null,short? mes = null)
        {
            try
            {
                var lg = new List<GRUPO_HORARIO>();
                aal = new cAgendaActividadLibertad().Obtener((int)aal.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                var res = aal.AGENDA_ACT_LIB_DETALLE.OrderBy(w => w.FECHA);
                if (res != null)
                {
                    foreach (var r in res)
                    {
                        lg.Add(new GRUPO_HORARIO() { HORA_INICIO = r.HORA_INICIO, HORA_TERMINO = r.HORA_FIN, ESTATUS = (short)r.ASISTENCIA, ID_TIPO_PROGRAMA = (short)r.ASISTENCIA });
                    }
                    if (lg != null)
                    {
                        if (lg.Count == 0)
                        {
                            Calendario.DisposeCalendario();
                            return;
                        }
                        var hoy = Fechas.GetFechaDateServer;
                        Calendario.SelectedMes = mes.HasValue ? mes.Value : hoy.Month;
                        Calendario.SelectedAnio = anio.HasValue ? anio.Value : hoy.Year;

                        Calendario.CrearCalendarioLiberado(
                            lg.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value,
                            lg.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value,
                            lg);
                        Calendario.DiasAgendados = new ObservableCollection<GRUPO_HORARIO>(lg);
                    }

                }
                else
                {
                    Calendario.DisposeCalendario();
                }
            
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Cargar Calendario", ex);
            }
        }
        #endregion

        #region Asistencia
        private async void GuardarAsistencia()
        {
            try
            {
                if (AgendaDetalle != null)
                {
                    var Lista = new List<AGENDA_ACT_LIB_DETALLE>();
                    #region Valida Anteriores Programadas
                    aal = new cAgendaActividadLibertad().Obtener((int)aal.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                    var lista = aal.AGENDA_ACT_LIB_DETALLE.Where(w => w.ID_DETALLE < AgendaDetalle.ID_DETALLE && w.ASISTENCIA == 0);
                    if (lista != null)
                    {
                        if (lista.Count() > 0)
                        {
                            if (await ConfirmarEliminar("Validación", "Tienes fechas anteriores sin asistencia, ¿Deseas ponerles falta?") == 1)
                            {
                                foreach (var l in lista)
                                {
                                    Lista.Add(new AGENDA_ACT_LIB_DETALLE()
                                    {
                                        ID_AGENDA_ACTIVIDAD_LIBERTAD = l.ID_AGENDA_ACTIVIDAD_LIBERTAD,
                                        ID_DETALLE = l.ID_DETALLE,
                                        FECHA = l.FECHA,
                                        HORA_INICIO = l.HORA_INICIO,
                                        HORA_FIN = l.HORA_FIN,
                                        ASISTENCIA = 2,
                                        OBSERVACION = l.OBSERVACION
                                    });
                                }
                            }
                        }
                    }
                    #endregion

                    var det = new AGENDA_ACT_LIB_DETALLE();
                    det.ID_AGENDA_ACTIVIDAD_LIBERTAD = AgendaDetalle.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                    det.ID_DETALLE = AgendaDetalle.ID_DETALLE;
                    det.FECHA = AgendaDetalle.FECHA;
                    det.HORA_INICIO = AgendaDetalle.HORA_INICIO;
                    det.HORA_FIN = AgendaDetalle.HORA_FIN;

                    if (ChkAsistencia)
                        det.ASISTENCIA = 1;//Asistencia
                    else
                        if (!ChkAsistencia && string.IsNullOrEmpty(Observacion))
                            det.ASISTENCIA = 2;//Falta
                        else
                            det.ASISTENCIA = 3;//Falta Justificada
                    det.OBSERVACION = Observacion;
                    if (new cAgendaActividadLibertadDetalle().Actualizar(det, Lista))
                    {
                        ChkAsistencia = false;
                        Observacion = string.Empty;
                        AgregarHora = false;
                        CargarCalendario();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }
        #endregion
       

        #region Mensajes
        public async Task<int> ConfirmarEliminar(string sTitulo, string sMensaje)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Si",
                NegativeButtonText = "No"

            };
            var metro = Application.Current.Windows[(Application.Current.Windows.Count - 1)] as MetroWindow;
            MessageDialogResult result = await metro.ShowMessageAsync(sTitulo, sMensaje,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
                return 1;
            else if (result == MessageDialogResult.Negative)
                return 0;
            return -1;
        }

        #endregion
    }
}
