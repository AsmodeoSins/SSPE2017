using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    public partial class IngresarSalidaCentroViewModel : FingerPrintScanner
    {
        #region Constructor
        /// <summary>
        /// Constructor del ViewModel - Inicializa los universos de huellas.
        /// </summary>
        public IngresarSalidaCentroViewModel() ///===CHECK===
        {
            //SE INDICA EL DEDO SELECCIONADO POR DEFECTO EN LA BÚSQUEDA DE HUELLAS
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;

            /*SE REALIZA LA BÚSQUEDA DE HUELLAS DE ACUERDO AL DEDO SELECCIONADO, Y AL ESTATUS DE LOS TRASLADOS Y EXCARCELACIONES (EN PROCESO) DIRECTAMENTE EN LOS INGRESOS LIGADOS A LOS TRASLADOS
             * Y EXCARCELACIONES
            */
            try
            {
                //UNIVERSO DE HUELLAS - TRASLADOS EN PROCESO
                HuellasImputadosTraslados = new cTrasladoDetalle().ObtenerTodosTraslado(GlobalVar.gCentro).Where(w =>
                    w.INGRESO.ID_UB_CENTRO.HasValue &&
                    w.INGRESO.ID_UB_CENTRO.Value == GlobalVar.gCentro &&
                    w.ID_ESTATUS == TRASLADO_EN_PROCESO //&&
                    //(w.TRASLADO.TRASLADO_FEC.Year == Fechas.GetFechaDateServer.Year &&
                    // w.TRASLADO.TRASLADO_FEC.Month == Fechas.GetFechaDateServer.Month //&&
                    // w.TRASLADO.TRASLADO_FEC.Day == Fechas.GetFechaDateServer.Day)
                   ).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                        new Imputado_Huella
                                        {
                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                        })
                                .ToList();

                //UNIVERSO DE HUELLAS - EXCARCELACIONES EN PROCESO y/o AUTORIZADAS
                System.DateTime _fechaHoy = Fechas.GetFechaDateServer;
                HuellasImputadosExcarcelaciones = new cExcarcelacion().ObtenerHuellasTodos(null, null, GlobalVar.gCentro).Where(w =>
                    (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO || w.ID_ESTATUS==EXCARCELACION_AUTORIZADA) &&
                    (w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                     w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                     w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day
                     )).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                    w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                        new Imputado_Huella
                                        {
                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                        })
                                .ToList();
                //UNIVERSO DE HUELLAS - EXCARCELACIONES ACTIVAS
                HuellasImputadosExcarcelacionesEntrada = new cExcarcelacion().ObtenerHuellasTodos(null, null, GlobalVar.gCentro).Where(w =>
           w.ID_ESTATUS == EXCARCELACION_ACTIVA).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                   w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                   w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                       new Imputado_Huella
                       {
                           IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                           FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                           tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                       })
               .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Inserta la nueva ubicación del interno cuando regresa de una excarcelación y cambia el estado de
        /// la misma a "CONCLUIDA".
        /// </summary>
        /// <param name="excarcelacion"></param>
        public void CambiarEstadoExcarcelacion(EXCARCELACION excarcelacion) ///===CHECK===
        {
            new cExcarcelacion().RetornoExcarcelacion(new INGRESO_UBICACION()
            {
                ID_CENTRO = SelectedImputadoUbicacion.ID_CENTRO,
                ID_ANIO = SelectedImputadoUbicacion.ID_ANIO,
                ID_IMPUTADO = SelectedImputadoUbicacion.ID_IMPUTADO,
                ID_INGRESO = SelectedImputadoUbicacion.ID_INGRESO,
                ID_AREA = SelectedImputadoUbicacion.ID_AREA,
                MOVIMIENTO_FEC = SelectedImputadoUbicacion.MOVIMIENTO_FEC,
                ACTIVIDAD = SelectedImputadoUbicacion.ACTIVIDAD,
                ESTATUS = SelectedImputadoUbicacion.ESTATUS
            }
                , new EXCARCELACION()
                {
                    ID_ANIO = excarcelacion.ID_ANIO,
                    ID_CENTRO = excarcelacion.ID_CENTRO,
                    ID_IMPUTADO = excarcelacion.ID_IMPUTADO,
                    ID_INGRESO = excarcelacion.ID_INGRESO,
                    ID_CONSEC = excarcelacion.ID_CONSEC,
                    ID_ESTATUS = EXCARCELACION_CONCLUIDA,
                    ID_INCIDENCIA_TRASLADO_RETORNO = CapturaIncidenciaVisible ? (short)enumIncidencias.CERTIFICADO_MEDICO_AUSENTE : (short?)null,
                    INCIDENCIA_OBSERVACION_RETORNO = CapturaIncidenciaVisible ? TextoIncidencia : null,
                    RETORNO_FEC = Fechas.GetFechaDateServer
                });
            var encontrado = HuellasImputadosExcarcelacionesEntrada.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO).FirstOrDefault();
            HuellasImputadosExcarcelacionesEntrada.Remove(encontrado);
        }

        /// <summary>
        /// Realiza una comparación sobre el universo de excarcelaciones activas, posterior a la comparación normal en caso de no encontrarse en los otros universos la huella obtenida por el lector, 
        /// y, de acuerdo al resultado, decide si permitir el acceso o no.
        /// </summary>
        /// <param name="bytesHuella"></param>
        /// <returns></returns>
        public enumMensajeResultadoComparacion CompararHuellaImputadoExcarcelaciones(byte[] bytesHuella)
        {
            var doIdentifyExcarcelacionActiva = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputadosExcarcelacionesEntrada.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);
            var resultEntrada = new List<object>();
            if (doIdentifyExcarcelacionActiva.ResultCode == Constants.ResultCode.DP_SUCCESS)
            {

                if (doIdentifyExcarcelacionActiva.Indexes.Count() > 0)
                {
                    foreach (var resultado in doIdentifyExcarcelacionActiva.Indexes.ToList())
                        resultEntrada.Add(HuellasImputadosExcarcelacionesEntrada[resultado.FirstOrDefault()].IMPUTADO);
                }

                if (resultEntrada.Count > 0)
                {

                    if (resultEntrada.Count == 1)
                    {
                        try
                        {
                            //SE OBTIENE LA INFORMACIÓN DEL IMPUTADO
                            var ingreso_ubicacion = new cIngresoUbicacion();
                            cHuellasImputado imputado = (cHuellasImputado)resultEntrada.FirstOrDefault();
                            var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                            // var ultima_ubicacion = ingreso_ubicacion.ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (int)imputado.ID_IMPUTADO, ultimo_ingreso);
                            var excarcelacion_activa = new cExcarcelacion().ObtenerImputadoExcarcelaciones(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO).Where(w =>
                                w.ID_ESTATUS == EXCARCELACION_ACTIVA).FirstOrDefault();
                            var imputado_entrante = new List<IMPUTADO>();
                            var existeMedico = new cAduana().ExisteMedico();
                            SelectedImputadoUbicacion = new INGRESO_UBICACION()
                            {
                                ID_CENTRO = imputado.ID_CENTRO,
                                ID_ANIO = imputado.ID_ANIO,
                                ID_IMPUTADO = imputado.ID_IMPUTADO,
                                ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                ID_AREA = excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO ?
                                (existeMedico ? (short)enumAreasRetorno.AREA_MEDICA_PB : (short)enumAreasRetorno.ESTANCIA) : (short)enumAreasRetorno.ESTANCIA,
                                MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                ACTIVIDAD = excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO ?
                                (existeMedico ? "ÁREA MÉDICA" : "ESTANCIA") : "ESTANCIA",
                                ESTATUS = (short)enumEstatusUbicacion.EN_TRANSITO
                            };

                            //SE AGREGA LA INFORMACIÓN RELEVANTE A LA LISTA QUE SE VA A MOSTRAR EN LA VENTANA PARA EL CUSTODIO EN EL EQUIPO
                            imputado_entrante.Add(new IMPUTADO()
                            {
                                ID_ANIO = ultimo_ingreso.ID_ANIO,
                                ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                NOMBRE = ultimo_ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                PATERNO = ultimo_ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                MATERNO = ultimo_ingreso.IMPUTADO.MATERNO.TrimEnd()
                            });
                            ImputadoEntrante = imputado_entrante;
                            //FINALMENTE SE INDICA QUE EL IMPUTADO TIENE PERMITIDO EL ACCESO AL ÁREA
                            var placeholder = new Imagenes().getImagenPerson();
                            var foto_seguimiento = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                            var foto_registro = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;

                            ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);

                            if (excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO &&
                                !existeMedico)
                            {

                                CapturaIncidenciaVisible = true;
                                //ImputadoEntrante = imputado_entrante;
                            }
                            else
                            {
                                CambiarEstadoExcarcelacion(excarcelacion_activa);

                            }
                            return enumMensajeResultadoComparacion.ENCONTRADO;

                        }
                        catch (Exception ex)
                        {

                            throw new ApplicationException(ex.Message);
                        }
                    }
                    else
                    {
                        return enumMensajeResultadoComparacion.COINCIDENCIAS;
                    }
                }
                else
                {
                    return enumMensajeResultadoComparacion.NO_ENCONTRADO;
                }
            }
            else
            {
                if (HuellasImputadosExcarcelacionesEntrada.Count == 0)
                {
                    return enumMensajeResultadoComparacion.NO_ENCONTRADO;
                }
                else
                {
                    return enumMensajeResultadoComparacion.OPERACION_LECTOR_INCORRECTA;
                }
            }
        }

        /// <summary>
        /// Realiza la comparación de la huella obtenida por el lector y de acuerdo al resultado, decide si permitir el acceso o no.
        /// </summary>
        public void CompararHuellaImputado()
        {
            //SE INICIALIZA EL MENSAJE DE RESULTADO EN "HUELLA VACIA"
            var MensajeResultado = enumMensajeResultadoComparacion.HUELLA_VACIA;

            //SE OBTIENEN EL ARREGLO DE BYTES QUE REPRESENTAN A LA HUELLA OBTENIDA POR EL LECTOR
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;

            //SI LA HUELLA ES NULA, ENTONCES...
            if (bytesHuella == null)
            {
                //SE CAMBIA EL MENSAJE DE LA VENTANA, INDICANDO QUE SEA CAPTURADA DE NUEVO
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));
            }
            //SI LA HUELLA FUE OBTENIDA CORRECTAMENTE, ENTONCES...
            else
            {
                //SE CAMBIA EL MENSAJE, INDICANDO QUE LA COMPARACIÓN ESTA EN PROCESO
                MensajeResultado = enumMensajeResultadoComparacion.PROCESANDO;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));

                //SE INICIA LA COMPARACIÓN
                try
                {
                    //COMPARAMOS EN AMBOS UNIVERSOS DE HUELLAS OBTENIDAS AL PRINCIPIO Ó EN CUALQUIER SELECCIÓN DE DEDO PREVIA
                    var doIdentifyExcarcelacion = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputadosExcarcelaciones.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                    var doIdentifyTraslado = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputadosTraslados.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);

                    //SE CREA UNA LISTA DONDE ALMACENAREMOS LOS RESULTADOS
                    var result = new List<object>();

                    //SI LA OPERACIÓN DE OBTENCIÓN DE HUELLA FUE EXITOSA EN ALGUNA DE LAS DOS COMPARACIONES, ENTONCES...
                    if ((doIdentifyExcarcelacion.ResultCode == Constants.ResultCode.DP_SUCCESS) ||
                        (doIdentifyTraslado.ResultCode == Constants.ResultCode.DP_SUCCESS))
                    {
                        //SI SE ENCONTRÓ ALGUNA COINCIDENCIA EN EL UNIVERSO DE EXCARCELACIONES, SE AGREGAN LOS RESULTADOS
                        if (HuellasImputadosExcarcelaciones.Count > 0)
                            if (doIdentifyExcarcelacion.Indexes.Count() > 0)
                            {
                                foreach (var resultado in doIdentifyExcarcelacion.Indexes.ToList())
                                    result.Add(HuellasImputadosExcarcelaciones[resultado.FirstOrDefault()].IMPUTADO);
                            }

                        //SI SE ENCONTRÓ ALGUNA COINCIDENCIA EN EL UNIVERSO DE TRASLADOS, SE AGREGAN LOS RESULTADOS
                        if (HuellasImputadosTraslados.Count > 0)
                            if (doIdentifyTraslado.Indexes.Count() > 0)
                            {
                                foreach (var resultado in doIdentifyTraslado.Indexes.ToList())
                                    result.Add(HuellasImputadosTraslados[resultado.FirstOrDefault()].IMPUTADO);
                            }

                        //SI EL RESULTADO FUE MAYOR A 0, ENTONCES...
                        if (result.Count > 0)
                        {
                            //SI SE ENCONTRÓ UN SOLO RESULTADO, ENTONCES...
                            if (result.Count == 1)
                            {
                                try
                                {
                                    //SE OBTIENE LA INFORMACIÓN DEL IMPUTADO, ASI COMO SU ÚLTIMO MOVIMIENTO
                                    var ingreso_ubicacion = new cIngresoUbicacion();
                                    cHuellasImputado imputado = (cHuellasImputado)result.FirstOrDefault();
                                    var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                                    // var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (int)imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                                    var consulta_excarcelacion = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                                        imputado.ID_CENTRO,
                                        imputado.ID_ANIO,
                                        imputado.ID_IMPUTADO,
                                        ultimo_ingreso.ID_INGRESO).Where(w =>
                                        (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO ||
                                        w.ID_ESTATUS == EXCARCELACION_AUTORIZADA) &&
                                        (w.PROGRAMADO_FEC.Value.AddHours(-Parametro.TOLERANCIA_EXC_EDIFICIO)) <= Fechas.GetFechaDateServer).FirstOrDefault();
                                    var consulta_traslado = new cTrasladoDetalle().ObtenerTraslado(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                                    var TieneSalida = ((consulta_excarcelacion != null) ||
                                                    consulta_traslado != null && consulta_traslado.TRASLADO.TRASLADO_FEC.AddHours(-Parametro.TOLERANCIA_TRASLADO_EDIFICIO) <= Fechas.GetFechaDateServer);
                                    var imputado_entrante = new List<IMPUTADO>();
                                    //SI SU ÚLTIMO MOVIMIENTO INDICA QUE EL IMPUTADO SE ENCUENTRA EN TRÁNSITO HACIA LA "SALIDA DE CENTRO", TIENE PERMITIDO EL ACCESO (ESTATUS: EN TRANSITO = 1, ID_AREA= 111- SALIDA DEL CENTRO)
                                    if (TieneSalida)
                                    {
                                        ingreso_ubicacion.Insertar(new INGRESO_UBICACION()
                                        {
                                            ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                            ID_ANIO = ultimo_ingreso.ID_ANIO,
                                            ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                            ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                            ID_AREA = SALIDA_DE_CENTRO,
                                            MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                            ACTIVIDAD = consulta_excarcelacion != null ? "EXCARCELACIÓN" : "TRASLADO",
                                            ESTATUS = (short)enumEstatusUbicacion.EN_ACTIVIDAD
                                        });

                                        //SE AGREGA LA INFORMACIÓN RELEVANTE PARA EL CUSTODIO EN EL EQUIPO, A LA LISTA QUE SE VA A MOSTRAR EN LA VENTANA
                                        imputado_entrante.Add(new IMPUTADO()
                                        {
                                            ID_ANIO = ultimo_ingreso.ID_ANIO,
                                            ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                            ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                            NOMBRE = ultimo_ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                            PATERNO = ultimo_ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                            MATERNO = ultimo_ingreso.IMPUTADO.MATERNO.TrimEnd()
                                        });
                                        ImputadoEntrante = imputado_entrante;
                                        //FINALMENTE SE INDICA QUE EL IMPUTADO TIENE PERMITIDO EL ACCESO AL ÁREA
                                        MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                                        var placeholder = new Imagenes().getImagenPerson();
                                        var foto_seguimiento = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                                        var foto_registro = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;


                                        SelectedImputadoUbicacion = new INGRESO_UBICACION()
                                        {
                                            ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                            ID_ANIO = ultimo_ingreso.ID_ANIO,
                                            ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                            ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                            ID_AREA = SALIDA_DE_CENTRO,
                                            MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                            ACTIVIDAD = consulta_excarcelacion != null ? "EXCARCELACIÓN" : "TRASLADO",
                                            ESTATUS = (short)enumEstatusUbicacion.EN_ACTIVIDAD
                                        };

                                        ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);
                                        if (HuellasImputadosExcarcelaciones.Count(c =>
                                            c.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                            c.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                            c.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO) == 1)
                                        {
                                            var encontrado = HuellasImputadosExcarcelaciones.Where(w =>
                                            w.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                            w.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                            w.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO).FirstOrDefault();
                                            HuellasImputadosExcarcelaciones.Remove(encontrado);
                                        }
                                        else
                                        {
                                            var encontrado = HuellasImputadosTraslados.Where(w =>
                                            w.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                            w.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                            w.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO).FirstOrDefault();
                                            HuellasImputadosTraslados.Remove(encontrado);
                                        }
                                    }
                                    //SI SU ÚLTIMO MOVIMIENTO NO INDICA QUE SE ENCUENTRA EN TRÁNSITO A LA "SALIDA DEL CENTRO", SE LE NIEGA EL ACCESO
                                    else
                                    {
                                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                                    }

                                }
                                catch (Exception ex)
                                {

                                    throw new ApplicationException(ex.Message);
                                }
                            }
                            //SI HUBO MÁS DE UN RESULTADO, SE INDICA QUE HUBO FALSOS POSITIVOS Y SE PROSIGUE A LA CAPTURA DE LA SIGUIENTE HUELLA
                            else
                            {

                                MensajeResultado = enumMensajeResultadoComparacion.COINCIDENCIAS;
                            }
                        }
                        /*
                         * SI EL RESULTADO NO FUE MAYOR A 0, SE INDICA QUE NO SE ENCONTRÓ NINGÚN RESULTADO. DEBIDO A QUE SE REVISO SI LA HUELLA OBTENIDA
                         * EXISTÍA EN LA LISTA DE HUELLAS DE EXCARCELACIONES EN PROCESO DEL DIA ACTUAL, SE PROSIGUE A REVISAR LAS EXCARCELACIONES ACTIVAS 
                         * EN EL MOMENTO PARA PODER PERMITIR LA ENTRADA A ALGUNO DE LOS INTERNOS QUE ESTAN FUERA RESOLVIENDO ALGUNA EXCARCELACIÓN.
                         * DE OTRO MODO, NO SE PERMITE LA ENTRADA
                         */
                        else
                        {


                            MensajeResultado = CompararHuellaImputadoExcarcelaciones(bytesHuella);

                        }
                    }
                    //SI LA OPERACIÓN NO FUE EXITOSA, ENTONCES...
                    else
                    {
                        //SI LOS UNIVERSOS DE HUELLAS DONDE SE BUSCA ESTAN  VACIOS,ENTONCES
                        if (HuellasImputadosTraslados.Count == 0 && HuellasImputadosExcarcelaciones.Count == 0)
                            //SE INDICA QUE EL IMPUTADO NO TIENE ACCESO AL ÁREA
                            MensajeResultado = CompararHuellaImputadoExcarcelaciones(bytesHuella);
                        //DE OTRO MODO, SE INDICA QUE LA OBTENCIÓN DE HUELLA FUE INCORRECTA
                        else
                            MensajeResultado = enumMensajeResultadoComparacion.OPERACION_LECTOR_INCORRECTA;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            //SE MUESTRA EL MENSAJE INDICADOR DE RESULTADO DE OPERACIÓN
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                CambiarMensaje(MensajeResultado);
            }));
        }

        /// <summary>
        /// Realiza la comparación del NIP capturado y de acuerdo al resultado, decide si permitir el acceso o no.
        /// </summary>
        public void CompararNIPImputado()
        {
            var MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
            if (!string.IsNullOrEmpty(NIPBuscar))
            {
                try
                {

                    var imputado = new cImputado().ObtenerPorNIP(NIPBuscar);
                    if (imputado != null)
                    {
                        //SE OBTIENE LA INFORMACIÓN DEL IMPUTADO, ASI COMO SU ÚLTIMO MOVIMIENTO
                        var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);

                        var consulta_excarcelacion = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                                        imputado.ID_CENTRO,
                                        imputado.ID_ANIO,
                                        imputado.ID_IMPUTADO,
                                        ultimo_ingreso.ID_INGRESO).Where(w =>
                                        (w.ID_ESTATUS == EXCARCELACION_EN_PROCESO ||
                                        w.ID_ESTATUS == EXCARCELACION_AUTORIZADA) &&
                                        (w.PROGRAMADO_FEC.Value.AddHours(-Parametro.TOLERANCIA_EXC_EDIFICIO)) <= Fechas.GetFechaDateServer).FirstOrDefault();
                        var consulta_traslado = new cTrasladoDetalle().ObtenerTraslado(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                        var TieneSalida = ((consulta_excarcelacion != null) ||
                                        consulta_traslado != null && consulta_traslado.TRASLADO.TRASLADO_FEC.AddHours(-Parametro.TOLERANCIA_TRASLADO_EDIFICIO) <= Fechas.GetFechaDateServer);
                        var imputado_entrante = new List<IMPUTADO>();
                        //SI SU ÚLTIMO MOVIMIENTO INDICA QUE EL IMPUTADO SE ENCUENTRA EN TRÁNSITO HACIA LA "SALIDA DE CENTRO", TIENE PERMITIDO EL ACCESO (ESTATUS: EN TRANSITO = 1, ID_AREA= 111- SALIDA DEL CENTRO)
                        if (ultimo_ingreso.ID_UB_CENTRO.HasValue && ultimo_ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro && TieneSalida)
                        {
                            new cIngresoUbicacion().Insertar(new INGRESO_UBICACION()
                            {
                                ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                ID_ANIO = ultimo_ingreso.ID_ANIO,
                                ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                ID_AREA = SALIDA_DE_CENTRO,
                                MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                ACTIVIDAD = consulta_excarcelacion != null ? "EXCARCELACIÓN" : "TRASLADO",
                                ESTATUS = (short)enumEstatusUbicacion.EN_ACTIVIDAD
                            });

                            //SE AGREGA LA INFORMACIÓN RELEVANTE PARA EL CUSTODIO EN EL EQUIPO, A LA LISTA QUE SE VA A MOSTRAR EN LA VENTANA
                            imputado_entrante.Add(new IMPUTADO()
                            {
                                ID_ANIO = ultimo_ingreso.ID_ANIO,
                                ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                NOMBRE = ultimo_ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                PATERNO = ultimo_ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                MATERNO = ultimo_ingreso.IMPUTADO.MATERNO.TrimEnd()
                            });
                            SelectedImputadoUbicacion = new INGRESO_UBICACION()
                            {
                                ID_CENTRO = ultimo_ingreso.ID_CENTRO,
                                ID_ANIO = ultimo_ingreso.ID_ANIO,
                                ID_IMPUTADO = ultimo_ingreso.ID_IMPUTADO,
                                ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                ID_AREA = SALIDA_DE_CENTRO,
                                MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                ACTIVIDAD = consulta_excarcelacion != null ? "EXCARCELACIÓN" : "TRASLADO",
                                ESTATUS = (short)enumEstatusUbicacion.EN_ACTIVIDAD
                            };
                            ImputadoEntrante = imputado_entrante;
                            //FINALMENTE SE INDICA QUE EL IMPUTADO TIENE PERMITIDO EL ACCESO AL ÁREA
                            MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                            var placeholder = new Imagenes().getImagenPerson();
                            var foto_seguimiento = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                            var foto_registro = ultimo_ingreso.INGRESO_BIOMETRICO != null ? ultimo_ingreso.INGRESO_BIOMETRICO.Where(w =>
                                w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;

                            ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);
                            if (HuellasImputadosExcarcelaciones.Count(c =>
                                            c.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                            c.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                            c.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO) == 1)
                            {
                                var encontrado = HuellasImputadosExcarcelaciones.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputadosExcarcelaciones.Remove(encontrado);
                            }
                            else
                            {
                                var encontrado = HuellasImputadosTraslados.Where(w =>
                                w.IMPUTADO.ID_CENTRO == SelectedImputadoUbicacion.ID_CENTRO &&
                                w.IMPUTADO.ID_ANIO == SelectedImputadoUbicacion.ID_ANIO &&
                                w.IMPUTADO.ID_IMPUTADO == SelectedImputadoUbicacion.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputadosTraslados.Remove(encontrado);
                            }
                        }
                        //SI SU ÚLTIMO MOVIMIENTO NO INDICA QUE SE ENCUENTRA EN TRÁNSITO A LA "SALIDA DEL CENTRO", SE VERIFICA SI ES UN INTERNO QUE REGRESA DE UNA EXCARCELACIÓN
                        else
                        {
                            var excarcelacion_activa = new cExcarcelacion().ObtenerImputadoExcarcelaciones(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO).Where(w =>
                                w.ID_ESTATUS == EXCARCELACION_ACTIVA).FirstOrDefault();
                            if (ultimo_ingreso.ID_UB_CENTRO.HasValue && ultimo_ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro && excarcelacion_activa != null)
                            {
                                var existeMedico = new cAduana().ExisteMedico();
                                var ingreso = imputado.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                //SE AGREGA LA INFORMACIÓN RELEVANTE A LA LISTA QUE SE VA A MOSTRAR EN LA VENTANA PARA EL CUSTODIO EN EL EQUIPO
                                imputado_entrante.Add(new IMPUTADO()
                                {
                                    ID_ANIO = ingreso.ID_ANIO,
                                    ID_CENTRO = ingreso.ID_CENTRO,
                                    ID_IMPUTADO = ingreso.ID_IMPUTADO,
                                    NOMBRE = ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                    PATERNO = ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                    MATERNO = ingreso.IMPUTADO.MATERNO.TrimEnd()
                                });

                                ImputadoEntrante = imputado_entrante;
                                SelectedImputadoUbicacion = new INGRESO_UBICACION()
                                {
                                    ID_CENTRO = imputado.ID_CENTRO,
                                    ID_ANIO = imputado.ID_ANIO,
                                    ID_IMPUTADO = imputado.ID_IMPUTADO,
                                    ID_INGRESO = ultimo_ingreso.ID_INGRESO,
                                    ID_AREA = excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO ?
                                    (existeMedico ? (short)enumAreasRetorno.AREA_MEDICA_PB : (short)enumAreasRetorno.ESTANCIA) : (short)enumAreasRetorno.ESTANCIA,
                                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                    ACTIVIDAD = excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO ?
                                    (existeMedico ? "ÁREA MÉDICA" : "ESTANCIA") : "ESTANCIA",
                                    ESTATUS = (short)enumEstatusUbicacion.EN_TRANSITO
                                };

                                //FINALMENTE SE INDICA QUE EL IMPUTADO TIENE PERMITIDO EL ACCESO AL ÁREA
                                var placeholder = new Imagenes().getImagenPerson();
                                var foto_seguimiento = ingreso.INGRESO_BIOMETRICO != null ? ingreso.INGRESO_BIOMETRICO.Where(w =>
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                                var foto_registro = ingreso.INGRESO_BIOMETRICO != null ? ingreso.INGRESO_BIOMETRICO.Where(w =>
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;

                                ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);
                                if (excarcelacion_activa.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO &&
                                !existeMedico)
                                {
                                    CapturaIncidenciaVisible = true;
                                }
                                else
                                {

                                    CambiarEstadoExcarcelacion(excarcelacion_activa);

                                }


                                MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;

                            }
                            else
                            {
                                MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                            }


                        }
                    }
                    else
                    {
                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensajeNIP(MensajeResultado);
                }));
            }
        }

        public bool LevantarIncidencia()
        {
            var incidente = new cIncidente();
            var imputado_presencial = new cImputado().ObtenerPorNIP(IncidenciaNIP);
            var ingreso = new cIngreso().ObtenerUltimoIngreso(imputado_presencial.ID_CENTRO, imputado_presencial.ID_ANIO, imputado_presencial.ID_IMPUTADO);
            if (imputado_presencial == null)
            {
                ValidacionNIPInexistente(imputado_presencial);
                return true;
            }
            else if (ingreso != null && ingreso.ID_UB_CENTRO.HasValue && ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro)
            {
                var ultimo_ingreso = ingreso.ID_INGRESO;
                incidente.Agregar(new INCIDENTE()
                {
                    ID_CENTRO = imputado_presencial.ID_CENTRO,
                    ID_ANIO = imputado_presencial.ID_ANIO,
                    ID_IMPUTADO = imputado_presencial.ID_IMPUTADO,
                    ID_INGRESO = ultimo_ingreso,
                    ID_INCIDENTE = incidente.ObtenerConsecutivo<short>(imputado_presencial.ID_CENTRO, imputado_presencial.ID_ANIO, imputado_presencial.ID_IMPUTADO, ultimo_ingreso),
                    ID_INCIDENTE_TIPO = (short)enumIncidenteTipo.NORMAL,
                    REGISTRO_FEC = Fechas.GetFechaDateServer,
                    MOTIVO = TextoIncidenciaFalsoPositivo,
                    ESTATUS = INCIDENTE_PENDIENTE
                });

                var huella = new cIngreso().ObtenerUltimoIngreso(SelectedImputadoUbicacion.ID_CENTRO, SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_IMPUTADO).
                                        IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                        wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                        wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                            new Imputado_Huella
                                            {
                                                IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                            }).FirstOrDefault();

                var retorno = new cExcarcelacion().
                ObtenerImputadoExcarcelaciones(SelectedImputadoUbicacion.ID_CENTRO, SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO).Where(w =>
                w.ID_ESTATUS == EXCARCELACION_CONCLUIDA &&
                (w.RETORNO_FEC.HasValue ? (w.RETORNO_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                w.RETORNO_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                w.RETORNO_FEC.Value.Day == Fechas.GetFechaDateServer.Day) : false)
                ).OrderByDescending(o => o.RETORNO_FEC).FirstOrDefault();


                SelectedImputadoUbicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_CENTRO, (int)SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO);
                if (retorno != null)
                {
                    var ultimo_consecutivo_imputado = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_CENTRO, (int)SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO);



                    new cExcarcelacion().RestaurarExcarcelacion(new INGRESO_UBICACION()
                    {
                        ID_CENTRO = SelectedImputadoUbicacion.ID_CENTRO,
                        ID_ANIO = SelectedImputadoUbicacion.ID_ANIO,
                        ID_IMPUTADO = SelectedImputadoUbicacion.ID_IMPUTADO,
                        ID_INGRESO = SelectedImputadoUbicacion.ID_INGRESO,
                        ID_CONSEC = ultimo_consecutivo_imputado.ID_CONSEC
                    }, new EXCARCELACION()
                    {
                        ID_CENTRO = SelectedImputadoUbicacion.ID_CENTRO,
                        ID_ANIO = SelectedImputadoUbicacion.ID_ANIO,
                        ID_IMPUTADO = SelectedImputadoUbicacion.ID_IMPUTADO,
                        ID_INGRESO = SelectedImputadoUbicacion.ID_INGRESO,
                        ID_CONSEC = retorno.ID_CONSEC,
                        ID_ESTATUS = EXCARCELACION_ACTIVA,
                        ID_INCIDENCIA_TRASLADO_RETORNO = null,
                        INCIDENCIA_OBSERVACION_RETORNO = null,
                        RETORNO_FEC = null
                    });

                    HuellasImputadosExcarcelacionesEntrada.Add(huella);
                }
                else
                {
                    new cIngresoUbicacion().Eliminar(SelectedImputadoUbicacion.ID_CENTRO, SelectedImputadoUbicacion.ID_ANIO, (short)SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO, (short)SelectedImputadoUbicacion.ID_CONSEC);

                    System.DateTime _fechaHoy = Fechas.GetFechaDateServer;

                    var excarcelacion = new cExcarcelacion().ObtenerImputadoExcarcelaciones(
                        SelectedImputadoUbicacion.ID_CENTRO,
                        SelectedImputadoUbicacion.ID_ANIO,
                        SelectedImputadoUbicacion.ID_IMPUTADO,
                        SelectedImputadoUbicacion.ID_INGRESO).Where(w =>
                        w.PROGRAMADO_FEC.Value.Year == _fechaHoy.Year &&
                        w.PROGRAMADO_FEC.Value.Month == _fechaHoy.Month &&
                        w.PROGRAMADO_FEC.Value.Day == _fechaHoy.Day &&
                        w.ID_ESTATUS == EXCARCELACION_EN_PROCESO).FirstOrDefault();

                    var traslado_detalle = new cTrasladoDetalle().Obtener(
                        SelectedImputadoUbicacion.ID_CENTRO,
                        SelectedImputadoUbicacion.ID_ANIO,
                        SelectedImputadoUbicacion.ID_IMPUTADO,
                        SelectedImputadoUbicacion.ID_INGRESO, TRASLADO_EN_PROCESO);

                    if (excarcelacion != null)
                        HuellasImputadosExcarcelaciones.Add(huella);
                    else if (traslado_detalle != null)
                        HuellasImputadosTraslados.Add(huella);
                }
                return false;
            }
            return ingreso != null && ingreso.ID_UB_CENTRO.HasValue && ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro;

        }

        public async void PermitirUbicacion()
        {
            var retorno = new cExcarcelacion().
                          ObtenerImputadoExcarcelaciones(SelectedImputadoUbicacion.ID_CENTRO, SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO).Where(w =>
                           w.ID_ESTATUS == EXCARCELACION_ACTIVA).SingleOrDefault();

            var salida = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectedImputadoUbicacion.ID_ANIO, SelectedImputadoUbicacion.ID_CENTRO, (int)SelectedImputadoUbicacion.ID_IMPUTADO, SelectedImputadoUbicacion.ID_INGRESO);
            if (salida != null && salida.ID_AREA == SALIDA_DE_CENTRO && salida.ESTATUS == (short)enumEstatusUbicacion.EN_TRANSITO)
            {
                new cIngresoUbicacion().Insertar(new INGRESO_UBICACION()
                {
                    ID_CENTRO = SelectedImputadoUbicacion.ID_CENTRO,
                    ID_ANIO = SelectedImputadoUbicacion.ID_ANIO,
                    ID_IMPUTADO = SelectedImputadoUbicacion.ID_IMPUTADO,
                    ID_INGRESO = SelectedImputadoUbicacion.ID_INGRESO,
                    ID_AREA = SelectedImputadoUbicacion.ID_AREA,
                    MOVIMIENTO_FEC = SelectedImputadoUbicacion.MOVIMIENTO_FEC,
                    ACTIVIDAD = SelectedImputadoUbicacion.ACTIVIDAD,
                    ESTATUS = (short)enumEstatusUbicacion.EN_ACTIVIDAD
                });
            }
            else if (retorno != null)
            {
                var existeMedico = new cAduana().ExisteMedico();
                if (retorno.CERTIFICADO_MEDICO == (short)enumCertificadoMedicoRequerido.REQUIERE_CERTIFICADO_MEDICO && !existeMedico)
                {
                    CapturaIncidenciaVisible = true;
                }
                else
                {
                    CambiarEstadoExcarcelacion(retorno);

                }
            }
            else
            {
                var metro = Ventana as MetroWindow;
                var mySettings = new MetroDialogSettings()
                {
                    AnimateShow = true,
                    AnimateHide = false,
                    AffirmativeButtonText = "Aceptar"
                };
                await metro.ShowMessageAsync("Validación", "No es posible continuar, no existe registro de operación definida.", MessageDialogStyle.Affirmative, mySettings);
            }
        }

        #endregion

        #region Métodos Eventos
        /// <summary>
        /// Gestiona las acciones indicadas por el botón sobre el que se da clic.
        /// * 1-10: Casos "0-9": Escribe un número en el campo de la captura del NIP
        /// 11: Caso "backspace": Elimina un caractér al final de la cadena en el campo de la captura del NIP
        /// 12: Caso "limpiarNIP": Limpia el campo de captura del NIP
        /// 13: Caso "OpenCloseFlyout": Abre la captura del NIP
        /// 14: Caso "onBuscarPorNIP": Realiza la búsqueda del imputado por NIP y le permite el acceso si es que se encuentra contemplado
        /// para un traslado ó excarcelación
        /// </summary>
        /// <param name="obj">Acción a realizar según el boón al cuál se le dé clic.</param>
        public void ClickSwitch(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
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
                case "onBuscarPorNIP":
                    if (string.IsNullOrEmpty(NIPBuscar) || NIPBuscar.Length != 13)
                    {
                        ValidacionNIP();
                    }
                    else
                    {
                        CompararNIPImputado();
                    }
                    break;
                case "CerrarIncidenciaFlyout":
                    try
                    {
                        var consulta_excarcelacion = new cExcarcelacion().
                            ObtenerImputadoExcarcelaciones(
                            SelectedImputadoUbicacion.ID_CENTRO,
                            SelectedImputadoUbicacion.ID_ANIO,
                            SelectedImputadoUbicacion.ID_IMPUTADO,
                            SelectedImputadoUbicacion.ID_INGRESO
                            ).Where(w =>
                            w.ID_ESTATUS == EXCARCELACION_ACTIVA).
                            FirstOrDefault();
                        CambiarEstadoExcarcelacion(consulta_excarcelacion);
                        CapturaIncidenciaVisible = false;
                    }
                    catch (Exception ex)
                    {

                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "PermitirUbicacion":
                    try
                    {
                        if (UbicacionPermitidaChecked)
                        {
                            PermitirUbicacion();
                        }
                        else
                        {
                            CapturaIncidenciaFalsoPositivoVisible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
                case "CerrarIncidenciaFalsoPositivoFlyout":
                    ValidacionNIP();
                    if (!base.HasErrors)
                        try
                        {
                            CapturaIncidenciaFalsoPositivoVisible = LevantarIncidencia();
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                    break;
                case "CancelarIncidenciaFalsoPositivoFlyout":
                    try
                    {
                        //PermitirUbicacion();
                        CapturaIncidenciaFalsoPositivoVisible = false;
                        UbicacionPermitidaChecked = true;
                    }
                    catch (Exception ex)
                    {

                        throw new ApplicationException(ex.Message);
                    }
                    break;

            }
        }



        /// <summary>
        /// Cambia el color de los botones de "Borrar 1 caractér" y "Borrar todo el NIP" en la captura del NIP que se encuentra sobre
        /// el Flyout de la ventana.
        /// </summary>
        /// <param name="obj">Indica el botón del cuál viene el evento "MouseOver"</param>
        private void MouseEnterSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "backspaceNIP":
                    FondoBackSpaceNIP = new SolidColorBrush(Color.FromRgb(69, 69, 69));
                    break;
                case "limpiarNIP":
                    FondoLimpiarNIP = new SolidColorBrush(Color.FromRgb(69, 69, 69));
                    break;
            }
        }



        /// <summary>
        /// Regresa el color original de los botones de "Borrar 1 caractér" y "Borrar todo el NIP" en la captura del NIP que se encuentra sobre
        /// el Flyout de la ventana, los cuales fueron modificados previamente por el método "MouseEnterSwitch" a causa del evento "MouseOver".
        /// </summary>
        /// <param name="obj">Indica el botón del cuál viene el evento "MouseLeave"</param>
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
        /// Sobre escribe el método "OnCaptured" proveniente de la clase "FingerPrintScanner". Realiza una acción de acuerdo a 
        /// la obtención de huellas digitales por medio del lector DigitalPersona. Posteriormente, realiza la comparación de 
        /// la huella obtenida contra los universos de huellas que se encuentran contemplados en los traslados y excarcelaciones 
        /// con el estatus EN PROCESO ("EP"), y, en caso de no encontrarse en estos universos, se busca en el universo de 
        /// excarcelaciones con el estatus ACTIVA ("AC").
        /// </summary>
        /// <param name="captureResult">Resultado de la captura obtenida por el lector</param>
        public override void OnCaptured(CaptureResult captureResult)
        {
            if (Readers.Any())
            {
                //SI LA CAPTURA DEL NIP NO SE ENCUENTRA EN USO O EN PANTALLA, ENTONCES...
                if (!CapturaNIPVisible)
                {
                    //SE LLAMA AL MÉTODO PADRE QUE REALIZA LA CAPTURA DE LA HUELLA
                    ImagenImputado = new Imagenes().getImagenPerson();
                    ImputadoEntrante = new List<IMPUTADO>();
                    base.OnCaptured(captureResult);
                    /*DESPUÉS, SE LLAMA AL MÉTODO QUE REALIZA LA COMPARACIÓN DE LA HUELLA CAPTURADA CON EL UNIVERSO DE HUELLAS EN LOS TRASLADOS Y LAS EXCARCELACIONES
                     * PARA DESPUÉS ARROJAR UN RESULTADO DE LA LECTURA
                     */
                    CompararHuellaImputado();
                }
            }
            else
            {
                ScannerMessageVisible = false;
            }
        }


        /// <summary>
        /// Inicializa el lector y abre el lector para dejarlo en espera de alguna huella.
        /// </summary>
        /// <param name="Window">La ventana de la cual proviene el evento</param>
        public void OnLoad(IngresarSalidaCentro Window)
        {
            //SE CREA UN OBJETO DE LA CLASE QUE OTORGA LOS RECURSOS DE IMÁGENES
            var image_retriever = new Imagenes();
            //INICIALIZACIÓN DE VARIABLES QUE AFECTAN A LA VENTANA VISUALMENTE
            ProgressRingVisible = Visibility.Collapsed;
            CampoCaptura = "NIP:";
            NIPBuscar = "";
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            ScannerMessage = "Capture huella\nen el lector";
            MensajeAprobacionNIP = "Capture NIP";
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ScannerMessageVisible = Readers.Any();
            ImagenEvaluacionVisible = true;
            ImputadoEntrante = new List<IMPUTADO>();
            TextoIncidencia = "SIN OBSERVACIONES";
            TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
            //SE OBTIENE RECURSO DE IMAGEN PARA MOSTRAR QUE EL LECTOR ESTA LISTO PARA USARSE
            ImagenEvaluacion = image_retriever.getImagenHuella();
            Ventana = Window;
            //SE CARGA LA IMAGEN POR DEFECTO AL INICIAR LA VENTANA
            ImagenImputado = new Imagenes().getImagenPerson();
            //SE INICIALIZA LA LISTA DE LA INFORMACIÓN DE LOS IMPUTADOS QUE SE MOSTRARAN A LA HORA DE TOMAR SU HUELLA
            ImputadoEntrante = new List<IMPUTADO>();

            //SE INICIALIZA Y ABRE LA CAPTURA DEL LECTOR DE HUELLAS
            Window.Closed += (s, e) =>
            {

                try
                {
                    if (OnProgress == null)
                        return;

                    if (!IsSucceed)
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
        /// Realiza una acción de acuerdo al elemento sobre el cuál cambio su selección.
        /// </summary>
        /// <param name="obj">Acción a realizar de acuerdo al control sobre el cuál se realizó un cambio del elemento seleccionado.</param>
        public void SelectionChangedSwitch(object obj)
        {
            switch (obj.ToString())
            {
                /// BUSCA Y ADAPTA EL UNIVERSO DE HUELLAS AL DEDO SELECCIONADO PARA COMPARAR EN LA LECTURA DE HUELLAS.
                case "SeleccionDedoBusqueda":
                    try
                    {
                        HuellasImputadosTraslados = new cTrasladoDetalle().ObtenerTodosTraslado(GlobalVar.gCentro).Where(w => w.ID_ESTATUS == TRASLADO_EN_PROCESO).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                new Imputado_Huella
                                                {
                                                    IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                    FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                    tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                })
                                        .ToList();


                        HuellasImputadosExcarcelaciones = new cExcarcelacion().ObtenerTodos(null, null, GlobalVar.gCentro).Where(w => w.ID_ESTATUS == EXCARCELACION_EN_PROCESO).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
                                            w.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && w.CALIDAD > 0 &&
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && w.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                new Imputado_Huella
                                                {
                                                    IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                    FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                    tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                })
                                        .ToList();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }
                    break;
            }
        }

        private void EnterKeyPressedID(object obj)
        {

            try
            {
                if (!string.IsNullOrEmpty(NIPBuscar))
                {
                    CompararNIPImputado();
                }
                else
                {
                    CambiarMensajeNIP(enumMensajeResultadoComparacion.NO_ENCONTRADO);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message); ;
            }
        }
        #endregion

        #region Métodos Mensajes Interfaz
        /// <summary>
        /// Notifica al usuario del resultado obtenido por la comparación y decisión del método de comparacion de huellas.
        /// </summary>
        /// <param name="MensajeResultado">Tipo de mensaje a indicar</param>
        public async void CambiarMensaje(enumMensajeResultadoComparacion MensajeResultado)
        {
            switch (MensajeResultado)
            {
                case enumMensajeResultadoComparacion.NO_ENCONTRADO:
                    ScannerMessage = "NO PERMITIDO";
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    ImagenEvaluacion = new Imagenes().getImagenDenegado();
                    break;
                case enumMensajeResultadoComparacion.PROCESANDO:
                    ScannerMessage = "PROCESANDO...";
                    ColorAprobacion = new SolidColorBrush(Color.FromRgb(51, 115, 242));
                    break;
                case enumMensajeResultadoComparacion.COINCIDENCIAS:
                    ScannerMessage = "CAPTURE DE NUEVO";
                    ColorAprobacion = new SolidColorBrush(Colors.Yellow);
                    ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                    break;
                case enumMensajeResultadoComparacion.ENCONTRADO:
                    ScannerMessage = "PERMITIDO";
                    ColorAprobacion = new SolidColorBrush(Colors.Green);
                    ImagenEvaluacion = new Imagenes().getImagenPermitido();
                    UbicacionPermitidaChecked = true;
                    break;
                case enumMensajeResultadoComparacion.OPERACION_LECTOR_INCORRECTA:
                    ScannerMessage = "LECTURA FALLIDA";
                    ColorAprobacion = new SolidColorBrush(Colors.DarkOrange);
                    ImagenEvaluacion = new Imagenes().getImagenAdvertencia();
                    break;
            }
            var image_retriever = new Imagenes();
            ProgressRingVisible = Visibility.Collapsed;
            ImagenEvaluacionVisible = true;
            await TaskEx.Delay(1500);
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ImagenEvaluacion = image_retriever.getImagenHuella();
            ScannerMessage = "Capture Huella\n en el lector";

            //if (!CapturaIncidenciaVisible)
            //{
            //    ImputadoEntrante = new List<IMPUTADO>();
            //    ImagenImputado = image_retriever.getImagenPerson();
            //}
        }

        /// <summary>
        /// Notifica al usuario del resultado obtenido por la comparación y decisión del método de comparacion del NIP.
        /// </summary>
        /// <param name="MensajeResultado">Tipo de mensaje a indicar</param>
        public async void CambiarMensajeNIP(enumMensajeResultadoComparacion MensajeResultado)
        {
            switch (MensajeResultado)
            {
                case enumMensajeResultadoComparacion.NO_ENCONTRADO:
                    MensajeAprobacionNIP = "NO PERMITIDO";
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    ColorAprobacion = new SolidColorBrush(Colors.Red);
                    ImagenEvaluacion = new Imagenes().getImagenDenegado();
                    CheckMark = "X";
                    break;
                case enumMensajeResultadoComparacion.ENCONTRADO:
                    CapturaNIPVisible = false;
                    ScannerMessage = "PERMITIDO";
                    ImagenEvaluacion = new Imagenes().getImagenPermitido();
                    UbicacionPermitidaChecked = true;
                    NIPBuscar = "";
                    break;
            }
            var image_retriever = new Imagenes();
            await TaskEx.Delay(2500);
            CheckMark = "🔍";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ImagenEvaluacion = image_retriever.getImagenHuella();
            MensajeAprobacionNIP = "Capture NIP";
            ScannerMessage = "Capture Huella\n en el lector";
        }
        #endregion
    }
}
