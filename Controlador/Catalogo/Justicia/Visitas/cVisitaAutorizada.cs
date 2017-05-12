using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitaAutorizada : EntityManagerServer<VISITA_AUTORIZADA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitaAutorizada()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VISITA_AUTORIZADA"</returns>
        public ObservableCollection<VISITA_AUTORIZADA> ObtenerTodos(string nombre = "", string paterno = "", string materno = "")
        {
            ObservableCollection<VISITA_AUTORIZADA> VisitaAutorizada;
            var Resultado = new List<VISITA_AUTORIZADA>();
            try
            {
                if (!string.IsNullOrEmpty(nombre) || !string.IsNullOrEmpty(paterno) || !string.IsNullOrEmpty(materno))
                    Resultado = GetData().Where(w => w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).ToList();
                VisitaAutorizada = new ObservableCollection<VISITA_AUTORIZADA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaAutorizada;
        }
        public ObservableCollection<VISITA_AUTORIZADA> ObtenerXIngreso(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            ObservableCollection<VISITA_AUTORIZADA> VisitaAutorizada;
            var Resultado = new List<VISITA_AUTORIZADA>();
            try
            {
                if (centro > 0 && anio > 0 && imputado > 0 && ingreso > 0)
                    Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
                //Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
                VisitaAutorizada = new ObservableCollection<VISITA_AUTORIZADA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaAutorizada;
        }
        public ObservableCollection<VISITA_AUTORIZADA> ObtenerXIngresoYNoCredencializados(short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            ObservableCollection<VISITA_AUTORIZADA> VisitaAutorizada;
            var Resultado = new List<VISITA_AUTORIZADA>();
            try
            {
                if (centro > 0 && anio > 0 && imputado > 0 && ingreso > 0)
                    Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso //&& w.ESTATUS == 0 
                        && w.ID_PERSONA == null).ToList();
                //Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
                VisitaAutorizada = new ObservableCollection<VISITA_AUTORIZADA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaAutorizada;
        }
        public ObservableCollection<VISITA_AUTORIZADA> ObtenerBusquedaXIngreso(string nombre = "", string paterno = "", string materno = "", short centro = 0, short anio = 0, int imputado = 0, short ingreso = 0)
        {
            ObservableCollection<VISITA_AUTORIZADA> VisitaAutorizada;
            var Resultado = new List<VISITA_AUTORIZADA>();
            try
            {
                if (centro > 0 && anio > 0 && imputado > 0 && ingreso > 0)
                    Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.NOMBRE.Contains(nombre)
                                                    && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).ToList();
                //Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.NOMBRE.Contains(nombre) && w.PATERNO.Contains(paterno) && w.MATERNO.Contains(materno)).ToList();
                VisitaAutorizada = new ObservableCollection<VISITA_AUTORIZADA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaAutorizada;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<VISITA_AUTORIZADA> Obtener(int Id)
        {
            var Resultado = new List<VISITA_AUTORIZADA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_VISITA == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(VISITA_AUTORIZADA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_VISITA = GetIDProceso<short>("VISITA_AUTORIZADA", "ID_VISITA", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}",
                        Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                    Context.VISITA_AUTORIZADA.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
                }
                //Entity.ID_VISITA = GetSequence<short>("VISITA_AUTORIZADA_SEQ");
                //return Insert(Entity);
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
        public bool Actualizar(VISITA_AUTORIZADA Entity)
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
                var Entity = GetData().Where(w => w.ID_VISITA == Id).SingleOrDefault();
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