using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class LeerInternosViewModel : FingerPrintScanner
    {
        #region Constructor
        /// <summary>
        /// Constructor de la clase:Inicializa las áreas que contempla el equipo y las huellas corresponden a dichas áreas
        /// </summary>
        public LeerInternosViewModel()
        {
            try
            {
                //Se obtienen las áreas que contempla el equipo
                ObtenerAreas();
                //Se obtiene el universo de huellas
                SeleccionHuella(enumTipoBiometrico.INDICE_DERECHO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Métodos Eventos
        /// <summary>
        /// Método al cuál se le delega la responsabilidad de inicializar las propiedades de la ventana, asi como tambien
        /// la de abrir el lector de huellas digitales.
        /// </summary>
        /// <param name="Window">Ventana de la que proviene el evento.</param>
        public void OnLoad(LeerInternos Window)
        {
            TextoIncidencia = "FALSO POSITIVO";
            NIPBuscar = "";
            CampoCaptura = "NIP:";
            var obtener_imagen = new Imagenes();
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FotoLectura = obtener_imagen.getImagenPerson();
            ImagenEvaluacionVisible = Readers.Count > 0;
            ImagenEvaluacion = obtener_imagen.getImagenHuella();
            ScannerMessage = "Capture Huella";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;
            ScannerMessageVisible = true;
            //Se revisa si existen lectores de huellas conectados al equipo. Si no hay, entonces...
            if (Readers.Count == 0)
            {
                //Se indica que se capture el NIP directamente
                ScannerMessage = "Capture NIP";
            }
            //Si hay lectores, entonces...
            else
            {
                //Se indica que pueden capturarse huellas en el lector
                ScannerMessage = "Capture Huella\n en el lector";
            }

            //Se inicializa el lector de huellas
            Window.Closed += (s, e) =>
            {

                try
                {
                    if (OnProgress == null)
                        return;

                    if (!_IsSucceed)
                        //SelectedCustodio = null;

                        OnProgress.Abort();
                    CancelCaptureAndCloseReader(OnCaptured);
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lectura de las huellas", ex);
                }
            };

            if (CurrentReader != null)
            {
                CurrentReader.Dispose();
                CurrentReader = null;
            }

            CurrentReader = Readers[0];

            if (CurrentReader == null)
                return;

            if (!OpenReader())
                Window.Close();

            if (!StartCaptureAsync(OnCaptured))
                Window.Close();

            OnProgress = new Thread(() => InvokeDelegate(Window));
        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de manejar los eventos de Clic de la ventana.
        /// </summary>
        /// <param name="obj">Objeto que retiene el parámetro del elemento sobre el cuál se realizó el clic.</param>
        public void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                //Caso "0" - "9": Si el campo del ID tiene menos a 13 caractéres, entonces se escribe un número del 0 al 9 según el botón sobre el cuál ocurrió el evento
                //Caso "backspace": Borra 1 caractér del campo del ID
                //Caso "limpiarNIP": Borra todo el ID
                case "0":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "0";
                    }
                    break;
                case "1":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "1";
                    }
                    break;
                case "2":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "2";
                    }
                    break;
                case "3":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "3";
                    }
                    break;
                case "4":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "4";
                    }
                    break;
                case "5":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "5";
                    }
                    break;
                case "6":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "6";
                    }
                    break;
                case "7":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "7";
                    }
                    break;
                case "8":
                    if (NIPBuscar.Length < 12)
                    {
                        NIPBuscar += "8";
                    }
                    break;
                case "9":
                    if (NIPBuscar.Length < 12)
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
                case "CambiarAsistencia":
                    //Si el interno encontrado se marca asistencia, entonces...
                    try
                    {
                        //Se obtiene la ubicación actual del interno
                        var ubicacion_actual = new cIngresoUbicacion().
                            ObtenerUltimaUbicacion(
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                            (int)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_INGRESO);
                        short? nulo = null;

                        System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                        //Se revisa si el interno que tomó asistencia esta permitido para entrar por el custodio que maneja el equipo. Si esta permitido, entonces...
                        if (AsistenciaChecked)
                        {
                            var actividades_no_elegidas = new cGrupoAsistencia().GetData(g =>
                            g.GRUPO_PARTICIPANTE.ING_ID_ANIO == ubicacion_actual.ID_ANIO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == ubicacion_actual.ID_CENTRO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == ubicacion_actual.ID_IMPUTADO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == ubicacion_actual.ID_INGRESO &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == _fechaHoy.Hour &&
                            g.ID_GRUPO != SelectedAsistencia.ID_GRUPO).
                            ToList();

                            //Se marca la asistencia
                            new cGrupoAsistencia().MarcarAsistencia(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = ubicacion_actual.ID_CENTRO,
                                ID_ANIO = ubicacion_actual.ID_ANIO,
                                ID_IMPUTADO = ubicacion_actual.ID_IMPUTADO,
                                ID_INGRESO = ubicacion_actual.ID_INGRESO,
                                ID_AREA = SelectedAsistencia.GRUPO_HORARIO.ID_AREA,
                                MOVIMIENTO_FEC = ubicacion_actual.MOVIMIENTO_FEC,
                                ESTATUS = (short)EstatusUbicacion.ACTIVIDAD,
                                ACTIVIDAD = ubicacion_actual.ACTIVIDAD
                            }, new GRUPO_ASISTENCIA()
                            {
                                ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                                ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA,
                                ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                                ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                                ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                                ASISTENCIA = 1,
                                EMPALME = SelectedAsistencia.EMPALME,
                                EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                                EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                                EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                                FEC_REGISTRO = _fechaHoy,
                                ESTATUS = (short)enumGrupoAsistenciaEstatus.ACTIVO
                            }, actividades_no_elegidas);

                            //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                            var participaciones_biometricas = Huellas_Imputados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                            //Después, procedemos a borrarlas de memoria
                            foreach (var participacion_biometrica in participaciones_biometricas)
                            {
                                Huellas_Imputados.Remove(participacion_biometrica);
                            }
                        }
                        //Si no esta permitido, entonces...
                        else
                        {
                            //Se le quita la asistencia
                            new cGrupoAsistencia().QuitarAsistencia(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = ubicacion_actual.ID_CENTRO,
                                ID_ANIO = ubicacion_actual.ID_ANIO,
                                ID_IMPUTADO = ubicacion_actual.ID_IMPUTADO,
                                ID_INGRESO = ubicacion_actual.ID_INGRESO,
                                ID_CONSEC = ubicacion_actual.ID_CONSEC
                            }, new GRUPO_ASISTENCIA()
                            {
                                ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                                ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA,
                                ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                                ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                                ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                                ASISTENCIA = nulo,
                                EMPALME = SelectedAsistencia.EMPALME,
                                EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                                EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                                EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                                FEC_REGISTRO = _fechaHoy,
                                ESTATUS = SelectedAsistencia.ESTATUS
                            });

                            Huellas_Imputados.AddRange(new cGrupoAsistencia().ObtenerParticionesImputadoAreas(
                                (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                                (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                                (int)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                                _fechaHoy,
                                Areas,
                                GlobalVar.gCentro).SelectMany(s =>
                                    s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                        w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                        w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger.Value && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                    new Imputado_Huella
                                    {
                                        IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                        FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                        tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                    })
                                   .ToList());
                            CapturaIncidenciaVisible = true;


                        }
                    }
                    //Si ocurrió un error al marcar ó quitar la asistencia, entonces...
                    catch (Exception ex)
                    {
                        //Se le notifica al usuario que ocurrió un error
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar manualmente la asistencia y ubicación del imputado", ex);
                    }
                    break;
                case "onBuscarPorNIP":
                    //La imagen del interno se inicializa en la imagen por defecto
                    FotoLectura = new Imagenes().getImagenPerson();

                    //La lista a mostrar se inicializa en una lista nueva y vacía
                    ListaAsistencias = new List<GRUPO_ASISTENCIA>();

                    //Si el NIP ingresado es del tamaño requerido, entonces...
                    if (NIPBuscar.Length == 13)
                    {
                        //Se realiza la búsqueda del interno por NIP
                        try
                        {
                            //Se obtiene el interno por NIP
                            var imputado = new cIngreso().ObtenerPorNIP(NIPBuscar);

                            //Si se encontró al interno, se elige la actividad de este. Si no la actividad se vuelve nula
                            var actividad_elegida = imputado != null ? ElegirActividad(imputado) : null;



                            //Si el interno tiene una actividad elegida, se le captura la asistecia. Si no la tiene, se le niega el acceso al interno
                            enumResultadoAsistencia mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;

                            //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                            var participaciones_biometricas = Huellas_Imputados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                            //Después, procedemos a borrarlas de memoria
                            foreach (var participacion_biometrica in participaciones_biometricas)
                            {
                                Huellas_Imputados.Remove(participacion_biometrica);
                            }


                            //Se le notifica al usuario el resultado de la búsqueda del NIP
                            CambiarMensajeNIP(mensajeResultado);
                        }
                        //Si ocurrió un error al realizar la búsqueda del interno por NIP, entonces...
                        catch (Exception ex)
                        {
                            //Se le notifica al usuario que ocurrió un error
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar asistencia por NIP.", ex);
                        }
                    }


                    break;
                case "OpenCloseFlyout":
                    //Se verifica si la captura del NIP es visible. Si lo está, entonces...
                    if (CapturaNIPVisible)
                    {
                        //Se cierra la captura del NIP
                        CapturaNIPVisible = false;
                    }
                    //Si la captura del NIP no es visible, entonces...
                    else
                    {
                        //Se muestra la captura del NIP
                        CapturaNIPVisible = true;
                    }
                    break;
                case "CapturarIncidencia":
                    try
                    {
                        //Se graba la incidencia capturada junto con el motivo (Motivo precargado: FALSO POSITIVO)
                        var incidente = new cIncidente();
                        var imputado_presencial = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                        var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado_presencial.ID_CENTRO, imputado_presencial.ID_ANIO, imputado_presencial.ID_IMPUTADO).ID_INGRESO;
                        incidente.Agregar(new INCIDENTE()
                        {
                            ID_CENTRO = imputado_presencial.ID_CENTRO,
                            ID_ANIO = imputado_presencial.ID_ANIO,
                            ID_IMPUTADO = imputado_presencial.ID_IMPUTADO,
                            ID_INGRESO = ultimo_ingreso,
                            ID_INCIDENTE = incidente.ObtenerConsecutivo<short>(imputado_presencial.ID_CENTRO, imputado_presencial.ID_ANIO, imputado_presencial.ID_IMPUTADO, ultimo_ingreso),
                            ID_INCIDENTE_TIPO = (short)enumIncidenteTipo.NORMAL,
                            REGISTRO_FEC = Fechas.GetFechaDateServer,
                            MOTIVO = TextoIncidencia,
                            ESTATUS = INCIDENTE_PENDIENTE
                        });
                        CapturaIncidenciaVisible = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al capturar la incidencia.", ex); ;
                    }
                    break;
                case "CerrarIncidenciaFlyout":
                    try
                    {
                        System.DateTime _fechaHoy = Fechas.GetFechaDateServer;

                        //Se obtiene la ubicación actual del interno
                        var ubicacion_actual = new cIngresoUbicacion().
                            ObtenerUltimaUbicacion(
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                            (int)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_INGRESO);

                        var actividades_no_elegidas = new cGrupoAsistencia().GetData(g =>
                            g.GRUPO_PARTICIPANTE.ING_ID_ANIO == ubicacion_actual.ID_ANIO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == ubicacion_actual.ID_CENTRO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == ubicacion_actual.ID_IMPUTADO &&
                            g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == ubicacion_actual.ID_INGRESO &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                            g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == _fechaHoy.Hour &&
                            g.ID_GRUPO != SelectedAsistencia.ID_GRUPO).
                            ToList();


                        //Se marca la asistencia
                        new cGrupoAsistencia().MarcarAsistencia(new INGRESO_UBICACION()
                        {
                            ID_CENTRO = ubicacion_actual.ID_CENTRO,
                            ID_ANIO = ubicacion_actual.ID_ANIO,
                            ID_IMPUTADO = ubicacion_actual.ID_IMPUTADO,
                            ID_INGRESO = ubicacion_actual.ID_INGRESO,
                            ID_AREA = SelectedAsistencia.GRUPO_HORARIO.ID_AREA,
                            MOVIMIENTO_FEC = ubicacion_actual.MOVIMIENTO_FEC,
                            ESTATUS = (short)EstatusUbicacion.ACTIVIDAD,
                            ACTIVIDAD = ubicacion_actual.ACTIVIDAD
                        }, new GRUPO_ASISTENCIA()
                        {
                            ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                            ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                            ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA,
                            ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                            ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                            ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                            ASISTENCIA = 1,
                            EMPALME = SelectedAsistencia.EMPALME,
                            EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                            EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                            EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                            FEC_REGISTRO = _fechaHoy,
                            ESTATUS = (short)enumGrupoAsistenciaEstatus.ACTIVO
                        }, actividades_no_elegidas);

                        //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                        var participaciones_biometricas = Huellas_Imputados.Where(w =>
                            w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                            w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                            w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                        //Después, procedemos a borrarlas de memoria
                        foreach (var participacion_biometrica in participaciones_biometricas)
                        {
                            Huellas_Imputados.Remove(participacion_biometrica);
                        }
                    }
                    //Si ocurrió un error al realizar la búsqueda del interno por NIP, entonces...
                    catch (Exception ex)
                    {
                        //Se le notifica al usuario que ocurrió un error
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al restaurar la asistencia.", ex);
                    }
                    CapturaIncidenciaVisible = false;
                    AsistenciaChecked = true;
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
        /// Método que se encarga de realizar la accion posterior de comparación a la captura de la huella del interno.
        /// </summary>
        /// <param name="captureResult">Resultado de captura del lector.</param>
        public override void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                //Si la captura del NIP no es visible, entonces...
                if (!CapturaNIPVisible)
                {
                    //Se indica al usuario que una captura de huella ocurrió y la espera del resultado de comparación esta en proceso
                    ProgressRingVisible = true;
                    ImagenEvaluacionVisible = false;
                    base.OnCaptured(captureResult);
                    CompararHuellaImputado();
                    //Una vez terminado, se indica el resultado al usuario
                    ProgressRingVisible = false;
                    ImagenEvaluacionVisible = true;
                }
            }
            //Si ocurrió algún error en la obtención del resultado de comparación, entonces...
            catch (Exception)
            {
                //Se le notifica al usuario que la lectura de la huella fue fallida
                ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                ScannerMessage = "Lectura Fallida.";
            }

        }

        /// <summary>
        /// Método al cual se le delega la responsabilidad de cambiar el universo de huellas a comparar de acuerdo al biométrico seleccionado.
        /// </summary>
        /// <param name="obj">Objeto del cual proviene el evento "SelectionChanged" (en este caso, un ComboBox).</param>
        private void SeleccionHuella(Object obj)
        {
            //Se inicializa la lista de huellas en una lista nueva sin elementos
            Huellas_Imputados = new List<Imputado_Huella>();
            //Se obtiene el tipo de biométrico (dedo seleccionado) con el que se filtrará la obtención de las huellas
            var TIPO_BIOMETRICO = (short)((enumTipoBiometrico)obj);

            try
            {
                //Se agrega a la lista de huellas los elementos de las áreas contempladas por el equipo en la iteración
                Huellas_Imputados.AddRange(new cGrupoAsistencia().ObtenerParticipantesAreas(Fechas.GetFechaDateServer, Areas, GlobalVar.gCentro).SelectMany(s => s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == TIPO_BIOMETRICO && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                new Imputado_Huella
                                {
                                    IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                    FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                    tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                })
                        .ToList());
            }
            //Si ocurrió un error al momento de obtener las huellas, entonces...
            catch (Exception ex)
            {
                //Se le notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las huellas del dedo seleccionado.", ex);

            }
        }

        private void EnterKeyPressedID(object obj)
        {


            try
            {
                if (!string.IsNullOrEmpty(NIPBuscar))
                {
                    //Se obtiene el interno por NIP
                    var imputado = new cIngreso().ObtenerPorNIP(NIPBuscar);

                    //Si se encontró al interno, se elige la actividad de este. Si no la actividad se vuelve nula
                    var actividad_elegida = imputado != null ? ElegirActividad(imputado) : null;



                    //Si el interno tiene una actividad elegida, se le captura la asistecia. Si no la tiene, se le niega el acceso al interno
                    enumResultadoAsistencia mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;

                    //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                    var participaciones_biometricas = Huellas_Imputados.Where(w =>
                        w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                        w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                        w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                    //Después, procedemos a borrarlas de memoria
                    foreach (var participacion_biometrica in participaciones_biometricas)
                    {
                        Huellas_Imputados.Remove(participacion_biometrica);
                    }


                    //Se le notifica al usuario el resultado de la búsqueda del NIP
                    CambiarMensajeNIP(mensajeResultado);
                }
                else
                {
                    //Se notifica que no se encontró un custodio
                    CambiarMensajeNIP(enumResultadoAsistencia.INTERNO_NO_PERMITIDO);
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Métodos

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
        /// Método de comparación de la información encontrada de la huella capturada por el lector, llamado previamente desde el método sobre escrito "OnCaptured".
        /// </summary>
        public void CompararHuellaImputado()
        {
            //Se inicializa la imagen del resultado a la imagen por defecto
            FotoLectura = new Imagenes().getImagenPerson();
            //Se inicializa la lista de muestra del resultado del interno a una lista nueva sin elementos
            ListaAsistencias = new List<GRUPO_ASISTENCIA>();
            //Se inicializa el indicador del resultado de la comparación
            enumResultadoAsistencia mensajeResultado = enumResultadoAsistencia.CAPTURE_HUELLA;
            //Se obtiene la huella capturada
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            //Si no hay huella capturada, entonces...
            if (bytesHuella == null)
            {
                //Se indica que se capture de nuevo la huella
                mensajeResultado = enumResultadoAsistencia.CAPTURE_DE_NUEVO;
            }
            //Si hay una huella capturada, entonces...
            else
            {
                //Se indica que la obtención del resultado de la captura está en proceso
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    CambiarMensaje(enumResultadoAsistencia.EN_PROCESO);
                }));

                //Se obtiene el resultado de comparación
                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, Huellas_Imputados.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                //Se inicializa la lista de resultados
                var result = new List<object>();



                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
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
                else
                {
                    //Si se encontró alguna coincidencia, entonces...
                    if (doIdentify.Indexes.Count() > 0)
                    {
                        //Se itera sobre las coincidencias y se agregan a la lista de resultados
                        foreach (var resultado in doIdentify.Indexes.ToList())
                            result.Add(huellas_Imputados[resultado.FirstOrDefault()].IMPUTADO);
                    }
                }

                GRUPO_ASISTENCIA actividad_elegida = null;
                INGRESO imputado_ingreso = null;

                //Si el resultado fue mayor a 0, entonces...
                if (result.Count > 0)
                {
                    //Si el resultado fue igual a 1, entonces...
                    if (result.Count == 1)
                    {

                        try
                        {
                            //Se obtiene al interno resultante
                            var imputado = new IMPUTADO() { ID_ANIO = ((cHuellasImputado)result.FirstOrDefault()).ID_ANIO, ID_CENTRO = ((cHuellasImputado)result.FirstOrDefault()).ID_CENTRO, ID_IMPUTADO = ((cHuellasImputado)result.FirstOrDefault()).ID_IMPUTADO };
                            //Se obtiene el ingreso del interno
                            imputado_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                            //Se obtiene y elige la actividad de ese interno
                            actividad_elegida = ElegirActividad(imputado_ingreso);
                            //Si tiene una actividad elegida, entonces se captura la asistencia del interno. Si no, se indica que el interno no tiene permitido el acceso
                            mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado_ingreso) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;

                            //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                            var participaciones_biometricas = Huellas_Imputados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                            //Después, procedemos a borrarlas de memoria
                            foreach (var participacion_biometrica in participaciones_biometricas)
                            {
                                Huellas_Imputados.Remove(participacion_biometrica);
                            }
                        }
                        //Si ocurrió un error al elegir la actividad, entonces...
                        catch (Exception ex)
                        {
                            //Se le notifica al usuario que ocurrió un error
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la información del imputado.", ex);
                        }
                    }
                    //Si el resultado fue mayor a 1, entonces...
                    else
                    {
                        //Se verifica si las coincidencias son participaciones del mismo interno. Si todas las coincidencias son por participaciones del interno en las áreas contempladas del equipo, entonces...
                        if (result.Cast<cHuellasImputado>().OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_CENTRO).ThenBy(t2 => t2.ID_IMPUTADO).Select(s => s.ID_IMPUTADO).Distinct().ToList().Count == 1)
                        {
                            try
                            {
                                //Se obtiene al interno resultante
                                var imputado = result.Cast<cHuellasImputado>().FirstOrDefault();
                                //Se obtiene el ingreso del interno
                                imputado_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                                //Se obtienie y elige la actividad a la que asistirá el interno
                                actividad_elegida = ElegirActividad(imputado_ingreso);
                                //Si tiene una actividad elegida, entonces se captura la asistencia del interno. Si no, se indica que el interno no tiene permitido el acceso
                                mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado_ingreso) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;

                                //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                                var participaciones_biometricas = Huellas_Imputados.Where(w =>
                                    w.IMPUTADO.ID_CENTRO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                    w.IMPUTADO.ID_ANIO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                    w.IMPUTADO.ID_IMPUTADO == SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                                //Después, procedemos a borrarlas de memoria
                                foreach (var participacion_biometrica in participaciones_biometricas)
                                {
                                    Huellas_Imputados.Remove(participacion_biometrica);
                                }
                            }
                            //Si ocurrió un error al elegir la actividad, entonces...
                            catch (Exception ex)
                            {
                                //Se le notifica al usuario que ocurrió un error
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades del imputado.", ex);
                            }
                        }
                        //Si las coincidencias pertenecen a distintos internos, entonces...
                        else
                        {
                            //Se indica que hubo falsos positivos
                            mensajeResultado = enumResultadoAsistencia.FALSO_POSITIVO;
                        }
                    }

                }
                //Si el resultado es igual a 0, entonces...
                else
                {
                    //Se indica que el interno no tiene permitido el acceso
                    mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                }
            }
            //Se le notifica al usuario el resultado de la lectura
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                CambiarMensaje(mensajeResultado);
            }));
        }

        /// <summary>
        /// Captura la asistencia de un interno si se encuentra dentro del tiempo de tolerancia y esta autorizado para entrar a la actividad.
        /// </summary>
        /// <param name="actividad">Actividad a la que se le va a tomar asistencia al interno.</param>
        /// <param name="imputado">Ingreso del interno al que se le va a tomar la asistencia.</param>
        /// <returns></returns>
        public enumResultadoAsistencia CapturarAsistencia(GRUPO_ASISTENCIA actividad, INGRESO imputado)
        {
            //Se obtiene la tolerancia de tiempo en la que el interno todavía puede accesar a la actividad
            var tolerancia = (actividad.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - actividad.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1 ? Parametro.TOLERANCIA_ASISTENCIA_HORAS : Parametro.TOLERANCIA_ASISTENCIA_HORA;
            //Se inicializa el indicador de resultado
            var mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
            try
            {
                //Si la hora actual esta dentro de la tolerancia permitida, entonces...
                if (Fechas.GetFechaDateServer.Minute < tolerancia)
                {
                    //Se obtiene la última ubicación del interno
                    var ubicacion_interno = new cIngresoUbicacion().ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (int)imputado.ID_IMPUTADO, imputado.ID_INGRESO);
                    //Se obtiene la autorización del interno
                    var autorizado = ubicacion_interno != null ? ((/*ubicacion_interno.ESTATUS == 1 &&*/ ubicacion_interno.ID_AREA == actividad.GRUPO_HORARIO.ID_AREA) ? true : false) : false;

                    //Si la actividad no tiene marcada la asistencia, entonces...
                    if (actividad.ASISTENCIA == null)
                    {
                        //Se pregunta si el interno esta autorizado. Si esta autorizado, entonces...
                        if (autorizado)
                        {

                            //Se crea el objeto que realizará las operaciones para cambiar y justificar asistencias
                            var grupo_asistencia = new cGrupoAsistencia();

                            //Se indica que la asistencia fue capturada
                            mensajeResultado = enumResultadoAsistencia.ASISTENCIA_CAPTURADA;

                            //La actividad seleccionada se iguala a la actividad que fue elegida para marcarle la asistencia
                            SelectedAsistencia = actividad;

                            System.DateTime _fechaHoy = Fechas.GetFechaDateServer;

                            //Se obtienen las demas actividades no elegidas
                            var actividades_no_elegidas = grupo_asistencia.GetData(g =>
                                g.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.ID_ANIO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == imputado.ID_CENTRO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.ID_IMPUTADO &&
                                g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == imputado.ID_INGRESO &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                                g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == _fechaHoy.Hour &&
                                g.ID_GRUPO != SelectedAsistencia.ID_GRUPO).
                                ToList();


                            //  if (actividades_no_elegidas.Count > 0)
                            //  {
                            //      grupo_asistencia.JustificarAsistencias(actividades_no_elegidas, ubicacion_interno, SelectedAsistencia);
                            //  }
                            //  else
                            //   {
                            //Se justifican las actividades no elegidas si es que las hubo y se marca la asistencia
                            grupo_asistencia.MarcarAsistencia(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = ubicacion_interno.ID_CENTRO,
                                ID_ANIO = ubicacion_interno.ID_ANIO,
                                ID_IMPUTADO = ubicacion_interno.ID_IMPUTADO,
                                ID_INGRESO = ubicacion_interno.ID_INGRESO,
                                ID_AREA = SelectedAsistencia.GRUPO_HORARIO.ID_AREA,
                                MOVIMIENTO_FEC = _fechaHoy,
                                ESTATUS = (short)EstatusUbicacion.ACTIVIDAD,
                                ACTIVIDAD = ubicacion_interno.ACTIVIDAD
                            }, new GRUPO_ASISTENCIA()
                            {
                                ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                                ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA,
                                ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                                ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                                ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                                ASISTENCIA = 1,
                                EMPALME = SelectedAsistencia.EMPALME,
                                EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                                EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                                EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                                FEC_REGISTRO = SelectedAsistencia.FEC_REGISTRO,
                                ESTATUS = SelectedAsistencia.ESTATUS
                            }, actividades_no_elegidas);
                            // }

                            //Si se llegó hasta este punto, buscamos todas las huellas en memoria del imputado al que se le marcó asistencia
                            var participaciones_biometricas = Huellas_Imputados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == actividad.GRUPO_PARTICIPANTE.ING_ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == actividad.GRUPO_PARTICIPANTE.ING_ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == actividad.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO).ToList();

                            //Después, procedemos a borrarlas de memoria
                            foreach (var participacion_biometrica in participaciones_biometricas)
                            {
                                Huellas_Imputados.Remove(participacion_biometrica);
                            }

                            //Se obtiene la foto de seguimiento del interno. Si no existe, se deja la imagen por defecto
                            var foto_seguimiento = new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                   new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;
                            //Se obtiene la foto de registro del interno. Si no existe, se deja la imagen por defecto
                            var foto_registro = new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                                new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;
                            //Se obtiene la foto de muestra para el usuario. Se escoge primero la de seguimiento. Si esta no existe, entonces se toma la de registro, y si esta no existe, se toma la imagen por defecto para mostrar.
                            FotoLectura = foto_seguimiento == null ? (foto_registro == null ? new Imagenes().getImagenPerson() : foto_registro) : foto_seguimiento;
                            //Se indica que la asistencia fue tomada y puede ser removida por el usuario dependiendo de su criterio
                            AsistenciaChecked = true;
                        }
                        //Si el interno no esta autorizado, entonces...
                        else
                        {
                            //Se le indica que el interno no esta autorizado para entrar al área
                            mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                        }
                    }
                    //Si la actividad ya tiene marcada la asistencia, entonces...
                    else
                    {
                        //Se le indica que ya hubo una toma de asistencia previa para ese interno en esa actividad
                        mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                    }
                }
                else
                {
                    mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                }



            }
            //Si ocurrió un error al capturar la asistencia, entonces...
            catch (Exception ex)
            {
                //Se le notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al capturar la asistencia.", ex);
            }
            //Se retorna el tipo de mensaje a mostrar al usuario
            return mensajeResultado;
        }

        /// <summary>
        /// Elige la actividad entre las actividades que tiene el interno. Desempata en caso de algún empalme.
        /// </summary>
        /// <param name="imputado">Ingreso del interno al que se le va a desempatar una actividad.</param>
        /// <returns></returns>
        public GRUPO_ASISTENCIA ElegirActividad(INGRESO imputado)
        {
            //Se inicializa la actividad elegida
            GRUPO_ASISTENCIA actividad_elegida = null;
            //Se crea el objeto con el que se va a consultar a la base de datos
            var grupo_asistencia = new cGrupoAsistencia();
            try
            {
                System.DateTime _fechaHoy = Fechas.GetFechaDateServer;

                //Se obtienen las actividades del interno en la fecha actual
                var actividades = grupo_asistencia.GetData(g =>
                    g.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.ID_ANIO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == imputado.ID_CENTRO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.ID_IMPUTADO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == imputado.ID_INGRESO &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Year == _fechaHoy.Year &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Month == _fechaHoy.Month &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Day == _fechaHoy.Day &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == _fechaHoy.Hour).
                    ToList();
                //Si el interno no tiene actividades
                if (actividades.Count == 0)
                {
                    return null;
                }
                //Si el interno tiene más de una actividad
                else if (actividades.Count > 1)
                {
                    //Se obtiene la resolución de coordinación en cuanto a empalmes
                    var resolucion_coordinacion = actividades.Where(w =>
                    w.EMP_COORDINACION == 2 &&
                    w.EMP_APROBADO == 1
                    ).
                    FirstOrDefault();
                    //Se obtiene la resolución de empalme por medio de la actividad que fue registrada primero
                    var resolucion_actividad_mas_antigua = actividades.OrderBy(o => o.FEC_REGISTRO).ToList().FirstOrDefault();

                    //La actividad elegida se obtiene de modo que si no hubo una resolución, se obtiene la actividad más antígua. Si ninguno de los casos pertenece a las áreas que contempla el equipo, no hay actividad elegida
                    actividad_elegida = resolucion_coordinacion != null ? (Areas.Where(w => w.ID_AREA == resolucion_coordinacion.GRUPO_HORARIO.ID_AREA).Any() ? resolucion_coordinacion : null) :
                                                                          (Areas.Where(w => w.ID_AREA == resolucion_actividad_mas_antigua.GRUPO_HORARIO.ID_AREA).Any() ? resolucion_actividad_mas_antigua : null);
                }
                //Si el interno solo tiene una actividad
                else
                {
                    //Se obtiene la única actividad del interno
                    var actividad_sin_empalme = actividades.FirstOrDefault();
                    //Se pregunta si la actividad pertenece a alguna de las áreas contempladas por el equipo. Si no pertenece, no hay actividad elegida
                    actividad_elegida = (Areas.Where(w => w.ID_AREA == actividad_sin_empalme.GRUPO_HORARIO.ID_AREA)).Any() ? actividad_sin_empalme : null;
                }


            }
            //Si ocurrió un error al obtener la actividad elegida, entonces...
            catch (Exception ex)
            {
                //Se le notifica al usuario que ocurrió un error
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades del imputado.", ex);
            }
            return actividad_elegida;

        }
        #endregion

        #region Métodos Mensajes Interfaz
        /// <summary>
        /// Muestra el resultado de la comparación de huella del interno.
        /// </summary>
        /// <param name="TipoMensaje"></param>
        public async void CambiarMensaje(enumResultadoAsistencia TipoMensaje)
        {
            //Se crea el objeto con el que se van a obtener los recursos de las imágenes
            var obtener_imagen = new Imagenes();

            //Se evalua el caso del mensaje a mostrar
            switch (TipoMensaje)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    ScannerMessage = "NO PERMITIDO";
                    ImagenEvaluacion = obtener_imagen.getImagenDenegado();
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    break;
                case enumResultadoAsistencia.ASISTENCIA_CAPTURADA:
                    ScannerMessage = "ASISTENCIA CAPTURADA";
                    ImagenEvaluacion = obtener_imagen.getImagenPermitido();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    var nueva_asistencia = new List<GRUPO_ASISTENCIA>();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.TrimEnd();
                    nueva_asistencia.Add(SelectedAsistencia);
                    ListaAsistencias = nueva_asistencia;
                    break;
                case enumResultadoAsistencia.EN_PROCESO:
                    ScannerMessage = "Procesando...";
                    ColorAprobacion = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                    break;
                case enumResultadoAsistencia.CAPTURE_DE_NUEVO:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    ImagenEvaluacion = obtener_imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    break;
                case enumResultadoAsistencia.FALSO_POSITIVO:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    ImagenEvaluacion = obtener_imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    break;
            }
            //Se muestra el mensaje por un momento y se inicializa de nuevo el mensaje de captura de una huella nueva
            await TaskEx.Delay(1500);
            ImagenEvaluacion = obtener_imagen.getImagenHuella();
            ScannerMessage = "Capture Huella";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
        }

        /// <summary>
        /// Muestra el resultado de la comparación del NIP del interno.
        /// </summary>
        /// <param name="TipoMensaje"></param>
        public async void CambiarMensajeNIP(enumResultadoAsistencia TipoMensaje)
        {
            //Se evalua el tipo de mensaje
            switch (TipoMensaje)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "X";
                    break;
                case enumResultadoAsistencia.ASISTENCIA_CAPTURADA:
                    var nueva_asistencia = new List<GRUPO_ASISTENCIA>();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.TrimEnd();
                    nueva_asistencia.Add(SelectedAsistencia);
                    ListaAsistencias = nueva_asistencia;
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Green);
                    CheckMark = "\u2713 \u2713";
                    CapturaNIPVisible = false;
                    break;
            }
            //Se muestra el mensaje por un momento y se inicializa de nuevo el mensaje de captura de un nuevo NIP
            await TaskEx.Delay(1500);
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            NIPBuscar = "";
        }
        #endregion
    }
}
