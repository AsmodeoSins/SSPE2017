using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cExcarcelacion_Estatus:EntityManagerServer<EXCARCELACION_ESTATUS>
    {
        /// <summary>
        /// metodo que se conecta a la base de datos para seleccionar un listado del tipo EXCARCELACION_ESTATUS
        /// </summary>
        /// <param name="activos">si los registros resultantes tienes que estar marcados como activos</param>

        public IQueryable<EXCARCELACION_ESTATUS> Seleccionar(bool? activos = true)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION_ESTATUS>();
                if (activos.HasValue)
                    if (activos.Value)
                        predicate = predicate.And(w => w.ESTATUS == "S");
                    else
                        predicate = predicate.And(w => w.ESTATUS == "N");
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
