using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Data.Objects;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAduanaIngreso : EntityManagerServer<ADUANA_INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAduanaIngreso()
        { }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ADUANA_INGRESO"</returns>
        public IQueryable<ADUANA_INGRESO> ObtenerTodos(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_ADUANA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<ADUANA_INGRESO> ObtenerTodos(DateTime? FechaEntrada = null, DateTime? FechaSalida = null, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA_INGRESO>();
                if (FechaEntrada != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) >= FechaEntrada);
                if (FechaSalida != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.ADUANA.ID_CENTRO == Centro);
                return GetData(predicate.Expand()).OrderBy(w => w.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerTodosPorTipoVisita(DateTime? FechaEntrada = null, DateTime? FechaSalida = null, short? Centro = null,short? TipoVisita = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA_INGRESO>();
                if (FechaEntrada.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) >= FechaEntrada);
                if (FechaSalida.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) <= FechaSalida);
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ADUANA.ID_CENTRO == Centro);
                if (TipoVisita.HasValue)
                    predicate = predicate.And(w => w.ADUANA.ID_TIPO_PERSONA == TipoVisita);
                return GetData(predicate.Expand()).OrderBy(w => w.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerTodosPorTipoVisita(DateTime? Fecha = null,short? Centro = null, short? TipoVisita = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA_INGRESO>();
                if (Fecha.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) == Fecha);
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ADUANA.ID_CENTRO == Centro);
                if (TipoVisita.HasValue)
                    predicate = predicate.And(w => w.ADUANA.ID_TIPO_PERSONA == TipoVisita);
                return GetData(predicate.Expand()).OrderBy(w => w.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerTotalInternosVisitados(short ID_TIPO_PERSONA, bool INTIMA, DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA_INGRESO>();
                predicate = predicate.And(a => a.ADUANA.ID_TIPO_PERSONA == ID_TIPO_PERSONA &&
                    (a.ADUANA.ENTRADA_FEC >= FechaInicial &&
                    a.ADUANA.SALIDA_FEC.HasValue &&
                    a.ADUANA.SALIDA_FEC.Value <= FechaFinal)
                    );
                if (INTIMA)
                    predicate = predicate.And(a => a.INTIMA != "N");
                else
                    predicate = predicate.And(a => a.INTIMA != "S");
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerAduanaIngresoSinNotificacion(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, short ID_INGRESO, DateTime FECHA_BUSQUEDA)
        {
            try
            {
                var INTERNO_NO_NOTIFICADO = "N";
                return GetData(g =>
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_IMPUTADO == ID_IMPUTADO &&
                    g.ID_INGRESO == ID_INGRESO &&
                    g.ENTRADA_FEC.Value.Year == FECHA_BUSQUEDA.Year &&
                    g.ENTRADA_FEC.Value.Month == FECHA_BUSQUEDA.Month &&
                    g.ENTRADA_FEC.Value.Day == FECHA_BUSQUEDA.Day &&
                    (g.INTERNO_NOTIFICADO == INTERNO_NO_NOTIFICADO || g.INTERNO_NOTIFICADO == null));
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerAduanaIngresoConNotificacion(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, short ID_INGRESO, DateTime FECHA_BUSQUEDA)
        {
            try
            {
                var INTERNO_NOTIFICADO = "S";
                return GetData(g =>
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_IMPUTADO == ID_IMPUTADO &&
                    g.ID_INGRESO == ID_INGRESO &&
                    g.ENTRADA_FEC.Value.Year == FECHA_BUSQUEDA.Year &&
                    g.ENTRADA_FEC.Value.Month == FECHA_BUSQUEDA.Month &&
                    g.ENTRADA_FEC.Value.Day == FECHA_BUSQUEDA.Day &&
                    g.INTERNO_NOTIFICADO == INTERNO_NOTIFICADO);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerTodosAbogadosIngreso(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA_INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                predicate = predicate.And(w => w.ADUANA.ID_TIPO_PERSONA == 2);
                return GetData(predicate.Expand()).OrderBy(w => w.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA_INGRESO> ObtenerXAbogadoEIngreso(int persona, short centro, short anio, int imputado, short ingreso)
        {

            try
            {
                return GetData().Where(w => w.ADUANA.ID_PERSONA == persona && w.ID_CENTRO == centro && w.ID_ANIO == anio &&
                     w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }


        public bool CambiarEstadoVisitaInterno(INGRESO_UBICACION Ingreso_Ubicacion, ADUANA_INGRESO Aduana_Ingreso, INCIDENTE Incidente = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.INGRESO_UBICACION.Add(Ingreso_Ubicacion);
                    Context.ADUANA_INGRESO.Attach(Aduana_Ingreso);
                    Context.Entry(Aduana_Ingreso).Property(x => x.INTERNO_NOTIFICADO).IsModified = true;
                    if (Incidente != null)
                        Context.INCIDENTE.Add(Incidente);
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }



        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(ADUANA_INGRESO Entity)
        {
            try
            {
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<ADUANA_INGRESO> Entity)
        {
            try
            {
                var resul = true;
                foreach (var item in Entity)
                {
                    resul = Insert(item);
                }
                return resul;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<ADUANA_INGRESO> ObtenerVisitasAbogados(DateTime FechaServer)
        {
            var ABOGADO = 2;
            var ACTUARIO = "A";
            var TITULAR = "T";
            var INTERNO_NO_NOTIFICADO = "N";

            try
            {
                var predicateAND = PredicateBuilder.True<ADUANA_INGRESO>();
                return GetData(g =>
                   g.ADUANA.ID_TIPO_PERSONA == ABOGADO &&
                   (g.INTERNO_NOTIFICADO == INTERNO_NO_NOTIFICADO || g.INTERNO_NOTIFICADO == null) &&
                   (g.ENTRADA_FEC.Value.Year == FechaServer.Year &&
                     g.ENTRADA_FEC.Value.Month == FechaServer.Month &&
                    g.ENTRADA_FEC.Value.Day == FechaServer.Day) &&
                    (g.ADUANA.PERSONA.ABOGADO.ID_ABOGADO_TITULO == TITULAR ||
                    (!g.ADUANA.SALIDA_FEC.HasValue && g.ADUANA.PERSONA.ABOGADO.ID_ABOGADO_TITULO == ACTUARIO))
                    );
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(ADUANA_INGRESO Entity)
        {
            try
            {
                Update(Entity);
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ADUANA == Id).SingleOrDefault();
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