using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cIngresoBiometrico : EntityManagerServer<INGRESO_BIOMETRICO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cIngresoBiometrico()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO_BIOMETRICO"</returns>
        public List<INGRESO_BIOMETRICO> ObtenerTodos(short anio = 0, short centro = 0, int imputado = 0)
        {
            var Resultado = new List<INGRESO_BIOMETRICO>();
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
        /// <returns>objeto de tipo "INGRESO_BIOMETRICO"</returns>
        public List<INGRESO_BIOMETRICO> Obtener(short anio = 0, short centro = 0, int imputado = 0, short tipo = 0)
        {
            var Resultado = new List<INGRESO_BIOMETRICO>();
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

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_BIOMETRICO" con valores a insertar</param>
        public bool Insertar(List<INGRESO_BIOMETRICO> Entity)
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


        public bool Insertar(INGRESO_BIOMETRICO Entity)
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
        public bool Actualizar(List<INGRESO_BIOMETRICO> Entity)
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


        public bool Actualizar(INGRESO_BIOMETRICO Entity)
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
        public bool Eliminar(short anio = 0, short centro = 0, int imputado = 0, short ingreso = 0, bool isRegistro = true)
        {
            try
            {
                var Entity = new List<INGRESO_BIOMETRICO>();
                if (isRegistro)
                    Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.ID_TIPO_BIOMETRICO >= 2 && w.ID_TIPO_BIOMETRICO <= 4).ToList();
                else
                    Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso && w.ID_TIPO_BIOMETRICO >= 5 && w.ID_TIPO_BIOMETRICO <= 7).ToList();
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