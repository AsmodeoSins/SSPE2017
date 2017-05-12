using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cProcesoUsuario : EntityManagerServer<PROCESO_USUARIO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProcesoUsuario()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<PROCESO_USUARIO> ObtenerTodos(string Usuario = "", short? Rol = null,short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_USUARIO>();
                if (!string.IsNullOrEmpty(Usuario))
                    predicate = predicate.And(w => w.ID_USUARIO.Trim() == Usuario.Trim());
                if (Rol != null)
                    predicate = predicate.And(w => w.ID_ROL == Rol);
                if(Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }            
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<PROCESO_USUARIO> ObtenerProcesosMenu(string Usuario = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_USUARIO>();
                if (!string.IsNullOrEmpty(Usuario))
                    predicate = predicate.And(w => w.ID_USUARIO == Usuario.Trim());
                return GetData(predicate.Expand()).Select(w => new PROCESO_USUARIO() { ID_PROCESO = w.ID_PROCESO,  }).Distinct();
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
        public IQueryable<PROCESO_USUARIO> Obtener(string Ventana,string Usuario)
        {
            try
            {
                return GetData().Where(w => w.PROCESO.VENTANA.Trim() == Ventana && w.ID_USUARIO.Contains(Usuario) && w.ID_CENTRO == GlobalVariables.gCentro);
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
        public bool Insertar(PROCESO_USUARIO Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;
            }
            catch (Exception ex)
            {                
                throw new ApplicationException(ex.Message);
            }
            return false;
        }


        public bool Insertar(List<PROCESO_USUARIO> Entity,string Usuario = "")
        {
            try
            {
                var aux = new List<PROCESO_USUARIO>();
                if (Entity != null)
                {
                    if (Entity.Any())
                    {
                        var procesos = ObtenerTodos(Usuario);
                        foreach (var x in Entity)
                        {
                            if (!procesos.Where(w => w.ID_PROCESO == x.ID_PROCESO && w.ID_ROL == x.ID_ROL).Any())
                                aux.Add(x);
                        }
                    }
                }
                if (Entity.Count == 0)
                    return true;
                else
                if (Insert(aux))
                    return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(PROCESO_USUARIO Entity)
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
        public bool Eliminar(PROCESO_USUARIO Entity)
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

        public bool Eliminar(List<PROCESO_USUARIO> Entity)
        {
            try
            {
                if (Entity.Count == 0)
                    return true;
                else
                {
                    foreach (var ent in Entity)
                    {
                        if (!Delete(ent))
                            return false;        
                    }
                }
                
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
            return true;
        }
    }
}