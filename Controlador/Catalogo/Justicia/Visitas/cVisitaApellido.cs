using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaApellido : EntityManagerServer<VISITA_APELLIDO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaApellido()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITA_APELLIDO"</returns>
        public IQueryable<VISITA_APELLIDO> ObtenerTodos(short centro)
        {
            //var visita = new List<VISITA_APELLIDO>().AsQueryable();
            try
            {
                return GetData().Where(w => w.ID_CENTRO == centro).OrderByDescending(o => o.ID_DIA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return visita;
        }

        public IQueryable<VISITA_APELLIDO> ObtenerDiaPorApellido(short Centro,Char Letra)
        {
            try
            {
                return from va in Context.VISITA_APELLIDO
                       where
                       va.ID_CENTRO == Centro && va.ESTATUS == "0" && va.LETRA_INICIAL[0] >= Letra && Letra <= va.LETRA_FINAL[0]
                       select va;
                    
                    //return GetData().Where(w => w.ID_CENTRO == Centro && (w.LETRA_INICIAL.CompareTo(Letra) >= 0 && 0 <= w.LETRA_FINAL.CompareTo(Letra)) && w.ESTATUS == "0");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return visita;
        }

        public IQueryable<VISITA_APELLIDO> ObtenerTodosActivos(short centro, int? dia)
        {
            //var visita = new List<VISITA_APELLIDO>().AsQueryable();
            try
            {
                return GetData().Where(w => w.ID_CENTRO == centro && (dia.HasValue ? w.ID_DIA == dia : true) &&
                    w.ESTATUS == "0").OrderByDescending(o => o.ID_CENTRO).ThenByDescending(t => t.LETRA_INICIAL);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return visita;
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
        public bool Insertar(VISITA_APELLIDO Entity)
        {
            try
            {
                Entity.ID_CONSEC = (short)(GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO).Any() ?
                GetData().Where(w => w.ID_CENTRO == Entity.ID_CENTRO).OrderByDescending(o => o.ID_CONSEC).FirstOrDefault().ID_CONSEC + 1 : 1);
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<VISITA_APELLIDO> Entity)
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
        public bool Actualizar(VISITA_APELLIDO Entity)
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
        public bool Eliminar(short centro, short id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_CONSEC == id).ToList();
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