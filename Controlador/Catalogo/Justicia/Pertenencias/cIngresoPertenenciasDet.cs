using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{

    public class cIngresoPertenenciasDet : EntityManagerServer<INGRESO_PERTENENCIA_DET>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cIngresoPertenenciasDet()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO_PERTENENCIA_DET"</returns>
        public IQueryable<INGRESO_PERTENENCIA_DET> ObtenerTodos()
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
        /// <returns>objeto de tipo "INGRESO_PERTENENCIA_DET"</returns>
        public IQueryable<INGRESO_PERTENENCIA_DET> Obtener(int ID_CENTRO, int ID_ANIO, int ID_IMPUTADO, int ID_INGRESO, int ID_CONSEC)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_INGRESO == ID_INGRESO && w.ID_CONSEC == ID_CONSEC);
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
        /// <returns>objeto de tipo "INGRESO_PERTENENCIA_DET"</returns>
        public short? ObtenerConsecutivo(int ID_CENTRO, int ID_ANIO, int ID_IMPUTADO, int ID_INGRESO)
        {
            try
            {
                var query = GetData().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_INGRESO == ID_INGRESO);

                if (query.Any())
                    return (short?)query.Max(m => m.ID_CONSEC);
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_PERTENENCIA_DET" con valores a insertar</param>
        public bool Insertar(INGRESO_PERTENENCIA_DET Entity)
        {
            try
            {
                var consecutivo = ObtenerConsecutivo(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                Entity.ID_CONSEC = consecutivo.HasValue ? (short)(consecutivo.Value + 1) : (short)1;


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

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_PERTENENCIA_DET" con valores a actualizar</param>
        public void Actualizar(INGRESO_PERTENENCIA_DET Entity)
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

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int ID_CENTRO, int ID_ANIO, int ID_IMPUTADO, int ID_INGRESO, int ID_CONSEC)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == ID_CENTRO && w.ID_ANIO == ID_ANIO && w.ID_IMPUTADO == ID_IMPUTADO && w.ID_INGRESO == ID_INGRESO && w.ID_CONSEC == ID_CONSEC);
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