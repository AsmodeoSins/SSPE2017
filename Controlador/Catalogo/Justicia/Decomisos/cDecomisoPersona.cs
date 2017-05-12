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
    public class cDecomisoPersona : EntityManagerServer<DECOMISO_PERSONA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDecomisoPersona()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_PERSONA> ObtenerTodos(short? Centro = 0, string Paterno = "", string Materno = "", string Nombre = "", short? Tipo = 0, bool EsOficial = false)
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO_PERSONA>();
                if (Centro != null)
                    predicate = predicate.And(w => w.DECOMISO.ID_CENTRO == Centro);
                if (Tipo > 0)
                    predicate = predicate.And(w => w.ID_TIPO_PERSONA == Tipo);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PERSONA.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.PERSONA.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.PERSONA.NOMBRE.Contains(Nombre));
                if (EsOficial)
                    predicate = predicate.And(w => w.OFICIAL_A_CARGO == "S");
                else
                    predicate = predicate.And(w => w.OFICIAL_A_CARGO != "S");
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
        public IQueryable<DECOMISO_PERSONA> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_DECOMISO == Id);
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
        public IQueryable<DECOMISO_PERSONA> ObtenerDecomisosPorCustodios(int[] IDs_Custodios, int? ID_PERSONA = null, string NOMBRE = "", string PATERNO = "", string MATERNO = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO_PERSONA>();
                var predicateOR = PredicateBuilder.False<DECOMISO_PERSONA>();
                if (ID_PERSONA != 0 && ID_PERSONA != null)
                    predicate = predicate.And(a => a.ID_PERSONA == ID_PERSONA);
                if (!string.IsNullOrEmpty(NOMBRE))
                    predicate = predicate.And(a => !string.IsNullOrEmpty(a.PERSONA.NOMBRE) && a.PERSONA.NOMBRE == NOMBRE);
                if (!string.IsNullOrEmpty(PATERNO))
                    predicate = predicate.And(a => !string.IsNullOrEmpty(a.PERSONA.NOMBRE) && a.PERSONA.PATERNO == PATERNO);
                if (!string.IsNullOrEmpty(MATERNO))
                    predicate = predicate.And(a => !string.IsNullOrEmpty(a.PERSONA.NOMBRE) && a.PERSONA.MATERNO == MATERNO);

                foreach (var ID in IDs_Custodios)
                {
                    if (ID > 0)
                        predicateOR = predicateOR.Or(o =>
                            o.PERSONA.EMPLEADO.ID_TIPO_EMPLEADO == ID);
                }

                if (IDs_Custodios.Any(a => a > 0))
                    predicate = predicate.And(predicateOR.Expand());

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
        public bool Insertar(DECOMISO_PERSONA Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(List<DECOMISO_PERSONA> Entity, int Id)
        {
            try
            {
                if (Eliminar(Id))
                {
                    if (Entity.Count == 0)
                        return true;
                    else
                        if (Insert(Entity))
                            return true;
                        else
                            return false;
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
        public void Actualizar(DECOMISO_PERSONA Entity)
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
        public bool Eliminar(int Id)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_DECOMISO == Id);
                foreach (var entity in ListEntity)
                {
                    if (!Delete(entity))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}
                //throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
                return false;
            }

        }
    }
}