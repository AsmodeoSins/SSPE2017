using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlPenales
{
    public partial class BusquedaAsistenciaViewModel : FingerPrintScanner
    {
        # region Copyright Quadro – 2016
        //
        // Todos los derechos reservados. La reproducción o trasmisión en su
        // totalidad o en parte, en cualquier forma o medio electrónico, mecánico
        // o similar es prohibida sin autorización expresa y por escrito del
        // propietario de este código.
        //
        // Archivo: Nombre_de_archivo.cs
        //
        #endregion

        #region Constructor
        public BusquedaAsistenciaViewModel(List<Imputado_Huella> huellas, List<EQUIPO_AREA> Areas)
        {
            NIPBuscar = "";
            CampoCaptura = "NIP:";
            Huellas_Imputados = huellas;
            this.Areas = Areas;
            if (Readers.Count == 0)
            {
                AsistenciaBiometricaEnabled = false;
                AsistenciaBiometricaSelected = false;
                AsistenciaNIPSelected = true;
            }
            else
            {
                AsistenciaBiometricaEnabled = true;
            }
            ShowPropertyImage = Visibility.Collapsed;
            ShowLine = Visibility.Collapsed;
            ShowFotoLectura = Visibility.Visible;
            ProgressRingVisible = false;
            ImagenHuellaVisible = true;
        }
        #endregion

        #region Métodos Eventos
        public void OnLoad(LeerInternos Window)
        {
            var obtener_imagen = new Imagenes();
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FotoLectura = obtener_imagen.getImagenPerson();
            Imagen = obtener_imagen.getImagenHuella();
            ScannerMessage = "Capture Huella";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;

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

        public async void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {

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
                    try
                    {
                        var ubicacion_actual = new cIngresoUbicacion().
                            ObtenerUltimaUbicacion(
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                            (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_INGRESO);

                        new cIngresoUbicacion().Actualizar(new INGRESO_UBICACION()
                        {
                            ID_CENTRO = ubicacion_actual.ID_CENTRO,
                            ID_ANIO = ubicacion_actual.ID_ANIO,
                            ID_IMPUTADO = ubicacion_actual.ID_IMPUTADO,
                            ID_INGRESO = ubicacion_actual.ID_INGRESO,
                            ID_CONSEC = ubicacion_actual.ID_CONSEC,
                            ID_AREA = SelectedAsistencia.GRUPO_HORARIO.ID_AREA,
                            MOVIMIENTO_FEC = ubicacion_actual.MOVIMIENTO_FEC,
                            ESTATUS = AsistenciaChecked ? (short)EstatusUbicacion.ACTIVIDAD : (short)EstatusUbicacion.EN_TRANSITO,
                            ACTIVIDAD = ubicacion_actual.ACTIVIDAD
                        });
                        short? nulo = null;
                        new cGrupoAsistencia().Actualizar(new GRUPO_ASISTENCIA()
                        {
                            ID_CENTRO = SelectedAsistencia.ID_CENTRO,
                            ID_ACTIVIDAD = SelectedAsistencia.ID_ACTIVIDAD,
                            ID_TIPO_PROGRAMA = SelectedAsistencia.ID_TIPO_PROGRAMA,
                            ID_GRUPO = SelectedAsistencia.ID_GRUPO,
                            ID_CONSEC = SelectedAsistencia.ID_CONSEC,
                            ID_GRUPO_HORARIO = SelectedAsistencia.ID_GRUPO_HORARIO,
                            ASISTENCIA = AsistenciaChecked ? 1 : nulo,
                            EMPALME = SelectedAsistencia.EMPALME,
                            EMP_COORDINACION = SelectedAsistencia.EMP_COORDINACION,
                            EMP_APROBADO = SelectedAsistencia.EMP_APROBADO,
                            EMP_FECHA = SelectedAsistencia.EMP_FECHA,
                            FEC_REGISTRO = Fechas.GetFechaDateServer,
                            ESTATUS = SelectedAsistencia.ESTATUS
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cambiar manualmente la asistencia y ubicación del imputado", ex);
                    }
                    break;
                case "onBuscarPorNIP":
                    FotoLectura = new Imagenes().getImagenPerson();
                    ListaAsistencias = new List<GRUPO_ASISTENCIA>();
                    if (NIPBuscar.Length == 13)
                    {
                        try
                        {
                            var imputado = new cIngreso().ObtenerPorNIP(NIPBuscar);

                            var actividad_elegida = imputado != null ? ElegirActividad(imputado) : null;

                            enumResultadoAsistencia mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;

                            CambiarMensajeNIP(mensajeResultado);
                        }
                        catch (Exception ex)
                        {

                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al marcar asistencia por NIP.", ex);
                        }
                    }


                    break;
                case "OpenCloseFlyout":
                    if (CapturaNIPVisible)
                    {
                        CapturaNIPVisible = false;
                    }

                    else
                    {
                        CapturaNIPVisible = true;
                    }
                    break;
            }

        }

        /*MOUSE ENTER SWITCH - CAMBIA COLOR AL ESTILO DE LOS BOTONES DEL ESTILO SOBRE
         * LOS BOTONES DE "BORRAR UN CARACTÉR" Y "BORRAR TODO EL NIP"
        */
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

        /*MOUSE ENTER SWITCH - REGRESA EL COLOR AL ESTILO DE LOS BOTONES DEL ESTILO SOBRE
         * LOS BOTONES DE "BORRAR UN CARACTÉR" Y "BORRAR TODO EL NIP"
        */
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

        public override void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                if (AsistenciaBiometricaSelected)
                {
                    base.OnCaptured(captureResult);
                    CompararHuellaImputado();
                }
            }
            catch (Exception)
            {
                Imagen = new Imagenes().getImagenAdvertencia();
                ScannerMessage = "Lectura Fallida.";
            }

        }

        private void SeleccionHuella(Object obj)
        {
            Huellas_Imputados = new List<Imputado_Huella>();
            var biometrico = (enumTipoBiometrico)obj;

            var grupo_asistencia = new cGrupoAsistencia();

            try
            {
                foreach (var Area in Areas)
                {
                    Huellas_Imputados.AddRange(
                                    grupo_asistencia.GetData(g =>
                                    g.ESTATUS == (short)enumGrupoAsistenciaEstatus.ACTIVO &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day &&
                                    g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour &&
                                    g.GRUPO_HORARIO.ID_AREA == Area.ID_AREA).SelectMany(s => s.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                        w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                        w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                            new Imputado_Huella
                                            {
                                                IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                            })
                                    .ToList());
                }

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las huellas del dedo seleccionado.", ex);

            }
        }
        #endregion

        #region Métodos
        public void CompararHuellaImputado()
        {
            FotoLectura = new Imagenes().getImagenPerson();
            ListaAsistencias = new List<GRUPO_ASISTENCIA>();
            enumResultadoAsistencia mensajeResultado = enumResultadoAsistencia.CAPTURE_HUELLA;
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            if (bytesHuella == null)
            {
                mensajeResultado = enumResultadoAsistencia.CAPTURE_DE_NUEVO;
            }
            else
            {
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    CambiarMensaje(enumResultadoAsistencia.EN_PROCESO);
                }));

                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, huellas_Imputados.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);


                var result = new List<object>();



                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {

                }
                else
                {
                    if (doIdentify.Indexes.Count() > 0)
                    {
                        foreach (var resultado in doIdentify.Indexes.ToList())
                            result.Add(huellas_Imputados[resultado.FirstOrDefault()].IMPUTADO);
                    }
                }

                GRUPO_ASISTENCIA actividad_elegida = null;
                INGRESO imputado_ingreso = null;
                if (result.Count > 0)
                {

                    if (result.Count == 1)
                    {

                        try
                        {
                            var imputado = new IMPUTADO() { ID_ANIO = ((cHuellasImputado)result.FirstOrDefault()).ID_ANIO, ID_CENTRO = ((cHuellasImputado)result.FirstOrDefault()).ID_CENTRO, ID_IMPUTADO = ((cHuellasImputado)result.FirstOrDefault()).ID_IMPUTADO };//(result.FirstOrDefault() as Imputado_Huella).IMPUTADO;
                            imputado_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                            actividad_elegida = ElegirActividad(imputado_ingreso);
                        }
                        catch (Exception ex)
                        {

                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la información del imputado.", ex);
                        }
                    }
                    else
                    {
                        if (result.Cast<cHuellasImputado>().OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_CENTRO).ThenBy(t2 => t2.ID_IMPUTADO).Select(s => s.ID_IMPUTADO).Distinct().ToList().Count == 1)
                        {
                            try
                            {
                                var imputado = result.Cast<cHuellasImputado>().FirstOrDefault();
                                imputado_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                                actividad_elegida = ElegirActividad(imputado_ingreso);
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades del imputado.", ex);
                            }
                        }
                        else
                        {
                            mensajeResultado = enumResultadoAsistencia.FALSO_POSITIVO;
                        }
                    }

                }
                else
                {
                    mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                }
                mensajeResultado = actividad_elegida != null ? CapturarAsistencia(actividad_elegida, imputado_ingreso) : enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
            }
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                CambiarMensaje(mensajeResultado);
            }));
        }

        public enumResultadoAsistencia CapturarAsistencia(GRUPO_ASISTENCIA actividad, INGRESO imputado)
        {
            var tolerancia = (actividad.GRUPO_HORARIO.HORA_TERMINO.Value.Hour - actividad.GRUPO_HORARIO.HORA_INICIO.Value.Hour) > 1 ? Parametro.TOLERANCIA_ASISTENCIA_HORAS : Parametro.TOLERANCIA_ASISTENCIA_HORA;
            var mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
            try
            {

                if (Fechas.GetFechaDateServer.Minute < tolerancia)
                {
                var ubicacion_interno = new cIngresoUbicacion().ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (short)imputado.ID_IMPUTADO, imputado.ID_INGRESO);

                var autorizado = ubicacion_interno != null ? ((ubicacion_interno.ESTATUS == 1 && ubicacion_interno.ID_AREA == actividad.GRUPO_HORARIO.ID_AREA) ? true : false) : false;

                if (actividad.ASISTENCIA == null)
                {
                    if (autorizado)
                    {
                        mensajeResultado = enumResultadoAsistencia.ASISTENCIA_CAPTURADA;
                        SelectedAsistencia = actividad;
                        new cIngresoUbicacion().Insertar(new INGRESO_UBICACION()
                        {
                            ID_CENTRO = ubicacion_interno.ID_CENTRO,
                            ID_ANIO = ubicacion_interno.ID_ANIO,
                            ID_IMPUTADO = ubicacion_interno.ID_IMPUTADO,
                            ID_INGRESO = ubicacion_interno.ID_INGRESO,
                            ID_AREA = SelectedAsistencia.GRUPO_HORARIO.ID_AREA,
                            MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                            ESTATUS = (short)EstatusUbicacion.ACTIVIDAD,
                            ACTIVIDAD = ubicacion_interno.ACTIVIDAD
                        });

                        new cGrupoAsistencia().Actualizar(new GRUPO_ASISTENCIA()
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
                        });

                        var foto_seguimiento = new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                               new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                        var foto_registro = new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                                            new cIngresoBiometrico().Obtener((short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_ANIO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_CENTRO, (short)SelectedAsistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;

                        FotoLectura = foto_seguimiento == null ? (foto_registro == null ? new Imagenes().getImagenPerson() : foto_registro) : foto_seguimiento;
                        AsistenciaChecked = true;
                    }
                    else
                    {
                        mensajeResultado = enumResultadoAsistencia.INTERNO_NO_AUTORIZADO;
                    }
                }
                else
                {
                    mensajeResultado = enumResultadoAsistencia.ASISTENCIA_PREVIA;
                }
                }
                else
                {
                    mensajeResultado = enumResultadoAsistencia.INTERNO_NO_PERMITIDO;
                }


            }
            catch (Exception ex)
            {

                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al capturar la asistencia.", ex);
            }

            return mensajeResultado;
        }

        public GRUPO_ASISTENCIA ElegirActividad(INGRESO imputado)
        {
            GRUPO_ASISTENCIA actividad_elegida = null;
            var grupo_asistencia = new cGrupoAsistencia();
            try
            {
                var actividades = grupo_asistencia.GetData(g =>
                    g.GRUPO_PARTICIPANTE.ING_ID_ANIO == imputado.ID_ANIO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_CENTRO == imputado.ID_CENTRO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == imputado.ID_IMPUTADO &&
                    g.GRUPO_PARTICIPANTE.ING_ID_INGRESO == imputado.ID_INGRESO &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day &&
                    g.GRUPO_HORARIO.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour).
                    ToList();

                if (actividades.Count > 1)
                {
                    var resolucion_coordinacion = actividades.Where(w =>
                    w.EMP_COORDINACION == 2 &&
                    w.EMP_APROBADO == 1
                    ).
                    FirstOrDefault();

                    var resolucion_actividad_mas_antigua = actividades.OrderBy(o => o.FEC_REGISTRO).ToList().FirstOrDefault();


                    actividad_elegida = resolucion_coordinacion != null ? (Areas.Where(w => w.ID_AREA == resolucion_coordinacion.GRUPO_HORARIO.ID_AREA).Any() ? resolucion_coordinacion : null) :
                                                                          (Areas.Where(w => w.ID_AREA == resolucion_actividad_mas_antigua.GRUPO_HORARIO.ID_AREA).Any() ? resolucion_actividad_mas_antigua : null);
                }
                else
                {
                    var actividad_sin_empalme = actividades.FirstOrDefault();
                    actividad_elegida = (Areas.Where(w => w.ID_AREA == actividad_sin_empalme.GRUPO_HORARIO.ID_AREA)).Any() ? actividad_sin_empalme : null;
                }


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades del imputado.", ex);
            }
            return actividad_elegida;

        }
        #endregion

        #region Métodos Mensajes Interfaz
        public async void CambiarMensaje(enumResultadoAsistencia TipoMensaje)
        {
            var obtener_imagen = new Imagenes();
            ProgressRingVisible = false;
            ImagenHuellaVisible = true;
            switch (TipoMensaje)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    ScannerMessage = "NO PERMITIDO";
                    Imagen = obtener_imagen.getImagenDenegado();
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    break;
                case enumResultadoAsistencia.INTERNO_NO_AUTORIZADO:
                    ScannerMessage = "NO AUTORIZADO";
                    Imagen = obtener_imagen.getImagenDenegado();
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    break;
                case enumResultadoAsistencia.ASISTENCIA_CAPTURADA:
                    ScannerMessage = "ASISTENCIA CAPTURADA";
                    Imagen = obtener_imagen.getImagenPermitido();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    var nueva_asistencia = new List<GRUPO_ASISTENCIA>();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.NOMBRE.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.PATERNO.TrimEnd();
                    SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO = SelectedAsistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO.MATERNO.TrimEnd();
                    nueva_asistencia.Add(SelectedAsistencia);
                    ListaAsistencias = nueva_asistencia;
                    break;
                case enumResultadoAsistencia.ASISTENCIA_PREVIA:
                    ScannerMessage = "ASISTENCIA PREVIA";
                    Imagen = obtener_imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    break;
                case enumResultadoAsistencia.EN_PROCESO:
                    ScannerMessage = "Procesando...";
                    ColorAprobacion = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                    break;
                case enumResultadoAsistencia.CAPTURE_DE_NUEVO:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    Imagen = obtener_imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    break;
                case enumResultadoAsistencia.FALSO_POSITIVO:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    Imagen = obtener_imagen.getImagenAdvertencia();
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    break;
            }

            await TaskEx.Delay(1500);
            Imagen = obtener_imagen.getImagenHuella();
            ScannerMessage = "Capture Huella";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
        }

        public async void CambiarMensajeNIP(enumResultadoAsistencia TipoMensaje)
        {
            switch (TipoMensaje)
            {
                case enumResultadoAsistencia.INTERNO_NO_PERMITIDO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "X";
                    break;
                case enumResultadoAsistencia.INTERNO_NO_AUTORIZADO:
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
                    break;
                case enumResultadoAsistencia.ASISTENCIA_PREVIA:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.DarkOrange);
                    CheckMark = "!";
                    break;
            }

            await TaskEx.Delay(1500);
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
        }
        #endregion

    }
}
