using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class BusquedaHuellaTraslado : FingerPrintScanner
    {
        #region Constructor
        //MÉTODO CONSTRUCTOR
        public BusquedaHuellaTraslado(TRASLADO SelectedTraslado, MetroWindow ventana)
        {
            this.SelectedTraslado = SelectedTraslado;
            Ventana = ventana;
            Ventana.Closing += (s, e) =>
            {
                e.Cancel = CapturaIncidenciaVisible ? true : (IncidenciaRevertirTrasladoVisible ? true : false);
            };


            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;
            var lista_traslados = new ObservableCollection<InternoTraslado>();
            try
            {
                foreach (var traslado_detalle in SelectedTraslado.TRASLADO_DETALLE)
                {

                    var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(
                            traslado_detalle.ID_ANIO,
                            traslado_detalle.ID_CENTRO,
                            traslado_detalle.ID_IMPUTADO,
                            traslado_detalle.ID_INGRESO);
                    //REVISA EL ESTATUS ADMINISTRATIVO DEL IMPUTADO PARA SABER SI SE AGREGARÁ A LA LISTA
                    if (traslado_detalle.INGRESO.ESTATUS_ADMINISTRATIVO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                        traslado_detalle.INGRESO.ESTATUS_ADMINISTRATIVO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                        traslado_detalle.INGRESO.ESTATUS_ADMINISTRATIVO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                        traslado_detalle.INGRESO.ESTATUS_ADMINISTRATIVO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL)
                        lista_traslados.Add(new InternoTraslado()
                        {
                            ID_ANIO = traslado_detalle.ID_ANIO,
                            ID_CENTRO = traslado_detalle.ID_CENTRO,
                            ID_IMPUTADO = traslado_detalle.ID_IMPUTADO,
                            ID_INGRESO = traslado_detalle.ID_INGRESO,
                            ID_TRASLADO = traslado_detalle.ID_TRASLADO,
                            NOMBRE = traslado_detalle.INGRESO.IMPUTADO.NOMBRE.TrimEnd(),
                            PATERNO = traslado_detalle.INGRESO.IMPUTADO.PATERNO.TrimEnd(),
                            MATERNO = traslado_detalle.INGRESO.IMPUTADO.MATERNO.TrimEnd(),
                            PERMITIR = false,
                            HABILITAR = false//ultima_ubicacion != null ? (ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_SALIDA_DE_CENTRO && ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO) : false
                        });
                }
                //obtiene huellas del imputado.
                HuellasImputados = SelectedTraslado.TRASLADO_DETALLE.Where(w =>
                    w.ID_ESTATUS == TRASLADO_EN_PROCESO || w.ID_ESTATUS == TRASLADO_PROGRAMADO).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                        new Imputado_Huella
                                        {
                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                        })
                                .ToList();

                ListaTrasladoInternos = lista_traslados;
                EmptyVisible = ListaTrasladoInternos.Count == 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            var image_retriever = new Imagenes();
            ImagenEvaluacionVisible = true;
            ImagenEvaluacion = image_retriever.getImagenHuella();
            FotoIngreso = image_retriever.getImagenPerson();
            FotoCentro = image_retriever.getImagenPerson();
            ScannerMessage = "Capture Huella\n en el lector";
            ProgressRingVisible = Visibility.Collapsed;
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            CampoCaptura = "NIP:";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ScannerMessageVisible = Readers.Count > 0;
            EstatusLector = Readers.Count > 0 ? LECTOR_DETECTADO : LECTOR_DESCONECTADO;
            ColorEstatusLector = new SolidColorBrush(EstatusLector == LECTOR_DETECTADO ? Colors.Green : Colors.Red);
            TextoSeleccionDedo = Readers.Count > 0 ? "Seleccione Tipo Biométrico:" : "Selección no Disponible";
            NIPBuscar = "";
            TextoIncidencia = "SIN OBSERVACIONES";
            TextoIncidenciaCertificadoMedico = "SIN OBSERVACIONES";
            FotoTrasladoUltimoImputado = image_retriever.getImagenPerson();
        }
        #endregion

        #region Métodos Eventos
        //MÉTODO PARA ELECCIÓN DE EVENTO SOBRE BOTONES
        public async void ClickSwitch(object obj)
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
                case "ObtenerFotoImputado":
                    try
                    {
                        var placeholder = new Imagenes().getImagenPerson();
                        var consulta_imputado = new cIngreso().Obtener((short)SelectedImputado.ID_CENTRO, (short)SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, (short)SelectedImputado.ID_INGRESO);
                        var foto_ingreso = consulta_imputado.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                        var foto_centro = consulta_imputado.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                        FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "GuardarNombreResponsableTraslado":
                    ValidacionResponsable();
                    if (!base.HasErrors)
                    {
                        Ventana.Hide();
                        var MensajeGuardar = Ventana as MetroWindow;
                        var mySettingsGuardar = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Aceptar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        try
                        {
                            SelectedTraslado.RESPONSABLE = string.Format("{1} {2} {0}", NombreResponsableTraslado, ApellidoPaternoResponsableTraslado, ApellidoMaternoResponsableTraslado);
                            var traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                            var verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                                (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO || c.ID_ESTATUS == TRASLADO_ACTIVO || c.ID_ESTATUS == TRASLADO_FINALIZADO)) == 1;
                            if (verifica_traslado && traslado.ID_ESTATUS == TRASLADO_ACTIVO || traslado.ID_ESTATUS == TRASLADO_FINALIZADO)
                            {
                                var metro = Application.Current.Windows[0] as MetroWindow;
                                var mySettings = new MetroDialogSettings()
                                {
                                    AffirmativeButtonText = "Cerrar",
                                    AnimateShow = true,
                                    AnimateHide = false
                                };
                                await metro.ShowMessageAsync("Validación", "Este traslado ya se encuentra ACTIVO y no puede modificarse.", MessageDialogStyle.Affirmative, mySettings);
                            }
                            else
                            {
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    if (verifica_traslado && HuellasImputados.Count == 0)
                                    {
                                        //if (SelectedImputado == null)
                                        //{
                                        //    if (SelectedTraslado.TRASLADO_DETALLE != null)
                                        //    {
                                        //        if (SelectedTraslado.TRASLADO_DETALLE != null)
                                        //        {
                                        //            var td = SelectedTraslado.TRASLADO_DETALLE.FirstOrDefault();
                                        //            if (td != null)
                                        //            { 
                                        //                SelectedImputado =
                                        //                      SelectedImputado = ListaTrasladoInternos.Where(w =>
                                        //                        w.ID_CENTRO == td.ID_CENTRO &&
                                        //                        w.ID_ANIO == td.ID_ANIO &&
                                        //                        w.ID_IMPUTADO == td.ID_IMPUTADO).FirstOrDefault();
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                        SelectedTraslado.RESPONSABLE = string.Format("{1} {2} {0}", NombreResponsableTraslado, ApellidoPaternoResponsableTraslado, ApellidoMaternoResponsableTraslado);
                                        if (SelectedImputado != null)
                                        {
                                            FinalizarTraslado(SelectedTraslado);
                                            CapturaFinalizarTrasladoVisible = false;
                                            TextoIncidenciaCertificadoMedico = "SIN OBSERVACIONES";
                                        }
                                        else
                                        {
                                            new cTraslado().ModificarResponsable(new TRASLADO()
                                            {
                                                ID_TRASLADO = SelectedTraslado.ID_TRASLADO,
                                                ID_CENTRO = SelectedTraslado.ID_CENTRO,
                                                RESPONSABLE = SelectedTraslado.RESPONSABLE
                                            });
                                        }
                                    }
                                    else
                                    {
                                        new cTraslado().ModificarResponsable(new TRASLADO()
                                        {
                                            ID_TRASLADO = SelectedTraslado.ID_TRASLADO,
                                            ID_CENTRO = SelectedTraslado.ID_CENTRO,
                                            RESPONSABLE = SelectedTraslado.RESPONSABLE
                                        });

                                    }
                                });
                                traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                                verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                                    (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO)) == 0;
                                if (verifica_traslado)
                                {
                                    Ventana.Close();
                                }
                                else
                                {
                                    Ventana.Show();
                                    await MensajeGuardar.ShowMessageAsync("ÉXITO", "Se guardo correctamente la información del responsable.", MessageDialogStyle.Affirmative, mySettingsGuardar);
                                    //NombreResponsableTraslado = "";
                                    //ApellidoPaternoResponsableTraslado = "";
                                    //ApellidoMaternoResponsableTraslado = "";
                                }
                            }

                        }
                        catch (Exception ex)
                        {

                            throw new ApplicationException(ex.Message);
                        }
                    }
                    break;
                case "LimpiarNombreResponsableTraslado":
                    var MensajeLimpiar = Ventana as MetroWindow;
                    var mySettingsLimpiar = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Aceptar",
                        AnimateShow = true,
                        AnimateHide = false
                    };
                    try
                    {
                        Ventana.Hide();
                        SelectedTraslado.RESPONSABLE = null;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {

                            new cTraslado().ModificarResponsable(new TRASLADO()
                            {
                                ID_TRASLADO = SelectedTraslado.ID_TRASLADO,
                                ID_CENTRO = SelectedTraslado.ID_CENTRO
                            });
                        });
                        Ventana.Show();
                        await MensajeLimpiar.ShowMessageAsync("ÉXITO", "Se borró correctamente la información del responsable.", MessageDialogStyle.Affirmative, mySettingsLimpiar);

                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "PermitirTraslado":

                    if (SelectedImputado.PERMITIR)
                    {
                        //var MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                        var Ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                        var Traslado_Detalle = new cTrasladoDetalle().ObtenerTraslado(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, SelectedImputado.ID_INGRESO);
                        if (Traslado_Detalle != null)
                        {
                            var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(Ingreso.ID_ANIO, Ingreso.ID_CENTRO, (int)Ingreso.ID_IMPUTADO, Ingreso.ID_INGRESO);
                            if (ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO && ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_SALIDA_DE_CENTRO)
                            {

                                SelectedImputado = ListaTrasladoInternos.Where(w =>
                                    w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                                    w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                                    w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO).FirstOrDefault();

                                if (Traslado_Detalle.ID_ATENCION_MEDICA != null)
                                {

                                    var traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                                    var verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                                        (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO)) == 1;

                                    if (verifica_traslado)
                                    {
                                        var foto_ingreso_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                        var foto_centro_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                        FotoTrasladoUltimoImputado = foto_centro_ultimo_imputado != null ? foto_centro_ultimo_imputado.BIOMETRICO : (foto_ingreso_ultimo_imputado != null ? foto_ingreso_ultimo_imputado.BIOMETRICO : new Imagenes().getImagenPerson());
                                        NombreUltimoInterno = string.Format("{1} {2} {0}", SelectedImputado.NOMBRE, SelectedImputado.PATERNO, SelectedImputado.MATERNO);
                                        CapturaFinalizarTrasladoVisible = true;
                                    }
                                    else
                                    {
                                        ProcesarTrasladoDetalle(Traslado_Detalle, Ingreso);
                                    }
                                }
                                else
                                {
                                    CapturaIncidenciaVisible = true;
                                }
                                var huella_traslado = HuellasImputados.Where(w =>
                                       w.IMPUTADO.ID_CENTRO == Ingreso.ID_CENTRO &&
                                       w.IMPUTADO.ID_ANIO == Ingreso.ID_ANIO &&
                                       w.IMPUTADO.ID_IMPUTADO == Ingreso.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputados.Remove(huella_traslado);
                                SelectedImputado.PERMITIR = true;

                                var placeholder = new Imagenes().getImagenPerson();

                                var foto_ingreso = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                                var foto_centro = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                                //MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                                ListaTrasladoInternos = new ObservableCollection<InternoTraslado>(ListaTrasladoInternos);
                            }
                            else
                            {
                                SelectedImputado.PERMITIR = false;
                            }
                        }
                    }
                    else
                    {
                        TextoBotonCancelar = "Cancelar";
                        IncidenciaRevertirTrasladoVisible = true;
                    }
                    break;
                case "CerrarIncidenciaFlyout":
                    try
                    {
                        var Ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                        var Traslado_Detalle = new cTrasladoDetalle().ObtenerTraslado(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, SelectedImputado.ID_INGRESO);
                        Traslado_Detalle.ID_INCIDENCIA_TRASLADO = (short)enumIncidencias.CERTIFICADO_MEDICO_AUSENTE;
                        Traslado_Detalle.INCIDENCIA_OBSERVACION = TextoIncidenciaCertificadoMedico;

                        var traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                        var verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                            (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO)) == 1;


                        CapturaIncidenciaVisible = false;
                        if (verifica_traslado)
                        {
                            var foto_ingreso_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                            var foto_centro_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                            FotoTrasladoUltimoImputado = foto_centro_ultimo_imputado != null ? foto_centro_ultimo_imputado.BIOMETRICO : (foto_ingreso_ultimo_imputado != null ? foto_ingreso_ultimo_imputado.BIOMETRICO : new Imagenes().getImagenPerson());
                            NombreUltimoInterno = string.Format("{1} {2} {0}", SelectedImputado.NOMBRE, SelectedImputado.PATERNO, SelectedImputado.MATERNO);
                            CapturaFinalizarTrasladoVisible = true;
                        }
                        else
                        {
                            ProcesarTrasladoDetalle(Traslado_Detalle, Ingreso);

                            TextoIncidenciaCertificadoMedico = "SIN OBSERVACIONES";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "FinalizarTraslado":
                    if (!SelectedImputado.PERMITIR)
                    {
                        IncidenciaRevertirTrasladoVisible = false;
                        SelectedImputado.PERMITIR = true;
                        var lista_traslado_internos = new ObservableCollection<InternoTraslado>();
                        var selected_imputado = new InternoTraslado()
                        {
                            ID_CENTRO = SelectedImputado.ID_CENTRO,
                            ID_ANIO = SelectedImputado.ID_ANIO,
                            ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                            ID_INGRESO = SelectedImputado.ID_INGRESO,
                            PERMITIR = SelectedImputado.PERMITIR,
                        };
                        foreach (var interno in ListaTrasladoInternos)
                        {
                            lista_traslado_internos.Add(interno);
                        }
                        ListaTrasladoInternos = lista_traslado_internos;
                        SelectedImputado = ListaTrasladoInternos.Where(w =>
                            w.ID_CENTRO == selected_imputado.ID_CENTRO &&
                            w.ID_ANIO == selected_imputado.ID_ANIO &&
                            w.ID_IMPUTADO == selected_imputado.ID_IMPUTADO &&
                            w.ID_INGRESO == selected_imputado.ID_INGRESO).
                            FirstOrDefault();
                    }
                    else
                    {
                        try
                        {
                            var responsable = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO).RESPONSABLE;
                            if (responsable != null)
                            {
                                FinalizarTraslado(SelectedTraslado);
                                var huella_traslado = HuellasImputados.Where(w =>
                                                w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputados.Remove(huella_traslado);
                                TextoIncidenciaCertificadoMedico = "SIN OBSERVACIONES";
                                Ventana.Close();
                            }
                            else if ((!string.IsNullOrEmpty(NombreResponsableTraslado) && !string.IsNullOrEmpty(ApellidoPaternoResponsableTraslado) && !string.IsNullOrEmpty(ApellidoMaternoResponsableTraslado)))
                            {
                                SelectedTraslado.RESPONSABLE = string.Format("{1} {2} {0}", NombreResponsableTraslado, ApellidoPaternoResponsableTraslado, ApellidoMaternoResponsableTraslado);
                                FinalizarTraslado(SelectedTraslado);
                                var huella_traslado = HuellasImputados.Where(w =>
                                                w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputados.Remove(huella_traslado);
                                TextoIncidenciaCertificadoMedico = "SIN OBSERVACIONES";
                                Ventana.Close();
                            }
                            else
                            {
                                ValidacionResponsable();
                                var MensajeGuardarResponsableFinalizarTraslado = Ventana as MetroWindow;
                                var mySettingsGuardarResponsableFinalizarTraslado = new MetroDialogSettings()
                                {
                                    AffirmativeButtonText = "Aceptar",
                                    AnimateShow = true,
                                    AnimateHide = false
                                };
                                await MensajeGuardarResponsableFinalizarTraslado.ShowMessageAsync("Validación", "Debe guardar la información del responsable antes de finalizar el traslado.", MessageDialogStyle.Affirmative, mySettingsGuardarResponsableFinalizarTraslado);
                                if (CapturaFinalizarTrasladoVisible)
                                    CapturaFinalizarTrasladoVisible = false;
                                else
                                    IncidenciaRevertirTrasladoVisible = false;
                                (Ventana as BusquedaHuellaTrasladoView).TextBoxNombre.Focus();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                    }
                    break;
                case "Revertir":
                    CapturaFinalizarTrasladoVisible = false;
                    TextoBotonCancelar = "Cancelar y Finalizar";
                    IncidenciaRevertirTrasladoVisible = true;
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
                                    var ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                                    var traslado_detalle = new cTrasladoDetalle().Obtener(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO, TRASLADO_ACTIVO);
                                    var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                                    var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                    var incidente = new INCIDENTE()
                                    {
                                        ID_CENTRO = imputado_incidente.ID_CENTRO,
                                        ID_ANIO = imputado_incidente.ID_ANIO,
                                        ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                                        ID_INGRESO = ingreso_incidente,
                                        ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                                        ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                                        MOTIVO = TextoIncidencia
                                    };

                                    if (traslado_detalle == null)
                                    {
                                        new cIncidente().Insert(incidente);
                                    }
                                    else
                                        RevertirUltimoMovimiento(ingreso, traslado_detalle, incidente);

                                    SelectedImputado.PERMITIR = false;
                                    SelectedImputado.HABILITAR = false;
                                    var lista_traslado_internos = new ObservableCollection<InternoTraslado>();
                                    var selected_imputado = new InternoTraslado()
                                    {
                                        ID_CENTRO = SelectedImputado.ID_CENTRO,
                                        ID_ANIO = SelectedImputado.ID_ANIO,
                                        ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                                        ID_INGRESO = SelectedImputado.ID_INGRESO,
                                        PERMITIR = SelectedImputado.PERMITIR,
                                    };
                                    foreach (var interno in ListaTrasladoInternos)
                                    {
                                        lista_traslado_internos.Add(interno);
                                    }
                                    ListaTrasladoInternos = lista_traslado_internos;
                                    SelectedImputado = ListaTrasladoInternos.Where(w =>
                                        w.ID_CENTRO == selected_imputado.ID_CENTRO &&
                                        w.ID_ANIO == selected_imputado.ID_ANIO &&
                                        w.ID_IMPUTADO == selected_imputado.ID_IMPUTADO &&
                                        w.ID_INGRESO == selected_imputado.ID_INGRESO).
                                        FirstOrDefault();
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
                                    IncidenciaRevertirTrasladoVisible = false;
                                    TextoIncidencia = "SIN OBSERVACIONES";
                                    IncidenciaNIP = "";
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message);
                                }
                            }
                        }

                    }
                    break;
                case "CapturarIncidenciaRevertirTraslado":
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
                                    var ingreso_revertir_traslado = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                                    var traslado_detalle_revertir_traslado = new cTrasladoDetalle().Obtener(ingreso_revertir_traslado.ID_CENTRO, ingreso_revertir_traslado.ID_ANIO, ingreso_revertir_traslado.ID_IMPUTADO, ingreso_revertir_traslado.ID_INGRESO, TRASLADO_ACTIVO);
                                    var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                                    var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
                                    var incidente = new INCIDENTE()
                                    {
                                        ID_CENTRO = imputado_incidente.ID_CENTRO,
                                        ID_ANIO = imputado_incidente.ID_ANIO,
                                        ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                                        ID_INGRESO = ingreso_incidente,
                                        ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                                        ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                                        REGISTRO_FEC = Fechas.GetFechaDateServer,
                                        MOTIVO = TextoIncidencia
                                    };

                                    if (traslado_detalle_revertir_traslado == null)
                                    {
                                        traslado_detalle_revertir_traslado = new cTrasladoDetalle().Obtener(ingreso_revertir_traslado.ID_CENTRO, ingreso_revertir_traslado.ID_ANIO, ingreso_revertir_traslado.ID_IMPUTADO, ingreso_revertir_traslado.ID_INGRESO, TRASLADO_EN_PROCESO);
                                        RevertirTraslado(traslado_detalle_revertir_traslado, incidente);
                                    }
                                    else
                                        RevertirTraslado(traslado_detalle_revertir_traslado, incidente, ingreso_revertir_traslado);

                                    SelectedImputado.PERMITIR = false;
                                    SelectedImputado.HABILITAR = false;
                                    var lista_traslado_internos_revertir_traslado = new ObservableCollection<InternoTraslado>();
                                    var selected_imputado_revertir_traslado = new InternoTraslado()
                                    {
                                        ID_CENTRO = SelectedImputado.ID_CENTRO,
                                        ID_ANIO = SelectedImputado.ID_ANIO,
                                        ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                                        ID_INGRESO = SelectedImputado.ID_INGRESO,
                                        PERMITIR = SelectedImputado.PERMITIR,
                                        HABILITAR = SelectedImputado.HABILITAR
                                    };
                                    foreach (var interno in ListaTrasladoInternos)
                                    {
                                        lista_traslado_internos_revertir_traslado.Add(interno);
                                    }
                                    ListaTrasladoInternos = lista_traslado_internos_revertir_traslado;
                                    SelectedImputado = ListaTrasladoInternos.Where(w =>
                                        w.ID_CENTRO == selected_imputado_revertir_traslado.ID_CENTRO &&
                                        w.ID_ANIO == selected_imputado_revertir_traslado.ID_ANIO &&
                                        w.ID_IMPUTADO == selected_imputado_revertir_traslado.ID_IMPUTADO &&
                                        w.ID_INGRESO == selected_imputado_revertir_traslado.ID_INGRESO).
                                        FirstOrDefault();
                                    var huella_revertir_traslado = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO).
                                                    IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                                    wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                                    wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                        new Imputado_Huella
                                                        {
                                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                        }).FirstOrDefault();
                                    if (huella_revertir_traslado != null)
                                        HuellasImputados.Add(huella_revertir_traslado);
                                    IncidenciaRevertirTrasladoVisible = false;
                                    TextoIncidencia = "SIN OBSERVACIONES";
                                    IncidenciaNIP = "";
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message);
                                }
                            }
                        }

                    }
                    break;
            }
        }

        //MÉTODO PARA EVENTO - EJECUTAR AL CARGAR LA VENTANA
        public void OnLoad(BusquedaHuellaTrasladoView Window)
        {
            Window.Closed += (s, e) =>
            {

                try
                {
                    if (OnProgress == null)
                        return;

                    if (!IsSucceed)
                        //SelectedCustodio = null;

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

        public override void OnCaptured(CaptureResult captureResult)
        {
            if (Readers.Count > 0)
            {
                ImagenEvaluacionVisible = false;
                ProgressRingVisible = Visibility.Visible;
                base.OnCaptured(captureResult);
                CompararHuellaImputado();

            }
            else
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    EstatusLector = LECTOR_DESCONECTADO;
                    ColorEstatusLector = new SolidColorBrush(Colors.Red);
                    ScannerMessageVisible = false;
                }));
            }

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
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message); ;
            }
        }
        #endregion

        #region Métodos
        public void CompararHuellaImputado()
        {
            var MensajeResultado = enumMensajeResultadoComparacion.HUELLA_VACIA;
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            if (bytesHuella == null)
            {
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
                    var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputados.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                    var result = new List<object>();

                    if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        //TODO: CASOS DEL RESULTADO DE LA OPERACIÓN DEL LECTOR DE HUELLAS
                        switch (doIdentify.ResultCode)
                        {
                            #region Resultados Operacion Lector
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
                            case Constants.ResultCode.DP_SUCCESS:
                                break;
                            case Constants.ResultCode.DP_TOO_SMALL_AREA:
                                break;
                            case Constants.ResultCode.DP_VERSION_INCOMPATIBILITY:
                                break;
                            default:
                                break;
                            #endregion
                        }
                    }
                    else
                    {
                        if (doIdentify.Indexes.Count() > 0)
                        {
                            foreach (var resultado in doIdentify.Indexes.ToList())
                                result.Add(HuellasImputados[resultado.FirstOrDefault()].IMPUTADO);
                        }
                    }

                    if (result.Count > 0)
                    {
                        if (result.Count == 1)
                        {
                            try
                            {
                                var imputado = (cHuellasImputado)result.FirstOrDefault();
                                var Ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                                var Traslado_Detalle = new cTrasladoDetalle().ObtenerTraslado(Ingreso.ID_CENTRO, Ingreso.ID_ANIO, Ingreso.ID_IMPUTADO, Ingreso.ID_INGRESO);
                                if (Traslado_Detalle != null)
                                {
                                    var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(Ingreso.ID_ANIO, Ingreso.ID_CENTRO, (int)Ingreso.ID_IMPUTADO, Ingreso.ID_INGRESO);
                                    if (ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO && ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_SALIDA_DE_CENTRO)
                                    {

                                        SelectedImputado = ListaTrasladoInternos.Where(w =>
                                            w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                                            w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                                            w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO).FirstOrDefault();
                                        if (Traslado_Detalle.ID_ATENCION_MEDICA != null)
                                        {
                                            var traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                                            var verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                                                (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO)) == 1;
                                            if (verifica_traslado)
                                            {
                                                var foto_ingreso_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                                var foto_centro_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                                FotoTrasladoUltimoImputado = foto_centro_ultimo_imputado != null ? foto_centro_ultimo_imputado.BIOMETRICO : (foto_ingreso_ultimo_imputado != null ? foto_ingreso_ultimo_imputado.BIOMETRICO : new Imagenes().getImagenPerson());
                                                NombreUltimoInterno = string.Format("{1} {2} {0}", SelectedImputado.NOMBRE, SelectedImputado.PATERNO, SelectedImputado.MATERNO);
                                                CapturaFinalizarTrasladoVisible = true;
                                            }
                                            else
                                            {
                                                ProcesarTrasladoDetalle(Traslado_Detalle, Ingreso);
                                            }
                                        }
                                        else
                                        {
                                            CapturaIncidenciaVisible = true;
                                        }
                                        var huella_traslado = HuellasImputados.Where(w =>
                                               w.IMPUTADO.ID_CENTRO == Ingreso.ID_CENTRO &&
                                               w.IMPUTADO.ID_ANIO == Ingreso.ID_ANIO &&
                                               w.IMPUTADO.ID_IMPUTADO == Ingreso.ID_IMPUTADO).FirstOrDefault();
                                        HuellasImputados.Remove(huella_traslado);
                                        SelectedImputado.PERMITIR = true;
                                        SelectedImputado.HABILITAR = true;
                                        var placeholder = new Imagenes().getImagenPerson();
                                        var foto_ingreso = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                        FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                                        var foto_centro = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                        FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                                        MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                                        ListaTrasladoInternos = new ObservableCollection<InternoTraslado>(ListaTrasladoInternos);
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
                            catch (Exception ex)
                            {
                                throw new ApplicationException(ex.Message);
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
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                CambiarMensaje(MensajeResultado);
            }));
        }

        public void CompararNIPImputado()
        {
            var MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
            try
            {
                var Ingreso = new cImputado().ObtenerPorNIP(NIPBuscar).INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                var Traslado_Detalle = new cTrasladoDetalle().ObtenerTraslado(Ingreso.ID_CENTRO, Ingreso.ID_ANIO, Ingreso.ID_IMPUTADO, Ingreso.ID_INGRESO);
                if (Traslado_Detalle != null)
                {
                    var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(Ingreso.ID_ANIO, Ingreso.ID_CENTRO, (int)Ingreso.ID_IMPUTADO, Ingreso.ID_INGRESO);
                    if (ultima_ubicacion != null && ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO && ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_SALIDA_DE_CENTRO &&
                        Ingreso.ID_UB_CENTRO.HasValue && Ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro)
                    {

                        SelectedImputado = ListaTrasladoInternos.Where(w =>
                            w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                            w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                            w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO).FirstOrDefault();

                        if (Traslado_Detalle.ID_ATENCION_MEDICA != null)
                        {


                            var traslado = new cTraslado().Obtener(SelectedTraslado.ID_CENTRO, SelectedTraslado.ID_TRASLADO);
                            var verifica_traslado = traslado.TRASLADO_DETALLE.AsEnumerable().Count(c =>
                                (c.ID_ESTATUS == TRASLADO_EN_PROCESO || c.ID_ESTATUS == TRASLADO_PROGRAMADO)) == 1;

                            if (verifica_traslado)
                            {
                                var foto_ingreso_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                var foto_centro_ultimo_imputado = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                FotoTrasladoUltimoImputado = foto_centro_ultimo_imputado != null ? foto_centro_ultimo_imputado.BIOMETRICO : (foto_ingreso_ultimo_imputado != null ? foto_ingreso_ultimo_imputado.BIOMETRICO : new Imagenes().getImagenPerson());
                                NombreUltimoInterno = string.Format("{1} {2} {0}", SelectedImputado.NOMBRE, SelectedImputado.PATERNO, SelectedImputado.MATERNO);
                                CapturaFinalizarTrasladoVisible = true;
                            }
                            else
                            {
                                ProcesarTrasladoDetalle(Traslado_Detalle, Ingreso);
                            }

                        }
                        else
                        {
                            CapturaNIPVisible = false;
                            CapturaIncidenciaVisible = true;

                        }
                        var huella_traslado = HuellasImputados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == Ingreso.ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == Ingreso.ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == Ingreso.ID_IMPUTADO).FirstOrDefault();
                        HuellasImputados.Remove(huella_traslado);
                        SelectedImputado.PERMITIR = true;
                        SelectedImputado.HABILITAR = true;
                        var placeholder = new Imagenes().getImagenPerson();

                        var foto_ingreso = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        FotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                        var foto_centro = Ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                        FotoCentro = foto_centro != null ? foto_centro.BIOMETRICO : placeholder;
                        MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                        ListaTrasladoInternos = new ObservableCollection<InternoTraslado>(ListaTrasladoInternos);
                        NIPBuscar = "";
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
                CambiarMensajeNIP(MensajeResultado);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool ProcesarTrasladoDetalle(TRASLADO_DETALLE Traslado_Detalle, INGRESO Ingreso)
        {
            try
            {
                return new cTrasladoDetalle().ProcesarTraslado(
                    new TRASLADO_DETALLE()
                    {
                        ID_TRASLADO = Traslado_Detalle.ID_TRASLADO,
                        ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                        ID_ANIO = Traslado_Detalle.ID_ANIO,
                        ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                        ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                        ID_ESTATUS = Traslado_Detalle.TRASLADO.CENTRO_DESTINO == null ? TRASLADO_FINALIZADO : TRASLADO_ACTIVO,
                        ID_ESTATUS_ADMINISTRATIVO = Ingreso.ID_ESTATUS_ADMINISTRATIVO,
                        ID_CENTRO_TRASLADO = Traslado_Detalle.ID_CENTRO_TRASLADO,
                        ID_INCIDENCIA_TRASLADO = Traslado_Detalle.ID_INCIDENCIA_TRASLADO,
                        INCIDENCIA_OBSERVACION = Traslado_Detalle.INCIDENCIA_OBSERVACION,
                        EGRESO_FEC = Fechas.GetFechaDateServer
                    },
                    new INGRESO()
                    {
                        ID_CENTRO = Ingreso.ID_CENTRO,
                        ID_ANIO = Ingreso.ID_ANIO,
                        ID_IMPUTADO = Ingreso.ID_IMPUTADO,
                        ID_INGRESO = Ingreso.ID_INGRESO,
                        ID_ESTATUS_ADMINISTRATIVO = TRASLADADO
                    },
                    Fechas.GetFechaDateServer
                    );
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool FinalizarTraslado(TRASLADO Traslado)
        {
            try
            {
                var Ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
                var Traslado_Detalle = new cTrasladoDetalle().ObtenerTraslado(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO, SelectedImputado.ID_INGRESO);
                return new cTraslado().FinalizarTraslado(new TRASLADO()
                {
                    ID_CENTRO = Traslado.ID_CENTRO,
                    ID_TRASLADO = Traslado.ID_TRASLADO,
                    ID_ESTATUS = TRASLADO_ACTIVO,
                    RESPONSABLE = Traslado.RESPONSABLE
                },
                new INGRESO()
                {
                    ID_CENTRO = Ingreso.ID_CENTRO,
                    ID_ANIO = Ingreso.ID_ANIO,
                    ID_IMPUTADO = Ingreso.ID_IMPUTADO,
                    ID_INGRESO = Ingreso.ID_INGRESO,
                    ID_ESTATUS_ADMINISTRATIVO = TRASLADADO
                },
                new TRASLADO_DETALLE()
                {
                    ID_TRASLADO = Traslado_Detalle.ID_TRASLADO,
                    ID_CENTRO_TRASLADO = Traslado_Detalle.ID_CENTRO_TRASLADO,
                    ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                    ID_ANIO = Traslado_Detalle.ID_ANIO,
                    ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                    ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                    ID_ESTATUS = TRASLADO_ACTIVO,
                    ID_ESTATUS_ADMINISTRATIVO = Ingreso.ID_ESTATUS_ADMINISTRATIVO,
                    ID_INCIDENCIA_TRASLADO = !Traslado_Detalle.ID_ATENCION_MEDICA.HasValue ? (short)enumIncidencias.CERTIFICADO_MEDICO_AUSENTE : (short?)null,
                    INCIDENCIA_OBSERVACION = !Traslado_Detalle.ID_ATENCION_MEDICA.HasValue ? TextoIncidenciaCertificadoMedico : null,
                    EGRESO_FEC = Fechas.GetFechaDateServer
                });
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool RevertirUltimoMovimiento(INGRESO Ingreso, TRASLADO_DETALLE Traslado_Detalle, INCIDENTE Incidente)
        {
            try
            {
                return new cTrasladoDetalle().RevertirUltimoMovimiento(
                    new INGRESO()
                    {
                        ID_CENTRO = Ingreso.ID_CENTRO,
                        ID_ANIO = Ingreso.ID_ANIO,
                        ID_IMPUTADO = Ingreso.ID_IMPUTADO,
                        ID_INGRESO = Ingreso.ID_INGRESO,
                        ID_ESTATUS_ADMINISTRATIVO = Traslado_Detalle.ID_ESTATUS_ADMINISTRATIVO
                    },
                    new TRASLADO_DETALLE()
                    {
                        ID_TRASLADO = Traslado_Detalle.ID_TRASLADO,
                        ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                        ID_ANIO = Traslado_Detalle.ID_ANIO,
                        ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                        ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                        ID_ESTATUS = TRASLADO_EN_PROCESO,
                        ID_ESTATUS_ADMINISTRATIVO = null,
                        ID_CENTRO_TRASLADO = Traslado_Detalle.ID_CENTRO_TRASLADO,
                        ID_INCIDENCIA_TRASLADO = null,
                        INCIDENCIA_OBSERVACION = null
                    }, Incidente
                    , Fechas.GetFechaDateServer);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool RevertirTraslado(TRASLADO_DETALLE Traslado_Detalle, INCIDENTE Incidente, INGRESO Ingreso = null)
        {
            try
            {

                if (Ingreso != null)
                    return new cTrasladoDetalle().RevertirTraslado(
                        new TRASLADO_DETALLE()
                        {
                            ID_TRASLADO = Traslado_Detalle.ID_TRASLADO,
                            ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                            ID_ANIO = Traslado_Detalle.ID_ANIO,
                            ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                            ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                            ID_ESTATUS = TRASLADO_PROGRAMADO,
                            ID_ESTATUS_ADMINISTRATIVO = null,
                            ID_CENTRO_TRASLADO = Traslado_Detalle.ID_CENTRO_TRASLADO,
                            ID_INCIDENCIA_TRASLADO = null,
                            INCIDENCIA_OBSERVACION = null
                        },
                        Incidente
                        , Fechas.GetFechaDateServer, new INGRESO()
                        {
                            ID_CENTRO = Ingreso.ID_CENTRO,
                            ID_ANIO = Ingreso.ID_ANIO,
                            ID_IMPUTADO = Ingreso.ID_IMPUTADO,
                            ID_INGRESO = Ingreso.ID_INGRESO,
                            ID_ESTATUS_ADMINISTRATIVO = Traslado_Detalle.ID_ESTATUS_ADMINISTRATIVO
                        });
                else
                {
                    return new cTrasladoDetalle().RevertirTraslado(
                        new TRASLADO_DETALLE()
                        {
                            ID_TRASLADO = Traslado_Detalle.ID_TRASLADO,
                            ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                            ID_ANIO = Traslado_Detalle.ID_ANIO,
                            ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                            ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                            ID_ESTATUS = TRASLADO_PROGRAMADO,
                            ID_ESTATUS_ADMINISTRATIVO = null,
                            ID_CENTRO_TRASLADO = Traslado_Detalle.ID_CENTRO_TRASLADO,
                            ID_INCIDENCIA_TRASLADO = null,
                            INCIDENCIA_OBSERVACION = null
                        },
                         Incidente
                        , Fechas.GetFechaDateServer);
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Métodos Mensajes Interfaz
        //MÉTODO PARA MOSTRAR UN MENSAJE EN LA VENTANA
        public async Task<MessageDialogResult> MensajeVentana(MessageDialogStyle tipoMensaje, string titulo, string mensaje)
        {
            var metro = Ventana as MetroWindow;
            MetroDialogSettings mySettings = null;
            switch (tipoMensaje)
            {
                case MessageDialogStyle.Affirmative:
                    mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Cerrar",
                    };
                    break;
                case MessageDialogStyle.AffirmativeAndNegative:
                    mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Sí",
                        NegativeButtonText = "No"
                    };
                    break;
            }
            mySettings.AnimateShow = true;
            mySettings.AnimateHide = false;
            return await metro.ShowMessageAsync(titulo, mensaje, tipoMensaje, mySettings);
        }

        //MÉTODO PARA CAMBIAR EL MENSAJE DEL ÍCONO DE LAS HUELLAS Y LA VISIBILIDAD DEL CONTROL "PROGRESS RING"
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
                    SelectedImputado.PERMITIR = true;
                    break;
            }
            var image_retriever = new Imagenes();
            ProgressRingVisible = Visibility.Collapsed;
            ImagenEvaluacionVisible = true;
            await TaskEx.Delay(1500);
            ImagenEvaluacion = image_retriever.getImagenHuella();
            ScannerMessage = "Capture Huella\n en el lector";
            //FotoIngreso = image_retriever.getImagenPerson();
            //FotoCentro = image_retriever.getImagenPerson();
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