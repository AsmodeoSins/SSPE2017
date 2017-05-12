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
using System.Data.Objects;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia.Ingreso
{
    public class cIngresoUbicacion : EntityManagerServer<INGRESO_UBICACION>
    {
        #region[CONSTRUCTOR]
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public cIngresoUbicacion()
        { }
        #endregion

        public T ObtenerConsecutivo<T>(int IdCentro, int IdAnio, int IdImputado, int IdIngreso) where T : struct
        {
            var max = GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_ANIO == IdAnio && w.ID_IMPUTADO == IdImputado && w.ID_INGRESO == IdIngreso).Max(m => (int?)(new { m.ID_CENTRO, m.ID_ANIO, m.ID_IMPUTADO, m.ID_INGRESO, m.ID_CONSEC }.ID_CONSEC));
            return (T)Convert.ChangeType(max.HasValue ? ++max : 1, typeof(T));
        }

        /// <summary>
        /// Metodo que se conecta a la BD para insertar un registro
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Insertar(INGRESO_UBICACION Entity, bool Excarcelacion, bool Traslado, int? IdAduana, bool? Entrada = false)
        {
            try
            {
                Entity.ID_CONSEC = ObtenerConsecutivo<int>(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.INGRESO_UBICACION.Add(Entity);

                    if (Excarcelacion)
                    {
                        var excarcelacion = Context.EXCARCELACION.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_ESTATUS == "AU").FirstOrDefault();
                        if (excarcelacion != null)
                        {
                            excarcelacion.ID_ESTATUS = "EP";
                            Context.Entry(excarcelacion).State = EntityState.Modified;
                        }
                    }
                    else if (Traslado)
                    {
                        if (Traslado)
                        {
                            var traslado = Context.TRASLADO_DETALLE.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_ESTATUS == "PR").FirstOrDefault();
                            if (traslado != null)
                            {
                                traslado.ID_ESTATUS = "EP";
                                Context.Entry(traslado).State = EntityState.Modified;
                            }
                        }
                    }
                    else if (IdAduana != null)
                    {
                        var i = new ADUANA_INGRESO();
                        i.ID_ADUANA = IdAduana.Value;
                        i.ID_CENTRO = Entity.ID_CENTRO;
                        i.ID_ANIO = Entity.ID_ANIO;
                        i.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        i.ID_INGRESO = Entity.ID_INGRESO;
                        i.INTERNO_NOTIFICADO = "S";
                        Context.ADUANA_INGRESO.Attach(i);
                        Context.Entry(i).Property(x => x.INTERNO_NOTIFICADO).IsModified = true;
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Insertar(INGRESO_UBICACION Entity)
        {
            try
            {
                Entity.ID_CONSEC = ObtenerConsecutivo<int>(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Ingresa una lista de ingresos en una ubicacion del centro
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        public bool Insertar(List<INGRESO_UBICACION> lista)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (lista != null)
                    {
                        foreach (var l in lista)
                        {
                            l.ID_CONSEC = GetIDProceso<int>("INGRESO_UBICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", l.ID_CENTRO, l.ID_ANIO, l.ID_IMPUTADO, l.ID_INGRESO));
                            Context.INGRESO_UBICACION.Add(l);
                        }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool InsertarNuevaUbicacion(INGRESO_UBICACION Entity, EXCARCELACION EntityExc = null, TRASLADO_DETALLE EntityTra = null, ADUANA_INGRESO EntityAduana_Ingreso = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_CONSEC = ObtenerConsecutivo<int>(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                    Context.INGRESO_UBICACION.Add(Entity);
                    if (EntityExc != null)
                    {
                        Context.EXCARCELACION.Attach(EntityExc);
                        Context.Entry(EntityExc).Property(x => x.ID_ESTATUS).IsModified = true;
                    }
                    else if (EntityTra != null)
                    {
                        Context.TRASLADO_DETALLE.Attach(EntityTra);
                        Context.Entry(EntityTra).Property(x => x.ID_ESTATUS).IsModified = true;
                    }
                    else if (EntityAduana_Ingreso != null)
                    {
                        Context.ADUANA_INGRESO.Attach(EntityAduana_Ingreso);
                        Context.Entry(EntityAduana_Ingreso).Property(x => x.INTERNO_NOTIFICADO).IsModified = true;
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<INGRESO_UBICACION> ObtenerTodos(string buscar = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO_UBICACION>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.ACTIVIDAD.Contains(buscar));
                return GetData(predicate.Expand()).OrderBy(w => w.ID_INGRESO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(INGRESO_UBICACION Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public void CambiarUbicaciones(List<INGRESO_UBICACION> ListaSeleccionados)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (var ubicacion in ListaSeleccionados)
                    {
                        var ultima_ubicacion = Obtener(ubicacion.ID_ANIO, ubicacion.ID_CENTRO, (short)ubicacion.ID_IMPUTADO).OrderByDescending(o => o.MOVIMIENTO_FEC).FirstOrDefault();
                        var consec = ObtenerConsecutivo<int>(ubicacion.ID_CENTRO, ubicacion.ID_ANIO, ubicacion.ID_IMPUTADO, ubicacion.ID_INGRESO);
                        Context.INGRESO_UBICACION.Add(new INGRESO_UBICACION()
                          {
                              ID_CENTRO = ubicacion.ID_CENTRO,
                              ID_ANIO = ubicacion.ID_ANIO,
                              ID_IMPUTADO = ubicacion.ID_IMPUTADO,
                              ID_INGRESO = ubicacion.ID_INGRESO,
                              ID_CONSEC = consec,
                              ID_AREA = ubicacion.ID_AREA,
                              MOVIMIENTO_FEC = ubicacion.MOVIMIENTO_FEC,
                              ACTIVIDAD = ubicacion.ACTIVIDAD,
                              ESTATUS = ubicacion.ESTATUS,
                              ID_CUSTODIO = ubicacion.ID_CUSTODIO
                          });
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

        public IQueryable<INGRESO_UBICACION> Obtener(short ID_ANIO, short ID_CENTRO, short ID_IMPUTADO)
        {
            try
            {
                return GetData(g =>
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_IMPUTADO == ID_IMPUTADO);

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public INGRESO_UBICACION ObtenerUltimaUbicacion(short ID_ANIO, short ID_CENTRO, int ID_IMPUTADO, short ID_INGRESO)
        {
            try
            {
                return GetData(g =>
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_IMPUTADO == ID_IMPUTADO &&
                    g.ID_INGRESO == ID_INGRESO).
                    OrderByDescending(o => o.ID_CONSEC).
                    FirstOrDefault();

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO"</returns>
        public ObservableCollection<INGRESO_UBICACION> ObtenerTodos(short? ID_CENTRO)
        {
            ObservableCollection<INGRESO_UBICACION> internos_ausentes;
            var Resultado = new List<INGRESO_UBICACION>();
            try
            {
                Resultado = GetData(g => g.INGRESO.ID_UB_CENTRO == ID_CENTRO).ToList();

                var query = (from p in Resultado
                             where p.ID_INGRESO ==
                             (from pp in Resultado
                              where pp.ID_IMPUTADO == p.ID_IMPUTADO
                              select pp.ID_INGRESO).Max()
                             select p).ToList();


                var query2 = (from p in query
                             where p.ID_CONSEC ==
                             (from pp in query
                              where pp.ID_IMPUTADO == p.ID_IMPUTADO
                              select pp.ID_CONSEC).Max()
                             select p).ToList();

                internos_ausentes = new ObservableCollection<INGRESO_UBICACION>(query2);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return internos_ausentes;
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO"</returns>
        public IQueryable<INGRESO_UBICACION> ObtenerTodos(short? ID_CENTRO, DateTime? FechaInicio = null, DateTime? FechaFin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO_UBICACION>();
                if (ID_CENTRO != null)
                    predicate = predicate.And(w => w.ID_CENTRO == ID_CENTRO);
                if (FechaInicio != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.MOVIMIENTO_FEC) >= FechaInicio);
                if (FechaFin != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.MOVIMIENTO_FEC) <= FechaFin);
                return GetData(predicate.Expand());
            }
            //ObservableCollection<INGRESO_UBICACION> internos_ausentes;
            //var Resultado = new List<INGRESO_UBICACION>();
            //try
            //{
            //    Resultado = GetData(g => g.ID_CENTRO == ID_CENTRO).ToList();

            //    var query = (from p in Resultado
            //                 where p.ID_CONSEC ==
            //                 (from pp in Resultado
            //                  where pp.ID_IMPUTADO == p.ID_IMPUTADO
            //                  select pp.ID_CONSEC).Max()
            //                 select p).ToList();

            //    internos_ausentes = new ObservableCollection<INGRESO_UBICACION>(query);
            //}
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return internos_ausentes;
        }

        /// <summary>
        /// Metodo que se conecta a la BD para eliminar un registro
        /// </summary>
        /// <param name="Id_centro"></param>
        /// <param name="Id_anio"></param>
        /// <param name="Id_imputado"></param>
        /// <param name="Id_ingreso"></param>
        /// <returns></returns>
        public bool Eliminar(short Id_centro, short Id_anio, int Id_imputado, short Id_ingreso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO == Id_ingreso).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
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


        public bool Eliminar(short Id_centro, short Id_anio, int Id_imputado, short Id_ingreso, short Id_Consec)
        {
            try
            {
                var Entity = GetData().Where(w =>
                    w.ID_CENTRO == Id_centro &&
                    w.ID_ANIO == Id_anio &&
                    w.ID_IMPUTADO == Id_imputado &&
                    w.ID_INGRESO == Id_ingreso &&
                    w.ID_CONSEC == Id_Consec).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
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

        /// <summary>
        /// Función para cambiar el estatus 0 = En Celda, 1 = En Transito, 2 = En Actividad
        /// </summary>
        /// <param name="Centro">Centro del Interno</param>
        /// <param name="Anio">Año del Interno</param>
        /// <param name="Imputado">Imputado del Interno</param>
        /// <param name="Ingreso">Ingreso del Interno</param>
        /// <returns>true = ok, false = error</returns>
        public bool CambiarEstatus(short Centro, short Anio, int Imputado, short Ingreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var iu = Context.INGRESO_UBICACION.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso).OrderByDescending(w => w.ID_CONSEC).FirstOrDefault();
                    if (iu != null)
                    {
                        if (iu.ESTATUS != 0)
                        {
                            iu.ESTATUS = 2;
                            Context.Entry(iu).State = EntityState.Modified;
                            Context.SaveChanges();
                            transaccion.Complete();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }


        /// <summary>
        /// Obtiene los usuarios que se tienen que mover entre las areas del centro
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Edificio"></param>
        /// <param name="Sector"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFinal"></param>
        /// <returns></returns>
        public IEnumerable<cControlInternoEdificio> ObtenerInternosRequeridos(short Centro,short? Edificio = null,short? Sector = null,DateTime? FechaInicio = null,DateTime? FechaFinal = null)
        {
            try
            {
                var query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("T.*,  ");
                query.Append("F.FECHA_ACTIVIDAD,F.HORA_ACTIVIDAD,F.ID_AREA,F.AREA,F.ACTIVIDAD,F.UBICACION_ACTUAL,F.HORA_FINAL_ACTIVIDAD,F.TIPO ");
                query.Append("FROM  ");
                query.Append("( ");
                query.Append("SELECT SSP.FUNC_UBICACION(ING.ID_CENTRO,ING.ID_ANIO,ING.ID_IMPUTADO,ING.ID_INGRESO) UBICACION,ING.ID_CENTRO,ING.ID_ANIO,ING.ID_IMPUTADO,ING.ID_INGRESO, " );
                query.Append("TRIM(IMP.NOMBRE) NOMBRE,TRIM(IMP.PATERNO) PATERNO,TRIM(IMP.MATERNO) MATERNO,ING.ID_UB_CENTRO,ING.ID_UB_EDIFICIO,ING.ID_UB_SECTOR,TRIM(ING.ID_UB_CELDA) ID_UB_CELDA,ING.ID_UB_CAMA,TRIM(E.DESCR) EDIFICIO,TRIM(S.DESCR) SECTOR " );
                query.Append("FROM  ");
                query.Append("SSP.INGRESO ING  " );
                query.Append("INNER JOIN SSP.IMPUTADO IMP ON ING.ID_CENTRO = IMP.ID_CENTRO AND ING.ID_ANIO = IMP.ID_ANIO AND ING.ID_IMPUTADO = IMP.ID_IMPUTADO " );
                query.Append("INNER JOIN SSP.EDIFICIO E ON ING.ID_UB_CENTRO = E.ID_CENTRO AND ING.ID_UB_EDIFICIO = E.ID_EDIFICIO " );
                query.Append("INNER JOIN SSP.SECTOR S ON ING.ID_UB_CENTRO = S.ID_CENTRO AND ING.ID_UB_EDIFICIO = S.ID_EDIFICIO AND ING.ID_UB_SECTOR = S.ID_SECTOR " );
                query.AppendFormat("WHERE ID_UB_CENTRO = {0} ",Centro); 
                if(Edificio.HasValue)
                    query.AppendFormat("AND ID_UB_EDIFICIO =  {0}",Edificio);
                if(Sector.HasValue)
                    query.AppendFormat("AND ID_UB_SECTOR =  {0}", Sector);
                query.Append("AND ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) " );
                query.Append(") T , " );
                //query.Append("TABLE(SSP.FUNC_CONTROL_INTERNOS(T.ID_CENTRO,T.ID_ANIO,T.ID_IMPUTADO,T.ID_INGRESO,to_date( '2016-10-09 15:58:00', 'yyyy-mm-dd hh24:mi:ss' ),to_date( '2016-10-09 15:58:00', 'yyyy-mm-dd hh24:mi:ss' ),T.ID_UB_CENTRO,T.ID_UB_EDIFICIO,T.ID_UB_SECTOR,T.ID_UB_CAMA)) F ");
                query.Append("TABLE(SSP.FUNC_CONTROL_INTERNOS(T.ID_CENTRO,T.ID_ANIO,T.ID_IMPUTADO,T.ID_INGRESO,SYSDATE,SYSDATE,T.ID_UB_CENTRO,T.ID_UB_EDIFICIO,T.ID_UB_SECTOR,T.ID_UB_CAMA)) F ");
                query.Append("WHERE T.UBICACION = 0 ");
                //"WHERE UBICACION = 0 ";
                //query = string.Format(query,Centro);
               return Context.Database.SqlQuery<cControlInternoEdificio>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IEnumerable<cControlInternoEdificio> ObtenerInternosAusentes(short Centro, short? Edificio = null, short? Sector = null, DateTime? FechaInicio = null, DateTime? FechaFinal = null)
        {
            try
            {
                string query = "SELECT " +
                "F.ESTATUS UBICACION,ING.ID_CENTRO,ING.ID_ANIO,ING.ID_IMPUTADO,ING.ID_INGRESO,TRIM(IMP.NOMBRE) NOMBRE,TRIM(IMP.PATERNO) PATERNO,TRIM(IMP.MATERNO) MATERNO," +
                "ING.ID_UB_CENTRO,ING.ID_UB_EDIFICIO,ING.ID_UB_SECTOR,TRIM(ING.ID_UB_CELDA) ID_UB_CELDA,ING.ID_UB_CAMA,TRIM(E.DESCR) EDIFICIO,TRIM(S.DESCR) SECTOR,F.FECHA_ACTIVIDAD,F.FECHA_ACTIVIDAD HORA_ACTIVIDAD,F.ID_AREA,F.AREA,F.ACTIVIDAD " +
                "FROM " +
                "SSP.INGRESO ING " +
                "INNER JOIN SSP.IMPUTADO IMP ON ING.ID_CENTRO = IMP.ID_CENTRO AND ING.ID_ANIO = IMP.ID_ANIO AND ING.ID_IMPUTADO = IMP.ID_IMPUTADO " +
                "INNER JOIN SSP.EDIFICIO E ON ING.ID_UB_CENTRO = E.ID_CENTRO AND ING.ID_UB_EDIFICIO = E.ID_EDIFICIO " +
                "INNER JOIN SSP.SECTOR S ON ING.ID_UB_CENTRO = S.ID_CENTRO AND ING.ID_UB_EDIFICIO = S.ID_EDIFICIO AND ING.ID_UB_SECTOR = S.ID_SECTOR," +
                "TABLE(SSP.FUNC_UBICACION_ACTUAL(ING.ID_CENTRO,ING.ID_ANIO,ING.ID_IMPUTADO,ING.ID_INGRESO)) F " +
                "WHERE ING.ID_UB_CENTRO = {0} AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8)  AND F.ESTATUS > 0";
                query = string.Format(query, Centro);
                return Context.Database.SqlQuery<cControlInternoEdificio>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }

    public class cControlInternoEdificio
    {
        public short UBICACION { get; set;}
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public short? ID_UB_CENTRO { get; set; }
        public short? ID_UB_EDIFICIO { get; set; }
        public short? ID_UB_SECTOR { get; set; }
        public string ID_UB_CELDA { get; set; }
        public short? ID_UB_CAMA { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public DateTime? FECHA_ACTIVIDAD { get; set; }
        public DateTime? HORA_ACTIVIDAD { get; set; }
        public short? ID_AREA { get; set; }
        public string AREA { get; set; }
        public string ACTIVIDAD { get; set; }
        public string UBICACION_ACTUAL {get;set;}
        public DateTime? HORA_FINAL_ACTIVIDAD{get;set;}
        public short? TIPO { get; set; }
    }
}
