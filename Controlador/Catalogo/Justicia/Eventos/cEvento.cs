using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Data;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEvento : EntityManagerServer<EVENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEvento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<EVENTO> ObtenerTodos(string Nombre = "", short? Tipo = null, DateTime? Fecha = null, short? Centro = null, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<EVENTO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.CENTRO_REGISTRA == Centro);
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(Nombre));
                if (Tipo != null)
                    if (Tipo != -1)
                        predicate = predicate.And(w => w.ID_EVENTO_TIPO == Tipo);
                if (Fecha != null)
                    predicate = predicate.And(w => w.EVENTO_FEC == Fecha);
                return GetData(predicate.Expand()).OrderByDescending(w => w.EVENTO_FEC).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30))); ;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "FUERO"</returns>
        public IQueryable<EVENTO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_EVENTO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a insertar</param>
        public int Insertar(EVENTO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_EVENTO = GetIDProceso<int>("EVENTO", "ID_EVENTO", "1=1");
                    Context.EVENTO.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_EVENTO;
                    //if (Insert(Entity))
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(EVENTO Entity, List<EVENTO_PROGRAMA> Programa, List<EVENTO_PRESIDIUM> Presidium, List<EVENTO_INF_TECNICA> InformacionTecnica, List<EVENTO_INGRESO> Ingreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    int T, TP, TBD, index = 0;
                    #region Programa
                    var programas = Context.EVENTO_PROGRAMA.Where(w => w.ID_EVENTO == Entity.ID_EVENTO).OrderBy(w => w.ID_CONSEC);
                    TP = Programa.Count();
                    TBD = programas.Count();
                    T = TP - TBD;

                    if (T >= 0)// Son Iguales
                    {
                        index = 1;
                        foreach (var p in Programa)
                        {

                            if (T > 0 && index > TBD)//Se Agrega
                                Context.EVENTO_PROGRAMA.Add(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    if (T < 0)//Se elimina
                    {
                        index = 1;
                        foreach (var p in programas)
                        {

                            if (index > TP)//Se Elimina
                                Context.EVENTO_PROGRAMA.Remove(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    #endregion
                    T = TP = TBD = index = 0;
                    #region Presidium
                    var presidiums = Context.EVENTO_PRESIDIUM.Where(w => w.ID_EVENTO == Entity.ID_EVENTO).OrderBy(w => w.ID_CONSEC);
                    TP = Presidium.Count();
                    TBD = presidiums.Count();
                    T = TP - TBD;

                    if (T >= 0)// Son Iguales
                    {
                        index = 1;
                        foreach (var p in Presidium)
                        {

                            if (T > 0 && index > TBD)//Se Agrega
                                Context.EVENTO_PRESIDIUM.Add(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    if (T < 0)//Se elimina
                    {
                        index = 1;
                        foreach (var p in presidiums)
                        {

                            if (index > TP)//Se Elimina
                                Context.EVENTO_PRESIDIUM.Remove(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    #endregion
                    T = TP = TBD = index = 0;
                    #region Informa Tecnica
                    var informacion = Context.EVENTO_INF_TECNICA.Where(w => w.ID_EVENTO == Entity.ID_EVENTO).OrderBy(w => w.ID_CONSEC);
                    TP = InformacionTecnica.Count();
                    TBD = informacion.Count();
                    T = TP - TBD;

                    if (T >= 0)// Son Iguales
                    {
                        index = 1;
                        foreach (var p in InformacionTecnica)
                        {

                            if (T > 0 && index > TBD)//Se Agrega
                                Context.EVENTO_INF_TECNICA.Add(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    if (T < 0)//Se elimina
                    {
                        index = 1;
                        foreach (var p in informacion)
                        {

                            if (index > TP)//Se Elimina
                                Context.EVENTO_INF_TECNICA.Remove(p);
                            else
                                Context.Entry(p).State = EntityState.Modified;
                            index++;
                        }
                    }
                    #endregion
                    #region Ingresos
                    var ingresos = Context.EVENTO_INGRESO.Where(w => w.ID_EVENTO == Entity.ID_EVENTO);
                    if (ingresos != null)
                    {
                        foreach (var i in ingresos)
                        {
                            Context.EVENTO_INGRESO.Remove(i);
                        }
                    }
                    foreach (var i in Ingreso)
                    {
                        Context.EVENTO_INGRESO.Add(i);
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(EVENTO Entity)
        {
            try
            {
                return Delete(Entity);
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