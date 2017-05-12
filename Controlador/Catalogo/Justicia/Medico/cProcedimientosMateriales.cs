using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cProcedimientosMateriales : EntityManagerServer<PROC_MATERIAL>
    {
        public cProcedimientosMateriales() { }

        /// <summary>
        /// Obtiene todo el catalogo de procedimientos materiales
        /// </summary>
        /// <returns>Listado del mundo de procedimientos materiales</returns>
        public IQueryable<PROC_MATERIAL> ObtenerTodos()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene la busqueda de procedimientos materiales por medio de un string. Busca tanto en productos como en procedimientos medicos.
        /// </summary>
        /// <param name="buscar">Busqueda de material</param>
        /// <returns>Listado de procedimientos materiales filtrados</returns>
        public IQueryable<PROC_MATERIAL> ObtenerXBusqueda(string buscar)
        {
            try
            {
                return string.IsNullOrEmpty(buscar) ? GetData() : GetData(g => g.PROC_MED != null ? g.PROC_MED.DESCR.Contains(buscar) : false ||
                    g.PRODUCTO != null ? g.PRODUCTO.DESCRIPCION.Contains(buscar) : false ||
                    g.PRODUCTO != null ? g.PRODUCTO.NOMBRE.Contains(buscar) : false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene la busqueda de procedimientos materiales por medio de un string. Busca tanto en productos como en procedimientos medicos.
        /// </summary>
        /// <param name="buscar">Busqueda de material</param>
        /// <returns>Listado de procedimientos materiales filtrados</returns>
        public IQueryable<PROC_MATERIAL> ObtenerXBusquedaYSubtipo(string buscar, short subtipo)
        {
            try
            {
                return string.IsNullOrEmpty(buscar) ? GetData(g => (subtipo > 0 ? g.PROC_MED.ID_PROCMED_SUBTIPO == subtipo : true)) :
                    GetData(g => (g.PROC_MED != null ? g.PROC_MED.DESCR.Contains(buscar) : false ||
                    g.PRODUCTO != null ? g.PRODUCTO.DESCRIPCION.Contains(buscar) : false ||
                    g.PRODUCTO != null ? g.PRODUCTO.NOMBRE.Contains(buscar) : false) &&
                    (subtipo > 0 ? g.PROC_MED.ID_PROCMED_SUBTIPO == subtipo : true));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos materiales que contiene un procedimiento medico.
        /// </summary>
        /// <param name="ProcMed">ID del procedimiento medico</param>
        /// <returns>Listado de procedimientos materiales filtrados</returns>
        public IQueryable<PROC_MATERIAL> ObtenerXProcMed(short ProcMed)
        {
            try
            {
                return GetData(g => g.ID_PROCMED == ProcMed);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos materiales a los que pertenece un producto en especifico.
        /// </summary>
        /// <param name="Producto">ID del producto a buscar</param>
        /// <returns>Listado de procedimientos materiales filtrados</returns>
        public IQueryable<PROC_MATERIAL> ObtenerXProducto(short Producto)
        {
            try
            {
                return GetData(g => g.ID_PRODUCTO == Producto);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos materiales que contiene un procedimiento medico y un producto en especifico.
        /// </summary>
        /// <param name="Producto">ID del producto a buscar</param>
        /// <param name="ProcMed">ID del procedimiento medico</param>
        /// <returns>Un solo procedimiento material, si no lo encuentra regresa nulo</returns>
        public PROC_MATERIAL ObtenerXProductoYProcMed(short Producto, short ProcMed)
        {
            try
            {
                return GetData().Any(g => g.ID_PRODUCTO == Producto && g.ID_PROCMED == ProcMed) ? GetData().First(g => g.ID_PRODUCTO == Producto && g.ID_PROCMED == ProcMed) : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos materiales que se encuentran en activo.
        /// </summary>
        /// <returns>Listado de procedimientos materiales filtrados</returns>
        public IQueryable<PROC_MATERIAL> ObtenerTodosActivos()
        {
            try
            {
                return GetData(w => w.ESTATUS == "S");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Inserta la especialidad
        /// </summary>
        /// <param name="entidad">Entidad de procedimiento material</param>
        public bool Insertar(PROC_MATERIAL entidad)
        {
            try
            {
                return Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza la especialidad
        /// </summary>
        /// <param name="entidad">Entidad de especialidad</param>
        public void Actualizar(PROC_MATERIAL entidad)
        {
            try
            {
                Update(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}