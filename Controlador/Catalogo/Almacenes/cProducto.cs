using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;
using System.Transactions;
using System.Data.Objects;

//falta implementar transacciones

namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cProducto:EntityManagerServer<PRODUCTO>
    {
        public bool Insertar(PRODUCTO _entidad,List<ALMACEN_TIPO_CAT> _almacen_tipos_cat, PRODUCTO_IMAGEN _producto_imagen=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel=IsolationLevel.ReadCommitted}))
                {
                    _entidad.ID_PRODUCTO = GetSequence<int>("PRODUCTO_SEQ");
                    foreach (var item in _almacen_tipos_cat)
                    {
                        var almacen_tipo_cat = Context.ALMACEN_TIPO_CAT.FirstOrDefault(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO);
                        if (almacen_tipo_cat == null)
                            throw new Exception("Un tipo de producto fue borrado en el transcurso de esta operacion");
                        //_entidad.ALMACEN_TIPO_CAT.Add(almacen_tipo_cat);
                    }
                    if (_producto_imagen!=null)
                    {
                        _producto_imagen.ID_PRODUCTO = _entidad.ID_PRODUCTO;
                        _entidad.PRODUCTO_IMAGEN = _producto_imagen;
                    }
                    Context.PRODUCTO.Add(_entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
             
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(PRODUCTO _entidad, List<ALMACEN_TIPO_CAT> _almacen_tipos_cat, byte[] _producto_imagen = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var _producto = Context.PRODUCTO.Where(w => w.ID_PRODUCTO == _entidad.ID_PRODUCTO).FirstOrDefault();
                    if (_producto == null)
                        throw new Exception("El producto fue eliminado por otro usuario en el transcurso de esta operacion"); //en caso de que el producto haya sido borrado por otro usuario en el transcurso de la operacion.
                    //if (_producto.ALMACEN_TIPO_CAT.Count() > 0)
                    //{
                    //    ALMACEN_TIPO_CAT[] _copia = new ALMACEN_TIPO_CAT[_producto.ALMACEN_TIPO_CAT.Count()];
                    //    _producto.ALMACEN_TIPO_CAT.CopyTo(_copia, 0);
                    //    foreach (var item in _copia)
                    //    {
                    //        var _encontro = _almacen_tipos_cat.FirstOrDefault(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO);
                    //        if (_encontro == null)
                    //            _producto.ALMACEN_TIPO_CAT.Remove(_producto.ALMACEN_TIPO_CAT.First(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO));
                    //    }
                    //    foreach (var item in _almacen_tipos_cat)
                    //    {
                    //        var _encontro = _copia.FirstOrDefault(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO);
                    //        if (_encontro == null)
                    //        {
                    //            //Importante obtener la entidad de producto_tipo_cat desde el contexto. Intentara insertar en el catalogo
                    //            //de producto_tipo_cat uno nuevo causando una violacion de llave primaria si usamos el que enviamos en la lista ya que es una entidad diferente
                    //            //al momento de insertar a la tabla producto_tipo
                    //            var _add_item = Context.ALMACEN_TIPO_CAT.First(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO);
                    //            _producto.ALMACEN_TIPO_CAT.Add(_add_item);
                    //        }
                    //    }
                    //}
                    if (_producto_imagen != null)
                    {
                        if (_producto.PRODUCTO_IMAGEN == null)
                        {
                            _producto.PRODUCTO_IMAGEN = new PRODUCTO_IMAGEN {
                                ID_PRODUCTO=_producto.ID_PRODUCTO,
                                IMAGEN=_producto_imagen 
                            };
                        }
                        else
                            _producto.PRODUCTO_IMAGEN.IMAGEN = _producto_imagen;
                    }
                    _producto.ACTIVO = _entidad.ACTIVO;
                    _producto.DESCRIPCION = _entidad.DESCRIPCION;
                    _producto.ID_CATEGORIA = _entidad.ID_CATEGORIA;
                    _producto.ID_SUBCATEGORIA = _entidad.ID_SUBCATEGORIA;
                   // _producto.ID_PRESENTACION = _entidad.ID_PRESENTACION;
                    _producto.ID_UNIDAD_MEDIDA = _entidad.ID_UNIDAD_MEDIDA;
                    _producto.NOMBRE = _entidad.NOMBRE;
                    Context.PRODUCTO.Attach(_producto);
                    var entry = Context.Entry(_producto);
                    entry.State = System.Data.EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();                   
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool ActualizarEstatus(int id_producto, string estatus)
        {
            try
            {
                var entidad = Context.PRODUCTO.FirstOrDefault(w => w.ID_PRODUCTO == id_producto);
                if (entidad == null)
                    throw new Exception("El producto fue eliminado durante este proceso");
                entidad.ACTIVO = "N";
                Context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(int _producto_id)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var _producto = Context.PRODUCTO.Where(w => w.ID_PRODUCTO == _producto_id).FirstOrDefault();
                    if (_producto == null)
                        throw new Exception("El producto fue eliminado por otro usuario en el transcurso de esta operacion"); //en caso de que el producto haya sido borrado por otro usuario en el transcurso de la operacion.

                    //if (_producto.ALMACEN_TIPO_CAT.Count() > 0)
                    //{
                    //    ALMACEN_TIPO_CAT[] _copia = new ALMACEN_TIPO_CAT[_producto.ALMACEN_TIPO_CAT.Count()];
                    //    _producto.ALMACEN_TIPO_CAT.CopyTo(_copia, 0);
                    //    foreach (var item in _copia) 
                    //        //hay que eliminar todos los registros de la tabla producto_tipo antes de borrar la entidad producto
                    //        _producto.ALMACEN_TIPO_CAT.Remove(_producto.ALMACEN_TIPO_CAT.First(f => f.ID_ALMACEN_TIPO == item.ID_ALMACEN_TIPO));
                    //    Context.SaveChanges();
                    //}
                    Context.PRODUCTO.Remove(_producto);
                    Context.SaveChanges();
                    transaccion.Complete();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO> Seleccionar(string almacen_grupo = "" ,string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                //if (!string.IsNullOrEmpty(almacen_grupo))
                //    predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.Any(a => a.ID_ALMACEN_GRUPO == almacen_grupo));
                return GetData(predicate.Expand()).OrderBy(o=>o.ID_PRODUCTO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO> Seleccionar(short id_tipo_almacen,string almacen_grupo = "", string estatus = "")
        {
            try
            {
                
                var predicate = PredicateBuilder.True<PRODUCTO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                //if (!string.IsNullOrEmpty(almacen_grupo))
                //    predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.Any(a => a.ID_ALMACEN_GRUPO == almacen_grupo));
                //predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.Any(a=>a.ID_ALMACEN_TIPO==id_tipo_almacen));
                return GetData(predicate.Expand()).OrderBy(o => o.ID_PRODUCTO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO> Seleccionar(string busqueda, string almacen_grupo = "", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()))
                    return Seleccionar(almacen_grupo, estatus);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                //if (!string.IsNullOrEmpty(almacen_grupo))
                //    predicate = predicate.And(w => w.ALMACEN_TIPO_CAT.Any(a => a.ID_ALMACEN_GRUPO == almacen_grupo));
                predicate = predicate.And(w => w.NOMBRE.Contains(busqueda));
                return GetData(predicate.Expand()).OrderBy(o => o.ID_PRODUCTO);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
