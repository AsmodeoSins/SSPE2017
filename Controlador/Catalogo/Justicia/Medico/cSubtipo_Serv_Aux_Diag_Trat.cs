using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSubtipo_Serv_Aux_Diag_Trat : EntityManagerServer<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>
    {
        /// <summary>
        /// Retorna todos los subtipos de servicios auxiliares para diagnostico 
        /// </summary>
        /// <param name="tipo_servicio">Valor de tipo de servicio auxiliar para la consulta</param>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;SUBTIPO_SERVICIO_AUX_DIAG_TRAT&gt;</returns>
        public IQueryable<SUBTIPO_SERVICIO_AUX_DIAG_TRAT> ObtenerTodos(short? tipo_servicio = null, string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<SUBTIPO_SERVICIO_AUX_DIAG_TRAT>();
                if (tipo_servicio.HasValue)
                    predicate = predicate.And(w => w.ID_TIPO_SADT==tipo_servicio.Value);
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
        /// Inserta el subtipo de servicio auxiliar para diagnostico 
        /// </summary>
        /// <param name="entidad">Entidad de subtipo de servicio auxiliar para diagnostico y tratamiento</param>
        public void Insertar(SUBTIPO_SERVICIO_AUX_DIAG_TRAT entidad)
        {
            try
            {
                var _id_consec = GetIDProceso<short>("SUBTIPO_SERVICIO_AUX_DIAG_TRAT", "ID_SUBTIPO_SADT", "1=1");
                entidad.ID_SUBTIPO_SADT = _id_consec;
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza el subtipo de servicio auxiliar para diagnostico
        /// </summary>
        /// <param name="entidad">Entidad de subtipo de servicio auxiliar para diagnostico y tratamiento</param>
        public void Actualizar(SUBTIPO_SERVICIO_AUX_DIAG_TRAT entidad)
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
