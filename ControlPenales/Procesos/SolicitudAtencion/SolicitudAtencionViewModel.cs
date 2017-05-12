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
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using ControlPenales.Clases;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Interop;
using System.IO;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class SolicitudAtencionViewModel : ValidationViewModelBase
    {
        #region constructor
        public SolicitudAtencionViewModel() { }
        #endregion

        #region metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;
                case "buscar_menu":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    LimpiarBusquedaInterno();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                //case "reporte_menu":
                //    break;
                case "buscar_salir":
                    LimpiarBusquedaInterno();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "add_interno":
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    LimpiarBusquedaInterno();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "del_interno":
                    EliminarInterno();
                    break;
                case "buscar_seleccionar"://"seleccionar_interno_pop":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un ingreso valido.");
                        return;
                    }
                    //Se quita candado de papeletas
                    //var hoy = Fechas.GetFechaDateServer; 
                    //if (new cAtencionIngreso().ObtenerSolicitudesPorMes(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, hoy.Year, hoy.Month) > VCitasMes)
                    //{
                    //    new Dialogos().ConfirmacionDialogo("Validación", "En interno ha sobrepasado el número de solicitudes por mes.");
                    //    return;
                    //}
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
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        return;
                    }
                    AgregarInterno();
                    LimpiarBuscar();
                    break;
                case "salir_interno_pop":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_INTERNO);
                    LimpiarBuscar();
                    break;
                case "buscar_interno_pop":
                    Buscar();
                    break;
                case "guardar_menu":
                    if (!PInsertar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    if (!base.HasErrors)
                    {
                        if (ValidacionIngresos())
                            GuardarSolicitud();

                        else
                            new Dialogos().ConfirmacionDialogo("Validación!", "Favor de agregar los internos que recibirán atención.");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación!", "Favor de capturar los campos obligatorios.");
                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new SolicitudAtencionView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new SolicitudAtencionViewModel();
                    break;
            }
        }

        private async void OnLoad(SolicitudAtencionView obj = null)
        {
            try
            {
                estatus_inactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }

        private async void ClickEnter(Object obj)
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
                    var textbox = obj as System.Windows.Controls.TextBox;
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

                Buscar();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }

        private void CargarListas()
        {
            try
            {
                LstAtencionTipos = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w=>w.ESTATUS=="S").OrderBy(w=>w.DESCR));
                LstArea = new ObservableCollection<AREA>(new cArea().ObtenerTodos().OrderBy(w => w.DESCR));
                LstAreaTecnica = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo().OrderBy(w => w.DESCR));
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstArea.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                    LstAreaTecnica.Insert(0, new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" });
                    LstAtencionTipos.Insert(0, new ATENCION_TIPO { ID_TIPO_ATENCION=-1,DESCR="SELECCIONE" });
                    ConfiguraPermisos();
                    ValidacionSolicitud();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listados", ex);
            }
        }

        private async void ModelChangedSwitch(object parametro)
        {
            if (parametro!=null)
                switch (parametro.ToString())
                {
                    case "cambio_area_tecnica":
                        if (SAreaTecnica.HasValue &&  SAreaTecnica.Value == (short)eAreas.AREA_MEDICA)
                        {
                            IsTipoAtencionVisible = Visibility.Visible;
                            ValidacionTipoSolicitud();
                        }
                        else
                        {
                            IsTipoAtencionVisible = Visibility.Collapsed;
                            QuitarValidacionTipoSolicitud();
                        }
                            
                    break;
                    #region Cambio SelectedItem de Busqueda de Expediente
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
                    #endregion

                }
        }

        private void LimpiarBusquedaInterno()
        { 
            AnioBuscar = null;
            FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            EmptyIngresoVisible = EmptyExpedienteVisible = false;
            ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.SOLICITUD_ATENCION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
                        //if (p.IMPRIMIR == 1)
                        //    PImprimir = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        private async Task<List<INGRESO>> SegmentarIngresoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<INGRESO>>(() =>
                             new ObservableCollection<INGRESO>(new cIngreso().ObtenerTodosActivos(GlobalVar.gCentro, AnioI, FolioI, NombreI, PaternoI, MaternoI, _Pag)));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargandoIngresos = true;
                }
                else
                    SeguirCargandoIngresos = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar internos.", ex);
                return new List<INGRESO>();
            }
        }
        #endregion

        #region Buscar
        private void LimpiarBuscar()
        {
            AnioI = null;
            FolioI = null;
            PaternoI = MaternoI = NombreI = UbicacionInterno = string.Empty;
            LstIngreso = null;
            InternosEmpty = true;
            SelectIngreso = null;
            ImagenIngresoPop = new Imagenes().getImagenPerson();
        }

        private async void Buscar()
        {
            try
            {
                //LstIngreso = new RangeEnabledObservableCollection<INGRESO>();
                //LstIngreso.InsertRange(await SegmentarIngresoBusqueda());

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                InternosEmpty = !(ListExpediente.Count > 0);
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar interno", ex);
            }
        }

        private void AgregarInterno()
        {
            try
            {
                if (LstInternos == null)
                    LstInternos = new ObservableCollection<INGRESO>();

                if (LstInternos.IndexOf(SelectIngreso) == -1)
                {
                    LstInternos.Add(SelectIngreso);
                    EmptyInternos = LstInternos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado ya existe");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar interno", ex);
            }
        }

        private void EliminarInterno()
        {
            try
            {
                if (SelectIngresoLista != null)
                {
                    if (LstInternos != null)
                    {
                        if(!LstInternos.Remove(SelectIngresoLista))
                            new Dialogos().ConfirmacionDialogo("Validación", "No se pudo eliminar interno");
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un interno");
            }
            catch (Exception ex)
            { 
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar interno", ex);
            }
        }
        #endregion

        #region Solicitud
        private async void GuardarSolicitud()
        {
            if (base.HasErrors)
                return;
            if (!ValidacionIngresos())
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de agregar internos a la solicitud");
                return;
            }
            var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Solicitud de Atención", () =>
            {
                try
                {
                    var obj = new ATENCION_SOLICITUD();
                    obj.ID_TIPO_ATENCION = IsTipoAtencionVisible == Visibility.Visible ? SelectedAtencion_Tipo : null;
                    obj.ID_TECNICA = SAreaTecnica;
                    obj.ID_AREA = SArea;
                    obj.SOLICITUD_FEC = Fechas.GetFechaDateServer;
                    obj.ACTIVIDAD = SActividad;
                    obj.SOLICITUD_HORA = SHora;
                    obj.AUTORIZACION = SAutorizacion;
                    obj.OFICIAL_TRASLADA = SOficialTraslada;
                    obj.ID_CENTRO = GlobalVar.gCentro;
                    obj.ESTATUS = (short)enumSolicitudCita.SOLICITADA;
                    if (SelectedAtencionSolicitud == null)
                    {
                        //atencion ingreso
                        var internos = new List<ATENCION_INGRESO>();
                        if (LstInternos != null)
                        {
                            foreach (var i in LstInternos)
                            {
                                internos.Add(new ATENCION_INGRESO() { ID_CENTRO = i.ID_CENTRO, ID_ANIO = i.ID_ANIO, ID_IMPUTADO = i.ID_IMPUTADO, ID_INGRESO = i.ID_INGRESO, REGISTRO_FEC=obj.SOLICITUD_FEC, ID_CENTRO_UBI=obj.ID_CENTRO });
                            }
                        }
                        obj.ATENCION_INGRESO = internos;
                        obj.ID_ATENCION = new cAtencionSolicitud().Agregar(obj);
                        if (obj.ID_ATENCION > 0)
                        {
                            SelectedAtencionSolicitud = obj;
                            StaticSourcesViewModel.SourceChanged = false;
                            return true;
                        }
                    }
                    else
                    {
                        obj.ID_ATENCION = SelectedAtencionSolicitud.ID_ATENCION;
                        var internos = new List<ATENCION_INGRESO>();
                        if (LstInternos != null)
                        {
                            foreach (var i in LstInternos)
                            {
                                internos.Add(new ATENCION_INGRESO() { ID_CENTRO = i.ID_CENTRO, ID_ANIO = i.ID_ANIO, ID_IMPUTADO = i.ID_IMPUTADO, ID_INGRESO = i.ID_INGRESO, ID_ATENCION = obj.ID_ATENCION, ID_CENTRO_UBI=obj.ID_CENTRO });
                            }
                        }
                        if (new cAtencionSolicitud().Actualizar(obj, internos))
                        {
                            StaticSourcesViewModel.SourceChanged = false;
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar la solicitud de atención", ex);
                    return false;
                }
            });

            if (respuesta)
            {
                await new Dialogos().ConfirmacionDialogoReturn("Éxito", "La solicitud de atención se ha guardado correctamente");
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new SolicitudAtencionView();
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new SolicitudAtencionViewModel();
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

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        if (estatus_inactivos.Contains(ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO))
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].ID_ANIO.ToString() + "/" +
                                ListExpediente[0].ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no tiene permitido ningún cambio de información.");
                            return;
                        }

                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        //SeleccionaIngreso();
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
            var result = new ObservableCollection<IMPUTADO>();
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar la búsqueda", ex);
            }
            return result.ToList();
        }

        #endregion

        #region Impresion Evento
        //private void ImpresionEvento() 
        //{
        //    if (SelectedEvento != null)
        //    {
        //        var parametros = new Dictionary<string, string>();
        //        parametros.Add("nombre_evento", SelectedEvento.NOMBRE);
        //        if (SelectedEvento.CENTRO != null)//CENTRO
        //        {
        //            parametros.Add("lugar", SelectedEvento.CENTRO.DESCR);
        //        }
        //        else//EXTERNO
        //        {
        //            parametros.Add("lugar", SelectedEvento.LUGAR);
        //        }
        //        parametros.Add("direccion", SelectedEvento.LUGAR_DIRECCION);
        //        parametros.Add("ciudad", string.Format("{0},{1}",SelectedEvento.MUNICIPIO.MUNICIPIO1,SelectedEvento.MUNICIPIO.ENTIDAD.DESCR));
        //        parametros.Add("fecha", Fechas.fechaLetra(SelectedEvento.EVENTO_FEC.Value,false));
        //        parametros.Add("hora_invitados",SelectedEvento.HORA_INVITACION.Value.ToString("hh:mm tt"));
        //        parametros.Add("hora_presidium",SelectedEvento.HORA_ARRIBO.Value.ToString("hh:mm tt"));
        //        parametros.Add("telefono", new Converters().MascaraTelefono(SelectedEvento.TELEFONO));
        //        parametros.Add("promovente", SelectedEvento.PROMOVENTE);
        //        parametros.Add("perfil_asistentes", SelectedEvento.PERFIL_ASISTENTES);
        //        parametros.Add("objetivo_evento", SelectedEvento.OBJETIVO);
        //        parametros.Add("maestro_ceremonias", SelectedEvento.MAESTRO);
        //        short index = 1;
        //        string programas = string.Empty;
        //        if(SelectedEvento.EVENTO_PROGRAMA != null)
        //        {
        //            foreach(var p in SelectedEvento.EVENTO_PROGRAMA)
        //            {
        //                if(!string.IsNullOrEmpty(programas))
        //                    programas = string.Format("{0}\n",programas);
        //                programas = string.Format("{0}{1}{2} ({3})",programas,index,p.DESCR,p.DURACION);
        //                index++;
        //            }
        //        }
        //        parametros.Add("programa", programas);
        //        index = 1;
        //        string presidium = string.Empty;
        //         if(SelectedEvento.EVENTO_PRESIDIUM != null)
        //        {
        //            foreach(var p in SelectedEvento.EVENTO_PRESIDIUM)
        //            {
        //                if(!string.IsNullOrEmpty(presidium))
        //                    presidium = string.Format("{0}\n",presidium);
        //                presidium = string.Format("{0}{1}{2} -{3}", presidium, index, p.NOMBRE, p.PUESTO);
        //                index++;
        //            }
        //        }
        //         parametros.Add("presidium", presidium);
        //        parametros.Add("comite", SelectedEvento.COMITE == "S" ? "SI" : "NO");
        //        parametros.Add("impacto_evento", SelectedEvento.EVENTO_IMPACTO.DESCR);
        //        parametros.Add("vestimenta", SelectedEvento.EVENTO_VESTIMENTA.DESCR);
        //        parametros.Add("medios", SelectedEvento.MEDIOS == "S" ? "SI" : "NO");
        //        parametros.Add("observaciones", SelectedEvento.OBSERV);
        //        parametros.Add("objetivo_general", SelectedEvento.OBJETIVO_GENERAL);
        //        index = 1;
        //        string informacionTecnica = string.Empty;
        //        if (SelectedEvento.EVENTO_INF_TECNICA != null)
        //        {
        //            foreach (var p in SelectedEvento.EVENTO_INF_TECNICA)
        //            {
        //                if (!string.IsNullOrEmpty(informacionTecnica))
        //                    informacionTecnica = string.Format("{0}\n", informacionTecnica);
        //                informacionTecnica = string.Format("{0}{1}{2}", programas, index, p.DESCR);
        //                index++;
        //            }
        //        }
        //        parametros.Add("informacion_tecnica", informacionTecnica);

        //        var doc = File.ReadAllBytes(@"C:\libertades\E.doc");
        //        var bytes = new cWord().FillFields(doc, parametros);
        //        if (bytes == null)
        //            return;
        //        var tc = new TextControlView();
        //        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //        tc.editor.Loaded += (s, e) =>
        //        {
        //            try
        //            {
        //                tc.editor.Load(bytes, TXTextControl.BinaryStreamType.MSWord);
        //            }
        //            catch (Exception ex)
        //            {
        //                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
        //            }
        //        };
        //        tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
        //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
        //        tc.Show();
        //    }
        //    else
        //        new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un evento.");

        //}
        #endregion

        
    }
}
