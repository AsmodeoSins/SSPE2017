using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
using LinqKit;
using System.Data.Objects;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cIngresoUbicacionAnterior : EntityManagerServer<INGRESO_UBICACION_ANT>
    {
        public cIngresoUbicacionAnterior() { }

        public IQueryable<INGRESO_UBICACION_ANT> ObtenerTodos(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null,DateTime? FechaInicio = null, DateTime? FechaFin = null) 
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO_UBICACION_ANT>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if(Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(Ingreso.HasValue)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= EntityFunctions.TruncateTime(FechaInicio));
                if (FechaFin.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= EntityFunctions.TruncateTime(FechaFin));
                return GetData(predicate.Expand()).OrderBy(w => new {w.ID_CENTRO,w.ID_ANIO,w.ID_IMPUTADO,w.REGISTRO_FEC });
            }

            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }


        public IQueryable<INGRESO_UBICACION_ANT> ObtenerTodos(short? Centro = null,DateTime? FechaInicio = null, DateTime? FechaFin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO_UBICACION_ANT>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) >= EntityFunctions.TruncateTime(FechaInicio));
                if (FechaFin.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.REGISTRO_FEC) <= EntityFunctions.TruncateTime(FechaFin));
                return GetData(predicate.Expand()).OrderBy(w => w.REGISTRO_FEC);
            }

            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public INGRESO_UBICACION_ANT Obtener(int Id)
        {
            try
            {
                return GetData().SingleOrDefault(s => s.ID_CONSEC == Id);
            }

            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }


        public bool Insertar(INGRESO_UBICACION_ANT Entidad,INGRESO Ingreso,CAMA CamaNueva,CAMA CamaVieja)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entidad.ID_CONSEC = GetIDProceso<short>("INGRESO_UBICACION_ANT", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entidad.ID_CENTRO, Entidad.ID_ANIO, Entidad.ID_IMPUTADO, Entidad.ID_INGRESO));
                    Context.INGRESO_UBICACION_ANT.Add(Entidad);
                    #region Ingreso
                    if (Ingreso != null) {
                        Context.INGRESO.Attach(Ingreso);
                        Context.Entry(Ingreso).Property(x => x.ID_UB_EDIFICIO).IsModified = true;
                        Context.Entry(Ingreso).Property(x => x.ID_UB_SECTOR).IsModified = true;
                        Context.Entry(Ingreso).Property(x => x.ID_UB_CELDA).IsModified = true;
                        Context.Entry(Ingreso).Property(x => x.ID_UB_CAMA).IsModified = true;
                    }
                    #endregion
                    #region Cama Nueva
                    if(CamaNueva != null)
                    {
                        /*
                         ID_CAMA
ID_CELDA
ID_SECTOR
ID_EDIFICIO
ID_CENTRO
                         */
                        var cn = Context.CAMA.Where(w => 
                            w.ID_CENTRO == CamaNueva.ID_CENTRO &&
                            w.ID_EDIFICIO == CamaNueva.ID_EDIFICIO &&
                            w.ID_SECTOR == CamaNueva.ID_SECTOR &&
                            w.ID_CELDA == CamaNueva.ID_CELDA &&
                            w.ID_CAMA == CamaNueva.ID_CAMA).FirstOrDefault();
                        if (cn != null)
                        {
                            cn.ESTATUS = CamaNueva.ESTATUS;
                            Context.Entry(cn).State = EntityState.Modified;
                        }

                        //Context.CAMA.Attach(CamaNueva);
                        //Context.Entry(CamaNueva).Property(x => x.ESTATUS).IsModified = true;
                    }
                    #endregion
                    #region Cama Vieja
                    if (CamaVieja != null)
                    {
                        var cv = Context.CAMA.Where(w =>
                            w.ID_CENTRO == CamaVieja.ID_CENTRO &&
                            w.ID_EDIFICIO == CamaVieja.ID_EDIFICIO &&
                            w.ID_SECTOR == CamaVieja.ID_SECTOR &&
                            w.ID_CELDA == CamaVieja.ID_CELDA &&
                            w.ID_CAMA == CamaVieja.ID_CAMA).FirstOrDefault();
                        if (cv != null)
                        {
                            cv.ESTATUS = CamaVieja.ESTATUS;
                            Context.Entry(cv).State = EntityState.Modified;
                        }
                        //Context.CAMA.Attach(CamaVieja);
                        //Context.Entry(CamaVieja).Property(x => x.ESTATUS).IsModified = true;
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //Entidad.ID_CONSEC = GetIDProceso<short>("INGRESO_UBICACION_ANT", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entidad.ID_CENTRO, Entidad.ID_ANIO, Entidad.ID_IMPUTADO, Entidad.ID_INGRESO));
                //if (Insert(Entidad))
                //    return true;

                //return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool Actualiza(INGRESO_UBICACION_ANT Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.INGRESO_UBICACION_ANT.Attach(Entidad);
                    Context.Entry(Entidad).Property(x => x.MOTIVO_CAMBIO).IsModified = true;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Update(Entidad))
                //    return true;

                //return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool Eliminar(INGRESO_UBICACION_ANT Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;

                return false;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public short GetLastConsecutivo() 
        {
            try
            {
                return GetData().Any() ? GetData().Max(x => x.ID_CONSEC) : new short();
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
