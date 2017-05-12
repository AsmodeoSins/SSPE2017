using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cOrden_Compra_Detalle:EntityManagerServer<ORDEN_COMPRA_DETALLE>
    {
        public IQueryable<ORDEN_COMPRA_DETALLE> Seleccionar(int id_orden_compra, short? id_almacen=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ORDEN_COMPRA_DETALLE>();
                if (id_almacen.HasValue)
                    predicate = predicate.And(w => w.ID_ALMACEN == id_almacen.Value);
                predicate = predicate.And(w => w.ID_ORDEN_COMPRA == id_orden_compra);
                return GetData(predicate.Expand()).OrderBy(o => o.PRODUCTO.NOMBRE);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
