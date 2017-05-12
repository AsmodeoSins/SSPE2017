using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCita_Tipo:EntityManagerServer<CITA_TIPO>
    {
        /// <summary>
        /// Retorna todos los tipos de cita de referencia medica
        /// </summary>
        /// <param name="estatus">Valor de estatus para la consulta, valores "S", "N"</param>
        /// <returns>IQueryable&lt;CITA_TIPO&gt;</returns>
        public IQueryable<CITA_TIPO> ObtenerTodos(string estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<CITA_TIPO>();
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
