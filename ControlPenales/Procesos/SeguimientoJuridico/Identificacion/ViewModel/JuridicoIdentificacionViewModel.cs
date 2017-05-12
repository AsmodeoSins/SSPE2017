using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel : FingerPrintScanner
    {
        private bool _GuardarSenasParticulares { get; set; }
        
        public JuridicoIdentificacionViewModel() { }
        
        private void TipoSwitch(Object obj)
        {
            try
            {
                if (Int32.TryParse(obj.ToString(), out selectTipoSenia))
                {
                    SelectTipoSenia = selectTipoSenia;
                }
                if (!string.IsNullOrEmpty(CodigoSenia))
                    RegionValorCodigo = CodigoSenia[1].ToString() + CodigoSenia[2].ToString() + CodigoSenia[3].ToString() + "";
                if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                    CodigoSenia = SelectTipoSenia.ToString() + "" + RegionValorCodigo + "" + SelectAnatomiaTopografica.LADO + "" + TextCantidad;
                if (SelectAnatomiaTopografica != null)
                {
                    TipoTatuajeEnabled = false;
                    if (!string.IsNullOrEmpty(TextTipoSenia))
                    {
                        var clasif = string.Empty;
                        if (SelectClasificacionTatuaje != null)
                            clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? "" : " " + SelectClasificacionTatuaje.DESCR;
                        if (SelectTatuaje != null && SelectTatuaje.ID_TATUAJE > 0)
                            TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                        else
                            TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                    }
                }
                if (SelectTipoSenia == 2)
                {
                    TipoTatuajeEnabled = true;
                    SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == -1).FirstOrDefault();
                    SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == "").FirstOrDefault();
                }
                else
                {
                    TipoTatuajeEnabled = false;
                    SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == 0).FirstOrDefault();
                    SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == "").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el código de la seña.", ex);
            }
        }
        
        private void RegionSwitch(Object obj)
        {
            try
            {
                string[] splits = obj.ToString().Split('-');
                //SelectSenaParticular
                RegionValorCodigo = splits[1];
                SelectAnatomiaTopografica = ListRegionCuerpo.Where(w => w.ID_REGION.ToString() == splits[0]).FirstOrDefault();
                if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                    CodigoSenia = SelectTipoSenia.ToString() + "" + RegionValorCodigo + "" + SelectAnatomiaTopografica.LADO + "" + TextCantidad;
                if (!string.IsNullOrEmpty(TextTipoSenia) && SelectAnatomiaTopografica != null)
                {
                    var clasif = string.Empty;
                    if (SelectClasificacionTatuaje != null)
                        clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? "" : SelectClasificacionTatuaje.DESCR;
                    if (SelectTatuaje != null && SelectTatuaje.ID_TATUAJE > 0)
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                    else
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el texto del significado.", ex);
            }
        }
        
        private bool HasErrors()
        { return base.HasErrors; }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "limpiar_menu":
                    //if (IsSelectedIdentificacion && TabSenasParticulares)
                    //{
                    //    LimpiarCamposSeniasParticulares();
                    //    break;m
                    //}
                    //if (CamaraWeb != null)
                    //{
                    //    await CamaraWeb.ReleaseVideoDevice();990
                    //    CamaraWeb = null;
                    //}
                    if (Opcion == (short)enumIngresoTabs.SENIAS_PARTICULARES)
                    {
                        LimpiarCamposSeniasParticulares();
                    }
                    else
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroIngresoView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.JuridicoIdentificacionViewModel();
                    }
                    break;
                case "ampliar_descripcion":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "guardar_ampliar_descripcion":
                    TextSignificado = TextSignificado + " " + TextAmpliarDescripcion;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "cancelar_ampliar_descripcion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "tomar_foto_senas":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    BotonTomarFotoEnabled = false;
                    TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    break;
                case "aceptar_tomar_foto_senas":
                    ImagenTatuaje = ImagesToSave[0].ImageCaptured;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    break;
                case "cancelar_tomar_foto_senas":
                    //ImagenTatuaje = new ima
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    break;
                case "buscar_visible":
                    this.buscarImputado();
                    break;
                case "buscar_salir":
                    SelectExpediente = tImputado;
                    SelectIngreso = tIngreso;
                    tImputado = null;
                    tIngreso = null;
                    //AnioBuscar = FolioBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    //TabVisible = false;
                    Imputado = ImputadoSeleccionadoAuxiliar;
                    //Auxiliar
                    if (Imputado != null)
                    {
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                    }
                    ////////////////////////////////////////////////
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "seleccionar_sena_particular": //DOBLE CLICK EN SENIAS PARTICULARES
                    try
                    {
                        if (SelectSenaParticular != null)
                        {
                            SeniasParticularesEditable = true;
                            RegionCodigo = SelectSenaParticular.CODIGO.ToCharArray();
                            SelectAnatomiaTopografica = SelectSenaParticular.ANATOMIA_TOPOGRAFICA == null ? SelectSenaParticular.ANATOMIA_TOPOGRAFICA : new cAnatomiaTopografica().Obtener((int)SelectSenaParticular.ID_REGION);
                            //SelectAnatomiaTopografica = new cAnatomiaTopografica().Obtener((int)SelectSenaParticular.ID_REGION);
                            if (regionCodigo[4].ToString() == "F")// || (RegionCodigo[4].ToString() == "U" && SelectSenaParticular.ID_REGION < 50))
                                TabFrente = true;
                            else if (regionCodigo[4].ToString() == "D")// || (RegionCodigo[4].ToString() == "U" && SelectSenaParticular.ID_REGION >= 50))
                                TabDorso = true;
                            foreach (var item in ListRadioButons)
                            {
                                if (item.CommandParameter != null)
                                    if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                        item.IsChecked = true;
                                    else
                                        item.IsChecked = false;
                            }
                            ImagenTatuaje = new Imagenes().ConvertByteToBitmap(SelectSenaParticular.IMAGEN);
                            TextCantidad = SelectSenaParticular.CANTIDAD.ToString();
                            SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == SelectSenaParticular.ID_TIPO_TATUAJE).FirstOrDefault();
                            SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == SelectSenaParticular.CLASIFICACION).FirstOrDefault();
                            SelectTipoSenia = (int)SelectSenaParticular.TIPO;
                            if (SelectSenaParticular.INTRAMUROS == "S")
                            {
                                SelectPresentaIntramuros = true;
                                SelectPresentaIngresar = false;
                            }
                            else
                            {
                                SelectPresentaIntramuros = false;
                                SelectPresentaIngresar = true;
                            }
                            SelectTipoCicatriz = SelectTipoTatuaje = SelectTipoLunar = SelectTipoDefecto = SelectTipoProtesis = SelectTipoAmputacion = false;
                            if (SelectSenaParticular.TIPO == 1)
                                SelectTipoCicatriz = true;
                            else if (SelectSenaParticular.TIPO == 2)
                                SelectTipoTatuaje = true;
                            else if (SelectSenaParticular.TIPO == 3)
                                SelectTipoLunar = true;
                            else if (SelectSenaParticular.TIPO == 4)
                                SelectTipoDefecto = true;
                            else if (SelectSenaParticular.TIPO == 5)
                                SelectTipoProtesis = true;
                            else if (SelectSenaParticular.TIPO == 6)
                                SelectTipoAmputacion = true;
                            TextSignificado = SelectSenaParticular.SIGNIFICADO;
                            CodigoSenia = SelectSenaParticular.CODIGO;
                            RegionValorCodigo = CodigoSenia[1].ToString() + CodigoSenia[2].ToString() + CodigoSenia[3].ToString() + "";
                            TextAmpliarDescripcion = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos de la seña particular seleccionada.", ex);
                    }
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectExpediente == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un expediente o crear uno nuevo.");
                            return;
                        }
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                            return;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.Any(a => a.ID_ESTATUS_ADMINISTRATIVO == item))
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            }
                        }
                        if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO.Value != GlobalVar.gCentro)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Ingreso no vigente.", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        SelectExpediente = Imputado = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        this.getDatosImputado();
                        ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                        ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                        ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                        ContenedorIdentificacionVisible = EditarImputado = true;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        IngresoEnabled = TrasladoEnabled = CamposBusquedaEnabled = false;
                        if (PInsertar || PEditar)
                            MenuGuardarEnabled = true;
                        if (PConsultar)
                            MenuBuscarEnabled = /*MenuReporteEnabled =*/ MenuFichaEnabled = true;

                        ImputadoSeleccionadoAuxiliar = Imputado;
                        Opcion = 0;
                        BanderaEntrada = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        StaticSourcesViewModel.SourceChanged = false;
                        //ContenedorIdentificacionVisible = EditarImputado = true;
                        //Imputado = SelectExpediente;
                        //ApellidoPaternoBuscar = Imputado.PATERNO;
                        //ApellidoMaternoBuscar = Imputado.MATERNO;
                        //NombreBuscar = Imputado.NOMBRE;
                        //AnioBuscar = Imputado.ID_ANIO;
                        //FolioBuscar = Imputado.ID_IMPUTADO;
                        //IngresoEnabled = TrasladoEnabled = CamposBusquedaEnabled = false;
                        //IdentificacionEnabled = TabDatosGenerales = true;
                        //getDatosImputado();
                        //ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                        //ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                        //ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                        //TextNombreCompleto = Imputado.PATERNO.Trim() + " " + Imputado.MATERNO.Trim() + " " + Imputado.NOMBRE.Trim();
                        //TextExpediente = Imputado.ID_ANIO + "/" + Imputado.ID_IMPUTADO;

                        ////MenuInsertarEnabled = MenuDeshacerEnabled = MenuGuardarEnabled = MenuLimpiarEnabled = MenuReporteEnabled = MenuBorrarEnabled =
                        ////    MenuBuscarEnabled = MenuFichaEnabled = MenuAyudaEnabled = MenuSalirEnabled = true;
                        ////permisos
                        //if (PInsertar || PEditar)
                        //    MenuInsertarEnabled = true;
                        //if (PConsultar)
                        //    MenuBuscarEnabled = MenuReporteEnabled = MenuFichaEnabled = true;

                        //ImputadoSeleccionadoAuxiliar = Imputado;
                        //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);

                        //ChecarValidaciones();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado.", ex);
                    }
                    break;
                case "agregar_causa_penal":
                    //TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "ingresos_discrecionales":
                    //TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "insertar_menu":
                    if (TabSenasParticulares)
                        LimpiarCamposSeniasParticulares();
                    break;
                case "insertar_delito":
                    //TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "insertar_alias":
                    if (PInsertar)
                    {
                        setValidacionesIdentificacionAlias();
                        NombreAlias = PaternoAlias = MaternoAlias = string.Empty;
                        TituloAlias = "Agregar Alias";
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        OpcionGuardarAlias = 1;
                        SelectAlias = null;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");

                    break;
                case "editar_alias":
                    if (pEditar)
                    {
                        setValidacionesIdentificacionAlias();
                        if (SelectAlias != null)
                        {
                            TituloAlias = "Editar Alias";
                            PaternoAlias = SelectAlias.PATERNO;
                            MaternoAlias = SelectAlias.MATERNO;
                            NombreAlias = SelectAlias.NOMBRE;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                            OpcionGuardarAlias = 0;
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "eliminar_alias":
                    if (pEditar)
                    {
                        this.EliminarAlias();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "guardar_alias":
                    if (!base.HasErrors)
                    {
                        this.GuardarAlias();
                        base.ClearRules();
                    }
                    break;
                case "cancelar_alias":
                    PaternoAlias = MaternoAlias = NombreAlias = string.Empty;
                    SelectAlias = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    base.ClearRules();
                    break;
                case "insertar_apodo":
                    if (PInsertar)
                    {
                        setValidacionesIdentificacionApodos();
                        TituloApodo = "Agregar Apodo";
                        Apodo = string.Empty;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        OpcionGuardarApodo = 1;
                        SelectApodo = null;
                        break;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "editar_apodo":
                    if (PEditar)
                    {
                        setValidacionesIdentificacionApodos();
                        if (SelectApodo != null)
                        {
                            TituloApodo = "Editar Apodo";
                            Apodo = SelectApodo.APODO1;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                            OpcionGuardarApodo = 0;
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "eliminar_apodo":
                    if (PEditar)
                    {
                        this.EliminarApodo();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "guardar_apodo":
                    if (!base.HasErrors)
                    {
                        this.GuardarApodo();
                        base.ClearRules();
                    }
                    break;
                case "cancelar_apodo":
                    Apodo = string.Empty;
                    SelectApodo = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                    base.ClearRules();
                    break;
                case "insertar_familiar_responsable":
                    //ListFamiliarResponsable.Add(new PERSONA());
                    break;
                case "eliminar_familiar_responsable":
                    //ListFamiliarResponsable.Remove(SelectFamiliarResponsable);
                    break;
                case "insertar_relacion_interno":
                    if (pInsertar)
                    {
                        setValidacionesIdentificacionRelacionesPersonales();
                        TituloRelacionInterno = "Agregar Relación Personal Interno";
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                        OpcionGuardarRelacionInterno = 1;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "editar_relacion_interno":
                    if (PEditar)
                    {
                        setValidacionesIdentificacionRelacionesPersonales();
                        if (SelectRelacionPersonalInterno != null)
                        {
                            TituloRelacionInterno = "Editar Relación Personal Interno";
                            NotaRelacionInterno = SelectRelacionPersonalInterno.NOTA;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                            OpcionGuardarRelacionInterno = 0;
                        }
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "eliminar_relacion_interno":
                    if (pEditar)
                    {
                        this.EliminarRelacionInterno();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "guardar_relacion_interno":
                    if (!base.HasErrors)
                    {
                        this.GuardarRelacionInterno();
                        base.ClearRules();
                    }
                    break;
                case "cancelar_relacion_interno":
                    CancelarBuscarRelacionInterno();
                    break;
                case "nueva_busqueda":
                    tImputado = SelectExpediente;
                    tIngreso = SelectIngreso;
                    SelectExpediente = null;
                    SelectIngreso = null;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    AnioBuscar = FolioBuscar = new Nullable<int>();
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    //SelectIngresoEnabled = false;
                    break;
                case "guardar_menu":
                    try
                    {
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un ingreso");
                            break;
                        }
                        if (!HasErrors())
                            if (Imputado != null)
                                if (!await updateImputado())
                                {
                                    //datos generales
                                    if (TabDatosGenerales && !PEditar)
                                        break;
                                    if (TabApodosAlias && (!PInsertar && !PEditar))
                                        break;
                                    if (TabFotosHuellas && (!PInsertar && !PEditar))
                                        break;
                                    if (TabMediaFiliacion && (!PInsertar && !PEditar))
                                        break;
                                    if (TabPandillas && (!PInsertar && !PEditar))
                                        break;
                                    if (TabSenasParticulares && (!PInsertar && !PEditar))
                                        break;

                                    //if (!TabSenasParticulares)
                                    //{
                                    //    (new Dialogos()).ConfirmacionDialogo("Error", "Al actualizar la información.");
                                    //    return;
                                    //}

                                    if (_GuardarSenasParticulares)
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "Faltan campos por llenar");
                                }
                                else
                                {
                                    new Dialogos().ConfirmacionDialogo("Éxito", "Información grabada exitosamente!");
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                            else
                                (new Dialogos()).ConfirmacionDialogo("Notificación", "Debe seleccionar un imputado. ");
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", string.Format("Requiere: {0}.", base.Error));
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
                    }
                    break;
                //BUSCAR RELACION INTERNO    
                case "buscar_relacion_interno":
                    //BuscarRelacionInterno();
                    RISeguirCargando = true;
                    RIPagina = 1;
                    ListBuscarRelacionInterno = new RangeEnabledObservableCollection<INGRESO>();
                    ListBuscarRelacionInterno.InsertRange(await SegmentarResultadoBusquedaRelacionInterno(RIPagina));
                    EmptyBuscarRelacionInternoVisible = ListBuscarRelacionInterno.Count > 0 ? false : true;
                    break;
                case "seleccionar_relacion_interno":
                    if (SelectBuscarRelacionInterno != null)
                        AgregarRelacionInterno();
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un interno.");
                    break;

                //PANDILLAS
                case "insertar_pandilla":
                    if (PInsertar)
                    {
                        SelectedImputadoPandilla = null;
                        setValidacionesPandilla();
                        AgregarPandilla();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "editar_pandilla":
                    if (PEditar)
                    {
                        EditarPandilla();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validacion", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "eliminar_pandilla":
                    if (PEditar)
                    {
                        this.EliminarPandilla();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "guardar_pandilla":
                    this.GuardarPandilla();
                    break;
                case "cancelar_pandilla":
                    this.LimpiarPandilla();
                    break;
                case "salir_menu":
                    Imputado = null;
                    PrincipalViewModel.SalirMenu();
                    break;
                //IMPRESION DE FICHA
                case "ficha_menu":
                    this.ImprimeFicha();
                    break;
                case "Open442":
                    if (PEditar)
                    {
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                        this.ShowIdentification();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    break;
                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    AnioBuscar = FolioBuscar = new Nullable<int>();
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
            }
        }
        
        private void CloseView(object obj)
        {
            PrincipalViewModel.SalirMenu();
        }
        
        private void ClickEnter(Object obj)
        {
            //SelectIngresoEnabled = false;
            buscarImputado(obj);
        }
        
        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
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
                                case "FolioBuscar":
                                    if (!string.IsNullOrWhiteSpace(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrWhiteSpace(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    break;
                            }
                        }
                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(1));
                if (ListExpediente.Count <= 0)
                {
                    SelectExpediente = null;
                    SelectIngreso = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    EmptyExpedienteVisible = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    return;
                }
                if (!AnioBuscar.HasValue && !FolioBuscar.HasValue)
                {
                    EmptyExpedienteVisible = !(ListExpediente.Count > 0);
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    return;
                }
                if (ListExpediente.Count == 1)
                {
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            (new Dialogos()).ConfirmacionDialogo("Ingreso no vigente.", "Debes seleccionar un ingreso vigente.");
                            return;
                        }
                    }
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                        ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.Value != GlobalVar.gCentro : false)
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        new Dialogos().ConfirmacionDialogo("Ingreso no vigente.", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString()
                            + "/" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                        return;
                    }
                    SelectExpediente = Imputado = ListExpediente[0];
                    SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    this.getDatosImputado();
                    ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                    ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                    ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                    ContenedorIdentificacionVisible = EditarImputado = true;
                    ApellidoPaternoBuscar = Imputado.PATERNO;
                    ApellidoMaternoBuscar = Imputado.MATERNO;
                    NombreBuscar = Imputado.NOMBRE;
                    AnioBuscar = Imputado.ID_ANIO;
                    FolioBuscar = Imputado.ID_IMPUTADO;
                    IngresoEnabled = TrasladoEnabled = CamposBusquedaEnabled = false;
                    if (PInsertar || PEditar)
                        MenuGuardarEnabled = true;
                    if (PConsultar)
                        MenuBuscarEnabled = /*MenuReporteEnabled =*/ MenuFichaEnabled = true;
                    ImputadoSeleccionadoAuxiliar = Imputado;
                    Opcion = 0;
                    BanderaEntrada = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    StaticSourcesViewModel.SourceChanged = false;
                    return;
                }
                else
                {
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                   
                 
                    //TabVisible = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }
        
        private void TiempoEstado()
        {
            try
            {
                AniosEstado = MesesEstado = string.Empty;
                if (FechaEstado != null)
                {
                    //int anios = 0, meses = 0, dias = 0;

                    var hoy = Fechas.GetFechaDateServer;


                    //anios = (hoy.Year - FechaEstado.Year);
                    //meses = (hoy.Month - FechaEstado.Month);
                    //dias = (hoy.Day - FechaEstado.Day);

                    //if (meses < 0)
                    //{
                    //    anios -= 1;
                    //    meses += 12;
                    //}
                    //if (dias < 0)
                    //{
                    //    meses -= 1;
                    //    dias += DateTime.DaysInMonth(hoy.Year, FechaEstado.Month);
                    //}

                    //if (anios < 0)
                    //{
                    //    System.Windows.MessageBox.Show("La fecha es inválida");
                    //}

                    //if (anios > 99)
                    //    anios = 99;
                    //if (meses > 99)
                    //    meses = 99;

                    //AniosEstado = string.Format("{0}", anios);
                    //MesesEstado = string.Format("{0}", meses);
                    int a, m, d = 0;
                    new Fechas().DiferenciaFechas(hoy, FechaEstado, out a, out  m, out d);
                    AniosEstado = string.Format("{0}", a);
                    MesesEstado = string.Format("{0}", m);

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular el tiempo en el estado.", ex);
            }
        }
        
        private void CalcularEdad()
        {
            DateTime hoy = Fechas.GetFechaDateServer;
            if (textFechaNacimiento.HasValue)
            {
                int edad = hoy.Year - textFechaNacimiento.Value.Year;
                if (hoy.Month < textFechaNacimiento.Value.Month || (hoy.Month == textFechaNacimiento.Value.Month && hoy.Day < textFechaNacimiento.Value.Day))
                    edad--;
                TextEdad = edad;
            }
        }
        
        private void LimpiarCampos()
        {
            if (IsSelectedIdentificacion == true)
            {
                if (TabDatosGenerales == true)
                    LimpiarCamposIdentificacion();

                if (TabSenasParticulares)
                    LimpiarCamposSeniasParticulares();
            }
        }
        
        private void LimpiarCamposIdentificacion()
        {
            try
            {
                #region DatosGenerales
                SelectEtnia = SelectEscolaridad = SelectOcupacion = SelectEstadoCivil = SelectNacionalidad = SelectReligion = -1;
                SelectSexo = "S";
                TextPeso = TextEstatura = string.Empty;
                #endregion

                #region Domicilio
                SelectPais = Parametro.PAIS; //82;
                SelectEntidad = Parametro.ESTADO;// 2;
                SelectMunicipio = -1;
                SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalle = TextNumeroInterior = AniosEstado = MesesEstado = TextDomicilioTrabajo = string.Empty;
                FechaEstado = Fechas.GetFechaDateServer;
                TextNumeroExterior = null;
                TextTelefono = null;
                TextCodigoPostal = null;
                #endregion

                #region Nacimiento
                SelectPaisNacimiento = Parametro.PAIS;//82;
                SelectEntidadNacimiento = SelectMunicipioNacimiento = -1;
                TextFechaNacimiento = Fechas.GetFechaDateServer;
                TextLugarNacimientoExtranjero = string.Empty;
                #endregion

                #region Padre
                TextPadreMaterno = TextPadrePaterno = TextPadreNombre = string.Empty;
                CheckPadreFinado = false;

                #region Domicilio
                SelectPaisDomicilioPadre = SelectEntidadDomicilioPadre = SelectMunicipioDomicilioPadre = -1;
                SelectColoniaItemDomicilioPadre = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalleDomicilioPadre = TextNumeroInteriorDomicilioPadre = string.Empty;
                TextNumeroExteriorDomicilioPadre = null;
                TextCodigoPostalDomicilioPadre = null;
                #endregion

                #endregion

                #region Madre
                TextMadreMaterno = TextMadrePaterno = TextMadreNombre = string.Empty;
                CheckMadreFinado = false;

                #region Domicilio
                SelectPaisDomicilioMadre = SelectEntidadDomicilioMadre = SelectMunicipioDomicilioMadre = -1;
                SelectColoniaItemDomicilioMadre = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextCalleDomicilioMadre = TextNumeroInteriorDomicilioMadre = string.Empty;
                TextNumeroExteriorDomicilioMadre = null;
                TextCodigoPostalDomicilioMadre = null;
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar limpiar los campos.", ex);
            }
        }
        
        private void LimpiarCamposSeniasParticulares()
        {
            try
            {
                SelectSenaParticular = null;
                SelectAnatomiaTopografica = null;
                SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == string.Empty).FirstOrDefault();
                SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == -1).FirstOrDefault();
                SelectPresentaIngresar = SelectPresentaIntramuros = SeniasParticularesEditable = false;
                ImagenTatuaje = null;
                TextCantidad = TextAmpliarDescripcion = string.Empty;
                TextCantidad = CodigoSenia = TextSignificado = TextAmpliarDescripcion = string.Empty;
                TextAmpliarDescripcion = string.Empty;
                var radioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((RegistroIngresoView)((ContentControl)Application.Current.Windows[0].FindName("contentControl")).Content)).ToList();
                foreach (var item in radioButons)
                {
                    item.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar campos.", ex);
            }
        }
        
        private void ChecarValidaciones()
        {
            if (base.HasErrors)
                ApodosAliasEnabled = DatosGeneralesEnabled = FotosHuellasEnabled = MediaFiliacionEnabled = PandillasEnabled = SenasParticularesEnabled = false; 
            else
                ApodosAliasEnabled = DatosGeneralesEnabled = FotosHuellasEnabled = MediaFiliacionEnabled = PandillasEnabled = SenasParticularesEnabled = true;
            IdentificacionEnabled = DatosGeneralesEnabled = true;
            IngresoEnabled = TrasladoEnabled = false;
        }

        #region OracleOperations
        private async void buscarImputado(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
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
                            case "FolioBuscar":
                                FolioBuscar = string.IsNullOrEmpty(textbox.Text) ? new Nullable<int>() : int.Parse(textbox.Text);
                                break;
                        }
                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente != null)
                    EmptyExpedienteVisible = ListExpediente.Count < 0;
                else
                    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
                return new List<IMPUTADO>();
            }
        }

        private Task<bool> getDatosGenerales()
        {
            try
            {
                ListEstadoCivil = ListEstadoCivil ?? new ObservableCollection<ESTADO_CIVIL>((new cEstadoCivil()).ObtenerTodos().OrderBy(o => o.DESCR));

                ListOcupacion = ListOcupacion ?? new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListEscolaridad = ListEscolaridad ?? new ObservableCollection<ESCOLARIDAD>((new cEscolaridad()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListReligion = ListReligion ?? new ObservableCollection<RELIGION>((new cReligion()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListEtnia = ListEtnia ?? new ObservableCollection<ETNIA>((new cEtnia()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListPaisNacionalidad = ListPaisNacionalidad ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerNacionalidad().OrderBy(o => o.NACIONALIDAD));
                ListPaisNacimiento = ListPaisNacimiento ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos().OrderBy(o => o.PAIS));
                LstIdioma = LstIdioma ?? new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos().OrderBy(o => o.DESCR));
                LstDialecto = LstDialecto ?? new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos().OrderBy(o => o.DESCR));

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                    SelectEstadoCivil = -1;
                    SelectSexo = "M";

                    ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                    SelectOcupacion = -1;

                    ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                    SelectEscolaridad = -1;

                    ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });
                    SelectReligion = -1;

                    ListEtnia.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
                    SelectEtnia = -1;

                    ListPaisNacionalidad.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });

                    ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });
                    ListPaisDomicilioMadre = ListPaisDomicilioPadre = ListPaisDomicilio = ListPaisNacimiento;
                    SelectPaisNacimiento = SelectPais = SelectNacionalidad = -1;
                    TextFechaNacimiento = null;

                    LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
                    SelectedIdioma = -1;

                    LstDialecto.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
                    SelectedDialecto = -1;
                }));

                //SelectPaisItem = new PAIS_NACIONALIDAD();

                StaticSourcesViewModel.SourceChanged = false;
                return TaskEx.FromResult(true);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales.", ex);
                return TaskEx.FromResult(false);
            }
        }
        
        private void GetMediaFiliacion()
        {
            try
            {
                if (Complexion == null || Complexion.Count < 1)
                {
                    var mediaFiliacion = new List<MEDIA_FILIACION>((new cMediaFiliacion()).ObtenerTodos());
                    if (mediaFiliacion.Count > 0)
                    {
                        #region SENIAS_GENERALES
                        var tipoFiliacion = new TIPO_FILIACION() { ID_TIPO_FILIACION = -1, DESCR = "SELECCIONE" };
                        Complexion = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 39)).FirstOrDefault().TIPO_FILIACION);
                        Complexion.Insert(0, tipoFiliacion);
                        ColorPiel = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 30)).FirstOrDefault().TIPO_FILIACION);
                        ColorPiel.Insert(0, tipoFiliacion);
                        Cara = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 7)).FirstOrDefault().TIPO_FILIACION);
                        Cara.Insert(0, tipoFiliacion);
                        #endregion
                        #region TIPO_SANGRE
                        TipoSangre = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 22)).FirstOrDefault().TIPO_FILIACION);
                        TipoSangre.Insert(0, tipoFiliacion);
                        FactorSangre = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 23)).FirstOrDefault().TIPO_FILIACION);
                        FactorSangre.Insert(0, tipoFiliacion);
                        #endregion
                        #region CABELLO
                        CantidadCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 8)).FirstOrDefault().TIPO_FILIACION);
                        CantidadCabello.Insert(0, tipoFiliacion);
                        ColorCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 9)).FirstOrDefault().TIPO_FILIACION);
                        ColorCabello.Insert(0, tipoFiliacion);
                        CalvicieCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 10)).FirstOrDefault().TIPO_FILIACION);
                        CalvicieCabello.Insert(0, tipoFiliacion);
                        FormaCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 11)).FirstOrDefault().TIPO_FILIACION);
                        FormaCabello.Insert(0, tipoFiliacion);
                        ImplantacionCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 31)).FirstOrDefault().TIPO_FILIACION);
                        ImplantacionCabello.Insert(0, tipoFiliacion);
                        #endregion
                        #region CEJA
                        DireccionCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 12)).FirstOrDefault().TIPO_FILIACION);
                        DireccionCeja.Insert(0, tipoFiliacion);
                        ImplantacionCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 13)).FirstOrDefault().TIPO_FILIACION);
                        ImplantacionCeja.Insert(0, tipoFiliacion);
                        FormaCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 14)).FirstOrDefault().TIPO_FILIACION);
                        FormaCeja.Insert(0, tipoFiliacion);
                        TamanioCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 15)).FirstOrDefault().TIPO_FILIACION);
                        TamanioCeja.Insert(0, tipoFiliacion);
                        #endregion
                        #region FRENTE
                        AlturaFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 27)).FirstOrDefault().TIPO_FILIACION);
                        AlturaFrente.Insert(0, tipoFiliacion);
                        InclinacionFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 28)).FirstOrDefault().TIPO_FILIACION);
                        InclinacionFrente.Insert(0, tipoFiliacion);
                        AnchoFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 29)).FirstOrDefault().TIPO_FILIACION);
                        AnchoFrente.Insert(0, tipoFiliacion);
                        #endregion
                        #region OJOS
                        ColorOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 16)).FirstOrDefault().TIPO_FILIACION);
                        ColorOjos.Insert(0, tipoFiliacion);
                        FormaOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 17)).FirstOrDefault().TIPO_FILIACION);
                        FormaOjos.Insert(0, tipoFiliacion);
                        TamanioOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 18)).FirstOrDefault().TIPO_FILIACION);
                        TamanioOjos.Insert(0, tipoFiliacion);
                        #endregion
                        #region NARIZ
                        RaizNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 1)).FirstOrDefault().TIPO_FILIACION);
                        RaizNariz.Insert(0, tipoFiliacion);
                        DorsoNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 3)).FirstOrDefault().TIPO_FILIACION);
                        DorsoNariz.Insert(0, tipoFiliacion);
                        AnchoNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 4)).FirstOrDefault().TIPO_FILIACION);
                        AnchoNariz.Insert(0, tipoFiliacion);
                        BaseNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 5)).FirstOrDefault().TIPO_FILIACION);
                        BaseNariz.Insert(0, tipoFiliacion);
                        AlturaNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 6)).FirstOrDefault().TIPO_FILIACION);
                        AlturaNariz.Insert(0, tipoFiliacion);
                        #endregion
                        #region LABIO
                        AlturaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 32)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        AlturaLabio.Insert(0, tipoFiliacion);
                        ProminenciaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 33)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        ProminenciaLabio.Insert(0, tipoFiliacion);
                        EspesorLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 21)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        EspesorLabio.Insert(0, tipoFiliacion);
                        #endregion
                        #region BOCA
                        TamanioBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 19)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        TamanioBoca.Insert(0, tipoFiliacion);
                        ComisuraBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 20)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        ComisuraBoca.Insert(0, tipoFiliacion);
                        #endregion
                        #region MENTON
                        TipoMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 24)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        TipoMenton.Insert(0, tipoFiliacion);
                        FormaMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 25)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        FormaMenton.Insert(0, tipoFiliacion);
                        InclinacionMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 26)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        InclinacionMenton.Insert(0, tipoFiliacion);
                        #endregion
                        #region OREJAS
                        FormaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 34)).FirstOrDefault().TIPO_FILIACION);
                        FormaOreja.Insert(0, tipoFiliacion);
                        HelixOriginalOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 40)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        HelixOriginalOreja.Insert(0, tipoFiliacion);
                        HelixSuperiorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 41)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        HelixSuperiorOreja.Insert(0, tipoFiliacion);
                        HelixPosteriorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 42)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        HelixPosteriorOreja.Insert(0, tipoFiliacion);
                        HelixAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 43)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        HelixAdherenciaOreja.Insert(0, tipoFiliacion);
                        LobuloContornoOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 44)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        LobuloContornoOreja.Insert(0, tipoFiliacion);
                        LobuloAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 45)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        LobuloAdherenciaOreja.Insert(0, tipoFiliacion);
                        LobuloParticularidadOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 46)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        LobuloParticularidadOreja.Insert(0, tipoFiliacion);
                        LobuloDimensionOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 47)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                        LobuloDimensionOreja.Insert(0, tipoFiliacion);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales.", ex);
            }
        }

        private async void getDatosImputado()
        {
            getDatosGeneralesIdentificacion();
            getDatosMediaFiliacion();
            ////getDatosSeniasParticulares();
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GetImputadoPandilla);
        }
        
        private void getDatosGeneralesIdentificacion()
        {
            try
            {
                //await TaskEx.Delay(200);
                #region IDENTIFICACION
                #region DatosGenerales
                SelectEtnia = Imputado.ID_ETNIA == null ? -1 : Imputado.ID_ETNIA;
                //SelectEscolaridad = Imputado.ID_ESCOLARIDAD == null ? -1 : Imputado.ID_ESCOLARIDAD;
                //SelectOcupacion = Imputado.ID_OCUPACION == null ? -1 : Imputado.ID_OCUPACION;
                //SelectEstadoCivil = Imputado.ID_ESTADO_CIVIL == null ? -1 : Imputado.ID_ESTADO_CIVIL;
                SelectEscolaridad = SelectIngreso.ID_ESCOLARIDAD == null ? -1 : SelectIngreso.ID_ESCOLARIDAD;
                SelectOcupacion = SelectIngreso.ID_OCUPACION == null ? -1 : SelectIngreso.ID_OCUPACION;
                SelectEstadoCivil = SelectIngreso.ID_ESTADO_CIVIL == null ? -1 : SelectIngreso.ID_ESTADO_CIVIL;
                SelectNacionalidad = Imputado.ID_NACIONALIDAD == null ? -1 : Imputado.ID_NACIONALIDAD;
                //SelectReligion = Imputado.ID_RELIGION == null ? -1 : Imputado.ID_RELIGION;
                SelectReligion = SelectIngreso.ID_RELIGION == null ? -1 : SelectIngreso.ID_RELIGION;
                SelectSexo = string.IsNullOrEmpty(Imputado.SEXO) ? "S" : Imputado.SEXO;
                SelectedDialecto = Imputado.ID_DIALECTO == null ? -1 : Imputado.ID_DIALECTO;
                SelectedIdioma = Imputado.ID_IDIOMA == null ? -1 : Imputado.ID_IDIOMA;
                RequiereTraductor = string.IsNullOrEmpty(Imputado.TRADUCTOR) ? false : Imputado.TRADUCTOR == "S" ? true : false;

                //TextPeso = Imputado.PESO.HasValue ? Imputado.PESO.ToString() : string.Empty;
                //TextEstatura = Imputado.ESTATURA.HasValue ? Imputado.ESTATURA.ToString() : string.Empty;
                TextPeso = SelectIngreso.PESO.HasValue ? SelectIngreso.PESO.ToString() : string.Empty;
                TextEstatura = SelectIngreso.ESTATURA.HasValue ? SelectIngreso.ESTATURA.ToString() : string.Empty;
                #endregion

                #region Domicilio
                //SelectPais = Imputado.ID_PAIS == null ? 82 : Imputado.ID_PAIS < 1 ? 82 : Imputado.ID_PAIS;
                //SelectEntidad = Imputado.ID_ENTIDAD == null ? 2 : Imputado.ID_ENTIDAD < 1 ? 2 : Imputado.ID_ENTIDAD;
                //SelectMunicipio = Imputado.ID_MUNICIPIO == null ? -1 : Imputado.ID_MUNICIPIO < 1 ? -1 : Imputado.ID_MUNICIPIO;
                //SelectColoniaItem = Imputado.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : Imputado.ID_COLONIA < 1 ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == Imputado.ID_COLONIA).FirstOrDefault();

                //TextCalle = !string.IsNullOrEmpty(Imputado.DOMICILIO_CALLE) ? Imputado.DOMICILIO_CALLE.Trim() : string.Empty;
                //TextNumeroExterior = Imputado.DOMICILIO_NUM_EXT;
                //TextNumeroInterior = !string.IsNullOrEmpty(Imputado.DOMICILIO_NUM_INT) ? Imputado.DOMICILIO_NUM_INT.Trim() : string.Empty;
                //AniosEstado = Imputado.RESIDENCIA_ANIOS.HasValue ? Imputado.RESIDENCIA_ANIOS.ToString() : string.Empty;
                //MesesEstado = Imputado.RESIDENCIA_MESES.HasValue ? Imputado.RESIDENCIA_MESES.ToString() : string.Empty;
                SelectPais = SelectIngreso.ID_PAIS == null ? 82 : SelectIngreso.ID_PAIS < 1 ? 82 : SelectIngreso.ID_PAIS;
                SelectEntidad = SelectIngreso.ID_ENTIDAD == null ? 2 : SelectIngreso.ID_ENTIDAD < 1 ? 2 : SelectIngreso.ID_ENTIDAD;
                SelectMunicipio = SelectIngreso.ID_MUNICIPIO == null ? -1 : SelectIngreso.ID_MUNICIPIO < 1 ? -1 : SelectIngreso.ID_MUNICIPIO;
                SelectColoniaItem = SelectIngreso.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : SelectIngreso.ID_COLONIA < 1 ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == SelectIngreso.ID_COLONIA).FirstOrDefault();

                TextCalle = !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty;
                TextNumeroExterior = SelectIngreso.DOMICILIO_NUM_EXT;
                TextNumeroInterior = !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_NUM_INT) ? SelectIngreso.DOMICILIO_NUM_INT.Trim() : string.Empty;
                AniosEstado = SelectIngreso.RESIDENCIA_ANIOS.HasValue ? SelectIngreso.RESIDENCIA_ANIOS.ToString() : string.Empty;
                MesesEstado = SelectIngreso.RESIDENCIAS_MESES.HasValue ? SelectIngreso.RESIDENCIAS_MESES.ToString() : string.Empty;
                int mes = 0, anio = 0;
                int.TryParse(MesesEstado, out mes);
                int.TryParse(AniosEstado, out anio);
                FechaEstado = Fechas.GetFechaDateServer.AddYears(-(anio)).AddMonths(-(mes));
                //TextTelefono = Imputado.TELEFONO.HasValue ? Imputado.TELEFONO.ToString() : null;
                //TextCodigoPostal = Imputado.DOMICILIO_CODIGO_POSTAL.HasValue ? Imputado.DOMICILIO_CODIGO_POSTAL.Value : new Nullable<int>();
                //TextDomicilioTrabajo = !string.IsNullOrEmpty(Imputado.DOMICILIO_TRABAJO) ? Imputado.DOMICILIO_TRABAJO.Trim() : string.Empty;
                TextTelefono = SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.ToString() : null;
                TextCodigoPostal = SelectIngreso.DOMICILIO_CP.HasValue ? SelectIngreso.DOMICILIO_CP.Value : new Nullable<int>();
                TextDomicilioTrabajo = !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_TRABAJO) ? SelectIngreso.DOMICILIO_TRABAJO.Trim() : string.Empty;
                #endregion

                #region Nacimiento
                SelectPaisNacimiento = Imputado.NACIMIENTO_PAIS == null ? 82 : Imputado.NACIMIENTO_PAIS < 1 ? 82 : Imputado.NACIMIENTO_PAIS;
                SelectEntidadNacimiento = Imputado.NACIMIENTO_ESTADO == null ? 2 : Imputado.NACIMIENTO_ESTADO < 1 ? 2 : Imputado.NACIMIENTO_ESTADO;
                SelectMunicipioNacimiento = Imputado.NACIMIENTO_MUNICIPIO == null ? -1 : Imputado.NACIMIENTO_MUNICIPIO < 1 ? -1 : Imputado.NACIMIENTO_MUNICIPIO;

                TextFechaNacimiento = Imputado.NACIMIENTO_FECHA == null ? Fechas.GetFechaDateServer.AddYears(-18) : Imputado.NACIMIENTO_FECHA;
                TextLugarNacimientoExtranjero = !string.IsNullOrEmpty(Imputado.NACIMIENTO_LUGAR) ? Imputado.NACIMIENTO_LUGAR.Trim() : string.Empty;
                #endregion

                #region Padres
                getDatosDomiciliosPadres();
                #endregion

                #region Nuevo
                if (SelectIngreso != null)
                {
                    SelectEstadoCivil = SelectIngreso.ID_ESTADO_CIVIL != null ? SelectIngreso.ID_ESTADO_CIVIL : -1;
                    SelectOcupacion = SelectIngreso.ID_OCUPACION != null ? SelectIngreso.ID_OCUPACION : -1;
                    SelectEscolaridad = SelectIngreso.ID_ESCOLARIDAD != null ? SelectIngreso.ID_ESCOLARIDAD : -1;
                    SelectReligion = SelectIngreso.ID_RELIGION != null ? SelectIngreso.ID_RELIGION : -1;

                    //Domicilio
                    TextCalle = SelectIngreso.DOMICILIO_CALLE;
                    TextNumeroExterior = SelectIngreso.DOMICILIO_NUM_EXT;
                    TextNumeroInterior = SelectIngreso.DOMICILIO_NUM_INT;

                    SelectPais = SelectIngreso.ID_PAIS == null ? 82 : SelectIngreso.ID_PAIS < 1 ? 82 : SelectIngreso.ID_PAIS;
                    SelectEntidad = SelectIngreso.ID_ENTIDAD == null ? 2 : SelectIngreso.ID_ENTIDAD < 1 ? 2 : SelectIngreso.ID_ENTIDAD;
                    SelectMunicipio = SelectIngreso.ID_MUNICIPIO == null ? -1 : SelectIngreso.ID_MUNICIPIO < 1 ? -1 : SelectIngreso.ID_MUNICIPIO;
                    SelectColoniaItem = SelectIngreso.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : SelectIngreso.ID_COLONIA < 1 ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == SelectIngreso.ID_COLONIA).FirstOrDefault();

                    TextCodigoPostal = SelectIngreso.DOMICILIO_CP;
                    //En el estado
                    AniosEstado = SelectIngreso.RESIDENCIA_ANIOS.ToString();
                    MesesEstado = SelectIngreso.RESIDENCIAS_MESES.ToString();

                    TextTelefono = SelectIngreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(SelectIngreso.TELEFONO.Value.ToString()) : string.Empty;
                    TextDomicilioTrabajo = SelectIngreso.DOMICILIO_TRABAJO = TextDomicilioTrabajo;

                    //Padres
                    CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S" ? true : false;
                    CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S" ? true : false;
                }
                #endregion

                #endregion
                setValidacionesIdentificacionDatosGenerales();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }
        
        private void getDatosDomiciliosPadres()
        {
            try
            {
                #region Padre
                TextPadreMaterno = !string.IsNullOrEmpty(Imputado.MATERNO_PADRE) ? Imputado.MATERNO_PADRE.Trim() : string.Empty;
                TextPadrePaterno = !string.IsNullOrEmpty(Imputado.PATERNO_PADRE) ? Imputado.PATERNO_PADRE.Trim() : string.Empty;
                TextPadreNombre = !string.IsNullOrEmpty(Imputado.NOMBRE_PADRE) ? Imputado.NOMBRE_PADRE.Trim() : string.Empty;
                //CheckPadreFinado = Imputado.PADRE_FINADO == "S";
                CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S";
                #endregion

                #region Madre
                TextMadreMaterno = !string.IsNullOrEmpty(Imputado.MATERNO_MADRE) ? Imputado.MATERNO_MADRE.Trim() : string.Empty;
                TextMadrePaterno = !string.IsNullOrEmpty(Imputado.PATERNO_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty;
                TextMadreNombre = !string.IsNullOrEmpty(Imputado.NOMBRE_MADRE) ? Imputado.NOMBRE_MADRE.Trim() : string.Empty;
                //CheckMadreFinado = Imputado.MADRE_FINADO == "S";
                CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S";
                #endregion

                #region Padres
                if (Imputado.IMPUTADO_PADRES.Count > 0)
                {
                    bool padreExiste = false, madreExiste = false;
                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "P")
                        {
                            SelectPaisDomicilioPadre = item.ID_PAIS == null ? 82 : item.ID_PAIS < 1 ? 82 : item.ID_PAIS;
                            SelectEntidadDomicilioPadre = item.ID_ENTIDAD == null ? 2 : item.ID_ENTIDAD < 1 ? 2 : item.ID_ENTIDAD;
                            SelectMunicipioDomicilioPadre = item.ID_MUNICIPIO == null ? -1 : item.ID_MUNICIPIO < 1 ? -1 : item.ID_MUNICIPIO;
                            SelectColoniaDomicilioPadre = item.ID_COLONIA == null ? -1 : item.ID_COLONIA < 1 ? -1 : item.ID_COLONIA;
                            TextCalleDomicilioPadre = !string.IsNullOrEmpty(item.CALLE) ? item.CALLE.Trim() : string.Empty;
                            TextNumeroExteriorDomicilioPadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioPadre = !string.IsNullOrEmpty(item.NUM_INT) ? item.NUM_INT.Trim() : string.Empty;
                            TextCodigoPostalDomicilioPadre = item.CP;
                            padreExiste = true;
                        }
                    }

                    if (!padreExiste)
                    {
                        if (!string.IsNullOrEmpty(TextPadreMaterno) || !string.IsNullOrEmpty(TextPadrePaterno) || !string.IsNullOrEmpty(TextPadreNombre))
                        {
                            if (SelectIngreso.PADRE_FINADO == "N")//SI NO ESTA FINADO TIENEN EL MISMO DOMICILIO
                            {
                                MismoDomicilioPadre = true;
                            }
                        }
                    }


                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "M")
                        {
                            SelectPaisDomicilioMadre = item.ID_PAIS == null ? 82 : item.ID_PAIS < 1 ? 82 : item.ID_PAIS;
                            SelectEntidadDomicilioMadre = item.ID_ENTIDAD == null ? 2 : item.ID_ENTIDAD < 1 ? 2 : item.ID_ENTIDAD;
                            SelectMunicipioDomicilioMadre = item.ID_MUNICIPIO == null ? -1 : item.ID_MUNICIPIO < 1 ? -1 : item.ID_MUNICIPIO;
                            SelectColoniaDomicilioMadre = item.ID_COLONIA == null ? -1 : item.ID_COLONIA < 1 ? -1 : item.ID_COLONIA;
                            TextCalleDomicilioMadre = !string.IsNullOrEmpty(item.CALLE) ? item.CALLE.Trim() : string.Empty;
                            TextNumeroExteriorDomicilioMadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioMadre = !string.IsNullOrEmpty(item.NUM_INT) ? item.NUM_INT.Trim() : string.Empty;
                            TextCodigoPostalDomicilioMadre = item.CP;
                            madreExiste = true;
                        }
                    }

                    if (!madreExiste)
                    {
                        if (!string.IsNullOrEmpty(TextMadreMaterno) || !string.IsNullOrEmpty(TextMadrePaterno) || !string.IsNullOrEmpty(TextMadreNombre))
                        {
                            if (SelectIngreso.MADRE_FINADO == "N")//SI NO ESTA FINADO TIENEN EL MISMO DOMICILIO
                            {
                                MismoDomicilioMadre = true;
                            }
                        }
                    }
                }
                else
                {
                    SelectPaisDomicilioPadre = Parametro.PAIS; //82;
                    TextCalleDomicilioPadre = TextNumeroInteriorDomicilioPadre = string.Empty;
                    TextNumeroExteriorDomicilioPadre = null;
                    TextCodigoPostalDomicilioPadre = null;
                    if (!string.IsNullOrEmpty(TextPadreMaterno) || !string.IsNullOrEmpty(TextPadrePaterno) || !string.IsNullOrEmpty(TextPadreNombre))
                    {
                        if (SelectIngreso.PADRE_FINADO == "N")//SI NO ESTA FINADO TIENEN EL MISMO DOMICILIO
                        {
                            MismoDomicilioPadre = true;
                        }
                    }

                    SelectPaisDomicilioMadre = Parametro.PAIS; //82;
                    TextCalleDomicilioMadre = TextNumeroInteriorDomicilioMadre = string.Empty;
                    TextNumeroExteriorDomicilioMadre = null;
                    TextCodigoPostalDomicilioMadre = null;
                    if (!string.IsNullOrEmpty(TextMadreMaterno) || !string.IsNullOrEmpty(TextMadrePaterno) || !string.IsNullOrEmpty(TextMadreNombre))
                    {
                        if (SelectIngreso.MADRE_FINADO == "N")//SI NO ESTA FINADO TIENEN EL MISMO DOMICILIO
                        {
                            MismoDomicilioMadre = true;
                        }
                    }
                }
                #endregion

                setValidacionesIdentificacionDatosGenerales();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }
        
        private void getDatosMediaFiliacion()
        {
            try
            {
                #region MEDIA_FILIACION

                var imputadoFiliacion = Imputado.IMPUTADO_FILIACION; //(new cImputadoFiliacion()).ObtenerTodos(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO).ToList();
                //await TaskEx.Delay(250);
                if (imputadoFiliacion.Count > 0)
                {
                    #region SENIAS_GENERALES
                    SelectComplexion = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 39).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 39).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectColorPiel = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 30).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 30).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectCara = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 7).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 7).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region TIPO_SANGRE
                    SelectTipoSangre = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 22).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 22).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectFactorSangre = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 23).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 23).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region CABELLO
                    SelectCantidadCabello = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 8).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 8).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectColorCabello = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 9).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 9).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectCalvicieCabello = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 10).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 10).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectFormaCabello = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 11).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 11).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectImplantacionCabello = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 31).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 31).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region CEJA
                    SelectDireccionCeja = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 12).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 12).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectImplantacionCeja = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 13).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 13).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectFormaCeja = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 14).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 14).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectTamanioCeja = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 15).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 15).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region FRENTE
                    SelectAlturaFrente = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 27).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 27).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectInclinacionFrente = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 28).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 28).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectAnchoFrente = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 29).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 29).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region OJOS
                    SelectColorOjos = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 16).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 16).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectFormaOjos = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 17).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 17).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectTamanioOjos = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 18).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 18).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region NARIZ
                    SelectRaizNariz = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 1).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 1).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectDorsoNariz = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 3).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 3).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectAnchoNariz = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 4).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 4).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectBaseNariz = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 5).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 5).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectAlturaNariz = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 6).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 6).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region LABIO
                    SelectAlturaLabio = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 32).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 32).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectProminenciaLabio = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 33).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 33).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectEspesorLabio = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 21).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 21).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region BOCA
                    SelectTamanioBoca = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 19).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 19).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectComisuraBoca = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 20).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 20).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region MENTON
                    SelectTipoMenton = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 24).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 24).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectFormaMenton = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 25).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 25).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectInclinacionMenton = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 26).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 26).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                    #region OREJAS
                    SelectFormaOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 34).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 34).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectHelixOriginalOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 40).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 40).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectHelixSuperiorOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 41).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 41).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectHelixPosteriorOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 42).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 42).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectHelixAdherenciaOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 43).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 43).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectLobuloContornoOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 44).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 44).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectLobuloAdherenciaOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 45).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 45).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectLobuloParticularidadOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 46).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 46).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    SelectLobuloDimensionOrejaDerecha = imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 47).Any() ? imputadoFiliacion.Where(w => w.ID_MEDIA_FILIACION == 47).FirstOrDefault().ID_TIPO_FILIACION : (short)-1;
                    #endregion
                }
                else
                {
                    #region SENIAS_GENERALES
                    SelectComplexion = SelectColorPiel = SelectCara = -1;
                    #endregion
                    #region TIPO_SANGRE
                    SelectTipoSangre = SelectFactorSangre = -1;
                    #endregion
                    #region CABELLO
                    SelectCantidadCabello = SelectColorCabello = SelectCalvicieCabello = SelectFormaCabello = SelectImplantacionCabello = -1;
                    #endregion
                    #region CEJA
                    SelectDireccionCeja = SelectImplantacionCeja = SelectFormaCeja = SelectTamanioCeja = -1;
                    #endregion
                    #region FRENTE
                    SelectAlturaFrente = SelectInclinacionFrente = SelectAnchoFrente = -1;
                    #endregion
                    #region OJOS
                    SelectColorOjos = SelectFormaOjos = SelectTamanioOjos = -1;
                    #endregion
                    #region NARIZ
                    SelectRaizNariz = SelectDorsoNariz = SelectAnchoNariz = SelectBaseNariz = SelectAlturaNariz = -1;
                    #endregion
                    #region LABIO
                    SelectAlturaLabio = SelectProminenciaLabio = SelectEspesorLabio = -1;
                    #endregion
                    #region BOCA
                    SelectTamanioBoca = SelectComisuraBoca = -1;
                    #endregion
                    #region MENTON
                    SelectTipoMenton = SelectFormaMenton = SelectInclinacionMenton = -1;
                    #endregion
                    #region OREJAS
                    SelectFormaOrejaDerecha = SelectHelixOriginalOrejaDerecha = SelectHelixSuperiorOrejaDerecha = SelectHelixPosteriorOrejaDerecha =
                        SelectHelixAdherenciaOrejaDerecha = SelectLobuloContornoOrejaDerecha = SelectLobuloAdherenciaOrejaDerecha =
                            SelectLobuloParticularidadOrejaDerecha = SelectLobuloDimensionOrejaDerecha = -1;
                    #endregion
                }
                #endregion
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
            }
        }
        
        private async Task<bool> updateImputado()
        {
            if (TabPandillas)
            {
                if (pInsertar || PEditar)
                {
                    if (GuardarPandillaDB())
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");

            }
            else if (TabMediaFiliacion)
            {
                if (pInsertar || PEditar)
                {
                    if (await GuardarImputadoFiliacion())
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
            }
            else if (TabApodosAlias)
            {
                if (PInsertar || PEditar)
                {
                    if (GuardarApodosAlias())
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
            }
            else if (TabFotosHuellas)
            {
                if (PInsertar || PEditar)
                {
                    if (GuardarFotosYHuellas())
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");

            }
            else if (TabSenasParticulares)
            {
                if (PInsertar || PEditar)
                {
                    _GuardarSenasParticulares = GuardarSenasParticulares();
                    if (_GuardarSenasParticulares)
                    {
                        SeniasParticularesEditable = false;
                        return true;
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
            }
            else if (TabDatosGenerales)
            {
                if (pEditar)
                {
                    //if (await GuardarDatosGenerales())
                    if (await GuardarDatosGenerales())
                        return true;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
            }

            return false;

        }
        
        private async Task<bool> GuardarDatosGenerales()
        {
           
                    try {
                        IMPUTADO imputado = new IMPUTADO();
                        imputado.ID_CENTRO = Imputado.ID_CENTRO;
                        imputado.ID_ANIO = Imputado.ID_ANIO;
                        imputado.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        imputado.NIP = Imputado.NIP;
                        imputado.PATERNO = Imputado.PATERNO;
                        imputado.MATERNO = Imputado.MATERNO;
                        imputado.NOMBRE = Imputado.NOMBRE;

                        #region DatosGenerales
                        imputado.SEXO = SelectSexo;
                        //imputado.ID_ESTADO_CIVIL = SelectEstadoCivil;
                        //imputado.ID_OCUPACION = SelectOcupacion;
                        //imputado.ID_ESCOLARIDAD = SelectEscolaridad;
                        //imputado.ID_RELIGION = SelectReligion;
                        imputado.ID_ETNIA = SelectEtnia;
                        imputado.ID_NACIONALIDAD = SelectNacionalidad;
                        //imputado.ESTATURA = !string.IsNullOrEmpty(TextEstatura) ? short.Parse(TextEstatura) : new short?();
                        //imputado.PESO = !string.IsNullOrEmpty(TextPeso) ? short.Parse(TextPeso) : new short?();
                        imputado.ID_IDIOMA = SelectedIdioma;
                        imputado.ID_DIALECTO = SelectedDialecto;
                        imputado.TRADUCTOR = RequiereTraductor ? "S" : "N";

                        #endregion

                        #region Domicilio
                        //imputado.DOMICILIO_CALLE = TextCalle;
                        //imputado.DOMICILIO_NUM_INT = TextNumeroInterior;
                        //imputado.DOMICILIO_NUM_EXT = TextNumeroExterior;
                        //imputado.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
                        //imputado.ID_MUNICIPIO = SelectMunicipio;
                        //imputado.ID_ENTIDAD = SelectEntidad;
                        //imputado.ID_PAIS = SelectPais;
                        //imputado.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                        //if (!string.IsNullOrEmpty(TextTelefono))
                        //    imputado.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                        //imputado.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                        //imputado.RESIDENCIA_ANIOS = string.IsNullOrEmpty(AniosEstado) ? new Nullable<short>() : short.Parse(AniosEstado);
                        //imputado.RESIDENCIA_MESES = string.IsNullOrEmpty(MesesEstado) ? new Nullable<short>() : short.Parse(MesesEstado);
                        #endregion

                        #region Nacimiento
                        imputado.NACIMIENTO_PAIS = SelectPaisNacimiento;
                        imputado.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                        imputado.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                        imputado.NACIMIENTO_FECHA = TextFechaNacimiento;
                        imputado.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                        #endregion

                        #region Padres
                        imputado.NOMBRE_PADRE = TextPadreNombre;
                        imputado.MATERNO_PADRE = TextPadreMaterno;
                        imputado.PATERNO_PADRE = TextPadrePaterno;
                        imputado.NOMBRE_MADRE = TextMadreNombre;
                        imputado.MATERNO_MADRE = TextMadreMaterno;
                        imputado.PATERNO_MADRE = TextMadrePaterno;
                        //imputado.PADRE_FINADO = CheckPadreFinado ? "S" : "N";
                        //imputado.MADRE_FINADO = CheckMadreFinado ? "S" : "N";
                        #endregion

                        #region Domicilio Padres
                        var padres = new List<IMPUTADO_PADRES>();
                        if (!MismoDomicilioPadre && !CheckPadreFinado)
                        {
                            padres.Add(new IMPUTADO_PADRES()
                            {
                                ID_PADRE = "P",
                                ID_CENTRO = Imputado.ID_CENTRO,
                                ID_ANIO = Imputado.ID_ANIO,
                                ID_IMPUTADO = Imputado.ID_IMPUTADO,
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
                        if (!MismoDomicilioMadre && !CheckMadreFinado)
                        {
                            padres.Add(new IMPUTADO_PADRES()
                            {
                                ID_PADRE = "M",
                                ID_CENTRO = Imputado.ID_CENTRO,
                                ID_ANIO = Imputado.ID_ANIO,
                                ID_IMPUTADO = Imputado.ID_IMPUTADO,
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

                        #region Nuevo
                        var ingreso = new INGRESO();
                        ingreso.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        ingreso.ID_ANIO = SelectIngreso.ID_ANIO;
                        ingreso.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        ingreso.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        
                        ingreso.ID_ESTADO_CIVIL = SelectEstadoCivil;
                        ingreso.ID_OCUPACION = SelectOcupacion;
                        ingreso.ID_ESCOLARIDAD = SelectEscolaridad;
                        ingreso.ID_RELIGION = SelectReligion;
                        //Domicilio
                        ingreso.DOMICILIO_CALLE = TextCalle;
                        ingreso.DOMICILIO_NUM_EXT = TextNumeroExterior;
                        ingreso.DOMICILIO_NUM_INT = TextNumeroInterior;
                        ingreso.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
                        ingreso.ID_MUNICIPIO = SelectMunicipio;
                        ingreso.ID_ENTIDAD = SelectEntidad;
                        ingreso.ID_PAIS = SelectPais;
                        ingreso.DOMICILIO_CP = TextCodigoPostal;
                        //En el estado
                        if (!string.IsNullOrEmpty(AniosEstado))
                            ingreso.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                        if (!string.IsNullOrEmpty(MesesEstado))
                            ingreso.RESIDENCIAS_MESES = short.Parse(MesesEstado);

                        ingreso.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                        ingreso.DOMICILIO_TRABAJO = TextDomicilioTrabajo;

                        //Padres
                        if (CheckPadreFinado)
                            ingreso.PADRE_FINADO = "S";
                        else
                            ingreso.PADRE_FINADO = "N";
                        if (CheckMadreFinado)
                            ingreso.MADRE_FINADO = "S";
                        else
                            ingreso.MADRE_FINADO = "N";

                        if(!string.IsNullOrEmpty(TextEstatura))
                            ingreso.ESTATURA = short.Parse(TextEstatura);
                        if(!string.IsNullOrEmpty(TextPeso))
                            ingreso.PESO = short.Parse(TextPeso);
                        #endregion

                        if ((new cImputado()).ActualizarImputadoDatosGeneralesyPadres(imputado, padres,ingreso))
                            return true;
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del imputado seleccionado.", ex);
                        return false;
                    }

            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al guardar los datos del imputado seleccionado.", ex);
            //}

            //new Dialogos().ConfirmacionDialogo("Error", "Al actualizar la información.");
            //return false;
        }
        
        private bool GuardarDomiciliosPadres()
        {
            try
            {
                var padres = new List<IMPUTADO_PADRES>();
                if (!MismoDomicilioPadre && !CheckPadreFinado)
                {
                    padres.Add(new IMPUTADO_PADRES()
                    {
                        ID_PADRE = "P",
                        ID_CENTRO = Imputado.ID_CENTRO,
                        ID_ANIO = Imputado.ID_ANIO,
                        ID_IMPUTADO = Imputado.ID_IMPUTADO,
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
                if (!MismoDomicilioMadre && !CheckMadreFinado)
                {
                    padres.Add(new IMPUTADO_PADRES()
                    {
                        ID_PADRE = "M",
                        ID_CENTRO = Imputado.ID_CENTRO,
                        ID_ANIO = Imputado.ID_ANIO,
                        ID_IMPUTADO = Imputado.ID_IMPUTADO,
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
                if (padres.Count > 0)
                {
                    if ((new cImputadoPadres()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, padres))
                    {
                        SelectExpediente.IMPUTADO_PADRES = padres;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos de los padres del imputado seleccionado.", ex);
            }
            return false;
        }
        
        private bool GuardarFotosYHuellas()
        {
            try
            {
                var guardafotos = false;
                var guardahuellas = false;
                if (ImagesToSave.Count != 3)
                    return false;
                else if (ImagesToSave.Where(w => w.FrameName == "LeftFace" && w.ImageCaptured != null).Count() == 0)
                    return false;
                else if (ImagesToSave.Where(w => w.FrameName == "RightFace" && w.ImageCaptured != null).Count() == 0)
                    return false;
                else if (ImagesToSave.Where(w => w.FrameName == "FrontFace" && w.ImageCaptured != null).Count() == 0)
                    return false;
                else
                {
                    List<int> LToma = new List<int>();

                    #region [Fotos]
                    var ingresoBiometrico = new List<SSP.Servidor.INGRESO_BIOMETRICO>();
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

                        ingresoBiometrico.Add(new SSP.Servidor.INGRESO_BIOMETRICO()
                        {
                            BIOMETRICO = bit,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO : (short)0,
                            ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                        });
                    }
                    if (new cIngresoBiometrico().GetData().Where(w => w.ID_INGRESO == SelectIngreso.ID_INGRESO && w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO).ToList().Count() == 3)
                    {
                        #region [Actualizar Fotos]
                        guardafotos = (new cIngresoBiometrico()).Actualizar(ingresoBiometrico);
                        #endregion
                    }
                    else
                    {
                        #region [Intertar Fotos]
                        guardafotos = (new cIngresoBiometrico()).Eliminar(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO, SelectIngreso.ID_INGRESO, false);
                        guardafotos = (new cIngresoBiometrico()).Insertar(ingresoBiometrico);
                        #endregion
                    }
                    if (guardafotos)
                    {
                        FotosHuellasChange = false;
                        SelectIngreso.INGRESO_BIOMETRICO = ingresoBiometrico;
                    }
                    #endregion

                    #region [Huellas]
                    string toma = "N";
                    var imputadoBiometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();
                    var VerificarHuella = new List<ComparationRequest>();
                    if (HuellasCapturadas != null)
                    {
                        foreach (var item in HuellasCapturadas)
                        {
                            if ((item.ID_TIPO_FORMATO == BiometricoServiceReference.enumTipoFormato.FMTO_DP || item.ID_TIPO_FORMATO == BiometricoServiceReference.enumTipoFormato.FMTO_WSQ) && item.BIOMETRICO.Length != 0)
                                VerificarHuella.Add(new ComparationRequest { ID_TIPO_BIOMETRICO = item.ID_TIPO_BIOMETRICO, BIOMETRICO = item.BIOMETRICO, ID_TIPO_FORMATO = item.ID_TIPO_FORMATO });
                            {
                                toma = "N";
                                switch ((short)item.ID_TIPO_BIOMETRICO)
                                {
                                    //Mano Derecha
                                    case (short)enumTipoBiometrico.PULGAR_DERECHO:
                                        toma = TPulgarD ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.INDICE_DERECHO:
                                        toma = TIndiceD ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.MEDIO_DERECHO:
                                        toma = TMedioD ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.ANULAR_DERECHO:
                                        toma = TAnularD ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.MENIQUE_DERECHO:
                                        toma = TMeniqueD ? "S" : "N";
                                        break;
                                    //Mano Izquierda
                                    case (short)enumTipoBiometrico.PULGAR_IZQUIERDO:
                                        toma = TPulgarI ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.INDICE_IZQUIERDO:
                                        toma = TIndiceI ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.MEDIO_IZQUIERDO:
                                        toma = TMedioI ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.ANULAR_IZQUIERDO:
                                        toma = TAnularI ? "S" : "N";
                                        break;
                                    case (short)enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                        toma = TMeniqueI ? "S" : "N";
                                        break;
                                }
                                imputadoBiometrico.Add(new SSP.Servidor.IMPUTADO_BIOMETRICO()
                                {
                                    ID_ANIO = Imputado.ID_ANIO,
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                    BIOMETRICO = item.BIOMETRICO,
                                    ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
                                    ID_FORMATO = (short)item.ID_TIPO_FORMATO,
                                    CALIDAD = item.CALIDAD.HasValue ? item.CALIDAD : null,
                                    TOMA = toma,
                                });
                            }
                        }
                    }
                    else 
                    {
                        //Mano Derecha
                        if (TPulgarD)
                            LToma.Add((short)enumTipoBiometrico.PULGAR_DERECHO);
                        if (TIndiceD)
                            LToma.Add((short)enumTipoBiometrico.INDICE_DERECHO);
                        if (TMedioD)
                            LToma.Add((short)enumTipoBiometrico.MEDIO_DERECHO);
                        if (TAnularD)
                            LToma.Add((short)enumTipoBiometrico.ANULAR_DERECHO);
                        if (TMeniqueD)
                            LToma.Add((short)enumTipoBiometrico.MENIQUE_DERECHO);
                        //Mano Izquierda
                        if (TPulgarI)
                            LToma.Add((short)enumTipoBiometrico.PULGAR_IZQUIERDO);
                        if (TIndiceI)
                            LToma.Add((short)enumTipoBiometrico.INDICE_IZQUIERDO);
                        if (TMedioI)
                            LToma.Add((short)enumTipoBiometrico.MEDIO_IZQUIERDO);
                        if (TAnularI)
                            LToma.Add((short)enumTipoBiometrico.ANULAR_IZQUIERDO);
                        if (TMeniqueI)
                            LToma.Add((short)enumTipoBiometrico.MENIQUE_IZQUIERDO);
                    }
                    //var ExistenHuellas = new BiometricoServiceClient().ExistenHuellasImputado( VerificarHuella != null ? VerificarHuella.ToArray() : null);

                    if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && (w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP || w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)).ToList().Count() == 20)
                    {
                        #region [actualizar biometrico]
                        HuellasCapturadas = null;
                        if (imputadoBiometrico.Any())
                            guardahuellas = (new cImputadoBiometrico()).Actualizar(imputadoBiometrico);
                        else
                            guardahuellas = new cImputadoBiometrico().ActualizarToma(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, LToma);
                            //guardahuellas = true;
                        #endregion
                    }
                    else
                    {
                        #region [insertar Huellas]
                        HuellasCapturadas = null;
                        guardahuellas = (new cImputadoBiometrico()).Eliminar(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO);
                        if (imputadoBiometrico.Any())
                            guardahuellas = (new cImputadoBiometrico()).Insertar(imputadoBiometrico);
                        else
                            guardahuellas = true;
                        #endregion
                    }
                    if (guardahuellas)
                    {
                        Imputado.IMPUTADO_BIOMETRICO = imputadoBiometrico;
                    }
                    #endregion
                }
                if (guardahuellas && guardafotos)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar las fotos y huellas del imputado.", ex);
                return false;
            }
        }
        
        private bool GuardarApodosAlias()
        {
            try
            {
                #region Apodos
                var apodos = new List<APODO>();
                if (ListApodo.Count > 0)
                {
                    short id_apodo = 1;
                    foreach (var entidad in ListApodo)
                    {
                        apodos.Add(new APODO
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_APODO = id_apodo,
                            APODO1 = entidad.APODO1
                        });
                        id_apodo++;
                    }
                }
                #endregion

                #region Apodos
                var alias = new List<ALIAS>();
                if (ListAlias.Count > 0)
                {
                    short id_alias = 1;
                    foreach (var entidad in ListAlias)
                    {
                        alias.Add(new ALIAS
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_ALIAS = id_alias,
                            PATERNO = entidad.PATERNO,
                            MATERNO = entidad.MATERNO,
                            NOMBRE = entidad.NOMBRE
                        });
                        id_alias++;
                    }
                }
                #endregion

                #region Relaciones Personales
                var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
                if (ListRelacionPersonalInterno.Count > 0)
                {
                    foreach (var entidad in ListRelacionPersonalInterno)
                    {
                        relacion_personal_internos.Add(new RELACION_PERSONAL_INTERNO
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            NOTA = entidad.NOTA,
                            ID_REL_ANIO = entidad.INGRESO.ID_ANIO,
                            ID_REL_CENTRO = entidad.INGRESO.ID_CENTRO,
                            ID_REL_IMPUTADO = entidad.INGRESO.ID_IMPUTADO,
                            ID_REL_INGRESO = entidad.INGRESO.ID_INGRESO
                        });
                    }
                }
                #endregion

                if (new cImputado().ActualizarImputadoAliasApodosRelaciones(SelectExpediente, alias, apodos, relacion_personal_internos))
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar alias, apodos y relación del interno.", ex);
            }
           
                //if (!saveApodos())
            //    return false;
            //if (!saveAlias())
            //    return false;
            //if (!saveRelacionesPersonales())
            //    return false;
            return false;
        }
        
        private bool GuardarSenasParticulares()
        {
            try
            {
                var presenta = SelectPresentaIngresar ? "N" : "S";
                if (SelectPresentaIngresar == false && SelectPresentaIntramuros == false)
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar si es al ingresar o fue intramuros.");
                    return false;
                }
                if (SelectAnatomiaTopografica == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes de seleccionar una región del cuerpo humano.");
                    return false;
                }
                if (SelectTipoSenia <= 0)
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar el tipo de seña.");
                    return false;
                }
                if (SelectTipoSenia == 2 && (SelectTatuaje == null || SelectTatuaje.ID_TATUAJE <= 0))
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar el tipo de tatuaje.");
                    return false;
                }
                /*else if (SelectTipoSenia == 2 && (SelectClasificacionTatuaje == null || SelectClasificacionTatuaje.ID_TATUAJE_CLA == ""))
                    return false;*/
                if (string.IsNullOrEmpty(CodigoSenia))
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "En el código.");
                    return false;
                }
                if (string.IsNullOrEmpty(TextSignificado))
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "En el significado.");
                    return false;
                }
                if (string.IsNullOrEmpty(TextCantidad))
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes ingresar la cantidad.");
                    FocusCantidad = true;
                    return false;
                }
                if (ImagenTatuaje == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Error", "Debes tomar la foto de la seña.");
                    return false;
                }

                int cant = 0;
                Int32.TryParse(TextCantidad, out cant);
                short? tipoTatuaje = null;
                string clasificacion = null;
                if (SelectTipoSenia == 2)
                {
                    tipoTatuaje = SelectTatuaje.ID_TATUAJE;
                    clasificacion = SelectClasificacionTatuaje == null ? string.Empty : SelectClasificacionTatuaje.ID_TATUAJE_CLA;
                }
                if (SeniasParticularesEditable)
                {
                    if (!PEditar)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(TextCantidad))
                    {
                        var id = SelectSenaParticular.ID_SENA;
                        ListSenasParticulares.Remove(SelectSenaParticular);
                        ListSenasParticulares.Add(new SENAS_PARTICULARES()
                        {
                            CANTIDAD = (short)cant,
                            CLASIFICACION = clasificacion,
                            CODIGO = CodigoSenia,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_REGION = SelectAnatomiaTopografica.ID_REGION,
                            ID_SENA = id,
                            ID_TIPO_TATUAJE = tipoTatuaje,
                            IMAGEN = new Imagenes().ConvertBitmapToByte(ImagenTatuaje),
                            INTRAMUROS = presenta,
                            SIGNIFICADO = TextSignificado,
                            TIPO = (short)SelectTipoSenia
                        });
                    }
                }
                else
                {
                    if (!PInsertar)
                    {
                        return false;
                    }
                    if (!string.IsNullOrEmpty(TextCantidad))
                    {
                        ListSenasParticulares.Add(new SENAS_PARTICULARES()
                        {
                            CANTIDAD = (short)cant,
                            CLASIFICACION = clasificacion,
                            CODIGO = CodigoSenia,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_REGION = SelectAnatomiaTopografica.ID_REGION,
                            ID_SENA = 0,
                            ID_TIPO_TATUAJE = tipoTatuaje,
                            IMAGEN = new Imagenes().ConvertBitmapToByte(ImagenTatuaje),
                            INTRAMUROS = presenta,
                            SIGNIFICADO = TextSignificado,
                            TIPO = (short)SelectTipoSenia
                        });
                    }

                }
                int i = 0;
                var listaAuxiliar = new List<SENAS_PARTICULARES>();
                foreach (var item in ListSenasParticulares.OrderBy(o => o.ID_SENA))
                {
                    listaAuxiliar.Add(new SENAS_PARTICULARES
                    {
                        CANTIDAD = item.CANTIDAD,
                        CLASIFICACION = clasificacion,
                        CODIGO = item.CODIGO,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        ID_REGION = item.ID_REGION,
                        ID_SENA = item.ID_SENA,
                        ID_TIPO_TATUAJE = item.ID_TIPO_TATUAJE,
                        IMAGEN = item.IMAGEN,
                        INTRAMUROS = item.INTRAMUROS,
                        SIGNIFICADO = item.SIGNIFICADO,
                        TIPO = item.TIPO
                    });
                }
                if ((new cSenasParticulares()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, listaAuxiliar.ToList()))
                {
                    

                    /*Imputado.SENAS_PARTICULARES = new List<SENAS_PARTICULARES>( await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SENAS_PARTICULARES>>(() =>
                        new cSenasParticulares().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO)));*/
                    //Imputado.SENAS_PARTICULARES = null;
                    //Imputado.SENAS_PARTICULARES = new cSenasParticulares().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                    //var y = new cSenasParticulares().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                    //ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>();
                    ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>(new cSenasParticulares().Obtener(Imputado.ID_CENTRO,Imputado.ID_ANIO,Imputado.ID_IMPUTADO));
                    LimpiarCamposSeniasParticulares();
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al guardar la seña particular.", ex);
            }
            (new Dialogos()).ConfirmacionDialogo("Error", "Al actualizar la información.");
            return false;
        }
        
        private async Task<bool> GuardarImputadoFiliacion()
        {
            try
            {
                var imputadoFiliacion = new List<IMPUTADO_FILIACION>();
                #region SENIAS_GENERALES
                imputadoFiliacion.Add(new IMPUTADO_FILIACION()
                {
                    ID_CENTRO = Imputado.ID_CENTRO,
                    ID_ANIO = Imputado.ID_ANIO,
                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
                    ID_MEDIA_FILIACION = 39,
                    ID_TIPO_FILIACION = SelectComplexion
                });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 30, ID_TIPO_FILIACION = SelectColorPiel, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 7, ID_TIPO_FILIACION = SelectCara, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region TIPO_SANGRE
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 22, ID_TIPO_FILIACION = SelectTipoSangre, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 23, ID_TIPO_FILIACION = SelectFactorSangre, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region CABELLO
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 8, ID_TIPO_FILIACION = SelectCantidadCabello, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 9, ID_TIPO_FILIACION = SelectColorCabello, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 10, ID_TIPO_FILIACION = SelectCalvicieCabello, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 11, ID_TIPO_FILIACION = SelectFormaCabello, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 31, ID_TIPO_FILIACION = SelectImplantacionCabello, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region CEJA
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 12, ID_TIPO_FILIACION = SelectDireccionCeja, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 13, ID_TIPO_FILIACION = SelectImplantacionCeja, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 14, ID_TIPO_FILIACION = SelectFormaCeja, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 15, ID_TIPO_FILIACION = SelectTamanioCeja, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region FRENTE
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 27, ID_TIPO_FILIACION = SelectAlturaFrente, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 28, ID_TIPO_FILIACION = SelectInclinacionFrente, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 29, ID_TIPO_FILIACION = SelectAnchoFrente, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region OJOS
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 16, ID_TIPO_FILIACION = SelectColorOjos, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 17, ID_TIPO_FILIACION = SelectFormaOjos, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 18, ID_TIPO_FILIACION = SelectTamanioOjos, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region NARIZ
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 1, ID_TIPO_FILIACION = SelectRaizNariz, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 3, ID_TIPO_FILIACION = SelectDorsoNariz, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 4, ID_TIPO_FILIACION = SelectAnchoNariz, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 5, ID_TIPO_FILIACION = SelectBaseNariz, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 6, ID_TIPO_FILIACION = SelectAlturaNariz, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region LABIO
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 32, ID_TIPO_FILIACION = SelectAlturaLabio, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 33, ID_TIPO_FILIACION = SelectProminenciaLabio, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 21, ID_TIPO_FILIACION = SelectEspesorLabio, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region BOCA
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 19, ID_TIPO_FILIACION = SelectTamanioBoca, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 20, ID_TIPO_FILIACION = SelectComisuraBoca, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region MENTON
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 24, ID_TIPO_FILIACION = SelectTipoMenton, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 25, ID_TIPO_FILIACION = SelectFormaMenton, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 26, ID_TIPO_FILIACION = SelectInclinacionMenton, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion
                #region OREJAS
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 34, ID_TIPO_FILIACION = SelectFormaOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 40, ID_TIPO_FILIACION = SelectHelixOriginalOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 41, ID_TIPO_FILIACION = SelectHelixSuperiorOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 42, ID_TIPO_FILIACION = SelectHelixPosteriorOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 43, ID_TIPO_FILIACION = SelectHelixAdherenciaOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 44, ID_TIPO_FILIACION = SelectLobuloContornoOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 45, ID_TIPO_FILIACION = SelectLobuloAdherenciaOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 46, ID_TIPO_FILIACION = SelectLobuloParticularidadOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 47, ID_TIPO_FILIACION = SelectLobuloDimensionOrejaDerecha, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
                #endregion

                if (imputado.IMPUTADO_FILIACION.Count == 0)//Es insert
                {
                    if (!PInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                        return false;
                    }
                }
                else//Es update
                {
                    if (!PEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                        return false;
                    }
                }

                if ((new cImputadoFiliacion()).InsertarFiliacion(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, imputadoFiliacion))
                {
                    //Imputado.IMPUTADO_FILIACION = await StaticSourcesViewModel.CargarDatosAsync<List<IMPUTADO_FILIACION>>(() =>
                    //    new cImputadoFiliacion().ObtenerTodos(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del imputado seleccionado.", ex);
            }
            return false;
        }
        
        private bool saveApodos()
        {
            try
            {
                var apodos = new List<APODO>();
                if (ListApodo.Count > 0)
                {
                    short id_apodo = 1;
                    foreach (var entidad in ListApodo)
                    {
                        //id_apodo = (new cApodo()).GetSequence<short>("APODOS_SEQ");
                        apodos.Add(new APODO
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_APODO = id_apodo,
                            APODO1 = entidad.APODO1
                        });
                        id_apodo++;
                    }
                }
                if ((new cApodo()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, apodos))
                {
                    AliasApodoChange = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del imputado seleccionado.", ex);
            }
            return false;
        }
        
        private bool saveAlias()
        {
            try
            {
                var alias = new List<ALIAS>();
                if (ListAlias.Count > 0)
                {
                    short id_alias = 1;
                    foreach (var entidad in ListAlias)
                    {
                        //id_alias = (new cAlias()).GetSequence<short>("ALIAS_SEQ");
                        alias.Add(new ALIAS
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_ALIAS = id_alias,
                            PATERNO = entidad.PATERNO,
                            MATERNO = entidad.MATERNO,
                            NOMBRE = entidad.NOMBRE
                        });
                        id_alias++;
                    }
                }
                if ((new cAlias()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, alias))
                {
                    AliasApodoChange = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del imputado seleccionado.", ex);
            }
            return false;
        }
        
        private bool saveRelacionesPersonales()
        {
            try
            {
                var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
                if (ListRelacionPersonalInterno.Count > 0)
                {
                    foreach (var entidad in ListRelacionPersonalInterno)
                    {
                        relacion_personal_internos.Add(new RELACION_PERSONAL_INTERNO
                        {
                            ID_CENTRO = Imputado.ID_CENTRO,
                            ID_ANIO = Imputado.ID_ANIO,
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            NOTA = entidad.NOTA,
                            ID_REL_ANIO = entidad.INGRESO.ID_ANIO,
                            ID_REL_CENTRO = entidad.INGRESO.ID_CENTRO,
                            ID_REL_IMPUTADO = entidad.INGRESO.ID_IMPUTADO,
                            ID_REL_INGRESO = entidad.INGRESO.ID_INGRESO
                        });
                    }
                }
                if ((new cRelacionPersonalInterno()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, relacion_personal_internos))
                {
                    AliasApodoChange = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar los datos del imputado seleccionado.", ex);
            }
            return false;
        }
        #endregion

        #region [ABC ALIAS]
        private void GuardarAlias(Object obj = null)
        {
            try
            {
                if (obj != null)
                    NombreAlias = ((TextBox)obj).Text;
                if (!string.IsNullOrEmpty(NombreAlias) && (!string.IsNullOrEmpty(PaternoAlias) || !string.IsNullOrEmpty(MaternoAlias)))
                {
                        if (OpcionGuardarAlias == 1)
                        {
                            if (ListAlias.Count(w => w.PATERNO.Trim() == PaternoAlias.Trim() && w.MATERNO.Trim() == MaternoAlias.Trim() && w.NOMBRE.Trim() == NombreAlias.Trim()) == 0)
                                ListAlias.Add(new ALIAS { PATERNO = PaternoAlias, MATERNO = MaternoAlias, NOMBRE = NombreAlias });
                            else
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El alias ya se ecuentra registrado");    
                                return;
                            }
                        }
                        else
                        {
                            if (SelectAlias != null)
                            {
                                SelectAlias.PATERNO = PaternoAlias;
                                SelectAlias.MATERNO = MaternoAlias;
                                SelectAlias.NOMBRE = NombreAlias;
                                ListAlias = new ObservableCollection<ALIAS>(ListAlias);
                            }
                        }
                        AliasApodoChange = true;
                        PaternoAlias = MaternoAlias = NombreAlias = string.Empty;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al agregar el nuevo alias.", ex);
            }
        }

        private void EliminarAlias()
        {
            if (SelectAlias != null)
            {
                ListAlias.Remove(SelectAlias);
                AliasApodoChange = true;
            }
            SelectAlias = null;
        }
        #endregion

        #region [ABC APODO]
        private void GuardarApodo(Object obj = null)
        {
            try
            {
                if (obj != null)
                    Apodo = ((TextBox)obj).Text;
                if (!string.IsNullOrEmpty(Apodo))
                {
                    if (OpcionGuardarApodo == 1)
                    {
                        if (ListApodo.Count(w => w.APODO1.Trim() == Apodo) == 0)
                            ListApodo.Add(new APODO { APODO1 = Apodo });
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El apodo ya se ecuentra registrado");
                            return;
                        }
                    }
                    else
                    {
                        if (SelectApodo != null)
                        {
                            SelectApodo.APODO1 = Apodo;
                            ListApodo = new ObservableCollection<APODO>(ListApodo);
                        }
                    }
                    AliasApodoChange = true;
                    Apodo = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al agregar un nuevo apodo.", ex);
            }
        }

        private void EliminarApodo()
        {
            if (SelectApodo != null)
            {
                ListApodo.Remove(SelectApodo);
                AliasApodoChange = true;
            }
            SelectApodo = null;
        }
        #endregion

        #region [ABC RELACION INTERNO]
        private async Task<List<INGRESO>> SegmentarResultadoBusquedaRelacionInterno(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(PaternoBuscarRelacionInterno) && string.IsNullOrEmpty(MaternoBuscarRelacionInterno) && string.IsNullOrEmpty(NombreBuscarRelacionInterno))
                return new List<INGRESO>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() => new ObservableCollection<INGRESO>(new cIngreso().ObtenerTodos(GlobalVar.gCentro, NombreBuscarRelacionInterno, PaternoBuscarRelacionInterno, MaternoBuscarRelacionInterno, true, _Pag)));
            if (result.Any())
            {
                RIPagina++;
                RISeguirCargando = true;
            }
            else
                RISeguirCargando = false;

            return result.ToList();
        }

        private async void BuscarRelacionInterno(Object obj = null)
        {
            try
            {
                if (obj != null)
                {
                    switch (((TextBox)obj).Name)
                    {
                        case "tbPaternoBuscarRelacionInterno":
                            PaternoBuscarRelacionInterno = ((TextBox)obj).Text;
                            break;
                        case "tbMaternoBuscarRelacionInterno":
                            MaternoBuscarRelacionInterno = ((TextBox)obj).Text;
                            break;
                        case "tbNombreBuscarRelacionInterno":
                            NombreBuscarRelacionInterno = ((TextBox)obj).Text;
                            break;
                    }
                }
                RISeguirCargando = true;
                RIPagina = 1;
                ListBuscarRelacionInterno = new RangeEnabledObservableCollection<INGRESO>();
                ListBuscarRelacionInterno.InsertRange(await SegmentarResultadoBusquedaRelacionInterno(RIPagina));
                EmptyBuscarRelacionInternoVisible = ListBuscarRelacionInterno.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al realizar la busqueda de imputados.", ex);
            }
        }
        private void AgregarRelacionInterno()
        {
            try
            {
                if (SelectBuscarRelacionInterno != null)
                {
                    if (ListRelacionPersonalInterno == null)
                        ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();

                    if (ListRelacionPersonalInterno.Count(w => w.INGRESO.ID_CENTRO == SelectBuscarRelacionInterno.ID_CENTRO && w.INGRESO.ID_ANIO == SelectBuscarRelacionInterno.ID_ANIO && w.INGRESO.ID_IMPUTADO == SelectBuscarRelacionInterno.ID_IMPUTADO) == 0)
                    {
                        ListRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO { INGRESO = SelectBuscarRelacionInterno });
                        //AliasApodoChange = true;
                        LimpiarBusquedaRelacionInterna();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado ya se encuentra registrado");
                    
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al agregar una nueva relaccion con un interno.", ex);
            }
        }
        private void LimpiarBusquedaRelacionInterna()
        {
            NombreBuscarRelacionInterno = PaternoBuscarRelacionInterno = MaternoBuscarRelacionInterno = string.Empty;
            SelectBuscarRelacionInterno = null;
            ListBuscarRelacionInterno = null;
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
        }
        private void CancelarBuscarRelacionInterno()
        {
            LimpiarBusquedaRelacionInterna();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
            base.ClearRules();
        }
        private void GuardarRelacionInterno(Object obj = null)
        {
            try
            {
                if (obj != null)
                    NotaRelacionInterno = ((TextBox)obj).Text;
                if (!string.IsNullOrEmpty(NombreRelacionInterno) && (!string.IsNullOrEmpty(PaternoRelacionInterno) || !string.IsNullOrEmpty(MaternoRelacionInterno)))
                {
                    if (OpcionGuardarRelacionInterno == 1)
                    {
                        //listRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO { PATERNO = paternoRelacionInterno, MATERNO = MaternoRelacionInterno, NOMBRE = nombreRelacionInterno, NOTA = NotaRelacionInterno }); 
                    }
                    else
                    {
                        if (selectRelacionPersonalInterno != null)
                        {
                            listRelacionPersonalInterno.RemoveAt(IndexRelacionInterno);
                            //listRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO { PATERNO = paternoRelacionInterno, MATERNO = MaternoRelacionInterno, NOMBRE = nombreRelacionInterno, NOTA = NotaRelacionInterno });
                            selectRelacionPersonalInterno = null;
                        }
                    }
                    AliasApodoChange = true;
                    PaternoRelacionInterno = MaternoRelacionInterno = NombreRelacionInterno = NotaRelacionInterno = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al guardar los datos del imputado seleccionado.", ex);
            }
        }
        private void EliminarRelacionInterno()
        {
            if (selectRelacionPersonalInterno != null)
            {
                listRelacionPersonalInterno.Remove(selectRelacionPersonalInterno);
                AliasApodoChange = true;
            }
            selectRelacionPersonalInterno = null;
        }
        #endregion

        #region [WebCam]
        private void OnUnLoad(RegistroIngresoView Window = null)
        {
            try
            {
                Window.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de registro ingreso", ex);
            }
        }
        private async void OnLoad(FotosHuellasDigitalesView Window = null)
        {
            try
            {
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

                #region [Web Cam]
                Window.Unloaded += async (s, e) =>
                {
                    try
                    {
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
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
                    var ingresobiometrico = new cIngreso().GetData().Where(wi => wi.INGRESO_BIOMETRICO.Any(w => w.ID_ANIO == Imputado.ID_ANIO && w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO && w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO)).ToList();
                    ImagesToSave = new List<ImageSourceToSave>();
                    if (ingresobiometrico.Any())
                    {
                        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.FrontFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.LeftFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                        ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.RightFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para capturar fotos y huellas.", ex);
            }
        }
        private void OnTakePicture(System.Windows.Controls.Image Picture)
        {
            try
            {
                if (!CamaraWeb.isVideoSourceInitialized)
                    return;
                if (Processing)
                    return;
                Processing = true;

                ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                {
                    Picture.Source = CamaraWeb.TomarFoto(Picture);
                    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                    StaticSourcesViewModel.Mensaje(Picture.Name == "LeftFace" ? "LADO IZQUIERDO" : Picture.Name == "RightFace" ? "LADO DERECHO" : Picture.Name == "FrontFace" ? "CENTRO" : Picture.Name == "ImgSenaParticular" ? "Tipografía Humana" : "Camara", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                }
                else
                {
                    CamaraWeb.QuitarFoto(Picture);
                    ImagesToSave.RemoveAll(w => w.FrameName == Picture.Name);
                    //ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                }
                if (ImagesToSave != null ? ImagesToSave.Count == 1 : false)
                    BotonTomarFotoEnabled = true;
                else
                    BotonTomarFotoEnabled = false;
                Processing = false;
                StaticSourcesViewModel.SourceChanged = true;
                //if (!PEditar && !PInsertar)
                //{
                //    new Dialogos().ConfirmacionDialogo("Validacion", "Su usuario no cuenta con permisos para realizar esta acción.");
                //    return;
                //}
                //if (Processing)
                //    return;
                //Processing = true;

                //ImagesToSave = ImagesToSave ?? new List<ImageSourceToSave>();
                //if (CamaraWeb.ImageControls.Where(w => w.Name == Picture.Name).Any())
                //{
                //    if (!PInsertar)
                //    {
                //        new Dialogos().ConfirmacionDialogo("Validacion", "Su usuario no cuenta con permisos para realizar esta acción.");
                //        return;
                //    }
                //    Picture.Source = CamaraWeb.TomarFoto(Picture);
                //    ImagesToSave.Add(new ImageSourceToSave { FrameName = Picture.Name, ImageCaptured = (BitmapSource)Picture.Source });
                //    StaticSourcesViewModel.SourceChanged = true;
                //    StaticSourcesViewModel.Mensaje(Picture.Name == "LeftFace" ? "LADO IZQUIERDO" : Picture.Name == "RightFace" ? "LADO DERECHO" : Picture.Name == "FrontFace" ? "CENTRO" : Picture.Name == "ImgSenaParticular" ? "Tipografía Humana" : "Camara", "Foto Capturada", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 1);
                //    FotosHuellasChange = true;
                //}
                //else
                //{
                //    if (!PEditar)
                //    {
                //        new Dialogos().ConfirmacionDialogo("Validacion", "Su usuario no cuenta con permisos para realizar esta acción.");
                //        return;
                //    }
                //    CamaraWeb.QuitarFoto(Picture);
                //    ImagesToSave.Remove(ImagesToSave.Where(wm => wm.FrameName == Picture.Name).SingleOrDefault());
                //}
                //if (ImagesToSave != null ? ImagesToSave.Count == 1 : false)
                //    BotonTomarFotoEnabled = true;
                //else
                //    BotonTomarFotoEnabled = false;
                //Processing = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al tomar la foto.", ex);
                Processing = false;
            }
        }
        private void OpenSetting(string obj)
        {
            CamaraWeb.AdvanceSetting();
        }
        #endregion

        #region [Huellas Digitales]
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

                                if ((new cImputadoBiometrico().ObtenerHuellas(imputado.ID_ANIO, imputado.ID_CENTRO, imputado.ID_IMPUTADO)).Any())
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
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error, revise que el escanner este bien configurado.", ex);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });
            AceptarBusquedaHuellaFocus = true;
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
        private void CargarHuellas()
        {
            try
            {
                if (imputado == null)
                    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && (w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP)).ToList();

                    foreach (var item in Huellas)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                               {
                                   switch ((enumTipoBiometrico)item.ID_TIPO_BIOMETRICO)
                                   {
                                       case enumTipoBiometrico.PULGAR_DERECHO:
                                           PulgarDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TPulgarD = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.INDICE_DERECHO:
                                           IndiceDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TIndiceD = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.MEDIO_DERECHO:
                                           MedioDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TMedioD = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.ANULAR_DERECHO:
                                           AnularDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TAnularD = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.MENIQUE_DERECHO:
                                           MeñiqueDerecho = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TMeniqueD = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.PULGAR_IZQUIERDO:
                                           PulgarIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TPulgarI = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.INDICE_IZQUIERDO:
                                           IndiceIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TIndiceI = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.MEDIO_IZQUIERDO:
                                           MedioIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TMedioI = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.ANULAR_IZQUIERDO:
                                           AnularIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TAnularI = item.TOMA == "S" ? true : false;
                                           break;
                                       case enumTipoBiometrico.MENIQUE_IZQUIERDO:
                                           MeñiqueIzquierdo = ObtenerCalidad(item.CALIDAD.HasValue ? (int)item.CALIDAD.Value : 0);
                                           TMeniqueI = item.TOMA == "S" ? true : false;
                                           break;
                                       default:
                                           break;
                                   }
                               }));
                    }
                });

                LoadHuellas.Start();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar las huellas del imputado seleccionado.", ex);
            }
        }
        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

                await TaskEx.Delay(400);

                var nRet = -1;
                var bandera = true;
                var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaJuridico;
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

                        Imputado = ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro != null ? ((BusquedaHuellaViewModel)windowBusqueda.DataContext).SelectRegistro.Imputado : null;
                        if (Imputado == null)
                            return;

                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        clickSwitch("buscar_visible");
                        SelectExpediente = Imputado;
                        Opcion = 0;
                        BanderaEntrada = true;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                    }
                };
                windowBusqueda.ShowDialog();
                AceptarBusquedaHuellaFocus = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar imputado por huellas.", ex);
            }
        }
        #endregion

        #region [PANDILLA]
        private void GetPandilla()
        {
            try
            {
                if (Pandilla == null || Pandilla.Count < 1)
                {
                    Pandilla = new ObservableCollection<PANDILLA>((new cPandilla()).ObtenerTodos());
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al obtener el listado de pandillas.", ex);
            }
        }
        
        private void GetImputadoPandilla()
        {
            try
            {
                ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>(new cImputadoPandilla().Obtener(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO));
                 Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar los datos de las pandillas del imputado.", ex);
            }
        }
        
        private void AgregarPandilla()
        {
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PANDILLA);
            setValidacionesIdentificacionPandillas();
            SelectedPandillaValue = -1;
        }
        
        private void LimpiarPandilla()
        {
            SelectedPandillaValue = -1;
            NotaPandilla = string.Empty;
            SelectedImputadoPandilla = null;
            base.ClearRules();
            HasErrors();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PANDILLA);
        }
        
        private void GuardarPandilla(Object nota = null)
        {
            try
            {
                if (nota != null)
                    NotaPandilla = ((System.Windows.Controls.TextBox)(nota)).Text;
                if (!base.HasErrors)
                {
                    if (ImputadoPandilla == null)
                        ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>();
                    if (SelectedImputadoPandilla == null)//INSERT
                    {
                        //ImputadoPandilla.Remove(SelectedImputadoPandilla);
                        if (ImputadoPandilla.Count(w => w.ID_PANDILLA == SelectedPandillaItem.ID_PANDILLA) > 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación","La pandilla y se encuentra registrada");
                            return;
                        }
                        else
                            ImputadoPandilla.Add(new IMPUTADO_PANDILLA { ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO, PANDILLA = SelectedPandillaItem, NOTAS = NotaPandilla, ID_PANDILLA = SelectedPandillaValue });
                    }
                    else//UPDATE
                    {
                        SelectedImputadoPandilla.ID_ANIO = Imputado.ID_ANIO;
                        SelectedImputadoPandilla.ID_CENTRO = Imputado.ID_CENTRO;
                        SelectedImputadoPandilla.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        SelectedImputadoPandilla.PANDILLA = SelectedPandillaItem;
                        SelectedImputadoPandilla.NOTAS = NotaPandilla;
                        SelectedImputadoPandilla.ID_PANDILLA = SelectedPandillaValue;
                        ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>(ImputadoPandilla);
                    }
                    PandillaChange = true;
                    LimpiarPandilla();
                    base.ClearRules();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al agregar una nueva pandilla del imputado.", ex);
            }
        }
        
        private void EliminarPandilla()
        {
            if (SelectedImputadoPandilla != null)
            {
                ImputadoPandilla.Remove(SelectedImputadoPandilla);
                PandillaChange = true;
            }
        }
        
        private void EditarPandilla()
        {
            if (SelectedImputadoPandilla != null)
            {
                SelectedPandillaValue = SelectedImputadoPandilla.ID_PANDILLA;
                NotaPandilla = SelectedImputadoPandilla.NOTAS;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PANDILLA);
            }
        }
        
        private bool GuardarPandillaDB()
        {
            try
            {
                if (ImputadoPandilla != null)
                {
                    //if (ImputadoPandilla.Count > 0)
                    //{
                        var pandillaList = new List<IMPUTADO_PANDILLA>();
                        foreach (var entidad in ImputadoPandilla)
                        {
                            pandillaList.Add(new IMPUTADO_PANDILLA { ID_CENTRO = entidad.ID_CENTRO, ID_ANIO = entidad.ID_ANIO, ID_IMPUTADO = entidad.ID_IMPUTADO, ID_PANDILLA = entidad.ID_PANDILLA, NOTAS = entidad.NOTAS });
                        }
                        if ((new cImputadoPandilla()).InsertarPandillas(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, pandillaList))
                            return true;
                    //}
                    //else
                    //    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al guardar los datos del imputado seleccionado.", ex);
            }
            return false;
        }
        #endregion

        #region [SENAS_PARTICULARES]
        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
                ImagesToSave = new List<ImageSourceToSave>();
                if (ImagenTatuaje != null)
                    CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, ImagenTatuaje);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla para tomar foto.", ex);
            }
        }
        private void GetRegionesCuerpo()
        {
            try
            {
                if (ListTipoTatuaje == null || ListTipoTatuaje.Count < 1)
                {
                    ListTipoTatuaje = new ObservableCollection<TATUAJE>((new cTatuaje()).ObtenerTodos());
                    ListTipoTatuaje.Insert(0, new TATUAJE() { ID_TATUAJE = -1, DESCR = "SELECCIONE" });
                }
                if (ListClasificacionTatuaje == null || ListClasificacionTatuaje.Count < 1)
                {
                    ListClasificacionTatuaje = new ObservableCollection<TATUAJE_CLASIFICACION>((new cTatuajeClasificacion()).ObtenerTodos());
                    ListClasificacionTatuaje.Insert(0, new TATUAJE_CLASIFICACION() { ID_TATUAJE_CLA = "", DESCR = "SELECCIONE" });
                }
                if (ListRegionCuerpo == null || ListRegionCuerpo.Count < 1)
                {
                    ListRegionCuerpo = new ObservableCollection<ANATOMIA_TOPOGRAFICA>((new cAnatomiaTopografica()).ObtenerTodos());
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos de las regiones del cuerpo.", ex);
            }
        }
        #endregion

        #region [TAB_LOADS]
        private async void IngresoLoad(RegistroIngresoView Window = null)
        {
            try
            {
                Window.Unloaded += (s, e) => { OnUnLoad(Window); };
                //PopUpsViewModels.MainWindow.ApodoView.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        ApodosLoad(PopUpsViewModels.MainWindow.ApodoView);
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar apodos", ex);
                //    }
                //};
                //PopUpsViewModels.MainWindow.AliasView.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        AliasLoad(PopUpsViewModels.MainWindow.AliasView);
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar alias", ex);
                //    }
                //};
                //PopUpsViewModels.MainWindow.RelacionView.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        RelacionLoad(PopUpsViewModels.MainWindow.RelacionView);
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar relacion", ex);
                //    }
                //};
                //PopUpsViewModels.MainWindow.PandillaView.IsVisibleChanged += (s, e) =>
                //{
                //    try
                //    {
                //        PandillaLoad(PopUpsViewModels.MainWindow.PandillaView);
                //        if (!((bool)e.NewValue))
                //            base.ClearRules();
                //    }
                //    catch (Exception ex)
                //    {
                //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar pandillas", ex);
                //    }
                //};
                
                //await StaticSourcesViewModel.CargarDatosMetodoAsync(async () => { await getDatosGenerales(); });
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la pantalla.", ex);
            }
        }
        
        private void DatosIdentificacionLoad(DatosGeneralesIdentificacionView Window = null)
        {
            if (IsSelectedIdentificacion)
            {
                setValidacionesIdentificacionDatosGenerales();
                //CargarPropiedades();
                //ChecarValidaciones();
            }
        }
        
        private void CargarPropiedades()
        {

            OnPropertyChanged("TextCalleDomicilioMadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioMadre");
            OnPropertyChanged("TextCodigoPostalDomicilioMadre");
            OnPropertyChanged("TextMadrePaterno");
            OnPropertyChanged("TextMadreMaterno");
            OnPropertyChanged("TextMadreNombre");
            OnPropertyChanged("TextCalleDomicilioPadre");
            OnPropertyChanged("TextNumeroExteriorDomicilioPadre");
            OnPropertyChanged("TextCodigoPostalDomicilioPadre");
            OnPropertyChanged("TextPadrePaterno");
            OnPropertyChanged("TextPadreMaterno");
            OnPropertyChanged("TextPadreNombre");
            OnPropertyChanged("SelectSexo");
            OnPropertyChanged("SelectEstadoCivil");
            OnPropertyChanged("SelectOcupacion");
            OnPropertyChanged("SelectEscolaridad");
            OnPropertyChanged("SelectNacionalidad");
            OnPropertyChanged("SelectReligion");
            OnPropertyChanged("SelectEtnia");
            OnPropertyChanged("TextPeso");
            OnPropertyChanged("TextEstatura");
            OnPropertyChanged("SelectedIdioma");
            OnPropertyChanged("SelectedDialecto");
            OnPropertyChanged("TextCalle");
            OnPropertyChanged("TextNumeroExterior");
            OnPropertyChanged("AniosEstado");
            OnPropertyChanged("MesesEstado");
            OnPropertyChanged("TextTelefono");
            OnPropertyChanged("TextCodigoPostal");
            OnPropertyChanged("TextFechaNacimiento");

            OnPropertyChanged("SelectPaisNacimiento");
            OnPropertyChanged("SelectEntidadNacimiento");
            OnPropertyChanged("SelectMunicipioNacimiento");

            OnPropertyChanged("SelectPaisDomicilioPadre");
            OnPropertyChanged("SelectEntidadDomicilioPadre");
            OnPropertyChanged("SelectMunicipioDomicilioPadre");
            OnPropertyChanged("SelectColoniaDomicilioPadre");

            OnPropertyChanged("SelectPaisDomicilioMadre");
            OnPropertyChanged("SelectEntidadDomicilioMadre");
            OnPropertyChanged("SelectMunicipioDomicilioMadre");
            OnPropertyChanged("SelectColoniaDomicilioMadre");

            OnPropertyChanged("SelectPais");
            OnPropertyChanged("SelectEntidad");
            OnPropertyChanged("SelectMunicipio");
            OnPropertyChanged("SelectColonia");
        }
        
        private async void MediaFiliacionLoad(MediaFiliacionView Window = null)
        {
            if (IsSelectedIdentificacion && TabMediaFiliacion)
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GetMediaFiliacion);
                setValidacionesIdentificacionMediaFiliacion();
                ChecarValidaciones();
                OnPropertyChanged();
            }
        }
        
        private async void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            try
            {
                if (TabSenasParticulares)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(GetRegionesCuerpo);
                    //llenar lista
                }
                if (Window != null && TabSenasParticulares && TabFrente)
                {

                    ListRadioButons = new List<RadioButton>();
                    ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))).ToList();
                    if (SelectSenaParticular != null && RegionCodigo.Length > 0)
                    {
                        foreach (var item in ListRadioButons)
                        {
                            if (item.CommandParameter != null)
                            {
                                if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                    item.IsChecked = true;
                                else
                                    item.IsChecked = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las señas particulares.", ex);
            }
        }
        
        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            try
            {
                if (Window != null && TabSenasParticulares && TabDorso)
                {
                    ListRadioButons = new List<RadioButton>();
                    ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))).ToList();
                    //CamaraWeb = null;
                    if (SelectSenaParticular != null && RegionCodigo.Length > 0)
                    {
                        foreach (var item in ListRadioButons)
                        {
                            if (item.CommandParameter != null)
                            {
                                if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3]))
                                {
                                    item.IsChecked = true;
                                }
                                else
                                {
                                    item.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos iniciales de las señas particulares.", ex);
            }
        }
        
        private void ApodosLoad(AgregarApodoView Window = null)
        {
            setValidacionesIdentificacionApodos();
            OnPropertyChanged();
        }
        
        private void AliasLoad(AgregarAliasView Window = null)
        {
            setValidacionesIdentificacionAlias();
            OnPropertyChanged();
        }
        
        private void RelacionLoad(AgregarRelacionInternoView Window = null)
        {
            setValidacionesIdentificacionRelacionesPersonales();
            OnPropertyChanged();
        }
        
        private async void PandillaLoad(AgregarPandillaImputadoView Window = null)
        {
            if (TabPandillas)
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(GetPandilla);
                setValidacionesIdentificacionPandillas();
                OnPropertyChanged();
            }
        }
        
        private void ApodosAliasReferenciasLoad(ApodosAliasReferenciasView Window = null)
        {
            if (IsSelectedIdentificacion && TabApodosAlias)
            {
                ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                //OnPropertyChanged();
            }
        }
        #endregion

        #region [TAB_UNLOADS]
        private async void DatosIdentificacionUnload(DatosGeneralesIdentificacionView Window = null)
        {
            if (PEditar)
            {
                if (!base.HasErrors)
                {
                    //if (await GuardarDatosGenerales())
                    if (await GuardarDatosGenerales())
                        StaticSourcesViewModel.Mensaje("DATOS GENERALES", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    else
                        StaticSourcesViewModel.Mensaje("DATOS GENERALES", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                }
            }
        }
        private async void MediaFiliacionUnload(MediaFiliacionView Window = null)
        {
            if (pInsertar || PEditar)
            {
                if (!base.HasErrors)
                {
                    if (await GuardarImputadoFiliacion())
                        StaticSourcesViewModel.Mensaje("IMPUTADO FILACIÓN", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    else
                        StaticSourcesViewModel.Mensaje("IMPUTADO FILACIÓN", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                }
            }
        }
        private void ApodosAliasReferenciasUnload(ApodosAliasReferenciasView Window = null)
        {
            if (PInsertar || PEditar)
            {
                if (AliasApodoChange)
                {
                    base.ClearRules();
                    if (!HasErrors())
                    {
                        if (GuardarApodosAlias())
                            StaticSourcesViewModel.Mensaje("APODOS ALIAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        else
                            StaticSourcesViewModel.Mensaje("APODOS ALIAS", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                }
                else
                    StaticSourcesViewModel.Mensaje("APODOS ALIAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
            }

        }
        private void FotosYHuellasUnload(FotosHuellasDigitalesView Window = null)
        {
            if (PInsertar || PEditar)
            {
                if (FotosHuellasChange)
                {
                    if (!base.HasErrors)
                    {
                        if (GuardarFotosYHuellas())
                            StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        else
                            StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                }
                else
                    StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
            }
        }
        private void SenasParticularesUnload(TopografiaHumanaView Window = null)
        {
            /*if (!base.HasErrors)
            {
                var listaAuxiliar = new List<SENAS_PARTICULARES>();
                foreach (var item in ListSenasParticulares.OrderBy(o => o.ID_SENA))
                {
                    listaAuxiliar.Add(new SENAS_PARTICULARES
                    {
                        CANTIDAD = item.CANTIDAD,
                        CLASIFICACION = null,
                        CODIGO = item.CODIGO,
                        ID_ANIO = item.ID_ANIO,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        ID_REGION = item.ID_REGION,
                        ID_SENA = item.ID_SENA,
                        ID_TIPO_TATUAJE = item.ID_TIPO_TATUAJE,
                        IMAGEN = item.IMAGEN,
                        INTRAMUROS = item.INTRAMUROS,
                        SIGNIFICADO = item.SIGNIFICADO,
                        TIPO = item.TIPO
                    });
                }
                if ((new cSenasParticulares()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, listaAuxiliar.ToList()))
                {
                    (new Dialogos()).ConfirmacionDialogo("EXITO!", "INFORMACION GRABADA EXITOSAMENTE!!");
                    LimpiarCampos();
                }
                else
                    (new Dialogos()).ConfirmacionDialogo("Error.", "Error al guardar.");
            }*/

        }
        private void PandillasUnload(PandillasView Window = null)
        {
            if (PInsertar || PEditar)
            {
                if (PandillaChange)
                {
                    if (!base.HasErrors)
                    {
                        if (GuardarPandillaDB())
                            StaticSourcesViewModel.Mensaje("PANDILLA", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        else
                            StaticSourcesViewModel.Mensaje("PANDILLA", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                }
            }
        }
        #endregion

        #region [FICHA]
        private void ImprimeFicha()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var vw = new ReporteView(SelectIngreso);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    vw.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    vw.Owner = PopUpsViewModels.MainWindow;
                    vw.Show();
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al cargar datos para imprimir la ficha de identificacion.", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.FICHA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrio un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region Cargar Listas
        private void CargarListas() 
        {
            try
            {
                #region Generales
                ListEstadoCivil = ListEstadoCivil ?? new ObservableCollection<ESTADO_CIVIL>((new cEstadoCivil()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListOcupacion = ListOcupacion ?? new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                ListEscolaridad = ListEscolaridad ?? new ObservableCollection<ESCOLARIDAD>((new cEscolaridad()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListReligion = ListReligion ?? new ObservableCollection<RELIGION>((new cReligion()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListEtnia = ListEtnia ?? new ObservableCollection<ETNIA>((new cEtnia()).ObtenerTodos().OrderBy(o => o.DESCR));
                ListPaisNacionalidad = ListPaisNacionalidad ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerNacionalidad().OrderBy(o => o.NACIONALIDAD));
                ListPaisNacimiento = ListPaisNacimiento ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos().OrderBy(o => o.PAIS));
                LstIdioma = LstIdioma ?? new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos().OrderBy(o => o.DESCR));
                LstDialecto = LstDialecto ?? new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos().OrderBy(o => o.DESCR));
                #endregion

                #region Senias Particulares
                var mediaFiliacion = new List<MEDIA_FILIACION>((new cMediaFiliacion()).ObtenerTodos());
                #region SENIAS_GENERALES
                Complexion = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 39)).FirstOrDefault().TIPO_FILIACION);
                ColorPiel = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 30)).FirstOrDefault().TIPO_FILIACION);
                Cara = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 7)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region TIPO_SANGRE
                TipoSangre = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 22)).FirstOrDefault().TIPO_FILIACION);
                FactorSangre = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 23)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region CABELLO
                CantidadCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 8)).FirstOrDefault().TIPO_FILIACION);
                ColorCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 9)).FirstOrDefault().TIPO_FILIACION);
                CalvicieCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 10)).FirstOrDefault().TIPO_FILIACION);
                FormaCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 11)).FirstOrDefault().TIPO_FILIACION);
                ImplantacionCabello = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 31)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region CEJA
                DireccionCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 12)).FirstOrDefault().TIPO_FILIACION);
                ImplantacionCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 13)).FirstOrDefault().TIPO_FILIACION);
                FormaCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 14)).FirstOrDefault().TIPO_FILIACION);
                TamanioCeja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 15)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region FRENTE
                AlturaFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 27)).FirstOrDefault().TIPO_FILIACION);
                InclinacionFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 28)).FirstOrDefault().TIPO_FILIACION);
                AnchoFrente = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 29)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region OJOS
                ColorOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 16)).FirstOrDefault().TIPO_FILIACION);
                FormaOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 17)).FirstOrDefault().TIPO_FILIACION);
                TamanioOjos = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 18)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region NARIZ
                RaizNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 1)).FirstOrDefault().TIPO_FILIACION);
                DorsoNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 3)).FirstOrDefault().TIPO_FILIACION);
                AnchoNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 4)).FirstOrDefault().TIPO_FILIACION);
                BaseNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 5)).FirstOrDefault().TIPO_FILIACION);
                AlturaNariz = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 6)).FirstOrDefault().TIPO_FILIACION);
                #endregion
                #region LABIO
                AlturaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 32)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                ProminenciaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 33)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                EspesorLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 21)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                #endregion
                #region BOCA
                TamanioBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 19)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                ComisuraBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 20)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                #endregion
                #region MENTON
                TipoMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 24)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                FormaMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 25)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                InclinacionMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 26)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                #endregion
                #region OREJAS
                FormaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 34)).FirstOrDefault().TIPO_FILIACION);
                HelixOriginalOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 40)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                HelixSuperiorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 41)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                HelixPosteriorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 42)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                HelixAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 43)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                LobuloContornoOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 44)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                LobuloAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 45)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                LobuloParticularidadOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 46)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                LobuloDimensionOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 47)).FirstOrDefault().TIPO_FILIACION.OrderBy(o => o.DESCR).ToList());
                #endregion
                #endregion

                #region Tatuajes
                ListTipoTatuaje = new ObservableCollection<TATUAJE>(new cTatuaje().ObtenerTodos());
                #endregion

                #region Pandilla
                Pandilla = new ObservableCollection<PANDILLA>(new cPandilla().ObtenerTodos());
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        #region Generales
                        ListEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                        SelectEstadoCivil = -1;

                        SelectSexo = "M";

                        ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                        SelectOcupacion = -1;

                        ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                        SelectEscolaridad = -1;

                        ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });
                        SelectReligion = -1;

                        ListEtnia.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
                        SelectEtnia = -1;

                        ListPaisNacionalidad.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });

                        ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });
                        ListPaisDomicilioMadre = ListPaisDomicilioPadre = ListPaisDomicilio = ListPaisNacimiento;
                        SelectPaisNacimiento = SelectPais = SelectNacionalidad = -1;
                        TextFechaNacimiento = null;

                        LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
                        SelectedIdioma = -1;

                        LstDialecto.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
                        SelectedDialecto = -1;
                        #endregion

                        #region Senias Particulares
                        var tipoFiliacion = new TIPO_FILIACION() { ID_TIPO_FILIACION = -1, DESCR = "SELECCIONE" };
                        Complexion.Insert(0, tipoFiliacion);
                        ColorPiel.Insert(0, tipoFiliacion);
                        Cara.Insert(0, tipoFiliacion);
                        TipoSangre.Insert(0, tipoFiliacion);
                        FactorSangre.Insert(0, tipoFiliacion);
                        CantidadCabello.Insert(0, tipoFiliacion);
                        ColorCabello.Insert(0, tipoFiliacion);
                        CalvicieCabello.Insert(0, tipoFiliacion);
                        FormaCabello.Insert(0, tipoFiliacion);
                        ImplantacionCabello.Insert(0, tipoFiliacion);
                        DireccionCeja.Insert(0, tipoFiliacion);
                        ImplantacionCeja.Insert(0, tipoFiliacion);
                        FormaCeja.Insert(0, tipoFiliacion);
                        TamanioCeja.Insert(0, tipoFiliacion);
                        AlturaFrente.Insert(0, tipoFiliacion);
                        InclinacionFrente.Insert(0, tipoFiliacion);
                        AnchoFrente.Insert(0, tipoFiliacion);
                        ColorOjos.Insert(0, tipoFiliacion);
                        FormaOjos.Insert(0, tipoFiliacion);
                        TamanioOjos.Insert(0, tipoFiliacion);
                        RaizNariz.Insert(0, tipoFiliacion);
                        DorsoNariz.Insert(0, tipoFiliacion);
                        AnchoNariz.Insert(0, tipoFiliacion);
                        BaseNariz.Insert(0, tipoFiliacion);
                        AlturaNariz.Insert(0, tipoFiliacion);
                        AlturaLabio.Insert(0, tipoFiliacion);
                        ProminenciaLabio.Insert(0, tipoFiliacion);
                        EspesorLabio.Insert(0, tipoFiliacion);
                        TamanioBoca.Insert(0, tipoFiliacion);
                        ComisuraBoca.Insert(0, tipoFiliacion);
                        TipoMenton.Insert(0, tipoFiliacion);
                        FormaMenton.Insert(0, tipoFiliacion);
                        InclinacionMenton.Insert(0, tipoFiliacion);
                        FormaOreja.Insert(0, tipoFiliacion);
                        HelixOriginalOreja.Insert(0, tipoFiliacion);
                        HelixSuperiorOreja.Insert(0, tipoFiliacion);
                        HelixPosteriorOreja.Insert(0, tipoFiliacion);
                        HelixAdherenciaOreja.Insert(0, tipoFiliacion);
                        LobuloContornoOreja.Insert(0, tipoFiliacion);
                        LobuloAdherenciaOreja.Insert(0, tipoFiliacion);
                        LobuloParticularidadOreja.Insert(0, tipoFiliacion);
                        LobuloDimensionOreja.Insert(0, tipoFiliacion);
                        #endregion

                        #region Tatuajes
                        ListTipoTatuaje.Insert(0, new TATUAJE() { ID_TATUAJE = -1, DESCR = "SELECCIONE" });
                        #endregion

                        #region Pandilla
                        Pandilla.Insert(0, new PANDILLA() { ID_PANDILLA = -1, NOMBRE = "SELECCIONE" });
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los listados.", ex);
                    }
                }));

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los listados.", ex);
            }
        }
        #endregion

        #region Tabs
        private async void GuardarTabs() 
        {
            if (Opcion == -1)
                return;
            if (Opcion != Anterior)
            {
                #region Anterior
                if (Anterior == (short)enumIngresoTabs.DATOS_GENERALES)
                {
                    if (!base.HasErrors)
                    {
                        if (pEditar)
                        {
                            //if (await GuardarDatosGenerales())
                            if (await GuardarDatosGenerales())
                            {
                                StaticSourcesViewModel.SourceChanged = false;
                                Mensaje("DATOS GENERALES", true); 
                            }
                            else
                                Mensaje("DATOS GENERALES", false);
                            Anterior = Opcion;
                            
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    }
                    else
                        Opcion = (short)enumIngresoTabs.DATOS_GENERALES;
                }
                else if (Anterior == (short)enumIngresoTabs.APODOS_ALIAS)
                {
                    if (!base.HasErrors)
                    {
                        if (pEditar)
                        {
                            if (GuardarApodosAlias())
                            {
                                StaticSourcesViewModel.SourceChanged = false;
                                Mensaje("APODOS ALIAS", true); 
                            }
                            else
                                Mensaje("APODOS ALIAS", false);
                                Anterior = Opcion;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    }
                    else
                        Opcion = (short)enumIngresoTabs.APODOS_ALIAS;
                }
                else if (Anterior == (short)enumIngresoTabs.FOTOS_HUELLAS)
                {
                    if (!base.HasErrors)
                    {
                        if (pEditar)
                        {
                            if (ImagesToSave != null)
                            {
                                if (ImagesToSave.Count >= 3)
                                {
                                    if (GuardarFotosYHuellas())
                                    {
                                        StaticSourcesViewModel.SourceChanged = false;
                                        Mensaje("FOTOS Y HUELLAS", true);
                                        Anterior = Opcion;
                                    }
                                    else
                                    {
                                        Mensaje("FOTOS Y HUELLAS", false);
                                        Anterior = Opcion;
                                    }

                                }
                                else
                                {
                                    StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "FAVOR DE CAPTURAR FOTOGRAFIAS", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                                    Anterior = Opcion;
                                }
                            }
                            else
                            {
                                StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "FAVOR DE CAPTURAR FOTOGRAFIAS", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION);
                                Anterior = Opcion;
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    }
                    else
                        Opcion = (short)enumIngresoTabs.FOTOS_HUELLAS;
                }
                else if (Anterior == (short)enumIngresoTabs.MEDIA_FILIACION)
                {
                    if (!base.HasErrors)
                    {
                        if (pEditar)
                        {
                            if (await GuardarImputadoFiliacion())
                            {
                                StaticSourcesViewModel.SourceChanged = false;
                                Mensaje("MEDIA FILIACIÓN", true); 
                            }
                            else
                                Mensaje("MEDIA FILIACIÓN", false);
                            Anterior = Opcion;
                            return;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    }
                    else
                        Opcion = (short)enumIngresoTabs.MEDIA_FILIACION;
                }
                else if (Anterior == (short)enumIngresoTabs.SENIAS_PARTICULARES)
                {
                    //if (!base.HasErrors)
                    //{
                    //    if (pEditar)
                    //    {
                    //        if (await GuardarSenasParticulares())
                    //            Mensaje("SEÑAS PARTICULARES", true);
                    //        else
                    //            Mensaje("SEÑAS PARTICULARES", false);
                            
                    //    }
                    //    else
                    //        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    //}
                    //else
                        Anterior = Opcion;
                        //Opcion = (short)enumIngresoTabs.SENIAS_PARTICULARES;
                }
                else if (Anterior == (short)enumIngresoTabs.PANDILLAS)
                {
                    if (!base.HasErrors)
                    {
                        if (pEditar)
                        {
                            if (GuardarPandillaDB())
                            {
                                StaticSourcesViewModel.SourceChanged = false;
                                Mensaje("PANDILLAS", true); 
                            }
                            else
                                Mensaje("PANDILLAS", false);
                            Anterior = Opcion;
                            
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no cuenta con permisos para realizar esta acción.");
                    }
                    else
                        Opcion = (short)enumIngresoTabs.DATOS_GENERALES;
                }
                #endregion
            }
            
            //Nuevo
            if (Opcion == (short)enumIngresoTabs.DATOS_GENERALES)
            {
                setValidacionesIdentificacionDatosGenerales();
            }
            else if (Opcion == (short)enumIngresoTabs.APODOS_ALIAS)
            {
                base.ClearRules();
            }
            else if (Opcion == (short)enumIngresoTabs.FOTOS_HUELLAS)
            {
                setValidacionesIdentificacionFotosHuellas();
                //if (CamaraWeb != null){
                //    if (!CamaraWeb.isVideoSourceInitialized)
                //        new Dialogos().ConfirmacionDialogo("Error de Dispositivo!", "La camara se encuentra desconectada");
                //}
                //else
                //    new Dialogos().ConfirmacionDialogo("Error de Dispositivo!", "La camara se encuentra desconectada");
                
            }
            else if (Opcion == (short)enumIngresoTabs.MEDIA_FILIACION)
            {
                setValidacionesIdentificacionMediaFiliacion();
                if (base.HasErrors)
                    DatosGeneralesEnabled = ApodosAliasEnabled = FotosHuellasEnabled = SenasParticularesEnabled = PandillasEnabled = false;
                else
                    DatosGeneralesEnabled = ApodosAliasEnabled = FotosHuellasEnabled = SenasParticularesEnabled = PandillasEnabled = true;
            }
            else if (Opcion == (short)enumIngresoTabs.SENIAS_PARTICULARES)
            {
                setValidacionesIdentificacionSeniasParticulares();
            }
            else if (Opcion == (short)enumIngresoTabs.PANDILLAS)
            {
                base.ClearRules();
            }
        }

        private void Mensaje(string titulo,bool resultado)
        {
            if (resultado)
                StaticSourcesViewModel.Mensaje(titulo, "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
            else
                StaticSourcesViewModel.Mensaje(titulo, "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
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
                }
            }
        }
        #endregion
    }
}