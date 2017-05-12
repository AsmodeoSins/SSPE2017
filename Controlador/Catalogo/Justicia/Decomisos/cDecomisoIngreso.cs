using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDecomisoIngreso : EntityManagerServer<DECOMISO_INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDecomisoIngreso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_INGRESO> ObtenerTodos(short? Centro = 0, string Paterno = "", string Materno = "", string Nombre = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO_INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.DECOMISO.ID_CENTRO == Centro);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre));
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_INGRESO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_DECOMISO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public bool Insertar(DECOMISO_INGRESO Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;
                else
                    return false;
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(List<DECOMISO_INGRESO> Entity,int Id)
        {
            try
            {
                if (Eliminar(Id))
                {
                    if (Entity.Count == 0)
                        return true;
                    else
                    if (Insert(Entity))
                        return true;
                    else
                        return false;
                }

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
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public void Actualizar(DECOMISO_INGRESO Entity)
        {
            try
            {
                Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(DECOMISO_INGRESO Entity)
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

        public bool Eliminar(int Id)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_DECOMISO == Id);
                foreach (var entity in ListEntity)
                {
                    if (!Delete(entity))
                        return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}
                //throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                return false;
            }
         
        }
    }
}