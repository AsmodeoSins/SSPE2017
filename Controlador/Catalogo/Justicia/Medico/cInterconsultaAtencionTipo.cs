using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cInterconsultaAtencionTipo : EntityManagerServer<INTERCONSULTA_ATENCION_TIPO>
    {

        /// <summary>
        /// Retorna el tipo de atencion para interconsulta
        /// </summary>
        /// <param name="id_interat">ID del tipo de interconsulta a obtener</param>
        /// <returns>INTERCONSULTA_ATENCION_TIPO;</returns>
        public INTERCONSULTA_ATENCION_TIPO Obtener(short id_interat)
        {
            try
            {
                return GetData(w=>w.ID_INTERAT==id_interat).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Retorna todos los tipos de atencion para interconsulta
        /// </summary>
        /// <param name="interconsulta_atencion_tipo">Tipo de atencion de interconsulta para la consulta</param>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;INTERCONSULTA_ATENCION_TIPO&gt;</returns>
        public IQueryable<INTERCONSULTA_ATENCION_TIPO> ObtenerTodos(string interconsulta_atencion_tipo="", string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INTERCONSULTA_ATENCION_TIPO>();
                if (!string.IsNullOrWhiteSpace(interconsulta_atencion_tipo))
                    predicate = predicate.And(w => w.DESCR.Contains(interconsulta_atencion_tipo));
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        /// <summary>
        /// Inserta el tipo de atencion para interconsulta 
        /// </summary>
        /// <param name="entidad">Entidad de tipo de atencion para interconsulta</param>
        public void Insertar(INTERCONSULTA_ATENCION_TIPO entidad)
        {
            try
            {
                var _id_consec = GetIDProceso<short>("INTERCONSULTA_ATENCION_TIPO", "ID_INTERAT", "1=1");
                entidad.ID_INTERAT = _id_consec;
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza el tipo de atencion para interconsulta 
        /// </summary>
        /// <param name="entidad">Entidad de tipo de atencion para interconsulta</param>
        public void Actualizar(INTERCONSULTA_ATENCION_TIPO entidad)
        {
            try
            {
                Update(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
