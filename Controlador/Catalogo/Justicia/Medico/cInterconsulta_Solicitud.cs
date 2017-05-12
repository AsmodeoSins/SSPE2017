using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using LinqKit;
using System.Data.Objects;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cInterconsulta_Solicitud:EntityManagerServer<INTERCONSULTA_SOLICITUD>
    {
        /// <summary>
        /// Actualiza un registro de solcitud de interconsulta externa
        /// </summary>
        /// <param name="entidad">Solicitud de interconsulta externa a actualizar</param>
        /// <param name="fecha_servidor">Fecha del servidor</param>
        /// <param name="isReagenda">Indica si la actualizacion es una reagenda</param>
        /// <param name="servicios_aux_interconsulta">Nuevo listado de servicios auxiliares de diagnostico para la solicitud de interconsulta</param>
        /// <param name="hoja_referencia_medica">Hoja de referencia medica a actualizar</param>
        /// <param name="interconsulta_interna">Datos de la interconsulta interna a actualizar</param>
        /// <param name="id_mensaje_sol_ext_reag">Id del mensaje para reagenda de solicitudes de interconsulta externa</param>
        /// <param name="id_mensaje_sol_ext_canc">Id del mensaje para cancelacion de solicitudes de interconsulta externa </param>
        /// <param name="id_mensaje_sol_int_canc">Id del mensaje para cancelacion de solicitudes de interconsulta interna</param>
        /// <param name="canalizacion">Entidad de la canalizacion que se va a afectar</param>
        /// <param name="canalizacion_especialidad">Listado de la entidades de Canalizacion_Especialidades que se van a afectar</param>
        /// <param name="canalizacion_serv_aux">Listado de las entidades de Canalizacion_Serv_Aux que se van a afectar</param>
        public void Actualizar (INTERCONSULTA_SOLICITUD entidad, DateTime fecha_servidor, bool isReagenda, List<SERVICIO_AUX_INTERCONSULTA> servicios_aux_interconsulta=null,
            HOJA_REFERENCIA_MEDICA hoja_referencia_medica = null, SOL_INTERCONSULTA_INTERNA interconsulta_interna = null,CANALIZACION canalizacion=null, List<CANALIZACION_SERV_AUX> canalizacion_serv_aux=null,
            List<CANALIZACION_ESPECIALIDAD> canalizacion_especialidad=null, short? id_mensaje_sol_ext_reag = null, short? id_mensaje_sol_ext_canc=null, short? id_mensaje_sol_int_canc=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                   
                    if (canalizacion!=null)
                    {
                        Context.CANALIZACION.Attach(canalizacion);
                        Context.Entry(canalizacion).State = EntityState.Modified;
                    }
                    if (canalizacion_serv_aux!=null && canalizacion_serv_aux.Count>0)
                    {
                        foreach(var item in canalizacion_serv_aux)
                        {
                            Context.CANALIZACION_SERV_AUX.Attach(item);
                            Context.Entry(item).State = EntityState.Modified;
                        }
                    }
                    if (canalizacion_especialidad!=null && canalizacion_especialidad.Count>0)
                    {
                        foreach(var item in canalizacion_especialidad)
                        {
                            Context.CANALIZACION_ESPECIALIDAD.Attach(item);
                            Context.Entry(item).State = EntityState.Modified;
                        }
                    }
                    Context.SaveChanges();
                    if (canalizacion!=null)
                    {
                        var _canalizacion_serv_aux = Context.CANALIZACION_SERV_AUX.Where(a => a.ID_ATENCION_MEDICA == canalizacion.ID_ATENCION_MEDICA);
                        var _canalizacion_especialidad = Context.CANALIZACION_ESPECIALIDAD.Where(a => a.ID_ATENCION_MEDICA == canalizacion.ID_ATENCION_MEDICA);
                        var boolCanalizacionCompletada = true;
                        if (_canalizacion_serv_aux != null && _canalizacion_serv_aux.Any(w => w.ID_ESTATUS != "A") || _canalizacion_especialidad != null && _canalizacion_especialidad.Any(w => w.ID_ESTATUS != "A"))
                            boolCanalizacionCompletada = false;
                        if (boolCanalizacionCompletada)
                        {
                            canalizacion.ID_ESTATUS_CAN = "A";
                            Context.SaveChanges();
                        }                    
                    }
                    var _interconsulta_solicitud = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == entidad.ID_INTERSOL && w.ID_CENTRO_UBI==entidad.ID_CENTRO_UBI);
                    
                    Context.Entry(_interconsulta_solicitud).CurrentValues.SetValues(entidad);
                    Context.SaveChanges();
                    if (servicios_aux_interconsulta!=null && servicios_aux_interconsulta.Count>0)
                    {
                        var algo = Context.INTERCONSULTA_SOLICITUD.First(w => w.ID_INTERSOL == entidad.ID_INTERSOL && w.ID_CENTRO_UBI==entidad.ID_CENTRO_UBI);
                        var _copia_solicitudes = Context.INTERCONSULTA_SOLICITUD.First(w => w.ID_INTERSOL == entidad.ID_INTERSOL).SERVICIO_AUX_INTERCONSULTA.ToList();
                        
                        foreach (var item in _copia_solicitudes)
                            Context.SERVICIO_AUX_INTERCONSULTA.Remove(item);
                        Context.SaveChanges();
                        foreach (var item in servicios_aux_interconsulta)
                            Context.SERVICIO_AUX_INTERCONSULTA.Add(item);
                        Context.SaveChanges();
                    }
                    if (interconsulta_interna!=null)
                    {
                        Context.SOL_INTERCONSULTA_INTERNA.Attach(interconsulta_interna);
                        Context.Entry(interconsulta_interna).State = EntityState.Modified;
                        Context.SaveChanges();
                    }
                    if(hoja_referencia_medica!=null)
                    {
                        Context.HOJA_REFERENCIA_MEDICA.Attach(hoja_referencia_medica);
                        Context.Entry(hoja_referencia_medica).State = EntityState.Modified;
                        Context.SaveChanges();
                    }

                    if(id_mensaje_sol_ext_canc.HasValue || id_mensaje_sol_ext_reag.HasValue || id_mensaje_sol_int_canc.HasValue)
                    {
                        var _cuerpo = string.Empty;
                        var _mensaje = new MENSAJE_TIPO();
                        var _nota_medica = Context.NOTA_MEDICA.First(w => w.ID_ATENCION_MEDICA == entidad.ID_NOTA_MEDICA);
                        var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ?
                            _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                        if (entidad.ID_INTER == 1) // Si es interconsulta interna
                        {
                            if (entidad.ESTATUS == "C" && id_mensaje_sol_int_canc.HasValue)
                            {
                                _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_sol_int_canc.Value);
                            }
                        }
                        else
                        {
                            if (entidad.ESTATUS == "C" && id_mensaje_sol_ext_canc.HasValue)
                                _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_sol_ext_canc.Value);
                            else if (isReagenda && id_mensaje_sol_ext_reag.HasValue)
                                _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_sol_ext_reag.Value);
                        }
                        if (_mensaje != null && _mensaje.ID_MEN_TIPO > 0)
                        {
                            var _prioridad = Context.INTERCONSULTA_NIVEL_PRIORIDAD.FirstOrDefault(w => w.ID_INIVEL == entidad.ID_INIVEL);
                            var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                            _cuerpo = string.Format(_mensaje.DESCR + " para el imputado {0}/{1} {2}", _nota_medica.ATENCION_MEDICA.INGRESO.ID_ANIO, _nota_medica.ATENCION_MEDICA.INGRESO.ID_IMPUTADO,
                           _nombre_completo, _prioridad.DESCR);
                            Context.MENSAJE.Add(new MENSAJE
                            {
                                CONTENIDO = _cuerpo,
                                ENCABEZADO = _mensaje.ENCABEZADO,
                                ID_CENTRO = _nota_medica.ATENCION_MEDICA.INGRESO.ID_UB_CENTRO,
                                ID_MEN_TIPO = _mensaje.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = fecha_servidor,
                            });
                            Context.SaveChanges();
                        }
                    }
                    transaccion.Complete();
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Inserta una solicitud de interconsulta
        /// </summary>
        /// <param name="entidad">Entidad de INTERCONSULTA_SOLICITUD a insertar</param>
        /// <param name="id_centro">ID del centro al cual se le van a enviar los parametros</param>
        /// <param name="id_mensaje_sol_int">ID del mensaje para enviar en caso de ser solicitud interna</param>
        /// <param name="id_mensaje_sol_ext">ID del mensaje para enviar en caso de ser solicitud externa</param>
        /// <param name="fecha_servidor">Fecha del servidor</param>
        public void Insertar(INTERCONSULTA_SOLICITUD entidad, short id_centro, short id_mensaje_sol_int, short id_mensaje_sol_ext, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var boolCanalizacionCompletada = true;
                    entidad.ID_INTERSOL = GetIDProceso<short>("INTERCONSULTA_SOLICITUD", "ID_INTERSOL", string.Format("ID_CENTRO_UBI={0}",entidad.ID_CENTRO_UBI));
                    if (entidad.SOL_INTERCONSULTA_INTERNA != null && entidad.SOL_INTERCONSULTA_INTERNA.Count > 0)
                        entidad.SOL_INTERCONSULTA_INTERNA.First().ID_SOLICITUD = GetIDProceso<int>("SOL_INTERCONSULTA_INTERNA", "ID_SOLICITUD", string.Format("ID_CENTRO_UBI={0}", entidad.ID_CENTRO_UBI));
                    if (entidad.HOJA_REFERENCIA_MEDICA != null && entidad.HOJA_REFERENCIA_MEDICA.Count > 0)
                        entidad.HOJA_REFERENCIA_MEDICA.First().ID_HOJA = GetIDProceso<int>("HOJA_REFERENCIA_MEDICA", "ID_HOJA", string.Format("ID_CENTRO_UBI={0}", entidad.ID_CENTRO_UBI));
                    Context.INTERCONSULTA_SOLICITUD.Add(entidad);
                    var _canalizacion = Context.CANALIZACION.First(w => w.ID_ATENCION_MEDICA == entidad.ID_NOTA_MEDICA);
                    if (entidad.SERVICIO_AUX_INTERCONSULTA !=null && entidad.SERVICIO_AUX_INTERCONSULTA.Count>0)
                    {
                        foreach(var item in entidad.SERVICIO_AUX_INTERCONSULTA)
                        {
                            _canalizacion.CANALIZACION_SERV_AUX.First(w => w.ID_SERV_AUX == item.ID_SERV_AUX).ID_ESTATUS = "A";
                            _canalizacion.CANALIZACION_SERV_AUX.First(w => w.ID_SERV_AUX == item.ID_SERV_AUX).ID_INIVEL = entidad.ID_INIVEL;
                            _canalizacion.CANALIZACION_SERV_AUX.First(w => w.ID_SERV_AUX == item.ID_SERV_AUX).ID_INTER = entidad.ID_INTER;
                        }
                        Context.SaveChanges();
                        
                    }
                    if (entidad.ID_ESPECIALIDAD.HasValue)
                    {
                        _canalizacion.CANALIZACION_ESPECIALIDAD.First(w => w.ID_ESPECIALIDAD == entidad.ID_ESPECIALIDAD.Value).ID_ESTATUS = "A";
                        _canalizacion.CANALIZACION_ESPECIALIDAD.First(w => w.ID_ESPECIALIDAD == entidad.ID_ESPECIALIDAD.Value).ID_INTER = entidad.ID_INTER;
                        _canalizacion.CANALIZACION_ESPECIALIDAD.First(w => w.ID_ESPECIALIDAD == entidad.ID_ESPECIALIDAD.Value).ID_INIVEL = entidad.ID_INIVEL;
                        Context.SaveChanges();
                    }
                    if (_canalizacion.CANALIZACION_SERV_AUX.Any(w => w.ID_ESTATUS != "A")||_canalizacion.CANALIZACION_ESPECIALIDAD.Any(w => w.ID_ESTATUS != "A"))
                        boolCanalizacionCompletada = false;
                    if (boolCanalizacionCompletada)
                    {
                        _canalizacion.ID_ESTATUS_CAN = "A";
                        Context.SaveChanges();
                    }
                    var _nota_medica = Context.NOTA_MEDICA.FirstOrDefault(w => w.ID_ATENCION_MEDICA == entidad.ID_NOTA_MEDICA);
                    var _cuerpo = string.Empty;
                    var _mensaje = new MENSAJE_TIPO();
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                    if (entidad.ID_INTER == 1)
                        _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_sol_int);
                    else
                        _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_sol_ext);
                    
                    if (_mensaje!=null)
                    {
                        var _prioridad = Context.INTERCONSULTA_NIVEL_PRIORIDAD.FirstOrDefault(w => w.ID_INIVEL == entidad.ID_INIVEL);
                        var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                        _cuerpo = string.Format(_mensaje.DESCR + " para el imputado {0}/{1} {2}. Prioridad {3} ", _nota_medica.ATENCION_MEDICA.INGRESO.ID_ANIO, _nota_medica.ATENCION_MEDICA.INGRESO.ID_IMPUTADO,
                       _nombre_completo, _prioridad.DESCR);
                        Context.MENSAJE.Add(new MENSAJE{
                            CONTENIDO=_cuerpo,
                            ENCABEZADO=_mensaje.ENCABEZADO,
                            ID_CENTRO=id_centro,
                            ID_MEN_TIPO=_mensaje.ID_MEN_TIPO,
                            ID_MENSAJE=_id_mensaje,
                            REGISTRO_FEC=fecha_servidor,
                        });
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<INTERCONSULTA_SOLICITUD> Buscar(short id_centro,List<string>estatus,short?tipo_atencion=null,short? anio_imputado = null, int? folio_imputado = null,
            string nombre = "", string paterno = "", string materno = "",short?tipo_interconsulta=null,DateTime?fecha_inicio=null,DateTime?fecha_final=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INTERCONSULTA_SOLICITUD>();
                var predicateOr = PredicateBuilder.False<INTERCONSULTA_SOLICITUD>();
                foreach (var item in estatus)
                    predicateOr = predicateOr.Or(w=>w.ESTATUS==item);
                if (estatus != null && estatus.Count > 0)
                    predicate = predicate.And(predicateOr.Expand());
                if (tipo_atencion.HasValue)
                    predicate = predicate.And(w => w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.ID_TIPO_ATENCION == tipo_atencion.Value);
                if (tipo_interconsulta.HasValue)
                {
                    predicate = predicate.And(w => w.ID_INTER == tipo_interconsulta);
                    if (fecha_inicio.HasValue || fecha_final.HasValue)
                        if (fecha_inicio.HasValue)
                            if (tipo_interconsulta == 1)
                                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= EntityFunctions.TruncateTime(fecha_inicio));
                            else
                                predicate = predicate.And(w => w.HOJA_REFERENCIA_MEDICA.Any(a=>EntityFunctions.TruncateTime(a.FECHA_CITA) >= EntityFunctions.TruncateTime(fecha_inicio)));
                        else
                            if (tipo_interconsulta == 1)
                                predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= EntityFunctions.TruncateTime(fecha_final));
                            else
                                predicate = predicate.And(w => w.HOJA_REFERENCIA_MEDICA.Any(a=>EntityFunctions.TruncateTime(a.FECHA_CITA) <= EntityFunctions.TruncateTime(fecha_final)));
                }
                else
                {
                    if (fecha_inicio.HasValue)
                            predicate = predicate.And(w => (EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= EntityFunctions.TruncateTime(fecha_inicio) && !w.HOJA_REFERENCIA_MEDICA.Any()) ||
                                (w.HOJA_REFERENCIA_MEDICA.Any(a=>EntityFunctions.TruncateTime(a.FECHA_CITA) >= EntityFunctions.TruncateTime(fecha_inicio))));
                    if (fecha_final.HasValue)
                            predicate = predicate.And(w => (EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= EntityFunctions.TruncateTime(fecha_final) && !w.HOJA_REFERENCIA_MEDICA.Any()) 
                                ||w.HOJA_REFERENCIA_MEDICA.Any(a=>EntityFunctions.TruncateTime(a.FECHA_CITA) <= EntityFunctions.TruncateTime(fecha_final)));
                }
                if (anio_imputado.HasValue && folio_imputado.HasValue)
                {
                    predicate = predicate.And(w => w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO == anio_imputado.Value && w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO == folio_imputado.Value);
                }
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.CANALIZACION.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(materno));
                predicate = predicate.And(w => w.ID_CENTRO_UBI == id_centro);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
