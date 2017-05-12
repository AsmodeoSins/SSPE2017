using ControlPenales.BiometricoServiceReference;
using System.Linq;
namespace ControlPenales
{
    partial class NotificacionTrabajoSocialViewModel : ValidationViewModelBase
    {
        public NotificacionTrabajoSocialViewModel() { }

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
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotificacionTrabajoSocialView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotificacionTrabajoSocialViewModel();
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
                    if (!EsMedico)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Verifique el perfil del usuario actual.");
                        return;
                    }

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
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                        break;
                    }

                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso valido.");
                        break;
                    }

                    break;

                case "reporte_menu":
                    if (SelectedNotificacionNueva != null)
                        if (EsMedico)
                            Imprimeformato();
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Solo el perfil de medico puede imprimir el oficio.");

                    break;

                #region Captura la Notificacion
                case "regresar_captura_notificacion":
                    var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Desea regresar?");
                    if (dialogresult != 0)
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotificacionTrabajoSocialView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotificacionTrabajoSocialViewModel();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        return;
                    break;

                case "borrar_enfermedad":
                    if (ListEnfermedades == null) return;
                    if (SelectEnfermedad == null) return;
                    ListEnfermedades.Remove(ListEnfermedades.First(f => f.ID_ENFERMEDAD == SelectEnfermedad.ID_ENFERMEDAD));
                    break;

                case "guardar_notificacion":
                    if (base.HasErrors)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", base.Error);
                        }));
                        return;
                    }
                    else
                    {
                        if (EsMedico)
                            GuardaNotificacionMedico();
                        else
                            if (EsTrabajoSocial)
                                if (SelectedNotificacionNueva != null)
                                    GuardaNotificacionTrabajoSocial();
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Verifique el perfil del usuario actual.");
                    }

                    break;
                case "limpiar_notificacion":
                    if (EsMedico)
                    {
                        LimpiaCamposCapturaNotificacion();
                        ValidacionesNotificacionMedico();
                    }
                    else
                        if (EsTrabajoSocial)
                        {
                            MensajeRespuestaTS = string.Empty;
                            ValidacionesTrabajoSocial();
                        };

                    break;

                case "atender_notificacion":
                    if (SelectedNotificacionNueva != null)
                        if (EsTrabajoSocial)
                            if (SelectedNotificacionNueva.ID_INGRESO != null)
                                ProcesaSolicitudSeleccionada();
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique el ingreso de la notificación seleccionada.");
                    break;

                case "buscar_perfil":
                    if (EsMedico)
                        BusquedaPorMedico();
                    else
                        if (EsTrabajoSocial)
                            BusquedaPorTrabajoSocial();
                    break;
                #endregion
            }
        }

        private void BusquedaPorMedico()
        {
            try
            {
                System.DateTime f1, f2;
                if (FechaInicioBusquedaNotificaciones.HasValue)
                    f1 = FechaInicioBusquedaNotificaciones.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaNotificaciones.HasValue)
                    f2 = FechaFinBusquedaNotificaciones.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new System.DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new System.DateTime(f2.Year, f2.Month, f2.Day);
                LstNotificacionesNuevas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().BuscarNotificacionesMedico(f1, f2));
                if (LstNotificacionesNuevas.Count > 0)
                    EmptyResultados = false;
                else
                    EmptyResultados = true;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de las notificaciones", exc);
            }
        }

        private void BusquedaPorTrabajoSocial()
        {
            try
            {
                System.DateTime f1, f2;
                if (FechaInicioBusquedaNotificaciones.HasValue)
                    f1 = FechaInicioBusquedaNotificaciones.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaNotificaciones.HasValue)
                    f2 = FechaFinBusquedaNotificaciones.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new System.DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new System.DateTime(f2.Year, f2.Month, f2.Day);
                LstNotificacionesNuevas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().BuscarNotificacionesTrabajoSocial(f1, f2));
                if (LstNotificacionesNuevas.Count > 0)
                    EmptyResultados = false;
                else
                    EmptyResultados = true;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de las notificaciones", exc);
            }
        }

        private void ProcesaSolicitudSeleccionada()
        {
            try
            {
                EnabledCamposMedicos = false;
                IsReadOnlyOtrosTS = true;
                var _centro = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                ListEnfermedades = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD>();
                var _NotificacionMedica = new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().GetData(x => x.ID_NOTIFICACION_TS == SelectedNotificacionNueva.ID_NOTIFICACION_TS).FirstOrDefault();
                if (_NotificacionMedica != null)
                {
                    SelectIngreso = _NotificacionMedica.INGRESO;
                    LimpiaCamposCapturaNotificacion();
                    FechaSolicitud = Fechas.GetFechaDateServer;
                    Expediente = string.Format("{0} / {1}", _NotificacionMedica.ID_ANIO , _NotificacionMedica.ID_IMPUTADO);
                    MotivoNotificacion = _NotificacionMedica.MENSAJE;
                    RiesgosNotificacionTS = _NotificacionMedica.ID_RIESGOS;
                    OtroRiesgoEspecifique = _NotificacionMedica.OTROS_RIESGOS;
                    CaracterNotificacion = _NotificacionMedica.ID_CARACTER;
                    MensajeRespuestaTS = string.Empty;
                    NombreCentro = _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty;
                    if (_NotificacionMedica.DIAGNOSTICO_MEDICO_NOTIFICA != null)
                        if (_NotificacionMedica.DIAGNOSTICO_MEDICO_NOTIFICA.Any())
                            foreach (var item in _NotificacionMedica.DIAGNOSTICO_MEDICO_NOTIFICA)
                                ListEnfermedades.Add(new SSP.Servidor.ENFERMEDAD
                                    {
                                        CLAVE = item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.CLAVE) ? item.ENFERMEDAD.CLAVE.Trim() : string.Empty : string.Empty,
                                        NOMBRE = item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty
                                    });

                    NombreImputado = string.Format("{0} {1} {2}",
                        _NotificacionMedica.INGRESO != null ? _NotificacionMedica.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_NotificacionMedica.INGRESO.IMPUTADO.NOMBRE) ? _NotificacionMedica.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                        _NotificacionMedica.INGRESO != null ? _NotificacionMedica.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_NotificacionMedica.INGRESO.IMPUTADO.PATERNO) ? _NotificacionMedica.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                        _NotificacionMedica.INGRESO != null ? _NotificacionMedica.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_NotificacionMedica.INGRESO.IMPUTADO.MATERNO) ? _NotificacionMedica.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);

                    EstanciaImputadp = string.Format("{0}-{1}{2}-{3}",
                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                SelectIngreso.ID_UB_CAMA);

                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CapturaNotificacionView();
                };
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
            }
        }

        private void GuardaNotificacionMedico()
        {
            try
            {
                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                SSP.Servidor.NOTIFICACION_TS _Notificacion = new SSP.Servidor.NOTIFICACION_TS()
                {
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    //ID_DIAGNOSTICO = Expediente,
                    REGISTRO_FEC = FechaSolicitud,
                    ID_RIESGOS = RiesgosNotificacionTS,
                    OTROS_RIESGOS = OtroRiesgoEspecifique,
                    MENSAJE = MotivoNotificacion,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    ID_CARACTER = CaracterNotificacion,
                    ID_USUARIO = _detalleUsuarioActual != null ? _detalleUsuarioActual.ID_USUARIO : string.Empty,
                    ID_CENTRO_UBI = GlobalVar.gCentro
                };

                System.DateTime _fechaActual = Fechas.GetFechaDateServer;
                if (ListEnfermedades != null)
                    if (ListEnfermedades.Any())
                        foreach (var item in ListEnfermedades)
                        {
                            _Notificacion.DIAGNOSTICO_MEDICO_NOTIFICA.Add(new SSP.Servidor.DIAGNOSTICO_MEDICO_NOTIFICA
                                {
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                    REGISTRO_FEC = _fechaActual,
                                    ID_CENTRO_UBI = GlobalVar.gCentro
                                });
                        };

                if (new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().GuardaNotificacionMedicaTransaccion(_Notificacion, (short)enumMensajeTipo.AVISO_NOTIFICACION_AREA_MEDICA))
                {
                    (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado la notificación exitosamente");
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotificacionTrabajoSocialView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotificacionTrabajoSocialViewModel();
                }
                else
                    (new Dialogos()).ConfirmacionDialogo("Error", "Surgió un error al ingresar la notificación");

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
            }
        }

        private void GuardaNotificacionTrabajoSocial()
        {
            try
            {
                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();
                SSP.Servidor.NOTIFICACION_TS _Notificacion = new SSP.Servidor.NOTIFICACION_TS()
                {
                    ID_ANIO = SelectedNotificacionNueva.ID_ANIO,
                    ID_IMPUTADO = SelectedNotificacionNueva.ID_IMPUTADO,
                    ID_CENTRO = SelectedNotificacionNueva.ID_CENTRO,
                    //ID_DIAGNOSTICO = Expediente,
                    REGISTRO_FEC = FechaSolicitud,
                    ID_RIESGOS = RiesgosNotificacionTS,
                    OTROS_RIESGOS = OtroRiesgoEspecifique,
                    MENSAJE = MensajeRespuestaTS,
                    ID_CENTRO_UBI = GlobalVar.gCentro,
                    ID_INGRESO = SelectedNotificacionNueva.ID_INGRESO,
                    ID_CARACTER = CaracterNotificacion,
                    ID_CANALIZACION_TS = SelectedNotificacionNueva.ID_CANALIZACION_TS,
                    ID_NOTIFICACION_TS = SelectedNotificacionNueva.ID_NOTIFICACION_TS,
                    ID_USUARIO = _detalleUsuarioActual != null ? _detalleUsuarioActual.ID_USUARIO : string.Empty
                };

                if (new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().GuardaNotificacionTrabajoSocial(_Notificacion, (short)enumMensajeTipo.AVISO_RESPUESTA_TRABAJO_SOCIAL))
                {
                    (new Dialogos()).ConfirmacionDialogo("Exito!", "Se ha registrado la notificación exitosamente");
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new NotificacionTrabajoSocialView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.NotificacionTrabajoSocialViewModel();
                }
                else
                    (new Dialogos()).ConfirmacionDialogo("Error", "Surgió un error al ingresar la notificación");

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
            }
        }

        private void LimpiaCamposCapturaNotificacion()
        {
            try
            {
                FechaSolicitud = Fechas.GetFechaDateServer;
                OtroRiesgoEspecifique = MotivoNotificacion = Expediente = string.Empty;
                RiesgosNotificacionTS = CaracterNotificacion = -1;
                ListEnfermedades = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ENFERMEDAD>();
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
                if (!EsMedico)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Verifique el perfil del usuario actual.");
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


        private void Imprimeformato()
        {
            try
            {
                cNotificacionTrabajoSocialReporte DatosReporte = new cNotificacionTrabajoSocialReporte();
                ReportesView View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.Show();

                if (EsMedico)
                {
                    if (SelectedNotificacionNueva == null)
                        return;

                    var _datosNotificacion = new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().GetData(x => x.ID_NOTIFICACION_TS == SelectedNotificacionNueva.ID_NOTIFICACION_TS);
                    if (_datosNotificacion.Any())
                    {
                        var _DatosTrabajoSocial = _datosNotificacion.FirstOrDefault();//EAVILA
                        var _datosParaMedico = _datosNotificacion.FirstOrDefault().NOTIFICACION_TS1;// SYSTEM
                        var _centro = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == _DatosTrabajoSocial.ID_CENTRO).FirstOrDefault();
                        DatosReporte.NombreAreaMedica = string.Format("ÁREA MÉDICA DEL <b>{0}</b>", _centro != null ? !string.IsNullOrEmpty(_centro.DESCR) ? _centro.DESCR.Trim() : string.Empty : string.Empty);

                        DatosReporte.Expediente = string.Format("{0} / {1}", _DatosTrabajoSocial.ID_ANIO, _DatosTrabajoSocial.ID_IMPUTADO);
                        DatosReporte.Caracter = _DatosTrabajoSocial.CARACTER_NOTIFICACION_TS != null ? !string.IsNullOrEmpty(_DatosTrabajoSocial.CARACTER_NOTIFICACION_TS.DESCR) ? _DatosTrabajoSocial.CARACTER_NOTIFICACION_TS.DESCR.Trim() : string.Empty : string.Empty;
                        DatosReporte.Activo1 = _DatosTrabajoSocial.ID_RIESGOS == (decimal)eTiposRiesgos.HOSPITALIZADO_HOSPITAL_GENERAL ? "X" : string.Empty;
                        DatosReporte.Activo2 = _DatosTrabajoSocial.ID_RIESGOS == (decimal)eTiposRiesgos.HOSPITALIZADO_CENTRO ? "X" : string.Empty;
                        DatosReporte.Activo3 = _DatosTrabajoSocial.ID_RIESGOS == (decimal)eTiposRiesgos.DECESO ? "X" : string.Empty;
                        DatosReporte.Activo4 = _DatosTrabajoSocial.ID_RIESGOS == (decimal)eTiposRiesgos.ESTUDIOS ? "X" : string.Empty;
                        DatosReporte.Activo5 = _DatosTrabajoSocial.ID_RIESGOS == (decimal)eTiposRiesgos.OTROS ? "X" : string.Empty;

                        if (_DatosTrabajoSocial.INGRESO != null)
                        {
                            var _IngresoActivo = _DatosTrabajoSocial.INGRESO;
                            if (_IngresoActivo != null)
                                DatosReporte.Estancia = string.Format("{0}-{1}-{2}-{3}",
                                    _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? _IngresoActivo.CAMA.CELDA.SECTOR != null ? _IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? _IngresoActivo.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                    _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? _IngresoActivo.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.SECTOR.DESCR) ? _IngresoActivo.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                    _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.CELDA != null ? !string.IsNullOrEmpty(_IngresoActivo.CAMA.CELDA.ID_CELDA) ? _IngresoActivo.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                    _IngresoActivo.CAMA != null ? _IngresoActivo.CAMA.ID_CAMA.ToString() : string.Empty);

                            DatosReporte.NombreInterno = string.Format("{0} {1} {2}",
                                    _DatosTrabajoSocial.INGRESO != null ? _DatosTrabajoSocial.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_DatosTrabajoSocial.INGRESO.IMPUTADO.NOMBRE) ? _DatosTrabajoSocial.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                    _DatosTrabajoSocial.INGRESO != null ? _DatosTrabajoSocial.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_DatosTrabajoSocial.INGRESO.IMPUTADO.PATERNO) ? _DatosTrabajoSocial.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    _DatosTrabajoSocial.INGRESO != null ? _DatosTrabajoSocial.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(_DatosTrabajoSocial.INGRESO.IMPUTADO.MATERNO) ? _DatosTrabajoSocial.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);
                        };

                        if (_datosParaMedico != null)
                        {
                            var _PreguntaMedico = _datosParaMedico.FirstOrDefault();
                            if (_PreguntaMedico != null)
                            {
                                DatosReporte.FechaSolicitud = _PreguntaMedico.REGISTRO_FEC.HasValue ? _PreguntaMedico.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                                DatosReporte.Mensaje = _PreguntaMedico.MENSAJE;
                                DatosReporte.Generico4 = string.Format("{0}-{1}", _PreguntaMedico.REGISTRO_FEC.HasValue ? _PreguntaMedico.REGISTRO_FEC.Value.Year.ToString() : string.Empty, _PreguntaMedico.FOLIO_NOTIF.HasValue ? _PreguntaMedico.FOLIO_NOTIF.Value.ToString() : string.Empty);
                                DatosReporte.NombreMedico = _PreguntaMedico.USUARIO != null ?
                                        string.Format("{0} {1} {2}",
                                        _PreguntaMedico.USUARIO.EMPLEADO != null ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_PreguntaMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                        _PreguntaMedico.USUARIO.EMPLEADO != null ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_PreguntaMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                        _PreguntaMedico.USUARIO.EMPLEADO != null ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA != null ? !string.IsNullOrEmpty(_PreguntaMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _PreguntaMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty) : string.Empty;

                                if (_PreguntaMedico.DIAGNOSTICO_MEDICO_NOTIFICA != null)
                                    if (_PreguntaMedico.DIAGNOSTICO_MEDICO_NOTIFICA.Any())
                                        foreach (var item in _PreguntaMedico.DIAGNOSTICO_MEDICO_NOTIFICA)
                                            DatosReporte.DiagnosticoMedico += string.Format("{0}, ", item.ENFERMEDAD != null ? !string.IsNullOrEmpty(item.ENFERMEDAD.NOMBRE) ? item.ENFERMEDAD.NOMBRE.Trim() : string.Empty : string.Empty);
                            };
                        };

                        if (_DatosTrabajoSocial != null)
                        {
                            DatosReporte.NombreTrabajoSocial = _DatosTrabajoSocial.USUARIO != null ? _DatosTrabajoSocial.USUARIO.EMPLEADO != null ? _DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                               !string.IsNullOrEmpty(_DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                               !string.IsNullOrEmpty(_DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                               !string.IsNullOrEmpty(_DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _DatosTrabajoSocial.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                            DatosReporte.Generico1 = _DatosTrabajoSocial.REGISTRO_FEC.HasValue ? _DatosTrabajoSocial.REGISTRO_FEC.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                            DatosReporte.Generico2 = _DatosTrabajoSocial.MENSAJE;
                        }

                        View.Report.LocalReport.ReportPath = "Reportes/rNotificacionTrabajoSocialFormato.rdlc";
                        View.Report.LocalReport.DataSources.Clear();

                        #region Encabezado
                        var Encabezado = new cEncabezado();
                        Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        Encabezado.NombreReporte = "COORDINACIÓN ESTATAL MÉDICA ";
                        Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                        Encabezado.ImagenFondo = Parametro.LOGO_ESTADO_BC;
                        #endregion

                        var ds1 = new System.Collections.Generic.List<cEncabezado>();
                        ds1.Add(Encabezado);
                        Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds1.Name = "DataSet1";
                        rds1.Value = ds1;
                        View.Report.LocalReport.DataSources.Add(rds1);

                        var ds2 = new System.Collections.Generic.List<cNotificacionTrabajoSocialReporte>();
                        ds2.Add(DatosReporte);
                        Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        rds2.Name = "DataSet2";
                        rds2.Value = ds2;
                        View.Report.LocalReport.DataSources.Add(rds2);
                        View.Report.RefreshReport();

                    }
                }
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el reporte", exc);
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
                    SSP.Servidor.CENTRO _centroActual = new SSP.Controlador.Catalogo.Justicia.cCentro().GetData(x => x.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    NombreCentro = _centroActual != null ? !string.IsNullOrEmpty(_centroActual.DESCR) ? _centroActual.DESCR.Trim() : string.Empty : string.Empty;
                    Expediente = string.Format("{0} / {1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO);
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CapturaNotificacionView();
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }


        #region Load de la pagina
        private void LoadNotificacionTrabajoSocial(NotificacionTrabajoSocialView Window = null)
        {
            ConfiguraPermisos();
        }

        private void LoadCapturaNotificacionTS(CapturaNotificacionView Window = null)
        {
            try
            {
                InicializaCapturaNotificacion(Window);
                if (EsMedico)
                    ValidacionesNotificacionMedico();
                else
                    if (EsTrabajoSocial)
                        ValidacionesTrabajoSocial();
            }
            catch (System.Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
            }
        }

        private async void InicializaCapturaNotificacion(CapturaNotificacionView Window = null)
        {

            try
            {
                if (SelectIngreso == null)
                    return;

                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        NombreTituloCapturaNotificacion = NombreImputado = EstanciaImputadp = NombreMotivo = TipoNotificacionDescripcion = string.Empty;
                        NombreImputado = string.Format("{0} {1} {2}",
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                        EstanciaImputadp = string.Format("{0}-{1}{2}-{3}",
                                    SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                    SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                    SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                    SelectIngreso.ID_UB_CAMA);

                        if (EsMedico)
                        {
                            NombreTituloCapturaNotificacion = "Notificación a Trabajo Social";
                            FechaSolicitud = Fechas.GetFechaDateServer;
                            TipoNotificacionDescripcion = "CANALIZA A TRABAJO SOCIAL";
                        }
                        else
                        {
                            if (EsTrabajoSocial)
                            {
                                BorrarEnfermedadEnabled = false;
                                VisibleMensajeTS = System.Windows.Visibility.Visible;
                                NombreTituloCapturaNotificacion = "Notificación a Área Médica";
                                FechaSolicitud = Fechas.GetFechaDateServer;
                                TipoNotificacionDescripcion = "RESPUESTA DE TRABAJO SOCIAL";
                            };
                        }

                        if (Window == null)
                            return;

                        AutoCompleteTB = Window.AutoCompleteTB;
                        AutoCompleteLB = AutoCompleteTB.Template.FindName("PART_ListBox", Window.AutoCompleteTB) as System.Windows.Controls.ListBox;
                        AutoCompleteTB.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(listBox_MouseUp);
                        AutoCompleteTB.KeyDown += listBox_KeyDown;
                        StaticSourcesViewModel.SourceChanged = true;
                    }

                    catch (System.Exception exc)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", exc);
                    }
                });
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración inicial", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var _detalleUsuarioActual = new SSP.Controlador.Catalogo.Justicia.cUsuario().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr).FirstOrDefault();

                LstCaracterNotificacionTS = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CARACTER_NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cCaracterNotificacionResultadoServAux().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstRiesgosNotificacionTS = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.RIESGO_NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cRiesgoNositicacionResultServAux().GetData(x => x.ESTATUS == "S", y => y.DESCR));
                LstTipoNotificacionTS = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.DIAGNOSTICO_NOTIFICA_TS_TIPO>(new SSP.Controlador.Catalogo.Justicia.cNotificacionTipoResultServAux().GetData(x => x.ESTATUS == "S", y => y.DESCR));

                LstCaracterNotificacionTS.Insert(0, new SSP.Servidor.CARACTER_NOTIFICACION_TS { ID_CARACTER = -1, DESCR = "SELECCIONE" });
                LstRiesgosNotificacionTS.Insert(0, new SSP.Servidor.RIESGO_NOTIFICACION_TS { ID_RIESGOS = -1, DESCR = "SELECCIONE" });
                LstTipoNotificacionTS.Insert(0, new SSP.Servidor.DIAGNOSTICO_NOTIFICA_TS_TIPO { ID_DIAGNOSTICO = -1, DESCR = "SELECCIONE" });
                RiesgosNotificacionTS = CaracterNotificacion = -1;

                if (_detalleUsuarioActual != null)
                {
                    foreach (var item in _detalleUsuarioActual.USUARIO_ROL)
                        if (item.ID_ROL == (short)eRolesNotificacionTrabajoSocial.COORDINACION_ESTATAL_MEDICA || item.ID_ROL == (short)eRolesNotificacionTrabajoSocial.COORDINADOR_MEDICO || item.ID_ROL == (short)eRolesNotificacionTrabajoSocial.MEDICO || item.ID_ROL == (short)eRolesNotificacionTrabajoSocial.DENTISTA)
                            EsMedico = true;

                    if (EsMedico)
                    {
                        EnabledCamposMedicos = true;
                        NombrePerfilActual = "PERFIL ACTUAL: NOTIFICACIÓN MÉDICA";
                        #region PERMISOS MEDICO
                        var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.NOTIFICACION_TS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                        foreach (var p in permisos)
                        {
                            if (p.INSERTAR == 1)
                                PInsertar = MenuGuardarEnabled = true;
                            if (p.EDITAR == 1)
                                PEditar = MenuGuardarEnabled = true;
                            if (p.CONSULTAR == 1)
                                PConsultar = MenuBuscarEnabled = true;
                            if (p.IMPRIMIR == 1)
                                PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                        };

                        #endregion

                        LstNotificacionesNuevas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().CargaInicialNotificaciones());
                        if (LstNotificacionesNuevas.Count > 0)
                            EmptyResultados = false;
                        else
                            EmptyResultados = true;
                    }
                    else
                    {
                        EnabledCamposMedicos = false;
                        if (_detalleUsuarioActual.USUARIO_ROL.Any(x => x.ID_ROL == (short)eRolesNotificacionTrabajoSocial.TRABAJADOR_SOCIAL || x.ID_ROL == (short)eRolesNotificacionTrabajoSocial.TRABAJO_SOCIAL))
                            EsTrabajoSocial = true;

                        if (EsTrabajoSocial)
                        {
                            NombrePerfilActual = "PERFIL ACTUAL: RESPUESTA DE TRABAJO SOCIAL";

                            #region PERMISOS TRABAJO SOCIAL
                            var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.NOTIFICACION_TS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                            foreach (var p in permisos)
                            {
                                if (p.INSERTAR == 1)
                                    PInsertar = MenuGuardarEnabled = true;
                                if (p.EDITAR == 1)
                                    PEditar = MenuGuardarEnabled = true;
                                if (p.CONSULTAR == 1)
                                    PConsultar = MenuBuscarEnabled = true;
                                if (p.IMPRIMIR == 1)
                                    PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                            };

                            #endregion

                            LstNotificacionesNuevas = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.NOTIFICACION_TS>(new SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP().GetData(x => x.ID_CANALIZACION_TS == null && x.ID_NOTIFICAION_TIPO == (decimal)SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP.eTiposNotificaciones.CANALIZA_TRABAJO_SOCIAL));
                            if (LstNotificacionesNuevas.Count > 0)
                                EmptyResultados = false;
                            else
                                EmptyResultados = true;
                        }
                        else
                        {//No es ni medico no trabajo social, valida si tiene permiso al proceso
                            var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.NOTIFICACION_TS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                            foreach (var p in permisos)
                            {
                                if (p.INSERTAR == 1)
                                    PInsertar = MenuGuardarEnabled = true;
                                if (p.EDITAR == 1)
                                    PEditar = MenuGuardarEnabled = true;
                                if (p.CONSULTAR == 1)
                                    PConsultar = MenuBuscarEnabled = true;
                                if (p.IMPRIMIR == 1)
                                    PImprimir = MenuFichaEnabled = MenuReporteEnabled = true;
                            };
                        }
                    };
                };
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
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
                }
            }
        }
        #endregion
    }
}