using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cImputadoBiometrico : EntityManagerServer<IMPUTADO_BIOMETRICO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cImputadoBiometrico()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "IMPUTADO_BIOMETRICO"</returns>
        public List<IMPUTADO_BIOMETRICO> ObtenerTodos(short anio = 0, short centro = 0, int imputado = 0)
        {
            var Resultado = new List<IMPUTADO_BIOMETRICO>();
            try
            {
                if (anio == 0 || centro == 0 || imputado == 0)
                {
                    //Resultado = GetData().ToList();
                }
                else
                    Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "IMPUTADO_BIOMETRICO"</returns>
        public List<IMPUTADO_BIOMETRICO> Obtener(short anio = 0, short centro = 0, int imputado = 0, short tipo = 0)
        {
            var Resultado = new List<IMPUTADO_BIOMETRICO>();
            try
            {
                if (anio == 0 || centro == 0 || imputado == 0 || tipo == 0)
                {
                    //Resultado = GetData().ToList();
                }
                else
                    Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && w.ID_TIPO_BIOMETRICO == tipo).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public List<IMPUTADO_BIOMETRICO> ObtenerHuellas(short anio = 0, short centro = 0, int imputado = 0)
        {
            var Resultado = new List<IMPUTADO_BIOMETRICO>();
            try
            {
                if (anio == 0 || centro == 0 || imputado == 0)
                {
                    //Resultado = GetData().ToList();
                }
                else
                    Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && 
                                                    w.ID_TIPO_BIOMETRICO >= 11 && w.ID_TIPO_BIOMETRICO <= 20).ToList();
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
        /// <param name="Entity">objeto de tipo "IMPUTADO_BIOMETRICO" con valores a insertar</param>
        public bool Insertar(List<IMPUTADO_BIOMETRICO> Entity)
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

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "IMPUTADO_FILIACION" con valores a actualizar</param>
        public bool Actualizar(List<IMPUTADO_BIOMETRICO> Entity)
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
        /// Actualiza las huellas que se toman en cuenta para la busqueda biometrica por huella
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Anio"></param>
        /// <param name="Imputado"></param>
        /// <param name="ListaToma"></param>
        /// <returns>true/false</returns>
        public bool ActualizarToma(short Centro,short Anio, int Imputado, List<int> ListaToma)
        {
            try
            {
                if (ListaToma.Count == 0)
                    return true;
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    var biometricos = Context.IMPUTADO_BIOMETRICO.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_FORMATO == 9 && (w.ID_TIPO_BIOMETRICO >= 0 && w.ID_TIPO_BIOMETRICO <= 9));
                    if (biometricos != null)
                    {
                        foreach (var b in biometricos)
                        {
                            if (ListaToma.Contains(b.ID_TIPO_BIOMETRICO))
                                b.TOMA = "S";
                            else
                                b.TOMA = "N";
                            Context.Entry(b).State = EntityState.Modified;
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
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(short anio = 0, short centro = 0, int imputado = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
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