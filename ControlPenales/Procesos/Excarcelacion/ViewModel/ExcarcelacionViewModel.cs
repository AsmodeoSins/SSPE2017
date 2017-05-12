using ControlPenales.BiometricoServiceReference;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFPdfViewer;
using SSP.Servidor.ModelosExtendidos;
using System.Windows.Interactivity;
using System.Windows.Data;
using System.Data.Objects;

namespace ControlPenales
{
    partial class ExcarcelacionViewModel : ValidationViewModelBase
    {

        private enumVentanaOrigen_Excarcelacion ventana_origen;
        private decimal? nuc;
        private List<ControlEvento> _lista_control_evento;

        /// <summary>
        /// metodo constructor de la clase
        /// </summary>
        /// <param name="_ventana_origen" tipo="int">Origen donde se llama el constructor</param>
        /// <param name="_nuc" tipo="decimal?" opcional>NUC del imputado</param>
        /// <param name="_documento" tipo="byte[]" opcional>Documento relacionado con la notificacion de la excarcelacion</param>
        /// <param name="_formato_doc" tipo="short?" opcional>Formato del documento relacionado con la notificacion de la excarcelacion</param>
        /// <returns></returns>
        public ExcarcelacionViewModel(enumVentanaOrigen_Excarcelacion _ventana_origen, decimal? _nuc, byte[] _documento, short? _formato_doc)
        {
            ventana_origen = _ventana_origen;
            nuc = _nuc;
            if (nuc.HasValue)
            {
                documento = new byte[_documento.Count()];
                _documento.CopyTo(documento, 0);
                formato_documentacion_excarcelacion = _formato_doc;
                tipo_documento_excarcelacion = 1;
            }
        }

        private IQueryable<PROCESO_USUARIO> permisos;
        #region Generales

        public async void ExcarcelacionOnLoad(ExcarcelacionView Window = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    ObtenerExcarcelacionTipos(true);
                }).ContinueWith((prevTask) =>
                {
                    Limpiar();
                });

