using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaEdificio : EntityManagerServer<VISITA_EDIFICIO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaEdificio()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITA_EDIFICIO"</returns>
        public IQueryable<VISITA_EDIFICIO> ObtenerTodos(short? centro = null, int? dia = null,string estatus = "")
        {
            //var visita = new List<VISITA_EDIFICIO>().AsQueryable();
            try
            {
                var predicate = PredicateBuilder.True<VISITA_EDIFICIO>();
                if (centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == centro);
                if (dia.HasValue)
                    predicate = predicate.And(w => w.DIA == dia);
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand()).OrderByDescending(o => o.DIA);
                //visita = GetData().Where(w => w.ID_CENTRO == centro && (dia.HasValue ? w.DIA == dia : true)).OrderByDescending(o => o.DIA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return visita;
        }
        public IQueryable<VISITA_EDIFICIO> ObtenerTodosActivos(short centro, int? dia)
        {
            var visita = new List<VISITA_EDIFICIO>().AsQueryable();
            try
            {
                visita = GetData().Where(w => w.ID_CENTRO == centro && (dia.HasValue ? w.DIA == dia : true) &&
                    w.ESTATUS == "0").OrderByDescending(o => o.ID_CENTRO).ThenByDescending(t => t.ID_EDIFICIO).ThenByDescending(t => t.ID_SECTOR).ThenByDescending(t => t.CELDA_INICIO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return visita;
        }
        //public IQueryable<VISITA_EDIFICIO> ObtenerActivosXCentroEdificioSectorYCelda(short centro, short edificio, short sector, string celda, int? dia)
        //{
        //    var visita = new List<VISITA_EDIFICIO>().AsQueryable();
        //    try
        //    {
        //        visita = GetData().Where(w => w.ID_CENTRO == centro && w.ID_EDIFICIO==edificio && w.ID_SECTOR==sector && (w.CELDA_INICIO>=celda && w.CELDA_FINAL<=celda)
        //            (dia.HasValue ? w.DIA == dia : true) && w.ESTATUS == "0").OrderByDescending(o => o.DIA);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //    return visita;
        //}


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(VISITA_EDIFICIO Entity)
        {
            try
            {
                Entity.ID_CONSEC = (short)(GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_EDIFICIO == Entity.ID_EDIFICIO && w.ID_SECTOR == Entity.ID_SECTOR).Any() ?
                GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_EDIFICIO == Entity.ID_EDIFICIO && w.ID_SECTOR == Entity.ID_SECTOR).OrderByDescending(o => o.ID_CONSEC).FirstOrDefault().ID_CONSEC + 1 : 1);
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_AREA = GetSequence<short>("AREA_VISITA_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<VISITA_EDIFICIO> Entity)
        {
            try
            {
                foreach (var item in Entity)
                {

                }
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
        public bool Actualizar(VISITA_EDIFICIO Entity)
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

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(short centro, short edificio, short sector, short id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_EDIFICIO == edificio && w.ID_SECTOR == sector && w.ID_CONSEC == id).ToList();
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