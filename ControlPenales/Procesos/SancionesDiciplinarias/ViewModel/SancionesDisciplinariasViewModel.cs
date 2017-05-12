using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class SancionesDisciplinariasViewModel : ValidationViewModelBase
    {
        public SancionesDisciplinariasViewModel()
        { }

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                        break;
                    }
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                    }
                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        break;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        return;
                    }
                    SelectedIngreso = SelectIngreso;
                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    if (IngresoAux != null)
                    {
                        SelectIngreso = IngresoAux;
                        IngresoAux = null;
                    }
                    if (SelectIngreso != null)
                    {
                        if (SelectedIngreso != null)
                            SelectIngreso = SelectedIngreso;
                        else
                            ImagenIngreso = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                //INcSIDENTE
                case "agregar_incidente":
                    if (!base.HasErrors)
                    {
                        GuardarIncidenteDB();
                        //AgregarIncidente();
                        //base.ClearRules();
                        //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos  requeridos.");
                    break;
                case "cancelar_incidente":
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    break;
                case "nuevo_incidente":
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    ObtenerCertificadoMedico();
                    if (ID_CERTIFICADO_MEDICO == 0)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Para agregar un nuevo incidete, requiere un certificado médico.");
                        return;
                    }
                    TituloPopUp = "Agregar Incidente";
                    SelectedIncidente = null;
                    LimpiarIncidente();
                    SetValidacionesIncidente();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    break;
                case "editar_incidente":
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    TituloPopUp = "Editar Incidente";
                    PopulateIncidente();
                    SetValidacionesIncidente();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    break;
                case "eliminar_incidente":
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    if (SelectedIncidente != null)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Validación!", "¿Esta seguro que desea eliminar este incidente?") == 1)
                        {
                            //EliminarIncidente();
                            EliminarIncidenteDB();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un incidente");
                    break;
                //SANCION
                case "guardar_sancionpopup":
                    if (!base.HasErrors)
                    {
                        GuardarSancionDB();
                        ////Validacion fechas
                        //if (FechaInicio.Value.Date > FechaFin.Value.Date)
                        //{ 
                        //    new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio debe ser menor o igual a la fecha final");
                        //    break;
                        //}
                        ////Validacion horas
                        //int inicio = int.Parse(HoraInicio + MinutoInicio);
                        //int fin = int.Parse(HoraFin + MinutoFin);
                        //if (inicio > fin)
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Validación", "La hora de inicio debe ser menor o igual a la hora final");
                        //    break;
                        //}

                        //AgregarSancion();
                        //base.ClearRules();
                        //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los campos obligatorios");

                    break;
                case "cancelar_sancion":
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    break;

                case "cancelar_sancionpopup":
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    break;

                case "nuevo_sancion":
                    if (SelectedIncidente != null)
                    {
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TituloPopUp = "Agregar Sanción";
                        //SancionSelected = null;
                        LimpiarSancion();
                        SetValidacionesSancion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar un incidente");
                    break;
                case "editar_sancion":
                    if (SancionSelected != null)
                    {
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        TituloPopUp = "Agregar Editar";
                        PopulateSancion();
                        SetValidacionesSancion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una sanción");
                    break;
                case "eliminar_sancion":
                    //EliminarSancion();
                     if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        break;
                    }
                    if (SancionSelected != null)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Validación!", "¿Esta seguro que desea eliminar esta sancion?") == 1)
                        {
                            //EliminarIncidente();
                            EliminarSancionDB();
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una sanción");
                    break;
                case "guardar_menu":
                    if (SelectedIngreso != null)
                    {
                        if (GuardarIncidente())
                            new Dialogos().ConfirmacionDialogo("Éxito", "La sanción fue guardada correctamente.");
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al guardar la sanción.");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                    break;
                case "buscar_menu":
                    IngresoAux = SelectIngreso;
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = null;
                    AnioBuscar = null;
                    SelectExpediente = null;
                    ListExpediente = null;
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = null;
                    AnioBuscar = null;
                    SelectExpediente = null;
                    ListExpediente = null;
                    ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new SancionesDisciplinariasView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new SancionesDisciplinariasViewModel();
                    break;
                case "reporte_menu":
                    ReporteParteInformativo(); 
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void ModelEnter(object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (!obj.GetType().Name.Equals("String"))
                    {

                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = textbox.Text;
                                    NombreD = NombreBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    break;
                            }
                        }

                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                if (string.IsNullOrEmpty(NombreD))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = NombreD;

                if (string.IsNullOrEmpty(PaternoD))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = PaternoD;

                if (string.IsNullOrEmpty(MaternoD))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = MaternoD;

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                if (ListExpediente.Count == 1)
                {
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                    }
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                            ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        return;
                    }
                    SelectedIngreso = SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    SeleccionaIngreso();
                }
                else
                {
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = NombreD = textbox.Text;
                                //AnioBuscar = AnioD;
                                //FolioBuscar = FolioD;
                                //ApellidoMaternoBuscar = MaternoD;
                                //ApellidoPaternoBuscar = PaternoD;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = PaternoD = textbox.Text;
                                //AnioBuscar = AnioD;
                                //FolioBuscar = FolioD;
                                //ApellidoMaternoBuscar = MaternoD;
                                //NombreBuscar = NombreD;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = MaternoD = textbox.Text;
                                //AnioBuscar = AnioD;
                                //FolioBuscar = FolioD;
                                //ApellidoPaternoBuscar = PaternoD;
                                //NombreBuscar = NombreD;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                //AnioBuscar = AnioD;
                                //ApellidoMaternoBuscar = MaternoD;
                                //NombreBuscar = NombreD;
                                //ApellidoPaternoBuscar = PaternoD;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                //FolioBuscar = FolioD;
                                //ApellidoMaternoBuscar = MaternoD;
                                //NombreBuscar = NombreD;
                                //ApellidoPaternoBuscar = PaternoD;
                                break;
                        }
                    }

                }
                //ListExpediente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar));
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
          }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<IMPUTADO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        private void SeleccionaIngreso()
        {
            //DATOS GENERALES
            AnioD = SelectIngreso.ID_ANIO;
            FolioD = SelectIngreso.ID_IMPUTADO;
            PaternoD = SelectIngreso.IMPUTADO.PATERNO.Trim();
            MaternoD = SelectIngreso.IMPUTADO.MATERNO.Trim();
            NombreD = SelectIngreso.IMPUTADO.NOMBRE.Trim();
            IngresosD = SelectIngreso.ID_INGRESO;
            if (SelectIngreso.CAMA != null)
            {
                UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                           SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Replace(" ", string.Empty),
                                           SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Replace(" ", string.Empty),
                                           SelectIngreso.CAMA.CELDA.ID_CELDA.Replace(" ", string.Empty),
                                           SelectIngreso.ID_UB_CAMA);
            }
            else
            {
                UbicacionD = string.Empty;
            }
            TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
            FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO.Value;
            ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
            if (SelectIngreso.ESTATUS_ADMINISTRATIVO != null)
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
            else
                EstatusD = string.Empty;
            //MOSTRAMOS LAS SANCIONES
            PopulateIncidenteListado();
        }

        private void PrepararListas()
        {

            if (LstIncidenteTipo == null)
            {
                LstIncidenteTipo = new ObservableCollection<INCIDENTE_TIPO>(new cIncidenteTipo().ObtenerTodas());
                LstIncidenteTipo.Insert(0, new INCIDENTE_TIPO() { ID_INCIDENTE_TIPO = -1, DESCR = "SELECCIONE" });
            }

            if (ListTipoSanciones == null)
            {
                ListTipoSanciones = new ObservableCollection<SANCION_TIPO>(new cSancionTipo().ObtenerTodas());
                ListTipoSanciones.Insert(0, new SANCION_TIPO() { ID_SANCION = -1, DESCR = "SELECCIONE" });
            }
        }

        private void SancionesDisciplinariasLoad(SancionesDisciplinariasView window = null)
        {
            PrepararListas();
            //SE CONSULTAN LOS PERMISOS DISPONIBLES PARA ESTA VENTANA
            ConfiguraPermisos();
        }
        #endregion

        #region Sanciones
        private void PopulateIncidenteListado()
        {
            if (SelectIngreso != null)
                LstIncidente = new ObservableCollection<INCIDENTE>(SelectIngreso.INCIDENTE);

            else
                LstIncidente = new ObservableCollection<INCIDENTE>();

            EmptyIncidente = LstIncidente.Count > 0 ? false : true;
            EmptySanciones = true;
        }

        private void PopulateIncidente()
        {
            if (SelectedIncidente != null)
            {
                FecIncidencia = SelectedIncidente.REGISTRO_FEC ?? new DateTime?();
                SelectedIncidenteTipo = LstIncidenteTipo.AsQueryable().Where(w => w.ID_INCIDENTE_TIPO == SelectedIncidente.ID_INCIDENTE_TIPO).FirstOrDefault();
                Motivo = SelectedIncidente.MOTIVO;
            }
        }

        private bool GuardarIncidente()
        {
            var lst = new List<INCIDENTE>();
            short indice = 1;
            if (LstIncidente != null)
                if (LstIncidente.Any())
                {

                    foreach (var i in LstIncidente)
                    {
                        var sanciones = new List<SANCION>(i.SANCION.Select(w =>
                            new SANCION()
                            {
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_INCIDENTE = indice,
                                ID_SANCION = w.ID_SANCION,
                                INICIA_FEC = w.INICIA_FEC,
                                TERMINA_FEC = w.TERMINA_FEC
                            }));
                        //var l = new List<SANCION>();
                        //foreach (var s in i.SANCION)
                        //{
                        //    l.Add(new SANCION()
                        //    {
                        //        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        //        ID_ANIO = SelectIngreso.ID_ANIO,
                        //        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        //        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        //        ID_INCIDENTE = indice,
                        //        ID_SANCION = s.ID_SANCION,
                        //        INICIA_FEC = s.INICIA_FEC,
                        //        TERMINA_FEC = s.TERMINA_FEC
                        //    });

                        //}
                        lst.Add(new INCIDENTE()
                        {
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_INCIDENTE = indice,
                            ID_INCIDENTE_TIPO = i.ID_INCIDENTE_TIPO,
                            REGISTRO_FEC = i.REGISTRO_FEC,
                            MOTIVO = i.MOTIVO,
                            SANCION = sanciones,
                            ESTATUS = "P"
                        });
                        indice++;
                    }
                }

            if (new cIncidente().Insertar(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, lst))
                return true;
            return false;
        }

        private void AgregarIncidente()
        {
            if (LstIncidente == null)
                LstIncidente = new ObservableCollection<INCIDENTE>();
            if (SelectedIncidente == null)//AGREGAR
            {
                LstIncidente.Add(new INCIDENTE()
                {
                    REGISTRO_FEC = FecIncidencia,
                    ID_INCIDENTE_TIPO = SelectedIncidenteTipo.ID_INCIDENTE_TIPO,
                    INCIDENTE_TIPO = SelectedIncidenteTipo,
                    MOTIVO = Motivo
                });
            }
            else//EDITAR
            {
                SelectedIncidente.REGISTRO_FEC = FecIncidencia;
                SelectedIncidente.ID_INCIDENTE_TIPO = SelectedIncidenteTipo.ID_INCIDENTE_TIPO;
                SelectedIncidente.INCIDENTE_TIPO = SelectedIncidenteTipo;
                SelectedIncidente.MOTIVO = Motivo;
                LstIncidente = new ObservableCollection<INCIDENTE>(LstIncidente);
            }

            EmptyIncidente = LstIncidente.Count > 0 ? false : true;
        }

        private void GuardarIncidenteDB()
        {
            try
            {
                var o = new INCIDENTE();
                //var ls = new List<SANCION>();
                //short i = 1;
                o.REGISTRO_FEC = FecIncidencia;
                o.ID_INCIDENTE_TIPO = SelectedIncidenteTipo.ID_INCIDENTE_TIPO; 
                o.MOTIVO = Motivo;
                if (SelectedIncidente == null)
                {
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    o.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    o.ID_ANIO = SelectIngreso.ID_ANIO;
                    o.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    o.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    o.ID_ATENCION_MEDICA = ID_CERTIFICADO_MEDICO;
                    o.ID_CENTRO_UBI = GlobalVar.gCentro;
                    o.ID_INCIDENTE = new cIncidente().Insertar(o);

                    #region Sanciones
                    //if (LstSancion != null)
                    //{
                    //    foreach (var s in LstSancion)
                    //    {
                    //        ls.Add(new SANCION()
                    //        {
                    //          ID_SANCION = i,
                    //          INICIA_FEC = s.INICIA_FEC,
                    //          TERMINA_FEC = s.TERMINA_FEC
                    //        });
                    //        i++;
                    //    }
                    //}
                    //o.SANCION = ls;
                    #endregion

                    if (o.ID_INCIDENTE > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        LstIncidente = new ObservableCollection<INCIDENTE>( new cIncidente().ObtenerTodas(o.ID_CENTRO, o.ID_ANIO, o.ID_IMPUTADO, o.ID_INGRESO));
                        SelectedIncidente = null;
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    }
                }
                else
                {
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    o.ID_CENTRO = SelectedIncidente.ID_CENTRO;
                    o.ID_ANIO = SelectedIncidente.ID_ANIO;
                    o.ID_IMPUTADO = SelectedIncidente.ID_IMPUTADO;
                    o.ID_INGRESO = SelectedIncidente.ID_INGRESO;
                    o.ID_INCIDENTE = SelectedIncidente.ID_INCIDENTE;
                    o.ID_ATENCION_MEDICA = SelectedIncidente.ID_ATENCION_MEDICA;
                    o.ID_CENTRO_UBI = SelectedIncidente.ID_CENTRO_UBI;
                    
                    #region Sancion
                    //if (LstSancion != null)
                    //{
                    //    foreach (var s in LstSancion)
                    //    {
                    //        ls.Add(new SANCION()
                    //        {
                    //            ID_CENTRO = s.ID_CENTRO,
                    //            ID_ANIO = s.ID_ANIO,
                    //            ID_IMPUTADO =s.ID_IMPUTADO,
                    //            ID_INGRESO = s.ID_INGRESO,
                    //            ID_INCIDENTE = s.ID_INCIDENTE,
                    //            ID_SANCION = i,
                    //            INICIA_FEC = s.INICIA_FEC,
                    //            TERMINA_FEC = s.TERMINA_FEC
                    //        });
                    //        i++;
                    //    }
                    //}
                    #endregion

                    if (new cIncidente().Actualizar(o))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        LstIncidente = new ObservableCollection<INCIDENTE>(new cIncidente().ObtenerTodas(o.ID_CENTRO, o.ID_ANIO, o.ID_IMPUTADO, o.ID_INGRESO));
                        SelectedIncidente = null;
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error....", ex);  
            }
        }

        private void GuardarSancionDB() 
        {
            try
            {
                var s = new SANCION();
                s.ID_CENTRO = SelectedIncidente.ID_CENTRO;
                s.ID_ANIO = SelectedIncidente.ID_ANIO;
                s.ID_IMPUTADO = SelectedIncidente.ID_IMPUTADO;
                s.ID_INGRESO = SelectedIncidente.ID_INGRESO;
                s.ID_INCIDENTE = SelectedIncidente.ID_INCIDENTE;
                
                var fi = FechaInicio.Value.Date;
                fi = fi.AddHours(int.Parse(HoraInicio));
                fi = fi.AddMinutes(int.Parse(MinutoInicio));

                var ff = FechaFin.Value.Date;
                ff = ff.AddHours(int.Parse(HoraFin));
                ff = ff.AddMinutes(int.Parse(MinutoFin));

                s.INICIA_FEC = fi;
                s.TERMINA_FEC = ff;

                if (s.INICIA_FEC > s.TERMINA_FEC)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "La fecha hora de inicio deben ser menores a la fecha hora final.");
                    return;
                }

                if (SancionSelected == null)
                {
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    s.ID_SANCION = new cSancion().Insertar(s);
                    if (s.ID_SANCION > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        var selected = SelectedIncidente;
                        SelectedIncidente = new ObservableCollection<INCIDENTE>(new cIncidente().ObtenerTodas(
                            selected.ID_CENTRO,
                            selected.ID_ANIO,
                            selected.ID_IMPUTADO,
                            selected.ID_INGRESO,
                            selected.ID_INCIDENTE)).FirstOrDefault();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    }
                }
                else
                {
                    if (!PEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    s.ID_SANCION = SancionSelected.ID_SANCION;
                    if (new cSancion().Actualizar(s))
                    {
                        var selected = SelectedIncidente;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        SelectedIncidente = new ObservableCollection<INCIDENTE>(new cIncidente().ObtenerTodas(
                            selected.ID_CENTRO,
                            selected.ID_ANIO,
                            selected.ID_IMPUTADO,
                            selected.ID_INGRESO,
                            selected.ID_INCIDENTE)).FirstOrDefault();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_SANCION);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error....", ex);  
            }
        }

        private void PopulateSancion()
        {
            if (SancionSelected != null)
            {
                IdSancionTipo = SancionSelected.ID_SANCION;
                SelectSancion = SancionSelected.SANCION_TIPO;

                FechaInicio = SancionSelected.INICIA_FEC;
                FechaFin = SancionSelected.TERMINA_FEC;

                if (SancionSelected.INICIA_FEC.Value.Hour > 9)
                    HoraInicio = SancionSelected.INICIA_FEC.Value.Hour.ToString();
                else
                    HoraInicio = "0" + SancionSelected.INICIA_FEC.Value.Hour;

                if (SancionSelected.INICIA_FEC.Value.Minute > 9)
                    MinutoInicio = SancionSelected.INICIA_FEC.Value.Minute.ToString();
                else
                    MinutoInicio = "0" + SancionSelected.INICIA_FEC.Value.Minute;

                if (SancionSelected.TERMINA_FEC.Value.Hour > 9)
                    HoraFin = SancionSelected.TERMINA_FEC.Value.Hour.ToString();
                else
                    HoraFin = "0" + SancionSelected.TERMINA_FEC.Value.Hour;

                if (SancionSelected.TERMINA_FEC.Value.Minute > 9)
                    MinutoFin = SancionSelected.TERMINA_FEC.Value.Minute.ToString();
                else
                    MinutoFin = "0" + SancionSelected.TERMINA_FEC.Value.Minute;
                //FechaLowerVal = SancionSelected.INICIA_FEC ?? new DateTime();
                //FechaUpperVal = SancionSelected.TERMINA_FEC ?? new DateTime();
            }
        }

        private void EliminarIncidente()
        {
            if (SelectedIncidente != null)
            {
                LstIncidente.Remove(SelectedIncidente);
                EmptyIncidente = LstIncidente.Count > 0 ? false : true;
            }
            else
                new Dialogos().NotificacionDialog("Notificacion", "Favor de seleccionar un incidente");

        }

        private void EliminarIncidenteDB()
        {
            try
            {
                if (!PEditar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (SelectedIncidente != null)
                {
                    var i = new INCIDENTE();
                    i.ID_CENTRO = SelectedIncidente.ID_CENTRO;
                    i.ID_ANIO = SelectedIncidente.ID_ANIO;
                    i.ID_IMPUTADO = SelectedIncidente.ID_IMPUTADO;
                    i.ID_INGRESO = SelectedIncidente.ID_INGRESO;
                    i.ID_INCIDENTE = SelectedIncidente.ID_INCIDENTE;
                    if (new cIncidente().Eliminar(i))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se elimino correctamente");
                        LstIncidente = new ObservableCollection<INCIDENTE>(new cIncidente().ObtenerTodas(i.ID_CENTRO, i.ID_ANIO, i.ID_IMPUTADO, i.ID_INGRESO));
                        SelectedIncidente = null;
                    }
                    //LstIncidente.Remove(SelectedIncidente);
                    EmptyIncidente = LstIncidente.Count > 0 ? false : true;
                }
                else
                    new Dialogos().NotificacionDialog("Notificacion", "Favor de seleccionar un incidente");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void EliminarSancionDB() 
        {
            try
            {
                if (!PEditar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (SancionSelected != null)
                {
                    var s = new SANCION();
                    s.ID_CENTRO = SancionSelected.ID_CENTRO;
                    s.ID_ANIO = SancionSelected.ID_ANIO;
                    s.ID_IMPUTADO = SancionSelected.ID_IMPUTADO;
                    s.ID_INGRESO = SancionSelected.ID_INGRESO;
                    s.ID_INCIDENTE = SancionSelected.ID_INCIDENTE;
                    s.ID_SANCION = SancionSelected.ID_SANCION;
                    if (new cSancion().Eliminar(s))
                    {
                        var selected = SelectedIncidente;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se elimino correctamente");
                        SelectedIncidente = new ObservableCollection<INCIDENTE>(new cIncidente().ObtenerTodas(
                            selected.ID_CENTRO,
                            selected.ID_ANIO,
                            selected.ID_IMPUTADO,
                            selected.ID_INGRESO,
                            selected.ID_INCIDENTE)).FirstOrDefault();
                        EmptySanciones = SelectedIncidente.SANCION.Count > 0 ? false : true;
                    }
                }
                else
                    new Dialogos().NotificacionDialog("Notificacion", "Favor de seleccionar una sanción");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void LimpiarIncidente()
        {
            FecIncidencia = null;
            IdIncidenteTipo = -1;
            SelectedIncidenteTipo = LstIncidenteTipo.AsQueryable().Where(w => w.ID_INCIDENTE_TIPO == IdIncidenteTipo).FirstOrDefault();
            Motivo = string.Empty;
        }


        private void AgregarSancion()
        {
            var x = SelectedIncidente;
            if (SelectedIncidente.SANCION == null)
                SelectedIncidente.SANCION = new ObservableCollection<SANCION>();

            var fi = FechaInicio.Value.Date;
            fi = fi.AddHours(int.Parse(HoraInicio));
            fi = fi.AddMinutes(int.Parse(MinutoInicio));

            var ff = FechaFin.Value.Date;
            ff = ff.AddHours(int.Parse(HoraFin));
            ff = ff.AddMinutes(int.Parse(MinutoFin));

            if (SancionSelected == null)//AGREGAR
            {
                SelectedIncidente.SANCION.Add(new SANCION()
                {
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_SANCION = SelectSancion.ID_SANCION,
                    SANCION_TIPO = SelectSancion,
                    INICIA_FEC = fi,//FechaLowerVal,
                    TERMINA_FEC = ff,//FechaUpperVal
                });
            }
            else//EDITAR
            {
                SancionSelected.ID_SANCION = SelectSancion.ID_SANCION;
                SancionSelected.SANCION_TIPO = SelectSancion;
                SancionSelected.INICIA_FEC = fi;//FechaLowerVal;
                SancionSelected.TERMINA_FEC = ff;//FechaUpperVal;
                SelectedIncidente.SANCION = new ObservableCollection<SANCION>(SelectedIncidente.SANCION);
            }

            SelectedIncidente = null;
            LstIncidente = new ObservableCollection<INCIDENTE>(LstIncidente);
            SelectedIncidente = x;

            EmptySanciones = SelectedIncidente.SANCION.Count > 0 ? false : true;
        }

        private void EliminarSancion()
        {
            if (SancionSelected != null)
            {
                SelectedIncidente.SANCION.Remove(SancionSelected);
                var x = SelectedIncidente;
                SelectedIncidente = null;
                LstIncidente = new ObservableCollection<INCIDENTE>(LstIncidente);
                SelectedIncidente = x;
                EmptySanciones = SelectedIncidente.SANCION.Count > 0 ? false : true;
            }
            else
                new Dialogos().NotificacionDialog("Notificacion", "Favor de seleccionar una sanción.");
        }

        private void LimpiarSancion()
        {
            IdSancionTipo = SelectedSancion = -1;
            SelectSancion = ListTipoSanciones.AsQueryable().Where(w => w.ID_SANCION == IdSancionTipo).FirstOrDefault();
            FecInicioSancion = FecFinSancion = null;
            FechaUpperVal = FechaLowerVal = Fechas.GetFechaDateServer;

            FechaInicio = FechaFin = null;
            HoraInicio = "00";
            MinutoInicio = "00";
            HoraFin = "23";
            MinutoFin = "59";
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.SANCIONES_DISCIPLINARIAS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        //if (p.IMPRIMIR == 1)
                        //    PImprimir = MenuFichaEnabled = true;

                        //if (!PInsertar && !PEditar)
                        //    MenuGuardarEnabled = false;
                        //else
                        //    MenuGuardarEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Reportes
        private void ReporteParteInformativo() 
        {
            try
            {
                var view = new ReportesView();
                view.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                var reporte = new List<cReporte>();
                reporte.Add(new cReporte()
                {
                    Logo1 = Parametro.LOGO_ESTADO,
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = Parametro.ENCABEZADO2,
                    Encabezado3 = centro.DESCR.ToUpper()
                });

                var lst = new List<cReporteDecomiso>();
                var r = new cReporteDecomiso();
                r.LogoBC = Parametro.LOGO_ESTADO;

                if (centro != null)
                {
                    r.Centro = centro.DESCR.Trim();
                    r.MunicipioFecha = string.Format("{0},{1} a {2}", centro.MUNICIPIO.MUNICIPIO1.Trim(), centro.MUNICIPIO.ENTIDAD.DESCR.Trim(), Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    r.DirigidoA = centro.DIRECTOR.Trim();
                    r.DirigidoPuesto = string.Format("DIRECTOR DEL {0}", centro.DESCR.Trim());
                }

                r.Folio = "OFICIO No. S/N";
                r.DecomisoResumen = "Esta es una prueba Esta es una prueba Esta es una prueba Esta es una prueba Esta es una prueba Esta es una prueba Esta es una prueba Esta es una prueba";

                r.Remitente = string.Empty;
                r.RemitentePuesto = string.Empty;
                view.Report.LocalReport.ReportPath = "Reportes/rParteInformativo.rdlc";
                view.Report.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = reporte;
                view.Report.LocalReport.DataSources.Add(rds2);

                lst.Add(r);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = lst;
                view.Report.LocalReport.DataSources.Add(rds1);
                view.Report.RefreshReport();
                view.Owner = PopUpsViewModels.MainWindow;
                view.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir parte informativo.", ex);
            }
        }

        private void ImprimirCitatorio() {
            try
            {
                if (SelectedIngreso != null)
                {
                    if (SelectedIncidente != null)
                    {
                        var doc = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.CITATORIO_INTERNO);
                        if (doc == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No se encontro plantilla para generar reporte");
                            return;
                        }
                        CultureInfo cultura = new CultureInfo("es-MX");
                        var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                        var hoy = Fechas.GetFechaDateServer;
                        var parametros = new Dictionary<string, string>();
                        parametros.Add("<<encabezado1>>", Parametro.ENCABEZADO1);
                        parametros.Add("<<encabezado2>>", Parametro.ENCABEZADO2);
                        parametros.Add("<<centro>>", centro.DESCR.Trim());
                        parametros.Add("<<horas>>", hoy.ToString("hh:mm"));
                        parametros.Add("<<dia>>", hoy.Day.ToString());
                        parametros.Add("<<mes>>", cultura.DateTimeFormat.GetMonthName(hoy.Month));
                        parametros.Add("<<anio>>", hoy.Year.ToString());
                        parametros.Add("<<interno>>", string.Format("{0} {1} {2}",
                            SelectedIngreso.IMPUTADO.NOMBRE.Trim(),
                            !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty));
                        parametros.Add("<<motivo>>", SelectedIncidente.MOTIVO.Trim());
                        parametros.Add("<<fracciones>>", "   ");
                        parametros.Add("<<signado>>", "                                                ");
                        parametros.Add("<<firma_policia>>", "   ");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un incidente");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un interno");
                
                
                /*
                 
<<encabezado2>>
<<centro>>
<<horas>>
<<dia>>
<<mes>>
<<anio>>
<<interno>>
<<motivo>>
<<fracciones>>
<<signado>>
<<firma_policia>>
                 */
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir citatorio.", ex);
            }
        }
        #endregion

        #region Certificado Medico
        private void ObtenerCertificadoMedico()
        {
            try
            {
                ID_CERTIFICADO_MEDICO = 0;
                if(SelectIngreso != null)
                {
                    var am = new cAtencionMedica().ObtenerUltimaAtencionCertificadoCentro(
                        SelectIngreso.ID_CENTRO,
                        SelectIngreso.ID_ANIO,
                        SelectIngreso.ID_IMPUTADO,
                        SelectIngreso.ID_INGRESO,
                        GlobalVar.gCentro);
                    if (am != null)
                    {
                        if (am.CERTIFICADO_MEDICO != null)
                        {
                            if (am.CERTIFICADO_MEDICO.ES_SANCION == "S")
                            {
                                var existe = new cIncidente().ObtenerIncidenteAtencionMedica(
                                    SelectIngreso.ID_CENTRO,
                                    SelectIngreso.ID_ANIO,
                                    SelectIngreso.ID_IMPUTADO,
                                    SelectIngreso.ID_INGRESO,
                                    am.ID_ATENCION_MEDICA,
                                    am.ID_CENTRO_UBI);
                                if(existe == null)
                                    ID_CERTIFICADO_MEDICO = am.ID_ATENCION_MEDICA; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
                        break;
                }
            }
        }
        #endregion

    }
}