                ConfiguraPermisos();
                if (ventana_origen == enumVentanaOrigen_Excarcelacion.BANDEJA_ENTRADA)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        ObtenerImputadosNUC(GlobalVar.gCentro, nuc.Value.ToString(), true);
                        Lista_Sources = escaner.Sources();
                        if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                        HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);
                    });
                    HeaderNUC = nuc.Value.ToString();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_IMPUTADO_NUC);
                }

                escaner.EscaneoCompletado += delegate
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    if (AutoGuardado)
                    {
                        if (DocumentoDigitalizado != null)
                        {
                            GuardarDocumento();
                        }
                    }
                    else
                    {
                        if (SelectedTipoDocumento != null && ListTipoDocumento.FirstOrDefault(w => w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO) != null)
                        {
                            foreach (var item in ListTipoDocumento.Where(w => w.DIGITALIZADO == true))
                                item.DIGITALIZADO = false;
                            ListTipoDocumento.FirstOrDefault(w => w.ID_TIPO_DOCUMENTO == SelectedTipoDocumento.ID_TIPO_DOCUMENTO).DIGITALIZADO = true;
                            ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                        }
                    }
                    escaner.Dispose();

                };

                Lista_Sources = escaner.Sources();
                if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);

                StaticSourcesViewModel.SourceChanged = false;
                setValidacionesExcarcelacion();
                _lista_control_evento = new List<ControlEvento>();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el traslado", ex);
            }
        }

        public void ClickBuscar(Object parametro)
        {
            if (BuscarImputadoHabilitado)
                buscarImputado(parametro); //busqueda desde pantalla principal
        }
        private void resetIngresoAnterior()
        {
            SelectIngreso = selectIngresoAuxiliar;
            if (selectIngresoAuxiliar != null)
            {
                AnioD = selectIngresoAuxiliar.ID_ANIO;
                FolioD = selectIngresoAuxiliar.ID_IMPUTADO;
                NombreD = selectIngresoAuxiliar.IMPUTADO.NOMBRE;
                PaternoD = selectIngresoAuxiliar.IMPUTADO.PATERNO;
                MaternoD = selectIngresoAuxiliar.IMPUTADO.MATERNO;
                IngresosD = selectIngresoAuxiliar.ID_INGRESO;
            }
        }

        public async void clickSwitch(object parametro)
        {
            try
            {
                if (parametro != null)
                {
                    string _param = string.Empty;
                    byte[] _documento = null;
                    short? _formato_documento = null;
                    if (parametro.GetType().IsArray)
                    {
                        var params1 = (object[])parametro;
                        _param = params1[0].ToString();
                        _documento = (byte[])params1[1];
                        _formato_documento = Convert.ToInt16(params1[2]);
                    }
                    else
                        _param = parametro.ToString();

                    switch (_param)
                    {
                        case "buscar_CP":
                            BuscaCausaPenal(buscar_Causa_Penal);
                            break;
                        case "seleccionar_agregar_destino":
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            var _id_hospital_otro = Parametro.ID_HOSPITAL_OTROS;
                            var _id_destino = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? SelectedJuzgadoValue : SelectedHospitalValue;
                            if (listaExcarcelacionDestinos != null && listaExcarcelacionDestinos.Any(w => w.ID_DESTINO != _id_hospital_otro && w.ID_DESTINO == _id_destino && w.ESTATUS != "CA"))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "NO SE PUEDEN REPETIR DESTINOS!");
                                return;
                            }

                            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA)
                                if (SelectedInterconsultaExcarcelacion == null)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONE UNA INTERCONSULTA PARA CONTINUAR !");
                                    return;
                                };

                            if (_modo_destino == MODO_DESTINO.INSERTAR)
                            {

                                InsertarDestinos();
                            }

                            else
                                ActualizarDestinos();
                            setValidacionesExcarcelacion();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            break;
                        case "cancelar_agregar_destino":
                            setValidacionesExcarcelacion();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            break;
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
                            BuscarSeleccionarImputado(); //busqueda desde el popup
                            break;
                        case "digitalizar_excarcelacion":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            await ObtenerTipoDocumento();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                            break;
                        case "Cancelar_digitalizar_documentos":
                            escaner.Hide();
                            DocumentoDigitalizado = null;
                            ObservacionDocumento = string.Empty;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            break;
                        case "guardar_documento":
                            GuardarDocumento();
                            break;
                        case "lista_documentos":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                fechaInicioMensaje = _FechaServer.AddDays(-10).Date;
                                OnPropertyChanged("FechaInicioMensaje");
                                fechaFinalMensaje = _FechaServer.Date;
                                OnPropertyChanged("FechaFinalMensaje");
                                ObtenerListadoDocumentos(selectIngreso.ID_ANIO, selectIngreso.ID_CENTRO, selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO,
                                    Parametro.ID_TIPO_MENSAJE_INTERCONECCION, null, null, fechaInicioMensaje, fechaFinalMensaje, true);
                            });
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_NUCS_IMPUTADO);
                            break;
                        case "buscarNUC":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                ObtenerListadoDocumentos(selectIngreso.ID_ANIO, selectIngreso.ID_CENTRO, selectIngreso.ID_IMPUTADO, selectIngreso.ID_INGRESO,
                                    Parametro.ID_TIPO_MENSAJE_INTERCONECCION, buscarNUCMensaje, buscarCausaPenalMensaje, fechaInicioMensaje, fechaFinalMensaje, true);
                            });
                            break;
                        case "ver_documento":
                            if (SelectedDocumento == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONAR UN DOCUMENTO!");
                                return;
                            }

                            MostrarDocumento(Origen_Comando_Visualizacion.LISTADO_DOCUMENTOS);
                            break;
                        case "seleccionar_documento":
                            if (SelectedDocumento == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONAR UN DOCUMENTO!");
                                return;
                            }
                            Documento = new byte[selectedDocumento.DOCUMENTO.Count()];
                            selectedDocumento.DOCUMENTO.CopyTo(Documento, 0);
                            tipo_documento_excarcelacion = 1; //tipo de documento para notificacion de audiencia
                            formato_documentacion_excarcelacion = selectedDocumento.FORMATO_DOCUMENTO;
                            CP_Excarcelacion_Destino = selectedDocumento.CAUSA_PENAL;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_NUCS_IMPUTADO);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            if (CP_Excarcelacion_Destino == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Error de información!", "EL DOCUMENTO NO TIENE UNA CAUSA PENAL RELACIONADA!");
                                return;
                            }
                            if (!CP_Excarcelacion_Destino.CP_FOLIO.HasValue && !CP_Excarcelacion_Destino.CP_ANIO.HasValue)
                            {
                                new Dialogos().ConfirmacionDialogo("Error de información!", "LA CAUSA PENAL RELACIONADA NO TIENE UN FOLIO CAPTURADO!");
                                return;
                            }
                            IsBuscarCPEnabled = false;
                            Buscar_Causa_Penal = string.Empty;
                            CargarDestinoCP(CP_Excarcelacion_Destino.CP_ANIO.Value, CP_Excarcelacion_Destino.CP_FOLIO.Value);
                            break;
                        case "cancelar_documento":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_NUCS_IMPUTADO);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
                            break;
                        case "guardar_menu":
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            await Guardar();
                            ConfiguraPermisos();
                            break;
                        case "limpiar_menu":
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                                    return;
                            }
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ExcarcelacionView();
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ExcarcelacionViewModel(enumVentanaOrigen_Excarcelacion.MAIN_WINDOW, null, null, null);
                            break;
                        case "salir_menu":
                            SalirMenu();
                            break;
                        case "buscar_menu":

                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                ObtenerExcarcelacionTiposBusqueda(true);
                            }).ContinueWith((prevTask) =>
                            {
                                LimpiarBuscar();
                            });
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                BuscarExcarcelaciones(GlobalVar.gCentro, new List<string> { "PR", "AU" }, anioBuscarExc, folioBuscarExc, nombreBuscarExc,
                                    apellidoPaternoBuscarExc, apellidoMaternoBuscarExc, fechaInicialBuscarExc, fechaFinalBuscarExc,
                                    selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, selectedJuzgadoBuscarExcValue == 0 ? null : (short?)selectedJuzgadoBuscarExcValue,
                                    selectedFueroBuscarExcValue == "0" ? null : selectedFueroBuscarExcValue, selectedMunicipioBuscarExcValue == 0 ? null : (short?)selectedMunicipioBuscarExcValue,
                                    selectedEstadoBuscarExcValue == 0 ? null : (short?)selectedEstadoBuscarExcValue, selectedPaisBuscarExcValue == 0 ? null : (short?)selectedPaisBuscarExcValue,
                                    selectedHospitalBuscarExcValue == 0 ? null : (short?)selectedHospitalBuscarExcValue, string.IsNullOrWhiteSpace(otroHospitalBuscarExc) ? "" : otroHospitalBuscarExc,
                                    true);
                            });
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EXCARCELACIONES);

                            break;
                        case "filtro_excarcelaciones":
                            if (!IsFechaIniBusquedaValida)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "LA FECHA DE INICIO TIENE QUE SER MENOR A LA FECHA FIN!");
                                return;
                            }
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                BuscarExcarcelaciones(GlobalVar.gCentro, new List<string> { "PR", "AU" }, anioBuscarExc, folioBuscarExc, nombreBuscarExc,
                                    apellidoPaternoBuscarExc, apellidoMaternoBuscarExc, fechaInicialBuscarExc, fechaFinalBuscarExc,
                                    selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, selectedJuzgadoBuscarExcValue == 0 ? null : (short?)selectedJuzgadoBuscarExcValue,
                                    selectedFueroBuscarExcValue == "0" ? null : selectedFueroBuscarExcValue, selectedMunicipioBuscarExcValue == 0 ? null : (short?)selectedMunicipioBuscarExcValue,
                                    selectedEstadoBuscarExcValue == 0 ? null : (short?)selectedEstadoBuscarExcValue, selectedPaisBuscarExcValue == 0 ? null : (short?)selectedPaisBuscarExcValue,
                                    selectedHospitalBuscarExcValue == 0 ? null : (short?)selectedHospitalBuscarExcValue, string.IsNullOrWhiteSpace(otroHospitalBuscarExc) ? "" : otroHospitalBuscarExc,
                                    true);
                            });
                            break;
                        case "cancelar_buscar_excarcelacion":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EXCARCELACIONES);
                            break;
                        case "seleccionar_excarcelacion":
                            SeleccionarExcarcelacion();
                            break;
                        case "visualizar_documento_excarcelacion":
                            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA)
                            {
                                if (SelectedExcarcelacionDestino != null)
                                    MuestraOficioSolicitudExcarcelacion(SelectedExcarcelacionDestino.ID_INTERC);
                            }
                            else
                                MostrarDocumento(Origen_Comando_Visualizacion.VER_DOCUMENTO, _documento, _formato_documento);

                            break;
                        //case "cancelar_menu":
                        //    if (StaticSourcesViewModel.SourceChanged)
                        //    {
                        //        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                        //            "Existen cambios sin guardar,¿desea cancelar la edición de la excarcelación?") != 1)
                        //            return;
                        //    }
                        //    Limpiar();
                        //    AnioBuscarHabilitado = true;
                        //    FolioBuscarHabilitado = true;
                        //    NombreBuscarHabilitado = true;
                        //    ApellidoPaternoBuscarHabilitado = true;
                        //    ApellidoMaternoBuscarHabilitado = true;
                        //    BuscarImputadoHabilitado = true;
                        //    EliminarMenuEnabled = false;
                        //    CancelarMenuEnabled = false;
                        //    IsDatosExcarcelacionEnabled = true;
                        //    IsExcarcelacion_TiposEnabled = true;
                        //    ConfiguraPermisos();
                        //    break;
                        case "cancelar_menu":
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea cancelar la excarcelación?") != 1)
                                    return;
                            }
                            _modo_cancelacion = MODO_CANCELACION.GLOBAL;

                            MotivosCancelaciones();
                            break;
                        case "cancelar_imputado_nuc":

                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_IMPUTADO_NUC);
                            break;
                        case "seleccionar_imputado_nuc":
                            SeleccionarImputadoNUC();
                            break;
                        case "agregar_destino_excarcelacion":
                            if (selectIngreso == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validacion!", "SELECCIONAR UN IMPUTADO!");
                                return;
                            }
                            if (selectedExc_tipoValue == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validacion!", "SELECCIONAR UN TIPO DE EXCARCELACIÓN!");
                                return;
                            }
                            IsDocumentoFisicoEnabled = true;
                            _modo_destino = MODO_DESTINO.INSERTAR;
                            AgregarDestinoExcarcelacion();
                            break;
                        case "cancelar_excarcelacion":
                            if (SelectedExcarcelacionDestino == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONAR UN DESTINO DE EXCARCELACIÓN");
                                return;
                            }
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "Desea cancelar el destino de esta excarcelación?") != 1)
                                return;

                            if (SelectedExcarcelacionDestino.ID_CONSEC.HasValue)
                            {
                                _modo_cancelacion = MODO_CANCELACION.INDIVIDUAL;
                                MotivosCancelaciones();
                            }
                            else
                                CancelarRegistroExcarcelacionDestino();
                            break;
                        case "editar_destino_excarcelacion":
                            if (SelectedExcarcelacionDestino == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación!", "SELECCIONAR UN DESTINO DE EXCARCELACIÓN");
                                return;
                            }

                            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA)
                                return;

                            _modo_destino = MODO_DESTINO.EDICION;
                            EditarDestinoExcarcelacion();
                            break;
                        case "cancelar_agregar_canc_motivo":
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
                            break;
                        case "agregar_canc_motivo":
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            if (_modo_cancelacion == MODO_CANCELACION.INDIVIDUAL)
                            {
                                SelectedExcarcelacionDestino.ID_CANCELACION_MOTIVO = SelectedCancelacion_MotivoValue;
                                SelectedExcarcelacionDestino.CANCELACION_OBSERVACION = Cancelacion_Observacion;
                                CancelarRegistroExcarcelacionDestino();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
                                setValidacionesExcarcelacion();
                            }
                            else if (_modo_cancelacion == MODO_CANCELACION.GLOBAL)
                            {
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
                                await Cancelar();
                                Limpiar();
                                setValidacionesExcarcelacion();
                                ConfiguraPermisos();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        private void CancelarRegistroExcarcelacionDestino()
        {
            SelectedExcarcelacionDestino.ESTATUS = "CA";
            ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(ListaExcarcelacionDestinos);

            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA)
                if (SelectedExcarcelacionDestino != null)
                    ListaExcarcelacionDestinos.Remove(SelectedExcarcelacionDestino);

            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA)
            {
                if (!ListaExcarcelacionDestinos.Any(w => w.ESTATUS != "CA"))
                {
                    IsCertificadoEnabled = false;
                    CertMedicoNoChecked = false;
                    CertMedicoSiChecked = false;
                }
                else
                {
                    if (ListaExcarcelacionDestinos.Where(w => w.ESTATUS != "CA" && w.CAUSA_PENAL != null).Any(w => w.CAUSA_PENAL.NUC != null))
                    {
                        IsCertificadoEnabled = false;
                        CertMedicoNoChecked = false;
                        CertMedicoSiChecked = true;
                    }
                    else
                    {
                        if (Parametro.CERT_MEDICO_OBLIGATORIO == 1)
                        {
                            IsCertificadoEnabled = false;
                            CertMedicoNoChecked = false;
                            CertMedicoSiChecked = true;
                        }
                        else
                        {
                            IsCertificadoEnabled = true;
                            CertMedicoNoChecked = true;
                            CertMedicoSiChecked = false;
                        }
                    }
                }
            }
        }

        private async void MotivosCancelaciones()
        {
            base.ClearRules();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                ObtenerExcarcelacion_Cancela_Motivos(true);
            }).ContinueWith((prevTask) =>
            {
                if (_modo_cancelacion == MODO_CANCELACION.INDIVIDUAL && SelectedExcarcelacionDestino.ID_CANCELACION_MOTIVO.HasValue)
                {
                    if (cancelacion_Motivos != null && cancelacion_Motivos.FirstOrDefault(w => w.ID_EXCARCELACION_CANCELA == SelectedExcarcelacionDestino.ID_CANCELACION_MOTIVO.Value) != null)
                    {
                        selectedCancelacion_MotivoValue = SelectedExcarcelacionDestino.ID_CANCELACION_MOTIVO.Value;
                        OnPropertyChanged("SelectedCancelacion_MotivoValue");
                    }
                    cancelacion_Observacion = SelectedExcarcelacionDestino.CANCELACION_OBSERVACION;
                    OnPropertyChanged("Cancelacion_Observacion");
                }
                else
                {
                    if (cancelacion_Motivos != null && cancelacion_Motivos.FirstOrDefault(w => w.ID_EXCARCELACION_CANCELA == 0) != null)
                    {
                        selectedCancelacion_MotivoValue = 0;
                        OnPropertyChanged("SelectedCancelacion_MotivoValue");
                    }
                    cancelacion_Observacion = string.Empty;
                    OnPropertyChanged("Cancelacion_Observacion");
                }
            });

            setValidacionesExcarcelacion_Cancela_Motivo();
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
        }

        private async void EditarDestinoExcarcelacion()
        {
            foreach (var item in _lista_control_evento)
                Interaction.GetTriggers(item.Control).Remove(item.Evento);
            base.ClearRules();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                switch (selectedExc_tipoValue)
                {
                    case ((int)enumExcarcelacionTipo.JURIDICA):
                        var _destino_juridico = new cJuzgado().Seleccionar(selectedExcarcelacionDestino.ID_DESTINO.Value).FirstOrDefault();
                        if (_destino_juridico != null)
                        {
                            ObtenerFueros(true);
                            ObtenerPaises(true);
                            ObtenerEstados(_destino_juridico.ID_PAIS.Value, true);
                            ObtenerMunicipios(_destino_juridico.ID_ENTIDAD.Value, true);
                            ObtenerJuzgados(_destino_juridico.ID_PAIS.Value, _destino_juridico.ID_ENTIDAD.Value, _destino_juridico.ID_MUNICIPIO.Value, _destino_juridico.ID_FUERO, true);
                        }
                        break;
                    case ((int)enumExcarcelacionTipo.MEDICA):
                        ObtenerHospital(true);
                        break;
                }
            }).ContinueWith((prevTask) =>
            {
                switch (selectedExc_tipoValue)
                {
                    case ((int)enumExcarcelacionTipo.JURIDICA):
                        var _destino_juridico = new cJuzgado().Seleccionar(selectedExcarcelacionDestino.ID_DESTINO.Value).FirstOrDefault();
                        if (fueros != null)
                        {
                            selectedFueroValue = _destino_juridico.ID_FUERO;
                            OnPropertyChanged("SelectedFueroValue");
                        }
                        if (paises != null && paises.FirstOrDefault(w => w.ID_PAIS_NAC == _destino_juridico.ID_PAIS) != null)
                        {
                            selectedPaisValue = _destino_juridico.ID_PAIS.Value;
                            OnPropertyChanged("SelectedPaisValue");
                        }
                        if (estados != null && estados.FirstOrDefault(w => w.ID_ENTIDAD == _destino_juridico.ID_ENTIDAD) != null)
                        {
                            selectedEstadoValue = _destino_juridico.ID_ENTIDAD.Value;
                            OnPropertyChanged("SelectedEstadoValue");
                        }
                        if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == _destino_juridico.ID_MUNICIPIO) != null)
                        {
                            selectedMunicipioValue = _destino_juridico.ID_MUNICIPIO.Value;
                            OnPropertyChanged("SelectedMunicipioValue");
                        }
                        if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == _destino_juridico.ID_JUZGADO) != null)
                        {
                            selectedJuzgadoValue = _destino_juridico.ID_JUZGADO;
                            OnPropertyChanged("SelectedJuzgadoValue");
                        }
                        //cp_folio_destino = selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO;
                        //OnPropertyChanged("CP_Folio_Destino");
                        isJuridicaVisible = Visibility.Visible;
                        OnPropertyChanged("IsJuridicaVisible");
                        isMedicaVisible = Visibility.Collapsed;
                        OnPropertyChanged("IsMedicaVisible");
                        isDocumentoSistemaEnabled = false;
                        OnPropertyChanged("IsDocumentoSistemaEnabled");
                        isDocumentoFisicoEnabled = false;
                        OnPropertyChanged("IsDocumentoFisicoEnabled");
                        headerDatos = "DATOS JURIDICOS";
                        OnPropertyChanged("HeaderDatos");
                        isDatosVisible = Visibility.Visible;
                        OnPropertyChanged("IsDatosVisible");
                        setValidacionesJuridicas();
                        break;
                    case ((int)enumExcarcelacionTipo.MEDICA):
                        if (hospitales != null && hospitales.FirstOrDefault(w => w.ID_HOSPITAL == selectedExcarcelacionDestino.ID_DESTINO) != null)
                            selectedHospitalValue = selectedExcarcelacionDestino.ID_DESTINO.Value;
                        isJuridicaVisible = Visibility.Collapsed;
                        OnPropertyChanged("IsJuridicaVisible");
                        isMedicaVisible = Visibility.Visible;
                        OnPropertyChanged("IsMedicaVisible");
                        isDocumentoSistemaEnabled = false;
                        OnPropertyChanged("IsDocumentoSistemaEnabled");
                        isDocumentoFisicoEnabled = false;
                        OnPropertyChanged("IsDocumentoFisicoEnabled");
                        headerDatos = "DATOS MEDICOS";
                        OnPropertyChanged("HeaderDatos");
                        setValidacionesMedicas();
                        break;
                }
            });
            Folio_Doc = SelectedExcarcelacionDestino.FOLIO;
            IsDocumentoAgregado = true;
            Documento = SelectedExcarcelacionDestino.DOCUMENTO;
            Formato_Documentacion_Excarcelacion = SelectedExcarcelacionDestino.FORMATO_DOCUMENTO;
            Tipo_Documento_Excarcelacion = SelectedExcarcelacionDestino.TIPO_DOCUMENTO;
            Buscar_Causa_Penal = string.Empty;
            IsBuscarCPEnabled = false;
            CP_Excarcelacion_Destino = SelectedExcarcelacionDestino.CAUSA_PENAL;
            if (CP_Excarcelacion_Destino != null && CP_Excarcelacion_Destino.CP_FOLIO.HasValue && CP_Excarcelacion_Destino.CP_ANIO.HasValue)
                CP_Folio_Destino = CP_Excarcelacion_Destino.CP_ANIO.Value + "/" + CP_Excarcelacion_Destino.CP_FOLIO.Value.ToString().PadLeft(5, '0');
            else if (CP_Excarcelacion_Destino == null && !string.IsNullOrWhiteSpace(selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO))
                CP_Folio_Destino = selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO;
            else
                CP_Folio_Destino = string.Empty;

            switch (selectedExc_tipoValue)
            {
                case ((int)enumExcarcelacionTipo.JURIDICA):
                    var _main = PopUpsViewModels.MainWindow;
                    BindUpdatedSources("SourceUpdated", "cambio_pais_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbPaisesDestino);
                    BindUpdatedSources("SourceUpdated", "cambio_estado_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbEstadosDestino);
                    BindUpdatedSources("SourceUpdated", "cambio_municipio_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbMunicipiosDestino);
                    BindUpdatedSources("SourceUpdated", "cambio_fuero_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbFuerosDestino);
                    break;
                case ((int)enumExcarcelacionTipo.MEDICA):
                    break;
            }
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
        }

        private async void AgregarDestinoExcarcelacion()
        {
            foreach (var item in _lista_control_evento)
                Interaction.GetTriggers(item.Control).Remove(item.Evento);
            base.ClearRules();
            var _main = PopUpsViewModels.MainWindow;
            Folio_Doc = string.Empty;
            IsDocumentoAgregado = false;
            Documento = null;
            OtroHospital = string.Empty;
            Formato_Documentacion_Excarcelacion = null;
            Tipo_Documento_Excarcelacion = null;
            CP_Excarcelacion_Destino = null;
            CP_Folio_Destino = string.Empty;
            Buscar_Causa_Penal = string.Empty;
            IsBuscarCPEnabled = true;
            SelectedTipoDocumento = null;
            try
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Visible;
                await Task.Factory.StartNew(() =>
                {
                    switch (selectedExc_tipoValue)
                    {
                        case ((int)enumExcarcelacionTipo.JURIDICA):
                            var _param_pais = Parametro.PAIS;
                            var _param_estado = Parametro.ESTADO;
                            ObtenerFueros(true);
                            VisibleDatosExcarcelacionDestino = Visibility.Visible;///OCULTA CONTROLES DEL LADO DERECHO EN EL CASO MEDICO

                            ObtenerDestinos(selectedExc_tipoValue, true);
                            if (fueros != null)
                            {
                                selectedFueroValue = "0";
                                OnPropertyChanged("SelectedFueroValue");
                            }
                            if (paises != null && paises.FirstOrDefault(w => w.ID_PAIS_NAC == _param_pais) != null)
                            {
                                selectedPaisValue = Parametro.PAIS;
                                OnPropertyChanged("SelectedPaisValue");
                            }
                            if (estados != null && estados.FirstOrDefault(w => w.ID_ENTIDAD == _param_estado) != null)
                            {
                                selectedEstadoValue = Parametro.ESTADO;
                                OnPropertyChanged("SelectedEstadoValue");
                            }
                            if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                            {
                                selectedMunicipioValue = 0;
                                OnPropertyChanged("SelectedMunicipioValue");
                            }
                            if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                            {
                                selectedJuzgadoValue = 0;
                                OnPropertyChanged("SelectedJuzgadoValue");
                            }
                            isJuridicaVisible = Visibility.Visible;
                            OnPropertyChanged("IsJuridicaVisible");
                            isMedicaVisible = Visibility.Collapsed;
                            OnPropertyChanged("IsMedicaVisible");
                            if (selectIngreso != null)
                            {
                                isDocumentoSistemaEnabled = true;
                                OnPropertyChanged("IsDocumentoSistemaEnabled");
                            }
                            else
                            {
                                isDocumentoSistemaEnabled = false;
                                OnPropertyChanged("IsDocumentoSistemaEnabled");
                            }
                            headerDatos = "DATOS JURIDICOS";
                            OnPropertyChanged("HeaderDatos");
                            isDatosVisible = Visibility.Visible;
                            OnPropertyChanged("IsDatosVisible");
                            setValidacionesJuridicas();
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                BindUpdatedSources("SourceUpdated", "cambio_pais_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbPaisesDestino);
                                BindUpdatedSources("SourceUpdated", "cambio_estado_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbEstadosDestino);
                                BindUpdatedSources("SourceUpdated", "cambio_municipio_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbMunicipiosDestino);
                                BindUpdatedSources("SourceUpdated", "cambio_fuero_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbFuerosDestino);
                            }));

                            break;
                        case ((int)enumExcarcelacionTipo.MEDICA):
                            ObtenerHospital(true);

                            if (hospitales != null && hospitales.FirstOrDefault(w => w.ID_HOSPITAL == 0) != null)
                            {
                                selectedHospitalValue = 0;
                                OnPropertyChanged("SelectedHospitalValue");
                            }
                            isJuridicaVisible = Visibility.Collapsed;
                            OnPropertyChanged("IsJuridicaVisible");
                            isMedicaVisible = Visibility.Visible;
                            OnPropertyChanged("IsMedicaVisible");
                            isDocumentoSistemaEnabled = false;
                            OnPropertyChanged("IsDocumentoSistemaEnabled");
                            headerDatos = "DATOS MÉDICOS";
                            OnPropertyChanged("HeaderDatos");
                            isDatosVisible = Visibility.Visible;
                            OnPropertyChanged("IsDatosVisible");
                            //setValidacionesMedicas();
                            IsDocumentoAgregado = false;

                            VisibleDatosExcarcelacionDestino = Visibility.Hidden;///OCULTA CONTROLES DEL LADO DERECHO EN EL CASO MEDICO

                            IsDocumentoFisicoEnabled = IsDocumentoSistemaEnabled = false;
                            lstInterconsultaMedica = new ObservableCollection<CustomGridInterconexionMedica>();
                            if (SelectIngreso != null)
                                if (SelectIngreso.ATENCION_MEDICA != null && SelectIngreso.ATENCION_MEDICA.Any())
                                    foreach (var item in SelectIngreso.ATENCION_MEDICA.Where(w =>
                                        w.NOTA_MEDICA != null &&
                                        w.NOTA_MEDICA.OCUPA_INTERCONSULTA == "S" &&
                                        w.NOTA_MEDICA.CANALIZACION != null &&
                                        w.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null &&
                                        w.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any() &&
                                        w.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a =>
                                            a.ESTATUS == "S" &&
                                            a.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA &&
                                            a.HOJA_REFERENCIA_MEDICA != null &&
                                            a.HOJA_REFERENCIA_MEDICA.Any() &&
                                            a.HOJA_REFERENCIA_MEDICA.FirstOrDefault().FECHA_CITA.HasValue &&
                                            a.HOJA_REFERENCIA_MEDICA.FirstOrDefault().FECHA_CITA.Value.Date >= FechaServer.Date)))
                                    {
                                        foreach (var item2 in item.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD)
                                        {
                                            if (item2.ESTATUS == "S")
                                                if (item2.ID_INTER == (short)enumInterconsulta_Tipo.EXTERNA)
                                                    if (item2.HOJA_REFERENCIA_MEDICA.Any())
                                                        if (item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().FECHA_CITA.Value.Date >= FechaServer.Date)
                                                            lstInterconsultaMedica.Add(new CustomGridInterconexionMedica
                                                            {
                                                                IdInterconsulta = item2.ID_INTERSOL,
                                                                FolioInterConsulta = !string.IsNullOrEmpty(item2.OFICIO_EXC) ? item2.OFICIO_EXC.Trim() : string.Empty,
                                                                FechaInterconsulta = item2.HOJA_REFERENCIA_MEDICA.Any() ? item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().FECHA_CITA : new DateTime?(),
                                                                IdDestino = item2.HOJA_REFERENCIA_MEDICA.Any() ? item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.Value : new short?() : new short?(),
                                                                NombreMedico = item2.USUARIO != null ?
                                                                    string.Format("{0} {1} {2}",
                                                                    item2.USUARIO.EMPLEADO != null ? item2.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item2.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? item2.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                                    item2.USUARIO.EMPLEADO != null ? item2.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item2.USUARIO.EMPLEADO.PERSONA.PATERNO) ? item2.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                                    item2.USUARIO.EMPLEADO != null ? item2.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(item2.USUARIO.EMPLEADO.PERSONA.MATERNO) ? item2.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty,
                                                                NombreDestino = item2.HOJA_REFERENCIA_MEDICA != null ? item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL != null ? !string.IsNullOrEmpty(item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? item2.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                                                NombrePrioridad = item2.INTERCONSULTA_NIVEL_PRIORIDAD != null ? !string.IsNullOrEmpty(item2.INTERCONSULTA_NIVEL_PRIORIDAD.DESCR) ? item2.INTERCONSULTA_NIVEL_PRIORIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                                                NombreTipoAtencion = item2.INTERCONSULTA_ATENCION_TIPO != null ? !string.IsNullOrEmpty(item2.INTERCONSULTA_ATENCION_TIPO.DESCR) ? item2.INTERCONSULTA_ATENCION_TIPO.DESCR.Trim() : string.Empty : string.Empty
                                                            });
                                        };
                                    };

                            RaisePropertyChanged("LstInterconsultaMedica");
                            break;
                        default:
                            isJuridicaVisible = Visibility.Collapsed;
                            OnPropertyChanged("IsJuridicaVisible");
                            isMedicaVisible = Visibility.Collapsed;
                            OnPropertyChanged("IsMedicaVisible");
                            isDatosVisible = Visibility.Collapsed;
                            OnPropertyChanged("IsDatosVisible");
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
            finally
            {
                StaticSourcesViewModel.ShowLoading = Visibility.Hidden;
            }
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);

        }

        /// <summary>
        /// metodo usado para bindear eventos a un objeto a traves de la libreria Interactivity
        /// </summary>
        /// <param name="event_name" tipo="string">Nombre del evento a escuchar</param>
        /// <param name="command_parameter" tipo="string">Parametro a enviar</param>
        /// <param name="command" tipo="string">Nombre del comando bindeado al evento</param>
        /// <param name="control" tipo="DependencyObject">Control a bindear</param>
        /// <returns></returns>

        private void BindUpdatedSources(string event_name, object command_parameter, string command, DependencyObject control)
        {
            // create the command action and bind the command to it
            var invokeCommandAction = new InvokeCommandAction { CommandParameter = command_parameter };
            BindingOperations.SetBinding(invokeCommandAction, InvokeCommandAction.CommandProperty, new Binding { Path = new PropertyPath(command) });

            // create the event trigger and add the command action to it
            var eventTrigger = new System.Windows.Interactivity.EventTrigger { EventName = event_name };
            eventTrigger.Actions.Add(invokeCommandAction);

            // attach the trigger to the control
            Interaction.GetTriggers(control).Add(eventTrigger);
            _lista_control_evento.Add(new ControlEvento
            {
                Control = control,
                Evento = eventTrigger
            });
        }

        private async void SeleccionarImputadoNUC()
        {
            var _tiene_traslado = false;

            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando Informacion", () =>
            {
                //AQUI SE MANTIENE EL VIEJO CRITERIO DE CANDADO DE TRASLADO. ES RESPONSABILIDAD DEL JURIDICO, 01,02 DETERMINAR QUE TIENE PRIORIDAD. LA EXCARCELACION O TRASLADO.
                //SI SE OCUPA HACER UNA EXCARCELACION CUANDO HAY UN TRASLADO ACTIVO, HAY QUE CANCELAR EL TRASLADO PORQUE SE DESCONOCE EL TIEMPO DE LA EXCARCELACION.
                if (SelectedIngresoNUC.INGRESO.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                {
                    _tiene_traslado = true;
                    return false;
                }
                CP_Excarcelacion_Destino = selectedIngresoNUC.CAUSA_PENAL;
                selectIngresoAuxiliar = SelectIngreso = SelectedIngresoNUC.INGRESO;
                SelectExpediente = SelectedIngresoNUC.INGRESO.IMPUTADO;
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
                SelectedExc_TipoValue = (short)enumExcarcelacionTipo.JURIDICA;
                IsExcarcelacion_TiposEnabled = false;
                IsDocumentoAgregado = true;
                IsDocumentoSistemaEnabled = false;
                IsDocumentoFisicoEnabled = false;


                if (CP_Excarcelacion_Destino == null || !CP_Excarcelacion_Destino.CP_FOLIO.HasValue || !CP_Excarcelacion_Destino.CP_ANIO.HasValue)
                {
                    var _param_pais = Parametro.PAIS;
                    var _param_estado = Parametro.ESTADO;
                    ObtenerFueros(true);
                    ObtenerDestinos(selectedExc_tipoValue, true);
                    if (fueros != null)
                    {
                        selectedFueroValue = "0";
                        OnPropertyChanged("SelectedFueroValue");
                    }
                    if (paises != null && paises.FirstOrDefault(w => w.ID_PAIS_NAC == _param_pais) != null)
                    {
                        selectedPaisValue = Parametro.PAIS;
                        OnPropertyChanged("SelectedPaisValue");
                    }
                    if (estados != null && estados.FirstOrDefault(w => w.ID_ENTIDAD == _param_estado) != null)
                    {
                        selectedEstadoValue = Parametro.ESTADO;
                        OnPropertyChanged("SelectedEstadoValue");
                    }
                    if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                    {
                        selectedMunicipioValue = 0;
                        OnPropertyChanged("SelectedMunicipioValue");
                    }
                    if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                    {
                        selectedJuzgadoValue = 0;
                        OnPropertyChanged("SelectedJuzgadoValue");
                    }
                }
                else
                {
                    cp_folio_destino = cp_excarcelacion_destino.CP_ANIO.Value.ToString() + "/" + cp_excarcelacion_destino.CP_FOLIO.ToString().PadLeft(5, '0');
                    OnPropertyChanged("CP_Folio_Destino");
                    ObtenerFueros(true);
                    ObtenerPaises(true);
                    ObtenerEstados(cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value, true);
                    ObtenerMunicipios(cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value, true);
                    ObtenerJuzgados(cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value, cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value,
                        cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO.Value, cp_excarcelacion_destino.JUZGADO.ID_FUERO, true);
                    if (fueros != null)
                    {
                        selectedFueroValue = cp_excarcelacion_destino.JUZGADO.ID_FUERO;
                        OnPropertyChanged("SelectedFueroValue");
                    }

                    if (paises != null && paises.FirstOrDefault(w => w.ID_PAIS_NAC == cp_excarcelacion_destino.JUZGADO.ID_PAIS) != null)
                    {
                        selectedPaisValue = cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value;
                        OnPropertyChanged("SelectedPaisValue");
                    }
                    if (estados != null && estados.FirstOrDefault(w => w.ID_ENTIDAD == cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD) != null)
                    {
                        selectedEstadoValue = cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value;
                        OnPropertyChanged("SelectedEstadoValue");
                    }
                    if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO) != null)
                    {
                        selectedMunicipioValue = cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO.Value;
                        OnPropertyChanged("SelectedMunicipioValue");
                    }
                    if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == cp_excarcelacion_destino.JUZGADO.ID_JUZGADO) != null)
                    {
                        selectedJuzgadoValue = cp_excarcelacion_destino.JUZGADO.ID_JUZGADO;
                        OnPropertyChanged("SelectedJuzgadoValue");
                    }
                    isBuscarCPEnabled = false;
                    OnPropertyChanged("IsBuscarCPEnabled");
                }

                headerDatos = "DATOS JURIDICOS";
                OnPropertyChanged("HeaderDatos");
                setValidacionesJuridicas();

                AnioBuscarHabilitado = false;
                FolioBuscarHabilitado = false;
                NombreBuscarHabilitado = false;
                ApellidoPaternoBuscarHabilitado = false;
                ApellidoMaternoBuscarHabilitado = false;
                BuscarImputadoHabilitado = false;

                isDatosVisible = Visibility.Visible;
                OnPropertyChanged("IsDatosVisible");
                isJuridicaVisible = Visibility.Visible;
                OnPropertyChanged("IsJuridicaVisible");
                return true;
            }))
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_IMPUTADO_NUC);
                var _main = PopUpsViewModels.MainWindow;
                BindUpdatedSources("SourceUpdated", "cambio_pais_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbPaisesDestino);
                BindUpdatedSources("SourceUpdated", "cambio_estado_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbEstadosDestino);
                BindUpdatedSources("SourceUpdated", "cambio_municipio_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbMunicipiosDestino);
                BindUpdatedSources("SourceUpdated", "cambio_fuero_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbFuerosDestino);

                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DESTINO_EXCARCELACION);
            }
            else
                if (_tiene_traslado)
                    new Dialogos().ConfirmacionDialogo("Notificacion!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                        SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
        }

        private async void SeleccionarExcarcelacion()
        {
            if (StaticSourcesViewModel.SourceChanged)
            {
                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                    "Existen cambios sin guardar,¿desea seleccionar una excarcelación para editar?") != 1)
                    return;
            }
            if (SelectedExcarcelacionBusqueda == null)
            {
                new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar una excarcelación.");
                return;
            }
            Limpiar();
            SelectedExcarcelacion = SelectedExcarcelacionBusqueda.SELECTED_EXCARCELACION;
            setValidacionesExcarcelacion();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_EXCARCELACIONES);
            var _hospital_otros = Parametro.ID_HOSPITAL_OTROS;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                selectedExc_tipoValue = selectedExcarcelacion.ID_TIPO_EX.Value;
                OnPropertyChanged("SelectedExc_TipoValue");
                excarcelacion_fecha = selectedExcarcelacion.PROGRAMADO_FEC.Value;
                OnPropertyChanged("Excarcelacion_Fecha");
                _fecha_programada_Old = selectedExcarcelacion.PROGRAMADO_FEC.Value;
                observaciones = selectedExcarcelacion.OBSERVACION;
                OnPropertyChanged("Observaciones");

                OnPropertyChanged("Folio_Doc");

                isDocumentoAgregado = true;
                OnPropertyChanged("IsDocumentoAgregado");
                IsDatosVisible = Visibility.Visible;
                listaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(selectedExcarcelacion.EXCARCELACION_DESTINO.Select(
                    s => new CT_EXCARCELACION_DESTINO
                    {
                        ID_CONSEC = s.ID_DESTINO,
                        CAUSA_PENAL = s.CAUSA_PENAL,
                        CAUSA_PENAL_TEXTO = s.CAUSA_PENAL_TEXTO,
                        DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ?
                        s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL == _hospital_otros ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL_OTRO : !string.IsNullOrEmpty(s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        DOCUMENTO = s.DOCUMENTO,
                        ESTATUS = s.ID_ESTATUS,
                        FOLIO = s.FOLIO_DOC,
                        ID_INTERC = s.ID_INTERSOL,
                        FORMATO_DOCUMENTO = s.ID_FORMATO,
                        ID_DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (short?)s.ID_JUZGADO : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? (short?)s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL : null : null : null,
                        TIPO_DOCUMENTO = s.ID_TIPO_DOC,
                        CANCELACION_OBSERVACION = s.CANCELADO_OBSERVA,
                        ID_CANCELACION_MOTIVO = s.CAN_ID_MOTIVO
                    }
                    ));
                OnPropertyChanged("ListaExcarcelacionDestinos");
                if (selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA)
                {
                    isCertificadoEnabled = false;
                    OnPropertyChanged("IsCertificadoEnabled");
                    certMedicoSiChecked = false;
                    OnPropertyChanged("CertMedicoSiChecked");
                    certMedicoNoChecked = false;
                    OnPropertyChanged("CertMedicoNoChecked");
                }
                else
                {
                    if (listaExcarcelacionDestinos.Where(w => w.CAUSA_PENAL != null).Any(a => a.CAUSA_PENAL.NUC != null) || Parametro.CERT_MEDICO_OBLIGATORIO == 1)
                    {
                        isCertificadoEnabled = false;
                        OnPropertyChanged("IsCertificadoEnabled");
                        certMedicoSiChecked = true;
                        OnPropertyChanged("CertMedicoSiChecked");
                        certMedicoNoChecked = false;
                        OnPropertyChanged("CertMedicoNoChecked");
                    }
                    else
                    {
                        isCertificadoEnabled = true;
                        OnPropertyChanged("IsCertificadoEnabled");
                        if (selectedExcarcelacion.CERTIFICADO_MEDICO == 1)
                        {
                            certMedicoSiChecked = true;
                            OnPropertyChanged("CertMedicoSiChecked");
                            certMedicoNoChecked = false;
                            OnPropertyChanged("CertMedicoNoChecked");
                        }
                        else
                        {
                            certMedicoSiChecked = false;
                            OnPropertyChanged("CertMedicoSiChecked");
                            certMedicoNoChecked = true;
                            OnPropertyChanged("CertMedicoNoChecked");
                        }
                    }
                }

                SelectExpediente = SelectedExcarcelacion.INGRESO.IMPUTADO;
                SelectIngreso = SelectedExcarcelacion.INGRESO;
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
                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? SelectIngreso.TIPO_SEGURIDAD.DESCR : string.Empty;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
            });
            AnioBuscarHabilitado = false;
            FolioBuscarHabilitado = false;
            NombreBuscarHabilitado = false;
            ApellidoPaternoBuscarHabilitado = false;
            ApellidoMaternoBuscarHabilitado = false;
            BuscarImputadoHabilitado = false;
            CancelarMenuEnabled = true;
            if (selectedExcarcelacion.ID_ESTATUS != "AU")
            {
                EliminarMenuEnabled = true;
                MenuGuardarEnabled = true;
                IsDatosExcarcelacionEnabled = true;
                IsExcarcelacion_TiposEnabled = true;
            }
            else
            {
                EliminarMenuEnabled = false;
                MenuGuardarEnabled = false;
                IsDatosExcarcelacionEnabled = false;
                IsExcarcelacion_TiposEnabled = false;
                IsCertificadoEnabled = false;
            }

        }

        private async void BuscarSeleccionarImputado()
        {
            var _tipo_error = 0;
            var _exc_prog = false;
            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cargando Informacion", () =>
            {
                if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                {
                    _tipo_error = 1;
                    return false;
                }
                //AQUI SE MANTIENE EL VIEJO CRITERIO DE CANDADO DE TRASLADO. ES RESPONSABILIDAD DEL JURIDICO, 01,02 DETERMINAR QUE TIENE PRIORIDAD. LA EXCARCELACION O TRASLADO.
                //SI SE OCUPA HACER UNA EXCARCELACION CUANDO HAY UN TRASLADO ACTIVO, HAY QUE CANCELAR EL TRASLADO PORQUE SE DESCONOCE EL TIEMPO DE LA EXCARCELACION.                
                if (SelectIngreso.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                {
                    _tipo_error = 2;
                    return false;
                }
                if (SelectIngreso.EXCARCELACION.Any(a => a.ID_ESTATUS == "AC"))
                {
                    _tipo_error = 3;
                    return false;
                }
                if (SelectIngreso.EXCARCELACION.Any(a => a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "CO" && a.ID_ESTATUS != "AC"))
                    _exc_prog = true;
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
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                if (_exc_prog)
                {
                    var mensaje = MensajeExcarcelacionesProgramadas(SelectIngreso.EXCARCELACION.Where(a => a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "CO" && a.ID_ESTATUS != "AC"));
                    new Dialogos().ConfirmacionDialogo("Notificación!", mensaje);
                }
            }
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
                    case 3:
                        new Dialogos().ConfirmacionDialogo("Notificación!", string.Format("El interno [{0}/{1}] tiene una excarcelación activa desde el día", SelectIngreso.ID_ANIO.ToString(),
                            SelectIngreso.ID_IMPUTADO.ToString(),
                            SelectIngreso.EXCARCELACION.First(a => a.ID_ESTATUS == "AC").PROGRAMADO_FEC.Value.ToString("dd/MM/yyyy")));
                        break;
                }
        }

        private async Task Cancelar()
        {
            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando la excarcelación", () =>
            {
                var _excarcelacion_destinos = new List<EXCARCELACION_DESTINO>();
                foreach (var item in selectedExcarcelacion.EXCARCELACION_DESTINO)
                    if (item.ID_ESTATUS != "CA")
                        _excarcelacion_destinos.Add(new EXCARCELACION_DESTINO
                        {
                            CAN_ID_MOTIVO = selectedCancelacion_MotivoValue,
                            CANCELADO_OBSERVA = cancelacion_Observacion,
                            CAU_ID_ANIO = item.CAU_ID_ANIO,
                            ID_INTERSOL = item.ID_INTERSOL,
                            CAU_ID_CAUSA_PENAL = item.CAU_ID_CAUSA_PENAL,
                            CAU_ID_CENTRO = item.CAU_ID_CENTRO,
                            CAU_ID_IMPUTADO = item.CAU_ID_IMPUTADO,
                            CAU_ID_INGRESO = item.CAU_ID_INGRESO,
                            DOCUMENTO = item.DOCUMENTO,
                            FOLIO_DOC = item.FOLIO_DOC,
                            //HOSPITAL_OTRO = item.HOSPITAL_OTRO,
                            ID_ANIO = item.ID_ANIO,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_CENTRO_UBI = GlobalVar.gCentro,
                            ID_CONSEC = item.ID_CONSEC,
                            ID_DESTINO = item.ID_DESTINO,
                            ID_ESTATUS = "CA",
                            ID_FORMATO = item.ID_FORMATO,
                            //ID_HOSPITAL = item.ID_HOSPITAL,
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_INGRESO = item.ID_INGRESO,
                            ID_JUZGADO = item.ID_JUZGADO,
                            ID_TIPO_DOC = item.ID_TIPO_DOC,
                            CANCELADO_TIPO = "J"
                        });
                var _excarcelacion = new EXCARCELACION
                {
                    ID_ANIO = selectedExcarcelacion.ID_ANIO,
                    ID_CENTRO = selectedExcarcelacion.ID_CENTRO,
                    ID_ESTATUS = "CA",
                    ID_IMPUTADO = selectedExcarcelacion.ID_IMPUTADO,
                    ID_INGRESO = selectedExcarcelacion.ID_INGRESO,
                    ID_TIPO_EX = selectedExcarcelacion.ID_TIPO_EX,
                    ID_USUARIO = StaticSourcesViewModel.UsuarioLogin.Username,
                    REGISTRO_FEC = selectedExcarcelacion.REGISTRO_FEC,
                    OBSERVACION = selectedExcarcelacion.OBSERVACION,
                    ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                    CERTIFICADO_MEDICO = selectedExcarcelacion.CERTIFICADO_MEDICO,
                    PROGRAMADO_FEC = selectedExcarcelacion.PROGRAMADO_FEC,
                    EXCARCELACION_DESTINO = _excarcelacion_destinos,
                    CANCELADO_TIPO = "J"
                };

                new cExcarcelacion().Actualizar(_excarcelacion, _excarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (int)enumMensajeTipo.MODIFICACION_EXCARCELACION : (int)enumMensajeTipo.MODIFICACION_EXCARCELACION_MEDICA,
                    _excarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (int)enumMensajeTipo.CANCELACION_EXCARCELACION : (int)enumMensajeTipo.CANCELACION_EXCARCELACION_MEDICA,
                    (int)enumMensajeTipo.AVISO_CANCELACION_EXCARCELACION_AREA_MEDICA, _FechaServer, Parametro.ID_HOSPITAL_OTROS, false, GlobalVar.gCentro);
                return true;
            }))
            {
                new Dialogos().ConfirmacionDialogo("EXITO!", "La excarcelación ha sido cancelada");
                Limpiar();
                AnioBuscarHabilitado = true;
                FolioBuscarHabilitado = true;
                NombreBuscarHabilitado = true;
                ApellidoPaternoBuscarHabilitado = true;
                ApellidoMaternoBuscarHabilitado = true;
                BuscarImputadoHabilitado = true;
                EliminarMenuEnabled = false;
                CancelarMenuEnabled = false;
                //VerDocumentoEnabled = false;
            }
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
                if (AnioBuscar.HasValue && FolioBuscar.HasValue && ListExpediente.Count() == 1)
                {
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    if (ListExpediente[0].INGRESO == null || ListExpediente[0].INGRESO.Count() == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso para este imputado.");
                        resetIngresoAnterior();
                        return;
                    }
                    if (estatus_inactivos.Contains(ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        resetIngresoAnterior();
                        return;
                    }
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        resetIngresoAnterior();
                        return;
                    }
                    //AQUI SE MANTIENE EL VIEJO CRITERIO DE CANDADO DE TRASLADO. ES RESPONSABILIDAD DEL JURIDICO, 01,02 DETERMINAR QUE TIENE PRIORIDAD. LA EXCARCELACION O TRASLADO.
                    //SI SE OCUPA HACER UNA EXCARCELACION CUANDO HAY UN TRASLADO ACTIVO, HAY QUE CANCELAR EL TRASLADO PORQUE SE DESCONOCE EL TIEMPO DE LA EXCARCELACION.
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                            ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        return;
                    }
                    //Cuando hay una excarcelación activa
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().EXCARCELACION.Any(a => a.ID_ESTATUS == "AC"))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", string.Format("El interno [{0}/{1}] tiene una excarcelación activa desde el día", ListExpediente[0].ID_ANIO.ToString(),
                            ListExpediente[0].ID_IMPUTADO.ToString(),
                            ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().EXCARCELACION.First(a => a.ID_ESTATUS == "AC").PROGRAMADO_FEC.Value.ToString("dd/MM/yyyy")));
                        return;
                    }
                    //Cuando hay una excarcelación programada/autorizada
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().EXCARCELACION.Any(a => a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "CO" && a.ID_ESTATUS != "AC"))
                    {
                        var mensaje = MensajeExcarcelacionesProgramadas(ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().EXCARCELACION.Where(a => a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "CO" && a.ID_ESTATUS != "AC"));
                        new Dialogos().ConfirmacionDialogo("Notificación!", mensaje);
                    }
                    selectIngresoAuxiliar = SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar el imputado", ex);
            }

        }

        private string MensajeExcarcelacionesProgramadas(IEnumerable<EXCARCELACION> _excarcelaciones)
        {
            var dias = string.Empty;
            var texto_varios = "una";
            var texto_dias = "el día";
            if (_excarcelaciones.Count() > 1)
            {
                texto_dias = "los dias";
                texto_varios = "varias";
                foreach (var item in _excarcelaciones)
                {
                    dias += item.PROGRAMADO_FEC.Value.ToString("dd/MM/yyyy") + ", ";
                }
                dias = dias.Remove(dias.Length - 2);
            }
            else
            {
                dias = _excarcelaciones.First().PROGRAMADO_FEC.Value.ToString("dd/MM/yyyy");
            }
            return string.Format("El interno [{0}/{1}] tiene {2} excarcelación programada/autorizada para {3} {4}", ListExpediente[0].ID_ANIO.ToString(),
                ListExpediente[0].ID_IMPUTADO.ToString(), texto_varios, texto_dias, dias);
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

        public void Limpiar()
        {
            #region Preseleccionar Listas
            if (excarcelacion_tipos != null && excarcelacion_tipos.FirstOrDefault(w => w.ID_TIPO_EX == 0) != null)
            {
                selectedExc_tipoValue = excarcelacion_tipos.First(w => w.ID_TIPO_EX == 0).ID_TIPO_EX;
                OnPropertyChanged("SelectedExc_TipoValue");
            }
            #endregion
            SelectedExcarcelacion = null;
            SelectExpediente = null;
            SelectIngreso = null;
            AnioD = null;
            FolioD = null;
            NombreD = string.Empty;
            PaternoD = string.Empty;
            MaternoD = string.Empty;
            IngresosD = null;
            UbicacionD = string.Empty;
            TipoSeguridadD = string.Empty;
            FecIngresoD = null;
            ClasificacionJuridicaD = null;
            EstatusD = null;
            ImagenIngreso = null;
            Excarcelacion_Fecha = null;
            Observaciones = string.Empty;
            IsDatosVisible = Visibility.Collapsed;
            IsJuridicaVisible = Visibility.Collapsed;
            IsMedicaVisible = Visibility.Collapsed;
            AnioBuscarHabilitado = true;
            FolioBuscarHabilitado = true;
            NombreBuscarHabilitado = true;
            ApellidoPaternoBuscarHabilitado = true;
            ApellidoMaternoBuscarHabilitado = true;
            BuscarImputadoHabilitado = true;
            IsDocumentoFisicoEnabled = true;
            IsDocumentoSistemaEnabled = true;
            IsExcarcelacion_TiposEnabled = true;
            ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>();
            IsCertificadoEnabled = false;
            CertMedicoNoChecked = false;
            CertMedicoSiChecked = false;
            StaticSourcesViewModel.SourceChanged = false;
        }

        public void LimpiarBuscar()
        {
            #region Preseleccionar Listas
            if (excarcelacion_TiposBuscar != null && excarcelacion_TiposBuscar.FirstOrDefault(w => w.ID_TIPO_EX == 0) != null)
            {
                selectedExc_TipoBuscarValue = excarcelacion_TiposBuscar.First(w => w.ID_TIPO_EX == 0).ID_TIPO_EX;
                OnPropertyChanged("SelectedExc_TipoBuscarValue");
            }
            #endregion
            AnioBuscarExc = null;
            FolioBuscarExc = null;
            NombreBuscarExc = string.Empty;
            ApellidoPaternoBuscarExc = string.Empty;
            ApellidoMaternoBuscarExc = string.Empty;
            FechaInicialBuscarExc = null;
            FechaFinalBuscarExc = null;
            ListaExcarcelacionesBusqueda = null;
        }

        /// <summary>
        /// metodo usado para rutear la escucha de eventos causado por cambio de valores en el modelo
        /// </summary>
        /// <param name="parametro" tipo="object">parametro enviado por el comando</param>
        /// <returns></returns>
        private async void ModelChangedSwitch(object parametro)
        {
            try
            {
                if (parametro != null)
                    switch (parametro.ToString())
                    {
                        case "cambio_excarcelacion_tipo":
                            if (ListaExcarcelacionDestinos != null && ListaExcarcelacionDestinos.Count > 0)
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                        "Los destinos ligados a este tipo de excarcelación se perderan si cambia de tipo de excarcelación ¿desea continuar con el cambio de tipo excarcelación?") != 1)
                                {
                                    selectedExc_tipoValue = selectedExc_tipoOldValue;
                                    OnPropertyChanged("SelectedExc_TipoValue");
                                    return;
                                }
                            ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>();
                            if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA)
                            {
                                CertMedicoNoChecked = false;
                                CertMedicoSiChecked = false;
                                IsCertificadoEnabled = false;
                            }
                            break;
                        case "cambio_pais_juridico":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnPaisChanged(selectedPaisValue, selectedFueroValue, true);
                                if (estados != null)
                                {
                                    if (selectedPaisValue == Parametro.PAIS && estados.FirstOrDefault(w => w.ID_ENTIDAD == Parametro.ESTADO) != null)
                                        selectedEstadoValue = Parametro.ESTADO;
                                    else if (estados.FirstOrDefault(w => w.ID_ENTIDAD == 0) != null)
                                        selectedEstadoValue = 0;
                                    OnPropertyChanged("SelectedEstadoValue");
                                }
                                if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                                {
                                    selectedMunicipioValue = 0;
                                    OnPropertyChanged("SelectedMunicipioValue");
                                }
                                if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoValue");
                                }
                            });
                            break;
                        case "cambio_estado_juridico":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnEstadoChanged(selectedPaisValue, selectedEstadoValue, selectedFueroValue, true);
                                if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                                {
                                    selectedMunicipioValue = 0;
                                    OnPropertyChanged("SelectedMunicipioValue");
                                }
                                if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoValue");
                                }
                            });
                            break;
                        case "cambio_municipio_juridico":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnMunicipioChanged(selectedPaisValue, selectedEstadoValue, selectedMunicipioValue, selectedFueroValue, true);
                                if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoValue");
                                }
                            });
                            break;
                        case "cambio_fuero_juridico":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnFueroChanged(selectedPaisValue, selectedEstadoValue, selectedMunicipioValue, selectedFueroValue, true);
                                if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoValue");
                                }
                            });
                            break;
                        case "cambio_hospital_medico":
                            if (selectedHospitalValue == Parametro.ID_HOSPITAL_OTROS)
                            {
                                OtroHospital = string.Empty;
                                IsOtroHospitalVisible = Visibility.Visible;
                                base.AddRule(() => OtroHospital, () => !string.IsNullOrWhiteSpace(OtroHospital), "OTRO HOSPITAL ES REQUERIDO!");
                                OnPropertyChanged("OtroHospital");
                            }
                            else
                            {
                                base.RemoveRule("OtroHospital");
                                OnPropertyChanged("OtroHospital");
                                IsOtroHospitalVisible = Visibility.Hidden;
                            }
                            break;
                        case "cambio_fecha_inicio_documentos":
                            if (fechaInicioMensaje.HasValue && fechaFinalMensaje.HasValue)
                                if (fechaFinalMensaje.Value < fechaInicioMensaje.Value)
                                {
                                    isFechaIniValida = false;
                                    OnPropertyChanged("IsFechaIniValida");
                                    isBuscarDocumentoEnabled = false;
                                    OnPropertyChanged("IsBuscarDocumentoEnabled");
                                }
                                else
                                {
                                    isFechaIniValida = true;
                                    OnPropertyChanged("IsFechaIniValida");
                                    isBuscarDocumentoEnabled = true;
                                    OnPropertyChanged("IsBuscarDocumentoEnabled");
                                }
                            else
                            {
                                isFechaIniValida = true;
                                OnPropertyChanged("IsFechaIniValida");
                                isBuscarDocumentoEnabled = true;
                                OnPropertyChanged("IsBuscarDocumentoEnabled");
                            }
                            break;
                        case "cambio_selected_documento":
                            if (SelectedDocumento != null)
                            {
                                isSeleccionarDocumentoEnabled = true;
                            }
                            else
                            {
                                isSeleccionarDocumentoEnabled = false;
                            }
                            OnPropertyChanged("IsSeleccionarDocumentoEnabled");
                            break;
                        case "cambio_excarcelacion_tipo_buscarexc":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                ObtenerFuerosBuscarExc(true);
                                ObtenerDestinosBuscarExc(selectedExc_TipoBuscarValue, true);
                                switch (selectedExc_TipoBuscarValue)
                                {
                                    case ((int)enumExcarcelacionTipo.JURIDICA):
                                        if (fuerosBuscarExc != null)
                                        {
                                            selectedFueroBuscarExcValue = "0";
                                            OnPropertyChanged("SelectedFueroBuscarExcValue");
                                        }
                                        if (paisesBuscarExc != null && paisesBuscarExc.FirstOrDefault(w => w.ID_PAIS_NAC == Parametro.PAIS) != null)
                                        {
                                            selectedPaisBuscarExcValue = Parametro.PAIS;
                                            OnPropertyChanged("SelectedPaisBuscarExcValue");
                                        }
                                        if (estadosBuscarExc != null && estadosBuscarExc.FirstOrDefault(w => w.ID_ENTIDAD == Parametro.ESTADO) != null)
                                        {
                                            selectedEstadoBuscarExcValue = Parametro.ESTADO;
                                            OnPropertyChanged("SelectedEstadoBuscarExcValue");
                                        }
                                        if (municipiosBuscarExc != null && municipiosBuscarExc.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                                        {
                                            selectedMunicipioBuscarExcValue = 0;
                                            OnPropertyChanged("SelectedMunicipioBuscarExcValue");
                                        }
                                        if (juzgadosBuscarExc != null && juzgadosBuscarExc.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                        {
                                            selectedJuzgadoBuscarExcValue = 0;
                                            OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
                                        }
                                        IsJuridicaVisibleBuscarExc = Visibility.Visible;
                                        IsMedicaVisibleBuscarExc = Visibility.Collapsed;
                                        headerDatosBuscarExc = "DATOS JURIDICOS";
                                        OnPropertyChanged("HeaderDatosBuscarExc");
                                        IsDatosVisibleBuscarExc = Visibility.Visible;
                                        break;
                                    case ((int)enumExcarcelacionTipo.MEDICA):
                                        ObtenerHospitalBuscarExc(true);
                                        if (hospitalesBuscarExc != null && hospitalesBuscarExc.FirstOrDefault(w => w.ID_HOSPITAL == 0) != null)
                                        {
                                            selectedHospitalBuscarExcValue = 0;
                                            OnPropertyChanged("SelectedHospitalBuscarExcValue");
                                        }
                                        IsJuridicaVisibleBuscarExc = Visibility.Collapsed;
                                        IsMedicaVisibleBuscarExc = Visibility.Visible;
                                        headerDatosBuscarExc = "DATOS MÉDICOS";
                                        OnPropertyChanged("HeaderDatosBuscarExc");
                                        IsDatosVisibleBuscarExc = Visibility.Visible;
                                        break;
                                    default:
                                        IsJuridicaVisibleBuscarExc = Visibility.Collapsed;
                                        IsMedicaVisibleBuscarExc = Visibility.Collapsed;
                                        IsDatosVisibleBuscarExc = Visibility.Collapsed;
                                        break;
                                }
                            });
                            break;
                        case "cambio_pais_juridico_buscarexc":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnPaisBuscarExcChanged(selectedPaisBuscarExcValue, selectedFueroBuscarExcValue, true);
                                if (estadosBuscarExc != null)
                                {
                                    if (selectedPaisBuscarExcValue == Parametro.PAIS && estadosBuscarExc.FirstOrDefault(w => w.ID_ENTIDAD == Parametro.ESTADO) != null)
                                        selectedEstadoBuscarExcValue = Parametro.ESTADO;
                                    else if (estadosBuscarExc.FirstOrDefault(w => w.ID_ENTIDAD == 0) != null)
                                        selectedEstadoBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedEstadoBuscarExcValue");
                                }
                                if (municipiosBuscarExc != null && municipiosBuscarExc.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                                {
                                    selectedMunicipioBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedMunicipioBuscarExcValue");
                                }
                                if (juzgadosBuscarExc != null && juzgadosBuscarExc.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
                                }
                            });
                            break;
                        case "cambio_estado_juridico_buscarexc":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnEstadoBuscarExcChanged(selectedPaisBuscarExcValue, selectedEstadoBuscarExcValue, selectedFueroBuscarExcValue, true);
                                if (municipiosBuscarExc != null && municipiosBuscarExc.FirstOrDefault(w => w.ID_MUNICIPIO == 0) != null)
                                {
                                    selectedMunicipioBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedMunicipioBuscarExcValue");
                                }
                                if (juzgadosBuscarExc != null && juzgadosBuscarExc.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
                                }
                            });
                            break;
                        case "cambio_municipio_juridico_buscarexc":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnMunicipioBuscarExcChanged(selectedPaisBuscarExcValue, selectedEstadoBuscarExcValue, selectedMunicipioBuscarExcValue, selectedFueroBuscarExcValue, true);
                                if (juzgadosBuscarExc != null && juzgadosBuscarExc.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
                                }
                            });
                            break;
                        case "cambio_fuero_juridico_buscarexc":
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                OnFueroBuscarExcChanged(selectedPaisBuscarExcValue, selectedEstadoBuscarExcValue, selectedMunicipioBuscarExcValue, selectedFueroBuscarExcValue, true);
                                if (juzgadosBuscarExc != null && juzgadosBuscarExc.FirstOrDefault(w => w.ID_JUZGADO == 0) != null)
                                {
                                    selectedJuzgadoBuscarExcValue = 0;
                                    OnPropertyChanged("SelectedJuzgadoBuscarExcValue");
                                }
                            });
                            break;
                        case "cambio_hospital_medico_buscarexc":
                            if (selectedHospitalBuscarExcValue == Parametro.ID_HOSPITAL_OTROS)
                            {
                                OtroHospitalBuscarExc = string.Empty;
                                IsOtroHospitalVisibleBuscarExc = Visibility.Visible;
                            }
                            else
                            {
                                IsOtroHospitalVisibleBuscarExc = Visibility.Hidden;
                            }
                            break;
                        case "cambio_ingreso_nuc":
                            if (selectedIngresoNUC.INGRESO.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                            {
                                ImagenIngresoNUC = selectedIngresoNUC.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                            }
                            else
                                ImagenIngresoNUC = new Imagenes().getImagenPerson();
                            IsFotoVisibleNUC = Visibility.Visible;
                            break;
                        case "cambio_fecha_inicio_busqueda":
                            if (fechaInicialBuscarExc.HasValue && fechaFinalBuscarExc.HasValue)
                                if (fechaFinalBuscarExc.Value < fechaInicialBuscarExc.Value)
                                    IsFechaIniBusquedaValida = false;
                                else
                                    IsFechaIniBusquedaValida = true;
                            else
                                IsFechaIniBusquedaValida = true;
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
            }
        }

        private async Task Guardar()
        {
            if (SelectedExcarcelacion == null)
            {
                if (ListaExcarcelacionDestinos.Any(w => w.ESTATUS != "CA"))
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Insertando la excarcelación", () =>
                    {
                        var _excarcelacion_destinos = new List<EXCARCELACION_DESTINO>();
                        foreach (var item in ListaExcarcelacionDestinos)
                            if (item.ESTATUS != "CA")
                                _excarcelacion_destinos.Add(new EXCARCELACION_DESTINO
                                {
                                    DOCUMENTO = item.DOCUMENTO,
                                    FOLIO_DOC = item.FOLIO,
                                    ID_INTERSOL = item.ID_INTERC,
                                    //HOSPITAL_OTRO = selectedExc_tipoValue == (short)enumExcarcelacionTipo.MEDICA && item.ID_DESTINO == (short)Parametro.ID_HOSPITAL_OTROS ? item.DESTINO : string.Empty,
                                    ID_ANIO = selectIngreso.ID_ANIO,
                                    ID_CENTRO = selectIngreso.ID_CENTRO,
                                    ID_ESTATUS = item.ESTATUS,
                                    ID_CENTRO_UBI = GlobalVar.gCentro,
                                    ID_FORMATO = item.FORMATO_DOCUMENTO,
                                    //ID_HOSPITAL = selectedExc_tipoValue == (short)enumExcarcelacionTipo.MEDICA ? item.ID_DESTINO : null,
                                    ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = selectIngreso.ID_INGRESO,
                                    ID_JUZGADO = selectedExc_tipoValue == (short)enumExcarcelacionTipo.JURIDICA ? item.ID_DESTINO : null,
                                    ID_TIPO_DOC = item.TIPO_DOCUMENTO,
                                    CAU_ID_ANIO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_ANIO : null,
                                    CAU_ID_CENTRO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_CENTRO : null,
                                    CAU_ID_IMPUTADO = item.CAUSA_PENAL != null ? (int?)item.CAUSA_PENAL.ID_IMPUTADO : null,
                                    CAU_ID_INGRESO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_INGRESO : null,
                                    CAU_ID_CAUSA_PENAL = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_CAUSA_PENAL : null,
                                    CAN_ID_MOTIVO = item.ID_CANCELACION_MOTIVO,
                                    CANCELADO_OBSERVA = item.ID_CANCELACION_MOTIVO.HasValue ? item.CANCELACION_OBSERVACION : string.Empty,
                                    CAUSA_PENAL_TEXTO = item.CAUSA_PENAL == null && !string.IsNullOrWhiteSpace(item.CAUSA_PENAL_TEXTO) ? item.CAUSA_PENAL_TEXTO : string.Empty
                                });
                        var _excarcelacion = new EXCARCELACION
                        {
                            ID_ANIO = selectIngreso.ID_ANIO,
                            ID_CENTRO = selectIngreso.ID_CENTRO,
                            ID_ESTATUS = "PR",
                            ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                            ID_INGRESO = selectIngreso.ID_INGRESO,
                            ID_TIPO_EX = selectedExc_tipoValue,
                            ID_USUARIO = StaticSourcesViewModel.UsuarioLogin.Username,
                            PROGRAMADO_FEC = Excarcelacion_Fecha,
                            REGISTRO_FEC = _FechaServer,
                            OBSERVACION = Observaciones,
                            EXCARCELACION_DESTINO = _excarcelacion_destinos,
                            CERTIFICADO_MEDICO = certMedicoSiChecked ? 1 : 0
                        };
                        new cExcarcelacion().Insertar(_excarcelacion, (int)enumMensajeTipo.CALENDARIZACION_EXCARCELACION, selectedExc_tipoValue == (short)enumExcarcelacionTipo.JURIDICA &&
                            CertMedicoSiChecked ? (int?)enumMensajeTipo.AVISO_EXCARCELACION_AREA_MEDICA : null, _FechaServer, Parametro.ID_HOSPITAL_OTROS, GlobalVar.gCentro);
                        return true;
                    }))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!", "La excarcelación ha sido registrada");
                        Limpiar();
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "POR LO MENOS UN DESTINO TIENE QUE ESTAR ACTIVO!");
                }
            }
            else
            {
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando la excarcelación", () =>
                {
                    var _excarcelacion_destinos = new List<EXCARCELACION_DESTINO>();
                    foreach (var item in ListaExcarcelacionDestinos)
                        if (item.ID_CONSEC.HasValue || item.ESTATUS != "CA")
                        {
                            _excarcelacion_destinos.Add(new EXCARCELACION_DESTINO
                            {
                                DOCUMENTO = item.DOCUMENTO,
                                FOLIO_DOC = item.FOLIO,
                                ID_INTERSOL = item.ID_INTERC,
                                //HOSPITAL_OTRO = selectedExc_tipoValue == (short)enumExcarcelacionTipo.MEDICA && item.ID_DESTINO == (short)Parametro.ID_HOSPITAL_OTROS ? item.DESTINO : string.Empty,
                                ID_ANIO = selectIngreso.ID_ANIO,
                                ID_CENTRO = selectIngreso.ID_CENTRO,
                                ID_ESTATUS = item.ESTATUS,
                                ID_FORMATO = item.FORMATO_DOCUMENTO,
                                ID_CENTRO_UBI = GlobalVar.gCentro,
                                //ID_HOSPITAL = selectedExc_tipoValue == (short)enumExcarcelacionTipo.MEDICA ? item.ID_DESTINO : null,
                                ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                                ID_INGRESO = selectIngreso.ID_INGRESO,
                                ID_JUZGADO = selectedExc_tipoValue == (short)enumExcarcelacionTipo.JURIDICA ? item.ID_DESTINO : null,
                                ID_TIPO_DOC = item.TIPO_DOCUMENTO,
                                CAU_ID_ANIO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_ANIO : null,
                                CAU_ID_CENTRO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_CENTRO : null,
                                CAU_ID_IMPUTADO = item.CAUSA_PENAL != null ? (int?)item.CAUSA_PENAL.ID_IMPUTADO : null,
                                CAU_ID_INGRESO = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_INGRESO : null,
                                CAU_ID_CAUSA_PENAL = item.CAUSA_PENAL != null ? (short?)item.CAUSA_PENAL.ID_CAUSA_PENAL : null,
                                ID_DESTINO = item.ID_CONSEC.HasValue ? item.ID_CONSEC.Value : 0,
                                ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                                CAN_ID_MOTIVO = item.ID_CANCELACION_MOTIVO,
                                CANCELADO_OBSERVA = item.CANCELACION_OBSERVACION,
                                CAUSA_PENAL_TEXTO = item.CAUSA_PENAL_TEXTO,
                                CANCELADO_TIPO = item.ID_CANCELACION_MOTIVO.HasValue ? "J" : string.Empty
                            });
                        }
                    var _excarcelacion = new EXCARCELACION
                    {
                        ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                        ID_ANIO = selectIngreso.ID_ANIO,
                        ID_CENTRO = selectIngreso.ID_CENTRO,
                        ID_ESTATUS = "PR",
                        ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                        ID_INGRESO = selectIngreso.ID_INGRESO,
                        ID_TIPO_EX = selectedExc_tipoValue,
                        ID_USUARIO = StaticSourcesViewModel.UsuarioLogin.Username,
                        PROGRAMADO_FEC = Excarcelacion_Fecha,
                        REGISTRO_FEC = _FechaServer,
                        OBSERVACION = Observaciones,
                        EXCARCELACION_DESTINO = _excarcelacion_destinos,
                        CERTIFICADO_MEDICO = certMedicoSiChecked ? 1 : 0
                    };
                    new cExcarcelacion().Actualizar(_excarcelacion, _excarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ?
                        (int)enumMensajeTipo.MODIFICACION_EXCARCELACION : (int)enumMensajeTipo.MODIFICACION_EXCARCELACION_MEDICA,
                        _excarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (int)enumMensajeTipo.CANCELACION_EXCARCELACION : (int)enumMensajeTipo.CANCELACION_EXCARCELACION_MEDICA,
                        (int)enumMensajeTipo.AVISO_CANCELACION_EXCARCELACION_AREA_MEDICA, _FechaServer, Parametro.ID_HOSPITAL_OTROS,
                        DateTime.Compare(_fecha_programada_Old.Value, Excarcelacion_Fecha.Value) == 0 ? false : true, GlobalVar.gCentro);
                    return true;
                }))
                {
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La excarcelación ha sido actualizada");
                    Limpiar();
                    AnioBuscarHabilitado = true;
                    FolioBuscarHabilitado = true;
                    NombreBuscarHabilitado = true;
                    ApellidoPaternoBuscarHabilitado = true;
                    ApellidoMaternoBuscarHabilitado = true;
                    BuscarImputadoHabilitado = true;
                    EliminarMenuEnabled = false;
                    CancelarMenuEnabled = false;
                }
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

        /// <summary>
        /// metodo usado para mostrar documento
        /// </summary>
        /// <param name="origen_comando" tipo="Origen_Comando_Visualizacion">origen del comando. </param>
        /// <param name="consec">Consecutivo  </param>
        /// <returns></returns>
        private void MostrarDocumento(Origen_Comando_Visualizacion origen_comando, byte[] documento = null, short? formato_documento = null)
        {
            try
            {
                switch (origen_comando)
                {
                    case Origen_Comando_Visualizacion.LISTADO_DOCUMENTOS:
                        if (SelectedDocumento.FORMATO_DOCUMENTO != 1 && SelectedDocumento.FORMATO_DOCUMENTO != 3 && SelectedDocumento.FORMATO_DOCUMENTO != 4)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                            return;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_NUCS_IMPUTADO);
                        var tc = new TextControlView();
                        tc.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_NUCS_IMPUTADO);
                        };
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                switch (SelectedDocumento.FORMATO_DOCUMENTO)
                                {
                                    case 1://DOCX
                                        tc.editor.Load(SelectedDocumento.DOCUMENTO, TXTextControl.BinaryStreamType.WordprocessingML);
                                        break;
                                    case 3://PDF
                                        tc.editor.Load(SelectedDocumento.DOCUMENTO, TXTextControl.BinaryStreamType.AdobePDF);
                                        break;
                                    case 4://DOC
                                        tc.editor.Load(SelectedDocumento.DOCUMENTO, TXTextControl.BinaryStreamType.MSWord);
                                        break;
                                    default:
                                        new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();
                        break;
                    case Origen_Comando_Visualizacion.VER_DOCUMENTO:

                        if (documento == null)
                            return;

                        if (formato_documento.Value == Parametro.ID_FORMATO_IMAGEN)
                        {
                            var tc3 = new TextControlView();
                            tc3.Closed += (s, e) =>
                            {
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            };
                            tc3.Loaded += async (s, e) =>
                            {
                                try
                                {
                                    string filepath = string.Empty;
                                    filepath = await Task.Factory.StartNew<string>(() =>
                                    {
                                        var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                                        File.WriteAllBytes(fileNamepdf, documento);
                                        return fileNamepdf;
                                    });
                                    tc3.editor.Load(File.ReadAllBytes(filepath), TXTextControl.BinaryStreamType.AdobePDF);
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                                }
                            };
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc3.Owner = PopUpsViewModels.MainWindow;
                            tc3.Show();
                        }
                        else
                        {
                            var tc2 = new TextControlView();
                            tc2.Closed += (s, e) =>
                            {
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            };
                            tc2.editor.Loaded += (s, e) =>
                            {
                                try
                                {
                                    switch (formato_documento.Value)
                                    {
                                        case 1://DOCX
                                            tc2.editor.Load(documento, TXTextControl.BinaryStreamType.WordprocessingML);
                                            break;
                                        case 3://PDF
                                            tc2.editor.Load(documento, TXTextControl.BinaryStreamType.AdobePDF);
                                            break;
                                        case 4://DOC
                                            tc2.editor.Load(documento, TXTextControl.BinaryStreamType.MSWord);
                                            break;
                                        default:
                                            new Dialogos().ConfirmacionDialogo("Notificación!", "Formato de archivo no válido");
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                                }
                            };
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc2.Owner = PopUpsViewModels.MainWindow;
                            tc2.Show();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
            }
        }

        private void ActualizarDestinos()
        {
            SelectedExcarcelacionDestino.ID_DESTINO = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? SelectedJuzgadoValue : SelectedHospitalValue;
            SelectedExcarcelacionDestino.DESTINO = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? Juzgados.Any(f => f.ID_JUZGADO == SelectedJuzgadoValue) ? Juzgados.FirstOrDefault(f => f.ID_JUZGADO == SelectedJuzgadoValue).DESCR : string.Empty :
                    SelectedHospitalValue == Parametro.ID_HOSPITAL_OTROS ? OtroHospital : Hospitales.Any(w => w.ID_HOSPITAL == selectedHospitalValue) ? Hospitales.FirstOrDefault(w => w.ID_HOSPITAL == selectedHospitalValue).DESCR : string.Empty;
            ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(ListaExcarcelacionDestinos);
        }

        private void InsertarDestinos()
        {
            try
            {
                if (ListaExcarcelacionDestinos == null)
                {
                    listaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>();
                }

                if(listaExcarcelacionDestinos != null && listaExcarcelacionDestinos.Any())
                    if(SelectedInterconsultaExcarcelacion.IdDestino.HasValue)
                        if(listaExcarcelacionDestinos.Any(x => x.ID_INTERC == SelectedInterconsultaExcarcelacion.IdInterconsulta))
                        {
                          new Dialogos().ConfirmacionDialogo("Validación!", "La interconsulta seleccionada ya se encuentra dentro de la lista");
                           return;
                        };

                listaExcarcelacionDestinos.Add(new CT_EXCARCELACION_DESTINO
                {
                    ID_DESTINO = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? SelectedJuzgadoValue : SelectedExc_TipoValue == (short)enumExcarcelacionTipo.MEDICA ? SelectedInterconsultaExcarcelacion != null ? SelectedInterconsultaExcarcelacion.IdDestino.HasValue ? SelectedInterconsultaExcarcelacion.IdDestino.Value : new short() : new short() : new short(),
                    DESTINO = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? Juzgados.Any(f => f.ID_JUZGADO == SelectedJuzgadoValue) ? Juzgados.FirstOrDefault(f => f.ID_JUZGADO == SelectedJuzgadoValue).DESCR : string.Empty :
                        //SelectedHospitalValue == Parametro.ID_HOSPITAL_OTROS ? OtroHospital : Hospitales.Any(w => w.ID_HOSPITAL == selectedHospitalValue) ? Hospitales.FirstOrDefault(w => w.ID_HOSPITAL == selectedHospitalValue).DESCR : string.Empty,
                        SelectedInterconsultaExcarcelacion != null ? SelectedInterconsultaExcarcelacion.IdDestino.HasValue ? SelectedInterconsultaExcarcelacion.NombreDestino : string.Empty : string.Empty,
                    DOCUMENTO = documento,
                    ID_INTERC = SelectedInterconsultaExcarcelacion != null ? (int?)SelectedInterconsultaExcarcelacion.IdInterconsulta : null,
                    FORMATO_DOCUMENTO = formato_documentacion_excarcelacion.HasValue ? formato_documentacion_excarcelacion.Value : new short?(),
                    TIPO_DOCUMENTO = tipo_documento_excarcelacion,
                    FOLIO = SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA ? SelectedInterconsultaExcarcelacion != null ? SelectedInterconsultaExcarcelacion.FolioInterConsulta : Folio_Doc : string.Empty,
                    ESTATUS = "PR", //programada
                    CAUSA_PENAL = CP_Excarcelacion_Destino,
                    FechaInterconsultaDestino = SelectedInterconsultaExcarcelacion != null ? SelectedInterconsultaExcarcelacion.FechaInterconsulta : new DateTime?(),
                    CAUSA_PENAL_TEXTO = CP_Excarcelacion_Destino == null && !string.IsNullOrWhiteSpace(CP_Folio_Destino) ? CP_Folio_Destino : string.Empty
                });
                OnPropertyChanged("ListaExcarcelacionDestinos");
                if (SelectedExc_TipoValue == (short)enumExcarcelacionTipo.JURIDICA)
                {
                    if (ListaExcarcelacionDestinos != null && ListaExcarcelacionDestinos.Count > 0 && ListaExcarcelacionDestinos.Where(w => w.CAUSA_PENAL != null).Any(w => w.CAUSA_PENAL.NUC != null))
                    {
                        IsCertificadoEnabled = false;
                        CertMedicoSiChecked = true;
                        CertMedicoNoChecked = false;
                    }
                    else
                    {
                        if ((CP_Excarcelacion_Destino != null && CP_Excarcelacion_Destino.NUC != null) || Parametro.CERT_MEDICO_OBLIGATORIO == 1)
                        {
                            IsCertificadoEnabled = false;
                            CertMedicoSiChecked = true;
                            CertMedicoNoChecked = false;
                        }
                        else
                        {
                            IsCertificadoEnabled = true;
                            CertMedicoSiChecked = false;
                            CertMedicoNoChecked = true;
                        }
                    }
                }
                else
                {
                    IsCertificadoEnabled = false;
                    CertMedicoSiChecked = false;
                    CertMedicoNoChecked = false;
                    ValidaFechas(SelectedInterconsultaExcarcelacion.FechaInterconsulta);
                    //Excarcelacion_Fecha = SelectedInterconsultaExcarcelacion.FechaInterconsulta;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el destino", ex);
            }
        }

        private void ValidaFechas(DateTime? FechaCompara)
        {
            try
            {
                //if (!Excarcelacion_Fecha.HasValue)//NO TIENE UN VALOR , SE LE ASIGNA ESTA FECHA COMO TENTATIVAMENTE LA MENOR
                //    Excarcelacion_Fecha = _fecha_programada_Old = FechaCompara;
                //else
                if (FechaCompara.HasValue)
                    if (!string.IsNullOrEmpty(SelectedInterconsultaExcarcelacion.NombrePrioridad))
                    {
                        if (SelectedInterconsultaExcarcelacion.NombrePrioridad.Contains("URGENTE"))
                        { // ES UNA URGENCIA, TOMA LA FECHA DEL SISTEMA, LA MINIMA FECHA PERMITIDA ES LA FECHA DE LA CITA
                            if (!Excarcelacion_Fecha.HasValue)
                                Excarcelacion_Fecha = FechaCompara;

                            int _com = DateTime.Compare(Excarcelacion_Fecha.Value, FechaCompara.Value);
                            FechaMinimaExcarcelacion = FechaMaximaExcarcelacion = null;
                            FechaMinimaExcarcelacion = DateTime.Parse(FechaCompara.ToString());
                            switch (_com)
                            {
                                case -1:// ES ANTES DE LA FECHA DE LA CITA, NO SE PERMITE
                                    break;

                                case 0:// ES LA MISMA FECHA
                                    break;

                                case 1:
                                    Excarcelacion_Fecha = FechaCompara;
                                    break;

                                default:
                                    ///no case
                                    break;
                            }
                        }

                        if (SelectedInterconsultaExcarcelacion.NombrePrioridad.Contains("ORDINARIA"))
                        {
                            if (!Excarcelacion_Fecha.HasValue)
                                Excarcelacion_Fecha = FechaCompara;

                            int _com = DateTime.Compare(Excarcelacion_Fecha.Value, FechaCompara.Value);
                            FechaMinimaExcarcelacion = FechaMaximaExcarcelacion = null;
                            FechaMaximaExcarcelacion = DateTime.Parse(FechaCompara.ToString());
                            switch (_com)
                            {
                                case -1:// ES ANTES DE LA FECHA DE LA CITA, NO SE PERMITE
                                    break;

                                case 0:// ES LA MISMA FECHA
                                    break;

                                case 1:
                                    Excarcelacion_Fecha = FechaCompara;
                                    break;

                                default:
                                    ///no case
                                    break;
                            }
                        }
                    };
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion

        #region Llenados de Catalogos
        private void ObtenerExcarcelacionTipos(bool isExceptionManaged = false)
        {
            try
            {
                excarcelacion_tipos = new ObservableCollection<EXCARCELACION_TIPO>(new cExcarcelacionTipo().Seleccionar());
                excarcelacion_tipos.Insert(0, new EXCARCELACION_TIPO
                {
                    DESCR = "SELECCIONAR",
                    ID_TIPO_EX = 0
                });
                OnPropertyChanged("Excarcelacion_Tipos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de excarcelación", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerDestinos(int excarcelacion_tipo, bool isExceptionManaged = false)
        {
            try
            {
                switch (excarcelacion_tipo)
                {
                    case ((int)enumExcarcelacionTipo.JURIDICA):
                        ObtenerPaises(isExceptionManaged);
                        ObtenerEstados(Parametro.PAIS, isExceptionManaged);
                        ObtenerMunicipios(Parametro.ESTADO, isExceptionManaged);
                        ObtenerJuzgados(Parametro.PAIS, Parametro.ESTADO, 0, "0", isExceptionManaged);
                        break;
                }

            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerPaises(bool isExceptionManaged = false)
        {
            try
            {
                paises = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                OnPropertyChanged("Paises");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerEstados(short pais, bool isExceptionManaged = false)
        {
            try
            {
                estados = new ObservableCollection<ENTIDAD>(new cEntidad().ObtenerTodosPais(pais));
                estados.Insert(0, new ENTIDAD
                {
                    ID_ENTIDAD = 0,
                    DESCR = "SELECCIONAR"
                });
                OnPropertyChanged("Estados");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerMunicipios(short estado, bool isExceptionManaged = false)
        {
            try
            {
                municipios = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener((short)estado));
                municipios.Insert(0, new MUNICIPIO
                {
                    ID_MUNICIPIO = 0,
                    MUNICIPIO1 = "SELECCIONAR"
                });
                OnPropertyChanged("Municipios");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerFueros(bool isExceptionManaged = false)
        {
            try
            {
                fueros = new ObservableCollection<FUERO>(new cFuero().ObtenerTodos());
                OnPropertyChanged("Fueros");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerJuzgados(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                juzgados = new ObservableCollection<JUZGADO>(new cJuzgado().Obtener(pais, estado, municipio, fuero));
                juzgados.Insert(0, new JUZGADO
                {
                    ID_JUZGADO = 0,
                    DESCR = "SELECCIONAR"
                });
                OnPropertyChanged("Juzgados");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void OnPaisChanged(short pais, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerEstados(pais, false);
                if (pais == Parametro.PAIS)
                {
                    ObtenerMunicipios(Parametro.ESTADO);
                    ObtenerJuzgados(pais, Parametro.ESTADO, 0, fuero, isExceptionManaged);
                }
                else
                {
                    ObtenerMunicipios(0, false);
                    ObtenerJuzgados(pais, 0, 0, fuero, false);
                }


            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnEstadoChanged(short pais, short estado, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerMunicipios(estado);
                ObtenerJuzgados(pais, estado, 0, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnMunicipioChanged(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerJuzgados(pais, estado, municipio, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnFueroChanged(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerJuzgados(pais, estado, municipio, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void ObtenerHospital(bool isExceptionManaged = false)
        {
            try
            {
                hospitales = new ObservableCollection<HOSPITAL>(new cHospitales().Seleccionar());
                hospitales.Insert(0, new HOSPITAL
                {
                    DESCR = "SELECCIONAR",
                    ID_HOSPITAL = 0
                });
                if (hospitales.FirstOrDefault(w => w.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS) != null)
                    hospitales.Move(hospitales.IndexOf(hospitales.First(w => w.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS)), hospitales.IndexOf(hospitales.Last()));
                OnPropertyChanged("Hospitales");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        #region Buscar Excarcelaciones
        private void ObtenerExcarcelacionTiposBusqueda(bool isExceptionManaged = false)
        {
            try
            {
                excarcelacion_TiposBuscar = new ObservableCollection<EXCARCELACION_TIPO>(new cExcarcelacionTipo().Seleccionar());
                excarcelacion_TiposBuscar.Insert(0, new EXCARCELACION_TIPO
                {
                    DESCR = "SELECCIONAR",
                    ID_TIPO_EX = 0
                });
                OnPropertyChanged("Excarcelacion_TiposBuscar");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de excarcelación", ex);
                else
                    throw ex;

            }
        }
        private void ObtenerDestinosBuscarExc(int excarcelacion_tipo, bool isExceptionManaged = false)
        {
            try
            {
                switch (excarcelacion_tipo)
                {
                    case ((int)enumExcarcelacionTipo.JURIDICA):
                        ObtenerPaisesBuscarExc(isExceptionManaged);
                        ObtenerEstadosBuscarExc(Parametro.PAIS, isExceptionManaged);
                        ObtenerMunicipiosBuscarExc(Parametro.ESTADO, isExceptionManaged);
                        ObtenerJuzgadosBuscarExc(Parametro.PAIS, Parametro.ESTADO, 0, "0", isExceptionManaged);
                        break;
                }

            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerPaisesBuscarExc(bool isExceptionManaged = false)
        {
            try
            {
                paisesBuscarExc = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                OnPropertyChanged("PaisesBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerEstadosBuscarExc(short pais, bool isExceptionManaged = false)
        {
            try
            {
                estadosBuscarExc = new ObservableCollection<ENTIDAD>(new cEntidad().ObtenerTodosPais(pais));
                estadosBuscarExc.Insert(0, new ENTIDAD
                {
                    ID_ENTIDAD = 0,
                    DESCR = "SELECCIONAR"
                });
                OnPropertyChanged("EstadosBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerMunicipiosBuscarExc(short estado, bool isExceptionManaged = false)
        {
            try
            {
                municipiosBuscarExc = new ObservableCollection<MUNICIPIO>(new cMunicipio().Obtener((short)estado));
                municipiosBuscarExc.Insert(0, new MUNICIPIO
                {
                    ID_MUNICIPIO = 0,
                    MUNICIPIO1 = "SELECCIONAR"
                });
                OnPropertyChanged("MunicipiosBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerFuerosBuscarExc(bool isExceptionManaged = false)
        {
            try
            {
                fuerosBuscarExc = new ObservableCollection<FUERO>(new cFuero().ObtenerTodos());
                OnPropertyChanged("FuerosBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void ObtenerJuzgadosBuscarExc(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                juzgadosBuscarExc = new ObservableCollection<JUZGADO>(new cJuzgado().Obtener(pais, estado, municipio, fuero));
                juzgadosBuscarExc.Insert(0, new JUZGADO
                {
                    ID_JUZGADO = 0,
                    DESCR = "SELECCIONAR"
                });
                OnPropertyChanged("JuzgadosBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de destinos", ex);
                else
                    throw ex;

            }
        }

        private void OnPaisBuscarExcChanged(short pais, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerEstadosBuscarExc(pais, false);
                if (pais == Parametro.PAIS)
                {
                    ObtenerMunicipiosBuscarExc(Parametro.ESTADO);
                    ObtenerJuzgadosBuscarExc(pais, Parametro.ESTADO, 0, fuero, isExceptionManaged);
                }
                else
                {
                    ObtenerMunicipiosBuscarExc(0, false);
                    ObtenerJuzgadosBuscarExc(pais, 0, 0, fuero, false);
                }


            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnEstadoBuscarExcChanged(short pais, short estado, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerMunicipiosBuscarExc(estado);
                ObtenerJuzgadosBuscarExc(pais, estado, 0, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnMunicipioBuscarExcChanged(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerJuzgadosBuscarExc(pais, estado, municipio, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void OnFueroBuscarExcChanged(short pais, short estado, short municipio, string fuero, bool isExceptionManaged = false)
        {
            try
            {
                ObtenerJuzgadosBuscarExc(pais, estado, municipio, fuero, isExceptionManaged);
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        private void ObtenerHospitalBuscarExc(bool isExceptionManaged = false)
        {
            try
            {
                hospitalesBuscarExc = new ObservableCollection<HOSPITAL>(new cHospitales().Seleccionar());
                hospitalesBuscarExc.Insert(0, new HOSPITAL
                {
                    DESCR = "SELECCIONAR",
                    ID_HOSPITAL = 0
                });
                if (hospitalesBuscarExc.FirstOrDefault(w => w.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS) != null)
                    hospitalesBuscarExc.Move(hospitalesBuscarExc.IndexOf(hospitalesBuscarExc.First(w => w.ID_HOSPITAL == Parametro.ID_HOSPITAL_OTROS)), hospitalesBuscarExc.IndexOf(hospitalesBuscarExc.Last()));
                OnPropertyChanged("HospitalesBuscarExc");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }

        #endregion

        #region Motivo Cancelacion Excarcelacion
        private void ObtenerExcarcelacion_Cancela_Motivos(bool isExceptionManaged = false)
        {
            try
            {
                cancelacion_Motivos = new ObservableCollection<EXCARCELACION_CANCELA_MOTIVO>(new cExcarcelacion_Cancela_Motivo().Seleccionar());
                cancelacion_Motivos.Insert(0, new EXCARCELACION_CANCELA_MOTIVO
                {
                    DESCR = "SELECCIONAR",
                    ID_EXCARCELACION_CANCELA = 0
                });
                OnPropertyChanged("Cancelacion_Motivos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los catalogos", ex);
                else
                    throw ex;
            }
        }
        #endregion

        #endregion

        #region escanner
        private async Task ObtenerTipoDocumento()
        {
            try
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                listTipoDocumento = new ObservableCollection<TipoDocumento>();
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    var _tipos_documentos_excarcelacion = new cExcarcelacion_Tipo_Docto().Seleccionar(selectedExc_tipoValue);
                    foreach (var item in _tipos_documentos_excarcelacion)
                    {
                        if (tipo_documento_excarcelacion.HasValue && _tipos_documentos_excarcelacion.Any(w => w.ID_TIPO_DOC == tipo_documento_excarcelacion.Value))
                            _SelectedTipoDocumento = _tipos_documentos_excarcelacion.Where(w => w.ID_TIPO_DOC == tipo_documento_excarcelacion.Value).Select(s => new TipoDocumento
                            {
                                ID_TIPO_DOCUMENTO = item.ID_TIPO_DOC,
                                DESCR = item.DESCR,
                                DIGITALIZADO = true
                            }).First();
                        listTipoDocumento.Add(new TipoDocumento
                        {
                            ID_TIPO_DOCUMENTO = item.ID_TIPO_DOC,
                            DESCR = item.DESCR,
                            DIGITALIZADO = _SelectedTipoDocumento != null && _SelectedTipoDocumento.ID_TIPO_DOCUMENTO == item.ID_TIPO_DOC ? true : false
                        });
                    }
                    OnPropertyChanged("ListTipoDocumento");
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }

        }

        private async void Scan(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(Duplex, SelectedSource, obj);
                });
                PopUpsViewModels.EnabledMenu = true;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }

        private void GuardarDocumento()
        {
            try
            {

                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    Documento = new byte[DocumentoDigitalizado.Count()];
                    DocumentoDigitalizado.CopyTo(Documento, 0);
                    Tipo_Documento_Excarcelacion = SelectedTipoDocumento.ID_TIPO_DOCUMENTO;
                    formato_documentacion_excarcelacion = Parametro.ID_FORMATO_IMAGEN;
                    isDocumentoAgregado = true;
                    OnPropertyChanged("IsDocumentoAgregado");
                    IsBuscarCPEnabled = true;
                    if (cp_excarcelacion_destino != null)
                    {
                        cp_folio_destino = string.Empty;
                        OnPropertyChanged("CP_Folio_Destino");
                    }
                    cp_excarcelacion_destino = null;
                    OnPropertyChanged("CP_Excarcelacion_Destino");

                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }));
                    if (AutoGuardado)
                        escaner.Show();
                }));

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }

        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (Documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "NO HA SIDO DIGITALIZADO UN DOCUMENTO!");
                    return;
                }

                if (ListTipoDocumento.FirstOrDefault(w => w.ID_TIPO_DOCUMENTO == Tipo_Documento_Excarcelacion) != null)
                {
                    ListTipoDocumento.FirstOrDefault(w => w.ID_TIPO_DOCUMENTO == Tipo_Documento_Excarcelacion).DIGITALIZADO = true;
                    ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                    SelectedTipoDocumento = ListTipoDocumento.FirstOrDefault(w => w.ID_TIPO_DOCUMENTO == Tipo_Documento_Excarcelacion);
                }

                if (formato_documentacion_excarcelacion != Parametro.ID_FORMATO_IMAGEN)
                {
                    new Dialogos().ConfirmacionDialogo("FORMATO DE DOCUMENTO!", "EL DOCUMENTO ESTA EN FORMATO DOCX. NO PUEDE SER MOSTRADO EN EL VISOR DEL ESCANER");
                    return;
                }

                documentoDigitalizado = new byte[Documento.Count()];
                Documento.CopyTo(documentoDigitalizado, 0);


                if (DocumentoDigitalizado == null)
                    return;

                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }
        #endregion

        #region Listado de Documentos
        private void ObtenerListadoDocumentos(short id_anio, short id_centro, int id_imputado, short id_ingreso, short tipo_mensaje, string nuc, string causa_penal, DateTime? fechaInicio = null, DateTime? fechaFinal = null, bool isExceptionManaged = false)
        {
            try
            {
                lstDocumentos = new ObservableCollection<DOCUMENTO_SISTEMA>(new cMensaje().SeleccionarMensajes(id_anio, id_centro, id_imputado, id_ingreso, tipo_mensaje, nuc, causa_penal, fechaInicio, fechaFinal));
                OnPropertyChanged("LstDocumentos");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el catalogo de tipos de excarcelación", ex);
                else
                    throw ex;
            }
        }
        #endregion

        #region Busqueda de Excarcelaciones

        private void BuscarExcarcelaciones(short centro, List<string> estatus = null, short? anio = null, int? folio = null, string nombre = "", string paterno = "", string materno = "", DateTime? fecha_inicio = null,
            DateTime? fecha_final = null, short? id_tipo_exc = null, short? id_juzgado = null, string id_fuero = "", short? id_municipio = null, short? id_estado = null, short? id_pais = null, short? id_hospital = null, string otro_hospitalB = "",
            bool isExceptionManaged = false)
        {
            try
            {
                var _id_otro_hospital = (short)Parametro.ID_HOSPITAL_OTROS;
                listaExcarcelacionesBusqueda = new ObservableCollection<EXCARCELACION_DATOS>(new cExcarcelacion().Seleccionar(centro, (short)enumExcarcelacionTipo.JURIDICA,
                    (short)enumExcarcelacionTipo.MEDICA, estatus, anio, folio, nombre, paterno, materno, fecha_inicio, fecha_final, id_tipo_exc, id_juzgado, id_fuero, id_municipio,
                    id_estado, id_pais, id_hospital, otro_hospitalB).Select(s => new EXCARCELACION_DATOS
                    {
                        PROGRAMADO_FEC = s.PROGRAMADO_FEC.Value,
                        ID_ANIO = s.ID_ANIO,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        ID_INGRESO = s.ID_INGRESO,
                        MATERNO = s.INGRESO.IMPUTADO.MATERNO == null || s.INGRESO.IMPUTADO.MATERNO.Trim() == "" ? "" : s.INGRESO.IMPUTADO.MATERNO.Trim(),
                        NOMBRE = s.INGRESO.IMPUTADO.NOMBRE == null || s.INGRESO.IMPUTADO.NOMBRE.Trim() == "" ? "" : s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                        PATERNO = s.INGRESO.IMPUTADO.PATERNO == null || s.INGRESO.IMPUTADO.PATERNO.Trim() == "" ? "" : s.INGRESO.IMPUTADO.PATERNO.Trim(),
                        CAMA = s.INGRESO.ID_UB_CAMA.HasValue ? s.INGRESO.CAMA : null,
                        SELECTED_EXCARCELACION = s
                    }));
                OnPropertyChanged("ListaExcarcelacionesBusqueda");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las excarcelaciones", ex);
                else
                    throw ex;
            }
        }

        #endregion

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

        #region Busqueda Imputados por NUC

        private void ObtenerImputadosNUC(short centro, string _nuc, bool isExceptionManaged = false)
        {
            try
            {
                listadoIngresoNUC = new ObservableCollection<CAUSA_PENAL_INGRESOS>(new cIngreso().SeleccionarPorNUC(centro, _nuc));
                OnPropertyChanged("ListadoIngresoNUC");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las excarcelaciones", ex);
                else
                    throw ex;
            }
        }

        #endregion

        #region Buscar Causa Penal
        public async void BuscaCausaPenal(string _causa_penal)
        {
            short _anio = 0;
            int _folio = 0;
            var _cp_invalido = false;
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {

                    if (_causa_penal.Length < 5)
                    {
                        _cp_invalido = true;
                        return;
                    }

                    _anio = short.Parse(_causa_penal.Substring(0, 4));
                    if (_causa_penal.Contains('/'))
                        _folio = int.Parse(_causa_penal.Substring(5, _causa_penal.Length - 5));
                    else
                        _folio = int.Parse(_causa_penal.Substring(4, _causa_penal.Length - 4));

                    cp_excarcelacion_destino = new cCausaPenal().ObtenerPorFolio(selectIngreso.ID_CENTRO, selectIngreso.ID_ANIO, selectIngreso.ID_IMPUTADO,
                        selectIngreso.ID_INGRESO, _anio, _folio);
                });

                if (_cp_invalido || CP_Excarcelacion_Destino == null)
                {

                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "EL FOLIO DE CAUSA PENAL " + _causa_penal + " NO HA SIDO ALIMENTADO AUN AL SISTEMA O NO ES UN FOLIO VALIDO PARA EL IMPUTADO,¿DESEA ASIGNARSELO A LA EXCARCELACIÓN?") == 1)
                    {
                        CP_Folio_Destino = _causa_penal;
                    }
                    else
                        CP_Folio_Destino = string.Empty;
                    return;
                }
                CargarDestinoCP(_anio, _folio);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las excarcelaciones", ex);
            }
        }

        private async void CargarDestinoCP(short _anio, int _folio)
        {
            CP_Folio_Destino = _anio.ToString() + "/" + _folio.ToString().PadLeft(5, '0');
            foreach (var item in _lista_control_evento)
                Interaction.GetTriggers(item.Control).Remove(item.Evento);
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                ObtenerFueros(true);
                ObtenerPaises(true);
                ObtenerEstados(cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value, true);
                ObtenerMunicipios(cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value, true);
                ObtenerJuzgados(cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value, cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value,
                    cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO.Value, cp_excarcelacion_destino.JUZGADO.ID_FUERO, true);
            }).ContinueWith((prevTask) =>
            {
                if (fueros != null)
                {
                    selectedFueroValue = cp_excarcelacion_destino.JUZGADO.ID_FUERO;
                    OnPropertyChanged("SelectedFueroValue");
                }
                if (paises != null && paises.FirstOrDefault(w => w.ID_PAIS_NAC == cp_excarcelacion_destino.JUZGADO.ID_PAIS) != null)
                {
                    selectedPaisValue = cp_excarcelacion_destino.JUZGADO.ID_PAIS.Value;
                    OnPropertyChanged("SelectedPaisValue");
                }
                if (estados != null && estados.FirstOrDefault(w => w.ID_ENTIDAD == cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD) != null)
                {
                    selectedEstadoValue = cp_excarcelacion_destino.JUZGADO.ID_ENTIDAD.Value;
                    OnPropertyChanged("SelectedEstadoValue");
                }
                if (municipios != null && municipios.FirstOrDefault(w => w.ID_MUNICIPIO == cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO) != null)
                {
                    selectedMunicipioValue = cp_excarcelacion_destino.JUZGADO.ID_MUNICIPIO.Value;
                    OnPropertyChanged("SelectedMunicipioValue");
                }
                if (juzgados != null && juzgados.FirstOrDefault(w => w.ID_JUZGADO == cp_excarcelacion_destino.JUZGADO.ID_JUZGADO) != null)
                {
                    selectedJuzgadoValue = cp_excarcelacion_destino.JUZGADO.ID_JUZGADO;
                    OnPropertyChanged("SelectedJuzgadoValue");
                }
            });
            var _main = PopUpsViewModels.MainWindow;
            BindUpdatedSources("SourceUpdated", "cambio_pais_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbPaisesDestino);
            BindUpdatedSources("SourceUpdated", "cambio_estado_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbEstadosDestino);
            BindUpdatedSources("SourceUpdated", "cambio_municipio_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbMunicipiosDestino);
            BindUpdatedSources("SourceUpdated", "cambio_fuero_juridico", "CmdModelChanged", _main.AgregarDestinosExcarcelacionView.cbFuerosDestino);
        }
        #endregion

        private enum Origen_Comando_Visualizacion
        {
            LISTADO_DOCUMENTOS,
            VER_DOCUMENTO
        }

        private class ControlEvento
        {
            public DependencyObject Control { get; set; }
            public System.Windows.Interactivity.EventTrigger Evento { get; set; }
        }



        #region REPORTE
        private void MuestraOficioSolicitudExcarcelacion(int? _IdInterconsulta = null)
        {
            try
            {
                if (_IdInterconsulta == null)
                    return;

                #region Iniciliza el entorno para mostrar el reporte al usuario
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();
                #endregion

                string _NombreJ = string.Empty;
                DEPARTAMENTO_ACCESO _CoordinadorJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 2).FirstOrDefault();
                if (_CoordinadorJuridico != null)
                    if (_CoordinadorJuridico.USUARIO != null)
                        if (_CoordinadorJuridico.USUARIO.EMPLEADO != null)
                            if (_CoordinadorJuridico.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro)
                                if (_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA != null)
                                    _NombreJ = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(_CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _CoordinadorJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);

                string NombreSubDirector = string.Empty;
                DEPARTAMENTO_ACCESO _SubDirector = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 23).FirstOrDefault();
                if (_SubDirector != null)
                    if (_SubDirector.USUARIO != null)
                        if (_SubDirector.USUARIO.EMPLEADO != null)
                            if (_SubDirector.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro)
                                if (_SubDirector.USUARIO.EMPLEADO.PERSONA != null)
                                    NombreSubDirector = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(_SubDirector.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _SubDirector.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);


                string NombreC = string.Empty;
                var NombreComandante = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 4).FirstOrDefault();
                if (NombreComandante != null)
                    if (NombreComandante.USUARIO != null)
                        if (NombreComandante.USUARIO.EMPLEADO != null)
                            if (NombreComandante.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro)
                                if (NombreComandante.USUARIO.EMPLEADO.PERSONA != null)
                                    NombreC = string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreComandante.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);

                string _NombreAreasTecnicas = string.Empty;
                var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
                if (NombreAreasTecnicas != null)
                    if (NombreAreasTecnicas.USUARIO != null)
                        if (NombreAreasTecnicas.USUARIO.EMPLEADO != null)
                            if (NombreAreasTecnicas.USUARIO.EMPLEADO.ID_CENTRO == GlobalVar.gCentro)
                                if (NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null)
                                    _NombreAreasTecnicas = string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                        !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);

                var _datosInterconsulta = new cInterconsulta_Solicitud().GetData(x => x.ID_INTERSOL == _IdInterconsulta).FirstOrDefault();
                cSolicitudExcarcelacionReporte _datosReporte = new cSolicitudExcarcelacionReporte();
                var _centro = new cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                _datosReporte.NombreCentro = string.Format("DIRECTOR DEL {0}", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);
                _datosReporte.TextoGenerico1 = string.Format("DEL {0}", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);

                if (_datosInterconsulta != null)
                {
                    string _detalle = string.Empty;
                    var _detalleRazonEstudios = _datosInterconsulta.SERVICIO_AUX_INTERCONSULTA;
                    if (_detalleRazonEstudios.Any())
                        foreach (var item in _detalleRazonEstudios)
                            _detalle += string.Format(" ESTUDIO {0}, ", item.SERVICIO_AUX_DIAG_TRAT != null ? !string.IsNullOrEmpty(item.SERVICIO_AUX_DIAG_TRAT.DESCR) ? item.SERVICIO_AUX_DIAG_TRAT.DESCR.Trim() : string.Empty : string.Empty);

                    _datosReporte.Parrafo1 = string.Format("HAGO DE SU CONOCIMIENTO QUE EL INTERNO: <b>{0}</b> CON NÚMERO DE EXPEDIENTE <b>{1}</b>, UBICADO EN <b>{2}</b>. DEBERÁ SER TRASLADADO EL DÍA <b>{3}</b>, AL <b>{4}</b>.",
                        string.Format("{0} {1} {2}", _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty),//0
                        _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ?
                            string.Format("{0} / {1}", _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO, _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO) : string.Empty : string.Empty,//1

                        _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO != null ?
                            string.Format("{0}-{1}{2}-{3}",
                            _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.DESCR) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? !string.IsNullOrEmpty(_datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.ID_CELDA) ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                            _datosInterconsulta.ID_NOTA_MEDICA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.CAMA.CELDA != null ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_UB_CAMA.HasValue ? _datosInterconsulta.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_UB_CAMA.Value.ToString() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty) : string.Empty : string.Empty : string.Empty,///2
                         string.Format("{0} ", Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper()),///3
                         string.Format("{0} AL ÁREA DE  {1} {2}",
                            _datosInterconsulta.HOJA_REFERENCIA_MEDICA != null ? _datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? !string.IsNullOrEmpty(_datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? _datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            _datosInterconsulta.INTERCONSULTA_ATENCION_TIPO != null ?
                            string.Concat(!string.IsNullOrEmpty(_datosInterconsulta.INTERCONSULTA_ATENCION_TIPO.DESCR) ? _datosInterconsulta.INTERCONSULTA_ATENCION_TIPO.DESCR.Trim() : string.Empty, _detalle) : string.Empty,
                            _datosInterconsulta.ID_ESPECIALIDAD.HasValue ? !string.IsNullOrEmpty(_datosInterconsulta.ESPECIALIDAD.DESCR) ? _datosInterconsulta.ESPECIALIDAD.DESCR.Trim() : string.Empty : string.Empty)///4
                    );

                    _datosReporte.JefeJuridico = _NombreJ;
                    _datosReporte.CCP1 = string.Format("{0}. SUBDIRECTOR DEL \"{1}\" PARA SU CONOCIMIENTO.", NombreSubDirector, _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);
                    _datosReporte.CCP2 = string.Format("{0}.- COMANDANTE DEL \"{1}\" PARA SU CONOCIMIENTO.", NombreC, _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);
                    _datosReporte.CCP3 = string.Format("{0}.- COORDINADOR DE ÁREAS TÉCNICAS \"{1}\" PARA SU CONOCIMIENTO", _NombreAreasTecnicas, _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);
                    _datosReporte.NombreMedico = _datosInterconsulta.USUARIO != null ? _datosInterconsulta.USUARIO.EMPLEADO != null ? _datosInterconsulta.USUARIO.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(_datosInterconsulta.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _datosInterconsulta.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_datosInterconsulta.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _datosInterconsulta.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_datosInterconsulta.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _datosInterconsulta.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _datosReporte.NombreDirector = _centro != null ? !string.IsNullOrEmpty(_centro.DIRECTOR) ? _centro.DIRECTOR.Trim() : string.Empty : string.Empty;
                    _datosReporte.TectoGenerico4 = string.Format("{0}, {1} {2}", _centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(_centro.MUNICIPIO.MUNICIPIO1) ? _centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        _centro.ID_MUNICIPIO.HasValue ? _centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(_centro.MUNICIPIO.ENTIDAD.DESCR) ? _centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        Fechas.fechaLetra(Fechas.GetFechaDateServer, false).ToUpper());
                    _datosReporte.TextoGenerico2 = !string.IsNullOrEmpty(_datosInterconsulta.OFICIO_EXC) ? _datosInterconsulta.OFICIO_EXC.Trim() : string.Empty;
                    _datosReporte.Cedula = string.Format("CED. PROF. {0}", _datosInterconsulta.USUARIO != null ? _datosInterconsulta.USUARIO.EMPLEADO != null ? !string.IsNullOrEmpty(_datosInterconsulta.USUARIO.EMPLEADO.CEDULA) ? _datosInterconsulta.USUARIO.EMPLEADO.CEDULA.Trim() : string.Empty : string.Empty : string.Empty);
                    _datosReporte.Parraro2 = string.Format("EL MOTIVO DEL TRASLADO DE ESTE PACIENTE (INTERNO), ES POR VEZ {0}, <b><u>{1}</u></b>, YA QUE EN ESTE CENTRO NO CONTAMOS CON LOS MEDIOS PARA TRATAR DICHA {2}",
                        _datosInterconsulta.HOJA_REFERENCIA_MEDICA != null ? _datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_TIPO_CITA.HasValue ? !string.IsNullOrEmpty(_datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().CITA_TIPO.DESCR) ? _datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().CITA_TIPO.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                        _datosInterconsulta.HOJA_REFERENCIA_MEDICA != null ? !string.IsNullOrEmpty(_datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().MOTIVO) ? _datosInterconsulta.HOJA_REFERENCIA_MEDICA.FirstOrDefault().MOTIVO.Trim() : string.Empty : string.Empty,
                        _datosInterconsulta.ID_INTERAT.HasValue ? !string.IsNullOrEmpty(_datosInterconsulta.INTERCONSULTA_ATENCION_TIPO.DESCR) ? _datosInterconsulta.INTERCONSULTA_ATENCION_TIPO.DESCR.Trim() : string.Empty : string.Empty);
                };

                var Encabezado = new cEncabezado();
                Encabezado.TituloUno = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty;
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.NombreReporte = string.Format("EXCARCELACIÓN {0}", _datosInterconsulta.ID_INIVEL.HasValue ? !string.IsNullOrEmpty(_datosInterconsulta.INTERCONSULTA_NIVEL_PRIORIDAD.DESCR) ? _datosInterconsulta.INTERCONSULTA_NIVEL_PRIORIDAD.DESCR.Trim() : string.Empty : string.Empty);
                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                var ds1 = new List<cEncabezado>();
                ds1.Add(Encabezado);
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                View.Report.LocalReport.DataSources.Add(rds1);


                var ds2 = new List<cSolicitudExcarcelacionReporte>();
                ds2.Add(_datosReporte);
                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = ds2;
                View.Report.LocalReport.DataSources.Add(rds2);

                View.Report.LocalReport.ReportPath = "Reportes/rSolicitudExcarcelacion.rdlc";
                View.Report.RefreshReport();
                byte[] renderedBytes;

                Microsoft.Reporting.WinForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;


                renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                        tc.editor.IsSpellCheckingEnabled = true;
                        tc.editor.TextFrameMarkerLines = false;
                        tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                        tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();

            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion


    }


}
