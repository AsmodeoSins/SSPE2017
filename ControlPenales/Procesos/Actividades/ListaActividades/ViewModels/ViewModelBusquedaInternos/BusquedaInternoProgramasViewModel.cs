using ControlPenales.Clases;
using ControlPenales.Clases.ControlProgramas;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using ControlPenales.BiometricoServiceReference;
using System.Threading.Tasks;
using System.Transactions;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using DPUruNet;
using System.Threading;
using System.Windows.Controls;

namespace ControlPenales
{
    public partial class BusquedaInternoProgramasViewModel : FingerPrintScanner
    {
        # region Copyright Quadro – 2016
        //
        // Todos los derechos reservados. La reproducción o trasmisión en su
        // totalidad o en parte, en cualquier forma o medio electrónico, mecánico
        // o similar es prohibida sin autorización expresa y por escrito del
        // propietario de este código.
        //
        // Archivo: Eventos.cs
        // 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor de la clase: Inicializa todas las propiedades necesarias para la ventana.
        /// </summary>
        /// <param name="Ventana">Ventana del módulo. Únicamente se utiliza para efectos de visibilidad y mensajes.</param>
        /// <param name="ListaExpedientes">Expedientes enviados del módulo anterior para mostrar en los resultados de la búsqueda.</param>
        /// <param name="Areas">Áreas que contempla el equipo.</param>
        /// <param name="NombreBuscar">Nombre con el que se realizó la búsqueda del módulo anterior.</param>
        /// <param name="ApellidoPaternoBuscar">Apellido Paterno con el que se realizó la búsqueda del módulo anterior.</param>
        /// <param name="ApellidoMaternoBuscar">Apellido Materno con el que se realizó la búsqueda del módulo anterior.</param>
        /// <param name="AnioBuscar">Año de ingreso con el que se realizó la búsqueda del módulo anterior.</param>
        /// <param name="FolioBuscar">Folio del ingreso con el que se realizó la búsqueda del módulo anterior.</param>
        public BusquedaInternoProgramasViewModel(BuscarInternosProgramas Ventana, List<GRUPO_ASISTENCIA> ListaExpedientes, string NombreBuscar, string ApellidoPaternoBuscar, string ApellidoMaternoBuscar, string AnioBuscar, string FolioBuscar)
        {
            System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
            //Se inicializan los campos de búsqueda por medio de los datos enviados por el módulo anterior
            this.NombreBuscar = NombreBuscar;
            this.ApellidoPaternoBuscar = ApellidoPaternoBuscar;
            this.ApellidoMaternoBuscar = ApellidoMaternoBuscar;
            this.AnioBuscar = AnioBuscar;
            this.FolioBuscar = FolioBuscar;

            //Se inicializan las propiedades de la ventana de búsqueda de internos
            CampoCaptura = "ID:";
            this.Ventana = Ventana;
            NIPBuscar = "";
            EscoltarEnabled = false;

            //Se crea e inicializa lista temporal para llenar con los resultados de búsqueda del módulo anterior y poder visualizarlos en la ventana actual
            var ListaAux = new List<InternosActividad>();

            //Se crea e inicializa objeto controlador para realizar operaciones hacia la base de datos
            var ingresoUbicacion = new cIngresoUbicacion();
            var ingreso = new cIngreso();
            //Se itera sobre los resultados e inicializamos cada adherencia de cada objeto nuevo a la lista temporal creada previamente
            foreach (var imputado in ListaExpedientes)
            {
                //Última ubicación del interno
                var ultima_ubicacion = ingresoUbicacion.Obtener((short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).OrderByDescending(o => o.MOVIMIENTO_FEC).FirstOrDefault();
                //Registro del último ingreso del interno
                var ultimo_ingreso = ingreso.ObtenerUltimoIngreso((short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, (int)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO);

                //Se agrega el interno a la lista temporal, con sus valores correspondientes a mostrar en la ventana
                ListaAux.Add(new InternosActividad()
                {
                    Actividad = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ACTIVIDAD != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? imputado.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty, //Descripcion de la actividad en la que se encuentra o la que le corresponde al interno
                    Anio = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO : new short() : new short(), //Año de ingreso
                    Area = imputado.GRUPO_HORARIO != null ? imputado.GRUPO_HORARIO.ID_AREA.HasValue ? (short)imputado.GRUPO_HORARIO.ID_AREA : new int() : new int(), //ID del Área donde se realiza la actividad que le corresponde al interno
                    //Asistencia = imputado.ASISTENCIA == 1 ? true : false,
                    Centro = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO : new short() : new short(), //Centro al que pertenece el interno
                    Expediente = string.Format("{0}/{1}",
                        imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.Value.ToString() : string.Empty : string.Empty, 
                        imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.Value.ToString() : string.Empty : string.Empty), //Número de Expediente del interno
                    IdIngreso = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO : new short() : new short(), //ID de ingreso del interno
                    IdImputado = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO : new short() : new short(), //ID de imputado del interno
                    Id_Grupo = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ID_GRUPO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ID_GRUPO : new short() : new short(),
                    //IdConsec = (short)imputado.ID_CONSEC,
                    Materno = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty, //Apellido Materno del interno
                    Paterno = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty, //Apellido Paterno del interno
                    Nombre = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty  :string.Empty : string.Empty : string.Empty, //Nombre del interno
                    //NIP = imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP,
                    Ubicacion = imputado.GRUPO_HORARIO != null ? imputado.GRUPO_HORARIO.AREA != null ? !string.IsNullOrEmpty(imputado.GRUPO_HORARIO.AREA.DESCR) ? imputado.GRUPO_HORARIO.AREA.DESCR.Trim() : string.Empty : string.Empty : string.Empty, //Nombre del área donde se realiza la actividad que le corresponde al interno
                    Seleccionar = false, //Determina si el interno se selecciona para alguna salida no programada
                    Responsable = string.Format("{1} {2} {0}",
                    imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty, imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty, imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty), //Nombre completo del responsable del interno

                    TrasladoInterno = ultimo_ingreso.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == TRASLADO_PROGRAMADO &&
                                 w.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                                 w.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                                 w.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day).Count() == 1, //Se utiliza para colorear la fila donde se visualiza al interno cuando es candidato a un traslado programado

                    ExcarcelacionInterno = ultimo_ingreso.EXCARCELACION.Where(w => w.ID_ESTATUS == EXCARCELACION_AUTORIZADA &&
                                w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                                w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                                w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day).Count() > 0, //Se utiliza para colorear la fila donde se visualiza al interno cuando es candidato a una excarcelación autorizada

                    VisitaLegalInterno = ultimo_ingreso.ADUANA_INGRESO.Count(c =>
                                              c.ENTRADA_FEC.Value.Year == _fechaHoy.Year &&
                                              c.ENTRADA_FEC.Value.Month == _fechaHoy.Month &&
                                              c.ENTRADA_FEC.Value.Day == _fechaHoy.Day &&
                                              c.INTERNO_NOTIFICADO == null) > 0,
                    //Valor que indica si este imputado es candidato para alguna salida y si se encuentra en alguna de las áreas que contempla el equipo. Si no lo es ó no se encuentra
                    //en alguna de las áreas, en primera instancia, no se puede seleccionar

                    //Si la última ubicación es diferente de nulo, entonces...
                    Seleccionable = ultima_ubicacion != null ?

                    //Se pregunta si el ESTATUS de la última ubicación indica si el interno se encuentra en la actividad y si tiene la ASISTENCIA de dicha actividad,
                        //ademas de que se verifica si el interno tiene algún traslado programado o alguna excarcelación autorizada
                        //que se encuentren dentro de la fecha del dia actual y la hora concuenrde con la tolerancia que ambos
                        //casos tengan establecidos de acuerdo al centro (PARAMETRIZABLE). Si todo lo anterior procede, el interno es candidato y seleccionable para
                        //permitirle una salida no programada
                    ((ultima_ubicacion.ESTATUS == (short)enumEstatusUbicacion.EN_ACTIVIDAD && imputado.ASISTENCIA == 1) &&
                    (ultimo_ingreso.TRASLADO_DETALLE.Where(w =>
                     w.ID_ESTATUS == TRASLADO_PROGRAMADO &&
                     w.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                     w.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                     w.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day &&
                     ((((w.TRASLADO.TRASLADO_FEC.Hour - _fechaHoy.Hour) <= Parametro.TOLERANCIA_TRASLADO_EDIFICIO) &&
                     _fechaHoy.Minute == 0)) ||
                     (((w.TRASLADO.TRASLADO_FEC.Hour - _fechaHoy.Hour) < Parametro.TOLERANCIA_TRASLADO_EDIFICIO) &&
                     (w.TRASLADO.TRASLADO_FEC.Minute >= 0 && w.TRASLADO.TRASLADO_FEC.Minute <= 59))).Count() == 1 ||

                     ultimo_ingreso.EXCARCELACION.Where(w =>
                     w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                     w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                     w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                     ((((w.PROGRAMADO_FEC.Value.Hour - _fechaHoy.Hour) <= Parametro.TOLERANCIA_EXC_EDIFICIO) &&
                     _fechaHoy.Minute == 0)) ||
                     (((w.PROGRAMADO_FEC.Value.Hour - _fechaHoy.Hour) < Parametro.TOLERANCIA_EXC_EDIFICIO) &&
                     (w.PROGRAMADO_FEC.Value.Minute >= 0 && w.PROGRAMADO_FEC.Value.Minute <= 59)) &&
                     w.ID_ESTATUS == EXCARCELACION_AUTORIZADA).Count() > 0 ||

                     ultimo_ingreso.ADUANA_INGRESO.Count(c =>
                                              c.ENTRADA_FEC.Value.Year == _fechaHoy.Year &&
                                              c.ENTRADA_FEC.Value.Month == _fechaHoy.Month &&
                                              c.ENTRADA_FEC.Value.Day == _fechaHoy.Day &&
                                              c.INTERNO_NOTIFICADO == null) > 0))

                      : false
                });

            }

            //La lista de resultados se inicializa en la lista temporal con los resultados con el formato adecuado para su visualización en la ventana de búsqueda
            ListExpediente = ListaAux;

            //Se inicializa y crea la lista de internos seleccionados
            ListaSeleccionados = new List<InternosActividad>();

            //Si la lista de resultados tiene algún interno resultante, entonces...
            if (ListExpediente.Count > 0)
            {
                //No se visualiza la etiqueta que muestra que no hay información resultante
                EmptyBusquedaVisible = false;
            }
            //En caso contrario de que la lista de resultados no tenga ningún interno resultante, entonces...
            else
            {
                //Se visualiza la etiqueta que muestra que no hay información resultante
                EmptyBusquedaVisible = true;
            }

            //Se inicializan las áreas contempladas por el equipo
            Areas = new cEquipo_Area().Seleccionar(GlobalVar.gIP, GlobalVar.gMAC_ADDRESS).ToList();
        }

        #endregion

        #region Métodos Eventos
        /// <summary>
        /// Método al cuál se le delega la responsabilidad de inicializar las propiedades visuales de la ventana, asi como tambien
        /// la de abrir el lector de huellas digitales.
        /// </summary>
        /// <param name="Window"></param>
        public void OnLoad(BuscarInternosProgramas Window)
        {
            //Se inicializan todas las propiedades visuales requeridas por la ventana
            var placeholder = new Imagenes();
            SelectedImputadoFotoIngreso = placeholder.getImagenPerson();
            SelectedImputadoFotoSeguimiento = placeholder.getImagenPerson();
            FotoCustodio = placeholder.getImagenPerson();
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            ImagenEvaluacionVisible = true;
            ProgressRingVisible = Visibility.Collapsed;
            ScannerMessage = "Capture Huella";
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ImagenEvaluacion = placeholder.getImagenHuella();

            //Se valida que al cerrar la ventana, el hilo que maneja el lector sea nulo y se termine la carga del método, así como también cerrar el lector
            Window.Closed += (s, e) =>
            {

                try
                {
                    if (OnProgress == null)
                        return;

                    //if (!_IsSucceed)
                    //    SelectedCustodio = null;

                    OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lectura de las huellas", ex);
                }
            };

            //Si el lector actual ya tiene un valor, se elimina y se iguala a nulo
            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            //Después se obtiene el lector actual para una nueva lectura
            CurrentReader = Readers[0];

            //Si el lector actual es igual a nulo, se termina la operación o carga del método
            if (CurrentReader == null)
                return;

            //En caso de que el lector no pudiera abrirse para la lectura, se cierra la ventana
            if (!OpenReader())
                Window.Close();

            //Si el lector no puede tomar la captura asíncrona, se cierra la ventana
            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));

        }

