using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cProcedimientosSubtipo : EntityManagerServer<PROC_MED_SUBTIPO>
    {
        public cProcedimientosSubtipo() { }

        /// <summary>
        /// Obtiene todo el catalogo de procedimientos materiales subtipo
        /// </summary>
        /// <returns>Listado del mundo de procedimientos materiales subtipo</returns>
        public IQueryable<PROC_MED_SUBTIPO> ObtenerTodos()
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
        /// Obtiene un listado de procedimientos medicos subtipo que se encuentran en activo.
        /// </summary>
        /// <returns>Listado de procedimientos medicos subtipo filtrados</returns>
        public IQueryable<PROC_MED_SUBTIPO> ObtenerTodosActivos()
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
        /// Obtiene la busqueda de procedimientos medicos subtipo por medio de un string. Tambien busca por el tipo de atencion
        /// </summary>
        /// <param name="buscar">Busqueda de procedimiento medico subtipo</param>
        /// <returns>Listado de procedimientos medicos subtipo filtrada</returns>
        public IQueryable<PROC_MED_SUBTIPO> ObtenerXBusqueda(string buscar)
        {
            try
            {
                return string.IsNullOrEmpty(buscar) ? GetData() : GetData(g => g.DESCR != null ? g.DESCR.Contains(buscar) : false ||
                    g.ATENCION_TIPO != null ? g.ATENCION_TIPO.DESCR.Contains(buscar) : false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene un listado de procedimientos medicos subtipo que contiene un tipo de atencion.
        /// </summary>
        /// <param name="ProcMed">ID del tipo de atencion</param>
        /// <returns>Listado de procedimientos medicos subtipo filtrados</returns>
        public IQueryable<PROC_MED_SUBTIPO> ObtenerXTipoAtencion(short TipoAtencion)
        {
            try
            {
                return GetData(g => g.ID_TIPO_ATENCION == TipoAtencion);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene la busqueda de procedimientos medicos subtipo por medio de un string. Tambien busca por el tipo de atencion
        /// </summary>
        /// <param name="buscar">Busqueda de procedimiento medico subtipo</param>
        /// <returns>Listado de procedimientos medicos subtipo filtrada</returns>
        public IQueryable<PROC_MED_SUBTIPO> ObtenerXBusquedaYTipoAtencion(string buscar, short TipoAtencion)
        {
            try
            {
                if (TipoAtencion > 0)
                    return string.IsNullOrEmpty(buscar) ? GetData(g => g.ID_TIPO_ATENCION == TipoAtencion) : GetData(g => g.ID_TIPO_ATENCION == TipoAtencion && g.DESCR != null ? g.DESCR.Contains(buscar) : false);
                else
                    return string.IsNullOrEmpty(buscar) ? GetData() : GetData(g => g.DESCR != null ? g.DESCR.Contains(buscar) : false ||
                        g.ATENCION_TIPO != null ? g.ATENCION_TIPO.DESCR.Contains(buscar) : false);
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
        public bool Insertar(PROC_MED_SUBTIPO entidad)
        {
            try
            {
                entidad.ID_PROCMED_SUBTIPO = GetSequence<short>("PROCMED_SUBTIPO_SEC"); ;
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
        public bool Actualizar(PROC_MED_SUBTIPO entidad)
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