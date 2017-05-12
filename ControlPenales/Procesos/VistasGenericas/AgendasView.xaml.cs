using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Office.Interop.Word;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TXTextControl;

namespace ControlPenales
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class AgendasView : MahApps.Metro.Controls.MetroWindow
    {
        #region Agenda
        #endregion

        #region Variables
        private PROCESO_LIBERTAD pl;
        //CalendarioView Calendario;
        //DataGrid DatagridHorario;
        #endregion
        public AgendasView()
        {
            //pl = PL;
            InitializeComponent();
            //BindCmdDayClick = "CmdAbrirAgenda";
            //CargarCalendario();
        }

        private void editor_Loaded(object sender, EventArgs e) 
        {
            CargarCalendario();
        }

        #region Calendario
        void CargarCalendario()
        {
            try
            {
               var lg = new List<GRUPO_HORARIO>();
                if (pl != null)
                {
                    var res = new cAgendaActividadLibertadDetalle().ObtenerTodos(pl.AGENDA_ACTIVIDAD_LIBERTAD.Select(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD).ToList());
                    if(res != null)
                    {
                        foreach (var r in res)
                        {
                            lg.Add(new GRUPO_HORARIO() { HORA_INICIO = r.FECHA, HORA_TERMINO = r.FECHA,ESTATUS = 1 });
                        }
                        if (lg != null)
                        {
                            if (lg.Count == 0)
                            {
                                Calendario.DisposeCalendario();
                                return;
                            }
                            var hoy = Fechas.GetFechaDateServer;
                            Calendario.SelectedMes = hoy.Month;
                            Calendario.SelectedAnio = hoy.Year;
                           
 

                            Calendario.CrearCalendario(
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
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Cargar Calendario", ex);
            }
        }
        #endregion

        #region Asistencia
        private async void AbrirAgenda(object obj)
        {
            try
            {
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
    }
}
