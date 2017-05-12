using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEmiIngreso : EntityManagerServer<EMI_INGRESO>
    {
        #region Constructors
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public cEmiIngreso() { }
        #endregion

        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion y retorna un objeto IQueryable.
        /// </summary>
        /// <returns></returns>
        public EMI_INGRESO ObtenerEmiInterno(int? Centro = null,int? Anio = null,int? Imputado = null,short? Ingreso = null)
        {
            try
            {
                    var predicate = PredicateBuilder.True<EMI_INGRESO>();
                    if(Centro.HasValue)    
                predicate = predicate.And(w => w.ID_CENTRO == Centro);
                    if(Anio.HasValue)    
                predicate = predicate.And(w => w.ID_ANIO == Anio);
                    if(Imputado.HasValue)    
                predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                    if(Ingreso.HasValue)    
                predicate = predicate.And(w => w.ID_INGRESO < Ingreso);
                    return GetData(predicate.Expand()).OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public IQueryable<EMI_INGRESO> ObtenerEmiInterno(int? Id = null)
        {
            try
            {
                if (Id != null)
                    return GetData().Where(w => w.ID_IMPUTADO == Id);
                else
                    return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }
}