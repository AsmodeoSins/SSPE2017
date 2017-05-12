using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cProcedimientosMedicos : EntityManagerServer<PROC_MED>
    {
        public cProcedimientosMedicos() { }

        /// <summary>
        /// Obtiene todo el catalogo de procedimientos medicos
        /// </summary>
        /// <returns>Listado del mundo de procedimientos medicos</returns>
        public IQueryable<PROC_MED> ObtenerTodos()
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
        /// Obtiene un listado de procedimientos medicos que se encuentran en activo.
        /// </summary>
        /// <returns>Listado de procedimientos medicos filtrados</returns>
        public IQueryable<PROC_MED> ObtenerTodosActivos()
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
        /// Obtiene la busqueda de procedimientos medicos por medio de un string. Tambien busca en el catalogo de subtipo.
        /// </summary>
        /// <param name="buscar">Busqueda de procedimiento medico</param>
        /// <returns>Listado de procedimientos medicos filtrada</returns>
        public IQueryable<PROC_MED> ObtenerXBusqueda(string buscar)
        {
            try
            {
                return string.IsNullOrEmpty(buscar) ? GetData() : GetData(g => g.DESCR != null ? g.DESCR.Contains(buscar) : false ||
                    g.PROC_MED_SUBTIPO != null ? g.PROC_MED_SUBTIPO.DESCR.Contains(buscar) : false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos medicos que contiene un procedimiento medico subtipo.
        /// </summary>
        /// <param name="ProcMed">ID del procedimiento medico subtipo</param>
        /// <returns>Listado de procedimientos medicos filtrados</returns>
        public IQueryable<PROC_MED> ObtenerXSubtipo(short Subtipo)
        {
            try
            {
                return GetData(g => g.ID_PROCMED_SUBTIPO == Subtipo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos medicos que contiene un procedimiento medico subtipo.
        /// </summary>
        /// <param name="ProcMed">ID del procedimiento medico subtipo</param>
        /// <returns>Listado de procedimientos medicos filtrados</returns>
        public IQueryable<PROC_MED> ObtenerXBusquedaYSubtipo(string buscar, short Subtipo)
        {
            try
            {
                if(Subtipo>0)
                    return string.IsNullOrEmpty(buscar) ? GetData(g => g.ID_PROCMED_SUBTIPO == Subtipo) : GetData(g => g.ID_PROCMED_SUBTIPO == Subtipo && g.DESCR != null ? g.DESCR.Contains(buscar) : false);
                else
                    return string.IsNullOrEmpty(buscar) ? GetData() : GetData(g => (g.DESCR != null ? g.DESCR.Contains(buscar) : false) ||
                        (g.PROC_MED_SUBTIPO != null ? g.PROC_MED_SUBTIPO.DESCR.Contains(buscar) : false));

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Inserta la especialidad
        /// </summary>
        /// <param name="entidad">Entidad de especialidad</param>
        public bool Insertar(PROC_MED entidad)
        {
            try
            {
                entidad.ID_PROCMED = GetSequence<short>("PROCMED_SEC"); ;
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
        public bool Actualizar(PROC_MED entidad)
        {
            try
            {
                return Update(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}