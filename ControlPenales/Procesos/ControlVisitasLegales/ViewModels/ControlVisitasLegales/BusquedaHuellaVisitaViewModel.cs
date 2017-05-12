using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales
{
    public partial class BusquedaHuellaVisitaViewModel : FingerPrintScanner
    {
        #region Constructor
        public BusquedaHuellaVisitaViewModel() { }
        #endregion

        #region Métodos
        public List<Imputado_Huella> ObtenerHuellasImputados()
        {
            try
            {
                return new cAduanaIngreso().GetData(g =>
                    g.INGRESO.ID_UB_CENTRO.HasValue &&
                    g.INGRESO.ID_UB_CENTRO.Value == GlobalVar.gCentro &&
                    g.ADUANA.ID_TIPO_PERSONA == ABOGADO &&
                    g.ENTRADA_FEC.Value.Year == Fechas.GetFechaDateServer.Year &&
                    g.ENTRADA_FEC.Value.Month == Fechas.GetFechaDateServer.Month &&
                    g.ENTRADA_FEC.Value.Day == Fechas.GetFechaDateServer.Day).SelectMany(s => s.INGRESO.IMPUTADO.IMPUTADO_BIOMETRICO).Where(w =>
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

        public void CompararHuellaImputado()
        {
            ImagenImputado = new Imagenes().getImagenPerson();
            ImputadoEntrante = new List<InternoVisitaLegal>();
            var bytesHuella = FingerPrintData != null ? FeatureExtraction.CreateFmdFromFid(FingerPrintData, Constants.Formats.Fmd.ANSI).Data.Bytes : null;
            var MensajeResultado = enumMensajeResultadoComparacion.HUELLA_VACIA;
            if (bytesHuella == null)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate()
                {
                    CambiarMensaje(MensajeResultado);
                }));
            }
            else
            {
                MensajeResultado = enumMensajeResultadoComparacion.PROCESANDO;
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    CambiarMensaje(MensajeResultado);
                }));

                var doIdentify = Comparison.Identify(Importer.ImportFmd(bytesHuella, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data, 0, HuellasImputadosVisitas.Where(w => w.FMD != null && w.tipo_biometrico == SelectedFinger).Select(s => s.FMD), (0x7fffffff / 100000), 10);
                var result = new List<object>();

                if (doIdentify.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    if (HuellasImputadosVisitas.Count == 0)
                    {
                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                    }
                    else
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
                }
                else
                {
                    if (doIdentify.Indexes.Count() > 0)
                    {
                        foreach (var resultado in doIdentify.Indexes.ToList())
                        {
                            result.Add(HuellasImputadosVisitas[resultado.FirstOrDefault()].IMPUTADO);
                        }
                    }
                    if (result.Count > 0)
                    {
                        if (result.Count == 1)
                        {
                            try
                            {
                                var imputado = ((cHuellasImputado)result.FirstOrDefault());
                                var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                                var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (int)imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                                var LOCUTORIOS = Parametro.UBICACION_VISITA_ACTUARIO;
                                var SALA_ABOGADOS = Parametro.UBICACION_VISITA_ABOGADO;
                                short SIN_AREA = 0;
                                if (ultima_ubicacion != null)
                                {
                                    //if (ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_TRANSITO && (ultima_ubicacion.ID_AREA == LOCUTORIOS || ultima_ubicacion.ID_AREA == SALA_ABOGADOS))
                                    //{
                                    var aduana_ingreso = new cAduanaIngreso();
                                    var consulta_aduana_ingreso = aduana_ingreso.ObtenerAduanaIngresoSinNotificacion(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO, Fechas.GetFechaDateServer).FirstOrDefault();
                                    if (consulta_aduana_ingreso != null)
                                    {


                                        new cAduanaIngreso().CambiarEstadoVisitaInterno(new INGRESO_UBICACION()
                                        {
                                            ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                                            ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                                            ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                                            ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                                            ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO),
                                            ID_AREA = consulta_aduana_ingreso.ADUANA.ID_AREA != null ? (consulta_aduana_ingreso.ADUANA.ID_AREA == SALA_ABOGADOS ? SALA_ABOGADOS : LOCUTORIOS) : SIN_AREA,//ultima_ubicacion.ID_AREA == LOCUTORIOS ? LOCUTORIOS : SALA_ABOGADOS,
                                            MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                            ACTIVIDAD = VISITA_LEGAL,
                                            ESTATUS = (short)enumUbicacion.ACTIVIDAD
                                        }, new ADUANA_INGRESO()
                                        {
                                            ID_ADUANA = consulta_aduana_ingreso.ID_ADUANA,
                                            ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                                            ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                                            ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                                            ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                                            INTERNO_NOTIFICADO = INTERNO_NOTIFICADO
                                        });
                                        var imputado_entrante = new List<InternoVisitaLegal>();
                                        imputado_entrante.Add(new InternoVisitaLegal()
                                        {
                                            ID_CENTRO = imputado.ID_CENTRO,
                                            ID_ANIO = imputado.ID_ANIO,
                                            ID_IMPUTADO = (short)imputado.ID_IMPUTADO,
                                            PATERNO = ultimo_ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                            MATERNO = ultimo_ingreso.IMPUTADO.MATERNO.TrimEnd(),
                                            NOMBRE = ultimo_ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                            PERMITIR = true,
                                            HABILITAR = true
                                        });
                                        ImputadoEntrante = imputado_entrante;
                                        SelectedImputado = ImputadoEntrante.FirstOrDefault();
                                        var huella = HuellasImputadosVisitas.Where(w =>
                                                    w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                    w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                    w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                                        HuellasImputadosVisitas.Remove(huella);
                                        var placeholder = new Imagenes().getImagenPerson();
                                        var foto_seguimiento = consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO != null ? consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                                        var foto_registro = consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO != null ? consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                                            w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;

                                        ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);
                                        MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                                    }
                                    else
                                    {
                                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                                    }
                                    //}
                                    //else
                                    //{
                                    //    MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                                    //}

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




                        }
                        else
                        {
                            MensajeResultado = enumMensajeResultadoComparacion.COINCIDENCIAS;
                        }
                    }
                    else
                    {
                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                    }

                }

            }
            Application.Current.Dispatcher.Invoke((Action)(delegate()
            {
                CambiarMensaje(MensajeResultado);
            }));
        }

        public void RevertirUltimoMovimiento()
        {
            var aduana_ingreso = new cAduanaIngreso();
            var ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
            var consulta_aduana_ingreso = aduana_ingreso.ObtenerAduanaIngresoConNotificacion(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO, Fechas.GetFechaDateServer).FirstOrDefault();
            var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
            var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
            var LOCUTORIOS = Parametro.UBICACION_VISITA_ACTUARIO;
            new cAduanaIngreso().CambiarEstadoVisitaInterno(new INGRESO_UBICACION()
            {
                ID_CENTRO = ingreso.ID_CENTRO,
                ID_ANIO = ingreso.ID_ANIO,
                ID_IMPUTADO = ingreso.ID_IMPUTADO,
                ID_INGRESO = ingreso.ID_INGRESO,
                ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO),
                ID_AREA = LOCUTORIOS,
                MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                ACTIVIDAD = VISITA_LEGAL,
                ESTATUS = (short)enumUbicacion.EN_TRANSITO
            }, new ADUANA_INGRESO()
            {
                ID_ADUANA = consulta_aduana_ingreso.ID_ADUANA,
                ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                INTERNO_NOTIFICADO = INTERNO_NO_NOTIFICADO
            }, new INCIDENTE()
            {
                ID_CENTRO = imputado_incidente.ID_CENTRO,
                ID_ANIO = imputado_incidente.ID_ANIO,
                ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                ID_INGRESO = ingreso_incidente,
                ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                REGISTRO_FEC = Fechas.GetFechaDateServer,
                MOTIVO = TextoIncidenciaFalsoPositivo
            });
        }

        public void RevertirVisitaLegal()
        {
            var aduana_ingreso = new cAduanaIngreso();
            var ingreso = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO);
            var consulta_aduana_ingreso = aduana_ingreso.ObtenerAduanaIngresoConNotificacion(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO, Fechas.GetFechaDateServer).FirstOrDefault();
            var imputado_incidente = new cImputado().ObtenerPorNIP(IncidenciaNIP);
            var ingreso_incidente = imputado_incidente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_INGRESO;
            new cAduanaIngreso().CambiarEstadoVisitaInterno(new INGRESO_UBICACION()
            {
                ID_CENTRO = ingreso.ID_CENTRO,
                ID_ANIO = ingreso.ID_ANIO,
                ID_IMPUTADO = ingreso.ID_IMPUTADO,
                ID_INGRESO = ingreso.ID_INGRESO,
                ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(ingreso.ID_CENTRO, ingreso.ID_ANIO, ingreso.ID_IMPUTADO, ingreso.ID_INGRESO),
                ID_AREA = ESTANCIA,
                MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                ACTIVIDAD = "ESTANCIA",
                ESTATUS = (short)enumUbicacion.ESTANCIA
            }, new ADUANA_INGRESO()
            {
                ID_ADUANA = consulta_aduana_ingreso.ID_ADUANA,
                ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                INTERNO_NOTIFICADO = INTERNO_NO_NOTIFICADO
            }, new INCIDENTE()
            {
                ID_CENTRO = imputado_incidente.ID_CENTRO,
                ID_ANIO = imputado_incidente.ID_ANIO,
                ID_IMPUTADO = imputado_incidente.ID_IMPUTADO,
                ID_INGRESO = ingreso_incidente,
                ID_INCIDENTE = new cIncidente().ObtenerConsecutivo<short>(imputado_incidente.ID_CENTRO, imputado_incidente.ID_ANIO, imputado_incidente.ID_IMPUTADO, ingreso_incidente),
                ID_INCIDENTE_TIPO = (short)enumIncidente.NORMAL,
                REGISTRO_FEC = Fechas.GetFechaDateServer,
                MOTIVO = TextoIncidenciaFalsoPositivo
            });
        }
        #endregion

        #region Métodos Eventos
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

        public void ClickSwitch(Object obj)
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
                case "onBuscarPorNIP":
                    var imputado = new cImputado().ObtenerPorNIP(NIPBuscar);
                    var MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                    var LOCUTORIOS = Parametro.UBICACION_VISITA_ACTUARIO;
                    var SALA_ABOGADOS = Parametro.UBICACION_VISITA_ABOGADO;
                    short SIN_AREA = 0;
                    if (imputado != null)
                        try
                        {
                            var ultimo_ingreso = new cIngreso().ObtenerUltimoIngreso(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO);
                            var ultima_ubicacion = new cIngresoUbicacion().ObtenerUltimaUbicacion(imputado.ID_ANIO, imputado.ID_CENTRO, (int)imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO);
                            //if (ultima_ubicacion.ESTATUS == (short)enumUbicacion.EN_TRANSITO && (ultima_ubicacion.ID_AREA == LOCUTORIOS || ultima_ubicacion.ID_AREA == SALA_ABOGADOS))
                            //{
                            var aduana_ingreso = new cAduanaIngreso();
                            var consulta_aduana_ingreso = aduana_ingreso.ObtenerAduanaIngresoSinNotificacion(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO, Fechas.GetFechaDateServer).FirstOrDefault();
                            if (consulta_aduana_ingreso != null && ultimo_ingreso.ID_UB_CENTRO.HasValue && ultimo_ingreso.ID_UB_CENTRO.Value == GlobalVar.gCentro)
                            {

                                new cAduanaIngreso().CambiarEstadoVisitaInterno(new INGRESO_UBICACION()
                                {
                                    ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                                    ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                                    ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                                    ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                                    ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(imputado.ID_CENTRO, imputado.ID_ANIO, imputado.ID_IMPUTADO, ultimo_ingreso.ID_INGRESO),
                                    ID_AREA = consulta_aduana_ingreso.ADUANA.ID_AREA != null ? (consulta_aduana_ingreso.ADUANA.ID_AREA == SALA_ABOGADOS ? SALA_ABOGADOS : LOCUTORIOS) : SIN_AREA,//ultima_ubicacion.ID_AREA == LOCUTORIOS ? LOCUTORIOS : SALA_ABOGADOS,
                                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer,
                                    ACTIVIDAD = VISITA_LEGAL,
                                    ESTATUS = (short)enumUbicacion.ACTIVIDAD
                                }, new ADUANA_INGRESO()
                                {
                                    ID_ADUANA = consulta_aduana_ingreso.ID_ADUANA,
                                    ID_CENTRO = consulta_aduana_ingreso.ID_CENTRO,
                                    ID_ANIO = consulta_aduana_ingreso.ID_ANIO,
                                    ID_IMPUTADO = consulta_aduana_ingreso.ID_IMPUTADO,
                                    ID_INGRESO = consulta_aduana_ingreso.ID_INGRESO,
                                    INTERNO_NOTIFICADO = INTERNO_NOTIFICADO
                                });

                                var imputado_entrante = new List<InternoVisitaLegal>();
                                imputado_entrante.Add(new InternoVisitaLegal()
                                {
                                    ID_CENTRO = imputado.ID_CENTRO,
                                    ID_ANIO = imputado.ID_ANIO,
                                    ID_IMPUTADO = (short)imputado.ID_IMPUTADO,
                                    PATERNO = ultimo_ingreso.IMPUTADO.PATERNO.TrimEnd(),
                                    MATERNO = ultimo_ingreso.IMPUTADO.MATERNO.TrimEnd(),
                                    NOMBRE = ultimo_ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                                    PERMITIR = true,
                                    HABILITAR = true
                                });
                                ImputadoEntrante = imputado_entrante;
                                SelectedImputado = ImputadoEntrante.FirstOrDefault();
                                var huella = HuellasImputadosVisitas.Where(w =>
                                                w.IMPUTADO.ID_CENTRO == SelectedImputado.ID_CENTRO &&
                                                w.IMPUTADO.ID_ANIO == SelectedImputado.ID_ANIO &&
                                                w.IMPUTADO.ID_IMPUTADO == SelectedImputado.ID_IMPUTADO).FirstOrDefault();
                                HuellasImputadosVisitas.Remove(huella);
                                var placeholder = new Imagenes().getImagenPerson();
                                var foto_seguimiento = consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO != null ? consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault() : null;
                                var foto_registro = consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO != null ? consulta_aduana_ingreso.INGRESO.INGRESO_BIOMETRICO.Where(w =>
                                    w.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault() : null;

                                ImagenImputado = foto_seguimiento != null ? foto_seguimiento.BIOMETRICO : (foto_registro != null ? foto_registro.BIOMETRICO : placeholder);
                                MensajeResultado = enumMensajeResultadoComparacion.ENCONTRADO;
                            }
                            else
                            {
                                MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                            }
                            //}
                            //else
                            //{
                            //    MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
                            //}
                            CambiarMensajeNIP(MensajeResultado);
                        }
                        catch (Exception ex)
                        {

                            throw new ApplicationException(ex.Message);
                        }
                    else
                        MensajeResultado = enumMensajeResultadoComparacion.NO_ENCONTRADO;
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
                case "PermitirEntradaVisita":
                    if (!SelectedImputado.PERMITIR)
                    {
                        IncidenciaRevertirVisitaLegalVisible = true;
                    }
                    break;
                case "CapturarIncidenciaRevertirUltimoMovimiento":
                    ValidacionNIP();
                    if (!base.HasErrors)
                    {
                        var imputado_validacion = new cImputado().ObtenerPorNIP(IncidenciaNIP);
                        ValidacionNIPInexistente(imputado_validacion);
                        if (!base.HasErrors)
                        {
                            ValidacionImputadoActivo(imputado_validacion);
                            if (!base.HasErrors)
                            {
                                try
                                {
                                    RevertirUltimoMovimiento();
                                    var huella = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO).
                                                                    IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                                                    wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                                                    wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                                        new Imputado_Huella
                                                                        {
                                                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                                        }).FirstOrDefault();
                                    if (huella != null)
                                        HuellasImputadosVisitas.Add(huella);
                                    var imputado_entrante_revertir_ultimo_movimiento = new List<InternoVisitaLegal>();
                                    imputado_entrante_revertir_ultimo_movimiento.Add(new InternoVisitaLegal()
                                    {
                                        ID_CENTRO = SelectedImputado.ID_CENTRO,
                                        ID_ANIO = SelectedImputado.ID_ANIO,
                                        ID_IMPUTADO = SelectedImputado.ID_IMPUTADO,
                                        PATERNO = SelectedImputado.NOMBRE,
                                        MATERNO = SelectedImputado.PATERNO,
                                        NOMBRE = SelectedImputado.MATERNO,
                                        PERMITIR = false,
                                        HABILITAR = false
                                    });
                                    TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
                                    ImputadoEntrante = imputado_entrante_revertir_ultimo_movimiento;
                                    SelectedImputado = ImputadoEntrante.FirstOrDefault();
                                    IncidenciaRevertirVisitaLegalVisible = false;
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message);
                                }
                            }
                        }
                    }


                    break;
                case "CapturarIncidenciaRevertirVisitaLegal":
                    ValidacionNIP();
                    if (!base.HasErrors)
                    {
                        var imputado_validacion = new cImputado().ObtenerPorNIP(incidenciaNIP);
                        ValidacionNIPInexistente(imputado_validacion);
                        if (!base.HasErrors)
                        {
                            ValidacionImputadoActivo(imputado_validacion);
                            if (!base.HasErrors)
                            {
                                try
                                {
                                    RevertirVisitaLegal();
                                    var huella = new cIngreso().ObtenerUltimoIngreso(SelectedImputado.ID_CENTRO, SelectedImputado.ID_ANIO, SelectedImputado.ID_IMPUTADO).
                                                                    IMPUTADO.IMPUTADO_BIOMETRICO.Where(wB =>
                                                                    wB.ID_FORMATO == (short)enumTipoFormato.FMTO_DP && wB.CALIDAD > 0 &&
                                                                    wB.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO == (short)SelectedFinger && wB.BIOMETRICO != null).AsEnumerable().Select(s =>
                                                                        new Imputado_Huella
                                                                        {
                                                                            IMPUTADO = new cHuellasImputado { ID_ANIO = s.ID_ANIO, ID_CENTRO = s.ID_CENTRO, ID_IMPUTADO = s.ID_IMPUTADO },
                                                                            FMD = Importer.ImportFmd(s.BIOMETRICO, Constants.Formats.Fmd.ANSI, Constants.Formats.Fmd.ANSI).Data,
                                                                            tipo_biometrico = (enumTipoBiometrico)s.BIOMETRICO_TIPO.ID_TIPO_BIOMETRICO
                                                                        }).FirstOrDefault();
                                    if (huella != null)
                                        HuellasImputadosVisitas.Add(huella);
                                    TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
                                    ImputadoEntrante = new List<InternoVisitaLegal>();
                                    ImagenImputado = new Imagenes().getImagenPerson();
                                    IncidenciaRevertirVisitaLegalVisible = false;
                                }
                                catch (Exception ex)
                                {

                                    throw new ApplicationException(ex.Message);
                                }
                            }

                        }
                    }

                    break;
                case "CancelarReversion":
                    IncidenciaRevertirVisitaLegalVisible = false;
                    break;
            }

        }

        public override void OnCaptured(CaptureResult captureResult)
        {
            ImagenEvaluacionVisible = false;
            ProgressRingVisible = Visibility.Visible;
            base.OnCaptured(captureResult);
            CompararHuellaImputado();
            ImagenEvaluacionVisible = true;
            ProgressRingVisible = Visibility.Collapsed;
        }

        public void OnLoad(BusquedaHuellaVisita Window)
        {
            SelectedFinger = enumTipoBiometrico.INDICE_DERECHO;
            HuellasImputadosVisitas = ObtenerHuellasImputados();
            FondoBackSpaceNIP = new SolidColorBrush(Colors.Green);
            FondoLimpiarNIP = new SolidColorBrush(Colors.Crimson);
            NIPBuscar = "";
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            CheckMark = "🔍";
            ScannerMessage = "Capture Huella\nen el lector";
            var placeholder = new Imagenes().getImagenPerson();
            ColorAprobacion = new SolidColorBrush(Colors.Green);
            ProgressRingVisible = Visibility.Collapsed;
            ImagenEvaluacion = new Imagenes().getImagenHuella();
            ScannerMessageVisible = true;
            ImagenEvaluacionVisible = Readers.Count > 0;
            TextoIncidenciaFalsoPositivo = "SIN OBSERVACIONES";
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
            //UbicacionPermitidaChecked = true;
            //if (!CapturaIncidenciaVisible)
            //{
            //    ImputadoEntrante = new List<IMPUTADO>();
            //    ImagenImputado = image_retriever.getImagenPerson();
            //}
        }


        public async void CambiarMensajeNIP(enumMensajeResultadoComparacion MensajeResultado)
        {
            switch (MensajeResultado)
            {
                case enumMensajeResultadoComparacion.NO_ENCONTRADO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "X";
                    break;

                case enumMensajeResultadoComparacion.ENCONTRADO:
                    ColorAprobacionNIP = new SolidColorBrush(Colors.Red);
                    CheckMark = "\u2713 \u2713";
                    CapturaNIPVisible = false;
                    break;
            }
            await TaskEx.Delay(1500);
            ColorAprobacionNIP = new SolidColorBrush(Colors.DarkBlue);
            CheckMark = "🔍";
        }
        #endregion
    }
}
