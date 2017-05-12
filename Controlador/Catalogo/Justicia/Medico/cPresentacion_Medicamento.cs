using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPresentacion_Medicamento:EntityManagerServer<PRESENTACION_MEDICAMENTO>
    {
        /// <summary>
        /// Obtiene las presentaciones de medicamentos
        /// </summary>
        /// <param name="estatus">estatus para el filtro de la busqueda</param>
        /// <returns>IQueryable lt;PRESENTACION_MEDICAMENTOgt;</returns>
        public IQueryable<PRESENTACION_MEDICAMENTO> Seleccionar(string estatus="")
        {
            try
            {
                var _predicate = PredicateBuilder.True<PRESENTACION_MEDICAMENTO>();
                if (!string.IsNullOrWhiteSpace(estatus))
                    _predicate = _predicate.And(w=>w.ACTIVO==estatus);
                return GetData(_predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
