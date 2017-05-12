using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNivelPrioridad:EntityManagerServer<INTERCONSULTA_NIVEL_PRIORIDAD>
    {
        /// <summary>
        /// Retorna todos los niveles de prioridad
        /// </summary>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;INTERCONSULTA_NIVEL_PRIORIDAD&gt;</returns>
        public IQueryable<INTERCONSULTA_NIVEL_PRIORIDAD> ObtenerTodos(string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INTERCONSULTA_NIVEL_PRIORIDAD>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
