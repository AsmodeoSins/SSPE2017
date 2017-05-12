using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEspecialidades:EntityManagerServer<ESPECIALIDAD>
    {
        /// <summary>
        /// Retorna todas las especialidades medicas para interconsulta
        /// </summary>
        /// <param name="especialidad">Descripcion de la especialidad para la consulta</param>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;Especialidad&gt;</returns>
        public IQueryable<ESPECIALIDAD>ObtenerTodos(string especialidad="",string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<ESPECIALIDAD>();
                if (!string.IsNullOrWhiteSpace(especialidad))
                    predicate = predicate.And(w => w.DESCR.Contains(especialidad));
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        /// <summary>
        /// Inserta la especialidad
        /// </summary>
        /// <param name="entidad">Entidad de especialidad</param>
        public void Insertar(ESPECIALIDAD entidad)
        {
            try
            {
                var _id_consec = GetIDProceso<short>("ESPECIALIDAD", "ID_ESPECIALIDAD", "1=1");
                entidad.ID_ESPECIALIDAD = _id_consec;
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza la especialidad
        /// </summary>
        /// <param name="entidad">Entidad de especialidad</param>
        public void Actualizar(ESPECIALIDAD entidad)
        {
            try
            {
                Update(entidad);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " +(ex.InnerException!=null?ex.InnerException.InnerException.Message : ""));
            }
        }


    }
}
