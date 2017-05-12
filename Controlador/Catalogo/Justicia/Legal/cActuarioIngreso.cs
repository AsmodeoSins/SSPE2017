using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cActuarioIngreso : EntityManagerServer<ACTUARIO_INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cActuarioIngreso()
        { }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ACTUARIO_INGRESO> ObtenerXLista(int idLista)
        {
            var Resultado = new List<ACTUARIO_INGRESO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_LISTA == idLista).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ACTUARIO_INGRESO> ObtenerXActuarioYFecha(int actuario, DateTime hoy)
        {
            var Resultado = new List<ACTUARIO_INGRESO>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ACTUARIO_LISTA.ID_ABOGADO == actuario && (w.ACTUARIO_LISTA.CAPTURA_FEC.HasValue ? (w.ACTUARIO_LISTA.CAPTURA_FEC.Value.Day == hoy.Day &&
                    w.ACTUARIO_LISTA.CAPTURA_FEC.Value.Month == hoy.Month && w.ACTUARIO_LISTA.CAPTURA_FEC.Value.Year == hoy.Year) : false));
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(ACTUARIO_INGRESO Entity)
        {
            try
            {
                var cons = GetData().Where(w => w.ID_LISTA == Entity.ID_LISTA).ToList();
                Entity.ID_CONSEC = (short)(cons.Any() ? cons.Count + 1 : 1);
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarLista(List<ACTUARIO_INGRESO> Entity, int idLista)
        {
            try
            {
                var Lista = GetData().Where(w => w.ID_LISTA == idLista).OrderByDescending(o => o.ID_CONSEC);
                var cons = (Lista.Any() ? Lista.FirstOrDefault().ID_CONSEC + 1 : 1);
                var bandera = false;
                foreach (var item in Entity)
                {
                    if (Insert(new ACTUARIO_INGRESO
                    {
                        ID_LISTA = item.ID_LISTA,
                        ID_CENTRO = item.ID_CENTRO,
                        ID_ANIO = item.ID_ANIO,
                        ID_IMPUTADO = item.ID_IMPUTADO,
                        ID_INGRESO = item.ID_INGRESO,
                        //ELEGIDO = item.ELEGIDO,
                        ID_CONSEC = (short)cons
                    }))
                    {
                        bandera = true;
                        cons++;
                    }
                    else
                        return false;
                }
                return bandera;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarListaTransaccion(List<ACTUARIO_INGRESO> Entity, int idLista)
        {
            try
            {
                using (var transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var Lista = GetData().Where(w => w.ID_LISTA == idLista).OrderByDescending(o => o.ID_CONSEC);
                    var cons = 1;
                    foreach (var item in Lista)
                        Context.ACTUARIO_INGRESO.Remove(item);
                    foreach (var item in Entity)
                    {
                        if (!GetData().Any(f => f.ID_LISTA == idLista && f.ID_CONSEC == item.ID_CONSEC && item.ID_CENTRO == f.ID_CENTRO && item.ID_ANIO == f.ID_ANIO && item.ID_IMPUTADO == f.ID_IMPUTADO && item.ID_INGRESO == f.ID_INGRESO))
                        {
                            Context.ACTUARIO_INGRESO.Add(new ACTUARIO_INGRESO
                                {
                                    ID_LISTA = item.ID_LISTA,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_ANIO = item.ID_ANIO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    //ELEGIDO = item.ELEGIDO,
                                    ID_CONSEC = (short)cons
                                });
                            cons++;
                        }
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
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(ACTUARIO_INGRESO Entity)
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
        public bool Eliminar(int idLista, int consecutivo)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_LISTA == idLista && w.ID_CONSEC == consecutivo).SingleOrDefault();
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