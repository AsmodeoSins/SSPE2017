using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Wsq2Bmp;

namespace ControlPenales
{
    partial class RegistroLiberadosViewModel : FingerPrintScanner
    {
        public RegistroLiberadosViewModel(){}

        #region Metodos
        private async void clickSwitch(object op)
        {
            try 
            {
                switch (op.ToString())
                {
                    #region Proceso
                    case "guardar_proceso":
                        
                        if (!base.HasErrors)
                        {
                            GuardarProceso();
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos." + base.Error);
                        break;
                    case "cancelar_proceso":
                        PNUC = string.Empty;
                        PFecha = null;
                        PCPAnio = null;
                        PCPFolio = null;
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD);
                        break;
                    #endregion

                    #region medidas
                    case "medida":
                        SelectedMedidaLibertad = null;
                        LimpiarMedida();
                        ValidarMedida();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA);
                        break;
                    case "editar_medida":
                        LimpiarMedida();
                        PopulateMedida();
                        ValidarMedida();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA);
                        break;
                    case "guardar_medida":
                        GuardarMedida();
                        break;
                    case "cancelar_medida":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA);
                        break;
                    //////////////////////////////////
                    case "ver_estatus":
                        base.ClearRules();
                        var lE = new cMedidaLibertadEstatus().ObtenerTodos(
                            SelectedMedidaLibertad.ID_CENTRO,
                            SelectedMedidaLibertad.ID_ANIO,
                            SelectedMedidaLibertad.ID_IMPUTADO,
                            SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                            SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                        if (lE != null)
                            LstMedidaLibertadEstatus = new ObservableCollection<MEDIDA_LIBERTAD_ESTATUS>(lE);
                        else
                            LstMedidaLibertadEstatus = new ObservableCollection<MEDIDA_LIBERTAD_ESTATUS>();
                        EstatusMedidaEmpty = LstMedidaLibertadEstatus.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_ESTATUS);
                        break;
                    case "ver_lugar":
                        base.ClearRules();
                        var lL = new cMedidaLugar().ObtenerTodos(
                            SelectedMedidaLibertad.ID_CENTRO,
                            SelectedMedidaLibertad.ID_ANIO,
                            SelectedMedidaLibertad.ID_IMPUTADO,
                            SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                            SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                        if (lL != null)
                            LstMedidaLugar = new ObservableCollection<MEDIDA_LUGAR>(lL);
                        else
                            LstMedidaLugar = new ObservableCollection<MEDIDA_LUGAR>();

                        LugarMedidaEmpty = LstMedidaLugar.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_LUGARES);
                        break;
                    case "ver_personas":
                        base.ClearRules();
                        var lP = new cMedidaPersona().ObtenerTodos(
                            SelectedMedidaLibertad.ID_CENTRO,
                            SelectedMedidaLibertad.ID_ANIO,
                            SelectedMedidaLibertad.ID_IMPUTADO,
                            SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                            SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);

