using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlProgramas;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ControlPenales
{
    public partial class ActividadesViewModel : ValidationViewModelBase, IPageViewModel
    {

        # region Copyright Quadro – 2016
        //
        // Todos los derechos reservados. La reproducción o trasmisión en su
        // totalidad o en parte, en cualquier forma o medio electrónico, mecánico
        // o similar es prohibida sin autorización expresa y por escrito del
        // propietario de este código.
        //
        // Archivo: ActividadesViewModel.cs
        //
        #endregion

        #region Variables
        /// <summary>
        /// Necesaria por la implementacion de la interfaz "IPageViewModel"
        /// </summary>
        public string Name
        {
            get
            {
                return "actividades";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor de la clase: No se utiliza para la inicialización. Se utiliza el método "CargarVentana(ActividadesView obj)" para este fin,
        /// aprovechando el evento "Loaded" de la vista (User Control) propietaria de la instancia de este DataContext para inicializar las propiedades
        /// requeridas por el módulo.
        /// </summary>
        public ActividadesViewModel() { }
        #endregion

        #region Metodos Eventos
        /// <summary>
        /// Método al cuál se le delega la responsabilidad de inicializar y mostrar el modal de la actividad seleccionada.
        /// </summary>
        /// <param name="obj">Objeto que se recibe desde el evento "MouseDoubleClick"</param>
        public async void DoubleClick(object obj)
        {
            //Se indica al usuario que se estan cargando los datos para la visualización de la actividad
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                //Si la actividad seleccionada es nula, entonces...
                if (SelectedActividad == null)
                {
                    //Se indica que se debe seleccionar una actividad
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        var metro = Application.Current.Windows[0] as MetroWindow;
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Cerrar",
                            AnimateShow = true,
                            AnimateHide = false
                        };
                        metro.ShowMessageAsync("Validación", "Para ver información de la actividad, debe dar doble clic sobre una de ellas.", MessageDialogStyle.Affirmative, mySettings);
                    }));
                }
                //Si la actividad seleccionada no es nula, entonces...
                else
                {
                    //Se obtienen los internos de la actividad seleccionada y se muestra el modal que contiene la información detallada de la misma
                    try
                    {
                        ObtenerInternosActividad();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA);
                    }
                    //Si ocurrió algun error en la obtención de internos de la actividad, entonces...
                    catch (Exception ex)
                    {
                        //Se notifica al usuario que ocurrió un error
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener los internos de la actividad", ex);
                    }
                }
            });
        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de Clic del módulo.
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento sobre el cuál se realizó el clic.</param>
        public async void ClickSwitch(object obj)
        {
            //Se evalúa el parámetro enviado para saber la acción a realizar
            switch (obj.ToString())
            {
                case "buscar_menu":

                    //Se crea e inicializa una lista sobre la cuál almacenar los internos de la búsqueda
                    List<GRUPO_ASISTENCIA> ListaImputados = new List<GRUPO_ASISTENCIA>();

                    //Se inhabilita el módulo principal bloqueando toda la ventana
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    //Se inicializa la ventana de búsqueda de internos
                    WindowBusquedaInternos = new BuscarInternosProgramas();


                    try
                    {
                        //Se indica al usuario que se estan cargando los datos para la visualización de resultados de la búsqueda
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            //Convertimos los valores obtenidos del Año y Folio del interno a buscar, si es que estos campos fueron ingresados
                            var AnioBusqueda = 0;
                            var FolioBusqueda = 0;
                            Int32.TryParse(AnioBuscar, out AnioBusqueda);
                            Int32.TryParse(FolioBuscar, out FolioBusqueda);

                            //Si alguno de los campos de la búsqueda tiene algún valor,entonces...
                            if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || AnioBusqueda != 0 || FolioBusqueda != 0)
                            {
                                System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                                //Se crea e inicializa una lista temporal sobre la cuál alojar a los internos,
                                var ListaImputadosAux = new cGrupoAsistencia().ObtenerInternos(NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, (short)AnioBusqueda, (short)FolioBusqueda, Areas).Where(
                                w =>
                                    //Filtra el centro en el que se encuentran
                                    w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&

                                    //Se filtran las actividades con la fecha del server
                                    w.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                                    w.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                                    w.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                                    (w.GRUPO_HORARIO.HORA_INICIO.Value.Hour <= _fechaHoy.Hour &&
                                    w.GRUPO_HORARIO.HORA_TERMINO.Value.Hour > _fechaHoy.Hour) &&

                                    //Se filtra a los internos que no tienen su actividad cancelada
                                    w.ESTATUS != (short)enumGrupoAsistenciaEstatus.CANCELADO &&

                                    //Se filtra a los internos con traslados en proceso ó activos
                                    (w.GRUPO_PARTICIPANTE.INGRESO.TRASLADO_DETALLE.Where(
                                    wTD =>
                                          wTD.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                                          wTD.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                                          wTD.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day &&
                                          (wTD.ID_ESTATUS == TRASLADO_EN_PROCESO || wTD.ID_ESTATUS == TRASLADO_ACTIVO)).Count() == 0) &&

                                    //Se filtra a los internos con excarcelaciones en proceso ó activas
                                   (w.GRUPO_PARTICIPANTE.INGRESO.EXCARCELACION.Where(
                                   wEXC =>
                                          wEXC.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                                          wEXC.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                                          wEXC.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                                          wEXC.ID_ESTATUS == EXCARCELACION_EN_PROCESO || wEXC.ID_ESTATUS == EXCARCELACION_ACTIVA).Count() == 0) &&
                                          w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.HasValue
                                    ).ToList();

                                //Se itera sobre la lista de internos encontrados
                                foreach (var imputado in ListaImputadosAux)
                                {
                                    //Si la lista definitiva donde se alojaran los resultados finales de la búsqueda ya tiene al interno en ella,entonces...
                                    if (!ListaImputados.Where(w =>
                                        w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value/*ING_ID_CENTRO*/ == imputado.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value/*ING_ID_CENTRO*/ &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO).Any())
                                    {
                                        //Inicia proceso de selección de la actividad a mostrar asociada al interno resultante de la búsqueda
                                        //Se buscan las actividades de ese interno y se crea una variable donde almacenar la actividad ganadora de acuerdo a la búsqueda y criterios de desempate
                                        //de la actividad a mostrar en cuestión al interno resultante
                                        var actividades = ListaImputadosAux.Where(w =>
                                        w.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                        w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value/*ING_ID_CENTRO*/ == imputado.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.Value/*ING_ID_CENTRO*/ &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO).ToList();
                                        GRUPO_ASISTENCIA imputado_actividad_elegida = null;

                                        //Se realiza el desempate de la actividad ganadora del interno
                                        //Si hay más de una actividad, entonces...
                                        if (actividades.Count > 1)
                                        {
                                            //Se toma la decisión sobre cual mostrar en el momento y estado actual de las circunstancias de ese imputado, de acuerdo a sus actividades
                                            //y si ha habido algún movimiento en coordinación
                                            var resolucion_coordinacion = actividades.Where(w =>
                                            w.EMP_COORDINACION == 2 &&
                                            w.EMP_APROBADO == 1).
                                            FirstOrDefault();

                                            var resolucion_actividad_mas_antigua = actividades.OrderBy(o => o.FEC_REGISTRO).ToList().FirstOrDefault();
                                            imputado_actividad_elegida = resolucion_coordinacion != null ? resolucion_coordinacion : resolucion_actividad_mas_antigua;
                                        }
                                        //Si solo se encontró una, entonces...
                                        else
                                        {
                                            //Se toma la actividad encontrada
                                            imputado_actividad_elegida = actividades.FirstOrDefault();
                                        }

                                        //Finalmente, se agrega el interno a la lista definitiva de resultados de la búsqueda
                                        ListaImputados.Add(imputado_actividad_elegida);
                                    }

                                }

                            }
                        });
                    }
                    //Si hubo algún error en las consultas hacia la base de datos, entonces... , entonces...
                    catch (Exception ex)
                    {
                        //Se le notifica al usuario que ocurrió un error
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda ó al obtener resultados.", ex);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        break;
                    }
                    //Se indica el DataContext de la ventana de búsqueda como una nueva instancia de la clase que maneja la ventana
                    WindowBusquedaInternos.DataContext = new BusquedaInternoProgramasViewModel(WindowBusquedaInternos, ListaImputados, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, AnioBuscar, FolioBuscar);
                    //Se indica que el propietario de la ventana de búsqueda es la ventana principal
                    WindowBusquedaInternos.Owner = PopUpsViewModels.MainWindow;
                    //En caso de ocurrir el evento "Closed" de la ventana de búsqueda, se desbloquea el módulo de programas
                    WindowBusquedaInternos.Closed += (s, e) =>
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    };

                    //Finalmente se muestra la ventana de búsqueda
                    WindowBusquedaInternos.ShowDialog();
                    break;
                case "tomarAsistencia":
                    //Si el equipo tiene Áreas asignadas, entonces...
                    // if (Areas.Count > 0)
                    // {
                    //Se inhabilita el módulo principal bloqueando toda la ventana
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);

                    //Se inicializa la ventana de toma de asistencia
                    WindowTomaDeAsistencia = new LeerInternos();
                    try
                    {
                        //Se indica el DataContext de la ventana de toma de asistencia como una nueva instancia de la clase que maneja la ventana
                        WindowTomaDeAsistencia.DataContext = new LeerInternosViewModel();

                        //Se indica que el propietario de la ventana de toma de asistencia es la ventana principal
                        WindowTomaDeAsistencia.Owner = PopUpsViewModels.MainWindow;

                        //En caso de ocurrir el evento "Closed" de la ventana de toma de asistencia, se desbloquea el módulo de programas
                        WindowTomaDeAsistencia.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };

                        //Finalmente se muestra la ventana de toma de asistencia

                        WindowTomaDeAsistencia.ShowDialog();
                    }
                    catch (Exception ex)
                    {

                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la toma de asistencia.", ex);
                    }

                    // }
                    //Si el equipo no tiene Áreas asignadas, entonces...
                    //else
                    //{
                    //    //Se le muestra mensaje al usuario de que el equipo actualmente no tiene áreas asignadas y la toma de asistencia no puede continuar
                    //    var met = Application.Current.Windows[0] as MetroWindow;
                    //    var mySettings = new MetroDialogSettings()
                    //    {
                    //        AffirmativeButtonText = "Cerrar",
                    //        AnimateShow = true,
                    //        AnimateHide = false
                    //    };
                    //    await met.ShowMessageAsync("AVISO", "El equipo no tiene ningún área asignada. Favor de contactar a un administrador.", MessageDialogStyle.Affirmative, mySettings);
                    //    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    //}
                    break;
                case "obtenerFotoInternoSeleccionado":
                    //Se consultan las fotos del interno seleccionado
                    try
                    {
                        //Se consultan las dos fotos de frente del interno: Seguimiento y Registro

                        var foto_seguimiento = new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                            new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                        var foto_registro = new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                            new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;

                        //Si no tiene foto de seguimiento, se toma la de registro; en caso de tampoco tener foto de registro, se deja la imagen de contacto de foto por defecto
                        FotoInternoSeleccionado = foto_seguimiento == null ? (foto_registro == null ? new Imagenes().getImagenPerson() : foto_registro) : foto_seguimiento;
                    }
                    //Si hubo algún error al obtener la foto, se deja la imagen de contacto de foto por defecto
                    catch (Exception)
                    {
                        FotoInternoSeleccionado = new Imagenes().getImagenPerson();
                    }
                    break;
                case "limpiar_menu":
                    //Se limpian todas las propiedades que contienen los parámetros de búsqueda de internos
                    NombreBuscar = "";
                    ApellidoPaternoBuscar = "";
                    ApellidoMaternoBuscar = "";
                    AnioBuscar = "";
                    FolioBuscar = "";
                    break;
                case "cerrarActividadSeleccionada":
                    //Se cierra el modal de la actividad seleccionada y se desbloquea el módulo de programas
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    break;
                case "salir_menu":
                    //Sale del módulo de programas y envía al usuario al módulo principal
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        /// <summary>
        /// Método al cuál se le delega la responsabilidad de cargar las actividades de acuerdo a la fecha seleccionada en el "DatePicker"
        /// </summary>
        /// <param name="obj">Objeto que recibe la ventana de la que se obtiene el método "SelectionChanged"</param>
        public async void CargarActividadesPorFecha(ActividadesView obj)
        {
            //Se indica al usuario que se estan cargando los datos para la visualización de las actividades
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {

                try
                {
                    //Se obtienen las actividades de acuerdo a la fecha seleccionada. Si la fecha seleccionada es el dia actual,
                    //se toma en cuenta la hora; si no, se toman todas las actividades de la fecha seleccionada
                    var hoy = Fechas.GetFechaDateServer;
                    var lista = (SelectedFecha.Year == hoy.Year &&
                                        SelectedFecha.Month == hoy.Month &&
                                        SelectedFecha.Day == hoy.Day) ?
                        //ObtenerActividades(hoy).Where(w => w.HORA_INICIO.Value.Hour == hoy.Hour).ToList() : ObtenerActividades(SelectedFecha).ToList();
                                        ObtenerActividades(hoy).Where(w => w.HORA_INICIO.Value.Hour == hoy.Hour) : ObtenerActividades(SelectedFecha);
                    if (lista != null)
                        ListaActividades = lista.ToList();
                    else
                        ListaActividades = new List<GRUPO_HORARIO>();
                    //Si no hay actividades en la fecha seleccionada, se muestra la etiqueta que indica que no hay información para mostrar
                    if (ListaActividades != null)
                        EmptyActividadesVisible = ListaActividades.Count == 0;
                    else
                        EmptyActividadesVisible = true;

                    //Se habilita o inhabilita el botón de toma de asistencia de acuerdo a la fecha seleccionada. Si la fecha corresponde al dia actual, se habilita. De otro modo, se deshabilita
                    TomaAsistenciaEnabled = (SelectedFecha.Year == hoy.Year &&
                                             SelectedFecha.Month == hoy.Month &&
                                             SelectedFecha.Day == hoy.Day);
                }
                //Si ocurrió algún error en la consulta de actividades, se muestra un mensaje de error al usuario
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades contempladas por el equipo.", ex);
                }
            });
        }
        #endregion

        #region Métodos
        void IPageViewModel.inicializa() { }

        /// <summary>
        /// Obtiene las actividades de acuerdo a una fecha
        /// </summary>
        /// <param name="Fecha">Fecha de la cuál se quieren consultar las actividades</param>
        /// <returns>Lista de actividades de acuerdo a la fecha enviada</returns>
        public List<GRUPO_HORARIO> ObtenerActividades(DateTime Fecha)
        {
            try
            {
                //Se obtienen las actividades de acuerdo a la fecha seleccionada y a las áreas asignadas al equipo
                return new cGrupoHorario().ObtenerActividades(Areas, Fecha).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene las áreas asignadas al equipo
        /// </summary>
        public void ObtenerAreas()
        {
            try
            {
                //Se obtienen las áreas asignadas al equipo, de acuerdo a la IP y MAC ADDRESS del mismo.
                Areas = new cEquipo_Area().Seleccionar(GlobalVar.gIP, GlobalVar.gMAC_ADDRESS).ToList();
            }
            //Si ocurrió un error a la hora de obtener las áreas del equipo, se muestra un mensaje de error al usuario
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los internos de la actividad seleccionada
        /// </summary>
        public void ObtenerInternosActividad()
        {
            //Se inicializa la foto del interno seleccionado a la imagen de contacto por defecto
            FotoInternoSeleccionado = new Imagenes().getImagenPerson();
            try
            {
                //Se obtienen los internos de la actividad de acuerdo a la actividad seleccionada por medio del ID del horario del grupo y el ID del grupo
                var lista_internos_actividad = new cGrupoAsistencia().ObtenerInternosActividad(SelectedActividad.ID_GRUPO_HORARIO, SelectedActividad.ID_GRUPO).Where(w => w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO.HasValue).ToList();

                //Se crea e inicializa una lista temporal sobre la cual se agregaran los internos de la actividad seleccionada para la lista definitiva vinculada a la lista (DataGrid) de la ventana
                List<InternosActividad> internos = new List<InternosActividad>();

                //Se itera sobre la lista de internos resultantes y se agregan a la lista temporal
                foreach (var interno in lista_internos_actividad)
                {
                    var ingreso = interno.GRUPO_PARTICIPANTE.INGRESO;
                    internos.Add(new InternosActividad()
                    {
                        Anio = interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? (short)interno.GRUPO_PARTICIPANTE.ING_ID_ANIO : new short() : new short(),
                        Centro = interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_CENTRO.HasValue ? (short)interno.GRUPO_PARTICIPANTE.ING_ID_CENTRO : new short() : new short(),
                        IdImputado = interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? (int)interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO : new int() : new int(),
                        IdIngreso = interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_INGRESO.HasValue ? (short)interno.GRUPO_PARTICIPANTE.ING_ID_INGRESO : new short() : new short(),
                        Expediente = string.Format("{0}/{1}",
                            interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? interno.GRUPO_PARTICIPANTE.ING_ID_ANIO.Value.ToString() : string.Empty : string.Empty,
                            interno.GRUPO_PARTICIPANTE != null ? interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.Value.ToString() : string.Empty : string.Empty),
                        Nombre = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.NOMBRE) ? ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                        Paterno = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.PATERNO) ? ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                        Materno = ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(ingreso.IMPUTADO.MATERNO) ? ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty,
                        Asistencia = interno.ASISTENCIA != null ? true : false,
                        Justificacion = interno.ESTATUS == (short)enumGrupoAsistenciaEstatus.JUSTIFICADO ? true : false
                    });
                }

                //Se iguala la lista vinculada a la lista (DataGrid) de la ventana 
                ListaInternosActividad = internos;

                //Se obtiene el responsable del grupo para mostrar en el modal de la actividad seleccionada
                Responsable = string.Format("{1} {2} {0}",
                    SelectedActividad.GRUPO != null ? SelectedActividad.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(SelectedActividad.GRUPO.PERSONA.NOMBRE) ? SelectedActividad.GRUPO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectedActividad.GRUPO != null ? SelectedActividad.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(SelectedActividad.GRUPO.PERSONA.PATERNO) ? SelectedActividad.GRUPO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                    SelectedActividad.GRUPO != null ? SelectedActividad.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(SelectedActividad.GRUPO.PERSONA.MATERNO) ? SelectedActividad.GRUPO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #endregion

        #region Inicializar
        /// <summary>
        /// Inicializa las propiedades requeridas por el módulo por medio del evento "Loaded"
        /// </summary>
        /// <param name="obj">Ventana de la cuál se van a inicializar las propiedades requeridas por el módulo</param>
        private async void CargarVentana(ActividadesView obj)
        {
            try
            {
                //Indicar al usuario que se esta cargando la información
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //Se realiza la configuración de la ventana de acuerdo a los permisos que tiene el usuario 
                    ConfiguraPermisos();

                    //Se habilitan las opciones autorizadas del menú sin la necesidad de permisos
                    MenuLimpiarEnabled = true;
                    MenuAyudaEnabled = true;
                    MenuSalirEnabled = true;
                });
                StaticSourcesViewModel.Mensaje("NOTA", "Para marcar asistencia de los internos, haga clic en 'Toma de asistencia'.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
            }
            //Si ocurrió un error al cargar la ventana, entonces...
            catch (Exception ex)
            {
                //Se notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las actividades.", ex);
            }
        }
        #endregion

        #region Permisos
        /// <summary>
        /// Configura el módulo de acuerdo a los permisos que tiene el usuario sobre el mismo.
        /// </summary>
        private void ConfiguraPermisos()
        {
            try
            {
                //Se obtienen los permisos que tiene el usuario que pertenece a la sesión
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_DE_ACTIVIDADES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                //Si tiene permisos, entonces...
                if (permisos.Any())
                {
                    //Se iteran los permisos
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
                    if (PConsultar)
                    {
                        System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                        Areas = new cEquipo_Area().Seleccionar(GlobalVar.gIP, GlobalVar.gMAC_ADDRESS).ToList();
                        ListaActividades = ObtenerActividades(_fechaHoy).Where(w =>
                            w.HORA_INICIO.Value.Hour == _fechaHoy.Hour).ToList();
                    }
                }
            }
            //Si ocurrió un error en la obtención de permisos, entonces...
            catch (Exception ex)
            {
                //Se envia una excepción con el mensaje de la misma
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}