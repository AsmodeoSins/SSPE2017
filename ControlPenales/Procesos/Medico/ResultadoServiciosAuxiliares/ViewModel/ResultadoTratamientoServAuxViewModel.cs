using ControlPenales.BiometricoServiceReference;
using System.Linq;
namespace ControlPenales
{
    partial class ResultadoTratamientoServAuxViewModel : ValidationViewModelBase
    {
        public ResultadoTratamientoServAuxViewModel() { }

        private WPFPdfViewer.PdfViewer PDFViewer;
        private void ResultadosServiciosLoading(ResultadosServiciosAuxiliaresView window)
        {
            CargarTipo_Servicios_Auxiliares();
            ConfiguraPermisos();
            PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
        }

        private async void clickSwitch(System.Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    AnioBuscar = FolioBuscar = null;
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ResultadosServiciosAuxiliaresView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.ResultadoTratamientoServAuxViewModel();
                    break;

                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<SSP.Servidor.IMPUTADO>();
                    SelectExpediente = new SSP.Servidor.IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
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

                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    break;
                case "guardar_menu":
                    if (SelectExpediente == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido.");
                        break;
                    }

                    break;

                case "buscar_result_existentes":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

                    if (SelectIngreso != null)
                        BuscarResultadosExistentes();
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido.");

                    break;

                case "agregar_resultado_trat":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido.");
                        break;
                    }
                    else
                    {
                        CargaListasEdicionAgregarArchivos();
                        ValidacionesAgregarArchivos();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO);
                    }
                    break;

                case "seleccionar_archivo":
                    SeleccionarArchivo();
                    break;

