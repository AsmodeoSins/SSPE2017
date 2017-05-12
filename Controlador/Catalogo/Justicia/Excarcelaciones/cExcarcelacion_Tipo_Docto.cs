using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cExcarcelacion_Tipo_Docto : EntityManagerServer<EXCARCELACION_TIPO_DOCTO>
    {
        /// <summary>
        /// metodo que se conecta a la base de datos para seleccionar un listado del tipo EXCARCELACION_TIPO_DOCTO
        /// </summary>
        /// <param name="id_tipo_exc">id del tipo de excarcelacion</param>
        /// <param name="activos">si los registros resultantes tienes que estar marcados como activos</param>
        public IQueryable<EXCARCELACION_TIPO_DOCTO> Seleccionar(short?id_tipo_exc=null, bool? activos=true)
        {
            try
            {
                var predicate = PredicateBuilder.True<EXCARCELACION_TIPO_DOCTO>();
                if (id_tipo_exc.HasValue)
                    predicate = predicate.And(w=>w.ID_TIPO_EX==id_tipo_exc);
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
