using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cForma_Farmaceutica:EntityManagerServer<FORMA_FARMACEUTICA>
    {
        /// <summary>
        /// Obtiene todos las formas farmaceuticas
        /// </summary>
        /// <param name="estatus">Estatus de la formas farmaceuticas para el filtro</param>
        /// <returns>IQueryable &lt;FORMA_FARMACEUTICA&gt;</returns>
        public IQueryable<FORMA_FARMACEUTICA> Seleccionar(string estatus="")
        {
            try
            {
                var _predicate = PredicateBuilder.True<FORMA_FARMACEUTICA>();
                if (!string.IsNullOrWhiteSpace(estatus))
                {
                    _predicate = _predicate.And(w => w.ESTATUS==estatus);
                }
                return GetData(_predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
