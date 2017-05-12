using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor.ModelosExtendidos;
using SSP.Servidor;
using System.Transactions;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cCalendarizar_Entrega:EntityManagerServer<CALENDARIZAR_ENTREGA>
    {
        public void Insertar (List<EXT_CALENDARIZAR_ENTREGA> entidades)
        {
            try
            {

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach(var entidad in entidades)
                    {
                        var sequencia = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
                        var calendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>();
                        foreach (var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO)
                            if (item.IsEditable)
                                calendarizar_entrega_producto.Add(new CALENDARIZAR_ENTREGA_PRODUCTO {
                                    ID_CALENDARIZACION_ENTREGA=sequencia,
                                    ID_PRODUCTO=item.ID_PRODUCTO,
                                    ID_CONSEC=1,
                                    CANTIDAD=item.CANTIDAD,
                                    ESTATUS="PR" //ESTATUS PROGRAMADO
                                });
                        var calendarizar_entrega = new CALENDARIZAR_ENTREGA
                        {
                            ESTATUS = entidad.ESTATUS,
                            FEC_PACTADA = entidad.FEC_PACTADA,
                            FECHA = entidad.FECHA,
                            ID_ALMACEN = entidad.ID_ALMACEN,
                            ID_CALENDARIZACION_ENTREGA = sequencia,
                            ID_ENTRADA = null,
                            //ID_INCIDENCIA = null,
                            ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                            ID_USUARIO = entidad.ID_USUARIO,
                            CALENDARIZAR_ENTREGA_PRODUCTO=calendarizar_entrega_producto
                        };
                        Context.CALENDARIZAR_ENTREGA.Add(calendarizar_entrega);
                        foreach (var calendario_producto in calendarizar_entrega.CALENDARIZAR_ENTREGA_PRODUCTO)
                        {
                            var orden_compra_detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(w => w.ORDEN_COMPRA.ID_ORDEN_COMPRA == calendario_producto.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA && 
                                w.ID_PRODUCTO==calendario_producto.ID_PRODUCTO && w.ID_ALMACEN==calendario_producto.CALENDARIZAR_ENTREGA.ID_ALMACEN);
                            if (orden_compra_detalle == null)
                                throw new Exception("El detalle de la orden de compra fue eliminado durante este proceso.");
                            orden_compra_detalle.CANTIDAD_TRANSITO += calendario_producto.CANTIDAD;
                            orden_compra_detalle.DIFERENCIA -= (calendario_producto.CANTIDAD);
                        }
                        var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == calendarizar_entrega.ID_ORDEN_COMPRA);
                        if (OC == null)
                            throw new Exception("La orden de compra se elimino durante este proceso");
                        OC.ESTATUS = "CL"; //modificar el estatus de la OC a CaLendarizado
                    }
                    Context.SaveChanges();
                    transaccion.Complete(); 
                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<CALENDARIZAR_ENTREGA>Seleccionar(int almacen_id,int mes,int anio)
        {
            try
            {
                return GetData(w => w.FEC_PACTADA.Value.Year == anio && w.FEC_PACTADA.Value.Month == mes && w.ID_ALMACEN == almacen_id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }            
        }


        public void Actualizar(List<EXT_CALENDARIZAR_ENTREGA> entidades, DateTime fecha_original)
        {
            try
            {

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var entidad in entidades)
                    {

                       
                            #region si no hay recalendarizacion o no hay agenda ya realizada para la fecha de recalendarizacion
                            int sequencia = 0;
                            int? seq_incidencia = null;
                            var entidadCalendarizar_Entrega = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.ID_ALMACEN == entidad.ID_ALMACEN && w.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA &&
                                w.FEC_PACTADA == fecha_original); //verificar si ya existe la calendarizacion para esta orden de compra
                            if (entidadCalendarizar_Entrega == null) //si no existe insertar
                            {
                                sequencia = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
                                var calendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>();
                                foreach (var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO)
                                    if (item.IsEditable)
                                        calendarizar_entrega_producto.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                        {
                                            ID_CALENDARIZACION_ENTREGA = sequencia,
                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                            ID_CONSEC = 1,
                                            CANTIDAD = item.CANTIDAD,
                                            ESTATUS = "PR"
                                        });
                                var calendarizar_entrega = new CALENDARIZAR_ENTREGA
                                {
                                    ESTATUS = entidad.ESTATUS,
                                    FEC_PACTADA = entidad.FEC_PACTADA,
                                    FECHA = entidad.FECHA,
                                    ID_ALMACEN = entidad.ID_ALMACEN,
                                    ID_CALENDARIZACION_ENTREGA = sequencia,
                                    ID_ENTRADA = null,
                                    //ID_INCIDENCIA = null,
                                    ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                                    ID_USUARIO = entidad.ID_USUARIO,
                                    CALENDARIZAR_ENTREGA_PRODUCTO = calendarizar_entrega_producto
                                };
                                entidad.ID_CALENDARIZACION_ENTREGA = sequencia;
                                Context.CALENDARIZAR_ENTREGA.Add(calendarizar_entrega);
                                foreach (var calendario_producto in calendarizar_entrega.CALENDARIZAR_ENTREGA_PRODUCTO)
                                {
                                    var orden_compra_detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(w => w.ORDEN_COMPRA.ID_ORDEN_COMPRA == calendario_producto.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA &&
                                        w.ID_PRODUCTO == calendario_producto.ID_PRODUCTO && w.ID_ALMACEN == calendario_producto.CALENDARIZAR_ENTREGA.ID_ALMACEN);
                                    if (orden_compra_detalle == null)
                                        throw new Exception("El detalle de la orden de compra fue eliminado durante este proceso.");
                                    orden_compra_detalle.CANTIDAD_TRANSITO += calendario_producto.CANTIDAD;
                                    orden_compra_detalle.DIFERENCIA -= (calendario_producto.CANTIDAD);
                                }
                                var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == calendarizar_entrega.ID_ORDEN_COMPRA);
                                if (OC == null)
                                    throw new Exception("La orden de compra se elimino durante este proceso");
                                OC.ESTATUS = "CL"; //modificar el estatus de la OC a CaLendarizado
                            }
                            else //si ya existe actualizar/insertar/eliminar registros pertinentes.
                            {
                                //insertar o actualizar registros de la calendarizacion
                                foreach (var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO.Where(w => w.IsEditable == true)) //solo los que se pueden editar
                                {
                                    var encontrado = entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ESTATUS == "PR");
                                    if (encontrado == null) //hay que insertar si no lo encontro
                                    {
                                        //hay que afectar calculos en la tabla orden_compra_detalle
                                        var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
                                            f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
                                        if (detalle == null)
                                            throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
                                        detalle.CANTIDAD_TRANSITO += item.CANTIDAD;
                                        detalle.DIFERENCIA -= item.CANTIDAD;
                                        //obtener el ultimo consecutivo
                                        var consec = entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO) == null ? 1 :
                                                            entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Where(f => f.ID_PRODUCTO == item.ID_PRODUCTO).Max(m => m.ID_CONSEC) + 1;

                                        //insertar en la calendarizacion
                                        var calendarizar_entrega_producto = new CALENDARIZAR_ENTREGA_PRODUCTO
                                        {
                                            CANTIDAD = item.CANTIDAD,
                                            ID_CALENDARIZACION_ENTREGA = entidadCalendarizar_Entrega.ID_CALENDARIZACION_ENTREGA,
                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                            ID_CONSEC = consec,
                                            ESTATUS = "PR"
                                        };
                                        item.ID_CALENDARIZACION_ENTREGA = entidadCalendarizar_Entrega.ID_CALENDARIZACION_ENTREGA;
                                        entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Add(calendarizar_entrega_producto);
                                    }
                                    else// si lo encontro hay que checar posible modificaciones a estatus y cantidad
                                    {
                                        //checar si fue cancelado
                                        if (item.ESTATUS == "CA")
                                        {

                                            //hay que afectar calculos en la tabla orden_compra_detalle
                                            var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == encontrado.ID_PRODUCTO && f.ID_ALMACEN == encontrado.CALENDARIZAR_ENTREGA.ID_ALMACEN &&
                                                f.ID_ORDEN_COMPRA == encontrado.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA);
                                            if (detalle == null)
                                                throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
                                            detalle.CANTIDAD_TRANSITO -= encontrado.CANTIDAD;
                                            detalle.DIFERENCIA += encontrado.CANTIDAD;
                                            //ingresar la incidencia en caso de existir
                                            if (item.INCIDENCIA_TIPO != null) //si hubo incidencia registrarla
                                            {
                                                if (!seq_incidencia.HasValue) //si no se ha registrado una incidencia para esta calendarizacion
                                                {
                                                    var cancelacion_incidencia_producto = new List<INCIDENCIA_PRODUCTO>(){new INCIDENCIA_PRODUCTO
                                                        {
                                                            CANTIDAD = item.CANTIDAD,
                                                            ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                                            ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                                            OBSERV = item.INCIDENCIA_OBSERVACIONES
                                                        }};
                                                    seq_incidencia = sequencia = GetSequence<int>("INCIDENCIA_SEQ");
                                                    Context.INCIDENCIA.Add(new INCIDENCIA
                                                    {
                                                        FECHA = DateTime.Now.Date,
                                                        ID_ALMACEN = entidad.ID_ALMACEN,
                                                        ID_INCIDENCIA = seq_incidencia.Value,
                                                        ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                                                        ID_USUARIO = entidad.ID_USUARIO,
                                                        INCIDENCIA_PRODUCTO = cancelacion_incidencia_producto
                                                    });
                                                    Context.SaveChanges();
                                                }
                                                else //si ya hay registro de incidencias para esta calendarizacion.
                                                {
                                                    var cancelacion_incidencia = Context.INCIDENCIA.First(f => f.ID_INCIDENCIA == seq_incidencia.Value);
                                                    cancelacion_incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
                                                    {
                                                        CANTIDAD = item.CANTIDAD,
                                                        ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                                        ID_PRODUCTO = item.ID_PRODUCTO,
                                                        ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                                        OBSERV = item.INCIDENCIA_OBSERVACIONES
                                                    });
                                                    Context.SaveChanges();
                                                }
                                            }
                                            //cancelar el registro de la calendarizacion
                                            encontrado.ESTATUS = "CA";
                                            encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
                                            Context.SaveChanges();
                                        }
                                        else
                                        {
                                            if (!item.FechaRecalendarizacion.HasValue && encontrado.CANTIDAD != item.CANTIDAD) //checar por cambios en cantidad sin recalendarizacion individual
                                            {
                                                //modificar calculos en la tabla compra detalle
                                                var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
                                                f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
                                                if (detalle == null)
                                                    throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
                                                detalle.CANTIDAD_TRANSITO -= (encontrado.CANTIDAD - item.CANTIDAD);
                                                detalle.DIFERENCIA += (encontrado.CANTIDAD - item.CANTIDAD);
                                                encontrado.CANTIDAD = item.CANTIDAD;
                                            }
                                            else
                                            {
                                                if (item.FechaRecalendarizacion.HasValue) //si hubo una recalendarizacion individual
                                                {
                                                    if (item.INCIDENCIA_TIPO != null) //si hubo incidencia registrarla
                                                    {
                                                        if (!seq_incidencia.HasValue) //si no se ha registrado una incidencia para esta calendarizacion
                                                        {
                                                            var recalendarizacion_incidencia_producto = new List<INCIDENCIA_PRODUCTO>(){new INCIDENCIA_PRODUCTO
                                                        {
                                                            CANTIDAD = item.CANTIDAD,
                                                            ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                                            ID_PRODUCTO = item.ID_PRODUCTO,
                                                            ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                                            OBSERV = item.INCIDENCIA_OBSERVACIONES
                                                        }};
                                                            seq_incidencia = sequencia = GetSequence<int>("INCIDENCIA_SEQ");
                                                            Context.INCIDENCIA.Add(new INCIDENCIA
                                                            {
                                                                FECHA = DateTime.Now.Date,
                                                                ID_ALMACEN = entidad.ID_ALMACEN,
                                                                ID_INCIDENCIA = seq_incidencia.Value,
                                                                ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                                                                ID_USUARIO = entidad.ID_USUARIO,
                                                                INCIDENCIA_PRODUCTO = recalendarizacion_incidencia_producto
                                                            });
                                                            Context.SaveChanges();
                                                        }
                                                        else //si ya hay registro de incidencias para esta calendarizacion.
                                                        {
                                                            var recalendarizacion_incidencia = Context.INCIDENCIA.First(f => f.ID_INCIDENCIA == seq_incidencia.Value);
                                                            recalendarizacion_incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
                                                            {
                                                                CANTIDAD = item.CANTIDAD,
                                                                ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
                                                                ID_PRODUCTO = item.ID_PRODUCTO,
                                                                ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
                                                                OBSERV = item.INCIDENCIA_OBSERVACIONES
                                                            });
                                                        }
                                                    }
                                                    var calendarizacion_existente = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(f => f.FEC_PACTADA == item.FechaRecalendarizacion && f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA
                                                        && f.ID_ALMACEN == entidad.ID_ALMACEN);
                                                    if (calendarizacion_existente == null) //si no hay una calendarizacion existente para esa fecha hay que insertar
                                                    {
                                                        var recalendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>(){new CALENDARIZAR_ENTREGA_PRODUCTO {
                                                        ESTATUS="PR",
                                                        ID_CONSEC=1,
                                                        CANTIDAD=item.CANTIDAD,
                                                        ID_PRODUCTO=item.ID_PRODUCTO
                                                    }};
                                                        sequencia = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
                                                        Context.CALENDARIZAR_ENTREGA.Add(new CALENDARIZAR_ENTREGA
                                                        {
                                                            CALENDARIZAR_ENTREGA_PRODUCTO = recalendarizar_entrega_producto,
                                                            ESTATUS = "PR",
                                                            FEC_PACTADA = item.FechaRecalendarizacion.Value,
                                                            FECHA = DateTime.Now.Date,
                                                            ID_ALMACEN = entidad.ID_ALMACEN,
                                                            ID_CALENDARIZACION_ENTREGA = sequencia,
                                                            ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
                                                            ID_USUARIO = entidad.ID_USUARIO,
                                                        });
                                                        Context.SaveChanges();
                                                        //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
                                                        encontrado.ESTATUS = "RL"; //RECALENDARIZADO
                                                        encontrado.CAL_ID_CONSEC = 1;
                                                        encontrado.CAL_ID_CALENDARIZACION_ENTREGA = sequencia;
                                                        encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
                                                    }
                                                    else //si ya hay calendarizacion hay que agregarle el detalle del producto
                                                    {
                                                        var calendarizacion_entrega_producto = calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO
                                                            && f.ESTATUS == "PR");
                                                        if (calendarizacion_entrega_producto == null) //si el producto no esta ya programado para esa calendarizacion hay que insertarlo
                                                        {
                                                            var consec = calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO) == null ? 1 :
                                                                calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.Where(f => f.ID_PRODUCTO == item.ID_PRODUCTO).Max(m => m.ID_CONSEC) + 1;
                                                            calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
                                                            {
                                                                ESTATUS = "PR",
                                                                ID_CONSEC = consec,
                                                                CANTIDAD = item.CANTIDAD,
                                                                ID_PRODUCTO = item.ID_PRODUCTO
                                                            });
                                                            //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
                                                            encontrado.ESTATUS = "RL"; //RECALENDARIZADO
                                                            encontrado.CAL_ID_CONSEC = consec;
                                                            encontrado.CAL_ID_CALENDARIZACION_ENTREGA = calendarizacion_existente.ID_CALENDARIZACION_ENTREGA;
                                                            encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
                                                        }
                                                        else //si el producto ya esta programado para esta calendarizacion hay que actualizar la cantidad
                                                        {
                                                            calendarizacion_entrega_producto.CANTIDAD += item.CANTIDAD;
                                                            //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
                                                            encontrado.ESTATUS = "RL"; //RECALENDARIZADO
                                                            encontrado.CAL_ID_CONSEC = calendarizacion_entrega_producto.ID_CONSEC;
                                                            encontrado.CAL_ID_CALENDARIZACION_ENTREGA = calendarizacion_existente.ID_CALENDARIZACION_ENTREGA;
                                                            encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
                                                        }
                                                    }
                                                    //hay que revisar si hubo cambios a la cantidad y reajustar el detalle de la orden de compra
                                                    if (encontrado.CANTIDAD != item.CANTIDAD)
                                                    {
                                                        //modificar calculos en la tabla compra detalle
                                                        var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
                                                        f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
                                                        if (detalle == null)
                                                            throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
                                                        detalle.CANTIDAD_TRANSITO -= (encontrado.CANTIDAD - item.CANTIDAD);
                                                        detalle.DIFERENCIA += (encontrado.CANTIDAD - item.CANTIDAD);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Any(a => a.ESTATUS == "PR" || a.ESTATUS == "RB" || a.ESTATUS == "RP")) // si se recalendarizaron o se cancelaron todas las calendarizaciones de productos de esa fecha hay que eliminar el registro
                                {
                                    var OC_original = entidadCalendarizar_Entrega.ID_ORDEN_COMPRA;
                                    entidadCalendarizar_Entrega.ESTATUS = "CA"; //cancelar la agenda
                                    Context.SaveChanges();
                                    if (!Context.CALENDARIZAR_ENTREGA.Any(a => a.ID_ORDEN_COMPRA == OC_original && a.ESTATUS != "CA")) //si no quedan calendarizaciones con esa OC que no esten canceladas
                                    {
                                        var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == OC_original);
                                        if (OC == null)
                                            throw new Exception("La orden de compra se elimino durante este proceso");
                                        OC.ESTATUS = "CP";//regresar a estatus orginal.
                                    }

                                }
                                else
                                {
                                    if (entidadCalendarizar_Entrega.ESTATUS == "CA") //si estaba cancelada toda la calendarizacion y se volvio a insertar una nueva agenda
                                        entidadCalendarizar_Entrega.ESTATUS = "PR";
                                }
                            }
                            #endregion

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

        //este codigo lo retomariamos y editariamos si quisieramos funcionalidad global de eliminar y reagendar 
        //public void Actualizar(List<EXT_CALENDARIZAR_ENTREGA> entidades, DateTime fecha_original)
        //{
        //    try
        //    {

        //        using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
        //        {
        //            foreach (var entidad in entidades)
        //            {

        //                CALENDARIZAR_ENTREGA entidadCalendarizarVerificar=null;
        //                if(entidad.FEC_PACTADA!=fecha_original)
        //                    entidadCalendarizarVerificar = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.ID_ALMACEN == entidad.ID_ALMACEN && w.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA &&
        //                     w.FEC_PACTADA == entidad.FEC_PACTADA ); //verificar si ya existe la calendarizacion para la fecha donde se va a mover
        //                if(entidadCalendarizarVerificar==null)
        //                {
        //                    #region si no hay recalendarizacion o no hay agenda ya realizada para la fecha de recalendarizacion
        //                    int sequencia = 0;
        //                    int? seq_incidencia = null;
                            

        //                    var entidadCalendarizar_Entrega = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.ID_ALMACEN == entidad.ID_ALMACEN && w.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA &&
        //                        w.FEC_PACTADA == fecha_original); //verificar si ya existe la calendarizacion para esta orden de compra
        //                    if (entidadCalendarizar_Entrega == null) //si no existe insertar
        //                    {
        //                        sequencia = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
        //                        var calendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>();
        //                        foreach (var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO)
        //                            if (item.IsEditable)
        //                                calendarizar_entrega_producto.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
        //                                {
        //                                    ID_CALENDARIZACION_ENTREGA = sequencia,
        //                                    ID_PRODUCTO = item.ID_PRODUCTO,
        //                                    ID_CONSEC=1,
        //                                    CANTIDAD = item.CANTIDAD,
        //                                    ESTATUS="PR"
        //                                });
        //                        var calendarizar_entrega = new CALENDARIZAR_ENTREGA
        //                        {
        //                            ESTATUS = entidad.ESTATUS,
        //                            FEC_PACTADA = entidad.FEC_PACTADA,
        //                            FECHA = entidad.FECHA,
        //                            ID_ALMACEN = entidad.ID_ALMACEN,
        //                            ID_CALENDARIZACION_ENTREGA = sequencia,
        //                            ID_ENTRADA = null,
        //                            //ID_INCIDENCIA = null,
        //                            ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
        //                            ID_USUARIO = entidad.ID_USUARIO,
        //                            CALENDARIZAR_ENTREGA_PRODUCTO = calendarizar_entrega_producto
        //                        };
        //                        entidad.ID_CALENDARIZACION_ENTREGA = sequencia;
        //                        Context.CALENDARIZAR_ENTREGA.Add(calendarizar_entrega);
        //                        foreach (var calendario_producto in calendarizar_entrega.CALENDARIZAR_ENTREGA_PRODUCTO)
        //                        {
        //                            var orden_compra_detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(w => w.ORDEN_COMPRA.ID_ORDEN_COMPRA == calendario_producto.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA &&
        //                                w.ID_PRODUCTO == calendario_producto.ID_PRODUCTO && w.ID_ALMACEN == calendario_producto.CALENDARIZAR_ENTREGA.ID_ALMACEN);
        //                            if (orden_compra_detalle == null)
        //                                throw new Exception("El detalle de la orden de compra fue eliminado durante este proceso.");
        //                            orden_compra_detalle.CANTIDAD_TRANSITO += calendario_producto.CANTIDAD;
        //                            orden_compra_detalle.DIFERENCIA -= (calendario_producto.CANTIDAD);
        //                        }
        //                        var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == calendarizar_entrega.ID_ORDEN_COMPRA);
        //                        if (OC == null)
        //                            throw new Exception("La orden de compra se elimino durante este proceso");
        //                        OC.ESTATUS = "CL"; //modificar el estatus de la OC a CaLendarizado
        //                    }
        //                    else //si ya existe actualizar/insertar/eliminar registros pertinentes.
        //                    {
        //                        //CALENDARIZAR_ENTREGA_PRODUCTO[] _copia = new CALENDARIZAR_ENTREGA_PRODUCTO[entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Count()];
        //                        //entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.CopyTo(_copia, 0);
        //                        //foreach (var item in _copia.Where(w=>w.ESTATUS=="PR"))
        //                        //{
        //                        //    var encontrado = entidad.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO);
        //                        //    if (encontrado == null) //eliminar si no lo encontro en la nueva lista de calendarizacion.
        //                        //    {
                                        
        //                        //    }
        //                        //}
        //                        //insertar o actualizar registros de la calendarizacion
        //                        foreach (var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO.Where(w=>w.IsEditable==true)) //solo los que se pueden editar
        //                        {
        //                            var encontrado = entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ESTATUS=="PR");
        //                            if (encontrado == null) //hay que insertar si no lo encontro
        //                            {
        //                                //hay que afectar calculos en la tabla orden_compra_detalle
        //                                var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
        //                                    f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
        //                                if (detalle == null)
        //                                    throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                                detalle.CANTIDAD_TRANSITO += item.CANTIDAD;
        //                                detalle.DIFERENCIA -= item.CANTIDAD;
        //                                //obtener el ultimo consecutivo
        //                                var consec = entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO) == null ? 1 :
        //                                                    entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Where(f => f.ID_PRODUCTO == item.ID_PRODUCTO).Max(m => m.ID_CONSEC) + 1;
                                                        
        //                                //insertar en la calendarizacion
        //                                var calendarizar_entrega_producto = new CALENDARIZAR_ENTREGA_PRODUCTO {
        //                                    CANTIDAD=item.CANTIDAD,
        //                                    ID_CALENDARIZACION_ENTREGA=entidadCalendarizar_Entrega.ID_CALENDARIZACION_ENTREGA,
        //                                    ID_PRODUCTO=item.ID_PRODUCTO,
        //                                    ID_CONSEC=consec,
        //                                    ESTATUS="PR"
        //                                };
        //                                item.ID_CALENDARIZACION_ENTREGA = entidadCalendarizar_Entrega.ID_CALENDARIZACION_ENTREGA;
        //                                entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Add(calendarizar_entrega_producto);
        //                            }
        //                            else// si lo encontro hay que checar posible modificaciones a estatus y cantidad
        //                            {
        //                                //checar si fue cancelado
        //                                if (item.ESTATUS=="CA")
        //                                {
                                            
        //                                    //hay que afectar calculos en la tabla orden_compra_detalle
        //                                    var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == encontrado.ID_PRODUCTO && f.ID_ALMACEN == encontrado.CALENDARIZAR_ENTREGA.ID_ALMACEN &&
        //                                        f.ID_ORDEN_COMPRA == encontrado.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA);
        //                                    if (detalle == null)
        //                                        throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                                    detalle.CANTIDAD_TRANSITO -= encontrado.CANTIDAD;
        //                                    detalle.DIFERENCIA += encontrado.CANTIDAD;
        //                                    //ingresar la incidencia en caso de existir
        //                                    if (item.INCIDENCIA_TIPO != null) //si hubo incidencia registrarla
        //                                    {
        //                                        if (!seq_incidencia.HasValue) //si no se ha registrado una incidencia para esta calendarizacion
        //                                        {
        //                                            var cancelacion_incidencia_producto = new List<INCIDENCIA_PRODUCTO>(){new INCIDENCIA_PRODUCTO
        //                                                {
        //                                                    CANTIDAD = item.CANTIDAD,
        //                                                    ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
        //                                                    ID_PRODUCTO = item.ID_PRODUCTO,
        //                                                    ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
        //                                                    OBSERV = item.INCIDENCIA_OBSERVACIONES
        //                                                }};
        //                                            seq_incidencia = sequencia = GetSequence<int>("INCIDENCIA_SEQ");
        //                                            Context.INCIDENCIA.Add(new INCIDENCIA
        //                                            {
        //                                                FECHA = DateTime.Now.Date,
        //                                                ID_ALMACEN = entidad.ID_ALMACEN,
        //                                                ID_INCIDENCIA = seq_incidencia.Value,
        //                                                ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
        //                                                ID_USUARIO = entidad.ID_USUARIO,
        //                                                INCIDENCIA_PRODUCTO = cancelacion_incidencia_producto
        //                                            });
        //                                            Context.SaveChanges();
        //                                        }
        //                                        else //si ya hay registro de incidencias para esta calendarizacion.
        //                                        {
        //                                            var cancelacion_incidencia = Context.INCIDENCIA.First(f => f.ID_INCIDENCIA == seq_incidencia.Value);
        //                                            cancelacion_incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
        //                                            {
        //                                                CANTIDAD = item.CANTIDAD,
        //                                                ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
        //                                                ID_PRODUCTO = item.ID_PRODUCTO,
        //                                                ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
        //                                                OBSERV = item.INCIDENCIA_OBSERVACIONES
        //                                            });
        //                                            Context.SaveChanges();
        //                                        }
        //                                    }
        //                                    //cancelar el registro de la calendarizacion
        //                                    encontrado.ESTATUS = "CA";
        //                                    encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
        //                                    Context.SaveChanges();
        //                                }
        //                                else
        //                                {
        //                                    if (!item.FechaRecalendarizacion.HasValue && encontrado.CANTIDAD != item.CANTIDAD) //checar por cambios en cantidad sin recalendarizacion individual
        //                                    {
        //                                        //modificar calculos en la tabla compra detalle
        //                                        var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
        //                                        f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
        //                                        if (detalle == null)
        //                                            throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                                        detalle.CANTIDAD_TRANSITO -= (encontrado.CANTIDAD - item.CANTIDAD);
        //                                        detalle.DIFERENCIA += (encontrado.CANTIDAD - item.CANTIDAD);
        //                                        encontrado.CANTIDAD = item.CANTIDAD;
        //                                    }
        //                                    else
        //                                    {
        //                                        if (item.FechaRecalendarizacion.HasValue) //si hubo una recalendarizacion individual
        //                                        {
        //                                            if (item.INCIDENCIA_TIPO != null) //si hubo incidencia registrarla
        //                                            {
        //                                                if (!seq_incidencia.HasValue) //si no se ha registrado una incidencia para esta calendarizacion
        //                                                {
        //                                                    var recalendarizacion_incidencia_producto = new List<INCIDENCIA_PRODUCTO>(){new INCIDENCIA_PRODUCTO
        //                                                {
        //                                                    CANTIDAD = item.CANTIDAD,
        //                                                    ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
        //                                                    ID_PRODUCTO = item.ID_PRODUCTO,
        //                                                    ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
        //                                                    OBSERV = item.INCIDENCIA_OBSERVACIONES
        //                                                }};
        //                                                    seq_incidencia = sequencia = GetSequence<int>("INCIDENCIA_SEQ");
        //                                                    Context.INCIDENCIA.Add(new INCIDENCIA
        //                                                    {
        //                                                        FECHA = DateTime.Now.Date,
        //                                                        ID_ALMACEN = entidad.ID_ALMACEN,
        //                                                        ID_INCIDENCIA = seq_incidencia.Value,
        //                                                        ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
        //                                                        ID_USUARIO = entidad.ID_USUARIO,
        //                                                        INCIDENCIA_PRODUCTO = recalendarizacion_incidencia_producto
        //                                                    });
        //                                                    Context.SaveChanges();
        //                                                }
        //                                                else //si ya hay registro de incidencias para esta calendarizacion.
        //                                                {
        //                                                    var recalendarizacion_incidencia = Context.INCIDENCIA.First(f => f.ID_INCIDENCIA == seq_incidencia.Value);
        //                                                    recalendarizacion_incidencia.INCIDENCIA_PRODUCTO.Add(new INCIDENCIA_PRODUCTO
        //                                                    {
        //                                                        CANTIDAD = item.CANTIDAD,
        //                                                        ID_ALMACEN_GRUPO = item.INCIDENCIA_TIPO.ID_ALMACEN_GRUPO,
        //                                                        ID_PRODUCTO = item.ID_PRODUCTO,
        //                                                        ID_TIPO_INCIDENCIA = item.INCIDENCIA_TIPO.ID_TIPO_INCIDENCIA,
        //                                                        OBSERV = item.INCIDENCIA_OBSERVACIONES
        //                                                    });
        //                                                }
        //                                            }
        //                                            var calendarizacion_existente = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(f => f.FEC_PACTADA == item.FechaRecalendarizacion && f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA
        //                                                && f.ID_ALMACEN == entidad.ID_ALMACEN);
        //                                            if (calendarizacion_existente == null) //si no hay una calendarizacion existente para esa fecha hay que insertar
        //                                            {
        //                                                var recalendarizar_entrega_producto = new List<CALENDARIZAR_ENTREGA_PRODUCTO>(){new CALENDARIZAR_ENTREGA_PRODUCTO {
        //                                                ESTATUS="PR",
        //                                                ID_CONSEC=1,
        //                                                CANTIDAD=item.CANTIDAD,
        //                                                ID_PRODUCTO=item.ID_PRODUCTO
        //                                            }};
        //                                                sequencia = GetSequence<int>("CALENDARIZAR_ENTREGA_SEQ");
        //                                                Context.CALENDARIZAR_ENTREGA.Add(new CALENDARIZAR_ENTREGA
        //                                                {
        //                                                    CALENDARIZAR_ENTREGA_PRODUCTO = recalendarizar_entrega_producto,
        //                                                    ESTATUS = "PR",
        //                                                    FEC_PACTADA = item.FechaRecalendarizacion.Value,
        //                                                    FECHA = DateTime.Now.Date,
        //                                                    ID_ALMACEN = entidad.ID_ALMACEN,
        //                                                    ID_CALENDARIZACION_ENTREGA = sequencia,
        //                                                    ID_ORDEN_COMPRA = entidad.ID_ORDEN_COMPRA,
        //                                                    ID_USUARIO = entidad.ID_USUARIO,
        //                                                });
        //                                                Context.SaveChanges();
        //                                                //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
        //                                                encontrado.ESTATUS = "RL"; //RECALENDARIZADO
        //                                                encontrado.CAL_ID_CONSEC = 1;
        //                                                encontrado.CAL_ID_CALENDARIZACION_ENTREGA = sequencia;
        //                                                encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
        //                                            }
        //                                            else //si ya hay calendarizacion hay que agregarle el detalle del producto
        //                                            {
        //                                                var calendarizacion_entrega_producto = calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO
        //                                                    && f.ESTATUS == "PR");
        //                                                if (calendarizacion_entrega_producto == null) //si el producto no esta ya programado para esa calendarizacion hay que insertarlo
        //                                                {
        //                                                    var consec = calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO) == null ? 1 :
        //                                                        calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.Where(f => f.ID_PRODUCTO == item.ID_PRODUCTO).Max(m => m.ID_CONSEC) + 1;
        //                                                    calendarizacion_existente.CALENDARIZAR_ENTREGA_PRODUCTO.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
        //                                                    {
        //                                                        ESTATUS = "PR",
        //                                                        ID_CONSEC = consec,
        //                                                        CANTIDAD = item.CANTIDAD,
        //                                                        ID_PRODUCTO = item.ID_PRODUCTO
        //                                                    });
        //                                                    //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
        //                                                    encontrado.ESTATUS = "RL"; //RECALENDARIZADO
        //                                                    encontrado.CAL_ID_CONSEC = consec;
        //                                                    encontrado.CAL_ID_CALENDARIZACION_ENTREGA = calendarizacion_existente.ID_CALENDARIZACION_ENTREGA;
        //                                                    encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
        //                                                }
        //                                                else //si el producto ya esta programado para esta calendarizacion hay que actualizar la cantidad
        //                                                {
        //                                                    calendarizacion_entrega_producto.CANTIDAD += item.CANTIDAD;
        //                                                    //hay que dejar insertar la liga de recalendarizacion en el registro original, actualizar estatus y ligar la incidencia
        //                                                    encontrado.ESTATUS = "RL"; //RECALENDARIZADO
        //                                                    encontrado.CAL_ID_CONSEC = calendarizacion_entrega_producto.ID_CONSEC;
        //                                                    encontrado.CAL_ID_CALENDARIZACION_ENTREGA = calendarizacion_existente.ID_CALENDARIZACION_ENTREGA;
        //                                                    encontrado.ID_INCIDENCIA = item.INCIDENCIA_TIPO != null ? seq_incidencia.Value : (int?)null;
        //                                                }
        //                                            }
        //                                            //hay que revisar si hubo cambios a la cantidad y reajustar el detalle de la orden de compra
        //                                            if (encontrado.CANTIDAD != item.CANTIDAD)
        //                                            {
        //                                                //modificar calculos en la tabla compra detalle
        //                                                var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidad.ID_ALMACEN &&
        //                                                f.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA);
        //                                                if (detalle == null)
        //                                                    throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                                                detalle.CANTIDAD_TRANSITO -= (encontrado.CANTIDAD - item.CANTIDAD);
        //                                                detalle.DIFERENCIA += (encontrado.CANTIDAD - item.CANTIDAD);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (!entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Any(a=>a.ESTATUS=="PR" || a.ESTATUS=="RB" || a.ESTATUS=="RP")) // si se recalendarizaron o se cancelaron todas las calendarizaciones de productos de esa fecha hay que eliminar el registro
        //                        {
        //                            var OC_original = entidadCalendarizar_Entrega.ID_ORDEN_COMPRA;
        //                            entidadCalendarizar_Entrega.ESTATUS = "CA"; //cancelar la agenda
        //                            Context.SaveChanges();
        //                            if (!Context.CALENDARIZAR_ENTREGA.Any(a => a.ID_ORDEN_COMPRA == OC_original && a.ESTATUS!="CA")) //si no quedan calendarizaciones con esa OC que no esten canceladas
        //                            {
        //                                var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == OC_original);
        //                                if (OC == null)
        //                                    throw new Exception("La orden de compra se elimino durante este proceso");
        //                                OC.ESTATUS = "CP";//regresar a estatus orginal.
        //                            }

        //                        }
        //                        else
        //                        {
        //                            if (entidadCalendarizar_Entrega.ESTATUS == "CA") //si estaba cancelada toda la calendarizacion y se volvio a insertar una nueva agenda
        //                                entidadCalendarizar_Entrega.ESTATUS = "PR";
        //                            if (entidad.FEC_PACTADA != fecha_original && !entidad.CALENDARIZAR_ENTREGA_PRODUCTO.Any(a=>a.IsEditable==false)) //si cambio, hay que mover la fecha pactada para recalendarizar
        //                                entidadCalendarizar_Entrega.FEC_PACTADA = entidad.FEC_PACTADA;
        //                        }
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region si ya hay una agenda para los productos a recalendarizar
        //                    var entidadCalendarizar_Entrega = Context.CALENDARIZAR_ENTREGA.FirstOrDefault(w => w.ID_ALMACEN == entidad.ID_ALMACEN && w.ID_ORDEN_COMPRA == entidad.ID_ORDEN_COMPRA &&
        //                        w.FEC_PACTADA == fecha_original); //obtener el registro original
        //                    if (entidadCalendarizar_Entrega == null)
        //                        throw new Exception("El registro original de calendarizacion fue borrado durante el proceso");
        //                    var _copia = new CALENDARIZAR_ENTREGA_PRODUCTO[entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.Count];
        //                    entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.CopyTo(_copia,0);
        //                    foreach (var item in _copia)
        //                    {
        //                        #region checar entidad para ver si se borro el registro y hacer calculos necesarios
        //                        var encontrado = entidad.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO);
        //                        if(encontrado==null)
        //                        {
        //                            #region si no lo encuentra hay que reajustar el la orden compra detalle
        //                            var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == item.CALENDARIZAR_ENTREGA.ID_ALMACEN &&
        //                                   f.ID_ORDEN_COMPRA == item.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA);
        //                            if (detalle == null)
        //                                throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                            detalle.CANTIDAD_TRANSITO -= item.CANTIDAD;
        //                            detalle.DIFERENCIA += item.CANTIDAD;
        //                            #endregion
        //                        }
        //                        #endregion
        //                    }
        //                    foreach(var item in entidad.CALENDARIZAR_ENTREGA_PRODUCTO.Where(w=>w.IsEditable==true)) //solo los que pueden ser editados
        //                    {
        //                        #region checar entidad para ver si es nuevo registro o se modifico, hacer calculos necesarios e recalendarizar en la agenda ya existente de ese dia
        //                        var encontrado = entidadCalendarizar_Entrega.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO);
        //                        if(encontrado==null)
        //                        {
        //                            #region si no lo encontro es nuevo
        //                            #region realizar calculos sobre orden compra detalle
        //                            var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidadCalendarizar_Entrega.ID_ALMACEN &&
        //                                   f.ID_ORDEN_COMPRA == entidadCalendarizar_Entrega.ID_ORDEN_COMPRA);
        //                            if (detalle == null)
        //                                throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                            detalle.CANTIDAD_TRANSITO += item.CANTIDAD;
        //                            detalle.DIFERENCIA -= item.CANTIDAD;
        //                            #endregion
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region si lo encontro hay que revisar cambios para afectar orden compra detalle
        //                            if(encontrado.CANTIDAD!=item.CANTIDAD)
        //                            {
        //                                var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == entidadCalendarizar_Entrega.ID_ALMACEN &&
        //                                   f.ID_ORDEN_COMPRA == entidadCalendarizar_Entrega.ID_ORDEN_COMPRA);
        //                                if (detalle == null)
        //                                    throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //                                detalle.CANTIDAD_TRANSITO -= (encontrado.CANTIDAD - item.CANTIDAD);
        //                                detalle.DIFERENCIA += (encontrado.CANTIDAD - item.CANTIDAD);
        //                            }
        //                            #endregion
        //                        }
        //                        #endregion
        //                        #region revisar si hay que agregar a detalle agenda de productos del dia recalendarizado o actualizar
        //                        var encontradoreageanda = entidadCalendarizarVerificar.CALENDARIZAR_ENTREGA_PRODUCTO.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO);
        //                        if (encontradoreageanda == null)
        //                            #region si no lo encontro hay que insertar agenda de producto al dia a recalendarizar
        //                            entidadCalendarizarVerificar.CALENDARIZAR_ENTREGA_PRODUCTO.Add(new CALENDARIZAR_ENTREGA_PRODUCTO
        //                            {
        //                                ID_CALENDARIZACION_ENTREGA = entidadCalendarizarVerificar.ID_CALENDARIZACION_ENTREGA,
        //                                ID_PRODUCTO = item.ID_PRODUCTO,
        //                                CANTIDAD = item.CANTIDAD
        //                            });
        //                            #endregion
        //                        else
        //                            #region si lo encontro hay que agregar a cantidades ya existentes
        //                            encontradoreageanda.CANTIDAD += item.CANTIDAD;
        //                            #endregion
        //                        #endregion
        //                    }
        //                    var OC_original = entidadCalendarizar_Entrega.ID_ORDEN_COMPRA;
        //                    Context.CALENDARIZAR_ENTREGA.Remove(entidadCalendarizar_Entrega); //remover el registro original
        //                    Context.SaveChanges();
        //                    if (!Context.CALENDARIZAR_ENTREGA.Any(a => a.ID_ORDEN_COMPRA == OC_original)) //si no quedan calendarizaciones con esa OC
        //                    {
        //                        var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == OC_original);
        //                        if (OC == null)
        //                            throw new Exception("La orden de compra se elimino durante este proceso");
        //                        OC.ESTATUS = "CP";//regresar a estatus orginal.
        //                    }
        //                    #endregion
        //                }
        //            }
        //            Context.SaveChanges();
        //            //posible eliminacion
        //            //var id_almacen = entidades.First().ID_ALMACEN;
        //            //var calendarizaciones_activas_almacen = Context.CALENDARIZAR_ENTREGA.Where(w => w.ID_ALMACEN == w.ID_ALMACEN && w.FEC_PACTADA == fecha_original);
        //            //var _copia_activas = new CALENDARIZAR_ENTREGA[calendarizaciones_activas_almacen.Count()];
        //            //calendarizaciones_activas_almacen.ToList().CopyTo(_copia_activas, 0);
        //            ////buscar por calendarizaciones de ordenes que fueron eliminadas de la programacion del dia
        //            //foreach (var item in _copia_activas)
        //            //{
        //            //    var encontrado = entidades.FirstOrDefault(f => f.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA);
        //            //    if (encontrado == null) //si no lo encontro borrar
        //            //    {
        //            //        foreach (var producto_eliminar in item.CALENDARIZAR_ENTREGA_PRODUCTO) //hay que cambiar las cantidades de transito a no programados
        //            //        {
        //            //            var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == producto_eliminar.ID_PRODUCTO && f.ID_ALMACEN == item.ID_ALMACEN &&
        //            //                       f.ID_ORDEN_COMPRA == item.ID_ORDEN_COMPRA);
        //            //            if (detalle == null)
        //            //                throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
        //            //            detalle.CANTIDAD_TRANSITO -= producto_eliminar.CANTIDAD;
        //            //            detalle.DIFERENCIA += producto_eliminar.CANTIDAD;
        //            //        }
        //            //        Context.CALENDARIZAR_ENTREGA.Remove(calendarizaciones_activas_almacen.First(w=>w.ID_ORDEN_COMPRA==item.ID_ORDEN_COMPRA));
        //            //    }
        //            //}
        //            //Context.SaveChanges();
        //            transaccion.Complete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
        //    }
        //}

        public void EliminarporFecha(List<EXT_CALENDARIZAR_ENTREGA> entidades)
        {
            try
            {

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    
                    foreach (var entidad in entidades)
                    {
                        if (!entidad.CALENDARIZAR_ENTREGA_PRODUCTO.Any(a=>a.IsEditable==false))
                        {
                            var temp = Context.CALENDARIZAR_ENTREGA.First(w=>w.ID_ORDEN_COMPRA==entidad.ID_ORDEN_COMPRA && w.ID_ALMACEN==entidad.ID_ALMACEN && w.FEC_PACTADA==entidad.FEC_PACTADA);
                            foreach (var item in temp.CALENDARIZAR_ENTREGA_PRODUCTO)
                            {
                                var detalle = Context.ORDEN_COMPRA_DETALLE.FirstOrDefault(f => f.ID_PRODUCTO == item.ID_PRODUCTO && f.ID_ALMACEN == item.CALENDARIZAR_ENTREGA.ID_ALMACEN &&
                                            f.ID_ORDEN_COMPRA == item.CALENDARIZAR_ENTREGA.ID_ORDEN_COMPRA);
                                if (detalle == null)
                                    throw new Exception("El detalle de la orden de compra a la que hace referencia esta calendarizacion fue eliminado durante el proceso");
                                detalle.CANTIDAD_TRANSITO -= item.CANTIDAD;
                                detalle.DIFERENCIA += item.CANTIDAD;
                            }
                            var OC_original = entidad.ID_ORDEN_COMPRA;
                            Context.CALENDARIZAR_ENTREGA.Remove(temp);
                            Context.SaveChanges();
                            if (!Context.CALENDARIZAR_ENTREGA.Any(a => a.ID_ORDEN_COMPRA == OC_original)) //si no quedan calendarizaciones con esa OC
                            {
                                var OC = Context.ORDEN_COMPRA.FirstOrDefault(f => f.ID_ORDEN_COMPRA == OC_original);
                                if (OC == null)
                                    throw new Exception("La orden de compra se elimino durante este proceso");
                                OC.ESTATUS = "CP";//regresar a estatus orginal.
                            }
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
