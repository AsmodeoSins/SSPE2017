using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTratamientoDental : EntityManagerServer<DENTAL_TRATAMIENTO>
    {
        public cTratamientoDental() { }

        public IQueryable<DENTAL_TRATAMIENTO> ObtenerTodo(string buscar = "")
        {
            try
            {
                return GetData().Where(w => (string.IsNullOrEmpty(buscar) ? true : w.DESCR.Contains(buscar))).OrderBy(w => w.ID_TIPO_TRATA).ThenBy(w => w.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<DENTAL_TRATAMIENTO> ObtenerActivos(string buscar = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(buscar))
                    return GetData().Where(w => w.ESTATUS == "S" && (string.IsNullOrEmpty(buscar) ? true : w.DESCR.Contains(buscar))).OrderBy(w => w.ID_TIPO_TRATA).ThenBy(w => w.DESCR);
                else
                    return GetData().OrderBy(w => w.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<DENTAL_TRATAMIENTO> ObtenerXTipo(short tipo, string buscar = "")
        {
            try
            {
                return GetData().Where(w => ((tipo > 0) ? w.ID_TIPO_TRATA == tipo : true) && (string.IsNullOrEmpty(buscar) ? true : w.DESCR.Contains(buscar))).OrderBy(w => w.ID_TIPO_TRATA).ThenBy(t => t.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public short Insertar(DENTAL_TRATAMIENTO Entity)
        {
            try
            {
                Entity.ID_TRATA = GetIDProceso<short>("DENTAL_TRATAMIENTO", "ID_TRATA", "1=1");
                if (Insert(Entity))
                    return Entity.ID_TRATA;
                return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(DENTAL_TRATAMIENTO Entity)
        {
            try
            {
                return Update(Entity);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TRATA == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}
