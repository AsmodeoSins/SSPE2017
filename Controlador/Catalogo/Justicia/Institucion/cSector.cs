using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSector : EntityManagerServer<SECTOR>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSector()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<SECTOR> ObtenerTodos(string buscar = "", int? municipio = null, int? centro = null, int? edificio = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<SECTOR>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                if(municipio.HasValue)
                    predicate = predicate.And(w => w.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio);
                if(centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == centro);
                if(edificio.HasValue)
                    predicate = predicate.And(w => w.ID_EDIFICIO == edificio);
                return GetData().AsExpandable().Where(predicate);

                #region Comentado
                //getDbSet();
                //var res = GetData().Where(w => w.ID_EDIFICIO == edificio &&
                //    w.ID_CENTRO == centro &&
                //        w.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio &&
                //            (string.IsNullOrEmpty(buscar) ? true :
                //                ((w.DESCR.Contains(buscar) || w.EDIFICIO.DESCR.Contains(buscar) || w.EDIFICIO.CENTRO.DESCR.Contains(buscar))))).ToList();
                //if (res.Any())
                //    res = res.OrderBy(o => o.EDIFICIO.CENTRO.ID_MUNICIPIO).ToList();
                //return res;
                #endregion

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
        public SECTOR Obtener(short? sector, short? edificio, short centro)
        {
            var Resultado = new SECTOR();
            try
            {
                Resultado = GetData().Where(w => w.ID_SECTOR == sector && w.ID_EDIFICIO == edificio && w.ID_CENTRO == centro).FirstOrDefault();
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(SECTOR Entity)
        {
            try
            {
                Entity.ID_SECTOR = (short)(GetData().Where(w => w.ID_EDIFICIO == Entity.ID_EDIFICIO).Any() ? 
                    GetData().Where(w => w.ID_EDIFICIO == Entity.ID_EDIFICIO).OrderByDescending(o => o.ID_SECTOR).FirstOrDefault().ID_SECTOR + 1 : 1);
                //Entity.ID_SECTOR = GetIDCatalogo<short>("SECTOR");
                //Entity.ID_SECTOR = GetSequence<short>("SECTOR_SEQ");
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
        public void Actualizar(SECTOR Entity)
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
        public bool Eliminar(int Id_Sector)
        {
            try
            {

                var Entity = GetData().Where(w => w.ID_SECTOR == Id_Sector).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                   // if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Error: No se puede eliminar sector Filiacion: Tiene dependencias.");
                }
                return false;
            }
        }
    }
}