using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Data;
using System.Text;
using System.Data.Objects;


namespace SSP.Controlador.Catalogo.Justicia
{

    class DatosImputado
    {
        public short Anio;
        public int Folio;
        public string NombreCompleto;
    }


    public class cTraslado : EntityManagerServer<TRASLADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTraslado()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// <param name="anio">anio del folio del imputado</param>
        /// <param name="centro">id centro donde se registro el traslado</param>
        /// <param name="estatus">id del estatus del traslado</param>
        /// <param name="fecha">fecha del traslado a partir de la cual se va realizar la consulta</param>
        /// <param name="imputado">folio del imputado</param>
        /// <param name="materno">ap. materno del imputado</param>
        /// <param name="nombre">nombre del imputado</param>
        /// <param name="origen_tipo">El origen del traslasdo. L=El traslado se origino en el estado, F=El traslado se origino desde otro estado o pais.</param>
        /// <param name="paterno">ap. paterno del imputado</param>
        /// <param name="tipo_traslado_local">El tipo de traslado para los traslado locales, LO (local)=Entre centros, LF (local foraneo)=de un centro estatal hacia un centro ubicado fuera del estado</param>
        /// </summary>
        /// <returns>IQueryable&lt;TRASLADO&gt;</returns>
        public IQueryable<TRASLADO> ObtenerTodos(short? centro = null, List<string> estatus = null, DateTime? fecha = null, string origen_tipo = "", string tipo_traslado_local = "", short? anio = null, int? imputado = null, string nombre = "", string paterno = "", string materno = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<TRASLADO>();
                var predicateOR = PredicateBuilder.False<TRASLADO>();
                if (centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == centro.Value);
                if (estatus != null && estatus.Count > 0)
                {
                    foreach (var item in estatus)
                        predicateOR = predicateOR.Or(w => w.ID_ESTATUS == item);
                    predicate = predicate.And(predicateOR.Expand());
                }
                if (fecha.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO_FEC) == EntityFunctions.TruncateTime(fecha.Value));
                if (!string.IsNullOrWhiteSpace(origen_tipo))
                    predicate = predicate.And(w => w.ORIGEN_TIPO == origen_tipo);
                if (!string.IsNullOrWhiteSpace(tipo_traslado_local))
                    switch (tipo_traslado_local)
                    {
                        case "LO":
                            predicate = predicate.And(w => w.CENTRO_DESTINO.HasValue);
                            break;
                        case "LF":
                            predicate = predicate.And(w => !w.CENTRO_DESTINO.HasValue && w.CENTRO_ORIGEN.HasValue);
                            break;
                    }
                if (anio.HasValue || imputado.HasValue || !string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(paterno) || !string.IsNullOrWhiteSpace(materno))
                {
                    predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.ID_TRASLADO == w.ID_TRASLADO && a.ID_ESTATUS != "CA" && a.ID_ESTATUS != "AU"));
                    if (anio.HasValue)
                        predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.ID_ANIO == anio.Value));
                    if (imputado.HasValue)
                        predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.ID_IMPUTADO == imputado.Value));
                    if (!string.IsNullOrWhiteSpace(nombre))
                        predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.INGRESO.IMPUTADO.NOMBRE.Contains(nombre)));
                    if (!string.IsNullOrWhiteSpace(paterno))
                        predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.INGRESO.IMPUTADO.PATERNO.Contains(paterno)));
                    if (!string.IsNullOrWhiteSpace(materno))
                        predicate = predicate.And(w => w.TRASLADO_DETALLE.Any(a => a.INGRESO.IMPUTADO.MATERNO.Contains(materno)));
                }

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool ModificarResponsable(TRASLADO Traslado)
        {
            try
            {
                Context.TRASLADO.Attach(Traslado);
                Context.Entry(Traslado).Property(x => x.RESPONSABLE).IsModified = true;
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public TRASLADO Obtener(short Centro, int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_TRASLADO == Id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }




        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="_traslado">objeto de tipo "TRASLADO" con valores a insertar</param>
        /// <param name="id_mensaje_tipo">llave numerica del tipo de mensaje para calendarizar traslado</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        public void Insertar(TRASLADO _traslado, int id_mensaje_tipo, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _id_centro = _traslado.CENTRO_ORIGEN.Value;
                    var imputados_mensaje = new List<DatosImputado>();
                    var id = GetSequence<int>("TRASLADO_SEQ");
                    var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo);
                    foreach (var item in _traslado.TRASLADO_DETALLE)
                    {
                        item.ID_TRASLADO = id;
                        var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO);
                        imputados_mensaje.Add(new DatosImputado
                        {
                            Anio = item.ID_ANIO,
                            Folio = item.ID_IMPUTADO,
                            NombreCompleto = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty)
                        });

                    }

                    _traslado.ID_TRASLADO = id;
                    Context.TRASLADO.Add(_traslado);
                    Context.SaveChanges();

                    if (imputados_mensaje != null && imputados_mensaje.Count > 0)
                    {
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        var _contenido = new StringBuilder();
                        _contenido.AppendLine(_mensaje_tipo.CONTENIDO + " CON LOS SIGUIENTES IMPUTADOS A TRASLADAR");

                        foreach (var item in imputados_mensaje)
                        {
                            _contenido.AppendLine(string.Format("{0}/{1}", item.Anio, item.Folio) + " " + item.NombreCompleto);
                        }
                        var _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido.ToString(),
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                            ID_CENTRO = _id_centro
                        };
                        Context.MENSAJE.Add(_mensaje);
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





        public bool FinalizarTraslado(TRASLADO Traslado, INGRESO Ingreso, TRASLADO_DETALLE Traslado_Detalle)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ListaTrasladoInternos = new cTrasladoDetalle().ObtenerTrasladoInternos(Traslado.ID_TRASLADO, Traslado.ID_CENTRO).AsEnumerable();
                    foreach (var Interno in ListaTrasladoInternos)
                    {
                        var cama = new CAMA()
                        {
                            ID_CENTRO = Interno.INGRESO.CAMA.ID_CENTRO,
                            ID_EDIFICIO = Interno.INGRESO.CAMA.ID_EDIFICIO,
                            ID_SECTOR = Interno.INGRESO.CAMA.ID_SECTOR,
                            ID_CELDA = Interno.INGRESO.CAMA.ID_CELDA,
                            ID_CAMA = Interno.INGRESO.CAMA.ID_CAMA,
                            ESTATUS = "S"
                        };
                        Context.CAMA.Attach(cama);
                        Context.Entry(cama).Property(x => x.ESTATUS).IsModified = true;
                    }

                    Context.TRASLADO.Attach(Traslado);
                    Context.Entry(Traslado).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Traslado).Property(x => x.RESPONSABLE).IsModified = Traslado.RESPONSABLE != null;

                    Context.INGRESO.Attach(Ingreso);
                    Context.Entry(Ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;

                    Context.TRASLADO_DETALLE.Attach(Traslado_Detalle);
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.EGRESO_FEC).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;

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

        /// <summary>
        /// metodo que se conecta a la base de datos para cancelar un registro
        /// </summary>
        /// <param name="_traslado">objeto de tipo "TRASLADO" con valores a insertar</param>
        /// <param name="id_mensaje_tipo">llave numerica del tipo de mensaje para cancelar una calendarizacion de traslado</param>
        /// <param name="fecha_servidor">fecha del servidor</param>
        public void Actualizar(TRASLADO _traslado, int id_mensaje_tipo_cancelado, DateTime fecha_servidor)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _id_centro = _traslado.CENTRO_ORIGEN.Value;
                    var imputados_mensaje = new List<DatosImputado>();
                    var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == id_mensaje_tipo_cancelado);

                    foreach (var item in _traslado.TRASLADO_DETALLE.Where(w => w.ID_ESTATUS == "CA"))
                    {
                        Context.Entry(item).State = EntityState.Modified;
                        var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO);
                        imputados_mensaje.Add(new DatosImputado
                        {
                            Anio = item.ID_ANIO,
                            Folio = item.ID_IMPUTADO,
                            NombreCompleto = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty)
                        });

                    }
                    Context.Entry(_traslado).State = EntityState.Modified;
                    Context.SaveChanges();
                    if (imputados_mensaje != null && imputados_mensaje.Count > 0)
                    {
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        var _contenido = new StringBuilder();
                        _contenido.AppendLine(_mensaje_tipo.CONTENIDO + " SE CANCELA EL TRASLADO DE LOS SIGUIENTES IMPUTADOS");

                        foreach (var item in imputados_mensaje)
                        {
                            _contenido.AppendLine(string.Format("{0}/{1}", item.Anio, item.Folio) + " " + item.NombreCompleto);
                        }
                        var _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido.ToString(),
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = fecha_servidor,
                            ID_CENTRO = _id_centro
                        };
                        Context.MENSAJE.Add(_mensaje);
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
    }
}