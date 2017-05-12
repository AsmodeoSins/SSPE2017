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
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using System.Windows.Controls;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using ControlPenales.BiometricoServiceReference;
using Novacode;



namespace ControlPenales
{
    partial class LiberacionViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region constructor
        public LiberacionViewModel() { }
        #endregion

        #region variables
        //public string Name
        //{
        //    get
        //    {
        //        return "liberacion";
        //    }
        //}
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;
                case "buscar_menu":
                    //if (StaticSourcesViewModel.SourceChanged)
                    //{
                    //    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                    //    if (respuesta == 1)
                    //    { 
                    //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA); 
                    //    }
                    //}
                    //else
                    LimpiarBusqueda();
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                        break;
                    }
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        if (await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?") != 1)
                            break;
                    }
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                    }
                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        break;
                    }
                    //SE MANEJA CRITERIO ORIGINAL DE CANDADO DE TRASLADO, YA QUE EL JURIDICO TIENE QUE CANCELAR EL TRASLADO EN CASO DE EXISTIR PARA PODER LIBERAR AL IMPUTADO.
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false ))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede ser liberado.");
                        return;
                    }
                    SelectedIngreso = SelectIngreso;
                    LimpiarEgreso();
                    ObtenerIngreso();
                    SelectIngreso = SelectedIngreso;
                    //CAUSAS PENALES
                    if (SelectedIngreso.CAUSA_PENAL != null)
                    {
                        //Mostramos solo las causa penales en estatus {0 = Por Compurgar, 1 = Activo,4 = Concluido, 6 = En Proceso}
                        LstCausaPenal = new ObservableCollection<CAUSA_PENAL>(SelectedIngreso.CAUSA_PENAL.Where(w => (w.ID_ESTATUS_CP == 0 || w.ID_ESTATUS_CP == 1 || w.ID_ESTATUS_CP == 4 || w.ID_ESTATUS_CP == 6))); // new[] { 0, 1, 4, 6 }.Equals(w.ID_ESTATUS_CP)));
                        CausasPenalesVisible = LstCausaPenal.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else
                        CausasPenalesVisible = Visibility.Visible;
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new LiberacionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.LiberacionViewModel();
                    break;
                case "guardar_menu":
                    if (SelectedIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "Favor de seleccionar un ingreso.");
                    }
                    else
                    {
                        //if (SelectedCausaPenal != null)
                        //{
                            if (!base.HasErrors)
                            {
                                if (GuardarEgreso())
                                {
                                    new Dialogos().ConfirmacionDialogo("EXITO", "Informaci\u00F3n registrada correctamente.");
                                    #region Causa Penal
                                    if (SelectedIngreso.CAUSA_PENAL != null)
                                    {
                                        //Mostramos solo las causa penales en estatus {0 = Por Compurgar, 1 = Activo,4 = Concluido, 6 = En Proceso}
                                        LstCausaPenal = new ObservableCollection<CAUSA_PENAL>(SelectedIngreso.CAUSA_PENAL.Where(w => (w.ID_ESTATUS_CP == 0 || w.ID_ESTATUS_CP == 1 || w.ID_ESTATUS_CP == 4 || w.ID_ESTATUS_CP == 6)));

                                        #region Liberaciones Externas
                                        var liberaciones = new cLiberacion().Obtener(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, null, false);
                                        if (liberaciones != null)
                                        {
                                            foreach (var l in liberaciones)
                                            {
                                                var lista = new List<LIBERACION>();
                                                lista.Add(l);
                                                LstCausaPenal.Add(new CAUSA_PENAL() { ID_CAUSA_PENAL = 0, CP_ANIO = l.CP_ANIO, CP_FOLIO = l.CP_FOLIO, LIBERACION = lista });
                                            }
                                        }
                                        #endregion

                                        CausasPenalesVisible = LstCausaPenal.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                                    }
                                    else
                                        CausasPenalesVisible = Visibility.Visible;
                                    #endregion
                                    #region Proxima Causa Penal
                                    if (SelectedCausaPenal != null)
                                        ActualizarEstatusCausaPenal(false, 4);
                                    //VALIDA CUAL CAUSA PENAL ES LA SIGUIENTE
                                    if (SelectedCausaPenal != null)
                                        lstProximaCausaPenal = new ObservableCollection<CAUSA_PENAL>(SelectedIngreso.CAUSA_PENAL.Where(w => w.ID_CAUSA_PENAL != SelectedCausaPenal.ID_CAUSA_PENAL && w.ID_ESTATUS_CP == 0));//busca la proxima causa penal que esta por compurgar
                                    if (lstProximaCausaPenal != null)
                                    {
                                        if (lstProximaCausaPenal.Count > 0)
                                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.PROXIMA_CAUSA_PENAL);
                                    }
                                    #endregion
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                                else
                                    new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "No se registr\u00F3 la informaci\u00F3n.");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar los campos requeridos");
                        //}
                        //else
                        //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal");
                    }
                    break;
                case "reporte_menu":
                    //ImprimirPreliberacion();
                    ImprimirOrdenLibertad();
                    break;
                case "proxima_causa_penal":
                    if (SelectedProximaCausaPenal != null)
                    {
                        if (ActualizarEstatusCausaPenal())
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PROXIMA_CAUSA_PENAL);
                            new Dialogos().ConfirmacionDialogo("ÉXITO", "El estatus de la causa penal ha cambiado a activa.");
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "No se actualizó el estatus de la causa penal.");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar la próxima causa penal");
                    break;
                case "cancelar_proxima_causa_penal":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.PROXIMA_CAUSA_PENAL);
                    LimpiarProximaCausaPenal();
                    break;
                case "buscar_salir":
                    if (SelectedIngreso != null)
                    {
                        if (SelectedIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                        {
                            ImagenIngreso = SelectedIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                        }
                        else
                            ImagenIngreso = new Imagenes().getImagenPerson();
                    }
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
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
                //TabVisible = false;
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

        private async void ModelEnter(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }

                if (obj != null)
                {
                    if (!obj.GetType().Name.Equals("String"))
                    {

                        //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                        var textbox = obj as System.Windows.Controls.TextBox;
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

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
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
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        //SE MANEJA CRITERIO ORIGINAL DE CANDADO DE TRASLADO, YA QUE EL JURIDICO TIENE QUE CANCELAR EL TRASLADO EN CASO DE EXISTIR PARA PODER LIBERAR AL IMPUTADO.
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                                ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede ser liberado.");
                            return;
                        }
                        SelectExpediente = ListExpediente[0];
                        SelectedIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        LimpiarEgreso();
                        ObtenerIngreso();
                        #region Causa Penal
                        if (SelectedIngreso.CAUSA_PENAL != null)
                        {
                            //Mostramos solo las causa penales en estatus {0 = Por Compurgar, 1 = Activo,4 = Concluido, 6 = En Proceso}
                            LstCausaPenal = new ObservableCollection<CAUSA_PENAL>(SelectedIngreso.CAUSA_PENAL.Where(w => (w.ID_ESTATUS_CP == 0 || w.ID_ESTATUS_CP == 1 || w.ID_ESTATUS_CP == 4 || w.ID_ESTATUS_CP == 6)));

                            #region Liberaciones Externas
                            var liberaciones = new cLiberacion().Obtener(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO, null, false);
                            if (liberaciones != null)
                            {
                                foreach(var l in liberaciones)
                                {
                                    var lista = new List<LIBERACION>();
                                    lista.Add(l);
                                    LstCausaPenal.Add(new CAUSA_PENAL() { ID_CAUSA_PENAL = 0, CP_ANIO = l.CP_ANIO, CP_FOLIO = l.CP_FOLIO, LIBERACION = lista  });
                                }
                            }
                            #endregion

                            CausasPenalesVisible = LstCausaPenal.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                            CausasPenalesVisible = Visibility.Visible;
                        #endregion
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                    else
                    {
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos( ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
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

        private async void OnLoad(LiberacionView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el traslado", ex);
            }
        }

        private void PrepararListas()
        {
            try
            {
                LstMotivo = new ObservableCollection<LIBERACION_MOTIVO>(new cLiberacionMotivo().ObtenerTodos());
                LstAutoridad = new ObservableCollection<LIBERACION_AUTORIDAD>(new cLiberacionAutoridad().ObtenerTodos());
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
               {
                   LstMotivo.Insert(0, new LIBERACION_MOTIVO() { ID_LIBERACION_MOTIVO = -1, DESCR = "SELECCIONE" });
                   LstAutoridad.Insert(0, new LIBERACION_AUTORIDAD() { ID_LIBERACION_AUTORIDAD = -1, DESCR = "SELECCIONE" });
                   SetValidaciones();
                   ConfiguraPermisos();
                   //LimpiarEgreso();
                   EFecha = null;
                   EOficio = string.Empty;
                   EMotivo = EAutoridad = -1;
                   StaticSourcesViewModel.SourceChanged = false;
               }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el traslado", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.LIBERACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                            PImprimir = true;
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

        #region Metodos Buscar
        private void LimpiarBusqueda()
        {
            AnioBuscar = FolioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            SelectExpediente = null;
        }

        private void ObtenerIngreso()
        {
            if (SelectedIngreso != null)
            {
                AnioD = SelectedIngreso.ID_ANIO;
                FolioD = SelectedIngreso.ID_IMPUTADO;
                PaternoD = SelectedIngreso.IMPUTADO.PATERNO;
                MaternoD = SelectedIngreso.IMPUTADO.MATERNO;
                NombreD = SelectedIngreso.IMPUTADO.NOMBRE;
                IngresosD = SelectedIngreso.ID_INGRESO;
                //NoControlD = SelectIngreso
                if (SelectedIngreso.CAMA != null)
                {
                    UbicacionD = string.Format("{0}-{1}{2}-{3}",
                                               SelectedIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                               SelectedIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                               SelectedIngreso.CAMA.CELDA.ID_CELDA.Trim(),
                                               SelectedIngreso.ID_UB_CAMA);
                }
                else
                {
                    UbicacionD = string.Empty;
                }
                TipoSeguridadD = SelectedIngreso.TIPO_SEGURIDAD.DESCR;
                FecIngresoD = SelectedIngreso.FEC_INGRESO_CERESO.Value;
                ClasificacionJuridicaD = SelectedIngreso.CLASIFICACION_JURIDICA.DESCR;
                if (SelectedIngreso.ESTATUS_ADMINISTRATIVO != null)
                    EstatusD = SelectedIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                else
                    EstatusD = string.Empty;

                //ObtenerEgreso();
                LimpiarBusqueda();
            }
        }
        #endregion

        #region MetodosLiberacion
        private void LimpiarEgreso()
        {
            EFecha = null;
            EOficio = string.Empty;
            EMotivo = EAutoridad = -1;
        }

        private void ObtenerEgreso()
        {
            try
            {
                if (SelectedLiberacion != null)
                {
                    EFecha = SelectedLiberacion.LIBERACION_FEC;
                    EOficio = SelectedLiberacion.LIBERACION_OFICIO;
                    EMotivo = SelectedLiberacion.ID_LIBERACION_MOTIVO;
                    EAutoridad = SelectedLiberacion.ID_LIBERACION_AUTORIDAD;
                    if (SelectedLiberacion.CP_FOLIO.HasValue && SelectedLiberacion.CP_ANIO.HasValue)
                    {
                        var anio = SelectedLiberacion.CP_ANIO;
                        var folio = SelectedLiberacion.CP_FOLIO;
                        CPAnio = anio;
                        CPFolio = folio;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener el egreso", ex);
            }
        }

        private bool GuardarEgreso()
        {
            try
            {
                if (SelectIngreso == null)
                    return false;

                    var obj = new LIBERACION();
                    obj.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    obj.ID_ANIO = SelectIngreso.ID_ANIO;
                    obj.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    obj.ID_INGRESO = SelectIngreso.ID_INGRESO;

                    obj.LIBERACION_FEC = EFecha.Value;
                    obj.LIBERACION_OFICIO = EOficio;
                    obj.ID_LIBERACION_AUTORIDAD = EAutoridad;
                    obj.ID_LIBERACION_MOTIVO = EMotivo;
                    //CAUSA PENAL NO REGISTRADA EN EL SISTEMA
                    obj.CP_ANIO = CPAnio;
                    obj.CP_FOLIO = CPFolio;
                    //CAUSA PENAL
                    if (SelectedCausaPenal != null)
                        if (SelectedCausaPenal.ID_CAUSA_PENAL != 0)
                            obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;
                    if (SelectedLiberacion == null)//INSERT
                    {
                        obj.LIBERADO = "N";
                        obj.ID_LIBERACION = new cLiberacion().Insertar(obj);
                        if (obj.ID_LIBERACION > 0)
                        {
                            SelectIngreso.LIBERACION.Add(obj);
                            SelectedLiberacion = obj;
                            return true;
                        }
                    }
                    else//UPDATE
                    {
                        obj.ID_LIBERACION = SelectedLiberacion.ID_LIBERACION;
                        obj.LIBERADO = SelectedLiberacion.LIBERADO;
                        obj.INCIDENTE_BIOMETRICO = SelectedLiberacion.INCIDENTE_BIOMETRICO;
                        if (new cLiberacion().Actualizar(obj, null, false))
                        {
                            return true;
                        }
                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el egreso", ex);
            }
            return false;
        }
        #endregion

        #region ImprimirPreliberacion
        private void ImprimirPreliberacion()
        {
            try
            {
                var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.BOLETA_LIBERTAD); //File.ReadAllBytes(@"C:\libertades\BL.doc");
                if (documento == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación","No se encontro la plantilla del documento");
                    return;
                }
                var diccionario = new Dictionary<string, string>();
                diccionario.Add("foto", @"C:\Users\Desarrollo4\Desktop\snake.jpg");
                diccionario.Add("centro", "Centro de Reinsersción Social Tijuana");
                diccionario.Add("expediente", "2015/87");
                diccionario.Add("comandante", "Comandante General");
                diccionario.Add("ciudad", "Tijuana");
                diccionario.Add("fec_letra", "8 de octubre del 2015");
                diccionario.Add("interno", "Francisco Palencia Hernandez");
                diccionario.Add("alias", "Gatillero Palencia");
                diccionario.Add("tipo_libertad", "Provicional Bajo Causión");
                diccionario.Add("juzgado", "JUEZ CUARTO DE DISTRITO DE PROCESOS PENALES FEDERALES EN EL ESTADO DE BAJA CALIFORNIA, TIJUANA");
                diccionario.Add("fecha", "8/10/2015");
                diccionario.Add("causa_penal", "2015/666");
                diccionario.Add("delito", "CONTRA LA SALUD EN LA MODALIDAD DE POSESIÓN DE CLORHIDRATO DE METANFETAMINA");
                diccionario.Add("director", "Lic.X");
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
                                new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                    }
                };
                tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                tc.Owner = PopUpsViewModels.MainWindow;
                tc.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir preliberación.", ex);
            }
        }

        private void ImprimirOrdenLibertad()
        {
            try
            {
                //if (SelectedCausaPenal != null)
                //{
                    if (SelectedLiberacion != null)
                    {
                        SelectedLiberacion = new cLiberacion().ObtenerLiberado(SelectedLiberacion.ID_CENTRO, SelectedLiberacion.ID_ANIO, SelectedLiberacion.ID_IMPUTADO, SelectedLiberacion.ID_INGRESO, SelectedLiberacion.ID_LIBERACION).FirstOrDefault();

                        var diccionario = new Dictionary<string, string>();
                        diccionario.Add("<<expediente>>", string.Format("{0}/{1}", SelectedLiberacion.INGRESO.ID_ANIO, SelectedLiberacion.INGRESO.ID_IMPUTADO));
                        diccionario.Add("<<folio>>", " ");
                        diccionario.Add("<<ciudad>>", SelectedLiberacion.INGRESO.CENTRO.MUNICIPIO.MUNICIPIO1.Trim());
                        diccionario.Add("<<fecha_letra>>", Fechas.fechaLetra(Fechas.GetFechaDateServer, false));

                        diccionario.Add("<<interno>>", string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectedLiberacion.INGRESO.IMPUTADO.NOMBRE) ? SelectedLiberacion.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedLiberacion.INGRESO.IMPUTADO.PATERNO) ? SelectedLiberacion.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectedLiberacion.INGRESO.IMPUTADO.MATERNO) ? SelectedLiberacion.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty));
                        if (SelectedCausaPenal != null)
                        {
                            diccionario.Add("<<juzgado>>", !string.IsNullOrEmpty(SelectedCausaPenal.JUZGADO.DESCR) ? SelectedCausaPenal.JUZGADO.DESCR.Trim() : " ");
                            diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", SelectedCausaPenal.CP_ANIO, SelectedCausaPenal.CP_FOLIO));

                            string ap = string.Empty;
                            if (SelectedCausaPenal.NUC != null) 
                            { 
                                ap = SelectedCausaPenal.NUC.ID_NUC;
                                if(string.IsNullOrEmpty(ap))
                                    ap = " ";
                            }
                            else
                            {
                                if (SelectedCausaPenal.AP_ANIO != null && SelectedCausaPenal.AP_FOLIO != null)
                                {
                                    ap = string.Format("{0}/{1}", SelectedCausaPenal.AP_ANIO, SelectedCausaPenal.AP_FOLIO);
                                }
                                else
                                    ap = " ";
                            }
                            
                            diccionario.Add("<<averiguacion_previa>>", ap);
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
                            diccionario.Add("<<delitos>>", !string.IsNullOrEmpty(delitos) ? delitos : " ");
                        }
                        else
                        {
                            diccionario.Add("<<juzgado>>", " ");
                            diccionario.Add("<<causa_penal>>", string.Format("{0}/{1}", SelectedLiberacion.CP_ANIO, SelectedLiberacion.CP_FOLIO));
                            diccionario.Add("<<averiguacion_previa>>", " ");
                            diccionario.Add("<<delitos>>", " ");
                        }
                       
                        diccionario.Add("<<motivo_libertad>>", SelectedLiberacion.LIBERACION_MOTIVO.DESCR);
                        diccionario.Add("<<autoridad_concede>>", SelectedLiberacion.LIBERACION_AUTORIDAD.DESCR);

                        #region Observaciones
                        var cps = new cCausaPenal().Obtener(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO);
                        if (cps != null)
                        {
                            cps = cps.Where(w => w.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO || w.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.POR_COMPURGAR);
                            if (cps != null)
                            {
                                string observacion = string.Empty;
                                if(cps.Count() > 0)
                                    observacion = "SE QUEDA, ";
                                foreach (var c in cps)
                                {
                                    observacion = string.Concat(observacion, string.Format("CAUSA PENAL: {0}/{1}, ",c.CP_ANIO,c.CP_FOLIO));
                                    if (c.CAUSA_PENAL_DELITO != null)
                                    {
                                        observacion = observacion + " DELITO(S): ";
                                        foreach (var d in c.CAUSA_PENAL_DELITO)
                                        {
                                            observacion = string.Concat(observacion, string.Format("{0}, ", d.DESCR_DELITO.Trim()));
                                        }
                                    }
                                    if(c.CP_JUZGADO != null)
                                        observacion = string.Concat(observacion, string.Format("JUZGADO: {0}, ", c.JUZGADO.DESCR.Trim()));
                                    var s = c.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                    if (s != null)
                                    {
                                        observacion = string.Concat(observacion, string.Format("FECHA: {0}. ", s.FEC_INICIO_COMPURGACION.Value.ToString("dd/MM/yyyy")));
                                    }
                                }
                                diccionario.Add("<<observaciones>>", observacion);
                            }
                            else
                                diccionario.Add("<<observaciones>>", " ");
                        }
                        else
                            diccionario.Add("<<observaciones>>", " ");
                        #endregion

                        diccionario.Add("<<fecha_ingreso>>", Fechas.fechaLetra(SelectedLiberacion.INGRESO.FEC_INGRESO_CERESO.Value, false));
                        diccionario.Add("<<coordinador_juridico>>", " ");
                        diccionario.Add("<<analista_juridico>>", " ");

                        var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.ORDEN_LIBERTAD);// //File.ReadAllBytes(@"C:\libertades\OL.docx");
                        if (documento == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                            return;
                        }

                        
                        var bytes = new cWord().FillFieldsDocx(documento.DOCUMENTO, diccionario);// FillFields(documento.DOCUMENTO, diccionario);
                        if (bytes == null)
                            return;
                        var tc = new TextControlView();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        
                        tc.editor.Loaded += (s, e) =>
                        {
                            try
                            {
                                
                                TXTextControl.LoadSettings ls = new TXTextControl.LoadSettings();
                                //ls.PageSize.Height = 1122;
                                //ls.PageSize.Width = 793;
                                switch (documento.ID_FORMATO)
                                {
                                    case (int)enumFormatoDocumento.DOCX:
                                        tc.editor.Load(bytes, TXTextControl.BinaryStreamType.WordprocessingML,ls);
                                        tc.editor.PageSize = ls.PageSize;
                                        break;
                                    case (int)enumFormatoDocumento.PDF:
                                        tc.editor.Load(bytes, TXTextControl.BinaryStreamType.AdobePDF,ls);
                                        break;
                                    case (int)enumFormatoDocumento.DOC:
                                        tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord,ls);
                                        break;
                                    default:
                                        new Dialogos().ConfirmacionDialogo("Validación", string.Format("El formato {0} del documento no es valido", documento.FORMATO_DOCUMENTO.DESCR));
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                            }
                        };
                        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar información de la liberación antes de imprimir");
                //}
                //else
                //    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una causa penal");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al imprimir orden de libertad.", ex);
            }
        }
        #endregion

        #region ProximaCausaPenal
        private void LimpiarProximaCausaPenal()
        {
            LstProximaCausaPenal = null;
            SelectedProximaCausaPenal = null;
        }

        private bool ActualizarEstatusCausaPenal(bool ProximaCausa = true, short EstatusCausaPenal = 1)
        {
            try
            {
                CAUSA_PENAL cp = new CAUSA_PENAL();
                if (ProximaCausa)
                    cp = SelectedProximaCausaPenal;
                else
                    cp = SelectedCausaPenal;
                var obj = new CAUSA_PENAL();
                obj.ID_CENTRO = cp.ID_CENTRO;
                obj.ID_ANIO = cp.ID_ANIO;
                obj.ID_IMPUTADO = cp.ID_IMPUTADO;
                obj.ID_INGRESO = cp.ID_INGRESO;
                obj.ID_CAUSA_PENAL = cp.ID_CAUSA_PENAL;
                obj.ID_ESTATUS_CP = EstatusCausaPenal;//ACTIVO

                if (new cCausaPenal().ActualizarEstatusCausaPenal(obj))
                {
                    var x = SelectedCausaPenal;
                    LstCausaPenal = new ObservableCollection<CAUSA_PENAL>(new cCausaPenal().Obtener(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO));
                    if (LstCausaPenal != null)
                        SelectedCausaPenal = x;
                    Edicion = false;
                    return true;
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Notificación", "Ocurrió un problema al guardar la información");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al actualizar el estatus de la causa penal", ex);
            }
            return false;
        }
        #endregion

        #region Validar Causas Penales Activas
        private bool CausasPenalesActivas()
        {
            if (SelectedIngreso.CAUSA_PENAL != null)
            {
                foreach (var cp in SelectedIngreso.CAUSA_PENAL.Where(w => w.ID_CAUSA_PENAL != SelectedCausaPenal.ID_CAUSA_PENAL))
                {
                    if (cp.SENTENCIA != null)
                    {
                        if (cp.SENTENCIA.Where(w => w.ESTATUS == "A").Count() > 0)
                            return true;
                    }
                }
            }
            return false;
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
