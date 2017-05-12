using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPronostico:EntityManagerServer<PRONOSTICO>
    {
        public IQueryable<PRONOSTICO> ObtenerTodos(string estatus="")
        {
            try
            {
                var predicado = PredicateBuilder.True<PRONOSTICO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    return GetData(predicado.Expand());
                else
                    return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
