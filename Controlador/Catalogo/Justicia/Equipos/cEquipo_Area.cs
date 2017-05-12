using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEquipo_Area:EntityManagerServer<EQUIPO_AREA>
    {
        public IQueryable<EQUIPO_AREA> Seleccionar(string IP, string MAC_ADDRESS)
        {
            try
            {
                return GetData(w => w.IP == IP && w.MAC_ADDRESS == MAC_ADDRESS);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Guardar(string IP,string MAC,List<EQUIPO_AREA> Lista)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var areas = Context.EQUIPO_AREA.Where(w => w.IP.Trim() == IP && w.MAC_ADDRESS.Trim() == MAC);
                    if (areas != null)
                    {
                        foreach (var a in areas)
                        {
                            Context.EQUIPO_AREA.Remove(a);
                        }
                    }

                    if (Lista != null)
                    { 
                        foreach(var l in Lista)
                        {
                            Context.EQUIPO_AREA.Add(l);
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

    }
}
