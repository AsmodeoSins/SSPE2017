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
    partial class ProgramacionVisitaEdificioViewModel : ValidationViewModelBase
    {
        public ProgramacionVisitaEdificioViewModel() { }

        private async void Load_Window(ProgramacionVisitaEdificioView Window)
        {
            try
            {
                   
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    CargarDatos();
                    ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                    EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        public async void FiltrarClick(Object obj)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private bool ValidacionesGuardar()
        {
            try
            {
                var programacion = new cVisitaEdificio().ObtenerTodos(GlobalVar.gCentro, (short)SelectDiaVisita, "0");//"1");
                if (programacion != null)
                {

                    int hi = 0, hf = 0,ci = 0,cf = 0;
                    int i = int.Parse(HoraEntrada);
                    int f = int.Parse(HoraSalida);
                    int i2 = int.Parse(selectCeldaInicio);
                    int f2 = int.Parse(selectCeldaFin);
                    foreach (var p in programacion)
                    {
                        if (!string.IsNullOrEmpty(p.HORA_INI) && !string.IsNullOrEmpty(p.HORA_FIN))
                        {
                            hi = int.Parse(p.HORA_INI);
                            hf = int.Parse(p.HORA_FIN);
                            if ((i >= hi && i <= hf) || (f >= hi && f <= hf))
                            {
                                ci = int.Parse(p.CELDA_INICIO);
                                cf = int.Parse(p.CELDA_FINAL);
                                if ((i2 >= ci && i2 <= cf) || (f2 >= ci && f2 <= cf))
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Validación!", "Este rango de celdas y horas se empalma con uno ya registrado.");
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar.", ex);
            }
            return false;
        }

        bool ValidacionesVisitasPorEdificio()
        {
            if (ListVisitasPorEdificio.Where(w => w.ID_CENTRO == List_Edificio.Where(wh => wh.ID_EDIFICIO == SelectEdificio).FirstOrDefault().ID_CENTRO &&
                                              w.ID_EDIFICIO == SelectEdificio.Value && w.ID_SECTOR == SelectSector.Value && w.ID_TIPO_VISITA == SelectTipoVisita &&
                                              w.CELDA_INICIO == SelectCeldaInicio && w.CELDA_FINAL == SelectCeldaFin).Any())
            {
                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ya existe un registro con los mismos datos.");
                return true;
            }
            var lista = CompararCeldas(List_Edificio.Where(wh => wh.ID_EDIFICIO == SelectEdificio).FirstOrDefault().ID_CENTRO, SelectEdificio.Value,
                SelectSector.Value, SelectCeldaInicio, SelectCeldaFin);
            foreach (var item in ListVisitasPorEdificio)
            {
                foreach (var itm in CompararCeldas(item.ID_CENTRO, item.ID_EDIFICIO, item.ID_SECTOR, item.CELDA_INICIO, item.CELDA_FINAL))
                {
                    if (lista.Where(w => w.ID_CELDA == itm.ID_CELDA && w.ID_EDIFICIO == itm.ID_EDIFICIO && w.ID_SECTOR == itm.ID_SECTOR && w.ID_CENTRO == itm.ID_CENTRO && item.ID_TIPO_VISITA == SelectTipoVisita).Any())
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Este rango de celdas se empalma con uno ya registrado.");
                        return true;
                    }
                }
            }
            return false;
        }

        public ObservableCollection<CELDA> CompararCeldas(short centro, short edificio, short sector, string inicio, string fin)
        {
            var band = false;
            var listaAuxiliar = new ObservableCollection<CELDA>();
            var lista = new cCelda().ObtenerPorSector(sector, edificio, centro).OrderBy(o => o.ID_CELDA);
            foreach (var item in lista.ToList())
            {
                if (item.ID_CELDA == inicio)
                    band = true;
                if (band)
                    listaAuxiliar.Add(item);
                if (item.ID_CELDA == fin)
                    band = false;
            }
            return listaAuxiliar;
        }

        public async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "actualizar":
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                            EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        });
                        break;
                    case "guardar_visita_edificio":
                        if (HasError())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación!", string.Format("Validar el campo: {0}.", base.Error));
                            break;
                        }
                        //Valida Horas
                        int inicio = int.Parse(HoraInicio + MinutoInicio);
                        int fin = int.Parse(HoraFin + MinutoFin);
                        if (inicio > fin)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación!", "La hora de inicio debe ser menor o igual a la hora final");
                            break;
                        }
                        //if (ValidacionesVisitasPorEdificio())
                        if (!ValidacionesGuardar())
                        {
                            break;
                        }
                        if (new cVisitaEdificio().Insertar(new VISITA_EDIFICIO
                        {
                            ID_TIPO_VISITA = SelectTipoVisita.Value,
                            ID_CENTRO = List_Edificio.Where(w => w.ID_EDIFICIO == SelectEdificio).FirstOrDefault().ID_CENTRO,
                            ID_EDIFICIO = SelectEdificio.Value,
                            ID_SECTOR = SelectSector.Value,
                            CELDA_INICIO = SelectCeldaInicio,
                            CELDA_FINAL = SelectCeldaFin,
                            ESTATUS = "0",
                            FECHA_ALTA = Fechas.GetFechaDateServer,
                            DIA = (short)SelectDiaVisita,
                            HORA_FIN = HoraInicio + MinutoInicio, //HoraSalida,
                            HORA_INI = HoraFin + MinutoFin,//HoraEntrada,
                            ID_AREA = SelectArea
                        }))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Éxito!", "Guardado con éxito.");
                            ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(GlobalVar.gCentro,
                                SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                            EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            LimpiarCampos();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EDIFICIO);
                            break;
                        }

                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error al guardar los datos de la visita.");
                        break;
                    case "borrar_visita_edificio":
                        if (SelectVisitaEdificio == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una visita.");
                            return;
                        }

                        if (await new Dialogos().ConfirmarEliminar("Advertencia!", "Esta seguro que desea borrar esta visita?") == 1)
                        {
                            if (new cVisitaEdificio().Actualizar(new VISITA_EDIFICIO
                            {
                                CELDA_FINAL = SelectVisitaEdificio.CELDA_FINAL,
                                CELDA_INICIO = SelectVisitaEdificio.CELDA_INICIO,
                                DIA = SelectVisitaEdificio.DIA,
                                ESTATUS = "1",
                                FECHA_ALTA = SelectVisitaEdificio.FECHA_ALTA,
                                ID_CENTRO = SelectVisitaEdificio.ID_CENTRO,
                                ID_CONSEC = SelectVisitaEdificio.ID_CONSEC,
                                ID_EDIFICIO = SelectVisitaEdificio.ID_EDIFICIO,
                                ID_SECTOR = SelectVisitaEdificio.ID_SECTOR,
                                ID_TIPO_VISITA = SelectVisitaEdificio.ID_TIPO_VISITA,
                                HORA_INI = SelectVisitaEdificio.HORA_INI,
                                HORA_FIN = SelectVisitaEdificio.HORA_FIN
                            }))
                            {
                                (new Dialogos()).ConfirmacionDialogo("Éxito!", "Guardado con éxito.");
                                ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(/*4*/GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                                EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Error al guardar.");
                        }
                        break;
                    case "cancelar_visita_edificio":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EDIFICIO);
                        break;
                    case "agregar_visita_edificio":
                        LimpiarCampos();
                        SetValidaciones();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_VISITA_EDIFICIO);
                        break;
                    case "filtrar":
                        ListVisitasPorEdificio = new ObservableCollection<VISITA_EDIFICIO>(new cVisitaEdificio().ObtenerTodosActivos(/*4*/GlobalVar.gCentro, SelectFechaFiltro == -1 ? new Nullable<int>() : SelectFechaFiltro));
                        EmptyVisible = ListVisitasPorEdificio.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        break;
                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ProgramacionVisitaEdificioView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ProgramacionVisitaEdificioViewModel();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ejecutar acción.", ex);
            }
        }

        bool HasError()
        {
            return HasErrors;
        }

        void GetDatosVisitaSeleccionada()
        {
            if (SelectVisitaEdificio == null)
            {
                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una visita.");
                return;
            }
            SelectDiaVisita = SelectVisitaEdificio.DIA.HasValue ? SelectVisitaEdificio.DIA.Value : (short)-1;
            SelectEdificio = SelectVisitaEdificio.ID_EDIFICIO;
            SelectSector = SelectVisitaEdificio.ID_SECTOR;
            SelectCeldaInicio = SelectVisitaEdificio.CELDA_INICIO;
            SelectCeldaFin = SelectVisitaEdificio.CELDA_FINAL;
            SelectTipoVisita = SelectVisitaEdificio.ID_TIPO_VISITA;
        }

        void LimpiarCampos()
        {
            try
            {
                SelectDiaVisita = -1;
                SelectTipoVisita = -1;
                SelectEdificio = -1;
                List_Sector = new ObservableCollection<SECTOR>();
                //                List_Sector.Insert(0, new SECTOR(){ ID_SECTOR = -1, DESCR = "SELECCIONE"});
                SelectSector = -1;
                List_Celda = new ObservableCollection<CELDA>();
                //              List_Celda.Insert(0, new CELDA(){ ID_CELDA = "SELECCIONE"});
                SelectCeldaInicio = SelectCeldaFin = "SELECCIONE";
                SelectArea = -1;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    List_Sector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    List_Celda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });

                    //Limpiar
                    HoraInicio = "07";
                    MinutoInicio = "00";

                    HoraFin = "19";
                    MinutoFin = "00";
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar campos.", ex);
            }
            #region comentado
            //SelectCeldaFin = SelectCeldaInicio = null;
            //SelectSector = SelectEdificio = -1;
            //SelectDiaVisita = 7;
            //SelectTipoVisita = Parametro.ID_TIPO_VISITA_FAMILIAR;
            #endregion
        }

        #region LLENAR_COMBOS
        void CargarDatos()
        {
            try
            {
                ListVisitaDia = new ObservableCollection<VISITA_DIA>(new cVisitaDia().ObtenerTodos().OrderBy(o => o.ID_DIA == 7 ? 1 : 2));
                ListVisitaDiaAlta = new ObservableCollection<VISITA_DIA>(ListVisitaDia);

                List_Tipo = new ObservableCollection<TIPO_VISITA>(new cTipoVisita().ObtenerTodos().OrderBy(w => w.DESCR));

                List_Edificio = new ObservableCollection<EDIFICIO>(new cEdificio().ObtenerTodos(string.Empty, 2, GlobalVar.gCentro).OrderBy(w => w.DESCR));

                List_Sector = new ObservableCollection<SECTOR>();

                List_Celda = new ObservableCollection<CELDA>();

                ListAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos().OrderBy(w => w.DESCR));

                //var aux = new cVisitaDia().ObtenerTodos().OrderBy(o => o.ID_DIA == 7 ? 1 : 2).ToList();
                //ListVisitaDia = new ObservableCollection<VISITA_DIA>(aux);
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    //ListAreas.Add(new AREA { ID_AREA = -1, DESCR = "SELECCIONE" });
                    //SelectArea = -1;
                    ListVisitaDia.Insert(0, new VISITA_DIA { ID_DIA = -1, DESCR = "TODOS" });
                    ListVisitaDiaAlta.Insert(0, new VISITA_DIA { ID_DIA = -1, DESCR = "SELECCIONE" });
                    //SelectFechaFiltro = -1;
                    ////ListVisitaDiaAlta = new ObservableCollection<VISITA_DIA>(aux);
                    //SelectDiaVisita = 7;
                    List_Tipo.Insert(0, new TIPO_VISITA() { ID_TIPO_VISITA = -1, DESCR = "SELECCIONE" });
                    List_Edificio.Insert(0, new EDIFICIO() { ID_EDIFICIO = -1, DESCR = "SELECCIONE" });
                    List_Sector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    List_Celda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
                    ListAreas.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar.", ex);
            }
        }


        #region Permisos
        ///CREADO POR ESTRUCTURA DE PERFIL DE TRABAJO SOCIAL
        //private void ConfiguraPermisos()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.PROGRAMACION_VISITA_POR_EDIFICIO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
        //        if (permisos.Any())
        //        {
        //            foreach (var p in permisos)
        //            {
        //                if (p.INSERTAR == 1)
        //                    PInsertar = true;
        //                if (p.EDITAR == 1)
        //                    PEditar = true;
        //                if (p.CONSULTAR == 1)
        //                    PConsultar = true;
        //                if (p.IMPRIMIR == 1)
        //                    PImprimir = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        #endregion

        async void obtenerSectores(short paramEdificio)
        {
            try
            {
                List_Sector = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SECTOR>>(() =>
                    new ObservableCollection<SECTOR>(new cSector().ObtenerTodos(string.Empty, 2, GlobalVar.gCentro, paramEdificio).OrderBy(o => o.DESCR)));
                List_Sector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar sectores.", ex);
            }
        }

        async void obtenerCeldas(short paramEdificio, short paramSector)
        {
            try
            {
                List_Celda = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<CELDA>>(() =>
                    new ObservableCollection<CELDA>(new cCelda().ObtenerPorSector(paramSector, paramEdificio, GlobalVar.gCentro).OrderBy(o => o.ID_CELDA)));
                List_Celda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar celdas.", ex);
            }
            
        }
        #endregion
    }
}
