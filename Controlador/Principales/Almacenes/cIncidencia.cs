using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using SSP.Servidor.ModelosExtendidos;
using System.Transactions;
using SSP.Controlador.Enums;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cIncidencia:EntityManagerServer<INCIDENCIA>
    {
        public void Insertar(INCIDENCIA incidencia, List<EXT_RECEPCION_PRODUCTOS_B> recepcion_productos, string _id_usuario, FormaEntradaOrden formaEntradaOrden)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var secuencia_incidencia = GetSequence<int>("INCIDENCIA_SEQ");
                    incidencia.ID_INCIDENCIA = secuencia_incidencia;
                    foreach(var item in recepcion_productos)
                    {
                        if (item.INCIDENCIA_TIPO !=null) //Si el articulo esta marcado como rechazado
                        {
                            incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO {
                                CANTIDAD=item.ORDENADO,
                                ID_ALMACEN_GRUPO=item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                ID_TIPO_INCIDENCIA=item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                ID_PRODUCTO=item.ID_PRODUCTO,
                                OBSERV=item.INCIDENCIA_OBSERVACIONES
                            });
                        
                        }

                        //como todos los articulos se regresan, se eliminan de en transito y se mueven a no en transito.
                        var orden_compra_detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(w => w.ID_ALMACEN == incidencia.ID_ALMACEN && w.ID_ORDEN_COMPRA == incidencia.ID_ORDEN_COMPRA
                               && w.ID_PRODUCTO == item.ID_PRODUCTO);
                        orden_compra_detalle.CANTIDAD_TRANSITO -= item.ORDENADO;
                        orden_compra_detalle.DIFERENCIA += item.ORDENADO;
                    }
                    Context.INCIDENCIA.Add(incidencia);
                    Context.SaveChanges();
                    var incidencia_fecha=incidencia.FECHA.Value.Date;
                    var calendarizar_entrega = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.FEC_PACTADA == incidencia_fecha && w.ID_ALMACEN == incidencia.ID_ALMACEN
                        && w.ID_ORDEN_COMPRA == incidencia.ID_ORDEN_COMPRA);
                    if (formaEntradaOrden==FormaEntradaOrden.EntradaCalendarizada && calendarizar_entrega == null)
                        throw new Exception("La calendarizacion programada de esta entrega fue eliminada durante el proceso. Favor de avisar al administrador");  //si fue una orden calendarizada, aventar excepcion si esta nula
                    if (calendarizar_entrega!=null)
                    {
                        calendarizar_entrega.ESTATUS = "RE"; //aunque se haya rechazado la entrega, el proveedor si llego en fecha
                        //Cambiar estatus de los campos de la calendarizacion del producto
                        foreach (var item in recepcion_productos)
                        {
                            var calendarizar_entrega_producto = calendarizar_entrega.CALENDARIZAR_ENTREGA_PRODUCTO.First(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ESTATUS == "PR");
                            calendarizar_entrega_producto.ESTATUS = "RC"; //como se rechaza toda la orden se tiene que rechazar todos los productos
                            if (item.INCIDENCIA_TIPO != null) //Si el articulo esta marcado con una incidencia
                                calendarizar_entrega_producto.ID_INCIDENCIA = incidencia.ID_INCIDENCIA; //hay que registrarla
                        }
                    }
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
