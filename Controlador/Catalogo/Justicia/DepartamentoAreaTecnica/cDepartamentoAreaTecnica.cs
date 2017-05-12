using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDepartamentoAreaTecnica:EntityManagerServer<DEPARTAMENTO_AREA_TECNICA>
    {
        /// <summary>
        /// Obtiene la lista de configuracion entre departamento y area tecnica
        /// </summary>
        /// <param name="id_area_tecnica">ID del area tecnica</param>
        /// <param name="id_departamento">ID del deparamento</param>
        /// <returns>IQueryable &lt;DEPARTAMENTO_AREA_TECNICA&gt;</returns>
        public IQueryable<DEPARTAMENTO_AREA_TECNICA> ObtenerTodos(short? id_area_tecnica=null, short? id_departamento=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<DEPARTAMENTO_AREA_TECNICA>();
                if (id_area_tecnica.HasValue)
                    predicate = predicate.And(w => w.ID_TECNICA == id_area_tecnica.Value);
                if (id_departamento.HasValue)
                    predicate = predicate.And(w => w.ID_DEPARTAMENTO == id_departamento.Value);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Guarda un registro en la tabla DEPARTAMENTO_AREA_TECNICA
        /// </summary>
        /// <param name="departamento_area_tecnica">Una instancia de la entidad DEPARTAMENTO_AREA_TECNICA</param>
        public void Guardar(DEPARTAMENTO_AREA_TECNICA departamento_area_tecnica)
        {
            try
            {
                Insert(departamento_area_tecnica);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Elimina un registro en la tabla DEPARTAMENTO_AREA_TECNICA
        /// </summary>
        /// <param name="id_area_tecnica">ID del area tecnica a eliminar</param>
        /// <param name="id_departamento">ID del departamento a eliminar</param>
        public void Eliminar(short id_area_tecnica, short id_departamento)
        {
            try
            {
                var _entidad = Context.DEPARTAMENTO_AREA_TECNICA.FirstOrDefault(w => w.ID_TECNICA == id_area_tecnica && w.ID_DEPARTAMENTO == id_departamento);
                if (_entidad == null)
                    throw new Exception("El registro fue eliminado durante la operación");
                Delete(_entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
