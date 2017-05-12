using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using ControlPenales.Clases.ReportePasesPorAutorizar;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class CancelacionVisitasViewModel : FingerPrintScanner
    {
        public CancelacionVisitasViewModel() { }

        private async void Load_Window(PadronVisitasView Window)
        {
            ConfiguraPermisos();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GetDatosVisitante);
        }

        private void GetDatosVisitante()
        {
            try
            {
                ListTipoVisitante = ListTipoVisitante ?? new ObservableCollection<TIPO_VISITANTE>((new cTipoVisitante()).ObtenerTodos().OrderBy(o => o.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListTipoVisitante.Insert(0, new TIPO_VISITANTE() { ID_TIPO_VISITANTE = -1, DESCR = "SELECCIONE" });
                    SelectTipoVisitante = -1;
                }));

                ListSituacion = ListSituacion ?? new ObservableCollection<ESTATUS_VISITA>((new cEstatusVisita()).ObtenerTodos().Where(w => w.ID_ESTATUS_VISITA >= Parametro.ID_ESTATUS_VISITA_REGISTRO).OrderBy(o => o.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListSituacion.Insert(0, new ESTATUS_VISITA() { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                    SelectSituacion = -1;
                    ListEstatusVisita = new ObservableCollection<ESTATUS_VISITA>(ListSituacion.Where(w => w.ID_ESTATUS_VISITA >= Parametro.ID_ESTATUS_VISITA_REGISTRO));
                    ListEstatusVisita.Insert(0, new ESTATUS_VISITA() { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "guardar_menu":
                    IsDetalleInternosEnable = false;
                    break;
                case "buscar_menu":
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    Pagina = 1;
                    SeguirCargando = true;
                    TextNombre = TextPaterno = TextMaterno = string.Empty;
                    TextCodigo = null;
                    ListVisitantes = null;
                    SelectVisitante = null;
                    EmptyBuscarRelacionInternoVisible = false;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                case "limpiar_menu":
                    //LimpiarGeneral();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CancelacionVisitasView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new CancelacionVisitasViewModel ();
                    break;
                case "reporte_menu":
                    break;
                case "ficha_menu":
                    break;
                case "ayuda_menu":
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_visita":
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    //BuscarVisita();
                    BuscarVisitaNew();
                    //StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarVisitaNew);
                    //OnPropertyChanged();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                case "cancela_visitante":
                    if (!PEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (string.IsNullOrEmpty(TextObservacion))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes escribir una observacion.");
                        break;
                    }
                    if (SelectVisitante == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                        break;
                    }
                    if (SelectVisitante.OBJETO_PERSONA == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                        break;
                    }
                    if (SelectVisitante.OBJETO_PERSONA.VISITANTE == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                        break;
                    }
                    if (SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_ESTATUS_VISITA != SelectSituacion)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Cancelar / Suspender Credenciales", "Esta seguro que desea cambiar el estatus a " +
                            ListEstatusVisita.Where(w => w.ID_ESTATUS_VISITA == SelectSituacion).FirstOrDefault().DESCR.Trim() + " ?") == 1)
                        {
                            if (new cVisitante().Actualizar(new VISITANTE
                            {
                                ID_PERSONA = SelectVisitante.OBJETO_PERSONA.ID_PERSONA,
                                ID_ESTATUS_VISITA = SelectSituacion,
                                FEC_ALTA = SelectVisitante.OBJETO_PERSONA.VISITANTE.FEC_ALTA,
                                //ID_TIPO_VISITANTE = SelectVisitante.OBJETO_PERSONA.VISITANTE.ID_TIPO_VISITANTE,
                                //ACCESO_UNICO = SelectVisitante.OBJETO_PERSONA.VISITANTE.ACCESO_UNICO,
                                //FEC_PLAZO = SelectVisitante.OBJETO_PERSONA.VISITANTE.FEC_PLAZO,
                                ULTIMA_MODIFICACION = SelectVisitante.OBJETO_PERSONA.VISITANTE.ULTIMA_MODIFICACION
                            }))
                                new Dialogos().ConfirmacionDialogo("Éxito", "El estatus ha cambiado exitosamente");
                        }
                    }
                    break;
                case "cancela_visitante_ingreso":
                    try
                    {
                        if (!PEditar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        if (SelectVisitante == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una persona.");
                            return;
                        }
                        if (SelectVisitanteIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (SelectEstatusImputado <= 0)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar el estatus al que lo quieres cambiar.");
                            return;
                        }
                        if (string.IsNullOrEmpty(TextMotivoImputado))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes escribir un motivo.");
                            return;
                        }
                        if (await new Dialogos().ConfirmarEliminar("Entrega de Credencial", "Esta seguro que desea cambiar el estatus a " +
                            ListEstatusVisita.Where(w => w.ID_ESTATUS_VISITA == SelectEstatusImputado).FirstOrDefault().DESCR.Trim() +
                            " en la credencial de " 
                            + (!string.IsNullOrEmpty(SelectVisitante.NOMBRE) ?  SelectVisitante.NOMBRE.Trim() : string.Empty) + " " 
                            + (!string.IsNullOrEmpty(SelectVisitante.PATERNO) ? SelectVisitante.PATERNO.Trim() : string.Empty) + " " 
                            + (!string.IsNullOrEmpty(SelectVisitante.MATERNO) ? SelectVisitante.MATERNO.Trim() : string.Empty) + "?") == 1)
                        {
                            #region VISITANTE_INGRESO
                            if(new cVisitanteIngreso().Actualizar(new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = SelectVisitanteIngreso.EMISION_GAFETE,
                                FEC_ALTA = SelectVisitanteIngreso.FEC_ALTA,
                                FEC_ULTIMA_MOD = Fechas.GetFechaDateServer,
                                ID_ANIO = SelectVisitanteIngreso.ID_ANIO,
                                ID_CENTRO = SelectVisitanteIngreso.ID_CENTRO,
                                ID_ESTATUS_VISITA = SelectEstatusImputado,
                                ID_IMPUTADO = SelectVisitanteIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectVisitanteIngreso.ID_INGRESO,
                                ID_PERSONA = SelectVisitanteIngreso.ID_PERSONA,
                                ID_TIPO_REFERENCIA = SelectVisitanteIngreso.ID_TIPO_REFERENCIA,
                                OBSERVACION = SelectVisitanteIngreso.OBSERVACION,
                                ESTATUS_MOTIVO = TextMotivoImputado
                            }))
                                new Dialogos().ConfirmacionDialogo("Éxito", "El estatus ha cambiado exitosamente");
                            #endregion
                            //StaticSourcesViewModel.Mensaje("Entrega Credencial", "La credencial fue entregada a la persona", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la cancelacion de la credencial.", ex);
                    }
                    break;
                case "buscar_visitante":
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    //BuscarVisita();
                    BuscarVisitaNew();
                    //StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarVisitaNew);
                    //OnPropertyChanged();
                    break;
                case "seleccionar_buscar_visita":
                    if (SelectVisitante != null)
                    {
                        ObservacionEnabled = true;
                        GetDatosVisitanteSeleccionadoPadron();
                        OnPropertyChanged();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    }
                    else
                    {
                        ObservacionEnabled = false;
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar una persona.");
                    }
                    break;
                case "cancelar_buscar_visita":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    break;
                default: // buscar visitantes click enter
                    if (!PConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    if (obj != null)
                    {
                        TextCodigo = null;
                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    TextNombre = textbox.Text;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    TextPaterno = textbox.Text;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    TextMaterno = textbox.Text;
                                    break;
                                case "FolioBuscar":
                                    var n = 0;
                                    TextCodigo = int.TryParse(textbox.Text, out n) ? n : 0;
                                    break;
                            }
                        }
                        //BuscarVisita();
                        BuscarVisitaNew();
                        //StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarVisitaNew);
                    }
                    break;
            }
        }

        private void SeleccionarAcompaniante(object obj)
        {
            try
            {
                if (obj is ACOMPANANTE)
                    if (((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO != null)
                        if (((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        {
                            ImagenAcompanante = ((ACOMPANANTE)obj).VISITANTE_INGRESO1.VISITANTE.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                            enableMotivoEstatusAcompaniante = true;
                            SelectEstatusAcompaniante = "S";
                            TextMotivoAcompaniante = string.Empty;
                        }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del acompañante seleccionado.", ex);
            }
        }

        private void SeleccionarImputado(Object obj)
        {
            try
            {
                if (obj != null ? obj is DataGrid ? ((DataGrid)obj).SelectedItem == null : true : true)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                    return;
                }
                SelectVisitanteIngreso = ((VISITANTE_INGRESO)((DataGrid)obj).SelectedItem);
                var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                foreach (var item in EstatusInactivos)
                {
                    if (SelectVisitanteIngreso.INGRESO.ID_ESTATUS_ADMINISTRATIVO == item)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        return;
                    }
                }
                if (SelectVisitanteIngreso.INGRESO.ID_UB_CENTRO.HasValue ? SelectVisitanteIngreso.INGRESO.ID_UB_CENTRO != GlobalVar.gCentro : false)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                    return;
                }
                var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                if (SelectVisitanteIngreso.INGRESO.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado)<= Fechas.GetFechaDateServer))
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectVisitanteIngreso.INGRESO.ID_ANIO.ToString() + "/" +
                        SelectVisitanteIngreso.INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                    return;
                }
                SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                enableMotivoEstatusImputado = true;
                SelectEstatusImputado = SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ? SelectVisitanteIngreso.ID_ESTATUS_VISITA : (short)-1;
                TextMotivoImputado = SelectVisitanteIngreso.ESTATUS_MOTIVO;
                GetDatosIngresoImputadoSeleccionado();
                IsDetalleInternosEnable = false;
                TextHeaderDatosInterno = "Datos del Interno Seleccionado";
                DatosExpedienteVisible = Visibility.Visible;
                TextObservacion = string.Empty;
                SelectParentescoAcompanante = -1;

                //* * * * * * * * * * * *//

                ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>

                    w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&

                    !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&

                    new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                        w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&

                    (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));

                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private void GetDatosIngresoImputadoSeleccionado()
        {
            try
            {
                if (SelectImputadoIngreso != null)
                {
                    AnioD = SelectImputadoIngreso.ID_ANIO.ToString();
                    FolioD = SelectImputadoIngreso.ID_IMPUTADO.ToString();
                    NombreD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.NOMBRE) ? string.Empty : SelectImputadoIngreso.IMPUTADO.NOMBRE.Trim();
                    PaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.PATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.PATERNO.Trim();
                    MaternoD = string.IsNullOrEmpty(SelectImputadoIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectImputadoIngreso.IMPUTADO.MATERNO.Trim();
                    IngresosD = SelectImputadoIngreso.IMPUTADO.INGRESO.Count.ToString();
                    UbicacionD = SelectImputadoIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectImputadoIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + "" +
                                 SelectImputadoIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + SelectImputadoIngreso.CAMA.ID_CAMA;
                    TipoSeguridadD = SelectImputadoIngreso.TIPO_SEGURIDAD.DESCR;
                    FecIngresoD = SelectImputadoIngreso.FEC_INGRESO_CERESO == null ? null : SelectImputadoIngreso.FEC_INGRESO_CERESO.ToString();
                    ClasificacionJuridicaD = SelectImputadoIngreso.CLASIFICACION_JURIDICA.DESCR;
                    EstatusD = SelectImputadoIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                    ImagenIngreso = SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                        SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                            SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                                SelectImputadoIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                    new Imagenes().getImagenPerson();


                    if (SelectVisitanteIngreso != null)
                    {
                        if (SelectVisitante.OBJETO_PERSONA != null)
                        {
                            var personaNip = SelectVisitante.OBJETO_PERSONA.PERSONA_NIP.Where(w => w.ID_CENTRO == SelectVisitanteIngreso.ID_CENTRO && w.ID_TIPO_VISITA == Parametro.ID_TIPO_VISITA_FAMILIAR);
                            TextNip = personaNip.Any() ? personaNip.FirstOrDefault().NIP.HasValue ? personaNip.FirstOrDefault().NIP.Value.ToString() : null : null;
                        }
                        SelectParentesco = SelectVisitanteIngreso.ID_TIPO_REFERENCIA.HasValue ? SelectVisitanteIngreso.ID_TIPO_REFERENCIA.Value : (short)-1;
                        SelectEstatusRelacion = SelectVisitanteIngreso.ID_ESTATUS_VISITA.HasValue ? SelectVisitanteIngreso.ID_ESTATUS_VISITA.Value : (short)-1;
                        ListAcompanantes = new ObservableCollection<ACOMPANANTE>(SelectVisitanteIngreso.ACOMPANANTE.Where(w => new Fechas().CalculaEdad(w.VISITANTE_INGRESO1.VISITANTE.PERSONA.FEC_NACIMIENTO) < Parametro.MAYORIA_EDAD && new Fechas().CalculaEdad(w.VISITANTE_INGRESO1.VISITANTE.PERSONA.FEC_NACIMIENTO) > (w.VISITANTE_INGRESO1.VISITANTE.PERSONA.SEXO == "M" ? Parametro.EDAD_MENOR_M : Parametro.EDAD_MENOR_F)));
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }

        private void LimpiarGeneral()
        {
            TextMotivoImputado = TextMotivoAcompaniante = TextNombre = TextPaterno = TextMaterno = TextNip = NombreD = PaternoD = MaternoD = AnioD = FolioD = IngresosD = UbicacionD = TipoSeguridadD = FecIngresoD = ClasificacionJuridicaD = EstatusD = string.Empty;
            TextCodigo = null;
            FotoVisita = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
            ImagenIngreso = ImagenPersona = new Imagenes().getImagenPerson();
            SelectVisitante = null;
            SelectVisitanteIngreso = null;
            SelectImputadoIngreso = null;
            FechaNacimiento = Fechas.GetFechaDateServer;
            TextEdad = new Nullable<short>();
            SelectSexo = "S";
            SelectParentesco = SelectSituacion = SelectTipoVisitante = -1;
            ListAcompanantes = new ObservableCollection<ACOMPANANTE>();
            ListadoInternos = ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>();
            //ListVisitantes = new ObservableCollection<PERSONAVISITAAUXILIAR>();
            ListVisitantes = null;
            TextHeaderDatosInterno = "Seleccion de nuevo interno para el visitante actual";
            DatosExpedienteVisible = Visibility.Collapsed;
            ImagenAcompanante = new Imagenes().getImagenPerson();
            SelectEstatusImputado = -1;
            enableMotivoEstatusImputado = enableMotivoEstatusAcompaniante = ObservacionEnabled = false;
            TextObservacion = string.Empty;

            OnPropertyChanged();
        }

        //private async void BuscarVisita()
        //{
        //    try
        //    {
        //        long? codigo = null;
        //        if (TextCodigo != null)
        //            codigo = TextCodigo;
        //        if (string.IsNullOrEmpty(TextNombre))
        //            TextNombre = string.Empty;
        //        if (string.IsNullOrEmpty(TextPaterno))
        //            TextPaterno = string.Empty;
        //        if (string.IsNullOrEmpty(TextMaterno))
        //            TextMaterno = string.Empty;
        //        var lista = new List<PERSONAVISITAAUXILIAR>();

        //        if (TextCodigo.HasValue)
        //        {
        //            #region PERSONA
        //            var persona = await StaticSourcesViewModel.CargarDatosAsync<SSP.Servidor.PERSONA>(() => new cPersona().Obtener(TextCodigo.Value).FirstOrDefault());
        //            if (persona != null)
        //            {
        //                SelectVisitante = new PadronVisitasViewModel().ConvertPersonaToAuxiliar(persona);
        //                GetDatosVisitanteSeleccionadoPadron();
        //                ObservacionEnabled = true;
        //            }
        //            else
        //            {
        //                ObservacionEnabled = false;
        //                (new Dialogos()).ConfirmacionDialogo("Validación", "No se encontro ninguna persona con ese numero de visitante.");
        //                TextCodigo = new Nullable<long>();
        //                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region PERSONAS
        //            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
        //            {
        //                try
        //                {
        //                    var personas = new cPersona().ObtenerPersonasVisitantes(TextNombre, TextPaterno, TextMaterno, codigo);
        //                    Application.Current.Dispatcher.Invoke((Action)(delegate
        //                    {
        //                        lista.AddRange(personas.ToList().Select(s => new PadronVisitasViewModel().ConvertPersonaToAuxiliar(s)));
        //                    }));
        //                }
        //                catch (Exception ex)
        //                {
        //                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda.", ex);
        //                }
        //            });
        //            #endregion

        //            ListVisitantes = new ObservableCollection<PERSONAVISITAAUXILIAR>(lista);
        //            if (ListVisitantes.Count > 0)//Empty row
        //            {
        //                EmptyBuscarRelacionInternoVisible = false;
        //                SeleccionarVisitaExistente = true;
        //            }
        //            else
        //            {
        //                EmptyBuscarRelacionInternoVisible = true;
        //                SeleccionarVisitaExistente = false;
        //            }
        //            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda.", ex);
        //    }
        //}

        private List<PERSONAVISITAAUXILIAR> SegmentarBusqueda(int _Pag = 1)
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                if (string.IsNullOrEmpty(TextPaterno) && string.IsNullOrEmpty(TextMaterno) && string.IsNullOrEmpty(TextNombre) && TextCodigo.HasValue)
                    return new List<PERSONAVISITAAUXILIAR>();
                Pagina = _Pag;
                var result = new List<PERSONAVISITAAUXILIAR>();
                var visitas = new cPersona().ObtenerPersonasVisitantes(TextNombre, TextPaterno, TextMaterno, TextCodigo, Pagina).ToList().Select(w => new PadronVisitasViewModel().ConvertPersonaToAuxiliar(w));
                if (visitas.Any())
                {
                    result.AddRange(visitas);
                    //result.AddRange(visitas.Select(s => new PadronVisitasViewModel().ConvertPersonaToAuxiliar(s)));
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<PERSONAVISITAAUXILIAR>();
            }
        }

        private void GetDatosVisitanteSeleccionadoPadron()
        {
            try
            {
                if (SelectVisitante != null)
                {
                    if (SelectVisitante.OBJETO_PERSONA != null)
                    {
                        #region PERSONA
                        TextCodigo = SelectVisitante.ID_PERSONA;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectSexo = SelectVisitante.SEXO;
                        FechaNacimiento = SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO;
                        SelectSituacion = SelectVisitante.ID_ESTATUS_VISITA;
                        SelectParentesco = SelectVisitante.ID_PARENTESCO.HasValue ? SelectVisitante.ID_PARENTESCO.Value : (short)-1;
                        FotoVisita =
                            SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Any() ?
                                new Imagenes().ConvertByteToBitmap(SelectVisitante.OBJETO_PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO) :
                                    new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                        TextObservacion = string.Empty;
                        if (SelectVisitante != null)
                            if (SelectVisitante.OBJETO_PERSONA != null)
                                if (SelectVisitante.OBJETO_PERSONA.VISITANTE != null)
                                    if (SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO != null)
                                        //if (SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.Any())
                                        {
                                            var i = SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.FirstOrDefault();
                                            if (i != null)
                                            {
                                                SelectTipoVisitante = i.ID_TIPO_VISITANTE;
                                            }
                                            else
                                                SelectTipoVisitante = null;
                                            //SelectTipoVisitante = SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO.FirstOrDefault().ID_TIPO_VISITANTE;
                                            foreach (var item in SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO)
                                            {
                                                if (item.INGRESO.CAMA.ID_CAMA != 0)
                                                {
                                                    item.INGRESO.CAMA.CELDA.ID_CELDA = item.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                                    item.INGRESO.CAMA.CELDA.SECTOR.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                                                    item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = item.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                                                }
                                            }
                                   
                                    }
                        #endregion

                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>(SelectVisitante.OBJETO_PERSONA.VISITANTE.VISITANTE_INGRESO);
                        if (ListadoInternos != null)
                            if (ListadoInternos.Count == 1)
                            {
                                SelectVisitanteIngreso = ListadoInternos.FirstOrDefault();
                                TextNip = SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP != null ?
                                    SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP.Count > 0 ?
                                        SelectVisitanteIngreso.VISITANTE.PERSONA.PERSONA_NIP.FirstOrDefault().NIP.ToString() :
                                            string.Empty : string.Empty;
                                SelectImputadoIngreso = ListadoInternos.FirstOrDefault().INGRESO;
                                GetDatosIngresoImputadoSeleccionado();
                                ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>();
                                ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>
                                    w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&
                                    !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&
                                    new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                    w.VISITANTE.PERSONA.FEC_NACIMIENTO :
                                    DateTime.Parse("01/01/1900")) >= 18 &&
                                    (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));

                                SelectTipoVisitante = SelectVisitanteIngreso.ID_TIPO_VISITANTE;
                            }
                    }
                    else if (SelectVisitante.OBJETO_VISITA_AUTORIZADA != null)
                    {
                        #region VISITA_AUTORIZADA

                        TextCodigo = null; //SelectVisitante.ID_PERSONA;
                        TextNombre = string.IsNullOrEmpty(SelectVisitante.NOMBRE) ? string.Empty : SelectVisitante.NOMBRE.Trim();
                        TextPaterno = string.IsNullOrEmpty(SelectVisitante.PATERNO) ? string.Empty : SelectVisitante.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectVisitante.MATERNO) ? string.Empty : SelectVisitante.MATERNO.Trim();
                        SelectTipoVisitante = Parametro.ID_TIPO_VISITANTE_ORDINARIO;
                        SelectSexo = SelectVisitante.SEXO;
                        FechaNacimiento = Fechas.GetFechaDateServer.AddYears(-(int)SelectVisitante.EDAD); // SelectVisitante.OBJETO_PERSONA.FEC_NACIMIENTO;
                        TextEdad = SelectVisitante.EDAD;
                        SelectSituacion = 11;
                        TextNip = string.Empty; //SelectVisitante.OBJETO_PERSONA.NIP;
                        SelectParentesco = SelectVisitante.ID_PARENTESCO.HasValue ? SelectVisitante.ID_PARENTESCO.Value : (short)-1;
                        SelectEstatusRelacion = SelectVisitante.ID_ESTATUS_VISITA;
                        if (SelectVisitante.INGRESO != null)
                        {
                            if (SelectVisitante.INGRESO.CAMA.ID_CAMA != 0)
                            {
                                SelectVisitante.INGRESO.CAMA.CELDA.ID_CELDA = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.ID_CELDA.ToString().Trim();
                                SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.DESCR.Trim();
                                SelectVisitante.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR = SelectVisitante.OBJETO_VISITA_AUTORIZADA.INGRESO.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim();
                            }

                        }
                        #endregion
                        ListadoInternos = new ObservableCollection<VISITANTE_INGRESO>();
                        ListadoInternos.Add(new VISITANTE_INGRESO()
                        {
                            INGRESO = SelectVisitante.INGRESO,
                            ID_CENTRO = (short)SelectVisitante.ID_CENTRO,
                            ID_ANIO = (short)SelectVisitante.ID_ANIO,
                            ID_IMPUTADO = (int)SelectVisitante.ID_IMPUTADO,
                            ID_ESTATUS_VISITA = SelectVisitante.ID_ESTATUS_VISITA,
                            TIPO_REFERENCIA = SelectVisitante.TIPO_REFERENCIA,
                            ESTATUS_VISITA = SelectVisitante.ESTATUS_VISITA,
                            ID_INGRESO = SelectVisitante.INGRESO.ID_INGRESO,
                            ID_TIPO_REFERENCIA = SelectVisitante.ID_PARENTESCO
                        });
                        SelectVisitanteIngreso = ListadoInternos.FirstOrDefault();
                        SelectImputadoIngreso = SelectVisitanteIngreso.INGRESO;
                        GetDatosIngresoImputadoSeleccionado();

                        ListBuscarAcompanantes = new ObservableCollection<VISITANTE_INGRESO>(SelectImputadoIngreso.VISITANTE_INGRESO.Where(w =>

                            w.ID_PERSONA != SelectVisitanteIngreso.ID_PERSONA &&

                            !ListAcompanantes.Where(x => x.ID_ACOMPANANTE == w.ID_PERSONA).Any() &&

                            new Fechas().CalculaEdad(w.VISITANTE.PERSONA.FEC_NACIMIENTO.HasValue ?
                                w.VISITANTE.PERSONA.FEC_NACIMIENTO : DateTime.Parse("01/01/1900")) >= 18 &&

                            (w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_EN_REVISION || w.ID_ESTATUS_VISITA == Parametro.ID_ESTATUS_VISITA_AUTORIZADO)));
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del visitante seleccionado.", ex);
            }
        }

        private void ClickEnter(Object obj)
        {
            if (!PConsultar)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
            base.ClearRules();
            if (obj != null)
            {
                //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                var textbox = obj as TextBox;
                if (textbox != null)
                {
                    switch (textbox.Name)
                    {
                        case "NombreBuscar":
                            TextNombre = textbox.Text;
                            break;
                        case "ApellidoPaternoBuscar":
                            TextPaterno = textbox.Text;
                            break;
                        case "ApellidoMaternoBuscar":
                            TextMaterno = textbox.Text;
                            break;
                        case "CodigoBuscar":
                            //var n = 0;
                            //TextCodigo = int.TryParse(textbox.Text, out n) ? n : 0;
                            if (!string.IsNullOrEmpty(textbox.Text))
                                TextCodigo = long.Parse(textbox.Text);
                            break;
                    }
                }
            }
            //BuscarVisita();
            BuscarVisitaNew();
            //StaticSourcesViewModel.CargarDatosMetodoAsync(BuscarVisitaNew);
        }

        private void BuscarVisitaNew() 
        {
            try
            {
                Pagina = 1;
                SeguirCargando = true;
                ListVisitantes = new RangeEnabledObservableCollection<PERSONAVISITAAUXILIAR>();
                //Application.Current.Dispatcher.Invoke((Action)(delegate
                //{
                    ListVisitantes = new RangeEnabledObservableCollection<PERSONAVISITAAUXILIAR>();
                    ListVisitantes.InsertRange(SegmentarBusqueda());
                //}));
                
                EmptyBuscarRelacionInternoVisible = ListVisitantes.Count > 0 ? false : true;
                if (ListVisitantes != null)
                {
                    if (ListVisitantes.Count == 1)
                    {
                        SelectVisitante = ListVisitantes.FirstOrDefault();
                        GetDatosVisitanteSeleccionadoPadron();
                        ObservacionEnabled = true;
                    }
                    else
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                    }
                }
                else
                {
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_VISITA_EXISTENTE);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del visitante seleccionado.", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CANCELAR_SUSPENDER_CREDENCIALES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = true;
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
}
