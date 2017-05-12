using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMI : EntityManagerServer<EMI>
    {
        #region Constructors
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public cEMI() { }
        #endregion

        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion y retorna un objeto IQueryable.
        /// </summary>
        /// <returns></returns>
        public IQueryable<EMI> Obtener(int? Id = null)
        {
            try {
                if (Id != null)
                    return GetData().Where(w => w.ID_EMI == Id);
                else
                    return GetData();
            }
            catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
        }

        public EMI ObtenerAnterior(short Centro = 0,short Anio = 0,int Imputado = 0,int Ingreso = 0)
        {
            try
            {
                return GetData().Where(w => w.EMI_INGRESO.ID_CENTRO == Centro && w.EMI_INGRESO.ID_ANIO == Anio && w.EMI_INGRESO.ID_IMPUTADO == Imputado && w.EMI_INGRESO.ID_INGRESO <= Ingreso && w.ESTATUS == "C").OrderByDescending(w => w.ID_EMI).ThenBy(w => w.ID_EMI_CONS).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        //public Task<IQueryable<EMI>> Obtener() 
        //{
        //    return Action<IQueryable<EMI>>(async () => { });
        //}
        #endregion
        #region Insercion
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public bool Agregar(EMI Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entidad.ID_EMI_CONS = GetIDProceso<short>("EMI", "ID_EMI_CONS", string.Format("ID_EMI = {0}", Entidad.ID_EMI));
                    Context.EMI.Add(Entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Insert(Entidad))
                //    return "Informaci\u00F3n registrada correctamente.";
                //else
                //    return "No se registr\u00F3 la informaci\u00F3n.";
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(EMI Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return "Informaci\u00F3n actualizada correctamente.";
                else
                    return "No se pudo actualizar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Eliminar(EMI Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return "Informaci\u00F3n eliminada correctamente";
                else
                    return "No se pudo eliminar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error: "+ ex.Message;
            }
        }
        #endregion

        
    }
}
