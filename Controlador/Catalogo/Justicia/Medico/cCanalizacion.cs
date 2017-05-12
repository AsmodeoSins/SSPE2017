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
    public class cCanalizacion : EntityManagerServer<CANALIZACION>
    {

        public void Actualizar(CANALIZACION entidad, List<CANALIZACION_ESPECIALIDAD>canalizacion_especialidades, List<CANALIZACION_SERV_AUX>canalizacion_serv_aux, short id_centro, DateTime fecha_servidor, short? id_mensaje_edicion=null, short? id_mensaje_cancelacion=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    if (canalizacion_serv_aux!=null && canalizacion_serv_aux.Count>0)
                    {
                        var _servicios_aux = Context.CANALIZACION_SERV_AUX.Where(w => w.ID_ATENCION_MEDICA == entidad.ID_ATENCION_MEDICA).ToList();
                        if (_servicios_aux != null)
                            foreach (var item in _servicios_aux)
                            Context.CANALIZACION_SERV_AUX.Remove(item);
                        foreach (var item in canalizacion_serv_aux)
                            Context.CANALIZACION_SERV_AUX.Add(item);
                    }
                    if (canalizacion_especialidades!=null && canalizacion_especialidades.Count>0)
                    {
                        var _canalizaciones_especialidades = Context.CANALIZACION_ESPECIALIDAD.Where(w => w.ID_ATENCION_MEDICA == entidad.ID_ATENCION_MEDICA).ToList();
                        foreach (var item in _canalizaciones_especialidades)
                            Context.CANALIZACION_ESPECIALIDAD.Remove(item);
                        foreach (var item in canalizacion_especialidades)
                            Context.CANALIZACION_ESPECIALIDAD.Add(item);
                    }
                    Context.SaveChanges();
                    Context.CANALIZACION.Attach(entidad);
                    Context.Entry(entidad).State = EntityState.Modified;
                    Context.SaveChanges();
                    var _nota_medica = Context.NOTA_MEDICA.FirstOrDefault(w => w.ID_ATENCION_MEDICA == entidad.ID_ATENCION_MEDICA);
                    var _cuerpo = string.Empty;
                    var _mensaje = new MENSAJE_TIPO();
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                    if (id_mensaje_edicion.HasValue)
                        _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_edicion);
                    else
                        _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_cancelacion);
                    if (_mensaje != null)
                    {
                        var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                        _cuerpo = string.Format(_mensaje.DESCR + " para el imputado {0}/{1} {2}.", _nota_medica.ATENCION_MEDICA.INGRESO.ID_ANIO, _nota_medica.ATENCION_MEDICA.INGRESO.ID_IMPUTADO,
                       _nombre_completo);
                        Context.MENSAJE.Add(new MENSAJE
                        {
                            CONTENIDO = _cuerpo,
                            ENCABEZADO = _mensaje.ENCABEZADO,
                            ID_CENTRO = id_centro,
                            ID_MEN_TIPO = _mensaje.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                        });
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        /// <summary>
        /// Inserta una solicitud de interconsulta
        /// </summary>
        /// <param name="entidad">Entidad de CANALIZACION a insertar</param>
        /// <param name="id_centro">ID del centro al cual se le van a enviar los parametros</param>
        /// <param name="id_mensaje_sol_int">ID del mensaje para canalizacion</param>
        /// <param name="fecha_servidor">Fecha del servidor</param>
        public void Insertar(CANALIZACION entidad, short id_centro, short id_mensaje, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.CANALIZACION.Add(entidad);
                    var _nota_medica = Context.NOTA_MEDICA.FirstOrDefault(w => w.ID_ATENCION_MEDICA == entidad.ID_ATENCION_MEDICA);
                    var _cuerpo = string.Empty;
                    var _mensaje = new MENSAJE_TIPO();
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(_nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO) ? _nota_medica.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);
                    _mensaje = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje);
                    if (_mensaje != null)
                    {
                        var _id_mensaje = GetIDProceso<Int32>("MENSAJE", "ID_MENSAJE", "1=1");
                        _cuerpo = string.Format(_mensaje.DESCR + " para el imputado {0}/{1} {2}.", _nota_medica.ATENCION_MEDICA.INGRESO.ID_ANIO, _nota_medica.ATENCION_MEDICA.INGRESO.ID_IMPUTADO,
                       _nombre_completo);
                        Context.MENSAJE.Add(new MENSAJE
                        {
                            CONTENIDO = _cuerpo,
                            ENCABEZADO = _mensaje.ENCABEZADO,
                            ID_CENTRO = id_centro,
                            ID_MEN_TIPO = _mensaje.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                        });
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        /// <summary>
        /// Obtiene las canalizaciones
        /// </summary>
        /// <param name="centro">Id del centro a consultar</param>
        /// <param name="anio_imputado">Año en id del Imputado</param>
        /// <param name="folio_imputado">Folio en id del Imputado</param>
        /// <param name="nombre">Nombre del imputado</param>
        /// <param name="paterno">Apellido paterno del imputado</param>
        /// <param name="materno">Apellido materno del nombre</param>
        /// <param name="fecha_inicio">Fecha Incial del Rango de Busqueda</param>
        /// <param name="fecha_fin">Fecha Final del Rango de Busqueda</param>
        /// <param name="estatus_administrativos_inactivos">Listado que contiene los estatus administrativos que vuelven inactivo un ingreso</param>
        /// <param name="tipo_atencion">Llave del tipo de atencion medica a buscar</param>
        /// <returns>IQueryable&lt;CANALIZACION&gt;</returns>
        public IQueryable<CANALIZACION> ObtenerCanalizaciones(string estatus, short centro, short?[] estatus_administrativos_inactivos, short? tipo_atencion = null, short? anio_imputado = null, int? folio_imputado = null, string nombre = "", string paterno = "", string materno = "", DateTime? fecha_inicio = null, DateTime? fecha_fin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CANALIZACION>();
                if (string.IsNullOrWhiteSpace(estatus))
                    throw new Exception("El estatus de la canalización para la busqueda no puede estar vacio");
                if (tipo_atencion.HasValue)
                    predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.ID_TIPO_ATENCION == tipo_atencion);
                if (anio_imputado.HasValue && folio_imputado.HasValue)
                {
                    predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ANIO == anio_imputado.Value && w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_IMPUTADO == folio_imputado.Value);
                }
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Contains(materno));
                if (fecha_inicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.NOTA_MEDICA.CANALIZACION.ID_FECHA) >= EntityFunctions.TruncateTime(fecha_inicio));
                if (fecha_fin.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.NOTA_MEDICA.CANALIZACION.ID_FECHA) <= EntityFunctions.TruncateTime(fecha_fin));
                foreach (var item in estatus_administrativos_inactivos)
                    if (item.HasValue)
                        predicate = predicate.And(w => w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                predicate = predicate.And(w => w.ID_ESTATUS_CAN == estatus && w.NOTA_MEDICA.ATENCION_MEDICA.INGRESO.ID_UB_CENTRO == centro);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene las canalizaciones
        /// </summary>
        /// <param name="id_atencion_medica">ID de la canalización</param>
        /// <returns>IQueryable&lt;CANALIZACION&gt;</returns>
        public IQueryable<CANALIZACION> ObtenerCanalizaciones(int id_atencion_medica, short id_centro_ubi)
        {
            try
            {
                return GetData(w=>w.ID_ATENCION_MEDICA==id_atencion_medica && w.ID_CENTRO_UBI==id_centro_ubi);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
