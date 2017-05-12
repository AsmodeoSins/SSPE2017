using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cServ_Aux_Diag_Trat : EntityManagerServer<SERVICIO_AUX_DIAG_TRAT>
    {
        /// <summary>
        /// Retorna todos los servicios auxiliares para diagnostico 
        /// </summary>
        /// <param name="tipo_servicio">Valor de tipo de servicio auxiliar para la consulta</param>
        /// <param name="subtipo_servicio">Valor de subtipo de servicio auxiliar para la consulta</param>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;SERVICIO_AUX_DIAG_TRAT&gt;</returns>
        public IQueryable<SERVICIO_AUX_DIAG_TRAT>ObtenerTodos(short? tipo_servicio=null,short? subtipo_servicio=null,string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<SERVICIO_AUX_DIAG_TRAT>();
                if (tipo_servicio.HasValue)
                    predicate = predicate.And(w => w.SUBTIPO_SERVICIO_AUX_DIAG_TRAT.ID_TIPO_SADT == tipo_servicio.Value);
                if (subtipo_servicio.HasValue)
                    predicate = predicate.And(w => w.ID_SUBTIPO_SADT == subtipo_servicio.Value);
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
        /// Inserta el servicio auxiliar para diagnostico 
        /// </summary>
        /// <param name="entidad">Entidad de servicio auxiliar para diagnostico y tratamiento</param>
        public void Insertar(SERVICIO_AUX_DIAG_TRAT entidad)
        {
            try
            {
                var _id_consec = GetIDProceso<short>("SERVICIO_AUX_DIAG_TRAT", "ID_SERV_AUX", "1=1");
                entidad.ID_SERV_AUX = _id_consec;
                Insert(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Actualiza el servicio auxiliar para diagnostico
        /// </summary>
        /// <param name="entidad">Entidad de servicio auxiliar para diagnostico y tratamiento</param>
        public void Actualizar(SERVICIO_AUX_DIAG_TRAT entidad)
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
