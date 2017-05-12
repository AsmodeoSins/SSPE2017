using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cRecursoDelito : EntityManagerServer<RECURSO_DELITO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cRecursoDelito()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "RECURSO_DELITO"</returns>
        public IQueryable<RECURSO_DELITO> ObtenerTodos()
        {
            try
            {
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
        /// <returns>objeto de tipo "CAUSA_PENAL_DELITO"</returns>
        public IQueryable<RECURSO_DELITO> ObtenerTodos(short Centro = 0, short Anio = 0, int Imputado = 0, short Ingreso = 0, short CausaPenal = 0, short Recurso = 0)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_RECURSO == Recurso);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "RECURSO_DELITO" con valores a insertar</param>
        public int Insertar(RECURSO_DELITO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_IMPUTADO;
            }
            catch (Exception ex)
            {
                return 0;
                //throw new ApplicationException(ex.Message);
            }
            
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "RECURSO_DELITO" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, short Ingreso, short CausaPenal, short Recurso, List<RECURSO_DELITO> Delitos)
        {
            try
            {
                EliminarMultiple(Centro, Anio, Imputado, Ingreso, CausaPenal,Recurso);
                if (Delitos.Count == 0)
                    return true;
                else
                {
                    if (Insert(Delitos))
                        return true;
                    else
                        return false;
                }
                
            }
            catch (Exception ex)
            {
                return false;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL_DELITO" con valores a actualizar</param>
        public bool Actualizar(RECURSO_DELITO Entity)
        {
            try
            {
                Update(Entity);
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
        public bool EliminarMultiple(int Centro, int Anio, int Imputado, int Ingreso,int CausaPenal,int Recurso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_RECURSO == Recurso).ToList();
                if (Entity != null)
                {
                    foreach (RECURSO_DELITO delito in Entity)
                    {
                        Delete(delito);
                    }
                    return true;
                }
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