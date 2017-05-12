using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cProceso : EntityManagerServer<PROCESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProceso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registro
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<PROCESO> ObtenerTodos(string Descr = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO>();
                if (!string.IsNullOrEmpty(Descr))
                    predicate = predicate.And(w => w.DESCR.Contains(Descr));
                return GetData(predicate.Expand()).OrderBy(w => w.ID_PROCESO);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }            
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer los registro para el menu del sistema
        /// </summary>
        /// <returns>listado de tipo "PROCESO"</returns>
        public IQueryable<PROCESO> ObtenerProcesoMenu(string Usuario = "")
        {
            try
            {
                return (from p in Context.PROCESO
                               join pu in Context.PROCESO_USUARIO on p.ID_PROCESO equals pu.ID_PROCESO
                               where pu.ID_USUARIO.Trim() == Usuario.Trim() && pu.ID_CENTRO == GlobalVariables.gCentro
                               select p).Distinct();
                #region Comentado
                //var predicate = PredicateBuilder.True<PROCESO>();
                //if (!string.IsNullOrEmpty(Usuario))
                //    //predicate = predicate.And(w => w.PROCESO_USUARIO.Any(x => x.ID_USUARIO == Usuario));
                //    predicate = predicate.And(w => w.PROCESO_USUARIO.Count(x => x.ID_USUARIO == Usuario) > 0);
                //return GetData(predicate.Expand()).OrderBy(w => w.ID_PROCESO);
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registro
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<PROCESO> ObtenerTodos(List<USUARIO_ROL> lst)
        {
            try
            {
                bool band = true;
                var predicate = PredicateBuilder.True<PROCESO>();
                foreach (var l in lst)
                {
                    predicate = predicate.And(w => w.PROCESO_ROL.Where(z => z.ID_ROL == l.ID_ROL).Count() == 0);
                }
                //
                predicate = predicate.And(w => w.PROCESO_USUARIO.Where(z => z.ID_ROL == 0).Count() == 0);
                return GetData(predicate.Expand());   
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<PROCESO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_PROCESO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public short Insertar(PROCESO Entity)
        {
            try
            {
                Entity.ID_PROCESO = GetIDProceso<short>("PROCESO", "ID_PROCESO", "1 = 1");
                if (Insert(Entity))
                    return Entity.ID_PROCESO;
            }
            catch (Exception ex)
            {                
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(PROCESO Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(PROCESO Entity)
        {
            try
            {
                if (Delete(Entity))
                    return true;
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
            return false;
        }
    }
}