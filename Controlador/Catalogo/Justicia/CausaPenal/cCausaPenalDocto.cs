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
    public class cCausaPenalDocto : EntityManagerServer<CAUSA_PENAL_DOCTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCausaPenalDocto()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<CAUSA_PENAL_DOCTO> ObtenerTodos(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? CausaPenal = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAUSA_PENAL_DOCTO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (CausaPenal != null)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == CausaPenal);
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<CAUSA_PENAL_DOCTO> Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? CausaPenal = null,short? Docto = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAUSA_PENAL_DOCTO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (CausaPenal != null)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == CausaPenal);
                if (Docto != null)
                    predicate = predicate.And(w => w.ID_DOCTO == Docto);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public short Insertar(CAUSA_PENAL_DOCTO Entity)
        {
            try
            {
                Entity.ID_DOCTO = GetIDProceso<short>("CAUSA_PENAL_DOCTO", "ID_DOCTO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_CAUSA_PENAL));
                if (Insert(Entity))
                    return Entity.ID_DOCTO;
                return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(CAUSA_PENAL_DOCTO Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
                return false;
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
        public bool Eliminar(CAUSA_PENAL_DOCTO Entity)
        {
            try
            {
                if (Delete(Entity))
                    return true;

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
            return false;
        }
    }
}