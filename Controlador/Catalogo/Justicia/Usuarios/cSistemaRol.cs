using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSistemaRol : EntityManagerServer<SISTEMA_ROL>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSistemaRol()
        { }



        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<SISTEMA_ROL> ObtenerTodos(string Descr = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<SISTEMA_ROL>();
                if (!string.IsNullOrEmpty(Descr))
                    predicate = predicate.And(w => w.DESCR.Contains(Descr));
                predicate = predicate.And(w => w.ID_ROL != 0);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }            
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<SISTEMA_ROL> Obtener(List<SISTEMA_ROL> Roles)
        {
            try
            {
                if (Roles != null)
                {
                    var predicate = PredicateBuilder.True<SISTEMA_ROL>();
                    foreach (var r in Roles)
                    {
                        predicate = predicate.Or(w => w.ID_ROL == r.ID_ROL);
                    }
                    return GetData(predicate.Expand()).OrderBy(w => w.ID_ROL);
                }
                else
                    return GetData();
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
        public SISTEMA_ROL Obtener(short Rol)
        {
            try
            {
               return GetData().Where(w => w.ID_ROL == Rol).SingleOrDefault();
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
        public short Insertar(SISTEMA_ROL Entity)
        {
            try
            {
                Entity.ID_ROL = GetIDProceso<short>("SISTEMA_ROL", "ID_ROL", "1 = 1");
                if (Insert(Entity))
                    return Entity.ID_ROL;
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
        public bool Actualizar(SISTEMA_ROL Entity)
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
        public bool Eliminar(SISTEMA_ROL Entity)
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