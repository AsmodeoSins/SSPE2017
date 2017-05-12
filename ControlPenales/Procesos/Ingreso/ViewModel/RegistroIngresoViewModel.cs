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
    partial class RegistroIngresoViewModel : FingerPrintScanner
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange _OnParameterChange { get; set; }
        public RegistroIngresoViewModel() { }
        private async void clickSwitch(Object obj)
        {
            BanderaEntrada = true;
            switch (obj.ToString())
            {
                case "ampliar_justificacion_traslado":
                    TituloHeaderExpandirDescripcion = "Justificación";
                    TextAmpliarDescripcion = DTJustificacion;
                    MaxLengthAmpliarDescripcion = 1000;
                    Justificacion = true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "guardar_ampliar_descripcion":
                    DTJustificacion = TextAmpliarDescripcion;
                    TextAmpliarDescripcion = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AMPLIAR_DESCRIPCION_GENERICO);
                    break;
                case "buscar_menu":
                case "buscar_visible":
                    //SelectIngresoEnabled = false;
                    TextBotonSeleccionarIngreso = "seleccionar ingreso";
                    SelectExpediente = null;
                    this.buscarImputado();
                    break;
                case "buscar_salir":
                    AnioBuscar = FolioBuscar = null;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    TabVisible = false;
                    Imputado = ImputadoSeleccionadoAuxiliar;
                    SelectIngreso = null;
                    MenuReporteEnabled = false;
                    //SelectIngresoEnabled = false;
                    //LIMPIAMOS FILTROS
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (!PInsertar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Su usuario no tiene permiso para guardar."));
                            break;
                        }

                        if (!SelectIngresoEnabled)
                            break;
                        if (SelectExpediente != null)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(this.getDatosIngreso);
                            await TreeViewViewModel();
                            LimpiarCamposDatosIngreso();
                            LimpiarCamposIdentificacion();
                            LimpiarDatosTraslado();
                            LimpiarModalDocumentos();
                            NuevoImputado = false;
                            EditarImputado = true;
                            Imputado = SelectExpediente;
                            //if (SelectedInterconexion != null)
                            //{
                            //    ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                            //    ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                            //    NombreBuscar = SelectedInterconexion.NOMBRE;
                            //}
                            //else
                            //{
                                ApellidoPaternoBuscar = Imputado.PATERNO;
                                ApellidoMaternoBuscar = Imputado.MATERNO;
                                NombreBuscar = Imputado.NOMBRE;
                            //}
                            AnioBuscar = Imputado.ID_ANIO;
                            FolioBuscar = Imputado.ID_IMPUTADO;

                            EditarIngreso = false;
                            TabVisible = false;
                            //LimpiarCampos();
                            ClasificacionJuridicaEnabled = false;
                            EstatusAdministrativoEnabled = false;
                            CamposBusquedaEnabled = false;
                            SetValidacionesGenerales();
                            setDatosIngreso();
                            SelectClasificacionJuridica = "I";
                            SelectEstatusAdministrativo = 8;
                            IngresoEnabled = true;
                            IsSelectedDatosIngreso = true;
                            TabDatosGenerales = true;
                            getDatosImputado();

                            IngresoEnabled = true;
                            IdentificacionEnabled = true;
                            //Cambio para cambiar la pantalla de Traslado. Salvador. 12/2/2015
                            //if (SelectTipoIngreso == 3)
                            //    TrasladoEnabled = true;
                            //else
                            //    TrasladoEnabled = false;

                            if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                                TrasladoEnabled = true;
                            else
                                TrasladoEnabled = false;

                            MenuInsertarEnabled = true;
                            MenuDeshacerEnabled = true;
                            MenuGuardarEnabled = true;
                            MenuLimpiarEnabled = true;
                            MenuReporteEnabled = true;
                            MenuBorrarEnabled = true;
                            MenuBuscarEnabled = true;
                            MenuFichaEnabled = false;
                            MenuAyudaEnabled = true;
                            MenuSalirEnabled = true;

                            ImputadoSeleccionadoAuxiliar = Imputado;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);

                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un expediente o crear uno nuevo.");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso.", ex);
                    }
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
                    //VisibleAlias = true;//MOSTRAMOS EL ABC
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                    OpcionGuardarAlias = 1;
                    SelectAlias = null;
                    break;
                case "editar_alias":
                    if (SelectAlias != null && SelectAlias.IMPUTADO == null)
                    {
                        setValidacionesIdentificacionAlias();
                        TituloAlias = "Editar Alias";
                        PaternoAlias = SelectAlias.PATERNO;
                        MaternoAlias = SelectAlias.MATERNO;
                        NombreAlias = SelectAlias.NOMBRE;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        // VisibleAlias = true;//MOSTRAMOS EL ABC
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
                        SetValidacionesGenerales();
                    }
                    break;
                case "cancelar_alias":
                    VisibleAlias = false;
                    PaternoAlias = string.Empty;
                    MaternoAlias = string.Empty;
                    NombreAlias = string.Empty;
                    SelectAlias = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    break;
                case "insertar_apodo":
                    setValidacionesIdentificacionApodos();
                    TituloApodo = "Agregar Apodo";
                    Apodo = string.Empty;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                    OpcionGuardarApodo = 1;
                    SelectAlias = null;
                    break;
                case "editar_apodo":

                    if (SelectApodo != null && SelectApodo.IMPUTADO == null)
                    {
                        setValidacionesIdentificacionApodos();
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
                        SetValidacionesGenerales();
                    }
                    break;
                case "cancelar_apodo":
                    Apodo = string.Empty;
                    SelectApodo = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RegistroIngresoView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RegistroIngresoViewModel();
                    //LimpiarCampos();
                    break;
                case "insertar_familiar_responsable":
                    //ListFamiliarResponsable.Add(new PERSONA());
                    break;
                case "eliminar_familiar_responsable":
                    //ListFamiliarResponsable.Remove(SelectFamiliarResponsable);
                    break;
                case "insertar_relacion_interno":
                    setValidacionesIdentificacionRelacionesPersonales();
                    TituloRelacionInterno = "Agregar Relación Personal Interno";
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                    OpcionGuardarRelacionInterno = 1;
                    break;
                case "editar_relacion_interno":

                    if (SelectRelacionPersonalInterno != null && SelectRelacionPersonalInterno.IMPUTADO == null)
                    {
                        setValidacionesIdentificacionRelacionesPersonales();
                        TituloRelacionInterno = "Editar Relación Personal Interno";
                        //PaternoRelacionInterno = SelectRelacionPersonalInterno.PATERNO;
                        //MaternoRelacionInterno = SelectRelacionPersonalInterno.MATERNO;
                        //NombreRelacionInterno = SelectRelacionPersonalInterno.NOMBRE;
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
                        this.GuardarRelacionInterno();
                        SetValidacionesGenerales();
                    }
                    break;
                case "cancelar_relacion_interno":
                    CancelarBuscarRelacionInterno();
                    break;
                case "nuevo_expediente":
                    try
                    {
                        if (!PInsertar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Su usuario no tiene permiso para guardar."));
                            break;
                        }
                        if (!string.IsNullOrEmpty(NombreBuscar) && (!string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(this.getDatosIngreso);
                            await TreeViewViewModel();
                            FolioBuscar = null;
                            AnioBuscar = null;
                            LimpiarCamposDatosIngreso();
                            LimpiarCamposIdentificacion();
                            LimpiarDatosTraslado();
                            LimpiarModalDocumentos();
                            NuevoImputado = true;
                            EditarImputado = false;
                            Imputado = SelectExpediente = new IMPUTADO();
                            Imputado.PATERNO = ApellidoPaternoBuscar;
                            Imputado.MATERNO = ApellidoMaternoBuscar;
                            Imputado.NOMBRE = NombreBuscar;
                            //Imputado.ID_ANIO = short.Parse(AnioBuscar);
                            //Imputado.ID_IMPUTADO = int.Parse(FolioBuscar);
                            Ingreso = SelectIngreso = new INGRESO();
                            EditarIngreso = false;
                            TabVisible = false;
                            //LimpiarCampos();
                            ClasificacionJuridicaEnabled = false;
                            EstatusAdministrativoEnabled = false;
                            CamposBusquedaEnabled = false;
                            SetValidacionesGenerales();
                            setDatosIngreso();
                            SelectClasificacionJuridica = "I";
                            SelectEstatusAdministrativo = 8;
                            //getDatosImputado();

                            MenuInsertarEnabled = MenuDeshacerEnabled = MenuGuardarEnabled = MenuLimpiarEnabled = MenuReporteEnabled =
                                MenuBorrarEnabled = MenuBuscarEnabled = MenuAyudaEnabled = MenuSalirEnabled = IngresoEnabled =
                                    IsSelectedDatosIngreso = TabDatosGenerales = true;
                            MenuFichaEnabled = false;

                            IngresoEnabled = true;
                            IdentificacionEnabled = true;

                            //Cambio para cambiar la pantalla de Traslado. Salvador. 12/2/2015
                            //if (SelectTipoIngreso == 3)
                            //    TrasladoEnabled = true;
                            //else
                            //    TrasladoEnabled = false;

                            if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                                TrasladoEnabled = true;
                            else
                                TrasladoEnabled = false;

                            OnPropertyChanged("");
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);

                            StaticSourcesViewModel.SourceChanged = false;
                            if (SelectedInterconexion != null)
                            {
                                if (!string.IsNullOrEmpty(SelectedInterconexion.SEXO))
                                    StaticSourcesViewModel.SourceChanged = true;
                                else
                                    if (SelectedInterconexion.ESTADOCIVILID.HasValue)
                                        StaticSourcesViewModel.SourceChanged = true;
                                    else
                                        if (!string.IsNullOrEmpty(SelectedInterconexion.TELEFONO))
                                            StaticSourcesViewModel.SourceChanged = true;
                                        else
                                            if (SelectedInterconexion.NACIONALIDADID.HasValue)
                                                StaticSourcesViewModel.SourceChanged = true;
                                            else
                                                if (SelectedInterconexion.ESTADOORIGENID.HasValue)
                                                    StaticSourcesViewModel.SourceChanged = true;
                                                else
                                                    if (SelectedInterconexion.MUNICIPIOORIGENID.HasValue)
                                                        StaticSourcesViewModel.SourceChanged = true;
                                                    else
                                                        if (SelectedInterconexion.FECHANACIMIENTO.HasValue)
                                                            StaticSourcesViewModel.SourceChanged = true;

                            }
                        }
                        else
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debes ingresar Nombre y Apellidos.");

                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear nuevo expediente.", ex);
                    }
                    break;
                case "nueva_busqueda":
                    ListExpediente.Clear();
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    SelectIngresoEnabled = false;
                    //SelectIngresoEnabled = false;
                    break;
                case "guardar_menu":
                    if (base.HasErrors != true)
                    {
                        if (SelectExpediente == null || SelectExpediente.ID_IMPUTADO == 0)
                        {
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Ingreso", GuardarNuevoImputado))
                            {
                                new Dialogos().ConfirmacionDialogo("Éxito", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                                StaticSourcesViewModel.SourceChanged = false;
                                MenuGuardarEnabled = false;
                            }
                        }
                        else
                        {
                            if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Ingreso", GuardarImputadoExistente))
                            {
                                new Dialogos().ConfirmacionDialogo("Éxito", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                                StaticSourcesViewModel.SourceChanged = false;
                                MenuGuardarEnabled = false;
                            }
                        }

                    }
                    #region comentado
                    //if (Imputado != null)
                    //    if (!addNuevoExpediente())
                    //        if (!addDatosIngreso())
                    //            //if (!await updateImputado())
                    //            if (!await updateImputado())
                    //                (new Dialogos()).ConfirmacionDialogo("Error", "Al guardar datos del imputado.");
                    //            else
                    //            {
                    //                new Dialogos().ConfirmacionDialogo("Éxito", "INFORMACIÓN GRABADA EXITOSAMENTE!");
                    //                StaticSourcesViewModel.SourceChanged = false;
                    //            }
                    //        else
                    //            (new Dialogos()).ConfirmacionDialogo("Error", "Al guardar datos del ingreso.");
                    //    else
                    //        (new Dialogos()).ConfirmacionDialogo("Error", "Al guardar un nuevo expediente.");
                    //else
                    //    (new Dialogos()).ConfirmacionDialogo("Validación", "Debe Seleccionar un imputado. ");
                    #endregion
                    else
                        (new Dialogos()).ConfirmacionDialogo("Validación", string.Format("Faltan datos por capurar: {0}.", base.Error));
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
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debes seleccionar un interno.");
                    break;
                case "salir_menu":
                    Imputado = null;
                    PrincipalViewModel.SalirMenu();
                    break;
                case "ficha_menu":
                    this.Reporte();
                    break;
                //DOCUMENTOS
                case "reporte_menu":
                    if (SelectIngreso != null)
                    {
                        /*VisibleDocumento = !VisibleDocumento;
                        if (VisibleDocumento)
                            this.PopulateImputadosDocumentos();
                        VisibleIngreso = !VisibleDocumento;*/
                        VerDocumento();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar antes de imprimir");
                    break;
                case "insertar_documento":
                    this.LimpiarModalDocumentos();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTO);
                    break;
                case "eliminar_documento":
                    this.EliminarDocumento();
                    break;
                case "guardar_documento":
                    //if (GuardarDocumento())
                    //{
                    //    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTO);
                    //}
                    break;
                case "cancelar_documento":
                    this.LimpiarModalDocumentos();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DOCUMENTO);
                    break;
                case "ver_documento":
                    //VerDocumento();
                    break;
                case "regresar_registro":
                    VisibleDocumento = false;
                    VisibleIngreso = !VisibleDocumento;
                    break;
                case "interconexion_busqueda":
                    VisibleBuscarNUC = true;
                    break;
                case "cancelar_buscar_nuc":
                    VisibleBuscarNUC = false;
                    break;
                case "seleccionar_nuc":
                    VisibleBuscarNUC = false;
                    break;
                case "buscar_nuc":
                    //this.BuscarNUCInterconexion();
                    break;
                case "Open442":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
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

        private bool GuardarImputadoExistente()
        {
            try
            {
                var error = false;

                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    try
                    {
                        var id_area_sala_cabos = Parametro.ID_AREA_SALA_CABOS;

                        var mediafiliacion = new List<IMPUTADO_FILIACION>();
                        var padres = new List<IMPUTADO_PADRES>();
                        var apodos = new List<APODO>();
                        var alias = new List<ALIAS>();
                        var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
                        var ingreso = new INGRESO();
                        var cama = new CAMA();
                        var traslado_detalle = new TRASLADO_DETALLE();
                        var ingresoBiometrico = new List<SSP.Servidor.INGRESO_BIOMETRICO>();
                        var imputadoBiometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();


                        Imputado = new IMPUTADO();

                        Imputado.ID_CENTRO = SelectExpediente.ID_CENTRO;
                        Imputado.ID_ANIO = SelectExpediente.ID_ANIO;
                        Imputado.ID_IMPUTADO = SelectExpediente.ID_IMPUTADO;
                        Imputado.NIP = SelectExpediente.NIP;

                        //if (SelectedInterconexion != null)
                        //{
                        //    Imputado.PATERNO = SelectedInterconexion.PRIMERAPELLIDO;
                        //    Imputado.MATERNO = SelectedInterconexion.SEGUNDOAPELLIDO;
                        //    Imputado.NOMBRE = SelectedInterconexion.NOMBRE;
                        //}
                        //else
                        //{
                            Imputado.PATERNO = SelectExpediente.PATERNO;
                            Imputado.MATERNO = SelectExpediente.MATERNO;
                            Imputado.NOMBRE = SelectExpediente.NOMBRE;
                        //}
                        #region DatosGenerales
                        Imputado.SEXO = SelectSexo;
                        //Imputado.ID_ESTADO_CIVIL = SelectEstadoCivil;
                        //Imputado.ID_OCUPACION = SelectOcupacion;
                        //Imputado.ID_ESCOLARIDAD = SelectEscolaridad;
                        //Imputado.ID_RELIGION = SelectReligion;
                        Imputado.ID_ETNIA = SelectEtnia;
                        Imputado.ID_NACIONALIDAD = SelectNacionalidad;

                        //nuevos campos
                        Imputado.ID_IDIOMA = SelectedIdioma;
                        Imputado.ID_DIALECTO = SelectedDialecto;
                        Imputado.TRADUCTOR = RequiereTraductor ? "S" : "N";

                        #endregion

                        #region Domicilio
                        //Imputado.DOMICILIO_CALLE = TextCalle;
                        //Imputado.DOMICILIO_NUM_INT = TextNumeroInterior;
                        //Imputado.DOMICILIO_NUM_EXT = TextNumeroExterior;
                        //Imputado.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
                        //Imputado.ID_MUNICIPIO = SelectMunicipio;
                        //Imputado.ID_ENTIDAD = SelectEntidad;
                        //Imputado.ID_PAIS = SelectPais;
                        //Imputado.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                        //Imputado.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                        //Imputado.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                        //if (!string.IsNullOrEmpty(AniosEstado))
                        //    Imputado.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                        //if (!string.IsNullOrEmpty(MesesEstado))
                        //    Imputado.RESIDENCIA_MESES = short.Parse(MesesEstado);
                        #endregion

                        #region Nacimiento
                        Imputado.NACIMIENTO_PAIS = SelectPaisNacimiento;
                        Imputado.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                        Imputado.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                        Imputado.NACIMIENTO_FECHA = TextFechaNacimiento;
                        Imputado.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                        #endregion

                        #region Padres
                        Imputado.NOMBRE_PADRE = TextPadreNombre;
                        Imputado.MATERNO_PADRE = TextPadreMaterno;
                        Imputado.PATERNO_PADRE = TextPadrePaterno;
                        Imputado.NOMBRE_MADRE = TextMadreNombre;
                        Imputado.MATERNO_MADRE = TextMadreMaterno;
                        Imputado.PATERNO_MADRE = TextMadrePaterno;
                        //if (CheckPadreFinado)
                        //    Imputado.PADRE_FINADO = "S";
                        //else
                        //    Imputado.PADRE_FINADO = "N";
                        //if (CheckMadreFinado)
                        //    Imputado.MADRE_FINADO = "S";
                        //else
                        //    Imputado.MADRE_FINADO = "N";
                        #endregion

                        #region Comentado
                        ////Agregar mediafiliacion para nuevo imputado
                        //if (LstImputadoFiliacion != null)
                        //{
                        //    foreach (var x in LstImputadoFiliacion)
                        //    {
                        //        if (x.IMPUTADO == null)
                        //            mediafiliacion.Add(new IMPUTADO_FILIACION()
                        //            {
                        //                ID_IMPUTADO = Imputado.ID_IMPUTADO,
                        //                ID_CENTRO = Imputado.ID_CENTRO,
                        //                ID_ANIO = Imputado.ID_ANIO,
                        //                ID_MEDIA_FILIACION = x.ID_MEDIA_FILIACION,
                        //                ID_TIPO_FILIACION = x.ID_TIPO_FILIACION,
                        //            });
                        //    }
                        //}
                        #endregion

                        Imputado.IMPUTADO_FILIACION = mediafiliacion;

                        //agrega direcciones de los padres
                        if (!CheckMadreFinado || !CheckPadreFinado)
                        {
                            if (!MismoDomicilioPadre && !CheckPadreFinado)
                            {

                                padres.Add(new IMPUTADO_PADRES()
                                {
                                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                    ID_PADRE = "P",
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
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
                                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                    ID_PADRE = "M",
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
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
                        Imputado.IMPUTADO_PADRES = padres;
                        //agrega apodos
                        if (ListApodo != null && ListApodo.Count > 0)
                        {
                            short id_apodo = 1;
                            foreach (var entidad in ListApodo)
                            {
                                if (entidad.IMPUTADO == null)
                                {
                                    apodos.Add(new APODO
                                    {
                                        ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                        ID_CENTRO = Imputado.ID_CENTRO,
                                        ID_ANIO = Imputado.ID_ANIO,
                                        ID_APODO = id_apodo,
                                        APODO1 = entidad.APODO1
                                    });
                                    id_apodo++;
                                }

                            }
                        }
                        Imputado.APODO = apodos;
                        //agrega alias
                        if (ListAlias != null && ListAlias.Count > 0)
                        {
                            short id_alias = 1;
                            foreach (var entidad in ListAlias)
                            {
                                if (entidad.IMPUTADO == null)
                                {
                                    alias.Add(new ALIAS
                                    {
                                        ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                        ID_CENTRO = Imputado.ID_CENTRO,
                                        ID_ANIO = Imputado.ID_ANIO,
                                        ID_ALIAS = id_alias,
                                        PATERNO = entidad.PATERNO,
                                        MATERNO = entidad.MATERNO,
                                        NOMBRE = entidad.NOMBRE
                                    });
                                    id_alias++;
                                }
                            }
                        }


                        Imputado.ALIAS = alias;
                        //agrega relaciones personales
                        if (ListRelacionPersonalInterno != null && ListRelacionPersonalInterno.Count > 0)
                        {
                            foreach (var entidad in ListRelacionPersonalInterno)
                            {
                                if (entidad.IMPUTADO == null)
                                {
                                    relacion_personal_internos.Add(new RELACION_PERSONAL_INTERNO
                                    {
                                        ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                        ID_CENTRO = Imputado.ID_CENTRO,
                                        ID_ANIO = Imputado.ID_ANIO,
                                        NOTA = entidad.NOTA,
                                        ID_REL_ANIO = entidad.INGRESO.ID_ANIO,
                                        ID_REL_CENTRO = entidad.INGRESO.ID_CENTRO,
                                        ID_REL_IMPUTADO = entidad.INGRESO.ID_IMPUTADO,
                                        ID_REL_INGRESO = entidad.INGRESO.ID_INGRESO
                                    });
                                }
                            }
                        }
                        Imputado.RELACION_PERSONAL_INTERNO = relacion_personal_internos;

                        ingreso.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        ingreso.ID_CENTRO = Imputado.ID_CENTRO;
                        ingreso.ID_ANIO = Imputado.ID_ANIO;

                        #region Nuevo
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

                        #endregion

                        //DATOS_INGRESO
                        ingreso.FEC_REGISTRO = FechaRegistroIngreso;
                        ingreso.FEC_INGRESO_CERESO = FechaCeresoIngreso;
                        ingreso.ID_TIPO_INGRESO = SelectTipoIngreso;
                        ingreso.ID_CLASIFICACION_JURIDICA = SelectClasificacionJuridica;
                        ingreso.ID_ESTATUS_ADMINISTRATIVO = SelectEstatusAdministrativo;
                        //ingreso.ID_INGRESO_DELITO = IngresoDelito;
                        if (SelectedDelito != null)
                        {
                            ingreso.ID_FUERO = SelectedDelito.ID_FUERO;
                            ingreso.ID_DELITO = SelectedDelito.ID_DELITO;
                        }

                        //DATOS_DOCUMENTO_INTERNACION
                        ingreso.ID_DISPOSICION = SelectTipoDisposicion;
                        ingreso.ID_AUTORIDAD_INTERNA = SelectTipoAutoridadInterna;
                        ingreso.DOCINTERNACION_NUM_OFICIO = TextNumeroOficio;
                        ingreso.ID_TIPO_SEGURIDAD = SelectTipoSeguridad;

                        //UBICACION
                        ingreso.ID_UB_CENTRO = SelectedCama.ID_CENTRO;
                        ingreso.ID_UB_EDIFICIO = SelectedCama.ID_EDIFICIO;
                        ingreso.ID_UB_SECTOR = SelectedCama.ID_SECTOR;
                        ingreso.ID_UB_CELDA = SelectedCama.ID_CELDA;
                        ingreso.ID_UB_CAMA = SelectedCama.ID_CAMA;

                        //INTERCONEXION
                        if (SelectedInterconexion != null)
                        {
                            ingreso.NUC = SelectedInterconexion.EXPEDIENTEID.ToString();
                            ingreso.ID_PERSONA_PG = SelectedInterconexion.PERSONAFISICAID;
                        }

                        //TRASLADO DETALLE
                        if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                        {
                            var traslado = new TRASLADO
                            {
                                AUTORIZA_TRASLADO = Autoridad_Traslado,
                                ID_CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value,
                                CENTRO_ORIGEN_FORANEO =  DTCentroOrigen.Value == Parametro.ID_EMISOR_OTROS ? DTCentroNombre : new cEmisor().Obtener(DTCentroOrigen.Value).DESCR,
                                ID_CENTRO = GlobalVar.gCentro,
                                ID_ESTATUS = "FI",
                                ID_MOTIVO = DTMotivo.Value,
                                JUSTIFICACION = DTJustificacion,
                                OFICIO_AUTORIZACION = DTNoOficio,
                                ORIGEN_TIPO = "F",
                                TRASLADO_FEC = ingreso.FEC_INGRESO_CERESO.Value
                            };
                            traslado_detalle = new TRASLADO_DETALLE
                            {
                                ID_IMPUTADO = Imputado.ID_IMPUTADO,
                                ID_ANIO = Imputado.ID_ANIO,
                                ID_CENTRO = Imputado.ID_CENTRO,
                                ID_CENTRO_TRASLADO = GlobalVar.gCentro,
                                ID_ESTATUS = "FI", //ESTATUS FINALIZADO
                                TRASLADO = traslado
                            };
                            ingreso.TRASLADO_DETALLE.Add(traslado_detalle);
                        }
                        else
                        {
                            ingreso.TRASLADO_DETALLE = new List<TRASLADO_DETALLE>();
                        }

                        cama = new CAMA()
                        {
                            ID_CAMA = SelectedCama.ID_CAMA,
                            ID_CELDA = SelectedCama.ID_CELDA,
                            ID_CENTRO = SelectedCama.ID_CENTRO,
                            ID_EDIFICIO = SelectedCama.ID_EDIFICIO,
                            ID_SECTOR = SelectedCama.ID_SECTOR,
                            DESCR = SelectedCama.DESCR,
                            ESTATUS = "N"
                        };

                        //BIOMETRICO
                        #region [Fotos]
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
                                ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                            });
                        }
                        #endregion

                        #region [Huellas]
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
                        #endregion
                        Imputado.IMPUTADO_BIOMETRICO = imputadoBiometrico;
                        ingreso.INGRESO_BIOMETRICO = ingresoBiometrico;

                        Imputado.INGRESO.Add(ingreso);
                        new cImputado().InsertarIngresoExisteImputado(Imputado, cama, id_area_sala_cabos,_FechaServer);
                        SelectExpediente = new cImputado().Obtener(Imputado.ID_IMPUTADO, Imputado.ID_ANIO, Imputado.ID_CENTRO).FirstOrDefault();
                        if (SelectExpediente != null)
                        {
                            if (SelectExpediente.INGRESO != null)
                            {
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
                        error = true;
                    }
                }));
                if (!error)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el ingreso.", ex);
                return false;
            }
        }

        private bool GuardarNuevoImputado()
        {
            try
            {
                var error = false;
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    try
                    {
                        var id_area_sala_cabos = Parametro.ID_AREA_SALA_CABOS;
                        var mediafiliacion = new List<IMPUTADO_FILIACION>();
                        var padres = new List<IMPUTADO_PADRES>();
                        var apodos = new List<APODO>();
                        var alias = new List<ALIAS>();
                        var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
                        var ingreso = new INGRESO();
                        var cama = new CAMA();
                        var traslado_detalle = new TRASLADO_DETALLE();
                        var ingresoBiometrico = new List<SSP.Servidor.INGRESO_BIOMETRICO>();
                        var imputadoBiometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();

                        Imputado = new IMPUTADO();
                        Imputado.ID_CENTRO = GlobalVar.gCentro;
                        Imputado.ID_ANIO = (short)Fechas.GetFechaDateServer.Year;
                        //if (SelectedInterconexion != null)
                        //{
                        //    Imputado.PATERNO = SelectedInterconexion.PRIMERAPELLIDO;
                        //    Imputado.MATERNO = SelectedInterconexion.SEGUNDOAPELLIDO;
                        //    Imputado.NOMBRE = SelectedInterconexion.NOMBRE;
                        //}
                        //else
                        //{
                            Imputado.PATERNO = ApellidoPaternoBuscar;
                            Imputado.MATERNO = ApellidoMaternoBuscar;
                            Imputado.NOMBRE = NombreBuscar;
                            //}
                        #region DatosGenerales
                        Imputado.SEXO = SelectSexo;
                        //Imputado.ID_ESTADO_CIVIL = SelectEstadoCivil;
                        //Imputado.ID_OCUPACION = SelectOcupacion;
                        //Imputado.ID_ESCOLARIDAD = SelectEscolaridad;
                        //Imputado.ID_RELIGION = SelectReligion;
                        Imputado.ID_ETNIA = SelectEtnia;
                        Imputado.ID_NACIONALIDAD = SelectNacionalidad;
                        //nuevos campos
                        Imputado.ID_IDIOMA = SelectedIdioma;
                        Imputado.ID_DIALECTO = SelectedDialecto;
                        Imputado.TRADUCTOR = RequiereTraductor ? "S" : "N";
                        #endregion

                        #region Domicilio
                        //Imputado.DOMICILIO_CALLE = TextCalle;
                        //Imputado.DOMICILIO_NUM_INT = TextNumeroInterior;
                        //Imputado.DOMICILIO_NUM_EXT = TextNumeroExterior;
                        //Imputado.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
                        //Imputado.ID_MUNICIPIO = SelectMunicipio;
                        //Imputado.ID_ENTIDAD = SelectEntidad;
                        //Imputado.ID_PAIS = SelectPais;
                        //Imputado.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
                        //Imputado.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
                        //Imputado.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
                        //if (!string.IsNullOrEmpty(AniosEstado))
                        //    Imputado.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
                        //if (!string.IsNullOrEmpty(MesesEstado))
                        //    Imputado.RESIDENCIA_MESES = short.Parse(MesesEstado);
                        #endregion

                        #region Nacimiento
                        Imputado.NACIMIENTO_PAIS = SelectPaisNacimiento;
                        Imputado.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
                        Imputado.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
                        Imputado.NACIMIENTO_FECHA = TextFechaNacimiento;
                        Imputado.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
                        #endregion

                        #region Padres
                        Imputado.NOMBRE_PADRE = TextPadreNombre;
                        Imputado.MATERNO_PADRE = TextPadreMaterno;
                        Imputado.PATERNO_PADRE = TextPadrePaterno;
                        Imputado.NOMBRE_MADRE = TextMadreNombre;
                        Imputado.MATERNO_MADRE = TextMadreMaterno;
                        Imputado.PATERNO_MADRE = TextMadrePaterno;
                        //if (CheckPadreFinado)
                        //    Imputado.PADRE_FINADO = "S";
                        //else
                        //    Imputado.PADRE_FINADO = "N";
                        //if (CheckMadreFinado)
                        //    Imputado.MADRE_FINADO = "S";
                        //else
                        //    Imputado.MADRE_FINADO = "N";
                        #endregion

                        #region Media Filiacion
                        if (LstImputadoFiliacion != null)
                        {
                            foreach (var x in LstImputadoFiliacion)
                            {
                                mediafiliacion.Add(new IMPUTADO_FILIACION()
                                {
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
                                    ID_MEDIA_FILIACION = x.ID_MEDIA_FILIACION,
                                    ID_TIPO_FILIACION = x.ID_TIPO_FILIACION,
                                });
                            }
                            if(mediafiliacion.Count > 0)
                                Imputado.IMPUTADO_FILIACION = mediafiliacion;
                        }
                        #endregion

                        #region Informacion Padres
                        if (!CheckMadreFinado || !CheckPadreFinado)
                        {
                            if (!MismoDomicilioPadre && !CheckPadreFinado)
                            {
                                padres.Add(new IMPUTADO_PADRES()
                                {
                                    ID_PADRE = "P",
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
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
                        Imputado.IMPUTADO_PADRES = padres;
                        #endregion

                        #region Apodos
                        if (ListApodo != null && ListApodo.Count > 0)
                        {
                            short id_apodo = 1;
                            foreach (var entidad in ListApodo)
                            {
                                apodos.Add(new APODO
                                {
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
                                    ID_APODO = id_apodo,
                                    APODO1 = entidad.APODO1
                                });
                                id_apodo++;
                            }
                            if(apodos.Count > 0 )
                                Imputado.APODO = apodos;
                        }
                        #endregion

                        #region Alias
                        if (ListAlias != null && ListAlias.Count > 0)
                        {
                            short id_alias = 1;
                            foreach (var entidad in ListAlias)
                            {
                                alias.Add(new ALIAS
                                {
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
                                    ID_ALIAS = id_alias,
                                    PATERNO = entidad.PATERNO,
                                    MATERNO = entidad.MATERNO,
                                    NOMBRE = entidad.NOMBRE
                                });
                                id_alias++;
                            }
                            if(alias.Count > 0)
                                Imputado.ALIAS = alias;
                        }
                        #endregion

                        #region Relaciones Personales
                        if (ListRelacionPersonalInterno != null && ListRelacionPersonalInterno.Count > 0)
                        {
                            foreach (var entidad in ListRelacionPersonalInterno)
                            {
                                relacion_personal_internos.Add(new RELACION_PERSONAL_INTERNO
                                {
                                    ID_CENTRO = Imputado.ID_CENTRO,
                                    ID_ANIO = Imputado.ID_ANIO,
                                    NOTA = entidad.NOTA,
                                    ID_REL_ANIO = entidad.INGRESO.ID_ANIO,
                                    ID_REL_CENTRO = entidad.INGRESO.ID_CENTRO,
                                    ID_REL_IMPUTADO = entidad.INGRESO.ID_IMPUTADO,
                                    ID_REL_INGRESO = entidad.INGRESO.ID_INGRESO
                                });
                            }
                            if(relacion_personal_internos.Count > 0)
                                Imputado.RELACION_PERSONAL_INTERNO = relacion_personal_internos;
                        }
                        #endregion

                        ingreso.ID_CENTRO = Imputado.ID_CENTRO;
                        ingreso.ID_ANIO = Imputado.ID_ANIO;

                        #region Datos Ingreso
                        ingreso.FEC_REGISTRO = FechaRegistroIngreso;
                        ingreso.FEC_INGRESO_CERESO = FechaCeresoIngreso;
                        ingreso.ID_TIPO_INGRESO = SelectTipoIngreso;
                        ingreso.ID_CLASIFICACION_JURIDICA = SelectClasificacionJuridica;
                        ingreso.ID_ESTATUS_ADMINISTRATIVO = SelectEstatusAdministrativo;
                        //ingreso.ID_INGRESO_DELITO = IngresoDelito;
                        if (SelectedDelito != null)
                        {    
                            ingreso.ID_FUERO = SelectedDelito.ID_FUERO;
                            ingreso.ID_DELITO = SelectedDelito.ID_DELITO;
                        }

                        #region Nuevo
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

                        //ingreso.LUGAR_RESIDENCIA
                        //ingreso.NUMERO_IDENTIFICACION
                        //ingreso.ID_TIPO_DISCAPACIDAD
                        //ingreso.ESTATURA
                        //ingreso.PESO
                        #endregion
                       
                        #endregion

                        #region Documentos de Internacion
                        ingreso.ID_DISPOSICION = SelectTipoDisposicion;
                        ingreso.ID_AUTORIDAD_INTERNA = SelectTipoAutoridadInterna;
                        ingreso.DOCINTERNACION_NUM_OFICIO = TextNumeroOficio;
                        ingreso.ID_TIPO_SEGURIDAD = SelectTipoSeguridad;
                        #endregion

                        #region Ubicacion
                        ingreso.ID_UB_CENTRO = SelectedCama.ID_CENTRO;
                        ingreso.ID_UB_EDIFICIO = SelectedCama.ID_EDIFICIO;
                        ingreso.ID_UB_SECTOR = SelectedCama.ID_SECTOR;
                        ingreso.ID_UB_CELDA = SelectedCama.ID_CELDA;
                        ingreso.ID_UB_CAMA = SelectedCama.ID_CAMA;
                        #endregion

                        #region Interconexion
                        if (SelectedInterconexion != null)
                        {
                            ingreso.NUC = SelectedInterconexion.EXPEDIENTEID.ToString();
                            ingreso.ID_PERSONA_PG = SelectedInterconexion.PERSONAFISICAID;
                        }
                        #endregion

                        #region Traslado Detalle
                        if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                        {
                            var traslado = new TRASLADO
                            {
                                AUTORIZA_TRASLADO = Autoridad_Traslado,
                                ID_CENTRO_ORIGEN_FORANEO=DTCentroOrigen.Value,
                                CENTRO_ORIGEN_FORANEO = DTCentroOrigen.Value==Parametro.ID_EMISOR_OTROS?DTCentroNombre :new cEmisor().Obtener(DTCentroOrigen.Value).DESCR,
                                ID_CENTRO = GlobalVar.gCentro,
                                ID_ESTATUS = "FI",
                                ID_MOTIVO = DTMotivo.Value,
                                JUSTIFICACION = DTJustificacion,
                                OFICIO_AUTORIZACION = DTNoOficio,
                                ORIGEN_TIPO = "F",
                                TRASLADO_FEC = ingreso.FEC_INGRESO_CERESO.Value
                            };
                            traslado_detalle = new TRASLADO_DETALLE
                            {
                                ID_ANIO = Imputado.ID_ANIO,
                                ID_CENTRO = Imputado.ID_CENTRO,
                                ID_CENTRO_TRASLADO = GlobalVar.gCentro,
                                ID_ESTATUS = "FI", //ESTATUS FINALIZADO
                                TRASLADO = traslado
                            };
                            ingreso.TRASLADO_DETALLE.Add(traslado_detalle);
                        }
                        else
                        {
                            ingreso.TRASLADO_DETALLE = new List<TRASLADO_DETALLE>();
                        }
                        #endregion

                        #region Ubicacion
                        cama = new CAMA()
                        {
                            ID_CAMA = SelectedCama.ID_CAMA,
                            ID_CELDA = SelectedCama.ID_CELDA,
                            ID_CENTRO = SelectedCama.ID_CENTRO,
                            ID_EDIFICIO = SelectedCama.ID_EDIFICIO,
                            ID_SECTOR = SelectedCama.ID_SECTOR,
                            DESCR = SelectedCama.DESCR,
                            ESTATUS = "N"
                        };
                        #endregion

                        #region Biometrico
                        #region [Fotos]
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
                                ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
                                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
                            });
                        }
                        #endregion

                        #region [Huellas]
                        string toma = "N";
                        if(HuellasCapturadas != null)
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
                        #endregion
                        
                        Imputado.IMPUTADO_BIOMETRICO = imputadoBiometrico;
                        ingreso.INGRESO_BIOMETRICO = ingresoBiometrico;
                        #endregion

                        Imputado.INGRESO.Add(ingreso);
                        folioBuscar = new cImputado().InsertarIngresoNuevoImputado(Imputado, cama, id_area_sala_cabos,_FechaServer);

                        SelectExpediente = new cImputado().Obtener(folioBuscar.Value, Imputado.ID_ANIO, Imputado.ID_CENTRO).FirstOrDefault();
                        if (SelectExpediente != null)
                        {
                            if (SelectExpediente.INGRESO != null)
                            {
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            }
                        }

                        RaisePropertyChanged("FolioBuscar");
                        anioBuscar = Imputado.ID_ANIO;
                        RaisePropertyChanged("AnioBuscar");
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ingreso.", ex);
                        error = true;
                    }

                }));

                if (!error)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ingreso.", ex);
                return false;
            }
        }
        private void CloseView(object obj)
        {
            PrincipalViewModel.SalirMenu();
        }
        private void ClickEnter(Object obj)
        {
            buscarImputado(obj);
        }
        private void TreeClick(Object obj)
        {
            try
            {
                var x = obj.GetType();
                if (x.BaseType.Name.Equals("CAMA"))
                    SelectedCama = (CAMA)obj;
                else
                    SelectedCama = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error.", ex);
            }
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
                    if (meses - 1 < 0)
                    {
                        anios -= 1;
                        meses = 11;
                    }

                    else
                        meses -= 1;
                    dias += DateTime.DaysInMonth(hoy.Year, FechaEstado.Month);
                }
                if (anios < 0)
                    System.Windows.MessageBox.Show("La fecha es inválida");

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
            if (IsSelectedDatosIngreso == true)
                LimpiarCamposDatosIngreso();
            else if (IsSelectedIdentificacion == true)
                if (TabDatosGenerales == true)
                    LimpiarCamposIdentificacion();

        }
        private void LimpiarCamposDatosIngreso()
        {
            CamposBusquedaEnabled = ClasificacionJuridicaEnabled = EstatusAdministrativoEnabled = true;
            #region DatosIngreso
            IngresoDelito = SelectTipoDelito = -1;
            SelectTipoIngreso = SelectEstatusAdministrativo = SelectTipoDisposicion = SelectTipoAutoridadInterna = -1;
            SelectClasificacionJuridica = SelectTipoSeguridad = TextNumeroOficio = string.Empty;
            SelectedCama = null;
            #endregion
        }
        private void LimpiarCamposIdentificacion()
        {
            try
            {
                #region DatosGenerales
                SelectNacionalidad = pais_param;
                SelectedIdioma = Parametro.IDIOMA;
                SelectEtnia = SelectEscolaridad = SelectOcupacion = SelectEstadoCivil = SelectReligion = -1;
                SelectSexo = "M";
                #endregion

                #region Domicilio
                SelectPais = CentroActual.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC;
                SelectEntidad = CentroActual.MUNICIPIO.ENTIDAD.ID_ENTIDAD;
                SelectMunicipio = CentroActual.MUNICIPIO.ID_MUNICIPIO;
                SelectColoniaItem = ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();
                TextNumeroExterior = null;
                TextNumeroInterior = AniosEstado = MesesEstado = TextCalle = TextDomicilioTrabajo = string.Empty;
                FechaEstado = Fechas.GetFechaDateServer;
                TextTelefono = null;
                TextCodigoPostal = null;
                #endregion

                #region Nacimiento
                SelectPaisNacimiento = CentroActual.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC; ;
                SelectEntidadNacimiento = CentroActual.MUNICIPIO.ENTIDAD.ID_ENTIDAD;
                SelectMunicipioNacimiento = CentroActual.MUNICIPIO.ID_MUNICIPIO;

                TextFechaNacimiento = Fechas.GetFechaDateServer;
                TextLugarNacimientoExtranjero = string.Empty;
                #endregion

                #region Padres
                TextPadreMaterno = TextPadrePaterno = TextPadreNombre = TextNumeroInteriorDomicilioPadre = TextCalleDomicilioPadre = string.Empty;
                CheckPadreFinado = MismoDomicilioPadre = false;
                SelectPaisDomicilioPadre = CentroActual.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC;
                SelectEntidadDomicilioPadre = CentroActual.MUNICIPIO.ENTIDAD.ID_ENTIDAD;
                SelectMunicipioDomicilioPadre = CentroActual.MUNICIPIO.ID_MUNICIPIO;
                TextNumeroExteriorDomicilioPadre = null;
                TextCodigoPostalDomicilioPadre = null;

                TextMadreMaterno = TextMadrePaterno = TextMadreNombre = TextNumeroInteriorDomicilioMadre = TextCalleDomicilioMadre = string.Empty;
                CheckMadreFinado = MismoDomicilioMadre = false;
                SelectPaisDomicilioMadre = CentroActual.MUNICIPIO.ENTIDAD.PAIS_NACIONALIDAD.ID_PAIS_NAC;
                SelectEntidadDomicilioMadre = CentroActual.MUNICIPIO.ENTIDAD.ID_ENTIDAD;
                SelectMunicipioDomicilioMadre = CentroActual.MUNICIPIO.ID_MUNICIPIO;
                TextNumeroExteriorDomicilioMadre = null;
                TextCodigoPostalDomicilioMadre = null;
                #endregion

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar campos de identificación.", ex);
            }
        }

        private void setDatosIngreso()
        {
            SelectTipoDelito = -1;
            SelectTipoAutoridadInterna = SelectOcupacion = SelectEstatusAdministrativo = SelectTipoIngreso = SelectTipoDisposicion = -1;
            SelectClasificacionJuridica = SelectTipoSeguridad = string.Empty;
        }
        private void SetValidacionesGenerales()
        {
            setValidacionesDatosIngreso();
            setValidacionesIdentificacionDatosGenerales();
            base.AddRule(() => ImagesToSave, () => ImagesToSave != null && ImagesToSave.Count == 3, "FOTOS DEL IMPUTADO SON REQUERIDAS!");
            //base.AddRule(() => HuellasCapturadas, () => HuellasCapturadas != null && HuellasCapturadas.Count >= 1, "HUELLAS DEL IMPUTADO SON REQUERIDAS!");
            if (SelectTipoIngreso == Parametro.TRASLADO_FOREANO_TIPO_INGRESO)
                AgregarValidacionesTraslado();
        }

        #region OracleOperations
        private async void getDatosIngreso()
        {
            try
            {
                #region DatosIngreso

                ListTipoIngreso = ListTipoIngreso ?? new cTipoIngreso().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                ListClasificacionJuridica = ListClasificacionJuridica ?? new cClasificacionJuridica().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                ListEstatusAdministrativo = ListEstatusAdministrativo ?? new cEstatusAdministrativo().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                IngresoDelitos = IngresoDelitos ?? new ObservableCollection<INGRESO_DELITO>(new cIngresoDelito().ObtenerTodos().OrderBy(o => o.DESCR).ToList());


                #endregion

                #region DATOS_DOCUMENTO_INTERNACION

                ListTipoAutoridadInterna = ListTipoAutoridadInterna ?? new cTipoAutoridadInterna().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                ListTipoDisposicion = ListTipoDisposicion ?? new cTipoDisposicion().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                ListTipoSeguridad = ListTipoSeguridad ?? new cTipoSeguridad().ObtenerTodos().OrderBy(o => o.DESCR).ToList();

                #endregion

                #region Ubicacion
                ListSector = ListSector ?? new ObservableCollection<SECTOR>(new cSector().ObtenerTodos(string.Empty, 0, /*4*/GlobalVar.gCentro, 0).OrderBy(o => o.DESCR));
                //SelectSector = -1;

                ListCelda = ListCelda ?? new ObservableCollection<CELDA>();

                //CENTRO
                Centro = new cCentro().Obtener(4).FirstOrDefault();//MEXICALI
                //await TreeViewViewModel();
                #endregion
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListCelda.Insert(0, new CELDA() { ID_CELDA = "SELECCIONE" });
                    SelectCelda = "SELECCIONE";
                    ListSector.Insert(0, new SECTOR() { ID_SECTOR = -1, DESCR = "SELECCIONE" });
                    ListTipoSeguridad.Insert(0, new TIPO_SEGURIDAD() { ID_TIPO_SEGURIDAD = string.Empty, DESCR = "SELECCIONE" });
                    SelectTipoSeguridad = string.Empty;
                    ListTipoDisposicion.Insert(0, new TIPO_DISPOSICION() { ID_DISPOSICION = -1, DESCR = "SELECCIONE" });
                    SelectTipoDisposicion = -1;
                    ListTipoAutoridadInterna.Insert(0, new TIPO_AUTORIDAD_INTERNA() { ID_AUTORIDAD_INTERNA = -1, DESCR = "SELECCIONE" });
                    SelectTipoAutoridadInterna = -1;
                    ListEstatusAdministrativo.Insert(0, new ESTATUS_ADMINISTRATIVO() { ID_ESTATUS_ADMINISTRATIVO = -1, DESCR = "SELECCIONE" });
                    SelectEstatusAdministrativo = -1;
                    ListTipoIngreso.Insert(0, new TIPO_INGRESO() { ID_TIPO_INGRESO = -1, DESCR = "SELECCIONE" });
                    SelectTipoIngreso = -1;
                    IngresoDelitos.Insert(0, new INGRESO_DELITO() { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE", ID_FUERO = string.Empty });
                    IngresoDelito = -1;
                    ListClasificacionJuridica.Insert(0, new CLASIFICACION_JURIDICA() { ID_CLASIFICACION_JURIDICA = string.Empty, DESCR = "SELECCIONE" });
                    SelectClasificacionJuridica = string.Empty;
                }));
                await TreeViewViewModel();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener los datos de ingreso.", ex);
            }

        }
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
                                FolioBuscar = Convert.ToInt32(textbox.Text);
                                break;
                            case "AnioBuscar":
                                AnioBuscar = int.Parse(textbox.Text);
                                break;
                        }
                    }
                }
                TabVisible = false;
                ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                SelectIngresoEnabled = false;
                if (ListExpediente != null)
                    EmptyExpedienteVisible = ListExpediente.Count < 0;
                else
                    EmptyExpedienteVisible = true;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }

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

        private void getDatosGenerales()
        {
            try
            {
                if (ListEstadoCivil == null || ListEstadoCivil.Count < 1)
                {
                    ListEstadoCivil = ListEstadoCivil ?? new ObservableCollection<ESTADO_CIVIL>((new cEstadoCivil()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListEstadoCivil.Insert(0, new ESTADO_CIVIL() { ID_ESTADO_CIVIL = -1, DESCR = "SELECCIONE" });
                        SelectEstadoCivil = -1;
                    }));
                }

                if (ListOcupacion == null || ListOcupacion.Count < 1)
                {
                    ListOcupacion = ListOcupacion ?? new ObservableCollection<OCUPACION>(new cOcupacion().ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListOcupacion.Insert(0, new OCUPACION() { ID_OCUPACION = -1, DESCR = "SELECCIONE" });
                        SelectOcupacion = -1;
                    }));
                }

                if (ListEscolaridad == null || ListEscolaridad.Count < 1)
                {
                    ListEscolaridad = ListEscolaridad ?? new ObservableCollection<ESCOLARIDAD>((new cEscolaridad()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListEscolaridad.Insert(0, new ESCOLARIDAD() { ID_ESCOLARIDAD = -1, DESCR = "SELECCIONE" });
                        SelectEscolaridad = -1;
                    }));
                }

                if (ListReligion == null || ListReligion.Count < 1)
                {
                    ListReligion = ListReligion ?? new ObservableCollection<RELIGION>((new cReligion()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListReligion.Insert(0, new RELIGION() { ID_RELIGION = -1, DESCR = "SELECCIONE" });
                        SelectReligion = -1;
                    }));
                }

                if (ListEtnia == null || ListEtnia.Count < 1)
                {
                    ListEtnia = ListEtnia ?? new ObservableCollection<ETNIA>((new cEtnia()).ObtenerTodos().OrderBy(o => o.DESCR));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListEtnia.Insert(0, new ETNIA() { ID_ETNIA = -1, DESCR = "SELECCIONE" });
                        SelectEtnia = -1;
                    }));
                }

                if (ListPaisNacionalidad == null || ListPaisNacionalidad.Count < 1)
                {
                    ListPaisNacionalidad = ListPaisNacionalidad ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerNacionalidad().OrderBy(o => o.NACIONALIDAD));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListPaisNacionalidad.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });
                    }));
                }
                if (ListPaisNacimiento == null || ListPaisNacimiento.Count < 1)
                {
                    ListPaisNacimiento = ListPaisNacimiento ?? new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos().OrderBy(o => o.PAIS));
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListPaisNacimiento.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE", NACIONALIDAD = "SELECCIONE" });
                        ListPaisDomicilioMadre = ListPaisDomicilioPadre = ListPaisDomicilio = ListPaisNacimiento;
                        SelectPaisNacimiento = SelectPais = SelectNacionalidad = Parametro.PAIS; //82;
                        TextFechaNacimiento = null;
                    }));
                }
                if (LstIdioma == null)
                {
                    LstIdioma = LstIdioma ?? new ObservableCollection<IDIOMA>(new cIdioma().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstIdioma.Insert(0, new IDIOMA() { ID_IDIOMA = -1, DESCR = "SELECCIONE" });
                        SelectedIdioma = -1;
                    }));
                }
                if (LstDialecto == null)
                {
                    LstDialecto = LstDialecto ?? new ObservableCollection<DIALECTO>(new cDialecto().ObtenerTodos());
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        LstDialecto.Insert(0, new DIALECTO() { ID_DIALECTO = -1, DESCR = "SELECCIONE" });
                        SelectedDialecto = -1;
                    }));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar catalogos de datos generales.", ex);
            }

        }
        private void getDatosImputado()
        {
            getDatosGeneralesIdentificacion();
        }
        private void getDatosGeneralesIdentificacion()
        {
            try
            {
                if (Imputado == null)
                    Imputado = new IMPUTADO();
                var ingreso = Imputado.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                #region IDENTIFICACION

                #region DatosGenerales
                SelectEtnia = Imputado.ID_ETNIA == null ? -1 : Imputado.ID_ETNIA;
                SelectEscolaridad = ingreso.ID_ESCOLARIDAD == null ? -1 : ingreso.ID_ESCOLARIDAD;
                SelectOcupacion = ingreso.ID_OCUPACION == null ? -1 : ingreso.ID_ESCOLARIDAD;
                SelectEstadoCivil = ingreso.ID_ESTADO_CIVIL == null ? -1 : ingreso.ID_ESTADO_CIVIL;
                SelectNacionalidad = Imputado.ID_NACIONALIDAD == null ? -1 : Imputado.ID_NACIONALIDAD;
                SelectReligion = ingreso.ID_RELIGION == null ? -1 : ingreso.ID_RELIGION;
                SelectSexo = string.IsNullOrEmpty(Imputado.SEXO) ? "S" : Imputado.SEXO;
                SelectedIdioma = Imputado.ID_IDIOMA == null ? -1 : Imputado.ID_IDIOMA;
                SelectedDialecto = Imputado.ID_DIALECTO == null ? -1 : Imputado.ID_DIALECTO;
                SelectReligion = ingreso.ID_RELIGION == null ? -1 : ingreso.ID_RELIGION;
                RequiereTraductor = (!string.IsNullOrEmpty(Imputado.TRADUCTOR) && Imputado.TRADUCTOR.Equals("S"));

                /*RequiereTraductor = string.IsNullOrEmpty(Imputado.TRADUCTOR) ? false : Imputado.TRADUCTOR.Equals("S") ? true : false;
                if (Imputado.TRADUCTOR != null)
                    if (Imputado.TRADUCTOR.Equals("S"))
                        RequiereTraductor = true;
                    else
                        RequiereTraductor = false;*/

                #endregion

                #region Domicilio
                SelectPais = ingreso.ID_PAIS == null ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : ingreso.ID_PAIS < 1 ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : ingreso.ID_PAIS;
                SelectEntidad = ingreso.ID_ENTIDAD == null ? CentroActual.ID_ENTIDAD : ingreso.ID_ENTIDAD < 1 ? CentroActual.ID_ENTIDAD : ingreso.ID_ENTIDAD;
                SelectMunicipio = ingreso.ID_MUNICIPIO == null ? CentroActual.ID_MUNICIPIO : ingreso.ID_MUNICIPIO < 1 ? CentroActual.ID_MUNICIPIO : ingreso.ID_MUNICIPIO;
                SelectColoniaItem = ingreso.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ingreso.ID_COLONIA > 1 ? ListColonia.Where(w => w.ID_COLONIA == ingreso.ID_COLONIA).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();


                TextCalle = ingreso.DOMICILIO_CALLE;
                TextNumeroExterior = ingreso.DOMICILIO_NUM_EXT;
                TextNumeroInterior = ingreso.DOMICILIO_NUM_INT;
                AniosEstado = ingreso.RESIDENCIA_ANIOS.ToString();
                MesesEstado = ingreso.RESIDENCIAS_MESES.ToString();
                int mes = 0, anio = 0;
                int.TryParse(MesesEstado, out mes);
                int.TryParse(AniosEstado, out anio);
                FechaEstado = Fechas.GetFechaDateServer.AddYears(-(anio)).AddMonths(-(mes));
                TextTelefono = ingreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(ingreso.TELEFONO.Value.ToString()) : string.Empty;
                TextCodigoPostal = ingreso.DOMICILIO_CP.HasValue ? (int)ingreso.DOMICILIO_CP : new Nullable<int>();
                TextDomicilioTrabajo = ingreso.DOMICILIO_TRABAJO;
                #endregion

                #region Nacimiento
                SelectPaisNacimiento = Imputado.NACIMIENTO_PAIS == null ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : Imputado.NACIMIENTO_PAIS;
                SelectEntidadNacimiento = Imputado.NACIMIENTO_ESTADO == null ? CentroActual.ID_ENTIDAD : Imputado.NACIMIENTO_ESTADO;
                SelectMunicipioNacimiento = Imputado.NACIMIENTO_MUNICIPIO == null ? CentroActual.ID_MUNICIPIO : Imputado.NACIMIENTO_MUNICIPIO;

                TextFechaNacimiento = Imputado.NACIMIENTO_FECHA == null ?
                    Fechas.GetFechaDateServer.AddYears(-18) :
                    Imputado.NACIMIENTO_FECHA;
                TextLugarNacimientoExtranjero = Imputado.NACIMIENTO_LUGAR;
                #endregion

                #region Padres
                getDatosDomiciliosPadres();
                #endregion

                ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                //ALIAS DEL INTECONEXION
                if (SelectedInterconexion != null)
                {
                    if (Imputado.PATERNO != SelectedInterconexion.PRIMERAPELLIDO || Imputado.MATERNO != SelectedInterconexion.SEGUNDOAPELLIDO ||
                                       Imputado.NOMBRE != SelectedInterconexion.NOMBRE)
                    {
                        if (ListAlias == null)
                            ListAlias = new ObservableCollection<ALIAS>();
                        ListAlias.Add(new ALIAS()
                        {
                            NOMBRE = SelectedInterconexion.NOMBRE,//Imputado.NOMBRE,
                            PATERNO = SelectedInterconexion.PRIMERAPELLIDO,//Imputado.PATERNO,
                            MATERNO = SelectedInterconexion.SEGUNDOAPELLIDO//Imputado.MATERNO
                        });
                    }
                }

                ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);

                #region Nuevo
                var ing = Imputado.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                if (ing != null)
                {
                    SelectEstadoCivil = ing.ID_ESTADO_CIVIL;
                    SelectOcupacion = ing.ID_OCUPACION;
                    SelectEscolaridad = ing.ID_ESCOLARIDAD;
                    SelectReligion = ing.ID_RELIGION;
                    //Domicilio
                    TextCalle = ing.DOMICILIO_CALLE;
                    TextNumeroExterior = ing.DOMICILIO_NUM_EXT;
                    TextNumeroInterior = ing.DOMICILIO_NUM_INT;

                    SelectPais = ing.ID_PAIS == null ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : ing.ID_PAIS < 1 ? CentroActual.MUNICIPIO.ENTIDAD.ID_PAIS_NAC : ing.ID_PAIS;
                    SelectEntidad = ing.ID_ENTIDAD == null ? CentroActual.ID_ENTIDAD : ing.ID_ENTIDAD < 1 ? CentroActual.ID_ENTIDAD : ing.ID_ENTIDAD;
                    SelectMunicipio = ing.ID_MUNICIPIO == null ? CentroActual.ID_MUNICIPIO : ing.ID_MUNICIPIO < 1 ? CentroActual.ID_MUNICIPIO : ing.ID_MUNICIPIO;
                    SelectColoniaItem = ing.ID_COLONIA == null ? ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault() : ing.ID_COLONIA > 1 ? ListColonia.Where(w => w.ID_COLONIA == ing.ID_COLONIA).FirstOrDefault() : ListColonia.Where(w => w.ID_COLONIA == -1).FirstOrDefault();

                    TextCodigoPostal = ing.DOMICILIO_CP;
                    //En el estado
                    AniosEstado = ing.RESIDENCIA_ANIOS.ToString();
                    MesesEstado = ing.RESIDENCIAS_MESES.ToString();

                    TextTelefono = ing.TELEFONO.HasValue ? new Converters().MascaraTelefono(ing.TELEFONO.Value.ToString()) : string.Empty;
                    TextDomicilioTrabajo = ingreso.DOMICILIO_TRABAJO = TextDomicilioTrabajo;

                    //Padres
                    CheckPadreFinado = ing.PADRE_FINADO == "S" ? true : false;
                    CheckMadreFinado = ing.MADRE_FINADO == "S" ? true : false;
                }
                #endregion
                
                #endregion
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al al cargar los datos generales.", ex);
            }

        }
        private void getDatosDomiciliosPadres()
        {
            try
            {
                var ingreso = Imputado.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                TextPadreMaterno = Imputado.MATERNO_PADRE;
                TextPadrePaterno = Imputado.PATERNO_PADRE;
                TextPadreNombre = Imputado.NOMBRE_PADRE;
                if (ingreso.PADRE_FINADO == "S")
                    CheckPadreFinado = true;
                else
                    CheckPadreFinado = false;

                TextMadreMaterno = Imputado.MATERNO_MADRE;
                TextMadrePaterno = Imputado.PATERNO_MADRE;
                TextMadreNombre = Imputado.NOMBRE_MADRE;
                if (ingreso.MADRE_FINADO == "S")
                    CheckMadreFinado = true;
                else
                    CheckMadreFinado = false;

                #region Padres
                if (Imputado.IMPUTADO_PADRES.Count > 0)
                {
                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "P")
                        {
                            if (item.ID_PAIS == null)
                                SelectPaisDomicilioPadre = Parametro.PAIS; // 82;
                            else
                                SelectPaisDomicilioPadre = item.ID_PAIS;
                            if (item.ID_ENTIDAD == null)
                                SelectEntidadDomicilioPadre = Parametro.ESTADO; //2;
                            else
                                SelectEntidadDomicilioPadre = item.ID_ENTIDAD;
                            if (item.ID_MUNICIPIO == null)
                                SelectMunicipioDomicilioPadre = -1;
                            else
                                SelectMunicipioDomicilioPadre = item.ID_MUNICIPIO;
                            if (item.ID_COLONIA == null)
                                SelectColoniaDomicilioPadre = -1;
                            else
                                SelectColoniaDomicilioPadre = item.ID_COLONIA;

                            TextCalleDomicilioPadre = item.CALLE;//Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "P").FirstOrDefault().calle;
                            TextNumeroExteriorDomicilioPadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioPadre = item.NUM_INT;
                            TextCodigoPostalDomicilioPadre = item.CP;
                        }
                    }
                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "M")
                        {
                            if (item.ID_PAIS == null)
                                SelectPaisDomicilioMadre = Parametro.PAIS; //82;
                            else
                                SelectPaisDomicilioMadre = item.ID_PAIS;
                            if (item.ID_ENTIDAD == null)
                                SelectEntidadDomicilioMadre = Parametro.ESTADO; //2;
                            else
                                SelectEntidadDomicilioMadre = item.ID_ENTIDAD;
                            if (item.ID_MUNICIPIO == null)
                                SelectMunicipioDomicilioMadre = -1;
                            else
                                SelectMunicipioDomicilioMadre = item.ID_MUNICIPIO;
                            if (item.ID_COLONIA == null)
                                SelectColoniaDomicilioMadre = -1;
                            else
                                SelectColoniaDomicilioMadre = item.ID_COLONIA;
                            TextCalleDomicilioMadre = item.CALLE;   // Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "P").FirstOrDefault().calle;
                            TextNumeroExteriorDomicilioMadre = item.NUM_EXT;
                            TextNumeroInteriorDomicilioMadre = item.NUM_INT;
                            TextCodigoPostalDomicilioMadre = item.CP;
                        }
                    }
                }
                else
                {
                    SelectPaisDomicilioPadre = Parametro.PAIS;// 82;
                    TextCalleDomicilioPadre = "";
                    TextNumeroExteriorDomicilioPadre = null;
                    TextNumeroInteriorDomicilioPadre = "";
                    TextCodigoPostalDomicilioPadre = null;

                    SelectPaisDomicilioMadre = Parametro.PAIS;//82;
                    TextCalleDomicilioMadre = "";
                    TextNumeroExteriorDomicilioMadre = null;
                    TextNumeroInteriorDomicilioMadre = "";
                    TextCodigoPostalDomicilioMadre = null;
                }
                #endregion

                if (Imputado.IMPUTADO_PADRES.Count == 1 && Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "P").Any())
                {
                    MismoDomicilioMadre = true;
                    MismoDomicilioPadre = false;
                }
                else if (Imputado.IMPUTADO_PADRES.Count == 1 && Imputado.IMPUTADO_PADRES.Where(w => w.ID_PADRE == "M").Any())
                {
                    MismoDomicilioMadre = false;
                    MismoDomicilioPadre = true;
                }
                else if (Imputado.IMPUTADO_PADRES.Count == 2)
                {
                    MismoDomicilioMadre = false;
                    MismoDomicilioPadre = false;
                }
                else
                {
                    MismoDomicilioMadre = false;
                    MismoDomicilioPadre = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Codigo viejo, remover despues de inspeccion de edgar.
        //private bool addNuevoExpediente()
        //{
        //    if (!EditarImputado)
        //    {
        //        if (!string.IsNullOrEmpty(NombreBuscar) && (!string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
        //        {
        //            Imputado = new IMPUTADO();
        //            Imputado.ID_CENTRO = GlobalVar.gCentro;
        //            Imputado.ID_ANIO = (short)Fechas.GetFechaDateServer.Year;
        //            Imputado.PATERNO = ApellidoPaternoBuscar;
        //            Imputado.MATERNO = ApellidoMaternoBuscar;
        //            Imputado.NOMBRE = NombreBuscar;
        //            Imputado.ID_IMPUTADO = (new cImputado()).Insertar(Imputado);
        //            if (Imputado.ID_IMPUTADO > 0)
        //            {
        //                //media filiacion
        //                GuardarMediaFiliacion();
        //                AnioBuscar = Imputado.ID_ANIO;
        //                FolioBuscar = Imputado.ID_IMPUTADO;
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        //private async Task<bool> updateImputado()
        //{
        //    var imputado = new IMPUTADO();
        //    imputado.ID_CENTRO = Imputado.ID_CENTRO;
        //    imputado.ID_ANIO = Imputado.ID_ANIO;
        //    imputado.ID_IMPUTADO = Imputado.ID_IMPUTADO;
        //    if (SelectedInterconexion != null)
        //    {
        //        imputado.PATERNO = SelectedInterconexion.PRIMERAPELLIDO;
        //        imputado.MATERNO = SelectedInterconexion.SEGUNDOAPELLIDO;
        //        imputado.NOMBRE = SelectedInterconexion.NOMBRE;
        //    }
        //    else
        //    {
        //        imputado.PATERNO = Imputado.PATERNO;
        //        imputado.MATERNO = Imputado.MATERNO;
        //        imputado.NOMBRE = Imputado.NOMBRE;
        //    }
        //    #region DatosGenerales
        //    imputado.SEXO = SelectSexo;
        //    imputado.ID_ESTADO_CIVIL = SelectEstadoCivil;
        //    imputado.ID_OCUPACION = SelectOcupacion;
        //    imputado.ID_ESCOLARIDAD = SelectEscolaridad;
        //    imputado.ID_RELIGION = SelectReligion;
        //    imputado.ID_ETNIA = SelectEtnia;
        //    imputado.ID_NACIONALIDAD = SelectNacionalidad;

        //    //nuevos campos
        //    imputado.ID_IDIOMA = SelectedIdioma;
        //    imputado.ID_DIALECTO = SelectedDialecto;
        //    imputado.TRADUCTOR = RequiereTraductor ? "S" : "N";

        //    #endregion

        //    #region Domicilio
        //    imputado.DOMICILIO_CALLE = TextCalle;
        //    imputado.DOMICILIO_NUM_INT = TextNumeroInterior;
        //    imputado.DOMICILIO_NUM_EXT = TextNumeroExterior;
        //    imputado.ID_COLONIA = SelectColoniaItem.ID_COLONIA;
        //    imputado.ID_MUNICIPIO = SelectMunicipio;
        //    imputado.ID_ENTIDAD = SelectEntidad;
        //    imputado.ID_PAIS = SelectPais;
        //    imputado.DOMICILIO_CODIGO_POSTAL = TextCodigoPostal;
        //    imputado.TELEFONO = long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""));
        //    imputado.DOMICILIO_TRABAJO = TextDomicilioTrabajo;
        //    if (!string.IsNullOrEmpty(AniosEstado))
        //        imputado.RESIDENCIA_ANIOS = short.Parse(AniosEstado);
        //    if (!string.IsNullOrEmpty(MesesEstado))
        //        imputado.RESIDENCIA_MESES = short.Parse(MesesEstado);
        //    #endregion

        //    #region Nacimiento
        //    imputado.NACIMIENTO_PAIS = SelectPaisNacimiento;
        //    imputado.NACIMIENTO_ESTADO = SelectEntidadNacimiento;
        //    imputado.NACIMIENTO_MUNICIPIO = SelectMunicipioNacimiento;
        //    imputado.NACIMIENTO_FECHA = TextFechaNacimiento;
        //    imputado.NACIMIENTO_LUGAR = TextLugarNacimientoExtranjero;
        //    #endregion

        //    #region Padres
        //    imputado.NOMBRE_PADRE = TextPadreNombre;
        //    imputado.MATERNO_PADRE = TextPadreMaterno;
        //    imputado.PATERNO_PADRE = TextPadrePaterno;
        //    imputado.NOMBRE_MADRE = TextMadreNombre;
        //    imputado.MATERNO_MADRE = TextMadreMaterno;
        //    imputado.PATERNO_MADRE = TextMadrePaterno;
        //    if (CheckPadreFinado)
        //        imputado.PADRE_FINADO = "S";
        //    else
        //        imputado.PADRE_FINADO = "N";
        //    if (CheckMadreFinado)
        //        imputado.MADRE_FINADO = "S";
        //    else
        //        imputado.MADRE_FINADO = "N";
        //    #endregion

        //    if (!(new cImputado()).Actualizar(imputado))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        SelectExpediente = imputado;
        //        if (!CheckMadreFinado || !CheckPadreFinado)
        //        {
        //            if (!await GuardarDomiciliosPadres())
        //                return false;
        //        }
        //    }
        //    if (!await GuardarApodosAlias())
        //        return false;
        //    if (SelectTipoIngreso == 4)
        //        if (!GuardarTraslado())
        //            return false;
        //    if (!await GuardarFotosYHuellas())
        //        return false;

        //    //if (SelectedInterconexion != null)
        //    //    GuardarNUC();
        //    return true;
        //}
        //private async Task<bool> GuardarDomiciliosPadres()
        //{
        //    var padres = new List<IMPUTADO_PADRES>();
        //    if (!MismoDomicilioPadre && !CheckPadreFinado)
        //    {
        //        padres.Add(new IMPUTADO_PADRES()
        //        {
        //            ID_PADRE = "P",
        //            ID_CENTRO = Imputado.ID_CENTRO,
        //            ID_ANIO = Imputado.ID_ANIO,
        //            ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //            ID_PAIS = SelectPaisDomicilioPadre,
        //            ID_ENTIDAD = SelectEntidadDomicilioPadre,
        //            ID_MUNICIPIO = SelectMunicipioDomicilioPadre,
        //            ID_COLONIA = SelectColoniaDomicilioPadre,
        //            CALLE = TextCalleDomicilioPadre,
        //            NUM_EXT = TextNumeroExteriorDomicilioPadre,
        //            NUM_INT = TextNumeroInteriorDomicilioPadre,
        //            CP = TextCodigoPostalDomicilioPadre
        //        });
        //    }
        //    if (!MismoDomicilioMadre && !CheckMadreFinado)
        //    {
        //        padres.Add(new IMPUTADO_PADRES()
        //        {
        //            ID_PADRE = "M",
        //            ID_CENTRO = Imputado.ID_CENTRO,
        //            ID_ANIO = Imputado.ID_ANIO,
        //            ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //            ID_PAIS = SelectPaisDomicilioMadre,
        //            ID_ENTIDAD = SelectEntidadDomicilioMadre,
        //            ID_MUNICIPIO = SelectMunicipioDomicilioMadre,
        //            ID_COLONIA = SelectColoniaDomicilioMadre,
        //            CALLE = TextCalleDomicilioMadre,
        //            NUM_EXT = TextNumeroExteriorDomicilioMadre,
        //            NUM_INT = TextNumeroInteriorDomicilioMadre,
        //            CP = TextCodigoPostalDomicilioMadre
        //        });
        //    }
        //    if (padres.Count > 0)
        //    {
        //        if ((new cImputadoPadres()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, padres))
        //        {
        //            SelectExpediente.IMPUTADO_PADRES = padres;
        //            return await TaskEx.FromResult(true);
        //        }
        //        else
        //            return await TaskEx.FromResult(false);
        //    }

        //    return true;
        //}
        //private async Task<bool> GuardarApodosAlias()
        //{
        //    if (!saveApodos())
        //        return false;
        //    if (!saveAlias())
        //        return false;
        //    if (!await saveRelacionesPersonales())
        //        return false;
        //    return true;
        //}
        //private async Task<bool> GuardarFotosYHuellas()
        //{
        //    var guardafotos = false;
        //    var guardahuellas = false;
        //    if (ImagesToSave.Count != 3)
        //        return await TaskEx.FromResult(false);
        //    else if (ImagesToSave.Where(w => w.FrameName == "LeftFace" && w.ImageCaptured != null).Count() == 0)
        //        return await TaskEx.FromResult(false);
        //    else if (ImagesToSave.Where(w => w.FrameName == "RightFace" && w.ImageCaptured != null).Count() == 0)
        //        return await TaskEx.FromResult(false);
        //    else if (ImagesToSave.Where(w => w.FrameName == "FrontFace" && w.ImageCaptured != null).Count() == 0)
        //        return await TaskEx.FromResult(false);
        //    else
        //    {
        //        #region [Fotos]
        //        var ingresoBiometrico = new List<SSP.Servidor.INGRESO_BIOMETRICO>();
        //        foreach (var item in ImagesToSave)
        //        {
        //            var encoder = new JpegBitmapEncoder();
        //            encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
        //            encoder.QualityLevel = 100;
        //            var bit = new byte[0];
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                encoder.Frames.Add(BitmapFrame.Create(item.ImageCaptured));
        //                encoder.Save(stream);
        //                bit = stream.ToArray();
        //                stream.Close();
        //            }

        //            ingresoBiometrico.Add(new SSP.Servidor.INGRESO_BIOMETRICO()
        //            {
        //                BIOMETRICO = bit,
        //                ID_ANIO = Imputado.ID_ANIO,
        //                ID_CENTRO = Imputado.ID_CENTRO,
        //                ID_INGRESO = Ingreso.ID_INGRESO,
        //                ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                ID_TIPO_BIOMETRICO = item.FrameName == "LeftFace" ? (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO : item.FrameName == "FrontFace" ? (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO : item.FrameName == "RightFace" ? (short)enumTipoBiometrico.FOTO_DER_REGISTRO : (short)0,
        //                ID_FORMATO = (short)enumTipoFormato.FMTO_JPG
        //            });
        //        }
        //        if (new cIngresoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_INGRESO == Ingreso.ID_INGRESO && w.ID_TIPO_BIOMETRICO >= (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_TIPO_BIOMETRICO <= (short)enumTipoBiometrico.FOTO_IZQ_REGISTRO).ToList().Count() == 3)
        //        {
        //            #region [Actualizar Fotos]
        //            guardafotos = (new cIngresoBiometrico()).Actualizar(ingresoBiometrico);
        //            #endregion
        //        }
        //        else
        //        {
        //            #region [Intertar Fotos]
        //            guardafotos = (new cIngresoBiometrico()).Eliminar(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO, Ingreso.ID_INGRESO, true);
        //            guardafotos = (new cIngresoBiometrico()).Insertar(ingresoBiometrico);
        //            #endregion
        //        }
        //        if (guardafotos)
        //        {
        //            Ingreso.INGRESO_BIOMETRICO = ingresoBiometrico;
        //            SelectIngreso = Ingreso;
        //        }
        //        #endregion

        //        #region [Huellas]
        //        var imputadoBiometrico = new List<SSP.Servidor.IMPUTADO_BIOMETRICO>();
        //        if (HuellasCapturadas != null)
        //            foreach (var item in HuellasCapturadas)
        //            {
        //                imputadoBiometrico.Add(new SSP.Servidor.IMPUTADO_BIOMETRICO()
        //                {
        //                    ID_ANIO = Imputado.ID_ANIO,
        //                    ID_CENTRO = Imputado.ID_CENTRO,
        //                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                    BIOMETRICO = item.BIOMETRICO,
        //                    ID_TIPO_BIOMETRICO = (short)item.ID_TIPO_BIOMETRICO,
        //                    ID_FORMATO = (short)item.ID_TIPO_FORMATO,
        //                    CALIDAD = item.CALIDAD.HasValue ? item.CALIDAD : null
        //                });
        //            }

        //        if (new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && (w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP || w.ID_FORMATO == (short)enumTipoFormato.FMTO_WSQ)).ToList().Count() == 20)
        //        {
        //            #region [actualizar biometrico]
        //            HuellasCapturadas = null;
        //            if (imputadoBiometrico.Any())
        //                guardahuellas = (new cImputadoBiometrico()).Actualizar(imputadoBiometrico);
        //            else
        //                guardahuellas = true;
        //            #endregion
        //        }
        //        else
        //        {
        //            #region [insertar Huellas]
        //            HuellasCapturadas = null;
        //            guardahuellas = (new cImputadoBiometrico()).Eliminar(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO);
        //            if (imputadoBiometrico.Any())
        //                guardahuellas = (new cImputadoBiometrico()).Insertar(imputadoBiometrico);
        //            else
        //                guardahuellas = true;
        //            #endregion
        //        }
        //        if (guardahuellas)
        //            Imputado.IMPUTADO_BIOMETRICO = imputadoBiometrico;
        //        #endregion
        //    }
        //    if (guardahuellas && guardafotos)
        //        return await TaskEx.FromResult(true);
        //    else
        //        return await TaskEx.FromResult(false);
        //}
        //private bool addDatosIngreso()
        //{
        //    try
        //    {
        //        if (SelectedCama != null)
        //        {
        //            var ingreso = new INGRESO();
        //            ingreso.ID_CENTRO = Imputado.ID_CENTRO;
        //            ingreso.ID_ANIO = Imputado.ID_ANIO;
        //            ingreso.ID_IMPUTADO = Imputado.ID_IMPUTADO;

        //            //DATOS_INGRESO
        //            ingreso.FEC_REGISTRO = FechaRegistroIngreso;
        //            ingreso.FEC_INGRESO_CERESO = FechaCeresoIngreso;
        //            ingreso.ID_TIPO_INGRESO = SelectTipoIngreso;
        //            ingreso.ID_CLASIFICACION_JURIDICA = SelectClasificacionJuridica;
        //            ingreso.ID_ESTATUS_ADMINISTRATIVO = SelectEstatusAdministrativo;
        //            ingreso.ID_INGRESO_DELITO = IngresoDelito;

        //            //DATOS_DOCUMENTO_INTERNACION
        //            ingreso.ID_DISPOSICION = SelectTipoDisposicion;
        //            ingreso.ID_AUTORIDAD_INTERNA = SelectTipoAutoridadInterna;
        //            ingreso.DOCINTERNACION_NUM_OFICIO = TextNumeroOficio;
        //            ingreso.ID_TIPO_SEGURIDAD = SelectTipoSeguridad;

        //            //UBICACION
        //            ingreso.ID_UB_CENTRO = SelectedCama.ID_CENTRO;
        //            ingreso.ID_UB_EDIFICIO = SelectedCama.ID_EDIFICIO;
        //            ingreso.ID_UB_SECTOR = SelectedCama.ID_SECTOR;
        //            ingreso.ID_UB_CELDA = SelectedCama.ID_CELDA;
        //            ingreso.ID_UB_CAMA = SelectedCama.ID_CAMA;

        //            if (EditarIngreso)
        //            {
        //                ingreso.ID_INGRESO = Ingreso.ID_INGRESO;
        //                if ((new cIngreso()).Actualizar(ingreso))
        //                {
        //                    var cama = new CAMA()
        //                       {
        //                           ID_CAMA = SelectedCama.ID_CAMA,
        //                           ID_CELDA = SelectedCama.ID_CELDA,
        //                           ID_CENTRO = SelectedCama.ID_CENTRO,
        //                           ID_EDIFICIO = SelectedCama.ID_EDIFICIO,
        //                           ID_SECTOR = SelectedCama.ID_SECTOR,
        //                           DESCR = SelectedCama.DESCR,
        //                           ESTATUS = "N"
        //                       };
        //                    if (new cCama().Actualizar(cama))
        //                        return false;
        //                }
        //            }
        //            else
        //            {
        //                //Ingreso = new INGRESO();
        //                //Ingreso = ingreso;

        //                ingreso.ID_INGRESO = new cIngreso().Insertar(ingreso);
        //                var selectCama = SelectedCama;
        //                Ingreso = new cIngreso().Obtener(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO);
        //                SelectedCama = selectCama;
        //                var cama = new CAMA()
        //                {
        //                    //ID_CAMA = Ingreso.ID_UB_CAMA.Value,
        //                    //ID_CELDA = Ingreso.ID_UB_CELDA,
        //                    //ID_CENTRO = Ingreso.ID_UB_CENTRO.Value,
        //                    //ID_EDIFICIO = Ingreso.ID_UB_EDIFICIO.Value,
        //                    //ID_SECTOR = Ingreso.ID_UB_SECTOR.Value,
        //                    //DESCR = Ingreso.CAMA.DESCR,
        //                    ID_CAMA = SelectedCama.ID_CAMA,
        //                    ID_CELDA = SelectedCama.ID_CELDA,
        //                    ID_CENTRO = SelectedCama.ID_CENTRO,
        //                    ID_EDIFICIO = SelectedCama.ID_EDIFICIO,
        //                    ID_SECTOR = SelectedCama.ID_SECTOR,
        //                    DESCR = SelectedCama.DESCR,
        //                    ESTATUS = "N"
        //                };
        //                EditarIngreso = true;
        //                if (new cCama().Actualizar(cama))
        //                    return false;

        //            }
        //        }
        //    }
        //    catch (Exception ex) { }
        //    return true;
        //}
        //private bool saveApodos()
        //{
        //    if (ListApodo != null)
        //        if (ListApodo.Count > 0)
        //        {
        //            var apodos = new List<APODO>();
        //            short id_apodo = 1;
        //            foreach (var entidad in ListApodo)
        //            {
        //                //id_apodo = (new cApodo()).GetSequence<short>("APODOS_SEQ");
        //                apodos.Add(new APODO
        //                {
        //                    ID_CENTRO = Imputado.ID_CENTRO,
        //                    ID_ANIO = Imputado.ID_ANIO,
        //                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                    ID_APODO = id_apodo,
        //                    APODO1 = entidad.APODO1
        //                });
        //                id_apodo++;
        //            }
        //            if (!(new cApodo()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, apodos))
        //                return false;
        //            Imputado.APODO = apodos;
        //        }
        //    return true;
        //}
        //private bool saveAlias()
        //{
        //    if (ListAlias != null)
        //        if (ListAlias.Count > 0)
        //        {
        //            var alias = new List<ALIAS>();
        //            short id_alias = 1;
        //            foreach (var entidad in ListAlias)
        //            {
        //                //id_alias = (new cAlias()).GetSequence<short>("ALIAS_SEQ");
        //                alias.Add(new ALIAS
        //                {
        //                    ID_CENTRO = Imputado.ID_CENTRO,
        //                    ID_ANIO = Imputado.ID_ANIO,
        //                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                    ID_ALIAS = id_alias,
        //                    PATERNO = entidad.PATERNO,
        //                    MATERNO = entidad.MATERNO,
        //                    NOMBRE = entidad.NOMBRE
        //                });
        //                id_alias++;
        //            }
        //            if (!(new cAlias()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, alias))
        //                return false;
        //            Imputado.ALIAS = alias;
        //        }
        //    return true;
        //}
        //private async Task<bool> saveRelacionesPersonales()
        //{
        //    if (ListRelacionPersonalInterno != null)
        //        if (ListRelacionPersonalInterno.Count > 0)
        //        {
        //            var relacion_personal_internos = new List<RELACION_PERSONAL_INTERNO>();
        //            foreach (var entidad in ListRelacionPersonalInterno)
        //            {
        //                relacion_personal_internos.Add(new RELACION_PERSONAL_INTERNO
        //                {
        //                    ID_CENTRO = Imputado.ID_CENTRO,
        //                    ID_ANIO = Imputado.ID_ANIO,
        //                    ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                    NOTA = entidad.NOTA,
        //                    ID_REL_ANIO = entidad.INGRESO.ID_ANIO,
        //                    ID_REL_CENTRO = entidad.INGRESO.ID_CENTRO,
        //                    ID_REL_IMPUTADO = entidad.INGRESO.ID_IMPUTADO,
        //                    ID_REL_INGRESO = entidad.INGRESO.ID_INGRESO
        //                });
        //            }
        //            if (!(new cRelacionPersonalInterno()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, relacion_personal_internos))
        //                return false;
        //            Imputado.RELACION_PERSONAL_INTERNO = await StaticSourcesViewModel.CargarDatosAsync<List<RELACION_PERSONAL_INTERNO>>(() => new cRelacionPersonalInterno().Obtener(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
        //        }
        //    return true;
        //}
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
                    PaternoAlias = string.Empty;
                    MaternoAlias = string.Empty;
                    NombreAlias = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el alias.", ex);
            }
        }

        private void EliminarAlias()
        {
            try
            {
                if (SelectAlias != null && SelectAlias.IMPUTADO == null)
                    ListAlias.Remove(SelectAlias);
                SelectAlias = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar el alias.", ex);
            }
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
                    Apodo = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar el apodo.", ex);
            }

        }

        private void EliminarApodo()
        {
            try
            {
                if (SelectApodo != null && SelectApodo.IMPUTADO == null)
                    ListApodo.Remove(SelectApodo);
                SelectApodo = null;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar el apodo.", ex);
            }
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en buscar las relaciones con internos.", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en insertar las relaciones con internos.", ex);
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
                    PaternoRelacionInterno = MaternoRelacionInterno = NombreRelacionInterno = NotaRelacionInterno = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.RELACION_INTERNO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en agregar las relaciones con internos.", ex);
            }

        }

        private void EliminarRelacionInterno()
        {
            try
            {
                if (selectRelacionPersonalInterno != null && SelectRelacionPersonalInterno.IMPUTADO == null)
                    listRelacionPersonalInterno.Remove(selectRelacionPersonalInterno);
                selectRelacionPersonalInterno = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en eliminar las relaciones con internos.", ex);
            }

        }
        #endregion

        #region [TREEVIEW]
        public async Task<bool> TreeViewViewModel()
        {
            try
            {
                _TreeList = new List<TreeViewList>();
                var Registros = new List<CAMA>();

               // if (Centro != null)
               Registros = await StaticSourcesViewModel.CargarDatosAsync<List<CAMA>>(() => new cCama().GetData().OrderBy(o => new { o.ID_CENTRO, o.CELDA.SECTOR.EDIFICIO.ID_EDIFICIO, o.CELDA.SECTOR.ID_SECTOR, o.CELDA.ID_CELDA, o.ID_CAMA }).Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ESTATUS == "S").ToList());

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

                TreeList.Clear();
                TreeList.Add(ItemTreeViewCentro);
                TreeList.Expand();
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar el arbol de ubicaciones.", ex);
                return false;
            }



        }

        private void SetIsSelectedProperty(TreeViewList TreeViewItem, bool isSelected)
        {
            if (SelectedItem != null)
                if (SelectedItem != TreeViewItem)
                    SelectedItem.IsCheck = false;
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
        private void OnUnLoad(RegistroIngresoView Window = null)
        {
            PopUpsViewModels.MainWindow.BuscarView.Visibility = Visibility.Collapsed;
        }
        private async void OnLoad(FotosHuellasDigitalesView Window = null)
        {
            try
            {
                base.ClearRules();
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
                }
                #endregion
                SetValidacionesGenerales();

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

                    var obj = ImagesToSave.Where(w => w.FrameName == Picture.Name).FirstOrDefault();
                    if (obj != null)
                    {
                        ImagesToSave.Remove(obj);
                    }
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
            await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));

            await TaskEx.Delay(400);

            var nRet = -1;
            var bandera = true;
            var requiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaRegistro;
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
                    {
                        
                        if (PConsultar)//Mientras tenga permisos e consultar
                            CamposBusquedaEnabled = true;
                        return;
                    }

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
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
            AceptarBusquedaHuellaFocus = true;
        }

        private void CargarHuellas()
        {
            try
            {
                if (imputado == null)
                    return;
                var LoadHuellas = new Thread((Init) =>
                {
                    var Huellas = new cImputadoBiometrico().GetData().Where(w => w.ID_ANIO == imputado.ID_ANIO && w.ID_CENTRO == imputado.ID_CENTRO && w.ID_IMPUTADO == imputado.ID_IMPUTADO && w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 30).ToList();

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

                if (Imputado != null)
                {
                    if (Imputado.IMPUTADO_BIOMETRICO != null ? Imputado.IMPUTADO_BIOMETRICO.Count > 0 : true)
                    {
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        foreach (var biometrico in Imputado.IMPUTADO_BIOMETRICO)
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

        #region [PANDILLA]
        async void GetPandilla()
        {
            Pandilla = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PANDILLA>>(() => new ObservableCollection<PANDILLA>((new cPandilla()).ObtenerTodos()));
        }

        async void GetImputadoPandilla()
        {
            ImputadoPandilla = ImputadoPandilla ?? await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO_PANDILLA>>(() => new cImputadoPandilla().Obtener(Imputado.ID_ANIO, Imputado.ID_CENTRO, Imputado.ID_IMPUTADO));
        }

        private void AgregarPandilla()
        {
            setValidacionesIdentificacionPandillas();
            SelectedPandillaValue = -1;
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PANDILLA);
        }

        private void CancelarPandilla()
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
                if (SelectedImputadoPandilla != null)
                    ImputadoPandilla.Remove(SelectedImputadoPandilla);
                ImputadoPandilla.Add(new IMPUTADO_PANDILLA { ID_ANIO = Imputado.ID_ANIO, ID_CENTRO = Imputado.ID_CENTRO, ID_IMPUTADO = Imputado.ID_IMPUTADO, PANDILLA = SelectedPandillaItem, NOTAS = NotaPandilla, ID_PANDILLA = SelectedPandillaValue });
                this.CancelarPandilla();
            }
        }

        private void EliminarPandilla()
        {
            if (SelectedImputadoPandilla != null)
            {
                ImputadoPandilla.Remove(SelectedImputadoPandilla);
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
            if (ImputadoPandilla != null)
            {
                if (ImputadoPandilla.Count > 0)
                {
                    var pandillaList = new List<IMPUTADO_PANDILLA>();
                    foreach (var entidad in ImputadoPandilla)
                    {
                        pandillaList.Add(new IMPUTADO_PANDILLA { ID_CENTRO = entidad.ID_CENTRO, ID_ANIO = entidad.ID_ANIO, ID_IMPUTADO = entidad.ID_IMPUTADO, ID_PANDILLA = entidad.ID_PANDILLA, NOTAS = entidad.NOTAS });
                    }
                    if ((new cImputadoPandilla()).Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, pandillaList))
                    {
                        SetValidacionesGenerales();
                        return true;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error", "Al guardar pandillas.");
                }
            }
            return false;
        }
        #endregion

        #region [TAB_LOADS]
        private async void IngresoLoad(RegistroIngresoView Window = null)
        {
            try
            {
                //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
                //OBTENEMOS EL CENTRO ACTUAL
                CentroActual = await StaticSourcesViewModel.CargarDatosAsync<CENTRO>(() => new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault());
                await StaticSourcesViewModel.CargarDatosMetodoAsync(this.getDatosGenerales);
                Window.Unloaded += (s, e) =>
                {
                    try
                    {
                        OnUnLoad(Window);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de ingreso", ex);
                    }
                };
                PopUpsViewModels.MainWindow.ApodoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        ApodosLoad(PopUpsViewModels.MainWindow.ApodoView);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar apodos", ex);
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
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar relación", ex);
                    }
                };
                PopUpsViewModels.MainWindow.AgregaDocumentoView.IsVisibleChanged += (s, e) =>
                {
                    try
                    {
                        DocumentoLoad(PopUpsViewModels.MainWindow.AgregaDocumentoView);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar documento", ex);
                    }
                };
                ConfiguraPermisos();
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error alCARGAR PANTALLA", ex);
            }
        }
        private void DatosIngresoInternoLoad(DatosIngresoInternoView Window = null)
        {
            if (IsSelectedDatosIngreso)
            {
                if (BanderaEntrada)
                    SetValidacionesGenerales();
                OnPropertyChanged("");
            }
        }
        private void DatosIdentificacionLoad(DatosGeneralesIdentificacionView Window = null)
        {
            if (IsSelectedIdentificacion)
            {
                PopulateDatosGeneralesInterconexion();
                SetValidacionesGenerales();
                Thread.Sleep(150);
                OnPropertyChanged();
            }
        }
        private async void TrasladoLoad(IngresoTrasladoView Window = null)
        {
            if (IsSelectedTraslado)
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(getDatosTraslados);
                SetValidacionesGenerales();
            }

        }
        private void ApodosLoad(AgregarApodoView Window = null)
        {
            setValidacionesIdentificacionApodos();
        }
        private void AliasLoad(AgregarAliasView Window = null)
        {
            if (ListAlias != null)
                ListAlias = new ObservableCollection<ALIAS>(ListAlias);
            setValidacionesIdentificacionAlias();
        }
        private void RelacionLoad(AgregarRelacionInternoView Window = null)
        {
            setValidacionesIdentificacionRelacionesPersonales();
        }
        private void DocumentoLoad(AgregarDocumentoView Window = null)
        {
            this.PopulateFunciones();
            setValidacionesDocumentos();
            OnPropertyChanged("");
        }
        private void ApodosAliasReferenciasLoad(ApodosAliasReferenciasView Window = null)
        {
            if (IsSelectedIdentificacion && TabApodosAlias)
            {
                if (Imputado != null)
                {
                    if (ListAlias == null)
                    {
                        if (Imputado.ALIAS != null)
                            ListAlias = new ObservableCollection<ALIAS>(Imputado.ALIAS);
                        else
                            ListAlias = new ObservableCollection<ALIAS>();
                        if (SelectedInterconexion != null)
                        {
                            if (Imputado.PATERNO != SelectedInterconexion.PRIMERAPELLIDO || Imputado.MATERNO != SelectedInterconexion.SEGUNDOAPELLIDO ||
                                Imputado.NOMBRE != SelectedInterconexion.NOMBRE)
                            {
                                ListAlias.Add(new ALIAS()
                                {
                                    NOMBRE = SelectedInterconexion.NOMBRE,//Imputado.NOMBRE,
                                    PATERNO = SelectedInterconexion.PRIMERAPELLIDO,//Imputado.PATERNO,
                                    MATERNO = SelectedInterconexion.SEGUNDOAPELLIDO//Imputado.MATERNO
                                });
                            }
                        }
                    }
                    if (ListApodo == null)
                    {
                        if (Imputado.APODO != null)
                            ListApodo = new ObservableCollection<APODO>(Imputado.APODO);
                        else
                            ListApodo = new ObservableCollection<APODO>();
                    }
                    if (ListRelacionPersonalInterno == null)
                    {
                        if (Imputado.RELACION_PERSONAL_INTERNO != null)
                            ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);
                        else
                            ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
                    }
                }
                else
                {
                    if (ListAlias == null)
                        ListAlias = new ObservableCollection<ALIAS>();
                    if (ListApodo == null)
                        ListApodo = new ObservableCollection<APODO>();
                    if (ListRelacionPersonalInterno == null)
                        ListRelacionPersonalInterno = new ObservableCollection<RELACION_PERSONAL_INTERNO>();
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region TRASLADOS
        private void getDatosTraslados()
        {
            try
            {
                if (lstCentroOrigenTraslado == null)
                {
                    lstCentroOrigenTraslado = new ObservableCollection<EMISOR>(new cEmisor().Obtener().OrderBy(o=>o.ID_EMISOR));
                    lstCentroOrigenTraslado.Insert(0, new EMISOR() { ID_EMISOR = -1, DESCR = "SELECCIONE" });
                }
                RaisePropertyChanged("LstCentroOrigenTraslado");
                if (lstMotivoTraslado == null)
                {
                    lstMotivoTraslado = new ObservableCollection<TRASLADO_MOTIVO>(new cTrasladoMotivo().ObtenerTodos());
                    lstMotivoTraslado.Insert(0, new TRASLADO_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" });
                }
                RaisePropertyChanged("LstMotivoTraslado");
                //id_autoridad_traslado = Parametro.AUTORIDAD_TRASLADO;
                autoridad_traslado = Parametro.AUTORIDAD_TRASLADO ;
                RaisePropertyChanged("Autoridad_Traslado");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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

        private void LimpiarDatosTraslado()
        {
            DTMotivo = -1;
            DTCentroOrigen = -1;
            DTJustificacion = DTNoOficio = string.Empty;
        }
        #endregion

        #region [REPORTE]
        private void Reporte()
        {
            try
            {
                if (Imputado != null)
                {
                    ReporteIngreso reporte = new ReporteIngreso();
                    reporte.Nombre = string.Format("{0} {1} {2}", Imputado.NOMBRE, Imputado.PATERNO, Imputado.MATERNO);
                    reporte.Alias = " ";
                    if (Imputado.ALIAS != null)
                    {
                        string alias = string.Empty;
                        foreach (var a in Imputado.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", a.NOMBRE, a.PATERNO, a.MATERNO);
                        }
                    }
                    reporte.Apodo = " ";
                    if (Imputado.APODO != null)
                    {
                        string apodos = string.Empty;
                        foreach (var a in Imputado.APODO)
                        {
                            if (!string.IsNullOrEmpty(apodos))
                                apodos = string.Format("{0},", apodos);
                            apodos = apodos + a.APODO1;
                        }
                    }

                    //reporte.EstadoCivil = Imputado.ESTADO_CIVIL != null ? Imputado.ESTADO_CIVIL.DESCR : " ";
                    reporte.EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR : " ";
                    reporte.Conyugue = " ";
                    reporte.Originario = " ";
                    reporte.FecNacimiento = " ";
                    //reporte.Escolaridad = Imputado.ESCOLARIDAD != null ? Imputado.ESCOLARIDAD.DESCR : " ";
                    reporte.Escolaridad = SelectIngreso.ESCOLARIDAD != null ? SelectIngreso.ESCOLARIDAD.DESCR : " ";
                    reporte.DomicilioActual = " ";//string.Format("{0} {1},{2},{3},{4}", Imputado.DOMICILIO_CALLE, Imputado.DOMICILIO_NUM_EXT, Imputado.COLONIA.DESCR, Imputado.COLONIA.MUNICIPIO.MUNICIPIO1, Imputado.COLONIA.MUNICIPIO.ENTIDAD.DESCR);
                    reporte.Edad = " ";
                    reporte.TiempoBC = " ";
                    //reporte.Telefono = Imputado.TELEFONO != null ? Imputado.TELEFONO.ToString() : " ";
                    reporte.Telefono = SelectIngreso.TELEFONO != null ? SelectIngreso.TELEFONO.ToString() : " ";
                    //reporte.Ocupacion = Imputado.OCUPACION != null ? Imputado.OCUPACION.DESCR : " ";
                    reporte.Ocupacion = SelectIngreso.OCUPACION != null ? SelectIngreso.OCUPACION.DESCR : " ";
                    //reporte.NombreMadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_MADRE, Imputado.PATERNO_MADRE, Imputado.MATERNO_MADRE, Imputado.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    reporte.NombreMadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_MADRE, Imputado.PATERNO_MADRE, Imputado.MATERNO_MADRE, SelectIngreso.MADRE_FINADO.Equals("S") ? "FINADO" : string.Empty);
                    //reporte.NombrePadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_PADRE, Imputado.PATERNO_PADRE, Imputado.MATERNO_PADRE, Imputado.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty); ;
                    reporte.NombrePadre = string.Format("{0} {1} {2} {3}", Imputado.NOMBRE_PADRE, Imputado.PATERNO_PADRE, Imputado.MATERNO_PADRE, SelectIngreso.PADRE_FINADO.Equals("S") ? "FINADO" : string.Empty); 
                    reporte.DomicilioPadres = " ";

                    var v = new EditorView(reporte);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    v.Owner = PopUpsViewModels.MainWindow;
                    v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    v.Show();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al generar reporte", ex);
            }


        }
        #endregion

        #region INTERCONEXION
        private void OnBuscarNUCInterconexion(string obj = "")
        {
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
                    LstImputadoFiliacion = ((BusquedaNUCViewModel)windowBusqueda.DataContext).LstImputadoFiliacion;

                    Imputado = ((BusquedaNUCViewModel)windowBusqueda.DataContext).Imputado;
                    if (Imputado == null)
                    {
                        if (SelectedInterconexion != null)
                        {
                            ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                            ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                            NombreBuscar = SelectedInterconexion.NOMBRE;
                            clickSwitch("buscar_visible");
                            //SelectExpediente = Imputado;
                        }
                        CamposBusquedaEnabled = true;
                        return;
                    }
                    else
                    {
                        AnioBuscar = Imputado.ID_ANIO;
                        FolioBuscar = Imputado.ID_IMPUTADO;
                        ApellidoPaternoBuscar = Imputado.PATERNO;
                        ApellidoMaternoBuscar = Imputado.MATERNO;
                        NombreBuscar = Imputado.NOMBRE;
                        clickSwitch("buscar_visible");
                        SelectExpediente = Imputado;
                        #region Nombre Interconexión
                        //if (SelectedInterconexion != null)
                        //{
                        //    ApellidoPaternoBuscar = SelectedInterconexion.PRIMERAPELLIDO;
                        //    ApellidoMaternoBuscar = SelectedInterconexion.SEGUNDOAPELLIDO;
                        //    NombreBuscar = SelectedInterconexion.NOMBRE;
                        //}
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
        }

        //eliminar codigo despues de revision de edgar
        //private void GuardarMediaFiliacion()
        //{
        //    if (LstImputadoFiliacion != null)
        //    {
        //        var mf = new List<IMPUTADO_FILIACION>();
        //        foreach (var x in LstImputadoFiliacion)
        //        {
        //            mf.Add(new IMPUTADO_FILIACION()
        //            {
        //                ID_CENTRO = Imputado.ID_CENTRO,
        //                ID_ANIO = Imputado.ID_ANIO,
        //                ID_IMPUTADO = Imputado.ID_IMPUTADO,
        //                ID_MEDIA_FILIACION = x.ID_MEDIA_FILIACION,
        //                ID_TIPO_FILIACION = x.ID_TIPO_FILIACION,
        //                //FEC_CAPTURA = DateTime.Now
        //            });
        //        }
        //        new cImputadoFiliacion().Insertar(Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO, mf);
        //    }
        //}
        #endregion

        #region [metodos de carga de datos]
        //async void CargarAlias()
        //{
        //    StaticSourcesViewModel.CargarDatosAsync<>(() => );
        //}
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.INGRESO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al configurar permisos en la pantalla", ex);
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
