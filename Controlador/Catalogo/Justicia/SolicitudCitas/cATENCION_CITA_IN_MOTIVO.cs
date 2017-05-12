using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencion_Cita_In_Motivo:EntityManagerServer<ATENCION_CITA_IN_MOTIVO>
    {

        /// <summary>
        /// Obtiene todos los motivos de incidencia durante una reprogramacion de solicitud de atencion.
        /// </summary>
        /// <param name="estatus">ID del estatus del registro (S=Activo, N=Deshabilitado)</param>
        /// <returns></returns>
        public IQueryable<ATENCION_CITA_IN_MOTIVO> ObtenerTodos(string estatus="S")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(estatus))
                    return GetData(w => w.ESTATUS == estatus);
                else
                    return GetData();
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public int Insertar(ATENCION_CITA_IN_MOTIVO Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entidad.ID_ACMOTIVO = GetIDProceso<short>("ATENCION_CITA_IN_MOTIVO", "ID_ACMOTIVO", "1=1");
                    Context.ATENCION_CITA_IN_MOTIVO.Add(Entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entidad.ID_ACMOTIVO;
                }
                //if (Insert(Entidad))
                //    return Entidad.ID_ATENCION;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
