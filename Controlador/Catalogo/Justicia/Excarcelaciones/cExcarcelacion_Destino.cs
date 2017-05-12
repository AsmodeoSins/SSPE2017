using LinqKit;
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
    public class cExcarcelacion_Destino:EntityManagerServer<EXCARCELACION_DESTINO>
    {
        /// <summary>
        /// Metodo para seleccionar destinos de excarcelaciones por INGRESO y lista de estatus de la excarcelacion.
        /// </summary>
        /// <param name="id_anio">ID_ANIO del ingreso</param>
        /// <param name="id_centro">ID_CENTRO del ingreso</param>
        /// <param name="id_imputado">ID_IMPUTADO del ingreso</param>
        /// <param name="id_ingreso">ID_INGRESO del ingreso</param>
        /// <param name="estatus">Lista &lt;string&gt; de estatus de la excarcelacion</param>
        /// <returns></returns>
        public IQueryable<EXCARCELACION_DESTINO> Seleccionar(short id_anio, short id_centro, int id_imputado, short id_ingreso, List<string> estatus =null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION_DESTINO>();
                var predicateOr = PredicateBuilder.False<EXCARCELACION_DESTINO>();
                foreach (var item in estatus)
                    predicateOr=predicateOr.Or(w=>w.EXCARCELACION.ID_ESTATUS==item);
                predicate = predicate.And(predicateOr.Expand());
                predicate = predicate.And(w => w.ID_ANIO == id_anio && w.ID_CENTRO == id_centro && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso);
                return GetData(predicate.Expand());

            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        /// <summary>
        /// metodo para actualizar estatus de destinos de excarcelacion
        /// </summary>
        /// <param name="excarcelacion_destino">entidad excarcelacion destino a actualizar</param>
        /// <param name="mensaje_tipo_autorizacion">llave del mensaje para enviar durante autorizacion/cancelacion de excarcelacion</param>
        /// <param name="mensaje_tipo_autorizacion_area_medica">llave del mensaje para enviar durante autorizacion/cancelacion de excarcelacion al area medica</param>
        /// <param name="id_otro_hospital">llave numerica de referencia de otro hospital en catalogo de hospitales</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        public void Actualizar(EXCARCELACION_DESTINO excarcelacion_destino, int mensaje_tipo_autorizacion, int mensaje_tipo_autorizacion_area_medica, short id_otro_hospital, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _mandar_mensaje = false;
                    var _destinos_cancelados = new List<string>();
                    var _destinos = new List<string>();
                    Context.Entry(excarcelacion_destino).State = EntityState.Modified;
                    Context.SaveChanges();
                    var _excarcelacion = Context.EXCARCELACION.FirstOrDefault(w => w.ID_ANIO == excarcelacion_destino.ID_ANIO && w.ID_CENTRO == excarcelacion_destino.ID_CENTRO && w.ID_CONSEC == excarcelacion_destino.ID_CONSEC
                        && w.ID_IMPUTADO == excarcelacion_destino.ID_IMPUTADO && w.ID_INGRESO == excarcelacion_destino.ID_INGRESO);
                    if (_excarcelacion != null)
                    {
                        if (!_excarcelacion.EXCARCELACION_DESTINO.Any(a => a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "AU"))
                        {
                            if (_excarcelacion.EXCARCELACION_DESTINO.Any(a => a.ID_ESTATUS != "CA"))
                                _excarcelacion.ID_ESTATUS = "AU";
                            else
                                _excarcelacion.ID_ESTATUS = "CA";
                            Context.Entry(_excarcelacion).State = EntityState.Modified;
                            Context.SaveChanges();

                            _mandar_mensaje = true;
                            foreach(var item in _excarcelacion.EXCARCELACION_DESTINO)
                            {
                                if (item.ID_ESTATUS == "CA")
                                {
                                    if (_excarcelacion.ID_TIPO_EX == 1)
                                        _destinos_cancelados.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                                    else if (_excarcelacion.ID_TIPO_EX == 2)
                                    {
                                        var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w=>w.ID_INTERSOL==item.ID_INTERSOL);
                                        if (_solicitud_interconsulta != null)
                                            _destinos_cancelados.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ? _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                                                : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                                        //_destinos_cancelados.Add(Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item.ID_HOSPITAL) == null ? string.Empty : item.ID_HOSPITAL == id_otro_hospital ? item.HOSPITAL_OTRO : Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item.ID_HOSPITAL).DESCR); 
                                    }
                                }
                                else
                                {
                                    if (_excarcelacion.ID_TIPO_EX == 1)
                                        _destinos.Add(Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO) == null ? string.Empty : Context.JUZGADO.FirstOrDefault(w => w.ID_JUZGADO == item.ID_JUZGADO).DESCR);
                                    else if (_excarcelacion.ID_TIPO_EX == 2)
                                    {
                                        var _solicitud_interconsulta = Context.INTERCONSULTA_SOLICITUD.FirstOrDefault(w => w.ID_INTERSOL == item.ID_INTERSOL);
                                        if (_solicitud_interconsulta != null)
                                            _destinos.Add(_solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_otro_hospital ? _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                                                : _solicitud_interconsulta.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR);
                                        //_destinos.Add(Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item.ID_HOSPITAL) == null ? string.Empty : item.ID_HOSPITAL == id_otro_hospital ? item.HOSPITAL_OTRO : Context.HOSPITAL.FirstOrDefault(w => w.ID_HOSPITAL == item.ID_HOSPITAL).DESCR); 
                                    }
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("La excarcelación padre de este registro fue eliminada fue eliminada durante el proceso");
                    if (_mandar_mensaje)
                    {
                        var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == mensaje_tipo_autorizacion);
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
                            REGISTRO_FEC = fecha_servidor
                        };
                        Context.MENSAJE.Add(_mensaje);
                        Context.SaveChanges();
                        if (_excarcelacion.CERTIFICADO_MEDICO.HasValue && _excarcelacion.CERTIFICADO_MEDICO.Value==1)
                        {
                            _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == mensaje_tipo_autorizacion_area_medica);
                            _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            _contenido = _mensaje_tipo.CONTENIDO + " PARA EL IMPUTADO " + _nombre_completo + " CON FOLIO " + string.Format("{0}/{1} PARA EL DIA {2} A LAS {3}", _excarcelacion.ID_ANIO, _excarcelacion.ID_IMPUTADO, string.Format("{0:dd/MM/yyyy}", _excarcelacion.PROGRAMADO_FEC.Value.Date), String.Format("{0:t}", _excarcelacion.PROGRAMADO_FEC.Value.TimeOfDay));
                            _mensaje = new MENSAJE
                            {
                                CONTENIDO = _contenido,
                                ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                                ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = fecha_servidor
                            };
                            Context.MENSAJE.Add(_mensaje);
                            Context.SaveChanges();
                        }
                    }
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
