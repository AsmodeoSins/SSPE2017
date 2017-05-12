using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCamaHospital : EntityManagerServer<CAMA_HOSPITAL>
    {


        public IQueryable<CAMA_HOSPITAL> ObtenerTodos(int Centro = 0)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAMA_HOSPITAL>();
                if (Centro != 0)
                    predicate = predicate.And(a => a.ID_CENTRO == Centro);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<CAMA_HOSPITAL> ObtenerCamasHospitalEstatus(string Estatus, int Centro = 0)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAMA_HOSPITAL>();
                if (Centro != 0)
                    predicate = predicate.And(a => a.ID_CENTRO == Centro);
                predicate = predicate.And(a => a.ESTATUS == Estatus);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public short ObtenerIDCama(int ID_CENTRO)
        {
            try
            {
                return (short)(GetData(g => g.ID_CENTRO == ID_CENTRO).OrderByDescending(o => o.ID_CAMA_HOSPITAL).FirstOrDefault().ID_CAMA_HOSPITAL + 1);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool ActualizarCama(CAMA_HOSPITAL cama_hospital)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.CAMA_HOSPITAL.Attach(cama_hospital);
                    Context.Entry(cama_hospital).Property(x => x.DESCR).IsModified = true;
                    Context.Entry(cama_hospital).Property(x => x.ESTATUS).IsModified = true;
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

        public bool InsertarCama(short ID_CENTRO, CAMA_HOSPITAL cama_hospital)
        {
            try
            {

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.CAMA_HOSPITAL.Add(new CAMA_HOSPITAL()
                    {
                        ID_CAMA_HOSPITAL = GetIDProceso<short>("CAMA_HOSPITAL", "ID_CAMA_HOSPITAL", string.Format("ID_CENTRO = {0}", ID_CENTRO)),//GetIDProceso<short>("CAMA_HOSPITAL", "ID_CAMA_HOSPITAL", "1=1"),
                        ID_CENTRO = ID_CENTRO,
                        DESCR = cama_hospital.DESCR,
                        ID_USUARIO = cama_hospital.ID_USUARIO,
                        REGISTRO_FEC = cama_hospital.REGISTRO_FEC,
                        ESTATUS = cama_hospital.ESTATUS
                    });
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
    }
}
