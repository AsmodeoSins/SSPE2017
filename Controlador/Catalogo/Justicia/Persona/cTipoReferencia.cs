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
    public class cTipoReferencia: EntityManagerServer<TIPO_REFERENCIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoReferencia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_REFERENCIA"</returns>
        public IQueryable<TIPO_REFERENCIA> ObtenerTodos(string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_REFERENCIA>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //var referencias = new ObservableCollection<TIPO_REFERENCIA>(GetData().AsEnumerable());
            //return referencias;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "TIPO_REFERENCIA"</returns>
        public List<TIPO_REFERENCIA> Obtener(int Id)
        {
            var Resultado = new List<TIPO_REFERENCIA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_REFERENCIA == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_REFERENCIA" con valores a insertar</param>
        public void Insertar(TIPO_REFERENCIA Entity)
        {
            try
            {

                Entity.ID_TIPO_REFERENCIA = GetSequence<short>("TIPO_REFERENCIA_SEQ");
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_REFERENCIA" con valores a actualizar</param>
        public void Actualizar(TIPO_REFERENCIA Entity)
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_REFERENCIA == Id).SingleOrDefault();
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