using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cCalendarizar_Entrega_Producto:EntityManagerServer<CALENDARIZAR_ENTREGA_PRODUCTO>
    {
        public IQueryable<CALENDARIZAR_ENTREGA_PRODUCTO> Seleccionar(int id_almacen, DateTime dia)
        {
            try
            {
                return GetData(w => w.CALENDARIZAR_ENTREGA.ID_ALMACEN == id_almacen && w.CALENDARIZAR_ENTREGA.FEC_PACTADA == dia);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<CALENDARIZAR_ENTREGA_PRODUCTO> Seleccionar(int id_almacen, int id_orden_compra, DateTime dia)
        {
            try
            {
                return GetData(w => w.CALENDARIZAR_ENTREGA.ID_ALMACEN == id_almacen && w.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA == id_orden_compra && w.CALENDARIZAR_ENTREGA.FEC_PACTADA == dia);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<CALENDARIZAR_ENTREGA_PRODUCTO>SeleccionarFechasEntregaProductoProveedorRestantesMes(int id_almacen, int id_proveedor, int id_producto, int mes)
        {
            try
            {
                

                var fecha_actual = DateTime.Now.Date;

                return GetData(w => w.CALENDARIZAR_ENTREGA.ID_ALMACEN == id_almacen && w.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ID_PROV == id_proveedor && w.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value.Month == mes
                    && w.ID_PRODUCTO == id_producto && w.CALENDARIZAR_ENTREGA.FEC_PACTADA.Value >= fecha_actual && w.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ORDEN_COMPRA_DETALLE.Any(a => a.ID_PRODUCTO == id_producto
                        && a.ID_ORDEN_COMPRA == w.CALENDARIZAR_ENTREGA.ORDEN_COMPRA.ID_ORDEN_COMPRA && a.ORDEN_COMPRA.ID_PROV == id_proveedor));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
           
        }
    }
}