                        if (lP != null)
                            LstMedidaPersona = new ObservableCollection<MEDIDA_PERSONA>(lP);
                        else
                            LstMedidaPersona = new ObservableCollection<MEDIDA_PERSONA>();
                        PersonasMedidaEmpty = LstMedidaPersona.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_PERSONA);
                        break;
                    case "ver_presentacion":
                        base.ClearRules();
                        SelectedMedidaLibertad = new cMedidaLibertad().Obtener(
                            SelectedMedidaLibertad.ID_CENTRO,
                            SelectedMedidaLibertad.ID_ANIO,
                            SelectedMedidaLibertad.ID_IMPUTADO,
                            SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                            SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                        if (SelectedMedidaLibertad.MEDIDA_PRESENTACION != null)
                        {
                            var lPD = SelectedMedidaLibertad.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE.OrderBy(w => w.FECHA);
                            if (lPD != null)
                                LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(lPD);
                            else
                                LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>();
                        }
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                        break;
                    case "reporte_bitacora":
                        GenerarBitacoraPresentacion();
                        break;
                    case "cerrar_medida_estatus":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_ESTATUS);
                        break;
                    case "cerrar_medida_lugar":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_LUGARES);
                        break;
                    case "cerrar_medida_persona":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_PERSONA);
                        break;
                    case "cerrar_medida_presentacion":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                        break;
                    case "tomar_asistencia":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        HuellaWindow = new BuscarPorHuellaYNipMedidaView();
                        MPObservacion = string.Empty;
                        SelectRegistro = null;
                        HuellaWindow.DataContext = this;
                        ConstructorHuella(0);
                        //HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                        HuellaWindow.Closed += HuellaClosed;
                        HuellaWindow.ShowDialog();
                        //{"Unable to cast object of type 'ControlPenales.BuscarPorHuellaYNipView' to type 'ControlPenales.FotosHuellasDigitalesEstatusAdminView'."}
                        break;

                    case "buscar_nip":
                        ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                        if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                        HuellaWindow.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                //var tipo = (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO;
                                var auxiliar = new List<ResultadoBusquedaBiometrico>();
                                ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                                if (SelectExpediente.NIP.Trim() == TextNipBusqueda.Trim())
                                {
                                    var ingresobiometrico = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                    var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                    if (ingresobiometrico != null)
                                        if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                            else
                                                if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                    FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                    auxiliar.Add(new ResultadoBusquedaBiometrico
                                    {
                                        AMaterno = SelectExpediente.MATERNO.Trim(),
                                        APaterno = SelectExpediente.PATERNO.Trim(),
                                        Expediente = SelectExpediente.ID_ANIO + "/" + SelectExpediente.ID_IMPUTADO,
                                        Foto = FotoBusquedaHuella,
                                        Imputado = SelectExpediente,
                                        NIP = !string.IsNullOrEmpty(SelectExpediente.NIP) ? SelectExpediente.NIP : string.Empty,
                                        Nombre = SelectExpediente.NOMBRE.Trim(),
                                        Persona = null
                                    });
                                }
                                ListResultado = auxiliar.Any() ? auxiliar.ToList() : new List<ResultadoBusquedaBiometrico>();
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la busqueda por nip.", ex);
                            }
                        });
                        HuellaWindow.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;
                    case "aceptar_huella":
                        try
                        {
                            HuellaWindow.Hide();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            if (ScannerMessage.Contains("Procesando..."))
                                return;
                            CancelKeepSearching = true;
                            isKeepSearching = true;
                            //await WaitForFingerPrints();
                            _IsSucceed = true;

                            if (SelectRegistro == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "No se encontraron resultados para la huella");
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                            }
                            else
                            {
                                if (SelectedMedidaPresentacionDetalle == null)
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una fecha de presentacion");
                                else
                                {
                                    var obj = new MEDIDA_PRESENTACION_DETALLE();
                                    obj.ID_CENTRO = SelectedMedidaPresentacionDetalle.ID_CENTRO;
                                    obj.ID_ANIO = SelectedMedidaPresentacionDetalle.ID_ANIO;
                                    obj.ID_IMPUTADO = SelectedMedidaPresentacionDetalle.ID_IMPUTADO;
                                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaPresentacionDetalle.ID_PROCESO_LIBERTAD;
                                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaPresentacionDetalle.ID_MEDIDA_LIBERADO;
                                    obj.ID_DETALLE = SelectedMedidaPresentacionDetalle.ID_DETALLE;
                                    obj.ID_MEDIDA_LUGAR = SelectedMedidaPresentacionDetalle.ID_MEDIDA_LUGAR;
                                    obj.FECHA = SelectedMedidaPresentacionDetalle.FECHA;
                                    obj.ASISTENCIA = "S";
                                    obj.OBSERVACION = MPObservacion;
                                    obj.FECHA_ASISTENCIA = Fechas.GetFechaDateServer;
                                    if (new cMedidaPresentacionDetalle().Actualizar(obj))
                                    {
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Se ha guardado la asistencia correctamente.");
                                        SelectedMedidaLibertad = new cMedidaLibertad().Obtener(SelectedMedidaLibertad.ID_CENTRO, SelectedMedidaLibertad.ID_ANIO, SelectedMedidaLibertad.ID_IMPUTADO, SelectedMedidaLibertad.ID_PROCESO_LIBERTAD, SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                                        LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(SelectedMedidaLibertad.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE.OrderBy(w => w.FECHA));
                                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                                    }
                                    else
                                    {
                                        new Dialogos().ConfirmacionDialogo("Error", "Ha ocurrido un error al guardado la asistencia.");
                                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            HuellaWindow.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                        }
                        break;

                    ///////////////////////////////////

                    case "estatus":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        SelectedMedidaLibertadEstatus = null;
                        LimpiarMedidaEstatus();
                        ValidarMedidaEstatus();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_ESTAUS);
                        break;
                    case "editar_estatus":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        LimpiarMedidaEstatus();
                        PopulateMedidaEstatus();
                        ValidarMedidaEstatus();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_ESTATUS);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_ESTAUS);
                        break;
                    case "guardar_medida_estatus":
                        GuardarMedidaEstatus();
                        break;
                    case "cancelar_medida_estatus":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_ESTAUS);
                        break;

                    case "persona":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        SelectedMedidaPersona = null;
                        LimpiarPersona();
                        ValidarMedidaPersona();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PERSONA);
                        break;
                    case "editar_personas":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        LimpiarPersona();
                        PopulateMedidaPersona();
                        ValidarMedidaPersona();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_PERSONA);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PERSONA);
                        break;
                    case "guardar_persona":
                        GuardarMedidaPersona();
                        break;
                    case "cancelar_persona":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PERSONA);
                        break;

                    case "lugar":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        SelectedMedidaLugar = null;
                        ValidarMedidaLugar();
                        LimpiarMedidaLugar();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_LUGAR);
                        break;
                    case "editar_lugares":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        LimpiarMedidaLugar();
                        PopulateMedidaLugar();
                        ValidarMedidaLugar();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_LUGARES);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_LUGAR);
                        break;

                    case "guardar_medida_lugar":
                        GuardarMedidaLugar();
                        break;
                    case "cancelar_medida_lugar":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_LUGAR);
                        break;

                    case "presentacion":
                        if (SelectedMedidaLibertad == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Valiación", "Favor de seleccionar una medida en libertad");
                            break;
                        }
                        SelectedMedidaPresentacion = new cMedidaPresentacion().Obtener(
                              SelectedMedidaLibertad.ID_CENTRO,
                              SelectedMedidaLibertad.ID_ANIO,
                              SelectedMedidaLibertad.ID_IMPUTADO,
                              SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                               SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                        LimpiarMedidaPresentacion();
                        if (SelectedMedidaPresentacion != null)
                        {
                            //new Dialogos().ConfirmacionDialogo("Valiación", "La medida ya cuenta con una programación de presentaciones.");
                            //break;
                            PopulateMedidaPresentacion();
                        }

                        ValidarMedidaPresentacion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_PRESENTACION);
                        break;
                    case "guardar_medida_presentacion":
                        GuardarMedidaPresentacion();
                        break;
                    case "cancelar_medida_presentacion":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_PRESENTACION);
                        break;
                    case "AregarFechaProgramacion":
                        AgregarFechaIndividual();
                        break;
                    case "generar_programacion":
                        ProgramarPresentacion();
                        break;
                    case "eliminar_fecha_presentacion":
                        EliminarFechaPresentacion();
                        break;

                    case "documento":
                        LimpiarDocumento();
                        ValidarDocumento();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO);
                        break;
                    case "editar_documento":
                        LimpiarDocumento();
                        PopulateMedidaDocumento();
                        ValidarDocumento();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO);
                        break;
                    case "guardar_medida_documento":
                        GuardarMedidaDocumento();
                        break;
                    case "cancelar_medida_documento":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO);
                        break;
                    case "subir_documento":
                        ElegirDocumentoGuardar();
                        break;
                    case "ver_documento":
                        VerDocumento();
                        break;
                    #endregion

                    #region Seguimientos
                    case "agregar_seguimiento":
                        SelectedSeguimiento = null;
                        LimpiarSeguimiento();
                        ValidarSeguimiento();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                        break;
                    case "editar_seguimiento":
                        LimpiarSeguimiento();
                        PopulateSegumientoDetalle();
                        ValidarSeguimiento();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                        break;
                    case "eliminar_seguimiento":
                        if (SelectedSeguimiento != null)
                        {

                            if (await new Dialogos().ConfirmarEliminar("Validación", "¿Confirma la eliminacion de este seguimiento?") != 0)
                            {
                                EliminarSeguimiento();
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar el seguimiento a eliminar");
                        break;
                    case "guardar_seguimiento":
                        GuardarSeguimiento();
                        break;
                    case "cancelar_seguimiento":
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                        break;
                    #endregion

                    case "insertar_alias":
                        if (!pInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }

                        LimpiarAlias();
                        ValidarAlias();
                        SelectAlias = null;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        break;
                    case "editar_alias":
                        if (SelectAlias != null)
                        {
                            if (!pEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                break;
                            }
                            ValidarAlias();
                            PopulateAlias();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar alias");
                        break;
                    case "eliminar_alias":
                        if (SelectAlias != null)
                        {
                            EliminarAlias();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar alias");
                        break;
                    case "guardar_alias":
                        if (SelectAlias != null)
                            EditarAlias();
                        else
                            AgregarAlias();
                        ValidacionesLiberado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                        break;
                    case "cancelar_alias":
                        LimpiarAlias();
                        ValidacionesLiberado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                        break;

                    case "insertar_apodo":
                        if (!pInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }

                        ValidarApodo();
                        LimpiarApodo();
                        SelectApodo = null;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        break;
                    case "editar_apodo":
                        if (SelectApodo != null)
                        {
                            if (!pEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                break;
                            }

                            ValidarApodo();
                            PopulateApodo();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar apodo");
                        break;
                    case "eliminar_apodo":
                        if (SelectApodo != null)
                        {
                            EliminarApodo();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar apodo");
                        break;
                    case "guardar_apodo":
                        if (SelectApodo != null)
                            EditarApodo();
                        else
                            AgregarApodo();
                        ValidacionesLiberado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                        break;
                    case "cancelar_apodo":
                        LimpiarApodo();
                        ValidacionesLiberado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                        break;

                    case "buscar_menu":
                        if (!pConsultar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                        break;
                    case "nueva_busqueda":
                        LimpiarBusqueda();
                        break;

                    case "buscar_salir":
                        //LimpiarBusqueda();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                        break;

                    case "buscar_visible":
                        if (!pConsultar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        ClickEnter();
                        break;
                    case "nuevo_expediente":
                        SelectExpediente = null;
                        SelectedProcesoLibertad = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                        ValidarProcesoLibertad();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD);
                        break;

                    case "buscar_nuevo_proceso":
                        if (!pConsultar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        SelectedProcesoLibertad = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                        ValidarProcesoLibertad();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD);
                        break;

                    case "buscar_seleccionar":
                        if (SelectExpediente == null)
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un imputado.");
                        else
                            if (SelectedProcesoLibertad != null)
                            {
                                SelectedProcesoLibertadListado = SelectedProcesoLibertad;
                                TabIdentificacionEnabled = TabMedidaEnabled = true;
                                LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                                LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(SelectedProcesoLibertad.MEDIDA_DOCUMENTO);
                                LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                                LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(SelectedProcesoLibertad.MEDIDA_LIBERTAD);
                                LstSeguimiento = new ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO>(SelectedProcesoLibertad.PROCESO_LIBERTAD_SEGUIMIENTO);
                                Obtener();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso.");
                        break;
                    case "buscar_seleccionar_listado":
                        SelectedProcesoLibertad = SelectedProcesoLibertadListado;
                        PTipo = -1;
                        if (SelectedProcesoLibertad != null)
                            {
                                SelectedProcesoLibertadListado = SelectedProcesoLibertad;
                                TabIdentificacionEnabled = TabMedidaEnabled = true;
                                LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                                LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(SelectedProcesoLibertad.MEDIDA_DOCUMENTO);
                                LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                                LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(SelectedProcesoLibertad.MEDIDA_LIBERTAD);
                                LstSeguimiento = new ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO>(SelectedProcesoLibertad.PROCESO_LIBERTAD_SEGUIMIENTO);
                                Obtener();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                            }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso.");
                        break;
                    case "ficha_menu":
                        if (SelectExpediente != null)
                        {
                            ///var CerifImputado = SelectIngreso.IMPUTADO;
                            var View = new ReportesView();
                            var DatosReporte = new cCuerpoReporte();
                            #region Iniciliza el entorno para mostrar el reporte al usuario
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            View.Owner = PopUpsViewModels.MainWindow;
                            View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                            View.Show();
                            #endregion
                            
                            #region Se forma el cuerpo del reporte

                            //if (SelectIngreso.IMPUTADO != null)
                            //{


                            DatosReporte.NombreInterno = selectExpediente.PATERNO + " " + selectExpediente.MATERNO + " " + selectExpediente.NOMBRE;
                            //DatosReporte.Telefono = SelectExpediente.TELEFONO.HasValue ? SelectExpediente.TELEFONO.Value.ToString() : string.Empty;
                            DatosReporte.Telefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.Value.ToString() : string.Empty;
                            //DatosReporte.Escolaridad = SelectExpediente.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectExpediente.ESCOLARIDAD.DESCR) ? SelectExpediente.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? SelectIngreso.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            DatosReporte.FechaNacimiento = SelectExpediente.NACIMIENTO_FECHA.HasValue ? SelectExpediente.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                            //DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectExpediente.DOMICILIO_CALLE, SelectExpediente.DOMICILIO_NUM_EXT.HasValue ? SelectExpediente.DOMICILIO_NUM_EXT != decimal.Zero ? SelectExpediente.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                            //SelectExpediente.COLONIA != null ? !string.IsNullOrEmpty(SelectExpediente.COLONIA.DESCR) ? SelectExpediente.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectExpediente.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectExpediente.MUNICIPIO.MUNICIPIO1) ? SelectExpediente.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                            DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectIngreso.DOMICILIO_CALLE, SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT != decimal.Zero ? SelectIngreso.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                            SelectIngreso.COLONIA != null ? !string.IsNullOrEmpty(SelectIngreso.COLONIA.DESCR) ? SelectIngreso.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectIngreso.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectIngreso.MUNICIPIO.MUNICIPIO1) ? SelectIngreso.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                            DatosReporte.Sexo = selectExpediente.SEXO;
                            DatosReporte.FechaNacimiento = textEdad.ToString();
                            DatosReporte.FechaEstudio = Fechas.GetFechaDateServer.ToShortDateString();
                            //DatosReporte.EstadoCivil = ListEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == (short)SelectEstadoCivil.Value).FirstOrDefault().DESCR;
                            DatosReporte.EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR : string.Empty;
                            DatosReporte.ActitudGeneralEntrevistado = ActitudGeneralEntrv;

                            //Prueba
                            DatosReporte.Observaciones = TextObservaciones;
                            DatosReporte.ActitudGeneralEntrevistado = ActitudGeneralEntrv;

                            //DatosReporte.HorariosDiasTrabajo=
                            ListApodo = new ObservableCollection<APODO>(selectExpediente.APODO);
                            if (ListApodo != null && ListApodo.Count > 0)
                            {
                                short ultimoApodo = selectExpediente.APODO.LastOrDefault().ID_APODO;
                                foreach (var entidad in selectExpediente.APODO.OrderBy(o => o.ID_APODO))
                                {
                                    if (entidad.IMPUTADO != null)
                                    {
                                        DatosReporte.Apodo += entidad.ID_APODO == ultimoApodo ? entidad.APODO1.Trim() + "" : entidad.APODO1.Trim() + ",";
                                    }
                                }

                            }


                            listAlias = new ObservableCollection<ALIAS>(selectExpediente.ALIAS);
                            if (listAlias != null && listAlias.Count > 0)
                            {

                                short ultimoAlias = selectExpediente.ALIAS.LastOrDefault().ID_ALIAS;
                                foreach (var entidad in selectExpediente.ALIAS.OrderBy(o => o.ID_ALIAS))
                                {
                                    short ultimoApodo = selectExpediente.APODO.LastOrDefault().ID_APODO;
                                    foreach (var entidad2 in selectExpediente.APODO.OrderBy(o => o.ID_APODO))
                                    {
                                        if (entidad2.IMPUTADO != null)
                                        {
                                            DatosReporte.Alias += entidad2.ID_APODO == ultimoApodo ? entidad2.APODO1.Trim() + "" : entidad2.APODO1.Trim() + ",";
                                        }
                                    }
                                }

                            }

                            //DatosReporte.Delito = ;
                            //DatosReporte.MedidaCautelar=SelectMJ.MEDIDA_JUDICIAL;
                            // }
                            #endregion

                            #region Se forma el encabezado del reporte
                            var Encabezado = new cEncabezado();
                            Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                            Encabezado.TituloDos = Parametro.ENCABEZADO2;
                            Encabezado.NombreReporte = "FICHA DE IDENTIFICACIÓN";
                            Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                            Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                            #endregion

                            #region Inicializacion de reporte
                            View.Report.LocalReport.ReportPath = "Reportes/rFichaIdentificacion.rdlc";
                            View.Report.LocalReport.DataSources.Clear();
                            #endregion

                            #region Definicion de origenes de datos


                            //datasource Uno
                            var ds1 = new List<cEncabezado>();
                            ds1.Add(Encabezado);
                            Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                            rds1.Name = "DataSet3";
                            rds1.Value = ds1;
                            View.Report.LocalReport.DataSources.Add(rds1);


                            //datasource dos
                            var ds2 = new List<cCuerpoReporte>();
                            ds2.Add(DatosReporte);
                            Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                            rds2.Name = "DataSet2";
                            rds2.Value = ds2;
                            View.Report.LocalReport.DataSources.Add(rds2);



                            //View.Report.LocalReport.DisplayName = string.Format("Fehca de Identificación{0} {1} {2} ",//Nombre del archivo que se va a generar con el reporte independientemente del formato que se elija
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty ,
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                            //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);


                            View.Report.ReportExport += (s, e) =>
                            {
                                try
                                {
                                    if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                                    {
                                        e.Cancel = true;//Detiene el evento
                                        s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene permitido exportar la información en este formato.");
                                    }
                                }

                                catch (Exception exc)
                                {
                                    throw exc;
                                }
                            };

                            View.Report.RefreshReport();
                        }
                        else { new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un Imputado."); }

                            #endregion

                        break;

                    case "reporte_menu":
                        if (!pImprimir)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            break;
                        }
                        if (SelectedProcesoLibertad != null)
                        {
                            CPAnio = null;
                            CPFolio = null;
                            LimpiarCausaPenal();
                            if (SelectedProcesoLibertad.CP_ANIO.HasValue && SelectedProcesoLibertad.CP_FOLIO.HasValue)
                            {
                                CPAnio = SelectedProcesoLibertad.CP_ANIO;
                                CPFolio = SelectedProcesoLibertad.CP_FOLIO;
                                BuscarCausaPenal(0);
                            }
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_CAUSA_PENAL);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso en libertad");
                        #region Comentado
                        //Creacion de Reporte
                        //if (SelectExpediente != null)
                        //{


                        //    ///var CerifImputado = SelectIngreso.IMPUTADO;
                        //    var View = new ReportesView();
                        //    var DatosReporte = new cCuerpoReporte();
                        //    #region Iniciliza el entorno para mostrar el reporte al usuario
                        //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        //    View.Owner = PopUpsViewModels.MainWindow;
                        //    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };


                        //    View.Show();
                        //    #endregion
                        //    #region Se forma el cuerpo del reporte

                        //    //if (SelectIngreso.IMPUTADO != null)
                        //    //{


                        //    DatosReporte.NombreInterno = selectExpediente.PATERNO + " " + selectExpediente.MATERNO + " " + selectExpediente.NOMBRE;
                        //    DatosReporte.Telefono = SelectExpediente.TELEFONO.HasValue ? SelectExpediente.TELEFONO.Value.ToString() : string.Empty;
                        //    DatosReporte.Escolaridad = SelectExpediente.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectExpediente.ESCOLARIDAD.DESCR) ? SelectExpediente.ESCOLARIDAD.DESCR.Trim() : string.Empty : string.Empty;
                        //    DatosReporte.FechaNacimiento = SelectExpediente.NACIMIENTO_FECHA.HasValue ? SelectExpediente.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty;
                        //    DatosReporte.Domicilio = string.Format("{0} {1} {2} {3}", SelectExpediente.DOMICILIO_CALLE, SelectExpediente.DOMICILIO_NUM_EXT.HasValue ? SelectExpediente.DOMICILIO_NUM_EXT != decimal.Zero ? SelectExpediente.DOMICILIO_NUM_EXT.ToString() : string.Empty : string.Empty,
                        //    SelectExpediente.COLONIA != null ? !string.IsNullOrEmpty(SelectExpediente.COLONIA.DESCR) ? SelectExpediente.COLONIA.DESCR.Trim() : string.Empty : string.Empty, SelectExpediente.MUNICIPIO != null ? !string.IsNullOrEmpty(SelectExpediente.MUNICIPIO.MUNICIPIO1) ? SelectExpediente.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty);
                        //    DatosReporte.Sexo = selectExpediente.SEXO;
                        //    DatosReporte.FechaNacimiento = textEdad.ToString();
                        //    DatosReporte.FechaEstudio = Fechas.GetFechaDateServer.ToShortDateString();
                        //    DatosReporte.EstadoCivil = ListEstadoCivil.Where(w => w.ID_ESTADO_CIVIL == (short)SelectEstadoCivil.Value).FirstOrDefault().DESCR;
                        //    DatosReporte.ActitudGeneralEntrevistado = ActitudGeneralEntrv;

                        //    //Prueba
                        //    DatosReporte.Observaciones = TextObservaciones;
                        //    DatosReporte.ActitudGeneralEntrevistado = ActitudGeneralEntrv;

                        //    //DatosReporte.HorariosDiasTrabajo=
                        //    ListApodo = new ObservableCollection<APODO>(selectExpediente.APODO);
                        //    if (ListApodo != null && ListApodo.Count > 0)
                        //    {
                        //        short ultimoApodo = selectExpediente.APODO.LastOrDefault().ID_APODO;
                        //        foreach (var entidad in selectExpediente.APODO.OrderBy(o => o.ID_APODO))
                        //        {
                        //            if (entidad.IMPUTADO != null)
                        //            {
                        //                DatosReporte.Apodo += entidad.ID_APODO == ultimoApodo ? entidad.APODO1.Trim() + "" : entidad.APODO1.Trim() + ",";
                        //            }
                        //        }

                        //    }


                        //    listAlias = new ObservableCollection<ALIAS>(selectExpediente.ALIAS);
                        //    if (listAlias != null && listAlias.Count > 0)
                        //    {

                        //        short ultimoAlias = selectExpediente.ALIAS.LastOrDefault().ID_ALIAS;
                        //        foreach (var entidad in selectExpediente.ALIAS.OrderBy(o => o.ID_ALIAS))
                        //        {
                        //            short ultimoApodo = selectExpediente.APODO.LastOrDefault().ID_APODO;
                        //            foreach (var entidad2 in selectExpediente.APODO.OrderBy(o => o.ID_APODO))
                        //            {
                        //                if (entidad2.IMPUTADO != null)
                        //                {
                        //                    DatosReporte.Alias += entidad2.ID_APODO == ultimoApodo ? entidad2.APODO1.Trim() + "" : entidad2.APODO1.Trim() + ",";
                        //                }
                        //            }
                        //        }

                        //    }

                        //    //DatosReporte.Delito = ;
                        //    //DatosReporte.MedidaCautelar=SelectMJ.MEDIDA_JUDICIAL;
                        //    // }
                        //    #endregion

                        //    #region Se forma el encabezado del reporte
                        //    var Encabezado = new cEncabezado();
                        //    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                        //    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                        //    Encabezado.NombreReporte = "FICHA DE IDENTIFICACIÓN";
                        //    Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                        //    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;

                        //    #endregion

                        //    #region Inicializacion de reporte
                        //    View.Report.LocalReport.ReportPath = "../../Reportes/rFichaIdentificacion.rdlc";
                        //    View.Report.LocalReport.DataSources.Clear();
                        //    #endregion

                        //    #region Definicion de origenes de datos


                        //    //datasource Uno
                        //    var ds1 = new List<cEncabezado>();
                        //    ds1.Add(Encabezado);
                        //    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        //    rds1.Name = "DataSet3";
                        //    rds1.Value = ds1;
                        //    View.Report.LocalReport.DataSources.Add(rds1);


                        //    //datasource dos
                        //    var ds2 = new List<cCuerpoReporte>();
                        //    ds2.Add(DatosReporte);
                        //    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        //    rds2.Name = "DataSet2";
                        //    rds2.Value = ds2;
                        //    View.Report.LocalReport.DataSources.Add(rds2);



                        //    //View.Report.LocalReport.DisplayName = string.Format("Fehca de Identificación{0} {1} {2} ",//Nombre del archivo que se va a generar con el reporte independientemente del formato que se elija
                        //    //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty ,
                        //    //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                        //    //    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);


                        //    View.Report.ReportExport += (s, e) =>
                        //    {
                        //        try
                        //        {
                        //            if (e != null && !e.Extension.LocalizedName.Equals("PDF"))//Solo permite pdf
                        //            {
                        //                e.Cancel = true;//Detiene el evento
                        //                s = e = null;//Detiene el mapeo de los parametros para que no continuen en el arbol
                        //                (new Dialogos()).ConfirmacionDialogo("Validación", "No tiene permitido exportar la información en este formato.");
                        //            }
                        //        }

                        //        catch (Exception exc)
                        //        {
                        //            throw exc;
                        //        }
                        //    };

                        //    View.Report.RefreshReport();
                        //}
                        //else { new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un Imputado."); }

                        //    #endregion
                        #endregion
                        break;

                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroLiberadosView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RegistroLiberadosViewModel();
                        break;
                    case "guardar_menu":
                        Guardar();
                        break;
                    case "Open442":
                        if (!pConsultar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                        this.ShowIdentification();
                        break;
                    case "salir_menu":
                        if (!Changed)
                            StaticSourcesViewModel.SourceChanged = false;
                        PrincipalViewModel.SalirMenu();
                        break;

                    case "buscar_causa_penal":
                        BuscarCausaPenal();
                        break;
                    case "imprimir_ficha":
                        ImprimirFicha();
                        break;
                    case "cancelar_ficha":
                        CPAnio = null;
                        CPFolio = null;
                        LimpiarCausaPenal();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CAUSA_PENAL);
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
            
        }

        private async void LiberadoLoad(RegistroLiberadosView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(ConfiguraPermisos);
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        //private void DatosIdentificacionLoad(DatosGeneralesIdentificacionEstatusAdminView obj = null)
        //{
        //    //ValidacionesLiberado();
        //}

        private void CargarListas()
        {
            try
            {
                LstTipoReferencia = new ObservableCollection<TIPO_REFERENCIA>(new cTipoReferencia().ObtenerTodos());
                ListEstadoCivil = new ObservableCollection<ESTADO_CIVIL>(new cEstadoCivil().ObtenerTodos());
                ListOcupacion = new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos());
                ListEscolaridad = new ObservableCollection<ESCOLARIDAD>(new cEscolaridad().ObtenerTodos());
                ListPaisNacionalidad = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerNacionalidad());
                LstIdioma = new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos());
                ListEtnia = new ObservableCollection<ETNIA>(new cEtnia().ObtenerTodos());
                LstDialecto = new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos());
                ListReligion = new ObservableCollection<RELIGION>(new cReligion().ObtenerTodos());

                var paises = new cPaises().ObtenerTodos();
                ListPaisDomicilio = new ObservableCollection<PAIS_NACIONALIDAD>(paises);
                ListPaisNacimiento = new ObservableCollection<PAIS_NACIONALIDAD>(paises);
                ListPaisDomicilioPadre = new ObservableCollection<PAIS_NACIONALIDAD>(paises);
                ListPaisDomicilioMadre = new ObservableCollection<PAIS_NACIONALIDAD>(paises);

                #region Medida
                LstLugarCumplimiento = new ObservableCollection<LUGAR_CUMPLIMIENTO>(new cLugarCumplimiento().ObtenerTodos());
                LstFuente = new ObservableCollection<FUENTE>(new cFuente().ObtenerTodos());
                LstTipoDocumentoMedida = new ObservableCollection<TIPO_DOCUMENTO_MEDIDA>(new cTipoDocumentoMedida().ObtenerTodos());
                LstTipoDocumentoMedidaFiltro = new ObservableCollection<TIPO_DOCUMENTO_MEDIDA>();

                //LstAsesor = new ObservableCollection<Asesor>(new cEmpleado().ObtenerTodos(GlobalVar.gCentro).Select(w => new Asesor() { ID_EMPLEADO = w.ID_EMPLEADO, NOMBRE = w.PERSONA.NOMBRE }));
                LstAsesor = new ObservableCollection<Asesor>();
                var empleados = new cEmpleado().ObtenerTodos(GlobalVar.gCentro);

                LstMedidaTipo = new ObservableCollection<MEDIDA_TIPO>(new cMedidaTipo().ObtenerTodos());
                LstMedida = new ObservableCollection<MEDIDA>();
                LstMedidaEstatus = new ObservableCollection<MEDIDA_ESTATUS>(new cMedidaEstatus().ObtenerTodos());
                LstMotivo = new ObservableCollection<MEDIDA_MOTIVO>();
                LstParticularidad = new ObservableCollection<PARTICULARIDAD>(new cParticularidad().ObtenerTodos());
                #endregion

                #region Lugar
                LstGiro = new ObservableCollection<GIRO>(new cGiro().ObtenerTodos());
                var mx = Parametro.PAIS;
                LstEntidadML = new ObservableCollection<ENTIDAD>(new cEntidad().Obtener(mx));
                LstMunicipioML = new ObservableCollection<MUNICIPIO>();
                #endregion
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstLugarCumplimiento.Insert(0, new LUGAR_CUMPLIMIENTO() { ID_LUGCUM = -1, DESCR = "SELECCIONE" });
                    LstFuente.Insert(0, new FUENTE() { ID_FUENTE = -1, DESCR = "SELECCIONE" });
                    LstTipoDocumentoMedidaFiltro.Insert(0, new TIPO_DOCUMENTO_MEDIDA() { ID_TIPDOCMED = -1, DESCR = "SELECCIONE" });
                    
                    if (empleados != null)
                    {
                        foreach (var e in empleados)
                        { 
                            LstAsesor.Add(new Asesor(){ ID_EMPLEADO = e.ID_EMPLEADO, NOMBRE = 
                                string.Format("{0} {1} {2}",
                                e.PERSONA.NOMBRE.Trim(),
                                !string.IsNullOrEmpty(e.PERSONA.PATERNO) ? e.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(e.PERSONA.MATERNO) ? e.PERSONA.MATERNO.Trim() : string.Empty)
                            });
                        }
                    }
                    LstAsesor.Insert(0, new Asesor() { ID_EMPLEADO = -1, NOMBRE = "SELECCIONE" });

                    LstTipoReferencia.Insert(0, new TIPO_REFERENCIA() { ID_TIPO_REFERENCIA = -1, DESCR = "SELECCIONE" });
                    ListEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    ListPaisNacionalidad.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, NACIONALIDAD = "SELECCIONE" });
                    LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
                    ListEtnia.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
                    LstDialecto.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
                    ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });

                    ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });

                    ListPaisDomicilioPadre.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });

                    ListPaisDomicilioMadre.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });

                    #region Medida
                    LstMedidaTipo.Insert(0, new MEDIDA_TIPO() { ID_TIPO_MEDIDA = -1, DESCR = "SELECCIONE" });
                    LstMedida.Insert(0, new MEDIDA() { ID_MEDIDA = -1, DESCR = "SELECCIONE" });
                    LstMedidaEstatus.Insert(0, new MEDIDA_ESTATUS() { ID_ESTATUS = -1, DESCR = "SELECCIONE" });
                    LstMotivo.Insert(0, new MEDIDA_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                    LstParticularidad.Insert(0, new PARTICULARIDAD() { ID_PARTICUARIDAD = -1, DESCR = "SELECCIONE" });
                    #endregion

                    #region Lugar
                    LstGiro.Add(new GIRO() { ID_GIRO = -1, DESCR = "SELECCIONE" });
                    LstEntidadML.Add(new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    LstMunicipioML.Add(new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    #endregion

                    ValidacionesLiberado();

                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }

        private async void ClickEnter(Object obj = null)
        {
            try
            {
                if (!pConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NUCBuscar":
                                NUCBuscar = textbox.Text;
                                break;
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    FolioBuscar = Convert.ToInt32(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrWhiteSpace(textbox.Text))
                                    AnioBuscar = short.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                        }
                    }
                }

                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                #region nuevo
                LstLiberados = new RangeEnabledObservableCollection<cLiberados>();
                LstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados());
                EmptyExpedienteVisible = LstLiberados.Count > 0 ? false : true;
                #endregion

                //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //if (ListExpediente != null)
                //    EmptyExpedienteVisible = ListExpediente.Count < 0;
                //else
                //    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }

        private async void ClickEnterCP(Object obj = null)
        {
            try
            {
                if (!pConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "cpAnio":
                                if(!string.IsNullOrEmpty(textbox.Text))
                                    CPAnio = short.Parse(textbox.Text);
                                break;
                            case "cpFolio":
                                if(!string.IsNullOrEmpty(textbox.Text))
                                    CPFolio = int.Parse(textbox.Text);
                                break;
                        }
                    }
                }
                if (CPAnio.HasValue && CPFolio.HasValue)
                {
                    BuscarCausaPenal();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }
        #endregion

        #region Buscar
        private void LimpiarBusqueda()
        {
            ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar =  NUCBuscar =  string.Empty;
            AnioBuscar = null;
            FolioBuscar = null;
            SelectExpediente = null;
            SelectIngreso = null;
            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            //ListExpediente = null;
            LstLiberados = null;
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

        private async Task<List<cLiberados>> SegmentarResultadoBusquedaLiberados(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && string.IsNullOrEmpty(NUCBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<cLiberados>();
             
            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<IEnumerable<cLiberados>>(() => new cImputado().ObtenerLiberados(NUCBuscar,AnioBuscar,FolioBuscar,NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, _Pag).Select(w => new cLiberados()
            { 
                 ID_CENTRO = w.ID_CENTRO,
                 ID_ANIO = w.ID_ANIO,
                 ID_IMPUTADO = w.ID_IMPUTADO,
                 CENTRO = w.CENTRO,
                 NOMBRE = string.Format("{0}{1}",w.NOMBRE,!string.IsNullOrEmpty(w.APODO_NOMBRE) ? "("+w.APODO_NOMBRE+")" : string.Empty),
                 PATERNO = string.Format("{0}{1}", w.PATERNO,!string.IsNullOrEmpty(w.PATERNO_A) ? "(" + w.PATERNO_A + ")" : string.Empty),
                 MATERNO = string.Format("{0}{1}", w.MATERNO,!string.IsNullOrEmpty(w.MATERNO_A) ? "(" + w.MATERNO_A + ")" : string.Empty)
            }));
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

        #region Liberados
        private void Nuevo()
        {
            if (!string.IsNullOrEmpty(ApellidoPaternoBuscar) && !string.IsNullOrEmpty(NombreBuscar))
            {
                TabsEnabled = true;
                CamposBusquedaEnabled = false;
                AnioBuscar = null;
                FolioBuscar = null;
                SelectExpediente = null;
                SelectIngreso = null;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                ValidacionesLiberado();
                Changed = true;
                CamposBusquedaEnabled = BHuellasEnabled = MenuBuscarEnabled = InterconexionEnabled = false;
                StaticSourcesViewModel.SourceChanged = false;
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de campturar nombre y apellido(s)");
        }

        private void Obtener()
        {
            try
            {
                if (SelectedProcesoLibertad != null && SelectExpediente != null)
                {
                    TabsEnabled = true;
                    #region Datos Generales
                    AnioBuscar = SelectedProcesoLibertad.CP_ANIO;//SelectExpediente.ID_ANIO;
                    FolioBuscar = SelectedProcesoLibertad.CP_FOLIO;//SelectExpediente.ID_IMPUTADO;
                    NombreBuscar = SelectExpediente.NOMBRE;
                    ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                    ApellidoMaternoBuscar = SelectExpediente.MATERNO;

                    NUCBuscar = SelectedProcesoLibertad.NUC;
                    SelectSexo = !string.IsNullOrEmpty(SelectExpediente.SEXO) ? SelectExpediente.SEXO : "S";
                    SelectEstadoCivil = SelectedProcesoLibertad.ID_ESTADO_CIVIL != null ? SelectedProcesoLibertad.ID_ESTADO_CIVIL  : -1;
                    SelectOcupacion = SelectedProcesoLibertad.ID_OCUPACION != null ? SelectedProcesoLibertad.ID_OCUPACION : -1;
                    SelectEscolaridad = SelectedProcesoLibertad.ID_ESCOLARIDAD != null ? SelectedProcesoLibertad.ID_ESCOLARIDAD : -1;
                    SelectReligion = SelectedProcesoLibertad.ID_RELIGION  != null ? SelectedProcesoLibertad.ID_RELIGION : -1;
                    SelectEtnia = SelectExpediente.ID_ETNIA != null ? SelectExpediente.ID_ETNIA : -1;
                    SelectedIdioma = SelectExpediente.ID_IDIOMA != null ? SelectExpediente.ID_IDIOMA : Parametro.IDIOMA;
                    SelectedDialecto = SelectExpediente.ID_DIALECTO != null ? SelectExpediente.ID_DIALECTO : -1;
                    RequiereTraductor = SelectExpediente.TRADUCTOR == "S" ? true : false;
                    PTipo = SelectedProcesoLibertad.TIPO;
                    PEscalaRiesgo = SelectedProcesoLibertad.ID_ESCALA_RIESGO;
                    #endregion

                    #region Fotos
                    #endregion

                    #region Domicilio
                    SelectPais = SelectedProcesoLibertad.ID_PAIS != null ? SelectedProcesoLibertad.ID_PAIS : Parametro.PAIS;
                    SelectEntidad = SelectedProcesoLibertad.ID_ENTIDAD != null ? SelectedProcesoLibertad.ID_ENTIDAD : -1;
                    SelectMunicipio = SelectedProcesoLibertad.ID_MUNICIPIO != null ? SelectedProcesoLibertad.ID_MUNICIPIO : -1;
                    SelectColonia = SelectedProcesoLibertad.ID_COLONIA;
                    TextCalle = SelectedProcesoLibertad.DOMICILIO_CALLE;
                    TextNumeroExterior = SelectedProcesoLibertad.DOMICILIO_NUM_EXT;
                    TextNumeroInterior = SelectedProcesoLibertad.DOMICILIO_NUM_INT;
                    TextTelefono = SelectedProcesoLibertad.TELEFONO != null ? SelectedProcesoLibertad.TELEFONO.Value.ToString() : null;
                    TextCodigoPostal = SelectedProcesoLibertad.DOMICILIO_CODIGO_POSTAL;
                    TextDomicilioTrabajo = SelectedProcesoLibertad.DOMICILIO_TRABAJO;
                    AniosEstado = SelectedProcesoLibertad.RESIDENCIA_ANIOS != null ? SelectedProcesoLibertad.RESIDENCIA_ANIOS.Value.ToString() : "0";
                    MesesEstado = SelectedProcesoLibertad.RESIDENCIA_MESES != null ? SelectedProcesoLibertad.RESIDENCIA_MESES.Value.ToString() : "0";
                    //SelectPais = SelectIngreso.ID_PAIS;
                    //SelectEntidad = SelectIngreso.ID_ENTIDAD;
                    //SelectMunicipio = SelectIngreso.ID_MUNICIPIO;
                    //SelectColonia = SelectIngreso.ID_COLONIA;
                    //TextCalle = SelectIngreso.DOMICILIO_CALLE;
                    //TextNumeroExterior = SelectIngreso.DOMICILIO_NUM_EXT;
                    //TextNumeroInterior = SelectIngreso.DOMICILIO_NUM_INT;
                    //TextTelefono = SelectIngreso.TELEFONO.Value.ToString();
                    //TextCodigoPostal = SelectIngreso.DOMICILIO_CP;
                    //TextDomicilioTrabajo = SelectIngreso.DOMICILIO_TRABAJO;
                    //AniosEstado = SelectIngreso.RESIDENCIA_ANIOS != null ? SelectIngreso.RESIDENCIA_ANIOS.Value.ToString() : "0";
                    //MesesEstado = SelectIngreso.RESIDENCIAS_MESES != null ? SelectIngreso.RESIDENCIAS_MESES.Value.ToString() : "0";
                    #endregion

                    #region Nacimiento
                    SelectPaisNacimiento = SelectExpediente.NACIMIENTO_PAIS != null ? SelectExpediente.NACIMIENTO_PAIS : Parametro.PAIS;
                    SelectEntidadNacimiento = SelectExpediente.NACIMIENTO_ESTADO != null ? SelectExpediente.NACIMIENTO_ESTADO : -1;
                    SelectMunicipioNacimiento = SelectExpediente.NACIMIENTO_MUNICIPIO != null ? SelectExpediente.NACIMIENTO_MUNICIPIO : -1;
                    TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;
                    TextLugarNacimientoExtranjero = SelectExpediente.NACIMIENTO_LUGAR;
                    #endregion

                    #region Padres
                    TextPadrePaterno = !string.IsNullOrEmpty(SelectExpediente.PATERNO_PADRE) ? SelectExpediente.PATERNO_PADRE.Trim() : string.Empty;
                    TextPadreMaterno = !string.IsNullOrEmpty(SelectExpediente.MATERNO_PADRE) ? SelectExpediente.MATERNO_PADRE.Trim() : string.Empty;
                    TextPadreNombre = !string.IsNullOrEmpty(SelectExpediente.NOMBRE_PADRE) ? SelectExpediente.NOMBRE_PADRE.Trim() : string.Empty;
                    CheckPadreFinado = SelectedProcesoLibertad.PADRE_FINADO == "S" ? true : false;
                    //CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S" ? true : false;

                    TextMadrePaterno = !string.IsNullOrEmpty(SelectExpediente.PATERNO_MADRE) ? SelectExpediente.PATERNO_MADRE.Trim() : string.Empty;
                    TextMadreMaterno = !string.IsNullOrEmpty(SelectExpediente.MATERNO_MADRE) ? SelectExpediente.MATERNO_MADRE.Trim() : string.Empty;
                    TextMadreNombre = !string.IsNullOrEmpty(SelectExpediente.NOMBRE_MADRE) ? SelectExpediente.NOMBRE_MADRE.Trim() : string.Empty;
                    CheckMadreFinado = SelectedProcesoLibertad.MADRE_FINADO == "S" ? true : false;
                    //CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S" ? true : false;

                    bool madre = false;
                    if (SelectExpediente.IMPUTADO_PADRES != null)
                    {
                        foreach (var p in SelectExpediente.IMPUTADO_PADRES)
                        {
                            if (p.ID_PADRE == "P")
                            {
                                SelectPaisDomicilioPadre = p.ID_PAIS != null ? p.ID_PAIS : -1;
                                SelectEntidadDomicilioPadre = p.ID_ENTIDAD != null ? p.ID_ENTIDAD : -1;
                                SelectMunicipioDomicilioPadre = p.ID_MUNICIPIO != null ? p.ID_MUNICIPIO : -1;
                                SelectColoniaDomicilioPadre = p.ID_COLONIA;
                                TextCalleDomicilioPadre = p.CALLE;
                                TextNumeroExteriorDomicilioPadre = p.NUM_EXT;
                                TextNumeroInteriorDomicilioPadre = p.NUM_INT;
                                TextCodigoPostalDomicilioPadre = p.CP;
                            }
                            else
                                if (p.ID_PADRE == "M")
                                {
                                    madre = true;
                                    SelectPaisDomicilioMadre = p.ID_PAIS != null ? p.ID_PAIS : -1;
                                    SelectEntidadDomicilioMadre = p.ID_ENTIDAD != null ? p.ID_ENTIDAD : -1;
                                    SelectMunicipioDomicilioMadre = p.ID_MUNICIPIO != null ? p.ID_MUNICIPIO : -1;
                                    SelectColoniaDomicilioMadre = p.ID_COLONIA;
                                    TextCalleDomicilioMadre = p.CALLE;
                                    TextNumeroExteriorDomicilioMadre = p.NUM_EXT;
                                    TextNumeroInteriorDomicilioMadre = p.NUM_INT;
                                    TextCodigoPostalDomicilioMadre = p.CP;
                                }
                        }
                    }

                    if (!CheckMadreFinado && !madre)
                        MismoDomicilioMadre = true;
                    #endregion

                    #region Alias
                    if (SelectExpediente.ALIAS != null)
                    {
                        ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                    }
                    #endregion

                    #region Apodos
                    if (SelectExpediente.APODO != null)
                    {
                        ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                    }
                    #endregion

                    //Si no ha guardado muestra informacion  del proceso en libertad anterior
                    if (SelectEstadoCivil == -1)
                    {
                        Populate();
                    }

                    #region Actitd General
                    ActitudGeneralEntrv = SelectedProcesoLibertad.ACTITUD_GENERAL;
                    #endregion

                    #region Observaciones
                    TextObservaciones = SelectedProcesoLibertad.OBSERVACIONES;
                    #endregion
                    
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    CamposBusquedaEnabled = BHuellasEnabled = MenuBuscarEnabled = InterconexionEnabled = false;
                    MenuGuardarEnabled = pEditar;

                    Changed = true;
                    StaticSourcesViewModel.SourceChanged = false;
                }
                #region comentado
                //else
                //{
                //    if (SelectExpediente != null)
                //    {
                //        TabsEnabled = true;
                //        #region Datos Generales
                //        AnioBuscar = SelectExpediente.ID_ANIO;
                //        FolioBuscar = SelectExpediente.ID_IMPUTADO;
                //        NombreBuscar = SelectExpediente.NOMBRE;
                //        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                //        ApellidoMaternoBuscar = SelectExpediente.MATERNO;

                //        SelectSexo = SelectExpediente.SEXO;
                //        SelectEstadoCivil = SelectExpediente.ID_ESTADO_CIVIL;
                //        SelectOcupacion = SelectExpediente.ID_OCUPACION;
                //        SelectEscolaridad = SelectExpediente.ID_ESCOLARIDAD;
                //        SelectReligion = SelectExpediente.ID_RELIGION;
                //        //SelectEstadoCivil = SelectIngreso.ID_ESTADO_CIVIL.HasValue ? SelectIngreso.ID_ESTADO_CIVIL : -1;
                //        //SelectOcupacion = SelectIngreso.ID_OCUPACION.HasValue ? SelectIngreso.ID_OCUPACION : -1;
                //        //SelectEscolaridad = SelectIngreso.ID_ESCOLARIDAD.HasValue ? SelectIngreso.ID_ESCOLARIDAD : -1;
                //        //SelectReligion = SelectIngreso.ID_RELIGION.HasValue ? SelectIngreso.ID_RELIGION : -1;
                //        SelectEtnia = SelectExpediente.ID_ETNIA;
                //        SelectedIdioma = SelectExpediente.ID_IDIOMA;
                //        SelectedDialecto = SelectExpediente.ID_DIALECTO;
                //        RequiereTraductor = SelectExpediente.TRADUCTOR == "S" ? true : false;
                //        #endregion

                //        #region Domicilio
                //        SelectPais = SelectExpediente.ID_PAIS;
                //        SelectEntidad = SelectExpediente.ID_ENTIDAD;
                //        SelectMunicipio = SelectExpediente.ID_MUNICIPIO;
                //        SelectColonia = SelectExpediente.ID_COLONIA;
                //        TextCalle = SelectExpediente.DOMICILIO_CALLE;
                //        TextNumeroExterior = SelectExpediente.DOMICILIO_NUM_EXT;
                //        TextNumeroInterior = SelectExpediente.DOMICILIO_NUM_INT;
                //        TextTelefono = SelectExpediente.TELEFONO.Value.ToString();
                //        TextCodigoPostal = SelectExpediente.DOMICILIO_CODIGO_POSTAL;
                //        TextDomicilioTrabajo = SelectExpediente.DOMICILIO_TRABAJO;
                //        AniosEstado = SelectExpediente.RESIDENCIA_ANIOS != null ? SelectExpediente.RESIDENCIA_ANIOS.Value.ToString() : "0";
                //        MesesEstado = SelectExpediente.RESIDENCIA_MESES != null ? SelectExpediente.RESIDENCIA_MESES.Value.ToString() : "0";
                //        //SelectPais = SelectIngreso.ID_PAIS.HasValue ? SelectIngreso.ID_PAIS : -1;
                //        //SelectEntidad = SelectIngreso.ID_ENTIDAD.HasValue ? SelectIngreso.ID_ENTIDAD : -1;
                //        //SelectMunicipio = SelectIngreso.ID_MUNICIPIO.HasValue ? SelectIngreso.ID_MUNICIPIO : -1;
                //        //SelectColonia = SelectIngreso.ID_COLONIA.HasValue ? SelectIngreso.ID_COLONIA : -1;
                //        //TextCalle = SelectIngreso.DOMICILIO_CALLE;
                //        //TextNumeroExterior = SelectIngreso.DOMICILIO_NUM_EXT;
                //        //TextNumeroInterior = SelectIngreso.DOMICILIO_NUM_INT;
                //        //TextTelefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.Value.ToString() : string.Empty;
                //        //TextCodigoPostal = SelectIngreso.DOMICILIO_CP;
                //        //TextDomicilioTrabajo = SelectIngreso.DOMICILIO_TRABAJO;
                //        //AniosEstado = SelectIngreso.RESIDENCIA_ANIOS.HasValue ? SelectIngreso.RESIDENCIA_ANIOS.Value.ToString() : "0";
                //        //MesesEstado = SelectIngreso.RESIDENCIAS_MESES.HasValue ? SelectIngreso.RESIDENCIAS_MESES.Value.ToString() : "0";
                //        #endregion

                //        #region Nacimiento
                //        SelectPaisNacimiento = SelectExpediente.NACIMIENTO_PAIS;
                //        SelectEntidadNacimiento = SelectExpediente.NACIMIENTO_ESTADO;
                //        SelectMunicipioNacimiento = SelectExpediente.NACIMIENTO_MUNICIPIO;
                //        TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;
                //        TextLugarNacimientoExtranjero = SelectExpediente.NACIMIENTO_LUGAR;
                //        #endregion

                //        #region Padres
                //        TextPadrePaterno = SelectExpediente.PATERNO_PADRE;
                //        TextPadreMaterno = SelectExpediente.MATERNO_PADRE;
                //        TextPadreNombre = SelectExpediente.NOMBRE_PADRE;
                //        CheckPadreFinado = SelectExpediente.PADRE_FINADO == "S" ? true : false;
                //        //CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S" ? true : false;

                //        TextMadrePaterno = SelectExpediente.PATERNO_MADRE;
                //        TextMadreMaterno = SelectExpediente.MATERNO_MADRE;
                //        TextMadreNombre = SelectExpediente.NOMBRE_MADRE;
                //        CheckMadreFinado = SelectExpediente.MADRE_FINADO == "S" ? true : false;
                //        //CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S" ? true : false;

                //        if (SelectExpediente.IMPUTADO_PADRES != null)
                //        {
                //            bool mismoDomicilio = true;
                //            foreach (var p in SelectExpediente.IMPUTADO_PADRES)
                //            {
                //                if (p.ID_PADRE == "P")
                //                {
                //                    SelectPaisDomicilioPadre = p.ID_PAIS;
                //                    SelectEntidadDomicilioPadre = p.ID_ENTIDAD;
                //                    SelectMunicipioDomicilioPadre = p.ID_MUNICIPIO;
                //                    SelectColoniaDomicilioPadre = p.ID_COLONIA;
                //                    TextCalleDomicilioPadre = p.CALLE;
                //                    TextNumeroExteriorDomicilioPadre = p.NUM_EXT;
                //                    TextNumeroInteriorDomicilioPadre = p.NUM_INT;
                //                    TextCodigoPostalDomicilioPadre = p.CP;
                //                }
                //                else
                //                    if (p.ID_PADRE == "M")
                //                    {
                //                        mismoDomicilio = false;
                //                        SelectPaisDomicilioMadre = p.ID_PAIS;
                //                        SelectEntidadDomicilioMadre = p.ID_ENTIDAD;
                //                        SelectMunicipioDomicilioMadre = p.ID_MUNICIPIO;
                //                        SelectColoniaDomicilioMadre = p.ID_COLONIA;
                //                        TextCalleDomicilioMadre = p.CALLE;
                //                        TextNumeroExteriorDomicilioMadre = p.NUM_EXT;
                //                        TextNumeroInteriorDomicilioMadre = p.NUM_INT;
                //                        TextCodigoPostalDomicilioMadre = p.CP;
                //                    }
                //            }
                //            if (CheckMadreFinado)
                //                mismoDomicilio = false;
                //            MismoDomicilioMadre = mismoDomicilio;
                //        }
                //        #endregion

                //        #region Alias
                //        if (SelectExpediente.ALIAS != null)
                //        {
                //            ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                //        }
                //        #endregion

                //        #region Apodos
                //        if (SelectExpediente.APODO != null)
                //        {
                //            ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                //        }
                //        #endregion

                //        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);

                //        CamposBusquedaEnabled = BHuellasEnabled = MenuBuscarEnabled = false;
                //        MenuGuardarEnabled = PEditar;

                //        Changed = true;
                //        StaticSourcesViewModel.SourceChanged = false;
                //    }
                //    else
                //        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una imputado");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }

        private void Guardar()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos: "+base.Error);
                    return;
                }
                var obj = new IMPUTADO();
                #region Datos Generales
                obj.ID_CENTRO = 0;
                obj.ID_ANIO = (short)Fechas.GetFechaDateServer.Year;
                obj.NOMBRE = NombreBuscar;
                obj.PATERNO = ApellidoPaternoBuscar;
                obj.MATERNO = ApellidoMaternoBuscar;
                obj.SEXO = SelectSexo;
                //obj.ID_ESTADO_CIVIL = SelectEstadoCivil;
                //obj.ID_OCUPACION = SelectOcupacion;
                //obj.ID_ESCOLARIDAD = SelectEscolaridad;
                //obj.ID_RELIGION = SelectReligion;
                obj.ID_ETNIA = SelectEtnia;
                obj.ID_IDIOMA = SelectedIdioma;
                obj.ID_DIALECTO = SelectedDialecto;
                obj.TRADUCTOR = RequiereTraductor ? "S" : "N";
                #endregion

                #region Domicilio
                //obj.ID_PAIS = SelectPais;
                //obj.ID_ENTIDAD = SelectEntidad;
                //obj.ID_MUNICIPIO = SelectMunicipio;
                //obj.ID_COLONIA = SelectColonia;
                //obj.DOMICILIO_CALLE = TextCalle;
                //obj.DOMICILIO_NUM_EXT = TextNumeroExterior;
                //obj.DOMICILIO_NUM_INT = TextNumeroInterior;
                //obj.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                //obj.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                //obj.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                //obj.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                //obj.RESIDENCIA_MESES = short.Parse(MesesEstado);
                #endregion

                #region Nacimiento
                obj.NACIMIENTO_PAIS = SelectPaisNacimiento;
                obj.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                obj.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                obj.NACIMIENTO_FECHA = TextFechaNacimiento;
                obj.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                #endregion

                #region Padres
                var padres = new List<IMPUTADO_PADRES>();
                obj.PATERNO_PADRE = TextPadrePaterno;
                obj.MATERNO_PADRE = TextPadreMaterno;
                obj.NOMBRE_PADRE = TextPadreNombre;
                //obj.PADRE_FINADO = CheckPadreFinado ? "S" : "N";
                if (!CheckPadreFinado)
                {
                    padres.Add(new IMPUTADO_PADRES()
                    {
                        ID_PADRE = "P",
                        ID_PAIS = SelectPaisDomicilioPadre,
                        ID_ENTIDAD = SelectEntidadDomicilioPadre,
                        ID_MUNICIPIO = SelectMunicipioDomicilioPadre,
                        ID_COLONIA = SelectColoniaDomicilioPadre,
                        CALLE = TextCalleDomicilioPadre,
                        NUM_EXT = TextNumeroExteriorDomicilioPadre,
                        NUM_INT = TextNumeroInteriorDomicilioPadre,
                        CP = TextCodigoPostalDomicilioPadre
                    });
                }
                obj.PATERNO_MADRE = TextMadrePaterno;
                obj.MATERNO_MADRE = TextMadrePaterno;
                obj.NOMBRE_MADRE = TextMadreNombre;
                //obj.MADRE_FINADO = CheckMadreFinado ? "S" : "N";
                if (!CheckMadreFinado && !MismoDomicilioMadre)
                {
                    padres.Add(new IMPUTADO_PADRES()
                    {
                        ID_PADRE = "M",
                        ID_PAIS = SelectPaisDomicilioMadre,
                        ID_ENTIDAD = SelectEntidadDomicilioMadre,
                        ID_MUNICIPIO = SelectMunicipioDomicilioMadre,
                        ID_COLONIA = SelectColoniaDomicilioMadre,
                        CALLE = TextCalleDomicilioMadre,
                        NUM_EXT = TextNumeroExteriorDomicilioMadre,
                        NUM_INT = TextNumeroInteriorDomicilioMadre,
                        CP = TextCodigoPostalDomicilioMadre
                    });
                }
                #endregion

                #region Alias
                var alias = new List<ALIAS>();
                if (ListAlias != null)
                    alias = new List<ALIAS>(ListAlias.Select((w, i) => new ALIAS() { ID_ALIAS = Convert.ToInt16(i + 1), PATERNO = w.PATERNO, MATERNO = w.MATERNO, NOMBRE = w.NOMBRE }));
                //obj.ALIAS = alias;
                #endregion

                #region Apodo
                var apodos = new List<APODO>();
                if (ListApodo != null)
                    apodos = new List<APODO>(ListApodo.Select((w, i) => new APODO() { ID_APODO = Convert.ToInt16(i + 1), APODO1 = w.APODO1 }));
                //obj.APODO = apodos;
                #endregion

                #region Liberado
                //var liberado = new LIBERADO();
                //liberado.OCUPACION_LUGAR = LLugarOcupacion;
                //liberado.LUGAR_FRECUENTA = LLugarFrecuenta;
                //liberado.REGISTRO_FEC = Fechas.GetFechaDateServer;
                //liberado.PAM_NOMBRE = ANombre;
                //liberado.PAM_ID_TIPO_REFERENCIA = ARelacion;
                //liberado.PAM_TIEMPO_CONOCE = ATiempoConocerlo;
                //liberado.PAM_DOMICILIO = ADocmicilio;
                //liberado.PAM_TEL_CELULAR = ATelefonoMovil.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                //liberado.PAM_TEL_FIJO = ATelefonoFijo.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                //liberado.ACTITUD_GENERAL = ActitudGeneralEntrv;
                //liberado.OBSERVACION = TextObservaciones;

                #region Medidas Judiciales
                //var MJ = new LIBERADO_MEDIDA_JUDICIAL();
                //MJ.RES_MC = RMC ? "S" : "N";
                //MJ.RES_SPP = RSSP ? "S" : "N";
                //MJ.RES_PROV_P = RProvp ? "S" : "N";
                //MJ.RES_NUC = RNUC;
                //MJ.RES_CP = RCP;
                //MJ.DELITOS = DDelitos;
                //MJ.RECLASIFICADO = DReclasificado.Value ? "S" : "N";
                //MJ.MEDIDA_JUDICIAL = MMedidaJudicial;
                //MJ.PERIOCIDAD = MPeridiocidad;
                //MJ.APARTIR = MApartir;
                //MJ.DURACION = MDuracion;
                //MJ.DEFENSOR_PUBLICO = DPublico.Value ? "S" : "N";
                //MJ.DEFENSOR_PRIVADO_NOM = DPublico.Value ? string.Empty : DNombreDefensor;
                //MJ.DEFENSOR_PRIVADO_TEL = DPublico.Value ? string.Empty : string.IsNullOrEmpty(DTelefonoDefensor) ? null : DTelefonoDefensor.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

                //var mj = new List<LIBERADO_MEDIDA_JUDICIAL>();
                //mj.Add(new LIBERADO_MEDIDA_JUDICIAL()
                //{
                //    ENTREVISTA_FEC = liberado.REGISTRO_FEC,
                //    //mj.ENTREVISTA_EXP_ANIO
                //    //mj.ENTREVISTA_EXP_FOLIO
                //    //mj.ENTREVISTA_OFICIO
                //    //mj.ACT_EVA_RIESGOS 
                //    //mj.ACT_EVA_ENTREVISTA
                //    //mj.ACT_EVA_OP_TECNICA
                //    //mj.ACT_EVA_OTRA
                //    RES_MC = RMC ? "S" : "N",
                //    RES_SPP = RSSP ? "S" : "N",
                //    RES_PROV_P = RProvp ? "S" : "N",
                //    RES_NUC = RNUC,
                //    RES_CP = RCP,
                //    DELITOS = DDelitos,
                //    RECLASIFICADO = DReclasificado.Value ? "S" : "N",
                //    MEDIDA_JUDICIAL = MMedidaJudicial,
                //    PERIOCIDAD = MPeridiocidad,
                //    APARTIR = MApartir,
                //    DURACION = MDuracion,
                //    DEFENSOR_PUBLICO = DPublico.Value ? "S" : "N",
                //    DEFENSOR_PRIVADO_NOM = DPublico.Value ? string.Empty : DNombreDefensor,
                //    DEFENSOR_PRIVADO_TEL = DPublico.Value ? string.Empty : string.IsNullOrEmpty(DTelefonoDefensor) ? null : DTelefonoDefensor.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                //});
                //liberado.LIBERADO_MEDIDA_JUDICIAL = mj;
                #endregion
                #endregion

                #region Fotos
                var fotos = new List<SSP.Servidor.PROCESO_LIBERTAD_BIOMETRICO>();
                if (ImagesToSave != null)
                    foreach (var item in ImagesToSave)
                    {
                        var encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                        encoder.QualityLevel = 100;
                        var bit = new byte[0];
                        using (MemoryStream stream = new MemoryStream())
                        {
                            encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                            encoder.Save(stream);
                            bit = stream.ToArray();
                            stream.Close();
                        }
                        fotos.Add(new SSP.Servidor.PROCESO_LIBERTAD_BIOMETRICO()
                        {
                            ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                            ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO,
                            ID_ANIO = SelectedProcesoLibertad.ID_ANIO,
                            ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO,
                            ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD,
                            BIOMETRICO = bit,
                            //BIOMETRICO = bit,
                            //ID_ANIO = 0,
                            //ID_CENTRO = 0,
                            //ID_IMPUTADO = 0,
                            //ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                            //ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                        }
                        );
                    }
                #endregion

                #region Huellas
                var biometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();
                if (HuellasCapturadas != null)
                    foreach (var item in HuellasCapturadas)
                    {
                        biometrico.Add(new SSP.Servidor.IMPUTADO_BIOMETRICO()
                        {
                            ID_ANIO = 0,
                            ID_CENTRO = 0,
                            ID_IMPUTADO = 0,
                            BIOMETRICO = item.BIOMETRICO,
                            ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                            ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                            CALIDAD = item.CALIDAD.HasValue ? item.CALIDAD : null
                        });
                    }
                #endregion

                #region Proceso en Libertad
                var pl = new PROCESO_LIBERTAD();
                //ID_CENTRO
                //ID_ANIO
                //ID_IMPUTADO
                //ID_PROCESO_LIBERTAD
                pl.ID_ESTADO_CIVIL = SelectEstadoCivil;
                pl.ID_OCUPACION = SelectOcupacion;
                pl.ID_ESCOLARIDAD = SelectEscolaridad;
                pl.ID_RELIGION = SelectReligion;
                pl.DOMICILIO_CALLE = TextCalle;
                pl.DOMICILIO_NUM_EXT = TextNumeroExterior;
                pl.DOMICILIO_NUM_INT = TextNumeroInterior;
                pl.ID_COLONIA = SelectColonia;
                pl.ID_ENTIDAD = SelectEntidad;
                pl.ID_MUNICIPIO = SelectMunicipio;
                pl.ID_PAIS = SelectPais;
                pl.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                pl.RESIDENCIA_ANIOS = !string.IsNullOrEmpty(AniosEstado) ? short.Parse(AniosEstado) : (short)0;
                pl.RESIDENCIA_MESES = !string.IsNullOrEmpty(MesesEstado) ? short.Parse(MesesEstado) : (short)0;
                pl.TELEFONO = !string.IsNullOrEmpty(TextTelefono) ? (long?)long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")) : null;
                pl.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                pl.MADRE_FINADO = CheckPadreFinado ? "S" : "N";
                pl.PADRE_FINADO = CheckMadreFinado ? "S" : "N";
                //pl.LUGAR_RESIDENCIA = 
                //pl.NUMERO_IDENTIFICACION
                //pl.ID_TIPO_DISCAPACIDAD
                //pl.ESTATURA
                //pl.PESO
                //pl.FECHA = PFecha;
                //pl.NUC = PNUC;
                //pl.CP_ANIO = PCPAnio;
                //pl.CP_FOLIO = PCPFolio;
                //ESTATUS
                pl.ACTITUD_GENERAL = ActitudGeneralEntrv;
                pl.OBSERVACIONES = TextObservaciones;
                #endregion

                //No existe registro del interno
                if (SelectExpediente == null)
                {
                    if (!pInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    var hoy = Fechas.GetFechaDateServer;
                    obj.ID_CENTRO = 0;
                    obj.ID_ANIO = (short)hoy.Year;

                    obj.ALIAS = alias;
                    obj.APODO = apodos;
                    obj.IMPUTADO_PADRES = padres;
                    obj.IMPUTADO_BIOMETRICO = biometrico;
                    pl.ID_PROCESO_LIBERTAD = 1;
                    pl.PROCESO_LIBERTAD_BIOMETRICO = fotos;
                    obj.PROCESO_LIBERTAD.Add(pl);
                    obj.ID_IMPUTADO = new cImputado().Insertar(obj);
                    if (obj.ID_IMPUTADO > 0)
                    {
                        StaticSourcesViewModel.SourceChanged = false;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                        SelectExpediente = new cImputado().Obtener(obj.ID_IMPUTADO, obj.ID_ANIO, obj.ID_CENTRO).FirstOrDefault();
                        if (SelectExpediente != null)
                            SelectedProcesoLibertad = SelectExpediente.PROCESO_LIBERTAD.FirstOrDefault();
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Error", "Error al guardar la inoformación");
                    }
                }
                else
                {
                    if (!pEditar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    obj.ID_CENTRO = SelectExpediente.ID_CENTRO;
                    obj.ID_ANIO = SelectExpediente.ID_ANIO;
                    obj.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;

                    pl.ID_CENTRO = SelectExpediente.ID_CENTRO;
                    pl.ID_ANIO = SelectExpediente.ID_ANIO;
                    pl.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                    //Existe registro del interno pero no existe registro del proceso
                    if (SelectedProcesoLibertad == null)
                        pl.ID_PROCESO_LIBERTAD = 0;
                    else
                    {
                        //Existe imputado y proceso de libertad
                        pl.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                        pl.ESTATURA = SelectedProcesoLibertad.ESTATURA;
                        pl.PESO = SelectedProcesoLibertad.PESO;
                        pl.FECHA = SelectedProcesoLibertad.FECHA;
                        pl.NUC = SelectedProcesoLibertad.NUC;
                        pl.ESTATUS = SelectedProcesoLibertad.ESTATUS;
                        pl.TIPO = SelectedProcesoLibertad.TIPO;
                        pl.CP_ANIO = SelectedProcesoLibertad.CP_ANIO;
                        pl.CP_FOLIO = SelectedProcesoLibertad.CP_FOLIO;
                    }
                    if (new cProcesoLibertad().Actualizar(obj, padres, alias, apodos, biometrico, pl,fotos))
                    {
                        StaticSourcesViewModel.SourceChanged = false;
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Error", "Error al guardar la inoformación");
                    }
                }

                #region Comentado
                //if (SelectExpediente == null)//Insert
                //{
                //    obj.ALIAS = alias;
                //    obj.APODO = apodos;
                //    obj.IMPUTADO_PADRES = padres;
                //    obj.IMPUTADO_BIOMETRICO = biometrico;
                //    obj.LIBERADO = liberado;
                //    obj.LIBERADO.LIBERADO_MEDIDA_JUDICIAL.Add(MJ);
                //    if (new cImputado().Insertar(obj) > 0)
                //    {
                //        MenuGuardarEnabled = false;
                //        StaticSourcesViewModel.SourceChanged = false;
                //        new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                //    }
                //    else
                //        new Dialogos().ConfirmacionDialogo("Error", "Error al guardar la información");
                //}
                //else//update
                //{
                //    #region Generales
                //    obj.ID_CENTRO = SelectExpediente.ID_CENTRO;
                //    obj.ID_ANIO = SelectExpediente.ID_ANIO;
                //    obj.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;

                //    if (SelectMJ != null)
                //    {
                //        MJ.ID_CENTRO = SelectMJ.ID_CENTRO;
                //        MJ.ID_ANIO = SelectMJ.ID_ANIO;
                //        MJ.ID_IMPUTADO = SelectMJ.ID_IMPUTADO;
                //        MJ.ID_CONSEC = SelectMJ.ID_CONSEC;
                //    }

                //    #endregion
                //    if (new cImputado().ActualizarLiberado(obj, padres, alias, apodos, biometrico, liberado, MJ))
                //    {
                //        MenuGuardarEnabled = false;
                //        StaticSourcesViewModel.SourceChanged = false;
                //        new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");
                //    }
                //    else
                //        new Dialogos().ConfirmacionDialogo("Error", "Error al guardar la información");
                //}
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
        }
        #endregion

        #region Alias
        private void LimpiarAlias()
        {
            NombreAlias = PaternoAlias = MaternoAlias = string.Empty;
            SelectAlias = null;
        }

        private void PopulateAlias()
        {
            NombreAlias = SelectAlias.NOMBRE;
            PaternoAlias = SelectAlias.PATERNO;
            MaternoAlias = SelectAlias.MATERNO;
        }

        private void AgregarAlias()
        {
            if (!base.HasErrors)
            {
                if (ListAlias == null)
                    ListAlias = new ObservableCollection<ALIAS>();
                ListAlias.Add(new ALIAS() { NOMBRE = NombreAlias, PATERNO = PaternoAlias, MATERNO = MaternoAlias });
            }
        }

        private void EditarAlias()
        {
            if (!base.HasErrors)
            {
                SelectAlias.NOMBRE = NombreAlias;
                SelectAlias.PATERNO = PaternoAlias;
                SelectAlias.MATERNO = MaternoAlias;
                ListAlias = new ObservableCollection<ALIAS>(ListAlias);
            }

        }

        private void EliminarAlias()
        {
            ListAlias.Remove(SelectAlias);
        }

        #endregion

        #region Apodo
        private void LimpiarApodo()
        {
            Apodo = string.Empty;
            SelectApodo = null;
        }

        private void PopulateApodo()
        {
            Apodo = SelectApodo.APODO1;
        }

        private void AgregarApodo()
        {
            if (!base.HasErrors)
            {
                if (ListApodo == null)
                    ListApodo = new ObservableCollection<APODO>();
                ListApodo.Add(new APODO() { APODO1 = Apodo });
            }
        }

        private void EditarApodo()
        {
            if (!base.HasErrors)
            {
                SelectApodo.APODO1 = Apodo;
                ListApodo = new ObservableCollection<APODO>(ListApodo);
            }

        }

        private void EliminarApodo()
        {
            ListApodo.Remove(SelectApodo);
        }
        #endregion

        #region Camara
        private async void OnLoad(FotosHuellasDigitalesEstatusAdminView Window = null)
        {
            try
            {
                CamposBusquedaEnabled = true;
                #region [Huellas Digitales]
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 240;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                if (PopUpsViewModels.MainWindow.HuellasView != null)
                    myStoryboard.Begin(PopUpsViewModels.MainWindow.HuellasView.Ln);

                if (FindVisualChildren<System.Windows.Controls.Image>(Window).ToList().Any())
                    CargarHuellas();
                #endregion
                #region [Web cam]
                Window.Unloaded += (s, e) =>
                {
                    try
                    {
                        if (CamaraWeb != null)
                        {
                            CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de fotos y huellas", ex);
                    }
                };

                if (TabFotosHuellas)
                {
                    CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                    await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.LeftFace, Window.RightFace, Window.FrontFace });
                    _ImagesToSave = new List<ImageSourceToSave>();
                    if (selectedProcesoLibertad != null)
                    {
                        if (selectedProcesoLibertad.PROCESO_LIBERTAD_BIOMETRICO != null)
                        {
                            var ingresobiometrico = selectedProcesoLibertad.PROCESO_LIBERTAD_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO);
                            if (ingresobiometrico.Any())
                            {
                                ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.FrontFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault().BIOMETRICO)));
                                ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.LeftFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO).SingleOrDefault().BIOMETRICO)));
                                ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.RightFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO).SingleOrDefault().BIOMETRICO)));
                            }
                        }
                    }
                    //if (SelectExpediente != null)
                    //{
                    //    var ingresobiometrico = SelectExpediente.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO || w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO);
                    //    if (ingresobiometrico.Any())
                    //    {
                    //        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.FrontFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault().BIOMETRICO)));
                    //        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.LeftFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO).SingleOrDefault().BIOMETRICO)));
                    //        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.RightFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO).SingleOrDefault().BIOMETRICO)));
                    //    }
                    //}
                    //_ventanaGuardar = VentanaGuardar.FOTOSYHUELLASDIGITALES;
                }
                #endregion
                //SetValidacionesGenerales();

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la vista de captura de huellas y fotos.", ex);
            }
        }

        private void OnTakePicture(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (Processing)
                    return;
                Processing = true;
                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje(Picture.Name == "LeftFace" ? "LADO IZQUIERDO" : Picture.Name == "RightFace" ? "LADO DERECHO" : "CENTRO", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                Processing = false;
            }
            catch (Exception ex)
            {
                //throw new ApplicationException("Ocurrió un error al tomar fotografía \n\n" + ex.Message);
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar fotografía, verifique que su camara se encuentre conectada", ex);
            }
        }

        private void OpenSetting(string obj)
        {
            CamaraWeb.AdvanceSetting();
        }
        #endregion

        #region Huellas Digitales
        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;

                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);

                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }

                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueDerecho = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                PulgarIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                IndiceIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MedioIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                AnularIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            Application.Current.Dispatcher.Invoke((Action)(delegate
                                            {
                                                MeñiqueIzquierdo = ObtenerCalidad(nNFIQ);
                                            }));
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                if (SelectExpediente == null)
                                    ScannerMessage = "Huellas Capturadas Correctamente";
                                else
                                    if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).ToList().Any())
                                        ScannerMessage = "Huellas Actualizadas Correctamente";
                                    else
                                        ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });

                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();

                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }

        private System.Windows.Media.Brush ObtenerCalidad(int nNFIQ)
        {
            if (nNFIQ == 0)
                return new SolidColorBrush(Colors.White);
            if (nNFIQ == 3)
                return new SolidColorBrush(Colors.Yellow);
            if (nNFIQ == 4)
                return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.LightGreen);
        }

        private void OkClick(object ImgPrint = null)
        {
            ShowPopUp = Visibility.Hidden;
        }

        private async void OnBuscarPorHuella(string obj = "")
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = false;// Parametro.GuardarHuellaEnBusquedaRegistro;
            if (requiereGuardarHuellas)
                try
                {
                    nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                }
                catch
                {
                    bandera = false;
                }
            else
                bandera = false;

            var windowBusqueda = new BusquedaHuella();
            windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);
            if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                return;
            }
            windowBusqueda.Owner = PopUpsViewModels.MainWindow;
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaHuellaViewModel)windowBusqueda.DataContext).IsSucceed)
                        return;

                    SelectExpediente = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;


                    if (SelectExpediente == null)
                    {
                        CamposBusquedaEnabled = true;
                        MenuBuscarEnabled = pConsultar;
                    }
                    else
                    {
                        //AnioBuscar = SelectExpediente.ID_ANIO;
                        //FolioBuscar = SelectExpediente.ID_IMPUTADO;
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        clickSwitch("buscar_visible");
                        MenuBuscarEnabled = pConsultar;
                        MenuGuardarEnabled = pEditar;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            AceptarBusquedaHuellaFocus = true;
        }

        private async void OnBuscarPorHuellaEmpleado(string obj = "")
        {
            try
            {
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                //if (!PConsultar)
                //{
                //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                //    return;
                //}

                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                if (requiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        bandera = false;
                    }
                else
                    bandera = false;

                var windowBusqueda = new BusquedaHuella();
                windowBusqueda.DataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_EMPLEADO, nRet == 0, requiereGuardarHuellas);
                windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                if (nRet != 0)
                    if (((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0)
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                        StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                        return;
                    }
                windowBusqueda.Owner = PopUpsViewModels.MainWindow;
                windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
                windowBusqueda.Closed += async (s, e) =>
                {
                    HuellasCapturadas = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
                    if (bandera == true)
                        CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var huella = ((BusquedaHuellaViewModel)windowBusqueda.DataContext);
                    if (!huella.IsSucceed)
                        return;
                    if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_EMPLEADO);
                        return;
                    }
                    if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null)
                        return;
                    
                    
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                    var SelectPersona = huella.SelectRegistro.Persona;
                    if (SelectPersona != null)
                    {
                        var persona = new cPersona().Obtener(SelectPersona.ID_PERSONA).FirstOrDefault();
                        if (persona != null)
                        {
                            if (persona.EMPLEADO != null)
                            {
                                if (persona.EMPLEADO.USUARIO != null)
                                {
                                    var usr = persona.EMPLEADO.USUARIO.FirstOrDefault();
                                    if(usr != null)
                                    {
                                        if(usr.USUARIO_ROL.Any(w => w.ID_ROL == 49))//rol para revisar la huella
                                        {
                                            if (SelectedMedidaPresentacionDetalle == null)
                                            { 
                                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una fecha de presentacion");
                                                return;
                                            }
                                            else
                                            {
                                                var mpd = new MEDIDA_PRESENTACION_DETALLE();
                                                mpd.ID_CENTRO = SelectedMedidaPresentacionDetalle.ID_CENTRO;
                                                mpd.ID_ANIO = SelectedMedidaPresentacionDetalle.ID_ANIO;
                                                mpd.ID_IMPUTADO = SelectedMedidaPresentacionDetalle.ID_IMPUTADO;
                                                mpd.ID_PROCESO_LIBERTAD = SelectedMedidaPresentacionDetalle.ID_PROCESO_LIBERTAD;
                                                mpd.ID_MEDIDA_LIBERADO = SelectedMedidaPresentacionDetalle.ID_MEDIDA_LIBERADO;
                                                mpd.ID_DETALLE = SelectedMedidaPresentacionDetalle.ID_DETALLE;
                                                mpd.ID_MEDIDA_LUGAR = SelectedMedidaPresentacionDetalle.ID_MEDIDA_LUGAR;
                                                mpd.FECHA = SelectedMedidaPresentacionDetalle.FECHA;
                                                mpd.ASISTENCIA = "S";
                                                mpd.OBSERVACION = MPObservacion;
                                                mpd.FECHA_ASISTENCIA = Fechas.GetFechaDateServer;
                                                if (new cMedidaPresentacionDetalle().Actualizar(mpd))
                                                {
                                                    new Dialogos().ConfirmacionDialogo("Éxito", "Se ha guardado la asistencia correctamente.");
                                                    SelectedMedidaLibertad = new cMedidaLibertad().Obtener(SelectedMedidaLibertad.ID_CENTRO, SelectedMedidaLibertad.ID_ANIO, SelectedMedidaLibertad.ID_IMPUTADO, SelectedMedidaLibertad.ID_PROCESO_LIBERTAD, SelectedMedidaLibertad.ID_MEDIDA_LIBERADO);
                                                    LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(SelectedMedidaLibertad.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE.OrderBy(w => w.FECHA));
                                                    return;
                                                }
                                                else
                                                {
                                                    new Dialogos().ConfirmacionDialogo("Error", "Ha ocurrido un error al guardado la asistencia.");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    new Dialogos().ConfirmacionDialogo("Validación", "El usuario no tieve privilegios para realizar esta acción");

                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        private void CargarHuellas()
        {
            try
            {
                //if (SelectExpediente == null)
                //    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    //if (SelectExpediente!=null)
                    //{
                    //    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 30).ToList();
                    //    //HuellasCapturadas = Huellas;
                    //}


                    if (HuellasCapturadas != null)
                        foreach (var item in HuellasCapturadas.Where(w => w.ID_TIPO_FORMATO == enumTipoFormato.FMTO_DP))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                switch ((enumTipoBiometrico)item.ID_TIPO_BIOMETRICO)
                                {
                                    case enumTipoBiometrico.PULGAR_DERECHO:
                                        PulgarDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.INDICE_DERECHO:
                                        IndiceDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MEDIO_DERECHO:
                                        MedioDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.ANULAR_DERECHO:
                                        AnularDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MENIQUE_DERECHO:
                                        MeñiqueDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.PULGAR_IZQUIERDO:
                                        PulgarIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.INDICE_IZQUIERDO:
                                        IndiceIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MEDIO_IZQUIERDO:
                                        MedioIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.ANULAR_IZQUIERDO:
                                        AnularIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    case enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                        MeñiqueIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                        break;
                                    default:
                                        break;
                                }
                            }));
                        }
                });

                if (SelectExpediente != null)
                {
                    if (SelectExpediente.IMPUTADO_BIOMETRICO != null ? SelectExpediente.IMPUTADO_BIOMETRICO.Count > 0 : true)
                    {
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        foreach (var biometrico in SelectExpediente.IMPUTADO_BIOMETRICO)
                        {
                            switch (biometrico.ID_TIPO_BIOMETRICO)
                            {
                                case 0:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 1:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 2:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 3:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 4:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 5:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 6:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 7:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 8:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;
                                case 9:
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = biometrico.BIOMETRICO, CALIDAD = biometrico.CALIDAD });
                                    if (biometrico.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)
                                        HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = biometrico.BIOMETRICO });
                                    break;

                            }
                        }
                    }
                }

                LoadHuellas.Start();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Interconexion
        private void OnBuscarNUCInterconexion(string obj = "")
        {
            if (!pConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            var windowBusqueda = new BuscarNUCView();
            windowBusqueda.DataContext = new BusquedaNUCViewModel();
            windowBusqueda.KeyDown += (s, e) =>
            {
                try
                {
                    if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar nuc", ex);
                }
            };
            windowBusqueda.Closed += (s, e) =>
            {
                try
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaNUCViewModel)windowBusqueda.DataContext).IsSucceed)
                        return;

                    SelectedInterconexion = ((BusquedaNUCViewModel)windowBusqueda.DataContext).SelectedInterconexion;
                    //LstImputadoFiliacion = ((BusquedaNUCViewModel)windowBusqueda.DataContext).LstImputadoFiliacion;
                    var Imputado = ((BusquedaNUCViewModel)windowBusqueda.DataContext).Imputado;
                    if (Imputado == null)
                    {
                        if (SelectedInterconexion != null)
                        {
                            ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                            ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                            NombreBuscar = SelectedInterconexion.NOMBRE;
                            clickSwitch("buscar_visible");
                            
                        }
                        CamposBusquedaEnabled = true;
                        return;
                    }
                    else
                    {
                        //AnioBuscar = Imputado.ID_ANIO;
                        //FolioBuscar = Imputado.ID_IMPUTADO;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        SelectExpediente = Imputado;
                        clickSwitch("buscar_visible");
                        //NOMBRE DE INTERCONEXION
                        //if (SelectedInterconexion != null)
                        //{
                        //    ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                        //    ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                        //    NombreBuscar = SelectedInterconexion.NOMBRE;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REGISTRO_LIBERADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                    {
                        pInsertar = MenuGuardarEnabled = true;
                    }
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                    {
                        pConsultar = BHuellasEnabled = InterconexionEnabled = true;
                    }
                    if (p.IMPRIMIR == 1)
                    {
                        pImprimir = MenuReporteEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Loads
        private void DatosIdentificacionLoad(DatosGeneralesIdentificacionEstatusAdminView obj) 
        {
            ValidacionesLiberado();
        }
        #endregion

        #region Proceso
        private void PopulateProcesoLibertad() 
        {
            try
            {
                TabIdentificacionEnabled = TabMedidaEnabled = true;
                LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(SelectedProcesoLibertad.MEDIDA_DOCUMENTO);
                LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(SelectedProcesoLibertad.MEDIDA_LIBERTAD);
                LstSeguimiento = new ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO>(SelectedProcesoLibertad.PROCESO_LIBERTAD_SEGUIMIENTO);
                Obtener();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void GuardarProceso() 
        {
            try
            {
                var pl = new PROCESO_LIBERTAD();
                pl.FECHA = PFecha;
                pl.NUC = PNUC;
                pl.CP_ANIO = PCPAnio;
                pl.CP_FOLIO = PCPFolio;
                pl.ESTATUS = "S";
                pl.TIPO = PTipo;
                if (SelectExpediente != null)
                {
                    var existe = new cProcesoLibertad().Obtener(SelectExpediente.ID_CENTRO, SelectExpediente.ID_ANIO, SelectExpediente.ID_IMPUTADO, null, PNUC);
                    if (existe == null)
                    {
                        pl.ID_PROCESO_LIBERTAD = 0;
                        pl.ID_CENTRO = SelectExpediente.ID_CENTRO;
                        pl.ID_ANIO = SelectExpediente.ID_ANIO;
                        pl.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                        pl.ID_PROCESO_LIBERTAD = new cProcesoLibertad().Insertar(pl);
                        if (pl.ID_PROCESO_LIBERTAD > 0)
                        {
                            //SelectedProcesoLibertad = new cProcesoLibertad().Obtener(pl.ID_CENTRO, pl.ID_ANIO, pl.ID_IMPUTADO, pl.ID_PROCESO_LIBERTAD);
                            //LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(new cProcesoLibertad().ObtenerTodos(SelectExpediente.ID_CENTRO, SelectExpediente.ID_ANIO, SelectExpediente.ID_IMPUTADO));
                            SelectExpediente = new cImputado().Obtener(SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO).FirstOrDefault();
                            LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                            SelectedProcesoLibertad = LstProcesoLibertad.Where(w => w.ID_CENTRO == SelectExpediente.ID_CENTRO && w.ID_ANIO == SelectExpediente.ID_ANIO && w.ID_IMPUTADO == SelectExpediente.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == pl.ID_PROCESO_LIBERTAD).FirstOrDefault();
                            Populate();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD);
                            new Dialogos().ConfirmacionDialogo("Éxito", "Información guardada correctamente");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Error al guardar información");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "El NUC ya esta registrado en este imputado");
                }
                else
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var imp = new IMPUTADO();
                    imp.ID_CENTRO = 0;
                    imp.ID_ANIO = (short)hoy.Year;
                    imp.NOMBRE = NombreBuscar;
                    imp.PATERNO = ApellidoPaternoBuscar;
                    imp.MATERNO = ApellidoMaternoBuscar;
                    pl.ID_PROCESO_LIBERTAD = 1;
                    imp.PROCESO_LIBERTAD.Add(pl);
                    imp.ID_IMPUTADO = new cImputado().Insertar(imp);
                    if (imp.ID_IMPUTADO > 0)
                    {
                        //LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(new cProcesoLibertad().ObtenerTodos(imp.ID_CENTRO, imp.ID_ANIO, imp.ID_IMPUTADO));
                        SelectExpediente = new cImputado().Obtener(imp.ID_IMPUTADO, imp.ID_ANIO, imp.ID_CENTRO).FirstOrDefault();
                        LstProcesoLibertad = new ObservableCollection<PROCESO_LIBERTAD>(SelectExpediente.PROCESO_LIBERTAD);
                        SelectedProcesoLibertad = LstProcesoLibertad.Where(w => w.ID_CENTRO == imp.ID_CENTRO && w.ID_ANIO == imp.ID_ANIO && w.ID_IMPUTADO == imp.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == 1).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD);
                        new Dialogos().ConfirmacionDialogo("Éxito", "Información guardada correctamente"); 
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Error", "Error al guardar información");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar proceso", ex);
            }
        }

        private void GuardarGenerales()
        {
            try
            {
                if (SelectProceso != null)
                {
                    if (base.HasErrors)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos: " + base.Error);
                        return;
                    }
                    var obj = new IMPUTADO();
                    var pl = new PROCESO_LIBERTAD();
                    #region Datos Generales
                    obj.ID_CENTRO = 0;
                    obj.ID_ANIO = (short)Fechas.GetFechaDateServer.Year;
                    obj.NOMBRE = NombreBuscar;
                    obj.PATERNO = ApellidoPaternoBuscar;
                    obj.MATERNO = ApellidoMaternoBuscar;
                    obj.SEXO = SelectSexo;
                    
                    pl.ID_ESTADO_CIVIL = SelectEstadoCivil;
                    pl.ID_OCUPACION = SelectOcupacion;
                    pl.ID_ESCOLARIDAD = SelectEscolaridad;
                    pl.ID_RELIGION = SelectReligion;
                    
                    obj.ID_ETNIA = SelectEtnia;
                    obj.ID_IDIOMA = SelectedIdioma;
                    obj.ID_DIALECTO = SelectedDialecto;
                    obj.TRADUCTOR = RequiereTraductor ? "S" : "N";
                    #endregion

                    #region Domicilio
                    pl.ID_PAIS = SelectPais;
                    pl.ID_ENTIDAD = SelectEntidad;
                    pl.ID_MUNICIPIO = SelectMunicipio;
                    pl.ID_COLONIA = SelectColonia;
                    pl.DOMICILIO_CALLE = TextCalle;
                    pl.DOMICILIO_NUM_EXT = TextNumeroExterior;
                    pl.DOMICILIO_NUM_INT = TextNumeroInterior;
                    pl.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                    pl.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                    pl.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                    pl.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                    pl.RESIDENCIA_MESES = short.Parse(MesesEstado);
                    #endregion

                    #region Nacimiento
                    obj.NACIMIENTO_PAIS = SelectPaisNacimiento;
                    obj.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                    obj.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                    obj.NACIMIENTO_FECHA = TextFechaNacimiento;
                    obj.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                    #endregion

                    #region Padres
                    var padres = new List<IMPUTADO_PADRES>();
                    obj.PATERNO_PADRE = TextPadrePaterno;
                    obj.MATERNO_PADRE = TextPadreMaterno;
                    obj.NOMBRE_PADRE = TextPadreNombre;
                    
                    pl.PADRE_FINADO = CheckPadreFinado ? "S" : "N";
                    
                    if (!CheckPadreFinado)
                    {
                        padres.Add(new IMPUTADO_PADRES()
                        {
                            ID_PADRE = "P",
                            ID_PAIS = SelectPaisDomicilioPadre,
                            ID_ENTIDAD = SelectEntidadDomicilioPadre,
                            ID_MUNICIPIO = SelectMunicipioDomicilioPadre,
                            ID_COLONIA = SelectColoniaDomicilioPadre,
                            CALLE = TextCalleDomicilioPadre,
                            NUM_EXT = TextNumeroExteriorDomicilioPadre,
                            NUM_INT = TextNumeroInteriorDomicilioPadre,
                            CP = TextCodigoPostalDomicilioPadre
                        });
                    }
                    obj.PATERNO_MADRE = TextMadrePaterno;
                    obj.MATERNO_MADRE = TextMadrePaterno;
                    obj.NOMBRE_MADRE = TextMadreNombre;
                  
                    pl.MADRE_FINADO = CheckMadreFinado ? "S" : "N";
                    if (!CheckMadreFinado && !MismoDomicilioMadre)
                    {
                        padres.Add(new IMPUTADO_PADRES()
                        {
                            ID_PADRE = "M",
                            ID_PAIS = SelectPaisDomicilioMadre,
                            ID_ENTIDAD = SelectEntidadDomicilioMadre,
                            ID_MUNICIPIO = SelectMunicipioDomicilioMadre,
                            ID_COLONIA = SelectColoniaDomicilioMadre,
                            CALLE = TextCalleDomicilioMadre,
                            NUM_EXT = TextNumeroExteriorDomicilioMadre,
                            NUM_INT = TextNumeroInteriorDomicilioMadre,
                            CP = TextCodigoPostalDomicilioMadre
                        });
                    }
                    #endregion

                    #region Alias
                    var alias = new List<ALIAS>();
                    if (ListAlias != null)
                        alias = new List<ALIAS>(ListAlias.Select((w, i) => new ALIAS() { ID_ALIAS = Convert.ToInt16(i + 1), PATERNO = w.PATERNO, MATERNO = w.MATERNO, NOMBRE = w.NOMBRE }));
                    //obj.ALIAS = alias;
                    #endregion

                    #region Apodo
                    var apodos = new List<APODO>();
                    if (ListApodo != null)
                        apodos = new List<APODO>(ListApodo.Select((w, i) => new APODO() { ID_APODO = Convert.ToInt16(i + 1), APODO1 = w.APODO1 }));
                    //obj.APODO = apodos;
                    #endregion

                    #region Fotos
                    var biometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();
                    if (ImagesToSave != null)
                        foreach (var item in ImagesToSave)
                        {
                            var encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                            encoder.QualityLevel = 100;
                            var bit = new byte[0];
                            using (MemoryStream stream = new MemoryStream())
                            {
                                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
                                encoder.Save(stream);
                                bit = stream.ToArray();
                                stream.Close();
                            }
                            biometrico.Add(new SSP.Servidor.IMPUTADO_BIOMETRICO()
                            {
                                BIOMETRICO = bit,
                                ID_ANIO = 0,
                                ID_CENTRO = 0,
                                ID_IMPUTADO = 0,
                                ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                            }
                            );
                        }
                    #endregion

                    #region Huellas
                    if (HuellasCapturadas != null)
                        foreach (var item in HuellasCapturadas)
                        {
                            biometrico.Add(new SSP.Servidor.IMPUTADO_BIOMETRICO()
                            {
                                ID_ANIO = 0,
                                ID_CENTRO = 0,
                                ID_IMPUTADO = 0,
                                BIOMETRICO = item.BIOMETRICO,
                                ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                CALIDAD = item.CALIDAD.HasValue ? item.CALIDAD : null
                            });
                        }
                    #endregion

                    #region Actitud General
                    pl.ACTITUD_GENERAL = ActitudGeneralEntrv;
                    #endregion

                    #region Observaciones
                    pl.OBSERVACIONES = TextObservaciones;
                    #endregion

                    if (new cProcesoLibertad().Actualizar(pl, obj, alias,apodos, biometrico))
                    {
                            var foto = biometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                            if (foto != null)
                            {
                                if (foto.BIOMETRICO != null)
                                    ImagenInterno = foto.BIOMETRICO;
                                else
                                    ImagenInterno = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenInterno = new Imagenes().getImagenPerson();
                        new Dialogos().ConfirmacionDialogo("Éxito", "La informacion se guardo correctamente");
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar", ex);
            }
        }

        private async void ActualizarEstatus()
        {
            if (await new Dialogos().ConfirmarEliminar("Validación", "¿Desea cambiar el estatus?") != 0)
            {
                var obj = new PROCESO_LIBERTAD();
                obj.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                obj.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                obj.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                obj.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                obj.TIPO = 2;
                if (new cProcesoLibertad().ActualizarEstatus(obj))
                {
                    new Dialogos().ConfirmacionDialogo("Éxito", "Se ha actualizado a sentenciado");
                    TipoEnabled = false;
                }
            }
            else
            {
                PTipo = 1;
            }
            TipoEnabled = true;
        }
        #endregion

        #region Medida
        private void LimpiarMedida()
        {
            MDocumento = MMedidaTipo = MMedida = -1;
            MFechaFin = MFechaInicio = null;
            MObservacion = string.Empty;
        }

        private void PopulateMedida()
        {
            try
            {
                if (SelectedMedidaLibertad != null)
                {
                    MDocumento = SelectedMedidaLibertad.ID_DOCUMENTO;
                    MMedidaTipo = SelectedMedidaLibertad.ID_TIPO_MEDIDA.HasValue ? SelectedMedidaLibertad.ID_TIPO_MEDIDA.Value : (short)-1;
                    MMedida = SelectedMedidaLibertad.ID_MEDIDA.HasValue ?  SelectedMedidaLibertad.ID_MEDIDA.Value : (short)-1;
                    MFechaInicio = SelectedMedidaLibertad.FECHA_INICIO;
                    MFechaFin = SelectedMedidaLibertad.FECHA_FINAL;
                    MObservacion = SelectedMedidaLibertad.OBSERVACION;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una medida en libertad");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar medida", ex);
            }
        }

        private void GuardarMedida()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. "+base.Error);
                    return;
                }
                var obj = new MEDIDA_LIBERTAD();
                obj.ID_DOCUMENTO = MDocumento;
                obj.ID_TIPO_MEDIDA = MMedidaTipo != -1 ? (short?)MMedidaTipo : null;
                obj.ID_MEDIDA = MMedida != -1 ? (short?)MMedida : null;
                obj.FECHA_INICIO = MFechaInicio;
                obj.FECHA_FINAL = MFechaFin;
                obj.OBSERVACION = MObservacion;
                if (SelectedMedidaLibertad != null)
                {
                    obj.ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO;
                    obj.ID_ANIO= SelectedMedidaLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
                    if (new cMedidaLibertad().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(new cMedidaLibertad().ObtenerTodos(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD));
                        SelectedMedidaLibertad = LstMedidaLibertad.Where(w => w.ID_CENTRO == obj.ID_CENTRO && w.ID_ANIO == obj.ID_ANIO && w.ID_IMPUTADO == obj.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == obj.ID_PROCESO_LIBERTAD && w.ID_MEDIDA_LIBERADO == obj.ID_MEDIDA_LIBERADO).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA);
                    }
                }
                else
                {
                obj.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                obj.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                obj.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                obj.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                obj.ID_MEDIDA_LIBERADO = new cMedidaLibertad().Insertar(obj);
                if (obj.ID_MEDIDA_LIBERADO > 0)
                {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(new cMedidaLibertad().ObtenerTodos(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD));
                        SelectedMedidaLibertad = LstMedidaLibertad.Where(w => w.ID_CENTRO == obj.ID_CENTRO && w.ID_ANIO == obj.ID_ANIO && w.ID_IMPUTADO == obj.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == obj.ID_PROCESO_LIBERTAD && w.ID_MEDIDA_LIBERADO == obj.ID_MEDIDA_LIBERADO).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA);
                }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar medida", ex);
            }
        }
        #endregion

        #region Medida Estatus
        private void LimpiarMedidaEstatus() {
            MLEstatus = MLMotivo = -1;
            MLFecha = null;
            MLComentario = string.Empty;
        }

        private void PopulateMedidaEstatus()
        {
            try
            {
                if (SelectedMedidaLibertadEstatus != null)
                {
                    MLEstatus = SelectedMedidaLibertadEstatus.ID_ESTATUS;
                    MLMotivo = SelectedMedidaLibertadEstatus.ID_MOTIVO.HasValue ? SelectedMedidaLibertadEstatus.ID_MOTIVO.Value : (short)-1;
                    MLFecha = SelectedMedidaLibertadEstatus.FECHA;
                    MLComentario = SelectedMedidaLibertadEstatus.COMENTARIOS;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un estatus");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar estatus", ex);
            }
        }

        private void GuardarMedidaEstatus()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    return;
                }
                if(SelectedMedidaLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una medida");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. " + base.Error);
                    return;
                }
                var obj = new MEDIDA_LIBERTAD_ESTATUS();
                obj.ID_ESTATUS = MLEstatus;
                obj.ID_MOTIVO = MLMotivo;
                obj.FECHA = MLFecha;
                obj.COMENTARIOS = MLComentario;
                if (SelectedMedidaLibertadEstatus != null)
                {
                    obj.ID_CENTRO = SelectedMedidaLibertadEstatus.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLibertadEstatus.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertadEstatus.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertadEstatus.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERTAD = SelectedMedidaLibertadEstatus.ID_MEDIDA_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertadEstatus.ID_MEDIDA_LIBERADO;
                    obj.ID_ESTATUS = SelectedMedidaLibertadEstatus.ID_ESTATUS;
                    obj.ID_CONSEC = SelectedMedidaLibertadEstatus.ID_CONSEC;
                    if (new cMedidaLibertadEstatus().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_ESTAUS);
                    }
                }
                else
                {
                    obj.ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERTAD = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
                    obj.ID_CONSEC = new cMedidaLibertadEstatus().Insertar(obj);
                    if (obj.ID_CONSEC > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        //SelectedMedidaLibertadEstatus = new cMedidaLibertadEstatus().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD, obj.ID_MEDIDA_LIBERADO, obj.ID_ESTATUS);
                        var tmp = SelectedMedidaLibertad;
                        LstMedidaLibertad = new ObservableCollection<MEDIDA_LIBERTAD>(new cMedidaLibertad().ObtenerTodos(
                            SelectedProcesoLibertad.ID_CENTRO,
                            SelectedProcesoLibertad.ID_ANIO,
                            SelectedProcesoLibertad.ID_IMPUTADO,
                            SelectedProcesoLibertad.ID_PROCESO_LIBERTAD));
                        if (LstMedidaLibertad != null && tmp != null)
                            SelectedMedidaLibertad = LstMedidaLibertad.Where(w =>
                                w.ID_CENTRO == tmp.ID_CENTRO &&
                                w.ID_ANIO == tmp.ID_ANIO &&
                                w.ID_IMPUTADO == tmp.ID_IMPUTADO &&
                                w.ID_PROCESO_LIBERTAD == tmp.ID_PROCESO_LIBERTAD &&
                                w.ID_MEDIDA_LIBERADO == tmp.ID_MEDIDA_LIBERADO).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_ESTAUS);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar estatus", ex);
            }
        }
        #endregion

        #region Medida Persona
        private void LimpiarPersona()
        {
            MPNombre = MPPaterno = MPMaterno = MPAlias = string.Empty;
            MPRelacion = MPParticularidad = -1;
        }

        private void PopulateMedidaPersona() 
        {
            try 
            {
                if (SelectedMedidaPersona != null)
                {
                    MPNombre = SelectedMedidaPersona.NOMBRE;
                    MPPaterno = SelectedMedidaPersona.PATERNO;
                    MPMaterno = SelectedMedidaPersona.MATERNO;
                    MPAlias = SelectedMedidaPersona.ALIAS;
                    MPRelacion = SelectedMedidaPersona.ID_TIPO_REFERENCIA.HasValue ? SelectedMedidaPersona.ID_TIPO_REFERENCIA.Value : (short)-1;
                    MPParticularidad = SelectedMedidaPersona.ID_PARTICULARIDAD.HasValue ? SelectedMedidaPersona.ID_PARTICULARIDAD.Value : (short)-1;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una persona");
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar persona", ex);
            }
        }
        private void GuardarMedidaPersona()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    return;
                }
                if (SelectedMedidaLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una medida");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. " + base.Error);
                    return;
                }
                var obj = new MEDIDA_PERSONA();
                obj.NOMBRE = MPNombre;
                obj.PATERNO = MPPaterno;
                obj.MATERNO = MPPaterno;
                obj.ALIAS = MPAlias;
                obj.ID_TIPO_REFERENCIA = MPRelacion;
                obj.ID_PARTICULARIDAD = obj.ID_PARTICUARIDAD = MPParticularidad;
                
                if (SelectedMedidaPersona != null)
                {
                    obj.ID_CENTRO = SelectedMedidaPersona.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaPersona.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaPersona.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaPersona.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaPersona.ID_MEDIDA_LIBERADO;
                    obj.ID_PERSONA = SelectedMedidaPersona.ID_PERSONA;
                    if (new cMedidaPersona().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PERSONA);
                    }
                }
                else
                {
                    obj.ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
                    obj.ID_PERSONA = new cMedidaPersona().Insertar(obj);
                    if (obj.ID_PERSONA > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        SelectedMedidaPersona = new cMedidaPersona().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD, obj.ID_MEDIDA_LIBERADO, obj.ID_PERSONA);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PERSONA);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar persona", ex);
            }
        }
        #endregion

        #region Medida Lugar
        private void LimpiarMedidaLugar() 
        {
            MLPertenece = MLCalle = MLNoInterior = MLTelefono = MLColonia = string.Empty;
            MLNoExterior = null;
            MLGiro = -1;
            MLEntidad = MLMunicipio = -1;
        }

        private void PopulateMedidaLugar() {
            try 
            {
                if (SelectedMedidaLugar != null)
                {
                        MLPertenece = SelectedMedidaLugar.PERTENECE;
                        MLCalle= SelectedMedidaLugar.CALLE;
                        MLNoExterior = SelectedMedidaLugar.NO_EXTERNO;
                        MLNoInterior = SelectedMedidaLugar.NO_INTERNO;
                        MLTelefono = SelectedMedidaLugar.TELEFONO.HasValue ?  SelectedMedidaLugar.TELEFONO.ToString() : string.Empty;
                        MLEntidad = SelectedMedidaLugar.ID_ENTIDAD;
                        MLMunicipio = SelectedMedidaLugar.ID_MUNICIPIO;
                        MLColonia = SelectedMedidaLugar.COLONIA;
                        MLGiro = SelectedMedidaLugar.ID_GIRO;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un lugar");
            }
            catch(Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar medida", ex);
            }
        }

        private void GuardarMedidaLugar()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    return;
                }
                if (SelectedMedidaLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una medida");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. " + base.Error);
                    return;
                }
                var obj = new MEDIDA_LUGAR();
                obj.PERTENECE = MLPertenece;
                obj.CALLE = MLCalle;
                obj.NO_EXTERNO = MLNoExterior;
                obj.NO_INTERNO = MLNoInterior;
                obj.TELEFONO = !string.IsNullOrEmpty(MLTelefono) ? (int?)long.Parse(MLTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")) : null;
                obj.PAIS = SelectedEntidadML.ID_PAIS_NAC;
                obj.ID_ENTIDAD = MLEntidad;
                obj.ID_MUNICIPIO = MLMunicipio;
                obj.COLONIA = MLColonia;
                obj.ID_GIRO = MLGiro;
                if (SelectedMedidaLugar != null)
                {
                    obj.ID_CENTRO = SelectedMedidaLugar.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLugar.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLugar.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLugar.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLugar.ID_MEDIDA_LIBERADO;
                    obj.ID_MEDIDA_LUGAR = SelectedMedidaLugar.ID_MEDIDA_LUGAR;
                    if (new cMedidaLugar().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_LUGAR);
                    }
                }
                else
                {
                obj.ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
                    obj.ID_MEDIDA_LUGAR = new cMedidaLugar().Insertar(obj);
                    if (obj.ID_MEDIDA_LUGAR > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        SelectedMedidaLugar = new cMedidaLugar().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD, obj.ID_MEDIDA_LIBERADO, obj.ID_MEDIDA_LUGAR);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_LUGAR);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar lugar", ex);
            }
        }
        #endregion

        #region Medida Presentacion
        private void LimpiarMedidaPresentacion() 
        {
            MPRLugar = MPRAsesor = -1;
            MPRObservacion = string.Empty;
            MPRTipo = true;
            LstFechasExcepcion = LstFechasProgramar = null;
            MPRDia = 1;
            MPRCada = 1;
            MPRPrimeraAsistencia = MPRUltimaAsistencia = MPRFechaAgrerar = null;
            LstFechasProgramar = null;
            LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>();
        }

        private void PopulateMedidaPresentacion() 
        {
            try
            {
                if (SelectedMedidaPresentacion != null)
                {
                    MPRLugar = SelectedMedidaPresentacion.ID_LUGCUM.Value;
                    MPRAsesor = SelectedMedidaPresentacion.ID_EMPLEADO.Value;
                    MPRObservacion = SelectedMedidaPresentacion.OBSERVACIONES;
                    MPRDia = SelectedMedidaPresentacion.DIAS;
                    MPRCada = SelectedMedidaPresentacion.CADA;
                    MPRPrimeraAsistencia = SelectedMedidaPresentacion.PRIMERA_ASISTENCIA;
                    MPRUltimaAsistencia = SelectedMedidaPresentacion.ULTIMA_ASISTENCIA;
                    
                    #region Detalle
                    if (SelectedMedidaPresentacion.MEDIDA_PRESENTACION_DETALLE != null)
                        LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(SelectedMedidaPresentacion.MEDIDA_PRESENTACION_DETALLE);
                    else
                        LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>();
                    #endregion
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Facor de seleccionar una presentación");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar presentación", ex);
            }
        }

        private void GuardarMedidaPresentacion()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    return;
                }
                if (SelectedMedidaLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una medida");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. " + base.Error);
                    return;
                }
                if (LstMedidaPresentacionDetalle.Count == 0)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar fechas. " + base.Error);
                    return;
                }
                var obj = new MEDIDA_PRESENTACION();
                obj.ID_LUGCUM = MPRLugar;
                obj.ID_EMPLEADO = MPRAsesor;
                obj.OBSERVACIONES = MPRObservacion;
                obj.DIAS = MPRDia != -1 ? MPRDia : null;
                obj.CADA = MPRCada != -1 ? MPRCada : null;
                obj.PRIMERA_ASISTENCIA = MPRPrimeraAsistencia;
                obj.ULTIMA_ASISTENCIA = MPRUltimaAsistencia;

                //Detalle
                var detalle = new List<MEDIDA_PRESENTACION_DETALLE>();

                if (SelectedMedidaPresentacion != null)
                {
                    obj.ID_CENTRO = SelectedMedidaPresentacion.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaPresentacion.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaPresentacion.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaPresentacion.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaPresentacion.ID_MEDIDA_LIBERADO;
                    //detalle
                    short i = 1;
                    foreach (var d in LstMedidaPresentacionDetalle)
                    {
                        
                        detalle.Add(new MEDIDA_PRESENTACION_DETALLE()
                        {
                            ID_CENTRO = SelectedMedidaPresentacion.ID_CENTRO,
                            ID_ANIO = SelectedMedidaPresentacion.ID_ANIO,
                            ID_IMPUTADO = SelectedMedidaPresentacion.ID_IMPUTADO,
                            ID_PROCESO_LIBERTAD = SelectedMedidaPresentacion.ID_PROCESO_LIBERTAD,
                            ID_MEDIDA_LIBERADO = SelectedMedidaPresentacion.ID_MEDIDA_LIBERADO,
                            ID_MEDIDA_LUGAR = (short)MPRLugar,
                            ID_DETALLE = i,
                            FECHA = d.FECHA,
                            ASISTENCIA = d.ASISTENCIA,
                            FECHA_ASISTENCIA = d.FECHA_ASISTENCIA,
                            EXCEPTO = "N"
                        });
                        i++;
                    }
                    if (new cMedidaPresentacion().Actualizar(obj,detalle))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_PRESENTACION);
                    }
                }
                else
                {
                    obj.ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO;
          
                    //detalle
                    short i = 1;
                    foreach (var d in LstMedidaPresentacionDetalle)
                    {
                        detalle.Add(new MEDIDA_PRESENTACION_DETALLE()
                        {
                            ID_CENTRO = SelectedMedidaLibertad.ID_CENTRO,
                            ID_ANIO = SelectedMedidaLibertad.ID_ANIO,
                            ID_IMPUTADO = SelectedMedidaLibertad.ID_IMPUTADO,
                            ID_PROCESO_LIBERTAD = SelectedMedidaLibertad.ID_PROCESO_LIBERTAD,
                            ID_MEDIDA_LIBERADO = SelectedMedidaLibertad.ID_MEDIDA_LIBERADO,
                            ID_MEDIDA_LUGAR = (short)MPRLugar,
                            ID_DETALLE = i,
                            FECHA = d.FECHA,
                            EXCEPTO = "N"
                        });
                        i++;
                    }

                    obj.MEDIDA_PRESENTACION_DETALLE = detalle;
                    if (new cMedidaPresentacion().Insertar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        SelectedMedidaPresentacion = new cMedidaPresentacion().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD, obj.ID_MEDIDA_LIBERADO);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_PRESENTACION);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar lugar", ex);
            }
        }

        private void AgregarFechaIndividual()
        {
            try
            {
                if (LstMedidaPresentacionDetalle == null)
                    LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>();
                if (MPRFechaAgrerar.HasValue)
                {
                    if (!LstMedidaPresentacionDetalle.Where(w => w.FECHA == MPRFechaAgrerar.Value.Date).Any())
                    {
                        LstMedidaPresentacionDetalle.Add(new MEDIDA_PRESENTACION_DETALLE() { FECHA = MPRFechaAgrerar.Value.Date });
                        MPRFechaAgrerar = null;
                        LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(LstMedidaPresentacionDetalle);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","La fecha ya fue registrada");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar una fecha");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar fecha", ex);
            }
        }

        private void EliminarFechaPresentacion()
        {
            try
            {
                if (LstMedidaPresentacionDetalle != null)
                {
                    if (SelectedMedidaPresentacionDetalle != null)
                    {
                        if (SelectedMedidaPresentacion != null)//editar
                        {
                            if (SelectedMedidaPresentacionDetalle.FECHA.Value.Date < HOY || SelectedMedidaPresentacionDetalle.ASISTENCIA == "S")
                                new Dialogos().ConfirmacionDialogo("Validación", "No puede eliminar fechas anteriores al dia actual o fechas que ya tienen una asistencia registrada");
                            else
                            {
                                LstMedidaPresentacionDetalle.Remove(SelectedMedidaPresentacionDetalle);
                                LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(LstMedidaPresentacionDetalle);
                            }
                        }
                        else//insertar
                        {
                            LstMedidaPresentacionDetalle.Remove(SelectedMedidaPresentacionDetalle);
                            LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(LstMedidaPresentacionDetalle);
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una fecha a eliminar");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar fecha", ex);
            }
        }

        private void ProgramarPresentacion()
        {
            try
            {
                if (MPRDia == -1)
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar dia.");
                if(MPRCada == -1)
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar frecuencia.");
                if(!MPRPrimeraAsistencia.HasValue)
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar primera presentación.");
                if (!MPRUltimaAsistencia.HasValue)
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar ultima presentación.");
                if(MPRPrimeraAsistencia.Value.Date > MPRUltimaAsistencia.Value.Date)
                    new Dialogos().ConfirmacionDialogo("Validación", "La fecha de Primera asistencia debe ser menor a la fecha de ultima asistencia.");
                if (LstMedidaPresentacionDetalle == null)
                    LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>();
                var tmp = MPRPrimeraAsistencia.Value.Date;
                while (tmp <= MPRUltimaAsistencia.Value.Date)
                {
                    if ((short)tmp.DayOfWeek == MPRDia)
                    {
                        LstMedidaPresentacionDetalle.Add(new MEDIDA_PRESENTACION_DETALLE() { FECHA = tmp });
                        tmp = tmp.AddDays(MPRCada.Value * 7);
                        if (tmp >= MPRUltimaAsistencia.Value.Date)
                            break;
                    }
                    else
                        tmp = tmp.AddDays(1);
                }
                LstMedidaPresentacionDetalle = new ObservableCollection<MEDIDA_PRESENTACION_DETALLE>(LstMedidaPresentacionDetalle);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al programar presentación", ex);
            }
        }
        #endregion

        #region Medida Documento
        private void LimpiarDocumento() 
        {
            MDFecha = null;
            MDFolio = MDTitulo = MDSeleccion = string.Empty;
            MDAutor = -1;
            MDFuente = -1;
            MDEntrada = true;
            LstTipoDocumentoMedidaFiltro = new ObservableCollection<TIPO_DOCUMENTO_MEDIDA>(LstTipoDocumentoMedida.Where(w => w.SENTIDO == "E"));
            LstTipoDocumentoMedidaFiltro.Insert(0, new TIPO_DOCUMENTO_MEDIDA() { ID_TIPDOCMED = -1, DESCR = "SELECCIONE" });
            MDTipoDocumento = -1;
            MDSalida = false;
            MDSeleccion = string.Empty;
            //LstAsesor = new ObservableCollection<Asesor>(LstAsesor);
        }

        private void PopulateMedidaDocumento() {
            try
            {
                if (SelectedMedidaDocumento != null)
                {
                    MDFecha = SelectedMedidaDocumento.FECHA;
                    MDDocumento = SelectedMedidaDocumento.DOCUMENTO;
                    MDFolio = SelectedMedidaDocumento.OFICIO;
                    MDAutor = SelectedMedidaDocumento.ID_EMPLEADO;
                    MDTitulo = SelectedMedidaDocumento.TITULO;
                    MDEntrada = SelectedMedidaDocumento.SENTIDO == "E" ? true : false;
                    MDFuente = SelectedMedidaDocumento.ID_FUENTE;
                    MDTipoDocumento = SelectedMedidaDocumento.ID_TIPDOCMED;
                
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un documento a editar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar ddocumento", ex);
            }
        }
        
        private void GuardarMedidaDocumento()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un proceso");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar campos obligatorios. " + base.Error);
                    return;
                }
                var obj = new MEDIDA_DOCUMENTO();
                obj.FECHA = MDFecha;
                obj.DOCUMENTO = MDDocumento;
                obj.VISTO = "N";
                obj.OFICIO = MDFolio;
                obj.ID_EMPLEADO = MDAutor;
                obj.TITULO = MDTitulo;
                obj.SENTIDO = MDEntrada ? "E" : "S";
                obj.ID_FUENTE = MDFuente;
                obj.ID_TIPDOCMED = MDTipoDocumento;
                if (SelectedMedidaDocumento != null)
                {
                    obj.ID_CENTRO = SelectedMedidaDocumento.ID_CENTRO;
                    obj.ID_ANIO = SelectedMedidaDocumento.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedMedidaDocumento.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedMedidaDocumento.ID_PROCESO_LIBERTAD;
                    obj.ID_DOCUMENTO = SelectedMedidaDocumento.ID_DOCUMENTO;
                    if (new cMedidaDocumento().Actualizar(obj))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(new cMedidaDocumento().ObtenerTodos(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD));
                        LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                        LstMedidaDocumentoCB.Insert(0, new MEDIDA_DOCUMENTO() { ID_DOCUMENTO = -1, TITULO = "SELECCIONE" });
                        SelectedMedidaDocumento = LstMedidaDocumento.Where(w => w.ID_CENTRO == obj.ID_CENTRO && w.ID_ANIO == obj.ID_ANIO && w.ID_IMPUTADO == obj.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == obj.ID_PROCESO_LIBERTAD && w.ID_DOCUMENTO == obj.ID_DOCUMENTO).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO);
                    }
                }
                else
                {
                    obj.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                    obj.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                    obj.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                    obj.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                    obj.ID_DOCUMENTO = new cMedidaDocumento().Insertar(obj);
                    if (obj.ID_DOCUMENTO > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se ha guardado correctamente");
                        LstMedidaDocumento = new ObservableCollection<MEDIDA_DOCUMENTO>(new cMedidaDocumento().ObtenerTodos(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_PROCESO_LIBERTAD));
                        LstMedidaDocumentoCB = new ObservableCollection<MEDIDA_DOCUMENTO>(LstMedidaDocumento);
                        LstMedidaDocumentoCB.Insert(0, new MEDIDA_DOCUMENTO() { ID_DOCUMENTO = -1, TITULO = "SELECCIONE" });
                        SelectedMedidaDocumento = LstMedidaDocumento.Where(w => w.ID_CENTRO == obj.ID_CENTRO && w.ID_ANIO == obj.ID_ANIO && w.ID_IMPUTADO == obj.ID_IMPUTADO && w.ID_PROCESO_LIBERTAD == obj.ID_PROCESO_LIBERTAD && w.ID_DOCUMENTO == obj.ID_DOCUMENTO).FirstOrDefault();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_MEDIDA_DOCUMENTO);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar medida", ex);
            }
        }

        private void ElegirDocumentoGuardar()
        {
            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione una imagen";
            op.Filter = "Pdf Files|*.pdf";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (new System.IO.FileInfo(op.FileName).Length > 5000000)
                    StaticSourcesViewModel.Mensaje("Documento no soportado", "El archivo debe ser de menos de 5 Mb", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                else
                {
                    MDDocumento = System.IO.File.ReadAllBytes(op.FileName);
                    MDSeleccion = op.SafeFileName;
                }
            }
        }

        private void VerDocumento()
        {
            try
            {
                if (SelectedMedidaDocumento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validadción", "Favor de seleccionar un documento");
                    return;
                }
                var tc = new TextControlView();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.editor.Loaded += (s, e) =>
                {
                    try
                    {
                        tc.editor.Load(SelectedMedidaDocumento.DOCUMENTO, TXTextControl.BinaryStreamType.AdobePDF);
                        #region comentado
                        //switch (documento.ID_FORMATO)
                        //{
                        //    case (int)enumFormatoDocumento.DOCX:
                        //        tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
                        //        break;
                        //    case (int)enumFormatoDocumento.PDF:
                        //tc.editor.Load(SelectedMedidaDocumento.DOCUMENTO, TXTextControl.BinaryStreamType.AdobePDF);
                            //    break;
                            //case (int)enumFormatoDocumento.DOC:
                            //    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                            //    break;
                            ////default:
                            //    new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                            //    break;
                        //}
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
            }
        }
        #endregion

        #region Buscar Huella
        private void LimpiarCampos()
        {
            Application.Current.Dispatcher.Invoke((System.Action)(delegate
            {
                ScannerMessage = "Capture Huella";
                ColorMessage = new SolidColorBrush(Colors.Green);
                AceptarBusquedaHuellaFocus = true;
            }));
            _SelectRegistro = null;
            PropertyImage = null;
        }

        private async void OnLoadMedida(BuscarPorHuellaYNipMedidaView Window)
        {
            try
            {
                BuscarPor = enumTipoPersona.IMPUTADO;
                ListResultado = null;
                PropertyImage = null;
                FotoRegistro = new Imagenes().getImagenPerson();
                TextNipBusqueda = string.Empty;

                #region [Huellas Digitales]
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0;
                myDoubleAnimation.To = 185;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.3));
                myDoubleAnimation.AutoReverse = true;
                myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Storyboard.SetTargetName(myDoubleAnimation, "Ln");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
                var myStoryboard = new Storyboard();
                myStoryboard.Children.Add(myDoubleAnimation);
                myStoryboard.Begin(Window.Ln);
                #endregion

                Window.Closed += (s, e) =>
                {
                    try
                    {
                        if (OnProgress == null)
                            return;

                        if (!_IsSucceed)
                            SelectRegistro = null;

                        OnProgress.Abort();
                        CancelCaptureAndCloseReader(OnCaptured);
                        //if (AtencionSeleccionada)
                        //{
                        //    var aux = SelectAtencionMedicaAux;
                        //    SelectAtencionMedica = aux;
                        //}
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar busqueda", ex);
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

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Capture Huella";
                    ColorMessage = new SolidColorBrush(Colors.Green);
                }));
                GuardandoHuellas = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de la busqueda por huella.", ex);
            }
        }

        private async void Aceptar(Window Window)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando..."))
                    return;
                CancelKeepSearching = true;
                isKeepSearching = true;
                await WaitForFingerPrints();
                _IsSucceed = true;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        if (SelectRegistro == null) return;
                        SelectExpediente = SelectRegistro.Imputado;
                        if (SelectExpediente.INGRESO.Count == 0)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                            }));
                            return;
                        }
                        if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                            SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            }));
                            return;
                        }
                        SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        //NotaMedicaVisible = Visibility.Visible;
                        //ListaImputadosVisible = Visibility.Collapsed;
                        //SeleccionarImputado();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        public async override void OnCaptured(DPUruNet.CaptureResult captureResult)
        {
            try
            {
                if (ScannerMessage.Contains("Procesando..."))
                    return;

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    TextNipBusqueda = string.Empty;
                    PropertyImage = new BitmapImage();
                    ShowLoading = Visibility.Visible;
                    //ShowLine = Visibility.Visible;
                    ShowCapturar = Visibility.Collapsed;
                    ShowLine = Visibility.Visible;
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                }));

                //await TaskEx.Delay(500);


                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    base.OnCaptured(captureResult);
                }));
                ListResultado = null;

                switch (BuscarPor)
                {
                    case enumTipoPersona.IMPUTADO:
                        await CompareImputado();
                        break;
                    case enumTipoPersona.PERSONA_TODOS:
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_ABOGADO:
                    case enumTipoPersona.PERSONA_EMPLEADO:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        //ComparePersona();
                        break;
                    default:
                        break;
                }

                GuardandoHuellas = true;
                ShowLoading = Visibility.Collapsed;
                ShowCapturar = Conectado ? Visibility.Visible : Visibility.Collapsed;
                ShowLine = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
        }

        private async void Capture(string obj)
        {
            try
            {
                ShowLoading = Visibility.Visible;
                ShowLine = Visibility.Visible;
                var nRet = -1;
                try
                {
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                    ShowCapturar = Visibility.Collapsed;

                    #region [Huellas]
                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowLine = Visibility.Visible;
                        ListResultado = null;
                        HuellasCapturadas = new List<PlantillaBiometrico>();

                        for (short i = 1; i <= 10; i++)
                        {
                            var pBuffer = IntPtr.Zero;
                            var nBufferLength = 0;
                            var nNFIQ = 0;
                            ListResultado = null;
                            GuardandoHuellas = false;

                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                            var bufferBMP = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero)
                                Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);

                            CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                            var bufferWSQ = new byte[nBufferLength];
                            if (pBuffer != IntPtr.Zero)
                                Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);

                            CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);

                            Fmd FMD = null;
                            if (bufferBMP.Length != 0)
                            {
                                PropertyImage = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                            }

                            ShowContinuar = Visibility.Collapsed;
                            await TaskEx.Delay(1);

                            switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                            {
                                #region [Pulgar Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_DERECHO);
                                    break;
                                #endregion
                                #region [Medio Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_DERECHO);
                                    break;
                                #endregion
                                #region [Anular Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_DERECHO);
                                    break;
                                #endregion
                                #region [Meñique Derecho]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_DERECHO);
                                    break;
                                #endregion
                                #region [Pulgar Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.PULGAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, (short)enumTipoBiometrico.PULGAR_DERECHO);
                                    break;
                                #endregion
                                #region [Indice Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.INDICE_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Medio Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MEDIO_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Anular Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.ANULAR_IZQUIERDO);
                                    break;
                                #endregion
                                #region [Meñique Izquierdo]
                                case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                    HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                    isKeepSearching = BuscarPor == enumTipoPersona.IMPUTADO ? await CompareImputado(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO) : await ComparePersona(FMD != null ? FMD.Bytes : null, enumTipoBiometrico.MENIQUE_IZQUIERDO);
                                    isKeepSearching = true;
                                    break;
                                #endregion
                                default:
                                    break;
                            }

                            ShowContinuar = Visibility.Visible;
                            ShowCapturar = Visibility.Collapsed;

                            if (!CancelKeepSearching)
                                await KeepSearch();
                            else
                                if (!_GuardarHuellas)
                                    break;
                        }

                        GuardandoHuellas = true;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            ScannerMessage = "Vuelve a capturar las huellas";
                            ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        }));
                    }
                    #endregion
                }
                catch
                {
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                }

                if (nRet == 0)
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Busqueda Terminada";
                        ColorMessage = new SolidColorBrush(Colors.Green);
                        AceptarBusquedaHuellaFocus = true;
                    }));

                ShowLine = Visibility.Collapsed;
                ShowLoading = Visibility.Collapsed;
                ShowContinuar = Visibility.Collapsed;
                await TaskEx.Delay(1500);
                ShowCapturar = Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }

        }

        private async Task WaitForFingerPrints()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!GuardandoHuellas) ;
            });
        }

        private Task<bool> CompareImputado(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);
                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = Finger == null ? "Vuelve a capturar las huellas" : ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));
                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                var list = SelectExpediente.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)verifyFinger && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 && w.BIOMETRICO != null).AsEnumerable()
                    .Select(s => new
                    {
                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data
                    }).ToList();
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, list.Where(w => w.FMD != null).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                var identify = true;
                identify = doIdentify.ResultCode == Constants.ResultCode.DP_SUCCESS ? doIdentify.Indexes.Count() > 0 : false;
                if (identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>(); var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                    if (SelectExpediente.IMPUTADO_BIOMETRICO != null)
                    { 
                        var foto = SelectExpediente.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        if(foto != null)
                        {
                            if (foto.BIOMETRICO != null)
                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(foto.BIOMETRICO);
                        }
                    }
                    
                    ListResultado.Add(new ResultadoBusquedaBiometrico
                    {
                        AMaterno = SelectExpediente.MATERNO,
                        APaterno = SelectExpediente.PATERNO,
                        Nombre = SelectExpediente.PATERNO,
                        Expediente = SelectExpediente.ID_ANIO + " / " + SelectExpediente.ID_IMPUTADO,
                        Foto = FotoBusquedaHuella,
                        Imputado = SelectExpediente,
                        NIP = SelectExpediente.NIP
                    });
                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);
                    SelectRegistro = ListResultado.FirstOrDefault();
                    ShowContinuar = Visibility.Collapsed;
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella empatada";
                            AceptarBusquedaHuellaFocus = true;
                            ColorMessage = new SolidColorBrush(Colors.Green);
                        }
                    }));
                    if (Finger != null)
                        Service.Close();
                    return TaskEx.FromResult(false);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no concuerda";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching)
                        _SelectRegistro = null;
                    PropertyImage = null;
                }
                Service.Close();
                FingerPrintData = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private Task<bool> ComparePersona(byte[] Huella = null, enumTipoBiometrico? Finger = null)
        {
            try
            {
                var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null ?? Huella;
                var verifyFinger = Finger ?? (DD_Dedo.HasValue ? DD_Dedo.Value : enumTipoBiometrico.INDICE_DERECHO);

                if (bytesHuella == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (Finger == null)
                            ScannerMessage = "Vuelve a capturar las huellas";
                        else
                            ScannerMessage = "Siguiente Huella";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.DarkOrange);
                        ShowLine = Visibility.Collapsed;
                    }));
                }
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    ScannerMessage = "Procesando...";
                    ColorMessage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 115, 242));
                    AceptarBusquedaHuellaFocus = false;
                }));

                var Service = new BiometricoServiceClient();
                if (Service == null)
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ScannerMessage = "Error en el servicio de comparacion";
                        AceptarBusquedaHuellaFocus = true;
                        ColorMessage = new SolidColorBrush(Colors.Red);
                        ShowLine = Visibility.Collapsed;
                    }));
                }

                var CompareResult = Service.CompararHuellaPersona(new ComparationRequest
                {
                    BIOMETRICO = bytesHuella,
                    ID_TIPO_BIOMETRICO = verifyFinger,
                    ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP,
                    ID_TIPO_PERSONA = BuscarPor == enumTipoPersona.PERSONA_TODOS ? new Nullable<enumTipoPersona>() : BuscarPor
                });

                if (CompareResult.Identify)
                {
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                    foreach (var item in CompareResult.Result)
                    {
                        var persona = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == item.ID_PERSONA && (w.ID_TIPO_BIOMETRICO == (DD_Dedo.HasValue ? (short)DD_Dedo.Value : (short)enumTipoBiometrico.INDICE_DERECHO)) && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).FirstOrDefault();

                        ShowContinuar = Visibility.Collapsed;
                        if (persona == null)
                            continue;

                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            var perosonabiometrico = new cPersonaBiometrico().GetData().Where(w => w.ID_PERSONA == persona.ID_PERSONA).ToList();
                            var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());

                            if (perosonabiometrico != null)
                                if (perosonabiometrico.Any())
                                    if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                        FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                    else
                                        if (perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(perosonabiometrico.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);

                            ListResultado.Add(new ResultadoBusquedaBiometrico()
                            {
                                Nombre = persona.PERSONA.NOMBRE,
                                APaterno = persona.PERSONA.PATERNO,
                                AMaterno = persona.PERSONA.MATERNO,
                                Expediente = persona.PERSONA.ID_PERSONA.ToString(),
                                NIP = persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                                    persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.HasValue
                                        ? persona.PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP.Value.ToString()
                                            : string.Empty : string.Empty,
                                Foto = FotoBusquedaHuella,
                                Persona = persona.PERSONA
                            });

                        }));
                    }

                    ListResultado = new List<ResultadoBusquedaBiometrico>(ListResultado);

                    ShowContinuar = Visibility.Collapsed;

                    if (ListResultado.Any())
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Registro encontrado";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Green);
                            }
                        }));

                        if (Finger != null)
                            Service.Close();

                        return TaskEx.FromResult(false);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            if (!CancelKeepSearching)
                            {
                                ScannerMessage = "Registro no encontrado";
                                AceptarBusquedaHuellaFocus = true;
                                ColorMessage = new SolidColorBrush(Colors.Red);
                            }
                        }));
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        if (!CancelKeepSearching)
                        {
                            ScannerMessage = "Huella no encontrada";
                            ColorMessage = new SolidColorBrush(Colors.Red);
                            AceptarBusquedaHuellaFocus = true;
                        }
                    }));
                    _IsSucceed = false;
                    if (!CancelKeepSearching)
                    {
                        _SelectRegistro = null;
                    }
                    PropertyImage = null;
                }

                Service.Close();
                FingerPrintData = null;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda por huella.", ex);
            }
            return TaskEx.FromResult(true);
        }

        private async Task KeepSearch()
        {
            await Task.Factory.StartNew(() =>
            {
                while (!isKeepSearching) ;
            });
            isKeepSearching = false;
        }

        private void ConstructorHuella(enumTipoPersona tipobusqueda, bool? set442 = null, bool GuardarHuellas = false)
        {
            try
            {
                BuscarPor = tipobusqueda;
                Conectado = set442.HasValue ? set442.Value : false;
                ShowCapturar = set442.HasValue ? set442.Value ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
                _GuardarHuellas = GuardarHuellas;
                switch (tipobusqueda)
                {
                    case enumTipoPersona.IMPUTADO:
                        CabeceraBusqueda = "Datos del Imputado";
                        CabeceraFoto = "Foto Imputado";
                        break;
                    case enumTipoPersona.PERSONA_VISITA:
                    case enumTipoPersona.PERSONA_EXTERNA:
                        CabeceraBusqueda = "Datos de la Persona";
                        CabeceraFoto = "Foto Persona";
                        break;
                    case enumTipoPersona.PERSONA_ABOGADO:
                        CabeceraBusqueda = "Datos del Abogado";
                        CabeceraFoto = "Foto Abogado";
                        break;
                    case enumTipoPersona.PERSONA_EMPLEADO:
                        CabeceraBusqueda = "Datos del Empleado";
                        CabeceraFoto = "Foto Empleado";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la busqueda por huella.", ex);
            }
        }

        //private async void OnBuscarPorHuella(string obj = "")
        //{
        //    try
        //    {
        //        //if (!PConsultar)
        //        //{
        //        //    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
        //        //    return;
        //        //}

        //        await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
        //        await TaskEx.Delay(400);
        //        var nRet = -1;
        //        var bandera = true;
        //        var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
        //        if (requiereGuardarHuellas)
        //            try
        //            {
        //                nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
        //            }
        //            catch
        //            {
        //                bandera = false;
        //            }
        //        else
        //            bandera = false;

        //        var windowBusqueda = new BusquedaHuella();
        //        windowBusqueda.DataContext = this;
        //        ConstructorHuella(enumTipoPersona.IMPUTADO, nRet == 0, requiereGuardarHuellas);
        //        windowBusqueda.dgHuella.Columns.Insert(windowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
        //        {
        //            Binding = new System.Windows.Data.Binding("Imputado")
        //            {
        //                Converter = new GetTipoPersona()
        //            },
        //            Header = "IMPUTADO"
        //        });
        //        if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(windowBusqueda.DataContext)).Readers.Count == 0 : false)
        //        {
        //            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
        //            StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
        //            return;
        //        }
        //        windowBusqueda.Owner = PopUpsViewModels.MainWindow;
        //        windowBusqueda.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Escape)windowBusqueda.Close(); };
        //        windowBusqueda.Closed += (s, e) =>
        //        {
        //            HuellasCapturadas = ((NotaMedicaViewModel)windowBusqueda.DataContext).HuellasCapturadas;
        //            if (bandera == true)
        //                CLSFPCaptureDllWrapper.CLS_Terminate();
        //            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //            //var huella = ((NotaMedicaViewModel)windowBusqueda.DataContext);
        //            if (!IsSucceed)
        //                return;
        //            if (SelectRegistro != null ? SelectRegistro.Imputado == null : null == null)
        //                return;
        //            //SelectPersona = huella.SelectRegistro.Persona;
        //        };
        //        windowBusqueda.ShowDialog();
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
        //    }
        //}

        private void HuellaClosed(object sender, EventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    //BuscarReadOnly = false;
                    HuellaWindow.Closed -= HuellaClosed;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.VER_MEDIDA_PRESENTACION);
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar la ventana de busqueda.", ex);
            }
        }

        #endregion

        #region Permisos
        //private void ConfiguraPermisos()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.REGISTRO_LIBERADOS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
        //        foreach (var p in permisos)
        //        {
        //            if (p.INSERTAR == 1)
        //                pInsertar = true;
        //            if (p.EDITAR == 1)
        //                pEditar = true;
        //            if (p.CONSULTAR == 1)
        //                pConsultar =  true;
        //            if (p.IMPRIMIR == 1)
        //                pImprimir = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        #endregion

        #region Seguimiento
        private void LimpiarSeguimiento()
        {
            SFecha = HOY;
            SObservacion = string.Empty;
        }

        private void PopulateSeguimientoListado() 
        {
            try
            {
                if (SelectedProcesoLibertad != null)
                {
                    LstSeguimiento = new ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO>(new cProcesoLibertadSeguimiento().ObtenerTodos(
                        SelectedProcesoLibertad.ID_CENTRO,
                        SelectedProcesoLibertad.ID_ANIO,
                        SelectedProcesoLibertad.ID_IMPUTADO,
                        SelectedProcesoLibertad.ID_PROCESO_LIBERTAD));
                }
                else
                    LstSeguimiento = new ObservableCollection<PROCESO_LIBERTAD_SEGUIMIENTO>();
                LstProcesoSeguimientoIsEmpty = LstSeguimiento.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener listado de seguimientos", ex);
            }
        }

        private void PopulateSegumientoDetalle() {
            try
            {
                if (SelectedSeguimiento != null)
                {
                    SFecha = SelectedSeguimiento.FECHA;
                    SObservacion = SelectedSeguimiento.OBSERVACION;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar el seguimiento a consultar");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener listado de seguimientos", ex);
            }
        }

        private void GuardarSeguimiento()
        {
            try
            {
                if (SelectedProcesoLibertad == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un proceso en libertad");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos: "+base.Error);
                    return;
                }
                var s = new PROCESO_LIBERTAD_SEGUIMIENTO();
                s.FECHA = SFecha;
                s.OBSERVACION = SObservacion;
                if (SelectedSeguimiento == null)//insert
                {
                    s.ID_CENTRO = SelectedProcesoLibertad.ID_CENTRO;
                    s.ID_ANIO = SelectedProcesoLibertad.ID_ANIO;
                    s.ID_IMPUTADO = SelectedProcesoLibertad.ID_IMPUTADO;
                    s.ID_PROCESO_LIBERTAD = SelectedProcesoLibertad.ID_PROCESO_LIBERTAD;
                    s.ID_CONSEC = new cProcesoLibertadSeguimiento().Insertar(s);
                    if (s.ID_CONSEC > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        PopulateSeguimientoListado();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp( PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                    }
                }
                else//update
                {
                    s.ID_CENTRO = SelectedSeguimiento.ID_CENTRO;
                    s.ID_ANIO = SelectedSeguimiento.ID_ANIO;
                    s.ID_IMPUTADO = SelectedSeguimiento.ID_IMPUTADO;
                    s.ID_PROCESO_LIBERTAD = SelectedSeguimiento.ID_PROCESO_LIBERTAD;
                    s.ID_CONSEC = SelectedSeguimiento.ID_CONSEC;
                    if (new cProcesoLibertadSeguimiento().Actualizar(s))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se edito correctamente");
                        PopulateSeguimientoListado();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el seguimiento", ex);
            }
        }

        private void EliminarSeguimiento()
        {
            try
            {
                if (SelectedSeguimiento != null)
                {
                    var s = new PROCESO_LIBERTAD_SEGUIMIENTO();
                    s.ID_CENTRO = SelectedSeguimiento.ID_CENTRO;
                    s.ID_ANIO = SelectedSeguimiento.ID_ANIO;
                    s.ID_IMPUTADO = SelectedSeguimiento.ID_IMPUTADO;
                    s.ID_PROCESO_LIBERTAD = SelectedSeguimiento.ID_PROCESO_LIBERTAD;
                    s.ID_CONSEC = SelectedSeguimiento.ID_CONSEC;

                    if (new cProcesoLibertadSeguimiento().Eliminar(s))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se elimino correctamente");
                        PopulateSeguimientoListado();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PROCESO_LIBERTAD_SEGUMIENTO);
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar el seguimiento a eliminar");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar el seguimiento", ex);
            }
        }
        #endregion

        #region Bitacora de Presentaciones
        private void GenerarBitacoraPresentacion()
        {
            if (SelectExpediente == null)
            {
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un imputado");
                return;
            }
            if (SelectedProcesoLibertad == null)
            {
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar un proceso en libertad");
                return;
            }
            if(SelectedMedidaLibertad == null)
            {
                new Dialogos().ConfirmacionDialogo("Validacion", "Favor de seleccionar una medida en libertad");
                return;
            }
            if(SelectedMedidaLibertad.MEDIDA_PRESENTACION == null)
            {
                new Dialogos().ConfirmacionDialogo("Validacion", "La media seleccionada no tiene presentaciones programadas ");
                return;
            }
            #region header
            var header = new List<cReporteDatos>();
            header.Add(new cReporteDatos()
            {
                 Encabezado1 = Parametro.ENCABEZADO1,
                 Encabezado2 = "DIRECCIÓN DE EJECUCIÓN DE PENAS Y MEDIDAS JUDICIALES",
                 Encabezado3 = "DEPARTAMENTO DE VIGILANCIA DE MEDIDAS Y BENEFICIONS JUDICIALES",
                 Titulo = "UNIDAD DE VIGILANCIA Y SEGUIMIENTO DE MEDIDAS JUDICIALES ZONA MEXICALI",
                 Logo1 = Parametro.REPORTE_LOGO1,
                 Logo2 = Parametro.REPORTE_LOGO2                  
            });
            #endregion

            #region Generales
            byte[] foto = new Imagenes().getImagenPerson();
            if (SelectExpediente.IMPUTADO_BIOMETRICO != null)
            {
                var b = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO);
                if (b != null)
                    foto = b.BIOMETRICO;
            }
            var l = new List<cBitacoraPresentaciones>();

            string estatus = string.Empty;
            if (SelectedMedidaLibertad.MEDIDA_LIBERTAD_ESTATUS != null)
            { 
                //estatus = SelectedMedidaLibertad.MEDIDA_LIBERTAD_ESTATUS
            }
            l.Add(new cBitacoraPresentaciones()
            {
                NOMBRE = string.Format("{0} {1} {2}",
                !string.IsNullOrEmpty(SelectExpediente.PATERNO) ? SelectExpediente.PATERNO.Trim() : string.Empty,
                !string.IsNullOrEmpty(SelectExpediente.MATERNO) ? SelectExpediente.MATERNO.Trim() : string.Empty,
                !string.IsNullOrEmpty(SelectExpediente.NOMBRE) ? SelectExpediente.NOMBRE.Trim() : string.Empty),
                ESTATUS = string.Empty,
                NUC = SelectedProcesoLibertad.NUC,
                CAUSA_PENAL = SelectedProcesoLibertad.CP_ANIO.HasValue && SelectedProcesoLibertad.CP_FOLIO.HasValue ? string.Format("{0}/{1}", SelectedProcesoLibertad.CP_ANIO, SelectedProcesoLibertad.CP_FOLIO) : string.Empty,
                DELITO = string.Empty,
                MEDIDA_JUDICIAL = SelectedMedidaLibertad.MEDIDA.DESCR.Trim(),
                PARTICULARIDAD = SelectedMedidaLibertad.OBSERVACION,
                FOTO = foto,
            });
            #endregion

            #region Bitacora
            var bitacora = SelectedMedidaLibertad.MEDIDA_PRESENTACION.MEDIDA_PRESENTACION_DETALLE.Where(w => w.FECHA.Value.Date <= HOY.Date).OrderBy(w => w.FECHA).Select(w => new cBitacoraPresentacionesDetalle() 
            { 
                FECHA_VENCIMIENTO = w.FECHA,
                FECHA_CUMPLIMIENTO = w.FECHA_ASISTENCIA,
                LUGAR_CUMPLIMIENTO = w.MEDIDA_PRESENTACION.LUGAR_CUMPLIMIENTO.DESCR
            });
            #endregion

            var View = new ReporteView();
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            View.Owner = PopUpsViewModels.MainWindow;
            View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

            View.ReporteViewer.LocalReport.ReportPath = "Reportes/rBitacoraPresentacion.rdlc";
            View.ReporteViewer.LocalReport.DataSources.Clear();
            
            ReportDataSource rds1 = new ReportDataSource();
            rds1.Name = "DataSet1";
            rds1.Value = header;
            View.ReporteViewer.LocalReport.DataSources.Add(rds1);

            ReportDataSource rds2 = new ReportDataSource();
            rds2.Name = "DataSet2";
            rds2.Value = l;
            View.ReporteViewer.LocalReport.DataSources.Add(rds2);

            ReportDataSource rds3 = new ReportDataSource();
            rds3.Name = "DataSet3";
            rds3.Value = bitacora;
            View.ReporteViewer.LocalReport.DataSources.Add(rds3);

            View.ReporteViewer.Refresh();
            View.ReporteViewer.RefreshReport();
            View.Show();
        }
        #endregion

        #region Consultar Informacion Anterior
        private void Populate()
        {
            try
            {
                if (SelectedProcesoLibertad != null)
                { 
                    var pl = new cProcesoLibertad().ObtenerProcesoLibertadAnterior(
                        SelectedProcesoLibertad.ID_CENTRO,
                        SelectedProcesoLibertad.ID_ANIO,
                        SelectedProcesoLibertad.ID_IMPUTADO,
                        SelectedProcesoLibertad.ID_PROCESO_LIBERTAD);
                    if (pl != null)
                    {
                        #region Datos Generales
                        AnioBuscar = SelectedProcesoLibertad.CP_ANIO;
                        FolioBuscar = SelectedProcesoLibertad.CP_FOLIO;
                        NombreBuscar = SelectExpediente.NOMBRE;
                        ApellidoPaternoBuscar = SelectExpediente.PATERNO;
                        ApellidoMaternoBuscar = SelectExpediente.MATERNO;

                        NUCBuscar = SelectedProcesoLibertad.NUC;
                        SelectSexo = !string.IsNullOrEmpty(SelectExpediente.SEXO) ? SelectExpediente.SEXO : "S";
                        SelectEstadoCivil = pl.ID_ESTADO_CIVIL != null ? pl.ID_ESTADO_CIVIL : -1;
                        SelectOcupacion = pl.ID_OCUPACION != null ? pl.ID_OCUPACION : -1;
                        SelectEscolaridad = pl.ID_ESCOLARIDAD != null ? pl.ID_ESCOLARIDAD : -1;
                        SelectReligion = pl.ID_RELIGION != null ? pl.ID_RELIGION : -1;
                        SelectEtnia = SelectExpediente.ID_ETNIA != null ? SelectExpediente.ID_ETNIA : -1;
                        SelectedIdioma = SelectExpediente.ID_IDIOMA != null ? SelectExpediente.ID_IDIOMA : Parametro.IDIOMA;
                        SelectedDialecto = SelectExpediente.ID_DIALECTO != null ? SelectExpediente.ID_DIALECTO : -1;
                        RequiereTraductor = SelectExpediente.TRADUCTOR == "S" ? true : false;
                        PTipo = SelectedProcesoLibertad.TIPO;
                        //PEscalaRiesgo = pl.ID_ESCALA_RIESGO;
                        #endregion

                        #region Domicilio
                        SelectPais = pl.ID_PAIS != null ? pl.ID_PAIS : Parametro.PAIS;
                        SelectEntidad = pl.ID_ENTIDAD != null ? pl.ID_ENTIDAD : -1;
                        SelectMunicipio = pl.ID_MUNICIPIO != null ? pl.ID_MUNICIPIO : -1;
                        SelectColonia = pl.ID_COLONIA;
                        TextCalle = pl.DOMICILIO_CALLE;
                        TextNumeroExterior = pl.DOMICILIO_NUM_EXT;
                        TextNumeroInterior = pl.DOMICILIO_NUM_INT;
                        TextTelefono = pl.TELEFONO != null ? pl.TELEFONO.Value.ToString() : null;
                        TextCodigoPostal = pl.DOMICILIO_CODIGO_POSTAL;
                        TextDomicilioTrabajo = pl.DOMICILIO_TRABAJO;
                        AniosEstado = pl.RESIDENCIA_ANIOS != null ? pl.RESIDENCIA_ANIOS.Value.ToString() : "0";
                        MesesEstado = pl.RESIDENCIA_MESES != null ? pl.RESIDENCIA_MESES.Value.ToString() : "0";
                        #endregion

                        #region Nacimiento
                        SelectPaisNacimiento = SelectExpediente.NACIMIENTO_PAIS != null ? SelectExpediente.NACIMIENTO_PAIS : Parametro.PAIS;
                        SelectEntidadNacimiento = SelectExpediente.NACIMIENTO_ESTADO != null ? SelectExpediente.NACIMIENTO_ESTADO : -1;
                        SelectMunicipioNacimiento = SelectExpediente.NACIMIENTO_MUNICIPIO != null ? SelectExpediente.NACIMIENTO_MUNICIPIO : -1;
                        TextFechaNacimiento = SelectExpediente.NACIMIENTO_FECHA;
                        TextLugarNacimientoExtranjero = SelectExpediente.NACIMIENTO_LUGAR;
                        #endregion

                        #region Padres
                        TextPadrePaterno = !string.IsNullOrEmpty(SelectExpediente.PATERNO_PADRE) ? SelectExpediente.PATERNO_PADRE.Trim() : string.Empty;
                        TextPadreMaterno = !string.IsNullOrEmpty(SelectExpediente.MATERNO_PADRE) ? SelectExpediente.MATERNO_PADRE.Trim() : string.Empty;
                        TextPadreNombre = !string.IsNullOrEmpty(SelectExpediente.NOMBRE_PADRE) ? SelectExpediente.NOMBRE_PADRE.Trim() : string.Empty;
                        CheckPadreFinado = pl.PADRE_FINADO == "S" ? true : false;
                        //CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S" ? true : false;

                        TextMadrePaterno = !string.IsNullOrEmpty(SelectExpediente.PATERNO_MADRE) ? SelectExpediente.PATERNO_MADRE.Trim() : string.Empty;
                        TextMadreMaterno = !string.IsNullOrEmpty(SelectExpediente.MATERNO_MADRE) ? SelectExpediente.MATERNO_MADRE.Trim() : string.Empty;
                        TextMadreNombre = !string.IsNullOrEmpty(SelectExpediente.NOMBRE_MADRE) ? SelectExpediente.NOMBRE_MADRE.Trim() : string.Empty;
                        CheckMadreFinado = pl.MADRE_FINADO == "S" ? true : false;
                        //CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S" ? true : false;

                        bool madre = false;
                        if (SelectExpediente.IMPUTADO_PADRES != null)
                        {
                            foreach (var p in SelectExpediente.IMPUTADO_PADRES)
                            {
                                if (p.ID_PADRE == "P")
                                {
                                    SelectPaisDomicilioPadre = p.ID_PAIS != null ? p.ID_PAIS : -1;
                                    SelectEntidadDomicilioPadre = p.ID_ENTIDAD != null ? p.ID_ENTIDAD : -1;
                                    SelectMunicipioDomicilioPadre = p.ID_MUNICIPIO != null ? p.ID_MUNICIPIO : -1;
                                    SelectColoniaDomicilioPadre = p.ID_COLONIA;
                                    TextCalleDomicilioPadre = p.CALLE;
                                    TextNumeroExteriorDomicilioPadre = p.NUM_EXT;
                                    TextNumeroInteriorDomicilioPadre = p.NUM_INT;
                                    TextCodigoPostalDomicilioPadre = p.CP;
                                }
                                else
                                    if (p.ID_PADRE == "M")
                                    {
                                        madre = true;
                                        SelectPaisDomicilioMadre = p.ID_PAIS != null ? p.ID_PAIS : -1;
                                        SelectEntidadDomicilioMadre = p.ID_ENTIDAD != null ? p.ID_ENTIDAD : -1;
                                        SelectMunicipioDomicilioMadre = p.ID_MUNICIPIO != null ? p.ID_MUNICIPIO : -1;
                                        SelectColoniaDomicilioMadre = p.ID_COLONIA;
                                        TextCalleDomicilioMadre = p.CALLE;
                                        TextNumeroExteriorDomicilioMadre = p.NUM_EXT;
                                        TextNumeroInteriorDomicilioMadre = p.NUM_INT;
                                        TextCodigoPostalDomicilioMadre = p.CP;
                                    }
                            }
                        }

                        if (!CheckMadreFinado && !madre)
                            MismoDomicilioMadre = true;
                        #endregion

                        #region Alias
                        if (SelectExpediente.ALIAS != null)
                        {
                            ListAlias = new ObservableCollection<ALIAS>(SelectExpediente.ALIAS);
                        }
                        #endregion

                        #region Apodos
                        if (SelectExpediente.APODO != null)
                        {
                            ListApodo = new ObservableCollection<APODO>(SelectExpediente.APODO);
                        }
                        #endregion

                        #region Actitd General
                        //ActitudGeneralEntrv = SelectedProcesoLibertad.ACTITUD_GENERAL;
                        #endregion

                        #region Observaciones
                        //TextObservaciones = SelectedProcesoLibertad.OBSERVACIONES;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Buscar Causa Penal
        private void LimpiarCausaPenal() 
        {
            Juzgado = Delito = string.Empty;
        }
        private void BuscarCausaPenal(short Tipo = 1) 
        {
            try
            {
                if (CPAnio == null || CPFolio == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar año y folio de la causa penal.");
                    return;
                }

                CausaPenal = new cCausaPenal().Obtener(
                    SelectExpediente.ID_CENTRO,
                    SelectExpediente.ID_ANIO,
                    SelectExpediente.ID_IMPUTADO,
                    null,
                    null,
                    CPAnio,
                    CPFolio).FirstOrDefault();

                if (CausaPenal != null)
                {
                    if (CausaPenal.JUZGADO != null)
                    {
                        Juzgado = CausaPenal.JUZGADO.DESCR;
                    }
                    if (CausaPenal.CAUSA_PENAL_DELITO != null)
                    {
                        foreach (var d in CausaPenal.CAUSA_PENAL_DELITO)
                        {
                            if (!string.IsNullOrEmpty(Delito))
                                Delito = Delito + ", ";
                            Delito = Delito + d.DESCR_DELITO;
                        }
                    }
                }
                else
                { 
                    if(Tipo == 1)
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la causa penal"); 
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion
        
        #region Imprimir Ficha
        private void ImprimirFicha() 
        {
            try
            {
                var logos = new List<cReporteProcesoLibertadLogos>();
                var generales = new List<cReporteProcesoLibertad>();
                var huellas_mano_izquierda = new List<cReporteProcesoLibertadHuellas>();
                var huellas_mano_derecha = new List<cReporteProcesoLibertadHuellas>();
                var medidas = new List<cReporteProcesoLibertadMedidas>();
                byte[] foto = new Imagenes().getImagenPerson();
                
                logos.Add(new cReporteProcesoLibertadLogos()
                {
                    LOGO1 = Parametro.REPORTE_LOGO1,
                    LOGO2 = Parametro.REPORTE_LOGO2,
                });

                if(SelectExpediente.IMPUTADO_BIOMETRICO != null)
                {
                    var biometrico = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO);
                    if(biometrico != null)
                    {
                        foto = biometrico.BIOMETRICO;
                    }

                    //Huellas mano izquierda
                    byte[] ip = null;
                    byte[] ii = null;
                    byte[] ia = null;
                    byte[] im = null;
                    byte[] ime = null;
                    var h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.PULGAR_IZQUIERDO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if(h != null)
                        ip = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.INDICE_IZQUIERDO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if(h != null)
                        ii = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.ANULAR_IZQUIERDO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if(h != null)
                        ia = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MEDIO_IZQUIERDO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if(h != null)
                        im = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MENIQUE_IZQUIERDO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if(h != null)
                        ime = getHuella(h.BIOMETRICO);
                    huellas_mano_izquierda.Add(new cReporteProcesoLibertadHuellas()
                    {
                        PULGAR = ip,
                        INDICE = ii,
                        ANULAR = ia,
                        MEDIO = im,
                        MENIQUE = ime
                    });

                    //Huellas mano Izquierda
                    byte[] dp = null;
                    byte[] di = null;
                    byte[] da = null;
                    byte[] dm = null;
                    byte[] dme = null;
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.PULGAR_DERECHO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if (h != null)
                        dp = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.INDICE_DERECHO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if (h != null)
                        di = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.ANULAR_DERECHO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if (h != null)
                        da = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MEDIO_DERECHO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if (h != null)
                        dm = getHuella(h.BIOMETRICO);
                    h = SelectExpediente.IMPUTADO_BIOMETRICO.FirstOrDefault(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.MENIQUE_DERECHO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ);
                    if (h != null)
                        dme = getHuella(h.BIOMETRICO);
                    huellas_mano_derecha.Add(new cReporteProcesoLibertadHuellas()
                    {
                        PULGAR = dp,
                        INDICE = di,
                        ANULAR = da,
                        MEDIO = dm,
                        MENIQUE = dme
                    });
                }

                string complexion = "";
                string color_piel = "";
                string cabello_color = "";
                string cabello_forma = "";
                if(selectExpediente.IMPUTADO_FILIACION != null)
                {
                    var f =  selectExpediente.IMPUTADO_FILIACION.FirstOrDefault(w => w.ID_TIPO_FILIACION == (short)enumMediaFilicacion.COMPLEXION);
                    if(f != null)
                        complexion = f.TIPO_FILIACION.DESCR;
                    f =  selectExpediente.IMPUTADO_FILIACION.FirstOrDefault(w => w.ID_TIPO_FILIACION == (short)enumMediaFilicacion.COLOR_PIEL);
                    if(f != null)
                        color_piel = f.TIPO_FILIACION.DESCR;
                    f =  selectExpediente.IMPUTADO_FILIACION.FirstOrDefault(w => w.ID_TIPO_FILIACION == (short)enumMediaFilicacion.CABELLO_COLOR);
                    if(f != null)
                        cabello_color = f.TIPO_FILIACION.DESCR;
                    f =  selectExpediente.IMPUTADO_FILIACION.FirstOrDefault(w => w.ID_TIPO_FILIACION == (short)enumMediaFilicacion.CABELLO_FORMA);
                    if(f != null)
                        cabello_forma = f.TIPO_FILIACION.DESCR;
                }

                string peso = "";
                string estatura = "";
                if(SelectExpediente.INGRESO != null)
                {
                    var ultimo_ingreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                    if(ultimo_ingreso != null)
                    {
                        peso = ultimo_ingreso.PESO != null ? ultimo_ingreso.PESO.ToString() : string.Empty;
                        estatura= ultimo_ingreso.ESTATURA != null ? ultimo_ingreso.ESTATURA.ToString() : string.Empty;
                    }
                }
                
                generales.Add(new cReporteProcesoLibertad(){
                    NOMBRE = SelectExpediente.NOMBRE,
                    PATERNO = SelectExpediente.PATERNO,
                    MATERNO = SelectExpediente.MATERNO,
                    EDAD = new Fechas().CalculaEdad(SelectExpediente.NACIMIENTO_FECHA) + "",
                    FECHA =  SelectExpediente.NACIMIENTO_FECHA.HasValue ? SelectExpediente.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty,
                    FOTO = foto,
                    COMPLEXION = complexion,
                    COLOR_PIEL = color_piel,
                    ESTATURA = estatura,
                    PESO = peso,
                    CABELLO_COLOR = cabello_color,
                    CABELLO_FORMA = cabello_forma,
                    CAUSA_PENAL = CPAnio.HasValue && CPFolio.HasValue ? string.Format("{0}/{1}",CPAnio,CPFolio) : string.Empty,
                    JUZGADO = Juzgado,
                    DELITO = Delito
                });

                if (SelectedProcesoLibertad.MEDIDA_LIBERTAD != null)
                {
                    foreach (var o in SelectedProcesoLibertad.MEDIDA_LIBERTAD)
                    {
                        medidas.Add(new cReporteProcesoLibertadMedidas()
                        {
                           OBLIGACIONES = o.MEDIDA.DESCR
                        });
                    }
                }

                var View = new ReporteView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { 
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); 
                };

                View.ReporteViewer.LocalReport.ReportPath = @"C:\git\seguridadpublica\ControlPenales\Reportes\rFichaLiberado.rdlc";//"Reportes/rFichaLiberado.rdlc";//
                
                View.ReporteViewer.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = generales;
                View.ReporteViewer.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = medidas;
                View.ReporteViewer.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = huellas_mano_izquierda;
                View.ReporteViewer.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = huellas_mano_derecha;
                View.ReporteViewer.LocalReport.DataSources.Add(rds4);

                ReportDataSource rds5 = new ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = logos;
                View.ReporteViewer.LocalReport.DataSources.Add(rds5);

                View.ReporteViewer.Refresh();
                View.ReporteViewer.RefreshReport();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_CAUSA_PENAL);
                View.Show();

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private byte[] getHuella(byte[] wsq)
        {
            if (wsq != null)
            {
                byte[] byteArray = new byte[0];
                MemoryStream ms = new MemoryStream(wsq);
                byte[] data = new byte[ms.Length];
                ms.Read(data, 0, data.Length);
                WsqDecoder decoder = new WsqDecoder();
                Bitmap bmp = decoder.Decode(data);
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Close();
                    byteArray = stream.ToArray();
                }
                return byteArray;
            }
            return null;
        }
        #endregion
    }

    public class MedidaDocumento
    { 
        public long AUDIENCIA_NOTIFICACION	{get;set;}
        public long NUC		{get;set;}
        public long CAUSA_PENAL		{get;set;}
        public string NUMERO_OFICIO	{get;set;}
        public string AUTO_DICTADO	{get;set;}
        public byte[] DOCUMENTO	{get;set;}
        public DateTime FECHA_INICIO_AUDIENCIA	{get;set;}
        public DateTime FECHA_FIN_AUDIENCIA	{get;set;}
        public int IDENTIFICADOR_AUD	{get;set;}
        public int IDENTIFICADOR_JUZGADO	{get;set;}
        public string SALA	{get;set;}
        public DateTime FECHA	{get;set;}
        public int BORRADO	{get;set;}
        public DateTime ULTIMA_MODIFICACION	{get;set;}
        public string FORMATO	{get;set;}
        public DateTime FECHA_REGISTRO	{get;set;}
        public DateTime FECHA_SMS	{get;set;}
        public int IDUNICO_AUDIENCIA	{get;set;}
        public int LIBERTAD { get; set; }
    }

    public class Asesor
    {
        public int ID_EMPLEADO { get; set; }
        public string NOMBRE { get; set; }
    }

    public class cLiberados
    {
        public short ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string CENTRO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string APODO_NOMBRE { get; set; }
        public string PATERNO_A { get; set; }
        public string MATERNO_A { get; set; }
    }
}
