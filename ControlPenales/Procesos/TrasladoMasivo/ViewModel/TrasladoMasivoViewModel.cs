using ControlPenales;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Compartidos;
using WPFPdfViewer;
using System.IO;

namespace ControlPenales
{
     partial class TrasladoMasivoViewModel : ValidationViewModelBase
    {
        private PdfViewer PDFViewer;
        private IQueryable<PROCESO_USUARIO> permisos;

        #region constructor
        public TrasladoMasivoViewModel() {}
        #endregion

        #region Generales

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
               
                case "buscar_menu":
                    await BuscarTraslado(GlobalVar.gCentro, new List<string>() { "PR", "EP" }, null,"LO");
                    LimpiarBusquedaTraslados();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                    SetValidacionesBuscaTraslados();
                    break;
                case "salir_menu":
                    SalirMenu();
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
                case "agregar_interno":
                    if (SelectedIngreso==null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar a un imputado.");
                        return;
                    }
                    IQueryable<EXCARCELACION_DESTINO> _excarcelacion_destinos=null;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                        _excarcelacion_destinos = new cExcarcelacion_Destino().Seleccionar(selectedIngreso.Ingreso.ID_ANIO, selectedIngreso.Ingreso.ID_CENTRO, selectedIngreso.Ingreso.ID_IMPUTADO,
                            selectedIngreso.Ingreso.ID_INGRESO, new List<string>() {"PR","AU","EP","AC"});
                    });
                    if (_excarcelacion_destinos!=null && _excarcelacion_destinos.Count() > 0)
                    {
                        if (selectedIngreso.Ingreso.EXCARCELACION !=null && selectedIngreso.Ingreso.EXCARCELACION.Any(a => a.ID_ESTATUS == "EP" || a.ID_ESTATUS == "AC"))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación!", "El imputado tiene una excarcelación EN PROCESO o ACTIVA");
                            return;
                        }
                        TituloExcarcelaciones= string.Format("EXCARCELACIONES PROGRAMADAS O AUTORIZADAS DEL IMPUTADO {0}/{1} {2}",selectedIngreso.Ingreso.ID_ANIO,
                            selectedIngreso.Ingreso.ID_IMPUTADO,ObtieneNombre(selectedIngreso.Ingreso.IMPUTADO));
                        Excarcelacion_Destinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(_excarcelacion_destinos.Where(w => w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AU").Select(s => new CT_EXCARCELACION_DESTINO
                        {
                            FECHA_EXCARCELACION=s.EXCARCELACION.PROGRAMADO_FEC.Value,
                            DESTINO = s.EXCARCELACION.ID_TIPO_EX.Value == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any()?
                            s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL.Value==Parametro.ID_HOSPITAL_OTROS?s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                            :s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR:""
                            //DESTINO = s.EXCARCELACION.ID_TIPO_EX.Value == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.ID_HOSPITAL.Value == Parametro.ID_HOSPITAL_OTROS?
                            //s.HOSPITAL_OTRO : s.HOSPITAL.DESCR
                        }));
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                    }
                    else
                        AgregarImputado();
                    break;
                case "quitar_interno":
                    if (SelectedIngresoSeleccionado != null)
                    {                        
                        if (HabilitaImputados)
                        {
                            QuitarImputado();
                        }
                        else
                        {
                            var res = await new Dialogos().ConfirmarEliminar("Mensaje", "¿Esta seguro de cancelar el traslado del interno " + ObtieneNombre(SelectedIngresoSeleccionado.IMPUTADO) + "?");
                            if (res == 1)
                                if (LstIngresosSeleccionados.Count > 1)
                                    QuitarImputado();
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación!", "Un traslado debe de tener por lo menos un traslado individual activo. Si desea cancelar el traslado por favor usar la opción de eliminar");
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar a un imputado.");
                    break;
                case "limpiar_menu":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                            return;
                    }
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new TrasladoMasivoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.TrasladoMasivoViewModel();
                    break;
                case "guardar_menu":
                    if (base.HasErrors)
                        new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                    else
                    {
                        Guardar();
                        ConfiguraPermisos();
                    }
                    break;
                case "menu_eliminar":
                    if(SelectedTraslado!=null)
                    {
                        if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de cancelar el traslado?") != 1)
                            return;
                        Eliminar();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", "No ha seleccionado ningun traslado");
                    break;
                case "buscar_imputado":
                    LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                    _anioBuscarImputado = anioBuscarImputado;
                    _apellidoMaternoBuscarImputado = ApellidoMaternoBuscarImputado;
                    _apellidoPaternoBuscarImputado = ApellidoPaternoBuscarImputado;
                    _folioBuscarImputado = FolioBuscarImputado;
                    _nombreBuscarImputado = NombreBuscarImputado;
                    await ObtenerTodosActivos();
                    break;
                case "cerrar_visualizador_documentos":
                    cerrarVisualizadorDocumentos();
                    break;
                case "filtro_traslados":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                    var _tipo_traslado_id = string.Empty;
                    if (SelectedTipo_Traslado != null)
                        _tipo_traslado_id = SelectedTipo_Traslado.ID;
                    await BuscarTraslado(GlobalVar.gCentro, new List<string>() { "PR", "EP" }, FechaBuscarTraslado, _tipo_traslado_id,(short?)AnioBuscarTraslado,FolioBuscarTraslado,NombreBuscarTraslado,ApellidoPaternoBuscarTraslado,ApellidoMaternoBuscarTraslado);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                    break;
                case "cancelar_buscar_traslados":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_TRASLADOS);
                    SetValidacionesTraslados();
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
                        DTCentroDestino = SelectedTraslado.CENTRO_DESTINO;
                        DTNoOficio = SelectedTraslado.OFICIO_AUTORIZACION;
                        //id_autoridad_traslado = SelectedTraslado.AUTORIZA_TRASLADO.Value;
                        Autoridad_Traslado = selectedTraslado.AUTORIZA_TRASLADO;
                        DENoOficio = SelectedTraslado.OFICIO_SALIDA;
                        AutorizaSalida = AutoridadesSalida.FirstOrDefault(w => w == SelectedTraslado.AUTORIZA_SALIDA);
                        HabilitaImputados = false;
                        
                        var id_motivo_traslado = SelectedTraslado.ID_MOTIVO_SALIDA;
                        if (id_motivo_traslado.HasValue)
                            MotivoSalida = new cTrasladoMotivoSalida().Obtener(id_motivo_traslado.Value).DESCR;
                        LstIngresosSeleccionados = new ObservableCollection<INGRESO>(SelectedTraslado.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "EP" || w.ID_ESTATUS == "AC").Select(s => s.INGRESO));
                        SeleccionadosIngresos = SelectedTraslado.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "EP" || w.ID_ESTATUS == "AC").Select(s => s.INGRESO).ToList();
                        SetValidacionesTraslados();
                        StaticSourcesViewModel.SourceChanged = false;
                        if (permisos.Any(w=>w.EDITAR==1))
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
                    SelectedTraslado = null;
                    HabilitaImputados = true;
                    CancelarMenuEnabled = false;
                    await ObtenerTodosActivos();
                    SetValidacionesTraslados();
                    ConfiguraPermisos();
                    break;
                case "reporte_menu":
                    if (selectedTraslado==null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Para imprimir el reporte debe de seleccionar un traslado");
                        return;
                    }
                    GeneraReporte();
                    break;
                case "cancelar_estatus_excarcelacion":
                    if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                            "¿Desea cancelar las excarcelaciones del imputado?") != 1)
                            return;
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Cancelando excarcelaciones", () => {
                        new cExcarcelacion().CancelarExcarcelacionesPorImputado(selectedIngreso.Ingreso.ID_ANIO, selectedIngreso.Ingreso.ID_CENTRO, selectedIngreso.Ingreso.ID_IMPUTADO,
                            selectedIngreso.Ingreso.ID_INGRESO, Fechas.GetFechaDateServer, (short)enumMensajeTipo.CANCELACION_EXCARCELACION,(short)Parametro.ID_HOSPITAL_OTROS);
                        return true;
                    }))
                    {
                        new Dialogos().ConfirmacionDialogo("EXITO!","LAS EXCARCELACIONES PROGRAMADAS Y AUTORIZADAS FUERON CANCELADAS CON EXITO!");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                        AgregarImputado(); 
                    }

                    break;
                case "salir_cancelar_excarcelacion":
                    new Dialogos().ConfirmacionDialogo("Validación!", "No se puede agregar al imputado al traslado si tiene excarcelaciones PROGRAMADAS o AUTORIZADAS!");
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACIONES_CANCELAR);
                    break;
                
               
            }
        }

         private void QuitarImputado()
        {
            try
            {
                var s = SelectedIngresoSeleccionado;
                if (LstIngresosSeleccionados.Remove(SelectedIngresoSeleccionado))
                {
                    var encontrado = LstIngresos.FirstOrDefault(w => w.Ingreso.ID_IMPUTADO == s.ID_IMPUTADO && w.Ingreso.ID_ANIO == s.ID_ANIO && w.Ingreso.ID_CENTRO==s.ID_CENTRO);
                    if (encontrado != null)
                        encontrado.Seleccionado = false;
                    SeleccionadosIngresos.Remove(SeleccionadosIngresos.FirstOrDefault(w => w.ID_ANIO == s.ID_ANIO && w.ID_IMPUTADO == s.ID_IMPUTADO && w.ID_INGRESO == s.ID_INGRESO && w.ID_CENTRO == s.ID_CENTRO));
                    StaticSourcesViewModel.SourceChanged = true;
                }
                var temp = LstIngresos;
                LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                LstIngresos.InsertRange(temp);
                SelectedIngresoSeleccionado = null;

            }
             catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el imputado", ex);
            }
        }

         private async  void cerrarVisualizadorDocumentos()
        {
            try
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VISUALIZAR_DOCUMENTOS);
                var amparo = new cAmparoIndirectoTipos().Obtener(SelectedIngreso.Ingreso.ID_CENTRO, SelectedIngreso.Ingreso.ID_ANIO, SelectedIngreso.Ingreso.ID_IMPUTADO, SelectedIngreso.Ingreso.ID_INGRESO, null, 5, 2).FirstOrDefault();
                var res = await new Dialogos().ConfirmacionTresBotonesDinamico("Mensaje", "El interno " + ObtieneNombre(SelectedIngreso.Ingreso.IMPUTADO) + " tiene un amparo para traslado", "Agregar", Convert.ToInt32(Tipo_Respuesta.Agregar),
                                "Ver amparo", Convert.ToInt32(Tipo_Respuesta.Mostrar_Documento), string.Empty, Convert.ToInt32(Tipo_Respuesta.Cancelar));
                switch ((Tipo_Respuesta)res)
                {
                    case Tipo_Respuesta.Agregar:
                        SelectedIngreso.Seleccionado = true;
                        LstIngresosSeleccionados.Add(SelectedIngreso.Ingreso);
                        SeleccionadosIngresos.Add(SelectedIngreso.Ingreso);
                        var temp = LstIngresos;
                        LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                        LstIngresos.InsertRange(temp);
                        SelectedIngreso = null;
                        break;
                    case Tipo_Respuesta.Cancelar:
                        SelectedIngreso = null;
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
             catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el imputado", ex);
            }
            
        }

         private async void AgregarImputado()
        {
             try
             {
                 if (SelectedIngreso != null)
                 {
                     if (LstIngresosSeleccionados == null)
                     {
                         LstIngresosSeleccionados = new ObservableCollection<INGRESO>();
                         SeleccionadosIngresos = new List<INGRESO>();
                     }
                     if (!LstIngresosSeleccionados.Any(a => a.ID_ANIO == SelectedIngreso.Ingreso.ID_ANIO && a.ID_CENTRO == SelectedIngreso.Ingreso.ID_CENTRO && a.ID_IMPUTADO == SelectedIngreso.Ingreso.ID_IMPUTADO && a.ID_INGRESO == SelectedIngreso.Ingreso.ID_INGRESO))
                     {
                         var amparo = new cAmparoIndirectoTipos().Obtener(SelectedIngreso.Ingreso.ID_CENTRO, SelectedIngreso.Ingreso.ID_ANIO, SelectedIngreso.Ingreso.ID_IMPUTADO, SelectedIngreso.Ingreso.ID_INGRESO, null, 5, 2).FirstOrDefault();
                         if (amparo == null)
                         {
                             SelectedIngreso.Seleccionado = true;
                             LstIngresosSeleccionados.Add(SelectedIngreso.Ingreso);
                             SeleccionadosIngresos.Add(SelectedIngreso.Ingreso);
                             var temp = LstIngresos;
                             LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                             LstIngresos.InsertRange(temp);
                             SelectedIngreso = null;
                             StaticSourcesViewModel.SourceChanged = true;
                         }
                         else
                         {
                             var res = await new Dialogos().ConfirmacionTresBotonesDinamico("Mensaje", "El interno " + ObtieneNombre(SelectedIngreso.Ingreso.IMPUTADO) + " tiene un amparo para traslado", "Agregar", Convert.ToInt32(Tipo_Respuesta.Agregar),
                                 "Ver amparo", Convert.ToInt32(Tipo_Respuesta.Mostrar_Documento), string.Empty, Convert.ToInt32(Tipo_Respuesta.Cancelar));
                             switch ((Tipo_Respuesta)res)
                             {
                                 case Tipo_Respuesta.Agregar:
                                     SelectedIngreso.Seleccionado = true;
                                     LstIngresosSeleccionados.Add(SelectedIngreso.Ingreso);
                                     SeleccionadosIngresos.Add(SelectedIngreso.Ingreso);
                                     var temp = LstIngresos;
                                     LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                                     LstIngresos.InsertRange(temp);
                                     SelectedIngreso = null;
                                     StaticSourcesViewModel.SourceChanged = true;
                                     break;
                                 case Tipo_Respuesta.Cancelar:
                                     SelectedIngreso = null;
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
                 }
                 else
                     new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar a un interno.");
             }
             catch(Exception ex)
             {
                 StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el imputado", ex);
             }
            
        }

        private string ObtieneNombre(IMPUTADO imp)
        {
            return string.Format("{0} {1} {2}", !string.IsNullOrEmpty(imp.NOMBRE) ? imp.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(imp.PATERNO) ? imp.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(imp.MATERNO) ? imp.MATERNO.Trim() : string.Empty);
        }

        private void MarcarImputados()
        {
            if (SeleccionadosIngresos != null)
                foreach (var item in SeleccionadosIngresos)
                {
                    var encontrado = LstIngresos.FirstOrDefault(w => w.Ingreso.ID_ANIO == item.ID_ANIO && w.Ingreso.ID_IMPUTADO == item.ID_IMPUTADO);
                    if (encontrado != null)
                        encontrado.Seleccionado = true;
                }
                    
                    
        }

        private async void TrasladoMasivoLoad(TrasladoMasivoView Window = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas).ContinueWith((prevTask) => {
                    Limpiar();
                });
                LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
                await ObtenerTodosActivos();
                SetValidacionesTraslados();
                PDFViewer = PopUpsViewModels.MainWindow.MostrarPDFView.pdfViewer;
                StaticSourcesViewModel.SourceChanged = false;
                ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el traslado", ex);
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
                        if (LstCentro == null)
                        {
                            LstCentro = new ObservableCollection<CENTRO>(new cCentro().ObtenerTodos());
                            LstCentro.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
                        }
                        id_motivo_traslado = Convert.ToInt16(new cParametro().Seleccionar("MOTIVO_TRASLADO", 0).First().VALOR_NUM);
                        if (id_motivo_traslado.HasValue)
                            MotivoSalida = new cTrasladoMotivoSalida().Obtener(id_motivo_traslado.Value).DESCR;
                        //id_autoridad_traslado = 
                        Autoridad_Traslado = new cParametro().Seleccionar("AUTORIDAD_TRASLADO", 0).First().VALOR;
                        AutoridadesSalida.Add(new cCentro().Obtener(GlobalVar.gCentro).First().DIRECTOR);
                        AutoridadesSalida.Add(new cParametro().Seleccionar("DIR_JURIDICO_CENTRO", GlobalVar.gCentro).First().VALOR);
                        AutoridadesSalida.Add(new cParametro().Seleccionar("SUBDIR_JURIDICO_CENTRO", GlobalVar.gCentro).First().VALOR);
                        AutoridadesSalida.Add("SELECCIONE");
                        FechaServer = Fechas.GetFechaDateServer;
                        Tipos_Traslado = new List<Tipo_Traslado> { new Tipo_Traslado("LO", "LOCAL"), new Tipo_Traslado("LF", "FORANEO"), new Tipo_Traslado("T", "TODOS") };
                    }
                    catch(Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                    }
                }));
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
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
        #endregion

        #region Ingresos
        private async Task ObtenerTodosActivos(int _Pag=1) 
        {
            try
            {
                var ingresos = await SegmentarIngresoBusqueda(_Pag);
                var temp = new ObservableCollection<cTrasladoIngreso>();
                foreach (var x in ingresos)
                {
                    if (SeleccionadosIngresos == null)
                        temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = false });
                    else
                    {
                        if (SeleccionadosIngresos.FirstOrDefault(w => w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_ANIO == x.ID_ANIO) == null)
                            temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = false });
                        else
                            temp.Add(new cTrasladoIngreso() { Ingreso = x, Seleccionado = true });
                    }
                }
                LstIngresos.InsertRange(temp);
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
            }
            
        }

        private async Task<List<INGRESO>> SegmentarIngresoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() =>
                             new ObservableCollection<INGRESO>(new cIngreso().ObtenerIngresosActivosFiltrados(estatus_inactivos,_anioBuscarImputado,_folioBuscarImputado,_nombreBuscarImputado,
                                 _apellidoPaternoBuscarImputado,_apellidoMaternoBuscarImputado,GlobalVar.gCentro, _Pag)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoIngresos = true;
                }
                else
                    SeguirCargandoIngresos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<INGRESO>();
            }
        }

        #endregion

        #region Metodos Traslado
        private void Limpiar()
        {
            //Datos Traslado
            DTFecha = null;
            DTMotivo = DTCentroDestino = -1;
            DTJustificacion = DTNoOficio = string.Empty;
            DENoOficio = string.Empty;
            AutorizaSalida = AutoridadesSalida.First(w => w == "SELECCIONE").ToString();
            LstIngresos = new RangeEnabledObservableCollection<cTrasladoIngreso>();
            LstIngresosSeleccionados = null;
            SeleccionadosIngresos = null;
            ImagenIngreso = null;
            NombreBuscarImputado = string.Empty;
            ApellidoPaternoBuscarImputado = string.Empty;
            ApellidoMaternoBuscarImputado = string.Empty;
            AnioBuscarImputado = null;
            FolioBuscarImputado = null;
            StaticSourcesViewModel.SourceChanged = false;
        }


        private async void Guardar()
        {
            try
            {
                var traslado_detalle = new List<TRASLADO_DETALLE>();
                if (SelectedTraslado==null)
                    foreach(var item in LstIngresosSeleccionados)
                    {
                        traslado_detalle.Add(new TRASLADO_DETALLE {
                            ID_ANIO=item.ID_ANIO,
                            ID_CENTRO=item.ID_CENTRO,
                            ID_CENTRO_TRASLADO=GlobalVar.gCentro,
                            ID_ESTATUS="PR",
                            ID_IMPUTADO=item.ID_IMPUTADO,
                            ID_INGRESO=item.ID_INGRESO,
                            ID_ESTATUS_ADMINISTRATIVO=item.ID_ESTATUS_ADMINISTRATIVO                            
                        });
                    }
                else
                {
                    foreach(var item in SelectedTraslado.TRASLADO_DETALLE)
                    {
                        traslado_detalle.Add(new TRASLADO_DETALLE
                        {
                            ID_ANIO = item.ID_ANIO,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_CENTRO_TRASLADO = GlobalVar.gCentro,
                            ID_ESTATUS = LstIngresosSeleccionados.FirstOrDefault(w=>w.ID_IMPUTADO==item.ID_IMPUTADO && w.ID_ANIO==item.ID_ANIO
                            && w.ID_CENTRO==item.ID_CENTRO && w.ID_INGRESO == item.ID_INGRESO)==null?"CA":item.ID_ESTATUS,
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_INGRESO = item.ID_INGRESO,
                            ID_ESTATUS_ADMINISTRATIVO = item.ID_ESTATUS_ADMINISTRATIVO,
                            ID_TRASLADO=item.ID_TRASLADO 
                        });
                    }
                }
                var _traslado = new TRASLADO {
                    AUTORIZA_SALIDA=AutorizaSalida,
                    AUTORIZA_TRASLADO=Autoridad_Traslado,
                    ID_CENTRO=GlobalVar.gCentro,
                    CENTRO_ORIGEN=GlobalVar.gCentro,
                    ORIGEN_TIPO="L",  //como es traslado originado en los centros estatales el tipo es L "LOCAL"
                    ID_ESTATUS = SelectedTraslado == null ? "PR" : SelectedTraslado.ID_ESTATUS,
                    ID_MOTIVO=DTMotivo,
                    ID_MOTIVO_SALIDA = id_motivo_traslado.Value,
                    JUSTIFICACION=DTJustificacion,
                    OFICIO_AUTORIZACION=DTNoOficio,
                    OFICIO_SALIDA=DENoOficio,
                    CENTRO_DESTINO = DTCentroDestino.Value,
                    TRASLADO_DETALLE=traslado_detalle,
                    TRASLADO_FEC=DTFecha.Value,
                    ID_TRASLADO = SelectedTraslado == null ? 0 : SelectedTraslado.ID_TRASLADO
                };
                if (SelectedTraslado == null)
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Insertando traslado", () =>
                    {
                        new cTraslado().Insertar(_traslado, (short)enumMensajeTipo.CALENDARIZACION_TRASLADO,_FechaServer);
                        return true;
                    }))
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido registrado");
                }
                else
                {
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando traslado", () =>
                    {
                        new cTraslado().Actualizar(_traslado,(short)enumMensajeTipo.CALENDARIZACION_TRASLADO, _FechaServer);
                        return true;
                    }))
                        new Dialogos().ConfirmacionDialogo("EXITO!", "El traslado ha sido registrado");
                }
                HabilitaImputados = true;
                CancelarMenuEnabled = false;
                Limpiar();
                selectedTraslado = null;
                await ObtenerTodosActivos();
            }
            catch(Exception ex)
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
                    HabilitaImputados = true;
                    CancelarMenuEnabled = false;
                    Limpiar();
                    await ObtenerTodosActivos();
                    SelectedTraslado = null;
                }
                    
                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salvar el traslado.", ex);
            }
        }
        #endregion

        #region Busqueda Traslados
         private void LimpiarBusquedaTraslados()
        {
            SelectedTipo_Traslado = Tipos_Traslado.First(w => w.ID == "LO");
            AnioBuscarTraslado = null;
            FolioBuscarTraslado = null;
            NombreBuscarTraslado = string.Empty;
            ApellidoPaternoBuscarTraslado = string.Empty;
            ApellidoMaternoBuscarTraslado = string.Empty;
        }

         private async Task BuscarTraslado(short? centro = null,List<string> estatus=null, DateTime? fecha=null,  string tipo_traslado_local="" ,short? anio = null, int? imputado = null, string nombre="", string paterno="", string materno="")
         {
             try
             {
                 if (tipo_traslado_local == "T")
                     tipo_traslado_local = string.Empty;
                 BusquedaTraslado = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TRASLADO>>(() => new ObservableCollection<TRASLADO>(new cTraslado().ObtenerTodos(centro, estatus, fecha, "L", tipo_traslado_local, anio, imputado, nombre, paterno, materno)));
             }
             catch(Exception ex)
             {
                 StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salvar el traslado.", ex);
             }
             
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
                        CERESO_DESTINO = s.TRASLADO.CENTRO.DESCR,
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salvar el traslado.", ex);
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
    }

    enum Tipo_Respuesta
    {
        Agregar,
        Cancelar,
        Mostrar_Documento
    }
}
