using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCelda : EntityManagerServer<CELDA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCelda()
        { }


        public IQueryable<CELDA> ObtenerCeldas(short ID_CENTRO, short? ID_EDIFICIO = 0, short? ID_SECTOR = 0)
        {
            try
            {
                var predicate = PredicateBuilder.True<CELDA>();
                predicate = predicate.And(x => x.ID_CENTRO == ID_CENTRO);
                if (ID_EDIFICIO != 0)
                    predicate = predicate.And(x => x.ID_EDIFICIO == ID_EDIFICIO);
                if (ID_SECTOR != 0)
                    predicate = predicate.And(x => x.ID_SECTOR == ID_SECTOR);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<CELDA> ObtenerTodos(string buscar = "", MUNICIPIO municipio = null, CENTRO centro = null, EDIFICIO edificio = null, SECTOR sector = null)
        {
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                {
                    #region municipio
                    if (municipio.ID_MUNICIPIO == 0)
                    {
                        #region centro
                        if (centro.ID_CENTRO == 0)
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData();
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region centro
                        if (centro.ID_CENTRO == 0)
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_SECTOR == sector.ID_SECTOR
                                                                && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                && w.ID_CENTRO == edificio.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region municipio
                    if (municipio.ID_MUNICIPIO == 0)
                    {
                        #region centro
                        if (centro.ID_CENTRO == 0)
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)));
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO
                                                                && w.ID_CENTRO == sector.ID_CENTRO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                w.ID_CENTRO == edificio.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR
                                                                && w.ID_CENTRO == edificio.ID_CENTRO);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR
                                                                && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                && w.ID_SECTOR == sector.ID_SECTOR);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region centro
                        if (centro.ID_CENTRO == 0)
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                }
                                else
                                {
                                    return GetData().Where(w => (w.ID_CELDA.Contains(buscar) || w.SECTOR.DESCR.Contains(buscar) || w.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                w.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region edificio
                            if (edificio.ID_EDIFICIO == 0)
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.ID_CELDA.Contains(buscar) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_CENTRO == centro.ID_CENTRO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_CELDA.Contains(buscar) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            else
                            {
                                #region sector
                                if (sector.ID_SECTOR == 0)
                                {
                                    return GetData().Where(w => w.ID_CELDA.Contains(buscar) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                }
                                else
                                {
                                    return GetData().Where(w => w.ID_CELDA.Contains(buscar) && w.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_EDIFICIO);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }




        public IQueryable<CELDA> ObtenerEstancias(short ID_CENTRO, short ID_EDIFICIO = 0, short ID_SECTOR = 0)
        {
            try
            {
                var predicate = PredicateBuilder.False<CELDA>();
                predicate = predicate.And(x => x.ID_CENTRO == ID_CENTRO);
                if (ID_EDIFICIO != 0)
                    predicate = predicate.And(x => x.ID_EDIFICIO == ID_EDIFICIO);
                if (ID_SECTOR != 0)
                    predicate = predicate.And(x => x.ID_SECTOR == ID_SECTOR);
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
        public IQueryable<CELDA> ObtenerPorSector(short? sector, short? edificio, short centro)
        {
            try
            {
                return GetData().Where(w => w.ID_SECTOR == sector && w.ID_EDIFICIO == edificio && w.ID_CENTRO == centro);
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
        public CELDA Obtener(string celda, short? sector, short? edificio, short centro)
        {
            var Resultado = new CELDA();
            try
            {
                Resultado = GetData().Where(w => w.ID_CELDA == celda && w.ID_SECTOR == sector && w.ID_EDIFICIO == edificio && w.ID_CENTRO == centro).FirstOrDefault();
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
        public void Insertar(CELDA Entity)
        {
            try
            {
                //Entity.ID_CELDA = GetIDCatalogo<string>("CELDA");
                //Entity.ID_ETNIA = GetSequence<short>("EDIFICIO_SEQ");
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
        public void Actualizar(CELDA Entity)
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
        /// metodo que se conecta a la base de datos para actualizar registros
        /// </summary>
        /// <param name="Entities">lista de objetos de tipo "ESTADO" con valores a actualizar</param>
        public string Actualizar(List<CELDA> Entities)
        {
            try
            {
                Update(Entities);
                return "Información actualizada correctamente.";
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
        public bool Eliminar(string Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CELDA == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("child record found"))
                {
                    return false;
                }
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