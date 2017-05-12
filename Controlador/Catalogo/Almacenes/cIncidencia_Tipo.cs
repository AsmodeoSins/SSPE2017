using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cIncidencia_Tipo:EntityManagerServer<INCIDENCIA_TIPO>
    {
        public IQueryable<INCIDENCIA_TIPO>Seleccionar(string id_almacen_grupo,string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<INCIDENCIA_TIPO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                predicate = predicate.And(w => w.ID_ALMACEN_GRUPO == id_almacen_grupo);
                return GetData(predicate.Expand());
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
