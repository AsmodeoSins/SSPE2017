using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cJuzgado : EntityManagerServer<JUZGADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cJuzgado()
        { }
        public IQueryable<JUZGADO> Obtener()
        {
            try
            {
               
                    return GetData();

            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "JUZGADO"</returns>
        public IQueryable<JUZGADO> ObtenerTodos(string tribunal = "N",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<JUZGADO>();
                if (!string.IsNullOrEmpty(tribunal))
                    predicate = predicate.And(w => w.TRIBUNAL == tribunal);
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
                //if(Tribunal)
                //    return GetData().Where(w => w.TRIBUNAL.Equals("S"));
                //else
                //    return GetData();

            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id" tipo="short">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "juzgado"</returns>
        public IQueryable<JUZGADO> Seleccionar(short id)
        {
            try
            {
                return GetData().Where(w => w.ID_JUZGADO==id);
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
        /// <returns>objeto de tipo "juzgado"</returns>
        public List<JUZGADO> Obtener(short? Pais, short? Estado, short? Municipio, string Fuero)
        {
            try
            {
                if (Pais == null)
                    Pais = 0;
                if (Estado == null)
                    Estado = 0;
                if (Municipio == null)
                    Municipio = 0;
                return GetData().Where(w => w.ID_PAIS == Pais && w.ID_ENTIDAD == Estado && w.ID_MUNICIPIO == Municipio && w.ID_FUERO.Equals(Fuero)).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "JUZGADO" con valores a insertar</param>
        public long Insertar(JUZGADO Entity)
        {
            try
            {
                Entity.ID_JUZGADO= GetSequence<short>("JUZGADO_SEQ");
                Insert(Entity);
                return  Entity.ID_JUZGADO;
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
        /// <param name="Entity">objeto de tipo "JUZGADO" con valores a actualizar</param>
        public bool Actualizar(JUZGADO Entity)
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
        public bool EliminarObtener(int Pais, int Estado, int Municipio, string Fuero, int Juzgado)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_PAIS == Pais && w.ID_ENTIDAD == Estado && w.ID_MUNICIPIO == Municipio && w.ID_FUERO.Equals(Fuero) && w.ID_JUZGADO == Juzgado).ToList();
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