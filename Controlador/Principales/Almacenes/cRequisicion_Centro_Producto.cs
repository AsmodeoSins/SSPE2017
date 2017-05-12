using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using SSP.Servidor.ModelosExtendidos;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cRequisicion_Centro_Producto:EntityManagerServer<REQUISICION_CENTRO_PRODUCTO>
    {
        public IQueryable<REQUISICION_CENTRO_PRODUCTO>Seleccionar(int id_requisicion, int id_producto)
        {
            try
            {
                return GetData(w => w.REQUISICION_CENTRO.ID_REQ_CONCENTRA.Value == id_requisicion &&
                    w.ID_PRODUCTO==id_producto).OrderBy(o=>o.REQUISICION_CENTRO.ALMACEN.CENTRO.DESCR).ThenBy(o=>o.PRODUCTO.NOMBRE);
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public List<EXT_Reporte_RequisicionExtraordinaria>SeleccionarReporte_RequisicionExtraordinaria(int id_requisicion)
        {
            try
            {
                return GetData(w => w.REQUISICION_CENTRO.ID_REQUISICION == id_requisicion).Select(s => new EXT_Reporte_RequisicionExtraordinaria {
                    CANTIDAD=s.CANTIDAD,
                    DESCRIPCION=s.PRODUCTO.DESCRIPCION,
                    NOMBRE=s.PRODUCTO.NOMBRE,
                    //PRESENTACION=s.PRODUCTO.PRODUCTO_PRESENTACION.DESCR,
                    UNIDAD_MEDIDA=s.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE,
                    CATEGORIA=s.PRODUCTO.PRODUCTO_CATEGORIA.NOMBRE
                }).ToList();
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}
