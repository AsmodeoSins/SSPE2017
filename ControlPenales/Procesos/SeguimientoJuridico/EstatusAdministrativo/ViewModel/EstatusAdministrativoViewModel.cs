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
    partial class EstatusAdministrativoViewModel : FingerPrintScanner
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange _OnParameterChange { get; set; }
        public EstatusAdministrativoViewModel() { }
        private enum VentanaGuardar
        {
            NINGUNA,
            INGRESO,
            DATOS_GENERALES,
            APODOS_ALIAS_REFERENCIAS,
            FOTOSYHUELLASDIGITALES,
            MEDIA_FILIACION,
            SENAS_PARTICULARES,
            PANDILLAS
        };

        private VentanaGuardar _ventanaGuardar;


        private void TipoSwitch(Object obj)
        {
            try
            {
                //int.TryParse(obj.ToString);
                if (Int32.TryParse(obj.ToString(), out selectTipoSenia))
                    SelectTipoSenia = selectTipoSenia;
                if (RegionCodigo != null)
                    RegionValorCodigo = RegionCodigo[1] + RegionCodigo[2] + RegionCodigo[3] + "";
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
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectTatuaje.DESCR;
                    else
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el significado de la seña.", ex);
            }
        }
        private async void clickSwitch(Object obj)
        {
            BanderaEntrada = true;
            switch (obj.ToString())
            {
                case "ampliar_justificacion_traslado":
                    TituloHeaderExpandirDescripcion = "Justificación";
                    TextAmpliarDescripcion = DTJustificacion;
                    MaxLengthAmpliarDescripcion = 1000;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "ampliar_descripcion":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "guardar_ampliar_descripcion":
                    if (!Justificacion)
                    {
                        TextSignificado = TextSignificado + " " + TextAmpliarDescripcion;
                    }
                    else
                    {
                        DTJustificacion = TextAmpliarDescripcion;
                        TextAmpliarDescripcion = string.Empty;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "cancelar_ampliar_descripcion":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION);
                    break;
                case "tomar_foto_senas":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    VisibleTomarFotoSenasParticulares = true;
                    BotonTomarFotoEnabled = false;
                    TomarFotoLoad(PopUpsViewModels.MainWindow.FotosSenasView);
                    break;
                case "aceptar_tomar_foto_senas":
                    VisibleTomarFotoSenasParticulares = false;
                    if (ImagesToSave[0].ImageCaptured != null)
                        ImagenTatuaje = ImagesToSave[0].ImageCaptured;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    break;
                case "cancelar_tomar_foto_senas":
                    VisibleTomarFotoSenasParticulares = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.FOTOSSENIASPARTICULAES);
                    if (CamaraWeb != null)
                    {
                        await CamaraWeb.ReleaseVideoDevice();
                        CamaraWeb = null;
                    }
                    break;
                case "seleccionar_sena_particular": //DOBLE CLICK EN SENIAS PARTICULARES
                    if (SelectSenaParticular != null)
                    {
                        SeniasParticularesEditable = true;
                        RegionCodigo = SelectSenaParticular.CODIGO.ToCharArray();
                        //await TaskEx.Delay(1000);
                        SelectAnatomiaTopografica = await StaticSourcesViewModel.CargarDatosAsync<ANATOMIA_TOPOGRAFICA>(() => new cAnatomiaTopografica().Obtener((int)SelectSenaParticular.ID_REGION));
                        if (regionCodigo[4].ToString() == "F" || (RegionCodigo[4].ToString() == "U" && SelectSenaParticular.ID_REGION < 50))
                            TabFrente = true;
                        else if (regionCodigo[4].ToString() == "D" || (RegionCodigo[4].ToString() == "U" && SelectSenaParticular.ID_REGION >= 50))
                            TabDorso = true;
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
                        ImagenTatuaje = new Imagenes().ConvertByteToBitmap(SelectSenaParticular.IMAGEN);
                        CodigoSenia = SelectSenaParticular.CODIGO;
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
                        TextAmpliarDescripcion = string.Empty;
                    }
                    break;
                case "buscar_visible":
                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                    this.buscarImputado();
                    break;
                case "buscar_salir":
                    TabVisible = true;
                    if (ImputadoSeleccionadoAuxiliar != null)
                    {
                        Imputado = ImputadoSeleccionadoAuxiliar;
                        SelectIngreso = selectIngresoOld;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                    }
                    StaticSourcesViewModel.SourceChanged = false;
                    //LIMPIAMOS FILTROS
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (StaticSourcesViewModel.SourceChanged ? await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                        "Existen cambios sin guardar,¿desea seleccionar otro imputado?") != 1 : false)
                    {
                        TabVisible = true;
                        Imputado = ImputadoSeleccionadoAuxiliar;
                        SelectIngreso = selectIngresoOld;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    if (SelectExpediente != null)
                    {
                        
                        if (SelectIngreso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                            resetIngreso();
                            return;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                resetIngreso();
                                return;
                            }
                        }
                        if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO.Value != GlobalVar.gCentro)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Ingreso no vigente.", "El ingreso seleccionado no pertenece a su centro.");
                            resetIngreso();
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningun cambio de información.");
                            resetIngreso();
                            return;
                        }
                        if (!IsSelectedDatosIngreso)
                            _ventanaGuardar = VentanaGuardar.NINGUNA;
                        else
                            _ventanaGuardar = VentanaGuardar.INGRESO;
                        CambioImputado = true;
                        await this.getDatosIngreso();
                        getDatosTraslados();
                        LimpiarCamposDatosIngreso();
                        NuevoImputado = false;
                        EditarImputado = true;
                        Imputado = SelectExpediente;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;

                        EditarIngreso = true;
                        Ingreso = SelectIngreso;
                        //await TreeViewViewModel();
                        TabVisible = IngresoEnabled = IdentificacionEnabled = TabDatosGenerales = IsSelectedDatosIngreso =
                            ClasificacionJuridicaEnabled = EstatusAdministrativoEnabled = true;
                        IsTrasladoVisible = Visibility.Collapsed;
                        setValidacionesDatosIngreso();
                        getDatosIngresoImputado();
                        if (IsTrasladoVisible != Visibility.Collapsed)
                            CargarDatosTraslado();
                        else
                            LimpiarDatosTraslado();
                        //ALIAS
                        ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                        //APODO
                        ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                        //RELACION PERSONAL INTERNO
                        ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);

                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);

                        getDatosImputado();

                        MenuInsertarEnabled = MenuDeshacerEnabled = MenuGuardarEnabled = MenuLimpiarEnabled = MenuReporteEnabled = MenuBorrarEnabled =
                            MenuBuscarEnabled = MenuFichaEnabled = MenuAyudaEnabled = MenuSalirEnabled = true;

                        ImputadoSeleccionadoAuxiliar = Imputado;
                        selectIngresoOld = SelectIngreso;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un expediente.");
                    ChecarValidaciones();

                    break;
                case "agregar_causa_penal":
                    TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "ingresos_discrecionales":
                    TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "insertar_delito":
                    TabVisible = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "insertar_alias":
                    setValidacionesIdentificacionAlias();
                    NombreAlias = PaternoAlias = MaternoAlias = string.Empty;
                    TituloAlias = "Agregar Alias";
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    OpcionGuardarAlias = 1;
                    break;
                case "editar_alias":
                    setValidacionesIdentificacionAlias();
                    //if (SelectAlias != null && SelectAlias.IMPUTADO == null)
                    if (SelectAlias != null)
                    {
                        TituloAlias = "Editar Alias";
                        PaternoAlias = SelectAlias.PATERNO;
                        MaternoAlias = SelectAlias.MATERNO;
                        NombreAlias = SelectAlias.NOMBRE;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        OpcionGuardarAlias = 0;
                    }
                    break;
                case "eliminar_alias":
                    this.EliminarAlias();
                    break;
                case "guardar_alias":
                    if (!base.HasErrors)
                    {
                        this.GuardarAlias();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos obligatorios. " + base.Error);
                    break;
                case "cancelar_alias":
                    VisibleAlias = false;
                    PaternoAlias = MaternoAlias = NombreAlias = string.Empty;
                    SelectAlias = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "insertar_apodo":
                    setValidacionesIdentificacionApodos();
                    TituloApodo = "Agregar Apodo";
                    Apodo = string.Empty;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                    OpcionGuardarApodo = 1;
                    break;
                case "editar_apodo":
                    setValidacionesIdentificacionApodos();
                    if (SelectApodo != null/* && SelectApodo.IMPUTADO == null*/)
                    {
                        TituloApodo = "Editar Apodo";
                        Apodo = SelectApodo.APODO1;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        OpcionGuardarApodo = 0;
                    }
                    break;
                case "eliminar_apodo":
                    this.EliminarApodo();
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
                    break;
                case "limpiar_menu":
                    //if (StaticSourcesViewModel.SourceChanged && await new Dialogos().ConfirmarEliminar("ADVERTENCIA!",
                    //        "Existen cambios sin guardar,¿desea limpiar la pantalla?") != 1)
                    //    return;
                    //if (IsSelectedIdentificacion && TabSenasParticulares)
                    //{
                    //    LimpiarCamposSeniasParticulares();
                    //    break;
                    //}
                    //if (CamaraWeb != null)
                    //{
                    //    await CamaraWeb.ReleaseVideoDevice();
                    //    CamaraWeb = null;
                    //}
                    StaticSourcesViewModel.SourceChanged = false;
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstatusAdministrativoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EstatusAdministrativoViewModel();
                    break;
                case "insertar_relacion_interno":
                    setValidacionesIdentificacionRelacionesPersonales();
                    TituloRelacionInterno = "Agregar Relación Personal Interno";
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                    OpcionGuardarRelacionInterno = 1;
                    break;
                case "editar_relacion_interno":
                    setValidacionesIdentificacionRelacionesPersonales();
                    if (SelectRelacionPersonalInterno != null)
                    {
                        TituloRelacionInterno = "Editar Relación Personal Interno";
                        NotaRelacionInterno = SelectRelacionPersonalInterno.NOTA;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                        OpcionGuardarRelacionInterno = 0;
                    }
                    break;
                case "eliminar_relacion_interno":
                    this.EliminarRelacionInterno();
                    break;
                case "guardar_relacion_interno":
                    if (!base.HasErrors)
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                        this.GuardarRelacionInterno();
                        StaticSourcesViewModel.SourceChanged = true;
                        base.ClearRules();
                    }
                    break;
                case "cancelar_relacion_interno":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                    CancelarBuscarRelacionInterno();
                    break;
                case "nuevo_expediente":
                    NuevoImputado = true;
                    LimpiarCampos();
                    IsSelectedDatosIngreso = true;
                    EditarImputado = EditarIngreso = IdentificacionEnabled = false;
                    await this.getDatosIngreso();
                    SelectClasificacionJuridica = "I";
                    SelectEstatusAdministrativo = 8;
                    ClasificacionJuridicaEnabled = CamposBusquedaEnabled = TabVisible = false;
                    IngresoEnabled = IdentificacionEnabled = true;
                    IsTrasladoVisible = Visibility.Collapsed;
                    Imputado = new IMPUTADO();
                    ChecarValidaciones();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;
                case "guardar_menu":
                    if (base.HasErrors != true)
                    {
                        if (Imputado != null)
                        {
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando información", updateImputado))
                                new Dialogos().ConfirmacionDialogo("ÉXITO!", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Notificación", "Debe Seleccionar un imputado. ");
                        }
                    }
                    else
                    {
                        (new Dialogos()).ConfirmacionDialogo("Error", string.Format("Al validar los campos: {0}.", base.Error));
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
                case "salir_menu":
                    Imputado = null;
                    PrincipalViewModel.SalirMenu();
                    break;
                case "cancelar_documento":
                    break;
                case "Open442":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                //PANDILLAS
                case "insertar_pandilla":
                    SelectedImputadoPandilla = null;
                    this.AgregarPandilla();
                    break;
                case "editar_pandilla":
                    this.EditarPandilla();
                    break;
                case "eliminar_pandilla":
                    this.EliminarPandilla();
                    break;
                case "guardar_pandilla":
                    this.GuardarPandilla();
                    break;
                case "cancelar_pandilla":
                    this.LimpiarPandilla();
                    break;
                case "buscar_menu":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_delito":
                    #region buscar delito
                     PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    var pantalla = new BuscarDelitoView();
                    pantalla.DataContext = new BuscarDelitoViewModel();
                    pantalla.KeyDown += (s, e) =>
                    {
                        try
                        {
                            if (e.Key == System.Windows.Input.Key.Escape) pantalla.Close();
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar nuc", ex);
                        }
                    };
                    pantalla.Closed += (s, e) =>
                    {
                        try
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            var d = ((BuscarDelitoViewModel)pantalla.DataContext).Delito;
                            if (d != null)
                                SelectedDelito = d;
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                        }
                    };
                    pantalla.ShowDialog();
                    #endregion
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
            BanderaEntrada = true;
            buscarImputado(obj);
        }
        private async void ModelEnter(Object obj)
        {
            BanderaEntrada = true;
            if (obj != null)
            {
                if (!obj.GetType().Name.Equals("String"))
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
                                FolioBuscar = Convert.ToInt32(textbox.Text);
                                break;
                            case "AnioBuscar":
                                AnioBuscar = Convert.ToInt32(textbox.Text);
                                break;
                        }
                    }
                }
            }
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

            if (AnioBuscar.HasValue && FolioBuscar.HasValue)
            {
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda(1));
                if (ListExpediente.Count == 1)
                {
                    if (ListExpediente[0].INGRESO.Count > 0)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                resetIngreso();
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            resetIngreso();
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            resetIngreso();
                            return;
                        }
                        await this.getDatosIngreso();
                        LimpiarCamposDatosIngreso();
                        SelectExpediente = Imputado = ListExpediente[0];
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;

                        selectIngresoOld=SelectIngreso = Ingreso = ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO && w.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_TRASLADO).FirstOrDefault();
                        EditarIngreso = TabVisible = ContenedorIdentificacionVisible = EditarImputado = TabDatosGenerales = true;

                        TabVisible = IngresoEnabled = IdentificacionEnabled = TabDatosGenerales = IsSelectedDatosIngreso =
                            ClasificacionJuridicaEnabled = EstatusAdministrativoEnabled = true;
                        IsTrasladoVisible = Visibility.Collapsed;
                        setValidacionesDatosIngreso();
                        getDatosTraslados();
                        getDatosImputado();
                        ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                        ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                        ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);

                        CamposBusquedaEnabled = false;
                        MenuInsertarEnabled = MenuDeshacerEnabled = MenuGuardarEnabled = MenuLimpiarEnabled = MenuReporteEnabled =
                            MenuBorrarEnabled = MenuBuscarEnabled = MenuFichaEnabled = MenuAyudaEnabled = MenuSalirEnabled = true;
                        ImputadoSeleccionadoAuxiliar = Imputado;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        TabVisible = false;
                    }
                }
                else
                {
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    TabVisible = false;
                }
            }
            else
            {
                //ListExpediente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar));
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                TabVisible = false;
            }
        }

        private void resetIngreso()
        {
            SelectIngreso = selectIngresoOld;
            if (selectIngresoOld!=null)
            {
                ApellidoPaternoBuscar = selectIngresoOld.IMPUTADO.PATERNO;
                ApellidoMaternoBuscar = selectIngresoOld.IMPUTADO.MATERNO;
                NombreBuscar = selectIngresoOld.IMPUTADO.NOMBRE;
                AnioBuscar = selectIngresoOld.ID_ANIO;
                FolioBuscar = selectIngresoOld.ID_IMPUTADO;
            }
        }

        private void TreeClick(Object obj)
        {
            var x = obj.GetType();
            if (x.BaseType.Name.Equals("CAMA"))
                SelectedCama = (CAMA)obj;
            else
                SelectedCama = null;
        }
        private void TiempoEstado()
        {
            AniosEstado = string.Empty;
            MesesEstado = string.Empty;
            if (FechaEstado != null)
            {
                int anios = 0, meses = 0, dias = 0;

                var hoy = Fechas.GetFechaDateServer;

                anios = (hoy.Year - FechaEstado.Year);
                meses = (hoy.Month - FechaEstado.Month);
                dias = (hoy.Day - FechaEstado.Day);

                if (meses < 0)
                {
                    anios -= 1;
                    meses += 12;
                }
                if (dias < 0)
                {
                    meses -= 1;
                    dias += DateTime.DaysInMonth(hoy.Year, FechaEstado.Month);
                }

                if (anios < 0)
                {
                    System.Windows.MessageBox.Show("La fecha es inválida");
                }

                if (anios > 99)
                    anios = 99;
                if (meses > 99)
                    meses = 99;
                AniosEstado = string.Format("{0}", anios);
                MesesEstado = string.Format("{0}", meses);
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
            if (IsSelectedDatosIngreso)
                LimpiarCamposDatosIngreso();
            else if (IsSelectedIdentificacion)
                if (TabDatosGenerales)
                    LimpiarCamposIdentificacion();
                else if (TabMediaFiliacion)
                    LimpiarCamposMediaFiliacion();
                else if (TabSenasParticulares)
                    LimpiarCamposSeniasParticulares();
        }
        private void LimpiarCamposDatosIngreso()
        {
            CamposBusquedaEnabled = ClasificacionJuridicaEnabled = EstatusAdministrativoEnabled = true;

            #region DatosGenerales
            AnioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            #endregion

            #region DatosIngreso
            SelectTipoIngreso = SelectEstatusAdministrativo = IngresoDelito = SelectTipoDisposicion = SelectTipoAutoridadInterna = -1;
            SelectClasificacionJuridica = TextNumeroOficio = SelectTipoSeguridad = string.Empty;
            SelectedCama = null;
            #endregion

        }
        private void LimpiarCamposIdentificacion()
        {
            #region DatosGenerales
            SelectEtnia = SelectEscolaridad = SelectOcupacion = SelectEstadoCivil = SelectNacionalidad = SelectReligion = -1;
            SelectSexo = "S";
            TextEstatura = TextPeso = string.Empty;
            #endregion

            #region Domicilio
            SelectPais = Parametro.PAIS; //82;
            SelectEntidad = Parametro.ESTADO; //2;
            SelectMunicipio = -1;
            SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
            TextNumeroExterior = null;
            TextCalle = TextNumeroInterior = AniosEstado = MesesEstado = TextDomicilioTrabajo = string.Empty;
            FechaEstado = Fechas.GetFechaDateServer;
            TextTelefono = null;
            TextCodigoPostal = null;
            #endregion

            #region Nacimiento
            SelectPaisNacimiento = Parametro.PAIS;//82;
            SelectEntidadNacimiento = SelectMunicipioNacimiento = -1;
            TextFechaNacimiento = Fechas.GetFechaDateServer;
            TextLugarNacimientoExtranjero = "";
            #endregion

            #region Padres
            TextPadreMaterno = TextPadrePaterno = TextPadreNombre = TextMadreMaterno = TextMadrePaterno = TextMadreNombre = string.Empty;
            CheckMadreFinado = CheckPadreFinado = false;
            #endregion
        }
        private void LimpiarCamposMediaFiliacion()
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
        private void LimpiarCamposSeniasParticulares()
        {
            TextSignificado = string.Empty;
            TextCantidad = string.Empty;
            CodigoSenia = string.Empty;
            SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == -1).FirstOrDefault();
            SelectAnatomiaTopografica = ListRegionCuerpo.Where(w => w.ID_REGION == -1).FirstOrDefault();
            SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == "").FirstOrDefault();
            ImagenTatuaje = null;
            SelectTipoSenia = 0;
            SelectPresentaIngresar = SelectPresentaIntramuros = SeniasParticularesEditable = SelectTipoCicatriz = SelectTipoTatuaje =
                SelectTipoLunar = SelectTipoDefecto = SelectTipoProtesis = SelectTipoAmputacion = false;
        }
        private void ChecarValidaciones()
        {
            if (base.HasErrors)
            {
                ApodosAliasEnabled = DatosGeneralesEnabled = FotosHuellasEnabled = MediaFiliacionEnabled = PandillasEnabled = IdentificacionEnabled =
                        SenasParticularesEnabled = IngresoEnabled = true;
                if (IsSelectedDatosIngreso)
                {
                    IdentificacionEnabled = false;
                }

                else if (IsSelectedIdentificacion)
                {
                    if (!TabApodosAlias && !TabPandillas && !TabSenasParticulares)
                        IngresoEnabled = false;
                    if (TabDatosGenerales)
                        ApodosAliasEnabled = FotosHuellasEnabled = MediaFiliacionEnabled = PandillasEnabled = SenasParticularesEnabled = false;
                    else if (TabFotosHuellas)
                        ApodosAliasEnabled = DatosGeneralesEnabled = MediaFiliacionEnabled = PandillasEnabled = SenasParticularesEnabled = false;
                    else if (TabMediaFiliacion)
                        ApodosAliasEnabled = DatosGeneralesEnabled = FotosHuellasEnabled = PandillasEnabled = SenasParticularesEnabled = false;
                }
            }
            else
            {
                if (BanderaEntrada)
                    IngresoEnabled = true;
                else
                    IngresoEnabled = false;
                IdentificacionEnabled = ApodosAliasEnabled = DatosGeneralesEnabled = FotosHuellasEnabled = MediaFiliacionEnabled = PandillasEnabled =
                    SenasParticularesEnabled = true;
            }
        }

        #region OracleOperations
        private async Task<bool> getDatosIngreso()
        {
            #region DatosIngreso
            ListTipoIngreso = ListTipoIngreso ?? await StaticSourcesViewModel.CargarDatosAsync<List<TIPO_INGRESO>>(() => (new cTipoIngreso()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListTipoIngreso.Insert(0, new TIPO_INGRESO() { ID_TIPO_INGRESO = -1, DESCR = "SELECCIONE" });
            SelectTipoIngreso = -1;

            ListClasificacionJuridica = ListClasificacionJuridica ?? await StaticSourcesViewModel.CargarDatosAsync<List<CLASIFICACION_JURIDICA>>(() => (new cClasificacionJuridica()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListClasificacionJuridica.Insert(0, new CLASIFICACION_JURIDICA() { ID_CLASIFICACION_JURIDICA = string.Empty, DESCR = "SELECCIONE" });
            SelectClasificacionJuridica = string.Empty;


            ListEstatusAdministrativo = ListEstatusAdministrativo ?? await StaticSourcesViewModel.CargarDatosAsync<List<ESTATUS_ADMINISTRATIVO>>(() => (new cEstatusAdministrativo()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListEstatusAdministrativo.Insert(0, new ESTATUS_ADMINISTRATIVO() { ID_ESTATUS_ADMINISTRATIVO = -1, DESCR = "SELECCIONE" });
            SelectEstatusAdministrativo = -1;


            IngresoDelitos = IngresoDelitos ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO_DELITO>>(() => new ObservableCollection<INGRESO_DELITO>((new cIngresoDelito()).ObtenerTodos().OrderBy(o => o.DESCR)));
            IngresoDelitos.Insert(0, new INGRESO_DELITO() { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE", ID_FUERO = string.Empty });
            IngresoDelito = -1;

            #endregion

            #region DATOS_DOCUMENTO_INTERNACION
            ListTipoAutoridadInterna = ListTipoAutoridadInterna ?? await StaticSourcesViewModel.CargarDatosAsync<List<TIPO_AUTORIDAD_INTERNA>>(() => (new cTipoAutoridadInterna()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListTipoAutoridadInterna.Insert(0, new TIPO_AUTORIDAD_INTERNA() { ID_AUTORIDAD_INTERNA = -1, DESCR = "SELECCIONE" });
            SelectTipoAutoridadInterna = -1;

            ListTipoDisposicion = ListTipoDisposicion ?? await StaticSourcesViewModel.CargarDatosAsync<List<TIPO_DISPOSICION>>(() => (new cTipoDisposicion()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListTipoDisposicion.Insert(0, new TIPO_DISPOSICION() { ID_DISPOSICION = -1, DESCR = "SELECCIONE" });
            SelectTipoDisposicion = -1;


            ListTipoSeguridad = ListTipoSeguridad ?? await StaticSourcesViewModel.CargarDatosAsync<List<TIPO_SEGURIDAD>>(() => (new cTipoSeguridad()).ObtenerTodos().OrderBy(o => o.DESCR).ToList());
            ListTipoSeguridad.Insert(0, new TIPO_SEGURIDAD() { ID_TIPO_SEGURIDAD = string.Empty, DESCR = "SELECCIONE" });
            SelectTipoSeguridad = string.Empty;

            #endregion

            #region Ubicacion
            //ListSector = ListSector ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SECTOR>>(() => new ObservableCollection<SECTOR>((new cSector()).ObtenerTodos(string.Empty, 0, 4, 0).OrderBy(o => o.DESCR)));
            //ListSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
            //ListCelda = ListCelda ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<CELDA>>(() => new ObservableCollection<CELDA>());
            //ListCelda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
            //SelectCelda = "SELECCIONE";

            ////CENTRO
            //Centro = await StaticSourcesViewModel.CargarDatosAsync<CENTRO>(() => (new cCentro()).Obtener(4).First());//MEXICALI
            //await TreeViewViewModel();
            #endregion
            return true;
        }
        private async void buscarImputado(Object obj = null)
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
                            if (!string.IsNullOrWhiteSpace(textbox.Text))
                                FolioBuscar = Convert.ToInt32(textbox.Text);
                            else
                                FolioBuscar = null;
                            break;
                    }
                }
            }
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            TabVisible = false;
            ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
            ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
            if (SelectExpediente != null && SelectExpediente.ID_IMPUTADO == 0) ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            if (SelectExpediente == null) ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            EmptyExpedienteVisible = !(ListExpediente.Count > 0);
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

        private async Task<bool> getDatosGenerales()
        {
            ListEstadoCivil = ListEstadoCivil ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ESTADO_CIVIL>>(() => new ObservableCollection<ESTADO_CIVIL>((new cEstadoCivil()).ObtenerTodos().OrderBy(o => o.DESCR)));
            ListEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
            SelectEstadoCivil = -1;
            SelectSexo = "M";

            ListOcupacion = ListOcupacion ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<OCUPACION>>(() => new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR)));
            ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
            SelectOcupacion = -1;

            ListEscolaridad = ListEscolaridad ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ESCOLARIDAD>>(() => new ObservableCollection<ESCOLARIDAD>((new cEscolaridad()).ObtenerTodos().OrderBy(o => o.DESCR)));
            ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
            SelectEscolaridad = -1;


            ListReligion = ListReligion ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<RELIGION>>(() => new ObservableCollection<RELIGION>((new cReligion()).ObtenerTodos().OrderBy(o => o.DESCR)));
            ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });
            SelectReligion = -1;


            ListEtnia = ListEtnia ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ETNIA>>(() => new ObservableCollection<ETNIA>((new cEtnia()).ObtenerTodos().OrderBy(o => o.DESCR)));
            ListEtnia.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
            SelectEtnia = -1;


            ListPaisNacionalidad = ListPaisNacionalidad ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PAIS_NACIONALIDAD>>(() => new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerNacionalidad().OrderBy(o => o.NACIONALIDAD)));
            ListPaisNacionalidad.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });

            ListPaisNacimiento = ListPaisNacimiento ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PAIS_NACIONALIDAD>>(() => new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos().OrderBy(o => o.PAIS)));
            ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });
            ListPaisDomicilioMadre = ListPaisDomicilioPadre = ListPaisDomicilio = ListPaisNacimiento;
            SelectPaisNacimiento = SelectPais = SelectNacionalidad = -1;
            TextFechaNacimiento = null;

            LstIdioma = LstIdioma ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IDIOMA>>(() => new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos()));
            LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
            SelectedIdioma = -1;

            LstDialecto = LstDialecto ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<DIALECTO>>(() => new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos()));
            LstDialecto.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
            SelectedDialecto = -1;
            return true;
        }
        
        private void getDatosImputado()
        {
            try
            {
                getDatosIngresoImputado();
                getDatosGeneralesIdentificacion();
                getDatosMediaFiliacion();
                GetImputadoPandilla();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar información del interno", ex);
            }
        }
     
        private void getDatosIngresoImputado()
        {
            if (Ingreso.FEC_INGRESO_CERESO != null)
                FechaCeresoIngreso = Ingreso.FEC_INGRESO_CERESO;
            if (Ingreso.FEC_REGISTRO != null)
                FechaRegistroIngreso = Ingreso.FEC_REGISTRO;
            AnioBuscar = Ingreso.ID_ANIO;
            FolioBuscar = ingreso.ID_IMPUTADO;

            //IngresoDelito = Ingreso.ID_INGRESO_DELITO;
            SelectedDelito = Ingreso.DELITO;
            SelectTipoIngreso = Ingreso.ID_TIPO_INGRESO;
            if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
            {
                IsTrasladoVisible = Visibility.Visible;
                setValidacionesTraslado();
            }
            else
            {
                IsTrasladoVisible = Visibility.Collapsed;
                unsetValidacionesTraslado();
            }
            SelectClasificacionJuridica = Ingreso.ID_CLASIFICACION_JURIDICA;
            SelectEstatusAdministrativo = Ingreso.ID_ESTATUS_ADMINISTRATIVO;
            TextNumeroOficio = !string.IsNullOrEmpty(Ingreso.DOCINTERNACION_NUM_OFICIO) ? Ingreso.DOCINTERNACION_NUM_OFICIO.Trim() : string.Empty;
            SelectTipoDisposicion = Ingreso.ID_DISPOSICION;
            SelectTipoSeguridad = Ingreso.ID_TIPO_SEGURIDAD;
            SelectTipoAutoridadInterna = Ingreso.ID_AUTORIDAD_INTERNA;
            SelectedCama = Ingreso.CAMA;

            if (Ingreso.CAMA != null)
            {
                Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                                 Ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                 Ingreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                 Ingreso.CAMA.ID_CELDA.Trim(),
                                                 Ingreso.CAMA.ID_CAMA);
            }

        }
       
        private bool getDatosGeneralesIdentificacion()
        {
            #region IDENTIFICACION

            #region DatosGenerales
            SelectEtnia = Imputado.ID_ETNIA == null ? -1 : Imputado.ID_ETNIA;
            //SelectEscolaridad = Imputado.ID_ESCOLARIDAD == null ? -1 : Imputado.ID_ESCOLARIDAD;
            SelectEscolaridad = SelectIngreso.ID_ESCOLARIDAD == null ? -1 : SelectIngreso.ID_ESCOLARIDAD;
            //SelectOcupacion = Imputado.ID_OCUPACION == null ? -1 : Imputado.ID_OCUPACION;
            SelectOcupacion = SelectIngreso.ID_OCUPACION == null ? -1 : SelectIngreso.ID_OCUPACION;
            //SelectEstadoCivil = Imputado.ID_ESTADO_CIVIL == null ? -1 : Imputado.ID_ESTADO_CIVIL;
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
            SelectPais = SelectIngreso.ID_PAIS == null ? 82 : SelectIngreso.ID_PAIS < 1 ? 82 : SelectIngreso.ID_PAIS;
            SelectEntidad = SelectIngreso.ID_ENTIDAD == null ? 2 : SelectIngreso.ID_ENTIDAD < 1 ? 2 : SelectIngreso.ID_ENTIDAD;
            SelectMunicipio = SelectIngreso.ID_MUNICIPIO == null ? -1 : SelectIngreso.ID_MUNICIPIO < 1 ? -1 : SelectIngreso.ID_MUNICIPIO;
            SelectColoniaItem = SelectIngreso.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : SelectIngreso.ID_COLONIA < 1 ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == SelectIngreso.ID_COLONIA).FirstOrDefault();

            //TextCalle = !string.IsNullOrEmpty(Imputado.DOMICILIO_CALLE) ?Imputado.DOMICILIO_CALLE.Trim() : string.Empty;
            //TextNumeroExterior = Imputado.DOMICILIO_NUM_EXT;
            //TextNumeroInterior = !string.IsNullOrEmpty(Imputado.DOMICILIO_NUM_INT) ? Imputado.DOMICILIO_NUM_INT.Trim() : string.Empty;
            //AniosEstado = Imputado.RESIDENCIA_ANIOS.HasValue ? Imputado.RESIDENCIA_ANIOS.ToString() : string.Empty;
            //MesesEstado = Imputado.RESIDENCIA_MESES.HasValue ? Imputado.RESIDENCIA_MESES.ToString() : string.Empty;
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
            TextLugarNacimientoExtranjero = !string.IsNullOrEmpty(Imputado.NACIMIENTO_LUGAR) ? Imputado.NACIMIENTO_LUGAR : string.Empty;
            #endregion

            #region Padres
            getDatosDomiciliosPadres();
            #endregion

            #endregion
            StaticSourcesViewModel.SourceChanged = false;
            return true;
        }
        
        private void getDatosDomiciliosPadres()
        {
            TextPadreMaterno = !string.IsNullOrEmpty(Imputado.MATERNO_PADRE) ? Imputado.MATERNO_PADRE.Trim() : string.Empty;
            TextPadrePaterno = !string.IsNullOrEmpty(Imputado.PATERNO_PADRE) ? Imputado.PATERNO_PADRE.Trim() : string.Empty;
            TextPadreNombre = !string.IsNullOrEmpty(Imputado.NOMBRE_PADRE) ? Imputado.NOMBRE_PADRE : string.Empty;
            //CheckPadreFinado = Imputado.PADRE_FINADO == "S";
            CheckPadreFinado = SelectIngreso.PADRE_FINADO == "S";

            TextMadreMaterno = !string.IsNullOrEmpty(Imputado.MATERNO_MADRE) ? Imputado.MATERNO_MADRE.Trim() : string.Empty;
            TextMadrePaterno = !string.IsNullOrEmpty(Imputado.PATERNO_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty;
            TextMadreNombre = !string.IsNullOrEmpty(Imputado.NOMBRE_MADRE) ? Imputado.PATERNO_MADRE.Trim() : string.Empty;
            //CheckMadreFinado = Imputado.MADRE_FINADO == "S";
            CheckMadreFinado = SelectIngreso.MADRE_FINADO == "S";

            #region Padres
            if (Imputado.IMPUTADO_PADRES.Count > 0)
            {
                foreach (var item in Imputado.IMPUTADO_PADRES)
                {
                    if (item.ID_PADRE == "P")
                    {
                        SelectPaisDomicilioPadre = item.ID_PAIS == null ? 82 : item.ID_PAIS < 1 ? 82 : item.ID_PAIS;
                        SelectEntidadDomicilioPadre = item.ID_ENTIDAD == null ? 2 : item.ID_ENTIDAD < 1 ? 2 : item.ID_ENTIDAD;
                        SelectMunicipioDomicilioPadre = item.ID_MUNICIPIO == null ? -1 : item.ID_MUNICIPIO < 1 ? -1 : item.ID_MUNICIPIO;
                        SelectColoniaDomicilioPadre = item.ID_COLONIA == null ? -1 : item.ID_COLONIA < 1 ? -1 : item.ID_COLONIA;
                        TextCalleDomicilioPadre = !string.IsNullOrEmpty(item.CALLE) ? item.CALLE : string.Empty;
                        TextNumeroExteriorDomicilioPadre = item.NUM_EXT;
                        TextNumeroInteriorDomicilioPadre = !string.IsNullOrEmpty(item.NUM_INT) ? item.NUM_INT.Trim() : string.Empty;
                        TextCodigoPostalDomicilioPadre = item.CP;
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
                    }
                }
            }
            else
            {
                SelectPaisDomicilioPadre = Parametro.PAIS; //82;
                TextCalleDomicilioPadre = TextNumeroInteriorDomicilioPadre = string.Empty;
                TextNumeroExteriorDomicilioPadre = null;
                TextCodigoPostalDomicilioPadre = null;

                SelectPaisDomicilioMadre = Parametro.PAIS; //82;
                TextCalleDomicilioMadre = TextNumeroInteriorDomicilioMadre = string.Empty;
                TextNumeroExteriorDomicilioMadre = null;
                TextCodigoPostalDomicilioMadre = null;
            }
            #endregion

            if (Imputado.IMPUTADO_PADRES.Count == 1 && Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "P").Any() && !CheckMadreFinado)
            {
                MismoDomicilioMadre = true;
                MismoDomicilioPadre = false;
            }
            else if (Imputado.IMPUTADO_PADRES.Count == 1 && Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "M").Any() && !CheckPadreFinado)
            {
                MismoDomicilioMadre = false;
                MismoDomicilioPadre = true;
            }
            else if (Imputado.IMPUTADO_PADRES.Count == 2)
                MismoDomicilioMadre = MismoDomicilioPadre = false;
            else
                MismoDomicilioMadre = MismoDomicilioPadre = false;
        }
        private void getDatosMediaFiliacion()
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
        }
        private async Task<bool> GetMediaFiliacion()
        {
            if (Complexion == null || Complexion.Count < 1)
            {
                var mediaFiliacion = await StaticSourcesViewModel.CargarDatosAsync<List<MEDIA_FILIACION>>(() => new List<MEDIA_FILIACION>((new cMediaFiliacion()).ObtenerTodos()));
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
                    AlturaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 32)).FirstOrDefault().TIPO_FILIACION);
                    AlturaLabio.Insert(0, tipoFiliacion);
                    ProminenciaLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 33)).FirstOrDefault().TIPO_FILIACION);
                    ProminenciaLabio.Insert(0, tipoFiliacion);
                    EspesorLabio = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 21)).FirstOrDefault().TIPO_FILIACION);
                    EspesorLabio.Insert(0, tipoFiliacion);
                    #endregion
                    #region BOCA
                    TamanioBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 19)).FirstOrDefault().TIPO_FILIACION);
                    TamanioBoca.Insert(0, tipoFiliacion);
                    ComisuraBoca = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 20)).FirstOrDefault().TIPO_FILIACION);
                    ComisuraBoca.Insert(0, tipoFiliacion);
                    #endregion
                    #region MENTON
                    TipoMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 24)).FirstOrDefault().TIPO_FILIACION);
                    TipoMenton.Insert(0, tipoFiliacion);
                    FormaMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 25)).FirstOrDefault().TIPO_FILIACION);
                    FormaMenton.Insert(0, tipoFiliacion);
                    InclinacionMenton = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 26)).FirstOrDefault().TIPO_FILIACION);
                    InclinacionMenton.Insert(0, tipoFiliacion);
                    #endregion
                    #region OREJAS
                    FormaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 34)).FirstOrDefault().TIPO_FILIACION);
                    FormaOreja.Insert(0, tipoFiliacion);
                    HelixOriginalOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 40)).FirstOrDefault().TIPO_FILIACION);
                    HelixOriginalOreja.Insert(0, tipoFiliacion);
                    HelixSuperiorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 41)).FirstOrDefault().TIPO_FILIACION);
                    HelixSuperiorOreja.Insert(0, tipoFiliacion);
                    HelixPosteriorOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 42)).FirstOrDefault().TIPO_FILIACION);
                    HelixPosteriorOreja.Insert(0, tipoFiliacion);
                    HelixAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 43)).FirstOrDefault().TIPO_FILIACION);
                    HelixAdherenciaOreja.Insert(0, tipoFiliacion);
                    LobuloContornoOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 44)).FirstOrDefault().TIPO_FILIACION);
                    LobuloContornoOreja.Insert(0, tipoFiliacion);
                    LobuloAdherenciaOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 45)).FirstOrDefault().TIPO_FILIACION);
                    LobuloAdherenciaOreja.Insert(0, tipoFiliacion);
                    LobuloParticularidadOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 46)).FirstOrDefault().TIPO_FILIACION);
                    LobuloParticularidadOreja.Insert(0, tipoFiliacion);
                    LobuloDimensionOreja = new ObservableCollection<TIPO_FILIACION>((mediaFiliacion.Where(q => q.ID_MEDIA_FILIACION == 47)).FirstOrDefault().TIPO_FILIACION);
                    LobuloDimensionOreja.Insert(0, tipoFiliacion);
                    #endregion
                }
            }
            return true;
        }
        private bool addNuevoExpediente()
        {
            if (!EditarImputado)
            {
                if (!string.IsNullOrEmpty(NombreBuscar) && (!string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
                {
                    Imputado = new IMPUTADO();
                    Imputado.ID_CENTRO = GlobalVar.gCentro;
                    Imputado.ID_ANIO = (short)Fechas.GetFechaDateServer.Year;
                    Imputado.PATERNO = ApellidoPaternoBuscar;
                    Imputado.MATERNO = ApellidoMaternoBuscar;
                    Imputado.NOMBRE = NombreBuscar;
                    Imputado.ID_IMPUTADO = (new cImputado()).Insertar(Imputado);
                    if (Imputado.ID_IMPUTADO > 0)
                    {
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }
        private bool updateImputado()
        {
            try
            {
                if (IsSelectedDatosIngreso)
                {
                    if (addDatosIngreso())
                        return true;
                }
                else if (IsSelectedIdentificacion)
                {
                    if (TabPandillas)
                    {
                        if (GuardarPandillaDB())
                            return true;
                    }
                    else if (TabMediaFiliacion)
                    {
                        if (GuardarImputadoFiliacion())
                            return true;
                    }
                    else if (TabApodosAlias)
                    {
                        if (GuardarApodosAlias())
                            return true;
                    }
                    else if (TabFotosHuellas)
                    {
                        if (GuardarFotosYHuellas())
                            return true;
                    }
                    else if (TabSenasParticulares)
                    {
                        if (GuardarSenasParticulares())
                        {
                            SeniasParticularesEditable = false;
                            return true;
                        }
                    }
                    else if (TabDatosGenerales)
                    {
                        if (GuardarDatosGenerales())
                            return true;
                    }
                }
                else
                    return true;

                (new Dialogos()).ConfirmacionDialogo("Error", "Al actualizar datos del imputado.");
                return false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private bool GuardarDatosGenerales()
        {
            try
            {
                var padres = new List<IMPUTADO_PADRES>();
                var _imputado = new IMPUTADO();
                _imputado.ID_CENTRO = imputado.ID_CENTRO;
                _imputado.ID_ANIO = imputado.ID_ANIO;
                _imputado.ID_IMPUTADO = imputado.ID_IMPUTADO;
                _imputado.NIP = imputado.NIP;
                _imputado.PATERNO = imputado.PATERNO;
                _imputado.MATERNO = imputado.MATERNO;
                _imputado.NOMBRE = imputado.NOMBRE;

                #region DatosGenerales
                _imputado.SEXO = SelectSexo;
                //_imputado.ID_ESTADO_CIVIL = SelectEstadoCivil;
                //_imputado.ID_OCUPACION = SelectOcupacion;
                //_imputado.ID_ESCOLARIDAD = SelectEscolaridad;
                //_imputado.ID_RELIGION = SelectReligion;
                _imputado.ID_ETNIA = SelectEtnia;
                _imputado.ID_NACIONALIDAD = SelectNacionalidad;
                //_imputado.ESTATURA = string.IsNullOrEmpty(TextEstatura) ? new Nullable<short>() : short.Parse(TextEstatura);
                //_imputado.PESO = string.IsNullOrEmpty(TextPeso) ? new Nullable<short>() : short.Parse(TextPeso);
                _imputado.ID_IDIOMA = SelectedIdioma;
                _imputado.ID_DIALECTO = SelectedDialecto;
                _imputado.TRADUCTOR = RequiereTraductor ? "S" : "N";
                #endregion

                #region Domicilio
                //_imputado.DOMICILIO_CALLE = TextCalle;
                //_imputado.DOMICILIO_NUM_INT = TextNumeroInterior;
                //_imputado.DOMICILIO_NUM_EXT = TextNumeroExterior;
                //_imputado.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
                //_imputado.ID_MUNICIPIO = SelectMunicipio;
                //_imputado.ID_ENTIDAD = SelectEntidad;
                //_imputado.ID_PAIS = SelectPais;
                //_imputado.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                //_imputado.TELEFONO = string.IsNullOrWhiteSpace(TextTelefono) ? null : (long?)long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                //_imputado.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                //_imputado.RESIDENCIA_ANIOS = string.IsNullOrEmpty(AniosEstado) ? new Nullable<short>() : short.Parse(AniosEstado);
                //_imputado.RESIDENCIA_MESES = string.IsNullOrEmpty(MesesEstado) ? new Nullable<short>() : short.Parse(MesesEstado);
                #endregion

                #region Nacimiento
                _imputado.NACIMIENTO_PAIS = SelectPaisNacimiento;
                _imputado.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                _imputado.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                _imputado.NACIMIENTO_FECHA = TextFechaNacimiento;
                _imputado.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                #endregion

                #region Padres
                _imputado.NOMBRE_PADRE = TextPadreNombre;
                _imputado.MATERNO_PADRE = TextPadreMaterno;
                _imputado.PATERNO_PADRE = TextPadrePaterno;
                _imputado.NOMBRE_MADRE = TextMadreNombre;
                _imputado.MATERNO_MADRE = TextMadreMaterno;
                _imputado.PATERNO_MADRE = TextMadrePaterno;
                //_imputado.PADRE_FINADO = CheckPadreFinado ? "S" : "N";
                //_imputado.MADRE_FINADO = CheckMadreFinado ? "S" : "N";
                #endregion

                //agrega direcciones de los padres
                if (!CheckMadreFinado || !CheckPadreFinado)
                {
                    if (!MismoDomicilioPadre && !CheckPadreFinado)
                    {

                        padres.Add(new IMPUTADO_PADRES()
                        {
                            ID_IMPUTADO = _imputado.ID_IMPUTADO,
                            ID_PADRE = "P",
                            ID_CENTRO = _imputado.ID_CENTRO,
                            ID_ANIO = _imputado.ID_ANIO,
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
                            ID_IMPUTADO = _imputado.ID_IMPUTADO,
                            ID_PADRE = "M",
                            ID_CENTRO = _imputado.ID_CENTRO,
                            ID_ANIO = _imputado.ID_ANIO,
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
                }
                _imputado.IMPUTADO_PADRES = padres;

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

                if (!string.IsNullOrEmpty(TextEstatura))
                    ingreso.ESTATURA = short.Parse(TextEstatura);
                if (!string.IsNullOrEmpty(TextPeso))
                    ingreso.PESO = short.Parse(TextPeso);
                #endregion

                new cImputado().ActualizarImputadoDatosGeneralesyPadres(_imputado,ingreso);
                //Imputado = new cImputado().Obtener(Imputado.ID_IMPUTADO, Imputado.ID_ANIO, Imputado.ID_CENTRO).FirstOrDefault();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private bool GuardarDomiciliosPadres()
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

            return false;
        }
        private bool GuardarFotosYHuellas()
        {
            try
            {
                var ingresoBiometrico = new List<SSP.Servidor.INGRESO_BIOMETRICO>();
                var imputadoBiometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();
                //BIOMETRICO
                #region [Fotos]
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
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
                            ID_IMPUTADO = Imputado.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                            ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                        });
                    }
                }));

                #endregion

                #region [Huellas]
                List<int> LToma = new List<int>();
                //si no recapturo huellas
                if (HuellasCapturadas != null)
                {
                    string toma = "N";
                    foreach (var item in HuellasCapturadas)
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
                #endregion

                var ingreso = new INGRESO
                {
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = selectIngreso.ID_INGRESO,
                    INGRESO_BIOMETRICO = ingresoBiometrico
                };

                var ingresos = new List<INGRESO>();
                ingresos.Add(ingreso);
                var _imputado = new IMPUTADO
                {
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    IMPUTADO_BIOMETRICO = imputadoBiometrico,
                    INGRESO = ingresos
                };
                new cImputado().ActualizarImputadoHuellasyFotos(_imputado, LToma);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool GuardarApodosAlias()
        {
            try
            {
                var apodos = new List<APODO>();
                if (ListApodo.Count > 0)
                {
                    short id_apodo = 1;
                    foreach (var entidad in ListApodo)
                    {
                        if (entidad.IMPUTADO == null)
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
                }

                var alias = new List<ALIAS>();
                if (ListAlias.Count > 0)
                {
                    short id_alias = 1;
                    foreach (var entidad in ListAlias)
                    {
                        if (entidad.IMPUTADO == null)
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
                }

                var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
                if (ListRelacionPersonalInterno.Count > 0)
                {
                    foreach (var entidad in ListRelacionPersonalInterno)
                    {
                        if (entidad.IMPUTADO == null)
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
                }

                var imputado = new IMPUTADO
                {
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    RELACION_PERSONAL_INTERNO = relacion_personal_internos,
                    APODO = apodos,
                    ALIAS = alias
                };
                new cImputado().ActualizarImputadoAliasApodosRelaciones(imputado);


                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //if (!await saveApodos())
            //    return false;
            //if (!await saveAlias())
            //    return false;
            //if (!await saveRelacionesPersonales())
            //    return false;
            //return true;
        }
        private bool GuardarSenasParticulares()
        {
            try
            {
                Exception _ex = null;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        var presenta = SelectPresentaIngresar ? "N" : "S";

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
                            Imputado.SENAS_PARTICULARES = ListSenasParticulares = new cSenasParticulares().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                            LimpiarCamposSeniasParticulares();
                        }
                        else
                            throw new Exception("Error al salvar las señas particulares");
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                    }
                }));
                if (_ex != null)
                    throw _ex;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private bool GuardarImputadoFiliacion()
        {
            var imputadoFiliacion = new List<IMPUTADO_FILIACION>();
            #region SENIAS_GENERALES
            imputadoFiliacion.Add(new IMPUTADO_FILIACION() { ID_MEDIA_FILIACION = 39, ID_TIPO_FILIACION = SelectComplexion, ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO });
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
            if ((new cImputadoFiliacion()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, imputadoFiliacion))
            {
                return true;
            }
            return false;
        }


        private bool addDatosIngreso()
        {
            try
            {
                if (SelectedCama != null)
                {
                    var camaVieja = new CAMA();
                    var camaNueva = new CAMA();
                    var traslado = new TRASLADO();
                    var ingreso = new INGRESO();
                    var traslado_detalle_nuevo = new List<TRASLADO_DETALLE>();

                    ingreso.ID_CENTRO = Imputado.ID_CENTRO;
                    ingreso.ID_ANIO = Imputado.ID_ANIO;
                    ingreso.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                    ingreso.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    /*****  DATOS_INGRESO *****/
                    ingreso.FEC_REGISTRO = FechaRegistroIngreso;
                    ingreso.FEC_INGRESO_CERESO = FechaCeresoIngreso;
                    ingreso.ID_TIPO_INGRESO = SelectTipoIngreso;
                    ingreso.ID_CLASIFICACION_JURIDICA = SelectClasificacionJuridica;
                    ingreso.ID_ESTATUS_ADMINISTRATIVO = SelectEstatusAdministrativo;
                    //ingreso.ID_INGRESO_DELITO = IngresoDelito;
                    if(SelectedDelito != null)
                    {
                        ingreso.ID_FUERO = SelectedDelito.ID_FUERO;
                        ingreso.ID_DELITO = SelectedDelito.ID_DELITO;
                    }
                    /*****  DATOS_GENERALES *****/
                    ingreso.ID_ESTADO_CIVIL = SelectEstadoCivil != -1 ? SelectEstadoCivil : null;
                    ingreso.ID_OCUPACION = SelectOcupacion != -1 ? SelectOcupacion : null;
                    ingreso.ID_ESCOLARIDAD = SelectEscolaridad !=-1 ? SelectEscolaridad : null;
                    ingreso.ID_RELIGION = SelectReligion != -1 ? SelectReligion : null;
                    //Domicilio
                    ingreso.DOMICILIO_CALLE = TextCalle;
                    ingreso.DOMICILIO_NUM_EXT = TextNumeroExterior;
                    ingreso.DOMICILIO_NUM_INT = TextNumeroInterior;
                    ingreso.ID_COLONIA = SelectColoniaItem.ID_COLONIA != -1 ? (int?)SelectColoniaItem.ID_COLONIA : null;
                    ingreso.ID_MUNICIPIO = SelectMunicipio != -1 ? SelectMunicipio : null;
                    ingreso.ID_ENTIDAD = SelectEntidad != -1 ? SelectEntidad : null;
                    ingreso.ID_PAIS = SelectPais != -1 ? SelectPais : null;
                    ingreso.DOMICILIO_CP = TextCodigoPostal;
                    //En el estado
                    if (!string.IsNullOrEmpty(AniosEstado))
                        ingreso.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                    if (!string.IsNullOrEmpty(MesesEstado))
                        ingreso.RESIDENCIAS_MESES = short.Parse(MesesEstado);
                    if (!string.IsNullOrEmpty(TextTelefono))
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

                    if (!string.IsNullOrEmpty(TextEstatura))
                        ingreso.ESTATURA = short.Parse(TextEstatura);
                    if (!string.IsNullOrEmpty(TextPeso))
                        ingreso.PESO = short.Parse(TextPeso);

                    /***** DATOS_DOCUMENTO_INTERNACION *****/
                    ingreso.ID_DISPOSICION = SelectTipoDisposicion;
                    ingreso.ID_AUTORIDAD_INTERNA = SelectTipoAutoridadInterna;
                    ingreso.DOCINTERNACION_NUM_OFICIO = TextNumeroOficio;
                    ingreso.ID_TIPO_SEGURIDAD = SelectTipoSeguridad;

                    /***** UBICACION *****/
                    ingreso.ID_UB_CENTRO = SelectedCama.ID_CENTRO;
                    ingreso.ID_UB_EDIFICIO = SelectedCama.ID_EDIFICIO;
                    ingreso.ID_UB_SECTOR = SelectedCama.ID_SECTOR;
                    ingreso.ID_UB_CELDA = SelectedCama.ID_CELDA;
                    ingreso.ID_UB_CAMA = SelectedCama.ID_CAMA;
                    /***** INTERCONEXION *****/
                    ingreso.NUC = SelectIngreso.NUC;
                    ingreso.ID_PERSONA_PG = SelectIngreso.ID_PERSONA_PG;

                    if (SelectedCama != Ingreso.CAMA)
                    {
                        camaVieja = new CAMA()
                        {
                            ID_CAMA = Ingreso.CAMA.ID_CAMA,
                            ID_CELDA = Ingreso.CAMA.ID_CELDA,
                            ID_CENTRO = Ingreso.CAMA.ID_CENTRO,
                            ID_EDIFICIO = Ingreso.CAMA.ID_EDIFICIO,
                            ID_SECTOR = Ingreso.CAMA.ID_SECTOR,
                            DESCR = Ingreso.CAMA.DESCR,
                            ESTATUS = "S"
                        };
                    }
                    else
                        camaVieja = null;
                    camaNueva = new CAMA()
                    {
                        ID_CAMA = SelectedCama.ID_CAMA,
                        ID_CELDA = SelectedCama.ID_CELDA,
                        ID_CENTRO = SelectedCama.ID_CENTRO,
                        ID_EDIFICIO = SelectedCama.ID_EDIFICIO,
                        ID_SECTOR = SelectedCama.ID_SECTOR,
                        DESCR = SelectedCama.DESCR,
                        ESTATUS = "N"
                    };

                    if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                    {
                        var _traslado_detalle = SelectIngreso.TRASLADO_DETALLE.FirstOrDefault(w => w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO
                        && w.ID_INGRESO == SelectIngreso.ID_INGRESO);
                        if (_traslado_detalle != null)
                        {
                            traslado = new TRASLADO
                            {
                                ID_CENTRO = _traslado_detalle.TRASLADO.ID_CENTRO,
                                AUTORIZA_TRASLADO = _traslado_detalle.TRASLADO.AUTORIZA_TRASLADO,
                                CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value == Parametro.ID_EMISOR_OTROS ? DTCentroNombre : new cEmisor().Obtener(DTCentroOrigen.Value).DESCR,
                                ID_CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value,
                                ID_ESTATUS = "FI",
                                ID_MOTIVO = DTMotivo.Value,
                                ID_TRASLADO = _traslado_detalle.TRASLADO.ID_TRASLADO,
                                JUSTIFICACION = DTJustificacion,
                                OFICIO_AUTORIZACION = DTNoOficio,
                                ORIGEN_TIPO = _traslado_detalle.TRASLADO.ORIGEN_TIPO,
                                TRASLADO_FEC = _traslado_detalle.TRASLADO.TRASLADO_FEC
                            };
                            traslado_detalle_nuevo = new List<TRASLADO_DETALLE>(){ new TRASLADO_DETALLE {
                                ID_ANIO=_traslado_detalle.ID_ANIO,
                                ID_CENTRO=_traslado_detalle.ID_CENTRO,
                                ID_IMPUTADO=_traslado_detalle.ID_IMPUTADO,
                                ID_INGRESO=_traslado_detalle.ID_INGRESO,
                                ID_TRASLADO=_traslado_detalle.ID_TRASLADO,
                                ID_CENTRO_TRASLADO=_traslado_detalle.ID_CENTRO_TRASLADO,
                                ID_ESTATUS="FI",
                                TRASLADO=traslado  
                            }};
                            ingreso.TRASLADO_DETALLE = traslado_detalle_nuevo;
                        }
                        else
                        {
                            traslado = new TRASLADO
                            {
                                ID_CENTRO = SelectIngreso.ID_UB_CENTRO.Value,
                                AUTORIZA_TRASLADO = Autoridad_Traslado,
                                CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value == Parametro.ID_EMISOR_OTROS ? DTCentroNombre : new cEmisor().Obtener(DTCentroOrigen.Value).DESCR,
                                ID_CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value,
                                ID_ESTATUS = "FI",
                                ID_MOTIVO = DTMotivo.Value,
                                JUSTIFICACION = DTJustificacion,
                                OFICIO_AUTORIZACION = DTNoOficio,
                                ORIGEN_TIPO = "F",
                                TRASLADO_FEC = SelectIngreso.FEC_INGRESO_CERESO.Value
                            };
                            traslado_detalle_nuevo = new List<TRASLADO_DETALLE>(){ new TRASLADO_DETALLE {
                                ID_ANIO=SelectIngreso.ID_ANIO,
                                ID_CENTRO=SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO=SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO=SelectIngreso.ID_INGRESO,
                                ID_CENTRO_TRASLADO=SelectIngreso.ID_UB_CENTRO.Value,
                                ID_ESTATUS="FI",
                                TRASLADO=traslado 
                            }};
                            ingreso.TRASLADO_DETALLE = traslado_detalle_nuevo;
                        }
                    }
                    else
                    {
                        var _traslado_detalle = SelectIngreso.TRASLADO_DETALLE.FirstOrDefault(w => w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO
                        && w.ID_INGRESO == SelectIngreso.ID_INGRESO);
                        if (_traslado_detalle != null && (_traslado_detalle.ID_ESTATUS != "CA" ? _traslado_detalle.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            traslado = new TRASLADO
                            {
                                ID_CENTRO = _traslado_detalle.TRASLADO.ID_CENTRO,
                                AUTORIZA_TRASLADO = _traslado_detalle.TRASLADO.AUTORIZA_TRASLADO,
                                CENTRO_ORIGEN_FORANEO = _traslado_detalle.TRASLADO.CENTRO_ORIGEN_FORANEO,
                                ID_CENTRO_ORIGEN_FORANEO = _traslado_detalle.TRASLADO.ID_CENTRO_ORIGEN_FORANEO,
                                ID_ESTATUS = "CA",
                                ID_MOTIVO = _traslado_detalle.TRASLADO.ID_MOTIVO,
                                ID_TRASLADO = _traslado_detalle.TRASLADO.ID_TRASLADO,
                                JUSTIFICACION = _traslado_detalle.TRASLADO.JUSTIFICACION,
                                OFICIO_AUTORIZACION = _traslado_detalle.TRASLADO.OFICIO_AUTORIZACION,
                                ORIGEN_TIPO = _traslado_detalle.TRASLADO.ORIGEN_TIPO,
                                TRASLADO_FEC = _traslado_detalle.TRASLADO.TRASLADO_FEC
                            };
                            traslado_detalle_nuevo = new List<TRASLADO_DETALLE>(){ new TRASLADO_DETALLE {
                                ID_ANIO=_traslado_detalle.ID_ANIO,
                                ID_CENTRO=_traslado_detalle.ID_CENTRO,
                                ID_IMPUTADO=_traslado_detalle.ID_IMPUTADO,
                                ID_INGRESO=_traslado_detalle.ID_INGRESO,
                                ID_TRASLADO=_traslado_detalle.ID_TRASLADO,
                                ID_CENTRO_TRASLADO=_traslado_detalle.ID_CENTRO_TRASLADO,
                                ID_ESTATUS="CA",
                                TRASLADO=traslado  
                            }};
                            ingreso.TRASLADO_DETALLE = traslado_detalle_nuevo;
                        }
                    }
                    new cIngreso().ActualizarIngresoYCama(ingreso, camaNueva, camaVieja);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region [ABC ALIAS]
        private void GuardarAlias(Object obj = null)
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
                        //ListAlias.RemoveAt(IndexAlias);
                        //ListAlias.Add(new ALIAS { PATERNO = PaternoAlias, MATERNO = MaternoAlias, NOMBRE = NombreAlias });
                        //SelectAlias = null;
                    }
                }
                VisibleAlias = false;
                PaternoAlias = string.Empty;
                MaternoAlias = string.Empty;
                NombreAlias = string.Empty;

                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                StaticSourcesViewModel.SourceChanged = true;
                base.ClearRules();
            }
        }
        private void EliminarAlias()
        {
            if (SelectAlias != null && SelectAlias.IMPUTADO == null)
                ListAlias.Remove(SelectAlias);
            SelectAlias = null;
        }
        #endregion

        #region [ABC APODO]
        private void GuardarApodo(Object obj = null)
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
                        //ListApodo.RemoveAt(IndexApodo);
                        //ListApodo.Add(new APODO { APODO1 = Apodo });
                        //SelectApodo = null;
                    }
                }
                Apodo = string.Empty;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
            }
        }

        private void EliminarApodo()
        {
            if (SelectApodo != null && SelectApodo.IMPUTADO == null)
                ListApodo.Remove(SelectApodo);
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
            #region comentado
            //ListBuscarRelacionInterno = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() => (new cIngreso()).ObtenerTodos(GlobalVar.gCentro, string.IsNullOrEmpty(NombreBuscarRelacionInterno) ? string.Empty : NombreBuscarRelacionInterno, string.IsNullOrEmpty(PaternoBuscarRelacionInterno) ? string.Empty : PaternoBuscarRelacionInterno, string.IsNullOrEmpty(MaternoBuscarRelacionInterno) ? string.Empty : MaternoBuscarRelacionInterno, 4));
            //if (ListBuscarRelacionInterno.Count > 0)
            //    EmptyBuscarRelacionInternoVisible = false;
            //else
            //    EmptyBuscarRelacionInternoVisible = true;
            #endregion
        }

        private void AgregarRelacionInterno()
        {
            if (SelectBuscarRelacionInterno != null)
            {
                if (ListRelacionPersonalInterno == null)
                    ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
                //ListRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO { INGRESO = SelectBuscarRelacionInterno });
                if (ListRelacionPersonalInterno.Count(w => w.INGRESO.ID_CENTRO == SelectBuscarRelacionInterno.ID_CENTRO && w.INGRESO.ID_ANIO == SelectBuscarRelacionInterno.ID_ANIO && w.INGRESO.ID_IMPUTADO == SelectBuscarRelacionInterno.ID_IMPUTADO) == 0)
                {
                    ListRelacionPersonalInterno.Add(new RELACION_PERSONAL_INTERNO { INGRESO = SelectBuscarRelacionInterno });
                    LimpiarBusquedaRelacionInterna();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                }
                else
                { 
                    new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado ya se encuentra registrado");
                    return;
                }
                VisibleRelacionInterno = false;
                LimpiarBusquedaRelacionInterna();
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
            }
        }

        private void LimpiarBusquedaRelacionInterna()
        {
            NombreBuscarRelacionInterno = PaternoBuscarRelacionInterno = MaternoBuscarRelacionInterno = string.Empty;
            SelectBuscarRelacionInterno = null;
            ListBuscarRelacionInterno = null;
            VisibleRelacionInterno = false;
        }

        private void CancelarBuscarRelacionInterno()
        {
            VisibleRelacionInterno = false;
            LimpiarBusquedaRelacionInterna();
        }

        private void GuardarRelacionInterno(Object obj = null)
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
                VisibleRelacionInterno = false;
                PaternoRelacionInterno = MaternoRelacionInterno = NombreRelacionInterno = NotaRelacionInterno = string.Empty;
            }
        }

        private void EliminarRelacionInterno()
        {
            if (selectRelacionPersonalInterno != null && selectRelacionPersonalInterno.IMPUTADO == null)
                listRelacionPersonalInterno.Remove(selectRelacionPersonalInterno);
            selectRelacionPersonalInterno = null;
        }
        #endregion

        #region [TREEVIEW]
        public async Task<bool> TreeViewViewModel()
        {
            _TreeList = new List<TreeViewList>();

            var objCama = new cCama();
            if (Ingreso == null)
                return false;

            var Registros = await StaticSourcesViewModel.CargarDatosAsync<List<CAMA>>(() => objCama.GetData().OrderBy(o => new { o.ID_CENTRO, o.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO, o.CELDA.SECTOR.ID_SECTOR, o.CELDA.ID_CELDA, o.ID_CAMA }).Where(w => w.ID_CENTRO == Centro.ID_CENTRO && w.ESTATUS == "S").ToList());
            Registros.AddRange(objCama.GetData().Where(w => w.ID_CAMA == ingreso.ID_UB_CAMA && w.ID_CELDA == ingreso.ID_UB_CELDA && w.ID_CENTRO == ingreso.ID_UB_CENTRO && w.ID_EDIFICIO == ingreso.ID_UB_EDIFICIO && w.ID_SECTOR == ingreso.ID_UB_SECTOR).ToList());
            Registros = Registros.OrderBy(o => o.ID_CENTRO).ThenBy(o => o.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO).ThenBy(o => o.CELDA.SECTOR.ID_SECTOR).ThenBy(o => o.CELDA.ID_CELDA).ThenBy(o => o.ID_CAMA).ToList();


            var c = Registros.Where(w => w == ingreso.CAMA).ToList();

            var NodoCentro = new CENTRO();
            var NodoEdificio = new EDIFICIO();
            var NodoSector = new SECTOR();
            var NodoCelda = new CELDA();
            var NodoCama = new CAMA();
            var ItemTreeViewCentro = new TreeViewList();
            var ItemTreeViewEdificio = new List<TreeViewList>();
            var ItemTreeViewSector = new List<TreeViewList>();
            var ItemTreeViewCelda = new List<TreeViewList>();
            var ItemTreeViewCama = new List<TreeViewList>();
            var _MismoCentro = true;
            var _MismoEdificio = true;
            var _MismoSector = true;
            var _MismoCelda = true;
            var _MismoCama = true;

            foreach (var item in Registros)
            {
                _MismoCentro = true;
                _MismoEdificio = true;
                _MismoSector = true;
                _MismoCelda = true;
                _MismoCama = true;

                if (NodoCentro != (item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR.EDIFICIO != null ? item.CELDA.SECTOR.EDIFICIO.CENTRO != null ? item.CELDA.SECTOR.EDIFICIO.CENTRO : null : null : null : null : null))
                {
                    NodoCentro = item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR.EDIFICIO != null ? item.CELDA.SECTOR.EDIFICIO.CENTRO != null ? item.CELDA.SECTOR.EDIFICIO.CENTRO : null : null : null : null : null;
                    ItemTreeViewEdificio = new List<TreeViewList>();
                    _MismoCentro = false;
                }

                if (NodoEdificio != (item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR.EDIFICIO != null ? item.CELDA.SECTOR.EDIFICIO : null : null : null : null))
                {
                    NodoEdificio = item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR.EDIFICIO != null ? item.CELDA.SECTOR.EDIFICIO : null : null : null : null;
                    ItemTreeViewSector = new List<TreeViewList>();
                    _MismoEdificio = false;
                }

                if (NodoSector != (item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR : null : null : null))
                {
                    NodoSector = item != null ? item.CELDA != null ? item.CELDA.SECTOR != null ? item.CELDA.SECTOR : null : null : null;
                    ItemTreeViewCelda = new List<TreeViewList>();
                    _MismoSector = false;
                }

                if (NodoCelda != (item != null ? item.CELDA != null ? item.CELDA : null : null))
                {
                    NodoCelda = item != null ? item.CELDA != null ? item.CELDA : null : null;
                    ItemTreeViewCama = new List<TreeViewList>();
                    _MismoCelda = false;
                }

                if (NodoCama != item)
                {
                    NodoCama = item;
                    _MismoCama = false;
                }

                if (!_MismoCama)
                {
                    ItemTreeViewCama.Add(new TreeViewList
                    {
                        Icon = "F1 M 17.4167,25.3333L 23.75,25.3333L 23.75,41.1667L 58.5833,41.1667L 58.5833,45.9167L 58.5833,50.6667L 53.8333,50.6667L 53.8333,45.9167L 23.75,45.9167L 23.75,50.6667L 17.4167,50.6667L 17.4167,25.3333 Z M 57,39.5833L 35.625,39.5833L 35.625,30.0834C 43.0139,30.0834 48.0278,30.0834 51.9861,31.6667C 55.9444,33.25 56.4722,36.4167 57,39.5833 Z M 25.3333,35.625L 31.6667,36.4167L 31.6667,39.5833L 25.3333,39.5833L 25.3333,35.625 Z M 28.5,28.5C 30.2489,28.5 31.6667,29.9178 31.6667,31.6667C 31.6667,33.4156 30.2489,34.8333 28.5,34.8333C 26.7511,34.8333 25.3333,33.4156 25.3333,31.6667C 25.3333,29.9178 26.7511,28.5 28.5,28.5 Z M 32.8542,30.0833L 34.8333,30.0833L 34.8333,39.5833L 32.8542,39.5833L 32.8542,30.0833 Z",
                        IsRoot = false,
                        IsCheck = false,
                        Text = item.ID_CAMA.ToString(),
                        Value = item
                    });
                }

                if (!_MismoCelda)
                {
                    ItemTreeViewCelda.Add(new TreeViewList
                    {
                        Icon = "F1 M 54,57L 22,57L 22,19L 54,19L 54,57 Z M 50,23L 39,23L 39,37L 50,37L 50,23 Z M 26,23L 26,37L 37,37L 37,23L 26,23 Z M 26,53L 37,53L 37,39L 26,39L 26,53 Z M 50,53L 50,39L 39,39L 39,53L 50,53 Z ",
                        IsRoot = false,
                        IsCheck = false,
                        Text = NodoCelda.ID_CELDA,
                        Value = NodoCelda,
                        Node = ItemTreeViewCama
                    });
                }

                if (!_MismoSector)
                {
                    ItemTreeViewSector.Add(new TreeViewList
                    {
                        Icon = "F1 M 54.2499,34L 42,34L 42,21.7501L 45.9999,17.7501L 45.9999,26.7501L 53.9999,18.7501L 57.2499,22.0001L 49.2499,30.0001L 58.2499,30.0001L 54.2499,34 Z M 34,21.7501L 34,34L 21.75,34L 17.75,30.0001L 26.75,30.0001L 18.75,22.0001L 22,18.7501L 30,26.7501L 30,17.7501L 34,21.7501 Z M 21.75,42L 34,42L 34,54.25L 30,58.25L 30,49.25L 22,57.25L 18.75,54L 26.75,46L 17.75,46L 21.75,42 Z M 42,54.25L 42,42L 54.2499,42L 58.2499,46L 49.2499,46.0001L 57.2499,54L 53.9999,57.25L 45.9999,49.25L 45.9999,58.25L 42,54.25 Z ",
                        IsRoot = false,
                        IsCheck = false,
                        Text = NodoSector.DESCR.Trim(),
                        Value = NodoSector,
                        Node = ItemTreeViewCelda
                    });
                }

                if (!_MismoEdificio)
                {
                    ItemTreeViewEdificio.Add(new TreeViewList
                    {
                        Icon = "F1 M 44.3333,30.0833L 57,30.0833L 57,57L 44.3333,57L 44.3333,30.0833 Z M 46.3125,35.2292L 46.3125,38L 49.0833,38L 49.0833,35.2292L 46.3125,35.2292 Z M 52.25,35.2292L 52.25,38L 55.0208,38L 55.0208,35.2292L 52.25,35.2292 Z M 46.3125,39.9792L 46.3125,42.75L 49.0833,42.75L 49.0833,39.9792L 46.3125,39.9792 Z M 52.25,39.9792L 52.25,42.75L 55.0208,42.75L 55.0208,39.9792L 52.25,39.9792 Z M 46.3125,44.7292L 46.3125,47.5L 49.0833,47.5L 49.0833,44.7292L 46.3125,44.7292 Z M 52.25,44.7292L 52.25,47.5L 55.0208,47.5L 55.0208,44.7292L 52.25,44.7292 Z M 46.3125,49.4792L 46.3125,52.25L 49.0833,52.25L 49.0833,49.4792L 46.3125,49.4792 Z M 52.25,49.4792L 52.25,52.25L 55.0208,52.25L 55.0208,49.4792L 52.25,49.4792 Z M 23.75,25.3333L 25.3333,22.1667L 26.9167,22.1667L 26.9167,18.2084L 28.5,18.2084L 28.5,22.1667L 31.6667,22.1667L 31.6667,18.2084L 33.25,18.2084L 33.25,22.1667L 34.8333,22.1667L 36.4167,25.3333L 36.4167,34.8334L 38.7917,34.8333L 41.1667,37.2083L 41.1667,57L 19,57L 19,37.2083L 21.375,34.8333L 23.75,34.8334L 23.75,25.3333 Z M 25.7291,27.3125L 25.7291,30.0834L 28.1041,30.0834L 28.1041,27.3125L 25.7291,27.3125 Z M 32.0625,27.3125L 32.0625,30.0834L 34.4375,30.0834L 34.4375,27.3125L 32.0625,27.3125 Z M 25.7291,32.0625L 25.7291,34.8334L 28.1041,34.8334L 28.1041,32.0625L 25.7291,32.0625 Z M 32.0625,32.0625L 32.0625,34.8334L 34.4375,34.8334L 34.4375,32.0625L 32.0625,32.0625 Z M 30.875,39.9792L 28.8958,39.9792L 28.8958,42.75L 30.875,42.75L 30.875,39.9792 Z M 24.5416,39.9792L 24.5416,42.75L 26.9166,42.75L 26.9166,39.9792L 24.5416,39.9792 Z M 36.0208,39.9792L 33.25,39.9792L 33.25,42.75L 36.0208,42.75L 36.0208,39.9792 Z M 30.875,44.7292L 28.8958,44.7292L 28.8958,47.5L 30.875,47.5L 30.875,44.7292 Z M 26.9166,44.7292L 24.5416,44.7292L 24.5416,47.5L 26.9166,47.5L 26.9166,44.7292 Z M 36.0208,44.7292L 33.25,44.7292L 33.25,47.5L 36.0208,47.5L 36.0208,44.7292 Z M 30.875,49.4792L 28.8958,49.4792L 28.8958,52.25L 30.875,52.25L 30.875,49.4792 Z M 26.9166,49.4792L 24.5416,49.4792L 24.5417,52.25L 26.9167,52.25L 26.9166,49.4792 Z M 36.0208,49.4792L 33.25,49.4792L 33.25,52.25L 36.0208,52.25L 36.0208,49.4792 Z ",
                        IsRoot = false,
                        IsCheck = false,
                        Text = NodoEdificio.DESCR.Trim(),
                        Value = NodoEdificio,
                        Node = ItemTreeViewSector
                    });
                }

                if (!_MismoCentro)
                {
                    ItemTreeViewCentro.Icon = "F1 M 28.5,20.5833L 47.5,20.5833L 47.5,23.75L 28.5,23.75L 28.5,20.5833 Z M 49.0833,31.6667L 64.9166,31.6667L 64.9166,34.8334L 49.0833,34.8334L 49.0833,31.6667 Z M 28.5,25.3334L 34.8333,25.3334L 41.1666,25.3334L 47.5,25.3334L 47.5,52.25L 41.1666,52.25L 41.1666,42.75L 34.8333,42.75L 34.8333,52.25L 28.5,52.25L 28.5,25.3334 Z M 49.0833,52.25L 49.0833,36.4167L 53.8333,36.4167L 60.1666,36.4167L 64.9166,36.4167L 64.9166,52.25L 60.1666,52.25L 60.1666,44.3333L 53.8333,44.3333L 53.8333,52.25L 49.0833,52.25 Z M 11.0833,52.25L 11.0833,44.3333L 11.0833,41.1667L 19.7917,34.8334L 26.9167,41.1667L 26.9167,44.3333L 26.9167,52.25L 22.1667,52.25L 22.1667,44.3333L 15.8333,44.3333L 15.8333,52.25L 11.0833,52.25 Z M 19.7916,29.6875L 26.9166,36.0209L 26.9166,39.1875L 19.7916,32.8542L 9.49999,40.375L 9.49999,37.2084L 19.7916,29.6875 Z ";
                    ItemTreeViewCentro.IsRoot = true;
                    ItemTreeViewCentro.IsCheck = false;
                    ItemTreeViewCentro.IsNodeExpanded = true;
                    ItemTreeViewCentro.Text = NodoCentro.DESCR.Trim();
                    ItemTreeViewCentro.Value = NodoCentro;
                    ItemTreeViewCentro.Node = ItemTreeViewEdificio;
                }
            }

            TreeList.Add(ItemTreeViewCentro);
            TreeList.Expand();

            LoadSelectedTreeViewItem(Ingreso, Imputado);
            return true;
        }

        private void SetIsSelectedProperty(TreeViewList TreeViewItem, bool isSelected)
        {
            if (SelectedItem != null)
            {
                if (SelectedItem != TreeViewItem)
                    SelectedItem.IsCheck = false;
                if (SelectedItem != TreeViewItem && isSelected)
                {
                    TreeList.UnCheckAll();
                    SelectedCama = null;
                }
            }
            else
                if (isSelected)
                {
                    TreeList.UnCheckAll();
                    SelectedCama = null;
                }

            SelectedItem = TreeViewItem;
            if (!isSelected)
            {
                TreeList.UnCheckAll();
                SelectedCama = null;
            }
            else
            {
                if (TreeViewItem.HasChildren())
                {
                    TreeViewItem.IsCheck = false;
                    TreeViewItem.IsNodeExpanded = true;
                }
                else
                {
                    TreeList.Collapse();
                    TreeViewItem.IsCheck = isSelected;
                    TreeList.Restart().Expand();
                    SelectedCama = (CAMA)TreeViewItem.Value;
                }
            }
        }

        private async void LoadSelectedTreeViewItem(INGRESO Item, IMPUTADO Item2)
        {
            if (Item == null || Item2 == null)
                return;
            TreeList.UnCheckAll();
            var TreeViewItem = await StaticSourcesViewModel.CargarDatosAsync<INGRESO>(() => (new cIngreso()).GetData().Where(w => w.ID_INGRESO == Item.ID_INGRESO && w.ID_IMPUTADO == Item2.ID_IMPUTADO && w.ID_ANIO == Item.ID_ANIO
                                                                && w.ID_CENTRO == Item.ID_CENTRO).FirstOrDefault());

            if (TreeViewItem == null)
                return;
            TreeList.Collapse();
            TreeList.FindElement(TreeViewItem);
            TreeList.Restart().Expand();
        }
        #endregion

        #region [WebCam]
        private void OnUnLoad(EstatusAdministrativoView Window = null)
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


        private async void OnLoad(FotosHuellasDigitalesEstatusAdminView Window = null)
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

            if (TabFotosHuellas)
            {
                #region switch_origen
                await GuardarSwitch();
                #endregion


                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.LeftFace, Window.RightFace, Window.FrontFace });
                _ImagesToSave = new List<ImageSourceToSave>();
                var ingresobiometrico = new cIngreso().GetData().Where(wi => wi.INGRESO_BIOMETRICO.Any(w => w.ID_ANIO == Imputado.ID_ANIO && w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO && w.ID_INGRESO == SelectIngreso.ID_INGRESO && w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO)).ToList();
                if (ingresobiometrico.Any())
                {
                    ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.FrontFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                    ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.LeftFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                    ImagesToSave.Add(CamaraWeb.AgregarImagenControl(Window.RightFace, new Imagenes().ConvertByteToBitmap(ingresobiometrico.FirstOrDefault().INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_DER_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO)));
                }

                _ventanaGuardar = VentanaGuardar.FOTOSYHUELLASDIGITALES;
                StaticSourcesViewModel.SourceChanged = false;
            }
            #endregion


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
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error al tomar fotografía \n\n" + ex.Message);
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

                                if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).ToList().Any())
                                {
                                    ScannerMessage = "Huellas Actualizadas Correctamente";
                                    StaticSourcesViewModel.SourceChanged = true;
                                }
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
        private void CargarHuellas()
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
        private async void OnBuscarPorHuella(string obj = "")
        {
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaEstatusAdministrativo;
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
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar busqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            AceptarBusquedaHuellaFocus = true;
        }
        #endregion

        #region [PANDILLA]
        private async void GetPandilla()
        {
            var _pandilla = new PANDILLA() { ID_PANDILLA = -1, NOMBRE = "SELECCIONE" };
            Pandilla = Pandilla ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PANDILLA>>(() => new ObservableCollection<PANDILLA>((new cPandilla()).ObtenerTodos()));
            Pandilla.Insert(0, _pandilla);
        }
        private async void GetImputadoPandilla()
        {
            ImputadoPandilla = ImputadoPandilla ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO_PANDILLA>>(() => (new cImputadoPandilla()).Obtener(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO));
        }
        private void AgregarPandilla()
        {
            setValidacionesIdentificacionPandillas();
            SelectedPandillaValue = -1;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PANDILLA);
        }
        private void LimpiarPandilla()
        {
            SelectedPandillaValue = -1;
            NotaPandilla = string.Empty;
            SelectedImputadoPandilla = null;
            base.ClearRules();
            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PANDILLA);
        }
        private void GuardarPandilla(Object nota = null)
        {
            if (nota != null)
                NotaPandilla = ((System.Windows.Controls.TextBox)(nota)).Text;
            if (!base.HasErrors)
            {
                if (ImputadoPandilla == null)
                    ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>();

                if (SelectedImputadoPandilla != null)//editar
                {
                    SelectedImputadoPandilla.PANDILLA = SelectedPandillaItem;
                    SelectedImputadoPandilla.ID_PANDILLA = SelectedPandillaItem.ID_PANDILLA;
                    SelectedImputadoPandilla.NOTAS = NotaPandilla;
                    ImputadoPandilla = new ObservableCollection<IMPUTADO_PANDILLA>(ImputadoPandilla);
                }
                else
                    if (ImputadoPandilla.Count(w => w.ID_PANDILLA == SelectedPandillaItem.ID_PANDILLA) > 0)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La pandilla y se encuentra registrada");
                        return;
                    }
                    else
                        ImputadoPandilla.Add(new IMPUTADO_PANDILLA { ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO, PANDILLA = SelectedPandillaItem, NOTAS = NotaPandilla, ID_PANDILLA = SelectedPandillaValue });
                StaticSourcesViewModel.SourceChanged = true;
                this.LimpiarPandilla();
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");
        }
        
        private void EliminarPandilla()
        {
            //if (SelectedImputadoPandilla != null && SelectedImputadoPandilla.IMPUTADO == null)
            if (SelectedImputadoPandilla != null)
            {
                ImputadoPandilla.Remove(SelectedImputadoPandilla);
            }
        }
        private void EditarPandilla()
        {
            if (SelectedImputadoPandilla != null && SelectedImputadoPandilla.IMPUTADO == null)
            {
                SelectedPandillaValue = SelectedImputadoPandilla.ID_PANDILLA;
                NotaPandilla = SelectedImputadoPandilla.NOTAS;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PANDILLA);
            }
        }
        private bool GuardarPandillaDB()
        {
            if (ImputadoPandilla != null)
            {
                //if (ImputadoPandilla.Count > 0)
                //{
                    var pandillaList = new List<IMPUTADO_PANDILLA>();
                    foreach (var entidad in ImputadoPandilla)
                    {
                        if (entidad.IMPUTADO == null)
                            pandillaList.Add(new IMPUTADO_PANDILLA { ID_CENTRO = entidad.ID_CENTRO, ID_ANIO = entidad.ID_ANIO, ID_IMPUTADO = entidad.ID_IMPUTADO, ID_PANDILLA = entidad.ID_PANDILLA, NOTAS = entidad.NOTAS });
                    }
                    if ((new cImputadoPandilla()).InsertarPandillas(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, pandillaList))
                        return true;
                //}
                //else
                //    return true;
            }
            return false;
        }
        #endregion

        #region [SENAS_PARTICULARES]
        private void IngresoLoad(EstatusAdministrativoView Window = null)
        {
            estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
            Window.Unloaded += (s, e) => { OnUnLoad(Window); };

            PopUpsViewModels.MainWindow.ApodoView.IsVisibleChanged += (s, e) =>
            {
                try
                {
                    ApodosLoad(PopUpsViewModels.MainWindow.ApodoView);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar apodo", ex);
                }
            };
            PopUpsViewModels.MainWindow.AliasView.IsVisibleChanged += (s, e) =>
            {
                try
                {
                    AliasLoad(PopUpsViewModels.MainWindow.AliasView);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar alias", ex);
                }
            };
            PopUpsViewModels.MainWindow.RelacionView.IsVisibleChanged += (s, e) =>
            {
                try
                {
                    RelacionLoad(PopUpsViewModels.MainWindow.RelacionView);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar relacion", ex);
                }
            };
        }

        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
            CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
            await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
            ImagesToSave = new List<ImageSourceToSave>();
            if (ImagenTatuaje != null)
                CamaraWeb.AgregarImagenControl(Window.ImgSenaParticular, ImagenTatuaje);
        }
        private async Task<bool> GetRegionesCuerpo()
        {
            ListTipoTatuaje = ListTipoTatuaje ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TATUAJE>>(() => new ObservableCollection<TATUAJE>((new cTatuaje()).ObtenerTodos()));
            ListTipoTatuaje.Insert(0, new TATUAJE() { ID_TATUAJE = -1, DESCR = "SELECCIONE" });


            ListClasificacionTatuaje = ListClasificacionTatuaje ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<TATUAJE_CLASIFICACION>>(() => new ObservableCollection<TATUAJE_CLASIFICACION>((new cTatuajeClasificacion()).ObtenerTodos()));
            ListClasificacionTatuaje.Insert(0, new TATUAJE_CLASIFICACION() { ID_TATUAJE_CLA = "", DESCR = "SELECCIONE" });

            ListRegionCuerpo = ListRegionCuerpo ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<ANATOMIA_TOPOGRAFICA>>(() => new ObservableCollection<ANATOMIA_TOPOGRAFICA>((new cAnatomiaTopografica()).ObtenerTodos()));
            return true;
        }
        #endregion

        #region VALIDACIONES MANUALES
        private bool ValidarDatosGenerales()
        {
            #region DATOS_GENERALES
            if (string.IsNullOrEmpty(SelectSexo) || (SelectSexo != "F" && SelectSexo != "M"))
                return false;
            if (!SelectEstadoCivil.HasValue || !(SelectEstadoCivil >= 1))
                return false;
            if (!SelectOcupacion.HasValue || !(SelectOcupacion >= 1))
                return false;
            if (!SelectEscolaridad.HasValue || !(SelectEscolaridad >= 1))
                return false;
            if (!SelectNacionalidad.HasValue || !(SelectNacionalidad >= 1))
                return false;
            if (!SelectReligion.HasValue || !(SelectReligion >= 1))
                return false;
            if (!SelectEtnia.HasValue || !(SelectEtnia >= 1))
                return false;
            if (string.IsNullOrEmpty(TextPeso))
                return false;
            if (string.IsNullOrEmpty(TextEstatura))
                return false;
            if (!SelectedIdioma.HasValue || !(SelectedIdioma >= 1))
                return false;
            if (!SelectedDialecto.HasValue || !(SelectedDialecto >= 1))
                return false;
            #endregion

            #region DOMICILIO
            if (!SelectPais.HasValue || !(SelectPais >= 1))
                return false;
            if (!SelectEntidad.HasValue || !(SelectEntidad >= 1))
                return false;
            if (!SelectMunicipio.HasValue || !(SelectMunicipio >= 1))
                return false;
            if (SelectColoniaItem == null || !(SelectColoniaItem.ID_COLONIA >= 1))
                return false;
            if (string.IsNullOrEmpty(TextCalle))
                return false;
            if (!TextNumeroExterior.HasValue || !(TextNumeroExterior.Value > 0))
                return false;
            if (string.IsNullOrEmpty(AniosEstado))
                return false;
            if (string.IsNullOrEmpty(MesesEstado))
                return false;
            if (TextTelefono == null)
                return false;
            if (!TextCodigoPostal.HasValue || !(TextCodigoPostal.Value >= 1))
                return false;
            #endregion

            #region NACIMIENTO
            if (!SelectPaisNacimiento.HasValue || !(SelectPaisNacimiento >= 1))
                return false;
            if (!SelectEntidadNacimiento.HasValue || !(SelectEntidadNacimiento >= 1))
                return false;
            if (!SelectMunicipioNacimiento.HasValue || !(SelectMunicipioNacimiento >= 1))
                return false;
            if (TextFechaNacimiento == null/* || !(new Fechas().CalculaEdad(TextFechaNacimiento) >= 18)*/)
                return false;
            #endregion

            #region PADRE
            if (string.IsNullOrEmpty(TextPadrePaterno))
                return false;
            if (string.IsNullOrEmpty(TextPadreMaterno))
                return false;
            if (string.IsNullOrEmpty(TextPadreNombre))
                return false;

            if (!MismoDomicilioPadre && !CheckPadreFinado)
            {
                #region DOMICILIO PADRE
                if (!SelectPaisDomicilioPadre.HasValue || !(SelectPaisDomicilioPadre >= 1))
                    return false;
                if (!SelectEntidadDomicilioPadre.HasValue || !(SelectEntidadDomicilioPadre >= 1))
                    return false;
                if (!SelectMunicipioDomicilioPadre.HasValue || !(SelectMunicipioDomicilioPadre >= 1))
                    return false;
                if (SelectColoniaItemDomicilioPadre == null || !(SelectColoniaItemDomicilioPadre.ID_COLONIA >= 1))
                    return false;
                if (string.IsNullOrEmpty(TextCalleDomicilioPadre))
                    return false;
                if (!TextNumeroExteriorDomicilioPadre.HasValue || !(TextNumeroExteriorDomicilioPadre.Value > 0))
                    return false;
                if (!TextCodigoPostalDomicilioPadre.HasValue || !(TextCodigoPostalDomicilioPadre.Value > 0))
                    return false;
                #endregion
            }
            #endregion

            #region MADRE
            if (string.IsNullOrEmpty(TextMadrePaterno))
                return false;
            if (string.IsNullOrEmpty(TextMadreMaterno))
                return false;
            if (string.IsNullOrEmpty(TextMadreNombre))
                return false;

            if (!MismoDomicilioMadre && !CheckMadreFinado)
            {
                #region DOMICILIO PADRE
                if (!SelectPaisDomicilioMadre.HasValue || !(SelectPaisDomicilioMadre >= 1))
                    return false;
                if (!SelectEntidadDomicilioMadre.HasValue || !(SelectEntidadDomicilioMadre >= 1))
                    return false;
                if (!SelectMunicipioDomicilioMadre.HasValue || !(SelectMunicipioDomicilioMadre >= 1))
                    return false;
                if (SelectColoniaItemDomicilioMadre == null || !(SelectColoniaItemDomicilioMadre.ID_COLONIA >= 1))
                    return false;
                if (string.IsNullOrEmpty(TextCalleDomicilioMadre))
                    return false;
                if (!TextNumeroExteriorDomicilioMadre.HasValue || !(TextNumeroExteriorDomicilioMadre.Value > 0))
                    return false;
                if (!TextCodigoPostalDomicilioMadre.HasValue || !(TextCodigoPostalDomicilioMadre.Value > 0))
                    return false;
                #endregion
            }

            #endregion


            return true;
        }

        private bool ValidarMediaFiliacion()
        {
            #region SeniasGenerales
            if (!(SelectComplexion >= 0))
                return false;
            if (!(SelectColorPiel >= 0))
                return false;
            if (!(SelectCara >= 0))
                return false;
            #endregion

            #region Sangre
            if (!(SelectTipoSangre >= 0))
                return false;
            if (!(SelectFactorSangre >= 0))
                return false;
            #endregion

            #region Cabello
            if (!(SelectCantidadCabello >= 0))
                return false;
            if (!(SelectColorCabello >= 0))
                return false;
            if (!(SelectFormaCabello >= 0))
                return false;
            if (!(SelectCalvicieCabello >= 0))
                return false;
            if (!(SelectImplantacionCabello >= 0))
                return false;
            #endregion

            #region Frente
            if (!(SelectAlturaFrente >= 0))
                return false;
            if (!(SelectInclinacionFrente >= 0))
                return false;
            if (!(SelectAnchoFrente >= 0))
                return false;
            #endregion

            #region Cejas
            if (!(SelectDireccionCeja >= 0))
                return false;
            if (!(SelectImplantacionCeja >= 0))
                return false;
            if (!(SelectFormaCeja >= 0))
                return false;
            if (!(SelectTamanioCeja >= 0))
                return false;
            #endregion

            #region Ojos
            if (!(SelectColorOjos >= 0))
                return false;
            if (!(SelectFormaOjos >= 0))
                return false;
            if (!(SelectTamanioOjos >= 0))
                return false;
            #endregion

            #region Nariz
            if (!(SelectRaizNariz >= 0))
                return false;
            if (!(SelectDorsoNariz >= 0))
                return false;
            if (!(SelectAnchoNariz >= 0))
                return false;
            if (!(SelectBaseNariz >= 0))
                return false;
            if (!(SelectAlturaNariz >= 0))
                return false;
            #endregion

            #region Labios
            if (!(SelectEspesorLabio >= 0))
                return false;
            if (!(SelectAlturaLabio >= 0))
                return false;
            if (!(SelectProminenciaLabio >= 0))
                return false;
            #endregion

            #region Boca
            if (!(SelectTamanioBoca >= 0))
                return false;
            if (!(SelectComisuraBoca >= 0))
                return false;
            #endregion

            #region Labios
            if (!(SelectFormaMenton >= 0))
                return false;
            if (!(SelectTipoMenton >= 0))
                return false;
            if (!(SelectInclinacionMenton >= 0))
                return false;
            #endregion

            #region OrejaDerecha
            if (!(SelectFormaOrejaDerecha >= 0))
                return false;

            #region Helix
            if (!(SelectHelixOriginalOrejaDerecha >= 0))
                return false;
            if (!(SelectHelixSuperiorOrejaDerecha >= 0))
                return false;
            if (!(SelectHelixPosteriorOrejaDerecha >= 0))
                return false;
            if (!(SelectHelixAdherenciaOrejaDerecha >= 0))
                return false;
            #endregion

            #region Lobulo
            if (!(SelectLobuloContornoOrejaDerecha >= 0))
                return false;
            if (!(SelectLobuloAdherenciaOrejaDerecha >= 0))
                return false;
            if (!(SelectLobuloParticularidadOrejaDerecha >= 0))
                return false;
            if (!(SelectLobuloDimensionOrejaDerecha >= 0))
                return false;
            #endregion

            #endregion


            return true;
        }

        private bool ValidarIngreso()
        {
            if (!(FechaCeresoIngreso > System.DateTime.Parse("01/01/1950")))
                return false;
            if (!(SelectTipoIngreso >= 0))
                return false;
            if (string.IsNullOrEmpty(SelectClasificacionJuridica))
                return false;
            if (!SelectEstatusAdministrativo.HasValue || !(SelectEstatusAdministrativo > 0))
                return false;
            if (!IngresoDelito.HasValue || !(IngresoDelito.Value > 0))
                return false;
            if (SelectedCama == null)
                return false;
            if (!SelectTipoAutoridadInterna.HasValue || !(SelectTipoAutoridadInterna.Value >= 0))
                return false;
            if (SelectTipoSeguridad == null || string.IsNullOrWhiteSpace(SelectTipoSeguridad))
                return false;
            if (!SelectTipoDisposicion.HasValue || !(SelectTipoDisposicion.Value >= 0))
                return false;
            if (TextNumeroOficio == null || string.IsNullOrWhiteSpace(TextNumeroOficio))
                return false;
            return true;
        }
        #endregion

        #region SWITCH ORIGEN
        private async Task GuardarSwitch()
        {
            switch (_ventanaGuardar)
            {
                case VentanaGuardar.APODOS_ALIAS_REFERENCIAS:
                    if (await StaticSourcesViewModel.OperacionesAsync("Actualizando datos de apodos, alias y relaciones personales", GuardarApodosAlias))
                    {
                        await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                        {
                            Imputado.RELACION_PERSONAL_INTERNO = new cRelacionPersonalInterno().Obtener(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                            Imputado.ALIAS = new cAlias().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                            Imputado.APODO = new cApodo().ObtenerTodosXImputado(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                            return true;
                        });
                        StaticSourcesViewModel.Mensaje("APODOS ALIAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }
                    else
                        StaticSourcesViewModel.Mensaje("APODOS ALIAS", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    break;
                case VentanaGuardar.DATOS_GENERALES:

                    if (ValidarDatosGenerales())
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync("Actualizando datos generales", GuardarDatosGenerales))
                        {
                            await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                            {
                                Imputado = new cImputado().Obtener(Imputado.ID_IMPUTADO, Imputado.ID_ANIO, Imputado.ID_CENTRO).FirstOrDefault();
                                return true;
                            });
                            StaticSourcesViewModel.Mensaje("DATOS GENERALES", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("DATOS GENERALES", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                    break;
                case VentanaGuardar.FOTOSYHUELLASDIGITALES:
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando datos de fotos y huellas", GuardarFotosYHuellas))
                    {
                        await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                        {
                            Imputado = new cImputado().Obtener(Imputado.ID_IMPUTADO, Imputado.ID_ANIO, Imputado.ID_CENTRO).FirstOrDefault();
                            return true;
                        });
                        if (CamaraWeb != null)
                        {
                            await CamaraWeb.ReleaseVideoDevice();
                            CamaraWeb = null;
                        }
                        StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }
                    else
                        StaticSourcesViewModel.Mensaje("FOTOS Y HUELLAS", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    break;
                case VentanaGuardar.MEDIA_FILIACION:
                    if (ValidarMediaFiliacion())
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando datos de media filiación", GuardarImputadoFiliacion))
                        {
                            await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                            {
                                Imputado.IMPUTADO_FILIACION = new cImputadoFiliacion().ObtenerTodos(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO);
                                return true;
                            });
                            StaticSourcesViewModel.Mensaje("IMPUTADO FILACIÓN", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        }
                        else
                            StaticSourcesViewModel.Mensaje("IMPUTADO FILACIÓN", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                    break;
                case VentanaGuardar.PANDILLAS:
                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando datos de media filiación", GuardarPandillaDB))
                    {
                        await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                        {
                            Imputado.IMPUTADO_PANDILLA = new cImputadoPandilla().Obtener(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO);
                            return true;
                        });
                        StaticSourcesViewModel.Mensaje("PANDILLA", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                    }
                    else
                        StaticSourcesViewModel.Mensaje("PANDILLA", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    break;
                case VentanaGuardar.INGRESO:
                    if (ValidarIngreso())
                    {
                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando datos de ingreso", addDatosIngreso))
                        {
                            await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                            {
                                SelectIngreso = new cIngreso().Obtener(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                                return true;
                            });
                            StaticSourcesViewModel.Mensaje("IMPUTADO INGRESO", "INFORMACIÓN GRABADA EXITOSAMENTE", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                        }

                        else
                            StaticSourcesViewModel.Mensaje("IMPUTADO INGRESO", "OCURRIÓ UN ERROR AL GUARDAR", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                    }
                    break;
            }
        }

        #endregion

        #region [TAB_LOADS]
        private async void DatosIngresoInternoLoad(DatosIngresoInternoEstatusAdminView Window = null)
        {
            if (IsSelectedDatosIngreso)
            {
                await GuardarSwitch();
                if (BanderaEntrada)
                    setValidacionesDatosIngreso();
                OnPropertyChanged();
                _ventanaGuardar = VentanaGuardar.INGRESO;
                StaticSourcesViewModel.SourceChanged = false;
            }
        }


        private async void DatosIdentificacionLoad(DatosGeneralesIdentificacionEstatusAdminView Window = null)
        {
            if (IsSelectedIdentificacion)
            {
                await GuardarSwitch();
                await this.getDatosGenerales();
                setValidacionesIdentificacionDatosGenerales();
                getDatosGeneralesIdentificacion();
                OnPropertyChanged();
                _ventanaGuardar = VentanaGuardar.DATOS_GENERALES;
                ChecarValidaciones();
                StaticSourcesViewModel.SourceChanged = false;
            }
        }
        private async void MediaFiliacionLoad(MediaFiliacionEstatusAdminView Window = null)
        {
            if (IsSelectedIdentificacion && TabMediaFiliacion)
            {
                await GuardarSwitch();
                await this.GetMediaFiliacion();
                setValidacionesIdentificacionMediaFiliacion();
                this.getDatosMediaFiliacion();
                OnPropertyChanged();
                ChecarValidaciones();
                _ventanaGuardar = VentanaGuardar.MEDIA_FILIACION;
                StaticSourcesViewModel.SourceChanged = false;
            }
        }
        private async void SeniasFrenteLoad(SeniasFrenteView Window = null)
        {
            if (TabSenasParticulares)
            {
                await GuardarSwitch();
                await this.GetRegionesCuerpo();
                Justificacion = false;
                ListSenasParticulares = new System.Collections.ObjectModel.ObservableCollection<SENAS_PARTICULARES>(Imputado.SENAS_PARTICULARES);
                setValidacionesIdentificacionSeniasParticulares();
                _ventanaGuardar = VentanaGuardar.SENAS_PARTICULARES;
                StaticSourcesViewModel.SourceChanged = false;
            }
            if (Window != null && TabSenasParticulares && TabFrente)
            {
                ListRadioButons = new List<RadioButton>();
                ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridFrente"))).ToList();
                WebCam = null;
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
        private void SeniasDorsoLoad(SeniasDorsoView Window = null)
        {
            if (Window != null && TabSenasParticulares && TabDorso)
            {
                Justificacion = false;
                ListRadioButons = new List<RadioButton>();
                ListRadioButons = new FingerPrintScanner().FindVisualChildren<RadioButton>(((Grid)Window.FindName("GridDorso"))).ToList();
                WebCam = null;
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
        private void ApodosLoad(AgregarApodoView Window = null)
        {
            setValidacionesIdentificacionApodos();
            OnPropertyChanged();
            ChecarValidaciones();
        }
        private void AliasLoad(AgregarAliasView Window = null)
        {
            setValidacionesIdentificacionAlias();
            OnPropertyChanged();
            ChecarValidaciones();
        }
        private void RelacionLoad(AgregarRelacionInternoView Window = null)
        {
            setValidacionesIdentificacionRelacionesPersonales();
            OnPropertyChanged();
            ChecarValidaciones();
        }
        private async void PandillaLoad(PandillasEstatusAdminView Window = null)
        {
            if (TabPandillas)
            {
                await GuardarSwitch();
                GetPandilla();
                GetImputadoPandilla();
                base.ClearRules();
                _ventanaGuardar = VentanaGuardar.PANDILLAS;
                StaticSourcesViewModel.SourceChanged = false;
            }

        }
        private async void TrasladoLoad(IngresoTrasladoView Window = null)
        {
            if (IsSelectedTraslado)
            {
                Justificacion = true;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(getDatosTraslados).ContinueWith((prevTask) =>
                {
                    LimpiarDatosTraslado();
                    CargarDatosTraslado();
                });
                setValidacionesTraslado();
            }
        }
        private async void ApodosAliasReferenciasLoad(ApodosAliasReferenciasEstatusAdminView Window = null)
        {
            if (IsSelectedIdentificacion && TabApodosAlias)
            {
                await GuardarSwitch();
                await StaticSourcesViewModel.CargarDatosAsync<bool>(() =>
                {
                    ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                    ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                    ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                    return true;
                });
                OnPropertyChanged();

                _ventanaGuardar = VentanaGuardar.APODOS_ALIAS_REFERENCIAS;
                base.ClearRules();
                StaticSourcesViewModel.SourceChanged = false;
            }
        }


        #endregion

        #region TRASLADOS

        private void ModeloUpdatedChanged(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
            {
                switch (parametro.ToString())
                {
                    case "cambio_centro":
                        if (DTCentroOrigen == Parametro.ID_EMISOR_OTROS)
                            IsNombreCentroVisible = Visibility.Visible;
                        else
                            IsNombreCentroVisible = Visibility.Collapsed;
                        break;
                }
            }
        }


        private void getDatosTraslados()
        {
            try
            {
                if (LstCentroOrigenTraslado == null)
                {
                    LstCentroOrigenTraslado = new ObservableCollection<EMISOR>(new cEmisor().Obtener().OrderBy(o => o.ID_EMISOR));
                    LstCentroOrigenTraslado.Insert(0, new EMISOR() { ID_EMISOR = -1, DESCR = "SELECCIONE" });
                }

                if (LstMotivoTraslado == null)
                {
                    LstMotivoTraslado = new ObservableCollection<TRASLADO_MOTIVO>(new cTrasladoMotivo().ObtenerTodos());
                    LstMotivoTraslado.Insert(0, new TRASLADO_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                }
                //id_autoridad_traslado = Parametro.AUTORIDAD_TRASLADO;
                Autoridad_Traslado = Parametro.AUTORIDAD_TRASLADO;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CargarDatosTraslado()
        {
            try
            {
                var _traslado_detalle = SelectIngreso.TRASLADO_DETALLE.FirstOrDefault(w => w.ID_ANIO == SelectIngreso.ID_ANIO && w.ID_CENTRO == SelectIngreso.ID_CENTRO && w.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO
                    && w.ID_INGRESO == SelectIngreso.ID_INGRESO);
                if (_traslado_detalle != null && (_traslado_detalle.ID_ESTATUS != "CA" ? _traslado_detalle.TRASLADO.ORIGEN_TIPO == "F" : false))
                {
                    DTMotivo = _traslado_detalle.TRASLADO.ID_MOTIVO;
                    DTJustificacion = _traslado_detalle.TRASLADO.JUSTIFICACION;
                    //id_autoridad_traslado = _traslado_detalle.TRASLADO.AUTORIZA_TRASLADO.Value;
                    Autoridad_Traslado = _traslado_detalle.TRASLADO.AUTORIZA_TRASLADO;
                    DTNoOficio = _traslado_detalle.TRASLADO.OFICIO_AUTORIZACION;
                    DTCentroOrigen = _traslado_detalle.TRASLADO.ID_CENTRO_ORIGEN_FORANEO;
                    if (DTCentroOrigen == Parametro.ID_EMISOR_OTROS)
                    {
                        IsNombreCentroVisible = Visibility.Visible;
                        DTCentroNombre = _traslado_detalle.TRASLADO.CENTRO_ORIGEN_FORANEO;
                    }

                }
                else
                    LimpiarDatosTraslado();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void LimpiarDatosTraslado()
        {
            IsNombreCentroVisible = Visibility.Collapsed;
            DTMotivo = -1;
            DTCentroOrigen = -1;
            DTJustificacion = DTNoOficio = string.Empty;
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
                            if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = selectExpediente.INGRESO.Count > 0 ? false : true;
                            }
                            else
                                EmptyIngresoVisible = true;
                            //MUESTRA LOS INGRESOS
                            if (selectExpediente.INGRESO != null && selectExpediente.INGRESO.Count > 0)
                            {
                                SelectIngreso = selectExpediente.INGRESO.OrderBy(o => o.FEC_INGRESO_CERESO).FirstOrDefault();
                            }
                        }
                        break;
                }
            }
        }
        #endregion

    }
}
