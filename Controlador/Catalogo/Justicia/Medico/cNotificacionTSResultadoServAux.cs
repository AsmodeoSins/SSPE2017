using System.Linq;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNotificacionTSResultadoServAuxSSP : SSP.Modelo.EntityManagerServer<SSP.Servidor.NOTIFICACION_TS>
    {
        public cNotificacionTSResultadoServAuxSSP() { }

        public enum eTiposNotificaciones
        {
            CANALIZA_TRABAJO_SOCIAL = 1,
            RESPUESTA_TRABAJO_SOCIAL = 2
        };

        public bool GuardaNotificacionMedicaTransaccion(SSP.Servidor.NOTIFICACION_TS Entity, short TipoMensaje)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ConsecutivoNotificacionTS = GetIDProceso<decimal>("NOTIFICACION_TS", "ID_NOTIFICACION_TS", string.Format("ID_CENTRO_UBI={0}", Entity.ID_CENTRO_UBI));
                    var NuevaNotificacion = new SSP.Servidor.NOTIFICACION_TS()
                    {
                        ID_ANIO = Entity.ID_ANIO,
                        ID_CANALIZACION_TS = Entity.ID_CANALIZACION_TS,
                        ID_CARACTER = Entity.ID_CARACTER,
                        ID_CENTRO = Entity.ID_CENTRO,
                        ID_DIAGNOSTICO = Entity.ID_DIAGNOSTICO,
                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                        ID_NOTIFICACION_TS = ConsecutivoNotificacionTS,//GetSequence<decimal>("NOTIFICACION_TS_SEQ"),
                        ID_RIESGOS = Entity.ID_RIESGOS,
                        ID_USUARIO = Entity.ID_USUARIO,
                        MENSAJE = Entity.MENSAJE,
                        OTROS_RIESGOS = Entity.OTROS_RIESGOS,
                        REGISTRO_FEC = Entity.REGISTRO_FEC,
                        ID_INGRESO = Entity.ID_INGRESO,
                        FOLIO_NOTIF = GetSequence<int>("NOTIFICACION_TS_FOLIO_SEQ"),
                        ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                        ID_NOTIFICAION_TIPO = (decimal)eTiposNotificaciones.CANALIZA_TRABAJO_SOCIAL
                    };

                    if (Entity.DIAGNOSTICO_MEDICO_NOTIFICA != null)
                        if (Entity.DIAGNOSTICO_MEDICO_NOTIFICA.Count > 0)
                            foreach (var item in Entity.DIAGNOSTICO_MEDICO_NOTIFICA)
                                NuevaNotificacion.DIAGNOSTICO_MEDICO_NOTIFICA.Add(new SSP.Servidor.DIAGNOSTICO_MEDICO_NOTIFICA
                                {
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                    ID_NOTIFICACION_TS = NuevaNotificacion.ID_NOTIFICACION_TS,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI
                                });

                    System.DateTime _fecha = GetFechaServerDate();
                    var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                    var _detallesImputado = Context.INGRESO.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_INGRESO == Entity.ID_INGRESO);
                    var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                    if (_MensajeConsiderado != null)
                        if (_id_mensaje != decimal.Zero)
                        {
                            var NvoMensaje = new SSP.Servidor.MENSAJE()
                            {
                                CONTENIDO = string.Format("{0} CON RESPECTO AL IMPUTADO: \n - {1} {2} {3}.",
                                    _MensajeConsiderado.CONTENIDO,
                                    _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.NOMBRE) ? _detallesImputado.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                    _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.PATERNO) ? _detallesImputado.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.MATERNO) ? _detallesImputado.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = _fecha,
                                ID_CENTRO = SSP.Servidor.GlobalVariables.gCentro
                            };

                            Context.MENSAJE.Add(NvoMensaje);
                        };

                    Context.NOTIFICACION_TS.Add(NuevaNotificacion);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        public bool GuardaNotificacionTrabajoSocial(SSP.Servidor.NOTIFICACION_TS Entity, short TipoMensaje)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ConsecutivoNotificacionTS = GetIDProceso<decimal>("NOTIFICACION_TS", "ID_NOTIFICACION_TS", string.Format("ID_CENTRO_UBI={0}", Entity.ID_CENTRO_UBI));
                    var _NotificacionHechaPorMedico = Context.NOTIFICACION_TS.FirstOrDefault(x => x.ID_NOTIFICACION_TS == Entity.ID_NOTIFICACION_TS);
                    if (_NotificacionHechaPorMedico != null)
                    {
                        var NotificacionHechaPorTrabajoSocial = new SSP.Servidor.NOTIFICACION_TS()
                        {
                            ID_ANIO = Entity.ID_ANIO,
                            ID_CANALIZACION_TS = null,
                            ID_CARACTER = Entity.ID_CARACTER,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_DIAGNOSTICO = Entity.ID_DIAGNOSTICO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ID_NOTIFICACION_TS = ConsecutivoNotificacionTS,
                            ID_RIESGOS = Entity.ID_RIESGOS,
                            ID_USUARIO = Entity.ID_USUARIO,
                            MENSAJE = Entity.MENSAJE,
                            OTROS_RIESGOS = Entity.OTROS_RIESGOS,
                            REGISTRO_FEC = Entity.REGISTRO_FEC,
                            ID_INGRESO = Entity.ID_INGRESO,
                            ID_CENTRO_UBI = Entity.ID_CENTRO_UBI,
                            FOLIO_NOTIF = null,
                            ID_NOTIFICAION_TIPO = (decimal)eTiposNotificaciones.RESPUESTA_TRABAJO_SOCIAL
                        };

                        _NotificacionHechaPorMedico.ID_CANALIZACION_TS = NotificacionHechaPorTrabajoSocial.ID_NOTIFICACION_TS;

                        System.DateTime _fecha = GetFechaServerDate();
                        var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                        var _detallesImputado = Context.INGRESO.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_INGRESO == Entity.ID_INGRESO);
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        if (_MensajeConsiderado != null)
                            if (_id_mensaje != decimal.Zero)
                            {
                                var NvoMensaje = new SSP.Servidor.MENSAJE()
                                {
                                    CONTENIDO = string.Format("{0} CON RESPECTO AL IMPUTADO: \n - {1} {2} {3}.",
                                        _MensajeConsiderado.CONTENIDO,
                                        _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.NOMBRE) ? _detallesImputado.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                        _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.PATERNO) ? _detallesImputado.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                        _detallesImputado != null ? _detallesImputado.IMPUTADO != null ? !string.IsNullOrEmpty(_detallesImputado.IMPUTADO.MATERNO) ? _detallesImputado.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty),
                                    ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                    ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                    ID_MENSAJE = _id_mensaje,
                                    REGISTRO_FEC = _fecha,
                                    ID_CENTRO = SSP.Servidor.GlobalVariables.gCentro
                                };

                                Context.MENSAJE.Add(NvoMensaje);
                            };

                        Context.Entry(_NotificacionHechaPorMedico).State = System.Data.EntityState.Modified;
                        Context.NOTIFICACION_TS.Add(NotificacionHechaPorTrabajoSocial);
                        Context.SaveChanges();
                        transaccion.Complete();

                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (System.Exception exc)
            {
                return false;
            }
        }

        public IQueryable<SSP.Servidor.NOTIFICACION_TS> BuscarNotificacionesMedico(System.DateTime? FechaInicio, System.DateTime? FechaFin)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.NOTIFICACION_TS>();
                predicado = predicado.And(x => x.ID_NOTIFICAION_TIPO == (decimal)eTiposNotificaciones.RESPUESTA_TRABAJO_SOCIAL);
                if (FechaInicio.HasValue)
                    predicado = predicado.And(x => System.Data.Objects.EntityFunctions.TruncateTime(x.REGISTRO_FEC) >= FechaInicio);
                if (FechaFin.HasValue)
                    predicado = predicado.And(x => System.Data.Objects.EntityFunctions.TruncateTime(x.REGISTRO_FEC) <= FechaFin);
                return GetData(predicado.Expand());
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public IQueryable<SSP.Servidor.NOTIFICACION_TS> CargaInicialNotificaciones()
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.NOTIFICACION_TS>();
                System.DateTime _fechaHoy = GetFechaServerDate();
                System.DateTime f1 = new System.DateTime(_fechaHoy.Year, _fechaHoy.Month, _fechaHoy.Day);

                predicado = predicado.And(x => x.ID_NOTIFICAION_TIPO == (decimal)SSP.Controlador.Catalogo.Justicia.cNotificacionTSResultadoServAuxSSP.eTiposNotificaciones.RESPUESTA_TRABAJO_SOCIAL);
                predicado = predicado.And(x => System.Data.Objects.EntityFunctions.TruncateTime(x.REGISTRO_FEC) >= f1);
                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public IQueryable<SSP.Servidor.NOTIFICACION_TS> BuscarNotificacionesTrabajoSocial(System.DateTime? FechaInicio, System.DateTime? FechaFin)
        {
            try
            {
                var predicado = PredicateBuilder.True<SSP.Servidor.NOTIFICACION_TS>();
                predicado = predicado.And(x => x.ID_NOTIFICAION_TIPO == (decimal)eTiposNotificaciones.CANALIZA_TRABAJO_SOCIAL);
                predicado = predicado.And(x => x.ID_CANALIZACION_TS == null);
                if (FechaInicio.HasValue)
                    predicado = predicado.And(x => System.Data.Objects.EntityFunctions.TruncateTime(x.REGISTRO_FEC) >= FechaInicio);
                if (FechaFin.HasValue)
                    predicado = predicado.And(x => System.Data.Objects.EntityFunctions.TruncateTime(x.REGISTRO_FEC) <= FechaFin);
                return GetData(predicado.Expand());
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}