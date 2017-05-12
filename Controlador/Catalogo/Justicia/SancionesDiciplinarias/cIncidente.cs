using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
using System.Data;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cIncidente : EntityManagerServer<INCIDENTE>
    {
        #region Constructor
        public cIncidente() { }
        #endregion
        #region Obtener

        public T ObtenerConsecutivo<T>(short IdCentro, short IdAnio, int IdImputado, short IdIngreso) where T : struct
        {
            var max = GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_ANIO == IdAnio && w.ID_IMPUTADO == IdImputado && w.ID_INGRESO == IdIngreso).Max(m => (int?)(new { m.ID_CENTRO, m.ID_ANIO, m.ID_IMPUTADO, m.ID_INGRESO, m.ID_INCIDENTE }.ID_INCIDENTE));
            return (T)Convert.ChangeType(max.HasValue ? ++max : 1, typeof(T));
        }



        public IQueryable<INCIDENTE> ObtenerTodas(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? Incidente = null)
        {
            var predicate = PredicateBuilder.True<INCIDENTE>();
            if (Centro.HasValue)
                predicate = predicate.And(w => w.ID_CENTRO == Centro);
            if (Anio.HasValue)
                predicate = predicate.And(w => w.ID_ANIO == Anio);
            if (Imputado.HasValue)
                predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
            if (Ingreso.HasValue)
                predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
            if (Incidente.HasValue)
                predicate = predicate.And(w => w.ID_INCIDENTE == Incidente);
            return GetData(predicate.Expand());
            //return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso);
        }

        public INCIDENTE ObtenerIncidenteAtencionMedica(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null,int? AtencionMedica = null,short? CentroUbicacion = null)
        {
            var predicate = PredicateBuilder.True<INCIDENTE>();
            if (Centro.HasValue)
                predicate = predicate.And(w => w.ID_CENTRO == Centro);
            if (Anio.HasValue)
                predicate = predicate.And(w => w.ID_ANIO == Anio);
            if (Imputado.HasValue)
                predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
            if (Ingreso.HasValue)
                predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
            if (AtencionMedica.HasValue)
                predicate = predicate.And(w => w.ID_ATENCION_MEDICA == Ingreso);
            if (CentroUbicacion.HasValue)
                predicate = predicate.And(w => w.ID_CENTRO_UBI == CentroUbicacion);
            return GetData(predicate.Expand()).FirstOrDefault();
            //return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso);
        }
        #endregion
        #region Insertar
        /// <summary>
        /// Inserta en la tabla la entidad enviada.
        /// </summary>
        /// <param name="entidad">Objeto que se desea insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Agregar(INCIDENTE entidad)
        {
            try
            {
                if (Insert(entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public short Insertar(INCIDENTE entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    entidad.ID_INCIDENTE = GetIDProceso<short>("INCIDENTE","ID_INCIDENTE",
                        string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}",
                        entidad.ID_CENTRO,
                        entidad.ID_ANIO,
                        entidad.ID_IMPUTADO,
                        entidad.ID_INGRESO));
                    Context.INCIDENTE.Add(entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return entidad.ID_INCIDENTE;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(short Centro, short Anio, int Imputado, short Ingreso, List<INCIDENTE> Incidente)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    int T, TP, TBD, index = 0;
                    #region Incidentes
                    var incidentes = Context.INCIDENTE.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso).OrderBy(w => w.ID_INCIDENTE);
                    #region Elimina Sanciones
                    var sanciones = Context.SANCION.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso).OrderBy(w => w.ID_INCIDENTE);
                    if (sanciones != null)
                    {
                        foreach (var s in sanciones)
                        {
                            Context.Entry(s).State = EntityState.Deleted;
                        }
                    }
                    Context.SaveChanges();
                    #endregion
                    TP = Incidente.Count();
                    TBD = incidentes.Count();
                    T = TP - TBD;

                    if (T >= 0)// Son Iguales
                    {
                        index = 1;
                        foreach (var p in Incidente)
                        {
                            //Sanciones
                            if (p.SANCION != null)
                                foreach (var s in p.SANCION)
                                {
                                    Context.SANCION.Add(s);
                                }
                            p.SANCION = null;
                            if (T > 0 && index > TBD)//Se Agrega
                            {
                                Context.INCIDENTE.Add(p);
                            }
                            else
                                Context.Entry(p).State = EntityState.Modified;

                            index++;
                        }
                    }
                    if (T < 0)//Se elimina
                    {
                        index = 1;
                        foreach (var p in Incidente)
                        {
                            //Sanciones
                            if (p.SANCION != null)
                                foreach (var s in p.SANCION)
                                {
                                    Context.SANCION.Add(s);
                                }
                            p.SANCION = null;
                            Context.Entry(p).State = EntityState.Modified;

                            index++;
                        }
                        index = 1;
                        foreach (var p in incidentes)
                        {
                            if (index > TP)//Se Elimina
                            {
                                Context.INCIDENTE.Remove(p);
                            }
                            index++;
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }

                //Eliminar(Centro, Anio, Imputado, Ingreso);
                //if (lista.Count == 0)
                //    return true;
                //else
                //    if (Insert(lista))
                //    return true;
                //else
                //    return false;

            }
            catch (Exception ex)
            {
                //throw new ApplicationException(ex.Message);
            }
            return false;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(INCIDENTE Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(INCIDENTE Entidad,List<SANCION> lstSancion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = EntityState.Modified;
                    var lista = Context.SANCION.Where(w => w.ID_CENTRO == Entidad.ID_CENTRO && w.ID_ANIO == Entidad.ID_ANIO && w.ID_IMPUTADO == Entidad.ID_IMPUTADO && w.ID_INCIDENTE == Entidad.ID_INCIDENTE);
                    if (lista != null)
                    {
                        foreach (var l in lista)
                        {
                            Context.SANCION.Remove(l);
                        }
                    }
                    if (lstSancion != null)
                    {
                        foreach (var l in lstSancion)
                        {
                            Context.SANCION.Add(l);
                        }
                    }
                    Context.INCIDENTE.Attach(Entidad);
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

        public bool ActualizarEstatus(INCIDENTE Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = EntityState.Modified;
                    Context.Entry(Entidad).Property(x => x.ESTATUS).IsModified = true;
                    if (Entidad.AUTORIZACION_FEC != null)
                    {
                        Context.Entry(Entidad).Property(x => x.AUTORIZACION_FEC).IsModified = true;
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

        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(INCIDENTE Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = EntityState.Deleted;
                    var lista = Context.SANCION.Where(w => w.ID_CENTRO == Entidad.ID_CENTRO && w.ID_ANIO == Entidad.ID_ANIO && w.ID_IMPUTADO == Entidad.ID_IMPUTADO && w.ID_INCIDENTE == Entidad.ID_INCIDENTE);
                    if (lista != null)
                    {
                        foreach (var l in lista)
                        {
                            Context.SANCION.Remove(l);
                        }
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

        public bool Eliminar(short Centro, short Anio, int Imputado, short Ingreso)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso);
                foreach (var entity in ListEntity)
                {
                    new cSancion().Eliminar(Centro, Anio, Imputado, Ingreso, entity.ID_INCIDENTE);
                    Delete(entity);
                }
                return true;

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
            return false;
        }
        #endregion

        public IEnumerable<cReporteIncidente> ObtenerReporteIncidente(short Centro) 
        {
            try
            {
                string query = "SELECT IT.DESCR INCIDENTE, "+
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '2' OR ING.ID_CLASIFICACION_JURIDICA = 'P')  AND CP.CP_FUERO = 'C' THEN 1 END) PROCESADO_COMUM, "+
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '3' OR ING.ID_CLASIFICACION_JURIDICA = 'S') AND CP.CP_FUERO = 'C' THEN 1 END) SENTENCIADO_COMUN, "+
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '1' OR ING.ID_CLASIFICACION_JURIDICA = 'I') AND (CP.CP_FUERO <> 'C' AND CP.CP_FUERO <> 'F') AND N.ID_NUC IS NOT NULL  THEN 1 END) SIN_FUERO_IMPUTADO, "+
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '2' OR ING.ID_CLASIFICACION_JURIDICA = 'P') AND (CP.CP_FUERO <> 'C' AND CP.CP_FUERO <> 'F') THEN 1 END) SIN_FUERO_PROCESADO "+
                "FROM SSP.INCIDENTE_TIPO IT "+
                "LEFT JOIN SSP.INCIDENTE I ON IT.ID_INCIDENTE_TIPO = I.ID_INCIDENTE_TIPO AND I.ESTATUS = 'A' "+
                "LEFT JOIN SSP.INGRESO ING ON I.ID_CENTRO = ING.ID_CENTRO AND I.ID_ANIO = ING.ID_ANIO AND I.ID_IMPUTADO = ING.ID_IMPUTADO AND I.ID_INGRESO = ING.ID_INGRESO AND ING.ID_UB_CENTRO = {0} AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) "+
                "LEFT JOIN SSP.CAUSA_PENAL CP ON ING.ID_CENTRO = CP.ID_CENTRO AND ING.ID_ANIO = CP.ID_ANIO AND ING.ID_IMPUTADO = CP.ID_IMPUTADO AND ING.ID_INGRESO = CP.ID_INGRESO AND CP.ID_ESTATUS_CP = 1 "+
                "LEFT JOIN SSP.NUC N ON CP.ID_CENTRO = N.ID_CENTRO AND CP.ID_ANIO = N.ID_ANIO AND CP.ID_IMPUTADO = N.ID_IMPUTADO AND CP.ID_INGRESO = N.ID_INGRESO AND CP.ID_CAUSA_PENAL = N.ID_CAUSA_PENAL "+
                "GROUP BY IT.DESCR " +
                "ORDER BY IT.DESCR";
                query = string.Format(query, Centro);
                return Context.Database.SqlQuery<cReporteIncidente>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }


    public class cReporteIncidente
    {
        public string INCIDENTE { get; set; }
        public int PROCESADO_COMUN { get; set; }
        public int SENTENCIADO_COMUN { get; set; }
        public int SIN_FUERO_IMPUTADO { get; set; }
        public int SIN_FUERO_PROCESADO { get; set; }
    }
}
