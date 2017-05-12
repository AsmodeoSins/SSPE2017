using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIImputado : EntityManagerServer<EMI_IMPUTADO>
    {
        #region Constructors
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public cEMIImputado() { }
        #endregion

        /// <summary>
        /// Obtiene toda la informacion y retorna un objeto IQueryable.
        /// </summary>
        /// <returns></returns>
        public EMI_IMPUTADO Obtener(short? Centro = null, short? Anio = null, int? Imputado = null,short? ProcesoLibertad = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EMI_IMPUTADO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(ProcesoLibertad.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == ProcesoLibertad);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<EMI_IMPUTADO> ObtenerAnterior(short? Centro = null, short? Anio = null, int? Imputado = null, short? ProcesoLibertad = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<EMI_IMPUTADO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (ProcesoLibertad.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD < ProcesoLibertad);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
