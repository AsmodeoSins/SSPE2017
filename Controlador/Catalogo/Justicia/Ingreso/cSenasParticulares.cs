using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSenasParticulares : EntityManagerServer<SENAS_PARTICULARES>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSenasParticulares()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "SENAS_PARTICULARES"</returns>
        public List<SENAS_PARTICULARES> ObtenerTodos(string buscar = "")
        {
            var Resultado = new List<SENAS_PARTICULARES>();
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    Resultado = GetData().ToList();
                else
                    Resultado = GetData().Where(w => w.SIGNIFICADO.Contains(buscar) || w.CODIGO.Contains(buscar)).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public ObservableCollection<SENAS_PARTICULARES> ObtenerTodosXImputado(int Centro, int Anio, int Imputado)
        {
            try
            {
                var x = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado).ToList();
                return new ObservableCollection<SENAS_PARTICULARES>(x);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<SENAS_PARTICULARES> ObtenerTodos(int Centro, int Anio, int Imputado, int Ingreso)
        {
            try
            {

                return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso);

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
        /// <returns>objeto de tipo "SENAS_PARTICULARES"</returns>
        public IQueryable<SENAS_PARTICULARES> Obtener(short centro, short anio, int imputado)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        private short ObtenerSecuenciaSenia(short anio, int imputado, short centro)
        {
            short Resultado;
            try
            {
                var ingres = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado).OrderByDescending(m => m.ID_SENA).FirstOrDefault();
                if (ingres != null)
                    Resultado = ingres.ID_SENA;
                else
                    Resultado = 0;
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
        /// <param name="Entity">objeto de tipo "SENAS_PARTICULARES" con valores a insertar</param>
        public bool Insertar(SENAS_PARTICULARES Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_SENA = GetIDProceso<short>("SENAS_PARTICULARES", "ID_SENA", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                    Context.SENAS_PARTICULARES.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //Entity.ID_SENA = GetIDProceso<short>("SENAS_PARTICULARES", "ID_SENA", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                //return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool Insertar(short Centro, short Anio, int Imputado, List<SENAS_PARTICULARES> list)
        {
            try
            {
                Eliminar(Centro, Anio, Imputado);
                int i = 0;
                foreach (var item in list)
                {
                    i = i + 1;
                    item.ID_SENA = (short)(i);
                }
                return Insert(list);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ANATOMIA_TOPOGRAFICA" con valores a actualizar</param>
        public void Actualizar(SENAS_PARTICULARES Entity)
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

        public bool ActualizarSP(SENAS_PARTICULARES Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Update(Entity))
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
        public bool Eliminar(short Centro, short Anio, int Imputado)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado);//.SingleOrDefault();
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