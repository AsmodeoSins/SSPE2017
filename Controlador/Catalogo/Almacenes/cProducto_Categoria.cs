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
    public class cProducto_Categoria:EntityManagerServer<PRODUCTO_CATEGORIA>
    {
        public bool Insertar(PRODUCTO_CATEGORIA _entidad)
        {
            try
            {
                _entidad.ID_CATEGORIA = GetIDProceso<int>("PRODUCTO_CATEGORIA","ID_CATEGORIA", "1=1");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Actualizar(PRODUCTO_CATEGORIA _entidad)
        {
            try
            {
                Update(_entidad);
                return true;
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRODUCTO_CATEGORIA _entidad)
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

        public IQueryable<PRODUCTO_CATEGORIA> Seleccionar(string almacen_grupo="",string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_CATEGORIA>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_PROD_GRUPO == almacen_grupo);
                return GetData(predicate.Expand(),o=>o.NOMBRE);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PRODUCTO_CATEGORIA> Seleccionar(string busqueda, string almacen_grupo="", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<PRODUCTO_CATEGORIA>();
                if (string.IsNullOrEmpty(busqueda.TrimEnd()))
                    return Seleccionar(almacen_grupo, estatus);
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ACTIVO == estatus);
                if (!string.IsNullOrEmpty(almacen_grupo))
                    predicate = predicate.And(w => w.ID_PROD_GRUPO == almacen_grupo);
                predicate=predicate.And(w=>w.NOMBRE.Contains(busqueda));
                return GetData(predicate.Expand(),o=>o.NOMBRE);

            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}
