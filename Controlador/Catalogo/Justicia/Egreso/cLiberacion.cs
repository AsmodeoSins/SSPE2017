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
    public class cLiberacion : EntityManagerServer<LIBERACION>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cLiberacion()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<LIBERACION> ObtenerTodos(string Liberado = "",short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<LIBERACION>();
                if (!string.IsNullOrEmpty(Liberado))
                    predicate = predicate.And(w => w.LIBERADO == Liberado);
                if(Centro.HasValue)
                    predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == Centro);
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
        public IQueryable<LIBERACION> Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? Liberacion = null,bool ConCausaPenal = true)
        {
            try
            {
                var predicate = PredicateBuilder.True<LIBERACION>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (Liberacion != null)
                    predicate = predicate.And(w => w.ID_LIBERACION == Liberacion);
                if(ConCausaPenal)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL != null);
                else
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == null);

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<LIBERACION> ObtenerLiberado(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, short? Liberacion = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<LIBERACION>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (Liberacion != null)
                    predicate = predicate.And(w => w.ID_LIBERACION == Liberacion);
                return GetData(predicate.Expand());
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
        public short Insertar(LIBERACION Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_LIBERACION = GetIDProceso<short>("LIBERACION", "ID_LIBERACION", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                    Context.LIBERACION.Add(Entity);

                    #region Cambiamos el Esatus de la Causa Penal
                    if (Entity.ID_CAUSA_PENAL != null)
                    {
                        var x = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL).FirstOrDefault();
                        if (x != null)
                        {
                            x.ID_ESTATUS_CP = 4;
                            Context.Entry(x).State = EntityState.Modified;
                        }
                    }
                    Context.SaveChanges();
                    #endregion

                    #region Busca Causas Penales Activas
                    int[] activas = {0,1,6};
                    var cp = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && activas.Contains(w.ID_ESTATUS_CP.Value)).FirstOrDefault();
                    if (cp == null)
                    { 
                        //si ya no hay causas penales enviamos el mensaje de notifivacion 
                        var mt = Context.MENSAJE_TIPO.Where(w => w.ID_MEN_TIPO == 104).FirstOrDefault();
                        if (mt != null)
                        {
                            var obj = new MENSAJE();
                            obj.ID_MENSAJE = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            obj.ID_MEN_TIPO = mt.ID_MEN_TIPO;
                            obj.ENCABEZADO = mt.ENCABEZADO;
                            var imputado = Context.IMPUTADO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO).FirstOrDefault();
                            if (imputado != null)
                                obj.CONTENIDO = string.Format("EL INTERNO: {0}/{1} {2} {3} {4}, YA NO TIENE CAUSAS PENALES ACTIVAS.",
                                    Entity.ID_ANIO,
                                    Entity.ID_IMPUTADO,
                                    !string.IsNullOrEmpty(imputado.NOMBRE) ? imputado.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(imputado.PATERNO) ? imputado.PATERNO.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(imputado.MATERNO) ? imputado.MATERNO.Trim() : string.Empty);
                            else
                                obj.CONTENIDO = string.Format("EL INTERNO: {0}/{1}, YA NO TIENE CAUSAS PENALES ACTIVAS.",
                                    Entity.ID_ANIO,
                                    Entity.ID_IMPUTADO);
                            obj.REGISTRO_FEC = GetFechaServerDate();
                            obj.ID_CENTRO = GlobalVariables.gCentro;
                            Context.MENSAJE.Add(obj);
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_LIBERACION;
                    //if (Insert(Entity))
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
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(LIBERACION Entity,INGRESO_BIOMETRICO FotoSalida,bool ActualizaEstatus = false)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;

                    #region Foto Salida
                    if (FotoSalida != null)
                    {
                        //if (Context.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTOGRAFIA_SALIDA && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())//UPDATE
                        if (Context.INGRESO_BIOMETRICO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_TIPO_BIOMETRICO == 101 && w.ID_FORMATO == 5).Any())//UPDATE
                        {
                            Context.Entry(FotoSalida).State = EntityState.Modified;
                        }
                        else//INSERT
                        {
                            Context.INGRESO_BIOMETRICO.Add(FotoSalida);
                        }
                    }
                    #endregion
                    #region Estatus Administrativo
                    if (ActualizaEstatus)
                    {
                        //var ingreso = Context.INGRESO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO).SingleOrDefault();// new INGRESO() { ID_CENTRO = Entity.ID_CENTRO, ID_ANIO = Entity.ID_ANIO, ID_IMPUTADO = Entity.ID_IMPUTADO, ID_INGRESO = Entity.ID_INGRESO, ID_ESTATUS_ADMINISTRATIVO = 4 };//SALIDA
                        //ingreso.ID_ESTATUS_ADMINISTRATIVO = 4;
                        //Context.Entry(ingreso).State = EntityState.Modified;
                        var ingreso = new INGRESO() { ID_CENTRO = Entity.ID_CENTRO, ID_ANIO = Entity.ID_ANIO, ID_IMPUTADO = Entity.ID_IMPUTADO, ID_INGRESO = Entity.ID_INGRESO, ID_ESTATUS_ADMINISTRATIVO = 4};
                        Context.INGRESO.Attach(ingreso);
                        Context.Entry(ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if(Update(Entity))
                //    return true;
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
        public bool Eliminar(LIBERACION Entity)
        {
            try
            {
                if (Delete(Entity))
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