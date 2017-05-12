using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ControlPenales.BiometricoServiceReference;
using System.Windows.Media;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using SSP.Servidor;

namespace ControlPenales
{
    public partial class BusquedaHuellaExcarcelaciones : FingerPrintScanner
    {
        #region Constructor
        /// <summary>
        /// Constructor de la clase: Hereda de la clase FingerPrintScanner y puede ser utilizado para inicializar las variables de la instancia.
        /// </summary>
        public BusquedaHuellaExcarcelaciones(BusquedaHuellaExcarcelacionView Ventana)
        {
            this.Ventana = Ventana;
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;
            HuellasImputados = ObtenerUniversoHuellasExcarcelacion();

        }
        #endregion

        #region Métodos Eventos
        /// <summary>
        /// Método que se utiliza para sobrecargar al método "OnCaptured" existente en la clase padre "FingerPrintScanner"
        /// para realizar más acciones.
        /// </summary>
        /// <param name="captureResult">Resultado de la captura</param>
        public override void OnCaptured(CaptureResult captureResult)
        {

            base.OnCaptured(captureResult);
            CompararHuellaImputado();
        }

        public async void ClickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "0";
                    }
                    break;
                case "1":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "1";
                    }
                    break;
                case "2":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "2";
                    }
                    break;
                case "3":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "3";
                    }
                    break;
                case "4":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "4";
                    }
                    break;
                case "5":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "5";
                    }
                    break;
                case "6":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "6";
                    }
                    break;
                case "7":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "7";
                    }
                    break;
                case "8":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "8";
                    }
                    break;
                case "9":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "9";
                    }
                    break;
                case "backspace":
                    if (NIPBuscar.Length > 0)
                    {
                        NIPBuscar = NIPBuscar.Substring(0, NIPBuscar.Length - 1);
                    }
                    break;
                case "limpiarNIP":
                    NIPBuscar = "";
                    break;
                case "onBuscarPorNIP":
                    try
                    {
                        if (!string.IsNullOrEmpty(NIPBuscar))
                        {
                            CompararNIPImputado();
                        }
                        else
                        {
                            CambiarMensajeNIP(enumMensajeResultadoComparacion.NO_ENCONTRADO);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    break;
                case "OpenCloseFlyout":
                    if (CapturaNIPVisible)
                    {
                        CapturaNIPVisible = false;
                    }

                    else
                    {
                        CapturaNIPVisible = true;
                    }
                    break;
                case "PermitirExcarcelacion":
                    if (!SelectedImputado.PERMITIR)
                    {
                        IncidenciaRevertirExcarcelacionVisible = true;
                    }
                    break;
                case "CapturarIncidenciaRevertirUltimoMovimiento":
                    ValidacionNIP();
                    if (!base.HasErrors)
                    {
                        var imputado_validacion = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                        ValidacionNIPInexistente(imputado_validacion);
                        if (!base.HasErrors)
                        {
                            ValidacionImputadoActivo(imputado_validacion);
                            if (!base.HasErrors)
                            {


                                try
                                {
                                    SelectedImputado.HABILITAR = false;

                                    var lista_excarcelacion_internos = new ObservableCollection<InternoExcarcelacion>();

                                    var selected_imputado = new InternoExcarcelacion()
                                    {
                                        ID_CENTRO = SelectedImputado.ID_CENTRO,
                                        ID_ANIO = SelectedImputado.ID_ANIO,
                                        ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                                        ID_INGRESO = SelectedImputado.ID_INGRESO,
                                        PERMITIR = SelectedImputado.PERMITIR,
                                        HABILITAR = SelectedImputado.HABILITAR
                                    };
                                    foreach (var interno in Excarcelacion)
                                    {
                                        lista_excarcelacion_internos.Add(interno);
                                    }
                                    Excarcelacion = lista_excarcelacion_internos;
                                    SelectedImputado = Excarcelacion.Where(w =>
                                        w.ID_CENTRO == selected_imputado.ID_CENTRO &&
                                        w.ID_ANIO == selected_imputado.ID_ANIO &&
                                        w.ID_IMPUTADO == selected_imputado.ID_IMPUTADO &&
                                        w.ID_INGRESO == selected_imputado.ID_INGRESO).
                                        FirstOrDefault();
                                    var revertir_ultimo_movimiento_excarcelacion_activa = new cExcarcelacion().ObtenerImputadoExcarcelaciones(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, SelectedImputado.ID_INGRESO).Where(w =>
                                        w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                        w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                        w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day &&
                                        w.ID_ESTATUS == EXCARCELACION_ACTIVA).FirstOrDefault();
                                    var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                                    var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                    RevertirUltimoMovimiento(revertir_ultimo_movimiento_excarcelacion_activa,
                                        new INCIDENTE()
                                        {
                                            ID_CENTRO = imputado_incidente.ID_CENTRO,
                                            ID_ANIO = imputado_incidente.ID_ANIO,
                                            ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                                            ID_INGRESO = ingreso_incidente,
                                            ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                                            ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                                            MOTIVO = TextoIncidenciaFalsoPositivo
                                        });
                                    var huella = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO).
                                                                IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                                                wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                                                wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                                    new Imputado_Huella
                                                                    {
                                                                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                                        tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                                    }).FirstOrDefault();
                                    if (huella != null)
                                        HuellasImputados.Add(huella);
                                    TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
                                    var placeholder = new Imagenes().getImagenPerson();
                                    Excarcelacion = new ObservableCollection<InternoExcarcelacion>();
                                    FotoCentro = placeholder;
                                    FotoIngreso = placeholder;
                                    IncidenciaNIP = "";
                                    IncidenciaRevertirExcarcelacionVisible = false;
                                }
                                catch (Exception ex)
                                {

                                    throw new ApplicationException(ex.Message);
                                }
                            }
                        }
                    }
                    break;
                case "CapturarIncidenciaRevertirExcarcelacion":
                    ValidacionNIP();
                    if (!base.HasErrors)
                    {
                        var imputado_validacion = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                        ValidacionNIPInexistente(imputado_validacion);
                        if (!base.HasErrors)
                        {
                            ValidacionImputadoActivo(imputado_validacion);
                            if (!base.HasErrors)
                            {
                                try
                                {
                                    SelectedImputado.HABILITAR = false;

                                    var lista_excarcelacion_internos_revertir_excarcelacion = new ObservableCollection<InternoExcarcelacion>();

                                    var selected_imputado_revertir_excarcelacion = new InternoExcarcelacion()
                                    {
                                        ID_CENTRO = SelectedImputado.ID_CENTRO,
                                        ID_ANIO = SelectedImputado.ID_ANIO,
                                        ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                                        ID_INGRESO = SelectedImputado.ID_INGRESO,
                                        PERMITIR = SelectedImputado.PERMITIR,
                                        HABILITAR = SelectedImputado.HABILITAR
                                    };
                                    foreach (var interno in Excarcelacion)
                                    {
                                        lista_excarcelacion_internos_revertir_excarcelacion.Add(interno);
                                    }
                                    Excarcelacion = lista_excarcelacion_internos_revertir_excarcelacion;
                                    SelectedImputado = Excarcelacion.Where(w =>
                                        w.ID_CENTRO == selected_imputado_revertir_excarcelacion.ID_CENTRO &&
                                        w.ID_ANIO == selected_imputado_revertir_excarcelacion.ID_ANIO &&
                                        w.ID_IMPUTADO == selected_imputado_revertir_excarcelacion.ID_IMPUTADO &&
                                        w.ID_INGRESO == selected_imputado_revertir_excarcelacion.ID_INGRESO).
                                        FirstOrDefault();
                                    var revertir_excarcelacion_activa = new cExcarcelacion().ObtenerImputadoExcarcelaciones(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, SelectedImputado.ID_INGRESO).Where(w =>
                                       w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                       w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                       w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day &&
                                       w.ID_ESTATUS == EXCARCELACION_ACTIVA).FirstOrDefault();
                                    var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                                    var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                    RevertirExcarcelacion(revertir_excarcelacion_activa, new INCIDENTE()
                                    {
                                        ID_CENTRO = imputado_incidente.ID_CENTRO,
                                        ID_ANIO = imputado_incidente.ID_ANIO,
                                        ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                                        ID_INGRESO = ingreso_incidente,
                                        ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                                        ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                                        MOTIVO = TextoIncidenciaFalsoPositivo
                                    });
                                    var huella_revertir_excarcelacion = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO).
                                                                IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                                                wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                                                wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                                    new Imputado_Huella
                                                                    {
                                                                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                                        tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                                    }).FirstOrDefault();
                                    if (huella_revertir_excarcelacion != null)
                                        HuellasImputados.Add(huella_revertir_excarcelacion);
                                    TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
                                    var placeholder = new Imagenes().getImagenPerson();
                                    Excarcelacion = new ObservableCollection<InternoExcarcelacion>();
                                    FotoCentro = placeholder;
                                    FotoIngreso = placeholder;
                                    IncidenciaNIP = "";
                                    IncidenciaRevertirExcarcelacionVisible = false;
                                }
                                catch (Exception ex)
                                {

                                    throw new ApplicationException(ex.Message);
                                }
                            }
                        }
                    }
                    break;
                case "CancelarReversion":
                    IncidenciaRevertirExcarcelacionVisible = false;
                    break;
                case "CerrarIncidenciaFlyout":
                    try
                    {
                        var excarcelacion_incidencia = new cExcarcelacion();
                        var ultimo_ingreso_incidencia = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                        var consulta_excarcelacion_incidencia = excarcelacion_incidencia.ObtenerImputadoExcarcelaciones(
                            ultimo_ingreso_incidencia.ID_CENTRO,
                            ultimo_ingreso_incidencia.ID_ANIO,
                            ultimo_ingreso_incidencia.ID_IMPUTADO,
                            ultimo_ingreso_incidencia.ID_INGRESO).Where(w =>
                            w.ID_ESTATUS == EXCARCELACION_EN_PROCESO &&
                            (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                            w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                            w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();

                        new cExcarcelacion().IniciarExcarcelacion(new EXCARCELACION()
                        {
                            ID_CENTRO = consulta_excarcelacion_incidencia.ID_CENTRO,
                            ID_ANIO = consulta_excarcelacion_incidencia.ID_ANIO,
                            ID_IMPUTADO = consulta_excarcelacion_incidencia.ID_IMPUTADO,
                            ID_INGRESO = consulta_excarcelacion_incidencia.ID_INGRESO,
                            ID_CONSEC = consulta_excarcelacion_incidencia.ID_CONSEC,
                            ID_ESTATUS = EXCARCELACION_ACTIVA,
                            SALIDA_FEC = Fechas.GetFechaDateServer,
                            ID_INCIDENCIA_TRASLADO = (short)enumIncidencias.CERTIFICADO_MEDICO_AUSENTE,
                            INCIDENCIA_OBSERVACION = TextoIncidencia
                        });
                        TextoIncidencia = "SIN OBSERVACIONES";
                        CapturaIncidenciaVisible = false;
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }

                    break;
                case "ConfirmacionCapturaResponsableFlyout":
                    AsignarResponsable = true;
                    ValidacionResponsable();
                    break;
                case "ContinuarFinalizaExcarcelacion":
                    try
                    {
                        var ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                        var excarcelacion_finaliza = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                               ingreso.ID_CENTRO,
                               ingreso.ID_ANIO,
                               ingreso.ID_IMPUTADO,
                               ingreso.ID_INGRESO).Where(w =>
                               w.ID_ESTATUS == EXCARCELACION_EN_PROCESO &&
                               (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                               w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                               w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();
                        new cExcarcelacion().IniciarExcarcelacion(new EXCARCELACION()
                        {
                            ID_CENTRO = excarcelacion_finaliza.ID_CENTRO,
                            ID_ANIO = excarcelacion_finaliza.ID_ANIO,
                            ID_IMPUTADO = excarcelacion_finaliza.ID_IMPUTADO,
                            ID_INGRESO = excarcelacion_finaliza.ID_INGRESO,
                            ID_CONSEC = excarcelacion_finaliza.ID_CONSEC,
                            ID_ESTATUS = EXCARCELACION_ACTIVA,
                            SALIDA_FEC = Fechas.GetFechaDateServer,
                            ID_INCIDENCIA_TRASLADO = !excarcelacion_finaliza.CERT_MEDICO_SALIDA.HasValue ? (short)enumIncidencias.CERTIFICADO_MEDICO_AUSENTE : (short?)null,
                            INCIDENCIA_OBSERVACION = !excarcelacion_finaliza.CERT_MEDICO_SALIDA.HasValue ? TextoIncidencia : null
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "GuardarResponsable":
                    if (SelectedImputado != null)
                    {
                        ValidacionResponsable();
                        if (!base.HasErrors)
                        {
                            Ventana.Hide();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                try
                                {
                                    var ingreso_excarcelacion_guarda_responsable = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                                    var excarcelacion_guarda_responsable = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                                       ingreso_excarcelacion_guarda_responsable.ID_CENTRO,
                                       ingreso_excarcelacion_guarda_responsable.ID_ANIO,
                                       ingreso_excarcelacion_guarda_responsable.ID_IMPUTADO,
                                       ingreso_excarcelacion_guarda_responsable.ID_INGRESO).Where(w =>
                                       (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO || w.ID_ESTATUS == EXCARCELACION_ACTIVA) &&
                                       (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                       w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                       w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();
                                    excarcelacion_guarda_responsable.RESPONSABLE = string.Format("{1} {2} {0}", NombreResponsableExcarcelacion, ApellidoPaternoResponsableExcarcelacion, ApellidoMaternoResponsableExcarcelacion);
                                    GuardarResponsable(excarcelacion_guarda_responsable);
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message);
                                }
                            });
                            Ventana.Show();
                            var MensajeGuardaResponsable = Ventana as MetroWindow;
                            var mySettingsGuardarResponsable = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Cerrar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            await MensajeGuardaResponsable.ShowMessageAsync("EXITO!", "Responsable asignado.", MessageDialogStyle.Affirmative, mySettingsGuardarResponsable);
                        }
                    }
                    else
                    {
                        var Mensaje = Ventana as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        await Mensaje.ShowMessageAsync("Validación", "Debe identificar un interno para asignar un responsable.", MessageDialogStyle.Affirmative, mySettings);
                    }
                    break;
                case "LimpiarResponsable":
                    if (SelectedImputado != null)
                    {
                        try
                        {
                            var ingreso_excarcelacion_limpia_responsable = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                            var excarcelacion_limpia_responsable = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                               ingreso_excarcelacion_limpia_responsable.ID_CENTRO,
                               ingreso_excarcelacion_limpia_responsable.ID_ANIO,
                               ingreso_excarcelacion_limpia_responsable.ID_IMPUTADO,
                               ingreso_excarcelacion_limpia_responsable.ID_INGRESO).Where(w =>
                               (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO || w.ID_ESTATUS == EXCARCELACION_EN_PROCESO) &&
                               (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                               w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                               w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();
                            excarcelacion_limpia_responsable.RESPONSABLE = null;
                            LimpiarResponsable(excarcelacion_limpia_responsable);
                            var MensajeLimpiaResponsable = Ventana as MetroWindow;
                            var mySettingsLimpiaResponsable = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Cerrar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            await MensajeLimpiaResponsable.ShowMessageAsync("EXITO!", "Se borró la información del responsable.", MessageDialogStyle.Affirmative, mySettingsLimpiaResponsable);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                    }
                    else
                    {
                        var Mensaje = Ventana as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        await Mensaje.ShowMessageAsync("Validación", "Debe identificar un interno para relevar su responsable.", MessageDialogStyle.Affirmative, mySettings);
                    }
                    break;

            }
        }

        /*MOUSE ENTER SWITCH - CAMBIA COLOR AL ESTILO DE LOS BOTONES DEL ESTILO SOBRE
         * LOS BOTONES DE "BORRAR UN CARACTÉR" Y "BORRAR TODO EL NIP"
        */
        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(69, 69, 69));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(69, 69, 69));
                    break;
            }
        }

        /*MOUSE ENTER SWITCH - REGRESA EL COLOR AL ESTILO DE LOS BOTONES DEL ESTILO SOBRE
         * LOS BOTONES DE "BORRAR UN CARACTÉR" Y "BORRAR TODO EL NIP"
        */
        private void MouseLeaveSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
                    break;
            }
        }

        //MÉTODO PARA EVENTO - EJECUTAR AL CARGAR LA VENTANA
        public void OnLoad(BusquedaHuellaExcarcelacionView Window)
        {
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            CheckMark = "🔍";
            AsignarResponsable = false;
            TextoIncidencia = TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
            ScannerMessage = "Capture Huella\nen el lector";
            ImagenEvaluacionVisible = Readers.Count > 0;
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            var placeholder = new Imagenes().getImagenPerson();
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ProgressRingVisible = Visibility.Collapsed;
            ImagenEvaluacion = new Imagenes().getImagenHuella();
            ScannerMessageVisible = true;
            FotoIngreso = placeholder;
            FotoCentro = placeholder;

            Window.Closing += async (s, e) =>
            {
                if (Excarcelacion != null && SelectedImputado != null && AsignarResponsable)
                {
                    var MensajeGuardarResponsableFinalizarExcarcelacion = Ventana as MetroWindow;
                    var mySettingsGuardarResponsableFinalizarExcarcelacion = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Cerrar",
                        AnimateShow = true,
                        AnimateHide = false
                    };
                    await MensajeGuardarResponsableFinalizarExcarcelacion.ShowMessageAsync("Validación", "Debe guardar la información del responsable antes de finalizar la excarcelación.", MessageDialogStyle.Affirmative, mySettingsGuardarResponsableFinalizarExcarcelacion);
                }
            };

            Window.Closed += (s, e) =>
            {

                try
                {
                    if (OnProgress == null)
                        return;

                    if (!IsSucceed)
                        SelectedImputado = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);

                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lectura de las huellas", ex);
                }
            };

            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            CurrentReader = Readers[0];

            if (CurrentReader == null)
                return;

            if (!OpenReader())
                Window.Close();

            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));
        }

        private void EnterKeyPressedID(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(NIPBuscar))
                {
                    CompararNIPImputado();
                }
                else
                {
                    CambiarMensajeNIP(enumMensajeResultadoComparacion.NO_ENCONTRADO);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Obtiene el Universo de Huellas de las Excarcelaciones que se encuentran viábles para su comparación.
        /// </summary>
        /// <returns>Lista de Huellas de Excarcelaciones En Proceso del dia actual.</returns>
        public List<Imputado_Huella> ObtenerUniversoHuellasExcarcelacion()
        {
            try
            {
                return new cExcarcelacion().GetData(g => (g.ID_ESTATUS == EXCARCELACION_EN_PROCESO || g.ID_ESTATUS==EXCARCELACION_AUTORIZADA) &&
                                                    g.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                                    g.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                                    g.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day &&
                                                    g.INGRESO.ID_UB_CENTRO.HasValue &&
                                                    g.INGRESO.ID_UB_CENTRO.Value == GlobalVar.gCentro).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                        new Imputado_Huella
                                        {
                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                        })
                                .ToList();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public void CompararHuellaImputado()
        {
            var MensajeResultado = enumMensajeResultadoComparacion.HUELLA_VACIA;
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;

            //SI LA HUELLA ES NULA, ENTONCES...
            if (bytesHuella == null)
            {
                //SE CAMBIA EL MENSAJE DE LA VENTANA, INDICANDO QUE SEA CAPTURADA DE NUEVO
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));
            }
            else
            {
                MensajeResultado = enumMensajeResultadoComparacion.PROCESANDO;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));

                try
                {
                    var doIdentifyExcarcelacion = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputados.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                    var result = new List<object>();

                    if (doIdentifyExcarcelacion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        if (HuellasImputados.Count == 0)
                        {
                            MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                        }
                        else
                        {
                            switch (doIdentifyExcarcelacion.ResultCode)
                            {
                                case Constants.ResultCode.DP_DEVICE_BUSY:
                                    break;
                                case Constants.ResultCode.DP_DEVICE_FAILURE:
                                    break;
                                case Constants.ResultCode.DP_ENROLLMENT_INVALID_SET:
                                    break;
                                case Constants.ResultCode.DP_ENROLLMENT_IN_PROGRESS:
                                    break;
                                case Constants.ResultCode.DP_ENROLLMENT_NOT_READY:
                                    break;
                                case Constants.ResultCode.DP_ENROLLMENT_NOT_STARTED:
                                    break;
                                case Constants.ResultCode.DP_FAILURE:
                                    break;
                                case Constants.ResultCode.DP_INVALID_DEVICE:
                                    break;
                                case Constants.ResultCode.DP_INVALID_FID:
                                    break;
                                case Constants.ResultCode.DP_INVALID_FMD:
                                    break;
                                case Constants.ResultCode.DP_INVALID_PARAMETER:
                                    break;
                                case Constants.ResultCode.DP_MORE_DATA:
                                    break;
                                case Constants.ResultCode.DP_NOT_IMPLEMENTED:
                                    break;
                                case Constants.ResultCode.DP_NO_DATA:
                                    break;
                                case Constants.ResultCode.DP_TOO_SMALL_AREA:
                                    break;
                                case Constants.ResultCode.DP_VERSION_INCOMPATIBILITY:
                                    break;
                            }
                        }

                    }
                    else
                    {
                        if (doIdentifyExcarcelacion.Indexes.Count() > 0)
                        {
                            foreach (var resultado in doIdentifyExcarcelacion.Indexes.ToList())
                                result.Add(HuellasImputados[resultado.FirstOrDefault()].IMPUTADO);
                        }


                        if (result.Count > 0)
                        {
                            if (result.Count == 1)
                            {
                                var imputado = ((cHuellasImputado)result.FirstOrDefault());
                                var excarcelacion = new cExcarcelacion();
                                var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, (short)imputado.ID_IMPUTADO);
                                var consulta_excarcelacion = excarcelacion.ObtenerImputadoExcarcelaciones(
                                    ultimo_ingreso.ID_CENTRO,
                                    ultimo_ingreso.ID_ANIO,
                                    ultimo_ingreso.ID_IMPUTADO,
                                    ultimo_ingreso.ID_INGRESO).Where(w =>
                                    (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO ||
                                    w.ID_ESTATUS == EXCARCELACION_AUTORIZADA) &&
                                    (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                    w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                    w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();

                                var ingreso_ubicacion = new cIngresoUbicacion();
                                var ultima_ubicacion = ingreso_ubicacion.ObtenerUltimaUbicacion(ultimo_ingreso.ID_ANIO, ultimo_ingreso.ID_CENTRO, (int)ultimo_ingreso.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                                if (/*ultima_ubicacion.ESTATUS == (short)enumUbicacion.ACTIVIDAD && ultima_ubicacion.ID_AREA == (short)enumAreas.SALIDA_DE_CENTRO && */consulta_excarcelacion != null)
                                {
                                    var excarcelacion_imputado = new ObservableCollection<InternoExcarcelacion>();
                                    excarcelacion_imputado.Add(new InternoExcarcelacion()
                                    {
                                        ID_CENTRO = consulta_excarcelacion.ID_CENTRO,
                                        ID_ANIO = consulta_excarcelacion.ID_ANIO,
                                        ID_IMPUTADO = consulta_excarcelacion.ID_IMPUTADO,
                                        ID_INGRESO = consulta_excarcelacion.ID_INGRESO,
                                        PATERNO = consulta_excarcelacion.INGRESO.IMPUTADO.PATERNO.TrimEnd(),
                                        MATERNO = consulta_excarcelacion.INGRESO.IMPUTADO.MATERNO.TrimEnd(),
                                        NOMBRE = consulta_excarcelacion.INGRESO.IMPUTADO.NOMBRE.TrimEnd(),
                                        PERMITIR = true,
                                        HABILITAR = true
                                    });
                                    Excarcelacion = excarcelacion_imputado;
                                    SelectedImputado = Excarcelacion.FirstOrDefault();
                                    if (!consulta_excarcelacion.CERT_MEDICO_SALIDA.HasValue && consulta_excarcelacion.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO)
                                    {
                                        CapturaIncidenciaVisible = true;
                                    }
                                    else
                                    {
                                        new cExcarcelacion().IniciarExcarcelacion(new EXCARCELACION()
                                        {
                                            ID_CENTRO = consulta_excarcelacion.ID_CENTRO,
                                            ID_ANIO = consulta_excarcelacion.ID_ANIO,
                                            ID_IMPUTADO = consulta_excarcelacion.ID_IMPUTADO,
                                            ID_INGRESO = consulta_excarcelacion.ID_INGRESO,
                                            ID_CONSEC = consulta_excarcelacion.ID_CONSEC,
                                            ID_ESTATUS = EXCARCELACION_ACTIVA,
                                            SALIDA_FEC = Fechas.GetFechaDateServer
                                        });
                                    }
                                    var huella_excarcelacion = HuellasImputados.Where(w =>
                                                w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                                    HuellasImputados.Remove(huella_excarcelacion);
                                    var placeholder = new Imagenes().getImagenPerson();
                                    var foto_ingreso = ultimo_ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                    var foto_centro = ultimo_ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                    FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                                    FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                                    MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                                }
                                else
                                {
                                    MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                                }
                            }
                            else
                            {
                                MensajeResultado = enumMensajeResultadoComparacion.COINCIDENCIAS;
                            }
                        }
                        else
                        {
                            MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new ApplicationException(ex.Message);
                }
                //SE MUESTRA EL MENSAJE INDICADOR DE RESULTADO DE OPERACIÓN
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));
            }
        }

        public void CompararNIPImputado()
        {

            var MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
            try
            {
                ValidacionNIPAcceso();
                if (!base.HasErrors)
                {


                    var imputado = new cImputado().ObtenerPorNIP(NIPBuscar);
                    if (imputado != null)
                    {
                        var excarcelacion = new cExcarcelacion();
                        var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, (short)imputado.ID_IMPUTADO);
                        var consulta_excarcelacion = excarcelacion.ObtenerImputadoExcarcelaciones(
                                        ultimo_ingreso.ID_CENTRO,
                                        ultimo_ingreso.ID_ANIO,
                                        ultimo_ingreso.ID_IMPUTADO,
                                        ultimo_ingreso.ID_INGRESO).Where(w =>
                                        (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO ||
                                        w.ID_ESTATUS == EXCARCELACION_AUTORIZADA) &&
                                        (w.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                                        w.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                                        w.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day)).FirstOrDefault();

                        var ingreso_ubicacion = new cIngresoUbicacion();
                        var ultima_ubicacion = ingreso_ubicacion.ObtenerUltimaUbicacion(ultimo_ingreso.ID_ANIO, ultimo_ingreso.ID_CENTRO, (int)ultimo_ingreso.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                        if (/*ultima_ubicacion.ESTATUS == (short)enumUbicacion.ACTIVIDAD && ultima_ubicacion.ID_AREA == (short)enumAreas.SALIDA_DE_CENTRO &&*/ultimo_ingreso.ID_UB_CENTRO.HasValue &&
                            ultimo_ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro &&
                            consulta_excarcelacion != null)
                        {
                            var excarcelacion_imputado = new ObservableCollection<InternoExcarcelacion>();
                            excarcelacion_imputado.Add(new InternoExcarcelacion()
                            {
                                ID_CENTRO = consulta_excarcelacion.ID_CENTRO,
                                ID_ANIO = consulta_excarcelacion.ID_ANIO,
                                ID_IMPUTADO = consulta_excarcelacion.ID_IMPUTADO,
                                ID_INGRESO = consulta_excarcelacion.ID_INGRESO,
                                PATERNO = consulta_excarcelacion.INGRESO.IMPUTADO.PATERNO.TrimEnd(),
                                MATERNO = consulta_excarcelacion.INGRESO.IMPUTADO.MATERNO.TrimEnd(),
                                NOMBRE = consulta_excarcelacion.INGRESO.IMPUTADO.NOMBRE.TrimEnd(),
                                PERMITIR = true,
                                HABILITAR = true
                            });
                            Excarcelacion = excarcelacion_imputado;
                            SelectedImputado = Excarcelacion.FirstOrDefault();
                            if (!consulta_excarcelacion.CERT_MEDICO_SALIDA.HasValue && consulta_excarcelacion.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO)
                            {
                                CapturaIncidenciaVisible = true;
                            }
                            else
                            {
                                new cExcarcelacion().IniciarExcarcelacion(new EXCARCELACION()
                                {
                                    ID_CENTRO = consulta_excarcelacion.ID_CENTRO,
                                    ID_ANIO = consulta_excarcelacion.ID_ANIO,
                                    ID_IMPUTADO = consulta_excarcelacion.ID_IMPUTADO,
                                    ID_INGRESO = consulta_excarcelacion.ID_INGRESO,
                                    ID_CONSEC = consulta_excarcelacion.ID_CONSEC,
                                    ID_ESTATUS = EXCARCELACION_ACTIVA,
                                    SALIDA_FEC = Fechas.GetFechaDateServer
                                });
                            }
                            var huella_excarcelacion = HuellasImputados.Where(w =>
                                                w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                            HuellasImputados.Remove(huella_excarcelacion);
                            var placeholder = new Imagenes().getImagenPerson();
                            var foto_ingreso = ultimo_ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                            var foto_centro = ultimo_ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                            FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                            FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                            MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                        }
                        else
                        {
                            MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                        }
                    }
                    else
                    {
                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                    }
                }
                CambiarMensajeNIP(MensajeResultado);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }

        }

        public void RevertirUltimoMovimiento(EXCARCELACION Excarcelacion, INCIDENTE Incidente)
        {
            try
            {
                new cExcarcelacion().RevertirUltimoMovimiento(new EXCARCELACION()
                {
                    ID_CENTRO = Excarcelacion.ID_CENTRO,
                    ID_ANIO = Excarcelacion.ID_ANIO,
                    ID_IMPUTADO = Excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = Excarcelacion.ID_INGRESO,
                    ID_CONSEC = Excarcelacion.ID_CONSEC,
                    ID_ESTATUS = EXCARCELACION_EN_PROCESO,
                    ID_INCIDENCIA_TRASLADO = (short?)null,
                    INCIDENCIA_OBSERVACION = null,
                    RESPONSABLE = null,
                    SALIDA_FEC = null
                }, new INCIDENTE()
                {
                    ID_CENTRO = Incidente.ID_CENTRO,
                    ID_ANIO = Incidente.ID_ANIO,
                    ID_IMPUTADO = Incidente.ID_IMPUTADO,
                    ID_INGRESO = Incidente.ID_INGRESO,
                    ID_INCIDENTE = Incidente.ID_INCIDENTE,
                    ID_INCIDENTE_TIPO = Incidente.ID_INCIDENTE_TIPO,
                    REGISTRO_FEC = Incidente.REGISTRO_FEC,
                    MOTIVO = Incidente.MOTIVO
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void RevertirExcarcelacion(EXCARCELACION Excarcelacion, INCIDENTE Incidente)
        {
            try
            {
                new cExcarcelacion().RevertirExcarcelacion(new INGRESO_UBICACION()
                {
                    ID_CENTRO = Excarcelacion.ID_CENTRO,
                    ID_ANIO = Excarcelacion.ID_ANIO,
                    ID_IMPUTADO = Excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = Excarcelacion.ID_INGRESO,
                    ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(Excarcelacion.ID_CENTRO, Excarcelacion.ID_ANIO, Excarcelacion.ID_IMPUTADO, Excarcelacion.ID_INGRESO),
                    ID_AREA = (short)enumAreas.ESTANCIA,
                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                    ACTIVIDAD = "ESTANCIA",
                    ESTATUS = (short)enumUbicacion.ESTANCIA
                }, new EXCARCELACION()
                {
                    ID_CENTRO = Excarcelacion.ID_CENTRO,
                    ID_ANIO = Excarcelacion.ID_ANIO,
                    ID_IMPUTADO = Excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = Excarcelacion.ID_INGRESO,
                    ID_CONSEC = Excarcelacion.ID_CONSEC,
                    ID_ESTATUS = EXCARCELACION_AUTORIZADA,
                    ID_INCIDENCIA_TRASLADO = (short?)null,
                    INCIDENCIA_OBSERVACION = null,
                    RESPONSABLE = null,
                    SALIDA_FEC = null
                }, new INCIDENTE()
                {
                    ID_CENTRO = Incidente.ID_CENTRO,
                    ID_ANIO = Incidente.ID_ANIO,
                    ID_IMPUTADO = Incidente.ID_IMPUTADO,
                    ID_INGRESO = Incidente.ID_INGRESO,
                    ID_INCIDENTE = Incidente.ID_INCIDENTE,
                    ID_INCIDENTE_TIPO = Incidente.ID_INCIDENTE_TIPO,
                    REGISTRO_FEC = Incidente.REGISTRO_FEC,
                    MOTIVO = Incidente.MOTIVO
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void GuardarResponsable(EXCARCELACION Excarcelacion)
        {
            try
            {
                new cExcarcelacion().CambiarResponsable(new EXCARCELACION()
                {
                    ID_CENTRO = Excarcelacion.ID_CENTRO,
                    ID_ANIO = Excarcelacion.ID_ANIO,
                    ID_IMPUTADO = Excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = Excarcelacion.ID_INGRESO,
                    ID_CONSEC = Excarcelacion.ID_CONSEC,
                    RESPONSABLE = Excarcelacion.RESPONSABLE
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void LimpiarResponsable(EXCARCELACION Excarcelacion)
        {
            try
            {
                new cExcarcelacion().CambiarResponsable(new EXCARCELACION()
                {
                    ID_CENTRO = Excarcelacion.ID_CENTRO,
                    ID_ANIO = Excarcelacion.ID_ANIO,
                    ID_IMPUTADO = Excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = Excarcelacion.ID_INGRESO,
                    ID_CONSEC = Excarcelacion.ID_CONSEC,
                    RESPONSABLE = null
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Métodos Mensajes Interfaz
        /// <summary>
        /// Notifica al usuario del resultado obtenido por la comparación y decisión del método de comparacion de huellas.
        /// </summary>
        /// <param name="MensajeResultado">Tipo de mensaje a indicar</param>
        public async void CambiarMensaje(enumMensajeResultadoComparacion MensajeResultado)
        {
            switch (MensajeResultado)
            {
                case enumMensajeResultadoComparacion.NO_ENCONTRADO:
                    ScannerMessage = "NO PERMITIDO";
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    ImagenEvaluacion = new Imagenes().getImagenDenegado();
                    break;
                case enumMensajeResultadoComparacion.PROCESANDO:
                    ScannerMessage = "PROCESANDO...";
                    ColorAprobacion = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                    break;
                case enumMensajeResultadoComparacion.COINCIDENCIAS:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    ColorAprobacion = new SolidColorBrush(Colors.Yellow);
                    ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                    break;
                case enumMensajeResultadoComparacion.ENCONTRADO:
                    ScannerMessage = "PERMITIDO";
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    ImagenEvaluacion = new Imagenes().getImagenPermitido();
                    break;
                case enumMensajeResultadoComparacion.OPERACION_LECTOR_INCORRECTA:
                    ScannerMessage = "LECTURA FALLIDA";
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                    break;
            }
            var image_retriever = new Imagenes();
            ProgressRingVisible = Visibility.Collapsed;
            ImagenEvaluacionVisible = true;
            await TaskEx.Delay(1500);
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ImagenEvaluacion = image_retriever.getImagenHuella();
            ScannerMessage = "Capture Huella\n en el lector";
        }


        public async void CambiarMensajeNIP(enumMensajeResultadoComparacion MensajeResultado)
        {
            switch (MensajeResultado)
            {
                case enumMensajeResultadoComparacion.NO_ENCONTRADO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "X";
                    break;

                case enumMensajeResultadoComparacion.ENCONTRADO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "\u2713 \u2713";
                    CapturaNIPVisible = false;
                    break;
            }
            await TaskEx.Delay(1500);
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            CheckMark = "🔍";
        }
        #endregion
    }
}
