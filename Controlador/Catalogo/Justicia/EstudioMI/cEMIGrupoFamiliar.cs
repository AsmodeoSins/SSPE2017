using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEMIGrupoFamiliar : EntityManagerServer<EMI_GRUPO_FAMILIAR>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEMIGrupoFamiliar()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "EMI_GRUPO_FAMILIAR"</returns>
        public IQueryable<EMI_GRUPO_FAMILIAR> ObtenerTodos(int Id, int Consecutivo)
        {
            try
            {
                return GetData().Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

      
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "EMI_GRUPO_FAMILIAR" con valores a insertar</param>
        public bool Insertar(int Id, int Consecutivo, List<EMI_GRUPO_FAMILIAR> list)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var grupo = Context.EMI_GRUPO_FAMILIAR.Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
                    if (grupo != null)
                    {
                        foreach (var g in grupo)
                        {
                            Context.Entry(g).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var g in list)
                    {
                        Context.EMI_GRUPO_FAMILIAR.Add(g);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

       

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Id, int Consecutivo)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
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
    }
}