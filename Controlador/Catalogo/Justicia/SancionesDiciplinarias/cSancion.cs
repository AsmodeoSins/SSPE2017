using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSancion : EntityManagerServer<SANCION>
    {
        #region Constructor
        public cSancion() { }
        #endregion
        #region Obtener
        public IQueryable<SANCION> ObtenerTodas(short Centro, short Anio, int Imputado, short Ingreso, short Incidente)
        {
           return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_INCIDENTE == Incidente);
        }
        #endregion
        #region Insertar
        /// <summary>
        /// Inserta en la tabla la entidad enviada.
        /// </summary>
        /// <param name="entidad">Objeto que se desea insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public short Insertar(SANCION entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    entidad.ID_SANCION = GetIDProceso<short>("SANCION", "ID_SANCION", 
                        string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_INCIDENTE = {4}",
                        entidad.ID_CENTRO,
                        entidad.ID_ANIO,
                        entidad.ID_IMPUTADO,
                        entidad.ID_INGRESO,
                        entidad.ID_INCIDENTE));
                    Context.SANCION.Add(entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return entidad.ID_SANCION;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(short Centro, short Anio, int Imputado, short Ingreso, short Incidente,List<SANCION> list) 
        {
            try
            {
                Eliminar(Centro, Anio, Imputado, Ingreso, Incidente);
                if (list.Count == 0)
                    return true;
                else
                    if (Insert(list))
                        return true;
                    else
                        return false;
            }
            catch (Exception ex) 
            {
                throw new ApplicationException(ex.Message);
            }
        }
      
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(SANCION Entidad)
        {
            try
            {
                return Update(Entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(SANCION Entidad)
        {
            try
            {
                return Delete(Entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Eliminar(short Centro, short Anio, int Imputado, short Ingreso, short Incidente)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_INCIDENTE == Incidente);
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
                return true;

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }
        #endregion
    }
}
