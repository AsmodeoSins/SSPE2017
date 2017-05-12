using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonaNIP : EntityManagerServer<PERSONA_NIP>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPersonaNIP()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PERSONA_EXTERNO"</returns>
        public ObservableCollection<PERSONA_NIP> ObtenerTodos(short centro = 0, int persona = 0)
        {
            ObservableCollection<PERSONA_NIP> personaExterno;
            var Resultado = new List<PERSONA_NIP>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona).ToList();
                personaExterno = new ObservableCollection<PERSONA_NIP>(Resultado);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return personaExterno;
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(PERSONA_NIP Entity)
        {
            try
            {
                //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                //Entity.ID_AREA = GetSequence<short>("AREA_VISITA_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public bool Insertar(List<PERSONA_NIP> Entity, short centro = 0, int persona = 0)
        {
            try
            {
                if (Eliminar(centro, persona))
                    return Insert(Entity);
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
        /// <param name="Entity">objeto de tipo "PERSONA_EXTERNO" con valores a actualizar</param>
        public bool Actualizar(PERSONA_NIP Entity)
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
        public bool Eliminar(short centro = 0, int persona = 0)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == centro && w.ID_PERSONA == persona).ToList();
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