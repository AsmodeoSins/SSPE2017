using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPrograma : EntityManagerServer<TIPO_PROGRAMA>
    {
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PROGRAMA"</returns>
        public IQueryable<TIPO_PROGRAMA> ObtenerTodos(string buscar = "", short? departamento = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_PROGRAMA>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar) || w.NOMBRE.Contains(buscar) || w.DEPARTAMENTO.DESCR.Contains(buscar));
                if(departamento.HasValue)
                    predicate = predicate.And(w => w.ID_DEPARTAMENTO == departamento);
                return GetData(predicate.Expand());
                #region comentado
                //if (string.IsNullOrEmpty(buscar) && departamento == 0)
                //    Resultado = GetData().ToList();

                //if (!string.IsNullOrEmpty(buscar) && departamento == 0)
                //    Resultado = GetData().Where(w => (w.DESCR.Contains(buscar) || w.NOMBRE.Contains(buscar) || w.DEPARTAMENTO.DESCR.Contains(buscar))).ToList();

                //if (string.IsNullOrEmpty(buscar) && departamento > 0)
                //    Resultado = GetData().Where(w => (w.DESCR.Contains(buscar) || w.NOMBRE.Contains(buscar) || w.DEPARTAMENTO.DESCR.Contains(buscar)) && w.ID_DEPARTAMENTO == departamento).ToList();
                
                //if (!string.IsNullOrEmpty(buscar) && departamento > 0)
                //    Resultado = GetData().Where(w => (w.DESCR.Contains(buscar) || w.NOMBRE.Contains(buscar) || w.DEPARTAMENTO.DESCR.Contains(buscar)) && w.ID_DEPARTAMENTO == departamento).ToList();

                //etnias = new List<TIPO_PROGRAMA>(Resultado);
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return etnias;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(TIPO_PROGRAMA Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                Entity.ID_TIPO_PROGRAMA = GetSequence<short>("TIPO_PROGRAMA_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(TIPO_PROGRAMA Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }
        public bool Eliminar(short? Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_PROGRAMA == Id).SingleOrDefault();
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
