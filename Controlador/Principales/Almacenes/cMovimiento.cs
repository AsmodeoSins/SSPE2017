using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;
using System.Transactions;
using SSP.Servidor.ModelosExtendidos;
using SSP.Controlador.Enums;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cMovimiento:EntityManagerServer<MOVIMIENTO>
    {
        public void Insertar(MOVIMIENTO entrada, List<EXT_RECEPCION_PRODUCTOS_B> recepcion_productos, string _id_usuario, FormaEntradaOrden formaEntradaOrden)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var sequencia_entrada = GetSequence<int>("ENTRADA_SEQ");
                    var entrada_fecha = entrada.FECHA.Value.Date;
                    entrada.ID_MOVIMIENTO = sequencia_entrada;
                    if (entrada.ENTRADA_FACTURA!=null)
                        entrada.ENTRADA_FACTURA.ID_ENTRADA = sequencia_entrada;
                    Context.MOVIMIENTO.Add(entrada);
                    var orden_compra = Context.ORDEN_COMPRA.FirstOrDefault(w => w.ID_ORDEN_COMPRA == entrada.ID_ORDEN_COMPRA);
                    var calendarizar_entrega = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.FEC_PACTADA == entrada_fecha
                        && w.ID_ALMACEN == entrada.ID_ALMACEN && w.ID_ORDEN_COMPRA == entrada.ID_ORDEN_COMPRA);
                    if (formaEntradaOrden==FormaEntradaOrden.EntradaCalendarizada && calendarizar_entrega == null)
                        throw new Exception("La calendarizacion para esta orden fue borrada durante el proceso"); //si la orden a la que se da entrada fue calendarizada hay que aventar excepcion
                    INCIDENCIA incidencia = null;
                    if(calendarizar_entrega!=null)
                    {
                        calendarizar_entrega.ID_ENTRADA = entrada.ID_MOVIMIENTO;
                        calendarizar_entrega.ESTATUS = "RE";
                    }
                    
                    foreach (var item in recepcion_productos)
                    {
                        int seq_recalendarizar=0;
                        int consec_recalendarizacion=0;
                            var orden_compra_detalle = orden_compra.ORDEN_COMPRA_DETALLE.FirstOrDefault(w => w.ID_ALMACEN == entrada.ID_ALMACEN && w.ID_ORDEN_COMPRA == entrada.ID_ORDEN_COMPRA
                                    && w.ID_PRODUCTO == item.ID_PRODUCTO);
                            if (orden_compra_detalle == null)
                                throw new Exception("El detalle de la orden de compra fue eliminado durante este proceso. Favor de contactar al administadror");
                            if (item.RECIBIDO > 0) //si hubo recepcion del producto
                            {
                                if (item.RECEPCION_PRODUCTO_DETALLE.Count > 0) //si fue una recepcion por lotes
                                {
                                    foreach (var item_recepcion in item.RECEPCION_PRODUCTO_DETALLE)
                                    {
                                        Context.MOVIMIENTO_PRODUCTO.Add(new MOVIMIENTO_PRODUCTO
                                        {
                                            CANTIDAD = item_recepcion.RECIBIDO,
                                            ID_MOVIMIENTO = entrada.ID_MOVIMIENTO,
                                            ID_PRODUCTO = item_recepcion.ID_PRODUCTO,
                                            ID_LOTE = item_recepcion.LOTE,
                                            CADUCIDAD_FEC = item_recepcion.FECHA_CADUCIDAD
                                        });
                                        var inventario_producto = Context.ALMACEN_INVENTARIO.FirstOrDefault(f => f.ID_LOTE == item_recepcion.LOTE && f.ID_PRODUCTO == item_recepcion.ID_PRODUCTO
                                            && f.ID_ALMACEN == entrada.ID_ALMACEN);
                                        if (inventario_producto == null)
                                        {
                                            Context.ALMACEN_INVENTARIO.Add(new ALMACEN_INVENTARIO
                                            {
                                                CADUCIDAD_FEC = item_recepcion.FECHA_CADUCIDAD,
                                                CANTIDAD = item_recepcion.RECIBIDO,
                                                ID_ALMACEN = entrada.ID_ALMACEN.Value,
                                                ID_LOTE = item_recepcion.LOTE,
                                                ID_PRODUCTO = item_recepcion.ID_PRODUCTO
                                            });
                                        }
                                        else
                                        {
                                            inventario_producto.CANTIDAD += item_recepcion.RECIBIDO;
                                        }
                                    }
                                        
                                }
                                else //si fue una recepcion normal
                                {
                                    Context.MOVIMIENTO_PRODUCTO.Add(new MOVIMIENTO_PRODUCTO
                                    {
                                        CANTIDAD = item.RECIBIDO,
                                        ID_MOVIMIENTO = entrada.ID_MOVIMIENTO,
                                        ID_LOTE = 0,
                                        ID_PRODUCTO = item.ID_PRODUCTO
                                    });
                                    var inventario_producto = Context.ALMACEN_INVENTARIO.FirstOrDefault(f => f.ID_LOTE == 0 && f.ID_PRODUCTO == item.ID_PRODUCTO
                                            && f.ID_ALMACEN == entrada.ID_ALMACEN);
                                    if (inventario_producto == null)
                                    {
                                        Context.ALMACEN_INVENTARIO.Add(new ALMACEN_INVENTARIO
                                        {
                                            CADUCIDAD_FEC = null,
                                            CANTIDAD = item.RECIBIDO,
                                            ID_ALMACEN = entrada.ID_ALMACEN.Value,
                                            ID_LOTE = 0,
                                            ID_PRODUCTO = item.ID_PRODUCTO
                                        });
                                    }
                                    else
                                    {
                                        inventario_producto.CANTIDAD += item.RECIBIDO;
                                    }
                                }
                                if (item.RESTANTE > 0) //si hubo faltante
                                {
                                    if (incidencia == null) //si no existe ya un registro de incidencia levantada para esta calendarizacion
                                    {
                                        var sequencia_incidencia = GetSequence<int>("INCIDENCIA_SEQ");
                                        var incidencias_producto = new List<INCIDENCIA_PRODUCTO>();
                                        incidencias_producto.Add(new INCIDENCIA_PRODUCTO
                                        {
                                            CANTIDAD = item.RESTANTE,
                                            ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                            ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                            OBSERV = item.INCIDENCIA_OBSERVACIONES
                                        }); // se agrega la primera incidencia detalle de la nueva incidencia
                                        incidencia = new INCIDENCIA
                                        {
                                            ID_INCIDENCIA = sequencia_incidencia,
                                            FECHA = DateTime.Now.Date,
                                            ID_ORDEN_COMPRA = entrada.ID_ORDEN_COMPRA,
                                            ID_USUARIO = _id_usuario,
                                            ID_ALMACEN = entrada.ID_ALMACEN,
                                            INCIDENCIA_PRODUCTO = incidencias_producto
                                        };
                                        Context.INCIDENCIA.Add(incidencia);
                                        Context.SaveChanges();
                                    }
                                    else
                                        incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
                                        {
                                            CANTIDAD = item.RESTANTE,
                                            ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                            ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                            OBSERV = item.INCIDENCIA_OBSERVACIONES
                                        });
                                }

                                if (calendarizar_entrega!=null && item.RESTANTE != 0) // si no se recibio en su totalidad
                                {
                                    if (item.FechaRecalendarizacion.HasValue) //si se va a recalendarizar el restante
                                    {
                                        var recalendarizacion = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.FEC_PACTADA == item.FechaRecalendarizacion.Value
                                            && w.ID_ALMACEN == entrada.ID_ALMACEN && w.ID_ORDEN_COMPRA == entrada.ID_ORDEN_COMPRA);
                                        if (recalendarizacion == null) //si no existe nada en la fecha a recalendarizar
                                        {
                                            seq_recalendarizar = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
                                            consec_recalendarizacion=1;
                                            var calendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>();
                                            calendarizar_entrega_producto.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                            {
                                                CANTIDAD = item.RESTANTE,
                                                ID_PRODUCTO = item.ID_PRODUCTO,
                                                ID_CONSEC=consec_recalendarizacion,
                                                ESTATUS="PR"
                                            });
                                            Context.CALENDARIZAR_ENTREGA.Add(new CALENDARIZAR_ENTREGA
                                            {
                                                CALENDARIZAR_ENTREGA_PRODUCTO = calendarizar_entrega_producto,
                                                ESTATUS = "PR",
                                                FEC_PACTADA = item.FechaRecalendarizacion,
                                                FECHA = DateTime.Now,
                                                ID_USUARIO = _id_usuario,
                                                ID_ALMACEN = entrada.ID_ALMACEN,
                                                ID_CALENDARIZACION_ENTREGA = seq_recalendarizar,
                                                ID_ORDEN_COMPRA = entrada.ID_ORDEN_COMPRA
                                            });
                                            Context.SaveChanges();
                                        }
                                        else  //si ya existe una calendarizacion para esa OC
                                        {
                                            var recalendarizacion_entrega_producto = recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ESTATUS=="PR");
                                            seq_recalendarizar=recalendarizacion.ID_CALENDARIZACION_ENTREGA;
                                            if (recalendarizacion_entrega_producto == null) //si no existe la calendarizacion de ese producto
                                            {
                                                consec_recalendarizacion = recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.Where(w => w.ID_PRODUCTO == item.ID_PRODUCTO).Max(c => c.ID_CONSEC)+1;
                                                recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                                {
                                                    ID_CONSEC=consec_recalendarizacion,
                                                    CANTIDAD = item.RESTANTE,
                                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                                    ESTATUS="PR"
                                                });
                                                Context.SaveChanges();
                                            }
                                            else
                                            {
                                                recalendarizacion_entrega_producto.CANTIDAD += item.RESTANTE;
                                                consec_recalendarizacion=recalendarizacion_entrega_producto.ID_CONSEC;
                                            }
                                                
                                        }
                                    }
                                    else //si no se recalendariza el restante se regresa 
                                    {
                                        orden_compra_detalle.CANTIDAD_TRANSITO -= item.RESTANTE;
                                        orden_compra_detalle.DIFERENCIA += item.RESTANTE;
                                    }
                                    //se mueve la cantidad de transito a entregada
                                    orden_compra_detalle.CANTIDAD_TRANSITO -= item.RECIBIDO;
                                }
                                else 
                                {
                                    if (calendarizar_entrega == null)
                                        orden_compra_detalle.DIFERENCIA -= item.RECIBIDO; //si no hay calendario hay que restarla de la diferencia directamente
                                    else
                                        orden_compra_detalle.CANTIDAD_TRANSITO -= item.RECIBIDO;
                                }   
                                orden_compra_detalle.CANTIDAD_ENTREGADA += item.RECIBIDO;
                            }
                            else //si no hubo recepcion de ese producto dentro de la entrada
                            {
                                if (incidencia == null) //si no existe ya un registro de incidencia levantada para esta calendarizacion
                                {
                                    var sequencia_incidencia = GetSequence<int>("INCIDENCIA_SEQ");
                                    var incidencias_producto = new List<INCIDENCIA_PRODUCTO>();
                                    incidencias_producto.Add(new INCIDENCIA_PRODUCTO
                                    {
                                        CANTIDAD = item.RESTANTE,
                                        ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                        ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                        ID_PRODUCTO = item.ID_PRODUCTO,
                                        OBSERV = item.INCIDENCIA_OBSERVACIONES
                                    }); // se agrega la primera incidencia detalle de la nueva incidencia
                                    incidencia = new INCIDENCIA
                                    {
                                        ID_INCIDENCIA = sequencia_incidencia,
                                        FECHA = DateTime.Now.Date,
                                        ID_ORDEN_COMPRA = entrada.ID_ORDEN_COMPRA,
                                        ID_USUARIO = _id_usuario,
                                        ID_ALMACEN = entrada.ID_ALMACEN,
                                        INCIDENCIA_PRODUCTO = incidencias_producto
                                    };
                                    Context.INCIDENCIA.Add(incidencia);
                                    Context.SaveChanges();
                                }
                                else
                                    incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
                                    {
                                        CANTIDAD = item.RESTANTE,
                                        ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                        ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                        ID_PRODUCTO = item.ID_PRODUCTO,
                                        OBSERV = item.INCIDENCIA_OBSERVACIONES
                                    });
                                if (calendarizar_entrega!=null)
                                {
                                    if (item.FechaRecalendarizacion.HasValue) //si se va a recalendarizar todo el producto
                                    {
                                        var recalendarizacion = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.FEC_PACTADA == item.FechaRecalendarizacion.Value
                                                && w.ID_ALMACEN == entrada.ID_ALMACEN && w.ID_ORDEN_COMPRA == entrada.ID_ORDEN_COMPRA);
                                        if (recalendarizacion == null) //si no existe nada en la fecha a recalendarizar
                                        {
                                            seq_recalendarizar = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
                                            consec_recalendarizacion=1;
                                            var calendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>();
                                            calendarizar_entrega_producto.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                            {
                                                CANTIDAD = item.ORDENADO,
                                                ID_PRODUCTO = item.ID_PRODUCTO,
                                                ID_CONSEC=consec_recalendarizacion,
                                                ESTATUS="PR"
                                            });
                                            Context.CALENDARIZAR_ENTREGA.Add(new CALENDARIZAR_ENTREGA
                                            {
                                                CALENDARIZAR_ENTREGA_PRODUCTO = calendarizar_entrega_producto,
                                                ESTATUS = "PR",
                                                FEC_PACTADA = item.FechaRecalendarizacion,
                                                FECHA = DateTime.Now,
                                                ID_USUARIO = _id_usuario,
                                                ID_ALMACEN = entrada.ID_ALMACEN,
                                                ID_CALENDARIZACION_ENTREGA = seq_recalendarizar,
                                                ID_ORDEN_COMPRA = entrada.ID_ORDEN_COMPRA
                                            });
                                            Context.SaveChanges();
                                        }
                                        else
                                        {
                                            var recalendarizacion_entrega_producto = recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO && w.ESTATUS=="PR");
                                            seq_recalendarizar=recalendarizacion.ID_CALENDARIZACION_ENTREGA;
                                            if (recalendarizacion_entrega_producto == null) //si no existe la calendarizacion de ese producto
                                            {
                                                consec_recalendarizacion = recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.Where(w => w.ID_PRODUCTO == item.ID_PRODUCTO).Max(c => c.ID_CONSEC)+1;
                                                recalendarizacion.CALENDARIZAR_ENTREGA_PRODUCTO.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                                {
                                                    CANTIDAD = item.ORDENADO,
                                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                                    ID_CONSEC = consec_recalendarizacion,
                                                    ESTATUS="PR"
                                                });
                                            }
                                            else
                                            {
                                                recalendarizacion_entrega_producto.CANTIDAD += item.ORDENADO;
                                                consec_recalendarizacion=recalendarizacion_entrega_producto.ID_CONSEC;
                                            }
                                                
                                        }
                                    }
                                    else //si no se va a recalendarizar, se regresa todo a su estado normal
                                    {
                                        orden_compra_detalle.CANTIDAD_TRANSITO -= item.ORDENADO;
                                        orden_compra_detalle.DIFERENCIA += item.ORDENADO;
                                    }
                                }
                                
                            }

                            if (calendarizar_entrega != null)//hay que cambiar los campos pertinentes de la calendarizacion del producto
                            {
                                var calendarizacion_entrega_producto=calendarizar_entrega.CALENDARIZAR_ENTREGA_PRODUCTO.First(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ESTATUS=="PR");
                                if (item.RECIBIDO>0) //si hubo recepcion
                                {
                                    if (item.RESTANTE > 0) //si hubo recepcion parcial
                                    {
                                        calendarizacion_entrega_producto.ESTATUS = "RP"; //estatus recibido parcial
                                        if (item.FechaRecalendarizacion.HasValue) //si hubo recalendarizacion
                                        {
                                            calendarizacion_entrega_producto.CAL_ID_CALENDARIZACION_ENTREGA = seq_recalendarizar;
                                            calendarizacion_entrega_producto.CAL_ID_CONSEC = consec_recalendarizacion;
                                        }
                                    }
                                    else
                                    {
                                        calendarizacion_entrega_producto.ESTATUS = "RB";
                                    }
                                }
                                else //si se rechazo todo el producto
                                {

                                    if(item.FechaRecalendarizacion.HasValue)
                                    {
                                        calendarizacion_entrega_producto.ESTATUS = "RL";
                                        calendarizacion_entrega_producto.CAL_ID_CALENDARIZACION_ENTREGA = seq_recalendarizar;
                                        calendarizacion_entrega_producto.CAL_ID_CONSEC = consec_recalendarizacion;
                                    }
                                    else
                                    {
                                        calendarizacion_entrega_producto.ESTATUS = "RC";
                                    }
                                }
                                if (item.INCIDENCIA_TIPO != null && item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA!=-1)
                                    calendarizacion_entrega_producto.ID_INCIDENCIA = incidencia.ID_INCIDENCIA;

                            }
                        }
                    if (orden_compra.ORDEN_COMPRA_DETALLE.Any(w => w.CANTIDAD_ENTREGADA != w.CANTIDAD_ORDEN && w.ID_ALMACEN==entrada.ID_ALMACEN))
                        orden_compra.ESTATUS = "PR";
                    else
                        orden_compra.ESTATUS = "RE";
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}
