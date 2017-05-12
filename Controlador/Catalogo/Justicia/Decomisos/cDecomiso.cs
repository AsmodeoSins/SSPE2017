using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Transactions;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDecomiso : EntityManagerServer<DECOMISO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDecomiso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO> ObtenerTodos()
        {
            try
            {
                return GetData().OrderBy(w => w.ID_DECOMISO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<DECOMISO> ObtenerTodosPorFecha(DateTime FechaInicial, bool ConRango, DateTime? FechaFinal = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO>();

                if (ConRango)
                    predicate = predicate.And(a =>
                        a.EVENTO_FEC.Value >= FechaInicial &&
                        a.EVENTO_FEC.Value <= FechaFinal.Value);
                else
                    predicate = predicate.And(a =>
                        a.EVENTO_FEC.Value.Year == FechaInicial.Year &&
                        a.EVENTO_FEC.Value.Month == FechaInicial.Month &&
                        a.EVENTO_FEC.Value.Day == FechaInicial.Day
                        );
                return GetData(predicate.Expand())
                    .OrderBy(w => w.ID_DECOMISO);
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_DECOMISO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar decomisos 
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO> ObtenerDecomisoEvento(int Tipo, string Nombre, string Paterno, string Materno, string Folio, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO>();
                if (Tipo == 0)
                {
                    if (!string.IsNullOrEmpty(Nombre))
                        predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.NOMBRE.Contains(Nombre)) > 0 || w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre)) > 0);
                    if (!string.IsNullOrEmpty(Paterno))
                        predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.PATERNO.Contains(Paterno)) > 0 || w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.PATERNO.Contains(Paterno)) > 0);
                    if (!string.IsNullOrEmpty(Materno))
                        predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.MATERNO.Contains(Materno)) > 0 || w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.MATERNO.Contains(Materno)) > 0);
                }
                else
                    if (Tipo == 5)
                    {
                        if (!string.IsNullOrEmpty(Nombre))
                            predicate = predicate.And(w => w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre)) > 0);
                        if (!string.IsNullOrEmpty(Paterno))
                            predicate = predicate.And(w => w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.PATERNO.Contains(Paterno)) > 0);
                        if (!string.IsNullOrEmpty(Materno))
                            predicate = predicate.And(w => w.DECOMISO_INGRESO.Count(x => x.INGRESO.IMPUTADO.MATERNO.Contains(Materno)) > 0);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Nombre))
                            predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.NOMBRE.Contains(Nombre) && x.ID_TIPO_PERSONA == Tipo) > 0);
                        if (!string.IsNullOrEmpty(Paterno))
                            predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.PATERNO.Contains(Paterno) && x.ID_TIPO_PERSONA == Tipo) > 0);
                        if (!string.IsNullOrEmpty(Materno))
                            predicate = predicate.And(w => w.DECOMISO_PERSONA.Count(x => x.PERSONA.MATERNO.Contains(Materno) && x.ID_TIPO_PERSONA == Tipo) > 0);
                    }
                return GetData(predicate.Expand()).OrderByDescending(w => w.ID_DECOMISO).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public int Insertar(DECOMISO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_DECOMISO = GetIDProceso<int>("DECOMISO", "ID_DECOMISO", "1 = 1");
                    Context.DECOMISO.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_DECOMISO;
                }
                //Entity.ID_DECOMISO = GetIDProceso<int>("DECOMISO", "ID_DECOMISO", "1 = 1");
                //if (Insert(Entity))
                //    return Entity.ID_DECOMISO;
                //else
                //    return 0;

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
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(DECOMISO Entity, List<DECOMISO_INGRESO> Ingreso, List<DECOMISO_PERSONA> Persona, List<DECOMISO_OBJETO> Objeto)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    #region Ingresos
                    var ingresos = Context.DECOMISO_INGRESO.Where(w => w.ID_DECOMISO == Entity.ID_DECOMISO);
                    if (ingresos != null)
                    {
                        foreach (var i in ingresos)
                        {
                            Context.Entry(i).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var i in Ingreso)
                    {
                        Context.DECOMISO_INGRESO.Add(i);
                    }
                    #endregion

                    #region Ingresos
                    var personas = Context.DECOMISO_PERSONA.Where(w => w.ID_DECOMISO == Entity.ID_DECOMISO);
                    if (personas != null)
                    {
                        foreach (var p in personas)
                        {
                            Context.Entry(p).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var p in Persona)
                    {
                        Context.DECOMISO_PERSONA.Add(p);
                    }
                    #endregion

                    #region Objetos
                    var imagenes = Context.DECOMISO_IMAGEN.Where(w => w.ID_DECOMISO == Entity.ID_DECOMISO);
                    if (imagenes != null)
                    {
                        foreach (var i in imagenes)
                        {
                            Context.Entry(i).State = System.Data.EntityState.Deleted;
                        }
                    }
                    var objetos = Context.DECOMISO_OBJETO.Where(w => w.ID_DECOMISO == Entity.ID_DECOMISO);
                    if (objetos != null)
                    {
                        foreach (var o in objetos)
                        {
                            Context.Entry(o).State = System.Data.EntityState.Deleted;
                        }
                    }
                    Context.SaveChanges();
                    foreach (var o in Objeto)
                    {
                        if (o.DECOMISO_IMAGEN != null)
                        {
                            foreach (var i in o.DECOMISO_IMAGEN)
                            {
                                Context.DECOMISO_IMAGEN.Add(i);
                            }
                        }
                        o.DECOMISO_IMAGEN = null;
                        Context.DECOMISO_OBJETO.Add(o);
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
                }

                //if(new cDecomisoImagen().Eliminar(Entity.ID_DECOMISO))
                //    if (new cDecomisoObjeto().Eliminar(Entity.ID_DECOMISO))
                //        if (Update(Entity))
                //            return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Metodo que actualiza los datos del parte informativo
        /// </summary>
        /// <param name="decomiso">Id</param>
        /// <returns>true o false</returns>
        public bool ActualizarParteInformativo(DECOMISO decomiso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.DECOMISO.Attach(decomiso);
                    Context.Entry(decomiso).Property(x => x.OFICIO_SEGURIDAD).IsModified = true;
                    Context.Entry(decomiso).Property(x => x.JEFE_TURNO).IsModified = true;
                    Context.Entry(decomiso).Property(x => x.COMANDANTE).IsModified = true;
                    Context.Entry(decomiso).Property(x => x.OFICIO_COMAN1).IsModified = true;
                    Context.Entry(decomiso).Property(x => x.OFICIO_COMAN2).IsModified = true;

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
        public bool Eliminar(int Id)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_DECOMISO == Id);
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