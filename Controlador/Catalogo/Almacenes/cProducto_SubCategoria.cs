using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;

namespace SSP.Controlador.Catalogo.Almacenes
{
    public class cProducto_SubCategoria:EntityManagerServer<PRODUCTO_SUBCATEGORIA>
    {
        public bool Insertar(PRODUCTO_SUBCATEGORIA _entidad)
        {
            try
            {
                _entidad.ID_SUBCATEGORIA = GetSequence<int>("PRODUCTO_SUBCATEGORIA_SEQ");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(PRODUCTO_SUBCATEGORIA _entidad)
        {
            try
            {
                Update(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRODUCTO_SUBCATEGORIA _entidad)
        {
            try
            {
                Delete(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_SUBCATEGORIA> BuscarSubcategorias(string buscar = null, int? categoria = new Nullable<int>())
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_SUBCATEGORIA>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                if (categoria.HasValue ? categoria.Value > 0 : false)
                    predicate = predicate.And(a => a.ID_CATEGORIA == categoria.Value);
                return GetData(predicate.Expand()).OrderBy(t => t.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_SUBCATEGORIA> Seleccionar(string almacen_grupo="", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_SUBCATEGORIA>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!String.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.PRODUCTO_CATEGORIA.ID_PROD_GRUPO == almacen_grupo);
                return GetData(predicate.Expand()).OrderBy(o => o.PRODUCTO_CATEGORIA.NOMBRE).ThenBy(t => t.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_SUBCATEGORIA> Seleccionar(string busqueda, int categoria, string almacen_grupo="", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_SUBCATEGORIA>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()) && categoria==-1)
                    return Seleccionar(almacen_grupo,estatus);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!String.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.PRODUCTO_CATEGORIA.ID_PROD_GRUPO == almacen_grupo);
                if (categoria != -1)
                    predicate = predicate.And(w => w.PRODUCTO_CATEGORIA.ID_CATEGORIA == categoria);
                if (!string.IsNullOrEmpty(busqueda.TrimEnd()))
                    predicate = predicate.And(w => w.DESCR.Contains(busqueda));
                return GetData(predicate.Expand()).OrderBy(o => o.PRODUCTO_CATEGORIA.NOMBRE).ThenBy(t => t.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
