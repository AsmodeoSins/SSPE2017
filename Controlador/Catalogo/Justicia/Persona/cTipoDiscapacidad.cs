using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoDiscapacidad : EntityManagerServer<TIPO_DISCAPACIDAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoDiscapacidad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_CICATRIZ"</returns>
        public IQueryable<TIPO_DISCAPACIDAD> ObtenerTodos(string buscar = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_DISCAPACIDAD>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
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
        /// <returns>objeto de tipo "TIPO_CICATRIZ"</returns>
        public List<TIPO_DISCAPACIDAD> Obtener(int Id)
        {
            var Resultado = new List<TIPO_DISCAPACIDAD>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_DISCAPACIDAD == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_CICATRIZ" con valores a insertar</param>
        public int Insertar(TIPO_DISCAPACIDAD Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                Entity.ID_TIPO_DISCAPACIDAD = GetSequence<short>("TIPO_DISCAPACIDAD_SEQ");
                Insert(Entity);
                return Entity.ID_TIPO_DISCAPACIDAD;
            }
            catch (Exception ex)
            {                
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_CICATRIZ" con valores a actualizar</param>
        public bool Actualizar(TIPO_DISCAPACIDAD Entity)
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
                var Entity = GetData().Where(w => w.ID_TIPO_DISCAPACIDAD == Id).SingleOrDefault();
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

        public bool Eliminar(TIPO_DISCAPACIDAD Entity)
        {
            try
            {
                return Delete(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}