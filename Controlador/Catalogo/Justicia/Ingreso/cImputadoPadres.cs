using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cImputadoPadres : EntityManagerServer<IMPUTADO_PADRES>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cImputadoPadres()
        { }

        ///// <summary>
        ///// metodo que conecta con la base de datos para extraer todos los registros
        ///// </summary>
        ///// <returns>listado de tipo "IMPUTADO_PADRES"</returns>
        //public List<IMPUTADO_PANDILLA> ObtenerTodos(short anio = 0, short centro = 0, int imputado = 0)
        //{
        //    var Resultado = new List<IMPUTADO_PADRES>();
        //    try
        //    {
        //        Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }
        //    return Resultado;
        //}

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "IMPUTADO_FILIACION"</returns>
        public ObservableCollection<IMPUTADO_PADRES> Obtener(short anio = 0, short centro = 0, int imputado = 0)
        {
            ObservableCollection<IMPUTADO_PADRES> Resultado;
            try
            {
                Resultado = new ObservableCollection<IMPUTADO_PADRES>(GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado));
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
        /// <param name="Entity">objeto de tipo "IMPUTADO_FILIACION" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, List<IMPUTADO_PADRES> Entity)
        {
            try
            {
                Eliminar(Anio, Centro, Imputado);
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
        public bool Actualizar(List<IMPUTADO_PADRES> Entity)
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
        public bool Eliminar(short anio = 0, short centro = 0, int imputado = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
                if (Entity != null)
                {
                    if (Entity.Count != 0)
                        return Delete(Entity);
                    return true;
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
    }
}