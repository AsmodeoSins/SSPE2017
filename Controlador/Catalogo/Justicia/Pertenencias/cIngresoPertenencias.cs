using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{

    public class cIngresoPertenencia : EntityManagerServer<INGRESO_PERTENENCIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cIngresoPertenencia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO_PERTENENCIA"</returns>
        public IQueryable<INGRESO_PERTENENCIA> ObtenerTodos()
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
        /// <returns>objeto de tipo "INGRESO_PERTENENCIA"</returns>
        public IQueryable<INGRESO_PERTENENCIA> Obtener(int ID_CENTRO, int ID_ANIO, int ID_IMPUTADO, int ID_INGRESO)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_INGRESO == ID_INGRESO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_PERTENENCIA" con valores a insertar</param>
        public bool Insertar(INGRESO_PERTENENCIA Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public bool Insertar(INGRESO_PERTENENCIA Entity, List<INGRESO_PERTENENCIA_DET> Lst)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Context.INGRESO_PERTENENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO).Any())
                        Context.Entry(Entity).State = EntityState.Modified;
                    else
                        Context.INGRESO_PERTENENCIA.Add(Entity);

                    var objetos = Context.INGRESO_PERTENENCIA_DET.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO);
                    if (objetos != null)
                    {
                        foreach (var obj in objetos)
                        {
                            Context.Entry(obj).State = EntityState.Deleted;    
                        }
                    }
                    if (Lst != null)
                    {
                        foreach (var l in Lst)
                        {
                            Context.INGRESO_PERTENENCIA_DET.Add(l);
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
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_PERTENENCIA" con valores a actualizar</param>
        public bool Actualizar(INGRESO_PERTENENCIA Entity)
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
        public bool Eliminar(int ID_CENTRO, int ID_ANIO, int ID_IMPUTADO, int ID_INGRESO)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_INGRESO == ID_INGRESO);
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