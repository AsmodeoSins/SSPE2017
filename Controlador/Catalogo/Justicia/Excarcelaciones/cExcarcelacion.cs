using LinqKit;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cExcarcelacion : EntityManagerServer<EXCARCELACION>
    {
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="_traslado">objeto de tipo "EXCARCELACION" con valores a insertar</param>
        /// <param name="id_mensaje_tipo_comandancia">llave numerica del tipo de mensaje para mensaje de excarcelacion para comandancia</param>
        /// <param name="id_mensaje_tipo_area_medica">llave numerica del tipo de mensaje para mensaje de excarcelacion para comandancia</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        /// <param name="id_otro_hospital">id de referencia de otro hospital en catalogo de hospitales</param>
        /// <param name="centro_ubicacion">id del centro donde se esta realizando la excarcelacion</param>
        public void Insertar(EXCARCELACION _excarcelacion, int id_mensaje_tipo_comandancia, int? id_mensaje_tipo_area_medica, DateTime fecha_servidor, int id_otro_hospital, short centro_ubicacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _destinos = new List<string>();
                    var _id_consec = GetIDProceso<int>("EXCARCELACION", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", _excarcelacion.ID_CENTRO,
                    _excarcelacion.ID_ANIO, _excarcelacion.ID_IMPUTADO, _excarcelacion.ID_INGRESO));
                    var _id_consec_destino = 1;
                    foreach (var item in _excarcelacion.EXCARCELACION_DESTINO)
                    {
                        item.ID_CONSEC = _id_consec;
                        item.ID_DESTINO = _id_consec_destino;
                        _id_consec_destino += 1;
                        if (_excarcelacion.ID_TIPO_EX == 1)
                            _destinos.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                        else if (_excarcelacion.ID_TIPO_EX == 2)
                        {
                            var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item.ID_INTERSOL);
                            if (_solicitud_interconsulta != null)
                            {
                                //modifica el estatus de la interconsulta
                                _solicitud_interconsulta.ESTATUS = "N";
                                //agrega destinos al mensaje para exc. medica.
                                _destinos.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ?
                                    _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                            }
                        }
                    }
                    _excarcelacion.ID_CONSEC = _id_consec;
                    Context.EXCARCELACION.Add(_excarcelacion);
                    var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_comandancia);
                    var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == _excarcelacion.ID_ANIO && w.ID_CENTRO == _excarcelacion.ID_CENTRO && w.ID_IMPUTADO == _excarcelacion.ID_IMPUTADO);
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty);
                    var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                    var _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3} \nDESTINOS:\n", _excarcelacion.ID_ANIO, _excarcelacion.ID_IMPUTADO, string.Format("{0:dd/MM/yyyy}", _excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", _excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                    foreach (var item in _destinos)
                        _contenido += (item + "\n");
                    var _mensaje = new MENSAJE
                    {
                        CONTENIDO = _contenido,
                        ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                        ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                        ID_MENSAJE = _id_mensaje,
                        REGISTRO_FEC = fecha_servidor,
                        ID_CENTRO = centro_ubicacion
                    };
                    Context.MENSAJE.Add(_mensaje);
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool GenerarIncidenciaCertificadoMedico(EXCARCELACION EntityExc)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.EXCARCELACION.Attach(EntityExc);
                    Context.Entry(EntityExc).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(EntityExc).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(EntityExc).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool CambiarResponsable(EXCARCELACION Excarcelacion)
        {
            try
            {
                Context.EXCARCELACION.Attach(Excarcelacion);
                Context.Entry(Excarcelacion).Property(x => x.RESPONSABLE).IsModified = true;
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool RestaurarExcarcelacion(INGRESO_UBICACION IngresoUbicacion, EXCARCELACION Excarcelacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var Ubicacion = Context.INGRESO_UBICACION.Where(w =>
                    w.ID_CENTRO == IngresoUbicacion.ID_CENTRO &&
                    w.ID_ANIO == IngresoUbicacion.ID_ANIO &&
                    w.ID_IMPUTADO == IngresoUbicacion.ID_IMPUTADO &&
                    w.ID_INGRESO == IngresoUbicacion.ID_INGRESO &&
                    w.ID_CONSEC == IngresoUbicacion.ID_CONSEC).FirstOrDefault();
                    if (Ubicacion != null)
                    {
                        Context.INGRESO_UBICACION.Remove(Ubicacion);
                        if (Excarcelacion != null)
                        {
                            Context.EXCARCELACION.Attach(Excarcelacion);
                            Context.Entry(Excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                            Context.Entry(Excarcelacion).Property(x => x.RETORNO_FEC).IsModified = true;
                            Context.Entry(Excarcelacion).Property(x => x.ID_INCIDENCIA_TRASLADO_RETORNO).IsModified = true;
                            Context.Entry(Excarcelacion).Property(x => x.INCIDENCIA_OBSERVACION_RETORNO).IsModified = true;

                        }
                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool IniciarExcarcelacion(EXCARCELACION Excarcelacion)
        {
            try
            {
                Context.EXCARCELACION.Attach(Excarcelacion);
                Context.Entry(Excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                Context.Entry(Excarcelacion).Property(x => x.SALIDA_FEC).IsModified = true;
                if (Excarcelacion.ID_INCIDENCIA_TRASLADO.HasValue)
                {
                    Context.Entry(Excarcelacion).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;
                }
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool RetornoExcarcelacion(INGRESO_UBICACION IngresoUbicacion, EXCARCELACION Excarcelacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    IngresoUbicacion.ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(IngresoUbicacion.ID_CENTRO, IngresoUbicacion.ID_ANIO, IngresoUbicacion.ID_IMPUTADO, IngresoUbicacion.ID_INGRESO);
                    Context.INGRESO_UBICACION.Add(IngresoUbicacion);
                    Context.EXCARCELACION.Attach(Excarcelacion);
                    Context.Entry(Excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.RETORNO_FEC).IsModified = true;
                    if (Excarcelacion.ID_INCIDENCIA_TRASLADO_RETORNO != null)
                    {
                        Context.Entry(Excarcelacion).Property(x => x.ID_INCIDENCIA_TRASLADO_RETORNO).IsModified = true;
                        Context.Entry(Excarcelacion).Property(x => x.INCIDENCIA_OBSERVACION_RETORNO).IsModified = true;
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public void IniciarExcarcelacionEnProceso(List<EXCARCELACION> ListaExcarcelaciones)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ListaExcarcelacionesAux = ListaExcarcelaciones.OrderBy(o => o.ID_CENTRO).ThenBy(t => t.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).ToList();
                    foreach (var excarcelacion in ListaExcarcelacionesAux)
                    {
                        var exc = Context.EXCARCELACION.Where(w =>
                            w.ID_CENTRO == excarcelacion.ID_CENTRO &&
                            w.ID_ANIO == excarcelacion.ID_ANIO &&
                            w.ID_IMPUTADO == excarcelacion.ID_IMPUTADO &&
                            w.ID_INGRESO == excarcelacion.ID_INGRESO &&
                            w.ID_CONSEC == excarcelacion.ID_CONSEC &&
                            w.ID_ESTATUS == excarcelacion.ID_ESTATUS &&
                            w.ID_CONSEC == excarcelacion.ID_CONSEC).FirstOrDefault();

                        var exc_destino = Context.EXCARCELACION_DESTINO.Where(w =>
                            w.ID_CENTRO == excarcelacion.ID_CENTRO &&
                            w.ID_ANIO == excarcelacion.ID_ANIO &&
                            w.ID_IMPUTADO == excarcelacion.ID_IMPUTADO &&
                            w.ID_INGRESO == excarcelacion.ID_INGRESO &&
                            w.ID_CONSEC == excarcelacion.ID_CONSEC &&
                            w.ID_ESTATUS == excarcelacion.ID_ESTATUS &&
                            w.ID_CONSEC == excarcelacion.ID_CONSEC).FirstOrDefault();

                        exc.ID_ESTATUS = "EP";
                        Context.Entry(excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                        Context.SaveChanges();
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

        public bool RevertirUltimoMovimiento(EXCARCELACION Excarcelacion, INCIDENTE Incidente)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.EXCARCELACION.Attach(Excarcelacion);
                    Context.Entry(Excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.RESPONSABLE).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;
                    Context.INCIDENTE.Add(Incidente);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool RevertirExcarcelacion(INGRESO_UBICACION Ingreso_Ubicacion, EXCARCELACION Excarcelacion, INCIDENTE Incidente)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.INGRESO_UBICACION.Add(Ingreso_Ubicacion);
                    Context.EXCARCELACION.Attach(Excarcelacion);
                    Context.Entry(Excarcelacion).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.RESPONSABLE).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Excarcelacion).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;
                    Context.INCIDENTE.Add(Incidente);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public List<EXCARCELACION> ObtenerImputadoExcarcelaciones(short Id_Centro, short Id_Anio, int Id_Imputado, short Id_Ingreso)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO == Id_Centro &&
                    g.ID_ANIO == Id_Anio &&
                    g.ID_IMPUTADO == Id_Imputado &&
                    g.ID_INGRESO == Id_Ingreso).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(EXCARCELACION Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro nuevo
        /// </summary>
        /// <param name="_excarcelacion">objeto de tipo "EXCARCELACION" con valores a actualizar</param>
        /// <param name="id_mensaje_tipo_comandancia_modificacion">llave numerica del tipo de mensaje que se le envia a comandancia y direccion durante modificacion del registro</param>
        /// <param name="id_mensaje_tipo_comandancia_cancelacion">llave numerica del tipo de mensaje que se le envia a comandancia y direccion durante cancelacion del registro</param>
        /// <param name="id_mensaje_tipo_area_medica_cancelacion">llave numerica del tipo de mensaje que se le envia al area medica durante cancelacion del registro para cancelacion de la toma de certificado</param>
        /// <param name="id_mensaje_tipo">llave numerica del tipo de mensaje para calendarizar excarcelacion</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        /// <param name="id_otro_hospital">llave numerica de referencia de otro hospital en catalogo de hospitales</param>
        /// <param name="modifico_fecha_programada">si se modifico la fecha programa de la excarcelacion</param>
        /// <param name="centro_ubicacion">id del centro donde se esta realizando la excarcelacion</param>
        public void Actualizar(EXCARCELACION _excarcelacion, int id_mensaje_tipo_comandancia_modificacion,
            int id_mensaje_tipo_comandancia_cancelacion, int id_mensaje_tipo_area_medica_cancelacion,
            DateTime fecha_servidor, int id_otro_hospital, bool modifico_fecha_programada, short id_centro_ubicacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _destinos_cancelados = new List<string>();
                    var _destinos = new List<string>();
                    var _consec = _excarcelacion.EXCARCELACION_DESTINO.Select(s => s.ID_DESTINO).Max() + 1;
                    foreach (var item in _excarcelacion.EXCARCELACION_DESTINO)
                    {
                        if (item.ID_DESTINO == 0)
                        {
                            item.ID_DESTINO = _consec;
                            Context.EXCARCELACION_DESTINO.Add(item);
                            _consec += 1;
                        }
                        else
                            Context.Entry(item).State = EntityState.Modified;

                        if (item.ID_ESTATUS == "CA")
                        {
                            if (_excarcelacion.ID_TIPO_EX == 1)
                                _destinos_cancelados.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                            else if (_excarcelacion.ID_TIPO_EX == 2)
                            {
                                var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item.ID_INTERSOL);
                                _destinos_cancelados.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ?
                                    _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                                _solicitud_interconsulta.CANALIZACION.ID_ESTATUS_CAN = "P";
                                if (_solicitud_interconsulta.SERVICIO_AUX_INTERCONSULTA!=null && _solicitud_interconsulta.SERVICIO_AUX_INTERCONSULTA.Count>0)
                                {
                                    foreach (var item_serv_aux in _solicitud_interconsulta.SERVICIO_AUX_INTERCONSULTA)
                                        _solicitud_interconsulta.CANALIZACION.CANALIZACION_SERV_AUX.FirstOrDefault(w => w.ID_SERV_AUX == item_serv_aux.ID_SERV_AUX).ID_ESTATUS = "P";
                                }
                                if (_solicitud_interconsulta.ID_ESPECIALIDAD.HasValue)
                                    _solicitud_interconsulta.CANALIZACION.CANALIZACION_ESPECIALIDAD.FirstOrDefault(w => w.ID_ESPECIALIDAD == _solicitud_interconsulta.ID_ESPECIALIDAD).ID_ESTATUS = "P";
                            }

                        }
                        else
                        {
                            if (_excarcelacion.ID_TIPO_EX == 1)
                                _destinos.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                            else if (_excarcelacion.ID_TIPO_EX == 2)
                            {
                                var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item.ID_INTERSOL);
                                _destinos.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ?
                                    _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                            }
                        }

                    }
                    Context.SaveChanges();
                    if (!_excarcelacion.EXCARCELACION_DESTINO.Any(w => w.ID_ESTATUS != "CA"))
                        _excarcelacion.ID_ESTATUS = "CA";
                    Context.Entry(_excarcelacion).State = EntityState.Modified;
                    MENSAJE_TIPO _mensaje_tipo = null;
                    if (!_excarcelacion.ID_ESTATUS.Equals("CA", StringComparison.InvariantCultureIgnoreCase))
                        _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_comandancia_modificacion);
                    else
                        _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_comandancia_cancelacion);
                    var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == _excarcelacion.ID_ANIO && w.ID_CENTRO == _excarcelacion.ID_CENTRO && w.ID_IMPUTADO == _excarcelacion.ID_IMPUTADO);
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty);
                    var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                    var _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", _excarcelacion.ID_ANIO, _excarcelacion.ID_IMPUTADO,
                        string.Format("{0:dd/MM/yyyy}", _excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", _excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                    if (_destinos.Count > 0)
                        _contenido += "\nDESTINOS:\n";
                    foreach (var item in _destinos)
                        _contenido += (item + "\n");
                    if (_destinos_cancelados.Count > 0)
                        _contenido += "\nDESTINOS CANCELADOS:\n";
                    foreach (var item in _destinos_cancelados)
                        _contenido += (item + "\n");
                    var _mensaje = new MENSAJE
                    {
                        CONTENIDO = _contenido,
                        ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                        ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                        ID_MENSAJE = _id_mensaje,
                        REGISTRO_FEC = fecha_servidor,
                        ID_CENTRO = id_centro_ubicacion
                    };
                    Context.MENSAJE.Add(_mensaje);
                    Context.SaveChanges();
                    if (_excarcelacion.ID_ESTATUS.Equals("CA", StringComparison.InvariantCultureIgnoreCase) && ((_excarcelacion.CERTIFICADO_MEDICO.HasValue && _excarcelacion.CERTIFICADO_MEDICO.Value == 1)
                        || _excarcelacion.ID_TIPO_EX == 2)
                        && _excarcelacion.CANCELADO_TIPO == "D")
                    {
                        _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_area_medica_cancelacion);
                        _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", _excarcelacion.ID_ANIO, _excarcelacion.ID_IMPUTADO, string.Format("{0:dd/MM/yyyy}", _excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", _excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                        _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido,
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                            ID_CENTRO = id_centro_ubicacion
                        };
                        Context.MENSAJE.Add(_mensaje);
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
        /// metodo que se conecta a la base de datos para seleccionar una serie de registros
        /// </summary>
        /// <param name="centroOrigen">centro origen de la busqueda excarcelacion</param>
        /// <param name="id_tipo_exc_juzgado">id de referencia del tipo de excarcelacion juridicas</param>
        /// <param name="id_tipo_exc_medico">id de referencia del tipo de excarcelacion medicas</param>
        /// <param name="estatus">id de los estatus a checar para la excarcelacion</param>
        /// <param name="anio">anio del folio del imputado</param>
        /// <param name="folio">folio del imputado</param>
        /// <param name="nombre">nombre del imputado</param>
        /// <param name="paterno">apellido paterno del imputado</param>
        /// <param name="materno">apellido materno del imputado</param>
        /// <param name="fecha_inicio">fecha de inicio del rango de la busqueda por fecha de registro</param>
        /// <param name="fecha_final">fecha final del rango de la busqueda por fecha de registro</param>
        /// <param name="id_tipo_exc">id del tipo de excarcelacion a buscar</param>
        /// <param name="id_juzgado">id juzgado para busqueda de excarcelaciones juridicas</param>
        /// <param name="id_fuero">id del fuero del juzgado para busqueda de excarcelaciones juridicas</param>
        /// <param name="id_municipio">id del municipio del juzgado para busqueda de excarcelaciones juridicas</param>
        /// <param name="id_estado">id del estado del juzgado para busqueda de excarcelaciones juridicas</param>
        /// <param name="id_pais">id del pais del juzgado para busqueda de excarcelaciones juridicas</param>
        /// <param name="id_hospital">id del hospital para busqueda de excarcelaciones medicas</param>
        /// <param name="otro_hospital">otro hospital para busqueda de excarcelaciones medicas</param>
        /// <returns>IQueryable&lt;EXCARCELACION&gt;</returns>
        public IQueryable<EXCARCELACION> Seleccionar(short centroOrigen, short id_tipo_exc_juzgado, short id_tipo_exc_medico, List<string> estatus = null, short? anio = null,
            int? folio = null, string nombre = "", string paterno = "", string materno = "", DateTime? fecha_inicio = null, DateTime? fecha_final = null, short? id_tipo_exc = null,
            short? id_juzgado = null, string id_fuero = "", short? id_municipio = null, short? id_estado = null, short? id_pais = null, short? id_hospital = null, string otro_hospital = "")
        {
            try
            {
                var predicateOr = PredicateBuilder.False<EXCARCELACION>();
                var predicate = PredicateBuilder.True<EXCARCELACION>();
                if (estatus != null && estatus.Count > 0)
                {
                    foreach (var item in estatus)
                        predicateOr = predicateOr.Or(w => w.ID_ESTATUS == item);
                    predicate = predicate.And(predicateOr.Expand());
                }
                if (anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == anio.Value);
                if (folio.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == folio.Value);
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(materno));
                if (fecha_inicio.HasValue)
                    predicate = predicate.And(w => w.PROGRAMADO_FEC >= fecha_inicio.Value);
                if (fecha_final.HasValue)
                    predicate = predicate.And(w => w.PROGRAMADO_FEC <= fecha_final.Value);
                if (id_tipo_exc.HasValue)
                {
                    predicate = predicate.And(w => w.ID_TIPO_EX == id_tipo_exc.Value);
                    if (id_tipo_exc == id_tipo_exc_juzgado)
                    {
                        if (id_juzgado.HasValue)
                        {
                            predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.ID_JUZGADO == id_juzgado.Value));
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(id_fuero))
                                predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.JUZGADO.ID_FUERO == id_fuero));
                            if (id_municipio.HasValue)
                                predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.JUZGADO.ID_MUNICIPIO == id_municipio));
                            else if (id_estado.HasValue)
                                predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.JUZGADO.MUNICIPIO.ID_ENTIDAD == id_estado));
                            else if (id_pais.HasValue)
                                predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.JUZGADO.MUNICIPIO.ENTIDAD.ID_PAIS_NAC == id_pais.Value));
                        }
                    }
                    else if (id_tipo_exc == id_tipo_exc_medico && id_hospital.HasValue)
                    {
                        predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a2 => a2.ID_HOSPITAL == id_hospital.Value)));
                        if (!string.IsNullOrWhiteSpace(otro_hospital))
                            predicate = predicate.And(w => w.EXCARCELACION_DESTINO.Any(a => a.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any(a2 => a2.HOSPITAL_OTRO == otro_hospital)));
                    }
                }

                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == centroOrigen);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para seleccionar una serie de registros
        /// </summary>
        /// <param name="centroOrigen">centro origen de la busqueda excarcelacion</param>
        /// <param name="estatus">id de los estatus a checar para la excarcelacion</param>
        /// <param name="anio">anio del folio del imputado</param>
        /// <param name="folio">folio del imputado</param>
        /// <param name="nombre">nombre del imputado</param>
        /// <param name="paterno">apellido paterno del imputado</param>
        /// <param name="materno">apellido materno del imputado</param>
        /// <param name="id_tipo_exc">id del tipo de excarcelacion a buscar</param>
        /// <param name="fecha_inicio">fecha de inicio del rango de busqueda</param>
        /// <param name="fecha_final">fecha de final del rango de busqueda</param>
        /// <param name="cancelado_tipo">Quien cancela la excarcelacion, Juridico "J" o Director/Subdirector "D"</param>
        /// <returns>IQueryable&lt;EXCARCELACION&gt;</returns>
        public IQueryable<EXCARCELACION> Seleccionar(short centroOrigen, List<string> estatus = null, short? anio = null,
            int? folio = null, string nombre = "", string paterno = "", string materno = "", short? id_tipo_exc = null, DateTime? fecha_inicio = null, DateTime? fecha_final = null, string cancelado_tipo = "")
        {
            try
            {
                var predicateOr = PredicateBuilder.False<EXCARCELACION>();
                var predicate = PredicateBuilder.True<EXCARCELACION>();
                if (estatus != null && estatus.Count > 0)
                {
                    foreach (var item in estatus)
                        predicateOr = predicateOr.Or(w => w.ID_ESTATUS == item);
                    predicate = predicate.And(predicateOr.Expand());
                }
                if (estatus.Any(w => w == "CA") && !string.IsNullOrWhiteSpace(cancelado_tipo))
                    predicate = predicate.And(w => w.CANCELADO_TIPO == cancelado_tipo || w.CANCELADO_TIPO == null || w.CANCELADO_TIPO.Trim() == "");
                if (anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == anio.Value);
                if (folio.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == folio.Value);
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(materno));
                if (fecha_inicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.PROGRAMADO_FEC) >= EntityFunctions.TruncateTime(fecha_inicio.Value));
                if (fecha_final.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.PROGRAMADO_FEC) <= EntityFunctions.TruncateTime(fecha_final.Value));
                if (id_tipo_exc.HasValue)
                {
                    predicate = predicate.And(w => w.ID_TIPO_EX == id_tipo_exc.Value);
                }

                predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == centroOrigen);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// metodo para autorizar una excarcelación
        /// </summary>
        /// <param name="excarcelacion">entidad excarcelacion a autorizar</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        /// <param name="id_mensaje">llave del mensaje para enviar durante autorizacion de excarcelacion</param>
        /// <param name="id_mensaje_area_medica">llave del mensaje para enviar al area durante autorizacion de excarcelacion</param>
        /// <param name="id_centro_ubicacion">ID del centro donde se realiza la autorizacion</param>
        /// <param name="id_otro_hospital">Referencia al ID del elemento Otros Hospitales en el catalogo de HOSPITALES</param>
        public void Autorizar(EXCARCELACION excarcelacion, DateTime fecha_servidor, int id_mensaje, int id_mensaje_area_medica, short id_otro_hospital, short id_centro_ubicacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _destinos = new List<string>();
                    foreach (var item in excarcelacion.EXCARCELACION_DESTINO)
                    {
                        Context.Entry(item).State = EntityState.Modified;
                        if (excarcelacion.ID_TIPO_EX == 1)
                            _destinos.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                        else if (excarcelacion.ID_TIPO_EX == 2)
                        {
                            var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item.ID_INTERSOL);
                            if (_solicitud_interconsulta != null)
                                _destinos.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ? _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                                    : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                        }

                    }

                    Context.Entry(excarcelacion).State = EntityState.Modified;
                    MENSAJE_TIPO _mensaje_tipo = null;
                    _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje);
                    var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == excarcelacion.ID_ANIO && w.ID_CENTRO == excarcelacion.ID_CENTRO && w.ID_IMPUTADO == excarcelacion.ID_IMPUTADO);
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty);
                    var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                    var _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", excarcelacion.ID_ANIO, excarcelacion.ID_IMPUTADO,
                        string.Format("{0:dd/MM/yyyy}", excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                    if (_destinos.Count > 0)
                        _contenido += "\nDESTINOS:\n";
                    foreach (var item in _destinos)
                        _contenido += (item + "\n");
                    var _mensaje = new MENSAJE
                    {
                        CONTENIDO = _contenido,
                        ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                        ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                        ID_MENSAJE = _id_mensaje,
                        REGISTRO_FEC = fecha_servidor,
                        ID_CENTRO = id_centro_ubicacion
                    };
                    Context.MENSAJE.Add(_mensaje);
                    Context.SaveChanges();
                    if (excarcelacion.CERTIFICADO_MEDICO.HasValue && excarcelacion.CERTIFICADO_MEDICO.Value == 1)
                    {
                        _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_area_medica);
                        _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == excarcelacion.ID_ANIO && w.ID_CENTRO == excarcelacion.ID_CENTRO && w.ID_IMPUTADO == excarcelacion.ID_IMPUTADO);
                        _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty);
                        _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", excarcelacion.ID_ANIO, excarcelacion.ID_IMPUTADO,
                            string.Format("{0:dd/MM/yyyy}", excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                        _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido,
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                            ID_CENTRO = id_centro_ubicacion
                        };
                        Context.MENSAJE.Add(_mensaje);
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

        public IQueryable<EXCARCELACION> ObtenerTodos(DateTime? FechaEntrada = null, DateTime? FechaSalida = null, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION>();
                if (FechaEntrada != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= FechaEntrada);
                if (FechaSalida != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                return GetData(predicate.Expand()).OrderBy(w => w.REGISTRO_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<EXCARCELACION> ObtenerHuellasTodos(DateTime? FechaEntrada = null, DateTime? FechaSalida = null, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION>();
                if (FechaEntrada != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= FechaEntrada);
                if (FechaSalida != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO.HasValue && w.INGRESO.ID_UB_CENTRO.Value == Centro);
                return GetData(predicate.Expand()).OrderBy(w => w.REGISTRO_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Cancela todas las excarcelaciones PROGRAMADAS y AUTORIZADAS de un ingreso
        /// </summary>
        /// <param name="id_anio">ID_ANIO del ingreso</param>
        /// <param name="id_centro">ID_CENTRO del ingreso</param>
        /// <param name="id_imputado">ID_IMPUTADO del ingreso</param>
        /// <param name="id_ingreso">ID_INGRESO del ingreso</param>
        /// <param name="fecha_servidor">Fecha dek servidor</param>
        /// <param name="id_mensaje_cancelacion_excarcelacion">ID del tipo de mensaje para cancelacion de excarcelacion</param>
        /// <param name="id_otro_hospital">ID del catalogo de HOSPITALES designado para otros</param>

        public void CancelarExcarcelacionesPorImputado(short id_anio, short id_centro, int id_imputado, short id_ingreso, DateTime fecha_servidor, short id_mensaje_cancelacion_excarcelacion, short id_otro_hospital)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _destinos = new List<string>();
                    var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_cancelacion_excarcelacion);
                    var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == id_anio && w.ID_CENTRO == id_centro && w.ID_IMPUTADO == id_imputado);
                    var _nombre_completo = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrWhiteSpace(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrWhiteSpace(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty);
                    foreach (var item in Context.EXCARCELACION.Where(w => w.ID_ANIO == id_anio && w.ID_CENTRO == id_centro && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso && (w.ID_ESTATUS == "PR" || w.ID_ESTATUS == "AU")))
                    {
                        _destinos.Clear();
                        item.ID_ESTATUS = "CA";
                        Context.Entry(item).Property(w => w.ID_ESTATUS).IsModified = true;
                        foreach (var item_destino in item.EXCARCELACION_DESTINO)
                        {
                            item_destino.ID_ESTATUS = "CA";
                            Context.Entry(item_destino).Property(w => w.ID_ESTATUS).IsModified = true;
                            if (item.ID_TIPO_EX == 1)
                                _destinos.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item_destino.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item_destino.ID_JUZGADO).DESCR);
                            else if (item.ID_TIPO_EX == 2)
                            {
                                var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item_destino.ID_INTERSOL);
                                if (_solicitud_interconsulta != null)
                                    _destinos.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ? _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                                        : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                                // _destinos.Add(Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item_destino.ID_HOSPITAL) == null ? string.Empty : item_destino.ID_HOSPITAL == id_otro_hospital ? item_destino.HOSPITAL_OTRO : Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item_destino.ID_HOSPITAL).DESCR);
                            }

                        }
                        if (_mensaje_tipo != null)
                        {
                            var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            var _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", item.ID_ANIO, item.ID_IMPUTADO,
                                string.Format("{0:dd/MM/yyyy}", item.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", item.PROGRAMADO_FEC.Value.TimeOfDay));
                            if (_destinos.Count > 0)
                                _contenido += "\nDESTINOS:\n";
                            foreach (var item_destinos_descr in _destinos)
                                _contenido += (item_destinos_descr + "\n");
                            Context.MENSAJE.Add(new MENSAJE
                            {
                                CONTENIDO = _contenido,
                                ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                                ID_CENTRO = item.INGRESO.ID_UB_CENTRO,
                                ID_ENTIDAD = item.INGRESO.CENTRO1.ID_ENTIDAD,
                                ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                ID_MUNICIPIO = item.INGRESO.CENTRO1.ID_MUNICIPIO,
                                REGISTRO_FEC = fecha_servidor
                            });
                            Context.SaveChanges();
                        }
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

        public IQueryable<EXCARCELACION> ObtenerTodosExc(DateTime? FechaEntrada = null, DateTime? FechaSalida = null, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION>();
                if (FechaEntrada != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.PROGRAMADO_FEC) >= FechaEntrada);
                if (FechaSalida != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.PROGRAMADO_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                return GetData(predicate.Expand()).OrderBy(w => w.PROGRAMADO_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
