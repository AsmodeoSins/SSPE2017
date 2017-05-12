using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using System.Transactions;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAgendaActividadLibertad : EntityManagerServer<AGENDA_ACTIVIDAD_LIBERTAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAgendaActividadLibertad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<AGENDA_ACTIVIDAD_LIBERTAD> ObtenerTodos(short? Centro = null,short? Anio = null, int? Imputado = null, short? PL = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<AGENDA_ACTIVIDAD_LIBERTAD>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (PL.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == PL);
                return GetData(predicate.Expand()).OrderBy(w => w.ID_PROCESO_LIBERTAD);
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
        public AGENDA_ACTIVIDAD_LIBERTAD Obtener(int Id)
        {
            try
            {
                return GetData().FirstOrDefault(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD == Id);
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
        public decimal Insertar(AGENDA_ACTIVIDAD_LIBERTAD Entity)
        {
            try
            {
                Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD = GetIDProceso<short>("AGENDA_ACTIVIDAD_LIBERTAD", "ID_AGENDA_ACTIVIDAD_LIBERTAD", "1=1");
                if(Insert(Entity))
                    return Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                return 0;
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
        public bool Actualizar(AGENDA_ACTIVIDAD_LIBERTAD Entity, List<AGENDA_ACT_LIB_DETALLE> Fechas,List<AGENDA_LIBERTAD_DOCUMENTO> LstOficios)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entity).State = System.Data.EntityState.Modified;

                    #region Fechas
                    var asistencias = Context.AGENDA_ACT_LIB_DETALLE.Where(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD == Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD && w.ASISTENCIA == 1);
                    if (asistencias != null)
                    {
                        foreach (var a in asistencias)
                        {
                            var x = Fechas.FirstOrDefault(w => w.FECHA.Value.Date == a.FECHA.Value.Date);
                            if (x != null)
                            {
                                x.ASISTENCIA = 1;
                            }
                            else
                            {
                                Fechas.Add(new AGENDA_ACT_LIB_DETALLE()
                                {
                                    ID_AGENDA_ACTIVIDAD_LIBERTAD = Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD,
                                    ID_DETALLE = 0,
                                    FECHA = a.FECHA,
                                    HORA_INICIO = a.HORA_INICIO,
                                    HORA_FIN = a.HORA_FIN,
                                    ASISTENCIA = a.ASISTENCIA,
                                    OBSERVACION = a.OBSERVACION
                                });
                            }
                        }
                    }
                    
                    var fec = Context.AGENDA_ACT_LIB_DETALLE.Where(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD == Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                    if(fec != null)
                        foreach (var f in fec)
                        {
                            Context.Entry(f).State = System.Data.EntityState.Deleted;
                        }
                    if (Fechas != null)
                    {
                        int i = 1;
                        foreach (var f in Fechas.OrderBy(w => w.FECHA))
                        {
                            f.ID_AGENDA_ACTIVIDAD_LIBERTAD = Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                            f.ID_DETALLE = i;
                            Context.Entry(f).State = System.Data.EntityState.Added;
                            i++;
                        }
                    }
                    #endregion

                    #region Oficios
                    var oficios = Context.AGENDA_LIBERTAD_DOCUMENTO.Where(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD == Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD);
                    if (oficios != null)
                    {
                        foreach (var o in oficios)
                        {
                            Context.Entry(o).State = System.Data.EntityState.Deleted;
                        }
                    }
                    if (LstOficios != null)
                    {
                        short i = 1;
                        foreach (var l in LstOficios)
                        {
                            l.ID_AGENDA_ACTIVIDAD_LIBERTAD = Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                            l.ID_DOCUMENTO = i;
                            Context.Entry(l).State = System.Data.EntityState.Added;
                            i++;
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
        public bool Eliminar(AGENDA_ACTIVIDAD_LIBERTAD Entity)
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