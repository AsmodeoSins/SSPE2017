using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCorrespondencia : EntityManagerServer<CORRESPONDENCIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCorrespondencia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "CORRESPONDENCIA"</returns>
        public ObservableCollection<CORRESPONDENCIA> ObtenerTodos()
        {
            ObservableCollection<CORRESPONDENCIA> Correspondencias;
            var Resultado = new List<CORRESPONDENCIA>();
            try
            {
                Resultado = GetData().Where(w => w.CONFIRMACION_RECIBIDO != "S").ToList();
                Correspondencias = new ObservableCollection<CORRESPONDENCIA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Correspondencias;
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros con confirmación de recibido
        /// </summary>
        /// <returns>listado de tipo "CORRESPONDENCIA"</returns>
        public IQueryable<CORRESPONDENCIA> ObtenerTodosRecibidos(DateTime Fecha)
        {
            try
            {
                return GetData().Where(w =>
                    w.RECEPCION_FEC.HasValue &&
                    w.RECEPCION_FEC.Value.Year == Fecha.Year &&
                    w.RECEPCION_FEC.Value.Month == Fecha.Month &&
                    w.RECEPCION_FEC.Value.Day == Fecha.Day &&
                    w.INGRESO.CAMA != null);
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<CORRESPONDENCIA> Obtener(int Id_anio, int id_centro, int id_imputado, int id_ingreso)
        {
            var Resultado = new List<CORRESPONDENCIA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ANIO == Id_anio && w.ID_CENTRO == id_centro && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(CORRESPONDENCIA Entity)
        {
            try
            {
                var consecutivo = GetData().Where(w => w.ID_ANIO == Entity.ID_ANIO && w.ID_CENTRO == Entity.ID_CENTRO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO);
                Entity.ID_CONSEC = consecutivo.ToList().Count > 0 ? (short)((consecutivo.Max(m => m.ID_CONSEC)) + 1) : (short)0;
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(CORRESPONDENCIA Entity)
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
        public bool Eliminar(int Id_anio, int id_centro, int id_imputado, int id_ingreso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ANIO == Id_anio && w.ID_CENTRO == id_centro && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso).SingleOrDefault();
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