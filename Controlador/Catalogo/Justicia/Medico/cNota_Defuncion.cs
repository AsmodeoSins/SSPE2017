using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNota_Defuncion:EntityManagerServer<NOTA_DEFUNCION>
    {
        public void Insertar(NOTA_DEFUNCION entidad, DateTime fecha_servidor, short id_mensaje_tipo, short id_mensaje_tipo_estatal, bool isAltaHospitalizacion, short? motivo_egreso_medico=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.NOTA_DEFUNCION.Add(entidad);
                    Context.SaveChanges();
                    if (isAltaHospitalizacion)
                    {

                        var _hospitalizacion = Context.HOSPITALIZACION.FirstOrDefault(w => w.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == entidad.ID_ANIO
                            && w.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO == entidad.ID_CENTRO && w.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == entidad.ID_IMPUTADO
                            && w.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == entidad.ID_INGRESO && w.ID_HOSEST == 1);
                        if (_hospitalizacion==null)
                            throw new Exception("No existen hospitalizaciones activas para este imputado");
                        _hospitalizacion.ID_HOSEST = 2;
                        _hospitalizacion.ALTA_FEC = entidad.FECHA_DECESO;
                        _hospitalizacion.ALTA_MEDICO = entidad.ID_EMPLEADO_COORDINADOR_MED;
                        _hospitalizacion.CAMA_HOSPITAL.ESTATUS = "S";
                        var _dias_hospitalizado =Convert.ToDecimal((_hospitalizacion.ALTA_FEC.Value - _hospitalizacion.INGRESO_FEC.Value).TotalDays);
                        Context.NOTA_EGRESO.Add(new NOTA_EGRESO {
                            DIAS_ESTANCIA=_dias_hospitalizado,
                            ID_HOSPITA=_hospitalizacion.ID_HOSPITA,
                            ID_CENTRO_UBI=_hospitalizacion.ID_CENTRO_UBI,
                            FECHA_REGISTRO=fecha_servidor,
                            USUARIO_REGISTRO=entidad.USUARIO_REGISTRO,
                            ID_MOEGMED=motivo_egreso_medico.Value
                        });
                        Context.SaveChanges();
                    }
                    #region mensaje
                    var _cuerpo = string.Empty;
                    var _mensaje = new MENSAJE_TIPO();
                    var _ingreso = Context.INGRESO.FirstOrDefault(w => w.ID_ANIO == entidad.ID_ANIO
                            && w.ID_CENTRO == entidad.ID_CENTRO && w.ID_IMPUTADO == entidad.ID_IMPUTADO
                            && w.ID_INGRESO == entidad.ID_INGRESO);
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_ingreso.IMPUTADO.NOMBRE) ?
                        _ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_ingreso.IMPUTADO.PATERNO) ? _ingreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_ingreso.IMPUTADO.MATERNO) ? _ingreso.IMPUTADO.MATERNO.Trim() : string.Empty);
                    #endregion
                    _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo);
                    if (_mensaje!=null)
                    {
                        var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                        _cuerpo = string.Format(_mensaje.CONTENIDO + " para el imputado {0}/{1} {2}", _ingreso.ID_ANIO, _ingreso.ID_IMPUTADO,
                           _nombre_completo);
                        Context.MENSAJE.Add(new MENSAJE
                        {
                            CONTENIDO = _cuerpo,
                            ENCABEZADO = _mensaje.ENCABEZADO,
                            ID_CENTRO = _ingreso.ID_UB_CENTRO,
                            ID_MEN_TIPO = _mensaje.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                        });
                        Context.SaveChanges();
                    }
                    _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_estatal);
                    if (_mensaje != null)
                    {
                        var _id_entidad = Context.MUNICIPIO.First(w => w.CENTRO.Any(a => a.ID_CENTRO == entidad.ID_CENTRO)).ID_ENTIDAD;
                        var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                        _cuerpo = string.Format(_mensaje.CONTENIDO + " para el imputado {0}/{1} {2}", _ingreso.ID_ANIO, _ingreso.ID_IMPUTADO,
                           _nombre_completo);
                        Context.MENSAJE.Add(new MENSAJE
                        {
                            CONTENIDO = _cuerpo,
                            ENCABEZADO = _mensaje.ENCABEZADO,
                            ID_ENTIDAD = _id_entidad,
                            ID_MEN_TIPO = _mensaje.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                        });
                        Context.SaveChanges();
                    }
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        /// <summary>
        /// Obtiene las tarjetas informativas de deceso
        /// </summary>
        /// <param name="id_centro">ID del centro al cual pertenece el registro</param>
        /// <param name="id_anio">ID_ANIO parte del folio del ingreso</param>
        /// <param name="id_imputado">ID_IMPUTADO parte del folio del ingreso</param>
        /// <param name="paterno">Apellido paterno del imputado</param>
        /// <param name="materno">Apellido materno del imputado</param>
        /// <param name="nombre">Nombre del imputado</param>
        /// <param name="fecha_inicio">Fecha inicial para el rango de busqueda en las fechas de deceso</param>
        /// <param name="fecha_final">Fecha final para el rango de busqueda en las fechas de deceso</param>
        /// <returns>IQueryable&lt;NOTA_DEFUNCION&gt;</returns>
        public IQueryable<NOTA_DEFUNCION>Buscar(short id_centro, short? id_anio=null, int? id_imputado=null, string paterno="", string materno="", string nombre="",
            DateTime? fecha_inicio=null, DateTime? fecha_final=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<NOTA_DEFUNCION>();
                if (id_anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == id_anio);
                if (id_imputado.HasValue)
                    predicate = predicate.And(w=>w.ID_IMPUTADO==id_imputado);
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(paterno.Trim()));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(materno.Trim()));
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(nombre.Trim()));
                if (fecha_inicio.HasValue)
                    predicate = predicate.And(w=>EntityFunctions.TruncateTime(w.INGRESO.NOTA_DEFUNCION.FECHA_DECESO)>=EntityFunctions.TruncateTime(fecha_inicio.Value));
                if (fecha_final.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.INGRESO.NOTA_DEFUNCION.FECHA_DECESO) <= EntityFunctions.TruncateTime(fecha_final.Value));
                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == id_centro);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
