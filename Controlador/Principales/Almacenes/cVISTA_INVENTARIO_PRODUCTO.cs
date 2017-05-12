using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using SSP.Servidor.ModelosExtendidos;
using LinqKit;

namespace SSP.Controlador.Principales.Almacenes
{
    public class cVista_Inventario_Producto:EntityManagerServer<V_ALMACEN_INVENTARIO>
    {

        //public IQueryable<V_ALMACEN_INVENTARIO> Seleccionar(int id_almacen, string producto_nombre="")
        //{
        //    try
        //    {
        //        var predicate = PredicateBuilder.True<V_ALMACEN_INVENTARIO>();
        //        predicate=predicate.And(w => w.ID_ALMACEN == id_almacen);
        //        if (!string.IsNullOrWhiteSpace(producto_nombre))
        //            predicate = predicate.And(w => w.NOMBRE.Contains(producto_nombre));
        //        return GetData(predicate.Expand());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
        //    }

        //}

        public IQueryable<V_ALMACEN_INVENTARIO> SeleccionarPorProductos(List<int> productos, int? id_almacen=null, int? cantidad_minima=null, string producto_nombre="")
        {
            try
            {
                //var predicate = PredicateBuilder.True<V_ALMACEN_INVENTARIO>();
                //var predicateOR = PredicateBuilder.False<V_ALMACEN_INVENTARIO>();
                //foreach(var item in productos)
                //{
                //    predicateOR = predicateOR.Or(w => w.ID_PRODUCTO == item);
                //}
                //predicate=predicate.And(predicateOR.Expand());
                //if (id_almacen.HasValue)
                //    predicate = predicate.And(w => w.ID_ALMACEN == id_almacen.Value);
                //if (cantidad_minima.HasValue)
                //    predicate = predicate.And(w => w.CANTIDAD >= cantidad_minima.Value);
                //if (!string.IsNullOrWhiteSpace(producto_nombre))
                //    predicate = predicate.And(w => w.NOMBRE.Contains(producto_nombre));
                //return GetData(predicate.Expand()).OrderBy(o => o.DESCR).ThenBy(o => o.NOMBRE);
                return null;
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public List<EXT_V_INVENTARIO_PRODUCTO_B> SeleccionarExtendido(int id_almacen)
        {
            try
            {
               return GetEjecutaQueries<EXT_V_INVENTARIO_PRODUCTO_B>("Select AL.ID_ALMACEN AS ID_ALMACEN, MAX(AL.DESCRIPCION) AS DESCR,P.ID_PRODUCTO AS ID_PRODUCTO ,MAX(P.NOMBRE) AS NOMBRE,"
                    + "MAX(PU.ID_UNIDAD_MEDIDA) AS ID_UNIDAD_MEDIDA,MAX(PU.NOMBRE) AS UNIDAD_MEDIDA, MAX(PP.ID_PRESENTACION) AS ID_PRESENTACION,MAX(PP.DESCR) AS PRESENTACION,COALESCE(SUM(AI.CANTIDAD),0) AS CANTIDAD,"
                    + "(Select COALESCE(SUM (CEP.CANTIDAD),0) FROM SSP.CALENDARIZAR_ENTREGA CE, SSP.CALENDARIZAR_ENTREGA_PRODUCTO CEP WHERE CEP.ID_CALENDARIZACION_ENTREGA=CE.ID_CALENDARIZACION_ENTREGA"
                    + " AND CEP.ID_PRODUCTO=P.ID_PRODUCTO AND CEP.ESTATUS='PR' AND CE.ID_ALMACEN=AL.ID_ALMACEN AND CE.ESTATUS='PR') AS TRANSITO,"
                    + "(SELECT COALESCE(MAX(TP.CANTIDAD),0) FROM SSP.REQUISICION_CENTRO RC,SSP.TRASPASO_PRODUCTO TP,SSP.TRASPASO TR WHERE AL.ID_ALMACEN=RC.ID_ALMACEN"
                    + " AND RC.ID_TIPO=2 AND RC.ESTATUS='AE' AND TR.ID_REQUISICION=RC.ID_REQUISICION AND TR.ID_TIPO=1 AND (TR.ID_ESTATUS<>'RE' AND TR.ID_ESTATUS<>'CO')"
                    + " AND (TR.ID_TRASPASO=TP.ID_TRASPASO AND TP.ID_PRODUCTO=P.ID_PRODUCTO)) AS TRASPASO"
                    + " FROM SSP.ALMACEN AL JOIN SSP.PRODUCTO_TIPO PT ON AL.ID_PRODUCTO_TIPO=PT.ID_ALMACEN_TIPO "
                    + " JOIN SSP.PRODUCTO P ON PT.ID_PRODUCTO=P.ID_PRODUCTO JOIN SSP.PRODUCTO_PRESENTACION PP ON P.ID_PRESENTACION=PP.ID_PRESENTACION "
                    + " JOIN SSP.PRODUCTO_UNIDAD_MEDIDA PU ON P.ID_UNIDAD_MEDIDA=PU.ID_UNIDAD_MEDIDA LEFT JOIN SSP.ALMACEN_INVENTARIO AI ON AL.ID_ALMACEN=AI.ID_ALMACEN "
                    + " AND P.ID_PRODUCTO=AI.ID_PRODUCTO WHERE AL.ACTIVO='S' AND P.ACTIVO='S' AND AL.ID_ALMACEN=" + id_almacen.ToString() + " GROUP BY AL.ID_ALMACEN, P.ID_PRODUCTO").ToList();

                         
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }
    }
}
