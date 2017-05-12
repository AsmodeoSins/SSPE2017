
using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Compartidos;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class TrasladoExternoViewModel:ValidationViewModelBase
    {
        private PdfViewer PDFViewer;
        private IQueryable<PROCESO_USUARIO> permisos;
        #region Generales

        private async void TrasladoExternoLoad(TrasladoExternoView Window = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas).ContinueWith((prevTask) => {
                    Limpiar();
                });
                SetValidacionesTraslados();
                PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                StaticSourcesViewModel.SourceChanged = false;
                ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
            }
        }

        public void ClickBuscar(Object parametro)
        {
            buscarImputado(parametro);
        }

        public async void clickSwitch(object parametro)
        {
            try
            {
                if (parametro != null)
                {
                    switch (parametro.ToString())
                    {
                        case "buscar_salir":
                            resetIngresoAnterior();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            break;
                        case "nueva_busqueda":
                            ListExpediente.Clear();
                            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                            FolioBuscar = AnioBuscar = null;
                            SelectExpediente = new IMPUTADO();
                            EmptyExpedienteVisible = true;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            SelectIngresoEnabled = false;
                            break;
                        case "buscar_seleccionar":
                            var _tipo_error = 0;

                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando Informacion", () =>
                            {
                                if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                                {
                                    _tipo_error = 1;
                                    return false;
                                }
                                if (SelectIngreso.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                                {
                                    _tipo_error = 2;
                                    return false;
                                }
                                AnioD = SelectIngreso.ID_ANIO;
                                FolioD = SelectIngreso.ID_IMPUTADO;
                                NombreD = SelectExpediente.NOMBRE;
                                PaternoD = SelectExpediente.PATERNO;
                                MaternoD = SelectExpediente.MATERNO;
                                IngresosD = SelectIngreso.ID_INGRESO;
                                if (SelectIngreso.CAMA != null)
                                {
                                    UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                                               SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                               SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                               SelectIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                                                               SelectIngreso.ID_UB_CAMA);
                                }
                                else
                                {
                                    UbicacionD = string.Empty;
                                }
                                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                                selectIngresoAuxiliar = SelectIngreso;
                                return true;
                            }))
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            else
                                switch (_tipo_error)
                                {
                                    case 1:
                                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                        break;
                                    case 2:
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                        SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                                        break;
                                }
                            break;
                        case "ampliar_justificacion":
                            TituloHeaderExpandirDescripcion = "Justificación";
                            TextAmpliarDescripcion = DTJustificacion;
                            MaxLengthAmpliarDescripcion = 1000;
                            Justificacion = true;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                            break;
                        case "guardar_ampliar_descripcion":
                            if (Justificacion)
                                DTJustificacion = TextAmpliarDescripcion;
                            else
                                DTNoOficio = TextAmpliarDescripcion;
                            TextAmpliarDescripcion = string.Empty;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                            break;
                        case "cancelar_ampliar_descripcion":
                            TextAmpliarDescripcion = string.Empty;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                            break;
                        case "guardar_menu":
                            if (base.HasErrors)
                                new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            else
                            {
                                var amparo = new cAmparoIndirectoTipos().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, null, 5, 2).FirstOrDefault();
                                if (amparo == null)
                                {
                                    if (await EvaluarGuardar())
                                        ConfiguraPermisos();
                                    else
                                        return;
                                }
                                else
                                {
                                    var res = await new Dialogos().ConfirmacionTresBotonesDinamico("Mensaje", "El interno " + ObtieneNombre(SelectIngreso.IMPUTADO) + " tiene un amparo para traslado", "Agregar", Convert.ToInt32(Tipo_Respuesta.Agregar),
                                        "Ver amparo", Convert.ToInt32(Tipo_Respuesta.Mostrar_Documento), string.Empty, Convert.ToInt32(Tipo_Respuesta.Cancelar));
                                    switch ((Tipo_Respuesta)res)
                                    {
                                        case Tipo_Respuesta.Agregar:
                                            if (await EvaluarGuardar())
                                                ConfiguraPermisos();
                                            break;
                                        case Tipo_Respuesta.Cancelar:
                                            break;
                                        case Tipo_Respuesta.Mostrar_Documento:
                                            var encontro_amparo_doc = new cAmparoIndirectoDocto().Obtener(amparo.ID_CENTRO, amparo.ID_ANIO, amparo.ID_IMPUTADO, amparo.ID_INGRESO, amparo.ID_AMPARO_INDIRECTO).FirstOrDefault();
                                            if (encontro_amparo_doc == null)
                                                new Dialogos().ConfirmacionDialogo("Mensaje", "El documento del amparo no se encuentra en el sistema");
                                            else
                                            {
                                                var _file = encontro_amparo_doc.DOCUMENTO;
                                                await Task.Factory.StartNew(() =>
                                                {
                                                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                                                    File.WriteAllBytes(fileNamepdf, _file);
                                                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                                    {
                                                        PDFViewer.LoadFile(fileNamepdf);
                                                        PDFViewer.Visibility = Visibility.Visible;
                                                    }));
                                                });
                                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case "buscar_menu":
                            await BuscarTraslado(GlobalVar.gCentro, new List<string>() { "PR" }, null, "LF");
                            LimpiarBusquedaTraslados();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            SetValidacionesBuscaTraslados();
                            break;
                        case "cancelar_buscar_traslados":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            SetValidacionesTraslados();
                            break;
                        case "filtro_traslados":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            var _tipo_traslado_id = string.Empty;
                            if (SelectedTipo_Traslado != null)
                                _tipo_traslado_id = SelectedTipo_Traslado.ID;
                            await BuscarTraslado(GlobalVar.gCentro, new List<string>() { "PR" }, FechaBuscarTraslado, _tipo_traslado_id, (short?)AnioBuscarTraslado, FolioBuscarTraslado, NombreBuscarTraslado, ApellidoPaternoBuscarTraslado, ApellidoMaternoBuscarTraslado);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            break;
                        case "seleccionar_traslado":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                            }
                            else
                            {
                                if (StaticSourcesViewModel.SourceChanged)
                                {
                                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                        "Existen cambios sin guardar,¿desea seleccionar un traslado para editar?") != 1)
                                        return;
                                }
                                DTFecha = SelectedTraslado.TRASLADO_FEC;
                                DTMotivo = SelectedTraslado.ID_MOTIVO;
                                DTJustificacion = SelectedTraslado.JUSTIFICACION;

                                DTNoOficio = SelectedTraslado.OFICIO_AUTORIZACION;
                                //id_autoridad_traslado = SelectedTraslado.AUTORIZA_TRASLADO.Value;
                                Autoridad_Traslado = selectedTraslado.AUTORIZA_TRASLADO;
                                DENoOficio = SelectedTraslado.OFICIO_SALIDA;
                                AutorizaSalida = AutoridadesSalida.FirstOrDefault(w => w == SelectedTraslado.AUTORIZA_SALIDA);

                                
                                var id_motivo_traslado = SelectedTraslado.ID_MOTIVO_SALIDA;
                                if (id_motivo_traslado.HasValue)
                                    MotivoSalida = new cTrasladoMotivoSalida().Obtener(id_motivo_traslado.Value).DESCR;

                                SelectedEmisor = SelectedTraslado.ID_CENTRO_DESTINO_FORANEO.HasValue? SelectedTraslado.ID_CENTRO_DESTINO_FORANEO.Value:-1;
                                if (SelectedEmisor != Parametro.ID_EMISOR_OTROS)
                                {
                                    OtroDestinoVisible=Visibility.Collapsed;
                                    OtroDestino=string.Empty;
                                }
                                else
                                {
                                    OtroDestinoVisible = Visibility.Visible;
                                    OtroDestino=SelectedTraslado.OTRO_CENTRO_DESTINO_FORANEO;
                                }

                                selectIngresoAuxiliar= SelectIngreso = SelectedTraslado.TRASLADO_DETALLE.First().INGRESO;

                                AnioD = SelectIngreso.ID_ANIO;
                                FolioD = SelectIngreso.ID_IMPUTADO;
                                NombreD = SelectIngreso.IMPUTADO.NOMBRE;
                                PaternoD = SelectIngreso.IMPUTADO.PATERNO;
                                MaternoD = SelectIngreso.IMPUTADO.MATERNO;
                                IngresosD = SelectIngreso.ID_INGRESO;
                                if (SelectIngreso.CAMA != null)
                                {
                                    UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                                               SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                               SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                               SelectIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                                                               SelectIngreso.ID_UB_CAMA);
                                }
                                else
                                {
                                    UbicacionD = string.Empty;
                                }
                                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                                
                                HabilitaBusquedaImputado(false);
                                SetValidacionesTraslados();
                                StaticSourcesViewModel.SourceChanged = false;
                                if (permisos.Any(w => w.EDITAR == 1))
                                {
                                    MenuGuardarEnabled = true;
                                    EliminarMenuEnabled = true;
                                }
                                CancelarMenuEnabled = true;
                            }
                            break;
                        case "cancelar_menu":
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea cancelar la edición del traslado?") != 1)
                                    return;
                            }
                            Limpiar();
                            LimpiarBusquedaTraslados();
                            SelectedTraslado = null;
                            SelectIngreso = null;
                            HabilitaBusquedaImputado(true);
                            CancelarMenuEnabled = false;
                            SetValidacionesTraslados();
                            ConfiguraPermisos();
                            break;
                        case "menu_eliminar":
                            if (SelectedTraslado != null)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                        "¿Esta seguro de cancelar el traslado?") != 1)
                                    return;
                                Eliminar();
                                HabilitaBusquedaImputado(true);
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación!", "No ha seleccionado ningun traslado");
                            break;
                        case "limpiar_menu":
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                                    return;
                            }
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoExternoView();
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.TrasladoExternoViewModel();
                            break;
                        case "salir_menu":
                            SalirMenu();
                            break;
                        case "reporte_menu":
                            if (selectedTraslado == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Para imprimir el reporte debe de seleccionar un traslado");
                                return;
                            }
                            GeneraReporte();
                            break;
                        case "cerrar_visualizador_documentos":
                            cerrarVisualizadorDocumentos();
                            break;
                        case "cancelar_estatus_excarcelacion":
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "¿Desea cancelar las excarcelaciones del imputado?") != 1)
                                return;
                            SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando excarcelaciones", () =>
                            {
                                new cExcarcelacion().CancelarExcarcelacionesPorImputado(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO,
                                   SelectIngreso.ID_INGRESO, Fechas.GetFechaDateServer, (short)enumMensajeTipo.CANCELACION_EXCARCELACION, (short)Parametro.ID_HOSPITAL_OTROS);
                                return true;
                            }))
                            {
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                                Guardar();
                                ConfiguraPermisos();                                
                            }
                            break;
                        case "salir_cancelar_excarcelacion":
                            new Dialogos().ConfirmacionDialogo("Validación!", "No se puede agregar el traslado si el imputado tiene excarcelaciones PROGRAMADAS o AUTORIZADAS!");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        private async void cerrarVisualizadorDocumentos()
        {
            try
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                var amparo = new cAmparoIndirectoTipos().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, null, 5, 2).FirstOrDefault();
                var res = await new Dialogos().ConfirmacionTresBotonesDinamico("Mensaje", "El interno " + ObtieneNombre(SelectIngreso.IMPUTADO) + " tiene un amparo para traslado", "Agregar", Convert.ToInt32(Tipo_Respuesta.Agregar),
                                "Ver amparo", Convert.ToInt32(Tipo_Respuesta.Mostrar_Documento), string.Empty, Convert.ToInt32(Tipo_Respuesta.Cancelar));
                switch ((Tipo_Respuesta)res)
                {
                    case Tipo_Respuesta.Agregar:
                        if (await EvaluarGuardar());
                            ConfiguraPermisos();
                        break;
                    case Tipo_Respuesta.Cancelar:
                        break;
                    case Tipo_Respuesta.Mostrar_Documento:
                        var encontro_amparo_doc = new cAmparoIndirectoDocto().Obtener(amparo.ID_CENTRO, amparo.ID_ANIO, amparo.ID_IMPUTADO, amparo.ID_INGRESO, amparo.ID_AMPARO_INDIRECTO).FirstOrDefault();
                        if (encontro_amparo_doc == null)
                            new Dialogos().ConfirmacionDialogo("Mensaje", "El documento del amparo no se encuentra en el sistema");
                        else
                        {
                            var _file = encontro_amparo_doc.DOCUMENTO;
                            await Task.Factory.StartNew(() =>
                            {
                                var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                                File.WriteAllBytes(fileNamepdf, _file);
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    PDFViewer.LoadFile(fileNamepdf);
                                    PDFViewer.Visibility = Visibility.Visible;
                                }));
                            });
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el imputado", ex);
            }

        }

        private void PrepararListas()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    try
                    {
                        if (LstMotivo == null)
                        {
                            LstMotivo = new ObservableCollection<TRASLADO_MOTIVO>(new cTrasladoMotivo().ObtenerTodos());
                            LstMotivo.Insert(0, new TRASLADO_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                        }
                        if (LstCentroDestinoForaneo==null)
                        {
                            LstCentroDestinoForaneo = new ObservableCollection<EMISOR>(new cEmisor().ObtenerporTipo(1).OrderBy(w=>w.DESCR));
                            LstCentroDestinoForaneo.Insert(0, new EMISOR() { ID_EMISOR=-1, DESCR="SELECCIONE"});
                        }
                        id_motivo_traslado = Convert.ToInt16(new cParametro().Seleccionar("MOTIVO_TRASLADO", 0).First().VALOR_NUM);
                        if (id_motivo_traslado.HasValue)
                            MotivoSalida = new cTrasladoMotivoSalida().Obtener(id_motivo_traslado.Value).DESCR;
                        //id_autoridad_traslado = new cParametro().Seleccionar("AUTORIDAD_TRASLADO", 0).First().VALOR_NUM.Value;
                        Autoridad_Traslado = new cParametro().Seleccionar("AUTORIDAD_TRASLADO", 0).First().VALOR;
                        AutoridadesSalida.Add(new cCentro().Obtener(GlobalVar.gCentro).First().DIRECTOR);
                        AutoridadesSalida.Add(new cParametro().Seleccionar("DIR_JURIDICO_CENTRO", GlobalVar.gCentro).First().VALOR);
                        AutoridadesSalida.Add(new cParametro().Seleccionar("SUBDIR_JURIDICO_CENTRO", GlobalVar.gCentro).First().VALOR);
                        AutoridadesSalida.Add("SELECCIONE");
                        FechaServer = Fechas.GetFechaDateServer;
                        Tipos_Traslado = new List<Tipo_Traslado> { new Tipo_Traslado("LO", "LOCAL"), new Tipo_Traslado("LF", "FORANEO"), new Tipo_Traslado("T", "TODOS") };
                        Tipo_TrasladoHabilitado = false;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Limpiar()
        {
            //Datos Traslado
            DTFecha = null;
            DTMotivo =  -1;
            SelectedEmisor = -1;
            OtroDestino = string.Empty;
            OtroDestinoVisible = Visibility.Collapsed;
            DTJustificacion = DTNoOficio = string.Empty;
            DENoOficio = string.Empty;
            AutorizaSalida = AutoridadesSalida.First(w => w == "SELECCIONE").ToString();
            NombreD = PaternoD = MaternoD = string.Empty;
            AnioD = FolioD=IngresosD = null;
            UbicacionD = TipoSeguridadD = ClasificacionJuridicaD = EstatusD = string.Empty;
            FecIngresoD = null;
            ImagenIngreso = null;
            StaticSourcesViewModel.SourceChanged = false;
        }

        private async Task<bool> EvaluarGuardar()
        {
            if (SelectIngreso.EXCARCELACION != null && SelectIngreso.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC" || w.ID_ESTATUS == "EP" || w.ID_ESTATUS == "AU" || w.ID_ESTATUS == "PR"))
            {
                IQueryable<EXCARCELACION_DESTINO> _excarcelacion_destinos = null;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    _excarcelacion_destinos = new cExcarcelacion_Destino().Seleccionar(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO,
                        SelectIngreso.ID_INGRESO, new List<string>() { "PR", "AU", "EP", "AC" });
                });
                if (SelectIngreso.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC" || w.ID_ESTATUS == "EP"))
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "El imputado tiene una excarcelación EN PROCESO o ACTIVA");
                    return false;
                }
                TituloExcarcelaciones = string.Format("EXCARCELACIONES PROGRAMADAS O AUTORIZADAS DEL IMPUTADO {0}/{1} {2}", SelectIngreso.ID_ANIO,
                    SelectIngreso.ID_IMPUTADO, ObtieneNombre(SelectIngreso.IMPUTADO));
                Excarcelacion_Destinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(_excarcelacion_destinos.Where(w => w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AU").Select(s => new CT_EXCARCELACION_DESTINO
                {
                    FECHA_EXCARCELACION = s.EXCARCELACION.PROGRAMADO_FEC.Value,
                    DESTINO = s.EXCARCELACION.ID_TIPO_EX.Value == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ?
                            s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL.Value == Parametro.ID_HOSPITAL_OTROS ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                            : s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR : ""
                    //DESTINO = s.EXCARCELACION.ID_TIPO_EX.Value == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.ID_HOSPITAL.Value == Parametro.ID_HOSPITAL_OTROS ?
                    //s.HOSPITAL_OTRO : s.HOSPITAL.DESCR
                }));
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                return false;
            }
            else
            {
                Guardar();
                return true;
            }
        }

        private async void Guardar()
        {
            try
            {
                if (SelectedTraslado == null)
                {
                    var traslado_detalle = new List<TRASLADO_DETALLE>();
                    traslado_detalle.Add(new TRASLADO_DETALLE
                    {
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_CENTRO_TRASLADO = GlobalVar.gCentro,
                        ID_ESTATUS = "PR",
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        
                    });
                    var _traslado = new TRASLADO
                    {
                        AUTORIZA_SALIDA = AutorizaSalida,
                        AUTORIZA_TRASLADO = Autoridad_Traslado,
                        ID_CENTRO = GlobalVar.gCentro,
                        CENTRO_ORIGEN = GlobalVar.gCentro,
                        ORIGEN_TIPO = "L",  //como es traslado originado en los centros estatales el tipo es L "LOCAL"
                        ID_ESTATUS = "PR",
                        ID_MOTIVO = DTMotivo,
                        ID_MOTIVO_SALIDA = id_motivo_traslado.Value,
                        JUSTIFICACION = DTJustificacion,
                        OFICIO_AUTORIZACION = DTNoOficio,
                        OFICIO_SALIDA = DENoOficio,
                        TRASLADO_DETALLE = traslado_detalle,
                        TRASLADO_FEC = DTFecha.Value,
                        ID_CENTRO_DESTINO_FORANEO=SelectedEmisor,
                        OTRO_CENTRO_DESTINO_FORANEO=OtroDestino
                    };
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Insertando traslado", () =>
                    {
                        new cTraslado().Insertar(_traslado, (short)enumMensajeTipo.CALENDARIZACION_TRASLADO, _FechaServer);
                        return true;
                    }))
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido registrado");
                }
                else
                {
                    var _traslado = new TRASLADO
                    {
                        AUTORIZA_SALIDA = AutorizaSalida,
                        AUTORIZA_TRASLADO = Autoridad_Traslado,
                        ID_CENTRO = GlobalVar.gCentro,
                        CENTRO_ORIGEN = GlobalVar.gCentro,
                        ORIGEN_TIPO = "L",  //como es traslado originado en los centros estatales el tipo es L "LOCAL"
                        ID_ESTATUS = "PR",
                        ID_MOTIVO = DTMotivo,
                        ID_MOTIVO_SALIDA = id_motivo_traslado.Value,
                        JUSTIFICACION = DTJustificacion,
                        OFICIO_AUTORIZACION = DTNoOficio,
                        OFICIO_SALIDA = DENoOficio,
                        TRASLADO_FEC = DTFecha.Value,
                        ID_TRASLADO=SelectedTraslado.ID_TRASLADO,
                        ID_CENTRO_DESTINO_FORANEO = SelectedEmisor,
                        OTRO_CENTRO_DESTINO_FORANEO = OtroDestino
                        
                    };
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando traslado", () =>
                    {
                        new cTraslado().Actualizar(_traslado, (short)enumMensajeTipo.CALENDARIZACION_TRASLADO, _FechaServer);
                        return true;
                    }))
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido registrado");
                }
                CancelarMenuEnabled = false;
                Limpiar();
                SelectIngreso  = null;


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salvar el traslado.", ex);
            }
        }

        private async void Eliminar()
        {
            try
            {


                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando traslado", () =>
                {
                    var traslado_detalle = new List<TRASLADO_DETALLE>();
                    foreach (var item in SelectedTraslado.TRASLADO_DETALLE)
                    {
                        traslado_detalle.Add(new TRASLADO_DETALLE
                        {
                            ID_ANIO = item.ID_ANIO,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_CENTRO_TRASLADO = GlobalVar.gCentro,
                            ID_ESTATUS = "CA",
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_INGRESO = item.ID_INGRESO,
                            ID_ESTATUS_ADMINISTRATIVO = item.ID_ESTATUS_ADMINISTRATIVO,
                            ID_TRASLADO = item.ID_TRASLADO
                        });
                    }
                    var _traslado = new TRASLADO
                    {
                        AUTORIZA_SALIDA = SelectedTraslado.AUTORIZA_SALIDA,
                        AUTORIZA_TRASLADO = SelectedTraslado.AUTORIZA_TRASLADO,
                        ID_CENTRO = SelectedTraslado.ID_CENTRO,
                        CENTRO_ORIGEN = SelectedTraslado.CENTRO_ORIGEN,
                        ORIGEN_TIPO = SelectedTraslado.ORIGEN_TIPO,
                        ID_ESTATUS = "CA",
                        ID_MOTIVO = SelectedTraslado.ID_MOTIVO,
                        ID_MOTIVO_SALIDA = SelectedTraslado.ID_MOTIVO_SALIDA,
                        JUSTIFICACION = SelectedTraslado.JUSTIFICACION,
                        OFICIO_AUTORIZACION = SelectedTraslado.OFICIO_AUTORIZACION,
                        OFICIO_SALIDA = SelectedTraslado.OFICIO_SALIDA,
                        CENTRO_DESTINO = SelectedTraslado.CENTRO_DESTINO,
                        TRASLADO_DETALLE = traslado_detalle,
                        TRASLADO_FEC = SelectedTraslado.TRASLADO_FEC,
                        ID_TRASLADO = SelectedTraslado.ID_TRASLADO
                    };
                    new cTraslado().Actualizar(_traslado, (short)enumMensajeTipo.CALENDARIZACION_TRASLADO, _FechaServer);
                    return true;
                }))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido cancelado con éxito");
                    CancelarMenuEnabled = false;
                    Limpiar();
                    SelectedTraslado = null;
                }


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salvar el traslado.", ex);
            }
        }

        public async static void SalirMenu()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                    if (dialogresult != 0)
                        StaticSourcesViewModel.SourceChanged = false;
                    else
                        return;
                }

                var metro = Application.Current.Windows[0] as MetroWindow;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                GC.Collect();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = new BandejaEntradaView();
                ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = new BandejaEntradaViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del módulo", ex);
            }
        }

        private string ObtieneNombre(IMPUTADO imp)
        {
            return string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE) ? imp.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO) ? imp.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO) ? imp.MATERNO.Trim() : string.Empty);
        }

        private async void CambioModel(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_centro_destino":
                        if (SelectedEmisor == Parametro.ID_EMISOR_OTROS)  //999999 es el valor de otro en el catalogo de centros de plataforma mexico
                            OtroDestinoVisible = Visibility.Visible;
                        else
                        {
                            OtroDestinoVisible = Visibility.Collapsed;
                            OtroDestino = string.Empty;
                        }
                            
                        break;
                    #region Cambio SelectedItem de Busqueda de Expediente
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = selectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                            {
                                SelectIngreso = null;
                                EmptyIngresoVisible = true;
                            }


                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                                TextBotonSeleccionarIngreso = "aceptar";
                                SelectIngresoEnabled = true;


                                if (estatus_inactivos != null && SelectIngreso != null && estatus_inactivos.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO))
                                {
                                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                                    SelectIngresoEnabled = false;
                                }
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                    #endregion
                }
            }
        }
        #endregion
        #region Busqueda

        private void HabilitaBusquedaImputado(bool habilitado=true)
        {
            BuscarImputadoHabilitado = habilitado;
            FolioBuscarHabilitado = habilitado;
            AnioBuscarHabilitado = habilitado;
            NombreBuscarHabilitado = habilitado;
            ApellidoMaternoBuscarHabilitado = habilitado;
            ApellidoPaternoBuscarHabilitado = habilitado;
        }

        private void ClickBuscarInterno(object parametro)
        {
            buscarImputadoInterno(parametro);
        }
        private async void buscarImputado(Object obj = null)
        {
            try
            {
                NombreBuscar = NombreD;
                ApellidoPaternoBuscar = PaternoD;
                ApellidoMaternoBuscar = MaternoD;
                FolioBuscar = FolioD;
                AnioBuscar = AnioD;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (AnioBuscar.HasValue && FolioBuscar.HasValue && ListExpediente.Count()==1)
                {
                    if (ListExpediente[0].INGRESO == null || ListExpediente[0].INGRESO.Count() == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso para este imputado.");
                        resetIngresoAnterior();
                        return;
                    }
                    var _ingreso=ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    if (estatus_inactivos.Contains(_ingreso.ID_ESTATUS_ADMINISTRATIVO))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        resetIngresoAnterior();
                        return;
                    }
                    if (_ingreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        resetIngresoAnterior();
                        return;
                    }
                    if (_ingreso.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        resetIngresoAnterior();
                        return;
                    }
                    selectIngresoAuxiliar = SelectIngreso = _ingreso;
                    SelectExpediente = ListExpediente[0];
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    NombreD = SelectExpediente.NOMBRE;
                    PaternoD = SelectExpediente.PATERNO;
                    MaternoD = SelectExpediente.MATERNO;
                    IngresosD = SelectIngreso.ID_INGRESO;
                    if (SelectIngreso.CAMA != null)
                    {
                        UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                                   SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                   SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                   SelectIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                                                   SelectIngreso.ID_UB_CAMA);
                    }
                    else
                    {
                        UbicacionD = string.Empty;
                    }
                    TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                    FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                    ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                }
                else
                {
                    SelectIngresoEnabled = false;
                    if (ListExpediente != null)
                        EmptyExpedienteVisible = ListExpediente.Count < 0;
                    else
                        EmptyExpedienteVisible = true;

                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    CrearNuevoExpedienteEnabled = false;
                }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar el imputado", ex);
            }
            
        }

        private void resetIngresoAnterior()
        {
            SelectIngreso = selectIngresoAuxiliar;
            if (selectIngresoAuxiliar!=null)
            {
                AnioD = selectIngresoAuxiliar.ID_ANIO;
                FolioD = selectIngresoAuxiliar.ID_IMPUTADO;
                NombreD = selectIngresoAuxiliar.IMPUTADO.NOMBRE;
                PaternoD = selectIngresoAuxiliar.IMPUTADO.PATERNO;
                MaternoD = selectIngresoAuxiliar.IMPUTADO.MATERNO;
                IngresosD = selectIngresoAuxiliar.ID_INGRESO;
            }
        }


        private async void buscarImputadoInterno(Object obj = null)
        {
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            SelectIngresoEnabled = false;
            if (ListExpediente != null)
                EmptyExpedienteVisible = ListExpediente.Count < 0;
            else
                EmptyExpedienteVisible = true;
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
        #endregion

        #region Busqueda Traslados
        private void LimpiarBusquedaTraslados()
        {
            SelectedTipo_Traslado = Tipos_Traslado.First(w => w.ID == "LF");
            AnioBuscarTraslado = null;
            FolioBuscarTraslado = null;
            NombreBuscarTraslado = string.Empty;
            ApellidoPaternoBuscarTraslado = string.Empty;
            ApellidoMaternoBuscarTraslado = string.Empty;
        }

        private async Task BuscarTraslado(short? centro = null, List<string> estatus = null, DateTime? fecha = null, string tipo_traslado_local = "", short? anio = null, int? imputado = null, string nombre = "", string paterno = "", string materno = "")
        {
            if (tipo_traslado_local == "T")
                tipo_traslado_local = string.Empty;
            BusquedaTraslado = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TRASLADO>>(() => new ObservableCollection<TRASLADO>(new cTraslado().ObtenerTodos(centro, estatus, fecha, "L", tipo_traslado_local, anio, imputado, nombre, paterno, materno)));
        }
        #endregion
        #region Reporte Traslados

        private async void GeneraReporte()
        {
            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Generando datos del reporte", GeneraReporteDatos))
                ReportViewer_Requisicion();
        }

        private bool GeneraReporteDatos()
        {
            try
            {
                ds_detalle = SelectedTraslado.TRASLADO_DETALLE
                    .Select(s => new EXT_REPORTE_TRASLADO_DETALLE
                    {
                        CERESO_DESTINO = "FORANEO",
                        EXPEDIENTE = s.ID_ANIO.ToString() + "/" + s.ID_CENTRO.ToString().PadLeft(2, '0') + "-" + s.ID_INGRESO.ToString(),
                        FEC_TRASLADO = s.TRASLADO.TRASLADO_FEC,
                        MOTIVO_TRASLADO = s.TRASLADO.TRASLADO_MOTIVO.DESCR,
                        NOMBRECOMPLETO = ObtieneNombre(s.INGRESO.IMPUTADO),
                        UBICACION = s.INGRESO.CAMA != null ?
                            s.INGRESO.CAMA.CELDA != null ?
                                s.INGRESO.CAMA.CELDA.SECTOR != null ?
                                    s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ?
                                        s.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + s.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() + "" + s.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + s.INGRESO.CAMA.ID_CAMA
                                    : string.Empty
                                : string.Empty
                            : string.Empty
                        : string.Empty,
                    }).ToList();
                var logo_bc = new cParametro().Seleccionar("LOGO_ESTADO", 0).FirstOrDefault().CONTENIDO;
                var centro = new cCentro().Obtener(SelectedTraslado.CENTRO_ORIGEN.Value).FirstOrDefault().DESCR;
                ds_encabezado = new List<EXT_REPORTE_TRASLADO_ENCABEZADO>() {new EXT_REPORTE_TRASLADO_ENCABEZADO{
                LOGO_BC=logo_bc,
                FEC_TRASLADO=SelectedTraslado.TRASLADO_FEC,
                CENTRO_ORIGEN=centro
                } };
                return true;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void ReportViewer_Requisicion()
        {
            try
            {
                var _reporte = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                _reporte.Closed += (s, e) =>
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                };
                _reporte.Owner = PopUpsViewModels.MainWindow;
                _reporte.Show();
                _reporte.Report.LocalReport.ReportPath = "Reportes/rTraslados_Egresos.rdlc";
                _reporte.Report.LocalReport.DataSources.Clear();
                Microsoft.Reporting.WinForms.ReportDataSource rsd1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd1.Name = "DS_DETALLE";
                rsd1.Value = ds_detalle;
                Microsoft.Reporting.WinForms.ReportDataSource rsd2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rsd2.Name = "DS_ENCABEZADO";
                rsd2.Value = ds_encabezado;
                _reporte.Report.LocalReport.DataSources.Add(rsd1);
                _reporte.Report.LocalReport.DataSources.Add(rsd2);
                _reporte.Report.RefreshReport();
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", ex);
            }
            
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuGuardarEnabled = false;
                EliminarMenuEnabled = false;
                MenuReporteEnabled = false;
                MenuBuscarEnabled = false;
                permisos = new cProcesoUsuario().Obtener(enumProcesos.TRASLADOMASIVO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Count() > 0)
                    MenuLimpiarEnabled = true;
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        MenuGuardarEnabled = true;

                    if (p.CONSULTAR == 1)
                    {
                        MenuBuscarEnabled = true;
                        EliminarMenuEnabled = true;
                    }

                    if (p.IMPRIMIR == 1)
                        MenuReporteEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #endregion
    }
}
