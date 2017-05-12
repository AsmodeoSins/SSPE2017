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
    public class cAmparoIncidenteTipo : EntityManagerServer<AMPARO_INCIDENTE_TIPO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAmparoIncidenteTipo()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        /// 
        public IQueryable<AMPARO_INCIDENTE_TIPO> ObtenerTodos(string buscar = "",string enCP = "",string Estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<AMPARO_INCIDENTE_TIPO>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                if (!string.IsNullOrEmpty(enCP))
                    predicate = predicate.And(w => w.APLICA_CAUSA_PENAL.Equals(enCP));
                if (!string.IsNullOrEmpty(Estatus))
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                return GetData(predicate.Expand()).OrderBy(w => w.ID_AMP_INC_TIPO);
                
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
        public IQueryable<AMPARO_INCIDENTE_TIPO> Obtener(short Id)
        {
            try
            {
                return GetData().Where(w => w.ID_AMP_INC_TIPO == Id);
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
        public short Insertar(AMPARO_INCIDENTE_TIPO Entity)
        {
            try
            {
                Entity.ID_AMP_INC_TIPO = GetIDProceso<short>("AMPARO_INCIDENTE_TIPO", "ID_AMP_INC_TIPO", "1 = 1");
                if (Insert(Entity))
                    return Entity.ID_AMP_INC_TIPO;
                return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        public short Insertar(AMPARO_INCIDENTE_TIPO Entity,SSPEntidades Contexto)
        {
            try
            {
                Entity.ID_AMP_INC_TIPO = GetIDProceso<short>("AMPARO_INCIDENTE_TIPO", "ID_AMP_INC_TIPO", "1 = 1", Contexto);
                if (Insert(Entity, Contexto))
                    return Entity.ID_AMP_INC_TIPO;
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
        public bool Actualizar(AMPARO_INCIDENTE_TIPO Entity)
        {
            try
            {
                return (Update(Entity));
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
        public bool Eliminar(short Id)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_AMP_INC_TIPO == Id);
                if (ListEntity != null)
                {
                    foreach (var entity in ListEntity)
                    {
                        if (Delete(entity))
                            return true;
                    }
                    return false;
                }
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