using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using LinqKit;
using System.Data.Objects;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHoja_Referencia_Medica:EntityManagerServer<HOJA_REFERENCIA_MEDICA>
    {
        public IQueryable<HOJA_REFERENCIA_MEDICA> ObtenerTodosporCanalizacion(int id_canalizacion,string estatus="",short? id_nivel_prioridad=null, short? id_tipo_sevicio_interconsulta=null)
        {
            try
            {
                var predicate = PredicateBuilder.True<HOJA_REFERENCIA_MEDICA>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w=>w.INTERCONSULTA_SOLICITUD.ESTATUS == estatus);
                if (id_nivel_prioridad.HasValue)
                    predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.ID_INIVEL == id_nivel_prioridad);
                if (id_tipo_sevicio_interconsulta.HasValue)
                    if (id_tipo_sevicio_interconsulta.Value==2)
                        predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.SERVICIO_AUX_INTERCONSULTA.Any());
                    else
                        predicate = predicate.And(w => w.INTERCONSULTA_SOLICITUD.ID_ESPECIALIDAD!=null);
                predicate=predicate.And(w=>w.INTERCONSULTA_SOLICITUD.CANALIZACION.ID_ATENCION_MEDICA==id_canalizacion);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
