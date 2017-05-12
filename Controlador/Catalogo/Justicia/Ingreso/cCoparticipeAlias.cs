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
    public class cCoparticipeAlias : EntityManagerServer<COPARTICIPE_ALIAS>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCoparticipeAlias()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "COPARTICIPE"</returns>
        public ObservableCollection<COPARTICIPE_ALIAS> ObtenerTodos()
        {
            ObservableCollection<COPARTICIPE_ALIAS> coparticipe_alias;
            var Resultado = new List<COPARTICIPE_ALIAS>();
            try
            {
                Resultado = GetData().ToList();
                coparticipe_alias = new ObservableCollection<COPARTICIPE_ALIAS>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return coparticipe_alias;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "COPARTICIPE_ALIAS"</returns>
        public List<COPARTICIPE_ALIAS> Obtener(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal, int Coparticipe)
        {
            var Resultado = new List<COPARTICIPE_ALIAS>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_COPARTICIPE == Coparticipe).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "COPARTICIPE_ALIAS" con valores a insertar</param>
        public long Insertar(COPARTICIPE_ALIAS Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return  Entity.ID_COPARTICIPE_ALIAS;
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
        /// <param name="Entity">objeto de tipo "COPARTICIPE_ALIAS" con valores a actualizar</param>
        public bool Actualizar(COPARTICIPE_ALIAS Entity)
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
        public bool EliminarObtener(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal,int Coparticipe,int Alias)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_COPARTICIPE == Coparticipe && w.ID_COPARTICIPE_ALIAS == Alias).ToList();
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

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool EliminarObtener(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
                if (Entity != null)
                {
                    foreach (COPARTICIPE_ALIAS alias in Entity)
                    {
                        Delete(alias);
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