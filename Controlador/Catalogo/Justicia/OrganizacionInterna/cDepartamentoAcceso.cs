using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Transactions;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia.OrganizacionInterna
{
    public class cDepartamentoAcceso : EntityManagerServer<DEPARTAMENTO_ACCESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDepartamentoAcceso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<DEPARTAMENTO_ACCESO> ObtenerTodos(short? Id_dep = null, string UsuarioActivo = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<DEPARTAMENTO_ACCESO>();
                if (Id_dep.HasValue)
                {
                    predicate = predicate.And(w => w.ID_DEPARTAMENTO == Id_dep);
                }
                if (!string.IsNullOrEmpty(UsuarioActivo))
                {
                    predicate = predicate.And(w => w.USUARIO.ESTATUS.Equals(UsuarioActivo));
                }

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
        public List<DEPARTAMENTO_ACCESO> Obtener(short IdDep, string Id_user)
        {
            var Resultado = new List<DEPARTAMENTO_ACCESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_DEPARTAMENTO == IdDep && w.ID_USUARIO == Id_user).ToList();
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
        public bool Insertar(DEPARTAMENTO_ACCESO Entity)
        {

            Insert(Entity);

            return true;


        }



        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(DEPARTAMENTO_ACCESO Entity)
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
        public bool Eliminar(DEPARTAMENTO_ACCESO Entity)
        {
            try
            {

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