        /// <summary>
        /// Realiza la búsqueda del custodio por medio del evento ocasionado en el campo de texto del ID presionando la tecla Enter.
        /// </summary>
        /// <param name="obj">Objeto del cuál proviene el evento de presionar la tecla Enter, en este caso, un campo de texto.</param>
        public void EnterKeyPressedID(object obj)
        {
            var NIP = 0;
            try
            {
                //Se obtiene el valor numérico del campo de captura del ID
                if (Int32.TryParse(NIPBuscar, out NIP))
                {
                    //Se busca el custodio por medio del ID obtenido
                    SelectedCustodio = new cEmpleado().ObtenerEmpleadoPorDepartamento(NIP, (short)enumAreaTrabajo.COMANDANCIA);

                    //Si se encontró un custodio, entonces...
                    if (SelectedCustodio != null)
                    {
                        //Se muestra la información del custodio encontrado
                        NombreCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.NOMBRE) ? SelectedCustodio.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty;
                        PaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.PATERNO) ? SelectedCustodio.PERSONA.PATERNO.Trim() : string.Empty : string.Empty;
                        MaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.MATERNO) ? SelectedCustodio.PERSONA.MATERNO.Trim() : string.Empty : string.Empty;
                        AnioCustodio = SelectedCustodio.REGISTRO_FEC.HasValue ? SelectedCustodio.REGISTRO_FEC.Value.Year.ToString() : string.Empty;
                        IDCustodio = SelectedCustodio.ID_EMPLEADO.ToString();
                        var consulta_foto_custodio = SelectedCustodio.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                        FotoCustodio = consulta_foto_custodio != null ? consulta_foto_custodio.BIOMETRICO : new Imagenes().getImagenPerson();

                        //Se notifica que se encontró un custodio
                        CambiarMensajeNIP(enumMensajeNIP.ENCONTRADO);
                        CapturaNIPVisible = false;
                    }
                    //Si no se encontró un custodio, entonces...
                    else
                    {
                        //Se notifica que no se encontró un custodio
                        CambiarMensajeNIP(enumMensajeNIP.NO_ENCONTRADO);
                    }
                }
            }
            //Si ocurrio algun error al buscar el custodio, se muestra un mensaje de error al usuario
            catch (Exception ex)
            {
                Ventana.Hide();
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener informacion de un custodio (Error al buscar por ID).", ex);
            }
        }

        /// <summary>
        /// Realiza la búsqueda de internos por medio del evento ocasionado en alguno de los campos de texto para enviar como parámetros
        /// presionando la tecla Enter.
        /// </summary>
        /// <param name="obj">Objeto del cuál proviene el evento de presionar la tecla Enter, en este caso, un campo de texto.</param>
        public async void EnterKeyPressed(object obj)
        {
            //Se oculta la ventana para iniciar la búsqueda
            Ventana.Hide();

            //Se obtienen los parámetros del año de registro y el folio del imputado
            var AnioBusqueda = 0;
            var FolioBusqueda = 0;
            Int32.TryParse(AnioBuscar, out AnioBusqueda);
            Int32.TryParse(FolioBuscar, out FolioBusqueda);

            //Si alguno de los campos no esta vacío, entonces...
            if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || AnioBusqueda != 0 || FolioBusqueda != 0)
            {
                //Se inicia y espera a que la búsqueda de internos finalíce
                ListExpediente = await BusquedaImputados((short)AnioBusqueda, FolioBusqueda);

                //Si hubo resultados, se oculta la etiqueta que indica que no hay información
                if (ListExpediente.Count > 0)
                    EmptyBusquedaVisible = false;
                //Si no hubo resultados, se muestra la etiqueta que indica que no hay información
                else
                    EmptyBusquedaVisible = true;
            }
            //Si ningún campo tiene información para buscar, se inicializa la lista de resultados en cero elementos
            else
            {
                ListExpediente = new List<InternosActividad>();
            }
            //Al finalizar la búsqueda, se muestra la ventana de nuevo para ver los resultados
            Ventana.ShowDialog();
        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de Clic de la ventana.
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento sobre el cuál se realizó el clic.</param>
        public async void ClickSwitch(object obj)
        {
            //Se evalúa el parámetro enviado para saber la acción a realizar
            switch (obj.ToString())
            {
                case "BuscarClick":
                    //Se oculta la ventana para iniciar la búsqueda
                    Ventana.Hide();
                    //Se obtienen los parámetros del año de registro y el folio del imputado
                    var AnioBusqueda = 0;
                    var FolioBusqueda = 0;
                    Int32.TryParse(AnioBuscar, out AnioBusqueda);
                    Int32.TryParse(FolioBuscar, out FolioBusqueda);
                    //Si alguno de los campos no esta vacío, entonces...
                    if (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar) || AnioBusqueda != 0 || FolioBusqueda != 0)
                    {
                        //Se inicia y espera a que la búsqueda de internos finalíce
                        ListExpediente = await BusquedaImputados((short)AnioBusqueda, FolioBusqueda);
                        //Si hubo resultados, se oculta la etiqueta que indica que no hay información
                        if (ListExpediente.Count > 0)
                            EmptyBusquedaVisible = false;
                        //Si no hubo resultados, se muestra la etiqueta que indica que no hay información
                        else
                            EmptyBusquedaVisible = true;
                    }
                    //Si ningún campo tiene información para buscar, se inicializa la lista de resultados en cero elementos
                    else
                    {
                        ListExpediente = new List<InternosActividad>();
                    }
                    //Al finalizar la búsqueda, se muestra la ventana de nuevo para ver los resultados
                    Ventana.ShowDialog();
                    break;
                case "SeleccionarImputado":
                    //Si el imputado seleccionado es diferente de nulo, entonces...
                    if (SelectedImputado != null)
                    {
                        //Si la foto del interno es igual a nulo, entonces...
                        if (SelectedImputado.FotoInterno == null)
                        {
                            try
                            {
                                //Se crea e inicializa el objeto para consultar la foto
                                var ingreso_biometrico = new cIngresoBiometrico();
                                //Se obtiene la imagen por defecto en caso de que el interno no tenga foto
                                var placeholder = new Imagenes().getImagenPerson();
                                //Se consultan las fotos de ingreso de frente y seguimiento de frente del interno
                                var foto_ingreso = ingreso_biometrico.Obtener(SelectedImputado.Anio, SelectedImputado.Centro, SelectedImputado.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                var foto_seguimiento = ingreso_biometrico.Obtener(SelectedImputado.Anio, SelectedImputado.Centro, SelectedImputado.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault();
                                //Si la foto de ingreso es diferente de nulo, se utiliza la foto. De lo contario, se utiliza la imagen por defecto
                                SelectedImputadoFotoIngreso = foto_ingreso != null ? foto_ingreso.BIOMETRICO : placeholder;
                                //Si la foto de seguimiento es diferente de nulo, se utiliza la foto. De lo contario, se utiliza la imagen por defecto
                                SelectedImputadoFotoSeguimiento = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : placeholder;
                            }
                            //Si hubo algún error al consultar las fotos del interno, se muestra un mensaje de error al usuario
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener fotos del imputado seleccionado.", ex);
                            }
                        }
                    }
                    break;
                case "0":
                    //Caso "0" - "9": Si el campo del ID tiene menos a 13 caractéres, entonces se escribe un número del 0 al 9 según el botón sobre el cuál ocurrió el evento
                    //Caso "backspace": Borra 1 caractér del campo del ID
                    //Caso "limpiarNIP": Borra todo el ID
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "0";
                    }
                    break;
                case "1":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "1";
                    }
                    break;
                case "2":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "2";
                    }
                    break;
                case "3":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "3";
                    }
                    break;
                case "4":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "4";
                    }
                    break;
                case "5":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "5";
                    }
                    break;
                case "6":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "6";
                    }
                    break;
                case "7":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "7";
                    }
                    break;
                case "8":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "8";
                    }
                    break;
                case "9":
                    if (NIPBuscar.Length < 13)
                    {
                        NIPBuscar += "9";
                    }
                    break;
                case "backspace":
                    if (NIPBuscar.Length > 0)
                    {
                        NIPBuscar = NIPBuscar.Substring(0, NIPBuscar.Length - 1);
                    }
                    break;
                case "limpiarNIP":
                    NIPBuscar = "";
                    break;
                case "onBuscarPorNIP":
                    var NIP = 0;
                    try
                    {
                        //Se obtiene el ID introducido en el campo del ID
                        if (Int32.TryParse(NIPBuscar, out NIP))
                        {
                            //Se busca el custodio utilizando su ID
                            SelectedCustodio = new cEmpleado().ObtenerEmpleadoPorDepartamento(NIP, (short)enumAreaTrabajo.COMANDANCIA);
                            //Si se encontró un custodio
                            if (SelectedCustodio != null)
                            {
                                //Se muestra la información del custodio encontrado
                                NombreCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.NOMBRE) ? SelectedCustodio.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty;
                                PaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.PATERNO) ? SelectedCustodio.PERSONA.PATERNO.Trim() : string.Empty : string.Empty;
                                MaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.MATERNO) ? SelectedCustodio.PERSONA.MATERNO.Trim()  :string.Empty : string.Empty;
                                AnioCustodio = SelectedCustodio.REGISTRO_FEC.HasValue ? SelectedCustodio.REGISTRO_FEC.Value.Year.ToString() : string.Empty;
                                IDCustodio = SelectedCustodio.ID_EMPLEADO.ToString();
                                FechaRegistroCustodio = SelectedCustodio.REGISTRO_FEC.HasValue ? SelectedCustodio.REGISTRO_FEC.Value.ToString() : string.Empty;
                                var consulta_foto_custodio = SelectedCustodio.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                                FotoCustodio = consulta_foto_custodio != null ? consulta_foto_custodio.BIOMETRICO : new Imagenes().getImagenPerson();
                                //Se notifica que se encontró un custodio
                                CambiarMensajeNIP(enumMensajeNIP.ENCONTRADO);
                            }
                            //Si no se encontró un custodio, entonces...
                            else
                            {
                                //Se notifica que no se encontró un custodio
                                CambiarMensajeNIP(enumMensajeNIP.NO_ENCONTRADO);
                            }
                        }
                    }
                    //Si ocurrio algun error al buscar el custodio, se muestra un mensaje de error al usuario
                    catch (Exception ex)
                    {
                        Ventana.Hide();
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener informacion de un custodio (Error al buscar por ID).", ex);
                    }
                    break;
                case "OpenCloseFlyout":
                    //Se evalua si se habilitó la captura del ID
                    if (CapturaNIPVisible)
                    {
                        //Se deshabilita la captura del ID
                        CapturaNIPVisible = false;
                        NIPBuscar = "";
                    }

                    else
                    {
                        //Se habilita la captura del ID
                        CapturaNIPVisible = true;
                    }
                    break;
                case "Autorizar":
                    //Si se autoriza la salida, se verifica si hay internos seleccionados. Si los hay, entonces...
                    if (ListaSeleccionados.Count > 0)
                    {
                        //Si la escolta esta habilitada y el custodio no ha sido asignado, entonces...
                        if (EscoltarEnabled && SelectedCustodio == null)
                            //Se notifica al usuario que debe asignar un custodio
                            await MensajeVentana(MessageDialogStyle.Affirmative, "Validación", "OPCIÓN DE ESCOLTA ACTIVA: Debe asignar un custodio.");
                        //Si la escolta no esta habilitada, entonces...
                        else
                        {
                            //Se solicita confirmación al usuario de la autorización de la salida
                            var result = await MensajeVentana(MessageDialogStyle.AffirmativeAndNegative, "Autorización", "¿Está seguro de autorizar la salida?");
                            //Si el resultado del diálogo de confirmación fue afirmativo, entonces...
                            if (result == MessageDialogResult.Affirmative)
                            {
                                //Se cambia la ubicación de los internos seleccionados
                                await CambiarUbicacionImputados();
                            }
                        }
                    }
                    //Si no hay internos seleccionados, entonces...
                    else
                        //Se notifica al usuario que debe seleccionar al menos un interno para poder realizar la operación
                        await MensajeVentana(MessageDialogStyle.Affirmative, "Validación", "Debe seleccionar al menos un interno.");
                    break;
                case "SeleccionarInternos":
                    //Si la lista de resultados de una búsqueda tiene elementos, entonces...
                    if (ListExpediente.Count > 0)
                    {
                        //Se crea e inicializa una lista temporal donde se almacenan los internos seleccionados de la búsqueda resultante
                        var lista_seleccionados = ListExpediente.Where(w => w.Seleccionar).ToList();

                        //Si hubo internos seleccionados, entonces...
                        if (lista_seleccionados.Count > 0)
                        {
                            //Se crea e inicializa una lista temporal donde alojar los internos seleccionados (esto en caso de que se hayan seleccionado internos previamente)
                            var lista_aux = new List<InternosActividad>(ListaSeleccionados);

                            //Se agregan los internos seleccionados a la lista temporal
                            lista_aux.AddRange(lista_seleccionados);

                            //Se itera sobre la lista temporal de los internos seleccionados y se inicializa el campo de Selección
                            foreach (var seleccionado in lista_aux)
                            {
                                seleccionado.Seleccionar = false;
                            }


                            //Se iguala la lista de seleccionados a la lista temporal que contiene tanto los internos seleccionados previamente como los nuevos internos seleccionados
                            ListaSeleccionados = lista_aux;

                            //La lista temporal  se iguala a la lista de resultados de la búsqueda
                            lista_aux = new List<InternosActividad>(ListExpediente);
                            //Se itera sobre la lista de seleccionados
                            foreach (var seleccionado in ListaSeleccionados)
                            {
                                //Se remueve el elemento actual. Esto es para eliminar de la lista de resultados de la búsqueda aquellos elementos que fueron seleccionados, con efecto
                                //de no mostrarlo en ambas listas (Resultados y Seleccionados)
                                lista_aux.Remove(seleccionado);
                            }
                            //Se iguala la lista de resultados de la búsqueda a la lista temporal
                            ListExpediente = lista_aux;
                        }
                        //Si la lista de seleccionados no tiene elementos, entonces...
                        else
                        {
                            //Se notifica al usuario que debe seleccionar al menos un interno de la lista de resultados
                            await Ventana.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.");
                        }
                    }
                    //Si la lista de resultados de una búsqueda no tiene elementos, entonces...
                    else
                    {
                        //Se notifica al usuario que no hay internos resultantes de una búsqueda que se puedan seleccionar
                        await Ventana.ShowMessageAsync("Validación", "No hay internos resultantes de la búsqueda. Por favor, intente más tarde realizando una nueva búsqueda.");
                    }
                    break;
                case "RemoverInternos":
                    //Si la lista de seleccionados tiene algún elemento, entonces...
                    if (ListaSeleccionados.Count > 0)
                    {
                        //Se crea e inicializa una lista temporal con los internos seleccionados
                        var lista_seleccionados = ListaSeleccionados.Where(w => w.Seleccionar).ToList();
                        //Si hubo algún seleccionado, entonces...
                        if (lista_seleccionados.Count > 0)
                        {
                            //Se crea e inicializa una lista donde se alojan los internos resultantes de una búsqueda
                            var lista_aux = new List<InternosActividad>(ListExpediente);
                            //Se agrega a la lista temporal los internos seleccionados
                            lista_aux.AddRange(lista_seleccionados);
                            //Se inicializa el campo de selección en los internos de la lista temporal
                            foreach (var seleccionado in lista_aux)
                            {
                                seleccionado.Seleccionar = false;
                            }
                            //Se iguala la lista de resultados de la búsqueda a la lista temporal
                            ListExpediente = lista_aux;
                            //Se iguala la lista temporal a la lista de seleccionados
                            lista_aux = new List<InternosActividad>(ListaSeleccionados);
                            foreach (var removido in lista_seleccionados)
                            {
                                //Se remueve el elemento de la lista temporal existente en la lista de los internos seleccionados, para limpiar la lista de selección
                                lista_aux.Remove(removido);
                            }
                            //Se iguala la lista de internos seleccionados a la lista temporal, para limpiar la lista
                            ListaSeleccionados = lista_aux;
                        }
                        //Si no hubo ningún interno seleccionado,entonces...
                        else
                        {
                            //Se notifica al usuario que se debe seleccionar al menos un interno
                            await Ventana.ShowMessageAsync("Validación", "Debe seleccionar al menos un interno.");
                        }
                    }
                    //Si la lista de seleccionados no tiene elementos, entonces...
                    else
                    {
                        //Se notifica al usuario que no existen elementos por remover
                        await Ventana.ShowMessageAsync("Validación", "No hay internos por remover. Solo puede remover elementos existentes en la lista.");
                    }
                    break;
                case "Limpiar":
                    //Se limpian los campos de información del custodio
                    NombreCustodio = "";
                    PaternoCustodio = "";
                    MaternoCustodio = "";
                    AnioCustodio = "";
                    IDCustodio = "";
                    FotoCustodio = new Imagenes().getImagenPerson();
                    break;
                case "LimpiarClick":
                    //Se limpia la información del interno seleccionado, de búsqueda y el interno asignado
                    var placeholder_limpiar = new Imagenes().getImagenPerson();
                    SelectedImputadoFotoIngreso = placeholder_limpiar;
                    SelectedImputadoFotoSeguimiento = placeholder_limpiar;
                    NombreBuscar = "";
                    ApellidoPaternoBuscar = "";
                    ApellidoMaternoBuscar = "";
                    AnioBuscar = "";
                    FolioBuscar = "";
                    ListExpediente = new List<InternosActividad>();
                    EmptyBusquedaVisible = true;
                    SelectedCustodio = null;
                    break;
            }

        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de colocar el cursor sobre uno de
        /// los botones ("BORRAR UN CARACTÉR" Y "BORRAR TODO EL ID").
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento sobre el cuál se posicionó el cursor.</param>
        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    break;
            }
        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de quitar el cursor sobre uno de
        /// los botones ("BORRAR UN CARACTÉR" Y "BORRAR TODO EL ID").
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento del cuál se quitó el cursor.</param>
        private void MouseLeaveSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
                    break;
            }
        }

        /// <summary>
        /// Método que se encarga de realizar la accion posterior de comparación a la captura de la huella del custodio.
        /// </summary>
        /// <param name="captureResult">Resultado de captura del lector.</param>
        public override void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                //Si la captura del ID no esta visible,entonces...
                if (!CapturaNIPVisible)
                {
                    //Se indica que la comparación de huellas esta en proceso
                    ProgressRingVisible = Visibility.Visible;
                    ImagenEvaluacionVisible = false;
                    //Se comparala huella del interno
                    base.OnCaptured(captureResult);
                    CompararHuellaCustodio();
                    //Se indica que la comparación de huellas terminó
                    ProgressRingVisible = Visibility.Collapsed;
                    ImagenEvaluacionVisible = true;
                }
            }
            //Si hubo algún error con la lectura de huellas, entonces...
            catch (Exception)
            {
                //Se notifica al usuario que hubo un error con la lectura
                ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                ScannerMessage = "Lectura Fallida.";
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Método que realiza la búsqueda de internos de acuerdo a los parámetros de búsqueda (Nombre, Apellido Paterno, Apellido Materno, Año y Folio/ID de Imputado)
        /// </summary>
        /// <param name="AnioBusqueda">Año de Ingreso</param>
        /// <param name="FolioBusqueda">Folio/ID de Imputado</param>
        /// <returns></returns>
        public async Task<List<InternosActividad>> BusquedaImputados(short AnioBusqueda, int FolioBusqueda)
        {
            var imputados_busqueda = new List<InternosActividad>();
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    //Se limpia búsqueda anterior
                    ListExpediente.Clear();
                    System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                    //Se inicia nueva búsqueda
                    List<GRUPO_ASISTENCIA> imputados = new cGrupoAsistencia().ObtenerInternos(NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, AnioBusqueda, FolioBusqueda, Areas).Where(w =>

                        //Filtra el centro en el que se encuentran
                        w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&


                        //Filtra con la fecha del server
                        w.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                     w.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                     w.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                    (w.GRUPO_HORARIO.HORA_INICIO.Value.Hour <= _fechaHoy.Hour &&
                     w.GRUPO_HORARIO.HORA_TERMINO.Value.Hour > _fechaHoy.Hour) &&

                     //Se filtra a los internos que no tienen su actividad cancelada
                      w.ESTATUS != (short)enumGrupoAsistenciaEstatus.CANCELADO &&

                        //Filtra a los internos con traslados en proceso ó activos
                     (w.GRUPO_PARTICIPANTE.INGRESO.TRASLADO_DETALLE.Where(
                     wTD =>
                               wTD.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                               wTD.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                               wTD.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day &&
                               (wTD.ID_ESTATUS == TRASLADO_EN_PROCESO || wTD.ID_ESTATUS == TRASLADO_ACTIVO)).Count() == 0) &&

                        //Filtra a los internos con excarcelaciones en proceso ó activas
                     (w.GRUPO_PARTICIPANTE.INGRESO.EXCARCELACION.Where(
                     wEXC =>
                               wEXC.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                               wEXC.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                               wEXC.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                               wEXC.ID_ESTATUS == EXCARCELACION_EN_PROCESO || wEXC.ID_ESTATUS == EXCARCELACION_AUTORIZADA).Count() == 0)
                        ).ToList();
                    var ingresoUbicacion = new cIngresoUbicacion();
                    var ingreso = new cIngreso();
                    //Se itera sobre la lista de imputados encontrados de acuerdo a los criterios de la búsqueda
                    foreach (var imputado in imputados)
                    {
                        //Condición - si la lista de los seleccionados con anterioridad no tiene a algún imputado de la búsqueda, se procede con la siguiente condición
                        if (!ListaSeleccionados.Where(w =>
                                    w.Anio == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                    w.Centro == imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                    w.IdImputado == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Any() &&

                        //Condición - Si la lista de los resultados no tiene a algún imputado de la búsqueda, se procede a recopilar todas las participaciones encontradas en la búsqueda.
                            //Esto se realiza directamente de la tabla "grupo_asistencia" para saber si el interno sobre el que se encuentra en el ciclo "foreach" tiene más de una actividad
                            //En las áreas que contempla el equipo, y así, decidir cuál es la información de la actividad que se mostrará en la ventana de búsqueda
                            !imputados_busqueda.Where(w =>
                                    w.Anio == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                    w.Centro == imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                    w.IdImputado == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).Any())
                        {
                            //Se obtienen las actividades del imputado actual en la lista de resultados de búsqueda
                            var actividades = imputados.Where(w =>
                                        w.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                        w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();
                            GRUPO_ASISTENCIA actividad_elegida = null;


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
                                actividad_elegida = resolucion_coordinacion != null ? resolucion_coordinacion : resolucion_actividad_mas_antigua;
                            }
                            //Si solo se encontró una, entonces...
                            else
                            {
                                //Se toma la actividad encontrada
                                actividad_elegida = actividades.FirstOrDefault();
                            }

                            //Se toma la última ubicación del imputado
                            var ultima_ubicacion = ingresoUbicacion.Obtener((short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).OrderByDescending(o => o.MOVIMIENTO_FEC).FirstOrDefault();
                            var ultimo_ingreso = ingreso.ObtenerUltimoIngreso((short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO, (int)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO);
                            //Se agrega el imputado a los resultados finales de la búsqueda
                            imputados_busqueda.Add(new InternosActividad()
                            {
                                Actividad = actividad_elegida != null ? actividad_elegida.GRUPO_PARTICIPANTE != null ? actividad_elegida.GRUPO_PARTICIPANTE.ACTIVIDAD != null ? !string.IsNullOrEmpty(actividad_elegida.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? actividad_elegida.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Anio = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO : new short() : new short(),
                                Area = imputado.GRUPO_HORARIO != null ? imputado.GRUPO_HORARIO.ID_AREA.HasValue ? (short)imputado.GRUPO_HORARIO.ID_AREA : new short() : new short(),
                                Asistencia = imputado.ASISTENCIA == 1,
                                Centro = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_CENTRO : new short() : new short(),
                                Expediente = string.Format("{0}/{1}",
                                    imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.HasValue ? imputado.GRUPO_PARTICIPANTE.ING_ID_ANIO.Value.ToString() : string.Empty : string.Empty,
                                    imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.Value.ToString() : string.Empty : string.Empty),
                                IdIngreso = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_INGRESO : new short() : new short(),
                                IdImputado = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO : new short() : new short(),
                                Id_Grupo = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.ID_GRUPO.HasValue ? (short)imputado.GRUPO_PARTICIPANTE.ID_GRUPO : new short() : new short(),
                                IdConsec = (short)imputado.ID_CONSEC,
                                Materno = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Paterno = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Nombre = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                NIP = imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.INGRESO != null ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP) ? imputado.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NIP.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Ubicacion = imputado.GRUPO_HORARIO != null ?  imputado.GRUPO_HORARIO.AREA != null ? !string.IsNullOrEmpty(imputado.GRUPO_HORARIO.AREA.DESCR) ? imputado.GRUPO_HORARIO.AREA.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                                Seleccionar = false,
                                Responsable = string.Format("{1} {2} {0}",
                                imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty, 
                                imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                imputado.GRUPO_PARTICIPANTE != null ? imputado.GRUPO_PARTICIPANTE.GRUPO != null ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA != null ? !string.IsNullOrEmpty(imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO) ? imputado.GRUPO_PARTICIPANTE.GRUPO.PERSONA.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty),
                                TrasladoInterno = ultimo_ingreso != null ? ultimo_ingreso.TRASLADO_DETALLE != null ? ultimo_ingreso.TRASLADO_DETALLE.Any() ? ultimo_ingreso.TRASLADO_DETALLE.Where(w =>
                                 w.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                                 w.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                                 w.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day &&
                                 w.ID_ESTATUS == "PR").Count() == 1 : false : false : false,
                                ExcarcelacionInterno = ultimo_ingreso != null ? ultimo_ingreso.EXCARCELACION != null ? ultimo_ingreso.EXCARCELACION.Any() ? ultimo_ingreso.EXCARCELACION.Where(w =>
                                w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                                w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                                w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                                w.ID_ESTATUS == "AU").ToList().Count > 0 : false : false : false,
                                VisitaLegalInterno = ultimo_ingreso != null ? ultimo_ingreso.ADUANA_INGRESO != null ? ultimo_ingreso.ADUANA_INGRESO.Any() ? ultimo_ingreso.ADUANA_INGRESO.Count(c =>
                                              c.ENTRADA_FEC.Value.Year == _fechaHoy.Year &&
                                              c.ENTRADA_FEC.Value.Month == _fechaHoy.Month &&
                                              c.ENTRADA_FEC.Value.Day == _fechaHoy.Day &&
                                              c.INTERNO_NOTIFICADO == null) > 0 : false : false : false,

                                //Valor que indica si este imputado es candidato para alguna salida y si se encuentra en alguna de las áreas que contempla el equipo. si no lo es ó no se encuentra
                                //En alguna de las áreas, en primera instancia, no se puede seleccionar
                                Seleccionable = ultima_ubicacion != null ?
                                (ultima_ubicacion.ESTATUS == (short)enumEstatusUbicacion.EN_ACTIVIDAD && actividad_elegida.ASISTENCIA == 1 &&
                                 (ultimo_ingreso.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == TRASLADO_PROGRAMADO &&
                                 w.TRASLADO.TRASLADO_FEC.Year == _fechaHoy.Year &&
                                 w.TRASLADO.TRASLADO_FEC.Month == _fechaHoy.Month &&
                                 w.TRASLADO.TRASLADO_FEC.Day == _fechaHoy.Day &&
                                 ((((w.TRASLADO.TRASLADO_FEC.Hour - _fechaHoy.Hour) <= Parametro.TOLERANCIA_TRASLADO_EDIFICIO) &&
                                 _fechaHoy.Minute == 0)) ||
                                 (((w.TRASLADO.TRASLADO_FEC.Hour - _fechaHoy.Hour) < Parametro.TOLERANCIA_TRASLADO_EDIFICIO) &&
                                 (_fechaHoy.Minute >= 0 && _fechaHoy.Minute <= 59))).Count() == 1 ||
                                 ultimo_ingreso.EXCARCELACION.Where(w => w.ID_ESTATUS == EXCARCELACION_AUTORIZADA &&
                                 w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                                 w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                                 w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                                 ((((w.PROGRAMADO_FEC.Value.Hour - _fechaHoy.Hour) <= Parametro.TOLERANCIA_EXC_EDIFICIO) &&
                                 _fechaHoy.Minute == 0)) ||
                                 (((w.PROGRAMADO_FEC.Value.Hour - _fechaHoy.Hour) < Parametro.TOLERANCIA_EXC_EDIFICIO) &&
                                 (_fechaHoy.Minute >= 0 && _fechaHoy.Minute <= 59))
                                 ).Count() > 0)) : false
                            });
                        }
                    }
                });
            }
            //Si ocurrio algún error con la búsqueda, se le notifica al usuario
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de participantes.", ex);
            }
            return imputados_busqueda;
        }

        /// <summary>
        /// Método de comparación de la información encontrada de la huella capturada por el lector, llamado previamente desde el método sobre escrito "OnCaptured".
        /// </summary>
        public void CompararHuellaCustodio()
        {
            //Se obtiene la huella capturada
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            //Se inicializa un mensaje de resultado
            enumMensajeHuella mensajeResultado = enumMensajeHuella.NO_ENCONTRADO;
            //Si la huella es nula, entonces...
            if (bytesHuella == null)
            {
                //Se indica que la huella es vacía
                mensajeResultado = enumMensajeHuella.HUELLA_VACIA;
            }
            //Si la huella no es nula, entonces...
            else
            {
                //Se notifica al usuario que la comparación de información de la huella esta en proceso
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    CambiarMensajeHuella(enumMensajeHuella.EN_PROCESO);
                }));
                try
                {
                    //Se obtiene el universo de huellas de los custodios
                    var huellas_custodios = new cEmpleado().ObtenerTodosLosCustodios().SelectMany(sm =>
                    sm.PERSONA.PERSONA_BIOMETRICO).Where(w =>
                    w.ID_TIPO_BIOMETRICO == (short)SelectedFinger &&
                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP).AsEnumerable().Select(s =>
                    new Custodio_Huella
                    {
                        PERSONA = new cHuellasPersona() { ID_PERSONA = s.ID_PERSONA },
                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                        tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                    }).ToList();

                    //Se realiza la comparación de la huella
                    var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, huellas_custodios.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                    //Se crea e inicializa la lista donde se almacenaran los resultados encontrados
                    var result = new List<object>();
                    //Si la identificación y comparación no fue exitosa, entonces...
                    if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                    {
                        //Se evalua el caso ocurrido en la comparación de acuerdo a la operación del lector
                        switch (doIdentify.ResultCode)
                        {
                            case Constants.ResultCode.DP_DEVICE_BUSY:
                                break;
                            case Constants.ResultCode.DP_DEVICE_FAILURE:
                                break;
                            case Constants.ResultCode.DP_ENROLLMENT_INVALID_SET:
                                break;
                            case Constants.ResultCode.DP_ENROLLMENT_IN_PROGRESS:
                                break;
                            case Constants.ResultCode.DP_ENROLLMENT_NOT_READY:
                                break;
                            case Constants.ResultCode.DP_ENROLLMENT_NOT_STARTED:
                                break;
                            case Constants.ResultCode.DP_FAILURE:
                                break;
                            case Constants.ResultCode.DP_INVALID_DEVICE:
                                break;
                            case Constants.ResultCode.DP_INVALID_FID:
                                break;
                            case Constants.ResultCode.DP_INVALID_FMD:
                                break;
                            case Constants.ResultCode.DP_INVALID_PARAMETER:
                                break;
                            case Constants.ResultCode.DP_MORE_DATA:
                                break;
                            case Constants.ResultCode.DP_NOT_IMPLEMENTED:
                                break;
                            case Constants.ResultCode.DP_NO_DATA:
                                break;
                            case Constants.ResultCode.DP_TOO_SMALL_AREA:
                                break;
                            case Constants.ResultCode.DP_VERSION_INCOMPATIBILITY:
                                break;
                            default:
                                break;
                        }
                    }
                    //Si la comparación fue exitosa, entonces...
                    else
                    {
                        //Se almacenan los resultados de la comparación
                        foreach (var resultado in doIdentify.Indexes.ToList())
                        {
                            result.Add(huellas_custodios[resultado.FirstOrDefault()].PERSONA);
                        }
                    }
                    //Si fue únicamente un resultado, entonces...
                    if (result.Count == 1)
                    {
                        //Se obtiene el ID de la huella encontrada
                        var id_persona = ((cHuellasPersona)result.FirstOrDefault()).ID_PERSONA;
                        //Se busca el custodio de acuerdo al ID y se asigna si se encuentra
                        SelectedCustodio = new cEmpleado().ObtenerEmpleadoPorDepartamento(id_persona, (short)enumAreaTrabajo.COMANDANCIA);
                        //Si el custodio encontrado y asignado no es nulo, entonces...
                        if (SelectedCustodio != null)
                        {
                            //Se muestra la información del custodio asignado
                            NombreCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.NOMBRE) ? SelectedCustodio.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty;
                            PaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.PATERNO) ? SelectedCustodio.PERSONA.PATERNO.Trim() : string.Empty : string.Empty;
                            MaternoCustodio = SelectedCustodio.PERSONA != null ? !string.IsNullOrEmpty(SelectedCustodio.PERSONA.MATERNO) ? SelectedCustodio.PERSONA.MATERNO.Trim() : string.Empty : string.Empty;
                            AnioCustodio = SelectedCustodio.REGISTRO_FEC.HasValue ? SelectedCustodio.REGISTRO_FEC.Value.Year.ToString() : string.Empty;
                            IDCustodio = SelectedCustodio.ID_EMPLEADO.ToString();
                            FechaRegistroCustodio = SelectedCustodio.REGISTRO_FEC.HasValue ? SelectedCustodio.REGISTRO_FEC.Value.ToString() : string.Empty;
                            var consulta_foto_custodio = SelectedCustodio.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault();
                            FotoCustodio = consulta_foto_custodio != null ? consulta_foto_custodio.BIOMETRICO : new Imagenes().getImagenPerson();
                            mensajeResultado = enumMensajeHuella.ENCONTRADO;
                        }
                        //Si no se encontró un custodio, entonces...
                        else
                        {
                            //Se limpian los campos del custodio y se indica que no se encontró el custodio
                            NombreCustodio = "";
                            PaternoCustodio = "";
                            MaternoCustodio = "";
                            AnioCustodio = "";
                            IDCustodio = "";
                            FotoCustodio = new Imagenes().getImagenPerson();
                            mensajeResultado = enumMensajeHuella.NO_ENCONTRADO;
                        }
                    }
                    //Si la comparación obtuvo un resultado diferente a uno, entonces...
                    else
                    {
                        //Si hubo más de un resultado, se indica que hubo coincidencias. Si no hubo ningún resultado, se indica que no se encontró el custodio
                        //y se limpian los campos del custodio
                        mensajeResultado = result.Count == 0 ? enumMensajeHuella.NO_ENCONTRADO : enumMensajeHuella.FALSO_POSITIVO;
                        NombreCustodio = "";
                        PaternoCustodio = "";
                        MaternoCustodio = "";
                        AnioCustodio = "";
                        IDCustodio = "";
                        FotoCustodio = new Imagenes().getImagenPerson();
                    }
                }
                //Si ocurrió un error a la hora de obtener el custodio por huella, se notifica al usuario
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener al custodio.", ex);
                }
            }
            //Se notifica el resultado de comparación de información al usuario
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                CambiarMensajeHuella(mensajeResultado);
            }));
        }

        /// <summary>
        /// Cambia la ubicación de los internos seleccionados de acuerdo al asunto que tengan los internos seleccionados.
        /// </summary>
        /// <returns>Valor booleano que indica si la operacion fue exitosa o no.</returns>
        public async Task<bool> CambiarUbicacionImputados()
        {
            //Se inicializa el verificador de asuntos
            var asuntos = 0;
            //Si en la lista de seleccionados hay internos con traslados, entonces...
            if (ListaSeleccionados.Where(w => w.TrasladoInterno).Any())
            {
                //Se indica que hay un asunto en la lista de seleccionados que atender
                asuntos++;
                //Se indica que la justificación o asunto de la salida es un traslado
                SelectedJustificacion = enumTipoSalida.TRASLADO;
            }

            //Si en la lista de seleccionados hay internos con excarcelaciones, entonces...
            if (ListaSeleccionados.Where(w => w.ExcarcelacionInterno).Any())
            {
                asuntos++;
                SelectedJustificacion = enumTipoSalida.EXCARCELACION;
            }

            //Si en la lista de seleccionados hay internos con visitas legales, entonces...
            if (ListaSeleccionados.Where(w => w.VisitaLegalInterno).Any())
            {
                asuntos++;
                SelectedJustificacion = enumTipoSalida.LEGAL;
            }

            //Si en la lista de seleccionados hay internos con visitas legales, entonces...
            if (ListaSeleccionados.Where(w => w.VisitaActuarioInterno).Any())
            {
                asuntos++;
                SelectedJustificacion = enumTipoSalida.ACTUARIO;
            }

            //Si solo se identificó un asunto, entonces...
            if (asuntos < 2)
            {
                //Se oculta la ventana para efectos de muestreo de realización de operación al usuario
                Ventana.Hide();
                try
                {
                    //Se inicializa el destino
                    var destino = "";

                    //Se realiza la operación de manera asíncrona
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        //Se crea e inicializa el objeto con el que se realizará la operación de cambio de ubicación
                        var ingreso_ubicacion = new cIngresoUbicacion();
                        //Se pregunta si existe un médico en el centro
                        var existeMedico = new cAduana().ExisteMedico();
                        //Se itera sobre la lista de seleccionados

                        System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                        foreach (var imputado in ListaSeleccionados)
                        {

                            int? nulo = null;

                            //Se evalua el asunto de los internos seleccionados
                            switch (SelectedJustificacion.Value)
                            {
                                case enumTipoSalida.TRASLADO:
                                    //Se evalua si existe médico en el centro y se determina el área a la que irán los internos de acuerdo a si existe o no
                                    var area_traslado = existeMedico ? (short)enumAreaSalida.AREA_MEDICA_PB : (short)enumAreaSalida.SALIDA_DEL_CENTRO;
                                    //Se evalua si el area de destino es el área medica para determinar la actividad
                                    var actividad_traslado = area_traslado == (short)enumAreaSalida.AREA_MEDICA_PB ? "CERTIFICACIÓN MÉDICA" : "TRASLADO";
                                    //Se crea el objeto con el cual se obtiene la información del traslado del interno
                                    var traslado_detalle = new cTrasladoDetalle();
                                    //Se consulta la información del traslado del interno
                                    var consulta_traslado_detalle = traslado_detalle.Obtener(imputado.Centro, imputado.Anio, imputado.IdImputado, imputado.IdIngreso, TRASLADO_PROGRAMADO);
                                    //Se determina si el mensaje de destino es vacío 
                                    if (string.IsNullOrEmpty(destino))
                                    {
                                        //Se determina el destino a mostrar en el mensaje para información del custodio
                                        destino = actividad_traslado == "CERTIFICACIÓN MÉDICA" ? "REQUIERE CERTIFICACIÓN MÉDICA" : "CERTIFICACIÓN MÉDICA NO REQUERIDA O NO DISPONIBLE";
                                    }

                                    //Se inserta la nueva ubicación del interno y se modifica el estatus del traslado del interno
                                    ingreso_ubicacion.InsertarNuevaUbicacion(new INGRESO_UBICACION()
                                    {
                                        ID_CENTRO = imputado.Centro,
                                        ID_ANIO = imputado.Anio,
                                        ID_IMPUTADO = imputado.IdImputado,
                                        ID_INGRESO = imputado.IdIngreso,
                                        ID_AREA = area_traslado,
                                        MOVIMIENTO_FEC = _fechaHoy,
                                        ACTIVIDAD = actividad_traslado,
                                        ESTATUS = (short)enumEstatusUbicacion.EN_TRANSITO,
                                        ID_CUSTODIO = SelectedCustodio != null ? SelectedCustodio.ID_EMPLEADO : nulo
                                    }, null
                                    , new TRASLADO_DETALLE()
                                    {
                                        ID_CENTRO = imputado.Centro,
                                        ID_ANIO = imputado.Anio,
                                        ID_IMPUTADO = imputado.IdImputado,
                                        ID_INGRESO = imputado.IdIngreso,
                                        ID_TRASLADO = consulta_traslado_detalle.ID_TRASLADO,
                                        ID_CENTRO_TRASLADO = consulta_traslado_detalle.ID_CENTRO_TRASLADO,
                                        ID_ESTATUS = TRASLADO_EN_PROCESO
                                    });
                                    break;

                                case enumTipoSalida.EXCARCELACION:
                                    //Se crea el objeto con el cual se obtiene la información del traslado del interno
                                    var excarcelacion = new cExcarcelacion();
                                    //Se consulta la información de la excarcelación del interno
                                    var consulta_excarcelacion = excarcelacion.ObtenerImputadoExcarcelaciones(imputado.Centro, imputado.Anio, imputado.IdImputado, imputado.IdIngreso).Where(w =>
                                        w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                                        w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                                        w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                                        w.ID_ESTATUS == EXCARCELACION_AUTORIZADA).OrderBy(o =>
                                        o.PROGRAMADO_FEC).FirstOrDefault();

                                    //Se evalua si existe médico en el centro y se determina el área a la que irán los internos de acuerdo a si existe o no
                                    var area_excarcelacion = existeMedico ? (consulta_excarcelacion.CERTIFICADO_MEDICO == 1 ? (short)enumAreaSalida.AREA_MEDICA_PB : (short)enumAreaSalida.SALIDA_DEL_CENTRO) : (short)enumAreaSalida.SALIDA_DEL_CENTRO;

                                    var actividad_excarcelacion = area_excarcelacion == (short)enumAreaSalida.AREA_MEDICA_PB ? "CERTIFICACIÓN MÉDICA" : "EXCARCELACIÓN";
                                    if (string.IsNullOrEmpty(destino))
                                    {
                                        //Se determina el destino a mostrar en el mensaje para información del custodio
                                        destino = actividad_excarcelacion == "CERTIFICACIÓN MÉDICA" ? "REQUIERE CERTIFICACIÓN MÉDICA" : "CERTIFICACIÓN MÉDICA NO REQUERIDA O NO DISPONIBLE";
                                    }

                                    //Se inserta la nueva ubicación del interno y se modifica el estatus de la excarcelación del interno
                                    ingreso_ubicacion.InsertarNuevaUbicacion(new INGRESO_UBICACION()
                                    {
                                        ID_CENTRO = imputado.Centro,
                                        ID_ANIO = imputado.Anio,
                                        ID_IMPUTADO = imputado.IdImputado,
                                        ID_INGRESO = imputado.IdIngreso,
                                        ID_AREA = area_excarcelacion,
                                        MOVIMIENTO_FEC = _fechaHoy,
                                        ACTIVIDAD = actividad_excarcelacion,
                                        ESTATUS = (short)enumEstatusUbicacion.EN_TRANSITO,
                                        ID_CUSTODIO = SelectedCustodio != null ? SelectedCustodio.ID_EMPLEADO : nulo
                                    }, new EXCARCELACION()
                                    {
                                        ID_CENTRO = imputado.Centro,
                                        ID_ANIO = imputado.Anio,
                                        ID_IMPUTADO = imputado.IdImputado,
                                        ID_INGRESO = imputado.IdIngreso,
                                        ID_CONSEC = consulta_excarcelacion.ID_CONSEC,
                                        ID_ESTATUS = EXCARCELACION_EN_PROCESO
                                    }
                                    );
                                    break;
                                default:
                                    if (string.IsNullOrEmpty(destino))
                                    {
                                        //Se asigna el área destino de cualquiera de los tipos de visitas legales (Actuario o Visita Legal)
                                        destino = "LOCUTORIOS";
                                    }

                                    //Se busca el registro de la aduana sobre el cuál se indicara que el interno no ha sido notificado aún
                                    var consulta_aduana_ingreso = new cAduanaIngreso().ObtenerAduanaIngresoSinNotificacion(imputado.Centro, imputado.Anio, imputado.IdImputado, imputado.IdIngreso, _fechaHoy).ToList().FirstOrDefault();

                                    //Se inserta la nueva ubicación del interno y su estatus actual en cuanto a la ubicación
                                    ingreso_ubicacion.InsertarNuevaUbicacion(new INGRESO_UBICACION()
                                    {
                                        ID_CENTRO = imputado.Centro,
                                        ID_ANIO = imputado.Anio,
                                        ID_IMPUTADO = imputado.IdImputado,
                                        ID_INGRESO = imputado.IdIngreso,
                                        ID_AREA = (short)enumAreaSalida.LOCUTORIOS,
                                        MOVIMIENTO_FEC = _fechaHoy,
                                        ACTIVIDAD = SelectedJustificacion == enumTipoSalida.LEGAL ? "VISITAL LEGAL" : "ACTUARIO",
                                        ESTATUS = (short)enumEstatusUbicacion.EN_TRANSITO,
                                        ID_CUSTODIO = SelectedCustodio != null ? SelectedCustodio.ID_EMPLEADO : nulo
                                    }, null, null, new ADUANA_INGRESO()
                                    {
                                        ID_ADUANA = consulta_aduana_ingreso.ID_ADUANA,
                                        ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                                        ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                                        ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                                        ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                                        INTERNO_NOTIFICADO = "N"
                                    });
                                    break;
                            }
                        }
                    });

                    //Se limpian todos los campos de la ventana y se notifica al usuario el estado de la operación y el destino de la salida
                    var placeholder = new Imagenes().getImagenPerson();
                    ListaSeleccionados = new List<InternosActividad>();
                    ListExpediente = new List<InternosActividad>();
                    SelectedCustodio = null;
                    FotoCustodio = placeholder;
                    NombreCustodio = "";
                    PaternoCustodio = "";
                    MaternoCustodio = "";
                    AnioCustodio = "";
                    IDCustodio = "";
                    SelectedImputadoFotoIngreso = placeholder;
                    SelectedImputadoFotoSeguimiento = placeholder;
                    NombreBuscar = "";
                    ApellidoPaternoBuscar = "";
                    ApellidoMaternoBuscar = "";
                    Ventana.Show();
                    var caso_mensaje = SelectedJustificacion.Value == enumTipoSalida.TRASLADO ? "Traslado En Proceso." : (SelectedJustificacion.Value == enumTipoSalida.EXCARCELACION ? "Excarcelación En Proceso." : (SelectedJustificacion.Value == enumTipoSalida.LEGAL ? "Visita Legal." : "Visita de Actuario"));
                    var metro = Ventana as MetroWindow;
                    await metro.ShowMessageAsync("Operación Exitosa", string.Format("{0} - {1}", caso_mensaje, destino), MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "Aceptar" });
                }
                //En caso de que un registro de alguna salida no se encuentre para el momento de la operacion, se notifica al usuario que no se encontró una salida registrada
                //y el asunto en el que se encontraba
                catch (NullReferenceException nullEx)
                {
                    var caso = "";

                    //Se evalua el asunto en el que se encontraba la operación
                    switch (SelectedJustificacion.Value)
                    {
                        case enumTipoSalida.TRASLADO:
                            caso = "TRASLADO";
                            break;
                        case enumTipoSalida.EXCARCELACION:
                            caso = "EXCARCELACIÓN";
                            break;
                        case enumTipoSalida.ACTUARIO:
                            caso = "ACTUARIO";
                            break;
                        case enumTipoSalida.LEGAL:
                            caso = "LEGAL";
                            break;
                    }
                    StaticSourcesViewModel.ShowMessageError("Algo paso...", string.Format("No se encontró una salida registrada. ASUNTO: {0}", caso), nullEx);
                }
                //Si ocurrió un error en la operación, se notifica al usuario
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar la ubicación de los imputados.", ex);
                }
                return true;
            }
            //Si se identificó más de un asunto, se le notifica al usuario que no se pueden combinar internos con distintos tipos de salidas y se termina la operación
            else
            {
                var metro = Ventana as MetroWindow;
                await metro.ShowMessageAsync("Validación", "No se permite mezclar asuntos en las salidas. Revise la selección de imputados, por favor.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "Aceptar" });
                return false;
            }

        }
        #endregion

        #region Métodos Mensajes Interfaz
        /// <summary>
        /// Muestra el mensaje de resultado de la comparación del ID del custodio.
        /// </summary>
        /// <param name="TipoMensaje">Resultado que indica el tipo de mensaje a mostrar.</param>
        public async void CambiarMensajeNIP(enumMensajeNIP TipoMensaje)
        {
            switch (TipoMensaje)
            {
                case enumMensajeNIP.ENCONTRADO:
                    CheckMark = "\u2713 \u2713";
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Green);
                    ScannerMessage = "Custodio Encontrado";
                    ImagenEvaluacion = new Imagenes().getImagenPermitido();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    CapturaNIPVisible = false;
                    break;
                case enumMensajeNIP.NO_ENCONTRADO:
                    CheckMark = "X";
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    break;
            }
            await TaskEx.Delay(1500);
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            NIPBuscar = "";

        }

        /// <summary>
        /// Muestra el mensaje de resultado de la comparación de la huella del custodio.
        /// </summary>
        /// <param name="TipoMensaje">Resultado que indica el tipo de mensaje a mostrar.</param>
        public async void CambiarMensajeHuella(enumMensajeHuella TipoMensaje)
        {
            var imagen = new Imagenes();
            switch (TipoMensaje)
            {
                case enumMensajeHuella.EN_ESPERA:
                    ScannerMessage = "Capture Huella";
                    ImagenEvaluacion = imagen.getImagenHuella();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    break;
                case enumMensajeHuella.ENCONTRADO:
                    ScannerMessage = "Custodio Encontrado";
                    ImagenEvaluacion = imagen.getImagenPermitido();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    break;
                case enumMensajeHuella.NO_ENCONTRADO:
                    ScannerMessage = "Custodio No Encontrado";
                    ImagenEvaluacion = imagen.getImagenDenegado();
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    break;
                case enumMensajeHuella.FALSO_POSITIVO:
                    ScannerMessage = "Capture de nuevo";
                    ImagenEvaluacion = imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    break;
                case enumMensajeHuella.EN_PROCESO:
                    ScannerMessage = "Procesando...";
                    ColorAprobacion = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                    break;
            }

            await TaskEx.Delay(1500);
            ScannerMessage = "Capture Huella";
            ImagenEvaluacion = imagen.getImagenHuella();
            ColorAprobacion = new SolidColorBrush(Colors.Green);
        }

        /// <summary>
        /// Muestra los mensajes requeridos por la ventana.
        /// </summary>
        /// <param name="tipoMensaje">Tipo de mensaje a mostrar.</param>
        /// <param name="titulo">Titulo del mensaje.</param>
        /// <param name="mensaje">Cuerpo del mensaje.</param>
        /// <returns>Resultado del mensaje.</returns>
        public async Task<MessageDialogResult> MensajeVentana(MessageDialogStyle tipoMensaje, string titulo, string mensaje)
        {
            var metro = Ventana as MetroWindow;
            MetroDialogSettings mySettings = null;
            //Se evalua el tipo de mensaje a mostrar (Decisión y De Aceptación)
            switch (tipoMensaje)
            {
                case MessageDialogStyle.Affirmative:
                    mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Cerrar",
                    };
                    break;
                case MessageDialogStyle.AffirmativeAndNegative:
                    mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Sí",
                        NegativeButtonText = "No"
                    };
                    break;
            }
            mySettings.AnimateShow = true;
            mySettings.AnimateHide = false;
            return await metro.ShowMessageAsync(titulo, mensaje, tipoMensaje, mySettings);
        }
        #endregion
    }
}