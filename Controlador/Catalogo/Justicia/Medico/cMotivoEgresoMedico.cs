using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cMotivoEgresoMedico:EntityManagerServer<MOTIVO_EGRESO_MEDICO>
    {
        /// <summary>
        /// Obtiene los motivos de egresos medicos por estatus
        /// </summary>
        /// <param name="estatus">Opcional. Estatus por el cual se va a filtrar el catalogo</param>
        /// <returns></returns>
        public IQueryable<MOTIVO_EGRESO_MEDICO> ObtenerMotivosEgresosMedicos(string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<MOTIVO_EGRESO_MEDICO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
