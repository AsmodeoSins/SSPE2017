﻿using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cRecursoResultado : EntityManagerServer<RECURSO_RESULTADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cRecursoResultado()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "RECURSO_RESULTADO"</returns>
        public IQueryable<RECURSO_RESULTADO> ObtenerTodos()
        {
            try
            {
                return GetData();
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
        /// <returns>objeto de tipo "RECURSO_RESULTADO"</returns>
        public List<RECURSO_RESULTADO> Obtener(int TipoRecurso)
        {
            var Resultado = new List<RECURSO_RESULTADO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_RECURSO == TipoRecurso).ToList();
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
        /// <param name="Entity">objeto de tipo "RECURSO_RESULTADO" con valores a insertar</param>
        public short Insertar(RECURSO_RESULTADO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_TIPO_RECURSO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "RECURSO_RESULTADO" con valores a actualizar</param>
        public bool Actualizar(RECURSO_RESULTADO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int TipoRecurso, string Resultado)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_RECURSO == TipoRecurso && w.RESULTADO.Equals(Resultado)).ToList();
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