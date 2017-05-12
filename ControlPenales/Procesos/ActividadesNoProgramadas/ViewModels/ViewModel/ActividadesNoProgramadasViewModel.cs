using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ActividadesNoProgramadasViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region Variables
        /// <summary>
        /// Necesaria por la implementacion de la interfaz "IPageViewModel"
        /// </summary>
        public string Name
        {
            get
            {
                return "actividadesNoProgramadas";
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase: No se utiliza para la inicialización. Se utiliza el método "CargarVentana(ActividadesNoProgramadasView obj)" para este fin,
        /// aprovechando el evento "Loaded" de la vista (User Control) propietaria de la instancia de este DataContext para inicializar las propiedades
        /// requeridas por el módulo.
        /// </summary>
        public ActividadesNoProgramadasViewModel() { }/// ===CHECK===
        #endregion

        #region Metodos
        void IPageViewModel.inicializa() { }


        /// <summary>
        /// Método para obtener el estatus de los traslados, seleccionado por el usuario.
        /// </summary>
        /// <returns>Estatus Seleccionado</returns>
        public string DecisionEstatusTraslados()
        {
            //Se crea variable que alojara el estatus seleccionado
            var estatus_seleccionado = "";
            //Se evalua el estatus seleccionado en la selección de estatus
            switch (SelectedEstatusTraslado)
            {
                case enumEstatusTraslado.PROGRAMADO:
                    estatus_seleccionado = TRASLADO_PROGRAMADO;
                    break;
                case enumEstatusTraslado.EN_PROCESO:
                    estatus_seleccionado = TRASLADO_EN_PROCESO;
                    break;
            }
            //Se retorna el valor del estatus seleccionado
            return estatus_seleccionado;
        }/// ===CHECK===


        /// <summary>
        /// Método que obtiene los traslados de acuerdo al estatus seleccionado.
        /// </summary>
        public void ObtenerTraslados()/// ===CHECK===
        {
            try
            {
                //Se obtiene el estatus seleccionado
                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                var estatus_seleccionado = DecisionEstatusTraslados();
                //Se obtienen los traslados de acuerdo al estatus seleccionado y se guardan en la lista a mostrar
                ListaTraslados = new cTraslado().ObtenerTodos(GlobalVar.gCentro).Where(w =>
                     w.ID_ESTATUS == estatus_seleccionado &&
                     (w.TRASLADO_FEC.Year == FechaServer.Year &&
                     w.TRASLADO_FEC.Month == FechaServer.Month &&
                     w.TRASLADO_FEC.Day == FechaServer.Day &&
                     (w.TRASLADO_DETALLE.Count(cTD =>
                         cTD.ID_ESTATUS == TRASLADO_EN_PROCESO ||
                         cTD.ID_ESTATUS == TRASLADO_PROGRAMADO ||
                         cTD.ID_ESTATUS == TRASLADO_ACTIVO) > 0))
                     ).ToList();

            }
            //Si ocurrió un error, entonces...
            catch (Exception ex)
            {
                //Se le notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo Paso...", "Ocurrió un error al obtener los traslados", ex);
            }
        }

        /// <summary>
        /// Método para obtener el estatus de las excarcelaciones, seleccionado por el usuario.
        /// </summary>
        /// <returns>Estatus Seleccionado</returns>
        public string DecisionEstatusExcarcelaciones() ///===CHECK===
        {
            //Se crea variable que alojara el estatus seleccionado
            var estatus_seleccionado = "";
            //Se evalua el estatus seleccionado en la selección de estatus
            switch (SelectedEstatusExcarcelacion)
            {
                case enumEstatusExcarcelacion.PROGRAMADA:
                    estatus_seleccionado = EXCARCELACION_PROGRAMADA;
                    break;
                case enumEstatusExcarcelacion.ACTIVA:
                    estatus_seleccionado = EXCARCELACION_ACTIVA;
                    break;
                case enumEstatusExcarcelacion.EN_PROCESO:
                    estatus_seleccionado = EXCARCELACION_EN_PROCESO;
                    break;
                case enumEstatusExcarcelacion.AUTORIZADA:
                    estatus_seleccionado = EXCARCELACION_AUTORIZADA;
                    break;
            }
            //Se retorna el valor del estatus seleccionado
            return estatus_seleccionado;
        }

        /// <summary>
        /// Método que obtiene las excarcelaciones de acuerdo al estatus seleccionado.
        /// </summary>
        public void ObtenerExcarcelaciones() ///===CHECK===
        {
            try
            {
                System.DateTime _fechaH = Fechas.GetFechaDateServer;

                //Se obtienen los ingresos que tengan alguna excarcelación
                var ingresos = new cIngreso().ObtenerIngresosActivos(GlobalVar.gCentro).Where(w => w.EXCARCELACION.Any()).ToList();
                //Se obtiene el estatus seleccionado
                var estatus_seleccionado = DecisionEstatusExcarcelaciones();
                //Se obtienen las excarcelaciones de acuerdo al estatus seleccionado en cuestión a los ingresos que tienen una excarcelación
                var lista_ingresos = ingresos != null ? ingresos.Any() ? ingresos.Where(w => (w.EXCARCELACION.Where(wEXC =>
                    wEXC.ID_ESTATUS == estatus_seleccionado &&
                    (wEXC.PROGRAMADO_FEC.Value.Year == FechaServer.Year && //LA VARIABLE DEL CAMPO DE FECHA SE LLAMA FECHASERVER
                    wEXC.PROGRAMADO_FEC.Value.Month == FechaServer.Month &&
                    wEXC.PROGRAMADO_FEC.Value.Day == FechaServer.Day))
                    .ToList().Count > 0)).ToList() : new List<INGRESO>() : new List<INGRESO>();
                var lista_ingresos_excarcelaciones = new List<InternoIngresoExcarcelacion>();
                var ingreso_ubicacion = new cIngresoUbicacion();
                var excarcelacion = new cExcarcelacion();
                foreach (var ingreso in lista_ingresos)
                {
                    var ultima_ubicacion = ingreso_ubicacion.ObtenerUltimaUbicacion(ingreso.ID_ANIO, ingreso.ID_CENTRO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO);
                    var excarcelacion_activa = excarcelacion.ObtenerImputadoExcarcelaciones(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO).Where(w =>
                        w.ID_ESTATUS == EXCARCELACION_ACTIVA &&
                        w.PROGRAMADO_FEC.Value.Year == FechaServer.Year &&
                        w.PROGRAMADO_FEC.Value.Month == FechaServer.Month &&
                        w.PROGRAMADO_FEC.Value.Day == FechaServer.Day).Any();
                    lista_ingresos_excarcelaciones.Add(new InternoIngresoExcarcelacion()
                    {
                        Id_Centro = ingreso.ID_CENTRO,
                        Id_Anio = ingreso.ID_ANIO,
                        Id_Imputado = ingreso.ID_IMPUTADO,
                        Id_Ingreso = ingreso.ID_INGRESO,
                        Nombre = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.NOMBRE) ? ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                        Paterno = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.PATERNO) ? ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                        Materno = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.MATERNO) ? ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty,
                        EnSalidaDeCentro = (ultima_ubicacion != null && ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO && ultima_ubicacion.ESTATUS == (short)enumUbicacion.ACTIVIDAD && !excarcelacion_activa)
                    });
                }
                ListaIngresos = lista_ingresos_excarcelaciones;
            }
            //Si ocurrió un error, entonces...
            catch (Exception ex)
            {
                //Se le notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo Paso...", "Ocurrió un error al obtener las excarcelaciones", ex);
            }
        }

        /// <summary>
        /// Método para refrescar la ventana y realizar la búsqueda de traslados y excarcelaciones, actualizando y consultando
        /// cualquier cambio en la información.
        /// </summary>
        public async void RefrescarVentana()///===CHECK===
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                //Se obtiene la imagen que se muestra por defecto para la foto de los internos
                var placeholder = new Imagenes().getImagenPerson();

                //Se obtienen las excarcelaciones y traslados existentes
                ObtenerExcarcelaciones();
                ObtenerTraslados();

                //Se inicializa el traslado seleccionado
                SelectedTraslado = null;

                //Se inicializa la lista de internos referentes a un traslado
                ListaTrasladosDetalle = new List<InternoTrasladoDetalle>();

                //Se inicializa el ingreso seleccionado
                SelectedIngreso = null;

                //Se inicializan las excarcelaciones médicas y jurídicas a una lista nueva sin elementos
                ListaExcarcelacionesMedicas = new List<EXCARCELACION>();
                ListaExcarcelacionesJuridicas = new List<EXCARCELACION>();

                //Se inicializa la lista de destinos de la excarcelacion seleccionada a una lista nueva sin elementos
                ListaDestinos = new List<Destino>();

                //Se inicializan las fotos de muestra del imputado tanto en excarcelaciones como en traslados
                FotoImputadoTraslado = placeholder;
                FotoImputadoExcarcelacion = placeholder;
            });
        }
        #endregion

        #region Metodos Eventos
        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de Clic del módulo.
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento sobre el cuál se realizó el clic.</param>
        public async void ClickSwitch(Object obj) ///===CHECK===
        {
            //Se obtiene la imagen por defecto para muestra en caso de que un interno no tenga foto
            var placeholder = new Imagenes().getImagenPerson();

            //Se evalúa el parámetro enviado para saber la acción a realizar
            switch (obj.ToString())
            {
                case "CargarTrasladoExcarcelacion":
                    RefrescarVentana();
                    break;
                case "ObtenerFotoImputado":
                    //Si el tab seleccionado es igual a 0, entonces...
                    if (IndexTab == 0)
                    {
                        try
                        {
                            //Si se ha seleccionado un interno del traslado, entonces...
                            if (SelectedTrasladoDetalle != null)
                            {
                                //Se obtiene la foto de registro de frente del interno seleccionado
                                var fotoRegistro = new cIngreso().ObtenerUltimoIngreso(SelectedTrasladoDetalle.Centro, SelectedTrasladoDetalle.Anio, SelectedTrasladoDetalle.IdImputado).INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                //Se obtiene la foto de seguimiento de frente del interno seleccionado
                                var fotoSeguimiento = new cIngreso().ObtenerUltimoIngreso(SelectedTrasladoDetalle.Centro, SelectedTrasladoDetalle.Anio, SelectedTrasladoDetalle.IdImputado).INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                //La foto del interno en el traslado a mostrar se obtiene de acuerdo a alguna de las fotos obtenidas anteriormente. Si la foto de seguimiento no existe, se toma la de registro, y si ésta no existe, se muestra la imagen por defecto
                                FotoImputadoTraslado = fotoSeguimiento != null ? (fotoSeguimiento.BIOMETRICO != null ? fotoSeguimiento.BIOMETRICO : new Imagenes().getImagenPerson()) : (fotoRegistro != null ? (fotoRegistro.BIOMETRICO != null ? fotoRegistro.BIOMETRICO : new Imagenes().getImagenPerson()) : new Imagenes().getImagenPerson());
                            }
                        }
                        //Si ocurrió un error al obtener la foto del interno, entonces...
                        catch (Exception ex)
                        {
                            //Se le notifica al usuario que ocurrió un error
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la foto del imputado.", ex);
                        }
                    }
                    //Si el tab seleccionado es diferente a 0, entonces...
                    else
                    {
                        //Si se ha seleccionado un interno, entonces...
                        if (SelectedIngreso != null)
                        {
                            try
                            {
                                var ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedIngreso.Id_Centro, SelectedIngreso.Id_Anio, SelectedIngreso.Id_Imputado);
                                //Se obtiene la foto de registro de frente del interno seleccionado
                                var fotoRegistro = ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                //Se obtiene la foto de seguimiento de frente del interno seleccionado
                                var fotoSeguimiento = ingreso.INGRESO_BIOMETRICO.Where(w => w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                //La foto del interno en la excarcelacion a mostrar se obtiene de acuerdo a alguna de las fotos obtenidas anteriormente. Si la foto de seguimiento no existe, se toma la de registro, y si ésta no existe, se muestra la imagen por defecto
                                FotoImputadoExcarcelacion = fotoSeguimiento != null ? (fotoSeguimiento.BIOMETRICO != null ? fotoSeguimiento.BIOMETRICO : new Imagenes().getImagenPerson()) : (fotoRegistro != null ? (fotoRegistro.BIOMETRICO != null ? fotoRegistro.BIOMETRICO : new Imagenes().getImagenPerson()) : new Imagenes().getImagenPerson());
                            }
                            //Si ocurrió un error al obtener la foto del interno, entonces...
                            catch (Exception ex)
                            {
                                //Se le notifica al usuario que ocurrió un error
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la foto del imputado.", ex);
                            }
                        }
                    }
                    break;
                case "Huellas":
                    //Si el tab seleccionado es igual a 0, entonces...
                    if (IndexTab == 0)
                    {
                        //Si se ha seleccionado un traslado, entonces...
                        if (SelectedTraslado != null)
                        {
                            try
                            {
                                //Se inicializa la ventana del traslado seleccionado
                                var WindowTraslado = new BusquedaHuellaTrasladoView();
                                //Se asigna el DataContext de la ventana al ViewModel de la búsqueda de huellas del traslado seleccionado
                                WindowTraslado.DataContext = new BusquedaHuellaTraslado(SelectedTraslado, WindowTraslado);
                                //Se asigna el dueño de la ventana a la ventana principal
                                WindowTraslado.Owner = PopUpsViewModels.MainWindow;
                                //Se agrega la acción de cerrar el PopUp que bloquea el módulo principal, al cerrar la ventana
                                WindowTraslado.Closed += (s, e) =>
                                {
                                    RefrescarVentana();

                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                };
                                //Se abre el PopUp que bloquea el módulo principal
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                //Se muestra la ventana de lectura de huellas para el traslado seleccionado
                                WindowTraslado.ShowDialog();
                            }
                            //Si ocurrió un error al inicializar o abrir la ventana de lectura de huellas sobre el traslado seleccionado, entonces...
                            catch (Exception ex)
                            {
                                //Se le notifica al usuario que ocurrió un error
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los imputados del traslado.", ex);
                            }
                        }
                        //Si no se ha seleccionado un traslado, entonces...
                        else
                        {

                            //Se le notifica al usuario que debe seleccionar un traslado para poder iniciar la lectura de huellas sobre el traslado que seleccione
                            var metro = Application.Current.Windows[0] as MetroWindow;
                            var mySettings = new MetroDialogSettings()
                            {
                                AffirmativeButtonText = "Aceptar",
                                AnimateShow = true,
                                AnimateHide = false
                            };
                            await metro.ShowMessageAsync("Validación", "Debe seleccionar un traslado.", MessageDialogStyle.Affirmative, mySettings);
                        }
                    }
                    //Si el tab seleccionado es diferente a 0, entonces...
                    else
                    {
                        try
                        {
                            //Se inicializa la ventana de las excarcelaciones
                            var WindowExcarcelacion = new BusquedaHuellaExcarcelacionView();
                            //Se asigna el DataContext de la ventana al ViewModel de la búsqueda de huellas de las excarcelaciones
                            WindowExcarcelacion.DataContext = new BusquedaHuellaExcarcelaciones(WindowExcarcelacion);
                            //Se asigna el dueño de la ventana a la ventana principal
                            WindowExcarcelacion.Owner = PopUpsViewModels.MainWindow;
                            //Se agrega la acción de cerrar el PopUp que bloquea el módulo principal, al cerrar la ventana
                            WindowExcarcelacion.Closed += (s, e) =>
                            {
                                RefrescarVentana();
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            };
                            //Se abre el PopUp que bloquea el módulo principal
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            //Se muestra la ventana de lectura de huellas para las excarcelaciones
                            WindowExcarcelacion.ShowDialog();
                        }
                        //Si ocurrió un error al inicializar o abrir la ventana de lectura de huellas sobre las excarcelaciones, entonces...
                        catch (Exception ex)
                        {
                            //Se le notifica al usuario que ocurrió un error
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los imputados del traslado.", ex);
                        }
                    }
                    break;
                case "SalidaCentroUbicacion":
                    //Se inicializa la ventana del control de la salida del centro (Cambio de ubicación que tienen un traslado ó excarcelación, ó vienen de fuera de una excarcelación)
                    var WindowIngresarSalidaCentro = new IngresarSalidaCentro();
                    //Se asigna el DataContext de la ventana al ViewModel de la búsqueda de huellas del control de la salida del centro
                    WindowIngresarSalidaCentro.DataContext = new IngresarSalidaCentroViewModel();
                    //Se asigna el dueño de la ventana a la ventana principal
                    WindowIngresarSalidaCentro.Owner = PopUpsViewModels.MainWindow;
                    //Se agrega la acción de cerrar el PopUp que bloquea el módulo principal, al cerrar la ventana
                    WindowIngresarSalidaCentro.Closed += (s, e) =>
                    {
                        RefrescarVentana();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };
                    //Se abre el PopUp que bloquea el módulo principal
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    //Se muestra la ventana de lectura de huellas para el control de la salida del centro
                    WindowIngresarSalidaCentro.ShowDialog();
                    break;
                case "limpiar_menu":
                    //Se limpian los campos de la ventana
                    SelectedTraslado = null;
                    SelectedIngreso = null;
                    SelectedTrasladoDetalle = null;
                    ListaExcarcelacionesMedicas = new List<EXCARCELACION>();
                    ListaExcarcelacionesJuridicas = new List<EXCARCELACION>();
                    ListaIngresos = new List<InternoIngresoExcarcelacion>();
                    ListaDestinos = new List<Destino>();
                    ListaTraslados = new List<TRASLADO>();
                    FotoImputadoTraslado = placeholder;
                    FotoImputadoExcarcelacion = placeholder;
                    break;
                case "salir_menu":
                    //Salida hacia el menú principal
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }
        #endregion

        #region InicializaActividades
        /// <summary>
        /// Inicializa las propiedades requeridas por el módulo por medio del evento "Loaded"
        /// </summary>
        /// <param name="obj">Ventana de la cuál se van a inicializar las propiedades requeridas por el módulo</param>
        public async void CargarVentana(ActividadesNoProgramadasView obj) ///===CHECK===
        {

            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //Se configuran los permisos del módulo
                    ConfiguraPermisos();
                    //Se obtiene la imagen por defecto
                    var placeholder = new Imagenes().getImagenPerson();
                    //Se inicializan las imágenes a mostrar de los imputados en la imágen por defecto
                    FotoImputadoTraslado = placeholder;
                    FotoImputadoExcarcelacion = placeholder;
                    SelectedEstatusTraslado = enumEstatusTraslado.EN_PROCESO;
                    SelectedEstatusExcarcelacion = enumEstatusExcarcelacion.EN_PROCESO;
                    EstatusTrasladosVisible = true;
                    EstatusExcarcelacionesVisible = false;
                    if (PConsultar)
                    {
                        ObtenerTraslados();
                        ObtenerExcarcelaciones();
                    }
                });
            }
            //Si ocurrió un error al cargar la ventana, entonces...
            catch (Exception ex)
            {
                //Se le nofitica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el módulo.", ex);
            }

        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos() ///===CHECK===
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_DE_ACTIVIDADES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    //foreach (var p in permisos)
                    //{
                    //    if (p.CONSULTAR == 1)
                    //        PConsultar = true;
                    //    if (p.EDITAR == 1)
                    //        PEditar = true;
                    //}
                    if (permisos.Count(c => c.CONSULTAR == 1) > 0)
                    {
                        PConsultar = true;
                        MenuBuscarEnabled = true;
                    }

                    if (permisos.Count(c => c.EDITAR == 1) > 0)
                        PEditar = true;

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