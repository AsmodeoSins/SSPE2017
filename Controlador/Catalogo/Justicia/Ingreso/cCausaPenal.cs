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


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCausaPenal : EntityManagerServer<CAUSA_PENAL>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCausaPenal()
        { }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="centro">id del centro del folio del ingreso</param>
        /// <param name="anio">id del centro del folio del ingreso</param>
        /// <returns>objeto de tipo "CAUSA_PENAL"</returns>
        public CAUSA_PENAL ObtenerPorFolio(short centro , short anio, int imputado, short ingreso, short cpAnio_Folio, int cpFolio  )
        {
            try
            {
                var predicate = PredicateBuilder.True<CAUSA_PENAL>();
                predicate = predicate.And(w => w.ID_CENTRO == centro);
                predicate = predicate.And(w => w.ID_ANIO == anio);
                predicate = predicate.And(w => w.ID_IMPUTADO == imputado);
                predicate = predicate.And(w => w.ID_INGRESO == ingreso);
                predicate = predicate.And(w => w.CP_ANIO == cpAnio_Folio);
                predicate = predicate.And(w => w.CP_FOLIO == cpFolio);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "CAUSA_PENAL"</returns>
        public IQueryable<CAUSA_PENAL> ObtenerTodos()
        {
            try
            {
                return GetData();
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
        /// <returns>objeto de tipo "CAUSA_PENAL"</returns>
        public IQueryable<CAUSA_PENAL> Obtener(short? Centro=null, short? Anio = null, int? Imputado = null, short? Ingreso = null,short? CausaPenal = null,short? CPAnio = null,int? CPFolio = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAUSA_PENAL>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (CausaPenal != null)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == CausaPenal);
                if (CPAnio.HasValue)
                    predicate = predicate.And(w => w.CP_ANIO == CPAnio);
                if (CPFolio.HasValue)
                    predicate = predicate.And(w => w.CP_FOLIO == CPFolio);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtniene el lainformacion juridica delimputado del ultimo ingreso 
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Anio"></param>
        /// <param name="Imputado"></param>
        /// <param name="AnioCP"></param>
        /// <param name="FolioCP"></param>
        /// <param name="NUC"></param>
        /// <returns></returns>
        public CAUSA_PENAL ObtenerCausaPenalProgramaLibertad(short? Centro= null,short? Anio= null,int? Imputado = null,short? AnioCP = null,int? FolioCP = null,string NUC = "") 
        {
            try
            {

                var predicate = PredicateBuilder.True<CAUSA_PENAL>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(AnioCP.HasValue)
                    predicate = predicate.And(w => w.CP_ANIO == AnioCP);
                if(FolioCP.HasValue)
                    predicate = predicate.And(w => w.CP_FOLIO == FolioCP);
                if(!string.IsNullOrEmpty(NUC))
                    predicate = predicate.And(w => w.NUC.ID_NUC.Trim() == NUC);

                return GetData(predicate.Expand()).OrderByDescending(w=> w.ID_INGRESO).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL" con valores a insertar</param>
        public short Insertar(CAUSA_PENAL Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_CAUSA_PENAL = GetIDProceso<short>("CAUSA_PENAL", "ID_CAUSA_PENAL", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND  ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                    #region NUC
                    if (Entity.NUC != null)
                    {
                        if (!string.IsNullOrEmpty(Entity.NUC.ID_NUC))
                        {
                            Entity.NUC.ID_CENTRO = Entity.ID_CENTRO;
                            Entity.NUC.ID_ANIO = Entity.ID_ANIO;
                            Entity.NUC.ID_IMPUTADO = Entity.ID_IMPUTADO;
                            Entity.NUC.ID_INGRESO = Entity.ID_INGRESO;
                            Entity.NUC.ID_CAUSA_PENAL = Entity.ID_CAUSA_PENAL;
                        }
                    }
                    #endregion
                    Context.CAUSA_PENAL.Add(Entity);

                    #region Cambio de Estatus Administrativo
                    if (Entity.NUC == null)//(solo se cambia cuando es tradicional y se selecciona formal prision)
                    {
                        if(Entity.CP_TERMINO != null)
                            if (Entity.CP_TERMINO == 1)//Formal prision
                            {
                                var ingreso = new INGRESO();
                                ingreso.ID_CENTRO = Entity.ID_CENTRO;
                                ingreso.ID_ANIO = Entity.ID_ANIO;
                                ingreso.ID_IMPUTADO = Entity.ID_IMPUTADO;
                                ingreso.ID_INGRESO = Entity.ID_INGRESO;
                                ingreso.ID_CLASIFICACION_JURIDICA = "2";//PROCESOS
                                Context.INGRESO.Attach(ingreso);
                                Context.Entry(ingreso).Property(x => x.ID_CLASIFICACION_JURIDICA).IsModified = true;
                            }
                    }
                    #region comentado
                    //var causasPenales = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO);
                    //if (causasPenales != null)
                    //{
                    //    #region Clasificacion Juridica
                    //    var ingreso = new INGRESO();
                    //    ingreso.ID_CENTRO = Entity.ID_CENTRO;
                    //    ingreso.ID_ANIO = Entity.ID_ANIO;
                    //    ingreso.ID_IMPUTADO = Entity.ID_IMPUTADO;
                    //    ingreso.ID_INGRESO = Entity.ID_INGRESO;

                    //    if (causasPenales.Count(w => w.ID_ESTATUS_CP == 1) > 0)//sentenciado
                    //    {
                    //        ingreso.ID_CLASIFICACION_JURIDICA = "3";//SENTENCIADO
                    //    }
                    //    else
                    //    {
                    //        ingreso.ID_CLASIFICACION_JURIDICA = "2";//PROCESADO
                    //    }
                    //    Context.INGRESO.Attach(ingreso);
                    //    Context.Entry(ingreso).Property(x => x.ID_CLASIFICACION_JURIDICA).IsModified = true;
                    //    #endregion
                    //}
                    #endregion
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_CAUSA_PENAL;
                }
                #region Comentado
                //Entity.ID_CAUSA_PENAL = GetIDProceso<short>("CAUSA_PENAL", "ID_CAUSA_PENAL", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND  ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                //if (Insert(Entity))
                //    return Entity.ID_CAUSA_PENAL;
                //else
                //    return 0;
                #endregion
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
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL" con valores a actualizar</param>
        public bool Actualizar(CAUSA_PENAL Entity,List<CAUSA_PENAL_DELITO> Delito,NUC Nuc)//string NUC)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    #region Delitos
                    var delitos = Context.CAUSA_PENAL_DELITO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL);
                    if (delitos != null)
                    {
                        foreach (var d in delitos)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in Delito)
                    {
                        Context.CAUSA_PENAL_DELITO.Add(d);
                    }
                    #endregion

                    #region NUC
                    var nuc = Context.NUC.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL ).SingleOrDefault();

                    //if (string.IsNullOrEmpty(NUC) && nuc != null)
                    if (Nuc == null && nuc != null)
                        Context.Entry(nuc).State = System.Data.EntityState.Deleted;
                    else
                        //if (!string.IsNullOrEmpty(NUC) && nuc == null)
                        if (Nuc != null && nuc == null)
                            //Context.NUC.Add(new NUC() { ID_CENTRO = Entity.ID_CENTRO, ID_ANIO = Entity.ID_ANIO, ID_IMPUTADO = Entity.ID_IMPUTADO, ID_INGRESO = Entity.ID_INGRESO, ID_CAUSA_PENAL = Entity.ID_CAUSA_PENAL, ID_NUC = NUC });
                            Context.NUC.Add(new NUC() { ID_CENTRO = Entity.ID_CENTRO, ID_ANIO = Entity.ID_ANIO, ID_IMPUTADO = Entity.ID_IMPUTADO, ID_INGRESO = Entity.ID_INGRESO, ID_CAUSA_PENAL = Entity.ID_CAUSA_PENAL, ID_NUC = Nuc.ID_NUC, ID_PERSONA_PG = Nuc.ID_PERSONA_PG });
                        else
                            //if (!string.IsNullOrEmpty(NUC) && nuc != null)
                            if (Nuc != null && nuc != null)
                            {
                                //nuc.ID_NUC = NUC;
                                nuc.ID_NUC = Nuc.ID_NUC;
                                nuc.ID_PERSONA_PG = Nuc.ID_PERSONA_PG;
                                Context.Entry(Entity).State = System.Data.EntityState.Modified;
                            }
                    #endregion

                    #region Cambio de Estatus Administrativo
                    var causasPenales = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO);
                    if (causasPenales != null)
                    {
                        var ingreso = new INGRESO();
                        ingreso.ID_CENTRO = Entity.ID_CENTRO;
                        ingreso.ID_ANIO = Entity.ID_ANIO;
                        ingreso.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        ingreso.ID_INGRESO = Entity.ID_INGRESO;
                        if (causasPenales.Count(w => w.ID_ESTATUS_CP == 1) > 0)//sentenciado
                        {
                            ingreso.ID_CLASIFICACION_JURIDICA = "3";//SENTENCIADO
                        }
                        else
                        {
                            ingreso.ID_CLASIFICACION_JURIDICA = "2";//PROCESADO
                        }
                        Context.INGRESO.Attach(ingreso);
                        Context.Entry(ingreso).Property(x => x.ID_CLASIFICACION_JURIDICA).IsModified = true;
                    }
                    #endregion
                    
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Update(Entity))
                //    return true;
                //else
                //    return false;
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("part of the object's key information"))
                //    throw new ApplicationException("La llave principal no se puede cambiar");
                //else
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool ActualizarEstatusCausaPenal(CAUSA_PENAL Entity)
        {
            try {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.CAUSA_PENAL.Attach(Entity);
                    Context.Entry(Entity).Property(x => x.ID_ESTATUS_CP).IsModified = true;

                    #region Cambio de Estatus Administrativo
                    var causasPenales = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO);
                    if (causasPenales != null)
                    {
                        var ingreso = new INGRESO();
                        ingreso.ID_CENTRO = Entity.ID_CENTRO;
                        ingreso.ID_ANIO = Entity.ID_ANIO;
                        ingreso.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        ingreso.ID_INGRESO = Entity.ID_INGRESO;
                        if (causasPenales.Count(w => w.ID_ESTATUS_CP == 1) > 0)//sentenciado
                        {
                            ingreso.ID_CLASIFICACION_JURIDICA = "3";//SENTENCIADO
                        }
                        else
                        {
                            ingreso.ID_CLASIFICACION_JURIDICA = "2";//PROCESADO
                        }
                        Context.INGRESO.Attach(ingreso);
                        Context.Entry(ingreso).Property(x => x.ID_CLASIFICACION_JURIDICA).IsModified = true;
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
            return false;
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso,int CausaPenal)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
                if (Entity != null)
                {
                    if (Delete(Entity))
                    {
                        if (new cNuc().Eliminar(Centro, Anio, Imputado, Ingreso, CausaPenal))
                            return true;
                        return false;
                    }
                    else
                    return false;
                }
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


        public List<cReporteCausaPenal> ObtenerReporte(int desde = 0,int hasta = 0, int tipo = 1,DateTime? Fec = null)
        {
            try
            {
                
                string query = "SELECT " +
                " CP.id_anio,CP.id_imputado,CP.id_INGRESO,CP.nombre,CP.paterno,CP.materno,nvl(CP.anio_gobierno,0) anio_gobierno,nvl(CP.folio_gobierno,0) folio_gobierno, " +
                " s.FECHA_INICIO,s.S_ANIO,s.S_MES,s.S_DIA,s.C_ANIO,s.C_MES,s.C_DIA,s.A_ANIO,s.A_MES,s.A_DIA,s.PC_ANIO,s.PC_MES,s.PC_DIA " +
                " ,CP.id_ub_centro " +
                " FROM " +
                " SSP.V_CAUSA_PENAL CP," +
                " table(SSP.sentencia_func(CP.id_centro,CP.id_anio,CP.id_imputado,CP.id_ingreso,'" + Fec.Value.ToString("dd-MMM-yyyy").Replace(".",string.Empty) + "')) s WHERE s.FECHA_INICIO IS NOT NULL";

                string filtro = string.Empty;
                
                if (tipo == 1)//Compurgado
                {
                    if(desde > 0)
                        filtro += string.Format(" AND (s.C_ANIO * 365 + s.C_MES * 30 + s.C_DIA) >= {0} ",desde);
                    if(hasta > 0)
                        filtro += string.Format(" AND (s.C_ANIO * 365 + s.C_MES * 30 + s.C_DIA) <= {0} ",hasta);
                }
                else//Por Compurgar
                {
                    if (desde > 0)
                        filtro += string.Format(" AND (s.C_ANIO * 365 + s.C_MES * 30 + s.C_DIA) >= {0} ", desde);
                    if (hasta > 0)
                        filtro += string.Format(" AND (s.C_ANIO * 365 + s.C_MES * 30 + s.C_DIA) <= {0} ", hasta);
                }
                //return GetEjecutaQueries<cReporteCausaPenal>(string.Format("SELECT * FROM  SSP.V_CAUSA_PENAL WHERE FECHA_INICIO IS NOT NULL {0}",filtro)); 
                return GetEjecutaQueries<cReporteCausaPenal>(query + filtro);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }

    public class cReporteCausaPenal
    {
        public int ID_ANIO { set; get; }
        public int ID_IMPUTADO { set; get; }
        public string NOMBRE { set; get; }
        public string PATERNO { set; get; }
        public string MATERNO { set; get; }
        public int ANIO_GOBIERNO { set; get; }
        public string FOLIO_GOBIERNO { set; get; }
        public DateTime? FECHA_INICIO { set; get; }
        public int? S_ANIO { set; get; }
        public int? S_MES { set; get; }
        public int? S_DIA { set; get; }
        public int? C_ANIO { set; get; }
        public int? C_MES { set; get; }
        public int? C_DIA { set; get; }
        public int? A_ANIO { set; get; }
        public int? A_MES { set; get; }
        public int? A_DIA { set; get; }
        public int? PC_ANIO { set; get; }
        public int? PC_MES { set; get; }
        public int? PC_DIA { set; get; }
        public int ID_UB_CENTRO { set; get; }
    }
}