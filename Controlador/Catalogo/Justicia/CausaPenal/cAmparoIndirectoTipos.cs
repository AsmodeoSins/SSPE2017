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
    public class cAmparoIndirectoTipos : EntityManagerServer<AMPARO_INDIRECTO_TIPOS>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAmparoIndirectoTipos()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        /// 
        public IQueryable<AMPARO_INDIRECTO_TIPOS> ObtenerTodos(short? Centro = null,short? Anio = null,int? Imputado = null,short? Ingreso = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<AMPARO_INDIRECTO_TIPOS>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
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
        public IQueryable<AMPARO_INDIRECTO_TIPOS> Obtener(short? Centro = null,short? Anio = null,int? Imputado = null,short? Ingreso = null,short? Id = null)
        {
            try
            {
                 var predicate = PredicateBuilder.True<AMPARO_INDIRECTO_TIPOS>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if(Id == null)
                    predicate = predicate.And(w => w.ID_AMPARO_INDIRECTO == Id);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<AMPARO_INDIRECTO_TIPOS> Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? Id = null, short? Amparo_tipo=null, short? Amparo_resultado=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<AMPARO_INDIRECTO_TIPOS>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (Id != null)
                    predicate = predicate.And(w => w.ID_AMPARO_INDIRECTO == Id);
                if (Amparo_tipo.HasValue)
                    predicate = predicate.And(w => w.ID_AMP_IND_TIPO == Amparo_tipo);
                if (Amparo_resultado.HasValue)
                    predicate = predicate.And(w => w.ID_SEN_AMP_RESULTADO == Amparo_resultado);
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
        public short Insertar(AMPARO_INDIRECTO_TIPOS Entity)
        {
            try
            {
                Entity.ID_AMPARO_INDIRECTO = GetIDProceso<short>("AMPARO_INDIRECTO_TIPOS", "ID_AMPARO_INDIRECTO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                if (Insert(Entity))
                    return Entity.ID_AMPARO_INDIRECTO;
                return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        public bool Insertar(List<AMPARO_INDIRECTO_TIPOS> Entity, short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null,short? Id = null)
        {
            try
            {
                if (Eliminar(Centro, Anio, Imputado, Ingreso, Id))
                {
                    if (Entity.Count == 0)
                        return true;
                    if (Insert(Entity))
                        return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(AMPARO_INDIRECTO_TIPOS Entity)
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
        public bool Eliminar(AMPARO_INDIRECTO_TIPOS Entity)
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

        public bool Eliminar(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? Id = null)
        {
            try
            {
                var list = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_AMPARO_INDIRECTO == Id).ToList();
                foreach (var obj in list)
                {
                    if (!Delete(obj))
                        return false;
                }
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