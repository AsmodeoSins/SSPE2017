using ControlPenales.BiometricoServiceReference;
using System.Linq;
using System.Threading;
namespace ControlPenales
{
    partial class RecetaMedicaViewModel : ValidationViewModelBase
    {
        public RecetaMedicaViewModel() { }

        private async void ClickEnter(System.Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                }

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }

            return;
        }

        private async void clickSwitch(object op)
        {

            switch (op.ToString())
            {
                case "reporte_menu":
                    if (SelectedNotaMedica == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un registro para imprimir la receta");
                        return;
                    }

                    InicializaMedicamentosRecetaMedica();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA);
                    break;
                case "continuar_impresion_receta":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA);
                    ImprimirRecetaMedica();
                    break;

                case "cancelar_impresion_receta":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OBSERVACIONES_MEDICAMENTOS_RECETA_MEDICA);
                    break;
                case "buscar_salir":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    NombreD = PaternoD = MaternoD = string.Empty;
                    AnioD = FolioD = new int?();
                    AnioBuscar = FolioBuscar = null;
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = null;
                    SelectIngreso = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                #region Busqueda de Imputado y Procesos
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    break;
                #endregion
                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = new SSP.Servidor.IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                #region Proceso de Guardar Informacion
                case "menu_guardar":
                    break;
                #endregion
                case "limpiar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea limpiar la pantalla sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }

                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RecetaMedicaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new RecetaMedicaViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        }

                    if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }

                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-Parametro.TOLERANCIA_TRASLADO_EDIFICIO) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }

                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

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

                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
            }
        }

        private async System.Threading.Tasks.Task<System.Collections.Generic.List<SSP.Servidor.IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.IMPUTADO>>(() => new SSP.Controlador.Catalogo.Justicia.cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }

            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new System.Collections.Generic.List<SSP.Servidor.IMPUTADO>();
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
                if (!BuscarImputadoHabilitado)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

                if (obj == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al realizar la búsqueda de imputados.");
                    return;
                }

                if (obj != null)
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as System.Windows.Controls.TextBox;
                        if (textbox != null)
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

                ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();

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

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        if (ListExpediente[0].INGRESO != null && !ListExpediente[0].INGRESO.Any())
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        };

                        foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            };
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        };


                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        SeleccionaIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }

                    SeleccionaIngreso();
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void SeleccionaIngreso()
        {
            try
            {

                if (SelectIngreso != null)
                {
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                };

                LstNotasMedicasReceta = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTA_MEDICA>();
                var _Recetas = new SSP.Controlador.Catalogo.Justicia.cNotaMedica().ObtenerNotasMedicasRecetaMedica(GlobalVar.gCentro, Parametro.ESTATUS_ADMINISTRATIVO_INACT, SelectedAtencionTipo, SelectedAtencionServicio, AnioBuscar, FolioBuscar, NombreD, PaternoD, MaternoD, FechaBusqueda);
                if (_Recetas.Any())
                    foreach (var item in _Recetas)
                        if (item.ATENCION_MEDICA != null)
                            if (item.ATENCION_MEDICA.RECETA_MEDICA != null && item.ATENCION_MEDICA.RECETA_MEDICA.Any())
                                System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate { LstNotasMedicasReceta.Add(item); }));

                if (LstNotasMedicasReceta.Any())
                    EmptyResultados = false;
                else
                    EmptyResultados = true;

                SelectedNotaMedica = null;
                RaisePropertyChanged("LstNotasMedicasReceta");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void LoadRecetaMedica(RecetaMedicaView Window = null)
        {
            InicializaListas();
            ConfiguraPermisos();
        }


        private void InicializaListas()
        {
            try
            {
                LstAtencionServicio = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_SERVICIO>(new SSP.Controlador.Catalogo.Justicia.cAtencionServicio().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstAtencionTipo = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_TIPO>(new SSP.Controlador.Catalogo.Justicia.cAtencionTipo().GetData(x => x.ESTATUS == "S", y => y.DESCR));

                LstAtencionServicio.Insert(0, new SSP.Servidor.ATENCION_SERVICIO { DESCR = "SELECCIONE", ID_TIPO_SERVICIO = -1 });
                LstAtencionTipo.Insert(0, new SSP.Servidor.ATENCION_TIPO { DESCR = "SELECCIONE", ID_TIPO_ATENCION = -1 });

                SelectedAtencionServicio = SelectedAtencionTipo = -1;
                RaisePropertyChanged("LstAtencionServicio");
                RaisePropertyChanged("LstAtencionTipo");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void InicializaMedicamentosRecetaMedica()
        {
            try
            {
                LstMedicamentosConAnotaciones = new System.Collections.ObjectModel.ObservableCollection<cCustomMedicamentosImpresionReceta>();
                if (SelectedNotaMedica != null)
                    if (SelectedNotaMedica.ATENCION_MEDICA != null)
                        if (SelectedNotaMedica.ATENCION_MEDICA.RECETA_MEDICA != null && SelectedNotaMedica.ATENCION_MEDICA.RECETA_MEDICA.Any())
                            foreach (var item in SelectedNotaMedica.ATENCION_MEDICA.RECETA_MEDICA)
                                if (item.RECETA_MEDICA_DETALLE != null && item.RECETA_MEDICA_DETALLE.Any())
                                    foreach (var item2 in item.RECETA_MEDICA_DETALLE)
                                        LstMedicamentosConAnotaciones.Add(new cCustomMedicamentosImpresionReceta
                                        {
                                            Cantidad = item2.DOSIS.HasValue ? item2.DOSIS.Value.ToString() : string.Empty,
                                            NombreMedicamento = item2.PRODUCTO != null ? !string.IsNullOrEmpty(item2.PRODUCTO.NOMBRE) ? item2.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                            Duracion = item2.DURACION.HasValue ? item2.DURACION.Value.ToString() : string.Empty,
                                            Maniana = item2.DESAYUNO.HasValue ? item2.DESAYUNO.Value == (short)eSINOProcesos.SI ? "X" : string.Empty : string.Empty,
                                            Tarde = item2.COMIDA.HasValue ? item2.COMIDA.Value == (short)eSINOProcesos.SI ? "X" : string.Empty : string.Empty,
                                            Noche = item2.CENA.HasValue ? item2.CENA.Value == (short)eSINOProcesos.SI ? "X" : string.Empty : string.Empty,
                                            UnidadMedida = item2.PRODUCTO != null ? item2.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA != null ? !string.IsNullOrEmpty(item2.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE) ? item2.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                            Presentacion = item2.PRODUCTO != null ? !string.IsNullOrEmpty(item2.PRODUCTO.NOMBRE) ? item2.PRODUCTO.NOMBRE.Trim() : string.Empty : string.Empty,
                                            OBSERVACIONES = string.Empty,
                                            Anotaciones = !string.IsNullOrEmpty(item2.OBSERV) ? item2.OBSERV.Trim() : string.Empty
                                        });
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        private void ImprimirRecetaMedica()
        {
            try
            {
                if (SelectedNotaMedica == null)
                    return;

                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                var _UsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                cRecetaMedicaReporte DatosReporte = new cRecetaMedicaReporte()
                {
                    Edad = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty : string.Empty : string.Empty,
                    Fecha = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy"),
                    NoExpediente = string.Format("{0} / {1}", SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.ID_ANIO.HasValue ? SelectedNotaMedica.ATENCION_MEDICA.ID_ANIO.Value.ToString() : string.Empty : string.Empty, SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.ID_IMPUTADO.HasValue ? SelectedNotaMedica.ATENCION_MEDICA.ID_IMPUTADO.Value.ToString() : string.Empty : string.Empty),
                    NombrePaciente = string.Format("{0} {1} {2}",
                        SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty),
                    Estancia = string.Format("{0}-{1}{2}-{3}",
                            SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.ID_CELDA) ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO != null ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.ID_UB_CAMA.HasValue ? SelectedNotaMedica.ATENCION_MEDICA.INGRESO.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty : string.Empty),
                    FC = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC) ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC.Trim() : string.Empty : string.Empty : string.Empty,
                    FR = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA) ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA.Trim() : string.Empty : string.Empty : string.Empty,
                    TA = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL) ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL.Trim() : string.Empty : string.Empty : string.Empty,
                    T = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TEMPERATURA) ? SelectedNotaMedica.ATENCION_MEDICA.NOTA_SIGNOS_VITALES.TEMPERATURA.Trim() : string.Empty : string.Empty : string.Empty,
                    Servicio = SelectedNotaMedica.ATENCION_MEDICA != null ? SelectedNotaMedica.ATENCION_MEDICA.ATENCION_SERVICIO != null ? !string.IsNullOrEmpty(SelectedNotaMedica.ATENCION_MEDICA.ATENCION_SERVICIO.DESCR) ? SelectedNotaMedica.ATENCION_MEDICA.ATENCION_SERVICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                    Generico = _UsuarioActual != null ? string.Format("{0} {1} {2}",
                        _UsuarioActual.EMPLEADO != null ? _UsuarioActual.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_UsuarioActual.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioActual.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _UsuarioActual.EMPLEADO != null ? _UsuarioActual.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_UsuarioActual.EMPLEADO.PERSONA.PATERNO) ? _UsuarioActual.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                _UsuarioActual.EMPLEADO != null ? _UsuarioActual.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_UsuarioActual.EMPLEADO.PERSONA.MATERNO) ? _UsuarioActual.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty,
                    Generico2 = _UsuarioActual != null ? _UsuarioActual.EMPLEADO != null ? !string.IsNullOrEmpty(_UsuarioActual.EMPLEADO.CEDULA) ? _UsuarioActual.EMPLEADO.CEDULA.Trim() : string.Empty : string.Empty : string.Empty
                };

                System.Collections.Generic.List<cMedicamentosRecetaMedica> ListaMedicamentos = new System.Collections.Generic.List<cMedicamentosRecetaMedica>();
                if(LstMedicamentosConAnotaciones != null && LstMedicamentosConAnotaciones.Any())
                    foreach (var item in LstMedicamentosConAnotaciones)
                    {
                        ListaMedicamentos.Add(new cMedicamentosRecetaMedica
                            {
                                Cantidad = item.Cantidad,
                                Duracion = item.Duracion,
                                Noche = item.Noche,
                                Maniana = item.Maniana,
                                Observaciones = item.Anotaciones,
                                Presentacion = string.Format("{0} {1}", item.NombreMedicamento, !string.IsNullOrEmpty(item.OBSERVACIONES) ? string.Format("({0})", item.OBSERVACIONES.Trim()) : string.Empty),
                                UnidadMedida = item.UnidadMedida,
                                Tarde = item.Tarde,
                            });
                    };

                var _CeresoActual = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                var Encabezado = new cEncabezado()
                {
                    TituloUno = _CeresoActual != null ? !string.IsNullOrEmpty(_CeresoActual.DESCR) ? _CeresoActual.DESCR.Trim() : string.Empty : string.Empty,
                    TituloDos = Parametro.ENCABEZADO2,
                    ImagenIzquierda = Parametro.REPORTE_LOGO2,
                    ImagenFondo = Parametro.REPORTE_LOGO_MEDICINA
                };

                View.Report.LocalReport.ReportPath = "Reportes/rRecetaMedica.rdlc";
                View.Report.LocalReport.DataSources.Clear();


                var ds1 = new System.Collections.Generic.List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);

                var ds2 = new System.Collections.Generic.List<cRecetaMedicaReporte>();
                ds2.Add(DatosReporte);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = ListaMedicamentos;
                View.Report.LocalReport.DataSources.Add(rds3);
                View.Report.RefreshReport();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_tipo_atencion":
                    CargaTiposAtencion();
                    SelectedAtencionServicio = -1;
                    OnPropertyChanged("SelectedAtencionServicio");
                    break;
                #region Cambio SelectedItem de Busqueda de Expediente
                case "cambio_expediente":
                    if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                    {
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            selectExpediente = new SSP.Controlador.Catalogo.Justicia.cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
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
                #endregion
            }
        }

        private void CargaTiposAtencion()
        {
            try
            {
                LstAtencionServicio = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ATENCION_SERVICIO>(new SSP.Controlador.Catalogo.Justicia.cAtencionServicio().ObtenerXAtencion(SelectedAtencionTipo.Value));
                LstAtencionServicio.Insert(0, new SSP.Servidor.ATENCION_SERVICIO { DESCR = "SELECCIONE", ID_TIPO_SERVICIO = -1 });
                RaisePropertyChanged("LstAtencionServicio");
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                if (_detalleUsuarioActual != null)
                {
                    #region PERMISOS MEDICOS
                    var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.RECETA_MEDICA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                    foreach (var p in permisos)
                    {
                        if (p.CONSULTAR == 1)
                            PConsultar = MenuBuscarEnabled = BuscarImputadoHabilitado = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                    };

                    #endregion
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
    }
}