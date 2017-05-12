using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Transactions;
using System.Data;
using LinqKit;
using System.Data.Objects;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAduana : EntityManagerServer<ADUANA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAduana()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<ADUANA> ObtenerTodos(string buscar = "")
        {
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    return GetData();
                else
                    return GetData().Where(w => w.OBSERV.Contains(buscar));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public IQueryable<ADUANA> ObtenerBitacora(short centro, DateTime hoy)
        {
            try
            {
                //getDbSet();
                return GetData().Where(w => w.ID_CENTRO == centro ?
                    w.ENTRADA_FEC.HasValue ?
                        (w.ENTRADA_FEC.Value.Day == hoy.Day && w.ENTRADA_FEC.Value.Month == hoy.Month && w.ENTRADA_FEC.Value.Year == hoy.Year) ?
                            !w.SALIDA_FEC.HasValue
                        : !w.SALIDA_FEC.HasValue
                    : false
                : false).OrderByDescending(o => o.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerReporteBitacora(short centro, DateTime Fecha)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO == centro &&
                    (g.ENTRADA_FEC.Value.Year == Fecha.Year &&
                    g.ENTRADA_FEC.Value.Month == Fecha.Month &&
                    g.ENTRADA_FEC.Value.Day == Fecha.Day)
                    ).OrderByDescending(o => o.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<ADUANA> ObtenerVisitantesFamiliaresPorFecha(short ID_TIPO_PERSONA, DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA>();
                if (ID_TIPO_PERSONA != 0)
                    predicate = predicate.And(a => a.ID_TIPO_PERSONA == ID_TIPO_PERSONA);

                predicate = predicate.And(g =>
                    g.ENTRADA_FEC.Value >= FechaInicial &&
                    g.ENTRADA_FEC.Value <= FechaFinal &&
                    g.ADUANA_INGRESO.Count(c => c.INTIMA == "N") == 0);

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerVisitantesXDiaPorFecha(DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                return GetData(g =>
                    g.ENTRADA_FEC.Value >= FechaInicial &&
                    g.ENTRADA_FEC.Value <= FechaFinal &&
                    g.ADUANA_INGRESO.Count(c => c.INTIMA == "N") == 0);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerVisitasIntimasXDiaPorFecha(DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                return GetData(g =>
                    g.ENTRADA_FEC.Value >= FechaInicial &&
                    g.ENTRADA_FEC.Value <= FechaFinal &&
                    g.ADUANA_INGRESO.Count(c => c.INTIMA == "S") > 0);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerVisitas(DateTime FechaInicial, DateTime FechaFinal, string Visitantes)
        {
            try
            {
                var predicateAND = PredicateBuilder.True<ADUANA>();
                var predicateOR = PredicateBuilder.False<ADUANA>();
                var Visitantes_Reporte = Visitantes.Split(',');
                predicateAND = predicateAND.And(
                    g =>
                    (g.ENTRADA_FEC >= FechaInicial &&
                    !g.SALIDA_FEC.HasValue) ||
                    (g.ENTRADA_FEC >= FechaInicial &&
                    g.SALIDA_FEC.HasValue &&
                    g.SALIDA_FEC <= FechaFinal) &&
                        //Filtros por información faltante en BD: Remover si la información esta completa despues
                    g.ADUANA_INGRESO.Count > 0 &&
                    g.PERSONA.SEXO != null &&
                    g.PERSONA.FEC_NACIMIENTO.HasValue
                    );
                foreach (var item in Visitantes_Reporte)
                {
                    var ID_VISITANTE = short.Parse(item);
                    predicateOR = predicateOR.Or(o => o.ID_TIPO_PERSONA == ID_VISITANTE);
                }

                predicateAND = predicateAND.And(predicateOR.Expand());

                return GetData(predicateAND.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerBitacoraXDia(short centro, DateTime hoy, int? parametro = null)
        {
            try
            {
                //getDbSet();
                var hoyParametro = hoy.AddHours(parametro.HasValue ? -parametro.Value : 0);
                return GetData().Where(w => (w.ENTRADA_FEC.Value.Day == hoy.Day || (w.ENTRADA_FEC.Value.Day == hoyParametro.Day && w.ENTRADA_FEC.Value >= hoyParametro)) &&
                    w.ENTRADA_FEC.Value.Month == hoy.Month &&
                    w.ENTRADA_FEC.Value.Year == hoy.Year &&
                    w.SALIDA_FEC == new Nullable<DateTime>() &&
                    w.ID_CENTRO == centro).OrderByDescending(o => o.ENTRADA_FEC);
                //return GetData().Where(w => w.ID_CENTRO == centro ?
                //    w.ENTRADA_FEC.HasValue ?
                //        (w.ENTRADA_FEC.Value.Day == hoy.Day && w.ENTRADA_FEC.Value.Month == hoy.Month && w.ENTRADA_FEC.Value.Year == hoy.Year) ?
                //            !w.SALIDA_FEC.HasValue
                //        : !w.SALIDA_FEC.HasValue
                //    : false
                //: false).OrderByDescending(o => o.ENTRADA_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<ADUANA> ObtenerTodosVisitantesIngreso(short ID_CENTRO, short ID_ANIO, int ID_IMPUTADO, short ID_INGRESO, DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                return GetData(g =>
                    /*(g.ENTRADA_FEC.Value.Year == FechaInicial.Year &&
                    g.ENTRADA_FEC.Value.Month == FechaInicial.Month &&
                    g.ENTRADA_FEC.Value.Day == FechaInicial.Day)*/
                    g.ENTRADA_FEC.Value >= FechaInicial &&
                    g.SALIDA_FEC.HasValue &&
                        /*(g.SALIDA_FEC.Value.Year == FechaFinal.Year &&
                        g.SALIDA_FEC.Value.Month == FechaFinal.Month &&
                        g.SALIDA_FEC.Value.Day == FechaFinal.Day)*/
                    g.ENTRADA_FEC.Value <= FechaFinal &&
                    g.ADUANA_INGRESO.Where(f =>
                    f.ID_CENTRO == ID_CENTRO &&
                    f.ID_ANIO == ID_ANIO &&
                    f.ID_IMPUTADO == ID_IMPUTADO &&
                    f.ID_INGRESO == ID_INGRESO).Any());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool ExisteMedico()
        {
            try
            {
                var COORDINADOR_MEDICO = 29;
                var MEDICO = 30;
                return GetData(g =>
                    !g.SALIDA_FEC.HasValue).
                    AsEnumerable().Where(w =>
                        w.PERSONA.EMPLEADO != null &&
                        w.PERSONA.EMPLEADO.USUARIO.Count(c =>
                            c.USUARIO_ROL.Count(cUR =>
                                cUR.SISTEMA_ROL.ID_ROL == COORDINADOR_MEDICO ||
                                cUR.SISTEMA_ROL.ID_ROL == MEDICO) > 0) > 0).Any();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public List<ADUANA> ObtenerXDia(short centro, DateTime hoy)
        {
            try
            {
                var res = GetData().Where(w => w.ENTRADA_FEC.Value.Day <= hoy.Day && w.ENTRADA_FEC.Value.Month == hoy.Month && w.ENTRADA_FEC.Value.Year == hoy.Year &&
                    w.SALIDA_FEC == new Nullable<DateTime>() && w.ID_CENTRO == centro).ToList();
                return res;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ADUANA> ObtenerTotalVisitantes(short ID_TIPO_PERSONA, bool INTIMA, DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA>();
                predicate = predicate.And(a => a.ID_TIPO_PERSONA == ID_TIPO_PERSONA &&
                    (a.ENTRADA_FEC >= FechaInicial &&
                    a.SALIDA_FEC.HasValue &&
                    a.SALIDA_FEC.Value <= FechaFinal)
                    );
                if (INTIMA)
                    predicate = predicate.And(a => a.ADUANA_INGRESO.Where(w => w.INTIMA != "N").Any());
                else
                    predicate = predicate.And(a => a.ADUANA_INGRESO.Where(w => w.INTIMA != "S").Any());
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<ADUANA> ObtenerTodos(short? Centro = null, DateTime? Inicio = null, DateTime? Fin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ADUANA>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Inicio != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) >= Inicio);
                if (Fin != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) <= Fin);
                return GetData(predicate.Expand());
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<ADUANA> Obtener(int Id)
        {
            var Resultado = new List<ADUANA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ADUANA == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public ADUANA ObtenerUltimaEntradaSinSalida(int persona)
        {
            try
            {
                var UltimaEntrada = GetData().Where(w => w.ID_PERSONA == persona && w.SALIDA_FEC == new Nullable<DateTime>());
                return UltimaEntrada.Any() ? UltimaEntrada.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public ADUANA ObtenerIDLocker(DateTime hoy, short tipoPersona)
        {
            try
            {
                var UltimaEntrada = GetData().Where(w => w.ID_TIPO_PERSONA == tipoPersona && w.ENTRADA_FEC.Value.Day == hoy.Day && w.ENTRADA_FEC.Value.Month == hoy.Month &&
                    w.ENTRADA_FEC.Value.Year == hoy.Year && (w.NUM_LOCKER.HasValue ? w.NUM_LOCKER.Value > 0 : false));
                return UltimaEntrada.Any() ? UltimaEntrada.OrderByDescending(o => o.NUM_LOCKER).FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public ADUANA ObtenerUltimaEntradaIntima(int persona, short tipoVisita, short tipoVisitante, short estatusAutorizado)
        {
            try
            {
                var UltimaEntrada = GetData().Where(w => w.ID_PERSONA == persona && w.ID_TIPO_PERSONA == tipoVisita &&
                    w.PERSONA.VISITANTE.VISITANTE_INGRESO.Where(wh => wh.ID_ESTATUS_VISITA == estatusAutorizado && wh.ID_TIPO_VISITANTE == tipoVisitante).Any());
                return UltimaEntrada.Any() ? UltimaEntrada.FirstOrDefault() : null;
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
        public bool Insertar(ADUANA Entity, string anio)
        {
            try
            {
                Entity.ID_ADUANA = int.Parse(anio + "" + GetSequence<short>("ADUANA_SEQ").ToString("D6"));
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool InsertarTransaccion(ADUANA aduana, List<ADUANA_INGRESO> aduanaIngresos, List<ADUANA_ACOMPANANTE> aduanaAcompanante, List<ADUANA_INGRESO_CP> aduanaCausaPenal)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.ADUANA.Add(aduana);
                    //aduanaIngresos ya debe de traer intima y/o administrativo
                    if (aduanaIngresos.Count > 0)
                        foreach (var item in aduanaIngresos)
                            Context.ADUANA_INGRESO.Add(item);
                    //aduanaAcompanante ya debe de traer persona y fecha
                    if (aduanaAcompanante.Count > 0)
                        foreach (var item in aduanaAcompanante)
                            Context.ADUANA_ACOMPANANTE.Add(item);
                    //aduanaCausaPenal ya debe de traer fecha
                    if (aduanaCausaPenal.Count > 0)
                        foreach (var item in aduanaCausaPenal)
                            Context.ADUANA_INGRESO_CP.Add(item);
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
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(ADUANA Entity)
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


        public IEnumerable<cControlVisitas> ObtenerReporteControlVisita(short Centro,DateTime FecInicio,DateTime FecFin)
        {
            try
            {

                string query = "SELECT A.ENTRADA_FEC,P.ID_PERSONA,TRIM(P.NOMBRE) VISITA_NOMBRE,TRIM(P.PATERNO) VISITA_PATERNO,TRIM(P.MATERNO) VISITA_MATERNO,P.SEXO VISITA_SEXO,P.FEC_NACIMIENTO VISITA_NACIMIENTO," +
                "I.ID_CENTRO,I.ID_ANIO,I.ID_IMPUTADO,TRIM(I.NOMBRE) INTERNO_NOMBRE,TRIM(I.PATERNO) INTERNO_PATERNO,TRIM(I.MATERNO) INTERNO_MATERNO," +
                "A.SALIDA_FEC,A.ID_TIPO_PERSONA,TP.DESCR AS TIPO_VISITA, AI.INTIMA," +
                "TRIM(E.DESCR) AS EDIFICIO,TRIM(S.DESCR) AS SECTOR,TRIM(ING.ID_UB_CELDA) AS CELDA,ING.ID_UB_CAMA AS CAMA, " +
                "TRIM(PU.NOMBRE) AS USUARIO_NOMBRE,TRIM(PU.PATERNO) AS USUARIO_PATERNO,TRIM(PU.MATERNO) AS USUARIO_MATERNO " + 
                "FROM SSP.ADUANA A " +
                "INNER JOIN SSP.ADUANA_INGRESO AI ON A.ID_ADUANA = AI.ID_ADUANA " +
                "INNER JOIN SSP.PERSONA P ON A.ID_PERSONA = P.ID_PERSONA " +
                "INNER JOIN SSP.IMPUTADO I ON AI.ID_CENTRO = I.ID_CENTRO AND AI.ID_ANIO = I.ID_ANIO AND AI.ID_IMPUTADO = I.ID_IMPUTADO " +
                "INNER JOIN SSP.INGRESO ING ON I.ID_CENTRO = ING.ID_CENTRO AND I.ID_ANIO = ING.ID_ANIO AND I.ID_IMPUTADO = ING.ID_IMPUTADO " +
                "INNER JOIN SSP.EDIFICIO E ON ING.ID_UB_CENTRO = E.ID_CENTRO AND ING.ID_UB_EDIFICIO = E.ID_EDIFICIO " +
                "INNER JOIN SSP.SECTOR S ON ING.ID_UB_CENTRO = S.ID_CENTRO AND ING.ID_UB_EDIFICIO = S.ID_EDIFICIO AND ING.ID_UB_SECTOR = S.ID_SECTOR " +
                "INNER JOIN SSP.CELDA C ON ING.ID_UB_CENTRO = C.ID_CENTRO AND ING.ID_UB_EDIFICIO = C.ID_EDIFICIO AND ING.ID_UB_SECTOR = C.ID_SECTOR AND ING.ID_UB_CELDA = C.ID_CELDA " +
                "INNER JOIN SSP.TIPO_PERSONA TP ON A.ID_TIPO_PERSONA = TP.ID_TIPO_PERSONA " +
                "LEFT JOIN SSP.USUARIO U ON A.ID_USUARIO = U.ID_USUARIO " +
                "LEFT JOIN SSP.PERSONA PU ON U.ID_PERSONA = PU.ID_PERSONA " +
                "WHERE ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND A.ID_CENTRO = {0} AND TRUNC(A.ENTRADA_FEC) >= '{1}' AND TRUNC(A.ENTRADA_FEC) <= '{2}'";
                query = string.Format(query, Centro, FecInicio.ToShortDateString(), FecFin.ToShortDateString());
                return Context.Database.SqlQuery<cControlVisitas>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /*
         base = {"The specified cast from a materialized 'System.String' type to a nullable 'System.Int32' type is not valid."}
         */
        public class cControlVisitas
        {
            public DateTime ENTRADA_FEC { get; set; }
            public int ID_PERSONA { get; set; }
            public string VISITA_NOMBRE { get; set; }
            public string VISITA_PATERNO { get; set; }
            public string VISITA_MATERNO { get; set; }
            public string VISITA_SEXO { get; set; }
            public DateTime? VISITA_NACIMIENTO { get; set; }
            public short ID_CENTRO { get; set; }
            public short ID_ANIO { get; set; }
            public int ID_IMPUTADO { get; set; }
            public string INTERNO_NOMBRE { get; set; }
            public string INTERNO_PATERNO { get; set; }
            public string INTERNO_MATERNO { get; set; }
            public DateTime? SALIDA_FEC { get; set; }
            public int? ID_TIPO_PERSONA { get; set; }
            public string TIPO_VISITA { get; set; }
            public string INTIMA { get; set; }
            public string EDIFICIO { get; set; }
            public string SECTOR { get; set; }
            public string CELDA { get; set; }
            public int? CAMA { get; set; }
            public string USUARIO_NOMBRE { get; set; }
            public string USUARIO_PATERNO { get; set; }
            public string USUARIO_MATERNO { get; set; }
        }
    }
}