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
    public class cProcesoLibertad : EntityManagerServer<PROCESO_LIBERTAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProcesoLibertad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<PROCESO_LIBERTAD> ObtenerTodos(short? Centro = null, short? Anio = null, int? Imputado = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_LIBERTAD>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
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
        /// <returns>objeto de tipo "FUERO"</returns>
        public PROCESO_LIBERTAD Obtener(short? Centro = null, short? Anio = null, int? Imputado = null,short? Id = null,string NUC = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_LIBERTAD>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(Id.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == Id);
                if(!string.IsNullOrEmpty(NUC))
                    predicate = predicate.And(w => w.NUC.Trim() == NUC);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el ultimo Proceso en libertad para obtener lod datos generales
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Anio"></param>
        /// <param name="Imputado"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PROCESO_LIBERTAD ObtenerProcesoLibertadAnterior(short? Centro = null, short? Anio = null, int? Imputado = null, short? Id = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_LIBERTAD>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Id.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD < Id);
                return GetData(predicate.Expand()).OrderByDescending(w => w.ID_PROCESO_LIBERTAD).FirstOrDefault();
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
        public short Insertar(PROCESO_LIBERTAD Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_PROCESO_LIBERTAD = GetIDProceso<short>("PROCESO_LIBERTAD", "ID_PROCESO_LIBERTAD", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO));
                    Context.PROCESO_LIBERTAD.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_PROCESO_LIBERTAD;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Actualizacion de la informacion general de liberados
        /// </summary>
        /// <param name="ProcesoLibertad"></param>
        /// <param name="Imputado"></param>
        /// <param name="Alias"></param>
        /// <param name="Apodos"></param>
        /// <param name="ImputadoBiometrico"></param>
        /// <returns></returns>
        public bool Actualizar(PROCESO_LIBERTAD ProcesoLibertad, IMPUTADO Imputado, List<ALIAS> Alias, List<APODO> Apodos, List<IMPUTADO_BIOMETRICO> ImputadoBiometrico)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Imputado
                    Context.IMPUTADO.Attach(Imputado);
                    #region Datos Generales
                    Context.Entry(Imputado).Property(x => x.SEXO).IsModified = true;
                    
                    Context.Entry(Imputado).Property(x => x.ID_ETNIA).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.ID_IDIOMA).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.ID_DIALECTO).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.TRADUCTOR).IsModified = true;
                    #endregion

                    #region Nacimiento
                    Context.Entry(Imputado).Property(x => x.NACIMIENTO_PAIS).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NACIMIENTO_ESTADO).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NACIMIENTO_MUNICIPIO).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NACIMIENTO_FECHA).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NACIMIENTO_LUGAR).IsModified = true;
                    #endregion

                    #region Padres
                    var padres = new List<IMPUTADO_PADRES>();
                    Context.Entry(Imputado).Property(x => x.PATERNO_PADRE).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.MATERNO_PADRE).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NOMBRE_PADRE).IsModified = true;

                    Context.Entry(Imputado).Property(x => x.PATERNO_MADRE).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.MATERNO_MADRE).IsModified = true;
                    Context.Entry(Imputado).Property(x => x.NOMBRE_MADRE).IsModified = true;
                    #endregion
                    #endregion

                    #region Alias
                    var listaAlias = Context.Aliases.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (listaAlias != null)
                    { 
                        foreach(var a in listaAlias)
                            Context.Entry(a).State = EntityState.Deleted;
                    }
                    if (Alias != null)
                    {
                        foreach (var a in Alias)
                        {
                            a.ID_CENTRO = Imputado.ID_CENTRO;
                            a.ID_ANIO = Imputado.ID_ANIO;
                            a.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                            Context.Aliases.Add(a);
                        }
                    }
                    #endregion

                    #region Apodos
                    var listaApodos = Context.APODOes.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (listaApodos != null)
                    {
                        foreach (var a in listaApodos)
                            Context.Entry(a).State = EntityState.Deleted;
                    }
                    if (Apodos != null)
                    {
                        foreach (var a in Apodos)
                        {
                            a.ID_CENTRO = Imputado.ID_CENTRO;
                            a.ID_ANIO = Imputado.ID_ANIO;
                            a.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                            Context.APODOes.Add(a);
                        }
                    }
                    #endregion

                    #region Biometrico
                    var listaBiometricos = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (listaBiometricos != null)
                    {
                        foreach (var b in listaBiometricos)
                            Context.Entry(b).State = EntityState.Deleted;
                    }
                    if (ImputadoBiometrico != null)
                    {
                        foreach (var b in ImputadoBiometrico)
                        {
                            b.ID_CENTRO = Imputado.ID_CENTRO;
                            b.ID_ANIO = Imputado.ID_ANIO;
                            b.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                            Context.IMPUTADO_BIOMETRICO.Add(b);
                        }
                    }
                    #endregion

                    #region Proceso en Libertad
                    Context.PROCESO_LIBERTAD.Attach(ProcesoLibertad);

                    Context.Entry(ProcesoLibertad).Property(x => x.ID_ESTADO_CIVIL).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_OCUPACION).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_ESCOLARIDAD).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_RELIGION).IsModified = true;

                    Context.Entry(ProcesoLibertad).Property(x => x.ID_PAIS).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_ENTIDAD).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_MUNICIPIO).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.ID_COLONIA).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.DOMICILIO_CALLE).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.DOMICILIO_NUM_EXT).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.DOMICILIO_NUM_INT).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.TELEFONO).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.DOMICILIO_CODIGO_POSTAL).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.DOMICILIO_TRABAJO).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.RESIDENCIA_ANIOS).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.RESIDENCIA_MESES).IsModified = true;

                    Context.Entry(ProcesoLibertad).Property(x => x.PADRE_FINADO).IsModified = true;
                    Context.Entry(ProcesoLibertad).Property(x => x.MADRE_FINADO).IsModified = true;
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
        /// Actualizar Tipo (sentenciado / procesado)
        /// </summary>
        /// <param name="ProcesoLibertad"></param>
        /// <returns></returns>
        public bool ActualizarEstatus(PROCESO_LIBERTAD ProcesoLibertad)
        { 
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.PROCESO_LIBERTAD.Attach(ProcesoLibertad);
                    Context.Entry(ProcesoLibertad).Property(x => x.TIPO).IsModified = true;
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

        public bool Actualizar(PROCESO_LIBERTAD Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.PROCESO_LIBERTAD.Attach(Entity);
                    Context.Entry(Entity).Property(x => x.NUC).IsModified = true;
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
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(IMPUTADO Imputado, List<IMPUTADO_PADRES> Padres, List<ALIAS> Alias, List<APODO> Apodos, List<IMPUTADO_BIOMETRICO> Biometrico,PROCESO_LIBERTAD ProcesoLibertad,List<PROCESO_LIBERTAD_BIOMETRICO> fotos)
        {
            //obj, padres, alias, apodos, biometrico
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Imputado).State = EntityState.Modified;

                    #region Padres
                    var padres = Context.IMPUTADO_PADRES.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (padres != null)
                    {
                        foreach (var p in padres)
                        {
                            Context.Entry(p).State = EntityState.Deleted;
                        }
                    }
                    foreach (var p in Padres)
                    {
                        p.ID_CENTRO = Imputado.ID_CENTRO;
                        p.ID_ANIO = Imputado.ID_ANIO;
                        p.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        Context.IMPUTADO_PADRES.Add(p);
                    }
                    #endregion

                    #region Alias
                    var alias = Context.Aliases.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (alias != null)
                    {
                        foreach (var a in alias)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in Alias)
                    {
                        a.ID_CENTRO = Imputado.ID_CENTRO;
                        a.ID_ANIO = Imputado.ID_ANIO;
                        a.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        Context.Aliases.Add(a);
                    }
                    #endregion

                    #region Apodos
                    var apodos = Context.APODOes.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (apodos != null)
                    {
                        foreach (var a in apodos)
                        {
                            Context.Entry(a).State = EntityState.Deleted;
                        }
                    }
                    foreach (var a in Apodos)
                    {
                        a.ID_CENTRO = Imputado.ID_CENTRO;
                        a.ID_ANIO = Imputado.ID_ANIO;
                        a.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                        Context.APODOes.Add(a);
                    }
                    #endregion

                    #region Biometrico
                    if (Biometrico.Count > 0)
                    {
                        var biometrico = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                        if (biometrico != null)
                        {
                            foreach (var b in biometrico)
                            {
                                Context.Entry(b).State = EntityState.Deleted;
                            }
                        }
                        foreach (var b in Biometrico)
                        {
                            b.ID_CENTRO = Imputado.ID_CENTRO;
                            b.ID_ANIO = Imputado.ID_ANIO;
                            b.ID_IMPUTADO = Imputado.ID_IMPUTADO;
                            Context.IMPUTADO_BIOMETRICO.Add(b);
                        }
                    }
                    #endregion

                    #region Proceso en libertad
                    var pl = Context.PROCESO_LIBERTAD.Where(w => w.ID_CENTRO == Imputado.ID_CENTRO && w.ID_ANIO == Imputado.ID_ANIO && w.ID_IMPUTADO == Imputado.ID_IMPUTADO);
                    if (pl != null)
                    {
                        if (pl.Count() > 0)
                        {
                            if (ProcesoLibertad.ID_PROCESO_LIBERTAD > 0)
                                Context.Entry(ProcesoLibertad).State = EntityState.Modified;
                            else
                            {
                                ProcesoLibertad.ID_PROCESO_LIBERTAD = GetIDProceso<short>("PROCESO_LIBERTAD", "ID_PROCESO_LIBERTAD", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
                                Context.PROCESO_LIBERTAD.Add(ProcesoLibertad);
                            }
                        }
                        else
                        {
                            ProcesoLibertad.ID_PROCESO_LIBERTAD = GetIDProceso<short>("PROCESO_LIBERTAD", "ID_PROCESO_LIBERTAD", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
                            Context.PROCESO_LIBERTAD.Add(ProcesoLibertad);
                        }
                    }
                    else
                    {
                        ProcesoLibertad.ID_PROCESO_LIBERTAD = GetIDProceso<short>("PROCESO_LIBERTAD", "ID_PROCESO_LIBERTAD", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", Imputado.ID_CENTRO, Imputado.ID_ANIO, Imputado.ID_IMPUTADO));
                        Context.PROCESO_LIBERTAD.Add(ProcesoLibertad);
                    }
                    #endregion

                    #region Fotos
                    var fot = Context.PROCESO_LIBERTAD_BIOMETRICO.Where(w =>
                        w.ID_CENTRO == ProcesoLibertad.ID_CENTRO &&
                        w.ID_ANIO == ProcesoLibertad.ID_ANIO &&
                        w.ID_IMPUTADO == ProcesoLibertad.ID_IMPUTADO &&
                        w.ID_PROCESO_LIBERTAD == ProcesoLibertad.ID_PROCESO_LIBERTAD);
                    if (fot != null)
                    {
                        foreach (var f in fot)
                        {
                            Context.Entry(f).State = EntityState.Deleted;
                        }
                    }
                    if (fotos != null)
                    {
                        foreach (var f in fotos)
                        {
                            Context.Entry(f).State = EntityState.Added;
                        }
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
        public bool Eliminar(PROCESO_LIBERTAD Entity)
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