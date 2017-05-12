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
    public class cNuc : EntityManagerServer<NUC>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cNuc()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "NUC"</returns>
        public IQueryable<NUC> ObtenerTodos()
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
        /// <returns>objeto de tipo "NUC"</returns>
        public IQueryable<NUC> Obtener(int Centro,int Anio,int Imputado,int Ingreso, string Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_NUC.Equals(Id));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "NUC" con valores a insertar</param>
        public bool Insertar(NUC Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                return Insert(Entity);
                
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
        /// <param name="Entity">objeto de tipo "NUC" con valores a actualizar</param>
        public bool Actualizar(NUC Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //if (ex.Message.Contains("part of the object's key information"))
                //    throw new ApplicationException("La llave principal no se puede cambiar");
                //else
                //    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
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