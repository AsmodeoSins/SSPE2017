using MoreLinq;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cImputadoPandilla : EntityManagerServer<IMPUTADO_PANDILLA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cImputadoPandilla()
        { }

        ///// <summary>
        ///// metodo que conecta con la base de datos para extraer todos los registros
        ///// </summary>
        ///// <returns>listado de tipo "IMPUTADO_FILIACION"</returns>
        //public List<IMPUTADO_PANDILLA> ObtenerTodos(short anio = 0, short centro = 0, int imputado = 0)
        //{
        //    var Resultado = new List<IMPUTADO_PANDILLA>();
        //    try
        //    {
        //        Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //    return Resultado;
        //}

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "IMPUTADO_FILIACION"</returns>
        public ObservableCollection<IMPUTADO_PANDILLA> Obtener(short anio = 0, short centro = 0, int imputado = 0)
        {
            ObservableCollection<IMPUTADO_PANDILLA> Resultado;
            try
            {
                Resultado = new ObservableCollection<IMPUTADO_PANDILLA>(GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO_FILIACION" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, List<IMPUTADO_PANDILLA> Entity)
        {
            try
            {
                Eliminar(Anio, Centro, Imputado);
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertarPandillas(short Centro, short Anio, int Imputado, List<IMPUTADO_PANDILLA> Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var Pandillas = Context.IMPUTADO_PANDILLA.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado);
                    if (Pandillas != null)
                    {
                        foreach (var o in Pandillas)
                        {
                            Context.Entry(o).State = EntityState.Deleted;
                        }
                    }
                    foreach (var item in Entity)
                        Context.IMPUTADO_PANDILLA.Add(item);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO_FILIACION" con valores a actualizar</param>
        public bool Actualizar(List<IMPUTADO_PANDILLA> Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(short anio = 0, short centro = 0, int imputado = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
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
        }
    }
}