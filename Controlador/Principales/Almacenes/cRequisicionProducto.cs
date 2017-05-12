using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using SSP.Servidor.ModelosExtendidos;
using System.Collections.ObjectModel;
using LinqKit;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cRequisicionProducto:EntityManagerServer<REQUISICION_PRODUCTO>
    {
        public IQueryable<EXT_Requisicion_Producto>Seleccionar(int id_requisicion)
        {
            try
            {
                return GetData(w => w.ID_REQUISICION == id_requisicion).Select(s=>new EXT_Requisicion_Producto(){
                    ID_PRODUCTO=s.ID_PRODUCTO,
                    CANTIDAD=s.CANTIDAD,
                    ID_REQUISICION=s.ID_REQUISICION,
                    IS_SELECTED=false,
                    NOMBRE_PRODUCTO=s.PRODUCTO.NOMBRE 
                });

            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<EXT_Requisicion_Producto> SeleccionarNoAsignadoCentro(int id_requisicion)
        {
            try
            {
                return GetData(w => w.ID_REQUISICION == id_requisicion && w.REQUISICION.REQUISICION_CENTRO.Any(a=>a.REQUISICION_CENTRO_PRODUCTO.Any(ac=>ac.ID_PRODUCTO==w.ID_PRODUCTO && ac.ESTATUS=="PA"))).Select(s => new EXT_Requisicion_Producto()
                {
                    ID_PRODUCTO = s.ID_PRODUCTO,
                    CANTIDAD = s.CANTIDAD,
                    ID_REQUISICION = s.ID_REQUISICION,
                    IS_SELECTED = false,
                    NOMBRE_PRODUCTO = s.PRODUCTO.NOMBRE
                });

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<EXT_Requisicion_Producto> SeleccionarAsignadosYPosibles(int id_requisicion, int id_orden_compra)
        {
            try
            {
                return GetData(w => w.ID_REQUISICION == id_requisicion && 
                    (w.REQUISICION.REQUISICION_CENTRO.Any(a => a.REQUISICION_CENTRO_PRODUCTO.Any(ac => ac.ID_PRODUCTO == w.ID_PRODUCTO && ac.ESTATUS == "PA")) ||
                    w.REQUISICION.ORDEN_COMPRA.Any(a=>a.ID_ORDEN_COMPRA==id_orden_compra && a.ORDEN_COMPRA_DETALLE.Any(aOCD=>aOCD.ID_ORDEN_COMPRA==a.ID_ORDEN_COMPRA && aOCD.ID_PRODUCTO==w.ID_PRODUCTO)))).Select(s => new EXT_Requisicion_Producto()
                {
                    ID_PRODUCTO = s.ID_PRODUCTO,
                    CANTIDAD = s.CANTIDAD,
                    ID_REQUISICION = s.ID_REQUISICION,
                    IS_SELECTED = false,
                    NOMBRE_PRODUCTO = s.PRODUCTO.NOMBRE
                });

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public List<EXT_Requisicion_Producto_Seleccionado> SeleccionadosPorBusquedaRequisicionGeneral(ObservableCollection<EXT_Requisicion_Producto> lista_productos)
        {
            try
            {
                if (lista_productos == null)
                    return new List<EXT_Requisicion_Producto_Seleccionado>();

                var predicate = PredicateBuilder.False<REQUISICION_PRODUCTO>();
                foreach (var item in lista_productos)
                    if (item.IS_SELECTED)
                        predicate = predicate.Or(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ID_REQUISICION == item.ID_REQUISICION);
                var productos_seleccionados = GetData(predicate.Expand()).AsEnumerable().Select(s => new EXT_Requisicion_Producto_Seleccionado {
                    CANTIDAD=s.CANTIDAD,
                    ID_PRODUCTO=s.ID_PRODUCTO,
                    ID_REQUISICION=s.ID_REQUISICION,
                    NOMBRE_PRODUCTO=s.PRODUCTO.NOMBRE,
                    DETALLE_CENTRO_PRODUCTO = new List<EXT_Requisicion_Centro_Producto>()
                }).ToList();
                foreach(var item in productos_seleccionados)
                {
                    var lista = Context.REQUISICION_CENTRO_PRODUCTO.Where(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.REQUISICION_CENTRO.ID_REQ_CONCENTRA == item.ID_REQUISICION)
                        .Select(s => new EXT_Requisicion_Centro_Producto
                        {
                            CANTIDAD = s.CANTIDAD,
                            CENTRO = s.REQUISICION_CENTRO.ALMACEN.CENTRO.DESCR,
                            ID_PRODUCTO = s.ID_PRODUCTO,
                            ID_REQUISICION = s.ID_REQUISICION,
                            ID_ALMACEN=s.REQUISICION_CENTRO.ID_ALMACEN
                        }).ToList();
                    item.DETALLE_CENTRO_PRODUCTO.AddRange(lista);
                }
                return productos_seleccionados;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public List<EXT_Requisicion_Producto_Seleccionado> SeleccionadosPorOrdenDeCompra(ObservableCollection<EXT_Requisicion_Producto> lista_productos)
        {
            try
            {
                if (lista_productos == null)
                    return new List<EXT_Requisicion_Producto_Seleccionado>();

                var predicate = PredicateBuilder.False<REQUISICION_PRODUCTO>();
                foreach (var item in lista_productos)
                    if (item.IS_SELECTED)
                        predicate = predicate.Or(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ID_REQUISICION == item.ID_REQUISICION);
                var productos_seleccionados = GetData(predicate.Expand()).AsEnumerable().Select(s => new EXT_Requisicion_Producto_Seleccionado
                {
                    CANTIDAD = s.CANTIDAD,
                    ID_PRODUCTO = s.ID_PRODUCTO,
                    ID_REQUISICION = s.ID_REQUISICION,
                    NOMBRE_PRODUCTO = s.PRODUCTO.NOMBRE,
                    DETALLE_CENTRO_PRODUCTO = new List<EXT_Requisicion_Centro_Producto>()
                }).ToList();
                foreach (var item in productos_seleccionados)
                {
                    var lista = Context.V_REQ_PRODUCTOS.Where(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.OC_REQ == item.ID_REQUISICION).Select(s => new EXT_Requisicion_Centro_Producto {
                        CANTIDAD=(int)s.CANTIDAD_CENTRO.Value,
                        CENTRO=s.CENTRO_DESCR,
                        ID_PRODUCTO=s.ID_PRODUCTO,
                        ID_REQUISICION=s.ID_REQUISICION,
                        ID_ALMACEN=s.ID_ALMACEN,
                        PRECIO=s.PRECIO_COMPRA
                    });
                    item.PRECIO_UNITARIO=lista.First().PRECIO;
                    item.DETALLE_CENTRO_PRODUCTO.AddRange(lista);
                }
                return productos_seleccionados;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
