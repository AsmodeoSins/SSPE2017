using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using System.Transactions;
using System.Data.Objects;

namespace SSP.Controlador.Catalogo.Justicia.Medico
{
    public class cHospitalizacion : EntityManagerServer<HOSPITALIZACION>
    {
        public bool InsertarNuevaHospitalizacion(HOSPITALIZACION Hospitalizacion, CAMA_HOSPITAL Cama)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.CAMA_HOSPITAL.Attach(Cama);
                    Context.Entry(Cama).Property(x => x.ESTATUS).IsModified = true;
                    Context.HOSPITALIZACION.Add(new HOSPITALIZACION()
                    {
                        ID_CAMA_HOSPITAL = Hospitalizacion.ID_CAMA_HOSPITAL,
                        ID_CENTRO = Hospitalizacion.ID_CENTRO,
                        ID_CENTRO_UBI = Hospitalizacion.ID_CENTRO_UBI,
                        ID_HOSEST = Hospitalizacion.ID_HOSEST,
                        ID_HOSPITA = Hospitalizacion.ID_HOSPITA,
                        ID_INGHOSTIP = Hospitalizacion.ID_INGHOSTIP,
                        ID_USUARIO = Hospitalizacion.ID_USUARIO,
                        INGRESO_FEC = Hospitalizacion.INGRESO_FEC,
                        INGRESO_MEDICO = Hospitalizacion.INGRESO_MEDICO,
                        REGISTRO_FEC = Hospitalizacion.REGISTRO_FEC,
                        ID_ATENCION_MEDICA = Hospitalizacion.ID_ATENCION_MEDICA
                    });
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



        public IQueryable<HOSPITALIZACION> ObtenerHospitalizacionesPorFecha(DateTime fecha_busqueda, short centro_ubi)
        {
            try
            {
                return GetData(w => EntityFunctions.TruncateTime(w.INGRESO_FEC) <= EntityFunctions.TruncateTime(fecha_busqueda) && (!w.ALTA_FEC.HasValue || EntityFunctions.TruncateTime(w.ALTA_FEC.Value) >= EntityFunctions.TruncateTime(fecha_busqueda)) && w.ID_CENTRO_UBI == centro_ubi);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<HOSPITALIZACION> ObtenerHospitalizacionesPorIngreso(INGRESO Entity)
        {
            try
            {
                var predicate = PredicateBuilder.True<HOSPITALIZACION>();
                if (Entity != null)
                {
                    predicate = predicate.And(x => x.NOTA_MEDICA != null && x.NOTA_MEDICA.ATENCION_MEDICA != null);
                    predicate = predicate.And(x => x.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == Entity.ID_ANIO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO_UBI == Entity.ID_UB_CENTRO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == Entity.ID_IMPUTADO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == Entity.ID_INGRESO);
                    predicate = predicate.And(x => x.NOTA_MEDICA.HOSPITALIZACION.Any(y => y.ID_HOSEST == 1));
                };

                return GetData(predicate.Expand());
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public IQueryable<HOSPITALIZACION> ObtenerHospitalizacionesPorIngresoSinEstatus(INGRESO Entity)
        {
            try
            {
                var predicate = PredicateBuilder.True<HOSPITALIZACION>();
                if (Entity != null)
                {
                    predicate = predicate.And(x => x.NOTA_MEDICA != null && x.NOTA_MEDICA.ATENCION_MEDICA != null);
                    predicate = predicate.And(x => x.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == Entity.ID_ANIO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO_UBI == Entity.ID_UB_CENTRO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == Entity.ID_IMPUTADO && x.NOTA_MEDICA.ATENCION_MEDICA.ID_INGRESO == Entity.ID_INGRESO);
                };

                return GetData(predicate.Expand());
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }
    }
}