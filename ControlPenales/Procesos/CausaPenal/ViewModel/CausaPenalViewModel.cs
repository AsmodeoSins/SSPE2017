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
using ControlPenales.Clases;
using WPFPdfViewer;
using System.IO;
using ControlPenales.BiometricoServiceReference;
//using Novacode;
using System.Diagnostics;
using Novacode;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private short?[] estatus_inactivos=null;

        public CausaPenalViewModel() { }

        #region variables
        private string headerRegistro = "Registro Discrecional";
        public string HeaderRegistro
        {
            get { return headerRegistro; }
            set { headerRegistro = value; OnPropertyChanged("HeaderRegistro"); }
        }
        private bool bandera_buscar = false;
        private bool discrecionalVisible = false;
        public bool DiscrecionalVisible
        {
            get { return discrecionalVisible; }
            set { discrecionalVisible = value; OnPropertyChanged("DiscrecionalVisible"); }
        }
        private bool agregarCausaPenalVisible = false;
        public bool AgregarCausaPenalVisible
        {
            get { return agregarCausaPenalVisible; }
            set { agregarCausaPenalVisible = value; OnPropertyChanged("AgregarCausaPenalVisible"); }
        }
        private bool datosCausaPenalVisible = false;
        public bool DatosCausaPenalVisible
        {
            get { return datosCausaPenalVisible; }
            set { datosCausaPenalVisible = value; OnPropertyChanged("DatosCausaPenalVisible"); }
        }
        private bool popupBuscarDelitoVisible;
        public bool PopupBuscarDelitoVisible
        {
            get { return popupBuscarDelitoVisible; }
            set { popupBuscarDelitoVisible = value; OnPropertyChanged("PopupBuscarDelitoVisible"); }
        }
        private bool datosIngresoVisible = false;
        public bool DatosIngresoVisible
        {
            get { return datosIngresoVisible; }
            set { datosIngresoVisible = value; OnPropertyChanged("DatosIngresoVisible"); }
        }
        private bool ingresosVisible = true;
        public bool IngresosVisible
        {
            get { return ingresosVisible; }
            set { ingresosVisible = value; OnPropertyChanged("IngresosVisible"); }
        }
        private bool discrecionVisible;
        public bool DiscrecionVisible
        {
            get { return discrecionVisible; }
            set { discrecionVisible = value; OnPropertyChanged("DiscrecionVisible"); }
        }
        private bool tabVisible = false;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }

        #region TABS
        private bool ingresoTab = true;
        public bool IngresoTab
        {
            get { return ingresoTab; }
            set { ingresoTab = value; OnPropertyChanged("IngresoTab"); }
        }
        private bool causaPenalTab = true;
        public bool CausaPenalTab
        {
            get { return causaPenalTab; }
            set { causaPenalTab = value; OnPropertyChanged("CausaPenalTab"); }
        }
        private bool coparticipeTab = true;
        public bool CoparticipeTab
        {
            get { return coparticipeTab; }
            set { coparticipeTab = value; OnPropertyChanged("CoparticipeTab"); }
        }
        private bool sentenciaTab = true;
        public bool SentenciaTab
        {
            get { return sentenciaTab; }
            set { sentenciaTab = value; OnPropertyChanged("SentenciaTab"); }
        }
        private bool rdiTab = true;
        public bool RdiTab
        {
            get { return rdiTab; }
            set { rdiTab = value; OnPropertyChanged("RdiTab"); }
        }
        private bool delitosTab = true;
        public bool DelitosTab
        {
            get { return delitosTab; }
            set { delitosTab = value; OnPropertyChanged("DelitosTab"); }
        }
        private bool delitoTab = true;
        public bool DelitoTab
        {
            get { return delitoTab; }
            set { delitoTab = value; OnPropertyChanged("DelitoTab"); }
        }
        private bool recursoTab = true;
        public bool RecursoTab
        {
            get { return recursoTab; }
            set { recursoTab = value; OnPropertyChanged("RecursoTab"); }
        }
        private bool recursosTab;
        public bool RecursosTab
        {
            get { return recursosTab; }
            set { recursosTab = value; OnPropertyChanged("RecursosTab"); }
        }
        private bool amparoDirectoListaTab = false;
        public bool AmparoDirectoListaTab
        {
            get { return amparoDirectoListaTab; }
            set { amparoDirectoListaTab = value; OnPropertyChanged("AmparoDirectoListaTab"); }
        }
        private bool amparoDirectoTab = false;
        public bool AmparoDirectoTab
        {
            get { return amparoDirectoTab; }
            set { amparoDirectoTab = value; OnPropertyChanged("AmparoDirectoTab"); }
        }
        private bool amparoIndirectoListaTab = false;
        public bool AmparoIndirectoListaTab
        {
            get { return amparoIndirectoListaTab; }
            set { amparoIndirectoListaTab = value; OnPropertyChanged("AmparoIndirectoListaTab"); }
        }
        private bool amparoIndirectoTab = false;
        public bool AmparoIndirectoTab
        {
            get { return amparoIndirectoTab; }
            set { amparoIndirectoTab = value; OnPropertyChanged("AmparoIndirectoTab"); }
        }
        private bool amparoIncidenteListaTab = false;
        public bool AmparoIncidenteListaTab
        {
            get { return amparoIncidenteListaTab; }
            set { amparoIncidenteListaTab = value; OnPropertyChanged("AmparoIncidenteListaTab"); }
        }
        private bool amparoIncidenteTab = false;
        public bool AmparoIncidenteTab
        {
            get { return amparoIncidenteTab; }
            set { amparoIncidenteTab = value; OnPropertyChanged("AmparoIncidenteTab"); }
        }
        /////////////////////////////////////////
        private bool ingreso2Tab = true;
        public bool Ingreso2Tab
        {
            get { return ingreso2Tab; }
            set { ingreso2Tab = value; OnPropertyChanged("Ingreso2Tab"); }
        }
        private bool causaPenal2Tab;
        public bool CausaPenal2Tab
        {
            get { return causaPenal2Tab; }
            set { causaPenal2Tab = value; OnPropertyChanged("CausaPenal2Tab"); }
        }
        private bool coparticipe2Tab;
        public bool Coparticipe2Tab
        {
            get { return coparticipe2Tab; }
            set { coparticipe2Tab = value; OnPropertyChanged("Coparticipe2Tab"); }
        }
        private bool sentencia2Tab;
        public bool Sentencia2Tab
        {
            get { return sentencia2Tab; }
            set { sentencia2Tab = value; OnPropertyChanged("Sentencia2Tab"); }
        }
        private bool rdi2Tab;
        public bool Rdi2Tab
        {
            get { return rdi2Tab; }
            set { rdi2Tab = value; OnPropertyChanged("Rdi2Tab"); }
        }
        private bool delitos2Tab;
        public bool Delitos2Tab
        {
            get { return delitos2Tab; }
            set { delitos2Tab = value; OnPropertyChanged("Delitos2Tab"); }
        }
        private bool delito2Tab;
        public bool Delito2Tab
        {
            get { return delito2Tab; }
            set { delito2Tab = value; OnPropertyChanged("Delito2Tab"); }
        }
        private bool recurso2Tab;
        public bool Recurso2Tab
        {
            get { return recurso2Tab; }
            set { recurso2Tab = value; OnPropertyChanged("Recurso2Tab"); }
        }
        #endregion

        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "nueva_busqueda":
                        if (ListExpediente != null)
                            ListExpediente.Clear();
                        SelectExpediente = null;
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        break;
                    case "buscar_menu":
                        tImputado = SelectExpediente;
                        tIngreso = SelectIngreso;
                        SelectExpediente = null;
                        SelectIngreso = null;
                        if (ListExpediente != null)
                            ListExpediente.Clear();
                        SelectExpediente = null;
                        ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                        FolioBuscar = AnioBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        break;
                    case "buscar_visible":
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        this.GetImputados();
                        break;
                    case "buscar_salir":
                        SelectIngreso = null;

                        //if (SelectedIngreso != null)
                        //{
                        //    if (SelectedIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                        //        ImagenIngreso = SelectedIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        //    else
                        //        ImagenIngreso = new Imagenes().getImagenPerson();
                        //}
                        //else
                        //    ImagenIngreso = new Imagenes().getImagenPerson();
                        SelectExpediente = tImputado;
                        SelectIngreso = tIngreso;
                        tImputado = null;
                        tIngreso = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        break;
                    case "buscar_seleccionar":
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                            return;
                        }
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        //foreach (var item in EstatusInactivos)
                        //{
                        //    if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        //    {
                        //        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        //        return;
                        //    }
                        //}
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        short[] Estatus = { 1, 2, 3, 8 };
                        if (Estatus.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value))
                        {
                            if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                    SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                                return;
                            }
                        }
                        SelectedInterno = SelectIngreso.IMPUTADO;
                        if (SelectIngreso != null)
                            SelectedIngreso = SelectIngreso;
                        LstIngresosCentro = new ObservableCollection<INGRESO>(SelectExpediente.INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro).OrderByDescending(o => o.ID_INGRESO));
                        IngresosVisible = true;
                        DatosIngresoVisible = false;
                        this.SeleccionaIngreso();
                        this.ViewModelArbol();
                        EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                        LimpiarBusqueda();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        
                        if (!Estatus.Contains(SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no se encuentra activo la información sera solo de consulta");
                            MenuGuardarEnabled = false;
                        }
                        else
                            MenuGuardarEnabled = true;
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "agregar_causa_penal":
                        TabVisible = false;
                        DiscrecionVisible = true;
                        AgregarCausaPenalVisible = false;
                        bandera_buscar = true;
                        GuardarBandera = false;
                        TabIngresoSelected = true;
                        //LIMPIAMOS LAS VARIABLES//////
                        SelectedCausaPenal = null;
                        SelectedCoparticipe = null;
                        SelectedSentencia = null;
                        SelectedDelito = null;
                        SelectedRecurso = null;
                        ///////////////////////////////
                        await TaskEx.Delay(100);
                        TabCausaPenalSelected = true;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        break;
                    case "ingresos_discrecionales":
                        TabVisible = false;
                        DiscrecionVisible = true;
                        DatosIngresoVisible = false;
                        IngresosVisible = true;
                        AgregarCausaPenalVisible = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        break;
                    case "seleccionar_delito_buscar":
                        PopupBuscarDelitoVisible = false;
                        break;
                    case "cancelar_seleccionar_delito":
                        SelectedUbicacion = null;
                        PopupBuscarDelitoVisible = false;
                        break;
                    case "digitalizar_causa_penal":
                        ObtenerTipoDocumento(5);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;
                    //DELITO
                    case "insertar_delito":
                        DelitoVisible = true;
                        break;
                    case "editar_delito":
                        DelitoVisible = true;
                        break;
                    //COPARTICIPES
                    case "insertar_coparticipe":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TituloModal = "Agregar Coparticipe";
                        LimpiarCoparticipe();
                        SelectedCoparticipe = null;
                        setValidacionesAgregarCoparticipe();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_COPARTICIPE);
                        break;
                    case "editar_coparticipe":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TituloModal = "Editar Coparticipe";
                        setValidacionesAgregarCoparticipe();
                        PopulateCoparticipe();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_COPARTICIPE);
                        break;
                    case "eliminar_coparticipe":
                        EliminarCoparticipe();
                        break;
                    case "agregar_coparticipe":
                        if (!base.HasErrors)
                        {
                            if (LstCoparticipe == null)
                                LstCoparticipe = new ObservableCollection<COPARTICIPE>();
                            if (LstCoparticipe.Count(w => w.NOMBRE == NombreCoparticipe && w.PATERNO == PaternoCoparticipe && w.MATERNO == MaternoCoparticipe) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El coparticipe ya se encuentra registrado");
                                return;
                            }
                            AgregarCoparticipe();
                            base.ClearRules();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_COPARTICIPE);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los datos obligatorios.");
                        break;
                    case "cancelar_coparticipe":
                        base.ClearRules();
                        LimpiarCoparticipe();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_COPARTICIPE);
                        break;

                    //ALIAS
                    case "insertar_coparticipe_alias":
                        if (SelectedCoparticipe != null)
                        {
                            if (!PInsertar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            SelectedAlias = null;
                            TituloAlias = "Agregar Alias";
                            LimpiarCoparticipeAlias();
                            setValidacionesAgregarCoparticipeAlias();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN!", "Debe seleccionar un coparticipe!");
                        break;
                    case "editar_coparticipe_alias":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TituloAlias = "Editar Alias";
                        setValidacionesAgregarCoparticipeAlias();
                        PopulateAlias();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ALIAS);
                        break;
                    case "eliminar_coparticipe_alias":
                        EliminarAlias();
                        break;
                    case "guardar_alias":
                        if (!base.HasErrors)
                        {
                            if (LstAlias == null)
                                LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>();
                            if (LstAlias.Count(w => w.NOMBRE == NombreAlias && w.PATERNO == PaternoAlias && w.MATERNO == MaternoAlias && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El alias ya se encuentra registrado");
                                break;
                            }
                            AgregarAlias();
                            base.ClearRules();
                            LimpiarComparticipeApodo();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
                        
                        break;
                    case "cancelar_alias":
                        base.ClearRules();
                        LimpiarCoparticipeAlias();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                        break;

                    //APODO
                    case "insertar_coparticipe_apodo":
                        if (SelectedCoparticipe != null)
                        {
                            if (!PInsertar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            TituloApodo = "Agregar Apodo";
                            setValidacionesAgregarCoparticipeApodo();
                            LimpiarComparticipeApodo();
                            SelectedApodo = null;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN!", "Debe seleccionar un coparticipe!");
                        break;
                    case "editar_coparticipe_apodo":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TituloApodo = "Editar Apodo";
                        setValidacionesAgregarCoparticipeApodo();
                        PopulateApodo();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.APODO);
                        break;
                    case "eliminar_coparticipe_apodo":
                        EliminarApodo();
                        break;
                    case "guardar_apodo":
                        if (!base.HasErrors)
                        {
                            if (LstApodo == null)
                                LstApodo = new ObservableCollection<COPARTICIPE_APODO>();
                            if(SelectedApodo == null)
                                if (LstApodo.Count(w => w.APODO == Apodo && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación","El apodo ya se encuentra registrado");
                                    break;
                                }
                            AgregarApodo();
                            base.ClearRules();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los campos requeridos");
                        
                        break;
                    case "cancelar_apodo":
                        base.ClearRules();
                        LimpiarComparticipeApodo();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                        break;
                    //CAUSA PENAL DELITO
                    case "insertar_delito_causa_penal":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        SelectedCausaPenalDelito = null;
                        SelectedDelito = null;
                        EditarCPDelito = DelitoExpander = false;
                        DelitoD = ModalidadD = GraveD = CantidadD = ObjetoD = string.Empty;
                        TipoD = -1;
                        TextHelper = string.Empty;
                        //ViewModelArbolDelito();
                        SetValidacionesAgregarDelito();
                        //DelitoVisible = true;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        break;
                    case "editar_delito_causa_penal":
                        if (SelectedCausaPenalDelito != null)
                        {
                            //COPIA VISIBLE///////////////
                            if (!PEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            DelitoCopia = false;
                            DelitoCopiaVisible = false;
                            //////////////////////////////
                            EditarCPDelito = true;
                            //ViewModelArbolDelito();
                            this.EditarCausaPenalDelito(IndexMenu);
                            SetValidacionesAgregarDelito();
                            //DelitoVisible = true;
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un delito");
                        break;
                    case "eliminar_delito_causa_penal":
                        //if (BandDelito > 0)
                        //    this.EliminarCausaPenalDelito(BandDelito);
                        //else
                        //    this.EliminarCausaPenalDelito(IndexMenu);
                        //BandDelito = 0;
                        switch (IndexMenu)
                        {
                            case 3://SENTENCIA
                                EliminarDelitoSentencia();
                                break;
                            case 8://RECUSRSO
                                EliminarDelitoRecurso();
                                break;
                            default://CAUSA PENAL
                                EliminarDelitoCausaPenal();
                                break;
                        }
                        break;
                    case "agregar_delito"://"agregar_delito_causa_penal":
                        if (!PInsertar && !PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        var se_agrego = false;
                        switch (IndexMenu)
                        {
                            case 3://SENTENCIA
                                if(AgregarDelitoSentencia())
                                {
                                    SetValidacionesSentencia();
                                    se_agrego= true;
                                }
                                break;
                            case 5://DELITOS
                                if (BandDelito == 1)
                                {
                                    if(AgregarDelitoCausaPenal())
                                        se_agrego  =true;
                                }
                                else if (BandDelito == 3)
                                {
                                    if(AgregarDelitoSentencia())
                                        se_agrego= true;
                                }
                                break;
                            case 8://RECUSRSO
                                if (AgregarDelitoRecurso())
                                {
                                    SetValidacionesRecurso();
                                    se_agrego = true;
                                }
                                break;
                            default://CAUSA PENAL
                                if (AgregarDelitoCausaPenal())
                                {
                                    SetValidacionesCausaPenal();
                                    se_agrego = true;
                                }
                                break;
                        }
                        //SetValidacionesIngresoCausaPenal();
                        if (se_agrego)
                        {
                            #region Limpiar Filtro Arbol
                            TextHelper = string.Empty;
                            LimpiarListasDelitos();
                            #endregion
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        }
                        
                        break;
                    case "cancelar_delito":
                        //SetValidacionesIngresoCausaPenal();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        //this.CancelarCausaPenalDelito();
                        break;
                    //LISTADO DELITOS
                    case "insertar_delito_causa_penal_listado":
                        //COPIA VISIBLE/////////////
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TextoDelitoCopia = "Copiar delito en la sentencia.";
                        DelitoCopia = false;
                        DelitoCopiaVisible = true;
                        ////////////////////////////
                        EditarCPDelito = false;
                        DelitoExpander = false;
                        SelectedDelito = null;
                        DelitoD = ModalidadD = GraveD = CantidadD = ObjetoD = string.Empty;
                        TipoD = -1;
                        BandDelito = 1;
                        SelectedCausaPenalDelito = null;
                        //DelitoVisible = true;
                        // ViewModelArbolDelito();
                        SetValidacionesAgregarDelito();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        break;
                    case "insertar_delito_sentencia_listado":
                        //COPIA VISIBLE/////////////
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TextoDelitoCopia = "Copiar delito en la causa penal.";
                        DelitoCopia = false;
                        DelitoCopiaVisible = true;
                        ////////////////////////////
                        EditarCPDelito = false;
                        DelitoExpander = false;
                        SelectedDelito = null;
                        DelitoD = ModalidadD = GraveD = CantidadD = objetoD = string.Empty;
                        TipoD = -1;
                        BandDelito = 3;
                        //DelitoVisible = true;
                        SelectedSentenciaDelito = null;
                        //ViewModelArbolDelito();
                        TextHelper = string.Empty;
                        SetValidacionesAgregarDelito();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        break;
                    case "editar_delito_causa_penal_listado":
                        if (SelectedCausaPenalDelito != null)
                        {
                            //COPIA VISIBLE///////////////
                            if (!PEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            DelitoCopia = false;
                            DelitoCopiaVisible = false;
                            //////////////////////////////
                            EditarCPDelito = true;
                            BandDelito = 1;
                            EditarCausaPenalDelito(BandDelito);
                            //DelitoVisible = true;
                            //ViewModelArbolDelito();
                            SetValidacionesAgregarDelito();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un delito");

                        break;
                    case "editar_delito_sentencia_listado":
                        if (SelectedSentenciaDelito != null)
                        {
                            //COPIA VISIBLE///////////////
                            if (!PEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            DelitoCopia = false;
                            DelitoCopiaVisible = false;
                            //////////////////////////////
                            EditarCPDelito = true;
                            BandDelito = 3;
                            EditarCausaPenalDelito(BandDelito);
                            //DelitoVisible = true;
                            //ViewModelArbolDelito();
                            SetValidacionesAgregarDelito();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DELITO_CP);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un delito");
                        break;
                    case "eliminar_delito_causa_penal_listado":
                        if (SelectedCausaPenalDelito != null)
                        {
                            //BandDelito = 1;
                            //this.EliminarCausaPenalDelito(BandDelito);
                            //BandDelito = 0;
                            EliminarDelitoCausaPenal();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un delito");
                        break;
                    case "eliminar_delito_sentencia_listado":
                        if (SelectedSentenciaDelito != null)
                        {
                            //BandDelito = 3;
                            //this.EliminarCausaPenalDelito(BandDelito);
                            //BandDelito = 0;
                            EliminarDelitoSentencia();
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un delito");
                        break;
                    //RECURSOS
                    case "insertar_recurso":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        SetValidacionesRecurso();
                        LimpiarRecurso();
                        this.AgregarRecurso();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;
                    case "editar_recurso":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        TabRecursoSelected = true;
                        IngresosVisible = false;
                        DatosIngresoVisible = true;
                        //TABS
                        RecursoTab = true;
                        CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursosTab = false;

                        SetValidacionesRecurso();
                        LimpiarRecurso();
                        ObtenerRecurso();
                        StaticSourcesViewModel.SourceChanged = false;

                        break;
                    case "eliminar_recurso":
                        EliminarRecurso();
                        break;
                    case "digitalizar_recurso":
                        ObtenerTipoDocumento(4);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;
                    //INGRESO
                    case "ingreso_ubicacion":
                        ViewModelArbolUbicacion();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                        break;
                    case "cancelar_ubicacion":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION);
                        break;
                    //MENU
                    case "guardar_menu":
                        if (!InternoActivo())
                            break;

                        switch (IndexMenu)
                        {
                            case 0://INGRESO
                                if (!base.HasErrors)
                                {
                                    if (GuardarIngreso())
                                    {
                                        ViewModelArbol();
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos.");
                                break;
                            case 1://CAUSA PENAL
                                var valida = true;
                                if (EstatusCP == 0 || EstatusCP == 1)///POR COMPURGAR/ACTIVO
                                    if (!ValidarCambioEstatus())
                                        valida = false;
                                if (valida)
                                {
                                    if (!base.HasErrors)
                                    {
                                        if (GuardarCausaPenal())
                                        {
                                            //var si = SelectedIngreso;
                                            //var cp = SelectedCausaPenal;
                                            ViewModelArbol();
                                            //SelectedIngreso = si;
                                            //SelectedCausaPenal = cp;
                                            new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                            StaticSourcesViewModel.SourceChanged = false;
                                        }
                                        else
                                            new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos "+base.Error);
                                }
                                break;
                            case 2://COPARTICIPES
                                if (SelectedCausaPenal != null)
                                {
                                    if (GuardarCoparticipes())
                                    {
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar una causa penal.");
                                break;
                            case 3://SENTENCIA
                                
                                if (ValidarSentenciaAbonos())
                                {
                                    if (!base.HasErrors)
                                    {
                                        
                                        if (GuardarSentencia())
                                        {
                                            //var si = SelectedIngreso;
                                            //var cp = SelectedCausaPenal;
                                            ViewModelArbol();
                                            //SelectedIngreso = si;
                                            //SelectedCausaPenal = cp;
                                            new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                            StaticSourcesViewModel.SourceChanged = false;
                                        }
                                        else
                                            new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                                }
                                break;
                            case 5://DELITOS
                                if (!base.HasErrors)
                                {
                                    if (GuardarDelitos())
                                    {
                                        //var si = SelectedIngreso;
                                        //var cp = SelectedCausaPenal;
                                        ViewModelArbol();
                                        //SelectedIngreso = si;
                                        //SelectedCausaPenal = cp;
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos.");
                                break;
                            case 8://RECURSOS
                                if (!base.HasErrors)
                                {
                                    if (RTipoRecurso == (short)enumRecursosTipo.APELACION_O_DETERMINACION_SEGUNDA_INSTANCIA && RResultadoRecurso == ((short)enumResultado2Instancia.MODIFICA).ToString())
                                        if (SelectedCausaPenal.SENTENCIA.Count == 0)
                                            if (await new Dialogos().ConfirmarEliminar("Advertencia", "La causa penal seleccionada no cuenta con una sentencia,¿Desea continuar?") != 1)
                                                break;
                                    if (GuardarRecurso())
                                    {
                                        //var si = SelectedIngreso;
                                        //var cp = SelectedCausaPenal;
                                        ViewModelArbol();
                                        //SelectedIngreso = si;
                                        //SelectedCausaPenal = cp;
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        SelectedRecurso = null;
                                        TabRecursosSelected = true;
                                        IngresosVisible = false;
                                        DatosIngresoVisible = true;
                                        RecursosTab = true;
                                        CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = AmparoIncidenteTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos." + base.Error);
                                break;
                            case 10://AMPAROS DIRECTOS
                                if (!base.HasErrors)
                                {
                                    if (GuardarAmparoDirectoTransaccion())
                                    {
                                        //var si = SelectedIngreso;
                                        //var cp = SelectedCausaPenal;
                                        ViewModelArbol();
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        ObtenerTodoAmparoDirecto();
                                        TabAmparoDirectoListaSelected = true;
                                        IngresosVisible = false;
                                        DatosIngresoVisible = true;
                                        AmparoDirectoListaTab = true;
                                        RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoIndirectoTab = false;
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    //else
                                    //    new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos.");
                                break;
                            case 12://AMPAROS INDIRECTOS
                                if (!base.HasErrors)
                                {
                                    if (LstAIT == null)
                                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar tipo de amparo indirecto.");
                                    else
                                        if (LstAIT.Count == 0)
                                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar tipo de amparo indirecto.");
                                        else
                                        {
                                            if (GuardarAmparoIndirecto())
                                            {
                                                //var si = SelectedIngreso;
                                                //var cp = SelectedCausaPenal;
                                                ViewModelArbol();
                                                //SelectedIngreso = si;
                                                //SelectedCausaPenal = cp;
                                                new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                                StaticSourcesViewModel.SourceChanged = false;
                                                //Validación
                                                ValidarTraslados();
                                                if (EnCausaPenal)
                                                {
                                                    TabAmparoDirectoListaSelected = true;
                                                    IngresosVisible = false;
                                                    DatosIngresoVisible = true;
                                                    AmparoDirectoListaTab = true;
                                                    RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoIndirectoTab = false;
                                                    ObtenerTodoAmparoIndirecto(2);
                                                }
                                                else
                                                {
                                                    TabAmparoIndirectoListaSelected = true;
                                                    IngresosVisible = false;
                                                    DatosIngresoVisible = true;
                                                    AmparoIndirectoListaTab = true;
                                                    RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoDirectoListaTab = AmparoIndirectoTab = false;
                                                    ObtenerTodoAmparoIndirecto(1);
                                                }
                                            }
                                            else
                                                new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                        }
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos.");
                                break;
                            case 14://INCIDENTES
                                if (!base.HasErrors)
                                {
                                    //if (!ValidaGuardarIncidencia())
                                    //{
                                    //    new Dialogos().ConfirmacionDialogo("Notificacion!", "Estos incidentes deben ser agregados antes de dictar sentencia");
                                    //}
                                    //else
                                    if ((ITipo == (short)enumIncidenteTipo.ADECUACION && IResultado == "M") || (ITipo == (short)enumIncidenteTipo.REMISION_PARCIAL_DE_LA_PENA && IResultado == "C"))
                                        if (SelectedCausaPenal.SENTENCIA.Count == 0)
                                            if (await new Dialogos().ConfirmarEliminar("Advertencia", "La causa penal seleccionada no cuenta con una sentencia,¿Desea continuar?") != 1)
                                                break;
                                    if (GuardarIncidente())
                                    {
                                        //var si = SelectedIngreso;
                                        //var cp = SelectedCausaPenal;
                                        ViewModelArbol();
                                        //SelectedIngreso = si;
                                        //SelectedCausaPenal = cp;
                                        new Dialogos().ConfirmacionDialogo("Éxito", "Informaci\u00F3n registrada correctamente.");
                                        TabAmparoIncidenteListaSelected = true;
                                        IngresosVisible = false;
                                        DatosIngresoVisible = true;
                                        AmparoIncidenteListaTab = true;
                                        RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoIndirectoTab = AmparoIncidenteTab = false;
                                        //PopulateAmparoIncidenteListado();
                                        ObtenerTodoIncidente();
                                        StaticSourcesViewModel.SourceChanged = false;
                                    }
                                    else
                                        new Dialogos().ConfirmacionDialogo("Error", "No se registr\u00F3 la informaci\u00F3n.");
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los datos requeridos.");
                                break;
                            default://CAUSA PENAL
                                //AgregarCausaPenal();
                                break;
                        }
                        break;

                    //AMPAROS DIRECTOS
                    case "addAmparoDirecto":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        if (SelectedCausaPenal != null)
                        {
                            if (SelectedCausaPenal.SENTENCIA.Count > 0)
                            {
                                LimpiarAmparoDirecto();
                                SelectedAmparoDirecto = null;
                                TabAmparoDirectoSelected = true;
                                IngresosVisible = false;
                                DatosIngresoVisible = true;
                                AmparoDirectoTab = true;
                                RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;

                                SetValidacionesAmparoDirecto();
                                LimpiarAmparoDirecto();
                                StaticSourcesViewModel.SourceChanged = false;
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia.");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal");
                        break;

                    case "editAmparoDirecto":
                        if (SelectedAmparoDirecto != null)
                        {
                            if (!PEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            TabAmparoDirectoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoDirectoTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoIndirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;

                            SetValidacionesAmparoDirecto();
                            LimpiarAmparoDirecto();
                            ObtenerAmparoDirecto();
                            StaticSourcesViewModel.SourceChanged = false;

                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un amparo indirecto.");
                        break;

                    case "delAmparoDirecto":
                        EliminarAmparoDirecto();
                        break;

                    case "digitalizar_amparo_directo":
                        ObtenerTipoDocumento(2);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;

                    //AMPARO INDIRECTO
                    case "addAmparoIndirecto":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        EnCausaPenal = false;
                        SelectedAmparoIndirecto = null;
                        TabAmparoIndirectoSelected = true;
                        IngresosVisible = false;
                        DatosIngresoVisible = true;
                        AmparoIndirectoTab = true;
                        RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;
                        ObtenerTipoAmparoIndirecto("N");
                        SetValidacionesAmparoIndirecto();
                        LimpiarAmparoIndirecto();
                        StaticSourcesViewModel.SourceChanged = false;

                        break;

                    case "editAmparoIndirecto":
                        if (SelectedAmparoIndirecto != null)
                        {
                            if (!PEditar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }
                            EnCausaPenal = false;
                            TabAmparoIndirectoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoIndirectoTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;

                            ObtenerTipoAmparoIndirecto("N");
                            SetValidacionesAmparoIndirecto();
                            LimpiarAmparoIndirecto();
                            if (SelectedAmparoIndirecto != null)
                                ObtenerAmparoIndirecto();
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un amparo indirecto.");
                        break;

                    case "delAmparoIndirecto":
                        EliminarAmparoIndirecto();
                        break;
                    //AMPARO INDIRECTO EN CAUSA PENAL
                    case "addAmparoIndirectoCP":
                        EnCausaPenal = true;
                        //LimpiarAmparoIndirecto();
                        SelectedAmparoIndirecto = null;
                        TabAmparoIndirectoSelected = true;
                        IngresosVisible = false;
                        DatosIngresoVisible = true;
                        AmparoIndirectoTab = true;
                        RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;

                        SetValidacionesAmparoIndirecto();
                        ObtenerTipoAmparoIndirecto("S");
                        LimpiarAmparoIndirecto();
                        StaticSourcesViewModel.SourceChanged = false;
                        break;

                    case "editAmparoIndirectoCP":
                        if (SelectedAmparoIndirecto != null)
                        {
                            EnCausaPenal = true;
                            TabAmparoIndirectoSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoIndirectoTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = false;

                            SetValidacionesAmparoIndirecto();
                            ObtenerTipoAmparoIndirecto("S");
                            LimpiarAmparoIndirecto();
                            if (SelectedAmparoIndirecto != null)
                                ObtenerAmparoIndirecto();
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un amparo indirecto.");
                        break;

                    case "delAmparoIndirectoCP":
                        EliminarAmparoIndirecto();
                        break;

                    case "agregar_tipo_amparo":
                        AgregarAmparoIndirectoTipo();
                        break;

                    case "digitalizar_amparo_indirecto":
                        ObtenerTipoDocumento(1);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;
                    //INCIDENTES
                    case "addAmparoIncidente":
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        SelectedAmparoIncidente = null;
                        TabAmparoIncidenteSelected = true;
                        IngresosVisible = false;
                        DatosIngresoVisible = true;
                        AmparoIncidenteTab = true;
                        RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = false;

                        SetValidacionesIncidentes();
                        LimpiarIncidente();
                        StaticSourcesViewModel.SourceChanged = false;

                        break;

                    case "editAmparoIncidente":
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }
                        if (SelectedAmparoIncidente != null)
                        {
                            TabAmparoIncidenteSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            AmparoIncidenteTab = true;
                            RecursosTab = CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = IngresoTab = DelitoTab = RecursoTab = DelitosTab = AmparoDirectoTab = AmparoIndirectoListaTab = AmparoDirectoListaTab = AmparoIncidenteListaTab = false;

                            SetValidacionesIncidentes();
                            LimpiarIncidente();
                            ObtenerIncidente();
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un incidente.");
                        break;

                    case "delAmparoIncidente":
                        EliminarIncidente();
                        break;
                    case "digitalizar_amparo_incidente":
                        ObtenerTipoDocumento(3);
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;

                    case "limpiar_menu":
                        //if (StaticSourcesViewModel.SourceChanged)
                        //{
                        //    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Segúro que desea salir sin guardar?");
                        //    if (respuesta == 1)
                        //    {
                        //        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CausaPenalView();
                        //        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CausaPenalViewModel();
                        //    }
                        //}
                        //else
                        //{
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CausaPenalView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.CausaPenalViewModel();
                        //}
                        break;
                    case "aceptar_empalme":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EMPALME_FECHAS);
                        break;
                    case "guardar_documento":
                        GuardarDocumento();
                        break;
                    case "Cancelar_digitalizar_documentos":
                        escaner.Hide();
                        DocumentoDigitalizado = null;
                        ObservacionDocumento = string.Empty;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DIGITALIZAR_DOCUMENTO);
                        break;
                    case "reporte_menu":
                        ImprimirPartidaJuridica2();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }
        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
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

                    }
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

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

                //anio = 2015;
                //folio = 7;

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar);
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        #region Comentado
                        //foreach (var item in EstatusInactivos)
                        //{
                        //    if (ListExpediente[0].INGRESO.OrderByDescending(o=>o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                        //    {
                        //        SelectExpediente = null;
                        //        SelectIngreso = null;
                        //        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //        TabVisible = false;
                        //        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                        //        return;
                        //    }
                        //}
                        //if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        //{
                        //    SelectExpediente = null;
                        //    SelectIngreso = null;
                        //    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        //    TabVisible = false;
                        //    new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        //    return;
                        //}
                        #endregion
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        //buscamos en los ingresos que pertenecen al centro
                        short[] Estatus = { 1, 2, 3, 8 };
                        var traslado = ListExpediente[0].INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro && Estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value)).OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        if (traslado != null)
                        {
                            if (traslado.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                    ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                                return;
                            }
                        }
                        
                        SelectExpediente = ListExpediente[0];
                        LstIngresosCentro = new ObservableCollection<INGRESO>(SelectExpediente.INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro).OrderByDescending(o => o.ID_INGRESO));
                        SelectIngreso = LstIngresosCentro.FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        SelectedIngreso = SelectIngreso;//ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        TabVisible = true;
                        this.SeleccionaIngreso();
                        this.ViewModelArbol();
                        EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);


                        if (!Estatus.Contains(SelectedIngreso.ID_ESTATUS_ADMINISTRATIVO.Value))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no se encuentra activo la información sera solo de consulta");
                            MenuGuardarEnabled = false;
                        }
                        else
                            MenuGuardarEnabled = true;
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                        TabVisible = false;
                    }
                }
                else
                {
                    //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar,FolioBuscar);
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO_CP);
                    TabVisible = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }
        private async void ClickEnter(Object obj)
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
                }

                //PopUpsViewModels.ShowPopUp(this,PopUpsViewModels.TipoPopUp.BUSQUEDA_CAUSAS_PENALES);
                TabVisible = false;
                //ListExpediente = new ObservableCollection<IMPUTADO>();
                //int anio = 0;
                //AnioBuscar = AnioD.ToString();
                //if (!string.IsNullOrEmpty(AnioBuscar))
                //    anio = int.Parse(AnioBuscar);
                //int folio = 0;
                //FolioBuscar = FolioD.ToString();
                //if (!string.IsNullOrEmpty(FolioBuscar))
                //    folio = int.Parse(FolioBuscar);
                //if (string.IsNullOrEmpty(NombreBuscar))
                //    NombreBuscar = string.Empty;
                //if (string.IsNullOrEmpty(ApellidoPaternoBuscar))
                //    ApellidoPaternoBuscar = string.Empty;
                //if (string.IsNullOrEmpty(ApellidoMaternoBuscar))
                //    ApellidoMaternoBuscar = string.Empty;
                //anio = 2015;
                //folio = 7;
                //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar);
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }
        
        private void ClickTreeView(Object obj)
        {
            try
            {
                DatosIngresoVisible = false;
                IngresosVisible = true;

                //var x = (TreeView)obj;
                //var i = x.SelectedItem.ToString();
                //i = i.Replace("System.Windows.Controls.TreeViewItem Header:", string.Empty);

                //if (i.StartsWith("Ingresos"))
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = true;
                //    DatosIngresoVisible = false;
                //    IngresosVisible = true;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = false;
                //    DelitosTab = false;
                //    DelitoTab = false;
                //    RecursoTab = false;
                //}
                //else if(i.StartsWith("Ingreso:"))
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = true;
                //    DatosIngresoVisible = false;
                //    IngresosVisible = true;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = false;
                //    DelitosTab = false;
                //    DelitoTab = false;
                //    RecursoTab = false;
                //}
                //else if(i.StartsWith("2013"))
                //{
                //    //GeneralVisible = false;
                //    //BusquedaVisible = false;
                //    //TabVisible = true;
                //    //DiscrecionVisible = false;
                //    //DatosIngresoVisible = true;
                //    //IngresosVisible = false;
                //    //AgregarCausaPenalVisible = false;
                //    BusquedaVisible = false;
                //    GeneralVisible = true;
                //    TabVisible = false;
                //    DatosIngresoVisible = true;
                //    IngresosVisible = false;
                //    DatosCausaPenalVisible = false;
                //    AgregarCausaPenalVisible = false;
                //    DiscrecionalVisible = false;


                //    IngresoTab = false;
                //    CausaPenalTab = true;
                //    CoparticipeTab = true;
                //    SentenciaTab = true;
                //    RdiTab = false;
                //    DelitosTab = false;
                //    DelitoTab = false;
                //    RecursoTab = false;

                //    CausaPenal2Tab = true;
                //}
                //else if (i.StartsWith("Delito Causa")) 
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = false;
                //    DatosIngresoVisible = true;
                //    IngresosVisible = false;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = false;
                //    DelitosTab = true;
                //    DelitoTab = false;
                //    RecursoTab = false;

                //    CausaPenal2Tab = false;
                //    Delitos2Tab = true;

                //}
                //else if (i.StartsWith("Delitos")) 
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = false;
                //    DatosIngresoVisible = true;
                //    IngresosVisible = false;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = false;
                //    DelitosTab = true;
                //    DelitoTab = false;
                //    RecursoTab = false;

                //    CausaPenal2Tab = false;
                //    Delitos2Tab = true;
                //}
                //else if (i.StartsWith("Partida"))
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = false;
                //    DatosIngresoVisible = true;
                //    IngresosVisible = false;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = true;
                //    DelitosTab = false;
                //    DelitoTab = false;
                //    RecursoTab = false;

                //    CausaPenal2Tab = false;
                //    Delitos2Tab = false;
                //    Rdi2Tab = true;
                //}
                //else 
                //{
                //    GeneralVisible = false;
                //    BusquedaVisible = false;
                //    TabVisible = false;
                //    DiscrecionVisible = false;
                //    DatosIngresoVisible = true;
                //    IngresosVisible = false;
                //    AgregarCausaPenalVisible = false;

                //    IngresoTab = false;
                //    CausaPenalTab = false;
                //    CoparticipeTab = false;
                //    SentenciaTab = false;
                //    RdiTab = false;
                //    DelitosTab = false;
                //    DelitoTab = false;
                //    RecursoTab = true;

                //    CausaPenal2Tab = false;
                //    Delitos2Tab = false;
                //    Rdi2Tab = false;
                //    Recurso2Tab = true;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar en árbol", ex);
            }
        }
        private async void GetImputados()
        {
            try
            {
                //ListExpediente = new ObservableCollection<IMPUTADO>();
                //ListExpediente = (new cImputado()).ObtenerTodos(PaternoD, MaternoD, NombreD, AnioD.Value, FolioD.Value);
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener imputados", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodosCentro(GlobalVar.gCentro, estatus_inactivos, ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void InicializaVariables()
        {
            try
            {
                PaternoD = MaternoD = NombreD = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al inicializar variables", ex);
            }
        }
        private void SetIngreso()
        {
            try
            {
                //INGRESO
                // TiposIngreso = new ObservableCollection<TIPO_INGRESO>((new cTipoIngreso()).ObtenerTodos());
                // TiposIngreso.Insert(0, new TIPO_INGRESO() { DESCR = "SELECCIONE", ID_TIPO_INGRESO = -1 });
                TipoI = -1;
                //EstatusAdministrativo = new ObservableCollection<ESTATUS_ADMINISTRATIVO>((new cEstatusAdministrativo()).ObtenerTodos());
                //EstatusAdministrativo.Insert(0, new ESTATUS_ADMINISTRATIVO() { DESCR = "SELECCIONE", ID_ESTATUS_ADMINISTRATIVO = -1 });
                EstatusAdministrativoI = -1;
                //Clasificaciones = new ObservableCollection<CLASIFICACION_JURIDICA>(new cClasificacionJuridica().ObtenerTodos());
                //Clasificaciones.Insert(0, new CLASIFICACION_JURIDICA() { DESCR = "SELECCIONE", ID_CLASIFICACION_JURIDICA = "" });
                ClasificacionI = string.Empty;
                //AutoridadesInterna = new ObservableCollection<TIPO_AUTORIDAD_INTERNA>((new cTipoAutoridadInterna()).ObtenerTodos());
                //AutoridadesInterna.Insert(0, new TIPO_AUTORIDAD_INTERNA() { DESCR = "SELECCIONE", ID_AUTORIDAD_INTERNA = -1 });
                AutoridadInternaI = -1;
                //TiposSeguridad = new ObservableCollection<TIPO_SEGURIDAD>((new cTipoSeguridad()).ObtenerTodos());
                //TiposSeguridad.Insert(0, new TIPO_SEGURIDAD() { DESCR = "SELECCIONE", ID_TIPO_SEGURIDAD = "" });
                TipoSeguridadI = string.Empty;
                //AutoridadDisposicion = new ObservableCollection<TIPO_DISPOSICION>((new cTipoDisposicion()).ObtenerTodos());
                //AutoridadDisposicion.Insert(0, new TIPO_DISPOSICION() { DESCR = "SELECCIONE", ID_DISPOSICION = -1 });
                QuedaDisposicionI = -1;
                //Tipos_delito = (new cTipoDelito()).ObtenerTodos();
                //Tipos_delito.Insert(0, new TIPO_DELITO { ID_TIPO_DELITO = -1, DESCR = "SELECCIONE" });
                TipoD = -1;
                //DELITO INGRESO
                //IngresoDelitos = new ObservableCollection<INGRESO_DELITO>((new cIngresoDelito()).ObtenerTodos());
                //IngresoDelitos.Insert(0, new INGRESO_DELITO { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE", ID_FUERO = string.Empty });
                IngresoDelito = -1;
                //CAUSA PENAL
                // Paises = new ObservableCollection<PAIS_NACIONALIDAD>((new cPaises()).ObtenerTodos());
                PaisJuzgadoCP = Parametro.PAIS; 
                //Edificios = new ObservableCollection<EDIFICIO>((new cEdificio()).ObtenerTodos(string.Empty, 0, 4));
                //Edificios.Insert(0, new EDIFICIO { ID_EDIFICIO = -1, DESCR = "SELECCIONE" });
                EdificioI = -1;
                //TiposOrden = (new cTipoOrden()).ObtenerTodos();
                //TiposOrden.Insert(0, new TIPO_ORDEN { CP_TIPO_ORDEN = -1, DESCR = "SELECCIONE" });
                TipoOrdenCP = -1;
                //Terminos = (new cTermino()).ObtenerTodos();
                //Terminos.Insert(0, new TERMINO { ID_TERMINO = -1, DESCR = "SELECCIONE" });
                TerminoCP = -1;
                //Fueros = (new cFuero()).ObtenerTodos();
                //Fueros.Insert(0, new FUERO { ID_FUERO = string.Empty, DESCR = "SELECCIONE" });
                FueroCP = string.Empty;
                //Agencias = (new cAgencia()).ObtenerTodos();
                //Agencias.Insert(0, new AGENCIA { ID_AGENCIA = -1, DESCR = "SELECCIONE" });
                AgenciaAP = -1;

                //Causas_penal_estatus = (new cCausaPenalEstatus()).ObtenerTodos();
                //Causas_penal_estatus.Insert(0, new CAUSA_PENAL_ESTATUS { ID_ESTATUS_CP = -1, DESCR = "SELECCIONE" });
                EstatusCP = -1;
                //GRADO AUTORIA
                //GradosAutoria = (new cGradoAutoria()).ObtenerTodos();
                GradoAutoriaS = -1;
                //GRADO PARTICIPACION
                //GradosParticipacion = (new cGradoParticipacion()).ObtenerTodos();
                GradoParticipacionS = -1;
                //TIPO_RECURSOS
                //TiposRecursos = new ObservableCollection<TIPO_RECURSO>((new cTipoRecurso()).ObtenerTodos());
                //TiposRecursos.Insert(0, new TIPO_RECURSO { ID_TIPO_RECURSO = -1, DESCR = "SELECCIONE" });
                TipoR = -1;
                //RECURSOS
                //Tribunales = (new cJuzgado()).ObtenerTodos(true);
                //Tribunales.Insert(0, new JUZGADO { });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso", ex);
            }
        }
       
        private void SeleccionaIngreso()
        {
            try
            {
                #region Datos generales
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                IngresosD = SelectIngreso.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (SelectIngreso.CAMA != null)
                {
                    UbicacionD = UbicacionI = string.Format("{0}-{1}{2}-{3}",
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty,
                                               SelectIngreso.ID_UB_CAMA);
                }
                else
                {
                    UbicacionD = UbicacionI = string.Empty;
                }

                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value : new DateTime?();
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                #endregion

                #region Datos ingreso
                FecRegistroI = SelectIngreso.FEC_REGISTRO.HasValue ? SelectIngreso.FEC_REGISTRO.Value : new DateTime?();
                FecCeresoI = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value : new DateTime?();
                TipoI = SelectIngreso.ID_TIPO_INGRESO.HasValue ? SelectIngreso.ID_TIPO_INGRESO.Value : new short();
                EstatusAdministrativoI = SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.HasValue ? SelectIngreso.ID_ESTATUS_ADMINISTRATIVO.Value : new short();
                ClasificacionI = SelectIngreso.ID_CLASIFICACION_JURIDICA;
                NoOficioI = SelectIngreso.DOCINTERNACION_NUM_OFICIO;
                AutoridadInternaI = SelectIngreso.ID_AUTORIDAD_INTERNA.HasValue ? SelectIngreso.ID_AUTORIDAD_INTERNA.Value : new short();
                TipoSeguridadI = SelectIngreso.ID_TIPO_SEGURIDAD;
                QuedaDisposicionI = SelectIngreso.ID_DISPOSICION.HasValue ? SelectIngreso.ID_DISPOSICION.Value : new short();
                #endregion

                #region Delitos
                if (SelectIngreso.ID_INGRESO_DELITO != null)
                {
                    if (SelectIngreso.ID_INGRESO_DELITO == 0)
                        IngresoDelito = -1;
                    else
                        IngresoDelito = SelectIngreso.ID_INGRESO_DELITO.Value;
                }
                #endregion

                #region Expediente gobierno
                FolioGobiernoI = SelectIngreso.FOLIO_GOBIERNO;
                AnioGobiernoI = SelectIngreso.ANIO_GOBIERNO;
                #endregion

                #region Sentencia
                TotalAnios = 0;
                TotalMeses = 0;
                TotalDias = 0;
                int a, m, d;
                a = m = d = 0;
                SENTENCIA sen;
                //LstSentenciasIngresos = new List<SentenciaIngreso>();
                LstSentenciasIngresos = new ObservableCollection<SentenciaIngreso>();
                foreach (var cp in SelectIngreso.CAUSA_PENAL)
                {
                    sen = cp.SENTENCIA.FirstOrDefault();
                    if (sen != null)
                    {
                        LstSentenciasIngresos.Add(new SentenciaIngreso
                        {
                            CausaPenal = string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO),
                            SentenciaAnios = sen.ANIOS,
                            SentenciaMeses = sen.MESES,
                            SentenciaDias = sen.DIAS,
                            Fuero = cp.CP_FUERO
                        });
                        a = sen.ANIOS != null ? sen.ANIOS.Value : 0;
                        m = sen.MESES != null ? sen.MESES.Value : 0;
                        d = sen.DIAS != null ? sen.DIAS.Value : 0;
                    }
                    else
                    {
                        LstSentenciasIngresos.Add(new SentenciaIngreso
                        {
                            CausaPenal = string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO),
                            SentenciaAnios = 0,
                            SentenciaMeses = 0,
                            SentenciaDias = 0,
                            Fuero = cp.CP_FUERO
                        });
                    }
                }

                while (d > 29)
                {
                    m++;
                    d = d - 30;
                }
                while (m > 11)
                {
                    a++;
                    m = m - 12;
                }
                TotalAnios = a;
                TotalMeses = m;
                TotalDias = d;
                #endregion
                //PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA_CAUSAS_PENALES);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }
        private void GetJuzgados()
        {
            try
            {
                if (PaisJuzgadoCP != null && EstadoJuzgadoCP != null && MunicipioJuzgadoCP != null && !string.IsNullOrEmpty(FueroCP))
                    LstJuzgados = new ObservableCollection<JUZGADO>((new cJuzgado()).Obtener(PaisJuzgadoCP, EstadoJuzgadoCP, MunicipioJuzgadoCP, FueroCP));
                else
                    LstJuzgados = new ObservableCollection<JUZGADO>();
                LstJuzgados.Insert(0, new JUZGADO() { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                JuzgadoCP = -1;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener juzgado", ex);
            }
        }
        private void SeleccionaIngreso(Object obj)
        {
            try
            {
                var tipo = obj.GetType();
                if (tipo.BaseType.Name.Equals("CAUSA_PENAL"))
                {
                    if (SelectedCausaPenal != null)
                    {
                        SetValidacionesCausaPenal();
                        this.SetCausaPenal();
                        TabCausaPenalSelected = true;
                        IngresosVisible = false;
                        DatosIngresoVisible = true;
                        //TABS
                        CausaPenalTab = CoparticipeTab = SentenciaTab = true;
                        IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = false;
                    }
                }
                else if (tipo.BaseType.Name.Equals("RECURSO"))
                {
                    SelectedCausaPenal = SelectedRecurso.CAUSA_PENAL;
                    SelectedIngreso = SelectedCausaPenal.INGRESO;
                    //this.SetRecurso();
                    TabRecursoSelected = true;
                    IngresosVisible = false;
                    DatosIngresoVisible = true;
                    //TABS
                    RecursoTab = true;
                    CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursosTab = false;

                    //PopulateRecurso();
                    SetValidacionesRecurso();
                    LimpiarRecurso();
                    ObtenerRecurso();
                }
                else if (tipo.BaseType.Name.Equals("RECURSOs"))
                {
                    SetValidacionesRecurso();
                }
                else
                {
                    if (tipo.Name.Equals("CausaPenalIngreso"))
                    {
                        var cp = (CausaPenalIngreso)obj;
                        if (cp.CausaPenal != null)
                        {
                            SetValidacionesIngresoCausaPenal();
                            //TABS
                            IngresoTab = true;
                            CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = false;
                            /***********************************************************************************************************/
                            TabCausaPenalSelected = DatosIngresoVisible = true;
                            IngresosVisible = false;
                            //TABS
                            CausaPenalTab = CoparticipeTab = SentenciaTab = true;
                            IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = RecursosTab = false;
                            SelectedCausaPenal = cp.CausaPenal;
                            PopulateCausaPenal();
                            StaticSourcesViewModel.SourceChanged = false;
                        }

                        ////TABS
                        //IngresoTab = true;
                        //CausaPenalTab = CoparticipeTab = SentenciaTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = false;
                        ///***********************************************************************************************************/

                        //TabCausaPenalSelected = DatosIngresoVisible = true;
                        //IngresosVisible = false;
                        ////TABS
                        //CausaPenalTab = CoparticipeTab = SentenciaTab = true;
                        //IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = RecursosTab = false;
                        //SelectIngreso = cp.CausaPenal.INGRESO;
                        //SelectedCausaPenal = cp.CausaPenal;
                        //if (SelectedCausaPenal != null)
                        //{
                        //    //SelectedIngreso = SelectedCausaPenal.INGRESO;
                        //    //this.SetCausaPenal();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        #region [INGRESOS]
        private bool EditarIngreso()
        {
            try
            {
                var ingreso = new INGRESO();
                ingreso.ID_CENTRO = SelectedIngreso.ID_CENTRO;
                ingreso.ID_ANIO = SelectedIngreso.ID_ANIO;
                ingreso.ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO;
                ingreso.ID_INGRESO = SelectedIngreso.ID_INGRESO;
                ingreso.FEC_REGISTRO = FecRegistroI;
                ingreso.FEC_INGRESO_CERESO = FecCeresoI;
                ingreso.ID_TIPO_INGRESO = TipoI;
                ingreso.ID_CLASIFICACION_JURIDICA = ClasificacionI;
                ingreso.ID_ESTATUS_ADMINISTRATIVO = EstatusAdministrativoI;
                ingreso.DOCINTERNACION_NUM_OFICIO = NoOficioI;
                ingreso.ID_AUTORIDAD_INTERNA = AutoridadInternaI;
                ingreso.ID_TIPO_SEGURIDAD = TipoSeguridadI;
                ingreso.ID_DISPOSICION = QuedaDisposicionI;
                if (SelectedUbicacion != null)
                {
                    ingreso.ID_UB_CENTRO = SelectedUbicacion.ID_CENTRO;
                    ingreso.ID_UB_EDIFICIO = SelectedUbicacion.ID_EDIFICIO;
                    ingreso.ID_UB_SECTOR = SelectedUbicacion.ID_SECTOR;
                    ingreso.ID_UB_CELDA = SelectedUbicacion.ID_CELDA;
                    ingreso.ID_UB_CAMA = SelectedUbicacion.ID_CAMA;
                }
                else
                {
                    ingreso.ID_UB_CENTRO = SelectedIngreso.ID_UB_CENTRO;
                    ingreso.ID_UB_EDIFICIO = SelectedIngreso.ID_UB_EDIFICIO;
                    ingreso.ID_UB_SECTOR = SelectedIngreso.ID_UB_SECTOR;
                    ingreso.ID_UB_CELDA = SelectedIngreso.ID_UB_CELDA;
                    ingreso.ID_UB_CAMA = SelectedIngreso.ID_UB_CAMA;
                }
                ingreso.ANIO_GOBIERNO = AnioGobiernoI;
                ingreso.FOLIO_GOBIERNO = FolioGobiernoI;
                //DELITO
                ingreso.ID_INGRESO_DELITO = IngresoDelito;

                if ((new cIngreso()).Actualizar(ingreso))
                {
                    new Dialogos().ConfirmacionDialogo("ÉXITO!", "Información Grabada Exitosamente!");
                    return true;
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("ERROR!", "Error al Guardar!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al editar ingreso", ex);
                return false;
            }
        }
        #endregion

        #region CAUSA PENAL
        private void SetCausaPenal()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    //AVERIGUACION PREVIA
                    //if (SelectedCausaPenal.AGENCIA != null)
                    //{
                    //    SelectedAgencia = Agencias.Where(w => w.ID_AGENCIA == SelectedCausaPenal.AGENCIA.ID_AGENCIA && w.ID_ENTIDAD == SelectedCausaPenal.AGENCIA.ID_ENTIDAD && w.ID_MUNICIPIO == SelectedCausaPenal.AGENCIA.ID_MUNICIPIO).FirstOrDefault();
                    //}
                    AnioAP = SelectedCausaPenal.AP_ANIO;
                    FolioAP = SelectedCausaPenal.AP_FOLIO;
                    AveriguacionPreviaAP = SelectedCausaPenal.AP_FORANEA;
                    FecAveriguacionAP = SelectedCausaPenal.AP_FEC_INICIO;
                    FecConsignacionAP = SelectedCausaPenal.AP_FEC_CONSIGNACION;
                    //CAUSA PENAL
                    AnioCP = SelectedCausaPenal.CP_ANIO;
                    FolioCP = SelectedCausaPenal.CP_FOLIO;
                    BisCP = SelectedCausaPenal.CP_BIS;
                    ForaneoCP = SelectedCausaPenal.CP_FORANEO;
                    TipoOrdenCP = SelectedCausaPenal.CP_TIPO_ORDEN != null ? SelectedCausaPenal.CP_TIPO_ORDEN : -1;
                    PaisJuzgadoCP = SelectedCausaPenal.CP_PAIS_JUZGADO != null ? SelectedCausaPenal.CP_PAIS_JUZGADO : -1;
                    EstadoJuzgadoCP = SelectedCausaPenal.CP_ESTADO_JUZGADO != null ? SelectedCausaPenal.CP_ESTADO_JUZGADO : -1;
                    MunicipioJuzgadoCP = SelectedCausaPenal.CP_MUNICIPIO_JUZGADO != null ? SelectedCausaPenal.CP_MUNICIPIO_JUZGADO : -1;
                    FueroCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_FUERO) ? SelectedCausaPenal.CP_FUERO : string.Empty;
                    JuzgadoCP = SelectedCausaPenal.CP_JUZGADO != null ? SelectedCausaPenal.CP_JUZGADO : -1;
                    FecRadicacionCP = SelectedCausaPenal.CP_FEC_RADICACION;
                    AmpliacionCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_AMPLIACION) ? SelectedCausaPenal.CP_AMPLIACION : string.Empty;
                    TerminoCP = SelectedCausaPenal.CP_TERMINO != null ? SelectedCausaPenal.CP_TERMINO : -1;
                    EstatusCP = SelectedCausaPenal.ID_ESTATUS_CP != null ? SelectedCausaPenal.ID_ESTATUS_CP : -1;
                    FecVencimientoTerinoCP = SelectedCausaPenal.CP_FEC_VENCIMIENTO_TERMINO;
                    //DELITO
                    if (SelectedCausaPenal.CAUSA_PENAL_DELITO != null)
                        LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(SelectedCausaPenal.CAUSA_PENAL_DELITO);
                    else
                        LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>();
                    if (LstCausaPenalDelitos != null)
                    {
                        if (LstCausaPenalDelitos.Count > 0)
                            CausaPenalDelitoEmpty = false;
                        else
                            CausaPenalDelitoEmpty = true;
                    }
                    else
                    {
                        CausaPenalDelitoEmpty = true;
                    }
                    //COPARTICIPE
                    LstCoparticipe = new ObservableCollection<COPARTICIPE>(SelectedCausaPenal.COPARTICIPE);


                    //SENTENCIA
                    if (SelectedCausaPenal.SENTENCIA != null)
                    {
                        if (SelectedCausaPenal.SENTENCIA.Count > 0)
                        {
                            var s = SelectedCausaPenal.SENTENCIA.FirstOrDefault();
                            FecS = s.FEC_SENTENCIA.Value;
                            FecEjecutoriaS = s.FEC_EJECUTORIA.Value;
                            FecInicioCompurgacionS = s.FEC_INICIO_COMPURGACION;
                            AniosS = s.ANIOS.Value;
                            MesesS = s.MESES.Value;
                            DiasS = s.DIAS.Value;
                            MultaS = s.MULTA;
                            //s.MULTA_PAGADA = "N";
                            if (s.MULTA_PAGADA.Equals("S"))
                                MultaSi = true;
                            else
                                MultaNo = true;
                            ReparacionDanioS = s.REPARACION_DANIO;
                            if (s.REPARACION_DANIO_PAGADA.Equals("S"))
                                ReparacionSi = true;
                            else
                                ReparacionNo = true;
                            SustitucionPenaS = s.SUSTITUCION_PENA;
                            if (s.SUSTITUCION_PENA_PAGADA.Equals("S"))
                                SustitucionSi = true;
                            else
                                SustitucionNo = true;
                            SuspensionCondicionalS = s.SUSPENSION_CONDICIONAL;
                            ObservacionS = s.OBSERVACION;
                            MotivoCancelacionAntecedenteS = s.MOTIVO_CANCELACION_ANTECEDENTE;
                            GradoAutoriaS = s.ID_GRADO_AUTORIA.Value;
                            GradoParticipacionS = s.ID_GRADO_PARTICIPACION.Value;
                            AniosAbonadosS = s.ANIOS_ABONADOS.Value;
                            MesesAbonadosS = s.MESES_ABONADOS.Value;
                            DiasAbonadosS = s.DIAS_ABONADOS.Value;
                            FecRealCompurgacionS = s.FEC_REAL_COMPURGACION.Value;
                            //SENTENCIA DELITOS
                            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>(s.SENTENCIA_DELITO);
                            if (LstSentenciaDelitos.Count > 0)
                                SentenciaDelitoEmpty = false;
                            else
                                SentenciaDelitoEmpty = true;
                        }
                        else
                        {
                            if (LstCausaPenalDelitos != null)
                            {
                                //COPIAMOS LOS DELITOS A LA SENTENCIA
                                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                                foreach (var delito in LstCausaPenalDelitos)
                                {
                                    LstSentenciaDelitos.Add(new SENTENCIA_DELITO
                                    {
                                        ID_DELITO = delito.ID_DELITO,
                                        ID_FUERO = delito.ID_FUERO,
                                        ID_MODALIDAD = delito.ID_MODALIDAD,
                                        ID_TIPO_DELITO = delito.ID_TIPO_DELITO,
                                        CANTIDAD = delito.CANTIDAD,
                                        OBJETO = delito.OBJETO,
                                        MODALIDAD_DELITO = delito.MODALIDAD_DELITO,
                                        TIPO_DELITO = delito.TIPO_DELITO,
                                        DESCR_DELITO = delito.DESCR_DELITO
                                    });
                                }
                                if (LstSentenciaDelitos.Count > 0)
                                    SentenciaDelitoEmpty = false;
                                else
                                    SentenciaDelitoEmpty = true;

                            }

                            AniosS = MesesS = DiasS = AniosAbonadosS = MesesAbonadosS = DiasAbonadosS = 0;
                        }
                    }
                    else
                    {
                        if (LstCausaPenalDelitos != null)
                        {
                            //COPIAMOS LOS DELITOS A LA SENTENCIA
                            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                            foreach (var delito in LstCausaPenalDelitos)
                            {
                                LstSentenciaDelitos.Add(new SENTENCIA_DELITO
                                {
                                    ID_DELITO = delito.ID_DELITO,
                                    ID_FUERO = delito.ID_FUERO,
                                    ID_MODALIDAD = delito.ID_MODALIDAD,
                                    ID_TIPO_DELITO = delito.ID_TIPO_DELITO,
                                    CANTIDAD = delito.CANTIDAD,
                                    OBJETO = delito.OBJETO,
                                    MODALIDAD_DELITO = delito.MODALIDAD_DELITO,
                                    TIPO_DELITO = delito.TIPO_DELITO,
                                    DESCR_DELITO = delito.DESCR_DELITO
                                });
                            }
                            if (LstSentenciaDelitos.Count > 0)
                                SentenciaDelitoEmpty = false;
                            else
                                SentenciaDelitoEmpty = true;
                        }
                        AniosS = MesesS = DiasS = AniosAbonadosS = MesesAbonadosS = DiasAbonadosS = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer causa penal", ex);
            }
        }
        private void SeleccionaDelito(Object obj)
        {
            try
            {
                var arbol = (TreeView)obj;
                if (arbol.SelectedItem != null)
                {
                    var x = arbol.SelectedItem;
                    var t = x.GetType();
                    if (t.BaseType.Name.ToString().Equals("MODALIDAD_DELITO"))
                    {
                        SelectedDelitoArbol = (MODALIDAD_DELITO)x;
                        if (SelectedDelitoArbol != null)
                        {
                            if (SelectedDelitoArbol.DELITO != null)
                            {
                                DelitoD = SelectedDelitoArbol.DELITO.DESCR;
                                GraveD = string.IsNullOrEmpty(SelectedDelitoArbol.DELITO.GRAVE) ? "NO" : SelectedDelitoArbol.DELITO.GRAVE.Equals("S") ? "SI" : "NO";
                            }
                            ModalidadD = SelectedDelitoArbol.DESCR;
                        }
                        DelitoExpander = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar delito", ex);
            }
        }
        private void MenuClick(Object parametro)
        {
            try
            {
                if (!InternoActivo())
                    return;
                switch (parametro.ToString())
                {
                    case "agregar_causa_penal":
                        if (SelectedIngreso != null)
                        {
                            if (!PInsertar)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                                return;
                            }

                            SetValidacionesCausaPenal();
                            SelectedCausaPenal = null;
                            TabCausaPenalSelected = true;
                            IngresosVisible = false;
                            DatosIngresoVisible = true;
                            //TABS
                            CausaPenalTab = CoparticipeTab = SentenciaTab = true;
                            IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursoTab = false;
                            LimpiarCausaPenal();
                            //ESTATUS CAUSA PENAL EN PROCESO POR DEFAULT
                            EstatusCP = 6;
                            StaticSourcesViewModel.SourceChanged = false;
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso.");
                        break;

                    case "eliminar_causa_penal":
                        if (SelectedCausaPenal != null)
                        {
                            if (ValidaEliminarCausaPenal())
                            {
                                if (new cCausaPenal().Eliminar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL))
                                    new Dialogos().ConfirmacionDialogo("Notificación", "Se ha eliminado la causa penal exitosamente");
                                else
                                    new Dialogos().ConfirmacionDialogo("Notificación", "Ocurrió un error al tratar de eliminar la causa penal");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Notificación", "No se puede eliminar la causa penal porque tiene información relacionada");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Notificación", "Favor de selaccionar una causa penal");
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo de modulo", ex);
            }
        }
        private bool AgregarCausaPenal()
        {
            //var causaPenal = new CAUSA_PENAL();
            //if (TabCausaPenalSelected)
            //{
            //    if (GuardarCausaPenal(causaPenal))
            //        return true;
            //    return false;
            //}
            //else if (TabCoparticipeSelected)
            //{
            //    if (GuardarCoparticipes())
            //        return true;
            //    return false;
            //}
            //else if (TabSentenciaSelected)
            //{
            //    if (GuardarSentencia())
            //        return true;
            //    return false;
            //}
            return false;
        }
        private bool GuardarCausaPenal(CAUSA_PENAL causaPenal)
        {
            try
            {
                if (SelectedCausaPenal != null)
                    causaPenal.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                else
                    causaPenal.ID_CAUSA_PENAL = 0;
                causaPenal.ID_CENTRO = SelectedIngreso.ID_CENTRO;
                causaPenal.ID_ANIO = SelectedIngreso.ID_ANIO;
                causaPenal.ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO;
                causaPenal.ID_INGRESO = SelectedIngreso.ID_INGRESO;
                //AVERIGUACION PREVIA
                if (SelectedAgencia.ID_AGENCIA != -1)
                    causaPenal.ID_AGENCIA = SelectedAgencia.ID_AGENCIA;
                if (SelectedAgencia.ID_ENTIDAD != -1)
                    causaPenal.ID_ENTIDAD = SelectedAgencia.ID_ENTIDAD;
                if (SelectedAgencia.ID_MUNICIPIO != -1)
                    causaPenal.ID_MUNICIPIO = SelectedAgencia.ID_MUNICIPIO;
                causaPenal.AP_ANIO = AnioAP;
                causaPenal.AP_FOLIO = FolioAP;
                causaPenal.AP_FORANEA = AveriguacionPreviaAP;
                causaPenal.AP_FEC_INICIO = FecAveriguacionAP;
                causaPenal.AP_FEC_CONSIGNACION = FecConsignacionAP;
                //CAUSA PENAL
                causaPenal.CP_ANIO = AnioCP;
                causaPenal.CP_FOLIO = FolioCP;
                causaPenal.CP_BIS = BisCP;
                causaPenal.CP_FORANEO = ForaneoCP;
                if (TipoOrdenCP != -1)
                    causaPenal.CP_TIPO_ORDEN = TipoOrdenCP;
                if (PaisJuzgadoCP != -1)
                    causaPenal.CP_PAIS_JUZGADO = PaisJuzgadoCP;
                if (EstadoJuzgadoCP != -1)
                    causaPenal.CP_ESTADO_JUZGADO = EstadoJuzgadoCP;
                if (MunicipioJuzgadoCP != -1)
                    causaPenal.CP_MUNICIPIO_JUZGADO = MunicipioJuzgadoCP;
                if (!string.IsNullOrEmpty(FueroCP))
                    causaPenal.CP_FUERO = FueroCP;
                if (JuzgadoCP != -1)
                    causaPenal.CP_JUZGADO = JuzgadoCP;
                causaPenal.CP_FEC_RADICACION = FecRadicacionCP;
                if (!string.IsNullOrEmpty(AmpliacionCP))
                    causaPenal.CP_AMPLIACION = AmpliacionCP;
                if (TerminoCP != -1)
                    causaPenal.CP_TERMINO = TerminoCP;
                if (EstatusCP != -1)
                    causaPenal.ID_ESTATUS_CP = EstatusCP;
                causaPenal.CP_FEC_VENCIMIENTO_TERMINO = FecVencimientoTerinoCP;

                if (SelectedCausaPenal != null)//ACTUALIZAR
                {
                    //if ((new cCausaPenal()).Actualizar(causaPenal))
                    //{
                    //    if (GuardarDelitosCausaPenal(causaPenal))//, CausaPenalDelitos.ToList()))
                    //    {
                    //        /*******************************************************/
                    //        new Dialogos().ConfirmacionDialogo("Éxito!", "Información Grabada Exitosamente.");
                    //        this.ViewModelArbol();
                    //        EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                    //        return true;
                    //        /******************************************************/
                    //    }
                    //}
                }
                else//INSERTAR
                {
                    causaPenal.ID_CAUSA_PENAL = (new cCausaPenal()).Insertar(causaPenal);
                    SelectedCausaPenal = causaPenal;
                    if (causaPenal.ID_CAUSA_PENAL > 0)
                    {
                        List<CAUSA_PENAL_DELITO> cpDelitos = new List<CAUSA_PENAL_DELITO>();
                        if (causaPenal.CAUSA_PENAL_DELITO.Count > 0)
                            cpDelitos = causaPenal.CAUSA_PENAL_DELITO.ToList();
                        //if (GuardarDelitosCausaPenal(causaPenal))//, cpDelitos))
                        //{
                        //COPIAMOS LOS DELITOS A LA SENTENCIA
                        LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                        foreach (var delito in LstCausaPenalDelitos)
                        {
                            LstSentenciaDelitos.Add(new SENTENCIA_DELITO
                            {
                                ID_DELITO = delito.ID_DELITO,
                                ID_FUERO = delito.ID_FUERO,
                                ID_MODALIDAD = delito.ID_MODALIDAD,
                                ID_TIPO_DELITO = delito.ID_TIPO_DELITO,
                                CANTIDAD = delito.CANTIDAD,
                                OBJETO = delito.OBJETO,
                                MODALIDAD_DELITO = delito.MODALIDAD_DELITO,
                                TIPO_DELITO = delito.TIPO_DELITO,
                                DESCR_DELITO = delito.DESCR_DELITO
                            });
                        }
                        if (LstSentenciaDelitos.Count > 0)
                            SentenciaDelitoEmpty = false;
                        else
                            SentenciaDelitoEmpty = true;

                        new Dialogos().ConfirmacionDialogo("Éxito!", "Información Grabada Exitosamente.");
                        this.ViewModelArbol();
                        EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                        GuardarBandera = true;
                        return true;
                        // }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar causa penal", ex);
            }
            return false;
        }
        //private bool GuardarDelitosCausaPenal(CAUSA_PENAL causaPenal, List<CAUSA_PENAL_DELITO> cpDelitos)
        private bool GuardarDelitosCausaPenal(CAUSA_PENAL causaPenal)
        {
            try
            {
                var cpDelitos = new List<CAUSA_PENAL_DELITO>();
                if (LstCausaPenalDelitos != null)
                {
                    short index = 1;
                    foreach (var d in LstCausaPenalDelitos)
                    {
                        cpDelitos.Add(new CAUSA_PENAL_DELITO
                        {
                            ID_CENTRO = SelectedIngreso.ID_CENTRO,
                            ID_ANIO = SelectedIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectedIngreso.ID_INGRESO,
                            ID_CAUSA_PENAL = causaPenal.ID_CAUSA_PENAL,
                            ID_DELITO = d.ID_DELITO,
                            ID_FUERO = d.ID_FUERO,
                            ID_MODALIDAD = d.ID_MODALIDAD,
                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                            CANTIDAD = d.CANTIDAD,
                            OBJETO = d.OBJETO,
                            DESCR_DELITO = d.DESCR_DELITO,
                            ID_CONS = index
                        });
                        index++;
                    }
                }
                //if ((new cCausaPenalDelito()).Insertar(causaPenal.ID_CENTRO, causaPenal.ID_ANIO, causaPenal.ID_IMPUTADO, causaPenal.ID_INGRESO, causaPenal.ID_CAUSA_PENAL, cpDelitos))
                //{
                //    LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(new cCausaPenalDelito().ObtenerTodos(causaPenal.ID_CENTRO, causaPenal.ID_ANIO, causaPenal.ID_IMPUTADO, causaPenal.ID_INGRESO, causaPenal.ID_CAUSA_PENAL));
                return true;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar delitos causa penal", ex);
            }
            return false;
        }

        private bool GuardarCoparticipes()//(CAUSA_PENAL causaPenal)
        {
            try
            {
                var cop = new List<COPARTICIPE>();
                //COPARTICIPES
                if (LstCoparticipe != null)
                {
                    if (LstCoparticipe.Count > 0)
                    {
                        short Id = 1, i = 0;
                        foreach (var c in LstCoparticipe)
                        {
                            cop.Add(new COPARTICIPE
                            {
                                ID_CENTRO = SelectedCausaPenal.ID_CENTRO,//causaPenal.ID_CENTRO,
                                ID_ANIO = SelectedCausaPenal.ID_ANIO,//causaPenal.ID_ANIO,
                                ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,//causaPenal.ID_IMPUTADO,
                                ID_INGRESO = SelectedCausaPenal.ID_INGRESO,//causaPenal.ID_INGRESO,
                                ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,//causaPenal.ID_CAUSA_PENAL,
                                ID_COPARTICIPE = Id,
                                PATERNO = c.PATERNO,
                                MATERNO = c.MATERNO,
                                NOMBRE = c.NOMBRE,
                                COPARTICIPE_ALIAS = GetCoparticipeAlias(i, Id),//cal,
                                COPARTICIPE_APODO = GetCoparticipeApodo(i, Id)//cap
                            });
                            i++;
                            Id++;
                        }
                    }

                    if ((new cCoparticipe()).Insertar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL, cop))
                    {
                        this.ViewModelArbol();
                        EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar coparticipes", ex);
            }
            return false;
        }

        private List<COPARTICIPE_ALIAS> GetCoparticipeAlias(short i, short Id)
        {
            var resultado = new List<COPARTICIPE_ALIAS>();
            try
            {
                if (LstCoparticipe[i].COPARTICIPE_ALIAS != null)
                {
                    var alias = LstCoparticipe[i].COPARTICIPE_ALIAS.ToList();
                    short index = 1;
                    foreach (var a in alias)
                    {
                        resultado.Add(
                             new COPARTICIPE_ALIAS()
                             {
                                 COPARTICIPE = null,
                                 ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                 ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                 ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                 ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                 ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                 ID_COPARTICIPE = Id,
                                 ID_COPARTICIPE_ALIAS = index,
                                 MATERNO = a.MATERNO,
                                 PATERNO = a.PATERNO,
                                 NOMBRE = a.NOMBRE
                             });
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener coparticipe alias", ex);
            }
            return resultado;
        }


        private List<COPARTICIPE_APODO> GetCoparticipeApodo(short i, short Id)
        {
            var resultado = new List<COPARTICIPE_APODO>();
            try
            {
                if (LstCoparticipe[i].COPARTICIPE_APODO != null)
                {
                    var apodo = LstCoparticipe[i].COPARTICIPE_APODO.ToList();
                    short index = 1;
                    foreach (var a in apodo)
                    {
                        resultado.Add(
                            new COPARTICIPE_APODO()
                            {
                                COPARTICIPE = null,
                                ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                ID_COPARTICIPE = Id,
                                ID_COPARTICIPE_APODO = index,
                                APODO = a.APODO
                            });
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error obtener coparticipe apodo", ex);
            }
            return resultado;
        }


        #endregion

        #region CAUSA PENAL DELITO
        private void AgregarCausaPenalDelito(int opcion)
        {
            //if (opcion == 3)//SENTENCIA
            //{
            //    if (SelectedDelitoArbol != null)
            //    {

            //        if (LstSentenciaDelitos == null)
            //            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
            //        if (!EditarCPDelito)
            //        {
            //            short id = 0;
            //            if (SelectedCausaPenal.SENTENCIAs != null)
            //            {
            //                if (SelectedCausaPenal.SENTENCIAs.Count > 0)
            //                {
            //                    id = SelectedCausaPenal.SENTENCIAs.First().ID_SENTENCIA;
            //                }
            //            }
            //            LstSentenciaDelitos.Add(new SENTENCIA_DELITO
            //            {
            //                ID_CENTRO = SelectedIngreso.ID_CENTRO,
            //                ID_ANIO = SelectedIngreso.ID_ANIO,
            //                ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
            //                ID_INGRESO = SelectedIngreso.ID_INGRESO,
            //                ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
            //                ID_SENTENCIA = id,
            //                ID_DELITO = SelectedDelitoArbol.ID_DELITO,
            //                ID_FUERO = SelectedDelitoArbol.ID_FUERO,
            //                ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD,
            //                ID_TIPO_DELITO = TipoD,
            //                CANTIDAD = CantidadD,
            //                OBJETO = ObjetoD,
            //                MODALIDAD_DELITO = SelectedDelitoArbol,
            //                TIPO_DELITO = SelectedTipoDelito,
            //                DESCR_DELITO = ModalidadD

            //            });
            //        }
            //        else
            //        {
            //            SelectedSentenciaDelito.ID_DELITO = SelectedDelitoArbol.ID_DELITO;
            //            SelectedSentenciaDelito.ID_FUERO = SelectedDelitoArbol.ID_FUERO;
            //            SelectedSentenciaDelito.ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD;
            //            SelectedSentenciaDelito.ID_TIPO_DELITO = TipoD;
            //            SelectedSentenciaDelito.CANTIDAD = CantidadD;
            //            SelectedSentenciaDelito.OBJETO = ObjetoD;
            //            SelectedSentenciaDelito.MODALIDAD_DELITO = SelectedDelitoArbol;
            //            SelectedSentenciaDelito.TIPO_DELITO = SelectedTipoDelito;
            //            SelectedSentenciaDelito.DESCR_DELITO = ModalidadD;
            //            LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>(LstSentenciaDelitos);
            //        }
            //        if (LstSentenciaDelitos.Count > 0)
            //            SentenciaDelitoEmpty = false;
            //        else
            //            SentenciaDelitoEmpty = true;
            //        this.CancelarCausaPenalDelito();

            //    }
            //}
            //else if (opcion == 8)//RECURSO
            //{
            //    if (SelectedDelito != null)
            //    {

            //        //if (RecursoDelitos == null)
            //        //    RecursoDelitos = new ObservableCollection<RECURSO_DELITO>();
            //        //if (!EditarCPDelito)
            //        //{
            //        //    RecursoDelitos.Add(new RECURSO_DELITO
            //        //    {
            //        //        ID_CENTRO = SelectedIngreso.ID_CENTRO,
            //        //        ID_ANIO = SelectedIngreso.ID_ANIO,
            //        //        ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
            //        //        ID_INGRESO = SelectedIngreso.ID_INGRESO,
            //        //        ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
            //        //        ID_RECURSO = 0,
            //        //        ID_DELITO = SelectedDelito.ID_DELITO,
            //        //        ID_FUERO = SelectedDelito.ID_FUERO,
            //        //        ID_MODALIDAD = SelectedDelito.ID_MODALIDAD,
            //        //        ID_TIPO_DELITO = TipoD,
            //        //        CANTIDAD = CantidadD,
            //        //        OBJETO = ObjetoD,
            //        //        MODALIDAD_DELITO = SelectedDelito,
            //        //        TIPO_DELITO = SelectedTipoDelito,
            //        //        DESCR_DELITO = ModalidadD
            //        //    });
            //        }
            //        else
            //        {
            //            SelectedRecursoDelito.ID_DELITO = SelectedDelito.ID_DELITO;
            //            SelectedRecursoDelito.ID_FUERO = SelectedDelito.ID_FUERO;
            //            SelectedRecursoDelito.ID_MODALIDAD = SelectedDelito.ID_MODALIDAD;
            //            SelectedRecursoDelito.ID_TIPO_DELITO = TipoD;
            //            SelectedRecursoDelito.CANTIDAD = CantidadD;
            //            SelectedRecursoDelito.OBJETO = ObjetoD;
            //            SelectedRecursoDelito.MODALIDAD_DELITO = SelectedDelito;
            //            SelectedRecursoDelito.TIPO_DELITO = SelectedTipoDelito;
            //            SelectedRecursoDelito.DESCR_DELITO = ModalidadD;
            //           // RecursoDelitos = new ObservableCollection<RECURSO_DELITO>(RecursoDelitos);
            //        }
            //        this.CancelarCausaPenalDelito();
            //    }
            //}
            //else //CAUSA PENAL
            //{
            //    if (SelectedDelitoArbol != null)
            //    {

            //        if (LstCausaPenalDelitos == null)
            //            LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>();


            //        if (SelectedCausaPenalDelito == null)//INSERT
            //        {
            //            LstCausaPenalDelitos.Add(new CAUSA_PENAL_DELITO
            //            {
            //                /////////////////////////////////
            //                ID_CENTRO = 0,
            //                ID_ANIO = 0,
            //                ID_IMPUTADO = 0,
            //                ID_INGRESO = 0,
            //                /////////////////////////////////
            //                ID_CAUSA_PENAL = (SelectedCausaPenal != null ? SelectedCausaPenal.ID_CAUSA_PENAL : (short)0),
            //                ID_DELITO = SelectedDelitoArbol.ID_DELITO,
            //                ID_FUERO = SelectedDelitoArbol.ID_FUERO,
            //                ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD,
            //                ID_TIPO_DELITO = TipoD,
            //                CANTIDAD = CantidadD,
            //                OBJETO = ObjetoD,
            //                MODALIDAD_DELITO = SelectedDelitoArbol,
            //                TIPO_DELITO = SelectedTipoDelito,
            //                DESCR_DELITO = ModalidadD
            //            });
            //        }
            //        else//UPDATE
            //        {
            //            SelectedCausaPenalDelito.ID_DELITO = SelectedDelitoArbol.ID_DELITO;
            //            SelectedCausaPenalDelito.ID_FUERO = SelectedDelitoArbol.ID_FUERO;
            //            SelectedCausaPenalDelito.ID_MODALIDAD = SelectedDelitoArbol.ID_MODALIDAD;
            //            SelectedCausaPenalDelito.ID_TIPO_DELITO = TipoD;
            //            SelectedCausaPenalDelito.CANTIDAD = CantidadD;
            //            SelectedCausaPenalDelito.OBJETO = ObjetoD;
            //            SelectedCausaPenalDelito.MODALIDAD_DELITO = SelectedDelitoArbol;
            //            SelectedCausaPenalDelito.TIPO_DELITO = SelectedTipoDelito;
            //            SelectedCausaPenalDelito.DESCR_DELITO = ModalidadD;
            //            LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos);
            //        }
            //        if (LstCausaPenalDelitos.Count > 0)
            //            CausaPenalDelitoEmpty = false;
            //        else
            //            CausaPenalDelitoEmpty = true;
            //        this.CancelarCausaPenalDelito();

            //    }
            //    else
            //    {
            //        SelectedCausaPenalDelito.ID_TIPO_DELITO = TipoD;
            //        SelectedCausaPenalDelito.CANTIDAD = CantidadD;
            //        SelectedCausaPenalDelito.OBJETO = ObjetoD;
            //        SelectedCausaPenalDelito.DESCR_DELITO = ModalidadD;
            //        LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos);
            //    }
            //}
        }

        private void CancelarCausaPenalDelito()
        {
            try
            {
                DelitoVisible = false;
                DelitoExpander = false;
                SelectedDelito = null;
                DelitoD = ModalidadD = GraveD = CantidadD = ObjetoD = string.Empty;
                TipoD = -1;
                SelectedCausaPenalDelito = null;
                SelectedSentenciaDelito = null;
                SelectedRecursoDelito = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cancelar causa penal delito", ex);
            }
        }

        private void EditarCausaPenalDelito(int opcion)
        {
            try
            {
                if (opcion == 3)//SENTENCIA
                {
                    if (SelectedSentenciaDelito != null)
                    {
                        SelectedDelito = SelectedSentenciaDelito.MODALIDAD_DELITO;
                        DelitoD = SelectedDelito.DELITO.DESCR;
                        ModalidadD = SelectedDelito.DESCR;
                        GraveD = SelectedDelito.DELITO.GRAVE;
                        TipoD = SelectedSentenciaDelito.ID_TIPO_DELITO.Value;
                        CantidadD = SelectedSentenciaDelito.CANTIDAD;
                        ObjetoD = SelectedSentenciaDelito.OBJETO;
                        DelitoVisible = true;
                    }
                }
                else if (opcion == 8)
                {
                    if (SelectedRecursoDelito != null)
                    {
                        SelectedDelito = SelectedRecursoDelito.MODALIDAD_DELITO;
                        DelitoD = SelectedDelito.DELITO.DESCR;
                        ModalidadD = SelectedDelito.DESCR;
                        GraveD = SelectedDelito.DELITO.GRAVE;
                        TipoD = SelectedRecursoDelito.ID_TIPO_DELITO.Value;
                        CantidadD = SelectedRecursoDelito.CANTIDAD;
                        ObjetoD = SelectedRecursoDelito.OBJETO;
                        DelitoVisible = true;
                    }
                }
                else //CAUSA PENAL
                {
                    if (SelectedCausaPenalDelito != null)
                    {
                        SelectedDelito = SelectedCausaPenalDelito.MODALIDAD_DELITO;
                        DelitoD = SelectedDelito.DELITO.DESCR;
                        ModalidadD = SelectedCausaPenalDelito.DESCR_DELITO;
                        GraveD = SelectedDelito.DELITO.GRAVE;
                        TipoD = SelectedCausaPenalDelito.ID_TIPO_DELITO.Value;
                        CantidadD = SelectedCausaPenalDelito.CANTIDAD;
                        ObjetoD = SelectedCausaPenalDelito.OBJETO;
                        SelectedDelitoArbol = SelectedCausaPenalDelito.MODALIDAD_DELITO;
                        DelitoVisible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al editar causa penal delito", ex);
            }
        }

        private void EliminarCausaPenalDelito(int opcion)
        {
            try
            {
                if (opcion == 3)//SENTENCIA
                {
                    if (SelectedSentenciaDelito != null)
                    {
                        LstSentenciaDelitos.Remove(SelectedSentenciaDelito);
                    }
                    if (LstSentenciaDelitos.Count > 0)
                        SentenciaDelitoEmpty = false;
                    else
                        SentenciaDelitoEmpty = true;
                }
                else if (opcion == 8)
                {
                    if (SelectedRecursoDelito != null)
                    {
                        //RecursoDelitos.Remove(SelectedRecursoDelito);
                    }
                }
                else//DELITO
                {
                    if (SelectedCausaPenalDelito != null)
                    {
                        LstCausaPenalDelitos.Remove(SelectedCausaPenalDelito);
                        if (LstCausaPenalDelitos != null)
                        {
                            if (LstCausaPenalDelitos.Count > 0)
                                CausaPenalDelitoEmpty = false;
                            else
                                CausaPenalDelitoEmpty = true;
                        }
                        else
                        {
                            CausaPenalDelitoEmpty = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar causa penal recurso", ex);
            }
        }
        #endregion

        #region RECURSOS
        private void AgregarRecurso()
        {
            try
            {
                TabRecursoSelected = true;
                IngresosVisible = false;
                DatosIngresoVisible = true;
                //TABS
                RecursoTab = true;
                CausaPenalTab = CoparticipeTab = SentenciaTab = IngresoTab = RdiTab = DelitosTab = DelitoTab = RecursosTab = false;
                RecursoDelitoEmpty = true;
                if (SelectedSentencia != null)
                {
                    if (LstRecursoDelitos == null)
                        LstRecursoDelitos = new ObservableCollection<RECURSO_DELITO>();
                    foreach (var d in SelectedSentencia.SENTENCIA_DELITO)
                    {
                        LstRecursoDelitos.Add(
                            new RECURSO_DELITO()
                            {
                                ID_DELITO = d.ID_DELITO,
                                ID_FUERO = d.ID_FUERO,
                                ID_MODALIDAD = d.ID_MODALIDAD,
                                ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                DESCR_DELITO = d.DESCR_DELITO,
                                CANTIDAD = d.CANTIDAD,
                                OBJETO = d.OBJETO,
                                MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                TIPO_DELITO = d.TIPO_DELITO
                            });
                    }
                    if (LstRecursoDelitos.Count > 0)
                        RecursoDelitoEmpty = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar recurso", ex);
            }
        }

        private bool GuardarRecursos()
        {
            try
            {
                if (!base.HasErrors)
                {
                    var recursos = new RECURSO();

                    recursos.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                    recursos.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                    recursos.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                    recursos.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                    recursos.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                    if (SelectedRecurso != null)
                        recursos.ID_RECURSO = SelectedRecurso.ID_RECURSO;
                    recursos.ID_TIPO_RECURSO = SelectedTipoRecurso.ID_TIPO_RECURSO;
                    recursos.ID_TRIBUNAL = SelectedTribunal.ID_JUZGADO;
                    recursos.FUERO = SelectedTribunal.ID_FUERO;
                    recursos.RESULTADO = RResultadoRecurso;// SelectedRecursoResultado.RESULTADO;
                    recursos.FEC_RECURSO = FecR;
                    recursos.TOCA_PENAL = TocaPenalR;
                    recursos.NO_OFICIO = NoOficioR;
                    recursos.RESOLUCION = ResolucionR;
                    recursos.FEC_RESOLUCION = FecResolucionR;
                    recursos.MULTA = MultaR;
                    recursos.REPARACION_DANIO = ReparacionDanioR;
                    recursos.SUSTITUCION_PENA = SustitucionPenaR;
                    recursos.MULTA_CONDICIONAL = MultaCondicionalR;
                    recursos.SENTENCIA_ANIOS = AniosR;
                    recursos.SENTENCIA_MESES = MesesR;
                    recursos.SENTENCIA_DIAS = DiasR;
                    //DELITOS
                    var del = new List<RECURSO_DELITO>();
                    //if (RecursoDelitos != null)
                    //{
                    //    if (RecursoDelitos.Count > 0)
                    //    {
                    //        short Id = 0;
                    //        if (SelectedRecurso != null)
                    //            Id = SelectedRecurso.ID_RECURSO;
                    //        foreach (var d in RecursoDelitos)
                    //        {
                    //            del.Add(new RECURSO_DELITO
                    //            {
                    //                ID_CENTRO = d.ID_CENTRO,
                    //                ID_ANIO = d.ID_ANIO,
                    //                ID_IMPUTADO = d.ID_IMPUTADO,
                    //                ID_INGRESO = d.ID_INGRESO,
                    //                ID_CAUSA_PENAL = d.ID_CAUSA_PENAL,
                    //                ID_RECURSO = Id,
                    //                ID_DELITO = d.ID_DELITO,
                    //                ID_FUERO = d.ID_FUERO,
                    //                ID_MODALIDAD = d.ID_MODALIDAD,
                    //                ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                    //                CANTIDAD = d.CANTIDAD,
                    //                OBJETO = d.OBJETO,
                    //                DESCR_DELITO = d.DESCR_DELITO
                    //            });
                    //        }
                    //    }
                    //}
                    if (SelectedRecurso != null)
                    {
                        //if ((new cRecurso()).Actualizar(recursos))
                        //{
                        if (recursos.ID_RECURSO > 0)
                        {
                            if ((new cRecursoDelito()).Insertar(recursos.ID_CENTRO, recursos.ID_ANIO, recursos.ID_IMPUTADO, recursos.ID_INGRESO, recursos.ID_CAUSA_PENAL, recursos.ID_RECURSO, del))
                            {
                                SelectExpediente = (new cImputado()).Obtener(SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO).FirstOrDefault();
                                this.ViewModelArbol();
                                new Dialogos().ConfirmacionDialogo("Éxito!", "Información Grabada Exitosamente.");
                                return true;
                            }
                        }
                        //}
                    }
                    else
                    {
                        recursos.RECURSO_DELITO = del;
                        recursos.ID_RECURSO = ((new cRecurso()).Insertar(recursos));
                        if (recursos.ID_RECURSO > 0)
                        {
                            SelectExpediente = (new cImputado()).Obtener(SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO).FirstOrDefault();
                            this.ViewModelArbol();
                            new Dialogos().ConfirmacionDialogo("Éxito!", "Información Grabada Exitosamente.");
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar recurso", ex);
                return false;
            }
        }

        private void SetRecurso()
        {
            try
            {
                if (SelectedRecurso != null)
                {
                    TipoR = SelectedRecurso.ID_TIPO_RECURSO.Value;
                    TribunalR = SelectedRecurso.ID_TRIBUNAL.Value;
                    var resultado = SelectedTipoRecurso.RECURSO_RESULTADO.Where(w => w.ID_TIPO_RECURSO == TipoR && w.RESULTADO.Equals(SelectedRecurso.RESULTADO)).SingleOrDefault();
                    if (resultado != null)
                        RResultadoRecurso = resultado.RESULTADO;
                    //SelectedRecursoResultado = SelectedTipoRecurso.RECURSO_RESULTADO.Where(w => w.ID_TIPO_RECURSO == TipoR && w.RESULTADO.Equals(SelectedRecurso.RESULTADO)).FirstOrDefault();
                    FecR = SelectedRecurso.FEC_RECURSO.Value;
                    TocaPenalR = SelectedRecurso.TOCA_PENAL;
                    NoOficioR = SelectedRecurso.NO_OFICIO;
                    ResolucionR = SelectedRecurso.RESOLUCION;
                    FecResolucionR = SelectedRecurso.FEC_RESOLUCION.Value;
                    MultaR = SelectedRecurso.MULTA;
                    ReparacionDanioR = SelectedRecurso.REPARACION_DANIO;
                    SustitucionPenaR = SelectedRecurso.SUSTITUCION_PENA;
                    MultaCondicionalR = SelectedRecurso.MULTA_CONDICIONAL;
                    AniosR = SelectedRecurso.SENTENCIA_ANIOS.Value;
                    MesesR = SelectedRecurso.SENTENCIA_MESES.Value;
                    DiasR = SelectedRecurso.SENTENCIA_DIAS.Value;
                    //DELITO
                    //RecursoDelitos = new ObservableCollection<RECURSO_DELITO>(SelectedRecurso.RECURSO_DELITO);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer recurso", ex);
            }
        }

        private async void EliminarRecurso()
        {
            if (SelectedRecurso != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Validación!", "Desea eliminar este recurso") == 1)
                {
                    if (new cRecurso().Eliminar(SelectedRecurso.ID_CENTRO, SelectedRecurso.ID_ANIO, SelectedRecurso.ID_IMPUTADO, SelectedRecurso.ID_INGRESO, SelectedRecurso.ID_CAUSA_PENAL, SelectedRecurso.ID_RECURSO))
                    {
                        new Dialogos().ConfirmacionDialogo("Confirmación!", "El recurso ha sido eliminado correctamente.");
                        ViewModelArbol();
                        ObtenerTodoRecurso();
                    }
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un recurso.");

            //try
            //{
            //    if (SelectedRecurso != null)
            //    {
            //        if ((new cRecurso()).Eliminar(SelectedRecurso.ID_CENTRO, SelectedRecurso.ID_ANIO, SelectedRecurso.ID_IMPUTADO, SelectedRecurso.ID_INGRESO, SelectedRecurso.ID_CAUSA_PENAL, SelectedRecurso.ID_RECURSO))
            //        {
            //            SelectExpediente = (new cImputado()).Obtener(SelectExpediente.ID_IMPUTADO, SelectExpediente.ID_ANIO, SelectExpediente.ID_CENTRO).FirstOrDefault();
            //            SelectIngreso = SelectExpediente.INGRESO.Where(w => w.ID_CENTRO == SelectedCausaPenal.INGRESO.ID_CENTRO && w.ID_ANIO == SelectedCausaPenal.INGRESO.ID_ANIO && w.ID_IMPUTADO == SelectedCausaPenal.INGRESO.ID_IMPUTADO && w.ID_INGRESO == SelectedCausaPenal.INGRESO.ID_INGRESO).FirstOrDefault();
            //            SelectedIngreso = SelectIngreso;
            //            SelectedCausaPenal = SelectedIngreso.CAUSA_PENAL.Where(w => w.ID_CENTRO == SelectedCausaPenal.INGRESO.ID_CENTRO && w.ID_ANIO == SelectedCausaPenal.ID_ANIO && w.ID_IMPUTADO == SelectedCausaPenal.ID_IMPUTADO && w.ID_INGRESO == SelectedCausaPenal.ID_INGRESO && w.ID_CAUSA_PENAL == SelectedCausaPenal.ID_CAUSA_PENAL).FirstOrDefault();
            //            SelectedRecurso = null;
            //            ViewModelArbol();
            //            ObtenerTodoRecurso();
            //            return true;
            //        }
            //    }
            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar recurso", ex);
            //    return false;
            //}
        }
        #endregion

        #region COPARTICIPE
        //private void AgregarCoparticipe()
        //{
        //    if (!base.HasErrors)
        //    {
        //        if (Coparticipes == null)
        //            Coparticipes = new ObservableCollection<COPARTICIPE>();
        //        if (!EditarCoparticipeBandera)
        //        {
        //            Coparticipes.Add(new COPARTICIPE
        //            {
        //                ID_CENTRO = SelectedIngreso.ID_CENTRO,
        //                ID_ANIO = SelectedIngreso.ID_ANIO,
        //                ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
        //                ID_INGRESO = SelectedIngreso.ID_INGRESO,
        //                ID_CAUSA_PENAL = (SelectedCausaPenal != null ? SelectedCausaPenal.ID_CAUSA_PENAL : (short)0),
        //                PATERNO = PaternoC,
        //                MATERNO = MaternoC,
        //                NOMBRE = NombreC
        //            });
        //        }
        //        else
        //        {
        //            SelectedCoparticipe.PATERNO = PaternoC;
        //            SelectedCoparticipe.MATERNO = MaternoC;
        //            SelectedCoparticipe.NOMBRE = NombreC;
        //            Coparticipes = new ObservableCollection<COPARTICIPE>(Coparticipes);
        //        }
        //        this.CancelarCoparticipe();
        //    }
        //}
        //private void CancelarCoparticipe()
        //{
        //    CoparticipeVisible = false;
        //    PaternoC = MaternoC = NombreC = string.Empty;
        //}
        //private void EditarCoparticipe()
        //{
        //    if (SelectedCoparticipe != null)
        //    {
        //        PaternoC = SelectedCoparticipe.PATERNO;
        //        MaternoC = SelectedCoparticipe.MATERNO;
        //        NombreC = SelectedCoparticipe.NOMBRE;
        //    }
        //}
        //private void EliminarCoparticipe()
        //{
        //    if (SelectedCoparticipe != null)
        //    {
        //        Coparticipes.Remove(SelectedCoparticipe);
        //        CoparticipesAlias.Clear();
        //        CoparticipeApodo.Clear();
        //    }
        //}
        #endregion

        #region COPARTICIPE ALIAS
        //private void AgregarCoparticipeAlias()
        //{
        //    if (!base.HasErrors)
        //    {
        //        if (!EditarCoparticipeAliasBandera)
        //        {
        //            Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_ALIAS.Add(new COPARTICIPE_ALIAS
        //            {
        //                ID_CENTRO = SelectIngreso.ID_CENTRO,
        //                ID_ANIO = SelectIngreso.ID_ANIO,
        //                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
        //                ID_INGRESO = SelectIngreso.ID_INGRESO,
        //                ID_CAUSA_PENAL = /*SelectedCausaPenal.ID_CAUSA_PENAL*/ 1,
        //                PATERNO = PaternoAliasC,
        //                MATERNO = MaternoAliasC,
        //                NOMBRE = NombreAliasC,

        //            });
        //        }
        //        else
        //        {
        //            if (SelectedCoparticipeAlias != null)
        //            {
        //                SelectedCoparticipeAlias.PATERNO = PaternoAliasC;
        //                SelectedCoparticipeAlias.MATERNO = MaternoAliasC;
        //                SelectedCoparticipeAlias.NOMBRE = NombreAliasC;
        //            }
        //        }
        //        CoparticipesAlias = new ObservableCollection<COPARTICIPE_ALIAS>(Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_ALIAS);
        //        this.CancelarCoparticipeAlias();
        //    }
        //}
        //private void CancelarCoparticipeAlias()
        //{
        //    CoparticipeAliasVisible = false;
        //    PaternoAliasC = MaternoAliasC = NombreAliasC = string.Empty;
        //}
        //private void EditarCoparticipeAlias()
        //{
        //    if (SelectedCoparticipeAlias != null)
        //    {
        //        PaternoAliasC = SelectedCoparticipeAlias.PATERNO;
        //        MaternoAliasC = SelectedCoparticipeAlias.MATERNO;
        //        NombreAliasC = SelectedCoparticipeAlias.NOMBRE;
        //    }
        //}
        //private void EliminarCoparticipeAlias()
        //{
        //    if (SelectedCoparticipeAlias != null)
        //    {
        //        Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_ALIAS.Remove(SelectedCoparticipeAlias);
        //        CoparticipesAlias = new ObservableCollection<COPARTICIPE_ALIAS>(Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_ALIAS);
        //    }
        //}
        //private void GuardarCoparticipes()
        //{
        //    if (Coparticipes.Count > 0)
        //    {
        //        (new cCoparticipe()).Insertar(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, 1, Coparticipes.ToList());
        //    }
        //}
        #endregion

        #region COPARTICIPE APODO
        //private void AgregarCoparticipeApodo()
        //{
        //    if (!base.HasErrors)
        //    {
        //        if (!EditarCoparticipeApodoBandera)
        //        {
        //            Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_APODO.Add(new COPARTICIPE_APODO
        //            {
        //                ID_CENTRO = SelectIngreso.ID_CENTRO,
        //                ID_ANIO = SelectIngreso.ID_ANIO,
        //                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
        //                ID_INGRESO = SelectIngreso.ID_INGRESO,
        //                ID_CAUSA_PENAL = /*SelectedCausaPenal.ID_CAUSA_PENAL*/ 1,
        //                APODO = ApodoC
        //            });
        //        }
        //        else
        //        {
        //            if (SelectedCoparticipeApodo != null)
        //            {
        //                SelectedCoparticipeApodo.APODO = ApodoC;
        //            }
        //        }
        //        CoparticipeApodo = new ObservableCollection<COPARTICIPE_APODO>(Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_APODO);
        //        this.CancelarCoparticipeApodo();
        //    }
        //}
        //private void CancelarCoparticipeApodo()
        //{
        //    CoparticipeApodoVisible = false;
        //    ApodoC = string.Empty;
        //}
        //private void EditarCoparticipeApodo()
        //{
        //    if (SelectedCoparticipeApodo != null)
        //    {
        //        ApodoC = SelectedCoparticipeApodo.APODO;
        //    }
        //}
        //private void EliminarCoparticipeApodo()
        //{
        //    if (SelectedCoparticipeApodo != null)
        //    {
        //        Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_APODO.Remove(SelectedCoparticipeApodo);
        //        CoparticipeApodo = new ObservableCollection<COPARTICIPE_APODO>(Coparticipes[SelectedCoparticipeIndex].COPARTICIPE_APODO);
        //    }
        //}
        #endregion

        #region LIMPIAR
        private void LimpiarCausaPenal()
        {
            try
            {
                //SelectedAgencia = Agencias.Where(w => w.ID_AGENCIA <= 0).FirstOrDefault();
                AnioAP = null;
                FolioAP = null;
                AveriguacionPreviaAP = string.Empty;
                FecAveriguacionAP = FecConsignacionAP = null;
                AgenciaAP = -1;

                AnioCP = null;
                FolioCP = null;
                BisCP = string.Empty;
                ForaneoCP = string.Empty;
                TipoOrdenCP = -1;
                PaisJuzgadoCP = Parametro.PAIS;
                EstadoJuzgadoCP = 2;
                MunicipioJuzgadoCP = -1;
                FueroCP = string.Empty;
                JuzgadoCP = -1;
                FecRadicacionCP = FecVencimientoTerinoCP = null;
                AmpliacionCP = string.Empty;
                TerminoCP = -1;
                EstatusCP = -1;

                ObservacionesCP = string.Empty;

                //LIMPIAR DELITOS
                LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>();
                if (LstCausaPenalDelitos != null)
                {
                    if (LstCausaPenalDelitos.Count > 0)
                        CausaPenalDelitoEmpty = false;
                    else
                        CausaPenalDelitoEmpty = true;
                }
                else
                    CausaPenalDelitoEmpty = true;

                //LIMPIAR COPARTICIPES
                LstCoparticipe = new ObservableCollection<COPARTICIPE>();
                LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>();
                LstApodo = new ObservableCollection<COPARTICIPE_APODO>();

                //LIMPIAR SENTENCIA
                FecS = FecEjecutoriaS = FecInicioCompurgacionS = null;
                AniosS = MesesS = DiasS = null;

                MultaS = string.Empty;
                MultaPagadaS = false;

                ReparacionDanioPagadaS = false;
                ReparacionDanioS = string.Empty;

                SustitucionPenaPagadaS = false;
                SustitucionPenaS = string.Empty;

                SuspensionCondicionalS = string.Empty;
                ObservacionS = string.Empty;

                MotivoCancelacionAntecedenteS = string.Empty;

                GradoParticipacionS = -1;
                GradoAutoriaS = -1;
                AniosAbonadosS = 0;
                MesesAbonadosS = 0;
                DiasAbonadosS = 0;
                MultaSi = false;
                MultaNo = false;
                ReparacionSi = false;
                ReparacionNo = false;
                SustitucionSi = false;
                SustitucionNo = false;
                //SENTENCIA DELITOS
                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                if (LstSentenciaDelitos.Count > 0)
                    SentenciaDelitoEmpty = false;
                else
                    SentenciaDelitoEmpty = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar causa penal", ex);
            }
        }
        #endregion

        #endregion

        private bool GuardarDelitosSentencia(short Id)
        {
            try
            {
                List<SENTENCIA_DELITO> sDelitos = new List<SENTENCIA_DELITO>();
                if (LstSentenciaDelitos != null)
                {

                    foreach (var d in LstSentenciaDelitos)
                    {
                        short index = 1;
                        sDelitos.Add(new SENTENCIA_DELITO
                        {
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_SENTENCIA = Id,
                            ID_DELITO = d.ID_DELITO,
                            ID_FUERO = d.ID_FUERO,
                            ID_MODALIDAD = d.ID_MODALIDAD,
                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                            CANTIDAD = d.CANTIDAD,
                            OBJETO = d.OBJETO,
                            DESCR_DELITO = d.DESCR_DELITO,
                            ID_CONS = index
                        });
                        index++;
                    }
                    return new cSentenciaDelito().Insertar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL, Id, sDelitos);
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar delito sentencia", ex);
                return false;
            }
        }

        #region [LOADS]
        private void IngresoCausaPenalLoad(CausasPenalesIngresoCausaPenalView Window = null)
        {
            try
            {
                if (TabCausaPenalSelected)
                {
                    SetValidacionesCausaPenal();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingreso causa penal", ex);
            }
        }
        private void CausaPenalCoparticipeLoad(CausasPenalesCoparticipeView Window = null)
        {
            try
            {
                if (TabCoparticipeSelected)
                {
                    base.ClearRules();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar causa penal coparticipe", ex);
            }
        }
        private void IngresoLoad(CausasPenalesIngresoView Window = null)
        {
            try
            {
                if (TabIngresoSelected)
                {
                    SetValidacionesDatosIngreso();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingreso", ex);
            }
        }
        private void RecursoLoad(CausasPenalesRecursoView Window = null)
        {
            try
            {
                if (TabRecursoSelected)
                {
                    SetValidacionesRecurso();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar recurso", ex);
            }
        }
        private void SentenciaLoad(CausasPenalesSentenciaView Window = null)
        {
            try
            {
                if (TabSentenciaSelected)
                {
                    SetValidacionesSentencia();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar sentencia", ex);
            }
        }
        private void RecursosListadoLoad(CausasPenalesRDIView Window = null)
        {
            try
            {
                if (TabRecursosSelected)
                {
                    SetValidacionesRecurso();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar recursos listado", ex);
            }
        }
        private void CausaPenalDelitosLoad(CausasPenalesDelitosView Window = null)
        {
            try
            {
                if (TabDelitosSelected)
                {
                    base.ClearRules();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar causa penal delitos", ex);
            }
        }
        private void IngresosCausaPenalLoad(InsertarDelitoView Window = null)
        {
            try
            {
                if (IngresosVisible)
                {
                    base.ClearRules();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingresos causa penal", ex);
            }
        }
        private async void CausaPenalLoad(CausaPenalView Window = null)
        {
            try
            {
                estatus_inactivos=Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                TabWidth = Window.ActualHeight - 315;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);
                escaner.EscaneoCompletado += delegate 
                {
                    DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                    DocumentoDigitalizado = escaner.ScannedDocument;
                    
                    if (AutoGuardado)
                        if (DocumentoDigitalizado != null)
                            GuardarDocumento();

                    escaner.Dispose();
                };
                //SetIngreso();
                //InicializaVariables();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar causa penal", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAUSA_PENAL.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = true;// MenuFichaEnabled = true;

                        if (!PInsertar && !PEditar)//No tiene privilegio de guardar o editar
                            MenuGuardarEnabled = false;
                        else
                            MenuGuardarEnabled = true;//Tiene al menos un privilegio(editar o agregar)
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion


        #endregion

        #region [UNLOAD]
        private void SentenciaUnload(CausasPenalesSentenciaView Window = null)
        {
            //LIMPIAMOS LAS REGLAS DE VALIDACION
            base.ClearRules();
        }
        #endregion

        #region EXTRAS
        private void LimpiarListasDelitos()
        {
            LstDelitoTitulo = new ObservableCollection<DELITO_TITULO>(new cDelitoTitulo().ObtenerTodos());
            LstTiposDelitos = new ObservableCollection<TIPO_DELITO>(new cTipoDelito().ObtenerTodos());
            DelitoD = string.Empty;
            ModalidadD = string.Empty;
            GraveD = string.Empty;
            ObjetoD = string.Empty;
            CantidadD = string.Empty;
            TipoD = -1;
        }
        private void PrepararListas()
        {
            try
            {

                Lista_Sources = escaner.Sources();
                if (Lista_Sources.Count > 0) SelectedSource = Lista_Sources.Where(w => w.Default).SingleOrDefault();
                HojasMaximo = string.Format("Escaneo permitido: {0} documentos máximo.", escaner.HojasMaximo);

                LstDelitos = new ObservableCollection<DELITO>((new cDelito()).ObtenerTodos().OrderBy(w => w.ID_DELITO));
                LstDelitoTitulo = new ObservableCollection<DELITO_TITULO>(new cDelitoTitulo().ObtenerTodos());
                LstPaises = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                LstEntidades = new ObservableCollection<ENTIDAD>();
                LstMunicipios = new ObservableCollection<MUNICIPIO>();
                LstTiposOrden = new ObservableCollection<TIPO_ORDEN>(new cTipoOrden().ObtenerTodos());
                LstTerminos = new ObservableCollection<TERMINO>(new cTermino().ObtenerTodos());
                LstFueros = new ObservableCollection<FUERO>(new cFuero().ObtenerTodos());
                LstAgencias = new ObservableCollection<AGENCIA>(new cAgencia().ObtenerTodos());
                LstJuzgados = new ObservableCollection<JUZGADO>();
                LstJuzgadoAmparo = new ObservableCollection<JUZGADO>(new cJuzgado().ObtenerTodos());
                LstCPEstatus = new ObservableCollection<CAUSA_PENAL_ESTATUS>(new cCausaPenalEstatus().ObtenerTodos());
                LstGradosAutoria = new ObservableCollection<GRADO_AUTORIA>(new cGradoAutoria().ObtenerTodos());
                LstGradosParticipacion = new ObservableCollection<GRADO_PARTICIPACION>(new cGradoParticipacion().ObtenerTodos());
                LstTiposIngreso = new ObservableCollection<TIPO_INGRESO>(new cTipoIngreso().ObtenerTodos());
                LstEstatusAdministrativo = new ObservableCollection<ESTATUS_ADMINISTRATIVO>(new cEstatusAdministrativo().ObtenerTodos());
                LstAutoridades = new ObservableCollection<TIPO_AUTORIDAD_INTERNA>(new cTipoAutoridadInterna().ObtenerTodos());
                LstTiposSeguridad = new ObservableCollection<TIPO_SEGURIDAD>(new cTipoSeguridad().ObtenerTodos());
                LstAutoridadDisposicion = new ObservableCollection<TIPO_DISPOSICION>(new cTipoDisposicion().ObtenerTodos());
                LstClasificaciones = new ObservableCollection<CLASIFICACION_JURIDICA>(new cClasificacionJuridica().ObtenerTodos());
                LstAutoridadesInterna = new ObservableCollection<TIPO_AUTORIDAD_INTERNA>(new cTipoAutoridadInterna().ObtenerTodos());
                LstIngresoDelitos = new ObservableCollection<INGRESO_DELITO>(new cIngresoDelito().ObtenerTodos());
                LstTiposDelitos = new ObservableCollection<TIPO_DELITO>(new cTipoDelito().ObtenerTodos());
                LstTiposRecursos = new ObservableCollection<TIPO_RECURSO>(new cTipoRecurso().ObtenerTodos("R"));
                LstTribunales = new ObservableCollection<JUZGADO>(new cJuzgado().ObtenerTodos());
                LstRecursoResultado = new ObservableCollection<RECURSO_RESULTADO>();
                LstSentenciaAI = new ObservableCollection<AMPARO_INDIRECTO_SENTENCIA>(new cAmparoIndirectoSentencia().ObtenerTodos());
                LstSentenciaAD = new ObservableCollection<AMPARO_DIRECTO_SENTENCIA>(new cAmparoDirectoSentencia().ObtenerTodos());
                LstIncidentes = new ObservableCollection<TIPO_RECURSO>(new cTipoRecurso().ObtenerTodos("I"));
                LstIncidenteResultado = new ObservableCollection<RECURSO_RESULTADO>();
                LstAmparoIncidenteTipo = new ObservableCollection<AMPARO_INCIDENTE_TIPO>(new cAmparoIncidenteTipo().ObtenerTodos(string.Empty, "S"));
                #region Baja
                //LstMotivoBaja = new ObservableCollection<CAUSA_PENAL_BAJA>(new cCausaPenalBaja().ObtenerTodos());
                //LstAutoridadBaja = new ObservableCollection<CAUSA_PENAL_AUTORIDAD_BAJA>(new cCausaPenalAutoridadBaja().ObtenerTodos());
                LstLiberacionAutoridad = new ObservableCollection<LIBERACION_AUTORIDAD>(new cLiberacionAutoridad().ObtenerTodos());
                LstLiberacionMotivo = new ObservableCollection<LIBERACION_MOTIVO>(new cLiberacionMotivo().ObtenerTodos());
                #endregion

                Opciones = new ObservableCollection<OpcionesArbol>();

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstPaises.Insert(0, new PAIS_NACIONALIDAD() { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                    LstEntidades.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                    LstMunicipios.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                    LstTiposOrden.Insert(0, new TIPO_ORDEN() { CP_TIPO_ORDEN = -1, DESCR = "SELECCIONE" });
                    LstTerminos.Insert(0, new TERMINO() { ID_TERMINO = -1, DESCR = "SELECCIONE" });
                    LstFueros.Insert(0, new FUERO() { ID_FUERO = string.Empty, DESCR = "SELECCIONE" });
                    LstAgencias.Insert(0, new AGENCIA() { ID_AGENCIA = -1, DESCR = "SELECCIONE" });
                    LstJuzgados.Insert(0, new JUZGADO() { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                    LstJuzgadoAmparo.Insert(0, new JUZGADO() { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                    LstCPEstatus.Insert(0, new CAUSA_PENAL_ESTATUS() { ID_ESTATUS_CP = -1, DESCR = "SELECCIONE" });
                    LstGradosAutoria.Insert(0, new GRADO_AUTORIA() { ID_GRADO_AUTORIA = -1, DESCR = "SELECCIONE" });
                    LstGradosParticipacion.Insert(0, new GRADO_PARTICIPACION() { ID_GRADO_PARTICIPACION = -1, DESCR = "SELECCIONE" });
                    LstTiposIngreso.Insert(0, new TIPO_INGRESO() { ID_TIPO_INGRESO = -1, DESCR = "SELECCIONE" });
                    LstEstatusAdministrativo.Insert(0, new ESTATUS_ADMINISTRATIVO() { ID_ESTATUS_ADMINISTRATIVO = -1, DESCR = "SELECCIONE" });
                    LstAutoridades.Insert(0, new TIPO_AUTORIDAD_INTERNA() { ID_AUTORIDAD_INTERNA = -1, DESCR = "SELECCIONE" });
                    LstTiposSeguridad.Insert(0, new TIPO_SEGURIDAD() { ID_TIPO_SEGURIDAD = string.Empty, DESCR = "SELECCIONE" });
                    LstAutoridadDisposicion.Insert(0, new TIPO_DISPOSICION() { ID_DISPOSICION = -1, DESCR = "SELECCIONE" });
                    LstClasificaciones.Insert(0, new CLASIFICACION_JURIDICA() { ID_CLASIFICACION_JURIDICA = string.Empty, DESCR = "SELECCIONE" });
                    LstAutoridadesInterna.Insert(0, new TIPO_AUTORIDAD_INTERNA() { ID_AUTORIDAD_INTERNA = -1, DESCR = "SELECCIONE" });
                    LstIngresoDelitos.Insert(0, new INGRESO_DELITO() { ID_INGRESO_DELITO = -1, DESCR = "SELECCIONE" });
                    LstTiposDelitos.Insert(0, new TIPO_DELITO() { ID_TIPO_DELITO = -1, DESCR = "SELECCIONE" });
                    LstTiposRecursos.Insert(0, new TIPO_RECURSO() { ID_TIPO_RECURSO = -3, DESCR = "SELECCIONE" });
                    LstTribunales.Insert(0, new JUZGADO() { ID_JUZGADO = -1, DESCR = "SELECCIONE" });
                    LstRecursoResultado.Insert(0, new RECURSO_RESULTADO() { RESULTADO = string.Empty, DESCR = "SELECCIONE" });
                    RResultadoRecurso = string.Empty;
                    LstSentenciaAD.Insert(0, new AMPARO_DIRECTO_SENTENCIA() { ID_SEN_AMP_RESULTADO = -1, DESCR = "SELECCIONE" });
                    LstSentenciaAI.Insert(0, new AMPARO_INDIRECTO_SENTENCIA() { ID_SEN_AMP_RESULTADO = -1, DESCR = "SELECCIONE" });
                    LstIncidentes.Insert(0, new TIPO_RECURSO() { ID_TIPO_RECURSO = -1, DESCR = "SELECCIONE" });
                    LstIncidenteResultado.Insert(0, new RECURSO_RESULTADO() { RESULTADO = string.Empty, DESCR = "SELECCIONE" });
                    LstAmparoIncidenteTipo.Insert(0, new AMPARO_INCIDENTE_TIPO() { ID_AMP_INC_TIPO = -1, DESCR = "SELECCIONE" });
                    
                    #region Baja
                    //LstMotivoBaja.Insert(0, new CAUSA_PENAL_BAJA() { ID_MOTIVO_BAJA = -1, DESCR = "SELECCIONE" });
                    //LstAutoridadBaja.Insert(0, new CAUSA_PENAL_AUTORIDAD_BAJA() { ID_AUTO_BAJA = -1, NOMBRE = "SELECCIONE" });
                    LstLiberacionAutoridad.Insert(0, new LIBERACION_AUTORIDAD() { ID_LIBERACION_AUTORIDAD = -1, DESCR = "SELECCIONE" });
                    LstLiberacionMotivo.Insert(0, new LIBERACION_MOTIVO() { ID_LIBERACION_MOTIVO = -1, DESCR = "SELECCIONE" });
                    #endregion

                    Opciones.Add(new OpcionesArbol { Descr = "Delitos Causa Penal" });
                    Opciones.Add(new OpcionesArbol { Descr = "Delitos Sentencia" });
                    Opciones.Add(new OpcionesArbol { Descr = "Partida Antecedentes" });

                    //SetIngreso();
                    TipoI = -1;
                    EstatusAdministrativoI = -1;
                    ClasificacionI = string.Empty;
                    AutoridadInternaI = -1;
                    TipoSeguridadI = string.Empty;
                    QuedaDisposicionI = -1;
                    TipoD = -1;
                    IngresoDelito = -1;
                    PaisJuzgadoCP = Parametro.PAIS;
                    EdificioI = -1;
                    TipoOrdenCP = -1;
                    TerminoCP = -1;
                    FueroCP = string.Empty;
                    AgenciaAP = -1;
                    EstatusCP = -1;
                    GradoAutoriaS = -1;
                    GradoParticipacionS = -1;
                    TipoR = -1;
                    PaternoD = MaternoD = NombreD = string.Empty;
                    ConfiguraPermisos();
                    //InicializaVariables();

                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información, favor de volver a ingresar a la pantalla", ex);
            }
        }
        #endregion

        //VALIDAR CAMBIO DE ESTATUS//////////////////////////////////////////////////////
        private bool ValidarCambioEstatus()
        {
            try
            {
                if (SelectedIngreso != null)
                {
                    if (SelectedIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                        {
                            if (SelectedCausaPenal == null)
                            {
                                if (EstatusCP == 1)
                                    if (cp.ID_ESTATUS_CP == 1)//ACTIVO
                                    {
                                        new Dialogos().ConfirmacionDialogo("Notificación", "Ya existe una causa penal activa");
                                        return false;
                                    }
                            }
                            else
                            {
                                if (SelectedCausaPenal.ID_CENTRO != cp.ID_CENTRO || SelectedCausaPenal.ID_ANIO != cp.ID_ANIO || SelectedCausaPenal.ID_IMPUTADO != cp.ID_IMPUTADO || SelectedCausaPenal.ID_INGRESO != cp.ID_INGRESO || SelectedCausaPenal.ID_CAUSA_PENAL != cp.ID_CAUSA_PENAL)
                                {
                                    if (EstatusCP == 1)
                                    {
                                        if (cp.ID_ESTATUS_CP == 1)//ACTIVO
                                        {
                                            new Dialogos().ConfirmacionDialogo("Notificación", "Ya existe una causa penal activa");
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (SelectedCausaPenal == null)
                    {
                        if (EstatusCP == 0)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación", "Favor de capturar sentencia para cambiar estatus a POR COMPURGAR");
                            return false;
                        }
                    }
                    else
                    {
                        if (EstatusCP == 0)
                        {
                            if (SelectedCausaPenal.SENTENCIA != null)
                            {
                                if (SelectedCausaPenal.SENTENCIA.Count == 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Notificación", "Favor de capturar sentencia para cambiar estatus a POR COMPURGAR");
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error validar cambio de estatus", ex);
            }
            return true;
        }

        private bool ValidacionEmpalmeFechas()
        {
            var resultado = true;
            try
            {
                EmpalmeDescr = string.Empty;
                LstFechasTraslape = new ObservableCollection<EmpalmeFechas>();
                if (SelectedSentencia == null)
                {
                    if (SelectedIngreso != null)
                    {
                        if (SelectedIngreso.CAUSA_PENAL != null)
                        {
                            foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                            {
                                if (cp.SENTENCIA != null)
                                {
                                    if (cp.SENTENCIA.Count > 0)
                                    {
                                        var s = cp.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                        if (s != null)
                                        {
                                            if (s.ID_INGRESO != SelectedIngreso.ID_INGRESO)
                                            {
                                                var fi = FecInicioCompurgacionS.Value.AddYears(AniosS != null ? AniosS.Value : 0);
                                                fi = fi.AddMonths(MesesS != null ? MesesS.Value : 0);
                                                fi = fi.AddDays(DiasS != null ? DiasS.Value : 0);
                                                //ABONOS
                                                fi = fi.AddYears(AniosAbonadosS != null ? (AniosAbonadosS.Value * -1) : 0);
                                                fi = fi.AddMonths(MesesAbonadosS != null ? (MesesAbonadosS.Value * -1) : 0);
                                                fi = fi.AddDays(DiasAbonadosS != null ? (DiasAbonadosS.Value * -1) : 0);

                                                var ff = s.FEC_INICIO_COMPURGACION.Value.AddYears(s.ANIOS != null ? s.ANIOS.Value : 0);
                                                ff = ff.AddMonths(s.MESES != null ? s.MESES.Value : 0);
                                                ff = ff.AddDays(s.DIAS != null ? s.DIAS.Value : 0);
                                                //ABONOS
                                                ff = ff.AddYears(s.ANIOS_ABONADOS != null ? (s.ANIOS_ABONADOS.Value * -1) : 0);
                                                ff = ff.AddMonths(s.MESES_ABONADOS != null ? (s.MESES_ABONADOS.Value * -1) : 0);
                                                ff = ff.AddDays(s.DIAS_ABONADOS != null ? (s.DIAS_ABONADOS.Value * -1) : 0);

                                                if ((FecInicioCompurgacionS >= s.FEC_INICIO_COMPURGACION && FecInicioCompurgacionS <= ff) || (fi >= s.FEC_INICIO_COMPURGACION && fi <= ff))
                                                {
                                                    if (string.IsNullOrEmpty(EmpalmeDescr))
                                                        EmpalmeDescr = string.Format("La fecha de inicio {0} y fecha termino {1} de compurgacion se empalman con las siguientes sentencias.", FecInicioCompurgacionS.Value.ToString("dd/MM/yyyy"), fi.ToString("dd/MM/yyyy"));
                                                    LstFechasTraslape.Add(new EmpalmeFechas()
                                                    {
                                                        NoIngreso = cp.INGRESO.ID_INGRESO,
                                                        CausaPenal = string.Format("{0}/{1}/{2}/{3}/{4}", cp.CP_ESTADO_JUZGADO != null ? cp.CP_ESTADO_JUZGADO.ToString() : string.Empty, cp.CP_MUNICIPIO_JUZGADO != null ? cp.CP_MUNICIPIO_JUZGADO.ToString() : string.Empty, cp.CP_JUZGADO != null ? cp.CP_JUZGADO.ToString() : string.Empty, cp.CP_ANIO, cp.CP_FOLIO),
                                                        FechaInicio = s.FEC_INICIO_COMPURGACION,
                                                        Anios = s.ANIOS,
                                                        Meses = s.MESES,
                                                        Dias = s.DIAS,
                                                        FechaFin = ff
                                                    });
                                                    resultado = false;
                                                }
                                            }
                                            //else
                                            //    resultado = false;
                                            }
                                    }
                                }
                            }
                        }
                    }
                }
                else//
                {
                    if (SelectedIngreso != null)
                    {
                        if (SelectedIngreso.CAUSA_PENAL != null)
                        {
                            foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                            {
                                if (cp.SENTENCIA != null)
                                {
                                    if (cp.SENTENCIA.Count > 0)
                                    {
                                        var s = cp.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                        if (s != null)
                                        {
                                            if (s.ID_INGRESO != SelectedIngreso.ID_INGRESO)
                                            {
                                                if (SelectedSentencia.ID_CENTRO != s.ID_CENTRO || SelectedSentencia.ID_ANIO != s.ID_ANIO || SelectedSentencia.ID_IMPUTADO != s.ID_IMPUTADO || SelectedSentencia.ID_INGRESO != s.ID_INGRESO || SelectedSentencia.ID_CAUSA_PENAL != s.ID_CAUSA_PENAL || SelectedSentencia.ID_SENTENCIA != s.ID_SENTENCIA)
                                                {
                                                    var fi = FecInicioCompurgacionS.Value.AddYears(AniosS != null ? AniosS.Value : 0);
                                                    fi = fi.AddMonths(MesesS != null ? MesesS.Value : 0);
                                                    fi = fi.AddDays(DiasS != null ? DiasS.Value : 0);
                                                    //ABONOS
                                                    fi = fi.AddYears(AniosAbonadosS != null ? (AniosAbonadosS.Value * -1) : 0);
                                                    fi = fi.AddMonths(MesesAbonadosS != null ? (MesesAbonadosS.Value * -1) : 0);
                                                    fi = fi.AddDays(DiasAbonadosS != null ? (DiasAbonadosS.Value * -1) : 0);

                                                    var ff = s.FEC_INICIO_COMPURGACION.Value.AddYears(s.ANIOS != null ? s.ANIOS.Value : 0);
                                                    ff = ff.AddMonths(s.MESES != null ? s.MESES.Value : 0);
                                                    ff = ff.AddDays(s.DIAS != null ? s.DIAS.Value : 0);
                                                    //ABONOS
                                                    ff = ff.AddYears(s.ANIOS_ABONADOS != null ? (s.ANIOS_ABONADOS.Value * -1) : 0);
                                                    ff = ff.AddMonths(s.MESES_ABONADOS != null ? (s.MESES_ABONADOS.Value * -1) : 0);
                                                    ff = ff.AddDays(s.DIAS_ABONADOS != null ? (s.DIAS_ABONADOS.Value * -1) : 0);

                                                    if (string.IsNullOrEmpty(EmpalmeDescr))
                                                        EmpalmeDescr = string.Format("La fecha de inició {0} y fecha terminó {1} de compurgacion se empalman con las siguientes sentencias.", FecInicioCompurgacionS.Value.ToString("dd/MM/yyyy"), fi.ToString("dd/MM/yyyy"));
                                                    if ((FecInicioCompurgacionS >= s.FEC_INICIO_COMPURGACION && FecInicioCompurgacionS <= ff) || (fi >= s.FEC_INICIO_COMPURGACION && fi <= ff))
                                                    {
                                                        LstFechasTraslape.Add(new EmpalmeFechas()
                                                        {
                                                            NoIngreso = cp.INGRESO.ID_INGRESO,
                                                            CausaPenal = string.Format("{0}/{1}/{2}/{3}/{4}", cp.CP_ESTADO_JUZGADO != null ? cp.CP_ESTADO_JUZGADO.ToString() : string.Empty, cp.CP_MUNICIPIO_JUZGADO != null ? cp.CP_MUNICIPIO_JUZGADO.ToString() : string.Empty, cp.CP_JUZGADO != null ? cp.CP_JUZGADO.ToString() : string.Empty, cp.CP_ANIO, cp.CP_FOLIO),
                                                            FechaInicio = s.FEC_INICIO_COMPURGACION,
                                                            Anios = s.ANIOS,
                                                            Meses = s.MESES,
                                                            Dias = s.DIAS,
                                                            FechaFin = ff
                                                        });
                                                        resultado = false;
                                                    }
                                                }
                                            }
                                        }
                                        //else
                                        //    resultado = false;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!resultado)
                {
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.EMPALME_FECHAS);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar empalme de fechas", ex);
                return false;
            }
            return resultado;
        }

        private void LimpiarBusqueda()
        {
            try
            {
                NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                AnioBuscar = FolioBuscar = null;
                var auxiliar = SelectIngreso;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                SelectExpediente = null;
                SelectIngreso = auxiliar;
                if (auxiliar.IMPUTADO != null)
                    SelectExpediente = auxiliar.IMPUTADO;
                //ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar búsqueda", ex);
            }
        }

        private bool ValidarSentenciaAbonos()
        {
            try
            {
                short aS = AniosS != null ? AniosS.Value : (short)0;
                short mS = MesesS != null ? MesesS.Value : (short)0;
                short dS = DiasS != null ? DiasS.Value : (short)0;
                var tAS = aS * 365;
                var tMS = mS * 30;


                var aAS = AniosAbonadosS != null ? AniosAbonadosS : 0;
                var mAS = MesesAbonadosS != null ? MesesAbonadosS : 0;
                var dAS = DiasAbonadosS != null ? DiasAbonadosS : 0;
                var tAAS = aAS * 365;
                var tMAS = mAS * 30;

                if ((tAAS + tMAS + dAS) > (tAS + tMS + dS))
                {
                    new Dialogos().ConfirmacionDialogo("Notificación", "El Total de abonos no puede ser mayor al tiempo de la sentencia.");
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar sentencia abonos", ex);
                return false;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////

        private bool ValidaEliminarCausaPenal()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    if (SelectedCausaPenal.CAUSA_PENAL_DELITO != null)
                        if (SelectedCausaPenal.CAUSA_PENAL_DELITO.Count > 0)
                            return false;
                    if (SelectedCausaPenal.COPARTICIPE != null)
                        if (SelectedCausaPenal.COPARTICIPE.Count > 0)
                            return false;
                    if (SelectedCausaPenal.SENTENCIA != null)
                        if (SelectedCausaPenal.SENTENCIA.Count > 0)
                            return false;
                    if (SelectedCausaPenal.RECURSO != null)
                        if (SelectedCausaPenal.RECURSO.Count > 0)
                            return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar eliminación de causa penal", ex);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        #region INTERCONEXION
        private void OnBuscarNUCInterconexion(string obj = "")
        {
            if (!PConsultar)
            {
                (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                return;
            }

            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
            var windowBusqueda = new BuscarNUC();
            windowBusqueda.DataContext = new BusquedaNUCVM();
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
                    SelectedInterconexion = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    if (!((BusquedaNUCVM)windowBusqueda.DataContext).IsSucceed)
                        return;

                    SelectedInterconexion = ((BusquedaNUCVM)windowBusqueda.DataContext).SelectedInterconexion;
                    if (SelectedInterconexion != null)
                    {
                        NUC = string.Format("NUC:{0}", SelectedInterconexion.EXPEDIENTEID.ToString());
                    }
                    else
                        NUC = string.Empty;

                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
                }
            };
            windowBusqueda.ShowDialog();
        }

        #endregion

        #region Amparos Indirectos
        private void LimpiarAmparoIndirecto()
        {
            try
            {
                AITipo = AIAutoridadInforma = AIAutoridadResuelve = AIResultadoSentencia = -1;
                AINoOficio = AINoAmparo = AINoOficioResuelve = AIActoReclamado = string.Empty;
                AIFechaDocumento = AIFechaNotificacion = AIFechaSuspencion = AIFechaDocumentoResuelve = AIFechaSentenciaResuelve = AIFechaEjecutoria = AIFechaRevision = null;
                LstAIT = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar amparo indirecto", ex);
            }
        }

        private void ObtenerTodoAmparoIndirecto(int tipo = 1)
        {
            if (SelectedIngreso != null)
            {
                //if (SelectedCausaPenal != null)
                //    LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>(new cAmparoIndirecto().ObtenerTodos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));  
                //else
                //    LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>(new cAmparoIndirecto().ObtenerTodos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, null)); 
                if (tipo == 1)
                    LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>(new cAmparoIndirecto().ObtenerTodos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, null));
                else
                {
                    if (SelectedCausaPenal != null)
                        LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>(new cAmparoIndirecto().ObtenerTodos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                }

            }
            else
                LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>();
            AmparoIndirectoEmpty = LstAmparoIndirecto.Count > 0 ? false : true;
        }

        private void ObtenerAmparoIndirecto()
        {
            if (SelectedAmparoIndirecto != null)
            {
                //AITipo = SelectedAmparoIndirecto.ID_AMP_IND_TIPO;
                AINoOficio = SelectedAmparoIndirecto.OFICIO_NUM;
                AIFechaDocumento = SelectedAmparoIndirecto.DOCUMENTO_FEC;
                AIFechaNotificacion = SelectedAmparoIndirecto.NOTIFICACION_FEC;
                AIFechaSuspencion = SelectedAmparoIndirecto.SUSPENSION_FEC;
                AIAutoridadInforma = SelectedAmparoIndirecto.SUSPENSION_AUT_INFORMA;
                //SEGUIMIENTO
                AINoAmparo = SelectedAmparoIndirecto.AMPARO_NUM;
                AIAutoridadResuelve = SelectedAmparoIndirecto.RESUELVE_AUTORIDAD != null ? SelectedAmparoIndirecto.RESUELVE_AUTORIDAD : -1;
                AINoOficioResuelve = SelectedAmparoIndirecto.RESUELVE_OFICIO_NUM;
                AIFechaDocumentoResuelve = SelectedAmparoIndirecto.RESUELVE_DOCUMENTO_FEC;
                AIFechaSentenciaResuelve = SelectedAmparoIndirecto.SENTENCIA_FEC;
                //AIResultadoSentencia = SelectedAmparoIndirecto.ID_SEN_AMP_RESULTADO != null ? SelectedAmparoIndirecto.ID_SEN_AMP_RESULTADO : -1;
                AIFechaEjecutoria = SelectedAmparoIndirecto.RESOLUCION_EJECUTORIA_FEC;
                AIFechaRevision = SelectedAmparoIndirecto.REVISION_RECURSO_FEC;
                AIActoReclamado = SelectedAmparoIndirecto.ACTO_RECLAMADO;

                LstAIT = new ObservableCollection<AMPARO_INDIRECTO_TIPOS>();
                if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_TIPOS != null)
                {
                    foreach (var obj in SelectedAmparoIndirecto.AMPARO_INDIRECTO_TIPOS)
                    {
                        LstAIT.Add(new AMPARO_INDIRECTO_TIPOS()
                        {
                            ID_CENTRO = obj.ID_CENTRO,
                            ID_ANIO = obj.ID_ANIO,
                            ID_IMPUTADO = obj.ID_IMPUTADO,
                            ID_INGRESO = obj.ID_INGRESO,
                            ID_AMP_IND_TIPO = obj.ID_AMP_IND_TIPO,
                            AMPARO_INDIRECTO_TIPO = obj.AMPARO_INDIRECTO_TIPO,
                            ID_SEN_AMP_RESULTADO = obj.ID_SEN_AMP_RESULTADO != null ? obj.ID_SEN_AMP_RESULTADO : -1
                        });
                    }
                }
            }
        }

        private void ObtenerTipoAmparoIndirecto(string param)
        {
            try
            {
                LstTipoAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO_TIPO>(new cAmparoIndirectoTipo().ObtenerTodos(string.Empty, param));
                LstTipoAmparoIndirecto.Insert(0, new AMPARO_INDIRECTO_TIPO() { ID_AMP_IND_TIPO = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar tipo de amparo indirecto", ex);
            }
        }

        private bool GuardarAmparoIndirecto()
        {
            try
            {
                if (SelectedIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        var obj = new AMPARO_INDIRECTO();
                        obj.ID_CENTRO = SelectedIngreso.ID_CENTRO;
                        obj.ID_ANIO = SelectedIngreso.ID_ANIO;
                        obj.ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO;
                        obj.ID_INGRESO = SelectedIngreso.ID_INGRESO;

                        obj.OFICIO_NUM = AINoOficio;
                        obj.DOCUMENTO_FEC = AIFechaDocumento;
                        obj.NOTIFICACION_FEC = AIFechaNotificacion;
                        obj.SUSPENSION_FEC = AIFechaSuspencion;
                        obj.SUSPENSION_AUT_INFORMA = AIAutoridadInforma;
                        //SEGUIMIENTO
                        obj.AMPARO_NUM = AINoAmparo;
                        obj.RESUELVE_AUTORIDAD = AIAutoridadResuelve != -1 ? AIAutoridadResuelve : null;
                        obj.RESUELVE_OFICIO_NUM = AINoOficioResuelve;
                        obj.RESUELVE_DOCUMENTO_FEC = AIFechaDocumentoResuelve;
                        obj.SENTENCIA_FEC = AIFechaSentenciaResuelve;
                        //obj.ID_SEN_AMP_RESULTADO = AIResultadoSentencia != -1 ? AIResultadoSentencia : null;
                        obj.RESOLUCION_EJECUTORIA_FEC = AIFechaEjecutoria;
                        obj.REVISION_RECURSO_FEC = AIFechaRevision;
                        obj.ACTO_RECLAMADO = AIActoReclamado;
                        if (EnCausaPenal)
                            if (SelectedCausaPenal != null)
                                obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        if (SelectedAmparoIndirecto == null) //INSERT
                        {
                            //Amparo Indirecto Tipo
                            var tipos = new List<AMPARO_INDIRECTO_TIPOS>(LstAIT == null ? null : LstAIT.Select(w => new AMPARO_INDIRECTO_TIPOS()
                            {
                                ID_CENTRO = SelectedIngreso.ID_CENTRO,
                                ID_ANIO = SelectedIngreso.ID_ANIO,
                                ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectedIngreso.ID_INGRESO,
                                ID_AMPARO_INDIRECTO = 0,
                                ID_AMP_IND_TIPO = w.ID_AMP_IND_TIPO,
                                ID_SEN_AMP_RESULTADO = w.ID_SEN_AMP_RESULTADO != -1 ? w.ID_SEN_AMP_RESULTADO : null
                            }));
                            obj.AMPARO_INDIRECTO_TIPOS = tipos;
                            obj.ID_AMPARO_INDIRECTO = new cAmparoIndirecto().Insertar(obj);
                            if (obj.ID_AMPARO_INDIRECTO > 0)
                            {
                                //if (GuardarAmparoIndirectoTipos(obj.ID_AMPARO_INDIRECTO))
                                return true;
                            }
                        }
                        else //UPDATE
                        {
                            obj.ID_AMPARO_INDIRECTO = SelectedAmparoIndirecto.ID_AMPARO_INDIRECTO;
                            //Amparo Indirecto Tipo
                            var tipos = new List<AMPARO_INDIRECTO_TIPOS>(LstAIT == null ? null : LstAIT.Select(w => new AMPARO_INDIRECTO_TIPOS()
                            {
                                ID_CENTRO = SelectedAmparoIndirecto.ID_CENTRO,
                                ID_ANIO = SelectedAmparoIndirecto.ID_ANIO,
                                ID_IMPUTADO = SelectedAmparoIndirecto.ID_IMPUTADO,
                                ID_INGRESO = SelectedAmparoIndirecto.ID_INGRESO,
                                ID_AMPARO_INDIRECTO = SelectedAmparoIndirecto.ID_AMPARO_INDIRECTO,
                                ID_AMP_IND_TIPO = w.ID_AMP_IND_TIPO,
                                ID_SEN_AMP_RESULTADO = w.ID_SEN_AMP_RESULTADO != -1 ? w.ID_SEN_AMP_RESULTADO : null
                            }));
                            if ((new cAmparoIndirecto()).Actualizar(obj, tipos))
                            {
                                //if (GuardarAmparoIndirectoTipos(obj.ID_AMPARO_INDIRECTO))
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar amparo indirecto", ex);
            }
            return false;
        }

        private bool GuardarAmparoIndirectoTipos(short Id)
        {
            try
            {
                List<AMPARO_INDIRECTO_TIPOS> list = new List<AMPARO_INDIRECTO_TIPOS>();
                foreach (var obj in LstAIT)
                {
                    list.Add(new AMPARO_INDIRECTO_TIPOS()
                    {
                        ID_CENTRO = SelectedIngreso.ID_CENTRO,
                        ID_ANIO = SelectedIngreso.ID_ANIO,
                        ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectedIngreso.ID_INGRESO,
                        ID_AMPARO_INDIRECTO = Id,
                        ID_AMP_IND_TIPO = obj.ID_AMP_IND_TIPO,
                        ID_SEN_AMP_RESULTADO = obj.ID_SEN_AMP_RESULTADO != -1 ? obj.ID_SEN_AMP_RESULTADO : null
                    });
                }
                if (new cAmparoIndirectoTipos().Insertar(list, SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, Id))
                    return true;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar amparo indirecto tipo", ex);
            }
            return false;
        }

        private async void EliminarAmparoIndirecto()
        {
            if (SelectedAmparoIndirecto != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Validación!", "Desea eliminar este amparo indirecto") == 1)
                {
                    if (new cAmparoIndirecto().Eliminar(SelectedAmparoIndirecto.ID_CENTRO, SelectedAmparoIndirecto.ID_ANIO, SelectedAmparoIndirecto.ID_IMPUTADO, SelectedAmparoIndirecto.ID_INGRESO, SelectedAmparoIndirecto.ID_AMPARO_INDIRECTO))
                    {
                        new Dialogos().ConfirmacionDialogo("Confirmación!", "El amparo indirecto ha sido eliminado correctamente.");
                        //var si = SelectedIngreso;
                        ViewModelArbol();
                        //SelectedIngreso = si;
                        ObtenerTodoAmparoIndirecto(EnCausaPenal ? 2 : 1);
                    }
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un amparo indirecto.");
        }

        private void AgregarAmparoIndirectoTipo()
        {
            if (SelectedTipoAmparoIndirecto != null)
            {
                if (SelectedTipoAmparoIndirecto.ID_AMP_IND_TIPO != -1)
                {
                    if (LstAIT == null)
                        LstAIT = new ObservableCollection<AMPARO_INDIRECTO_TIPOS>();

                    if (LstAIT.Where(w => w.ID_AMP_IND_TIPO == AITipo).Count() == 0)
                    {
                        LstAIT.Add(new AMPARO_INDIRECTO_TIPOS()
                        {
                            ID_CENTRO = SelectedIngreso.ID_CENTRO,
                            ID_ANIO = SelectedIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectedIngreso.ID_INGRESO,
                            ID_AMP_IND_TIPO = SelectedTipoAmparoIndirecto.ID_AMP_IND_TIPO,
                            AMPARO_INDIRECTO_TIPO = SelectedTipoAmparoIndirecto,
                            ID_SEN_AMP_RESULTADO = -1
                        });
                        AITipo = -1;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", "El tipo de amparo indirecto ya se encuentra en el listado.");
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un tipo de amparo indirecto.");
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un tipo de amparo indirecto.");
        }

        private bool EliminarAmparoIndirectoTipo()
        {
            if (SelectedAAmparoIndirectoTipos != null)
            {
                LstAIT.Remove(SelectedAAmparoIndirectoTipos);
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un tipo de amparo indirecto.");
            return false;
        }
        #endregion

        #region Amparos Directos
        private void LimpiarAmparoDirecto()
        {
            try
            {
                ADNoOficio = ADNoAmparo = ADNoOficioResuelve = string.Empty;
                ADFechaDocumento = ADFechaNotificacion = ADFechaSuspencion = ADFechaDocumentoResuelve = ADFechaSentenciaResuelve = null;
                ADAutoridadInforma = ADAutoridadNotifica = ADResultadoSentencia = ADAutoridadResuelve = -1;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar amparo directo", ex);
            }
        }

        private void ObtenerTodoAmparoDirecto()
        {
            LstAmparoDirecto = new ObservableCollection<AMPARO_DIRECTO>();
            if (SelectedCausaPenal != null)
                if (SelectedCausaPenal.AMPARO_DIRECTO != null)
                    LstAmparoDirecto = new ObservableCollection<AMPARO_DIRECTO>(SelectedCausaPenal.AMPARO_DIRECTO);
            AmparoDirectoEmpty = LstAmparoDirecto.Count > 0 ? false : true;
        }

        private void ObtenerAmparoDirecto()
        {
            if (SelectedAmparoDirecto != null)
            {
                ADNoOficio = SelectedAmparoDirecto.OFICIO_NUM;
                ADFechaDocumento = SelectedAmparoDirecto.DOCUMENTO_FEC;
                ADFechaNotificacion = SelectedAmparoDirecto.NOTIFICACION_FEC;
                ADFechaSuspencion = SelectedAmparoDirecto.SUSPENSION_FEC;
                ADAutoridadInforma = SelectedAmparoDirecto.SUSPENSION_AUT_INFORMA;
                //SEGIUIMIENTO
                ADNoAmparo = SelectedAmparoDirecto.AMPARO_NUM;
                ADAutoridadNotifica = SelectedAmparoDirecto.AUTORIDAD_NOTIFICA != null ? SelectedAmparoDirecto.AUTORIDAD_NOTIFICA : -1;
                ADNoOficioResuelve = SelectedAmparoDirecto.RESUELVE_OFICIO_NUM;
                ADFechaDocumentoResuelve = SelectedAmparoDirecto.RESUELVE_DOCUMENTO_FEC;
                ADFechaSentenciaResuelve = SelectedAmparoDirecto.RESUELVE_SENTENCIA_FEC;
                ADResultadoSentencia = SelectedAmparoDirecto.ID_SEN_AMP_RESULTADO != null ? SelectedAmparoDirecto.ID_SEN_AMP_RESULTADO : -1;
                ADAutoridadResuelve = SelectedAmparoDirecto.AUTORIDAD_PRONUNCIA_SENTENCIA != null ? SelectedAmparoDirecto.AUTORIDAD_PRONUNCIA_SENTENCIA : -1;
            }
        }

        private bool GuardarAmparoDirecto()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    if (!base.HasErrors)
                    {
                        var obj = new AMPARO_DIRECTO();
                        obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                        obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                        obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                        obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                        obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        obj.OFICIO_NUM = ADNoOficio;
                        obj.DOCUMENTO_FEC = ADFechaDocumento;
                        obj.NOTIFICACION_FEC = ADFechaNotificacion;
                        obj.SUSPENSION_FEC = ADFechaSuspencion;
                        obj.SUSPENSION_AUT_INFORMA = ADAutoridadInforma;
                        //SEGUIMIENTO
                        obj.AMPARO_NUM = ADNoAmparo;
                        obj.AUTORIDAD_NOTIFICA = ADAutoridadNotifica != -1 ? ADAutoridadNotifica : null;
                        obj.RESUELVE_OFICIO_NUM = ADNoOficioResuelve;
                        obj.RESUELVE_DOCUMENTO_FEC = ADFechaDocumentoResuelve;
                        obj.RESUELVE_SENTENCIA_FEC = ADFechaSentenciaResuelve;
                        obj.ID_SEN_AMP_RESULTADO = ADResultadoSentencia != -1 ? ADResultadoSentencia : null;
                        obj.AUTORIDAD_PRONUNCIA_SENTENCIA = ADAutoridadResuelve != -1 ? ADAutoridadResuelve : null;

                        if (SelectedAmparoDirecto == null) //INSERT
                        {
                            obj.ID_AMPARO_DIRECTO = new cAmparoDirecto().Insertar(obj);
                            if (obj.ID_AMPARO_DIRECTO > 0)
                            {
                                if (obj.ID_SEN_AMP_RESULTADO == 3)//ORDENA REPOSICION DEL PROCEDIMIENTO
                                {
                                    if (ReposicionDeProcedimiento())
                                        return true;
                                }
                                else
                                    return true;
                            }
                        }
                        else //UPDATE
                        {
                            obj.ID_AMPARO_DIRECTO = SelectedAmparoDirecto.ID_AMPARO_DIRECTO;
                            if ((new cAmparoDirecto()).Actualizar(obj))
                            {
                                if (obj.ID_SEN_AMP_RESULTADO == 3)//ORDENA REPOSICION DEL PROCEDIMIENTO
                                    if (ReposicionDeProcedimiento())
                                        return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar amparo directo", ex);
            }
            return false;
        }

        private bool GuardarAmparoDirectoTransaccion()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    if (!base.HasErrors)
                    {
                        var obj = new AMPARO_DIRECTO();
                        obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                        obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                        obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                        obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                        obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        obj.OFICIO_NUM = ADNoOficio;
                        obj.DOCUMENTO_FEC = ADFechaDocumento;
                        obj.NOTIFICACION_FEC = ADFechaNotificacion;
                        obj.SUSPENSION_FEC = ADFechaSuspencion;
                        obj.SUSPENSION_AUT_INFORMA = ADAutoridadInforma;
                        //SEGUIMIENTO
                        obj.AMPARO_NUM = ADNoAmparo;
                        obj.AUTORIDAD_NOTIFICA = ADAutoridadNotifica != -1 ? ADAutoridadNotifica : null;
                        obj.RESUELVE_OFICIO_NUM = ADNoOficioResuelve;
                        obj.RESUELVE_DOCUMENTO_FEC = ADFechaDocumentoResuelve;
                        obj.RESUELVE_SENTENCIA_FEC = ADFechaSentenciaResuelve;
                        obj.ID_SEN_AMP_RESULTADO = ADResultadoSentencia != -1 ? ADResultadoSentencia : null;
                        obj.AUTORIDAD_PRONUNCIA_SENTENCIA = ADAutoridadResuelve != -1 ? ADAutoridadResuelve : null;

                        if (SelectedAmparoDirecto == null) //INSERT
                        {
                            var resultado = new cAmparoDirecto().InsertarTransaccion(obj);
                            if (resultado == 3)
                            {
                                SelectedSentencia = null;
                                LimpiarSentencia();
                                //CAUSAPENAL
                                SelectedCausaPenal = new cCausaPenal().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_INGRESO, obj.ID_CAUSA_PENAL).FirstOrDefault();
                            }
                            return true;
                        }
                        else //UPDATE
                        {
                            obj.ID_AMPARO_DIRECTO = SelectedAmparoDirecto.ID_AMPARO_DIRECTO;
                            var resultado = new cAmparoDirecto().ActualizarTransaccion(obj);
                            if (resultado == 3)
                            {
                                SelectedSentencia = null;
                                LimpiarSentencia();
                                //CAUSAPENAL
                                SelectedCausaPenal = new cCausaPenal().Obtener(obj.ID_CENTRO, obj.ID_ANIO, obj.ID_IMPUTADO, obj.ID_INGRESO, obj.ID_CAUSA_PENAL).FirstOrDefault();
                            }
                            return true;
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "Favor de capturar los campos obligatorios.");
                        return false;
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Éxito", "Favor de seleccionar una causa penal.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar amparo directo", ex);
            }
            return false;
        }

        private async void EliminarAmparoDirecto()
        {
            if (SelectedAmparoDirecto != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Validación!", "Desea eliminar este amparo directo") == 1)
                {
                    if (new cAmparoDirecto().Eliminar(SelectedAmparoDirecto.ID_CENTRO, SelectedAmparoDirecto.ID_ANIO, SelectedAmparoDirecto.ID_IMPUTADO, SelectedAmparoDirecto.ID_INGRESO, SelectedAmparoDirecto.ID_CAUSA_PENAL, SelectedAmparoDirecto.ID_AMPARO_DIRECTO))
                    {
                        new Dialogos().ConfirmacionDialogo("Confirmación!", "El amparo directo ha sido eliminado correctamente.");
                        ViewModelArbol();
                        ObtenerTodoAmparoDirecto();
                    }
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación!", "Favor de seleccionar un amparo directo.");
        }

        private bool ReposicionDeProcedimiento()
        {
            if (SelectedCausaPenal != null)
            {
                var cp = SelectedCausaPenal;
                var obj = new CAUSA_PENAL();
                obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                #region datos
                //obj.AP_ANIO = SelectedCausaPenal.AP_ANIO;
                //obj.AP_FOLIO = SelectedCausaPenal.AP_FOLIO;
                //obj.AP_FORANEA = SelectedCausaPenal.AP_FORANEA;
                //obj.AP_FEC_INICIO = SelectedCausaPenal.AP_FEC_INICIO;
                //obj.AP_FEC_CONSIGNACION = SelectedCausaPenal.AP_FEC_CONSIGNACION;
                //obj.CP_ANIO = SelectedCausaPenal.CP_ANIO;
                //obj.CP_FOLIO = SelectedCausaPenal.CP_FOLIO;
                //obj.CP_BIS = SelectedCausaPenal.CP_BIS;
                //obj.CP_FORANEO = SelectedCausaPenal.CP_FORANEO;
                //obj.CP_TIPO_ORDEN = SelectedCausaPenal.CP_TIPO_ORDEN;
                //obj.CP_PAIS_JUZGADO = SelectedCausaPenal.CP_PAIS_JUZGADO;
                //obj.CP_ESTADO_JUZGADO = SelectedCausaPenal.CP_ESTADO_JUZGADO;
                //obj.CP_MUNICIPIO_JUZGADO = SelectedCausaPenal.CP_MUNICIPIO_JUZGADO;
                //obj.CP_FUERO = SelectedCausaPenal.CP_FUERO;
                //obj.CP_JUZGADO = SelectedCausaPenal.CP_JUZGADO;
                //obj.CP_FEC_RADICACION = SelectedCausaPenal.CP_FEC_RADICACION;
                //obj.CP_AMPLIACION = SelectedCausaPenal.CP_AMPLIACION;
                //obj.CP_FEC_VENCIMIENTO_TERMINO = SelectedCausaPenal.CP_FEC_VENCIMIENTO_TERMINO;
                //obj.CP_TERMINO = SelectedCausaPenal.CP_TERMINO;
                //obj.ID_AGENCIA = SelectedCausaPenal.ID_AGENCIA;
                //obj.ID_ENTIDAD= SelectedCausaPenal.ID_ENTIDAD;
                //obj.ID_MUNICIPIO = SelectedCausaPenal.ID_MUNICIPIO;
                //obj.OBSERV = SelectedCausaPenal.OBSERV;
                #endregion
                //estatus
                obj.ID_ESTATUS_CP = 6;//EN PROCESO
                if (new cCausaPenal().ActualizarEstatusCausaPenal(obj))
                {
                    if (SelectedSentencia != null)
                    {
                        var sen = new SENTENCIA();
                        sen.ID_CENTRO = SelectedSentencia.ID_CENTRO;
                        sen.ID_ANIO = SelectedSentencia.ID_ANIO;
                        sen.ID_IMPUTADO = SelectedSentencia.ID_IMPUTADO;
                        sen.ID_INGRESO = SelectedSentencia.ID_INGRESO;
                        sen.ID_CAUSA_PENAL = SelectedSentencia.ID_CAUSA_PENAL;
                        sen.ID_SENTENCIA = SelectedSentencia.ID_SENTENCIA;
                        sen.FEC_SENTENCIA = SelectedSentencia.FEC_SENTENCIA;
                        #region datos
                        //sen.FEC_EJECUTORIA = SelectedSentencia.FEC_EJECUTORIA;
                        //sen.FEC_INICIO_COMPURGACION = SelectedSentencia.FEC_INICIO_COMPURGACION;
                        //sen.ANIOS = SelectedSentencia.ANIOS;
                        //sen.MESES = SelectedSentencia.MESES;
                        //sen.DIAS = SelectedSentencia.DIAS;
                        //sen.MULTA = SelectedSentencia.MULTA;
                        //sen.MULTA_PAGADA = SelectedSentencia.MULTA_PAGADA;
                        //sen.REPARACION_DANIO = SelectedSentencia.REPARACION_DANIO;
                        //sen.REPARACION_DANIO_PAGADA = SelectedSentencia.REPARACION_DANIO_PAGADA;
                        //sen.SUSTITUCION_PENA = SelectedSentencia.SUSTITUCION_PENA;
                        //sen.SUSTITUCION_PENA_PAGADA = SelectedSentencia.SUSTITUCION_PENA_PAGADA;
                        //sen.SUSPENSION_CONDICIONAL = SelectedSentencia.SUSPENSION_CONDICIONAL;
                        //sen.OBSERVACION = SelectedSentencia.OBSERVACION;
                        //sen.MOTIVO_CANCELACION_ANTECEDENTE = SelectedSentencia.MOTIVO_CANCELACION_ANTECEDENTE;
                        //sen.ID_GRADO_PARTICIPACION = SelectedSentencia.ID_GRADO_PARTICIPACION;
                        //sen.ID_GRADO_AUTORIA = SelectedSentencia.ID_GRADO_AUTORIA;
                        //sen.ANIOS_ABONADOS = SelectedSentencia.ANIOS_ABONADOS;
                        //sen.MESES_ABONADOS = SelectedSentencia.MESES_ABONADOS;
                        //sen.DIAS_ABONADOS = SelectedSentencia.DIAS_ABONADOS;
                        //sen.FEC_REAL_COMPURGACION = SelectedSentencia.FEC_REAL_COMPURGACION;
                        #endregion
                        sen.ESTATUS = "I";//INACTIVO
                        if (new cSentencia().ActualizarEstatusSentencia(sen))
                        {
                            SelectedSentencia = null;
                            LimpiarSentencia();
                            //CAUSAPENAL
                            SelectedCausaPenal = new cCausaPenal().Obtener(cp.ID_CENTRO, cp.ID_ANIO, cp.ID_IMPUTADO, cp.ID_INGRESO, cp.ID_CAUSA_PENAL).FirstOrDefault();
                            return true;
                        }
                    }
                    else
                    {
                        //CAUSAPENAL
                        SelectedCausaPenal = new cCausaPenal().Obtener(cp.ID_CENTRO, cp.ID_ANIO, cp.ID_IMPUTADO, cp.ID_INGRESO, cp.ID_CAUSA_PENAL).FirstOrDefault();
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Incidentes
        private void LimpiarIncidente()
        {
            try
            {
                ITipo = IAutoridadInforma = -1;
                IResultado = INoOficio = string.Empty;
                IDiasRemision = IGarantia = null;
                IFechaDocumento = null;
                IModificaPenaAnios = IModificaPenaMeses = IModificaPenaDias = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar incidentes", ex);
            }
        }

        private void ObtenerTodoIncidente()
        {
            LstAmparoIncidente = new ObservableCollection<AMPARO_INCIDENTE>();
            if (SelectedCausaPenal != null)
                if (SelectedCausaPenal.AMPARO_INCIDENTE != null)
                    LstAmparoIncidente = new ObservableCollection<AMPARO_INCIDENTE>(SelectedCausaPenal.AMPARO_INCIDENTE);
            AmparoIncidenteEmpty = LstAmparoIncidente.Count > 0 ? false : true;
        }

        private void ObtenerIncidente()
        {
            if (SelectedAmparoIncidente != null)
            {
                ITipo = SelectedAmparoIncidente.ID_AMP_INC_TIPO;
                IResultado = SelectedAmparoIncidente.RESULTADO;
                IDiasRemision = SelectedAmparoIncidente.DIAS_REMISION;
                IFechaDocumento = SelectedAmparoIncidente.DOCUMENTO_FEC;
                INoOficio = SelectedAmparoIncidente.OFICIO_NUM;
                IAutoridadInforma = SelectedAmparoIncidente.AUTORIDAD_NOTIFICA;
                IGarantia = SelectedAmparoIncidente.GARANTIA;
                IModificaPenaAnios = SelectedAmparoIncidente.MODIFICA_PENA_ANIO;
                IModificaPenaMeses = SelectedAmparoIncidente.MODIFICA_PENA_MES;
                IModificaPenaDias = SelectedAmparoIncidente.MODIFICA_PENA_DIA;
            }
        }

        private bool GuardarIncidente()
        {
            if (SelectedCausaPenal != null)
            {
                var obj = new AMPARO_INCIDENTE();
                obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                obj.ID_AMP_INC_TIPO = ITipo;
                obj.RESULTADO = IResultado;

                obj.DOCUMENTO_FEC = IFechaDocumento;
                obj.OFICIO_NUM = INoOficio;
                obj.AUTORIDAD_NOTIFICA = IAutoridadInforma;

                if (obj.RESULTADO == "C")
                {
                    obj.DIAS_REMISION = IDiasRemision;
                    obj.GARANTIA = IGarantia;
                }
                else
                    if (obj.RESULTADO == "M")
                    {
                        obj.MODIFICA_PENA_ANIO = IModificaPenaAnios;
                        obj.MODIFICA_PENA_MES = IModificaPenaMeses;
                        obj.MODIFICA_PENA_DIA = IModificaPenaDias;
                    }


                if (SelectedAmparoIncidente == null) //INSERT
                {
                    obj.ID_AMPARO_INCIDENTE = new cAmparoIncidente().Insertar(obj);
                    if (obj.ID_AMPARO_INCIDENTE > 0)
                    {
                        return true;
                    }
                }
                else //UPDATE
                {
                    obj.ID_AMPARO_INCIDENTE = SelectedAmparoIncidente.ID_AMPARO_INCIDENTE;
                    if ((new cAmparoIncidente()).Actualizar(obj))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async void EliminarIncidente()
        {
            if (SelectedAmparoIncidente != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Validacion!", "Desea eliminar este incidente") == 1)
                {
                    if (new cAmparoIncidente().Eliminar(SelectedAmparoIncidente.ID_CENTRO, SelectedAmparoIncidente.ID_ANIO, SelectedAmparoIncidente.ID_IMPUTADO, SelectedAmparoIncidente.ID_INGRESO, SelectedAmparoIncidente.ID_CAUSA_PENAL, SelectedAmparoIncidente.ID_AMPARO_INCIDENTE))
                    {
                        new Dialogos().ConfirmacionDialogo("Confirmacion!", "El incidente ha sido eliminado correctamente.");
                        ViewModelArbol();
                        ObtenerTodoIncidente();
                    }
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un incidente.");
        }
        #endregion

        #region Recursos
        private void LimpiarRecurso()
        {
            RTipoRecurso = -3;
            RTribunal = -1;
            RFuero = RTocaPenal = RNoOficio = RMulta = RReparacionDanio = RSustitucionPena = RMultaCondicional = RResultadoRecurso = string.Empty;
            RResolucion = string.Empty;
            OnPropertyChanged("RResolucion");
            RFechaRecurso = RFechaResolucion = null;
            RAnio = RMeses = RDias = null;
        }

        private void ObtenerTodoRecurso()
        {
            LstRecursos = new ObservableCollection<RECURSO>();
            if (SelectedCausaPenal != null)
                if (SelectedCausaPenal.RECURSO != null)
                    LstRecursos = new ObservableCollection<RECURSO>(SelectedCausaPenal.RECURSO);
            RecursoEmpty = LstRecursos.Count > 0 ? false : true;
        }

        private void ObtenerRecurso()
        {
            if (SelectedRecurso != null)
            {
                RTipoRecurso = SelectedRecurso.ID_TIPO_RECURSO;
                RTribunal = SelectedRecurso.ID_TRIBUNAL;
                RResultadoRecurso = SelectedRecurso.RESULTADO;
                RFechaRecurso = SelectedRecurso.FEC_RECURSO;
                RTocaPenal = SelectedRecurso.TOCA_PENAL;
                RNoOficio = SelectedRecurso.NO_OFICIO;
                RResolucion = SelectedRecurso.RESOLUCION;
                RFechaResolucion = SelectedRecurso.FEC_RESOLUCION;
                RMulta = SelectedRecurso.MULTA;
                RReparacionDanio = SelectedRecurso.REPARACION_DANIO;
                RSustitucionPena = SelectedRecurso.SUSTITUCION_PENA;
                RMultaCondicional = SelectedRecurso.MULTA_CONDICIONAL;
                RAnio = SelectedRecurso.SENTENCIA_ANIOS;
                RMeses = SelectedRecurso.SENTENCIA_MESES;
                RDias = SelectedRecurso.SENTENCIA_DIAS;
                //DELITOS
                PopulateDelitoRecusrso();
            }
        }

        private bool GuardarRecurso()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    if (!base.HasErrors)
                    {
                        var r = new RECURSO();
                        r.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                        r.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                        r.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                        r.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                        r.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        r.ID_TIPO_RECURSO = RTipoRecurso;
                        r.ID_TRIBUNAL = RTribunal;
                        r.ID_TIPO_RECURSO = RTipoRecurso; //SelectedRecursoResultado.ID_TIPO_RECURSO;
                        r.RESULTADO = RResultadoRecurso;// SelectedRecursoResultado.RESULTADO;
                        r.FEC_RECURSO = RFechaRecurso;
                        r.FEC_RESOLUCION = RFechaResolucion;
                        r.FUERO = SelectedTribunal.ID_FUERO;//SelectedJuzgado.ID_FUERO;
                        r.TOCA_PENAL = RTocaPenal;
                        r.NO_OFICIO = RNoOficio;
                        r.RESOLUCION = RResolucion;

                        r.MULTA = RMulta;
                        r.MULTA_CONDICIONAL = RMultaCondicional;
                        r.REPARACION_DANIO = RReparacionDanio;
                        r.SUSTITUCION_PENA = RSustitucionPena;

                        r.SENTENCIA_ANIOS = RAnio;
                        r.SENTENCIA_MESES = RMeses;
                        r.SENTENCIA_DIAS = RDias;

                        if (SelectedRecurso == null) //INSERT
                        {
                            //Delitos
                            var delitos = new List<RECURSO_DELITO>(LstRecursoDelitos == null ? null : LstRecursoDelitos.Select((w, i) => new RECURSO_DELITO()
                            {
                                ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                ID_RECURSO = 0,
                                ID_DELITO = w.ID_DELITO,
                                ID_FUERO = w.ID_FUERO,
                                ID_MODALIDAD = w.ID_MODALIDAD,
                                ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                                CANTIDAD = w.CANTIDAD,
                                OBJETO = w.OBJETO,
                                DESCR_DELITO = w.DESCR_DELITO,
                                ID_CONS = Convert.ToInt16(i + 1)
                            }));
                            r.RECURSO_DELITO = delitos;
                            if (new cRecurso().Insertar(r) > 0)
                            {

                                LstRecursos = new ObservableCollection<RECURSO>(new cRecurso().ObtenerTodos(
                                    SelectedCausaPenal.ID_CENTRO,
                                    SelectedCausaPenal.ID_ANIO,
                                    SelectedCausaPenal.ID_IMPUTADO,
                                    SelectedCausaPenal.ID_INGRESO,
                                    SelectedCausaPenal.ID_CAUSA_PENAL));

                                RecursoEmpty = LstRecursos.Count > 0 ? false : true;
                                //if (GuardarDelitoRecurso(r.ID_RECURSO))
                                return true;
                                //else
                                //    return false;
                            }
                        }
                        else //UPDATE
                        {
                            r.ID_RECURSO = SelectedRecurso.ID_RECURSO;
                            //Delitos
                            var delitos = new List<RECURSO_DELITO>(LstRecursoDelitos == null ? null : LstRecursoDelitos.Select((w, i) => new RECURSO_DELITO()
                            {
                                ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                ID_RECURSO = SelectedRecurso.ID_RECURSO,
                                ID_DELITO = w.ID_DELITO,
                                ID_FUERO = w.ID_FUERO,
                                ID_MODALIDAD = w.ID_MODALIDAD,
                                ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                                CANTIDAD = w.CANTIDAD,
                                OBJETO = w.OBJETO,
                                DESCR_DELITO = w.DESCR_DELITO,
                                ID_CONS = Convert.ToInt16(i + 1)
                            }));
                            if ((new cRecurso()).Actualizar(r, delitos))
                            {
                                LstRecursos = new ObservableCollection<RECURSO>(new cRecurso().ObtenerTodos(
                                    SelectedCausaPenal.ID_CENTRO,
                                    SelectedCausaPenal.ID_ANIO,
                                    SelectedCausaPenal.ID_IMPUTADO,
                                    SelectedCausaPenal.ID_INGRESO,
                                    SelectedCausaPenal.ID_CAUSA_PENAL));
                                
                                //if (GuardarDelitoRecurso(r.ID_RECURSO))
                                return true;
                                //else
                                //    return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar recurso", ex);
            }
            return false;
        }

        //private void EliminarRecurso() 
        //{
        //}
        #endregion

        #region Digitalizacion
        private void ObtenerTipoDocumento(short tipo)
        {
            try
            {
                DocumentoDigitalizado = null;
                ObservacionDocumento = string.Empty;
                DatePickCapturaDocumento = Fechas.GetFechaDateServer;
                ListTipoDocumento = new ObservableCollection<TipoDocumento>();
                switch (tipo)
                {
                    case 1://AMPARO INDIRECTO
                        if (SelectedAmparoIndirecto != null)
                        {
                            if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO != null)
                            {
                                if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.Count > 0)
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 1, DESCR = "AMPARO INDIRECTO", DIGITALIZADO = true });
                                else
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 1, DESCR = "AMPARO INDIRECTO", DIGITALIZADO = false });
                            }
                            else
                                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 1, DESCR = "AMPARO INDIRECTO", DIGITALIZADO = false });
                        }
                        else
                            ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 1, DESCR = "AMPARO INDIRECTO", DIGITALIZADO = false });
                        break;
                    case 2://AMPARO DIRECTO
                        if (SelectedAmparoDirecto != null)
                        {
                            if (SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO != null)
                            {
                                if (SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.Count > 0)
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 2, DESCR = "AMPARO DIRECTO", DIGITALIZADO = true });
                                else
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 2, DESCR = "AMPARO DIRECTO", DIGITALIZADO = false });
                            }
                            else
                                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 2, DESCR = "AMPARO DIRECTO", DIGITALIZADO = false });
                        }
                        else
                            ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 2, DESCR = "AMPARO DIRECTO", DIGITALIZADO = false });
                        break;
                    case 3://INCIDENTE
                        if (SelectedAmparoIncidente != null)
                        {
                            if (SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO != null)
                            {
                                if (SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.Count > 0)
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 3, DESCR = "INCIDENTE", DIGITALIZADO = true });
                                else
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 3, DESCR = "INCIDENTE", DIGITALIZADO = false });
                            }
                            else
                                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 3, DESCR = "INCIDENTE", DIGITALIZADO = false });
                        }
                        else
                            ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 3, DESCR = "INCIDENTE", DIGITALIZADO = false });
                        break;
                    case 4://RECURSO
                        if (SelectedRecurso != null)
                        {
                            if (SelectedRecurso.RECURSO_DOCTO != null)
                            {
                                if (SelectedRecurso.RECURSO_DOCTO.Count > 0)
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 4, DESCR = "RECURSO", DIGITALIZADO = true });
                                else
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 4, DESCR = "RECURSO", DIGITALIZADO = false });
                            }
                            else
                                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 4, DESCR = "RECURSO", DIGITALIZADO = false });
                        }
                        else
                            ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 4, DESCR = "RECURSO", DIGITALIZADO = false });
                        break;
                    case 5://CAUSA PENAL
                        if (SelectedCausaPenal != null)
                        {
                            if (SelectedCausaPenal.CAUSA_PENAL_DOCTO != null)
                            {
                                if (SelectedCausaPenal.CAUSA_PENAL_DOCTO.Count > 0)
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 5, DESCR = "CAUSA PENAL", DIGITALIZADO = true });
                                else
                                    ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 5, DESCR = "CAUSA PENAL", DIGITALIZADO = false });
                            }
                            else
                                ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 5, DESCR = "CAUSA PENAL", DIGITALIZADO = false });
                        }
                        else
                            ListTipoDocumento.Add(new TipoDocumento() { ID_TIPO_DOCUMENTO = 5, DESCR = "CAUSA PENAL", DIGITALIZADO = false });
                        break;
                }

                SelectedTipoDocumento = ListTipoDocumento[0];
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los tipos de documento.", ex);
            }

        }
        private async void Scan(PdfViewer obj)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    await escaner.Scann(Duplex, SelectedSource, obj);
                    
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al escanear el documento.", ex);
            }
        }
        private async void AbrirDocumento(PdfViewer obj)
        {
            try
            {
                if (SelectedTipoDocumento == null)
                {
                    escaner.Hide();
                    await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                    return;
                }

                switch (SelectedTipoDocumento.ID_TIPO_DOCUMENTO)
                {
                    case 1://AMPARO INDIRECTO
                        if (SelectedAmparoIndirecto == null)
                            return;

                        if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO != null && SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.Any())
                        {
                            var dAI = SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.FirstOrDefault();
                            if (dAI != null)
                            {
                                DocumentoDigitalizado = dAI.DOCUMENTO;
                                ObservacionDocumento = dAI.DESCR;
                                DatePickCapturaDocumento = dAI.DIGITALIZACION_FEC;
                            }
                        }
                        break;
                    case 2://AMPARO DIRECTO
                        if (SelectedAmparoDirecto == null)
                            return;

                        if (SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO != null && SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.Any())
                        {
                            var dAD = SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.FirstOrDefault();
                            if (dAD != null)
                            {
                                DocumentoDigitalizado = dAD.DOCUMENTO;
                                ObservacionDocumento = dAD.DESCR;
                                DatePickCapturaDocumento = dAD.DIGITALIZACION_FEC;
                            }
                        }

                        break;
                    case 3://INCIDENTE
                        if (SelectedAmparoIncidente == null)
                            return;

                        if (SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO != null && SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.Any())
                        {
                            if (SelectedAmparoIncidente == null)
                                return;

                            var dI = SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.FirstOrDefault();
                            if (dI != null)
                            {
                                DocumentoDigitalizado = dI.DOCUMENTO;
                                ObservacionDocumento = dI.DESCR;
                                DatePickCapturaDocumento = dI.DIGITALIZACION_FEC;
                            }
                        }
                        break;
                    case 4://RECURSO
                        if (SelectedRecurso == null)
                            return;

                        var dR = SelectedRecurso.RECURSO_DOCTO.FirstOrDefault();
                        if (dR != null)
                        {
                            DocumentoDigitalizado = dR.DOCUMENTO;
                            ObservacionDocumento = dR.DESCR;
                            DatePickCapturaDocumento = dR.DIGITALIZACION_FEC;
                        }
                        break;
                    case 5://CAUSA PENAL
                        if (SelectedCausaPenal == null)
                            return;

                        var dCP = SelectedCausaPenal.CAUSA_PENAL_DOCTO.FirstOrDefault();
                        if (dCP != null)
                        {
                            DocumentoDigitalizado = dCP.DOCUMENTO;
                            ObservacionDocumento = dCP.DESCR;
                            DatePickCapturaDocumento = dCP.DIGITALIZACION_FEC;
                        }
                        break;
                }

                if (DocumentoDigitalizado == null)
                    return;
                await Task.Factory.StartNew(() =>
                {
                    var fileNamepdf = Path.GetTempPath() + Path.GetRandomFileName().Split('.')[0] + ".pdf";
                    File.WriteAllBytes(fileNamepdf, DocumentoDigitalizado);
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        obj.LoadFile(fileNamepdf);
                        obj.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al abrir documento.", ex);
            }
        }
        private void GuardarDocumento()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((System.Action)(async delegate
                {
                    escaner.Hide();
                    if (SelectedTipoDocumento == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Elija El Tipo De Documento A Digitalizar");
                        return;
                    }
                    if (DocumentoDigitalizado == null)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }
                    if (DocumentoDigitalizado.Length <= 0)
                    {
                        await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Digitalice Un Documento Para Guardar");
                        return;
                    }

                    switch (SelectedTipoDocumento.ID_TIPO_DOCUMENTO)
                    {
                        case 1://AMPARO INDIRECTO
                            if (SelectedAmparoIndirecto == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar un amparo indirecto.");
                                return;
                            }
                            else
                            {
                                var obj = new AMPARO_INDIRECTO_DOCTO();
                                obj.ID_CENTRO = SelectedAmparoIndirecto.ID_CENTRO;
                                obj.ID_ANIO = SelectedAmparoIndirecto.ID_ANIO;
                                obj.ID_IMPUTADO = SelectedAmparoIndirecto.ID_IMPUTADO;
                                obj.ID_INGRESO = SelectedAmparoIndirecto.ID_INGRESO;
                                obj.ID_AMPARO_INDIRECTO = SelectedAmparoIndirecto.ID_AMPARO_INDIRECTO;
                                obj.DESCR = ObservacionDocumento;
                                obj.DIGITALIZACION_FEC = DatePickCapturaDocumento;
                                obj.DOCUMENTO = DocumentoDigitalizado;
                                if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO != null)
                                    if (SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.Count > 0)
                                        obj.ID_DOCTO = SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.FirstOrDefault().ID_DOCTO;

                                if (obj.ID_DOCTO == null || obj.ID_DOCTO == 0)
                                {
                                    obj.ID_DOCTO = new cAmparoIndirectoDocto().Insertar(obj);
                                    if (obj.ID_DOCTO > 0)
                                    {
                                        SelectedAmparoIndirecto.AMPARO_INDIRECTO_DOCTO.Add(obj);
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cAmparoIndirectoDocto().Actualizar(obj))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }

                                }
                            }
                            break;
                        case 2://AMPARO DIRECTO
                            if (SelectedAmparoDirecto == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar un amparo directo.");
                                return;
                            }
                            else
                            {
                                var obj = new AMPARO_DIRECTO_DOCTO();
                                obj.ID_CENTRO = SelectedAmparoDirecto.ID_CENTRO;
                                obj.ID_ANIO = SelectedAmparoDirecto.ID_ANIO;
                                obj.ID_IMPUTADO = SelectedAmparoDirecto.ID_IMPUTADO;
                                obj.ID_INGRESO = SelectedAmparoDirecto.ID_INGRESO;
                                obj.ID_CAUSA_PENAL = SelectedAmparoDirecto.ID_CAUSA_PENAL;
                                obj.ID_AMPARO_DIRECTO = SelectedAmparoDirecto.ID_AMPARO_DIRECTO;
                                obj.DESCR = ObservacionDocumento;
                                obj.DIGITALIZACION_FEC = DatePickCapturaDocumento;
                                obj.DOCUMENTO = DocumentoDigitalizado;
                                if (SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO != null)
                                    if (SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.Count > 0)
                                        obj.ID_DOCTO = SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.FirstOrDefault().ID_DOCTO;

                                if (obj.ID_DOCTO == null || obj.ID_DOCTO == 0)
                                {
                                    obj.ID_DOCTO = new cAmparoDirectoDocto().Insertar(obj);
                                    if (obj.ID_DOCTO > 0)
                                    {
                                        SelectedAmparoDirecto.AMPARO_DIRECTO_DOCTO.Add(obj);
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cAmparoDirectoDocto().Actualizar(obj))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                            }
                            break;
                        case 3://INCIDENTE
                            if (SelectedAmparoIncidente == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar un incidente.");
                                return;
                            }
                            else
                            {
                                var obj = new AMPARO_INCIDENTE_DOCTO();
                                obj.ID_CENTRO = SelectedAmparoIncidente.ID_CENTRO;
                                obj.ID_ANIO = SelectedAmparoIncidente.ID_ANIO;
                                obj.ID_IMPUTADO = SelectedAmparoIncidente.ID_IMPUTADO;
                                obj.ID_INGRESO = SelectedAmparoIncidente.ID_INGRESO;
                                obj.ID_CAUSA_PENAL = SelectedAmparoIncidente.ID_CAUSA_PENAL;
                                obj.ID_AMPARO_INCIDENTE = SelectedAmparoIncidente.ID_AMPARO_INCIDENTE;
                                obj.DESCR = ObservacionDocumento;
                                obj.DIGITALIZACION_FEC = DatePickCapturaDocumento;
                                obj.DOCUMENTO = DocumentoDigitalizado;
                                if (SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO != null)
                                    if (SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.Count > 0)
                                        obj.ID_DOCTO = SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.FirstOrDefault().ID_DOCTO;

                                if (obj.ID_DOCTO == null || obj.ID_DOCTO == 0)
                                {
                                    obj.ID_DOCTO = new cAmparoIncidenteDocto().Insertar(obj);
                                    if (obj.ID_DOCTO > 0)
                                    {
                                        SelectedAmparoIncidente.AMPARO_INCIDENTE_DOCTO.Add(obj);
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cAmparoIncidenteDocto().Actualizar(obj))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                            }
                            break;
                        case 4://RECURSO
                            if (SelectedRecurso == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar un recurso.");
                                return;
                            }
                            else
                            {
                                var obj = new RECURSO_DOCTO();
                                obj.ID_CENTRO = SelectedRecurso.ID_CENTRO;
                                obj.ID_ANIO = SelectedRecurso.ID_ANIO;
                                obj.ID_IMPUTADO = SelectedRecurso.ID_IMPUTADO;
                                obj.ID_INGRESO = SelectedRecurso.ID_INGRESO;
                                obj.ID_CAUSA_PENAL = SelectedRecurso.ID_CAUSA_PENAL;
                                obj.ID_RECURSO = SelectedRecurso.ID_RECURSO;
                                obj.DESCR = ObservacionDocumento;
                                obj.DIGITALIZACION_FEC = DatePickCapturaDocumento;
                                obj.DOCUMENTO = DocumentoDigitalizado;
                                if (SelectedRecurso.RECURSO_DOCTO != null)
                                    if (SelectedRecurso.RECURSO_DOCTO.Count > 0)
                                        obj.ID_DOCTO = SelectedRecurso.RECURSO_DOCTO.FirstOrDefault().ID_DOCTO;
                                if (obj.ID_DOCTO == null || obj.ID_DOCTO == 0)
                                {
                                    obj.ID_DOCTO = new cRecursoDocto().Insertar(obj);
                                    if (obj.ID_DOCTO > 0)
                                    {
                                        SelectedRecurso.RECURSO_DOCTO.Add(obj);
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cRecursoDocto().Actualizar(obj))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                            }
                            break;
                        case 5://CAUSA PENAL
                            if (SelectedCausaPenal == null)
                            {
                                await new Dialogos().ConfirmacionDialogoReturn("Digitalización", "Debe seleccionar una causa penal.");
                                return;
                            }
                            else
                            {
                                var obj = new CAUSA_PENAL_DOCTO();
                                obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                                obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                                obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                                obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                                obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                                obj.DESCR = ObservacionDocumento;
                                obj.DIGITALIZACION_FEC = DatePickCapturaDocumento;
                                obj.DOCUMENTO = DocumentoDigitalizado;
                                if (SelectedCausaPenal.CAUSA_PENAL_DOCTO != null)
                                    if (SelectedCausaPenal.CAUSA_PENAL_DOCTO.Count > 0)
                                        obj.ID_DOCTO = SelectedCausaPenal.CAUSA_PENAL_DOCTO.FirstOrDefault().ID_DOCTO;
                                if (obj.ID_DOCTO == null || obj.ID_DOCTO == 0)
                                {
                                    obj.ID_DOCTO = new cCausaPenalDocto().Insertar(obj);
                                    if (obj.ID_DOCTO > 0)
                                    {
                                        SelectedCausaPenal.CAUSA_PENAL_DOCTO.Add(obj);
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                                else
                                {
                                    if (new cCausaPenalDocto().Actualizar(obj))
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Documento Guardado Exitosamente", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                                        }));
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                        {
                                            StaticSourcesViewModel.Mensaje("Digitalización", "Ocurrió un Error al Grabar el Documento", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                                        }));
                                    }
                                }
                            }
                            break;
                    }
                    ListTipoDocumento[0].DIGITALIZADO = true;
                    ListTipoDocumento = new ObservableCollection<TipoDocumento>(ListTipoDocumento);
                    ObservacionDocumento = string.Empty;
                    if (AutoGuardado)
                        escaner.Show();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al digitalizar documento.", ex);
            }
        }
        #endregion

        #region PartidaJuridica
        private void ImprimirPartidaJuridica()
        {
            if (SelectedCausaPenal != null)
            {

                try
                {
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                    SENTENCIA sentencia;
                    if (SelectedCausaPenal.SENTENCIA == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                        return;
                    }
                    else
                    {
                        sentencia = SelectedCausaPenal.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                        if (sentencia == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La causa penal no cuenta con una sentencia");
                            return;
                        }
                    }
                    var diccionario = new Dictionary<string, string>();
                    diccionario.Add("centro", SelectedCausaPenal.INGRESO.CENTRO.DESCR.Trim());
                    diccionario.Add("expediente", string.Format("{0}/{1}", SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO));
                    diccionario.Add("ciudad", SelectedCausaPenal.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1.Trim());
                    diccionario.Add("fecha_letra", Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    diccionario.Add("dirigido", centro.DIRECTOR.Trim());
                    string nombres = string.Empty;
                    nombres = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedCausaPenal.INGRESO.IMPUTADO.NOMBRE) ? SelectedCausaPenal.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedCausaPenal.INGRESO.IMPUTADO.PATERNO) ? SelectedCausaPenal.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedCausaPenal.INGRESO.IMPUTADO.MATERNO) ? SelectedCausaPenal.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                    if (SelectedCausaPenal.INGRESO.IMPUTADO.ALIAS != null)
                    {
                        foreach (var a in SelectedCausaPenal.INGRESO.IMPUTADO.ALIAS)
                        {
                            nombres = nombres + string.Format("(O)\n{0} {1} {2}", !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);
                        }
                    }
                    diccionario.Add("interno", nombres);
                    diccionario.Add("causa_penal", string.Format("{0}/{1}", SelectedCausaPenal.CP_ANIO, SelectedCausaPenal.CP_FOLIO));
                    diccionario.Add("juzgado", SelectedCausaPenal.JUZGADO.DESCR.Trim());
                    string delitos = string.Empty;
                    if (SelectedCausaPenal.CAUSA_PENAL_DELITO != null)
                    {
                        foreach (var d in SelectedCausaPenal.CAUSA_PENAL_DELITO)
                        {
                            if (!string.IsNullOrEmpty(delitos))
                                delitos = delitos + ",";
                            delitos = delitos + d.MODALIDAD_DELITO.DELITO.DESCR.Trim();
                        }
                    }
                    diccionario.Add("delito", delitos);
                    diccionario.Add("fecha_traslado", " ");
                    diccionario.Add("cereso_procedencia", " ");
                    diccionario.Add("ingreso_origen", Fechas.fechaLetra(SelectedCausaPenal.INGRESO.FEC_INGRESO_CERESO.Value, false));

                    if (SelectedCausaPenal.SENTENCIA != null)
                    {
                        if (sentencia != null)
                        {
                            string primera;
                            primera = Fechas.fechaLetra(sentencia.FEC_EJECUTORIA, false) + ",";
                            if (sentencia.ANIOS != null && sentencia.ANIOS > 0)
                                primera = primera + string.Format(" {0} Años", sentencia.ANIOS);
                            if (sentencia.MESES != null && sentencia.MESES > 0)
                                primera = primera + string.Format(" {0} Meses", sentencia.MESES);
                            if (sentencia.DIAS != null && sentencia.DIAS > 0)
                                primera = primera + string.Format(" {0} Dias", sentencia.DIAS);
                            diccionario.Add("sentencia_primera_instancia", primera);
                            diccionario.Add("reparacion_danio", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            //MULTAS
                            diccionario.Add("multa_primera", !string.IsNullOrEmpty(sentencia.MULTA) ? sentencia.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_primera", !string.IsNullOrEmpty(sentencia.REPARACION_DANIO) ? sentencia.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_primera", !string.IsNullOrEmpty(sentencia.SUSTITUCION_PENA) ? sentencia.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_primera", !string.IsNullOrEmpty(sentencia.SUSPENSION_CONDICIONAL) ? sentencia.SUSPENSION_CONDICIONAL.Trim() : " ");
                            //ABONOS
                            string abonos = string.Empty;
                            if (sentencia.ANIOS_ABONADOS != null && sentencia.ANIOS_ABONADOS > 0)
                                abonos = string.Format("{0} Años ", sentencia.ANIOS_ABONADOS);
                            if (sentencia.MESES_ABONADOS != null && sentencia.MESES_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Meses ", sentencia.MESES_ABONADOS);
                            if (sentencia.DIAS_ABONADOS != null && sentencia.DIAS_ABONADOS > 0)
                                abonos = abonos + string.Format("{0} Dias ", sentencia.DIAS_ABONADOS);
                            if (!string.IsNullOrEmpty(abonos))
                                diccionario.Add("abonos", abonos);
                            else
                                diccionario.Add("abonos", " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_primera_instancia", " ");
                            //MULTAS
                            diccionario.Add("multa_primera", " ");
                            diccionario.Add("reparacion_primera", " ");
                            diccionario.Add("sustitucion_primera", " ");
                            diccionario.Add("suspencion_primera", " ");
                            //ABONOS
                            diccionario.Add("abonos", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_primera_instancia", " ");
                        //MULTAS
                        diccionario.Add("multa_primera", " ");
                        diccionario.Add("reparacion_primera", " ");
                        diccionario.Add("sustitucion_primera", " ");
                        diccionario.Add("suspencion_primera", " ");
                        //ABONOS
                        diccionario.Add("abonos", " ");
                    }


                    diccionario.Add("beneficio_ley", "No");

                    if (SelectedCausaPenal.RECURSO != null)
                    {
                        var recurso = SelectedCausaPenal.RECURSO.Where(w => w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                        if (recurso != null)
                        {
                            string segunda;
                            segunda = Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ",";
                            if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                            if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                            if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);
                            diccionario.Add("sentencia_segunda_instancia", segunda);
                            //MULTA
                            diccionario.Add("multa_segunda", !string.IsNullOrEmpty(recurso.MULTA) ? recurso.MULTA.Trim() : " ");
                            diccionario.Add("reparacion_segunda", !string.IsNullOrEmpty(recurso.REPARACION_DANIO) ? recurso.REPARACION_DANIO.Trim() : " ");
                            diccionario.Add("sustitucion_segunda", !string.IsNullOrEmpty(recurso.SUSTITUCION_PENA) ? recurso.SUSTITUCION_PENA.Trim() : " ");
                            diccionario.Add("suspencion_segunda", !string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL) ? recurso.MULTA_CONDICIONAL.Trim() : " ");
                        }
                        else
                        {
                            diccionario.Add("sentencia_segunda_instancia", " ");
                            diccionario.Add("multa_segunda", " ");
                            diccionario.Add("reparacion_segunda", " ");
                            diccionario.Add("sustitucion_segunda", " ");
                            diccionario.Add("suspencion_segunda", " ");
                        }
                    }
                    else
                    {
                        diccionario.Add("sentencia_segunda_instancia", " ");
                        diccionario.Add("multa_segunda", " ");
                        diccionario.Add("reparacion_segunda", " ");
                        diccionario.Add("sustitucion_segunda", " ");
                        diccionario.Add("suspencion_segunda", " ");
                    }

                    //INCIDENTE MODIFICA SENTENCIA
                    if (selectedCausaPenal.AMPARO_INCIDENTE != null)
                    {
                        var incidente = selectedCausaPenal.AMPARO_INCIDENTE.Where(w => w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                        if (incidente != null)
                        {
                            string adecuacion = string.Empty;
                            if (incidente.MODIFICA_PENA_ANIO > 0)
                                adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                            if (incidente.MODIFICA_PENA_MES > 0)
                                adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                            if (incidente.MODIFICA_PENA_DIA > 0)
                                adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);

                            diccionario.Add("incidente_adecuacion_pena", adecuacion);
                        }
                        else
                            diccionario.Add("incidente_adecuacion_pena", " ");
                    }
                    else
                        diccionario.Add("incidente_adecuacion_pena", " ");

                    diccionario.Add("director_centro", centro.DIRECTOR.Trim());
                    diccionario.Add("elaboro", StaticSourcesViewModel.UsuarioLogin.Nombre);

                    var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.PARTIDA_JURIDICA); //File.ReadAllBytes(@"C:\libertades\PJ.doc");
                    if (documento == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                        return;
                    }
                    var bytes = new cWord().FillFields(documento.DOCUMENTO, diccionario);
                    if (bytes == null)
                        return;
                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            switch (documento.ID_FORMATO)
                            { 
                                case (int)enumFormatoDocumento.DOCX:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
                                    break;
                                case (int)enumFormatoDocumento.PDF:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.AdobePDF);
                                    break;
                                case (int)enumFormatoDocumento.DOC:
                                    tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
                                    break;
                                default:
                                    new Dialogos().ConfirmacionDialogo("Validación",string.Format("El formato {0} del documento no es valido",documento.FORMATO_DOCUMENTO.DESCR));
                                    break;
                            }
                            
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
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir la partida juridica.", ex);
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal.");


        }
        #endregion

        #region Validar Traslados
        private async void ValidarTraslados()
        {
            try
            {
                if (LstAIT != null)
                {
                    var obj = LstAIT.Where(w => w.ID_AMP_IND_TIPO == (short)enumAmparoIndirectoTipo.TRASLADO && w.ID_SEN_AMP_RESULTADO == (short)enumAmparoIndirectoSentencia.SE_CONCEDE_AMPARO).SingleOrDefault();
                    if (obj != null)
                    {
                        //verificamos si hay translado programado
                        if (SelectedIngreso != null)
                        {
                            if (SelectedIngreso.TRASLADO_DETALLE != null)
                            {
                                var traslado = SelectedIngreso.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == "PR").SingleOrDefault();
                                if (traslado != null)
                                {
                                    if (await new Dialogos().ConfirmarEliminar("Validación!", "El interno cuenta con un traslado programado, ¿desea cancelarlo?") == 1)
                                    {
                                        var detalle = new TRASLADO_DETALLE();
                                        detalle.ID_TRASLADO = traslado.ID_TRASLADO;
                                        detalle.ID_CENTRO = traslado.ID_CENTRO;
                                        detalle.ID_ANIO = traslado.ID_ANIO;
                                        detalle.ID_IMPUTADO = traslado.ID_IMPUTADO;
                                        detalle.ID_INGRESO = traslado.ID_INGRESO;
                                        detalle.ID_ESTATUS = "CA";
                                        detalle.ID_ESTATUS_ADMINISTRATIVO = traslado.ID_ESTATUS_ADMINISTRATIVO;
                                        detalle.EGRESO_FEC = traslado.EGRESO_FEC;
                                        detalle.ID_CENTRO_TRASLADO = traslado.ID_CENTRO_TRASLADO;
                                        if (new cTrasladoDetalle().Actualizar(detalle))
                                        {
                                            var tras = new cTraslado().Obtener(GlobalVar.gCentro, traslado.ID_TRASLADO);
                                            if (tras != null)
                                            {
                                                if (tras.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == "PR").Count() == 0)
                                                {
                                                    var x = new TRASLADO();
                                                    x.ID_TRASLADO = tras.ID_TRASLADO;
                                                    x.ID_CENTRO = tras.ID_CENTRO;
                                                    x.ORIGEN_TIPO = tras.ORIGEN_TIPO;
                                                    x.CENTRO_ORIGEN = tras.CENTRO_ORIGEN;
                                                    x.CENTRO_ORIGEN_FORANEO = tras.CENTRO_ORIGEN_FORANEO;
                                                    x.TRASLADO_FEC = tras.TRASLADO_FEC;
                                                    x.ID_MOTIVO = tras.ID_MOTIVO;
                                                    x.JUSTIFICACION = tras.JUSTIFICACION;
                                                    x.CENTRO_DESTINO = tras.CENTRO_DESTINO;
                                                    x.OFICIO_AUTORIZACION = tras.OFICIO_AUTORIZACION;
                                                    x.OFICIO_SALIDA = tras.OFICIO_SALIDA;
                                                    x.ID_MOTIVO_SALIDA = tras.ID_MOTIVO_SALIDA;
                                                    x.ID_ESTATUS = "CA";
                                                    x.AUTORIZA_TRASLADO = tras.AUTORIZA_TRASLADO;
                                                    x.AUTORIZA_SALIDA = tras.AUTORIZA_SALIDA;
                                                    new cTraslado().Actualizar(x, (short)enumMensajeTipo.CALENDARIZACION_TRASLADO, Fechas.GetFechaDateServer);
                                                    new Dialogos().ConfirmacionDialogo("Éxito", "Se ha cancelado el traslado del interno.");
                                                }
                                                else
                                                    new Dialogos().ConfirmacionDialogo("Éxito", "Se ha cancelado el traslado del interno.");
                                            }
                                        }
                                        else
                                            new Dialogos().ConfirmacionDialogo("Error", "Error al cambiar estatus del interno en el traslado");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cancelar el traslado del interno", ex);
            }
        }
        #endregion

        #region Coparticipes
        private async void ClickEnterCoparticipe(Object obj)
        {
            if (obj != null)
            {
                //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                var textbox = obj as TextBox;

                if (textbox != null)
                {
                    NombreCoparticipe = textbox.Text;

                    if (!base.HasErrors)
                    {
                        if (LstCoparticipe == null)
                            LstCoparticipe = new ObservableCollection<COPARTICIPE>();
                        if(SelectedCoparticipe == null)
                            if (LstCoparticipe.Count(w =>  w.NOMBRE == NombreCoparticipe && w.PATERNO == PaternoCoparticipe && w.MATERNO == MaternoCoparticipe) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El coparticipe ya se encuentra registrado");
                                return;
                            }
                        else
                            if (LstCoparticipe.Count(w => w.ID_COPARTICIPE != SelectedCoparticipe.ID_COPARTICIPE && w.NOMBRE == NombreCoparticipe && w.PATERNO == PaternoCoparticipe && w.MATERNO == MaternoCoparticipe) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El coparticipe ya se encuentra registrado");
                                return;
                            }
                        AgregarCoparticipe();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_COPARTICIPE);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación","Favor de capturar los campos obligatorios");
                }
            }
        }

        private async void ClickEnterCoparticipeAlias(Object obj)
        {
            if (obj != null)
            {
                //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                var textbox = obj as TextBox;

                if (textbox != null)
                {
                    NombreAlias = textbox.Text;

                    if (!base.HasErrors)
                    {
                        if (LstAlias == null)
                            LstAlias = new ObservableCollection<COPARTICIPE_ALIAS>();
                        if(SelectedAlias == null)
                            if (LstAlias.Count(w => w.NOMBRE == NombreAlias && w.PATERNO == PaternoAlias && w.MATERNO == MaternoAlias && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El alias ya se encuentra registrado");
                                return;
                            }
                            else
                                if (LstAlias.Count(w => w.NOMBRE == NombreAlias && w.PATERNO == PaternoAlias && w.MATERNO == MaternoAlias && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE && w.ID_COPARTICIPE_ALIAS != SelectedAlias.ID_COPARTICIPE_ALIAS && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El alias ya se encuentra registrado");
                                    return;
                                }

                        AgregarAlias();
                        base.ClearRules();
                        LimpiarComparticipeApodo();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ALIAS);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
                }
            }
        }

        private async void ClickEnterCoparticipeApodo(Object obj)
        {
            if (obj != null)
            {
                //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                var textbox = obj as TextBox;

                if (textbox != null)
                {
                    Apodo = textbox.Text;

                    if (!base.HasErrors)
                    {
                        if (LstApodo == null)
                            LstApodo = new ObservableCollection<COPARTICIPE_APODO>();
                        if (SelectedApodo == null)
                            if (LstApodo.Count(w => w.APODO == Apodo && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El apodo ya se encuentra registrado");
                                return;
                            }
                            else
                                if (LstApodo.Count(w => w.APODO == Apodo && w.ID_COPARTICIPE_APODO != SelectedApodo.ID_COPARTICIPE_APODO && w.ID_COPARTICIPE == SelectedCoparticipe.ID_COPARTICIPE) > 0)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El apodo ya se encuentra registrado");
                                    return;
                                }

                        AgregarApodo();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.APODO);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos obligatorios");
                }
            }
        }
        #endregion

        #region Partida Juridica
        private void ImprimirPartidaJuridica2()
        {
            try
            {
                if (SelectedIngreso != null)
                {
                    //string FileName = Path.GetTempPath() + @"\PartidaJuridica.docx";
                    
                    MemoryStream stream = new MemoryStream();
                    using (DocX document = DocX.Create(stream))
                    {
                        var cp_usadas = new List<CAUSA_PENAL>();
                        var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();

                        #region Configuracion del Documento
                        document.MarginLeft = 40;
                        document.MarginRight = 40;
                        #endregion

                        #region Header
                        var fuero = "Desconocido";
                        document.AddHeaders();
                        document.AddFooters();
                        // Get the default Header for this document.
                        Header header_default = document.Headers.odd;

                        // Insert a Paragraph into the default Header.
                        var headLineFormat = new Formatting();
                        headLineFormat.FontFamily = new System.Drawing.FontFamily("Arial Black");

                        Novacode.Paragraph p1 = header_default.InsertParagraph();
                        p1.Alignment = Alignment.right;
                        p1.Append("Secretaria de Seguridad Publica").Bold();
                        p1.AppendLine("Subsecretaria del Sistema Estatal Penitenciario");
                        p1.AppendLine("Centro de Reinserción Social " + centro.DESCR.Trim());
                        p1.AppendLine("Jurídico");
                        p1.AppendLine();
                        p1.AppendLine(string.Format("Exp.{0}/{1}", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO));
                        p1.AppendLine();
                        //p1.AppendLine("Secretaria de Seguridad Publica").Bold();
                        #endregion

                        #region Body
                        Novacode.Paragraph p2 = document.InsertParagraph();
                        p2.Alignment = Alignment.right;
                        //p2.AppendLine(centro.MUNICIPIO.MUNICIPIO1.Trim());
                        p2.AppendLine(centro.MUNICIPIO.MUNICIPIO1.Trim() + ", " + Fechas.fechaLetra(Fechas.GetFechaDateServer, false));

                        var presente = Parametro.DIRECTOR_EJECUCION_PENAS;
                        if (SelectedIngreso != null)
                        {
                            if (SelectedIngreso.CAUSA_PENAL != null)
                            {
                                var activa = SelectedIngreso.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).FirstOrDefault();
                                if (activa != null)
                                {
                                    fuero = activa.CP_FUERO == "C" ? "Común" : activa.CP_FUERO == "F" ? "Federal" : "Desconocido";
                                    if (activa.CP_FUERO == "F")
                                    {
                                        presente = Parametro.ORGANO_ADMINISTRATIVO;
                                    }
                                }
                            }
                        }

                        Novacode.Paragraph p3 = document.InsertParagraph();
                        p3.Alignment = Alignment.left;
                        presente = presente.Replace("<>", Environment.NewLine);                        
                        p3.AppendLine(presente).Bold();
                        p3.AppendLine("PRESENTE.-").Bold();
                        p3.Spacing(4);

                        Novacode.Paragraph p4 = document.InsertParagraph();
                        p4.AppendLine();
                        p4.AppendLine("Por este conducto me permito remitir a Usted, Partida de Antecedentes Penales, que registra en este Centro de Reinserción Social " + centro.DESCR.Trim() + ", el interno del Fuero " + fuero + " de nombre:");
                        p4.AppendLine();

                        Novacode.Paragraph p5 = document.InsertParagraph();
                        p5.Alignment = Alignment.center;
                        //
                        string nombres = string.Empty;
                        nombres = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.NOMBRE) ? SelectedIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                        if (SelectedIngreso.IMPUTADO.ALIAS != null)
                        {
                            foreach (var a in SelectedIngreso.IMPUTADO.ALIAS)
                            {
                                nombres = nombres + string.Format("(O)" + Environment.NewLine + "{0} {1} {2}", !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);
                            }
                        }
                        p5.AppendLine(nombres).Bold();
                        p5.UnderlineStyle(UnderlineStyle.singleLine);
                        p5.AppendLine();

                        var causasPenales = new cCausaPenal().Obtener(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO);
                        if (causasPenales != null)
                        {
                            var ingresos = new cIngreso().ObtenerIngresos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO);

                            #region Causa Penal Activas
                            foreach (var cp in causasPenales.Where(w => w.ID_ESTATUS_CP == 1 && w.ID_INGRESO == SelectedIngreso.ID_INGRESO))
                            {
                                Novacode.Paragraph p6 = document.InsertParagraph();
                                p6.AppendLine();
                                int i = 0;
                                Novacode.Table t = document.AddTable(1,2);//(17, 2);
                                float[] x = { 350, 350 };
                                t.SetWidths(x);
                                t.Alignment = Alignment.center;
                                t.Design = TableDesign.TableNormal;
                                

                                t.Rows[i].Cells[0].Paragraphs.First().Append("Causa Penal").Bold();
                                t.Rows[i].Cells[1].Paragraphs.First().Append(cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                t.Rows[i].Height = 25;
                                t.InsertRow();
                                i++;

                                if (cp.NUC != null)
                                {
                                    if (!string.IsNullOrEmpty(cp.NUC.ID_NUC))
                                    {
                                        t.Rows[i].Cells[0].Paragraphs.First().Append("NUC").Bold();
                                        t.Rows[i].Cells[1].Paragraphs.First().Append(cp.NUC.ID_NUC);
                                        t.InsertRow();
                                        i++;
                                    }
                                }

                                t.Rows[i].Cells[0].Paragraphs.First().Append("Juzgado").Bold();
                                t.Rows[i].Cells[1].Paragraphs.First().Append(cp.JUZGADO.DESCR.Trim());
                                t.InsertRow();
                                i++;

                                t.Rows[i].Cells[0].Paragraphs.First().Append("Delito").Bold();
                                
                                string delitos = string.Empty;
                                if(cp.SENTENCIA != null)
                                {
                                    var sa = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                    if (sa != null)
                                    {
                                        if (sa.SENTENCIA_DELITO != null)
                                        {
                                            foreach (var d in sa.SENTENCIA_DELITO)
                                            {
                                                if (!string.IsNullOrEmpty(delitos))
                                                    delitos = delitos + ",";
                                                delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                            }
                                        }
                                    }
                                }
                                
                                //if (cp.CAUSA_PENAL_DELITO != null)
                                //{
                                //    foreach (var d in cp.CAUSA_PENAL_DELITO)
                                //    {
                                //        if (!string.IsNullOrEmpty(delitos))
                                //            delitos = delitos + ",";
                                //        delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(),d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                //    }
                                //}
                                t.Rows[i].Cells[1].Paragraphs.First().Append(delitos);
                                t.InsertRow();
                                i++;

                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al " + centro.DESCR.Trim()).Bold();
                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(cp.INGRESO.FEC_INGRESO_CERESO, false));
                                t.InsertRow();
                                i++;

                                if (cp.ID_INGRESO > 1)
                                {
                                    var anterior = ingresos.Where(w => w.ID_INGRESO == (cp.ID_INGRESO - 1)).FirstOrDefault();
                                    if (anterior != null)
                                    {
                                        if (anterior.ID_ESTATUS_ADMINISTRATIVO == 5)//viene de un traslado
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Procedencia").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(anterior.CENTRO1.DESCR.Trim());
                                            t.InsertRow();
                                            i++;

                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al Centro de Origen:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(anterior.FEC_INGRESO_CERESO, false));
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                }

                                #region Bajas y Reingresos
                                var anteriores = causasPenales.Where(w => w.CP_ANIO == cp.CP_ANIO && w.CP_FOLIO == cp.CP_FOLIO && w.ID_INGRESO != cp.ID_INGRESO).OrderByDescending(w => w.ID_INGRESO);
                                if (anteriores != null)
                                {
                                    foreach (var a in anteriores)
                                    {
                                        cp_usadas.Add(new CAUSA_PENAL()
                                        {
                                          ID_INGRESO = a.ID_INGRESO,
                                          ID_CAUSA_PENAL = a.ID_CAUSA_PENAL
                                        });

                                        if (a.LIBERACION != null)
                                        {
                                            var l = a.LIBERACION.FirstOrDefault();
                                            if (l != null)
                                            {
                                                //Baja
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Baja:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", Fechas.fechaLetra(l.LIBERACION_FEC, false),l.LIBERACION_MOTIVO.DESCR.Trim()));
                                                t.InsertRow();
                                                i++;
                                            }
                                        }

                                        //Reingreso
                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Reingreso:").Bold();
                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(a.INGRESO.FEC_INGRESO_CERESO, false));
                                        t.InsertRow();
                                        i++;
                                    }
                                }
                                #endregion

                                if (cp.SENTENCIA != null)
                                {
                                    #region Historico
                                    var historico = cp.SENTENCIA.Where(w => w.ESTATUS == "I").OrderBy(w => w.ID_SENTENCIA);
                                    if (historico != null)
                                    {
                                        foreach (var h in historico)
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                            string primera = string.Empty;
                                            if (h.ANIOS != null && h.ANIOS > 0)
                                                primera = primera + string.Format(" {0} Años", h.ANIOS);
                                            if (h.MESES != null && h.MESES > 0)
                                                primera = primera + string.Format(" {0} Meses", h.MESES);
                                            if (h.DIAS != null && h.DIAS > 0)
                                                primera = primera + string.Format(" {0} Dias", h.DIAS);
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(h.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                            t.InsertRow();
                                            i++;

                                            if (cp.RECURSO != null)
                                            {
                                                var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                if (rec != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;

                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                                else
                                                {
                                                    var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                    if (recRevoca == null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                t.InsertRow();
                                                i++;
                                            }


                                            //Segunda Instancia
                                            var seg = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                            if (seg != null)
                                            {
                                                string segunda = string.Empty;
                                                if (seg.SENTENCIA_ANIOS != null && seg.SENTENCIA_ANIOS > 0)
                                                    segunda = segunda + string.Format(" {0} Años", seg.SENTENCIA_ANIOS);
                                                if (seg.SENTENCIA_MESES != null && seg.SENTENCIA_MESES > 0)
                                                    segunda = segunda + string.Format(" {0} Meses", seg.SENTENCIA_MESES);
                                                if (seg.SENTENCIA_DIAS != null && seg.SENTENCIA_DIAS > 0)
                                                    segunda = segunda + string.Format(" {0} Dias", seg.SENTENCIA_DIAS);

                                                if (!string.IsNullOrEmpty(segunda))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(seg.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: "+seg.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                            //Recursos
                                            //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                            //if (rec1 != null)
                                            //{
                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                            //    t.InsertRow();
                                            //    i++;

                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                            //    t.InsertRow();
                                            //    i++;
                                            //}
                                            
                                            var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                            if (rec3 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                            if (rec4 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                            if (recurso2 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }


                                            var inc = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                            if (inc != null)
                                            {
                                                string adecuacion = string.Empty;
                                                if (inc.MODIFICA_PENA_ANIO > 0)
                                                    adecuacion = string.Format("{0} Años ", inc.MODIFICA_PENA_ANIO);
                                                if (inc.MODIFICA_PENA_MES > 0)
                                                    adecuacion = adecuacion + string.Format("{0} Meses ", inc.MODIFICA_PENA_MES);
                                                if (inc.MODIFICA_PENA_DIA > 0)
                                                    adecuacion = adecuacion + string.Format("{0} Dias ", inc.MODIFICA_PENA_DIA);
                                                if (!string.IsNullOrEmpty(adecuacion))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                            /////////////////////
                                            var amdir = cp.AMPARO_DIRECTO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_SEN_AMP_RESULTADO == 3).FirstOrDefault();
                                            if (amdir != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Directo:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Reposicion de Procedimiento");
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }
                                    #endregion

                                    foreach (var s in cp.SENTENCIA.Where(w => w.ESTATUS == "A"))
                                    {
                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                        string primera = string.Empty;
                                        if (s.ANIOS != null && s.ANIOS > 0)
                                            primera = primera + string.Format(" {0} Años", s.ANIOS);
                                        if (s.MESES != null && s.MESES > 0)
                                            primera = primera + string.Format(" {0} Meses", s.MESES);
                                        if (s.DIAS != null && s.DIAS > 0)
                                            primera = primera + string.Format(" {0} Dias", s.DIAS);
                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(s.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                        t.InsertRow();
                                        i++;

                                        if (cp.RECURSO != null)
                                        {
                                            var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                            if (rec != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                t.InsertRow();
                                                i++;

                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                t.InsertRow();
                                                i++;
                                            }
                                            else
                                            {
                                                var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                if (recRevoca == null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                            t.InsertRow();
                                            i++;
                                        }

                                        if (!string.IsNullOrEmpty(s.MULTA))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.MULTA, s.MULTA_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                            t.InsertRow();
                                            i++;
                                        }

                                        if (!string.IsNullOrEmpty(s.REPARACION_DANIO))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.REPARACION_DANIO, s.REPARACION_DANIO_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                            t.InsertRow();
                                            i++;
                                        }

                                        if (!string.IsNullOrEmpty(s.SUSTITUCION_PENA))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitución de la pena:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.SUSTITUCION_PENA, s.SUSTITUCION_PENA_PAGADA == "S" ? ", PAGADA" : "NO PAGADA"));
                                            t.InsertRow();
                                            i++;
                                        }

                                        if (!string.IsNullOrEmpty(s.SUSPENSION_CONDICIONAL))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(s.SUSPENSION_CONDICIONAL);
                                            t.InsertRow();
                                            i++;
                                        }

                                        string abonos = string.Empty;
                                        if (s.ANIOS_ABONADOS != null && s.ANIOS_ABONADOS > 0)
                                            abonos = string.Format("{0} Años ", s.ANIOS_ABONADOS);
                                        if (s.MESES_ABONADOS != null && s.MESES_ABONADOS > 0)
                                            abonos = abonos + string.Format("{0} Meses ", s.MESES_ABONADOS);
                                        if (s.DIAS_ABONADOS != null && s.DIAS_ABONADOS > 0)
                                            abonos = abonos + string.Format("{0} Dias ", s.DIAS_ABONADOS);

                                        if (!string.IsNullOrEmpty(abonos))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(abonos);
                                            t.InsertRow();
                                            i++;
                                        }

                                        #region Recursos
                                        if (cp.RECURSO != null)
                                        {
                                            var recurso = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                            if (recurso != null)
                                            {
                                                string segunda = string.Empty;
                                                if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                                    segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                                                if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                                    segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                                                if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                                    segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);

                                                if (!string.IsNullOrEmpty(segunda))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + recurso.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                if (!string.IsNullOrEmpty(recurso.MULTA))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                if (!string.IsNullOrEmpty(recurso.REPARACION_DANIO))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.REPARACION_DANIO);
                                                    t.InsertRow();
                                                    i++;
                                                }


                                                if (!string.IsNullOrEmpty(recurso.SUSTITUCION_PENA))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.SUSTITUCION_PENA);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                if (!string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA_CONDICIONAL);
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        
                                            //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                            //if (rec1 != null)
                                            //{
                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                            //    t.InsertRow();
                                            //    i++;

                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                            //    t.InsertRow();
                                            //    i++;
                                            //}
                                            
                                            var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                            if (rec3 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                            if (rec4 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                            if (recurso2 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                        #endregion

                                        #region Amparo Incidente
                                        if (cp.AMPARO_INCIDENTE != null)
                                        {
                                            var incidente = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                            if (incidente != null)
                                            {
                                                string adecuacion = string.Empty;
                                                if (incidente.MODIFICA_PENA_ANIO > 0)
                                                    adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                                                if (incidente.MODIFICA_PENA_MES > 0)
                                                    adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                                                if (incidente.MODIFICA_PENA_DIA > 0)
                                                    adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);
                                                if (!string.IsNullOrEmpty(adecuacion))
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #region comentado
                                //else
                                //{
                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de PrimeraInstancia:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                //    t.InsertRow();
                                //    i++;

                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                //    t.InsertRow();
                                //    i++;

                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                //    t.InsertRow();
                                //    i++;

                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                //    t.InsertRow();
                                //    i++;

                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                //    t.InsertRow();
                                //    i++;

                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Empty);
                                //    t.InsertRow();
                                //    i++;
                                //}
                                #endregion
                               
                                t.RemoveRow(i);
                                document.InsertTable(t);
                            }
                            #endregion

                            #region Causa por compurgar
                            foreach (var cp in causasPenales.Where(w => w.ID_ESTATUS_CP == 0 && w.ID_INGRESO == SelectedIngreso.ID_INGRESO))
                            {
                                if (cp_usadas.Exists(x => x.ID_INGRESO == cp.ID_INGRESO && x.ID_CAUSA_PENAL == cp.ID_CAUSA_PENAL && x.CP_JUZGADO == cp.CP_JUZGADO) == false)
                                {
                                    Novacode.Paragraph p6 = document.InsertParagraph();
                                    p6.AppendLine();
                                    int i = 0;
                                    Novacode.Table t = document.AddTable(1, 2);//(17, 2);
                                    float[] x = { 350, 350 };
                                    t.SetWidths(x);
                                    t.Alignment = Alignment.center;
                                    t.Design = TableDesign.TableNormal;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Causa Penal").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                    t.Rows[i].Height = t.Rows[i].MinHeight = 25;
                                    t.Rows[i].Height = 5;
                                    t.InsertRow();
                                    i++;

                                    if (cp.NUC != null)
                                    {
                                        if (!string.IsNullOrEmpty(cp.NUC.ID_NUC))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("NUC").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(cp.NUC.ID_NUC);
                                            t.InsertRow();
                                            i++;
                                        }
                                    }

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Juzgado").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.JUZGADO.DESCR.Trim());
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Delito").Bold();
                                    
                                    string delitos = string.Empty;

                                    if(cp.SENTENCIA != null)
                                    {
                                        var sa = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                        if (sa != null)
                                        {
                                            if (sa.SENTENCIA_DELITO != null)
                                            {
                                                foreach (var d in sa.SENTENCIA_DELITO)
                                                {
                                                    if (!string.IsNullOrEmpty(delitos))
                                                        delitos = delitos + ",";
                                                    delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                                }
                                            }
                                        }
                                    }
                                    //if (cp.CAUSA_PENAL_DELITO != null)
                                    //{
                                    //    foreach (var d in cp.CAUSA_PENAL_DELITO)
                                    //    {
                                    //        if (!string.IsNullOrEmpty(delitos))
                                    //            delitos = delitos + ",";
                                    //        delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                    //    }
                                    //}
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(delitos);
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al " + centro.DESCR.Trim()).Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(cp.INGRESO.FEC_INGRESO_CERESO, false));
                                    t.InsertRow();
                                    i++;

                                    if (cp.ID_INGRESO > 1)
                                    {
                                        var anterior = ingresos.Where(w => w.ID_INGRESO == (cp.ID_INGRESO - 1)).FirstOrDefault();
                                        if (anterior != null)
                                        {
                                            if (anterior.ID_ESTATUS_ADMINISTRATIVO == 5)//viene de un traslado
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Procedencia").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(anterior.CENTRO1.DESCR.Trim());
                                                t.InsertRow();
                                                i++;

                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al Centro de Origen:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(anterior.FEC_INGRESO_CERESO, false));
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }

                                    #region Bajas y Reingresos
                                    var anteriores = causasPenales.Where(w => w.CP_ANIO == cp.CP_ANIO && w.CP_FOLIO == cp.CP_FOLIO && w.ID_INGRESO > cp.ID_INGRESO).OrderByDescending(w => w.ID_INGRESO);
                                    if (anteriores != null)
                                    {
                                        foreach (var a in anteriores)
                                        {
                                            cp_usadas.Add(new CAUSA_PENAL()
                                            {
                                                ID_INGRESO = a.ID_INGRESO,
                                                ID_CAUSA_PENAL = a.ID_CAUSA_PENAL
                                            });

                                            if (a.LIBERACION != null)
                                            {
                                                var l = a.LIBERACION.FirstOrDefault();
                                                if (l != null)
                                                {
                                                    //Baja
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Baja:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", Fechas.fechaLetra(l.LIBERACION_FEC, false), l.LIBERACION_MOTIVO.DESCR.Trim()));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }

                                            //Reingreso
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Reingreso:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(a.INGRESO.FEC_INGRESO_CERESO, false));
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                    #endregion

                                    if (cp.SENTENCIA != null)
                                    {
                                        #region Historico
                                        var historico = cp.SENTENCIA.Where(w => w.ESTATUS == "I").OrderBy(w => w.ID_SENTENCIA);
                                        if (historico != null)
                                        {
                                            foreach (var h in historico)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                                string primera = string.Empty;
                                                if (h.ANIOS != null && h.ANIOS > 0)
                                                    primera = primera + string.Format(" {0} Años", h.ANIOS);
                                                if (h.MESES != null && h.MESES > 0)
                                                    primera = primera + string.Format(" {0} Meses", h.MESES);
                                                if (h.DIAS != null && h.DIAS > 0)
                                                    primera = primera + string.Format(" {0} Dias", h.DIAS);
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(h.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                                t.InsertRow();
                                                i++;

                                                if (cp.RECURSO != null)
                                                {
                                                    var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    if (rec != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: "+rec.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;

                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                    else
                                                    {
                                                        var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                        if (recRevoca == null)
                                                        {
                                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                            t.InsertRow();
                                                            i++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                //Segunda Instancia
                                                var seg = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (seg != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (seg.SENTENCIA_ANIOS != null && seg.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", seg.SENTENCIA_ANIOS);
                                                    if (seg.SENTENCIA_MESES != null && seg.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", seg.SENTENCIA_MESES);
                                                    if (seg.SENTENCIA_DIAS != null && seg.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", seg.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(seg.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + seg.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                            //if (rec1 != null)
                                            //{
                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                            //    t.InsertRow();
                                            //    i++;

                                            //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                            //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                            //    t.InsertRow();
                                            //    i++;
                                            //}
                                            
                                            var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                            if (rec3 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                            if (rec4 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                            if (recurso2 != null)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento");
                                                t.InsertRow();
                                                i++;
                                            }
                                                /////////////////////
                                                var inc = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (inc != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (inc.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", inc.MODIFICA_PENA_ANIO);
                                                    if (inc.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", inc.MODIFICA_PENA_MES);
                                                    if (inc.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", inc.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                /////////////////////
                                                var amdir = cp.AMPARO_DIRECTO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_SEN_AMP_RESULTADO == 3).FirstOrDefault();
                                                if (amdir != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Directo:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Reposicion de Procedimiento");
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        #endregion

                                        foreach (var s in cp.SENTENCIA.Where(w => w.ESTATUS == "A"))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().AppendLine("Sentencia de Primera Instancia:").Bold();
                                            string primera = string.Empty;
                                            if (s.ANIOS != null && s.ANIOS > 0)
                                                primera = primera + string.Format(" {0} Años", s.ANIOS);
                                            if (s.MESES != null && s.MESES > 0)
                                                primera = primera + string.Format(" {0} Meses", s.MESES);
                                            if (s.DIAS != null && s.DIAS > 0)
                                                primera = primera + string.Format(" {0} Dias", s.DIAS);
                                            t.Rows[i].Cells[1].Paragraphs.First().AppendLine(Fechas.fechaLetra(s.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                            t.InsertRow();
                                            i++;

                                            if (cp.RECURSO != null)
                                            {
                                                var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                if (rec != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;

                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                                else
                                                {
                                                    var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                    if (recRevoca == null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.MULTA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.MULTA, s.MULTA_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.REPARACION_DANIO))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.REPARACION_DANIO, s.REPARACION_DANIO_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSTITUCION_PENA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitución de la pena:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.SUSTITUCION_PENA, s.SUSTITUCION_PENA_PAGADA == "S" ? ", PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSPENSION_CONDICIONAL))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(s.SUSPENSION_CONDICIONAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            string abonos = string.Empty;
                                            if (s.ANIOS_ABONADOS != null && s.ANIOS_ABONADOS > 0)
                                                abonos = string.Format("{0} Años ", s.ANIOS_ABONADOS);
                                            if (s.MESES_ABONADOS != null && s.MESES_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Meses ", s.MESES_ABONADOS);
                                            if (s.DIAS_ABONADOS != null && s.DIAS_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Dias ", s.DIAS_ABONADOS);

                                            if (!string.IsNullOrEmpty(abonos))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(abonos);
                                                t.InsertRow();
                                                i++;
                                            }

                                            #region Recursos
                                            if (cp.RECURSO != null)
                                            {
                                                var recurso = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (recurso != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                                                    if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                                                    if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + recurso.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.REPARACION_DANIO))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.REPARACION_DANIO);
                                                        t.InsertRow();
                                                        i++;
                                                    }


                                                    if (!string.IsNullOrEmpty(recurso.SUSTITUCION_PENA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.SUSTITUCION_PENA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA_CONDICIONAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    //if (rec1 != null)
                                                    //{
                                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                    //    t.InsertRow();
                                                    //    i++;

                                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                    //    t.InsertRow();
                                                    //    i++;
                                                    //}

                                                    var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                    if (rec3 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                    if (rec4 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                    if (recurso2 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region Incidente
                                            if (cp.AMPARO_INCIDENTE != null)
                                            {
                                                var incidente = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (incidente != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (incidente.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                                                    if (incidente.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                                                    if (incidente.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    #region comentado
                                    //else
                                    //{
                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de PrimeraInstancia:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Empty);
                                    //    t.InsertRow();
                                    //    i++;
                                    //}
                                    #endregion
                                    
                                    t.RemoveRow(i);
                                    document.InsertTable(t);
                                }
                            }
                            #endregion

                            #region Causa penal en proceso
                            foreach (var cp in causasPenales.Where(w => w.ID_ESTATUS_CP == 6 && w.ID_INGRESO == SelectedIngreso.ID_INGRESO))
                            {
                                if (cp_usadas.Count(x => x.ID_INGRESO == cp.ID_INGRESO && x.ID_CAUSA_PENAL == cp.ID_CAUSA_PENAL && x.CP_JUZGADO == cp.CP_JUZGADO) == 0)
                                {
                                    Novacode.Paragraph p6 = document.InsertParagraph();
                                    p6.AppendLine();
                                    int i = 0;
                                    Novacode.Table t = document.AddTable(1, 2);//(17, 2);
                                    float[] x = { 350, 350 };
                                    t.SetWidths(x);
                                    t.Alignment = Alignment.center;
                                    t.Design = TableDesign.TableNormal;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Causa Penal").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                    t.Rows[i].Height = t.Rows[i].MinHeight = 25;
                                    t.Rows[i].Height = 5;
                                    t.InsertRow();
                                    i++;

                                    if (cp.NUC != null)
                                    {
                                        if (!string.IsNullOrEmpty(cp.NUC.ID_NUC))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("NUC").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(cp.NUC.ID_NUC);
                                            t.InsertRow();
                                            i++;
                                        }
                                    }

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Juzgado").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.JUZGADO.DESCR.Trim());
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Delito").Bold();
                                    string delitos = string.Empty;
                                    if (cp.CAUSA_PENAL_DELITO != null)
                                    {
                                        foreach (var d in cp.CAUSA_PENAL_DELITO)
                                        {
                                            if (!string.IsNullOrEmpty(delitos))
                                                delitos = delitos + ",";
                                            delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                        }
                                    }
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(delitos);
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al " + centro.DESCR.Trim()).Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(cp.INGRESO.FEC_INGRESO_CERESO, false));
                                    t.InsertRow();
                                    i++;

                                    if (cp.ID_INGRESO > 1)
                                    {
                                        var anterior = ingresos.Where(w => w.ID_INGRESO == (cp.ID_INGRESO - 1)).FirstOrDefault();
                                        if (anterior != null)
                                        {
                                            if (anterior.ID_ESTATUS_ADMINISTRATIVO == 5)//viene de un traslado
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Procedencia").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(anterior.CENTRO1.DESCR.Trim());
                                                t.InsertRow();
                                                i++;

                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al Centro de Origen:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(anterior.FEC_INGRESO_CERESO, false));
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }

                                    #region Bajas y Reingresos
                                    var anteriores = causasPenales.Where(w => w.CP_ANIO == cp.CP_ANIO && w.CP_FOLIO == cp.CP_FOLIO && w.ID_INGRESO > cp.ID_INGRESO).OrderByDescending(w => w.ID_INGRESO);
                                    if (anteriores != null)
                                    {
                                        foreach (var a in anteriores)
                                        {
                                            cp_usadas.Add(new CAUSA_PENAL()
                                            {
                                                ID_INGRESO = a.ID_INGRESO,
                                                ID_CAUSA_PENAL = a.ID_CAUSA_PENAL
                                            });

                                            if (a.LIBERACION != null)
                                            {
                                                var l = a.LIBERACION.FirstOrDefault();
                                                if (l != null)
                                                {
                                                    //Baja
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Baja:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", Fechas.fechaLetra(l.LIBERACION_FEC, false), l.LIBERACION_MOTIVO.DESCR.Trim()));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }

                                            //Reingreso
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Reingreso:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(a.INGRESO.FEC_INGRESO_CERESO, false));
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                    #endregion

                                    if (cp.SENTENCIA != null)
                                    {
                                        #region Historico
                                        var historico = cp.SENTENCIA.Where(w => w.ESTATUS == "I").OrderBy(w => w.ID_SENTENCIA);
                                        if (historico != null)
                                        {
                                            foreach (var h in historico)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                                string primera = string.Empty;
                                                if (h.ANIOS != null && h.ANIOS > 0)
                                                    primera = primera + string.Format(" {0} Años", h.ANIOS);
                                                if (h.MESES != null && h.MESES > 0)
                                                    primera = primera + string.Format(" {0} Meses", h.MESES);
                                                if (h.DIAS != null && h.DIAS > 0)
                                                    primera = primera + string.Format(" {0} Dias", h.DIAS);
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(h.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                                t.InsertRow();
                                                i++;

                                                if (cp.RECURSO != null)
                                                {
                                                    var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    if (rec != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;

                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                    else
                                                    {
                                                        var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                        if (recRevoca == null)
                                                        {
                                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                            t.InsertRow();
                                                            i++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                //Segunda Instancia
                                                var seg = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (seg != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (seg.SENTENCIA_ANIOS != null && seg.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", seg.SENTENCIA_ANIOS);
                                                    if (seg.SENTENCIA_MESES != null && seg.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", seg.SENTENCIA_MESES);
                                                    if (seg.SENTENCIA_DIAS != null && seg.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", seg.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(seg.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + seg.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                //Reposicion de Procedimiento
                                                //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                //if (rec1 != null)
                                                //{
                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                //    t.InsertRow();
                                                //    i++;

                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                //    t.InsertRow();
                                                //    i++;
                                                //}

                                                var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                if (rec3 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: "+rec3.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                if (rec4 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: "+rec4.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                if (recurso2 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                /////////////////////
                                                var inc = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (inc != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (inc.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", inc.MODIFICA_PENA_ANIO);
                                                    if (inc.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", inc.MODIFICA_PENA_MES);
                                                    if (inc.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", inc.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                /////////////////////
                                                var amdir = cp.AMPARO_DIRECTO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_SEN_AMP_RESULTADO == 3).FirstOrDefault();
                                                if (amdir != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Directo:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Reposicion de Procedimiento");
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        #endregion

                                        foreach (var s in cp.SENTENCIA.Where(w => w.ESTATUS == "A"))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                            string primera = string.Empty;
                                            if (s.ANIOS != null && s.ANIOS > 0)
                                                primera = primera + string.Format(" {0} Años", s.ANIOS);
                                            if (s.MESES != null && s.MESES > 0)
                                                primera = primera + string.Format(" {0} Meses", s.MESES);
                                            if (s.DIAS != null && s.DIAS > 0)
                                                primera = primera + string.Format(" {0} Dias", s.DIAS);
                                            t.Rows[i].Cells[1].Paragraphs.First().Append("Fecha Ejecutoria:"+Fechas.fechaLetra(s.FEC_EJECUTORIA, false) + "," + primera);
                                            t.InsertRow();
                                            i++;

                                            if (!string.IsNullOrEmpty(s.MULTA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.MULTA, s.MULTA_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.REPARACION_DANIO))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.REPARACION_DANIO, s.REPARACION_DANIO_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSTITUCION_PENA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitución de la pena:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.SUSTITUCION_PENA, s.SUSTITUCION_PENA_PAGADA == "S" ? ", PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSPENSION_CONDICIONAL))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(s.SUSPENSION_CONDICIONAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            string abonos = string.Empty;
                                            if (s.ANIOS_ABONADOS != null && s.ANIOS_ABONADOS > 0)
                                                abonos = string.Format("{0} Años ", s.ANIOS_ABONADOS);
                                            if (s.MESES_ABONADOS != null && s.MESES_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Meses ", s.MESES_ABONADOS);
                                            if (s.DIAS_ABONADOS != null && s.DIAS_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Dias ", s.DIAS_ABONADOS);

                                            if (!string.IsNullOrEmpty(abonos))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(abonos);
                                                t.InsertRow();
                                                i++;
                                            }

                                            #region Recursos
                                            if (cp.RECURSO != null)
                                            {
                                                var recurso = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (recurso != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                                                    if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                                                    if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + recurso.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.REPARACION_DANIO))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.REPARACION_DANIO);
                                                        t.InsertRow();
                                                        i++;
                                                    }


                                                    if (!string.IsNullOrEmpty(recurso.SUSTITUCION_PENA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.SUSTITUCION_PENA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA_CONDICIONAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                    //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    //if (rec1 != null)
                                                    //{
                                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                    //    t.InsertRow();
                                                    //    i++;

                                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                    //    t.InsertRow();
                                                    //    i++;
                                                    //}

                                                    var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                    if (rec3 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                    if (rec4 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                    if (recurso2 != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region Incidente
                                            if (cp.AMPARO_INCIDENTE != null)
                                            {
                                                var incidente = cp.AMPARO_INCIDENTE.Where(w =>w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (incidente != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (incidente.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                                                    if (incidente.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                                                    if (incidente.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    #region comentado
                                    //else
                                    //{
                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de PrimeraInstancia:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Empty);
                                    //    t.InsertRow();
                                    //    i++;
                                    //}
                                    #endregion
                                   
                                    t.RemoveRow(i);
                                    document.InsertTable(t);
                                }
                            }
                            #endregion

                            #region Causa penal concluidas
                            foreach (var cp in causasPenales.Where(w => w.ID_ESTATUS_CP == 4 && w.ID_INGRESO == SelectedIngreso.ID_INGRESO))
                            {
                                if (cp_usadas.Count(x => x.ID_INGRESO == cp.ID_INGRESO && x.ID_CAUSA_PENAL == cp.ID_CAUSA_PENAL && x.CP_JUZGADO == cp.CP_JUZGADO) == 0)
                                {
                                    Novacode.Paragraph p6 = document.InsertParagraph();
                                    p6.AppendLine();
                                    int i = 0;
                                    Novacode.Table t = document.AddTable(1, 2);//(17, 2);
                                    float[] x = { 350, 350 };
                                    t.SetWidths(x);
                                    t.Alignment = Alignment.center;
                                    t.Design = TableDesign.TableNormal;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Causa Penal").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                    t.Rows[i].Height = t.Rows[i].MinHeight = 25;
                                    t.Rows[i].Height = 5;
                                    t.InsertRow();
                                    i++;

                                    if (cp.NUC != null)
                                    {
                                        if (!string.IsNullOrEmpty(cp.NUC.ID_NUC))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("NUC").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(cp.NUC.ID_NUC);
                                            t.InsertRow();
                                            i++;
                                        }
                                    }

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Juzgado").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.JUZGADO.DESCR.Trim());
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Delito").Bold();
                                    string delitos = string.Empty;
                                    if (cp.SENTENCIA != null)
                                    {
                                        var sa = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                        if (sa != null)
                                        {
                                            if (sa.SENTENCIA_DELITO != null)
                                            {
                                                foreach (var d in sa.SENTENCIA_DELITO)
                                                {
                                                    if (!string.IsNullOrEmpty(delitos))
                                                        delitos = delitos + ",";
                                                    delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                                }
                                            }
                                        }
                                    }
                                    //if (cp.CAUSA_PENAL_DELITO != null)
                                    //{
                                    //    foreach (var d in cp.CAUSA_PENAL_DELITO)
                                    //    {
                                    //        if (!string.IsNullOrEmpty(delitos))
                                    //            delitos = delitos + ",";
                                    //        delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                    //    }
                                    //}
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(delitos);
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al " + centro.DESCR.Trim()).Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(cp.INGRESO.FEC_INGRESO_CERESO, false));
                                    t.InsertRow();
                                    i++;

                                    if (cp.ID_INGRESO > 1)
                                    {
                                        var anterior = ingresos.Where(w => w.ID_INGRESO == (cp.ID_INGRESO - 1)).FirstOrDefault();
                                        if (anterior != null)
                                        {
                                            if (anterior.ID_ESTATUS_ADMINISTRATIVO == 5)//viene de un traslado
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Procedencia").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(anterior.CENTRO1.DESCR.Trim());
                                                t.InsertRow();
                                                i++;

                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al Centro de Origen:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(anterior.FEC_INGRESO_CERESO, false));
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }

                                    #region Bajas y Reingresos
                                    var anteriores = causasPenales.Where(w => w.CP_ANIO == cp.CP_ANIO && w.CP_FOLIO == cp.CP_FOLIO && w.ID_INGRESO > cp.ID_INGRESO).OrderByDescending(w => w.ID_INGRESO);
                                    if (anteriores != null)
                                    {
                                        foreach (var a in anteriores)
                                        {
                                            cp_usadas.Add(new CAUSA_PENAL()
                                            {
                                                ID_INGRESO = a.ID_INGRESO,
                                                ID_CAUSA_PENAL = a.ID_CAUSA_PENAL
                                            });

                                            if (a.LIBERACION != null)
                                            {
                                                var l = a.LIBERACION.FirstOrDefault();
                                                if (l != null)
                                                {
                                                    //Baja
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Baja:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", Fechas.fechaLetra(l.LIBERACION_FEC, false), l.LIBERACION_MOTIVO.DESCR.Trim()));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }

                                            //Reingreso
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Reingreso:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(a.INGRESO.FEC_INGRESO_CERESO, false));
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                    #endregion

                                    if (cp.SENTENCIA != null)
                                    {
                                        #region Historico
                                        var historico = cp.SENTENCIA.Where(w => w.ESTATUS == "I").OrderBy(w => w.ID_SENTENCIA);
                                        if (historico != null)
                                        {
                                            foreach (var h in historico)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                                string primera = string.Empty;
                                                if (h.ANIOS != null && h.ANIOS > 0)
                                                    primera = primera + string.Format(" {0} Años", h.ANIOS);
                                                if (h.MESES != null && h.MESES > 0)
                                                    primera = primera + string.Format(" {0} Meses", h.MESES);
                                                if (h.DIAS != null && h.DIAS > 0)
                                                    primera = primera + string.Format(" {0} Dias", h.DIAS);
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(h.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                                t.InsertRow();
                                                i++;

                                                if (cp.RECURSO != null)
                                                {
                                                    var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    if (rec != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;

                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                    else
                                                    {
                                                        var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                        if (recRevoca == null)
                                                        {
                                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                            t.InsertRow();
                                                            i++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }


                                                //Segunda Instancia
                                                var seg = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (seg != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (seg.SENTENCIA_ANIOS != null && seg.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", seg.SENTENCIA_ANIOS);
                                                    if (seg.SENTENCIA_MESES != null && seg.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", seg.SENTENCIA_MESES);
                                                    if (seg.SENTENCIA_DIAS != null && seg.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", seg.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(seg.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + seg.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                //if (rec1 != null)
                                                //{
                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                //    t.InsertRow();
                                                //    i++;

                                                //   t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                //    t.InsertRow();
                                                //    i++;
                                                //}

                                                var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                if (rec3 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                if (rec4 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                if (recurso2 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                /////////////////////
                                                var inc = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (inc != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (inc.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", inc.MODIFICA_PENA_ANIO);
                                                    if (inc.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", inc.MODIFICA_PENA_MES);
                                                    if (inc.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", inc.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                /////////////////////
                                                var amdir = cp.AMPARO_DIRECTO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_SEN_AMP_RESULTADO == 3).FirstOrDefault();
                                                if (amdir != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Directo:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Reposicion de Procedimiento");
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        #endregion

                                        foreach (var s in cp.SENTENCIA.Where(w => w.ESTATUS == "A"))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().AppendLine("Sentencia de Primera Instancia:").Bold();
                                            string primera = string.Empty;
                                            if (s.ANIOS != null && s.ANIOS > 0)
                                                primera = primera + string.Format(" {0} Años", s.ANIOS);
                                            if (s.MESES != null && s.MESES > 0)
                                                primera = primera + string.Format(" {0} Meses", s.MESES);
                                            if (s.DIAS != null && s.DIAS > 0)
                                                primera = primera + string.Format(" {0} Dias", s.DIAS);
                                            t.Rows[i].Cells[1].Paragraphs.First().AppendLine(Fechas.fechaLetra(s.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                            t.InsertRow();
                                            i++;

                                            if (cp.RECURSO != null)
                                            {
                                                var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                if (rec != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;

                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                                else
                                                {
                                                    var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                    if (recRevoca == null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.MULTA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.MULTA, s.MULTA_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.REPARACION_DANIO))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.REPARACION_DANIO, s.REPARACION_DANIO_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSTITUCION_PENA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitución de la pena:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.SUSTITUCION_PENA, s.SUSTITUCION_PENA_PAGADA == "S" ? ", PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSPENSION_CONDICIONAL))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(s.SUSPENSION_CONDICIONAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            string abonos = string.Empty;
                                            if (s.ANIOS_ABONADOS != null && s.ANIOS_ABONADOS > 0)
                                                abonos = string.Format("{0} Años ", s.ANIOS_ABONADOS);
                                            if (s.MESES_ABONADOS != null && s.MESES_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Meses ", s.MESES_ABONADOS);
                                            if (s.DIAS_ABONADOS != null && s.DIAS_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Dias ", s.DIAS_ABONADOS);

                                            if (!string.IsNullOrEmpty(abonos))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(abonos);
                                                t.InsertRow();
                                                i++;
                                            }

                                            #region Recurso
                                            if (cp.RECURSO != null)
                                            {
                                                var recurso = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (recurso != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                                                    if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                                                    if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + recurso.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.REPARACION_DANIO))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.REPARACION_DANIO);
                                                        t.InsertRow();
                                                        i++;
                                                    }


                                                    if (!string.IsNullOrEmpty(recurso.SUSTITUCION_PENA))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.SUSTITUCION_PENA);
                                                        t.InsertRow();
                                                        i++;
                                                    }

                                                    if (!string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA_CONDICIONAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }

                                                //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                //if (rec1 != null)
                                                //{
                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                //    t.InsertRow();
                                                //    i++;

                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                //    t.InsertRow();
                                                //    i++;
                                                //}

                                                var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                if (rec3 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                if (rec4 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                if (recurso2 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                            #endregion

                                            #region Incidente
                                            if (cp.AMPARO_INCIDENTE != null)
                                            {
                                                var incidente = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (incidente != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (incidente.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                                                    if (incidente.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                                                    if (incidente.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    #region comentado
                                    //else
                                    //{
                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de PrimeraInstancia:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Empty);
                                    //    t.InsertRow();
                                    //    i++;
                                    //}
                                    #endregion
                                    
                                    t.RemoveRow(i);
                                    document.InsertTable(t);
                                }
                            }
                            #endregion

                            #region Causas penales de ingresos anteriores
                            foreach (var cp in causasPenales.Where(w => w.ID_ESTATUS_CP == 4 && w.ID_INGRESO != SelectedIngreso.ID_INGRESO))
                            {
                                if (cp_usadas.Count(x => x.ID_INGRESO == cp.ID_INGRESO && x.ID_CAUSA_PENAL == cp.ID_CAUSA_PENAL) == 0)
                                {
                                    Novacode.Paragraph p6 = document.InsertParagraph();
                                    p6.AppendLine();
                                    int i = 0;
                                    Novacode.Table t = document.AddTable(1, 2);//(17, 2);
                                    float[] x = { 350, 350 };
                                    t.SetWidths(x);
                                    t.Alignment = Alignment.center;
                                    t.Design = TableDesign.TableNormal;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Causa Penal").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                    t.Rows[i].Height = t.Rows[i].MinHeight = 25;
                                    t.Rows[i].Height = 5;
                                    t.InsertRow();
                                    i++;

                                    if (cp.NUC != null)
                                    {
                                        if (!string.IsNullOrEmpty(cp.NUC.ID_NUC))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("NUC").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(cp.NUC.ID_NUC);
                                            t.InsertRow();
                                            i++;
                                        }
                                    }

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Juzgado").Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(cp.JUZGADO.DESCR.Trim());
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Delito").Bold();
                                    string delitos = string.Empty;
                                    if (cp.SENTENCIA != null)
                                    {
                                        var sa = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                        if (sa != null)
                                        {
                                            if (sa.SENTENCIA_DELITO != null)
                                            {
                                                foreach (var d in sa.SENTENCIA_DELITO)
                                                {
                                                    if (!string.IsNullOrEmpty(delitos))
                                                        delitos = delitos + ",";
                                                    delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                                }
                                            }
                                        }
                                    }
                                    //if (cp.CAUSA_PENAL_DELITO != null)
                                    //{
                                    //    foreach (var d in cp.CAUSA_PENAL_DELITO)
                                    //    {
                                    //        if (!string.IsNullOrEmpty(delitos))
                                    //            delitos = delitos + ",";
                                    //        delitos = delitos + string.Format("{0} {1}", d.MODALIDAD_DELITO.DESCR.Trim(), d.MODALIDAD_DELITO.DELITO.DESCR.Trim());
                                    //    }
                                    //}
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(delitos);
                                    t.InsertRow();
                                    i++;

                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al " + centro.DESCR.Trim()).Bold();
                                    t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(cp.INGRESO.FEC_INGRESO_CERESO, false));
                                    t.InsertRow();
                                    i++;

                                    if (cp.ID_INGRESO > 1)
                                    {
                                        var anterior = ingresos.Where(w => w.ID_INGRESO == (cp.ID_INGRESO - 1)).FirstOrDefault();
                                        if (anterior != null)
                                        {
                                            if (anterior.ID_ESTATUS_ADMINISTRATIVO == 5)//viene de un traslado
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Procedencia").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(anterior.CENTRO1.DESCR.Trim());
                                                t.InsertRow();
                                                i++;

                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ingreso al Centro de Origen:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(anterior.FEC_INGRESO_CERESO, false));
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }

                                    #region Bajas y Reingresos
                                    var anteriores = causasPenales.Where(w => w.CP_ANIO == cp.CP_ANIO && w.CP_FOLIO == cp.CP_FOLIO && w.ID_INGRESO > cp.ID_INGRESO).OrderByDescending(w => w.ID_INGRESO);
                                    if (anteriores != null)
                                    {
                                        foreach (var a in anteriores)
                                        {
                                            cp_usadas.Add(new CAUSA_PENAL()
                                            {
                                                ID_INGRESO = a.ID_INGRESO,
                                                ID_CAUSA_PENAL = a.ID_CAUSA_PENAL
                                            });

                                            if (a.LIBERACION != null)
                                            {
                                                var l = a.LIBERACION.FirstOrDefault();
                                                if (l != null)
                                                {
                                                    //Baja
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Baja:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", Fechas.fechaLetra(l.LIBERACION_FEC, false), l.LIBERACION_MOTIVO.DESCR.Trim()));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }

                                            //Reingreso
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Reingreso:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(a.INGRESO.FEC_INGRESO_CERESO, false));
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                    #endregion

                                    if (cp.SENTENCIA != null)
                                    {
                                        #region Historico
                                        var historico = cp.SENTENCIA.Where(w => w.ESTATUS == "I").OrderBy(w => w.ID_SENTENCIA);
                                        if (historico != null)
                                        {
                                            foreach (var h in historico)
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Primera Instancia:").Bold();
                                                string primera = string.Empty;
                                                if (h.ANIOS != null && h.ANIOS > 0)
                                                    primera = primera + string.Format(" {0} Años", h.ANIOS);
                                                if (h.MESES != null && h.MESES > 0)
                                                    primera = primera + string.Format(" {0} Meses", h.MESES);
                                                if (h.DIAS != null && h.DIAS > 0)
                                                    primera = primera + string.Format(" {0} Dias", h.DIAS);
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(h.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                                t.InsertRow();
                                                i++;

                                                if (cp.RECURSO != null)
                                                {
                                                    var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                    if (rec != null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;

                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                    else
                                                    {
                                                        var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                        if (recRevoca == null)
                                                        {
                                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                            t.InsertRow();
                                                            i++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", h.FEC_EJECUTORIA.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }


                                                //Segunda Instancia
                                                var seg = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                                if (seg != null)
                                                {
                                                    string segunda = string.Empty;
                                                    if (seg.SENTENCIA_ANIOS != null && seg.SENTENCIA_ANIOS > 0)
                                                        segunda = segunda + string.Format(" {0} Años", seg.SENTENCIA_ANIOS);
                                                    if (seg.SENTENCIA_MESES != null && seg.SENTENCIA_MESES > 0)
                                                        segunda = segunda + string.Format(" {0} Meses", seg.SENTENCIA_MESES);
                                                    if (seg.SENTENCIA_DIAS != null && seg.SENTENCIA_DIAS > 0)
                                                        segunda = segunda + string.Format(" {0} Dias", seg.SENTENCIA_DIAS);

                                                    if (!string.IsNullOrEmpty(segunda))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(seg.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + seg.TOCA_PENAL);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                                //if (rec1 != null)
                                                //{
                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                                //    t.InsertRow();
                                                //    i++;

                                                //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                                //    t.InsertRow();
                                                //    i++;
                                                //}

                                                var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                                if (rec3 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                                if (rec4 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                                if (recurso2 != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;
                                                }

                                                /////////////////////
                                                var inc = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                                if (inc != null)
                                                {
                                                    string adecuacion = string.Empty;
                                                    if (inc.MODIFICA_PENA_ANIO > 0)
                                                        adecuacion = string.Format("{0} Años ", inc.MODIFICA_PENA_ANIO);
                                                    if (inc.MODIFICA_PENA_MES > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Meses ", inc.MODIFICA_PENA_MES);
                                                    if (inc.MODIFICA_PENA_DIA > 0)
                                                        adecuacion = adecuacion + string.Format("{0} Dias ", inc.MODIFICA_PENA_DIA);
                                                    if (!string.IsNullOrEmpty(adecuacion))
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                                /////////////////////
                                                var amdir = cp.AMPARO_DIRECTO.Where(w => w.ID_SENTENCIA == h.ID_SENTENCIA && w.ID_SEN_AMP_RESULTADO == 3).FirstOrDefault();
                                                if (amdir != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Directo:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Reposicion de Procedimiento");
                                                    t.InsertRow();
                                                    i++;
                                                }
                                            }
                                        }
                                        #endregion

                                        foreach (var s in cp.SENTENCIA.Where(w => w.ESTATUS == "A"))
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().AppendLine("Sentencia de Primera Instancia:").Bold();
                                            string primera = string.Empty;
                                            if (s.ANIOS != null && s.ANIOS > 0)
                                                primera = primera + string.Format(" {0} Años", s.ANIOS);
                                            if (s.MESES != null && s.MESES > 0)
                                                primera = primera + string.Format(" {0} Meses", s.MESES);
                                            if (s.DIAS != null && s.DIAS > 0)
                                                primera = primera + string.Format(" {0} Dias", s.DIAS);
                                            t.Rows[i].Cells[1].Paragraphs.First().AppendLine(Fechas.fechaLetra(s.FEC_INICIO_COMPURGACION, false) + "," + primera);
                                            t.InsertRow();
                                            i++;

                                            if (cp.RECURSO != null)
                                            {
                                                var rec = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1" && w.ID_SENTENCIA == s.ID_SENTENCIA).FirstOrDefault();
                                                if (rec != null)
                                                {
                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma, con Toca Penal: " + rec.TOCA_PENAL);
                                                    t.InsertRow();
                                                    i++;

                                                    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec.FEC_RESOLUCION.Value));
                                                    t.InsertRow();
                                                    i++;
                                                }
                                                else
                                                {
                                                    var recRevoca = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && (w.RESULTADO == "2" || w.RESULTADO == "3")).FirstOrDefault();
                                                    if (recRevoca == null)
                                                    {
                                                        t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                        t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                        t.InsertRow();
                                                        i++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", s.FEC_EJECUTORIA.Value));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.MULTA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.MULTA, s.MULTA_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.REPARACION_DANIO))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.REPARACION_DANIO, s.REPARACION_DANIO_PAGADA == "S" ? "PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSTITUCION_PENA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitución de la pena:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0}, {1}", s.SUSTITUCION_PENA, s.SUSTITUCION_PENA_PAGADA == "S" ? ", PAGADA" : "NO PAGADA"));
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(s.SUSPENSION_CONDICIONAL))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(s.SUSPENSION_CONDICIONAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            string abonos = string.Empty;
                                            if (s.ANIOS_ABONADOS != null && s.ANIOS_ABONADOS > 0)
                                                abonos = string.Format("{0} Años ", s.ANIOS_ABONADOS);
                                            if (s.MESES_ABONADOS != null && s.MESES_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Meses ", s.MESES_ABONADOS);
                                            if (s.DIAS_ABONADOS != null && s.DIAS_ABONADOS > 0)
                                                abonos = abonos + string.Format("{0} Dias ", s.DIAS_ABONADOS);

                                            if (!string.IsNullOrEmpty(abonos))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(abonos);
                                                t.InsertRow();
                                                i++;
                                            }

                                            #region Recurso
                                            if (cp.RECURSO != null)
                                            {
                                        var recurso = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "2").FirstOrDefault();
                                        if (recurso != null)
                                        {
                                            string segunda = string.Empty;
                                            if (recurso.SENTENCIA_ANIOS != null && recurso.SENTENCIA_ANIOS > 0)
                                                segunda = segunda + string.Format(" {0} Años", recurso.SENTENCIA_ANIOS);
                                            if (recurso.SENTENCIA_MESES != null && recurso.SENTENCIA_MESES > 0)
                                                segunda = segunda + string.Format(" {0} Meses", recurso.SENTENCIA_MESES);
                                            if (recurso.SENTENCIA_DIAS != null && recurso.SENTENCIA_DIAS > 0)
                                                segunda = segunda + string.Format(" {0} Dias", recurso.SENTENCIA_DIAS);

                                            if (!string.IsNullOrEmpty(segunda))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de Segunda Instancia:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(Fechas.fechaLetra(recurso.FEC_RECURSO, false) + ", " + segunda + ", con Toca Penal: " + recurso.TOCA_PENAL);
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(recurso.MULTA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA);
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(recurso.REPARACION_DANIO))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.REPARACION_DANIO);
                                                t.InsertRow();
                                                i++;
                                            }


                                            if (!string.IsNullOrEmpty(recurso.SUSTITUCION_PENA))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.SUSTITUCION_PENA);
                                                t.InsertRow();
                                                i++;
                                            }

                                            if (!string.IsNullOrEmpty(recurso.MULTA_CONDICIONAL))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(recurso.MULTA_CONDICIONAL);
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                        //var rec1 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "1").FirstOrDefault();
                                        //if (rec1 != null)
                                        //{
                                        //    t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinación Segunda Instancia:").Bold();
                                        //    t.Rows[i].Cells[1].Paragraphs.First().Append("Confirma");
                                        //    t.InsertRow();
                                        //    i++;

                                        //    t.Rows[i].Cells[0].Paragraphs.First().Append("Ejecutoria:").Bold();
                                        //            t.Rows[i].Cells[1].Paragraphs.First().Append(string.Format("{0:dd/MM/yyyy}", rec1.FEC_RESOLUCION.Value));
                                        //            t.InsertRow();
                                        //            i++;
                                        //}

                                        var rec3 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "3").FirstOrDefault();
                                        if (rec3 != null)
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + rec3.TOCA_PENAL);
                                            t.InsertRow();
                                            i++;
                                        }

                                        var rec4 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 2 && w.RESULTADO == "4").FirstOrDefault();
                                        if (rec4 != null)
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Apelación o Determinacion de Segunda Instancia:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append("Libertad por Revocación de Sentencia, con Toca Penal: " + rec4.TOCA_PENAL);
                                            t.InsertRow();
                                            i++;
                                        }

                                        var recurso2 = cp.RECURSO.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_TIPO_RECURSO == 3 && w.RESULTADO == "3").FirstOrDefault();
                                        if (recurso2 != null)
                                        {
                                            t.Rows[i].Cells[0].Paragraphs.First().Append("Amparo Contra Apelacion o Segunda Instancia:").Bold();
                                            t.Rows[i].Cells[1].Paragraphs.First().Append("Revoca con Reposicion de Procedimiento, con Toca Penal: " + recurso2.TOCA_PENAL);
                                            t.InsertRow();
                                            i++;
                                        }
                                    }
                                            #endregion

                                            #region Incidente
                                    if (cp.AMPARO_INCIDENTE != null)
                                    {
                                        var incidente = cp.AMPARO_INCIDENTE.Where(w => w.ID_SENTENCIA == s.ID_SENTENCIA && w.ID_AMP_INC_TIPO == 3 && w.RESULTADO == "M").FirstOrDefault();
                                        if (incidente != null)
                                        {
                                            string adecuacion = string.Empty;
                                            if (incidente.MODIFICA_PENA_ANIO > 0)
                                                adecuacion = string.Format("{0} Años ", incidente.MODIFICA_PENA_ANIO);
                                            if (incidente.MODIFICA_PENA_MES > 0)
                                                adecuacion = adecuacion + string.Format("{0} Meses ", incidente.MODIFICA_PENA_MES);
                                            if (incidente.MODIFICA_PENA_DIA > 0)
                                                adecuacion = adecuacion + string.Format("{0} Dias ", incidente.MODIFICA_PENA_DIA);
                                            if (!string.IsNullOrEmpty(adecuacion))
                                            {
                                                t.Rows[i].Cells[0].Paragraphs.First().Append("Incidente de adecuación:").Bold();
                                                t.Rows[i].Cells[1].Paragraphs.First().Append(adecuacion);
                                                t.InsertRow();
                                                i++;
                                            }
                                        }
                                    }

                                            #endregion
                                        }
                                    }
                                    #region comentado
                                    //else
                                    //{
                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sentencia de PrimeraInstancia:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Multa:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Reparación del daño:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Sustitucion de la pena:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Suspensión condicional:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(" ");
                                    //    t.InsertRow();
                                    //    i++;

                                    //    t.Rows[i].Cells[0].Paragraphs.First().Append("Abonos:").Bold();
                                    //    t.Rows[i].Cells[1].Paragraphs.First().Append(string.Empty);
                                    //    t.InsertRow();
                                    //    i++;
                                    //}
                                    #endregion
                                    

                                    t.RemoveRow(i);
                                    document.InsertTable(t);
                                }
                                
                            }
                            #endregion

                        }
                        #endregion

                        #region Firma del director de Centro
                        Novacode.Paragraph pFooter = document.InsertParagraph();
                        pFooter.Alignment = Alignment.center;
                        pFooter.AppendLine();
                        pFooter.AppendLine();
                        pFooter.AppendLine();
                        pFooter.AppendLine("A T E N T A M E N T E").Bold();
                        pFooter.AppendLine(!string.IsNullOrEmpty(centro.DIRECTOR) ? centro.DIRECTOR.Trim().ToUpper() : string.Empty).Bold();
                        pFooter.AppendLine(string.Format("DIRECTOR DEL {0}",!string.IsNullOrEmpty(centro.DESCR) ? centro.DESCR.Trim().ToUpper() : string.Empty)).Bold();
                        #endregion

                        #region Footer

                        Footer footer_default = document.Footers.odd;
                        Novacode.Paragraph p7 = footer_default.InsertParagraph();
                        //p7.Append("pie de pagina").Bold();
                        //p6.AppendPageCount(PageNumberFormat.normal);
                        //p6.AppendPageNumber(PageNumberFormat.normal);
                        //p6.Append().Bold();
                        #endregion
                        document.Save();

                        byte[] bytes = stream.ToArray();

                        var tc = new TextControlView();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML);
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
                        //Process.Start("WINWORD.EXE", FileName);
                    }// Release this document from memory.
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar a un interno");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
            }
        }
        #endregion

        #region Causa Penal de ingreso anterior
        /// <summary>
        /// Valida si la causa penal exise en un ingreso anterior, si existe pregunta si quiere que muestre la informacion capturada
        /// en esta
        /// </summary>
        private async void ValidarCausaPenalIngresoAnterior()
        {
            try
            {
                if (AnioCP.HasValue && FolioCP.HasValue)
                { 
                    if(SelectedIngreso != null)
                    {
                        if(SelectedIngreso.ID_INGRESO > 1)
                        {
                            short ingreso = SelectedIngreso.ID_INGRESO;
                            ingreso--;
                            var cp = new cCausaPenal().Obtener(
                                SelectedIngreso.ID_INGRESO,
                                SelectedIngreso.ID_ANIO,
                                SelectedIngreso.ID_IMPUTADO,
                                ingreso,
                                null,
                                AnioCP,
                                FolioCP).FirstOrDefault();
                            //si encuentra una causa penal igual en el ingreso anterior pregunta si quiere mostrar la informacion anterior
                            if (cp != null)
                            {
                                if (cp.SENTENCIA == null)
                                {
                                    if (await new Dialogos().ConfirmarEliminar("Validación", "La causa penal se encuentra registrada en el ingreso anterior,¿Desea mostrar información?") == 1)
                                    {
                                        BisCP = cp.CP_BIS;
                                        ForaneoCP = cp.CP_FORANEO;
                                        TipoOrdenCP = cp.CP_TIPO_ORDEN != null ? cp.CP_TIPO_ORDEN : -1;
                                        PaisJuzgadoCP = cp.CP_PAIS_JUZGADO != null ? cp.CP_PAIS_JUZGADO : -1;
                                        EstadoJuzgadoCP = cp.CP_ESTADO_JUZGADO != null ? cp.CP_ESTADO_JUZGADO : -1;
                                        MunicipioJuzgadoCP = cp.CP_MUNICIPIO_JUZGADO != null ? cp.CP_MUNICIPIO_JUZGADO : -1;
                                        FueroCP = !string.IsNullOrEmpty(cp.CP_FUERO) ? cp.CP_FUERO : string.Empty;
                                        JuzgadoCP = cp.CP_JUZGADO != null ? cp.CP_JUZGADO : -1;
                                        FecRadicacionCP = cp.CP_FEC_RADICACION;
                                        AmpliacionCP = !string.IsNullOrEmpty(cp.CP_AMPLIACION) ? cp.CP_AMPLIACION : string.Empty;
                                        TerminoCP = cp.CP_TERMINO != null ? cp.CP_TERMINO : -1;
                                        EstatusCP = cp.ID_ESTATUS_CP != null ? cp.ID_ESTATUS_CP : -1;
                                        FecVencimientoTerinoCP = cp.CP_FEC_VENCIMIENTO_TERMINO;
                                        //DELITO
                                        if (cp.CAUSA_PENAL_DELITO != null)
                                            LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>(cp.CAUSA_PENAL_DELITO);
                                        else
                                            LstCausaPenalDelitos = new ObservableCollection<CAUSA_PENAL_DELITO>();
                                        if (LstCausaPenalDelitos != null)
                                        {
                                            if (LstCausaPenalDelitos.Count > 0)
                                                CausaPenalDelitoEmpty = false;
                                            else
                                                CausaPenalDelitoEmpty = true;
                                        }
                                        else
                                        {
                                            CausaPenalDelitoEmpty = true;
                                        }
                                        //COPARTICIPE
                                        LstCoparticipe = new ObservableCollection<COPARTICIPE>(cp.COPARTICIPE);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
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

                            LstBuscarIngreso = new ObservableCollection<INGRESO>();
                            //OBTENERMOS LOS INGRESOS PERTENECIENTES AL CENTRO
                            if (selectExpediente.INGRESO != null)
                                LstBuscarIngreso = new ObservableCollection<INGRESO>(selectExpediente.INGRESO.Where(w => w.ID_UB_CENTRO == GlobalVar.gCentro));
                            else
                                LstBuscarIngreso = new ObservableCollection<INGRESO>();
                            //MUESTRA LOS INGRESOS
                            //if (selectExpediente.INGRESO.Count > 0)
                            if (LstBuscarIngreso != null && LstBuscarIngreso.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = LstBuscarIngreso.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
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
                                ImagenIngreso = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion

    }
}