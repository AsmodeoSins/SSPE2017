using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCentro : EntityManagerServer<CENTRO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCentro()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<CENTRO> ObtenerTodos(string buscar = "", int entidad = 0, int municipio = 0,string estatus = "")
        {
            try
            {   
                var predicate = PredicateBuilder.True<CENTRO>();
                if (!string.IsNullOrWhiteSpace(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar) || w.CALLE.Contains(buscar) || 
                        w.COLONIA.Contains(buscar) || w.MUNICIPIO.MUNICIPIO1.Contains(buscar) || w.DIRECTOR.Contains(buscar));
                if (entidad != 0)
                    predicate = predicate.And(w => w.ID_ENTIDAD == entidad);
                if (municipio != 0)
                    predicate = predicate.And(w => w.ID_MUNICIPIO == municipio);
                if(!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
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
        public IQueryable<CENTRO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Id);
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
        public void Insertar(CENTRO Entity)
        {
            try
            {
                //Entity.ID_CENTRO = GetIDCatalogo<short>("CENTRO");
                //Entity.ID_CENTRO = GetSequence<short>("CENTRO_SEQ");
                Entity.ID_CENTRO = (short)(GetData().OrderByDescending(o => o.ID_CENTRO).FirstOrDefault().ID_CENTRO + 1);
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(CENTRO Entity)
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
                var Entity = GetData().Where(w => w.ID_CENTRO == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("child record found"))
                    return false;
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}
                //throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }
    }
}