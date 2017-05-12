using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAduanaAcompanante : EntityManagerServer<ADUANA_ACOMPANANTE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAduanaAcompanante()
        { }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ADUANA_INGRESO"</returns>
        public IQueryable<ADUANA_ACOMPANANTE> ObtenerXAduana(int Id)
        {
            var Resultado = new List<ADUANA_ACOMPANANTE>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ID_ADUANA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ADUANA_ACOMPANANTE> ObtenerXPersona(int persona)
        {
            var Resultado = new List<ADUANA_ACOMPANANTE>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ADUANA.ID_PERSONA == persona);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public IQueryable<ADUANA_ACOMPANANTE> ObtenerXAcompananteYPersona(int persona, int aduana)
        {
            var Resultado = new List<ADUANA_ACOMPANANTE>().AsQueryable();
            try
            {
                Resultado = GetData().Where(w => w.ADUANA.ID_PERSONA == persona && w.ID_ADUANA == aduana);
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
        public bool Insertar(ADUANA_ACOMPANANTE Entity)
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
        public bool Insertar(List<ADUANA_ACOMPANANTE> Entity)
        {
            try
            {
                //var resul = true;
                //foreach (var item in Entity)
                //{
                //    resul = Insert(item);
                //}
                return Insert(Entity);
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
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(ADUANA_ACOMPANANTE Entity)
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
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ADUANA == Id).SingleOrDefault();
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