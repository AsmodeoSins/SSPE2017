using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
using System.Data.Objects.SqlClient;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cRequisicion:EntityManagerServer<REQUISICION>
    {
        public IQueryable<REQUISICION> Seleccionar(int id_requisicion)
        {
            try
            {
                return GetData(w=>w.ID_REQUISICION==id_requisicion);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<REQUISICION> SeleccionarNoAsignadas(short id_producto_tipo_cat, string producto_grupo = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<REQUISICION>();
                if (id_producto_tipo_cat != -1)
                    predicate = predicate.And(w => w.ID_PRODUCTO_TIPO == id_producto_tipo_cat);
                else
                    if (!string.IsNullOrEmpty(producto_grupo))
                        predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == producto_grupo);
                predicate = predicate.And(w => (w.ESTATUS == "AE" || w.ESTATUS == "AP"));
                return GetData(predicate.Expand()).OrderBy(o => o.ID_REQUISICION);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public IQueryable<REQUISICION> SeleccionarNoAsignadas(short id_producto_tipo_cat, int id_orden_compra, string producto_grupo = null)
        {
            try
            {
                var inner_predicate = PredicateBuilder.True<REQUISICION>();
                if (id_producto_tipo_cat != -1)
                    inner_predicate = inner_predicate.And(w => w.ID_PRODUCTO_TIPO == id_producto_tipo_cat);
                else
                    if (!string.IsNullOrEmpty(producto_grupo))
                        inner_predicate = inner_predicate.And(w => w.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == producto_grupo);
                inner_predicate = inner_predicate.And(w => (w.ESTATUS == "AE" || w.ESTATUS=="AP"));
                var outer_predicate = PredicateBuilder.False<REQUISICION>();
                outer_predicate = outer_predicate.Or(inner_predicate.Expand());
                outer_predicate = outer_predicate.Or(w => w.ORDEN_COMPRA.Any(a => a.ID_ORDEN_COMPRA == id_orden_compra));
                return GetData(outer_predicate.Expand()).OrderBy(o => o.ID_REQUISICION);
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
