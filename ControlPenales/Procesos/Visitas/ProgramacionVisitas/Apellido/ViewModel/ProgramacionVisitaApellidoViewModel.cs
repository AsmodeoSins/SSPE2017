using Andora.UserControlLibrary;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramacionVisitaApellidoViewModel : ValidationViewModelBase
    {
        public ProgramacionVisitaApellidoViewModel() { }

        private async void Load_Window(ProgramacionVisitaApellidoView Window)
        {
            try
            {
                

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    CargarDatos();
                    ListVisitasPorApellido = new ObservableCollection<VISITA_APELLIDO>(new cVisitaApellido().ObtenerTodosActivos(
                        GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro).OrderBy(o => o.ID_DIA == 1 ? 2 : 1).ThenBy(T => T.ID_DIA).ThenBy(t => t.LETRA_INICIAL));
                    EmptyVisible = ListVisitasPorApellido.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        void CargarDatos()
        {
            var x = new ObservableCollection<VISITA_DIA>(new cVisitaDia().ObtenerTodos().OrderBy(o => o.ID_DIA == 7 ? 2 : 1).ToList());
            ListVisitaDiaFiltro = new RangeEnabledObservableCollection<VISITA_DIA>();
            ListVisitaDia = x;
            ListTipoVisita = new ObservableCollection<TIPO_VISITA>(new cTipoVisita().ObtenerTodos());
            ListAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
            ListLetras = new ObservableCollection<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                ListAreas.Add(new AREA { ID_AREA = -1, DESCR = "SELECCIONE" });
                SelectArea = -1;
                ListVisitaDia.Add(new VISITA_DIA { ID_DIA = -1, DESCR = "SELECCIONE" });
                ListVisitaDiaFiltro.Add(new VISITA_DIA { ID_DIA = -1, DESCR = "TODOS" });
                ListVisitaDiaFiltro.InsertRange(x);
                ListTipoVisita.Insert(0, new TIPO_VISITA { ID_TIPO_VISITA = -1, DESCR = "SELECCIONE" });
                //ListLetras.Insert(0, string.Empty);
                SelectLetraInicial = SelectLetraFinal = string.Empty;
                SelectTipoVisita = SelectFechaFiltro = -1;
                SelectDiaVisita = 1;
            }));
        }

        private bool HasError() { return base.HasErrors; }

        private bool ValidarConcatenacionDeLetraExistente()
        {
            //if (ListVisitasPorApellido.Where(w => w.ESTATUS == "0" && w.ID_TIPO_VISITA == SelectTipoVisita && (w.LETRA_INICIAL == SelectLetraInicial || w.LETRA_FINAL == SelectLetraInicial ||
            //    w.LETRA_INICIAL == SelectLetraFinal || w.LETRA_FINAL == SelectLetraFinal)).Any())

            var validacion = ListVisitasPorApellido.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ESTATUS == "0" && w.ID_DIA == SelectDiaVisita &&
                (((ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraInicial) &&
                ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraInicial))) ||
                 (ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraFinal) &&
                ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraFinal))));

            if (validacion != null)
            {
                int x = validacion.Count();
            }

            if (ListVisitasPorApellido.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ESTATUS == "0" && w.ID_DIA == SelectDiaVisita && 
                (((ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraInicial) &&
                ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraInicial))) ||
                 (ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraFinal) &&
                ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraFinal)))).Any())
            {
                return true;
            }
            //if (ListVisitasPorApellido.Where(w => w.ID_DIA == SelectDiaVisita && ((w.LETRA_INICIAL == SelectLetraInicial || 
            //    w.LETRA_FINAL == SelectLetraInicial) &&
            //    (w.LETRA_INICIAL == SelectLetraFinal ||
            //    w.LETRA_FINAL == SelectLetraFinal))).Any())
            //{
            //    return true;
            //}

            //if (ListVisitasPorApellido.Where(w => w.ID_TIPO_VISITA == SelectTipoVisita &&
            //    ((ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraInicial) &&
            //    ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraInicial)) ||
            //    (ListLetras.IndexOf(w.LETRA_INICIAL) <= ListLetras.IndexOf(SelectLetraFinal) &&
            //    ListLetras.IndexOf(w.LETRA_FINAL) >= ListLetras.IndexOf(SelectLetraFinal)))).Any())
            //{
            //    return true;
            //}
            return false;
        }

        public async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "actualizar":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        ListVisitasPorApellido = new ObservableCollection<VISITA_APELLIDO>(new cVisitaApellido().ObtenerTodosActivos(
                            GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro).OrderBy(o => o.ID_DIA == 1 ? 2 : 1).ThenBy(T => T.ID_DIA).ThenBy(t => t.LETRA_INICIAL));
                        EmptyVisible = ListVisitasPorApellido.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    });
                    break;
                case "guardar_visita_apellido":
                    if (HasError())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", string.Format("Error al validar el campo: {0}.", base.Error));
                        break;
                    }
                    //Validacion 
                    int inicio = int.Parse(HoraInicio + MinutoInicio);
                    int fin = int.Parse(HoraFin + MinutoFin);
                    if (inicio > fin)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación!", "La hora de inicio debe ser menor o igual a la hora final");
                        break;
                    }

                    if (ValidarConcatenacionDeLetraExistente())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Existe una concatenación con una letra seleccionada.");
                        break;
                    }
                    if (new cVisitaApellido().Insertar(new VISITA_APELLIDO
                    {
                        ID_CENTRO = GlobalVar.gCentro,
                        LETRA_INICIAL = _SelectLetraInicial,
                        LETRA_FINAL = SelectLetraFinal,
                        ESTATUS = "0",
                        ALTA_FEC = Fechas.GetFechaDateServer,
                        ID_DIA = SelectDiaVisita,
                        HORA_INI = HoraInicio + MinutoInicio,//HoraEntrada,
                        HORA_FIN = HoraFin + MinutoFin,//HoraSalida,
                        ID_TIPO_VISITA = SelectTipoVisita,
                        ID_AREA = SelectArea
                    }))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Éxito!", "Guardado con éxito.");
                        ListVisitasPorApellido = new ObservableCollection<VISITA_APELLIDO>(new cVisitaApellido().ObtenerTodosActivos(GlobalVar.gCentro,
                            SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro).OrderBy(o => o.ID_DIA == 1 ? 2 : 1).ThenBy(T => T.ID_DIA).ThenBy(t => t.LETRA_INICIAL));
                        EmptyVisible = ListVisitasPorApellido.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_APELLIDO);
                        break;
                    }

                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error al guardar los datos de la visita.");
                    break;
                case "borrar_visita_apellido":
                    if (SelectVisitaApellido == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una visita.");
                        return;
                    }

                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "Esta seguro que desea borrar esta visita?") == 1)
                    {
                        if (new cVisitaApellido().Actualizar(new VISITA_APELLIDO
                        {
                            LETRA_FINAL = SelectVisitaApellido.LETRA_FINAL,
                            LETRA_INICIAL = SelectVisitaApellido.LETRA_INICIAL,
                            ID_DIA = SelectVisitaApellido.ID_DIA,
                            ESTATUS = "1",
                            ALTA_FEC = SelectVisitaApellido.ALTA_FEC,
                            ID_CENTRO = SelectVisitaApellido.ID_CENTRO,
                            ID_CONSEC = SelectVisitaApellido.ID_CONSEC,
                            HORA_FIN = SelectVisitaApellido.HORA_FIN,
                            HORA_INI = SelectVisitaApellido.HORA_INI,
                            ID_TIPO_VISITA = SelectVisitaApellido.ID_TIPO_VISITA
                        }))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Éxito!", "Borrado con éxito.");
                            ListVisitasPorApellido = new ObservableCollection<VISITA_APELLIDO>(new cVisitaApellido().ObtenerTodosActivos(
                                GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro).OrderBy(o => o.ID_DIA == 1 ? 2 : 1).ThenBy(T => T.ID_DIA).ThenBy(t => t.LETRA_INICIAL));
                            EmptyVisible = ListVisitasPorApellido.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error al guardar.");
                    }
                    break;
                case "cancelar_visita_apellido":
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_APELLIDO);
                    break;
                case "agregar_visita_apellido":
                    SetValidaciones();
                    SelectLetraFinal = SelectLetraInicial = "A";
                    SelectTipoVisita = -1;
                    SelectArea = -1;
                    SelectDiaVisita = SelectFechaFiltro;
                    //HoraInicio = new DateTime().Date.AddHours(7);
                    //HoraFinal = new DateTime().Date.AddHours(19);
                    HoraInicio = "07";
                    MinutoInicio = "00";

                    HoraFin = "19";
                    MinutoFin = "00";
                    /*DateRangeSlider.LowValue = Convert.ToDateTime("1/1/0001 7:00:00 AM");
                    DateRangeSlider.HighValue = Convert.ToDateTime("1/1/0001 7:00:00 PM");*/
                    
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_APELLIDO);
                    break;
                case "filtrar":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        ListVisitasPorApellido = new ObservableCollection<VISITA_APELLIDO>(new cVisitaApellido().ObtenerTodosActivos(
                            GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro).OrderBy(o => o.ID_DIA == 1 ? 2 : 1).ThenBy(T => T.ID_DIA).ThenBy(t => t.LETRA_INICIAL));
                        EmptyVisible = ListVisitasPorApellido.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    });
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ProgramacionVisitaApellidoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ProgramacionVisitaApellidoViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

    }
}
