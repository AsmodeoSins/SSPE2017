using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
using SSP.Servidor.ModelosExtendidos;
using LinqKit;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cOrden_Compra:EntityManagerServer<ORDEN_COMPRA>
    {
        public IQueryable<ORDEN_COMPRA> Seleccionar(string status="",string producto_grupo="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ORDEN_COMPRA>();
                if (!string.IsNullOrWhiteSpace(status))
                    predicate = predicate.And(w => w.ESTATUS==status);
                if (!string.IsNullOrWhiteSpace(producto_grupo))
                    predicate = predicate.And(w => w.REQUISICION.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == producto_grupo);
                return GetData(predicate.Expand()).OrderBy(o => o.NUM_ORDEN);
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ORDEN_COMPRA> SeleccionarPorAlmacenparaCalendarizar(int almacen)
        {
            var predicate = PredicateBuilder.True<ORDEN_COMPRA>();
            predicate = predicate.And(w => w.ESTATUS == "CP" || w.ESTATUS == "CL" || w.ESTATUS == "PR");
            predicate = predicate.And(w => w.REQUISICION.REQUISICION_CENTRO.Any(a=>a.ID_ALMACEN==almacen));
            predicate = predicate.And(w => !w.MOVIMIENTO.Any(a => a.ID_ALMACEN == almacen && a.ID_ORDEN_COMPRA == w.ID_ORDEN_COMPRA
                && !a.CALENDARIZAR_ENTREGA.Any(a2 => a2.ID_ENTRADA == a.ID_MOVIMIENTO)));
            return GetData(predicate.Expand()).OrderBy(o => o.NUM_ORDEN);
        }


        public void Insertar(ORDEN_COMPRA entidad, List<EXT_Orden_Compra_Detalle> orden_compra_detalle)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    short orden_compra_detalle_seq = 1;
                    entidad.ID_ORDEN_COMPRA = GetSequence<int>("ORDEN_COMPRA_SEQ");
                    foreach (var item in orden_compra_detalle)
                    {
                        entidad.ORDEN_COMPRA_DETALLE.Add(new ORDEN_COMPRA_DETALLE {
                            CANTIDAD_ENTREGADA=item.CANTIDAD_ENTREGADA,
                            CANTIDAD_ORDEN=item.CANTIDAD_ORDEN,
                            DIFERENCIA=item.DIFERENCIA,
                            ID_ALMACEN=item.ID_ALMACEN,
                            ID_ORDEN_COMPRA=entidad.ID_ORDEN_COMPRA,
                            ID_ORDEN_COMPRA_DET=orden_compra_detalle_seq,
                            ID_PRODUCTO=item.ID_PRODUCTO,
                            PRECIO_COMPRA=item.PRECIO_COMPRA,
                            CANTIDAD_TRANSITO=item.CANTIDAD_TRANSITO
                        });
                        orden_compra_detalle_seq += 1;
                        //marcar el detalle en requisicion_centro_producto como ASIGNADO
                        var requisicion_centro_producto = Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_REQUISICION == item.ID_REQUISICION_CENTRO);
                        if (requisicion_centro_producto == null)
                            throw new Exception("Se elimino esta requisicion de producto del centro durante la operacion");
                        requisicion_centro_producto.ESTATUS = "AS";
                    }

                    Context.ORDEN_COMPRA.Add(entidad);
                    Context.SaveChanges();
                    foreach(var id in orden_compra_detalle.Select(s=>s.ID_REQUISICION_CENTRO).Distinct())
                    {
                        var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(f => f.ID_REQUISICION == id);
                        if (requisicion_centro == null)
                            throw new Exception("Esta requisicion de centro fue eliminada en el transcurso de la operacion");
                        if (Context.REQUISICION_CENTRO_PRODUCTO.Any(a => a.ID_REQUISICION == id && a.ESTATUS != "AS"))
                            requisicion_centro.ESTATUS = "AP";
                        else
                            requisicion_centro.ESTATUS = "AS";                        
                    }
                    Context.SaveChanges();
                    var requisicion = Context.REQUISICION.FirstOrDefault(f => f.ID_REQUISICION == entidad.ID_REQUISICION);
                    if(requisicion==null)
                        throw new Exception("Esta requisicion fue eliminada en el transcurso de la operacion");
                    if (requisicion.REQUISICION_CENTRO.Any(a => a.ESTATUS != "AS"))
                        requisicion.ESTATUS = "AP";
                    else
                        requisicion.ESTATUS = "AS";
                    Context.SaveChanges();
                    transaccion.Complete();   
                }
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void Actualizar(ORDEN_COMPRA entidad, List<EXT_Orden_Compra_Detalle> orden_compra_detalle)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var orden_compra_entidad = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA  == entidad.ID_ORDEN_COMPRA);                  
                    short orden_compra_detalle_seq = orden_compra_entidad.ORDEN_COMPRA_DETALLE.Max(m=>m.ID_ORDEN_COMPRA_DET); 
                    if(orden_compra_entidad.FECHA!=entidad.FECHA)
                        orden_compra_entidad.FECHA=entidad.FECHA;
                    if(orden_compra_entidad.ID_PROV!=entidad.ID_PROV)
                        orden_compra_entidad.ID_PROV=entidad.ID_PROV;
                    if(orden_compra_entidad.ID_REQUISICION!=entidad.ID_REQUISICION) //Si se cambio de requisicion en la edicion de la orden.
                    {
                        //regresar al estado "Autorizado" todo el detalle de productos de las requisiciones de los centros antiguas de la OC.
                        var v_req_productos = Context.V_REQ_PRODUCTOS.Where(w => w.ID_ORDEN_COMPRA == orden_compra_entidad.ID_ORDEN_COMPRA && w.OC_REQ == orden_compra_entidad.ID_REQUISICION);
                        foreach(var item in v_req_productos)
                        {
                            var requisicion_centro_producto = Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_REQUISICION == item.ID_REQUISICION);
                            if (requisicion_centro_producto == null)
                                throw new Exception("Se elimino esta requisicion de producto del centro durante la operacion");
                            requisicion_centro_producto.ESTATUS = "PA";
                            var entry = Context.Entry(requisicion_centro_producto);
                            entry.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }
                        //regresar al estado correcto la requisicion del centro antiguas de la OC.
                        foreach (var id in Context.V_REQ_PRODUCTOS.Where(w => w.ID_ORDEN_COMPRA == orden_compra_entidad.ID_ORDEN_COMPRA && w.OC_REQ == orden_compra_entidad.ID_REQUISICION).Select(s => s.ID_REQUISICION).Distinct())
                        {
                            var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(f => f.ID_REQUISICION == id);
                            if (requisicion_centro == null)
                                throw new Exception("Esta requisicion de centro fue eliminada en el transcurso de la operacion");
                            if (Context.REQUISICION_CENTRO_PRODUCTO.Any(a => a.ID_REQUISICION == id && a.ESTATUS != "PA"))
                                requisicion_centro.ESTATUS = "AP";
                            else
                                requisicion_centro.ESTATUS = "AE";
                            var entry = Context.Entry(requisicion_centro);
                            entry.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }

                        //cambiar el estatus de la requisicion concentrada original asignada a la oc
                        var requisicion = Context.REQUISICION.FirstOrDefault(f => f.ID_REQUISICION == orden_compra_entidad.ID_REQUISICION);
                        if (requisicion == null)
                            throw new Exception("Esta requisicion fue eliminada en el transcurso de la operacion");
                        if (requisicion.REQUISICION_CENTRO.Any(a => a.ESTATUS != "AP"))
                            requisicion.ESTATUS = "AP";
                        else
                            requisicion.ESTATUS = "AE";
                        var entryReq = Context.Entry(requisicion);
                        entryReq.State = System.Data.EntityState.Modified;
                        Context.SaveChanges();

                        //eliminar el detalle de productos antiguo de la OC
                        orden_compra_entidad.ORDEN_COMPRA_DETALLE.Clear();

                        //insertar el nuevo detalle de producto de la OC
                        orden_compra_detalle_seq = 1;
                        foreach (var item in orden_compra_detalle)
                        {
                            orden_compra_entidad.ORDEN_COMPRA_DETALLE.Add(new ORDEN_COMPRA_DETALLE
                            {
                                CANTIDAD_ENTREGADA = item.CANTIDAD_ENTREGADA,
                                CANTIDAD_ORDEN = item.CANTIDAD_ORDEN,
                                DIFERENCIA = item.DIFERENCIA,
                                ID_ALMACEN = item.ID_ALMACEN,
                                ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                                ID_ORDEN_COMPRA_DET = orden_compra_detalle_seq,
                                ID_PRODUCTO = item.ID_PRODUCTO,
                                PRECIO_COMPRA = item.PRECIO_COMPRA,
                                CANTIDAD_TRANSITO=item.CANTIDAD_TRANSITO 
                            });
                            orden_compra_detalle_seq += 1;
                            //marcar el detalle en requisicion_centro_producto como ASIGNADO
                            var requisicion_centro_producto = Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_REQUISICION == item.ID_REQUISICION_CENTRO);
                            if (requisicion_centro_producto == null)
                                throw new Exception("Se elimino esta requisicion de producto del centro durante la operacion");
                            requisicion_centro_producto.ESTATUS = "AS";
                            var entry = Context.Entry(requisicion_centro_producto);
                            entry.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }

                        //cambiar el estatus de las requisicion de centro de la nueva requisicion asignada a la orden de compra
                        foreach (var id in orden_compra_detalle.Select(s => s.ID_REQUISICION_CENTRO).Distinct())
                        {
                            var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(f => f.ID_REQUISICION == id);
                            if (requisicion_centro == null)
                                throw new Exception("Esta requisicion de centro fue eliminada en el transcurso de la operacion");
                            if (Context.REQUISICION_CENTRO_PRODUCTO.Any(a => a.ID_REQUISICION == id && a.ESTATUS != "AS"))
                                requisicion_centro.ESTATUS = "AP";
                            else
                                requisicion_centro.ESTATUS = "AS";
                            var entry = Context.Entry(requisicion_centro);
                            entry.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }

                        //cambiar el estatus de la nueva requisicion concentrada asignada a la oc
                        requisicion = Context.REQUISICION.FirstOrDefault(f => f.ID_REQUISICION == entidad.ID_REQUISICION);
                        if (requisicion == null)
                            throw new Exception("Esta requisicion fue eliminada en el transcurso de la operacion");
                        if (requisicion.REQUISICION_CENTRO.Any(a => a.ESTATUS != "AS"))
                            requisicion.ESTATUS = "AP";
                        else
                            requisicion.ESTATUS = "AS";
                        entryReq = Context.Entry(requisicion);
                        entryReq.State = System.Data.EntityState.Modified;
                        Context.SaveChanges();

                        orden_compra_entidad.ID_REQUISICION = entidad.ID_REQUISICION;


                    }
                    else //Si no hubo cambio de requisicion pero si hay posibilidad de cambios en los productos requeridos
                    {
                        orden_compra_detalle_seq += 1;
                        var lista_requisiciones_centro_modificadas = new List<int>();
                        ORDEN_COMPRA_DETALLE[] _copia = new ORDEN_COMPRA_DETALLE[orden_compra_entidad.ORDEN_COMPRA_DETALLE.Count()];
                        orden_compra_entidad.ORDEN_COMPRA_DETALLE.CopyTo(_copia, 0);
                        //se elimina los detalles no existentes en la nueva lista de detalle de productos de OC
                        foreach(var item in _copia)
                        {
                            var encontrado = orden_compra_detalle.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA);
                            if (encontrado == null)
                            {
                                var requisicion_centro_producto=Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO==item.ID_PRODUCTO && f.REQUISICION_CENTRO.ID_ALMACEN==item.ID_ALMACEN &&
                                    f.REQUISICION_CENTRO.REQUISICION.ID_REQUISICION==item.ORDEN_COMPRA.ID_REQUISICION);
                                if (requisicion_centro_producto == null)
                                    throw new Exception("El detalle de producto de la requisicion fue eliminado durante el proceso de edicion");
                                requisicion_centro_producto.ESTATUS = "PA";
                                var entry = Context.Entry(requisicion_centro_producto);
                                entry.State = System.Data.EntityState.Modified;
                                Context.SaveChanges();
                                if (!lista_requisiciones_centro_modificadas.Contains(requisicion_centro_producto.ID_REQUISICION))
                                    lista_requisiciones_centro_modificadas.Add(requisicion_centro_producto.ID_REQUISICION);
                                orden_compra_entidad.ORDEN_COMPRA_DETALLE.Remove(orden_compra_entidad.ORDEN_COMPRA_DETALLE.First(f => f.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA &&
                                    f.ID_ALMACEN == item.ID_ALMACEN && f.ID_PRODUCTO == item.ID_PRODUCTO));
                            }
                                
                        }
                        foreach(var item in orden_compra_detalle)
                        {
                            var encontrado = orden_compra_entidad.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA && f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN==item.ID_ALMACEN);
                            if (encontrado==null)
                            {
                                //se agrega el producto al detalle de la orden
                                orden_compra_entidad.ORDEN_COMPRA_DETALLE.Add(new ORDEN_COMPRA_DETALLE
                                {
                                    CANTIDAD_ENTREGADA = item.CANTIDAD_ENTREGADA,
                                    CANTIDAD_ORDEN = item.CANTIDAD_ORDEN,
                                    DIFERENCIA = item.DIFERENCIA,
                                    ID_ALMACEN = item.ID_ALMACEN,
                                    ID_ORDEN_COMPRA = orden_compra_entidad.ID_ORDEN_COMPRA,
                                    ID_ORDEN_COMPRA_DET = orden_compra_detalle_seq,
                                    ID_PRODUCTO = item.ID_PRODUCTO,
                                    PRECIO_COMPRA = item.PRECIO_COMPRA,
                                    CANTIDAD_TRANSITO=item.CANTIDAD_TRANSITO 
                                });
                                //se cambio el estatus del producto el detalle de la requisicion del centro.
                                var requisicion_centro_producto = Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f=>f.ID_PRODUCTO==item.ID_PRODUCTO && f.REQUISICION_CENTRO.ID_ALMACEN==item.ID_ALMACEN && 
                                    f.REQUISICION_CENTRO.ID_REQUISICION==item.ID_REQUISICION_CENTRO);
                                if (requisicion_centro_producto==null)
                                    throw new Exception("El detalle de producto de la requisicion fue eliminado durante el proceso de edicion");
                                requisicion_centro_producto.ESTATUS = "AS";
                                var entry = Context.Entry(requisicion_centro_producto);
                                entry.State = System.Data.EntityState.Modified;
                                Context.SaveChanges();
                                if (!lista_requisiciones_centro_modificadas.Contains(requisicion_centro_producto.ID_REQUISICION))
                                    lista_requisiciones_centro_modificadas.Add(requisicion_centro_producto.ID_REQUISICION);
                                orden_compra_detalle_seq += 1;
                            }
                            else
                            {
                                if (encontrado.PRECIO_COMPRA != item.PRECIO_COMPRA) //si el usuario modifico el precio de compra
                                    encontrado.PRECIO_COMPRA = item.PRECIO_COMPRA;
                            }
                        }
                        foreach (var id in lista_requisiciones_centro_modificadas) //si el detalle de la OC fue modificado hay que revaluar la requisicion de centro para poner estatus correcto
                        {
                            var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(f => f.ID_REQUISICION == id);
                            if (requisicion_centro == null)
                                throw new Exception("Esta requisicion de centro fue eliminada en el transcurso de la operacion");
                            if (Context.REQUISICION_CENTRO_PRODUCTO.Any(a => a.ID_REQUISICION == id && a.ESTATUS != "AS"))
                                requisicion_centro.ESTATUS = "AP";
                            else
                                requisicion_centro.ESTATUS = "AS";
                            var entry = Context.Entry(requisicion_centro);
                            entry.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }

                        if (lista_requisiciones_centro_modificadas.Count > 0) //hay que revaluar el estatus de la nueva requisicion concentrada asignada a la oc en caso de existir cambios a las req. de centros.
                        {
                            var requisicion = Context.REQUISICION.FirstOrDefault(f => f.ID_REQUISICION == orden_compra_entidad.ID_REQUISICION);
                            if (requisicion == null)
                                throw new Exception("Esta requisicion fue eliminada en el transcurso de la operacion");
                            if (requisicion.REQUISICION_CENTRO.Any(a => a.ESTATUS != "AS"))
                                requisicion.ESTATUS = "AP";
                            else
                                requisicion.ESTATUS = "AS";
                            var entryReq = Context.Entry(requisicion);
                            entryReq.State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                        }
                    }
                        
                    orden_compra_entidad.ID_USUARIO = entidad.ID_USUARIO;

                    var entryOC = Context.Entry(orden_compra_entidad);
                    entryOC.State = System.Data.EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void Eliminar(ORDEN_COMPRA entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                   

                    var orden_compra_entidad = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);

                    //regresar al estado "Autorizado" todo el detalle de productos de las requisiciones de los centros antiguas de la OC.
                    var v_req_productos = Context.V_REQ_PRODUCTOS.Where(w => w.ID_ORDEN_COMPRA == orden_compra_entidad.ID_ORDEN_COMPRA && w.OC_REQ == orden_compra_entidad.ID_REQUISICION);
                    foreach (var item in v_req_productos)
                    {
                        var requisicion_centro_producto = Context.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_REQUISICION == item.ID_REQUISICION);
                        if (requisicion_centro_producto == null)
                            throw new Exception("Se elimino esta requisicion de producto del centro durante la operacion");
                        requisicion_centro_producto.ESTATUS = "PA";
                        Context.SaveChanges();
                    }

                    //regresar al estado correcto la requisicion del centro antiguas de la OC.
                    foreach (var id in Context.V_REQ_PRODUCTOS.Where(w => w.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA && w.OC_REQ == entidad.ID_REQUISICION).Select(s => s.ID_REQUISICION).Distinct())
                    {
                        var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(f => f.ID_REQUISICION == id);
                        if (requisicion_centro == null)
                            throw new Exception("Esta requisicion de centro fue eliminada en el transcurso de la operacion");
                        if (requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Any(a => a.ESTATUS != "PA"))
                            requisicion_centro.ESTATUS = "AP";
                        else
                            requisicion_centro.ESTATUS = "AE";
                        Context.SaveChanges();
                    }

                    //cambiar el estatus de la requisicion concentrada original asignada a la oc
                    var requisicion = Context.REQUISICION.FirstOrDefault(f => f.ID_REQUISICION == orden_compra_entidad.ID_REQUISICION);
                    if (requisicion == null)
                        throw new Exception("Esta requisicion fue eliminada en el transcurso de la operacion");
                    if (requisicion.REQUISICION_CENTRO.Any(a => a.ESTATUS != "AU"))
                        requisicion.ESTATUS = "AP";
                    else
                        requisicion.ESTATUS = "AE";
                    Context.SaveChanges();

                    orden_compra_entidad.ORDEN_COMPRA_DETALLE.Clear();

                    Context.ORDEN_COMPRA.Remove(orden_compra_entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ORDEN_COMPRA> SeleccionarOrdenesCalendarizadas(DateTime dia, int id_almacen)
        {
            try
            {
                return GetData(w => w.CALENDARIZAR_ENTREGA.Any(a => a.FEC_PACTADA == dia.Date  && a.ID_ALMACEN == id_almacen && a.ESTATUS=="PR")).OrderBy(o => o.NUM_ORDEN).ThenBy(o => o.PROVEEDOR.NOMBRE);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<ORDEN_COMPRA> SeleccionarPorEstatus(List<string> estatus, short? id_almacen=null, string almacen_grupo="")
        {
            try
            {
                var predicateOr = PredicateBuilder.False<ORDEN_COMPRA>();
                var predicate = PredicateBuilder.True<ORDEN_COMPRA>();
                if (estatus==null || estatus.Count==0)
                    throw new Exception("Se debe de buscar por lo menos por un estatus de orden de compra.");
                foreach (var item in estatus)
                    predicateOr=predicateOr.Or(w => w.ESTATUS == item);
                predicate = predicate.And(predicateOr.Expand());
                if (id_almacen.HasValue)
                    predicate=predicate.And(w=>w.ORDEN_COMPRA_DETALLE.Any(a=>a.ID_ALMACEN==id_almacen));
                else
                    if(string.IsNullOrWhiteSpace(almacen_grupo))
                        predicate=predicate.And(w=>w.ORDEN_COMPRA_DETALLE.Any(a=>a.ALMACEN.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO==almacen_grupo));
                return GetData(predicate.Expand()).OrderBy(o => o.NUM_ORDEN).ThenBy(o => o.PROVEEDOR.NOMBRE);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}
