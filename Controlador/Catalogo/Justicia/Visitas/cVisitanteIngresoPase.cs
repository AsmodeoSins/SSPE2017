using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cVisitanteIngresoPase : EntityManagerServer<VISITANTE_INGRESO_PASE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cVisitanteIngresoPase()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros que no han sido autorizados o cancelados
        /// </summary>
        /// <returns>listado de tipo "VISITANTE_INGRESO"</returns>
        public IQueryable<VISITANTE_INGRESO_PASE> ObtenerSinAutorizar()
        {
            try
            {
                return GetData().Where(w => string.IsNullOrEmpty(w.AUTORIZADO));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros que no han sido autorizados o cancelados
        /// </summary>
        /// <returns>listado de tipo "VISITANTE_INGRESO"</returns>
        public IQueryable<VISITANTE_INGRESO_PASE> ObtenerTodos()
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public List<VISITANTE_INGRESO_PASE> Obtener(int Id)
        {
            var Resultado = new List<VISITANTE_INGRESO_PASE>();
            try
            {
                Resultado = GetData().Where(w => w.ID_PERSONA == Id).ToList();
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
        public bool Insertar(VISITANTE_INGRESO_PASE Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_VISITANTE = GetSequence<short>("VISITANTE_SEQ");
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
        public bool Actualizar(VISITANTE_INGRESO_PASE Entity)
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
        public bool Eliminar(short? Centro, short? Anio, int? Imputado, short Ingreso, int? Persona)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_PERSONA == Persona && w.ID_INGRESO == Ingreso && w.ID_IMPUTADO == Imputado &&
                                             w.ID_ANIO == Anio && w.ID_CENTRO == Centro).ToList();
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