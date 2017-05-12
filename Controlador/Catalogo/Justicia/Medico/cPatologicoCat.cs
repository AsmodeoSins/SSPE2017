using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPatologicoCat : EntityManagerServer<PATOLOGICO_CAT>
    {
        public cPatologicoCat() { }

        public IQueryable<PATOLOGICO_CAT> ObtenerTodo(string buscar = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(buscar))
                    return GetData().Where(w => w.DESCR.Contains(buscar)).OrderBy(w => w.DESCR);
                else
                    return GetData().OrderBy(w => w.DESCR);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public void Insertar(PATOLOGICO_CAT entidad, List<short> gpos_vulnerables = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _id = GetIDProceso<int>("PATOLOGICO_CAT", "ID_PATOLOGICO", "1=1");
                    entidad.ID_PATOLOGICO = _id;
                    if (gpos_vulnerables != null && gpos_vulnerables.Count > 0)
                    {
                        foreach (var item in gpos_vulnerables)
                        {
                            entidad.SECTOR_CLASIFICACION.Add(Context.SECTOR_CLASIFICACION.First(w => w.ID_SECTOR_CLAS == item));
                        }
                    }
                    Context.PATOLOGICO_CAT.Add(entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void Editar(PATOLOGICO_CAT entidad, List<short> gpos_vulnerables = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _patologico = Context.PATOLOGICO_CAT.First(w => w.ID_PATOLOGICO == entidad.ID_PATOLOGICO);
                   
                    if (gpos_vulnerables != null && gpos_vulnerables.Count > 0)
                    {
                        _patologico.SECTOR_CLASIFICACION.Clear();
                        Context.SaveChanges();

                        foreach (var item in gpos_vulnerables)
                        {
                            _patologico.SECTOR_CLASIFICACION.Add(Context.SECTOR_CLASIFICACION.First(w => w.ID_SECTOR_CLAS == item));
                        }
                    }
                    _patologico.ESTATUS = entidad.ESTATUS;
                    _patologico.DESCR = entidad.DESCR;
                    _patologico.PUEDE_CURARSE = entidad.PUEDE_CURARSE;
                    Context.SaveChanges();
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