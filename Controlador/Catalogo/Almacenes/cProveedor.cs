using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cProveedor:EntityManagerServer<PROVEEDOR>
    {
        public IQueryable<PROVEEDOR>Seleccionar(string busqueda="",string grupo="",bool solo_activos=false)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROVEEDOR>();
                if (!string.IsNullOrEmpty(busqueda.Trim()))
                    predicate = predicate.And(w => w.NOMBRE == busqueda || w.RAZON_SOCIAL == busqueda);
                if (!string.IsNullOrEmpty(grupo))
                    predicate = predicate.And(w => w.ID_PROD_GRUPO == grupo);
                if (solo_activos)
                    predicate = predicate.And(w => w.ACTIVO == "S");
                return GetData(predicate.Expand()).OrderBy(o => o.NOMBRE).ThenBy(o => o.RAZON_SOCIAL);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
