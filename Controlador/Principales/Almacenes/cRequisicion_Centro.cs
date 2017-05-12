using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
using System.Transactions;
using SSP.Servidor.ModelosExtendidos;
namespace SSP.Controlador.Principales.Almacenes
{
    public class cRequisicion_Centro:EntityManagerServer<REQUISICION_CENTRO>
    {
        public int Insertar(REQUISICION_CENTRO requisicion_centro)
        {
            try
            {
                var seq = DateTime.Now.ToString("yy");
                seq += Context.ALMACEN.First(w => w.ID_ALMACEN == requisicion_centro.ID_ALMACEN).ID_CENTRO.ToString().PadLeft(2, '0');
                seq += GetSequence<short>("REQUISICION_CENTRO_SEQ").ToString().PadLeft(3, '0');
                requisicion_centro.ID_REQUISICION = int.Parse(seq);
                foreach (var item in requisicion_centro.REQUISICION_CENTRO_PRODUCTO)
                    item.ID_REQUISICION = requisicion_centro.ID_REQUISICION;
                Insert(requisicion_centro);
                return int.Parse(seq);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void Actualizar(List<REQUISICION_CENTRO_PRODUCTO> requisicion_centro_productos)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var id_requisicion_centro=requisicion_centro_productos.First().ID_REQUISICION;
                    var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(w => w.ID_REQUISICION == id_requisicion_centro );
                    REQUISICION_CENTRO_PRODUCTO[] _copia = new REQUISICION_CENTRO_PRODUCTO[requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Count()];
                    requisicion_centro.REQUISICION_CENTRO_PRODUCTO.CopyTo(_copia, 0);
                    foreach(var item in _copia)
                    {
                        var encontrado = requisicion_centro_productos.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                        if (encontrado == null) //si no lo encontro en la nueva lista hay que eliminar
                            requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Remove(requisicion_centro.REQUISICION_CENTRO_PRODUCTO.First(w=>w.ID_PRODUCTO==item.ID_PRODUCTO));
                    }
                    foreach(var item in requisicion_centro_productos)
                    {
                        var encontrado = requisicion_centro.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                        if (encontrado == null) // si no lo encontro hay que insertar
                            requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Add(item);
                        else //si lo encontro hay que checar por cambios
                            if (encontrado.CANTIDAD != item.CANTIDAD)
                                encontrado.CANTIDAD = item.CANTIDAD;
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

        public void Actualizar(List<REQUISICION_CENTRO_PRODUCTO> requisicion_centro_productos, string estatus)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var id_requisicion_centro = requisicion_centro_productos.First().ID_REQUISICION;
                    var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(w => w.ID_REQUISICION == id_requisicion_centro);
                    REQUISICION_CENTRO_PRODUCTO[] _copia = new REQUISICION_CENTRO_PRODUCTO[requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Count()];
                    requisicion_centro.REQUISICION_CENTRO_PRODUCTO.CopyTo(_copia, 0);
                    foreach (var item in _copia)
                    {
                        var encontrado = requisicion_centro_productos.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                        if (encontrado == null) //si no lo encontro en la nueva lista hay que eliminar
                            requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Remove(requisicion_centro.REQUISICION_CENTRO_PRODUCTO.First(w => w.ID_PRODUCTO == item.ID_PRODUCTO));
                    }
                    foreach (var item in requisicion_centro_productos)
                    {
                        var encontrado = requisicion_centro.REQUISICION_CENTRO_PRODUCTO.FirstOrDefault(w => w.ID_PRODUCTO == item.ID_PRODUCTO);
                        if (encontrado == null) // si no lo encontro hay que insertar
                            requisicion_centro.REQUISICION_CENTRO_PRODUCTO.Add(item);
                        else //si lo encontro hay que checar por cambios
                            if (encontrado.CANTIDAD != item.CANTIDAD)
                                encontrado.CANTIDAD = item.CANTIDAD;
                    }
                    requisicion_centro.ESTATUS = estatus;
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void CancelarRequisicionCentro(int id_requisicion_centro)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var requisicion_centro = Context.REQUISICION_CENTRO.FirstOrDefault(w => w.ID_REQUISICION == id_requisicion_centro);
                    foreach (var item in requisicion_centro.REQUISICION_CENTRO_PRODUCTO)
                    {
                        item.ESTATUS = "CA";
                    }
                    requisicion_centro.ESTATUS = "CA";
                    Context.SaveChanges();
                    transaccion.Complete();
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<REQUISICION_CENTRO> SeleccionarPorEstatus(List<string> estatus,short? id_tipo_requisicion=null,int? id_centro=null,string almacen_grupo="",short? id_almacen = null)
        {
            try
            {
                var predicateOr = PredicateBuilder.False<REQUISICION_CENTRO>();
                var predicate = PredicateBuilder.True<REQUISICION_CENTRO>();
                if (estatus == null || estatus.Count == 0)
                    throw new Exception("Se debe de buscar por lo menos por un estatus de orden de compra.");
                foreach (var item in estatus)
                    predicateOr = predicateOr.Or(w => w.ESTATUS == item);
                predicate = predicate.And(predicateOr.Expand());
                if (id_tipo_requisicion.HasValue)
                    predicate = predicate.And(w => w.ID_TIPO == id_tipo_requisicion);
                if (id_centro.HasValue)
                    predicate = predicate.And(w => w.ALMACEN.CENTRO.ID_CENTRO == id_centro.Value);
                if (!string.IsNullOrWhiteSpace(almacen_grupo))
                    predicate = predicate.And(w => w.ALMACEN.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO == almacen_grupo);
                if (id_almacen.HasValue)
                    predicate = predicate.And(w => w.ALMACEN.ID_ALMACEN == id_almacen);
                return GetData(predicate.Expand()).OrderBy(o => o.ID_REQUISICION).ThenBy(o => o.ALMACEN.CENTRO.DESCR).ThenBy(o => o.ALMACEN.ALMACEN_TIPO_CAT.ID_ALMACEN_GRUPO)
                    .ThenBy(o => o.ALMACEN.DESCRIPCION);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public List<EXT_RequisicionExtraordinaria_Encabezado> SeleccionarEncabezadoReporteRequisicionExtraordinaria(int id_requisicion)
        {
            try
            {

                var res = GetData(w => w.ID_REQUISICION == id_requisicion).First();
                var _usuario = Context.USUARIO.First(f=>f.ID_USUARIO==res.ID_USUARIO);
                string nombre_completo= string.Empty;
                if(!string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.NOMBRE))
                    nombre_completo += _usuario.EMPLEADO.PERSONA.NOMBRE.TrimEnd();
                if (!string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.PATERNO))
                    nombre_completo += " " + _usuario.EMPLEADO.PERSONA.PATERNO.TrimEnd();
                if (!string.IsNullOrWhiteSpace(_usuario.EMPLEADO.PERSONA.MATERNO))
                    nombre_completo += " " + _usuario.EMPLEADO.PERSONA.MATERNO.TrimEnd();
                var logo_BC = Context.PARAMETRO.First(w => w.ID_CLAVE=="LOGO_ESTADO").CONTENIDO;
                return new List<EXT_RequisicionExtraordinaria_Encabezado> {
                    new EXT_RequisicionExtraordinaria_Encabezado{
                        Almacen=res.ALMACEN.DESCRIPCION,
                        Almacen_Tipo_Cat=res.ALMACEN.ALMACEN_TIPO_CAT.DESCR,
                        Centro=res.ALMACEN.CENTRO.DESCR,
                        Estatus=res.REQ_CENTRO_ESTATUS.DESCR,
                        FechaRequisicion=res.FECHA.Value,
                        Periodo="Extaordinario",
                        SolicitadoPor=nombre_completo,
                        Folio=id_requisicion,
                        LOGO_BC=logo_BC
                    }
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
