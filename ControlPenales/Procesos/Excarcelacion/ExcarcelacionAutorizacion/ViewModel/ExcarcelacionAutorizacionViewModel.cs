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

namespace ControlPenales
{
    partial class ExcarcelacionAutorizacionViewModel : ValidationViewModelBase
    {
        private short?[] estatus_inactivos_ingreso = null;
        #region Generales
        public async void ExcarcelacionAutorizacionOnLoad(ExcarcelacionAutorizacionView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    estatus_inactivos_ingreso = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    ConfiguraPermisos();
                    estatus_habilitados = Parametro.AUT_ESTATUS_EXCARC_DIRECCION.Split(',');
                    ObtenerExcarcelacionTiposBuscar(true);
                    ObtenerExcarcelacionEstatusBuscar(true);
                    fechaInicialBuscarExc = _FechaServer;
                    OnPropertyChanged("FechaInicialBuscarExc");
                    if (menuBuscarEnabled || menuGuardarEnabled)
                        ObtenerExcarcelaciones(GlobalVar.gCentro,estatus_habilitados.ToList(),null,null,string.Empty,string.Empty,string.Empty,null,fechaInicialBuscarExc,fechaFinalBuscarExc,true);
                }).ContinueWith((prevTask) =>
                {
                    Limpiar();
                });
                setValidacionesExcarcelacion_Actualizacion();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el modulo", ex);
            }
        }

        public void Limpiar()
        {
            #region Preseleccionar Listas
            if (excarcelacion_TiposBuscar != null && excarcelacion_TiposBuscar.FirstOrDefault(w => w.ID_TIPO_EX == 0) != null)
            {
                selectedExc_TipoBuscarValue = excarcelacion_TiposBuscar.First(w => w.ID_TIPO_EX == 0).ID_TIPO_EX;
                OnPropertyChanged("SelectedExc_TipoBuscarValue");
            }
            if (excarcelacion_EstatusBuscar != null && excarcelacion_EstatusBuscar.FirstOrDefault(w=>w.ID_ESTATUS=="0")!=null)
            {
                selectedExcarcelacion_EstatusBuscarValue = excarcelacion_EstatusBuscar.First(w => w.ID_ESTATUS == "0").ID_ESTATUS;
                OnPropertyChanged("SelectedExcarcelacion_EstatusBuscarValue");
            }
            #endregion
            
            StaticSourcesViewModel.SourceChanged = false;
        }

        private void ObtenerExcarcelaciones(short centroOrigen, List<string> estatus = null, short? anio = null,
            int? folio = null, string nombre = "", string paterno = "", string materno = "", short? id_tipo_exc = null, DateTime? fecha_inicio = null, DateTime? fecha_final = null, bool isExceptionManaged = false)
        {
            try
            {
                listaExcarcelaciones = new ObservableCollection<EXCARCELACION>(new cExcarcelacion().Seleccionar(centroOrigen,estatus,
                    anio,folio,nombre.Trim(),paterno.Trim(),materno.Trim(),id_tipo_exc,fecha_inicio,fecha_final,"D"));
                OnPropertyChanged("ListaExcarcelaciones");
            }
            catch (Exception ex)
            {
                if (!isExceptionManaged)
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las excarcelaciones", ex);
                else
                    throw ex;

            }
        }

        private async void ClickSwitch (object parametro)
        {
            try
            {
                if (parametro!=null)
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
                        case "buscar_excarcelacion":
                            if (!IsFechaIniValida)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "La fecha de inicio de la busqueda tiene que ser menor o igual a la fecha final de la busqueda!");
                                return;
                            }
                            BuscarExcarcelaciones(GlobalVar.gCentro, selectedExcarcelacion_EstatusBuscarValue == "0" ? estatus_habilitados.ToList() : new List<string> { SelectedExcarcelacion_EstatusBuscarValue },
                                anioBuscarExc, folioBuscarExc, nombreBuscarExc, apellidoPaternoBuscarExc,
                                apellidoMaternoBuscarExc, selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, fechaInicialBuscarExc, fechaFinalBuscarExc);
                            break;
                        case "visualizar_documento_excarcelacion":
                            if (SelectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA)
                            {

                                if (SelectedExcarcelacionDestino == null)
                                {
                                    await new Dialogos().ConfirmacionDialogoReturn("Validación", "Se debe de seleccionar un destino de excarcelación");
                                    return;
                                }
                                MuestraOficioSolicitudExcarcelacion(SelectedExcarcelacionDestino.ID_INTERC);
                            }
                            else
                                MostrarDocumento(_documento,_formato_documento.Value);
                            break;
                        case "autorizar_menu":
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de autorizar la excarcelación?, se autorizaran todos los destinos") != 1)
                                return;
                            var _id_otro_hospital = Parametro.ID_HOSPITAL_OTROS;
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando excarcelación", () =>
                            {
                                var excarcelacion_destinos = new List<EXCARCELACION_DESTINO>();
                                foreach (var item in selectedExcarcelacion.EXCARCELACION_DESTINO)
                                    if (item.CANCELADO_TIPO!="J" || string.IsNullOrWhiteSpace(item.CANCELADO_TIPO))
                                        excarcelacion_destinos.Add(new EXCARCELACION_DESTINO
                                        {
                                            CANCELADO_TIPO=null,
                                            CAN_ID_MOTIVO = null,
                                            CANCELADO_OBSERVA = null,
                                            CAU_ID_ANIO = item.CAU_ID_ANIO,
                                            CAU_ID_CAUSA_PENAL = item.CAU_ID_CAUSA_PENAL,
                                            CAU_ID_CENTRO = item.CAU_ID_CENTRO,
                                            CAU_ID_IMPUTADO = item.CAU_ID_IMPUTADO,
                                            CAU_ID_INGRESO = item.CAU_ID_INGRESO,
                                            CAUSA_PENAL_TEXTO = item.CAUSA_PENAL_TEXTO,
                                            DOCUMENTO = item.DOCUMENTO,
                                            FOLIO_DOC = item.FOLIO_DOC,
                                            //HOSPITAL_OTRO = item.HOSPITAL_OTRO,
                                            ID_ANIO = item.ID_ANIO,
                                            ID_CENTRO = item.ID_CENTRO,
                                            ID_CONSEC = item.ID_CONSEC,
                                            ID_DESTINO = item.ID_DESTINO,
                                            ID_ESTATUS = "AU",
                                            ID_FORMATO = item.ID_FORMATO,
                                           // ID_HOSPITAL = item.ID_HOSPITAL,
                                            ID_IMPUTADO = item.ID_IMPUTADO,
                                            ID_INGRESO = item.ID_INGRESO,
                                            ID_JUZGADO = item.ID_JUZGADO,
                                            ID_TIPO_DOC = item.ID_TIPO_DOC,
                                            ID_INTERSOL=item.ID_INTERSOL,
                                            ID_CENTRO_UBI=item.ID_CENTRO_UBI
                                        });
                                new cExcarcelacion().Autorizar(new EXCARCELACION
                                {
                                    ID_CENTRO_UBI=selectedExcarcelacion.ID_CENTRO_UBI,
                                    CANCELADO_TIPO = null,
                                    CERTIFICADO_MEDICO = selectedExcarcelacion.CERTIFICADO_MEDICO,
                                    ID_ANIO = selectedExcarcelacion.ID_ANIO,
                                    ID_CENTRO = selectedExcarcelacion.ID_CENTRO,
                                    ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                                    ID_ESTATUS = "AU",
                                    ID_IMPUTADO = selectedExcarcelacion.ID_IMPUTADO,
                                    ID_INGRESO = selectedExcarcelacion.ID_INGRESO,
                                    ID_TIPO_EX = selectedExcarcelacion.ID_TIPO_EX,
                                    ID_USUARIO = selectedExcarcelacion.ID_USUARIO,
                                    OBSERVACION = selectedExcarcelacion.OBSERVACION,
                                    PROGRAMADO_FEC = selectedExcarcelacion.PROGRAMADO_FEC,
                                    REGISTRO_FEC = selectedExcarcelacion.REGISTRO_FEC,
                                    RETORNO_FEC = selectedExcarcelacion.RETORNO_FEC,
                                    SALIDA_FEC = selectedExcarcelacion.SALIDA_FEC,
                                    EXCARCELACION_DESTINO = excarcelacion_destinos
                                }, _FechaServer,selectedExcarcelacion.ID_TIPO_EX==(short)enumExcarcelacionTipo.JURIDICA?(int)enumMensajeTipo.AVISO_AUTORIZACION_DIRECCION_EXCARCELACION:(int)enumMensajeTipo.AVISO_AUTORIZACION_DIRECCION_EXCARCELACION_MEDICA,
                                (int)enumMensajeTipo.AVISO_EXCARCELACION_AREA_MEDICA,(short) _id_otro_hospital, GlobalVar.gCentro );
                                return true;
                            }))
                            {
                                await BuscarExcarcelaciones(GlobalVar.gCentro, selectedExcarcelacion_EstatusBuscarValue == "0" ? estatus_habilitados.ToList() : new List<string> { SelectedExcarcelacion_EstatusBuscarValue },
                                    anioBuscarExc, folioBuscarExc, nombreBuscarExc, apellidoPaternoBuscarExc,
                                apellidoMaternoBuscarExc, selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, fechaInicialBuscarExc, fechaFinalBuscarExc);
                                new Dialogos().ConfirmacionDialogo("EXITO!", "La excarcelación ha sido actualizada");
                            }
                            break;
                        case "cancelar_menu":
                            if (base.HasErrors)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                return;
                            }
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de cancelar la excarcelación?,se cancelaran todos los destinos") != 1)
                                return;
                            _modo_cancelacion = MODO_CANCELACION.GLOBAL;
                            MotivosCancelaciones();
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
                            Cancelacion_Excarcelacion();
                            break;
                        case "salir_menu":
                            SalirMenu();
                            break;
                        case "autorizar_destino_excarcelacion":
                            if (SelectedExcarcelacionDestino == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Se debe seleccionar un destino de excarcelación");
                                return;
                            }
                            var id_hospital_otros = (short)Parametro.ID_HOSPITAL_OTROS;
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando destino de excarcelación", () =>
                            {
                                new cExcarcelacion_Destino().Actualizar(new EXCARCELACION_DESTINO
                                {
                                    CANCELADO_TIPO = null,
                                    CAN_ID_MOTIVO = null,
                                    CANCELADO_OBSERVA = string.Empty,
                                    CAU_ID_ANIO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_ANIO : null,
                                    CAU_ID_CENTRO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_CENTRO : null,
                                    CAU_ID_IMPUTADO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (int?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_IMPUTADO : null,
                                    CAU_ID_INGRESO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_INGRESO : null,
                                    CAU_ID_CAUSA_PENAL = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_CAUSA_PENAL : null,
                                    DOCUMENTO = selectedExcarcelacionDestino.DOCUMENTO,
                                    FOLIO_DOC = selectedExcarcelacionDestino.FOLIO,
                                    //HOSPITAL_OTRO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA && selectedExcarcelacionDestino.ID_DESTINO == (short)id_hospital_otros ? selectedExcarcelacionDestino.DESTINO : string.Empty,
                                    ID_ANIO = selectedExcarcelacion.ID_ANIO,
                                    ID_CENTRO = selectedExcarcelacion.ID_CENTRO,
                                    ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                                    ID_ESTATUS = "AU",
                                    ID_FORMATO = selectedExcarcelacionDestino.FORMATO_DOCUMENTO,
                                    //ID_HOSPITAL = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA ? selectedExcarcelacionDestino.ID_DESTINO : null,
                                    ID_IMPUTADO = selectedExcarcelacion.ID_IMPUTADO,
                                    ID_INGRESO = selectedExcarcelacion.ID_INGRESO,
                                    ID_JUZGADO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? selectedExcarcelacionDestino.ID_DESTINO : null,
                                    ID_TIPO_DOC = selectedExcarcelacionDestino.TIPO_DOCUMENTO,
                                    ID_DESTINO = selectedExcarcelacionDestino.ID_CONSEC.Value,
                                    CAUSA_PENAL_TEXTO = selectedExcarcelacionDestino.CAUSA_PENAL == null && !string.IsNullOrWhiteSpace(selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO) ? selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO : string.Empty,
                                    ID_INTERSOL=selectedExcarcelacion.ID_TIPO_EX== (short)enumExcarcelacionTipo.MEDICA? selectedExcarcelacionDestino.ID_INTERC:null,
                                    ID_CENTRO_UBI=selectedExcarcelacion.ID_TIPO_EX==(short)enumExcarcelacionTipo.MEDICA? selectedExcarcelacionDestino.ID_CENTRO_UBI:null,
                                }, (int)enumMensajeTipo.AVISO_AUTORIZACION_DIRECCION_EXCARCELACION, (int)enumMensajeTipo.AVISO_EXCARCELACION_AREA_MEDICA, id_hospital_otros, _FechaServer);
                                return true;
                            }))
                            {
                                var _temp = selectedExcarcelacion;
                                await BuscarExcarcelaciones(GlobalVar.gCentro, selectedExcarcelacion_EstatusBuscarValue == "0" ? estatus_habilitados.ToList() : new List<string> { SelectedExcarcelacion_EstatusBuscarValue },
                                    anioBuscarExc, folioBuscarExc, nombreBuscarExc, apellidoPaternoBuscarExc,
                                apellidoMaternoBuscarExc, selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, fechaInicialBuscarExc, fechaFinalBuscarExc);
                                selectedExcarcelacion = ListaExcarcelaciones.FirstOrDefault(w => w.ID_ANIO == _temp.ID_ANIO && w.ID_CENTRO == _temp.ID_CENTRO
                                    && w.ID_CONSEC == _temp.ID_CONSEC && w.ID_IMPUTADO == _temp.ID_IMPUTADO && w.ID_INGRESO == _temp.ID_INGRESO);
                                OnPropertyChanged("SelectedExcarcelacion");
                                if (selectedExcarcelacion != null)
                                {
                                    listaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(selectedExcarcelacion.EXCARCELACION_DESTINO.Select(
                                        s => new CT_EXCARCELACION_DESTINO
                                        {
                                            ID_CONSEC = s.ID_DESTINO,
                                            CAUSA_PENAL = s.CAUSA_PENAL,
                                            DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? 
                                            s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL==id_hospital_otros?s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL_OTRO: !string.IsNullOrEmpty(s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                            //DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.ID_HOSPITAL.HasValue && s.ID_HOSPITAL.Value == id_hospital_otros ?
                                            //s.HOSPITAL_OTRO : s.HOSPITAL.DESCR,
                                            DOCUMENTO = s.DOCUMENTO,
                                            ESTATUS = s.ID_ESTATUS,
                                            FOLIO = s.FOLIO_DOC,
                                            FORMATO_DOCUMENTO = s.ID_FORMATO,
                                            ID_DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (short?)s.ID_JUZGADO : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? (short?)s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL : null : null : null,
                                            TIPO_DOCUMENTO = s.ID_TIPO_DOC,
                                            CANCELACION_OBSERVACION = s.CANCELADO_OBSERVA,
                                            ID_CANCELACION_MOTIVO = s.CAN_ID_MOTIVO,
                                            CERTIFICADO_MEDICO = null,
                                            CERTIFICADO_MEDICO_ENABLED = false,
                                            ID_INTERC=s.ID_INTERSOL,
                                            ID_CENTRO_UBI=s.ID_CENTRO_UBI
                                        }
                                        ));
                                    OnPropertyChanged("ListaExcarcelacionDestinos");
                                    new Dialogos().ConfirmacionDialogo("EXITO!", "El destino de la excarcelación ha sido actualizado");
                                }
                                else
                                {
                                    ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>();
                                    IsExcarcelacionDestinosVisible = Visibility.Collapsed;
                                }

                            }
                            break;
                        case "cancelar_destino_excarcelacion":
                            if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                "¿Esta seguro de cancelar el destino de la excarcelación?") != 1)
                                return;
                            _modo_cancelacion = MODO_CANCELACION.INDIVIDUAL;
                            MotivosCancelaciones();
                            break;
                        case "limpiar_menu":
                            if (StaticSourcesViewModel.SourceChanged)
                            {
                                if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                                    "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                                    return;
                            }
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ExcarcelacionAutorizacionView();
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ExcarcelacionAutorizacionViewModel();
                            break;

                    }
                }
                    
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error", ex);
            }
        }

        public static void SalirMenu()
        {
            try
            {
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

        private async Task  BuscarExcarcelaciones(short centroOrigen, List<string> estatus = null, short? anio = null,
            int? folio = null, string nombre = "", string paterno = "", string materno = "", short? id_tipo_exc = null,
            DateTime? fecha_inicio=null, DateTime? fecha_final=null)
        {
            
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                var _imputados = new cImputado().ObtenerTodosCentro(GlobalVar.gCentro, estatus_inactivos_ingreso,paterno, materno, nombre, anio, folio);
                if (_imputados != null & _imputados.Count() == 1)
                {
                    nombre = _imputados[0].NOMBRE;
                    paterno = _imputados[0].PATERNO;
                    materno = _imputados[0].MATERNO;
                    nombreBuscarExc = nombre;
                    RaisePropertyChanged("NombreBuscarExc");
                    apellidoPaternoBuscarExc = paterno;
                    RaisePropertyChanged("ApellidoPaternoBuscarExc");
                    apellidoMaternoBuscarExc = materno;
                    RaisePropertyChanged("ApellidoMaternoBuscarExc");
                }
                ObtenerExcarcelaciones(centroOrigen, estatus, anio, folio, nombre, paterno, materno, id_tipo_exc,fecha_inicio,fecha_final, true);

            });
        }

        private async void ModelChangedSwitch(object parametro)
        {
            try
            {
                if (parametro!=null)
                    switch(parametro.ToString())
                    {
                        case "cambio_excarcelacion_seleccionada":
                            if (selectedExcarcelacion!=null)
                            {
                                var _hospital_otros = Parametro.ID_HOSPITAL_OTROS;
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    listaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(selectedExcarcelacion.EXCARCELACION_DESTINO.
                                        Where(w=>w.CANCELADO_TIPO!="J")
                                        .Select(
                                        s => new CT_EXCARCELACION_DESTINO
                                        {
                                            ID_CONSEC = s.ID_DESTINO,
                                            CAUSA_PENAL = s.CAUSA_PENAL,
                                            DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ?
                                            s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL == _hospital_otros ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL_OTRO : !string.IsNullOrEmpty(s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                            DOCUMENTO = s.DOCUMENTO,
                                            ESTATUS = s.ID_ESTATUS,
                                            FOLIO = s.FOLIO_DOC,
                                            FORMATO_DOCUMENTO = s.ID_FORMATO,
                                            ID_DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (short?)s.ID_JUZGADO : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? (short?)s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL : null : null : null,
                                            TIPO_DOCUMENTO = s.ID_TIPO_DOC,
                                            CANCELACION_OBSERVACION = s.CANCELADO_OBSERVA,
                                            ID_CANCELACION_MOTIVO = s.CAN_ID_MOTIVO,
                                            CERTIFICADO_MEDICO = null,
                                            CERTIFICADO_MEDICO_ENABLED = false,
                                            ID_INTERC=s.ID_INTERSOL,
                                            ID_CENTRO_UBI=s.ID_CENTRO_UBI
                                        }
                                        ));
                                    OnPropertyChanged("ListaExcarcelacionDestinos");
                                }).ContinueWith((prevTask) =>
                                {
                                    IsExcarcelacionDestinosVisible = Visibility.Visible;
                                    HeaderDestinosExcarcelacion = "Destinos de la excarcelación del imputado " + selectedExcarcelacion.ID_ANIO + "/" + selectedExcarcelacion.ID_IMPUTADO;
                                });

                            }
                            else
                            {
                                IsExcarcelacionDestinosVisible = Visibility.Collapsed;
                                HeaderDestinosExcarcelacion = "Destinos de la excarcelación del imputado";
                            }
                            break;
                        case "cambio_fecha_inicio_busqueda":
                            if (fechaInicialBuscarExc.HasValue && fechaFinalBuscarExc.HasValue)
                                if (fechaFinalBuscarExc.Value < fechaInicialBuscarExc.Value)
                                    IsFechaIniValida = false;
                                else
                                    IsFechaIniValida = true;
                            else
                                IsFechaIniValida = true;
                            break;
                    }
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó", "Ocurrió un error",ex);
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
                    if (cancelacion_Motivos != null && cancelacion_Motivos.FirstOrDefault(w => w.ID_EXCARCELACION_CANCELA == 0) != null)
                    {
                        selectedCancelacion_MotivoValue = 0;
                        OnPropertyChanged("SelectedCancelacion_MotivoValue");
                    }
                    cancelacion_Observacion = string.Empty;
                    OnPropertyChanged("Cancelacion_Observacion");

            });
            setValidacionesExcarcelacion_Cancela_Motivo();
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
        }

        private async void Cancelacion_Excarcelacion()
        {
            var id_otro_hospital=Parametro.ID_HOSPITAL_OTROS;
            if (_modo_cancelacion==MODO_CANCELACION.GLOBAL)
            {
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando excarcelación", () =>
                {
                    var excarcelacion_destinos = new List<EXCARCELACION_DESTINO>();
                    foreach (var item in selectedExcarcelacion.EXCARCELACION_DESTINO)
                        if (item.CANCELADO_TIPO != "J" || string.IsNullOrWhiteSpace(item.CANCELADO_TIPO))
                            excarcelacion_destinos.Add(new EXCARCELACION_DESTINO
                            {
                                CANCELADO_TIPO = "D",
                                CAN_ID_MOTIVO = SelectedCancelacion_MotivoValue,
                                CANCELADO_OBSERVA = Cancelacion_Observacion,
                                CAU_ID_ANIO = item.CAU_ID_ANIO,
                                CAU_ID_CAUSA_PENAL = item.CAU_ID_CAUSA_PENAL,
                                CAU_ID_CENTRO = item.CAU_ID_CENTRO,
                                CAU_ID_IMPUTADO = item.CAU_ID_IMPUTADO,
                                CAU_ID_INGRESO = item.CAU_ID_INGRESO,
                                CAUSA_PENAL_TEXTO = item.CAUSA_PENAL_TEXTO,
                                DOCUMENTO = item.DOCUMENTO,
                                FOLIO_DOC = item.FOLIO_DOC,
                                //HOSPITAL_OTRO = item.HOSPITAL_OTRO,
                                ID_ANIO = item.ID_ANIO,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_DESTINO = item.ID_DESTINO,
                                ID_ESTATUS = "CA",
                                ID_FORMATO = item.ID_FORMATO,
                                //ID_HOSPITAL = item.ID_HOSPITAL,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO,
                                ID_JUZGADO = item.ID_JUZGADO,
                                ID_TIPO_DOC = item.ID_TIPO_DOC,
                                ID_INTERSOL=item.ID_INTERSOL
                            });
                    new cExcarcelacion().Actualizar(new EXCARCELACION
                    {
                        CANCELADO_TIPO = "D",
                        CERTIFICADO_MEDICO = selectedExcarcelacion.CERTIFICADO_MEDICO,
                        ID_ANIO = selectedExcarcelacion.ID_ANIO,
                        ID_CENTRO = selectedExcarcelacion.ID_CENTRO,
                        ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                        ID_ESTATUS = "CA",
                        ID_IMPUTADO = selectedExcarcelacion.ID_IMPUTADO,
                        ID_INGRESO = selectedExcarcelacion.ID_INGRESO,
                        ID_TIPO_EX = selectedExcarcelacion.ID_TIPO_EX,
                        ID_USUARIO = selectedExcarcelacion.ID_USUARIO,
                        OBSERVACION = selectedExcarcelacion.OBSERVACION,
                        PROGRAMADO_FEC = selectedExcarcelacion.PROGRAMADO_FEC,
                        REGISTRO_FEC = selectedExcarcelacion.REGISTRO_FEC,
                        RETORNO_FEC = selectedExcarcelacion.RETORNO_FEC,
                        SALIDA_FEC = selectedExcarcelacion.SALIDA_FEC,
                        EXCARCELACION_DESTINO = excarcelacion_destinos
                    },selectedExcarcelacion.ID_TIPO_EX==(short)enumExcarcelacionTipo.JURIDICA?(int)enumMensajeTipo.MODIFICACION_EXCARCELACION:(int)enumMensajeTipo.MODIFICACION_EXCARCELACION_MEDICA,
                    selectedExcarcelacion.ID_TIPO_EX==(short)enumExcarcelacionTipo.JURIDICA?(int)enumMensajeTipo.CANCELACION_EXCARCELACION:(int)enumMensajeTipo.CANCELACION_EXCARCELACION_MEDICA, (int)enumMensajeTipo.AVISO_CANCELACION_EXCARCELACION_AREA_MEDICA, _FechaServer,
                    id_otro_hospital, false, GlobalVar.gCentro);
                    return true;
                }))
                {
                    await BuscarExcarcelaciones(GlobalVar.gCentro, estatus_habilitados.ToList(), anioBuscarExc, folioBuscarExc, nombreBuscarExc, apellidoPaternoBuscarExc,
                                    apellidoMaternoBuscarExc, selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue,fechaInicialBuscarExc,fechaFinalBuscarExc);
                    new Dialogos().ConfirmacionDialogo("EXITO!", "La excarcelación ha sido actualizada");
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
                    setValidacionesExcarcelacion_Actualizacion();
                }
            }
            else if (_modo_cancelacion==MODO_CANCELACION.INDIVIDUAL)
            {
                if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando destino de excarcelación", () =>
                {
                    new cExcarcelacion_Destino().Actualizar(new EXCARCELACION_DESTINO
                    {
                        CANCELADO_TIPO = "D",
                        CAN_ID_MOTIVO = selectedCancelacion_MotivoValue,
                        CANCELADO_OBSERVA = cancelacion_Observacion,
                        CAU_ID_ANIO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_ANIO : null,
                        CAU_ID_CENTRO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_CENTRO : null,
                        CAU_ID_IMPUTADO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (int?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_IMPUTADO : null,
                        CAU_ID_INGRESO = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_INGRESO : null,
                        CAU_ID_CAUSA_PENAL = selectedExcarcelacionDestino.CAUSA_PENAL != null ? (short?)selectedExcarcelacionDestino.CAUSA_PENAL.ID_CAUSA_PENAL : null,
                        DOCUMENTO = selectedExcarcelacionDestino.DOCUMENTO,
                        FOLIO_DOC = selectedExcarcelacionDestino.FOLIO,
                        //HOSPITAL_OTRO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA && selectedExcarcelacionDestino.ID_DESTINO == (short)id_otro_hospital ? selectedExcarcelacionDestino.DESTINO : string.Empty,
                        ID_ANIO = selectedExcarcelacion.ID_ANIO,
                        ID_CENTRO = selectedExcarcelacion.ID_CENTRO,
                        ID_CONSEC = selectedExcarcelacion.ID_CONSEC,
                        ID_ESTATUS = "CA",
                        ID_FORMATO = selectedExcarcelacionDestino.FORMATO_DOCUMENTO,
                        //ID_HOSPITAL = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA ? selectedExcarcelacionDestino.ID_DESTINO : null,
                        ID_IMPUTADO = selectedExcarcelacion.ID_IMPUTADO,
                        ID_INGRESO = selectedExcarcelacion.ID_INGRESO,
                        ID_JUZGADO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? selectedExcarcelacionDestino.ID_DESTINO : null,
                        ID_TIPO_DOC = selectedExcarcelacionDestino.TIPO_DOCUMENTO,
                        ID_DESTINO = selectedExcarcelacionDestino.ID_CONSEC.Value,
                        CAUSA_PENAL_TEXTO = selectedExcarcelacionDestino.CAUSA_PENAL == null && !string.IsNullOrWhiteSpace(selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO) ? selectedExcarcelacionDestino.CAUSA_PENAL_TEXTO : string.Empty,
                        ID_INTERSOL=selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.MEDICA? selectedExcarcelacionDestino.ID_INTERC:null
                    },selectedExcarcelacion.ID_TIPO_EX==(short)enumExcarcelacionTipo.JURIDICA?(int)enumMensajeTipo.CANCELACION_EXCARCELACION:(int)enumMensajeTipo.CANCELACION_EXCARCELACION_MEDICA,(int)enumMensajeTipo.AVISO_CANCELACION_EXCARCELACION_AREA_MEDICA,(short)id_otro_hospital, _FechaServer);
                    return true;
                }))
                {
                    var _temp = selectedExcarcelacion;
                    await BuscarExcarcelaciones(GlobalVar.gCentro, estatus_habilitados.ToList(), anioBuscarExc, folioBuscarExc, nombreBuscarExc, apellidoPaternoBuscarExc,
                    apellidoMaternoBuscarExc, selectedExc_TipoBuscarValue == 0 ? null : (short?)selectedExc_TipoBuscarValue, fechaInicialBuscarExc,fechaFinalBuscarExc);
                    selectedExcarcelacion = ListaExcarcelaciones.FirstOrDefault(w => w.ID_ANIO == _temp.ID_ANIO && w.ID_CENTRO == _temp.ID_CENTRO
                        && w.ID_CONSEC == _temp.ID_CONSEC && w.ID_IMPUTADO == _temp.ID_IMPUTADO && w.ID_INGRESO == _temp.ID_INGRESO);
                    OnPropertyChanged("SelectedExcarcelacion");
                    if (selectedExcarcelacion!=null)
                    {
                        listaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>(selectedExcarcelacion.EXCARCELACION_DESTINO.Select(
                            s => new CT_EXCARCELACION_DESTINO
                            {
                                ID_CONSEC = s.ID_DESTINO,
                                CAUSA_PENAL = s.CAUSA_PENAL,
                                DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? s.JUZGADO.DESCR : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ?
                                            s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL == id_otro_hospital ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL_OTRO : !string.IsNullOrEmpty(s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR) ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().HOSPITAL.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                DOCUMENTO = s.DOCUMENTO,
                                ESTATUS = s.ID_ESTATUS,
                                FOLIO = s.FOLIO_DOC,
                                FORMATO_DOCUMENTO = s.ID_FORMATO,
                                ID_DESTINO = selectedExcarcelacion.ID_TIPO_EX == (short)enumExcarcelacionTipo.JURIDICA ? (short?)s.ID_JUZGADO : s.INTERCONSULTA_SOLICITUD != null ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any() ? s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL.HasValue ? (short?)s.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.FirstOrDefault().ID_HOSPITAL : null : null : null,
                                TIPO_DOCUMENTO = s.ID_TIPO_DOC,
                                CANCELACION_OBSERVACION = s.CANCELADO_OBSERVA,
                                ID_CANCELACION_MOTIVO = s.CAN_ID_MOTIVO,
                                CERTIFICADO_MEDICO = null,
                                CERTIFICADO_MEDICO_ENABLED = false,
                                ID_INTERC=s.ID_INTERSOL,
                                ID_CENTRO_UBI=s.ID_CENTRO_UBI
                            }
                            ));
                        OnPropertyChanged("ListaExcarcelacionDestinos");
                    }
                    else
                    {
                        ListaExcarcelacionDestinos = new ObservableCollection<CT_EXCARCELACION_DESTINO>();
                        IsExcarcelacionDestinosVisible = Visibility.Collapsed;
                    }
                    
                    new Dialogos().ConfirmacionDialogo("EXITO!", "El destino de la excarcelación ha sido actualizado");
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EXCARCELACION_CANCELACION_MOTIVO);
                    setValidacionesExcarcelacion_Actualizacion();
                }
            }
        }

        #endregion

        #region Llenado de Catalogos
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

        #region Buscar Evaluaciones
        private void ObtenerExcarcelacionTiposBuscar(bool isExceptionManaged = false)
        {
            try
            {
                excarcelacion_TiposBuscar = new ObservableCollection<EXCARCELACION_TIPO>(new cExcarcelacionTipo().Seleccionar());
                excarcelacion_TiposBuscar.Insert(0, new EXCARCELACION_TIPO
                {
                    DESCR = "TODOS",
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

        private void ObtenerExcarcelacionEstatusBuscar(bool isExceptionManaged = false)
        {
            try
            {
                excarcelacion_EstatusBuscar = new ObservableCollection<EXCARCELACION_ESTATUS>(new cExcarcelacion_Estatus().Seleccionar());
                var temp = new EXCARCELACION_ESTATUS[excarcelacion_EstatusBuscar.Count];
                excarcelacion_EstatusBuscar.CopyTo(temp, 0);
                foreach (var item in temp)
                    if (!estatus_habilitados.Contains(item.ID_ESTATUS))
                        excarcelacion_EstatusBuscar.Remove(excarcelacion_EstatusBuscar.First(w => w.ID_ESTATUS == item.ID_ESTATUS));
                excarcelacion_EstatusBuscar.Insert(0, new EXCARCELACION_ESTATUS
                {
                    DESCR = "TODOS",
                    ID_ESTATUS = "0"
                });
                OnPropertyChanged("Excarcelacion_EstatusBuscar");
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

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                MenuGuardarEnabled = false;
                MenuBuscarEnabled = false;
                permisos = new cProcesoUsuario().Obtener(enumProcesos.EXCARCELACIONAUTORIZACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        MenuGuardarEnabled = true;

                    if (p.CONSULTAR == 1)
                    {
                        MenuBuscarEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

       
        #endregion

        #region Documentos
        /// <summary>
        /// metodo usado para mostrar documento
        /// </summary>
        /// <param name="origen_comando" tipo="Origen_Comando_Visualizacion">origen del comando. </param>
        /// <returns></returns>
        private void MostrarDocumento(byte[] _documento, short _formato_documento)
        {
            try
            {
                if (_formato_documento == Parametro.ID_FORMATO_IMAGEN)
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
                                File.WriteAllBytes(fileNamepdf, _documento);
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
                            switch (_formato_documento)
                            {
                                case 1://DOCX
                                    tc2.editor.Load(_documento, TXTextControl.BinaryStreamType.WordprocessingML);
                                    break;
                                case 3://PDF
                                    tc2.editor.Load(_documento, TXTextControl.BinaryStreamType.AdobePDF);
                                    break;
                                case 4://DOC
                                    tc2.editor.Load(_documento, TXTextControl.BinaryStreamType.MSWord);
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
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
            }
        }
        #endregion

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
