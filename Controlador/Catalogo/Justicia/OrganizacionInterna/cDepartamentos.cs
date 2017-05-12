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
    public class cDepartamentos : EntityManagerServer<DEPARTAMENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDepartamentos()
        { }

        /// <summary>
        /// Metodo para regresar una lista de departamentos
        /// </summary>
        /// <param name="buscar">Nombre del departamento</param>
        /// <param name="estatus">Estatus "S" o "N" de los departamentos </param>
        /// <returns>ObservableCollection &lt;DEPARTAMENTO&gt;</returns>
        public IQueryable<DEPARTAMENTO> ObtenerTodos(string buscar = "", string estatus="S")
        {
            try
            {
                var predicate = PredicateBuilder.True<DEPARTAMENTO>();
                if (!string.IsNullOrWhiteSpace(buscar))
                    predicate = predicate.And(w=>w.DESCR.Contains(buscar.Trim()));
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
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
        public List<DEPARTAMENTO> Obtener(short Id)
        {
            var Resultado = new List<DEPARTAMENTO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_DEPARTAMENTO == Id).ToList();
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
        public void Insertar(DEPARTAMENTO Entity)
        {
            try
            {
                Entity.ID_DEPARTAMENTO = GetSequence<short>("DEPARTAMENTO_SEQ");
                //Entity.ID_DEPARTAMENTO = GetSequence<short>("DEPARTAMENTO_SEQ");
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
        public void Actualizar(DEPARTAMENTO Entity)
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
        public bool Eliminar(short? Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_DEPARTAMENTO == Id).SingleOrDefault();
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