                case "guardar_archivo_result_edicion":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido.");
                        return;
                    };

                    if (base.HasErrors)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        }));

                        return;
                    }

                    if (_ArchivoSubido == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Adjunte un archivo para continuar.");
                        return;
                    }

                    if (_ArchivoSubido.Length == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El archivo que anexo esta corrupto.");
                        return;
                    }

                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando resultado", () => GuardaArchivoResultadoTratamiento()))
                    {
                        LimpiaCamposEdicionAgregarArchivos();
                        CargaListaArchivosSubidos();
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado el resultado exitosamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO);
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar el archivo");

                    break;

                case "ver_documento_resultado_servicios":
                    if (SeletedResultadoSinArchivo == null)
                        return;

                    if (string.IsNullOrEmpty(SeletedResultadoSinArchivo.ExtensionArchivo))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique la extensión del archivo seleccionado");
                        return;
                    }

                    DescargaArchivo(SeletedResultadoSinArchivo);
                    break;
                case "cancelar_archivo_result_edicion":
                    LimpiaCamposEdicionAgregarArchivos();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTOS_RESULT_TRATAMIENTO);
                    break;
                case "cerrar_visualizador_documentos":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                    break;
            }
        }

        private void BuscarResultadosExistentes()
        {
            try
            {
                if (SelectIngreso == null)
                    return;

                System.DateTime f1, f2;
                if (FechaInicioBusquedaResultServ.HasValue)
                    f1 = FechaInicioBusquedaResultServ.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaResultServ.HasValue)
                    f2 = FechaFinBusquedaResultServ.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new System.DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new System.DateTime(f2.Year, f2.Month, f2.Day);

                var _datos = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().BuscarResultados(f1, f2, SelectedTipoServAux, SelectedSubtipoServAux, SelectedDiagnPrincipal, SelectIngreso, SelectedIngresoBusquedas);
                if (_datos.Any())
                {
                    LstCustomizadaSinArchivos = new System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes>();
                    foreach (var item in _datos)
                    {
                        LstCustomizadaSinArchivos.Add(new CustomGridSinBytes
                            {
                                FechaRegistro = item.REGISTRO_FEC.HasValue ? item.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                IdResult = item.ID_SA_RESULTADO,
                                ServicioAuxiliar = item.ID_SERV_AUX.HasValue ? !string.IsNullOrEmpty(item.SERVICIO_AUX_DIAG_TRAT.DESCR) ? item.SERVICIO_AUX_DIAG_TRAT.DESCR.Trim() : string.Empty : string.Empty,
                                NombreUsuario = item.USUARIO != null ? item.USUARIO.EMPLEADO != null ? item.USUARIO.EMPLEADO.PERSONA != null ?
                                    string.Format("{0} {1} {2}",
                                        !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? item.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.PATERNO) ? item.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.MATERNO) ? item.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                         : string.Empty : string.Empty : string.Empty,
                                VisibleDocumentoResult = item.CAMPO_BLOB != null ? item.CAMPO_BLOB.Any() ? true : false : false,
                                ExtensionArchivo = item.ID_FORMATO.HasValue ? !string.IsNullOrEmpty(item.FORMATO_DOCUMENTO.DESCR) ? item.FORMATO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty
                            });
                    };

                    if (LstCustomizadaSinArchivos.Count > 0)
                        EmptyResultados = false;
                    else
                        EmptyResultados = true;
                }
                else
                {
                    LstCustomizadaSinArchivos = new System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes>();
                    EmptyResultados = true;
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }
        private void CargaListaArchivosSubidos()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    LstCustomizadaSinArchivos = new System.Collections.ObjectModel.ObservableCollection<CustomGridSinBytes>();

                    var _Archivos = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().GetData(x => x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).OrderByDescending(x => x.REGISTRO_FEC);
                    if (_Archivos.Any())
                        foreach (var item in _Archivos)
                            LstCustomizadaSinArchivos.Add(new CustomGridSinBytes
                                {
                                    FechaRegistro = item.REGISTRO_FEC.HasValue ? item.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                    IdResult = item.ID_SA_RESULTADO,
                                    ServicioAuxiliar = item.ID_SERV_AUX.HasValue ? !string.IsNullOrEmpty(item.SERVICIO_AUX_DIAG_TRAT.DESCR) ? item.SERVICIO_AUX_DIAG_TRAT.DESCR.Trim() : string.Empty : string.Empty,
                                    NombreUsuario = item.USUARIO != null ? item.USUARIO.EMPLEADO != null ? item.USUARIO.EMPLEADO.PERSONA != null ?
                                        string.Format("{0} {1} {2}",
                                            !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? item.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.PATERNO) ? item.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.USUARIO.EMPLEADO.PERSONA.MATERNO) ? item.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty)
                                             : string.Empty : string.Empty : string.Empty,
                                    VisibleDocumentoResult = item.CAMPO_BLOB != null ? item.CAMPO_BLOB.Any() ? true : false : false,
                                    ExtensionArchivo = item.ID_FORMATO.HasValue ? !string.IsNullOrEmpty(item.FORMATO_DOCUMENTO.DESCR) ? item.FORMATO_DOCUMENTO.DESCR.Trim() : string.Empty : string.Empty
                                });


                    if (LstCustomizadaSinArchivos.Count > 0)
                        EmptyResultados = false;
                    else
                        EmptyResultados = true;
                };
            }

            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }
        private bool GuardaArchivoResultadoTratamiento()
        {
            bool _respuesta = false;

            try
            {
                var _usuario = new SSP.Controlador.Catalogo.Justicia.cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault();
                System.DateTime _FechaActual = Fechas.GetFechaDateServer;

                var _NuevoArchivo = new SSP.Servidor.SERVICIO_AUXILIAR_RESULTADO()
                {
                    CAMPO_BLOB = _ArchivoSubido,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_USUARIO = _usuario != null ? _usuario.ID_USUARIO : string.Empty,
                    REGISTRO_FEC = _FechaActual,
                    ID_FORMATO = ExtensionArchivoElegido,
                    ID_SERV_AUX = SelectedDiagnosticoEdicion,
                    NOMBRE_ARCHIVO = _NombreArchivoSubido
                };

                _respuesta = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().AgregarArchivo(_NuevoArchivo);
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el archivo", exc);
            }

            return _respuesta;
        }

        private void LimpiaCamposEdicionAgregarArchivos()
        {
            try
            {
                SelectedTipoServAuxEdicion = SelectedSubTipoServAuxEdicion = SelectedFormatoArchivo = -1;
                SelectedDiagnosticoEdicion = -1;
                _ArchivoSubido = new byte[] { };
                ExtensionArchivoElegido = -1;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }

        private void CargaListasEdicionAgregarArchivos()
        {
            try
            {
                LstFormatosArchivos = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.FORMATO_DOCUMENTO>(new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO != decimal.Zero, y => y.DESCR));
                LstFormatosArchivos.Insert(0, new SSP.Servidor.FORMATO_DOCUMENTO { DESCR = "SELECCIONE", ID_FORMATO = -1 });

                LstSubTipoServAuxEdicion = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>();
                LstSubTipoServAuxEdicion.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });

                LstTipoServAuxEdicion = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("", "S"));
                LstTipoServAuxEdicion.Insert(0, new SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT { ID_TIPO_SADT = -1, DESCR = "SELECCIONE" });

                LstServAux = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstServAux.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", exc);
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
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
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }

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
                    IngresosD = SelectIngreso.ID_INGRESO;

                    if (SelectIngreso.CAMA != null)
                        UbicacionD = string.Format("{0}-{1}{2}-{3}",
                            SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty, SelectIngreso.ID_UB_CAMA);
                    else
                        UbicacionD = string.Empty;

                    TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                    ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;

                    LstCustomizadaIngresos = new System.Collections.ObjectModel.ObservableCollection<CustomIngresos>();
                    var _Ingresos = new SSP.Controlador.Catalogo.Justicia.cIngreso().ObtenerIngresos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                    if (_Ingresos.Any())
                        foreach (var item in _Ingresos)
                            LstCustomizadaIngresos.Add(new CustomIngresos
                            {
                                DescripcionIngreso = string.Format("INGRESO {0}", item.ID_INGRESO),
                                IdIngreso = item.ID_INGRESO
                            });

                    LstCustomizadaIngresos.Insert(0, new CustomIngresos { DescripcionIngreso = "TODOS", IdIngreso = -1 });
                    SelectedIngresoBusquedas = -1;
                    CargaListaArchivosSubidos();
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private void CargarTipo_Servicios_Auxiliares()
        {
            try
            {
                //limpia la sub lista antes de llenar de nuevo
                lstSubtipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>();
                lstSubtipoServAux.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubtipoServAux");

                lstTipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cTipo_Serv_Aux_Diag_Trat().ObtenerTodos("", "S"));
                lstTipoServAux.Insert(0, new SSP.Servidor.TIPO_SERVICIO_AUX_DIAG_TRAT { ID_TIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstTipoServAux");

                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                RaisePropertyChanged("LstDiagnosticosPrincipal");

                LstCustomizadaIngresos = new System.Collections.ObjectModel.ObservableCollection<CustomIngresos>();
                LstCustomizadaIngresos.Insert(0, new CustomIngresos { DescripcionIngreso = "TODOS", IdIngreso = -1 });
                RaisePropertyChanged("LstCustomizadaIngresos");

                SelectedIngresoBusquedas = -1;
                SelectedDiagnosticoEdicion = -1;
                SelectedDiagnPrincipal = -1;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipos de servicios auxiliares", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.RESULT_SERVICIOS_AUX.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = MenuGuardarEnabled = true;
                        if (p.EDITAR == 1)
                            PEditar = MenuGuardarEnabled = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = MenuBuscarEnabled = BuscarImputadoHabilitado = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = MenuReporteEnabled = MenuFichaEnabled = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        private void CargarSubTipo_Servicios_Auxiliares(short? tipo_serv_aux)
        {
            try
            {
                lstSubtipoServAux = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                lstSubtipoServAux.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubtipoServAux");

                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>();
                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                RaisePropertyChanged("LstDiagnosticosPrincipal");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarSubTipoServiciosAuxiliaresEdicion(short? tipo_serv_aux)
        {
            try
            {
                LstSubTipoServAuxEdicion = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT>(new SSP.Controlador.Catalogo.Justicia.cSubtipo_Serv_Aux_Diag_Trat().ObtenerTodos(tipo_serv_aux, "S"));
                LstSubTipoServAuxEdicion.Insert(0, new SSP.Servidor.SUBTIPO_SERVICIO_AUX_DIAG_TRAT { ID_SUBTIPO_SADT = -1, DESCR = "SELECCIONE" });
                RaisePropertyChanged("LstSubTipoServAuxEdicion");
                //SelectedDiagnosticoEdicion = -1;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarTipoTratamientoEdicion(short? subTipo)
        {
            try
            {
                LstServAux = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new SSP.Controlador.Catalogo.Justicia.cServ_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAuxEdicion, subTipo, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.DESCR,
                    ESTATUS = s.ESTATUS,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.ID_SUBTIPO_SADT,
                    SUBTIPO_DESCR = s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));
                LstServAux.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                OnPropertyChanged("LstServAux");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private void CargarDiagnosticosPrincipal(short? subTipo)
        {
            try
            {
                LstDiagnosticosPrincipal = new System.Collections.ObjectModel.ObservableCollection<EXT_SERV_AUX_DIAGNOSTICO>(new SSP.Controlador.Catalogo.Justicia.cServ_Aux_Diag_Trat().ObtenerTodos(SelectedTipoServAux, subTipo, "S").Select(s => new EXT_SERV_AUX_DIAGNOSTICO
                {
                    DESCR = s.DESCR,
                    ESTATUS = s.ESTATUS,
                    ID_SERV_AUX = s.ID_SERV_AUX,
                    ID_SUBTIPO_SADT = s.ID_SUBTIPO_SADT,
                    SUBTIPO_DESCR = s.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.DESCR
                }));

                LstDiagnosticosPrincipal.Insert(0, new EXT_SERV_AUX_DIAGNOSTICO { DESCR = "SELECCIONE", ID_SERV_AUX = -1 });
                OnPropertyChanged("LstDiagnosticosPrincipal");
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", ex);
            }
        }

        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_tipo_serv_aux":
                    CargarSubTipo_Servicios_Auxiliares(selectedTipoServAux);
                    selectedSubtipoServAux = -1;
                    SelectedDiagnPrincipal = -1;
                    RaisePropertyChanged("SelectedSubtipoServAux");
                    OnPropertyChanged("SelectedDiagnPrincipal");
                    break;

                case "cambio_subtipo_serv_aux":
                    CargarDiagnosticosPrincipal(SelectedSubtipoServAux);
                    SelectedDiagnPrincipal = -1;
                    RaisePropertyChanged("SelectedDiagnPrincipal");
                    break;

                case "cambio_tipo_serv_aux_edicion":
                    CargarSubTipoServiciosAuxiliaresEdicion(SelectedTipoServAuxEdicion);
                    SelectedSubTipoServAuxEdicion = -1;
                    SelectedDiagnosticoEdicion = -1;
                    RaisePropertyChanged("SelectedSubTipoServAuxEdicion");
                    break;

                case "cambio_subtipo_serv_aux_edicion":
                    SelectedSubtipoServAux = -1;
                    CargarTipoTratamientoEdicion(SelectedSubTipoServAuxEdicion);
                    SelectedDiagnosticoEdicion = -1;
                    RaisePropertyChanged("SelectedSubtipoServAux");
                    OnPropertyChanged("SelectedDiagnosticoEdicion");
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

        private async void DescargaArchivo(CustomGridSinBytes Entity)
        {
            try
            {
                if (Entity == null)
                    return;

                var _detallesArchivo = new SSP.Controlador.Catalogo.Justicia.cServicioAuxiliarResultado().GetData(z => z.ID_SA_RESULTADO == SeletedResultadoSinArchivo.IdResult).FirstOrDefault();
                if (_detallesArchivo == null)
                    return;

                if (_detallesArchivo.ID_FORMATO == null)
                    return;

                var tc = new TextControlView();

                switch (_detallesArchivo.ID_FORMATO)
                {
                    case 1: // docx
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.CAMPO_BLOB, TXTextControl.BinaryStreamType.WordprocessingML);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        break;

                    case 2://xls
                        break;

                    case 3://pdf
                        var _file = _detallesArchivo.CAMPO_BLOB;
                        await System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".pdf";
                            System.IO.File.WriteAllBytes(fileNamepdf, _file);
                            System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                PDFViewer.LoadFile(fileNamepdf);
                                PDFViewer.Visibility = System.Windows.Visibility.Visible;
                            }));
                        });

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                        break;

                    case 4://doc
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(_detallesArchivo.CAMPO_BLOB, TXTextControl.BinaryStreamType.MSWord);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        break;

                    case 5://jpeg
                        break;

                    case 6://xlsx
                        break;

                    case 15:
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(_detallesArchivo.CAMPO_BLOB);
                                System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
                                TXTextControl.Image _imagen = new TXTextControl.Image(returnImage);
                                tc.editor.Images.Add(_imagen, TXTextControl.HorizontalAlignment.Center, 0, TXTextControl.ImageInsertionMode.FixedOnPage);
                            }
                            catch (System.Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Show();
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar subtipos de servicios auxiliares", exc);
            }
        }

        private void SeleccionarArchivo()
        {
            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione un archivo";
            var _ImagenesPermitidasParametro = new short[] { (short)eformatosPermitidos.JPEG, (short)eformatosPermitidos.JPG };
            var _DocumentosPermitidosParametros = new short[] { (short)eformatosPermitidos.PDF };
            string formatosP = string.Empty;
            string formatosDisponibles = string.Empty;
            string _todosArchivosCatalogo = "Todos los archivos permitidos | ";
            string FormatosImagenes = "Todas las imágenes permitidas |";
            string FormatosDocumentos = "Todos los documentos permitidos |";
            var _archivosCatalogo = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO == (short)eformatosPermitidos.JPEG || x.ID_FORMATO == (short)eformatosPermitidos.JPG || x.ID_FORMATO == (short)eformatosPermitidos.PDF);
            if (_archivosCatalogo.Any())
                foreach (var item in _archivosCatalogo)
                    _todosArchivosCatalogo += string.Format("*{0};", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.ToLower() : string.Empty);

            if (_ImagenesPermitidasParametro.Any())
                foreach (var item in _ImagenesPermitidasParametro)
                {
                    var _ImagenIndividual = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData().FirstOrDefault(x => x.ID_FORMATO == item);
                    if (_ImagenIndividual != null)
                        FormatosImagenes += string.Format("*{0};", !string.IsNullOrEmpty(_ImagenIndividual.DESCR) ? _ImagenIndividual.DESCR.ToLower() : string.Empty);
                };

            if (_DocumentosPermitidosParametros.Any())
                foreach (var item in _DocumentosPermitidosParametros)
                {
                    var _DocumentoPermitido = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData().FirstOrDefault(x => x.ID_FORMATO == item);
                    if (_DocumentoPermitido != null)
                        FormatosDocumentos += string.Format("*{0};", !string.IsNullOrEmpty(_DocumentoPermitido.DESCR) ? _DocumentoPermitido.DESCR.ToLower() : string.Empty);
                };

            var _archivosPermitidos = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO == (short)eformatosPermitidos.JPEG || x.ID_FORMATO == (short)eformatosPermitidos.JPG || x.ID_FORMATO == (short)eformatosPermitidos.PDF);//new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData(x => x.ID_FORMATO != 0);
            if (_archivosPermitidos.Any())
                foreach (var item in _archivosPermitidos)
                    formatosDisponibles += string.Format("Archivo Tipo {0}  (*{1})|*{2}|", !string.IsNullOrEmpty(item.DESCR) ? item.DESCR.Replace('.', ' ') : string.Empty, item.DESCR.ToLower(), item.DESCR.ToLower());

            string _refinados = !string.IsNullOrEmpty(formatosDisponibles) ? formatosDisponibles.TrimEnd('|') : string.Empty;
            op.Filter = _todosArchivosCatalogo + '|' + FormatosImagenes + '|' + FormatosDocumentos + '|' + _refinados;
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _ArchivoSubido = new byte[] { };
                    string _extension = System.IO.Path.GetExtension(op.FileName).TrimStart('.').ToUpper();
                    var _ArchivoElegido = new SSP.Controlador.Catalogo.Justicia.cFormatoDocumento().GetData().FirstOrDefault(x => x.DESCR.Trim() == _extension);
                    if (_ArchivoElegido == null)
                        return;

                    var _MaximoArchivos = Parametro.MAXIMO_KILOBYTES_ARCHIVOS_MEDICOS;
                    if (_MaximoArchivos != decimal.Zero)
                    {
                        if (new System.IO.FileInfo(op.FileName).Length > (System.Convert.ToInt64(_MaximoArchivos) * 1024))
                        {
                            StaticSourcesViewModel.Mensaje("Archivo no soportado", "El archivo supera el máximo permitido", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }

                        _ArchivoSubido = System.IO.File.ReadAllBytes(op.FileName);
                        _NombreArchivoSubido = System.IO.Path.GetFileNameWithoutExtension(op.SafeFileName);
                        ExtensionArchivoElegido = _ArchivoElegido.ID_FORMATO;
                    };
                }

                catch (System.IO.IOException exArc)
                {
                    if (!string.IsNullOrEmpty(exArc.Message))
                    {
                        if (exArc.Message.Contains("The process cannot access the file"))
                            new Dialogos().ConfirmacionDialogo("Validación!", "EL ARCHIVO SELECCIONADO ESTÁ EN USO, GUARDE SU PROGRESO Y CIERRE EL ARCHIVO PARA INGRESARLO");
                        else
                            new Dialogos().ConfirmacionDialogo("Validación!", exArc.Message.ToUpper());
                    };

                    return;
                }

                catch (System.Exception exc)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", !string.IsNullOrEmpty(exc.Message) ? exc.Message.ToUpper() : string.Empty);
                    return;
                }
            }
        }
    }
}