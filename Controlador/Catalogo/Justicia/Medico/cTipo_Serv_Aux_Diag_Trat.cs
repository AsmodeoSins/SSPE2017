using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipo_Serv_Aux_Diag_Trat:EntityManagerServer<TIPO_SERVICIO_AUX_DIAG_TRAT>
    {
        /// <summary>
        /// Retorna todas las tipos de servicios auxiliares para diagnostico y tratamiento
        /// </summary>
        /// <param name="tipo_servicio">Descripcion del tipo de servicio auxiliar para la consulta</param>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;TIPO_SERVICIO_AUX_DIAG_TRAT&gt;</returns>
        public IQueryable<TIPO_SERVICIO_AUX_DIAG_TRAT> ObtenerTodos(string tipo_servicio="",string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_SERVICIO_AUX_DIAG_TRAT>();
                if (!string.IsNullOrWhiteSpace(tipo_servicio))
                    predicate = predicate.And(w => w.DESCR.Contains(tipo_servicio));
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
        /// Inserta el tipo de servicio auxiliar para diagnostico y tratamiento
        /// </summary>
        /// <param name="entidad">Entidad de tipo de servicio auxiliar para diagnostico y tratamiento</param>
        public void Insertar(TIPO_SERVICIO_AUX_DIAG_TRAT entidad)
        {
            try
            {
                var _id_consec = GetIDProceso<short>("TIPO_SERVICIO_AUX_DIAG_TRAT", "ID_TIPO_SADT", "1=1");
                entidad.ID_TIPO_SADT = _id_consec;
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza el tipo de servicio auxiliar para diagnostico y tratamiento
        /// </summary>
        /// <param name="entidad">Entidad de tipo de servicio auxiliar para diagnostico y tratamiento</param>
        public void Actualizar(TIPO_SERVICIO_AUX_DIAG_TRAT entidad)
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
