using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cExcarcelacionTipo : EntityManagerServer<EXCARCELACION_TIPO>
    {
        /// <summary>
        /// metodo que se conecta a la base de datos para seleccionar un listado del tipo EXCARCELACION_TIPO
        /// </summary>
        /// <param name="activos">si los registros resultantes tienes que estar marcados como activos</param>

        public IQueryable<EXCARCELACION_TIPO> Seleccionar(bool? activos=true)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION_TIPO>();
                if (activos.HasValue)
                    if (activos.Value)
                        predicate = predicate.And(w => w.ESTATUS == "S");
                    else
                        predicate = predicate.And(w => w.ESTATUS == "N");

                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {

                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
